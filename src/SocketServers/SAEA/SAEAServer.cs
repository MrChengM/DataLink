using DataServer;
using DataServer.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServers.SAEA
{
    public class SAEAServer:ISockteServer
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
        private int readCacheSize;
        private string serverName;
        private bool isRunning = false;

        private SaeaConnectStatePool connectStatePool;
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

        public SaeaConnectStatePool ConnectStatePool
        {
            get { return connectStatePool; }
            private set { connectStatePool = value; }
        }
        #endregion
        #region 方法
        //public event Action<IConnectState> AcceptComplete;
        public event Action<IConnectState> DisconnectEvent;
        public event Action<IConnectState> ReadComplete;
        public event Action<IConnectState> SendComplete;
        public SAEAServer(string serverName,string ipstring, int ipport, ILog log, TimeOut timeOut, int maxConnect,int readsize)
        {
            this.serverName = serverName;
            this.ipString = ipstring;
            this.ipPort = ipport;
            this.log = log;
            this.timeOut = timeOut;
            this.maxConnectNum = maxConnect;
            this.readCacheSize = readsize;
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
                connectStatePool = new SaeaConnectStatePool(serverName,log, timeOut);
                connectStatePool.Init(maxConnectNum);
           
                return true;
            }
            catch (Exception e)
            {
                string errorInfor = string.Format("{0} Server init error: {1}",serverName, e.Message);
                log.ErrorLog(errorInfor);
                return false;
            }

        }

        public bool Start()
        {
            try
            {
                isRunning = true;
                listenSocket.Bind(ipEndPoint);
                listenSocket.Listen(maxConnectNum);
                startAccept();
                return true;
            }
            catch (Exception e)
            {
                string errorInfor = string.Format("{0} Server start error: {1}", serverName, e.Message);
                log.ErrorLog(errorInfor);
                isRunning = false;
                return false;

            }
        }

        private void startAccept()
        {
            if (isRunning)
            {
                var accpetEventArg = new SocketAsyncEventArgs();
                accpetEventArg.Completed += AccpetEventArg_Completed;
                var willRaiseEvent = listenSocket.AcceptAsync(accpetEventArg);
            }
            
            //
            //if (!willRaiseEvent)
            //{
                //ProcessAccept(accpetEventArg);
            //}
                
        }

        private void AccpetEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError==SocketError.Success)
            {
                ProcessAccept(e);
            }
            else
            {
                //log.ErrorLog(string.Format("{0} connect information,ReceiveAsync Start,ID:{1} , IPAdderss:{2}", serverName, connectState.ID, accpetEventArg.AcceptSocket.RemoteEndPoint));

            }
        }

        private void ProcessAccept(SocketAsyncEventArgs accpetEventArg)
        {
            SaeaConnectState connectState = connectStatePool.Get();
            connectState.ReadComplete += ConnectState_ReadComplete;
            connectState.SendComplete += ConnectState_SendComplete;
            connectState.DisconnectEvent += ConnectState_DisconnectEvent;
            SocketAsyncEventArgs readWiteEventArg = connectState.SocketArg;
            readWiteEventArg.SetBuffer(new byte[readCacheSize], 0, readCacheSize);
            readWiteEventArg.UserToken = new AsyncUserToken { AcceptSocket = accpetEventArg.AcceptSocket };
            accpetEventArg.AcceptSocket.ReceiveAsync(readWiteEventArg);
            
            log.InfoLog(string.Format("{0} connect information,ReceiveAsync Start,ID:{1} , IPAdderss:{2}",serverName, connectState.ID, accpetEventArg.AcceptSocket.RemoteEndPoint));

            //Accept Next;
            accpetEventArg.Completed -= AccpetEventArg_Completed;
            startAccept();

        }

        private void ConnectState_DisconnectEvent(SaeaConnectState connectState)
        {
            connectState.ReadComplete -= ConnectState_ReadComplete;
            connectState.SendComplete -= ConnectState_SendComplete;
            connectState.DisconnectEvent -= ConnectState_DisconnectEvent;
            DisconnectEvent?.Invoke(connectState);
        }

        private void ConnectState_SendComplete(SaeaConnectState obj)
        {
            SendComplete?.Invoke(obj);
        }

        private void ConnectState_ReadComplete(SaeaConnectState obj)
        {

            ReadComplete?.Invoke(obj);
        }

        public bool Stop()
        {
            if (listenSocket.Connected)
            {
                listenSocket.Shutdown(SocketShutdown.Both);
            }
            listenSocket.Close();
            isRunning = false;
            ReadComplete = null;
            SendComplete = null;
            DisconnectEvent = null;
            connectStatePool.Clear();
            return true;
           
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
                    connectStatePool.Dispose();
                }
                listenSocket = null;

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~SAEAServer()
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

    internal class AsyncUserToken
    {
        public Socket AcceptSocket
        {
            get;
            set;
        }
        public SaeaConnectState ConnectState
        {
            get;
            set;
        }
    }
}
