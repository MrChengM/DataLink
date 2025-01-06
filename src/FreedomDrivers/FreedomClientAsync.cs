using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Points;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Globalization;
using SocketServers;
using Utillity.Data;

namespace FreedomDrivers
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

        public FreedomClientAsync(EthernetSetUp ethernetSetUp, TimeOut timeOut, ILog log)
        {
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
                        Log.ErrorLog("IP address invalid");
                        return _isConnect = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                DisConnect();
                Log.ErrorLog("FreedomServer Connect Error:" + ex.Message);
                return _isConnect = false;
            }
        }
        #region 创建发送数据

        private byte[] createSendBytes(List<Address> addressList,string funCode)
        {
            string resultStr = "";
            foreach (var address in addressList)
            {
                var index = address.Index;
                var name = string.Concat(address.Name, "[", index, "]");
                var type = address.Type;
                resultStr = string.Concat(resultStr, "<", name, ">", "<", type, ">");
            }
            funCode = string.Concat("<", funCode, ">");
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        private byte[] createSendBytes<T>(List<PointGroup<T>> pointStructList,string funCode)
        {
            string resultStr = "";
            foreach (var pointStruct in pointStructList)
            {
                var point = pointStruct.Point;
                var index = pointStruct.Index;
                var name = string.Concat(point.Name, "[", index, "]");
                DataType type = DataType.Int;
                if (typeof(T) == typeof(bool))
                {
                    type = DataType.Bool;
                }
                else if (typeof(T) == typeof(byte))
                {
                    type = DataType.Byte;
                }
                else if (typeof(T) == typeof(short))
                {
                    type = DataType.Short;
                }
                else if (typeof(T) == typeof(ushort))
                {
                    type = DataType.UShort;

                }
                else if (typeof(T) == typeof(int))
                {
                    type = DataType.Int;

                }
                else if (typeof(T) == typeof(uint))
                {
                    type = DataType.UInt;

                }
                else if (typeof(T) == typeof(float))
                {
                    type = DataType.Float;
                }
                else if (typeof(T) == typeof(string))
                {
                    type = DataType.String;
                }
                resultStr = string.Concat(resultStr, "<", name, ">", "<", type, ">");
            }
            funCode = string.Concat("<", funCode, ">");
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        private byte[] createSendWirteBytes<T>(List<PointGroup<T>> pointStructList,string funCode)
        {
            string resultStr = "";
            foreach (var pointStruct in pointStructList)
            {
                var point = pointStruct.Point;
                var index = pointStruct.Index;
                var name = string.Concat(point.Name, "[", index, "]");
                DataType type = DataType.Int;
                if (typeof(T) == typeof(bool))
                {
                    type = DataType.Bool;
                }
                else if (typeof(T) == typeof(byte))
                {
                    type = DataType.Byte;
                }
                else if (typeof(T) == typeof(short))
                {
                    type = DataType.Short;
                }
                else if (typeof(T) == typeof(ushort))
                {
                    type = DataType.UShort;

                }
                else if (typeof(T) == typeof(int))
                {
                    type = DataType.Int;

                }
                else if (typeof(T) == typeof(uint))
                {
                    type = DataType.UInt;

                }
                else if (typeof(T) == typeof(float))
                {
                    type = DataType.Float;
                }
                else if (typeof(T) == typeof(string))
                {
                    type = DataType.String;
                }
                var value = point.GetValue(index);
                resultStr = string.Concat(resultStr, "<", name, ">", "<", type, ">", "<", value, ">");
            }
            funCode = string.Concat("<", funCode, ">");
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        private byte[] createSendWriteBytes(List<WriteData> writeDataList, string funCode)
        {
            string resultStr = "";
            foreach (var writeData in writeDataList)
            {
                var index = writeData.Index;
                var name = string.Concat(writeData.Name, "[", index, "]");
                var type = writeData.Type;
                var value = writeData.Value;
                resultStr = string.Concat(resultStr, "<", name, ">", "<", type, ">", "<", value, ">");
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
                        Log.ErrorLog(string.Format("Freedom Read Meta Data{0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read Meta Data{0} ", "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Read Meta Data{0} ", e.Message));
            }
        }
        #endregion
        #region async read
        public void ReadAsync<T>(List<PointGroup<T>> pointGroups)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(pointGroups,"01");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("Freedom Read {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Read {0} ", e.Message));
            }
        }
        public void ReadAsync(List<Address> addressList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(addressList,"01");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("Freedom Read {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Read {0} ", e.Message));
            }
        }
        #endregion 
        #region write
       
        #region async write
        public void WriteAsync<T>(List<PointGroup<T>> pointGroupList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendWirteBytes(pointGroupList,"02");
                    var receiveBuffer = new byte[12768];
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("Freedom Write {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Write {0} ", "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Write {0} ", e.Message));
            }
        }
        public void WriteAsync(List<WriteData> writeDataList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendWriteBytes(writeDataList,"02");
                    var receiveBuffer = new byte[12768];
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("Freedom Write {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Write {0} ", "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Write {0} ", e.Message));
            }
        }
        #endregion
        #endregion
        #region subcribe
       
        /// <summary>
        /// 订阅数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pointGroupList"></param>
        /// <returns></returns>
        public void Subscribe<T>(List<PointGroup<T>> pointGroupList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(pointGroupList,"05");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", e.Message));
            }
        }
        public void Subscribe(List<Address> addressList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(addressList,"05");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "connect is not build!"));
                }
            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", e.Message));
            }
        }
        #endregion
        #region 取消订阅
        public void CancelSubscribe<T>(List<PointGroup<T>> pointGroupList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(pointGroupList,"06");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "connect is not build!"));
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", e.Message));
            }
        }
        public void CancelSubscribe(List<Address> addressList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createSendBytes(addressList,"06");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        invokSocketArg();
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", "connect is not build!"));
                }
            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("FreedomAsync subscribe {0} ", e.Message));
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
                        _log.ErrorLog("socket anyc read error:{0}", e.SocketError.ToString());
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
                    AsyncWriteEvent?.Invoke(true);
                    return;
                case "22":
                    AsyncWriteEvent?.Invoke(false);
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
            List<AsyncResult> asynResults = new List<AsyncResult>();
            for (int i = 0; i < (datas.Length / 4); i++)
            {
                var nameGroup = datas[4 * i].Split('[');
                string index = "0";
                var name = nameGroup[0];

                if (nameGroup.Length >= 2)
                {
                    index = nameGroup[1].Replace("]", "");
                }
                asynResults.Add(new AsyncResult { Name = name, Index = index, Type = datas[4 * i + 1], Value = datas[4 * i + 2], Quality = datas[4 * i + 3] });
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
        public event Action<List<AsyncResult>> AsyncReadOrSubsEvent;
        public event Action<List<PointMetadata>> AsyncReadMetaData;
        public event Action<bool> AsyncWriteEvent;
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
    public class Address
    {
        public string Name;
        public string Index;
        public string Type;
    }
    public class WriteData :Address
    {
        public string Value;
    }
}
