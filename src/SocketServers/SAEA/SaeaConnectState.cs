using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Log;
using System.Threading;
using Utillity.Data;

namespace SocketServers.SAEA
{
     public class SaeaConnectState: IConnectState
    {
        #region 字段
        //当前连接的Socket
        private SocketAsyncEventArgs socketArg;
        //private SocketAsyncEventArgs sendsocketArg;
        private string serverName;
        //最大缓存数
        private int buffSize ;
        private BufferMangment bufferPool;

        private int id;
        private ILog log;
        private SaeaConnectStatePool _m_ConnectStatePool;

        private TimeOut timeOut;
        #endregion
        #region 属性
        public SocketAsyncEventArgs SocketArg
        {
            get { return socketArg; }
           private set { socketArg = value; }
        }
        //public SocketAsyncEventArgs SendsocketArg
        //{
        //    get { return sendsocketArg; }
        //    set { sendsocketArg = value; }
        //}

        public SaeaConnectStatePool M_ConnectStatePool
        {
            get { return _m_ConnectStatePool; }
            set { _m_ConnectStatePool = value; }
        }
        public ILog Log
        {
            get { return log; }
            set { log = value; }
        }
        public TimeOut TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        public int ID
        {
            get { return id; }
        }
        public BufferMangment ReadBufferPool
        {
            get { return bufferPool; }
        }
        #endregion
        #region 方法

        public SaeaConnectState(string serverName,ILog log, int id ,TimeOut timeOut,SaeaConnectStatePool pool)
        {
            this.serverName = serverName;
            this.log = log;
            this.id = id;
            this.timeOut = timeOut;
            _m_ConnectStatePool = pool;
        }

        //public SaeaConnectState(ILog log,TimeOut timeOut,int id,int bufferSize )
        //{
        //    this.log = log;
        //    this.timeOut = timeOut;
        //    this.id = id;
        //    this.buffSize = bufferSize;
        //}
        public void Init()
        {
            socketArg = new SocketAsyncEventArgs();
            socketArg.Completed += SocketArg_Completed;
            //sendsocketArg = new SocketAsyncEventArgs();
            //sendsocketArg.Completed += SocketArg_Completed;

            bufferPool = new BufferMangment();
        }

        static readonly object locker = new object();
        private void SocketArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            lock (locker)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        //数据接收成功且不为0，则将收到数据写入缓存池，并触发接收完成事件
                        if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
                        {
                            bufferPool.EnCache(socketArg.Buffer, socketArg.BytesTransferred);

                            ///收到报文记录
                            byte[] logByte = new byte[socketArg.BytesTransferred];
                            Array.Copy(socketArg.Buffer, logByte, logByte.Length);
                            log.DebugLog($"{serverName}:Rx <= {NetConvert.GetHexString(logByte)}");

                            ReadComplete?.Invoke(this);
                        }
                        else
                        {
                            Clear();
                            if (_m_ConnectStatePool != null)
                                _m_ConnectStatePool.Return(this);
                        }
                        break;
                    case SocketAsyncOperation.Send:
                        //数据发送成功且不为0，触发发送完成事件
                        if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
                        {
                            SendComplete?.Invoke(this);
                        }
                        else
                        {
                            Clear();
                            if (_m_ConnectStatePool != null)
                                _m_ConnectStatePool.Return(this);
                        }
                        break;
                    default:
                        Clear();
                        if (_m_ConnectStatePool != null)
                            _m_ConnectStatePool.Return(this);
                        break;
                }
            }
         
        }
        //public event Action<SaeaConnectState> AcceptComplete;
        public event Action<SaeaConnectState> DisconnectEvent;

        public event Action<SaeaConnectState> ReadComplete;
        public event Action<SaeaConnectState> SendComplete;
        /// <summary>
        ///异步接受数据
        /// </summary>
        public void ReceiveAsync(int count)
        {
            var s = ((AsyncUserToken)socketArg.UserToken).AcceptSocket;
            try
            {
                socketArg.SetBuffer(new byte[count], 0, count);
                s.ReceiveAsync(socketArg);
            }
            catch (SocketException ex)
            {
                string error = string.Format("{0} Async Receive data Error：{1}, ID:{2}, IPAdderss:{3}", serverName, ex.Message, id, s.RemoteEndPoint);
                log.ErrorLog(error);
                Clear();
                if (_m_ConnectStatePool != null)
                    _m_ConnectStatePool.Return(this);
            }

        }
        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="buff">数据缓存</param>
        public void SendAsync(byte[] buff)
        {
            var s = ((AsyncUserToken)socketArg.UserToken).AcceptSocket;
            try
            {
                socketArg.SetBuffer(buff, 0, buff.Length);
                s.SendAsync(socketArg);
                //发送报文记录
                log.DebugLog($"{serverName}:Tx => {NetConvert.GetHexString(buff)}");
            }
            catch (SocketException ex)
            {
                string error = string.Format("{0} Async Send data Error：{1}, ID:{2}, IPAdderss:{3}", serverName,ex.Message, id, s.RemoteEndPoint);
                log.ErrorLog(error);
                Clear();
                if (_m_ConnectStatePool != null)
                    _m_ConnectStatePool.Return(this);
            }


        }
        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="buff"></param>
        public int Send(byte[] buff)
        {
            var s = ((AsyncUserToken)socketArg.UserToken).AcceptSocket;
            try
            {
                s.SendTimeout = (int)timeOut.TimeOutSet;
                s.Send(buff, buff.Length, SocketFlags.None);
                return 1;
            }
            catch (SocketException ex)
            {
                string error = string.Format("{0} Sync Send data Error：{1}, ID:{2}, IPAdderss:{3}",serverName, ex.Message, id, s.RemoteEndPoint);
                log.ErrorLog(error);
                Clear();
                if (_m_ConnectStatePool != null)
                    _m_ConnectStatePool.Return(this);
                return -1;
            }
        }
        public void Disconnect()
        {
            var s = ((AsyncUserToken)socketArg.UserToken).AcceptSocket;
            if (s.Connected)
            {
                s.Shutdown(SocketShutdown.Both);
            }
            log.InfoLog(string.Format("{0} Disconnect information,ID:{1} , IPAdderss:{2}",serverName, ID, s.RemoteEndPoint));
            s.Close();
            DisconnectEvent?.Invoke(this);
        }
        public void Clear()
        {
            Disconnect();
            bufferPool.ClearAll();
            ///清除订阅，以防复用时重复订阅
            SendComplete = null;
            ReadComplete = null;
            DisconnectEvent = null;
        }
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(socketArg!=null)
                        socketArg.Dispose();
                }
                socketArg = null;
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~SaeaConnectState()
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
