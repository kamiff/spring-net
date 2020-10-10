using Common.Logging;
using Microsoft.Extensions.Configuration;

using Spring.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Spring.Core.TypeResolution;
using Spring.Core;

namespace Spring.Context.Support
{
    /// <summary>
	/// Creates an <see cref="Spring.Context.IApplicationContext"/> instance
	/// using context definitions supplied in a custom configuration and
	/// configures the <see cref="ContextRegistry"/> with that instance.
	/// </summary>
	/// <remarks>
	/// Implementations of the <see cref="Spring.Context.IApplicationContext"/>
	/// interface <b>must</b> provide the following two constructors:
	/// <list type="number">
	/// <item>
	/// <description>
	/// A constructor that takes a string array of resource locations.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// A constructor that takes a reference to a parent application context
	/// and a string array of resource locations (and in that order).
	/// </description>
	/// </item>
	/// </list>
	/// <p>
	/// Note that if the <c>type</c> attribute is not present in the declaration
	/// of a particular context, then a default
	/// <see cref="Spring.Context.IApplicationContext"/> <see cref="System.Type"/>
	/// is assumed. This default
	/// <see cref="Spring.Context.IApplicationContext"/> <see cref="System.Type"/>
	/// is currently the <see cref="Spring.Context.Support.XmlApplicationContext"/>
	/// <see cref="System.Type"/>; please note the exact <see cref="System.Type"/>
	/// of this default <see cref="Spring.Context.IApplicationContext"/> is an
	/// implementation detail, that, while unlikely, may do so in the future.
	/// to
	/// </p>
	/// </remarks>
	/// <example>
	/// <![CDATA[
    /// {
    /// "springContext":{
    ///  "type":"Spring.Context.Support.XmlApplicationContext, Spring.Core", /*Optional*/
    ///  "name":"parent",/*Optional*/
    ///  "caseSensitive" : true, /*Optional*/
    ///  "childContexts":[],
    ///  "resource":[
    ///   {"uri":"assembly://MyAssemblyName/MyResourceNamespace/MyObjects.xml"},
    ///   {"uri":"assembly://MyAssemblyName/MyResourceNamespace/MyObjects.xml"},
    ///  ],
    /// }
    ///}
    /// ]]>
	/// </example>
	/// <author>Mark Pollack</author>
	/// <author>Aleksandar Seovic</author>
	/// <author>Rick Evans</author>
    /// <author>kamiff lee</author>
	/// <seealso cref="ContextRegistry"/>
    public class ContextCreater
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContextCreater));
        /// <summary>
        /// 
        /// </summary>
        private const string CHILD_TAG = "$CHILD_CONTEXT$";
        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration netCoreConfigObject;
        /// <summary>
        /// 
        /// </summary>
        private readonly string netCoreConfigObjectName = "NetCoreConfig";
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="Spring.Context.IApplicationContext"/>
        /// created if no <c>type</c> attribute is specified on a <c>context</c> element.
        /// </summary>
        /// <seealso cref="GetContextType"/>
        protected virtual Type DefaultApplicationContextType
        {
            get { return typeof(XmlApplicationContext); }
        }

        /// <summary>
        /// Get the context's case-sensitivity to use if none is specified
        /// </summary>
        /// <remarks>
        /// <p>
        /// Derived handlers may override this property to change their default case-sensitivity.
        /// </p>
        /// <p>
        /// Defaults to 'true'.
        /// </p>
        /// </remarks>
        protected virtual bool DefaultCaseSensitivity
        {
            get { return true; }
        }

        /// <summary>
        /// Specifies, whether the instantiated context will be automatically registered in the 
        /// global <see cref="ContextRegistry"/>.
        /// </summary>
        protected virtual bool AutoRegisterWithContextRegistry
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="netCoreConfigObject"></param>
        /// <param name="netCoreConfigObjectName"></param>
        public ContextCreater(IConfiguration netCoreConfigObject, string netCoreConfigObjectName = "NetCoreConfig")
        {
            AssertUtils.ArgumentNotNull(netCoreConfigObject, "netCoreConfigObject");
            AssertUtils.ArgumentHasText(netCoreConfigObjectName, "netCoreConfigObjectName");

            this.netCoreConfigObject = netCoreConfigObject;
            this.netCoreConfigObjectName = netCoreConfigObjectName;
        }

        /// <summary>
        /// Creates an <see cref="Spring.Context.IApplicationContext"/> instance
        /// using the context definitions supplied in a custom
        /// configuration section.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This <see cref="Spring.Context.IApplicationContext"/> instance is
        /// also used to configure the <see cref="ContextRegistry"/>.
        /// </p>
        /// </remarks>
        /// <param name="section">
        /// The <see cref="System.Xml.XmlNode"/> for the section.
        /// </param>
        /// <returns>
        /// An <see cref="Spring.Context.IApplicationContext"/> instance
        /// populated with the object definitions supplied in the configuration
        /// section.
        /// </returns>
        public IApplicationContext Create(string contextElement = "springContext")
        {
            return Create(null, this.netCoreConfigObject, contextElement);
        }

        /// <summary>
        /// Creates an <see cref="Spring.Context.IApplicationContext"/> instance
        /// using the context definitions supplied in a custom
        /// configuration section.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This <see cref="Spring.Context.IApplicationContext"/> instance is
        /// also used to configure the <see cref="ContextRegistry"/>.
        /// </p>
        /// </remarks>
        /// <param name="parent">
        /// The configuration settings in a corresponding parent
        /// configuration section.
        /// </param>
        /// <param name="netCoreConfigObject">
        /// The configuration context when called from the ASP.NET
        /// configuration system. Otherwise, this parameter is reserved and
        /// is <see langword="null"/>.
        /// </param>
        /// <param name="section">
        /// The <see cref="System.Xml.XmlNode"/> for the section.
        /// </param>
        /// <returns>
        /// An <see cref="Spring.Context.IApplicationContext"/> instance
        /// populated with the object definitions supplied in the configuration
        /// section.
        /// </returns>
        protected IApplicationContext Create(IApplicationContext parent, IConfiguration netCoreConfigObject, string contextElement = "springContext")
        {
            
            #region Sanity Checks
            if (StringUtils.IsNullOrEmpty(contextElement))
            {
                throw ConfigurationUtils.CreateConfigurationException(
                    "Context configuration section could not be empty.");
            }
            IConfigurationSection section = contextElement.Equals(CHILD_TAG, StringComparison.OrdinalIgnoreCase) ? netCoreConfigObject as IConfigurationSection : netCoreConfigObject.GetSection(contextElement);

            if (section == null || !section.Exists())
            {
                throw ConfigurationUtils.CreateConfigurationException(
                    "Context configuration section could must be an json node.");
            }
            #endregion

            // determine name of context to be created
            string contextName = GetContextName(section);
            if (!StringUtils.HasLength(contextName))
            {
                contextName = AbstractApplicationContext.DefaultRootContextName;
            }

            #region Instrumentation
            if (Log.IsDebugEnabled) Log.Debug(string.Format("creating context '{0}'", contextName));
            #endregion

            IApplicationContext context = null;
            try
            {
                IApplicationContext parentContext = parent as IApplicationContext;

                // determine context type
                Type contextType = GetContextType(section, parentContext);

                // determine case-sensitivity
                bool caseSensitive = GetCaseSensitivity(section);

                // get resource-list
                IList<string> resources = GetResources(section);

                // finally create the context instance
                context = InstantiateContext(parentContext, contextName, contextType, caseSensitive, resources);
                // and register with global context registry
                if (AutoRegisterWithContextRegistry && !ContextRegistry.IsContextRegistered(context.Name))
                {
                    ContextRegistry.RegisterContext(context);
                }

                // get and create child context definitions
                IList<IConfigurationSection> childContexts = GetChildContexts(section);
                CreateChildContexts(context, childContexts);

                if (Log.IsDebugEnabled) Log.Debug(string.Format("context '{0}' created for name '{1}'", context, contextName));
            }
            catch (Exception ex)
            {
                if (!ConfigurationUtils.IsConfigurationException(ex))
                {
                    throw ConfigurationUtils.CreateConfigurationException(
                        String.Format("Error creating context '{0}': {1}",
                        contextName, ReflectionUtils.GetExplicitBaseException(ex).Message), ex);
                }
                throw;
            }
            return context;
        }

        /// <summary>
        /// Create all child-contexts in the given <see cref="XmlNodeList"/> for the given context.
        /// </summary>
        /// <param name="parentContext">The parent context to use</param>
        /// <param name="childContexts">The list of child context elements</param>
        protected virtual void CreateChildContexts(IApplicationContext parentContext, IList<IConfigurationSection> childContexts)
        {
            // create child contexts for 'the most recently created context'...
            foreach (IConfigurationSection childContext in childContexts)
            {
                this.Create(parentContext, childContext, string.Empty);
            }
        }

        /// <summary>
        /// Instantiates a new context.
        /// </summary>
        protected virtual IApplicationContext InstantiateContext(IApplicationContext parentContext, string contextName, Type contextType, bool caseSensitive, IList<string> resources)
        {
            IApplicationContext context;
            ContextInstantiator instantiator = new NetCoreContextInstantiator(parentContext, contextType, contextName, caseSensitive, resources.ToArray(), this.netCoreConfigObject, this.netCoreConfigObjectName);

            if (IsLazy)
            {
                // TODO
            }
            context = instantiator.InstantiateContext();
            return context;
        }

        /// <summary>
        /// Gets the context's name specified in the name attribute of the context element.
        /// </summary>
        /// <param name="configContext">The current configContext <see cref="IConfigurationSectionHandler.Create"/></param>
        /// <param name="contextElement">The context element</param>
        protected virtual string GetContextName(IConfigurationSection section)
        {
            string contextName = section.GetValue<string>(ContextSchema.NameAttribute);
            return contextName;
        }

        /// <summary>
        /// Extracts the context-type from the context element. 
        /// If none is specified, returns the parent's type.
        /// </summary>
        private Type GetContextType(IConfigurationSection section, IApplicationContext parentContext)
        {
            Type contextType;
            if (parentContext != null)
            {
                // set default context type to parent's type (allows for type inheritance)
                contextType = GetConfiguredContextType(section, parentContext.GetType());
            }
            else
            {
                contextType = GetConfiguredContextType(section, this.DefaultApplicationContextType);
            }
            return contextType;
        }

        /// <summary>
        /// Extracts the case-sensitivity attribute from the context element
        /// </summary>
        private bool GetCaseSensitivity(IConfigurationSection section)
        {
            bool caseSensitive = DefaultCaseSensitivity;

            string caseSensitiveAttr = section.GetValue<string>(ContextSchema.CaseSensitiveAttribute);
            if (StringUtils.HasText(caseSensitiveAttr))
            {
                caseSensitive = Boolean.Parse(caseSensitiveAttr);
            }
            return caseSensitive;
        }

        /// <summary>
        /// Gets the context <see cref="System.Type"/> specified in the type
        /// attribute of the context element.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If this attribute is not defined it defaults to the
        /// <see cref="Spring.Context.Support.XmlApplicationContext"/> type.
        /// </p>
        /// </remarks>
        /// <exception cref="TypeMismatchException">
        /// If the context type does not implement the
        /// <see cref="Spring.Context.IApplicationContext"/> interface.
        /// </exception>
        private Type GetConfiguredContextType(IConfigurationSection section, Type defaultContextType)
        {
            string typeName = section.GetValue<string>(ContextSchema.TypeAttribute);

            if (StringUtils.IsNullOrEmpty(typeName))
            {
                return defaultContextType;
            }
            else
            {
                Type type = TypeResolutionUtils.ResolveType(typeName);
                if (typeof(IApplicationContext).IsAssignableFrom(type))
                {
                    return type;
                }
                else
                {
                    throw new TypeMismatchException(type.Name + " does not implement IApplicationContext.");
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the context should be lazily
        /// initialized.
        /// </summary>
        private bool IsLazy
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the array of resources containing object definitions for
        /// this context.
        /// </summary>
        private IList<string> GetResources(IConfigurationSection section)
        {
            IConfigurationSection resourceSection = section.GetSection(ContextSchema.ResourceElement);
            if (resourceSection.Exists())
            {
                List<string> resourceNodes = new List<string>(resourceSection.GetChildren().Count());
                foreach (var item in resourceSection.GetChildren())
                {
                    string resourceName = item.GetValue<string>(ContextSchema.URIAttribute);
                    if (StringUtils.HasText(resourceName))
                    {
                        resourceNodes.Add(resourceName);
                    }
                }
                return resourceNodes;
            }

            return new string[0];
            
        }

        /// <summary>
        /// Returns the array of child contexts for this context.
        /// </summary>
        private IList<IConfigurationSection> GetChildContexts(IConfigurationSection contextElement)
        {
            List<IConfigurationSection> contextNodes = new List<IConfigurationSection>();
            
            IConfigurationSection childContext = contextElement.GetSection(ContextSchema.ContextElement);
            if (childContext.Exists())
            {
                foreach (var item in childContext.GetChildren())
                {
                    contextNodes.Add(item);
                }
            }
            
            return contextNodes;
        }


    }

}
