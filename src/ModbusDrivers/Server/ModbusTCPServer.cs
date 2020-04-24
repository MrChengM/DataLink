using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using SocketServers;

namespace ModbusDrivers.Server
{
    public class ModbusTCPServer: IServerDrivers
    {
        private ILog _log;
        private ModbusPointMapping _mapping;
        private APMServer _apmServer;
        private TimeOut _timeOut;
        private int _maxConnect;
        private const string _ipString="127.0.0.1";
        private const int _port = 502;
        /// <summary>
        /// 报头长度
        /// </summary>
        private const int headLength = 6;
        /// <summary>
        /// 每次读取数量
        /// </summary>
        private const int readLength = 256;
        private bool _isRunning;
        private int _salveId;

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
        public ModbusTCPServer()
        {
        }
        public ModbusTCPServer(ILog log,TimeOut timeOut, int maxConnect,int salveId)
        {
            _log = log;
            _timeOut = timeOut;
            _maxConnect = maxConnect;
            _salveId = salveId;
        }

        public void Init()
        {
            _mapping = ModbusPointMapping.GetInstance();
            _apmServer = new APMServer(_ipString, _port, _log, _timeOut, _maxConnect,readLength);
            _apmServer.Init();
            foreach (ConnectState connecter in _apmServer.Connecters)
            {
                connecter.ReadChangeEvent += bufferHandling;
                connecter.SendFinshEvent += Connecter_SendFinshEvent;
            }
        }

        private void Connecter_SendFinshEvent(ConnectState connecter, int sendCount)
        {
            if (sendCount > 0)
            {
                if (connecter.BufferPool.IsEmpty)
                {
                    connecter.AsyncReceive(readLength);
                }
                else
                {
                    bufferHandling(connecter,0);//如果bufferPool主队列缓存不为空，继续处理数据。
                }
            }
        }
        /// <summary>
        /// Modbus报文处理函数
        /// </summary>
        /// <param name="connecter">连接对象</param>
        /// <param name="readCount">读取数量</param>
        private void bufferHandling(ConnectState connecter, int readCount)
        {
            lock (locker)
            {
                var bufferPool = connecter.BufferPool;
                bufferPool.HeadLength = headLength;
                if (bufferPool.HeadBuffer == null)
                {
                        connecter.AsyncReceive(readLength);
                }
                else
                {
                    bufferPool.BodyLength = bufferPool.HeadBuffer[5];
                    if (bufferPool.BodyBuffer == null)
                    {
                        connecter.AsyncReceive(readLength);
                    }
                    else
                    {
                        byte[] relyBuffer;
                        if ((relyBuffer = bufferRely(bufferPool.BodyBuffer))!=null)
                        {
                            bufferPool.SendBuffer = new byte[headLength + relyBuffer.Length];
                            Array.Copy(bufferPool.HeadBuffer, 0, bufferPool.SendBuffer,0, headLength-1);
                            bufferPool.SendBuffer[5] = (byte)relyBuffer.Length;
                            Array.Copy(relyBuffer, 0, bufferPool.SendBuffer, headLength, relyBuffer.Length);
                            connecter.AsyncSend(bufferPool.SendBuffer);
                            bufferPool.clear();
                        }
                        else
                        {
                            connecter.AsyncReceive(readLength);
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
                                break;
                        }
                    }
             }
            catch (Exception ex)
            {

                Log.ErrorLog(string.Format("Modbus buffer Rely Error:{0}", ex.ToString()));
            }
            return getBuffer;
        } 

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

            
            for (int i = 0; i < count; i++)
            {
                var addressString = string.Format("{0:D5}", address+i+1);//地址偏移+1
                if (_mapping.Find(addressString))
                {
                    boolDate[i] = _mapping.GetValue(addressString)[0];
                }
                else
                {
                    return errorRely(FunctionCode.ReadCoil, ErrorCode.LllegalDataAddress);
                };
            }
            dataBuffer = NetConvert.BoolstoBytes(boolDate);
            relyBuffer = new byte[dataBuffer.Length + 3];
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] =(byte) FunctionCode.ReadCoil;
            relyBuffer[2] = (byte)dataBuffer.Length;
            Array.Copy(dataBuffer, 0, relyBuffer, 3, dataBuffer.Length);
            return relyBuffer;
        }
        #region Rely Buffer
        /// <summary>
        /// Read input status Rely
        /// Function:0x02
        /// </summary>
        /// <param name="address">10001~19999</param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] readInStatusRely(int address, int count)
        {

            bool[] boolDate = new bool[count];
            byte[] dataBuffer;
            byte[] relyBuffer; //addtion SlaveId，Function.


            for (int i = 0; i < count; i++)
            {
                var addressString = string.Format("{0:D5}", address+i+1+10000);
                if (_mapping.Find(addressString))
                {
                    boolDate[i] = _mapping.GetValue(addressString)[0];
                }
                else
                {
                    return errorRely(FunctionCode.ReadInputStatus, ErrorCode.LllegalDataAddress);
                };
            }
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

            byte[] dataBuffer=new byte[count*2];
            byte[] relyBuffer=new byte[count*2+3]; //addtion SlaveId，Function,Count
            IPointMapping<ushort> _ushortMapping = _mapping as IPointMapping<ushort>;


            var ushortData = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                var addressString = string.Format("{0:D5}", address+i+1 + 30000);//地址偏移+1
                if (_mapping.Find(addressString))
                {
                    ushortData[i] = _ushortMapping.GetValue(addressString)[0];
                }
                else
                {
                    return errorRely(FunctionCode.ReadInputRegister, ErrorCode.LllegalDataAddress);
                };
            }
            dataBuffer = UnsafeNetConvert.UShortsToBytes(ushortData, ByteOrder.BigEndian);

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
            IPointMapping<ushort> _ushortMapping = _mapping as IPointMapping<ushort>;


