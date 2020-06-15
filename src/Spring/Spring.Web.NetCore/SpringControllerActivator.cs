using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using System;
using System.Linq;

namespace Spring.Web.NetCore
{
    /// <summary>
    /// Create a Controller using Spring IOC as a bridge function
    /// </summary>
    public class SpringControllerActivator : IControllerActivator
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationContext context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public SpringControllerActivator(IApplicationContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// First try to use the Spring.NET container to create the corresponding controller. If the corresponding controller configuration is not found in the Spring container, the default method is used to create the controller
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public virtual object Create(ControllerContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
            Type serviceType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();

            var matchMap = context.GetObjectsOfType(serviceType);
            if (matchMap.Any())
            {
                return matchMap.Values.First();
            }
            return actionContext.HttpContext.RequestServices.GetRequiredService(serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public virtual void Release(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            (controller as IDisposable)?.Dispose();
        }
    }
}
