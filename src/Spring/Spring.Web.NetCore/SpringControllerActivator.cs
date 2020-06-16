using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Spring.Web.NetCore
{
    /// <summary>
    /// Use the Spring container to create the Controller object. If the specified object is not configured in the container, it will be created by default <see cref="DefaultControllerActivator"/>.
    /// 
    /// <see cref="IControllerActivator"/> that uses type activation to create controllers.
    /// </summary>
    public class SpringControllerActivator : IControllerActivator
    {
        private readonly IApplicationContext context;
        private static readonly TypeActivatorCache typeActivatorCache = new TypeActivatorCache();
        /// <summary>
        /// Creates a new <see cref="SpringControllerActivator"/>.
        /// </summary>
        /// <param name="context">The <see cref="IApplicationContext"/>.</param>
        public SpringControllerActivator(IApplicationContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public object Create(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
            var controllerTypeInfo = controllerContext.ActionDescriptor.ControllerTypeInfo;
            if (controllerTypeInfo == null)
            {
                throw new ArgumentException("controllerTypeInfo is null");
            }

            Type serviceType = controllerTypeInfo.AsType();

            var matchMap = context.GetObjectsOfType(serviceType);
            if (matchMap.Any())
            {
                return matchMap.Values.First();
            }
            var serviceProvider = controllerContext.HttpContext.RequestServices;
            return typeActivatorCache.CreateInstance<object>(serviceProvider, serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="controller"></param>
        public void Release(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (controller is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        /// <summary>
        /// <see cref="Microsoft.AspNetCore.Mvc.Infrastructure.TypeActivatorCache"/>
        /// </summary>
        internal class TypeActivatorCache
        {
            private readonly Func<Type, ObjectFactory> _createFactory = (type) => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
            private readonly ConcurrentDictionary<Type, ObjectFactory> _typeActivatorCache = new ConcurrentDictionary<Type, ObjectFactory>();

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TInstance"></typeparam>
            /// <param name="serviceProvider"></param>
            /// <param name="implementationType"></param>
            /// <returns></returns>
            public TInstance CreateInstance<TInstance>(IServiceProvider serviceProvider, Type implementationType)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException(nameof(serviceProvider));
                }

                if (implementationType == null)
                {
                    throw new ArgumentNullException(nameof(implementationType));
                }

                var createFactory = _typeActivatorCache.GetOrAdd(implementationType, _createFactory);
                return (TInstance)createFactory(serviceProvider, arguments: null);
            }
        }
    }
}
