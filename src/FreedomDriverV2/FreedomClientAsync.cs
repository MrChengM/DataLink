using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Log;
using DataServer.Points;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Globalization;
using SocketServers;
using Utillity.Data;

namespace FreedomDriversV2
{
    public class FreedomClientAsync : IDisposable
    {
        private CommunicationType _driverType;
        private TimeOut _timeOut;
        private bool _isConnect = false;
        private ILog _log;
        private EthernetSetUp _ethernetSetUp = new EthernetSetUp();
        private Socket _socket;
        private SocketAsyncEventArgs _socketArg;
        private BufferMangment _bufferPool;
        private int _readCacheSize = 65535;
        private string _name;

        private bool _subSocketArgFlag;

        private string _headStr = "<F0>";
        private string _endStr = "<F1>";

        public FreedomClientAsync(string name, EthernetSetUp ethernetSetUp, TimeOut timeOut, ILog log)
        {
            _name = name;
            _ethernetSetUp = ethernetSetUp;
            _timeOut = timeOut;
            _log = log;
            _driverType = CommunicationType.Ethernet;
            _socketArg = new SocketAsyncEventArgs();
            _socketArg.Completed += _socketArg_Completed;
            _bufferPool = new BufferMangment(_readCacheSize);
            _subSocketArgFlag = false;
        }

        public EthernetSetUp EthernetSetUp
        {
            get { return _ethernetSetUp; }
            set { _ethernetSetUp = value; }
        }

        public CommunicationType DriType
        {
            get
            {
                return _driverType;
            }
        }

        public bool IsClose
        {
            get
            {
                return _socket == null || !_socket.Connected;
            }
        }

        public bool IsConnect
        {
            get
            {
                return _isConnect;
            }
        }

        public ILog Log
        {
            get
            {
                return _log;
            }

            set
            {
                _log = value;
            }
        }
        public TimeOut TimeOut
        {
            get
            {
                return _timeOut;
            }

            set
            {
                _timeOut = value;
            }
        }
        /// <summary>
        /// 连接到服务端
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                if (!_isConnect)
                {
                    if (_socket == null)
                        _socket = new Socket(SocketType.Stream, _ethernetSetUp.ProtocolType);
                    if (TimeOut.TimeOutSet < 1000)
                        TimeOut.TimeOutSet = 1000;
                    _socket.SendTimeout = (int)TimeOut.TimeOutSet;
                    _socket.ReceiveTimeout = (int)TimeOut.TimeOutSet;
                    IPAddress ipaddress;
                    if (IPAddress.TryParse(_ethernetSetUp.IPAddress, out ipaddress))
                    {
                        _socket.Connect(ipaddress, _ethernetSetUp.PortNumber);
                        return _isConnect = true;
                    }
                    else
                    {
                        Log.ErrorLog($"{_name}: IP address invalid");
                        return _isConnect = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                DisConnect();
                Log.ErrorLog($"{_name}: FreedomServer Connect Error:" + ex.Message);
                return _isConnect = false;
            }
        }
        #region 创建发送数据

        private byte[] createSendBytes(string[] pointNames,string funCode)
        {
            string resultStr = "";
            foreach (var pointName in pointNames)
            {
                resultStr = string.Concat(resultStr, "<", pointName, ">");
            }
            funCode = string.Concat("<", funCode, ">");
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
       
       
        private byte[] createWriteBytes(ITag[] tags, string funCode)
        {
            string resultStr = "";
            foreach (var tag in tags)
            {
                resultStr = string.Concat(resultStr, "<", tag.Name, ">", "<", tag.Value, ">");
            }
            funCode = string.Concat("<", funCode, ">");
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        private byte[] createReadMetabytes()
        {
            string funcode = "<0F>";
            string resultStr = string.Concat(_headStr, funcode, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        #endregion
        #region read meta data
        public void ReadMetaData()
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createReadMetabytes();
                    var receiveBuffer = new byte[12768];
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("{0}: Freedom Read Meta Data {1} ", _name,"send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("{0}: Freedom Read Meta Data {1} ",_name, "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("{0}: Freedom Read Meta Data {1} ",_name, e.Message));
            }
        }
        #endregion
        #region async read
        public void ReadAsync(string[] pointNames)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(pointNames, "01");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("{0}: Freedom Read {1} ",_name, "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("{0}: Freedom Read {1} ",_name, "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("{0}: Freedom Read {0} ",_name, e.Message));
            }
        }
        #endregion 
        #region write
       
