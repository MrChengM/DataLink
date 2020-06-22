using DataServer;
using DataServer.Serialization;
using DataServer.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHandler.Config;
using TaskHandler.Factory;
using System.IO.Ports;

namespace TaskHandler.Builder
{
    public abstract class ServerTaskBuilder
    {
        protected ServerTaskHander _serverTask;
        protected ILog _log;
        protected string _taskName;
        protected ServerName _serverName;
        protected string _configFilePath = "../../../../conf/Configuration.xml";
        protected ServerConfig  _baseconfig;
        public ServerTaskBuilder(ServerName name, ILog log)
        {
            _serverTask = new ServerTaskHander(this);
            _serverName = name;
            _taskName = log.GetName();
            _log = log;
        }
        public virtual bool BuildTaskName()
        {

            _serverTask.TaskName = _taskName;
            return true;
        }
        public virtual bool BuildConfig()
        {
            return true;
        }
        public virtual bool BuildPoints()
        {
            var workBook = XmlSerialiaztion.XmlDeserial<XMLWorkbook>(_baseconfig.SignalListFilePath, _log);
            if (workBook == null)
            {
                _log.ErrorLog("BuildPoints:Points List data read Error!!");
                return false;
            }
            var pointsFactory = new PointsCollcetFactory(workBook, _log);
            _serverTask.Points = pointsFactory.CreatePoints(_serverName);
            return true;
        }
        public virtual bool BuildServer()
        {
            return true;
        }
        public virtual bool BuildLog()
        {
            _serverTask.Log = _log;
            return true;
        }
        public ServerTaskHander GetResult()
        {
            return _serverTask;
        }
    }
    public class TCPServerTaskBuilder : ServerTaskBuilder
    {
        private TCPServerConfig _config;
        public TCPServerTaskBuilder(ServerName name,ILog log,ServerConfig serverConfig):base(name,log)
        {
            _config = serverConfig as TCPServerConfig;
        }
        public override bool BuildConfig()
        {
            if (_config != null)
            {
                _serverTask.Config = _config;
                _baseconfig = _config;
                return base.BuildConfig();
            }
            else
            {
                return false;
            }
        }
        public override bool BuildServer()
        {
            if (_config.IpAddress == "")
            {
                _log.ErrorLog("BuildClient:IpAddres is null!!");
                return false;
            }
            var setup = new EthernetSetUp(_config.IpAddress, _config.Port);
            var timeout = new TimeOut(_taskName, _config.TimeOut, _log);
            var factory = new TCPServerFactory(setup,timeout,_log,_config.SalveId,_config.MaxConnect);
            _serverTask.Server = factory.CreateServer(_serverName);
            return base.BuildServer();
        }
    }
    public class ComServerTaskBuilder : ServerTaskBuilder
    {
        private ComServerConfig _config;
        public ComServerTaskBuilder(ServerName name, ILog log,ServerConfig serverConfig) : base(name,log)
        {
            _config = serverConfig as ComServerConfig;
        }
        public override bool BuildConfig()
        {
            if (_config != null)
            {
                _serverTask.Config = _config;
                _baseconfig = _config;
                return base.BuildConfig();
            }
            else
            {
                return false;
            }
        }
        public override bool BuildServer()
        {
            if (_config.ComPort == "")
            {
                _log.ErrorLog("BuildServer:ComPort is null!!");
                return false;
            }
            var setup = new SerialportSetUp(_config.ComPort,_config.BuadRate,(StopBits)_config.StopBit);
            var timeout = new TimeOut(_taskName, _config.TimeOut, _log);
            var factory = new ComServerFactory(setup, timeout, _log, _config.SalveId);
            _serverTask.Server = factory.CreateServer(_serverName);
            return base.BuildServer();
        }
    }
}
