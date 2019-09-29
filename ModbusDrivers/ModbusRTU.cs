using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using System.IO.Ports;
using System.Threading;
 
namespace ModbusDrivers
{
    /// <summary>
    /// ModbusRTU Slave协议 IPLCDriver:IRead IWrite IDriver IDisposable
    /// </summary>
    public class ModbusRTUSalve : IPLCDriver
    {
        // 内部成员定义
        private DriverType _driverType = DriverType.Serialport;
        private SerialportSetUp _portSetUp = SerialportSetUp.Default;
        private SerialPort _serialPort = new SerialPort();
        private TimeOut _timeOut;
        private bool _isConnect = false;
        // private bool _isClose = true;
        private ILog _log;
        private int _pdu = 252;

        public ModbusRTUSalve() { }

        public ModbusRTUSalve(SerialportSetUp portSetUp, TimeOut timeOut, ILog log)
        {
            _portSetUp = portSetUp;
            _timeOut = timeOut;
            _log = log;
        }


        public SerialportSetUp PortSetUp
        {
            get
            {
                return _portSetUp;
            }
            set
            {
                _portSetUp = value;
            }
        }
        public DriverType DriType
        {
            get
            {
                return _driverType;
            }
            private set
            {
                _driverType = value;
            }
        }

        public bool IsClose
        {
            get
            {
                return _serialPort.IsOpen == false;
            }
        }
        public bool IsConnect
        {
            get
            {
                return _isConnect;
            }
            private set
            {
                _isConnect = value;
            }
        }

        public int PDU
        {
            get
            {
                return _pdu;
            }

            set
            {
                _pdu = value;
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

        /// <summary>
        /// COM口设置并打开
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                if (_timeOut.TimeOutSet < 1000)
                    _timeOut.TimeOutSet = 1000;
                _serialPort.PortName = _portSetUp.ComPort;
                _serialPort.BaudRate = _portSetUp.BuadRate;
                _serialPort.DataBits = _portSetUp.DataBit;
                _serialPort.StopBits = _portSetUp.StopBit;
                _serialPort.Parity = _portSetUp.OddEvenCheck;
                _serialPort.WriteTimeout = (int)_timeOut.TimeOutSet;
                _serialPort.ReadTimeout = (int)_timeOut.TimeOutSet;
                _serialPort.Open();
                _isConnect = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorLog("ModbusRTU Connect Error:" + ex.Message);
                _isConnect = false;
                return false;
            }
        }

