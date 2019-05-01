using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Context.Support
{
    /// <summary>
    /// 
    /// </summary>
    public static class ContextCreaterExtensions
    {
        /// <summary>
        /// Init Spring Ioc Context and set into services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IServiceCollection WithSpring(this IServiceCollection services, IConfiguration configuration, string sectionName = "springContext")
        {
            var creater = new ContextCreater();
            IApplicationContext springContext = creater.Create(null, configuration, sectionName);
            services.AddSingleton<IApplicationContext>((IServiceProvider sp) => { return springContext; });
            return services;
        }
    }
}
