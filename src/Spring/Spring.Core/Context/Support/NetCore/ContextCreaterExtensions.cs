using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spring.Objects.Factory;
using Spring.Util;
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
        /// Default config section name in appsettings.json
        /// </summary>
        private const string CONFIG_SECTION_NAME = "springContext";
        /// <summary>
        /// The object name by the net core system configuration object
        /// <see cref="IConfiguration"/>
        /// </summary>
        private const string NET_CORE_CONFIG_OBJECT_NAME = "NetCoreConfig";
        /// <summary>
        /// Init Spring Ioc Context and set into services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IServiceCollection WithSpring(this IServiceCollection services, IConfiguration configuration, string sectionName = "springContext", string netCoreConfigObjectName = "NetCoreConfig")
        {
            var creater = new ContextCreater();
            IApplicationContext springContext = creater.Create(null, configuration, sectionName);
            if (springContext is IConfigurableApplicationContext)
            {
                if (StringUtils.IsNullOrEmpty(netCoreConfigObjectName))
                {
                    netCoreConfigObjectName = NET_CORE_CONFIG_OBJECT_NAME;
                }

                if (springContext.ContainsObject(netCoreConfigObjectName))
                {
                    throw new ObjectDefinitionStoreException(string.Format("Object name:{0} is already in use", netCoreConfigObjectName));
                }
                else
                {
                    ((IConfigurableApplicationContext)springContext).ObjectFactory.RegisterSingleton(netCoreConfigObjectName, configuration);
                }
            }
            services.AddSingleton<IApplicationContext>((IServiceProvider sp) => { return springContext; });
            return services;
        }
    }
}