        #region async write
        public void WriteAsync(ITag[] tags)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createWriteBytes(tags, "02");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("{0}: Freedom Write {1} ",_name, "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("{0}: Freedom Write {1} ",_name, "connect is not build!"));
                }
            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("{0}: Freedom Write {1} ",_name, e.Message));
            }
        }
        #endregion
        #endregion
        #region subcribe
        public void Subscribe(string[] pointNames)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(pointNames, "05");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("{0}: Freedom Write {1} ", _name, "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("{0}: Freedom Write {1} ", _name, "connect is not build!"));
                }
            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("{0}: Freedom Write {1} ", _name, e.Message));
            }
        }
        #endregion
        #region 取消订阅
        public void CancelSubscribe(string[] pointNames)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(pointNames, "06");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("{0}: Freedom Write {1} ", _name, "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("{0}: Freedom Write {1} ", _name, "connect is not build!"));
                }
            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("{0}: Freedom Write {1} ", _name, e.Message));
            }
        }
        #endregion
        #region 异步接收数据
        private static readonly object locker = new object();
        //当一个EVentArgs异步线程未结束时，不能重复调用EventArgs；
        private void _socketArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            lock (locker)
            {
                if (e.LastOperation == SocketAsyncOperation.Receive)
                {
                    //数据接收成功且不为0，则将收到数据写入缓存池，并触发接收完成事件
                    if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
                    {
                        _bufferPool.EnCache(e.Buffer, e.BytesTransferred);
                        //收到报文记录
                        byte[] logByte = new byte[e.BytesTransferred];
                        Array.Copy(e.Buffer, logByte, logByte.Length);
                        Log.DebugLog($"{_name}:Rx <= {NetConvert.GetHexString(logByte)}");
                        readDataHanlder();
                    }
                    else
                    {
                        DisConnect();
                        _log.ErrorLog("{0}: socket anyc read error:{1}",_name, e.SocketError.ToString());
                        return;
                    }
                }
                _subSocketArgFlag = false;
                invokSocketArg();
            }
        }
        private void readDataHanlder()
        {
            //将字节数组转换为字符串
            _bufferPool.ConvertToStrCache();
            int headIndex = 0;
            int endIndex = 0;
            while (_bufferPool.StrCache != null && _bufferPool.StrCache !="")
            {
                headIndex = _bufferPool.StrCache.IndexOf(_headStr);
                if (headIndex != -1)
                {
                    endIndex = _bufferPool.StrCache.IndexOf(_endStr, headIndex);
                    if (endIndex != -1)
                    {
                        var length = endIndex - headIndex + 4;
                        var dataStr = _bufferPool.StrCache.Substring(headIndex, length);
                        dataStr = dataStr.Replace("<", "");
                        var dataArray = dataStr.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                        dataProcess(dataArray);
                        if (_bufferPool.StrCache.Length > (endIndex + 4))
                            _bufferPool.StrCache = _bufferPool.StrCache.Substring(endIndex + 4);
                        else
                            _bufferPool.StrCache = "";
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }


        }
        private void dataProcess(string[] source)
        {
            string errorinfo = "";
            string[] datas = new string[source.Length - 3];
            Array.Copy(source, 2, datas, 0, datas.Length);
            switch (source[1])
            {
                case "11":
                    goto lable01;
                case "21":
                    errorinfo = "Freedom read error: ";
                    goto lable10;
                case "12":
                    AsyncWriteEvent?.Invoke(true, datas);
                    return;
                case "22":
                    AsyncWriteEvent?.Invoke(false, datas);
                    errorinfo = "Freedom write error: ";
                    goto lable10;
                case "15":
                    goto lable01;
                case "25":
                    errorinfo = "Freedom subscribe error: ";
                    goto lable10;
                case "16":
                    AsyncCancelSubEvent?.Invoke(true);
                    return;
                case "26":
                    AsyncCancelSubEvent?.Invoke(false);
                    errorinfo = "Freedom Cancelsubscribe error: ";
                    goto lable10;
                case "1F":
                    List<PointMetadata> result = new List<PointMetadata>();
                    for (int i = 0; i < (datas.Length / 4); i++)
                    {
                        bool isVirual;
                        int length;
                        if(bool.TryParse(datas[4*i+2],out isVirual)&&int.TryParse(datas[4 * i + 3],out length))
                        {
                            Enum.TryParse(datas[4 * i + 1], out DataType type);
                            PointMetadata metaData = new PointMetadata(datas[4 * i],type, length, isVirual);
                            result.Add(metaData);
                        }
                    }
                    AsyncReadMetaData?.Invoke(result);
                    return;
                case "2F":
                    errorinfo = "Freedom read Meta Data error: ";
                    goto lable10;
                default:
                    return;
            }
        lable01:
            List<ITag> asynResults = new List<ITag>();
            for (int i = 0; i < (datas.Length / 4); i++)
            {
                Enum.TryParse(datas[4 * i + 1], out DataType type);
                Enum.TryParse(datas[4 * i + 3], out QUALITIES qulity);
                asynResults.Add(new Tag { Name = datas[4 * i], Type = type, Value = datas[4 * i + 2], Quality = qulity });
            }
            AsyncReadOrSubsEvent?.Invoke(asynResults);
            return;
        lable10:
            foreach (string s in datas)
            {
                errorinfo = string.Concat(errorinfo, s);
            }
            _log.ErrorLog(errorinfo);

        }
        public event Action<List<ITag>> AsyncReadOrSubsEvent;
        public event Action<List<PointMetadata>> AsyncReadMetaData;
        public event Action<bool, string[]> AsyncWriteEvent;
        public event Action<bool> AsyncCancelSubEvent;
        public event Action<FreedomClientAsync> DisconnectEvent;

        private static readonly object locker1 = new object();
        /// <summary>
        /// 触发Socket异步事件
        /// </summary>
        private void invokSocketArg()
        {
            lock (locker1)
            {
                if (!_subSocketArgFlag)
                {
                    if (_socketArg == null)
                    {
                        _socketArg = new SocketAsyncEventArgs();
                        _socketArg.Completed += _socketArg_Completed;
                    }
                    _socketArg.SetBuffer(new byte[_readCacheSize], 0, _readCacheSize);
                    if (_socket != null)
                    {
                        _socket.ReceiveAsync(_socketArg);
                        _subSocketArgFlag = true;
                    }
                }
            }
            
        }
        #endregion
        public bool DisConnect()
        {
            if (IsConnect)
            {
                _socket?.Close();
            }
            _socketArg?.Dispose();
            _socket?.Dispose();
            _socket = null;
            _socketArg = null;
            _isConnect = false;
            _subSocketArgFlag = false;
            DisconnectEvent?.Invoke(this);
            return true;
        }

        public void Dispose()
        {
            _socketArg?.Dispose();
            _socket?.Dispose();
            _socket = null;
            _socketArg = null;
        }
    }
}
