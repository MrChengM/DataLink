using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using SocketServers;
using SocketServers.SAEA;
using System.Globalization;
using DataServer.Points;
using System.ComponentModel;
using System.Threading;
using Utillity.Data;
using DataServer.Config;

namespace FreedomDriversV2
{
    public class FreedomServer : IServerDrivers
    {
        private ILog _log;
        private TimeOut _timeOut;
        private string _ipString = "127.0.0.1";
        private int _port = 9527;
        private ISockteServer _socketServer;
        private SocketServerType _socketServerType;
        private List<SubscribeItem> _subscribeGroup;
        private string _serverName;
        private int _maxConnect = 100;
        private ComPhyLayerSetting _phyLayerSetting;

        private IPointMapping _pointMapping;
        ///// <summary>
        ///// 报头长度+后续报文数量长度6
        ///// </summary>
        //private const int _headLength = 10;
        /// <summary>
        /// 每次读取数量
        /// </summary>
        private const int _readCacheSize = 65535;
        private bool _isRunning;
        private const string _headStr = "<F0>";//报头
        private const string _endStr = "<F1>";//报尾

        public bool IsRunning
        {
            get
            {
               return _isRunning;
            }
        }
        //public string IpString
        //{
        //    get { return _ipString; }
        //    set { _ipString = value; }
        //}
        //public int Port
        //{
        //    get { return _port; }
        //    set { _port = value; }
        //}
        public string ServerName { get => _serverName; set => _serverName = value; }
        public ComPhyLayerSetting PhyLayerSetting { get => _phyLayerSetting; set => _phyLayerSetting = value; }
        public TimeOut TimeOut { get => _timeOut; set => _timeOut = value; }
        public int MaxConnect { get => _maxConnect; set => _maxConnect = value; }
        public ILog Log { get =>_log; set => _log=value; }
        public IPointMapping PointMapping { get => _pointMapping; set => _pointMapping = value; }

        /// <summary>
        /// 自定义服务实例化
        /// </summary>
        /// <param name="log">消息记录</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="type">socket服务格式：Apm，SAEA等</param>
        public FreedomServer(string severName, EthernetSetUp setUp, TimeOut timeOut, ILog log, SocketServerType type = SocketServerType.SaeaServer)
        {
            _serverName = severName;
            _log = log;
            _timeOut = timeOut;
            _socketServerType = type;
            _ipString = setUp.IPAddress;
            _port = setUp.PortNumber;
            _subscribeGroup = new List<SubscribeItem>();
        }
        public FreedomServer(ServerItemConfig config, ILog log, SocketServerType type = SocketServerType.SaeaServer)
        {
            _serverName = config.Name;
            _ipString = config.ComunicationSetUp.EthernetSet.LocalNetworkAdpt;
            _port = config.ComunicationSetUp.EthernetSet.PortNumber;
            _timeOut = new TimeOut() { TimeOutSet = config.TimeOut };
            _maxConnect = config.MaxConnect;
            _log = log;
            _socketServerType = type;

        }
        public bool Init()
        {
            _subscribeGroup = new List<SubscribeItem>();

            var factory = new SocketServerFactroy( _serverName,_ipString, _port, _log, _timeOut,_readCacheSize, _maxConnect);
            _socketServer = factory.CreateInstance(_socketServerType);
            if (_socketServer.Init())
            {
                _socketServer.ReadComplete += recivceDataHanding;
                _socketServer.SendComplete += sendComplete;
                _socketServer.DisconnectEvent += disconnectEvent;
                return true;
            }
            return false;
        }

        private void disconnectEvent(IConnectState connecter)
        {
           var subscribeItem= _subscribeGroup.Find(s => s.ConnectState == connecter);
            subscribeItem?.RemoveAll();
            _subscribeGroup.Remove(subscribeItem);
        }

        private void sendComplete(IConnectState connecter)
        {
            if (connecter.ReadBufferPool.StrCache==null||connecter.ReadBufferPool.StrCache.Length==0)
            {
                connecter.ReceiveAsync(_readCacheSize);
            }
            else
            {
                recivceDataHanding(connecter);//如果bufferPool主队列缓存不为空，继续处理数据。
            }
        }

