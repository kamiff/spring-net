using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spring.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Spring.Web.NetCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class MVCExtensions
    {
        /// <summary>
        ///  Use the Spring framework to load the controller
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="springContext"></param>
        /// <returns></returns>
        public static IMvcBuilder UseSpringMVC(this IMvcBuilder builder, IApplicationContext springContext)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            if(springContext == null)
            {
                throw new ArgumentNullException("springContext");
            }
            var activator = new SpringControllerActivator(springContext);
            builder.Services.Replace(ServiceDescriptor.Singleton<IControllerActivator, SpringControllerActivator>((inter) => {
                return activator;
            }));
            return builder;
        }

        /// <summary>
        /// Use the Spring framework to load the controller
        /// Before use this method, you need to make sure that Spring's IApplicationContext has been injected into builder.Services
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="NullReferenceException">Fires when IApplicationContext does not exist builder.Services or is not a singleton</exception>
        /// <returns></returns>
        public static IMvcBuilder UseSpringMVC(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            Type springContextClazz = typeof(IApplicationContext);
            if (builder.Services.Any(o => o.ServiceType.Equals(springContextClazz) && o.Lifetime == ServiceLifetime.Singleton))
            {
                builder.Services.Replace(ServiceDescriptor.Singleton<IControllerActivator, SpringControllerActivator>());
            }
            else
            {
                throw new NullReferenceException("Spring Context is not exist in builder.Services or is not Singleton");
            }
            return builder;
        }
    }
}
