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

namespace SignalRSelfHost
{
    public class SignalRServerDemo
    {
        private ILog _log;
        public static UnityContainer Container;

        private IDisposable disposable;
        public SignalRServerDemo(ILog log)
        {
            _log = log;
        }
        public void StartServer(string url )
        {
            disposable =  WebApp.Start<Startup>(url); 
            _log.InfoLog("Server running on {0}", url);
        }
        public void StopServer(string url)
        {
            disposable.Dispose();
            _log.InfoLog("Server stop {0}", url);

        }
    }
}
