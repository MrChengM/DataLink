using DataServer;
using DataServer.Serialization;
using ModbusDrivers.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHandler.Config;
using DataServer.Points;
using TaskHandler.Builder;

namespace TaskHandler
{
    public class ServerTaskHander : AbstractTask
    {
        IServerDrivers _server;
        ServerConfig _config;
        PointVirtualCollcet _points;
        ServerTaskBuilder _builder;
        public IServerDrivers Server
        {
            get { return _server; }
            set { _server = value; }
        }
        public ServerConfig Config
        {
            get { return _config; }
            set { _config = value; }
        }
        public PointVirtualCollcet Points
        {
            get { return _points; }
            set { _points = value; }
        }
        public ServerTaskHander(ServerTaskBuilder builder)
        {
            _builder = builder;
        }
        public override bool OnInit()
        {
            try
            {
                if (_builder.BuildTaskName())
                {
                    if (_builder.BuildLog())
                    {
                        _log.InfoLog(string.Format("{0}:Init=>Initing ", "OnInit()"));
                        if (_builder.BuildConfig())
                        {
                            if (_builder.BuildPoints())
                            {
                                if (_builder.BuildServer())
                                {
                                    _server.Init();
                                    _log.InfoLog(string.Format("{0}:Initing=>Inited ", "OnInit()"));
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                _log.ErrorLog(string.Format("Init fail:{0}", e.Message));
                return false;
            }
        }
        public override bool OnStart()
        {
            _log.InfoLog(string.Format("{0}:Start=>Starting", "OnStart"));
            var result = _server.Start();
            _log.InfoLog(string.Format("{0}:Starting=>Sarted", "OnStart"));
            return result;
        }
        public override bool OnStop()
        {
            return _server.Stop();
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _server.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }
        #endregion
    }
}
