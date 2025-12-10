using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Unity;

namespace FashionStore
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Set Unity resolver for Web API (after routes are configured)
            // This must be called after UnityConfig is initialized
            config.DependencyResolver = new UnityWebApiDependencyResolver(UnityConfig.Container);
        }
    }

    // Unity Dependency Resolver for Web API
    public class UnityWebApiDependencyResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityWebApiDependencyResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityWebApiDependencyResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}