        public bool DisConnect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _isConnect = false;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorLog("Modbus DisConnect Error:" + ex.Message);
                _isConnect = false;
                return false;
            }
        }
        /// <summary>
        /// 读数据报文头
        /// 由设备地址，功能码，其实地址，数量构成
        /// 返回带校验8位字节数组
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="func"></param>
        /// <param name="startAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] readHeader(byte slaveId, byte func, ushort startAddress, ushort count)
        {
            byte[] sendBytes = new byte[8];
            sendBytes[0] = slaveId;
            sendBytes[1] = func;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);//起始位为0，偏移1位
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            byte[] countBytes = BitConverter.GetBytes(count);
            sendBytes[4] = countBytes[1];//高位在前
            sendBytes[5] = countBytes[0];//低位在后
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            sendBytes[6] = CRCBytes[0];
            sendBytes[7] = CRCBytes[1];
            return sendBytes;
        }
        public delegate byte[] GetWriteHeader(byte slaveID, ushort startAddress, byte funcCode, byte[] datas);
        /// <summary>
        /// 写单个线圈或寄存器，包括：
        /// 从地址 功能码 地址位 数据位 CRC共8位bytes
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="funcCode"></param>
        /// <param name="datas"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] writeSigHeader(byte slaveID, ushort startAddress, byte funcCode, byte[] datas)
        {
            byte[] sendBytes = new byte[8];
            sendBytes[0] = slaveID;
            sendBytes[1] = funcCode;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            sendBytes[4] = datas[1];
            sendBytes[5] = datas[0];
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            sendBytes[6] = CRCBytes[0];
            sendBytes[7] = CRCBytes[1];
            return sendBytes;
        }
        /// <summary>
        /// 写多个线圈或寄存器，包括：
        /// 从地址 功能码 地址位 数量 字节长度 数据数组 CRC校验组成 共9+length（发送字节数）
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="funcCode"></param>
        /// <param name="datas"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] writeMulHeader(byte slaveID, ushort startAddress, byte funcCode, byte[] datas)
        {
            byte[] sendBytes = new byte[9 + datas.Length];
            sendBytes[0] = slaveID;
            sendBytes[1] = funcCode;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            byte[] CountBytes = BitConverter.GetBytes((ushort)datas.Length);
            sendBytes[4] = CountBytes[1];
            sendBytes[5] = CountBytes[0];

            sendBytes[6] = (byte)datas.Length;
            Array.ConstrainedCopy(datas, 0, sendBytes, 7, datas.Length);

            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            sendBytes[sendBytes.Length - 2] = CRCBytes[0];
            sendBytes[sendBytes.Length - 1] = CRCBytes[1];
            return sendBytes;
        }
        /// <summary>
        /// 读数据
        /// 0x01读线圈，地址：00001-09999，类型：bit
        /// 0x02读输入状态，地址：10001-19999，类型：bit
        /// 0x03读保持寄存器，地址：40001-49999，类型：Word
        /// 0x04读输入寄存器，地址：30001-39999，类型：Word
        /// </summary>
        /// <returns>返回带CRC校验8位字节数组</returns>
        object _async = new object();
        private byte[] readBytes(byte slaveID, ushort startAddress, byte funcCode, ushort count)
        {
            try
            {
                if (IsConnect)
                {
                    byte byteCount = Function.GetReadBytesCount(funcCode, count);
                    if (byteCount == 0)
                    {
                        _log.ErrorLog("Modbus 读取功能码不正常");
                        return null;
                    }
                    byte[] sendBytes = readHeader(slaveID, funcCode, startAddress, count);
                    lock (_async)
                    {
                        byte[] receiveBytes = new byte[3 + byteCount + 2];
                        byte[] dataBytes = new byte[byteCount];
                        byte errorFuncCode = (byte)(0x80 + funcCode);
                        _serialPort.Write(sendBytes, 0, sendBytes.Length);
                        Thread.Sleep(10);
                        int index = 0;
                        bool continueFlag = true;
                        _timeOut.InitAndClear();
                        /*----------------------------------------
                        *循环找头：
                        * 先读一个字节判断是否为SlaveID
                        * 如果是,则开启循环
                        * 则再读一个字节判断是否为功能码或者是错误码
                        * 判断是，则指针index+2并跳出循环
                        * 若第二个字节等于SlaveID则复制给头
                        * 否则将头置0
                     ------------------------------------------ */
                        while (_timeOut.TimeOutFlag & continueFlag)
                        {
                            if (index < 2)
                            {
                                _serialPort.Read(receiveBytes, 0, 1);
                                while (receiveBytes[0] == slaveID)
                                {
                                    _serialPort.Read(receiveBytes, 1, 1);
                                    if (receiveBytes[1] == funcCode || receiveBytes[1] == errorFuncCode)
                                    {
                                        index += 2;
                                        break;
                                    }
                                    else if (receiveBytes[1] == slaveID)
                                    {
                                        receiveBytes[0] = receiveBytes[1];
                                    }
                                    else
                                    {
                                        receiveBytes[0] = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (receiveBytes[1] == funcCode)
                                {
                                    index += _serialPort.Read(receiveBytes, index, byteCount + 3);
                                    continueFlag = index == receiveBytes.Length ? false : true;
                                }
                                else if (receiveBytes[1] == errorFuncCode)
                                {
                                    index += _serialPort.Read(receiveBytes, index, 3);
                                    continueFlag = index == 5 ? false : true;
                                }
                            }

                            _timeOut.EndTime = DateTime.Now;
                        }

                        //判断是否超时，并复位
                        if (_timeOut.TimeOutFlag)
                        {
                            _timeOut.LogTimeOutError();
                            return null;
                        }

                        //获取正确报文并处理
                        if (receiveBytes[1] == funcCode)
                        {
                            if (!Utility.CheckSumCRC(receiveBytes, receiveBytes.Length))
                            {
                                _log.ErrorLog("Modbus CRC校验错误");
                                return null;
                            }
                            Array.ConstrainedCopy(receiveBytes, 3, dataBytes, 0, byteCount);
                            return dataBytes;
                        }
                        else if (receiveBytes[1] == errorFuncCode)
                        {
                            if (!Utility.CheckSumCRC(receiveBytes, 5))
                            {
                                _log.ErrorLog("Modbus CRC校验错误");
                                return null;
                            }
                            _log.ErrorLog(string.Format("Modbus {0} ", Function.GetErrorString(receiveBytes[2])));
                        }
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _log.ErrorLog(string.Format("Modbus {0} ", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 0x05强制单个线圈
        /// 0x06预置单个寄存器
        /// 0x0F强制多个线圈
        /// 0x10预置多个寄存器
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="funcCode"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        private int writeDatas(byte slaveID, byte funcCode, ushort startAddress, byte[] datas, ushort count, GetWriteHeader getHeader)
        {
            try
            {
                if (_isConnect)
                {
                    byte errorFuncCode = (byte)(0x80 + funcCode);
                    byte[] sendBytes = getHeader(slaveID, startAddress, funcCode, datas);

                    lock (_async)
                    {
                        _serialPort.Write(sendBytes, 0, sendBytes.Length);
                        Thread.Sleep(10);

                        int index = 0;
                        bool continueFlag = true;
                        _timeOut.InitAndClear();
                        byte[] receiveBytes = new byte[8];

                        /*----------------------------------------
                         *循环找头：
                         * 先读一个字节判断是否为SlaveID
                         * 如果是,则开启循环
                         * 则再读一个字节判断是否为功能码或者是错误码
                         * 判断是，则指针index+2并跳出循环
                         * 若第二个字节等于SlaveID则复制给头
                         * 否则将头置0
                         ------------------------------------------ */

                        while (_timeOut.TimeOutFlag & continueFlag)
                        {
                            if (index < 2)
                            {
                                _serialPort.Read(receiveBytes, 0, 1);
                                while (receiveBytes[0] == slaveID)
                                {
                                    _serialPort.Read(receiveBytes, 1, 1);
                                    if (receiveBytes[1] == funcCode || receiveBytes[1] == errorFuncCode)
                                    {
                                        index += 2;
                                        break;
                                    }
                                    else if (receiveBytes[1] == slaveID)
                                    {
                                        receiveBytes[0] = receiveBytes[1];
                                    }
                                    else
                                    {
                                        receiveBytes[0] = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (receiveBytes[1] == funcCode)
                                {
                                    index += _serialPort.Read(receiveBytes, index, 6);
                                    continueFlag = index == 8 ? false : true;
                                }
                                else if (receiveBytes[1] == errorFuncCode)
                                {
                                    index += _serialPort.Read(receiveBytes, index, 3);
                                    continueFlag = index == 5 ? false : true;
                                }
                            }
                            _timeOut.EndTime = DateTime.Now;
                        }

                        if (_timeOut.TimeOutFlag)
                        {
                            _timeOut.LogTimeOutError();
                            return -1;
                        }

                        //获取正确报文并处理
                        if (receiveBytes[1] == funcCode)
                        {
                            if (Utility.CheckSumCRC(receiveBytes, receiveBytes.Length))
                            {
                                return 1;
                            }
                            else
                            {
                                _log.ErrorLog("Modbus CRC校验错误");
                                return -1;
                            }

                        }
                        else if (receiveBytes[1] == errorFuncCode)
                        {
                            if (Utility.CheckSumCRC(receiveBytes, 5))
                            {
                                _log.ErrorLog(string.Format("Modbus {0} ", Function.GetErrorString(receiveBytes[2])));
                            }
                            else
                            {
                                _log.ErrorLog("Modbus CRC校验错误");
                            }
                            return -1;
                        }
                        return -1;
                    }

                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                _log.ErrorLog(string.Format("Modbus {0} ", ex.Message));
                return -1;
            }

        }
        public string GetAddress(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public DeviceAddress GetDeviceAddress(string address)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(DeviceAddress deviceAddress, ushort length)
        {
            return readBytes((byte)deviceAddress.SlaveID, (ushort)deviceAddress.Address, (byte)deviceAddress.FuctionNumber, length);
        }
        public Item<bool> ReadBool(DeviceAddress deviceAddress)
        {
            var datas = ReadBytes(deviceAddress, 1);
            return datas == null ? Item<bool>.Default :
                new Item<bool>() { Vaule = NetConvert.ByteToBool(datas[0], 0), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length)
        {
            var datas = ReadBytes(deviceAddress, length);
            bool[] bdatas = NetConvert.BytesToBools(datas, length);
            return NetConvert.ToItems(bdatas, 0, length);
        }
        public Item<TResult> ReadData<TResult>(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<TResult>[] ReadDatas<TResult>(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }


        public Item<short> ReadShort(DeviceAddress deviceAddress)
        {

            var datas = ReadBytes(deviceAddress, 1);
            return datas == null ? Item<short>.Default :
                new Item<short>() { Vaule = UNetConvert.BytesToShort(datas, 0, deviceAddress.ByteOrder), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = ReadBytes(deviceAddress, 1);
            var bdatas = UNetConvert.BytesToShorts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }
        public Item<ushort> ReadUShort(DeviceAddress deviceAddress)
        {
            var datas = ReadBytes(deviceAddress, 1);
            return datas == null ? Item<ushort>.Default :
                new Item<ushort>() { Vaule = UNetConvert.BytesToUShort(datas, 0, deviceAddress.ByteOrder), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = ReadBytes(deviceAddress, 1);
            var bdatas = UNetConvert.BytesToUShorts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }
        public Item<int> ReadInt(DeviceAddress deviceAddress)
        {
            var datas = ReadBytes(deviceAddress, 2);
            return datas == null ? Item<int>.Default :
                new Item<int>()
                {
                    Vaule = UNetConvert.BytesToInt(datas, 0, deviceAddress.ByteOrder),
                    UpdateTime = DateTime.Now,
                    Quality = QUALITIES.QUALITY_GOOD
                };

        }

        public Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = ReadBytes(deviceAddress, (ushort)(2 * length));
            int[] bdatas = UNetConvert.BytesToInts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }
        public Item<uint> ReadUInt(DeviceAddress deviceAddress)
        {
            var datas = ReadBytes(deviceAddress, 2);
            return datas == null ? Item<uint>.Default :
                new Item<uint>()
                {
                    Vaule = UNetConvert.BytesToUInt(datas, 0, deviceAddress.ByteOrder),
                    UpdateTime = DateTime.Now,
                    Quality = QUALITIES.QUALITY_GOOD
                };
        }

        public Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = ReadBytes(deviceAddress, (ushort)(2 * length));
            uint[] bdatas = UNetConvert.BytesToUInts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }

        public Item<float> Readfloat(DeviceAddress deviceAddress)
        {
            var datas = ReadBytes(deviceAddress, 2);
            return datas == null ? Item<float>.Default :
                      new Item<float>()
                      {
                          Vaule = UNetConvert.BytesToFloat(datas, 0, deviceAddress.ByteOrder),
                          UpdateTime = DateTime.Now,
                          Quality = QUALITIES.QUALITY_GOOD
                      };
        }

        public Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            var datas = ReadBytes(deviceAddress, (ushort)(2 * length));
            float[] bdatas = UNetConvert.BytesToFloats(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }

        public Item<string> ReadString(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }
        public int WriteBool(DeviceAddress deviceAddress, bool datas)
        {
            byte[] sendBytes = new byte[2];
            if (datas)
                sendBytes[1] = 0xFF; 
            GetWriteHeader getHeader = writeSigHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber,(ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteBools(DeviceAddress deviceAddress, bool[] datas)
        {
            byte[] sendBytes = NetConvert.BoolstoBytes(datas);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteByte(DeviceAddress deviceAddress, byte datas)
        {
            throw new NotImplementedException();
        }

        public int WriteBytes(DeviceAddress deviceAddress, byte[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteData<T>(DeviceAddress deviceAddress, T datas)
        {
            throw new NotImplementedException();
        }

        public int WriteDatas<T>(DeviceAddress deviceAddress, T[] datas)
        {
            throw new NotImplementedException();
        }
        public int WriteShort(DeviceAddress deviceAddress, short datas)
        {
            byte[] sendBytes = UNetConvert.ShortToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeSigHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteUShort(DeviceAddress deviceAddress, ushort datas)
        {
            byte[] sendBytes = UNetConvert.UShortToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeSigHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteShorts(DeviceAddress deviceAddress, short[] datas)
        {
            byte[] sendBytes = UNetConvert.ShortsToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas)
        {
            byte[] sendBytes = UNetConvert.UShortsToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

       
        public int WriteInt(DeviceAddress deviceAddress, int datas)
        {
            byte[] sendBytes = UNetConvert.IntToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteInts(DeviceAddress deviceAddress, int[] datas)
        {
            byte[] sendBytes = UNetConvert.IntsToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteUInt(DeviceAddress deviceAddress, uint datas)
        {
            byte[] sendBytes = UNetConvert.UIntToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteUInts(DeviceAddress deviceAddress, uint[] datas)
        {
            byte[] sendBytes = UNetConvert.UIntsToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteFloat(DeviceAddress deviceAddress, float datas)
        {
            byte[] sendBytes = UNetConvert.FloatToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteFloats(DeviceAddress deviceAddress, float[] datas)
        {
            byte[] sendBytes = UNetConvert.FloatsToBytes(datas, deviceAddress.ByteOrder);
            GetWriteHeader getHeader = writeMulHeader;
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
        }

        public int WriteString(DeviceAddress deviceAddress, string datas)
        {
            throw new NotImplementedException();
        }

        public int WriteStrings(DeviceAddress deviceAddress, string[] datas)
        {
            throw new NotImplementedException();
        }

 

  
        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
