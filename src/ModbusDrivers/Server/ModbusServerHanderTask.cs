using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Serialization;
using DataServer.Utillity;
using DataServer.Points;
namespace ModbusDrivers.Server
{
    public class ModbusServerHanderTask: IDisposable
    {
        ModbusTCPServer _server ;
        ModbusServerConfig _config;
        PointVirtualCollcet _points;
        XMLWorkbook _workbook;
        ILog _log ;
        TimeOut _timeout ;
        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        public ModbusServerHanderTask(ILog log)
        {
            _log = log;
        }
        public bool OnInit()
        {
            _log.NormalLog(string.Format("ModbusServerHanderTask:Init=>Initing"));
            try
            {
                _config = ReaderXMLUtil.ReadXMLConfig<ModbusServerConfig>("../../../../conf/Configuration.xml", ModbusServerConfig.ReadConfig, "setup", "ModbusTCPServerHandlerTask")[0];
                if (_config.SignalListFilePath == "")
                {
                    _log.ErrorLog("ModbusServerHanderTask Init fail: signal file path is null!");
                    return false;
                }
                _workbook = XmlSerialiaztion.XmlDeserial<XMLWorkbook>(_config.SignalListFilePath, _log);
                if (_workbook == default(XMLWorkbook))
                {
                    _log.ErrorLog("ModbusServerHanderTask Init fail:Read signal file fail!");
                    return false;
                }
                _points = PointsCollcetCreate.CreateMoudbus(_workbook, _log);
                ModbusPointsRegister.Register(_points, _log);
                _timeout = new TimeOut("ModbusServerHanderTask", _config.TimeOut, _log);
                _server = new ModbusTCPServer(new EthernetSetUp(_config.IpAddress, _config.Port), _timeout, _log, _config.MaxConnect, _config.SalveId, SocketServers.SocketServerType.SaeaServer);
                if(_server.Init())
                {
                    _log.NormalLog(string.Format("ModbusServerHanderTask:Initing=>Inited"));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                _log.ErrorLog(string.Format("ModbusServerHanderTask Init fail:{0}", e.Message));
                return false;
            }
        }
        public bool OnStart()
        {
            _log.NormalLog(string.Format("ModbusServerHanderTask:Start=>Starting"));
            var result= _server.Start();
            _log.NormalLog(string.Format("ModbusServerHanderTask:Starting=>Sarted"));
            return result;
        }
        public bool OnStop()
        {
            return _server.Stop();
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ModbusServerHanderTask() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