            var ushortData = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                var addressString = string.Format("{0:D5}", address +i+1+ 40000);//地址偏移+1
                if (_mapping.Find(addressString))
                {
                    ushortData[i] = _ushortMapping.GetValue(addressString)[0];
                }
                else
                {
                    return errorRely(FunctionCode.ReadHoldRegister, ErrorCode.LllegalDataAddress);
                };
            }
            dataBuffer = UnsafeNetConvert.UShortsToBytes(ushortData, ByteOrder.BigEndian);

            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.ReadHoldRegister;
            relyBuffer[2] = (byte)dataBuffer.Length;
            Array.Copy(dataBuffer, 0, relyBuffer, 3, dataBuffer.Length);
            return relyBuffer;
        }
        /// <summary>
        /// Force Single Coile Value
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
            if (_mapping.SetValue(addressString, value)==-1)
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
            for(int i = 0; i < count; i++)
            {
                var addressString = string.Format("{0:D5}", address+i+1);//地址偏移+1
                if(_mapping.SetValue(addressString, new bool[] { values[i] })== -1)
                {
                    return errorRely(FunctionCode.ForceMulCoil, ErrorCode.LllegalDataAddress);
                }
            }
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.ForceSingleCoil;
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
            var addressString = string.Format("{0:D5}", address+1+40000);             //地址偏移+1
            if (_mapping.SetValue(addressString, new ushort[] { value }) == -1)
                return errorRely(FunctionCode.WriteSingleRegister, ErrorCode.LllegalDataAddress);
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.WriteSingleRegister;
            var addrByte = UnsafeNetConvert.UShortToBytes((ushort)address, ByteOrder.BigEndian);
            Array.Copy(addrByte, 0, relyBuffer, 2, 2);
            Array.Copy(data, 0, relyBuffer, 4, 2);
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
            for(int i = 0; i < count; i++)
            {
                var addressString = string.Format("{0:D5}", address +i+ 1+40000);             //地址偏移+1
                if (_mapping.SetValue(addressString, new ushort[] { value[i] }) == -1)
                    return errorRely(FunctionCode.WriteMulRegister, ErrorCode.LllegalDataAddress);
            }
            relyBuffer[0] = (byte)_salveId;
            relyBuffer[1] = (byte)FunctionCode.WriteMulRegister;
            var addrByte = UnsafeNetConvert.UShortToBytes((ushort)address, ByteOrder.BigEndian);
            Array.Copy(addrByte, 0, relyBuffer, 2, 2);
            Array.Copy(data, 0, relyBuffer, 4, 2);
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
            
           return _isRunning= _apmServer.Start(); 
        }

        public void Stop()
        {
            if( _isRunning)
            {
                _apmServer.Stop();
            }
        }
    }
}
