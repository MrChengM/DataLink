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

namespace FreedomDrivers
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

        private int _maxConnect = 100;

        private PointMeDataList _pointMeDataList = PointMeDataList.GetInstance();
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
        public string IpString
        {
            get { return _ipString; }
            set { _ipString = value; }
        }
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        public FreedomServer()
        {
        }
        /// <summary>
        /// 自定义服务实例化
        /// </summary>
        /// <param name="log">消息记录</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="type">socket服务格式：Apm，SAEA等</param>
        public FreedomServer(EthernetSetUp setUp, TimeOut timeOut, ILog log, SocketServerType type = SocketServerType.SaeaServer)
        {
            _log = log;
            _timeOut = timeOut;
            _socketServerType = type;
            _ipString = setUp.IPAddress;
            _port = setUp.PortNumber;
            _subscribeGroup = new List<SubscribeItem>();
        }

        public bool Init()
        {

            var factory = new SocketServerFactroy(_ipString, _port, _log, _timeOut,_readCacheSize, _maxConnect);
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
                if (byte.TryParse(source[1], NumberStyles.HexNumber, null as IFormatProvider, out funCode))
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
            List<string> strList=new List<string>();
            string errorInfo=null;
            if (source.Length % 2 == 0)
            {
                for (int i = 0; i < source.Length; i = i + 2)
                {
                    var pointName = source[i];
                    var pointType = source[i + 1];
                    string typeInMapping;
                    var pointNameArrary = pointName.Split('[');
                    byte index = 0;
                    if (pointNameArrary.Length >= 2)
                    {
                        pointNameArrary[1] = pointNameArrary[1].Replace("]", "");
                        byte.TryParse(pointNameArrary[1], out index);
                    }
                    if (_pointMeDataList.Find(pointNameArrary[0], out typeInMapping))
                    {
                        if (pointType.ToLower() == typeInMapping)
                        {
                            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameArrary[0], Index = index, Type = typeInMapping };
                            strList.AddRange(readFeedBack(nameGroup));
                        }
                        else
                        {
                            errorInfo = string.Concat("the point name:", pointName, " type : ", pointType, " type not match");
                            break;
                        }

                    }
                    else
                    {
                        errorInfo = string.Concat("Not found the point name:", pointName);
                        break;
                    }
                }
            }
            else
            {
                errorInfo="steam length error";
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
            if (source.Length % 3 == 0)
            {
                for (int i = 0; i < source.Length; i = i + 3)
                {
                    var pointName = source[i];
                    var pointType = source[i + 1];
                    var pointValue = source[i + 2];
                    string typeInMapping;
                    var pointNameStruct = pointName.Split('[');
                    byte index = 0;
                    if (pointNameStruct.Length >= 2)
                    {
                        byte.TryParse(pointNameStruct[1].Replace("]", ""), out index);
                    }
                    if (_pointMeDataList.Find(pointNameStruct[0], out typeInMapping))
                    {
                        if (pointType.ToLower() == typeInMapping)
                        {
                            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameStruct[0], Index = index, Type = typeInMapping };
                            if (writeMapping(nameGroup, pointValue) == -1)
                            {
                                errorInfoList.Add(string.Concat("the point name:", pointName, " type : ", pointType, " write error"));
                            }
                        }
                        else
                        {
                            errorInfoList.Add(string.Concat("the point name:", pointName, " type : ", pointType, " type not match"));
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
                result = encoding.GetBytes(string.Concat(_headStr, funCodeStr, _endStr));
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
            if (source.Length % 2 == 0)
            {
                for (int i = 0; i < source.Length; i = i + 2)
                {
                    var pointName = source[i];
                    var pointType = source[i + 1];
                    var pointNameArrary = pointName.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                    byte index = 0;
                    string type;
                    if (pointNameArrary.Length >= 2)
                    {
                        byte.TryParse(pointNameArrary[1].Replace("]", ""), out index);
                    }
                    if (_pointMeDataList.Find(pointNameArrary[0], out type))
                    {
                        if (pointType.ToLower() == type)
                        {

                            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameArrary[0], Index = index, Type = type };
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
                                item = new SubscribeItem(connecter, _log);
                                _subscribeGroup.Add(item);
                            }
                            item.Add(nameGroup);

                        }
                        else
                        {
                            errorInfo = string.Concat("the point name:", pointName, " type : ", pointType, " type not match");
                            errorInfoList.Add(errorInfo);
                        }
                    }
                    else
                    {
                        errorInfo = string.Concat("Not found the point name:", pointName);
                        errorInfoList.Add(errorInfo);
                    }
                }
            }
            else
            {
                errorInfoList.Add("steam length error");
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
            if (source.Length % 2 == 0)
            {
                for (int i = 0; i < source.Length; i = i + 2)
                {
                    var pointName = source[i];
                    var pointType = source[i + 1];
                    var pointNameArrary = pointName.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                    byte index = 0;
                    string type;
                    if (pointNameArrary.Length >= 2)
                    {
                        byte.TryParse(pointNameArrary[1].Replace("]", ""), out index);
                    }
                    if (_pointMeDataList.Find(pointNameArrary[0], out type))
                    {
                        if (pointType.ToLower() == type)
                        {

                            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameArrary[0], Index = index, Type = type };
                            //反馈第一次订阅值

                            //取消添加订阅
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
                                errorInfo = string.Concat("the point name:", pointName, " not Subscrible");
                            }

                        }
                        else
                        {
                            errorInfo = string.Concat("the point name:", pointName, " type : ", pointType, " type not match");
                            errorInfoList.Add(errorInfo);
                        }
                    }
                    else
                    {
                        errorInfo = string.Concat("Not found the point name:", pointName);
                        errorInfoList.Add(errorInfo);
                    }
                }
            }
            else
            {
                errorInfoList.Add("steam length error");
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
            foreach(var metaData in _pointMeDataList.Data)
            {
                strList.Add(metaData.Name);
                strList.Add(metaData.ValueType);
                strList.Add(metaData.IsVirtual.ToString());
                strList.Add(metaData.Length.ToString());
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

                    if((connecter.Send(encoding.GetBytes(string.Concat(_headStr, funCodeStr, resultStr, _endStr)))!=1))
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
            byte index = nameGroup.Index;
            string type = nameGroup.Type;
            List<string> result = new List<string>();
            if (type == DataServer.ValueType.Bool)
            {
                var pointMapping = PointMapping<bool>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName,"[",index,"]"));
                result.Add(type);
                result.Add(value.ToString());
                result.Add(quality.ToString());

            }
            else if (type == DataServer.ValueType.Byte)
            {
                var pointMapping = PointMapping<byte>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type);
                result.Add(value.ToString());
                result.Add(quality.ToString());

            }
            else if (type == DataServer.ValueType.UInt16)
            {
                var pointMapping = PointMapping<ushort>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type);
                result.Add(value.ToString());
                result.Add(quality.ToString());

            }
            else if (type == DataServer.ValueType.Int16)
            {
                var pointMapping = PointMapping<short>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type);
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataServer.ValueType.UInt32)
            {
                var pointMapping = PointMapping<uint>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type);
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataServer.ValueType.Int32)
            {
                var pointMapping = PointMapping<int>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type);
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataServer.ValueType.Float)
            {
                var pointMapping = PointMapping<float>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type);
                result.Add(value.ToString());
                result.Add(quality.ToString());
            }
            else if (type == DataServer.ValueType.String)
            {
                var pointMapping = PointMapping<string>.GetInstance(_log);
                var value = pointMapping.GetValue(pointName, index);
                byte quality = pointMapping.GetPoint(pointName).GetQuality(index);
                result.Add(string.Concat(pointName, "[", index, "]"));
                result.Add(type);
                result.Add(value);
                result.Add(quality.ToString());
            }
            return result;
        }
        private int writeMapping(PointNameGroup nameGroup,string value)
        {
            string pointName = nameGroup.PointName;
            byte index = nameGroup.Index;
            string type = nameGroup.Type;
            List<string> result = new List<string>();
            if (type == DataServer.ValueType.Bool)
            {
                bool temp;
                if(bool.TryParse(value,out temp))
                {
                    var pointMapping = PointMapping<bool>.GetInstance(_log);
                    return pointMapping.SetValue(pointName, temp, index);
                }
                else
                {
                    return -1;
                }
            

            }
            else if (type == DataServer.ValueType.Byte)
            {
                byte temp;
                if (byte.TryParse(value, out temp))
                {
                    var pointMapping = PointMapping<byte>.GetInstance(_log);
                    return pointMapping.SetValue(pointName, temp, index);
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataServer.ValueType.UInt16)
            {
                ushort temp;
                if (ushort.TryParse(value, out temp))
                {
                    var pointMapping = PointMapping<ushort>.GetInstance(_log);
                    return pointMapping.SetValue(pointName, temp, index);
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataServer.ValueType.Int16)
            {
                short temp;
                if (short.TryParse(value, out temp))
                {
                    var pointMapping = PointMapping<short>.GetInstance(_log);
                    return pointMapping.SetValue(pointName, temp, index);
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataServer.ValueType.UInt32)
            {
                uint temp;
                if (uint.TryParse(value, out temp))
                {
                    var pointMapping = PointMapping<uint>.GetInstance(_log);
                    return pointMapping.SetValue(pointName, temp, index);
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataServer.ValueType.Int32)
            {
                int temp;
                if (int.TryParse(value, out temp))
                {
                    var pointMapping = PointMapping<int>.GetInstance(_log);
                    return pointMapping.SetValue(pointName, temp, index);
                }
                else
                {
                    return -1;
                }
            }
            else if (type == DataServer.ValueType.Float)
            {
                float temp;
                if (float.TryParse(value, out temp))
                {
                    var pointMapping = PointMapping<float>.GetInstance(_log);
                    return pointMapping.SetValue(pointName, temp, index);
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
        #endregion
    }
    /// <summary>
    ///  订阅子项
    /// </summary>
    internal class SubscribeItem
    {
        private IConnectState _connectState;
        private List<PointNameGroup> _pointNames;
        private PointMeDataList pointIndex = PointMeDataList.GetInstance();
        private ILog _log;
        private string _headStr = "<F0>";
        private string _endStr = "<F1>";
        public SubscribeItem(IConnectState connectState,ILog log)
        {
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
            pointIndex.Find(pointNameStruct[0], out type);
            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameStruct[0], Index = index,Type=type };
            return Add(nameGroup);
        }
        public bool Add(PointNameGroup nameGroup)
        {
            if (_pointNames.Exists((s) => (s == nameGroup)))
            {
                return false;
            }
            var type = nameGroup.Type;

            if (!_pointNames.Exists(s => s.PointName == nameGroup.PointName))
            {
                if (type == DataServer.ValueType.Bool)
                {
                    var point = PointMapping<bool>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendBoolData;

                }
                else if (type == DataServer.ValueType.Byte)
                {
                    var point = PointMapping<byte>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendByteData;


                }
                else if (type == DataServer.ValueType.Int16)
                {
                    var point = PointMapping<short>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendShortData;

                }
                else if (type == DataServer.ValueType.UInt16)
                {
                    var point = PointMapping<ushort>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendUshortData;

                }
                else if (type == DataServer.ValueType.Int32)
                {
                    var point = PointMapping<int>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendIntData;

                }
                else if (type == DataServer.ValueType.UInt32)
                {
                    var point = PointMapping<uint>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendUintData;

                }
                else if (type == DataServer.ValueType.Float)
                {
                    var point = PointMapping<float>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendFloatData;
                }
                else if (type == DataServer.ValueType.String)
                {
                    var point = PointMapping<string>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged += sendStringData;
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
            pointIndex.Find(pointNameStruct[0], out type);
            PointNameGroup nameGroup = new PointNameGroup { PointName = pointNameStruct[0], Index = index, Type = type };
           return Remove(nameGroup);
        }
        public bool Remove(PointNameGroup nameGroup)
        {
            bool result = false;
            if (_pointNames.Exists((s) => (s == nameGroup)))
            {
                var type = nameGroup.Type;
                if (type == DataServer.ValueType.Bool)
                {
                    var point = PointMapping<bool>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendBoolData;
                    result = true;
                }
                else if (type == DataServer.ValueType.Byte)
                {
                    var point = PointMapping<byte>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendByteData;
                    result = true;
                }
                else if (type == DataServer.ValueType.Int16)
                {
                    var point = PointMapping<short>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendShortData;
                    result = true;
                }
                else if (type == DataServer.ValueType.UInt16)
                {
                    var point = PointMapping<ushort>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendUshortData;
                    result = true;
                }
                else if (type == DataServer.ValueType.Int32)
                {
                    var point = PointMapping<int>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendIntData;
                    result = true;
                }
                else if (type == DataServer.ValueType.UInt32)
                {
                    var point = PointMapping<uint>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendUintData;
                    result = true;
                }
                else if (type == DataServer.ValueType.Float)
                {
                    var point = PointMapping<float>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendFloatData;
                    result = true;
                }
                else if (type == DataServer.ValueType.Float)
                {
                    var point = PointMapping<string>.GetInstance(_log).GetPoint(nameGroup.PointName);
                    point.PropertyChanged -= sendStringData;
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

        private void sendBoolData(object sender,PropertyChangedEventArgs e)
        {
            lock (_locker1)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<bool>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.Bool, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendByteData(object sender, PropertyChangedEventArgs e)
        {
            lock (_locker2)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<byte>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.Byte, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendUshortData(object sender, PropertyChangedEventArgs e)
        {
            lock (_locker3)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<ushort>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.UInt16, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendShortData(object sender, PropertyChangedEventArgs e)
        {
            lock (_locker4)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<short>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.Int16, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendIntData(object sender, PropertyChangedEventArgs e)
        {
            lock (_locker5)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<int>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.Int32, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendUintData(object sender, PropertyChangedEventArgs e)
        {
            lock (_locker6)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<uint>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.UInt32, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendFloatData(object sender, PropertyChangedEventArgs e)
        {
            lock (_locker7)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<float>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.Float, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        private void sendStringData(object sender, PropertyChangedEventArgs e)
        {
            lock (_locker8)
            {
                ASCIIEncoding endcoding = new ASCIIEncoding();
                var point = sender as IPoint<string>;
                var pointName = point.Name;
                var index = byte.Parse(e.PropertyName);
                if (_pointNames.Exists(s => s.PointName == pointName && s.Index == index))
                {
                    var value = point.GetValue(index);
                    byte quality = point.GetQuality(index);
                    string sendStr = string.Concat(_headStr, "<15>", "<", pointName, "[", index, "]", ">", "<", DataServer.ValueType.Float, ">", "<", value, ">", "<", quality, ">", _endStr);
                    _connectState.Send(endcoding.GetBytes(sendStr));
                }
            }
        }
        #endregion
    }
    internal struct PointNameGroup
    {
        public string PointName;
        public byte Index;
        public string Type;
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
