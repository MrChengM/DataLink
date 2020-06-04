using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketServers;
using ModbusDrivers.Server;

namespace TaskHandler.Factory
{
    public abstract class ServerFactory
    {
        protected TimeOut _timeOut;
        protected ILog _log;
        protected int _savleId;
        protected int _maxConnect;
        public ServerFactory()
        {
        }
        public abstract IServerDrivers CreateServer(ServerName name);
    }

    public class TCPServerFactory : ServerFactory
    {
        private EthernetSetUp _setUp;
        private SocketServerType _ssType;
        public TCPServerFactory(EthernetSetUp setUp,TimeOut timeOut,ILog log,int savleId,int maxConnect, SocketServerType ssType=SocketServerType.SaeaServer)
        {
            _setUp = setUp;
            _timeOut = timeOut;
            _log = log;
            _savleId = savleId;
            _maxConnect = maxConnect;
            _ssType = ssType;
        }
        public override IServerDrivers CreateServer(ServerName name)
        {
            if(name == ServerName.ModbusTCPServer)
            {
                return new ModbusTCPServer(_setUp, _timeOut, _log, _maxConnect,_savleId,_ssType);
            }
            else
            {
                return null; ///还未实现其他TCP 服务类型
            }
        }
    }
    /// <summary>
    /// 还未实现其他基于Com口服务类型
    /// </summary>
    public class ComServerFactory : ServerFactory
    {
        private SerialportSetUp _setUp;
        public ComServerFactory(SerialportSetUp setUp, TimeOut timeOut, ILog log, int savleId)
        {
            _setUp = setUp;
            _timeOut = timeOut;
            _log = log;
            _savleId = savleId;
        }
        public override IServerDrivers CreateServer(ServerName name)
        {
            if (name == ServerName.ModbusRTUServer)
            {
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
