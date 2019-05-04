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
    /// 
    /// </summary>
    public class OpenSessionInViewMiddleware : SessionScope, IMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        public OpenSessionInViewMiddleware(IConfiguration configuration, string configSection = "openSessionInView") : base(new NetCoreConfigSectionSessionScopeSettings(configSection, configuration), false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactory"></param>
        public OpenSessionInViewMiddleware(ISessionFactory sessionFactory) : base(sessionFactory, false)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactory"></param>
        public OpenSessionInViewMiddleware(ISessionFactory sessionFactory, IInterceptor entityInterceptor = null, bool singleSession = true, FlushMode defaultFlushMode = FlushMode.Never) : base(sessionFactory, entityInterceptor, singleSession, defaultFlushMode, false)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public OpenSessionInViewMiddleware(SessionScopeSettings settings) : base(settings, false)
        {

        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Do something with context near the beginning of request processing.
            try
            {
                this.Open();
                return next(context);
            }
            finally
            {
                this.Close();
            }
            // Clean up.
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
        /// <param name="builder"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IApplicationBuilder WithOpenSessionInViewMiddleware(this IApplicationBuilder builder, string configSection = "openSessionInView")
        {
            AssertUtils.ArgumentNotNull(builder, "builder");
            AssertUtils.ArgumentHasText(configSection, "configSection");
            return builder.UseMiddleware<OpenSessionInViewMiddleware>(configSection);
        }

        /// <summary>
        /// 
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
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IApplicationBuilder WithOpenSessionInViewMiddleware(this IApplicationBuilder builder, SessionScopeSettings settings)
        {
            AssertUtils.ArgumentNotNull(builder, "builder");
            AssertUtils.ArgumentNotNull(settings, "settings");
            return builder.UseMiddleware<OpenSessionInViewMiddleware>(settings);
        }

    }


}
