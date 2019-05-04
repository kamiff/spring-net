using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using System;
using System.Linq;

namespace Spring.Web.NetCore
{
    /// <summary>
    /// 
    /// </summary>
    public class SpringControllerActivator : IControllerActivator
    {
        ///
        private readonly IApplicationContext context;
        public SpringControllerActivator(IApplicationContext context)
        {
            this.context = context;
        }
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
