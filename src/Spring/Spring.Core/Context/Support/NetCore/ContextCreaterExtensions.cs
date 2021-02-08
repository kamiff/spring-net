using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spring.Context;
using Spring.Context.Support;
using Spring.Objects.Factory;
using Spring.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up Spring IOC Container in an <see cref="IServiceCollection" />.
    /// </summary>
    /// <author>kamiff lee</author>
    public static class ContextCreaterExtensions
    {
        /// <summary>
        /// Init Spring Ioc Context and set into services <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <param name="netCoreConfigObjectName">The object name by the net core system configuration object</param>
        /// <returns></returns>
        public static IServiceCollection WithSpring(this IServiceCollection services, IConfiguration configuration, string sectionName = "springContext", string netCoreConfigObjectName = "NetCoreConfig")
        {
            WithSpringReturnContext(services, configuration, sectionName, netCoreConfigObjectName);
            return services;
        }

        /// <summary>
        /// Init Spring Ioc Context and set into services <see cref="IServiceCollection"/> and return the <see cref="IApplicationContext"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <param name="netCoreConfigObjectName">The object name by the net core system configuration object</param>
        /// <returns></returns>
        public static IApplicationContext WithSpringReturnContext(this IServiceCollection services, IConfiguration configuration, string sectionName = "springContext", string netCoreConfigObjectName = "NetCoreConfig")
        {
            var creater = new ContextCreater(configuration, netCoreConfigObjectName);
            IApplicationContext springContext = creater.Create(sectionName);
            services.AddSingleton(springContext);
            return springContext;
        }
        /// <summary>
        /// Register the net core default ServiceProvider into Spring Context as a singleton object named 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="serviceProviderName">the name of ServiceProvider in Spring context</param>
        /// <returns></returns>
        public static IHost LoadServiceProviderIntoSpring(this IHost host, string serviceProviderName = "systemServiceProvider")
        {
            if (string.IsNullOrWhiteSpace(serviceProviderName))
            {
                serviceProviderName = "systemServiceProvider";
            }
            IConfigurableApplicationContext springContext = host.Services.GetService(typeof(IApplicationContext)) as IConfigurableApplicationContext;
            if (springContext != null)
            {
                springContext.ObjectFactory.RegisterSingleton(serviceProviderName, host.Services);
            }

            return host;
        }
    }
}
