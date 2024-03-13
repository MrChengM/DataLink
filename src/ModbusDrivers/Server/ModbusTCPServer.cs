using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Points;
using SocketServers;
using Utillity.Data;
using DataServer.Config;

namespace ModbusDrivers.Server
{
    public class ModbusTCPServer : IServerDrivers
    {
        private ILog _log;
        private ModbusPointMapping _mapping;
        private ISockteServer _socketServer;
        private TimeOut _timeOut;
        private int _maxConnect = 100;
        private string _ipString="127.0.0.1";
        private int _port = 502;
        private SocketServerType _socketServerType;
        private ComPhyLayerSetting _phyLayerSetting;
        private string _serverName;
        /// <summary>
        /// 报头长度
        /// </summary>
        private const int headLength = 6;
        /// <summary>
        /// 每次读取数量
        /// </summary>
        private const int readCacheSize = 1024;
        private bool _isRunning;
        private int _salveId = 1;

        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        public bool IsRunning
        {
            get { return _isRunning; }
        }
        public int salveId
        {
            get { return _salveId; }
            set { _salveId = value; }
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

        public string ServerName { get => _serverName; set => _serverName = value; }
        public ComPhyLayerSetting PhyLayerSetting { get => _phyLayerSetting; set => _phyLayerSetting = value; }
        public TimeOut TimeOut { get => _timeOut; set => _timeOut = value; }
        public int MaxConnect { get => _maxConnect; set => _maxConnect = value; }
        public IPointMapping PointMapping { get => _mapping.PointMapping ; set => _mapping.PointMapping = value; }

        /// <summary>
        /// ModbusTCP服务实例化
        /// </summary>
        /// <param name="log">消息记录</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="maxConnect">最大连接数</param>
        /// <param name="salveId">地址码</param>
        /// <param name="type">socket服务格式：Apm，SAEA等</param>
        public ModbusTCPServer(string serverName, ComPhyLayerSetting phyLayerSetting, TimeOut timeOut, ILog log, int maxConnect,int salveId,SocketServerType type=SocketServerType.SaeaServer)
        {
            _serverName = serverName;
            _log = log;
            _timeOut = timeOut;
            _maxConnect = maxConnect;
            _salveId = salveId;
            _socketServerType = type;
            _phyLayerSetting = phyLayerSetting;
            _mapping = new ModbusPointMapping(_log);
        }
        public ModbusTCPServer( SocketServerType type = SocketServerType.SaeaServer)
        {
            _socketServerType = type;
        }
        public bool Init()
        {
            _mapping = new ModbusPointMapping(_log);

            _ipString = _phyLayerSetting.EthernetSet.IPAddress;
            _port = _phyLayerSetting.EthernetSet.PortNumber;

            var factory = new SocketServerFactroy(_serverName, _ipString,_port,_log,_timeOut,readCacheSize,_maxConnect);
            _socketServer = factory.CreateInstance(_socketServerType);
            if(_socketServer.Init())
            {
                _socketServer.ReadComplete += recivceDataHanding;
                _socketServer.SendComplete += sendComplete;
                return true;
            }
            return false;
        }
        private void sendComplete(IConnectState connecter)
        {
            if (connecter.ReadBufferPool.IsEmpty)
            {
                connecter.ReceiveAsync(readCacheSize);
            }
            else
            {
                recivceDataHanding(connecter);//如果bufferPool主队列缓存不为空，继续处理数据。
            }
        }
        /// <summary>
        /// Modbus报文处理函数
        /// </summary>
        /// <param name="connecter">连接对象</param>
        private void recivceDataHanding(IConnectState connecter)
        {
            lock (locker)
            {
                var bufferPool = connecter.ReadBufferPool;
                bufferPool.HeadLength = headLength;
                if (bufferPool.HeadBuffer == null)
                {
                        connecter.ReceiveAsync(readCacheSize);
                }
                else
                {
                    bufferPool.BodyLength = bufferPool.HeadBuffer[5];
                    if (bufferPool.BodyBuffer == null)
                    {
                        connecter.ReceiveAsync(readCacheSize);
                    }
                    else
                    {
                        byte[] relyBuffer = bufferRely(bufferPool.BodyBuffer);
                        if (relyBuffer !=null)
                        {
                            bufferPool.SendBuffer = new byte[headLength + relyBuffer.Length];
                            Array.Copy(bufferPool.HeadBuffer, 0, bufferPool.SendBuffer,0, headLength-1);
                            bufferPool.SendBuffer[5] = (byte)relyBuffer.Length;
                            Array.Copy(relyBuffer, 0, bufferPool.SendBuffer, headLength, relyBuffer.Length);
                            connecter.SendAsync(bufferPool.SendBuffer);
                            bufferPool.clear();
                        }
                        else
                        {
                            connecter.ReceiveAsync(readCacheSize);
                            bufferPool.clear();
                        }
                    }
                }
            }
        }
        static readonly object locker=new object();
        private byte[] bufferRely(byte[] buffer)
        {
            byte[] getBuffer=null;
            try
            {
                if (buffer[0] == (byte)_salveId)
                {
                    switch ((FunctionCode)buffer[1])
                    {
                        case FunctionCode.ReadCoil:
                            if (buffer.Length < 6)
                                getBuffer = errorRely(FunctionCode.ReadCoil, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var count = UnsafeNetConvert.BytesToShort(buffer, 4, ByteOrder.BigEndian);
                                getBuffer = readCoilRely(startAddress, count);
                            }
                            break;
                        case FunctionCode.ReadInputStatus:
                            if (buffer.Length < 6)
                                getBuffer = errorRely(FunctionCode.ReadInputStatus, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var count = UnsafeNetConvert.BytesToShort(buffer, 4, ByteOrder.BigEndian);
                                getBuffer = readInStatusRely(startAddress, count);
                            }
                            break;
                        case FunctionCode.ReadInputRegister:
                            if (buffer.Length < 6)
                                getBuffer = errorRely(FunctionCode.ReadInputRegister, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var count = UnsafeNetConvert.BytesToShort(buffer, 4, ByteOrder.BigEndian);
                                getBuffer = readInputRegisterRely(startAddress, count);
                            }
                            break;
                        case FunctionCode.ReadHoldRegister:
                            if (buffer.Length < 6)
                                getBuffer = errorRely(FunctionCode.ReadHoldRegister, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var count = UnsafeNetConvert.BytesToShort(buffer, 4, ByteOrder.BigEndian);
                                getBuffer = readHoldRegisterRely(startAddress, count);
                            }
                            break;
                        case FunctionCode.ForceSingleCoil:
                            if (buffer.Length < 6)
                                getBuffer = errorRely(FunctionCode.ForceSingleCoil, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var data = new byte[] { buffer[4], buffer[5] };
                                getBuffer = writeSingleCoilRely(startAddress, data);
                            }
                            break;
                        case FunctionCode.ForceMulCoil:
                            if (buffer.Length < 7 || buffer.Length < 7 + buffer[6])
                                getBuffer = errorRely(FunctionCode.ForceMulCoil, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var count = UnsafeNetConvert.BytesToShort(buffer, 4, ByteOrder.BigEndian);
                                var data = new byte[buffer[6]];
                                Array.Copy(buffer, 7, data, 0, buffer[6]);
                                getBuffer = writeMulColilRely(startAddress, data, count);
                            }
                            break;
                        case FunctionCode.WriteSingleRegister:
                            if (buffer.Length < 6)
                                getBuffer = errorRely(FunctionCode.WriteSingleRegister, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var data = new byte[] { buffer[4], buffer[5] };
                                getBuffer = writeSingleReisterRely(startAddress, data);
                            }
                            break;
                        case FunctionCode.WriteMulRegister:
                            if (buffer.Length < 7 || buffer.Length < 7 + buffer[6])
                                getBuffer = errorRely(FunctionCode.WriteMulRegister, ErrorCode.LllegalData);
                            else
                            {
                                var startAddress = UnsafeNetConvert.BytesToShort(buffer, 2, ByteOrder.BigEndian);
                                var data = new byte[buffer[6]];
                                Array.Copy(buffer, 7, data, 0, buffer[6]);
                                getBuffer = writeMultiReisterRely(startAddress, data);
                            }
                            break;
                        default:
                            getBuffer = errorRely((FunctionCode)buffer[1], ErrorCode.LllegalFuctionCode);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

                Log.ErrorLog(string.Format("{0}: Modbus buffer Rely Error:{1}",_serverName, ex.ToString()));
            }
            return getBuffer;
        }
        #region Rely Buffer
        /// <summary>
        /// Read Coil Rely:
        /// Fuction 0x01 
        /// </summary>
        /// <param name="address">00001~09999</param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] readCoilRely(int address,int count)
        {

            bool[] boolDate = new bool[count];
            byte[] dataBuffer;
            byte[] relyBuffer; //addtion SlaveId，Function.

            

                var addressString = string.Format("{0:D5}", address +1);//地址偏移+1
            var getBools = _mapping.GetBools(addressString);
            if (getBools != null || getBools.Length >= count)
            {
                Array.Copy(getBools, boolDate, count);
            }
            else
            {
                return errorRely(FunctionCode.ReadCoil, ErrorCode.LllegalDataAddress);
            };
            dataBuffer = NetConvert.BoolstoBytes(boolDate);
            relyBuffer = new byte[dataBuffer.Length + 3];
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] =(byte) FunctionCode.ReadCoil;
            relyBuffer[2] = (byte)dataBuffer.Length;
            Array.Copy(dataBuffer, 0, relyBuffer, 3, dataBuffer.Length);
            return relyBuffer;
        }
    
        /// <summary>
        /// Read input status Rely
        /// Function:0x02
        /// </summary>
        /// <param name="address">10001~19999</param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] readInStatusRely(int address, int count)
        {

            bool[] boolDate=new bool[count];
            byte[] dataBuffer;
            byte[] relyBuffer; //addtion SlaveId，Function.



                var addressString = string.Format("{0:D5}", address+1+10000);

                    var getBools = _mapping.GetBools(addressString);
            if (getBools != null || getBools.Length >= count)
            {
                Array.Copy(getBools, boolDate, count);
            }
            else
            {
                return errorRely(FunctionCode.ReadInputStatus, ErrorCode.LllegalDataAddress);
            };
            dataBuffer = NetConvert.BoolstoBytes(boolDate);
            relyBuffer = new byte[dataBuffer.Length + 3];
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.ReadInputStatus;
            relyBuffer[2] = (byte)dataBuffer.Length;
            Array.Copy(dataBuffer, 0, relyBuffer, 3, dataBuffer.Length);
            return relyBuffer;
        }
        /// <summary>
        /// Read Input Register Rely
        /// Fuction:0x04
        /// </summary>
        /// <param name="address">30001~39999</param>
        /// <param name="count">less than 128</param>
        /// <returns></returns>
        private byte[] readInputRegisterRely(int address, int count)
        {

            byte[] dataBuffer = new byte[count * 2];
            byte[] relyBuffer = new byte[count * 2 + 3]; //addtion SlaveId，Function,Count


            var addressString = string.Format("{0:D5}", address + 1 + 30000);//地址偏移+1
            byte[] getBuffer = _mapping.GetBytes(addressString);
            if (getBuffer != null || getBuffer.Length >= dataBuffer.Length)
            {
                Array.Copy(getBuffer, dataBuffer, dataBuffer.Length);
            }
            else
            {
                return errorRely(FunctionCode.ReadInputRegister, ErrorCode.LllegalDataAddress);
            };
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.ReadInputRegister;
            relyBuffer[2] = (byte)dataBuffer.Length;
            Array.Copy(dataBuffer, 0, relyBuffer, 3, dataBuffer.Length);
            return relyBuffer;
        }
        /// <summary>
        /// Read Hold Register Rely 
        /// Fucntion:0x03
        /// </summary>
        /// <param name="address">40001~49999</param>
        /// <param name="count">less than 128</param>
        /// <returns></returns>
        private byte[] readHoldRegisterRely(int address, int count)
        {

            byte[] dataBuffer = new byte[count * 2];
            byte[] relyBuffer = new byte[count * 2 + 3]; //addtion SlaveId，Function,Count

            var addressString = string.Format("{0:D5}", address + 1 + 40000);//地址偏移+1

            byte[] getBuffer = _mapping.GetBytes(addressString);
            if (getBuffer != null || getBuffer.Length >= dataBuffer.Length)
            {
                Array.Copy(getBuffer, dataBuffer, dataBuffer.Length);
            }
            else
            {
                return errorRely(FunctionCode.ReadInputRegister, ErrorCode.LllegalDataAddress);
            };
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.ReadHoldRegister;
            relyBuffer[2] = (byte)dataBuffer.Length;
            Array.Copy(dataBuffer, 0, relyBuffer, 3, dataBuffer.Length);
            return relyBuffer;
        }
        /// <summary>
        /// Force Single Coil Value
        /// Function:0x05
        /// </summary>
        /// <param name="address">00001~09999</param>
        /// <param name="data">Length=2,data[0]=0xFF as true</param>
        /// <returns></returns>
        private byte[] writeSingleCoilRely(int address, byte[] data)
        {

            byte[] relyBuffer = new byte[6];
            var addressString = string.Format("{0:D5}", address+1);//地址偏移+1
            bool[] value = new bool[1];
            if (data[0] == 0xff)
            {
                value[0] = true;
            }
            if (_mapping.SetValue(addressString,value))
            {
                return errorRely(FunctionCode.ForceSingleCoil, ErrorCode.LllegalDataAddress);
            }
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.ForceSingleCoil;
            var addrByte = UnsafeNetConvert.UShortToBytes((ushort)address, ByteOrder.BigEndian);
            Array.Copy(addrByte, 0, relyBuffer, 2, 2);
            relyBuffer[4] = data[0];
            relyBuffer[5] = data[1];
            return relyBuffer;
        }
        /// <summary>
        /// Force Multi Coils Rely
        /// Function:0x0F
        /// </summary>
        /// <param name="address">00001～09999</param>
        /// <param name="data"></param>
        /// <param name="count">bools count of byte array</param>
        /// <returns></returns>
        private byte[] writeMulColilRely(int address, byte[] data,int count)
        {
            byte[] relyBuffer = new byte[6];
            var values = NetConvert.BytesToBools(data, count);

            var addressString = string.Format("{0:D5}", address + 1);//地址偏移+1
            if (!_mapping.SetValue(addressString, values))
            {
                return errorRely(FunctionCode.ForceMulCoil, ErrorCode.LllegalDataAddress);
            }
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.ForceMulCoil;
            var addrByte = UnsafeNetConvert.UShortToBytes((ushort)address, ByteOrder.BigEndian);
            Array.Copy(addrByte, 0, relyBuffer, 2, 2);
            var countByte = UnsafeNetConvert.UShortToBytes((ushort)count, ByteOrder.BigEndian);
            Array.Copy(countByte, 0, relyBuffer, 4, 2);
            return relyBuffer;
        }
        /// <summary>
        /// Write single hold reister Rely
        /// Function:0x06
        /// </summary>
        /// <param name="address">40001~49999</param>
        /// <param name="data">length=2,Data Hi:data[0],Data Lo:data[1]</param>
        /// <returns></returns>
        private byte[] writeSingleReisterRely(int address, byte[] data)
        {
            byte[] relyBuffer = new byte[6];
            var value = UnsafeNetConvert.BytesToUShort(data, 0, ByteOrder.BigEndian);
            var addressString = string.Format("{0:D5}", address + 1 + 40000);             //地址偏移+1
            if (!_mapping.SetValue(addressString, data,1))
                return errorRely(FunctionCode.WriteSingleRegister, ErrorCode.LllegalDataAddress);
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.WriteSingleRegister;
            var addrByte = UnsafeNetConvert.UShortToBytes((ushort)address, ByteOrder.BigEndian);
            Array.Copy(addrByte, 0, relyBuffer, 2, 2);
            Array.Copy(data, 0, relyBuffer, 4, data.Length);
            return relyBuffer;
        }
        /// <summary>
        /// Write Multi Reister Rely:
        /// Function:0x10
        /// </summary>
        /// <param name="address">40001~49999</param>
        /// <param name="data">length less than 128</param>
        /// <returns></returns>
        private byte[] writeMultiReisterRely(int address, byte[] data)
        {
            byte[] relyBuffer = new byte[6];
            var count = data.Length / 2;
            var value = UnsafeNetConvert.BytesToUShorts(data, 0, count, ByteOrder.BigEndian);
            var addressString = string.Format("{0:D5}", address + 1 + 40000);             //地址偏移+1
            if (!_mapping.SetValue(addressString, data, count))
                return errorRely(FunctionCode.WriteMulRegister, ErrorCode.LllegalDataAddress);
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.WriteMulRegister;
            var addrByte = UnsafeNetConvert.UShortToBytes((ushort)address, ByteOrder.BigEndian);
            var countByte = UnsafeNetConvert.UShortToBytes((ushort)count, ByteOrder.BigEndian);
            Array.Copy(addrByte, 0, relyBuffer, 2, 2);
            Array.Copy(countByte, 0, relyBuffer, 4, 2);
            return relyBuffer;
        }
        /// <summary>
        /// error rely buffer
        /// Function：0x80+function
        /// </summary>
        /// <param name="func">function code</param>
        /// <param name="errorCode">error code</param>
        /// <returns></returns>
        private byte[] errorRely(FunctionCode func, ErrorCode errorCode)
        {
            byte[] data = new byte[3];
            data[0] = (byte)_salveId;
            data[1] = (byte)(0x80 + func);
            data[2] = (byte)errorCode;
            return data;
        }
        #endregion
        public bool Start()
        {
            return _isRunning = _socketServer.Start();
        }
        public bool Stop()
        {
            if( _isRunning)
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

        public void RegisterMapping(Dictionary<string, TagBindingConfig> tagBindings)
        {
            foreach (var tagBinding in tagBindings)
            {
                var pointNameGroup = StringHandler.Split(tagBinding.Value. SourceTagName);
                if (pointNameGroup.Length > 1)
                {
                    var pointName = pointNameGroup[0];
                    var index = int.Parse(pointNameGroup[1]);
                    var pointNameIndex = new PointNameIndex(pointName, index);
                    _mapping.Register(tagBinding.Value.DestTagName, pointNameIndex);
                }
                else
                {
                    var pointNameIndex = new PointNameIndex(pointNameGroup[0], -1);
                    _mapping.Register(tagBinding.Value.DestTagName, pointNameIndex);
                }
            }
        }
        #endregion
    }
}
