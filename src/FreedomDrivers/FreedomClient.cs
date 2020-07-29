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
using DataServer.Utillity;
using SocketServers;

namespace FreedomDrivers
{
    public class FreedomClient:IDisposable
    {
        private DriverType _driverType;
        private TimeOut _timeOut;
        private bool _isConnect = false;
        private ILog _log;
        private EthernetSetUp _ethernetSetUp = new EthernetSetUp();
        private Socket _socket;
        private int _readCacheSize = 16384;


        private bool _subSocketArgFlag;

        private string _headStr = "<F0>";
        private string _endStr = "<F1>";

        public FreedomClient(EthernetSetUp ethernetSetUp, TimeOut timeOut, ILog log)
        {
            _ethernetSetUp = ethernetSetUp;
            _timeOut = timeOut;
            _log = log;
            _driverType = DriverType.Ethernet;
            _subSocketArgFlag = false;
        }

      
        public EthernetSetUp EthernetSetUp
        {
            get { return _ethernetSetUp; }
            set { _ethernetSetUp = value; }
        }
        public DriverType DriType
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
        #region read
        private byte[] createReadbytes<T>(List<PointGroup<T>> pointStructList)
        {
            string resultStr = "";
            foreach (var pointStruct in pointStructList)
            {
                var point = pointStruct.Point;
                var index = pointStruct.Index;
                var name = string.Concat(point.Name, "[", index, "]");
                var type = point.ValueType;
                resultStr = string.Concat(resultStr, "<", name, ">", "<", type, ">");
            }
            var funCode = "<01>";
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        #region sync read
        private int read<T>(List<PointGroup<T>> pointGroups, readDataProcess<T> process)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = createReadbytes(pointGroups);
                    var receiveBuffer = new byte[12768];
                    _log.ByteSteamLog(ActionType.SEND, sendBytes);//数据流Log记录
                    if (_socket.Send(sendBytes) != -1)
                    {
                        Thread.Sleep(10);
                        int count = _socket.Receive(receiveBuffer);

                        var receivedata = new byte[count];
                        Array.Copy(receiveBuffer, receivedata, count);
                        _log.ByteSteamLog(ActionType.RECEIVE, receiveBuffer);
                        var receiveStr = encoding.GetString(receivedata);
                        receiveStr=receiveStr.Replace("<", "");
                        var strArrary = receiveStr.Split(new char[] { '>'},StringSplitOptions.RemoveEmptyEntries);
                        if (strArrary.Length > 2)
                        {
                            if (strArrary[1] == "11")
                            {
                                if (strArrary.Length >= (pointGroups.Count*4+3))
                                {
                                    return process(pointGroups, strArrary);
                                }
                                else
                                {
                                    Log.ErrorLog(string.Format("Freedom Read {0} ", "receive data length too less!"));
                                    return -1;
                                }
                            }
                            else if (strArrary[1] == "21")
                            {
                                string errorInfo = "";
                                for (int i = 2; i < strArrary.Length - 1; i++)
                                {
                                    errorInfo = string.Concat(errorInfo, "<", strArrary[i], ">");
                                }
                                Log.ErrorLog(string.Format("Freedom Read {0} ", errorInfo));
                                return -1;
                            }
                            else
                            {
                                Log.ErrorLog(string.Format("Freedom Read {0} ", "receive function code error!"));
                                return -1;
                            }
                        }
                        else
                        {
                            Log.ErrorLog(string.Format("Freedom Read {0} ", "receive data length too less!"));
                            return -1;
                        }
                    }
                    else
                    {
                        Log.ErrorLog(string.Format("Freedom Read {0} ", "send buffer fail!"));
                        return -1;
                    }
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "connect is not build!"));
                    return -1;
                }

            }
            catch (Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Read {0} ", e.Message));
                return -1;
            }
        }
        private delegate int readDataProcess<T>(List<PointGroup<T>> pointGroups, string[] source);
        private int boolDataProcess(List<PointGroup<bool>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                bool updateValue;
                byte updataQuality;
                if (bool.TryParse(data[4 * i + 2], out updateValue) && byte.TryParse(data[4 * i + 3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));
                    return -1;
                }

            }

            return 1;

        }
        private int byteDataProcess(List<PointGroup<byte>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                byte updateValue;
                byte updataQuality;
                if (byte.TryParse(data[4 * i + 2], out updateValue) && byte.TryParse(data[4 * i + 3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));

                    return -1;
                }

            }
            return 1;
        }
        private int shortDataProcess(List<PointGroup<short>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                short updateValue;
                byte updataQuality;
                if (short.TryParse(data[4 * i + 2], out updateValue) && byte.TryParse(data[4 * i + 3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));

                    return -1;
                }
            }
            return 1;
        }
        private int ushortDataProcess(List<PointGroup<ushort>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                ushort updateValue;
                byte updataQuality;
                if (ushort.TryParse(data[4 * i + 2], out updateValue) && byte.TryParse(data[4 * i + 3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));

                    return -1;
                }
            }
            return 1;
        }
        private int intDataProcess(List<PointGroup<int>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                int updateValue;
                byte updataQuality;
                if (int.TryParse(data[4 * i + 2], out updateValue) && byte.TryParse(data[4 * i + 3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));
                    return -1;
                }
            }
            return 1;
        }
        private int uintDataProcess(List<PointGroup<uint>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                uint updateValue;
                byte updataQuality;
                if (uint.TryParse(source[4 * i + 2], out updateValue) && byte.TryParse(source[4 * i +3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));

                    return -1;
                }
            }
            return 1;
        }
        private int floatDataProcess(List<PointGroup<float>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                float updateValue;
                byte updataQuality;
                if (float.TryParse(data[4 * i + 2], out updateValue) && byte.TryParse(data[4 * i + 3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));

                    return -1;
                }
            }
            return 1;
        }
        private int stringDataProcess(List<PointGroup<string>> pointGroups, string[] source)
        {
            string[] data = new string[pointGroups.Count * 4];
            Array.Copy(source, 2, data, 0, data.Length);
            for (int i = 0; i < pointGroups.Count; i++)
            {
                var index = pointGroups[i].Index;
                var point = pointGroups[i].Point;
                string updateValue=source[3*i+1];
                byte updataQuality;
                if (byte.TryParse(data[4 * i + 3], out updataQuality))
                {
                    point.SetValue(updateValue, index);
                    point.SetQuality((QUALITIES)(updataQuality), index);
                }
                else
                {
                    Log.ErrorLog(string.Format("Freedom Read {0} ", "data type error!"));

                    return -1;
                }
            }
            return 1;
        }
        public int ReadBool(PointGroup<bool> pointGroup)
        {
            return read(new List<PointGroup<bool>> { pointGroup }, boolDataProcess);
        }
       
        public int ReadBools(List<PointGroup<bool>> pointGroups)
        {
            return read(pointGroups, boolDataProcess);
        }
        public int ReadByte(PointGroup<byte> pointGroup)
        {
            return read(new List<PointGroup<byte>> { pointGroup }, byteDataProcess);

        }

        public int ReadBytes(List<PointGroup<byte>> pointGroups)
        {
            return read(pointGroups, byteDataProcess);
        }
        public int ReadShort(PointGroup<short> pointGroup)
        {
            return read(new List<PointGroup<short>> { pointGroup }, shortDataProcess);

        }

        public int ReadShorts(List<PointGroup<short>> pointGroups)
        {
            return read(pointGroups, shortDataProcess);

        }
        public int ReadUShort(PointGroup<ushort> pointGroup)
        {
            return read(new List<PointGroup<ushort>> { pointGroup },ushortDataProcess);
        }

        public int ReadUShorts(List<PointGroup<ushort>> pointGroups)
        {
            return read(pointGroups, ushortDataProcess);
        }
        public int ReadInt(PointGroup<int> pointGroup)
        {
            return read(new List<PointGroup<int>> { pointGroup }, intDataProcess);
        }

        public int ReadInts(List<PointGroup<int>> pointGroups)
        {
            return read(pointGroups, intDataProcess);
        }
        public int ReadUInt(PointGroup<uint> pointGroup)
        {
            return read(new List<PointGroup<uint>> { pointGroup }, uintDataProcess);
        }

        public int ReadUInts(List<PointGroup<uint>> pointGroups)
        {
            return read(pointGroups, uintDataProcess);
        }

        public int Readfloat(PointGroup<float> pointGroup)
        {
            return read(new List<PointGroup<float>> { pointGroup }, floatDataProcess);
        }

        public int Readfloats(List<PointGroup<float>> pointGroups)
        {
            return read(pointGroups, floatDataProcess);
        }

        public int ReadString(PointGroup<string> pointGroup)
        {
            return read(new List<PointGroup<string>> { pointGroup }, stringDataProcess);
        }

        public int ReadStrings(List<PointGroup<string>> pointGroups)
        {
            return read(pointGroups, stringDataProcess);
        }
        public int ReadData<T>(PointGroup<T> pointGroup)
        {
            throw new NotImplementedException();
        }

        public int ReadDatas<T>(List<PointGroup<T>> pointGroups)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        #region write
        private byte[] creatWriteData<T>(List<PointGroup<T>> pointStructList)
        {
            string resultStr = "";
            foreach (var pointStruct in pointStructList)
            {
                var point = pointStruct.Point;
                var index = pointStruct.Index;
                var name = string.Concat(point.Name, "[", index, "]");
                var type = point.ValueType;
                var value = point.GetValue(index);
                resultStr = string.Concat(resultStr, "<", name, ">", "<", type, ">", "<", value, ">");
            }
            var funCode = "<02>";
            resultStr = string.Concat(_headStr, funCode, resultStr, _endStr);
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(resultStr);
        }
        #region  sync write
        public int Write<T>(PointGroup<T> pointGroup)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = creatWriteData(new List<PointGroup<T>>{ pointGroup });
                    var receiveBuffer = new byte[12768];
                    _log.ByteSteamLog(ActionType.SEND, sendBytes);
                    if (_socket.Send(sendBytes) != -1)
                    {
                       Thread.Sleep(10);
                       int count= _socket.Receive(receiveBuffer);
                        var receivedata = new byte[count];
                        Array.Copy(receiveBuffer, receivedata, count);
                        _log.ByteSteamLog(ActionType.RECEIVE, receiveBuffer);
                        var receiveStr = encoding.GetString(receivedata);
                        receiveStr.Replace("<", "");
                        var strArrary = receiveStr.Split('>');
                        if (strArrary.Length > 2)
                        {
                            if(strArrary[1] == "12")
                            {
                                return 1;
                            }
                            else if (strArrary[1] == "22")
                            {
                                string errorInfo = "";
                                for(int i = 2; i < strArrary.Length - 1; i++)
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
            catch(Exception e)
            {
                DisConnect();
                Log.ErrorLog(string.Format("Freedom Write {0} ", e.Message));
                return -1;
            }
        }
        public int Write<T>(List<PointGroup<T>> pointGroupList)
        {
            try
            {
                if (_isConnect)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    var sendBytes = creatWriteData(pointGroupList);
                    var receiveBuffer = new byte[12768];
                    _log.ByteSteamLog(ActionType.SEND, sendBytes);

                    if (_socket.Send(sendBytes) != -1)
                    {
                        Thread.Sleep(10);
                        int count = _socket.Receive(receiveBuffer);
                        var receivedata = new byte[count];
                        Array.Copy(receiveBuffer, receivedata, count);
                        _log.ByteSteamLog(ActionType.RECEIVE, receiveBuffer);

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
    
    public struct PointGroup<T>
    {
        public IPoint<T> Point;
        public byte Index;
    }
    public struct AsyncResult
    {
        public string Name;
        public string Index;
        public string Type;
        public string Value;
        public string Quality;
    }
}