        private readonly static object locker = new object();
        private void recivceDataHanding(IConnectState connecter)
        {
            lock (locker)
            {
                var bufferPool = connecter.ReadBufferPool;
                //将字节数组转换为字符串
                bufferPool.ConvertToStrCache();
                var headIndex = bufferPool.StrCache.IndexOf(_headStr);
                if (headIndex != -1)
                {
                    var endIndex = bufferPool.StrCache.IndexOf(_endStr, headIndex);
                    if (endIndex != -1)
                    {
                        var length = endIndex - headIndex + 4;
                        var bodyStr = bufferPool.StrCache.Substring(headIndex, length);
                        bodyStr = bodyStr.Replace("<", "");
                        var bodyArray = bodyStr.Split(new char[] { '>' },StringSplitOptions.RemoveEmptyEntries);
                        var sentCache= bufferRely(bodyArray,connecter);
                        if (sentCache != null)
                        {
                            ////发送次数
                            //int sendCount =(int) Math.Ceiling((double)sentCache.Length / 60000);
                            ////发送缓存
                            //byte[] sendBuffer = new byte[60000];
                            //for(int i = 0; i < sendCount; i++)
                            //{
                            //    connecter.Send(sentCache);
                            //}
                            connecter.SendAsync(sentCache);
                        }
                        else
                        {
                            connecter.ReceiveAsync(_readCacheSize);
                        }
                        if (bufferPool.StrCache.Length >= endIndex + 4)
                            bufferPool.StrCache = bufferPool.StrCache.Substring(endIndex + 4);
                        else
                            bufferPool.StrCache = "";
                    }
                    else
                    {
                        connecter.ReceiveAsync(_readCacheSize);
                    }
                }
                else
                {
                    connecter.ReceiveAsync(_readCacheSize);
                }
            }
        }
        /// <summary>
        /// ASCII码字节数组转换成String[]
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string[] aSCToStrings(byte[] source)
        {
            string[] result = null;
            ASCIIEncoding enconding = new ASCIIEncoding();
            string str =enconding.GetString(source).Replace("<","");
            result = str.Split('>');
            return result;
        }

