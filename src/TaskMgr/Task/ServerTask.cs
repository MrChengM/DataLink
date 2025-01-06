using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Log;
using DataServer.Config;
using DataServer.Points;
using DataServer.Task;
using TaskMgr.Factory;
using Unity;

namespace TaskMgr.Task
{
    public class ServerTask : AbstractTask
    {
        private IServerDrivers _server;
        private IPointMapping _pointMapping;
        private ILog _log;
        private ServerItemConfig serverConfig;

        public ServerItemConfig ServerConfig
        {
            get { return serverConfig; }
            set { serverConfig = value; }
        }
     

        [InjectionConstructor]
        public ServerTask(IPointMapping pointMapping, ILog log)
        {
            _initLevel = 4;
            _pointMapping = pointMapping;
            _log = log;
            _timeout = new TimeOut() { TimeOutSet = 1000 };
        }
        public override bool OnInit()
        {
            _taskName = serverConfig.Name;
            _log.InfoLog($"{_taskName}: Init => Initing ");
            _server = ctreatServer(serverConfig);

            if (_server != null && _server.Init())
            {
                _server.RegisterMapping(serverConfig.TagBindingList);
                _log.InfoLog($"{_taskName}: Initing=>Inited");
                return true;
            }
            else
            {
                _log.ErrorLog($"{_taskName}: Init error ");
                return false;
            }
        }
        private IServerDrivers ctreatServer(ServerItemConfig serverItem)
        {
            return new ServerFactory(_pointMapping, _log).CreateInstance(serverItem);
        }
        public override bool OnStart()
        {
            _log.InfoLog($"{_taskName}: Start=>Starting ");
            if (_server != null && _server.Start())
            {
                _log.InfoLog($"{_taskName}: Starting=>Started ");
                return true;
            }
            else
            {
                _log.InfoLog($"{_taskName}: Start Failed ");
                return false;
            }
        }

        public override bool OnStop()
        {
            _log.InfoLog($"{_taskName}: Stop=>Stoping ");
            if (_server != null && _server.Stop())
            {
                _log.InfoLog($"{_taskName}: Stoping=>Stopped ");
                return true;
            }
            else
            {
                _log.InfoLog($"{_taskName}: Stop Failed ");
                return false;
            }
        }

        public override bool Restart()
        {
            bool result = false;
            _log.InfoLog($"{_taskName}: Restart => Restarting ");
            if (OnStop())
            {
                if (OnInit() && OnStart())
                {
                    _log.InfoLog($"{_taskName}: Restart => Restarted ");
                    result = true;
                }
            }
            else
            {
                _log.InfoLog($"{_taskName}: Restart => Failed ");
                result = false;
            };

            return result;
        }
    }
}
