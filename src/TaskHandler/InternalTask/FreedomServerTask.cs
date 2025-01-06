using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreedomDrivers;
using DataServer;

namespace TaskHandler
{
    class FreedomServerTask : AbstractTask
    {
        private FreedomServer _freedomServer;
 
        public FreedomServerTask(string taskName,ILog log,TimeOut timeOut,EthernetSetUp setup)
        {
            TaskName = taskName;
            Log = log;
            InitLevel = 4;
            TimeOut = timeOut;
            _freedomServer = new FreedomServer(setup, TimeOut, Log);
        }
        public override bool OnInit()
        {
            
            _log.InfoLog(string.Format("{0}:Init=>Initing ", "OnInit()"));

            if(_freedomServer.Init())
            {
                _log.InfoLog(string.Format("{0}:Initing=>Inited ", "OnInit()"));
                return true;
            }
            return false;

        }

        public override bool OnStart()
        {
            _log.InfoLog(string.Format("{0}:Start=>Starting ", "OnStart()"));

            if(_freedomServer.Start())
            {
                _log.InfoLog(string.Format("{0}:Starting=>Started ", "OnStart()"));
                return true;
            }
            return false;
        }

        public override bool OnStop()
        {
            return _freedomServer.Stop();
        }
    }
}
