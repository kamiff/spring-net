using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Spring.Data.NHibernate.Support
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenSessionInViewMiddleware : SessionScope
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        public OpenSessionInViewMiddleware(RequestDelegate next, IConfiguration configuration, string configSection = "openSessionInView") : base(new NetCoreConfigSectionSessionScopeSettings(configSection, configuration), false)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.
            try
            {
                this.Open();
#pragma warning disable ConfigureAwaitChecker // CAC001
                await _next(context);
#pragma warning restore ConfigureAwaitChecker // CAC001
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