        private byte[] bufferRely(string[] source,IConnectState connecter)
        {
            byte[] result = null;
            if (source.Length >= 3)
            {
                string[] data = new string[source.Length - 3];
                byte funCode;
                if (byte.TryParse(source[1], NumberStyles.HexNumber, null , out funCode))
                {
                    Array.Copy(source, 2, data, 0, data.Length);
                    switch (funCode)
                    {
                        case 0x01:
                            result = readCodeRely(data);
                            break;
                        case 0x02:
                            result = writeCodeRely(data);
                            break;
                        case 0x05:
                            result = subscribeCodeRely(data, connecter);
                            break;
                        case 0x06:
                            result = cancelSubCodeRely(data, connecter);
                            break;
                        case 0x0F:
                            result = metaDataRely(connecter);
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 返回读数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="funCode"></param>
        /// <returns></returns>
        private byte[] readCodeRely(string[] source)
        {
            byte[] result = null;
            List<string> strList = new List<string>();
            string errorInfo = null;
            for (int i = 0; i < source.Length; i++)
            {
                var pointName = source[i];
                var pointNameGroup = StringHandler.SplitEndWith(pointName);
                int.TryParse(pointNameGroup[1], out int index);
                var pointMeta = _pointMapping.GetPointMetaData(pointNameGroup[0]);
                if (pointMeta != null)
                {
                    PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameGroup[0], Index = index, Type = pointMeta.ValueType };
                    strList.AddRange(readFeedBack(nameGroup));
                }
                else
                {
                    errorInfo = string.Concat("Not found the point name:", pointName);
                    break;
                }
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            if (errorInfo != null)
            {
                string funCodeStr ="<21>";
                errorInfo = string.Concat("<", errorInfo, ">");
                result =encoding.GetBytes(string.Concat(_headStr, funCodeStr,errorInfo, _endStr));
            }
            else
            {
                string resultStr="";
                foreach(var s in strList)
                {
                    resultStr = string.Concat(resultStr,"<", s, ">");
                }
                string funCodeStr = "<11>";
                result = encoding.GetBytes(string.Concat(_headStr, funCodeStr,resultStr, _endStr));
            }
            return result;
        }

        private byte[] writeCodeRely(string[] source)
        {
            byte[] result = null;
            List<string> strList = new List<string>();
            List<string> errorInfoList =new List<string>();
            List<string> pointNames = new List<string>();
            if (source.Length % 2 == 0)
            {
                for (int i = 0; i < source.Length; i += 2)
                {
                    var pointName = source[i];
                    pointNames.Add(pointName);
                    var pointValue = source[i + 1];
                    var pointNameGroup = StringHandler.SplitEndWith(pointName);
                    int index = 0;
                    if (pointNameGroup.Length > 1)
                    {
                        int.TryParse(pointNameGroup[1], out index);
                    }
                    var pointMeta = _pointMapping.GetPointMetaData(pointNameGroup[0]);

                    if (pointMeta != null)
                    {
                        PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameGroup[0], Index = index, Type = pointMeta.ValueType };
                        if (writeMapping(nameGroup, pointValue) == -1)
                        {
                            errorInfoList.Add(string.Concat("the point name:", pointName, " type : ", pointMeta.ValueType, " write error"));
                        }

                    }
                    else
                    {
                        errorInfoList.Add(string.Concat("Not found the point name:", pointName));
                    }
                }
            }
            else
            {
                errorInfoList.Add("steam length error");
            }

            ASCIIEncoding encoding = new ASCIIEncoding();
            if (errorInfoList.Count !=0)
            {
                string funCodeStr = "<22>";
                string errorInfo="";
                foreach(var s in errorInfoList)
                {
                    errorInfo = string.Concat(errorInfo, "<", s, ">");
                }

                result = encoding.GetBytes(string.Concat(_headStr, funCodeStr, errorInfo, _endStr));
            }
            else
            {
                string funCodeStr = "<12>";
                string names = "";

                foreach (var n in pointNames)
                {
                    names = string.Concat(names, "<", n, ">");
                }
                result = encoding.GetBytes(string.Concat(_headStr, funCodeStr, names, _endStr));
            }
            return result;
        }
        private byte[] subscribeCodeRely(string[] source, IConnectState connecter)
        {
            byte[] result = null;
            List<string> errorInfoList = new List<string>();
            string errorInfo = null;
            List<string> strList = new List<string>();
            ASCIIEncoding encoding = new ASCIIEncoding();

            for (int i = 0; i < source.Length; i++)
            {
                var pointName = source[i];
                var pointNameGroup = StringHandler.SplitEndWith(pointName);
                int index = 0;
                if (pointNameGroup.Length > 1)
                {
                    int.TryParse(pointNameGroup[1], out index);
                }
                var pointMeta = _pointMapping.GetPointMetaData(pointNameGroup[0]);

                if (pointMeta != null)
                {


                    PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameGroup[0], Index = index, Type = pointMeta.ValueType };
                    //反馈第一次订阅值
                    strList.AddRange(readFeedBack(nameGroup));

                    //添加订阅
                    SubscribeItem item;
                    if (_subscribeGroup.Exists(s => s.ConnectState.Equals(connecter)))
                    {
                        item = _subscribeGroup.Find(s => s.ConnectState.Equals(connecter));
                    }
                    else
                    {
                        item = new SubscribeItem(connecter,_pointMapping, _log);
                        _subscribeGroup.Add(item);
                    }
                    item.Add(nameGroup);
                }
                else
                {
                    errorInfo = string.Concat("Not found the point name:", pointName);
                    errorInfoList.Add(errorInfo);
                }
            }

            if (errorInfoList.Count != 0)
            {
                string funCodeStr = "<25>";
                errorInfo = "";
                foreach (var s in errorInfoList)
                {
                    errorInfo = string.Concat(errorInfo, "<", s, ">");
                }
                errorInfo = string.Concat(_headStr, funCodeStr, "<", errorInfo, ">", _endStr);
                connecter.Send(encoding.GetBytes(errorInfo));
            }
            if (strList.Count != 0)
            {
                string resultStr = "";
                foreach (var s in strList)
                {
                    resultStr = string.Concat(resultStr, "<", s, ">");
                }
                string funCodeStr = "<15>";
                result = encoding.GetBytes(string.Concat(_headStr, funCodeStr, resultStr, _endStr));
            }
            return result;
        }
        /// <summary>
        /// 返回取消订阅数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="connecter"></param>
        /// <returns></returns>
        private byte[] cancelSubCodeRely(string[] source,IConnectState connecter)
        {
            byte[] result = null;
            List<string> errorInfoList = new List<string>();
            string errorInfo = null;
            ASCIIEncoding encoding = new ASCIIEncoding();
            for (int i = 0; i < source.Length; i++)
            {
                var pointName = source[i];
                var pointNameArrary = pointName.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                byte index = 0;
                if (pointNameArrary.Length >= 2)
                {
                    byte.TryParse(pointNameArrary[1].Replace("]", ""), out index);
                }
                var pointMeta = _pointMapping.GetPointMetaData(pointNameArrary[0]);
                if (pointMeta != null)
                {
                    PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameArrary[0], Index = index, Type = pointMeta.ValueType };
                    //取消订阅
                    SubscribeItem item;
                    if (_subscribeGroup.Exists(s => s.ConnectState.Equals(connecter)))
                    {
                        item = _subscribeGroup.Find(s => s.ConnectState.Equals(connecter));
                        if (!item.Remove(pointName))
                        {
                            errorInfo = string.Concat("the point name:", pointName, " not Subscrible");
                        }
                    }
                    else
                    {
                        errorInfo = string.Concat("the point name:", pointName,  " not exist in Subscirbe List");
                        errorInfoList.Add(errorInfo);
                    }
                }
                else
                {
                    errorInfo = string.Concat("Not found the point name:", pointName);
                    errorInfoList.Add(errorInfo);
                }
            }
            if (errorInfoList.Count != 0)
            {
                string funCodeStr = "<26>";
                errorInfo = "";
                foreach (var s in errorInfoList)
                {
                    errorInfo = string.Concat(errorInfo, "<", s, ">");
                }
                errorInfo = string.Concat(_headStr, funCodeStr, "<", errorInfo, ">", _endStr);
                connecter.Send(encoding.GetBytes(errorInfo));
            }
            else
            {
                string funCodeStr = "<16>";
                result = encoding.GetBytes(string.Concat(_headStr, funCodeStr, _endStr));
            }
            return result;
        }
        /// <summary>
        /// 返回所有点的元数据
        /// </summary>
        /// <returns></returns>
        private byte[] metaDataRely(IConnectState connecter)
        {
            string funCodeStr = "<1F>";

            byte[] result = null;
            List<string> strList = new List<string>();
            ASCIIEncoding encoding = new ASCIIEncoding();
            var pointMetas = _pointMapping.GetPointMetadatas();
            foreach(var metaData in pointMetas)
            {
                strList.Add(metaData.Value.Name);
                strList.Add(metaData.Value.ValueType.ToString());
                strList.Add(metaData.Value.IsVirtual.ToString());
                strList.Add(metaData.Value.Length.ToString());
            }
            if (strList.Count != 0)
            {
                string resultStr;
                while (strList.Count >= 5000)
                {
                    resultStr = "";
                    for (int i = 0; i < 5000; i++)
                    {
                        resultStr = string.Concat(resultStr, "<", strList[i], ">");
                    }
                    strList.RemoveRange(0, 5000);

                    if(connecter.Send(encoding.GetBytes(string.Concat(_headStr, funCodeStr, resultStr, _endStr)))!=1)
                        return null;
                }
                resultStr = "";
                foreach (var s in strList)
                {
                    resultStr = string.Concat(resultStr, "<", s, ">");
                }
                if(resultStr!="")
                    connecter.Send( encoding.GetBytes(string.Concat(_headStr, funCodeStr, resultStr, _endStr)));
            }
            else
            {
                funCodeStr = "<2F>";
                result = encoding.GetBytes(string.Concat(_headStr, funCodeStr,"<Point Meta data is Null>" , _endStr));
            }
            return result;
        }
        /// <summary>
        /// 根据类型不同，反馈不同类型数据
        /// </summary>
        /// <param name="nameGroup"></param>
        /// <returns></returns>
        private List<string> readFeedBack(PointNameGroup nameGroup)
        {
            string pointName = nameGroup.PointName;
            int index = nameGroup.Index;
            var type = nameGroup.Type;
            List<string> result = new List<string>();
            if (type == DataType.Bool)
            {
                var point = _pointMapping.GetBoolPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName,"[",index,"]"));
                result.Add(type.ToString());
                result.Add(value.ToString());
                result.Add(quality.ToString());

            }
            else if (type == DataType.Byte)
            {
                var point = _pointMapping.GetBytePoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type.ToString());
                result.Add(value.ToString());
                result.Add(quality.ToString());

            }
            else if (type == DataType.UShort)
            {
                var point = _pointMapping.GetUShortPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type.ToString());
                result.Add(value.ToString());
                result.Add(quality.ToString());

            }
            else if (type == DataType.Short)
            {
                var point = _pointMapping.GetShortPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type.ToString());
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataType.UInt)
            {
                var point = _pointMapping.GetUIntPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type.ToString());
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataType.Int)
            {
                var point = _pointMapping.GetIntPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type.ToString());
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataType.Float)
            {
                var point = _pointMapping.GetFloatPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type.ToString());
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataType.String)
            {
                var point = _pointMapping.GetStringPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type.ToString());
                result.Add(value);
                result.Add(quality.ToString());
            }
            return result;
        }
        private int writeMapping(PointNameGroup nameGroup,string value)
        {
            string pointName = nameGroup.PointName;
            int index = nameGroup.Index;
            var type = nameGroup.Type;
            List<string> result = new List<string>();
            if (type == DataType.Bool)
            {
                bool temp;
                if(bool.TryParse(value,out temp))
                {
                    var point =_pointMapping.GetBoolPoint(pointName);
                    return point.SetValue(temp, index) ? 1 : -1;
                }
                else
                {
                    return -1;
                }


            }
            else if (type == DataType.Byte)
            {
                byte temp;
                if (byte.TryParse(value, out temp))
                {
                    var point = _pointMapping.GetBytePoint(pointName);
                    return point.SetValue(temp, index) ? 1 : -1;
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataType.UShort)
            {
                ushort temp;
                if (ushort.TryParse(value, out temp))
                {
                    var point = _pointMapping.GetUShortPoint(pointName);
                    return point.SetValue(temp, index) ? 1 : -1;
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataType.Short)
            {
                short temp;
                if (short.TryParse(value, out temp))
                {
                    var point = _pointMapping.GetShortPoint(pointName);
                    return point.SetValue(temp, index) ? 1 : -1;
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataType.UInt)
            {
                uint temp;
                if (uint.TryParse(value, out temp))
                {
                    var point = _pointMapping.GetUIntPoint(pointName);
                    return point.SetValue(temp, index) ? 1 : -1;
                }
                else
                {
                    return -1;
                }
            }
            else if (type ==DataType.Int)
            {
                int temp;
                if (int.TryParse(value, out temp))
                {
                    var point = _pointMapping.GetIntPoint(pointName);
                    return point.SetValue(temp, index) ? 1 : -1;
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataType.Float)
            {
                float temp;
                if (float.TryParse(value, out temp))
                {
                    var point = _pointMapping.GetFloatPoint(pointName);
                    return point.SetValue(temp, index) ? 1 : -1;
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }
        public bool Start()
        {
            return _isRunning = _socketServer.Start();
        }

        public bool Stop()
        {
            if (_isRunning)
            {

                _isRunning = false;
                return _socketServer.Stop();
            }
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
                    _socketServer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ModbusTCPServer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public void RegisterMapping(Dictionary<string,TagBindingConfig> tagBindings)
        {
        }
        #endregion
    }
    /// <summary>
    ///  订阅子项
    /// </summary>
    internal class SubscribeItem
    {
        private IConnectState _connectState;
        private List<PointNameGroup> _pointNames;
        //private PointMeDataList pointIndex = PointMeDataList.GetInstance();
        private IPointMapping _pointMapping;
        private ILog _log;
        private string _headStr = "<F0>";
        private string _endStr = "<F1>";
        public SubscribeItem(IConnectState connectState,IPointMapping pointMapping, ILog log)
        {
            _pointMapping = pointMapping;
            _connectState = connectState;
            _pointNames = new List<PointNameGroup>();
            _log = log;
        }
        public IConnectState ConnectState
        {
            get { return _connectState; }
        }
        public bool Add(string pointName)
        {
            var pointNameStruct = pointName.Split('[');
            byte index = 0;
            if (pointNameStruct.Length >= 2)
            {
                byte.TryParse(pointNameStruct[1].Remove(']'), out index);
            }
            string type;
            var pointMeta = _pointMapping.GetPointMetaData(pointNameStruct[0]);
            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameStruct[0], Index = index,Type= pointMeta.ValueType };
            return Add(nameGroup);
        }
        public bool Add(PointNameGroup nameGroup)
        {
            if (_pointNames.Exists((s) => s == nameGroup))
            {
                return false;
            }
            var type = nameGroup.Type;

            if (!_pointNames.Exists(s => s.PointName == nameGroup.PointName))
            {
                if (type == DataType.Bool)
                {
                    var point = _pointMapping.GetBoolPoint(nameGroup.PointName);
                    point.UpdataEvent += sendBoolData;

                }
                else if (type == DataType.Byte)
                {
                    var point =_pointMapping.GetBytePoint(nameGroup.PointName);
                    point.UpdataEvent += sendByteData;


                }
                else if (type == DataType.Short)
                {
                    var point = _pointMapping.GetShortPoint(nameGroup.PointName);
                    point.UpdataEvent += sendShortData;

                }
                else if (type == DataType.UShort)
                {
                    var point = _pointMapping.GetUShortPoint(nameGroup.PointName);
                    point.UpdataEvent += sendUshortData;

                }
                else if (type == DataType.Int)
                {
                    var point = _pointMapping.GetIntPoint(nameGroup.PointName);
                    point.UpdataEvent += sendIntData;

                }
                else if (type == DataType.UInt)
                {
                    var point = _pointMapping.GetUIntPoint(nameGroup.PointName);
                    point.UpdataEvent += sendUintData;

                }
                else if (type ==DataType.Float)
                {
                    var point = _pointMapping.GetFloatPoint(nameGroup.PointName);
                    point.UpdataEvent += sendFloatData;
                }
                else if (type == DataType.String)
                {
                    var point = _pointMapping.GetStringPoint(nameGroup.PointName);
                    point.UpdataEvent += sendStringData;
                }
            }
            _pointNames.Add(nameGroup);
            return true;

        }

        public bool Remove(string pointName)
        {
            var pointNameStruct = pointName.Split('[');
            byte index = 0;
            if (pointNameStruct.Length >= 2)
            {
                byte.TryParse(pointNameStruct[1].Replace("]",""), out index);
            }
            string type;
            var pointMeta = _pointMapping.GetPointMetaData(pointNameStruct[0]);
            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameStruct[0], Index = index, Type = pointMeta.ValueType };
           return Remove(nameGroup);
        }
        public bool Remove(PointNameGroup nameGroup)
        {
            bool result = false;
            if (_pointNames.Exists((s) => s == nameGroup))
            {
                var type = nameGroup.Type;
                if (type == DataType.Bool)
                {
                    var point = _pointMapping.GetBoolPoint(nameGroup.PointName);
                    point.UpdataEvent -= sendBoolData;
                    result = true;
                }
                else if (type ==DataType.Byte)
                {
                    var point = _pointMapping.GetBytePoint(nameGroup.PointName);
                    point.UpdataEvent -= sendByteData;
                    result = true;
                }
                else if (type == DataType.Short)
                {
                    var point = _pointMapping.GetShortPoint(nameGroup.PointName); 
                    point.UpdataEvent -= sendShortData;
                    result = true;
                }
                else if (type == DataType.UShort)
                {
                    var point = _pointMapping.GetUShortPoint(nameGroup.PointName); 
                    point.UpdataEvent -= sendUshortData;
                    result = true;
                }
                else if (type == DataType.Int)
                {
                    var point = _pointMapping.GetIntPoint(nameGroup.PointName); 
                    point.UpdataEvent -= sendIntData;
                    result = true;
                }
                else if (type == DataType.UInt)
                {
                    var point = _pointMapping.GetUIntPoint(nameGroup.PointName);
                    point.UpdataEvent -= sendUintData;
                    result = true;
                }
                else if (type == DataType.Float)
                {
                    var point = _pointMapping.GetFloatPoint(nameGroup.PointName);
                    point.UpdataEvent -= sendFloatData;
                    result = true;
                }
                else if (type == DataType.String)
                {
                    var point = _pointMapping.GetStringPoint(nameGroup.PointName);
                    point.UpdataEvent -= sendStringData;
                    result = true;
                }
            }
            return result;
          
        }
        public void RemoveAll()
        {
            foreach(var s in _pointNames)
            {
                Remove(s);
            }
            _pointNames.Clear();
        }
        #region 订阅触发执行方法
        private static readonly object _locker1 = new object();
        private static readonly object _locker2 = new object();
        private static readonly object _locker3 = new object();
        private static readonly object _locker4 = new object();
        private static readonly object _locker5 = new object();
        private static readonly object _locker6 = new object();
        private static readonly object _locker7 = new object();
        private static readonly object _locker8 = new object();
        private static readonly object _locker9 = new object();

        private void sendBoolData(IPoint<bool> point,int index)
        {
            lock (_locker1)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var pointName = point.Name;
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    var quality = point.GetQuality();
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.Bool.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendByteData(IPoint<byte> point, int index)
        {
            lock (_locker2)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var pointName = point.Name;
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    var quality = point.GetQuality();
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.Byte.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendUshortData(IPoint<ushort> point, int index)
        {
            lock (_locker3)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var pointName = point.Name;
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    var quality = point.GetQuality();
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.UShort.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendShortData(IPoint<short> point, int index)
        {
            lock (_locker4)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var pointName = point.Name;
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    var quality = point.GetQuality();
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.Short.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendIntData(IPoint<int> point, int index)
        {
            lock (_locker5)
            {
                lock (_locker4)
                {
                    ASCIIEncoding endcoding = new ASCIIEncoding();
                    var pointName = point.Name;
                    if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                    {
                        var value = point.GetValue(index);
                        var quality = point.GetQuality();
                        string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.Int.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                        _connectState.Send(endcoding.GetBytes(sendStr));
                    }
                }
            }
        }
        private void sendUintData(IPoint<uint> point, int index)
        {
            lock (_locker6)
            {

                lock (_locker4)
                {
                    ASCIIEncoding endcoding = new ASCIIEncoding();
                    var pointName = point.Name;
                    if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                    {
                        var value = point.GetValue(index);
                        var quality = point.GetQuality();
                        string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.UInt.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                        _connectState.Send(endcoding.GetBytes(sendStr));
                    }
                }
            }
        }
        private void sendFloatData(IPoint<float> point, int index)
        {
            lock (_locker7)
            {
                lock (_locker4)
                {
                    ASCIIEncoding endcoding = new ASCIIEncoding();
                    var pointName = point.Name;
                    if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                    {
                        var value = point.GetValue(index);
                        var quality = point.GetQuality();
                        string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.Float.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                        _connectState.Send(endcoding.GetBytes(sendStr));
                    }
                }
            }
        }
        private void sendStringData(IPoint<string> point, int index)
        {
            lock (_locker8)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var pointName = point.Name;
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    var quality = point.GetQuality();
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataType.String.ToString(), ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        #endregion
    }
    internal struct PointNameGroup
    {
        public string PointName;
        public int Index;
        public DataType Type;
        public static bool operator ==(PointNameGroup a, PointNameGroup b)
        {
            if (a.PointName == b.PointName && a.Index == b.Index)
                return true;
            else
                return false;
        }
        public static bool operator !=(PointNameGroup a, PointNameGroup b)
        {
            if (a.PointName == b.PointName && a.Index == b.Index)
                return false;
            else
                return true;
        }

    }
}
