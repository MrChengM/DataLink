using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SocketServers.SAEA;

namespace SocketServers
{
    public  class SocketServerFactroy
    {
        private string _serverName;
        public string IPString
        {
            get;
            set;
        }

        //public IPAddress IpAddress
        //{
        //    get { return ipAddress; }
        //    set { ipAddress = value; }
        //}
        public int IpPort
        {
            get;
            set;
        }

        public ILog Log
        {
            get;
            set;
        }
        public TimeOut TimeOut
        {
            get;
            set;
        }
        public int ReadCacheSize
        {
            get;
            set;
        }
        public int MaxConnecter
        {
            get;
            set;
        }

        public SocketServerFactroy(string serverName,string ipString,int port,ILog log,TimeOut timeout,int readCacheSize,int maxConnecter)
        {
            _serverName = serverName;
            IPString = ipString;
            IpPort = port;
            Log = log;
            TimeOut = timeout;
            ReadCacheSize = readCacheSize;
            MaxConnecter = maxConnecter;
        }

        public ISockteServer CreateInstance(SocketServerType type)
        {
            switch (type)
            {
                case SocketServerType.ApmServer:
                    return new APMServer(_serverName,IPString, IpPort, Log, TimeOut, MaxConnecter, ReadCacheSize);
                case SocketServerType.SaeaServer:
                    return new SAEAServer(_serverName,IPString, IpPort, Log, TimeOut, MaxConnecter, ReadCacheSize);
                default:
                    return null;
            }
        }
    }
    public enum SocketServerType
    {
        SaeaServer=0x01,
        ApmServer=0x02,
        //TAPServer=0x03, 暂时未实现
    }
}
