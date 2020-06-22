using DataServer.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Serialization;
using DataServer.Points;
using TaskHandler.Factory;
using System.IO.Ports;
using TaskHandler.Config;

namespace TaskHandler.Builder
{
   /*********
    PS：考虑是否合并为一个客户端建造类
   *********/
    public abstract class ClientTaskBuilder
    {
        protected ClientHandlerTask _clientTask;

        protected ILog _log;
        protected string _taskName;
        protected ClientName _clientName;
        protected string _configFilePath = "../../../../conf/Configuration.xml";
        protected ClientConfig _baseconfig;

        ////相同类型Task创建多个
        //protected List<ClientHandlerTask> _clientTasks;
        //protected List<ClientConfig> _baseconfigs;

        public ClientTaskBuilder(ClientName name, ILog log)
        {
            _clientTask = new ClientHandlerTask(this);
            _clientName = name;
            _log = log;
            _taskName = log.GetName();
            //_clientTasks = new List<ClientHandlerTask>();
        }
        public virtual bool BuildTaskName()
        {
            
            _clientTask.TaskName = _taskName;
            return true;
        }
        public virtual bool BuildConfig()
        {
            return true;
        }
        public virtual bool BuildPoints()
        {
            if (_baseconfig.SignalListFilePath == "")
            {
                _log.ErrorLog("BuildPoints:Points List data read Error,Path is null!!");
                return false;
            }
            var workBook = XmlSerialiaztion.XmlDeserial<XMLWorkbook>(_baseconfig.SignalListFilePath, _log);
            if (workBook == null)
            {
                _log.ErrorLog("BuildPoints:Points List data read Error!!");
                return false;
            }
            var pointsFactory = new PointsCollcetFactory(workBook, _log);
            var points = pointsFactory.CreatePoints(_clientName);
            PointsRegister.Register(ref points , _log);
            _clientTask.Points = points;
            return true;
        }
        public virtual bool BuildClient()
        {
            return true;
        }
        public virtual bool BuildLog()
        {
            _clientTask.Log = _log;
            return true;
        }
        public ClientHandlerTask GetResult()
        {
            return _clientTask;
        }
    }

    public class TCPClientTaskBuilder : ClientTaskBuilder
    {
        private TCPClientConfig _config;
        public TCPClientTaskBuilder(ClientName name,ILog log,ClientConfig config):base(name,log)
        {
            _config = config as TCPClientConfig;
        }
        public override bool BuildConfig()
        {
            if (_config != null)
            {
                _clientTask.Config = _config;
                _baseconfig = _config;
                return base.BuildConfig();
            }
            else
            {
                return false;
            }
              
        }
        public override bool BuildClient()
        {
            if (_config.IpAddress == "")
            {
                _log.ErrorLog("BuildClient :IP Address config is failed!!!");
                return false;
            }
            if (_config.Port == 0)
            {
                _log.ErrorLog("BuildClient :IP Port config is failed!!!");
                return false;
            }
            //var setup = new EthernetSetUp(_config.IpAddress, _config.Port);
            var timeOut = new TimeOut(_taskName, _config.TimeOut, _log);
            var factory = new TCPClientFactory(_config, timeOut, _log);
            _clientTask.Client = factory.CreateClient(_clientName);
            return base.BuildClient();
        }
    }

    public class ComClientTaskBuilder : ClientTaskBuilder
    {
        private ComClientConfig _config;
        public ComClientTaskBuilder(ClientName name,ILog log,ClientConfig clientConfig):base(name,log)
        {
            _config = clientConfig as ComClientConfig;
        }
        public override bool BuildConfig()
        {
            if (_config != null)
            {
                _clientTask.Config = _config;
                _baseconfig = _config;
                return base.BuildConfig();
            }
            else
            {
                return false;
            }

        }
        public override bool BuildClient()
        {
            if (_config.ComPort == "")
            {
                _log.ErrorLog("BuildClient :ComPort config is failed!!!");
                return false;
            }
            //var setup = new SerialportSetUp(_config.ComPort,_config.BuadRate,(StopBits)_config.StopBit);
            var timeOut = new TimeOut(_taskName, _config.TimeOut, _log);
            var factory = new ComClientFactory(_config, timeOut, _log);
            _clientTask.Client = factory.CreateClient(_clientName);
            return true;
        }
    }
}
