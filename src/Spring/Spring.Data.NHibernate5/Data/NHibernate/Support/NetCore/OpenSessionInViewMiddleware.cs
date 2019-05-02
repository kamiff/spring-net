using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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
        public static IApplicationBuilder OpenSessionInViewMiddleware(this IApplicationBuilder builder, string configSection = "openSessionInView")
        {
            return builder.UseMiddleware<OpenSessionInViewMiddleware>(configSection);
        }
    }


}
