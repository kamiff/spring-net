using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Spring.Data.NHibernate.Support
{
    public class OpenSessionInViewMiddleware : SessionScope
    {
        private readonly RequestDelegate _next;

        public OpenSessionInViewMiddleware(RequestDelegate next, string configSection, IConfiguration configuration) : base(new NetCoreConfigSectionSessionScopeSettings(configSection, configuration), false)
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
}
