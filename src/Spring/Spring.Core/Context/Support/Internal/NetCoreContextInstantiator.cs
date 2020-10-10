using Microsoft.Extensions.Configuration;
using Spring.Objects.Factory.Config;
using Spring.Reflection.Dynamic;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Spring.Context.Support
{
    
    /// <summary>
    /// 
    /// </summary>
    internal sealed class NetCoreContextInstantiator : ContextInstantiator
    {
        private readonly IConfiguration netCoreConfigObject;
        private readonly string netCoreConfigObjectName = "NetCoreConfig";
        private readonly IApplicationContext parentContext;
        public NetCoreContextInstantiator(
            IApplicationContext parentContext,
            Type contextType, string contextName, bool caseSensitive, string[] resources, IConfiguration netCoreConfigObject, string netCoreConfigObjectName = "NetCoreConfig")
            : base(contextType, contextName, caseSensitive, resources)
        {
            AssertUtils.ArgumentNotNull(netCoreConfigObject, "netCoreConfigObject");
            AssertUtils.ArgumentHasText(netCoreConfigObjectName, "netCoreConfigObjectName");

            this.netCoreConfigObject = netCoreConfigObject;
            this.parentContext = parentContext;
            this.netCoreConfigObjectName = netCoreConfigObjectName;
        }

        protected override ConstructorInfo GetContextConstructor()
        {
            return ContextType.GetConstructor(new Type[] { typeof(bool), typeof(string), typeof(bool), typeof(IApplicationContext), typeof(string[]) });
        }

        protected override IApplicationContext InvokeContextConstructor(
            ConstructorInfo ctor)
        {
            IConfigurableApplicationContext ctx = new SafeConstructor(ctor).Invoke(new object[] { false, ContextName, CaseSensitive, parentContext, Resources }) as IConfigurableApplicationContext;
            //Only Root Context Register the NetCore Config Object :IConfiguration
            if (parentContext == null)
            {
                ctx.AddObjectFactoryPostProcessor(new DelegateObjectFactoryConfigurer(of => of.RegisterSingleton(this.netCoreConfigObjectName, this.netCoreConfigObject)));
            }
            ctx.Refresh();

            return ctx;
        }
    }
}
