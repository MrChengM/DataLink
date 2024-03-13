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

namespace FreedomDriversV2
{
    public class FreedomClient : IDisposable
    {
        private CommunicationType _driverType;
        private TimeOut _timeOut;
        private bool _isConnect = false;
        private ILog _log;
        private EthernetSetUp _ethernetSetUp = new EthernetSetUp();
        private Socket _socket;
        private string _name;


        private string _headStr = "<F0>";
        private string _endStr = "<F1>";

        public FreedomClient(string name, EthernetSetUp ethernetSetUp, TimeOut timeOut, ILog log)
        {
            _name = name;
            _ethernetSetUp = ethernetSetUp;
            _timeOut = timeOut;
            _log = log;
            _driverType = CommunicationType.Ethernet;
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
                        Log.ErrorLog($"{_name}:IP address invalid");
                        return _isConnect = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                DisConnect();
                Log.ErrorLog($"{_name}:FreedomServer Connect Error:" + ex.Message);
                return _isConnect = false;
            }
        }
        #region read
        private byte[] createReadbytes(string[] pointNames)
        {
            string resultStr = "";
            foreach (var pointName in pointNames)
            {
                resultStr = string.Concat(resultStr, "<", pointName, ">");
            }
            var funCode = "<01>";
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        #region sync read
        public ITag[] ReadTags(string[] pointNames)
        {
            ITag[] result = null;
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createReadbytes(pointNames);
                    var receiveBuffer = new byte[12768];
                    Log.DebugLog($"{_name}:Tx => {NetConvert.GetHexString(sendBytes)}");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        Thread.Sleep(10);
                        int count = _socket.Receive(receiveBuffer);
                        var receivedata = new byte[count];
                        Array.Copy(receiveBuffer, receivedata, count);
                        Log.DebugLog($"{_name}:Rx <= {NetConvert.GetHexString(receiveBuffer)}");
                        var receiveStr = encoding.GetString(receivedata);
                        receiveStr = receiveStr.Replace("<", "");
                        var strArrary = receiveStr.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                        if (strArrary.Length > 2)
                        {
                            if (strArrary[1] == "11")
                            {
                                if (strArrary.Length >= (pointNames.Length * 4 + 3))
                                {
                                    result = new Tag[pointNames.Length];
                                    string[] data = new string[pointNames.Length * 4];
                                    Array.Copy(strArrary, 2, data, 0, data.Length);
                                    for (int i = 0; i < pointNames.Length; i++)
                                    {
                                        Enum.TryParse(data[4 * i + 1], out DataType type);
                                        Enum.TryParse(data[4 * i + 3], out QUALITIES qulity);
                                        result[i] = new Tag() { Name = data[4 * i], Type = type, Value = data[4 * i + 2], Quality = qulity };
                                    }
                                }
                                else
                                {
                                    Log.ErrorLog(string.Format("{0}: Freedom Read {1} ",_name, "receive data length too less!"));
                                }
                            }
                            else if (strArrary[1] == "21")
                            {
                                string errorInfo = "";
                                for (int i = 2; i < strArrary.Length - 1; i++)
                                {
                                    errorInfo = string.Concat(errorInfo, "<", strArrary[i], ">");
                                }
                                Log.ErrorLog(string.Format("{0} :Freedom Read {1} ",_name, errorInfo));
                            }
                            else
                            {
                                Log.ErrorLog(string.Format("{0}:Freedom Read {1} ",_name, "receive function code error!"));
                            }
                        }
                        else
                        {
                            Log.ErrorLog(string.Format("{0}:Freedom Read {1} ",_name, "receive data length too less!"));
                        }
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("{0}:Freedom Read {1} ",_name, "send buffer fail!"));
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("{0}:Freedom Read {1} ", _name,"connect is not build!"));
                }
                return result;

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("{0}:Freedom Read {1} ",_name, e.Message));
                return null;
            }
        }
        public ITag ReadTag(string pointName)
        {
            var tags = ReadTags(new string[] { pointName });
            return tags?[0];
        }
        #endregion
        #endregion
        #region write
        private byte[] creatWriteData(ITag[] tags)
        {
            string resultStr = "";
            foreach (var tag in tags)
            {
                resultStr = string.Concat(resultStr, "<", tag.Name, ">", "<", tag.Value, ">");
            }
            var funCode = "<02>";
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        #region  sync write
        public int Write(ITag tag)
        {
            return Writes(new ITag[] { tag });
        }
        public int Writes(ITag[] tags)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = creatWriteData(tags);
                    var receiveBuffer = new byte[12768];
                    Log.DebugLog($"{_name}:Tx => {NetConvert.GetHexString(sendBytes)}");
                    if (_socket.Send(sendBytes) != -1)
                    {
                        Thread.Sleep(10);
                        int count = _socket.Receive(receiveBuffer);
                        var receivedata = new byte[count];
                        Array.Copy(receiveBuffer, receivedata, count);
                        Log.DebugLog($"{_name}:Rx <= {NetConvert.GetHexString(receivedata)}");

                        var receiveStr = encoding.GetString(receivedata);
                        receiveStr=receiveStr.Replace("<", "");
                        var strArrary = receiveStr.Split(new char[] { '>' },StringSplitOptions.RemoveEmptyEntries);
                        if (strArrary.Length > 2)
                        {
                            if (strArrary[1] == "12")
                            {
                                return 1;
                            }
                            else if (strArrary[1] == "22")
                            {
                                string errorInfo = "";
                                for (int i = 2; i < strArrary.Length - 1; i++)
                                {
                                    errorInfo = string.Concat(errorInfo, "<", strArrary[i], ">");
                                }
                                Log.ErrorLog(string.Format("Freedom Write {0} ", errorInfo));
                                return -1;
                            }
                            else
                            {
                                Log.ErrorLog(string.Format("Freedom Write {0} ", "receive function code error!"));
                                return -1;
                            }
                        }
                        else
                        {
                            Log.ErrorLog(string.Format("Freedom Write {0} ", "receive data length too less!"));
                            return -1;
                        }
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("Freedom Write {0} ", "send buffer fail!"));
                        return -1;
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Write {0} ", "connect is not build!"));
                    return -1;
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Write {0} ", e.Message));
                return -1;
            }
        }
        #endregion
        #endregion
        public bool DisConnect()
        {
            if (IsConnect)
            {
                _socket?.Close();
            }
            _socket?.Dispose();
            _socket = null;
            _isConnect = false;
            return true;
        }

        public void Dispose()
        {
            _socket?.Dispose();
            _socket = null;
        }
    }
}
