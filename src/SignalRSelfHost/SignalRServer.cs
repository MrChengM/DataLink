using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using DataServer;
using DataServer.Log;
using DataServer.Task;
using System.Collections.Generic;
using DataServer.Alarm;
using Unity;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRSelfHost.Hubs;
using SignalRSelfHost.Connections;

namespace SignalRSelfHost
{
    public class SignalRServer
    {
        private ILog _log;
        public static UnityContainer Container;

        private IDisposable disposable;
        public SignalRServer(ILog log)
        {
            _log = log;
        }
        public void StartServer(string url, UnityContainer container)
        {
            disposable = WebApp.Start(url, app => new Startup().Configuration(app, container));
            _log.InfoLog("SignalServer: Server running on {0}", url);
            Container = container;
        }
        public void Stop()
        {
            disposable.Dispose();
        }
    }

    

}
