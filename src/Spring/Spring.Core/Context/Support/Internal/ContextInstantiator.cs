using Spring.Reflection.Dynamic;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Spring.Context.Support
{
    #region Inner Class : ContextInstantiator
    internal abstract class ContextInstantiator
    {
        protected ContextInstantiator(
            Type contextType, string contextName, bool caseSensitive, string[] resources)
        {
            _contextType = contextType;
            _contextName = contextName;
            _caseSensitive = caseSensitive;
            _resources = resources;
        }

        public IApplicationContext InstantiateContext()
        {
            ConstructorInfo ctor = GetContextConstructor();
            if (ctor == null)
            {
                string errorMessage = "No constructor with string[] argument found for context type [" + ContextType.Name + "]";
                throw ConfigurationUtils.CreateConfigurationException(errorMessage);
            }
            IApplicationContext context = InvokeContextConstructor(ctor);
            return context;
        }

        protected abstract ConstructorInfo GetContextConstructor();

        protected abstract IApplicationContext InvokeContextConstructor(
            ConstructorInfo ctor);

        protected Type ContextType
        {
            get { return _contextType; }
        }

        protected string ContextName
        {
            get { return _contextName; }
        }

        protected bool CaseSensitive
        {
            get { return _caseSensitive; }
        }

        protected IList<string> Resources
        {
            get { return _resources; }
        }

        private Type _contextType;
        private string _contextName;
        private bool _caseSensitive;
        private IList<string> _resources;
    }
    #endregion


    #region Inner Class : RootContextInstantiator

    internal sealed class RootContextInstantiator : ContextInstantiator
    {
        public RootContextInstantiator(
            Type contextType, string contextName, bool caseSensitive, string[] resources)
            : base(contextType, contextName, caseSensitive, resources)
        {
        }

        protected override ConstructorInfo GetContextConstructor()
        {
            return ContextType.GetConstructor(new Type[] { typeof(string), typeof(bool), typeof(string[]) });
        }

        protected override IApplicationContext InvokeContextConstructor(
            ConstructorInfo ctor)
        {
            return (IApplicationContext)(new SafeConstructor(ctor).Invoke(new object[] { ContextName, CaseSensitive, Resources }));
        }
    }

    #endregion

    #region Inner Class : DescendantContextInstantiator

    internal sealed class DescendantContextInstantiator : ContextInstantiator
    {
        public DescendantContextInstantiator(
            IApplicationContext parentContext, Type contextType,
            string contextName, bool caseSensitive, string[] resources)
            : base(contextType, contextName, caseSensitive, resources)
        {
            this.parentContext = parentContext;
        }

        protected override ConstructorInfo GetContextConstructor()
        {
            return ContextType.GetConstructor(
                new Type[] { typeof(string), typeof(bool), typeof(IApplicationContext), typeof(string[]) });
        }

        protected override IApplicationContext InvokeContextConstructor(
            ConstructorInfo ctor)
        {
            return (IApplicationContext)(new SafeConstructor(ctor).Invoke(new object[] { ContextName, CaseSensitive, this.parentContext, Resources }));
        }

        private IApplicationContext parentContext;
    }

    #endregion
}
