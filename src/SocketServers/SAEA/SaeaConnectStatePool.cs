using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;

namespace SocketServers.SAEA
{
    public class SaeaConnectStatePool : ObjectPool<SaeaConnectState>
    {
        private ILog _log;
        private int _id;
        private TimeOut _timeout;
        public SaeaConnectStatePool(ILog log, TimeOut timeout) :base()
        {
            _log = log;
            _id = 0;
            _timeout = timeout;
        } 
        public void Init(int size)
        {
            if (size < 0)
                _log.ErrorLog("SaeaConnectStatePool Init error ,count must  not be less than zero.");
            for(int i = 0; i < size; i++)
            {
                Add(Create());
            }

        }
        protected override SaeaConnectState Create()
        {
            var item= new SaeaConnectState(_log, ++_id,_timeout,this);
            item.Init();
            return item;
        }
    }
}
