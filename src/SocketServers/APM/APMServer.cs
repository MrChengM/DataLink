using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using DataServer;

namespace SocketServers
{
    public class APMServer : IDisposable
    {
        #region 字段

        //socket参数
        private Socket listenSocket;
        private string ipString;
        private IPAddress ipAddress;
        private int ipPort;
        private IPEndPoint ipEndPoint;
        private int maxConnectNum;
        private ILog log;
        private TimeOut timeOut;
        private int readSize;

        private ConnectState[] connecters;
        #endregion
        #region 属性
        public string IPString
        {
            get { return ipString; }
            set { ipString = value; }
        }

        //public IPAddress IpAddress
        //{
        //    get { return ipAddress; }
        //    set { ipAddress = value; }
        //}
        public int IpPort
        {
            get { return ipPort; }
            set { ipPort = value; }
        }
        public IPEndPoint IpEndPoint
        {
            get { return ipEndPoint; }
            set { ipEndPoint = value; }
        }

        public ILog Log
        {
            get { return log; }
            set { log = value; }
        }

        public ConnectState[] Connecters
        {
            get { return connecters; }
            private set { connecters = value; }
        }
        #endregion
        #region 方法

        public APMServer(string ipstring, int ipport, ILog log, TimeOut timeOut, int maxConnect,int size)
        {
            this.ipString = ipstring;
            this.ipPort = ipport;
            this.log = log;
            this.timeOut = timeOut;
            this.maxConnectNum = maxConnect;
            readSize = size;
        }
        public bool Init()
        {
            try
            {
                //初始化Sokect
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //初始化IP地址
                ipAddress = IPAddress.Parse(ipString);
                ipEndPoint = new IPEndPoint(ipAddress, ipPort);

                //初始化缓存池
                connecters = new ConnectState[maxConnectNum];

                for (int i = 0; i < connecters.Length; i++)
                {
                    connecters[i] = new ConnectState(log, timeOut, i,readSize);
                    //connecters[i].Init();
                }
                return true;
            }
            catch (Exception e)
            {
                string errorInfor = string.Format("Server init error: {0}", e.Message);
                log.ErrorLog(errorInfor);
                return false;
            }

        }

        /// <summary>
        /// 连接测试,判断是否连接
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool connectPoll(Socket client)
        {
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];
                client.Blocking = true;
                client.Send(tmp, 0, 0);
                return true;
            }
            catch (SocketException e)
            {
                // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                if (e.NativeErrorCode.Equals(10035))
                    return false;
                else if (e.NativeErrorCode.Equals(10101))
                    return false;
                else
                    return true;
            }
            finally
            {
                client.Blocking = blockingState;    // 恢复状态
            }
        }

        public bool Start()
        {
            try
            {
                listenSocket.Bind(ipEndPoint);
                listenSocket.Listen(maxConnectNum);

                listenSocket.BeginAccept(new AsyncCallback(acceptCallBack), listenSocket);
                return true;
            }
            catch (Exception e)
            {
                string errorInfor = string.Format("Server start error: {0}", e.Message);
                log.ErrorLog(errorInfor);
                return false;
            }
        }
        private void acceptCallBack(IAsyncResult result)
        {
            int index = newIndex();
            if (index == -1)
            {
                string errorInfor = string.Format("Server connect error: {0}", "Over the max connect number.");
                log.ErrorLog(errorInfor);
                return;
            }
            ConnectState connecter = connecters[index];
            connecter.Init();
            connecter.IsUsed = true;
            var server = result.AsyncState as Socket;
            connecter.CurrentSocket = server.EndAccept(result);

            log.NormalLog(  string.Format("connect information,ID:{0} , IPAdderss:{1}", connecter.ID, connecter.CurrentSocket.RemoteEndPoint));
            //first Receive data
            connecter.AsyncReceive(readSize);
            //继续监听新的连接
            server.BeginAccept(acceptCallBack, listenSocket);
        }

        public bool Stop()
        {
            listenSocket.Disconnect(true);
            foreach (ConnectState connect in connecters)
            {
                connect.disconnect();
            }
            return false;
        }
        private int newIndex()
        {
            if (connecters == null)
                return -1;
            for (int i = 0; i < connecters.Length; i++)
            {
                
                if (connecters[i].IsUsed)
                {
                    //if (connectPoll(connecters[i].CurrentSocket))
                    //{
                        continue;
                    //}
                    //else
                    //{
                    //    connecters[i].IsUsed = false;
                    //    connecters[i].Dispose();
                    //}
                }
                else
                    return i;
            }
            return -1;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    listenSocket.Dispose();
                    foreach (ConnectState connect in connecters)
                        connect.Dispose();
                }
                listenSocket = null;
              
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~APMServer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
        #endregion
    }
}

