using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Spring.Context;
using Spring.Objects.Factory;
using Spring.Util;

namespace Spring.Data.NHibernate.Support
{
    /// <summary>
    ///The OpenSessionInView Middleware
    /// </summary>
    public class OpenSessionInViewMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate next;
        private readonly NetCoreConfigSectionSessionScopeSettings sessionScopeSettings;
        /// <summary>
        /// Create OpenSessionInViewMiddleware by Default Config Name:<paramref name="configSectionName"/> in appsettings.json
        /// </summary>
        /// <remarks>
        /// "OpenSessionInView": {
        ///     "SessionFactoryObjectName": "HibernateSessionFactory",
        ///     "EntityInterceptorObjectName": "EntityInterceptorObjectName", /*Optional*/
        ///     "SingleSession": true/false, /*Optional default:true*/
        ///     "DefaultFlushMode" : "Never|Commit|Auto|Always" /*Optional default:Never*/
        ///  }
        /// </remarks>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="configSection">Root Config Section Name in appsettings.json</param>
        public OpenSessionInViewMiddleware(RequestDelegate next, IConfiguration configuration, string configSectionName, IApplicationContext applicationContext)
        {
            this.next = next;
            this.sessionScopeSettings = new NetCoreConfigSectionSessionScopeSettings(configSectionName, configuration, applicationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="sessionFactory"></param>
        public OpenSessionInViewMiddleware(RequestDelegate next, ISessionFactory sessionFactory)
        {
            this.next = next;
            this.sessionScopeSettings = new NetCoreConfigSectionSessionScopeSettings(sessionFactory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="sessionFactory"></param>
        public OpenSessionInViewMiddleware(RequestDelegate next, ISessionFactory sessionFactory, IInterceptor entityInterceptor = null, bool singleSession = true, FlushMode defaultFlushMode = FlushMode.Never)
        {
            this.next = next;
            this.sessionScopeSettings = new NetCoreConfigSectionSessionScopeSettings(sessionFactory, entityInterceptor, singleSession, defaultFlushMode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            using (new SessionScope(this.sessionScopeSettings, false))
            {
                return next(context);
            }
            /*
            // Do something with context near the beginning of request processing.
            try
            {
                this.Open();
                return next(context);
            }
            finally
            {
                // Clean up.
                this.Close();
            }
            */
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class OpenSessionInViewMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DEFAULT_CONFIG_SECTION_NAME = "OpenSessionInView";
        /// <summary>
        /// Add OpenSessionInViewMiddleware 
        /// </summary>
        /// <remarks>
        /// "OpenSessionInView": {
        ///     "SessionFactoryObjectName": "HibernateSessionFactory",
        ///     "EntityInterceptorObjectName": "EntityInterceptorObjectName", /*Optional*/
        ///     "SingleSession": true/false, /*Optional default:true*/
        ///     "DefaultFlushMode" : "Never|Commit|Auto|Always" /*Optional default:Never*/
        ///  }
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IApplicationBuilder WithOpenSessionInViewMiddleware(this IApplicationBuilder builder, string configSection = DEFAULT_CONFIG_SECTION_NAME)
        {
            AssertUtils.ArgumentNotNull(builder, "builder");
            AssertUtils.ArgumentHasText(configSection, "configSection");
            IApplicationContext ctx = builder.ApplicationServices.GetRequiredService<IApplicationContext>();

            return builder.UseMiddleware<OpenSessionInViewMiddleware>(configSection, ctx);
        }

        /// <summary>
        /// Add OpenSessionInViewMiddleware 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IApplicationBuilder WithOpenSessionInViewMiddleware(this IApplicationBuilder builder, ISessionFactory sessionFactory)
        {
            AssertUtils.ArgumentNotNull(builder, "builder");
            AssertUtils.ArgumentNotNull(sessionFactory, "sessionFactory");
            
            return builder.UseMiddleware<OpenSessionInViewMiddleware>(sessionFactory);
        }
        /// <summary>
        /// Add OpenSessionInViewMiddleware 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IApplicationBuilder WithOpenSessionInViewMiddlewareByFactoryName(this IApplicationBuilder builder, string sessionFactoryName)
        {
            AssertUtils.ArgumentNotNull(builder, "builder");
            AssertUtils.ArgumentHasText(sessionFactoryName, "sessionFactoryName");

            IApplicationContext ctx = builder.ApplicationServices.GetRequiredService<IApplicationContext>();
            ISessionFactory sf = ctx.GetObject<ISessionFactory>(sessionFactoryName);
            if (sf == null)
            {
                throw new NoSuchObjectDefinitionException("sessionFactoryName", "Init Open Session In View Middleware By FactoryName");
            }
            return builder.UseMiddleware<OpenSessionInViewMiddleware>(sf);
        }

    }


}
