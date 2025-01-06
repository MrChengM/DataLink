// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using SignalRSelfHost.Connections;
using Unity;
using Web=System.Web.Http.Dependencies;

namespace SignalRSelfHost
{
    public class Startup
    {

        /// <summary>
        /// SignalR Configuratio
        /// </summary>
        /// <param name="app"></param>
        /// <param name="container"></param>
        public void Configuration(IAppBuilder app, UnityContainer container)
        {
    //        GlobalHost.DependencyResolver.Register(
    //typeof(AlarmHub),
    //() => new AlarmHub(container.Resolve<IAlarmTask>()));
            app.UseErrorPage();

            //app.Map("/raw-connection", map =>
            //{
            //    // Turns cors support on allowing everything
            //    // In real applications, the origins should be locked down
            //    map.UseCors(CorsOptions.AllowAll)
            //   .RunSignalR<RawConnection>();
            //});
            //app.Map("/signalr", map =>
            //{
            //    var config = new HubConfiguration
            //    {
            //        // You can enable JSONP by uncommenting this line
            //        // JSONP requests are insecure but some older browsers (and some
            //        // versions of IE) require JSONP to work cross domain
            //        // EnableJSONP = true
            //    };

            //    // Turns cors support on allowing everything
            //    // In real applications, the origins should be locked down
            //    map.UseCors(CorsOptions.AllowAll)
            //       .RunSignalR(config);
            //});

            //for SignalR config
            var hubconfig = new HubConfiguration();
            //传送详细错误信息给客户端
            hubconfig.EnableDetailedErrors = true;
            hubconfig.Resolver = new UnitySignalRDependencyResolver(container);
            //采用IOC容器，GlobalHost DependencyResolver 必须绑定，否则全局调用Content会失效;
            GlobalHost.DependencyResolver = hubconfig.Resolver;
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(hubconfig);

            // Configure Web API for self-host. 
            HttpConfiguration httpConfig = new HttpConfiguration();
            httpConfig.DependencyResolver =new UnityHttpResolver(container);
            httpConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            app.UseWebApi(httpConfig);
        }
     
        /// <summary>
        /// SignalR Hub IOC
        /// </summary>
        internal class UnitySignalRDependencyResolver : DefaultDependencyResolver
        {
            private readonly UnityContainer _container;
            public UnitySignalRDependencyResolver(UnityContainer container)
            {
                _container = container;
            }

            public override object GetService(Type serviceType)
            {
                try
                {
                    return _container.Resolve(serviceType) ?? base.GetService(serviceType);
                }
                catch (Exception)
                {

                    return base.GetService(serviceType);
                }
            }

            public override IEnumerable<object> GetServices(Type serviceType)
            {
                return _container.ResolveAll(serviceType).Concat(base.GetServices(serviceType));
            }
        }
        /// <summary>
        /// Web API IOC
        /// </summary>
        internal class UnityHttpResolver : Web.IDependencyResolver
        {
            protected IUnityContainer container;

            public UnityHttpResolver(IUnityContainer container)
            {
                if (container == null)
                {
                    throw new ArgumentNullException(nameof(container));
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

            public Web.IDependencyScope BeginScope()
            {
                var child = container.CreateChildContainer();
                return new UnityHttpResolver(child);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                container.Dispose();
            }
        }
    }
}
