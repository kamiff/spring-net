using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            var creater = new ContextCreater(configuration, netCoreConfigObjectName);
            IApplicationContext springContext = creater.Create(sectionName);
            /*
            if (springContext is IConfigurableApplicationContext)
            {
                if (StringUtils.IsNullOrEmpty(netCoreConfigObjectName))
                {
                    netCoreConfigObjectName = NET_CORE_CONFIG_OBJECT_NAME;
                }
                IConfigurableApplicationContext ctx = (IConfigurableApplicationContext)springContext;
                if (ctx.ObjectFactory.ContainsSingleton(netCoreConfigObjectName))
                {
                    throw new ObjectDefinitionStoreException(string.Format("Object name:{0} is already in use", netCoreConfigObjectName));
                }
                else
                {
                    ctx.ObjectFactory.RegisterSingleton(netCoreConfigObjectName, configuration);
                }
            }
            */
            services.AddSingleton(springContext);
            return services;
        }
    }
}
