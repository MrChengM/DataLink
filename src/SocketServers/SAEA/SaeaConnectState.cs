using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using System.Threading;

namespace SocketServers.SAEA
{
     public class SaeaConnectState: IConnectState
    {
        #region 字段
        //当前连接的Socket
        private SocketAsyncEventArgs socketArg;
        //private SocketAsyncEventArgs sendsocketArg;

        //最大缓存数
        private int buffSize ;
        private BufferMangment bufferPool;

        private int id;
        private ILog log;
        private SaeaConnectStatePool _m_ConnectStatePool;

        private TimeOut timeOut;
        #endregion
        #region 属性
        public SocketAsyncEventArgs ReadSocketArg
        {
            get { return socketArg; }
            set { socketArg = value; }
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
        public BufferMangment BufferPool
        {
            get { return bufferPool; }
        }
        #endregion
        #region 方法

        public SaeaConnectState(ILog log, int id ,TimeOut timeOut,SaeaConnectStatePool pool)
        {
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
                            log.ByteSteamLog(ActionType.RECEIVE, logByte);

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
                string error = string.Format("Async Receive data Error：{0}, ID:{1}, IPAdderss:{2}", ex.Message, id, s.RemoteEndPoint);
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
                log.ByteSteamLog(ActionType.SEND, buff);
            }
            catch (SocketException ex)
            {
                string error = string.Format("Async Send data Error：{0}, ID:{1}, IPAdderss:{2}", ex.Message, id, s.RemoteEndPoint);
                log.ErrorLog(error);
                Clear();
                if (_m_ConnectStatePool != null)
                    _m_ConnectStatePool.Return(this);
            }


        }
        public void Disconnect()
        {
            var s = ((AsyncUserToken)socketArg.UserToken).AcceptSocket;
            if (s.Connected)
            {
                log.NormalLog(string.Format("Disconnect information,ID:{0} , IPAdderss:{1}", ID, s.RemoteEndPoint));
                s.Shutdown(SocketShutdown.Both);
                s.Close();
                DisconnectEvent?.Invoke(this);
            }
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
