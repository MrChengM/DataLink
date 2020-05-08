using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DataServer;
namespace SocketServers
{
     public class APMConnectState:IConnectState
    {
        #region 字段
        //当前连接的Socket
        private Socket currentSocket;
        //最大缓存数
        private  int buffSize ;
        //分配读缓存
        private byte[] readyBuff;
        //缓存使用数量
        private int buffIndex;
        private bool isUsed;
        private BufferMangment bufferPool;

        private int id;
        private ILog log;

        private TimeOut timeOut;
        #endregion

        #region 属性
        public Socket CurrentSocket
        {
            get { return currentSocket; }
            set { currentSocket = value; }
        }

        public byte[] ReadyBuff
        {
            get { return readyBuff; }
            private set { readyBuff = value; }
        }

        public int BuffIndex
        {
            get { return buffIndex; }
            set { buffIndex = value; }
        }

        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }

        public int BuffRemain
        {
            get
            {
                if (buffIndex < buffSize)
                    return buffSize - buffIndex;
                else
                    return 0;
            }
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
        public APMConnectState(ILog log,TimeOut timeOut,int ID,int bufferSize )
        {
            this.log = log;
            this.timeOut = timeOut;
            id = ID;
            this.buffSize = bufferSize;
        }
        public void Init()
        {
            currentSocket = null;
            bufferPool = new BufferMangment(buffSize);
            //buffIndex = 0;
            //readyBuff = new byte[buffSize];
            isUsed = false;
        }
        public event Action<APMConnectState> DisconnectEvent;
        public event Action<APMConnectState,int> ReadComplete;
        public event Action<APMConnectState, int> SendComplete;
        /// <summary>
        /// 异步接受数据反馈
        /// </summary>
        /// <param name="result"></param>
        private void ReadCallback(IAsyncResult result)
        {
            try
            {
                int readCount = currentSocket.EndReceive(result);
                bufferPool.EnCache(readCount);
                if (readCount > 0 && ReadComplete != null)
                {
                    ReadComplete(this, readCount);
                }
                else
                {
                    Disconnect();
                    Dispose();
                }
            }
            catch (SocketException ex)
            {
                string error = string.Format("Read Callback data Error：{0}, ID:{1}, IPAdderss:{2}", ex.Message,id,currentSocket.RemoteEndPoint);
                log.ErrorLog(error);
                Disconnect();
                Dispose();
            }


        }
        /// <summary>
        ///异步发送数据反馈
        /// </summary>
        /// <param name="result"></param>
        private void SendCallback(IAsyncResult result)
        {
            try
            {
                int sendCount = currentSocket.EndReceive(result);
                if (sendCount > 0)
                {
                    SendComplete?.Invoke(this, sendCount);
                }
                else
                {
                    Disconnect();
                    Dispose();
                }
            }
            catch(SocketException ex)
            {
                string error = string.Format("Send Callback data Error：{0}, ID:{1}, IPAdderss:{2}", ex.Message, id, currentSocket.RemoteEndPoint);
                log.ErrorLog(error);
                Disconnect();
                Dispose();
            }
           
        }
        /// <summary>
        ///异步接受数据
        /// </summary>
        /// <param name="count">数量</param>
        public void ReceiveAsync(int count)
        {
            try
            {
                if (currentSocket != null && currentSocket.Connected)
                    currentSocket.BeginReceive(bufferPool.ReadBuffer, 0, count, SocketFlags.None, ReadCallback, currentSocket);
                else
                    isUsed = false;
            }
            catch(SocketException ex)
            {
                string error = string.Format("Async Receive data Error：{0}, ID:{1}, IPAdderss:{2}", ex.Message, id, currentSocket.RemoteEndPoint);
                log.ErrorLog(error);
                Disconnect();
                Dispose();
            }
            
        }
        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="buff">数据缓存</param>
        public void SendAsync(byte[] buff)
        {
            try
            {
                if (currentSocket != null && currentSocket.Connected)
                    currentSocket.BeginSend(buff, 0, buff.Length, SocketFlags.None, SendCallback, currentSocket);
                else
                    isUsed = false;
            }
            catch(SocketException ex)
            {
                string error = string.Format("Async Send data Error：{0}, ID:{1}, IPAdderss:{2}", ex.Message, id, currentSocket.RemoteEndPoint);
                log.ErrorLog(error);
                Disconnect();
                Dispose();
            }
           

        }
        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="buff"></param>
        public void Send(byte[] buff)
        {
            try
            {
                if (currentSocket != null && currentSocket.Connected)
                {
                    currentSocket.SendTimeout = (int)timeOut.TimeOutSet;
                    currentSocket.Send(buff, buff.Length, SocketFlags.None);
                }
                else
                    isUsed = false;
            }
            catch (SocketException ex)
            {
                string error = string.Format("Sync Send data Error：{0}, ID:{1}, IPAdderss:{2}", ex.Message, id, currentSocket.RemoteEndPoint);
                log.ErrorLog(error);
                Disconnect();
                Dispose();
            }
        }
        public void Disconnect()
        {
            if (currentSocket.Connected)
            {
                log.NormalLog(string.Format("Disconnect information,ID:{0} , IPAdderss:{1}", ID, CurrentSocket.RemoteEndPoint));
                currentSocket.Disconnect(true);
                DisconnectEvent?.Invoke(this);
            }
            isUsed = false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(currentSocket!=null)
                    currentSocket.Dispose();
                }
                currentSocket = null;
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~APMConnectState()
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
