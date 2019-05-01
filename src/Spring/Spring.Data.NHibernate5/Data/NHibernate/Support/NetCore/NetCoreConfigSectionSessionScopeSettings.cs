using Microsoft.Extensions.Configuration;
using NHibernate;
using Spring.Context.Support;
using Spring.Objects.Factory.Config;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Data.NHibernate.Support
{
    public class NetCoreConfigSectionSessionScopeSettings : SessionScopeSettings
    {
        /// <summary>
        /// The default session factory name to use when retrieving the Hibernate session factory from
        /// the root context.
        /// </summary>
        public static readonly string DEFAULT_SESSION_FACTORY_OBJECT_NAME = "SessionFactory";

        private readonly string sessionFactoryObjectName = DEFAULT_SESSION_FACTORY_OBJECT_NAME;
        private readonly string entityInterceptorObjectName = null;




        /// <summary>
        /// Initializes a new <see cref="NetCoreConfigSectionSessionScopeSettings"/> instance.
        /// </summary>
        /// <param name="ownerType">The type, who's name will be used to prefix setting variables with</param>
        /// <param name="variableSource">The variable source to obtain settings from.</param>
        public NetCoreConfigSectionSessionScopeSettings(string sectionName, IConfiguration configuration)
            : base()
        {
            IVariableSource variableSource = new NetCoreConfigSectionVariableSource(sectionName, configuration);
            const string sessionFactoryObjectNameSettingsKey =  "SessionFactoryObjectName";
            const string entityInterceptorObjectNameSettingsKey = "EntityInterceptorObjectName";
            const string singleSessionSettingsKey = "SingleSession";
            const string defaultFlushModeSettingsKey = "DefaultFlushMode";

            VariableAccessor variables = new VariableAccessor(variableSource);
            this.sessionFactoryObjectName = variables.GetString(sessionFactoryObjectNameSettingsKey, DEFAULT_SESSION_FACTORY_OBJECT_NAME);
            this.entityInterceptorObjectName = variables.GetString(entityInterceptorObjectNameSettingsKey, null);
            this.SingleSession = variables.GetBoolean(singleSessionSettingsKey, this.SingleSession);
            this.DefaultFlushMode = (FlushMode)variables.GetEnum(defaultFlushModeSettingsKey, this.DefaultFlushMode);

            AssertUtils.ArgumentNotNull(sessionFactoryObjectName, "sessionFactoryObjectName"); // just to be sure
        }

        /// <summary>
        /// Resolve the entityInterceptor by looking up <see cref="entityInterceptorObjectName"/> 
        /// in the root application context.
        /// </summary>
        /// <returns>The resolved <see cref="IInterceptor"/> instance or <see langword="null"/></returns>
        protected override IInterceptor ResolveEntityInterceptor()
        {
            if (StringUtils.HasText(entityInterceptorObjectName))
            {
                return (IInterceptor)ContextRegistry.GetContext().GetObject(entityInterceptorObjectName);
            }
            return null;
        }

        /// <summary>
        /// Resolve the <see cref="ISessionFactory"/> by looking up <see cref="sessionFactoryObjectName"/> 
        /// in the root application context.
        /// </summary>
        /// <returns>The resolved <see cref="ISessionFactory"/> instance or <see langword="null"/></returns>
        protected override ISessionFactory ResolveSessionFactory()
        {
            if (StringUtils.HasText(sessionFactoryObjectName))
            {
                return (ISessionFactory)ContextRegistry.GetContext().GetObject(sessionFactoryObjectName);
            }
            return null;
        }
    }
}
