using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Log;

namespace SocketServers.SAEA
{
    public class SaeaConnectStatePool : ObjectPool<SaeaConnectState>
    {
        private ILog _log;
        private int _id;
        private TimeOut _timeout;
        private string _serverName;
        public SaeaConnectStatePool(string serverName,ILog log, TimeOut timeout) :base()
        {
            _serverName = serverName;
            _log = log;
            _id = 0;
            _timeout = timeout;
        } 
        public void Init(int size)
        {
            if (size < 0)
                _log.ErrorLog("SaeaConnectStatePool Init error ,count must  not be less than zero.");
            _id = 0;
            for (int i = 0; i < size; i++)
            {
                Add(Create());
            }

        }
        protected override SaeaConnectState Create()
        {
            var item= new SaeaConnectState(_serverName,_log, _id++, _timeout,this);
            item.Init();
            return item;
        }
    }
}
