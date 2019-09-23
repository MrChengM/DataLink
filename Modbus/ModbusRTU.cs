using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using System.IO.Ports;
using System.Threading;

namespace Modbus
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
        private int _pdu = 248;

        public ModbusRTUSalve() { }

        public ModbusRTUSalve(SerialportSetUp portSetUp,TimeOut timeOut, ILog log)
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
                _portSetUp =value;
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
                return _serialPort.IsOpen==false;
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
                _isConnect=value;
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
                _timeOut = value  ;
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
                 _log=value;
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
            catch(Exception ex)
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
            catch(Exception ex)
            {
                Log.ErrorLog("ModbusRTU DisConnect Error:" + ex.Message);
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
        private byte[] readHead(byte slaveId,byte func,ushort startAddress, ushort count)
        {
            byte[] sendBytes = new byte[8];
            sendBytes[0] = slaveId;
            sendBytes[1] = func;
            byte[] addressBytes = BitConverter.GetBytes(startAddress);
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            byte[] countBytes= BitConverter.GetBytes(count);
            sendBytes[4] = countBytes[1];//高位在前
            sendBytes[5] = countBytes[0];//低位在后
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            sendBytes[6] = CRCBytes[0];
            sendBytes[7] = CRCBytes[1];
            return sendBytes;
        }

        /// <summary>
        /// 功能码01，读线圈状态
        /// 地址：00001-09999，类型：bit
        /// 最大个数2000线圈
        /// </summary>
        /// <returns>返回带CRC校验8位字节数组</returns>
        object _async = new object();
        private byte[] readCoil(byte slaveID, ushort startAddress, ushort count)
        {
            
            try
            {
                if (IsConnect)
                {
                    byte[] sendBytes = readHead(slaveID, (byte)FucthionCode.ReadCoil, startAddress, count);
                    lock (_async)
                    {
                        byte byteCount =(byte)((count % 8 == 0) ? count / 8 : (count / 8 + 1));
                        byte[] receiveBytes = new byte[3 + byteCount + 2];
                        byte[] dataBytes = new byte[byteCount];
                        _serialPort.Write(sendBytes, 0, sendBytes.Length);
                        Thread.Sleep(100);
                        _timeOut.StartTime = DateTime.Now;
                        _timeOut.EndTime = DateTime.Now;
                        int index =0;
                        bool continueFlag = true;
                        /*---------------------------------------------------------
                         * 先找收到数据报文头（从站地址，功能码），读取3个字节
                         * 是否符合正确报文数据
                         * 若为正常响应，读取剩下的Length-2长度
                         * 若为不正常响应，读取剩下3位长度
                        ----------------------------------------------------------*/
                        while (_timeOut.TimeOutFlag&continueFlag)
                        {
                            if (index < 2)
                            {
                                _serialPort.Read(receiveBytes, index, 2);
                                if (receiveBytes[0] == slaveID)
                                {
                                    switch (receiveBytes[1])
                                    {
                                        case (byte)FucthionCode.ReadCoil:
                                            index += 2;
                                            break;
                                        case (0x80 + (byte)FucthionCode.ReadCoil):
                                            index += 2;
                                            break;
                                    }
                                }
                            }
                            if(receiveBytes[1]== (byte)FucthionCode.ReadCoil)
                            {
                                index += _serialPort.Read(receiveBytes, index, byteCount +3);
                                continueFlag = index == receiveBytes.Length ? false : true;
                            }
                            if (receiveBytes[1] == 0x80 + (byte)FucthionCode.ReadCoil)
                            {
                                index += _serialPort.Read(receiveBytes, index,  3);
                                continueFlag = index == 5 ? false : true;
                            }
                            _timeOut.EndTime = DateTime.Now;
                        }

                    }
 
                }
                else
                {
                    return default(byte[]);
                }
            }
            catch
            {

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

        public Item<bool> ReadBool(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<byte> ReadByte(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<byte>[] ReadBytes(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<TResult> ReadData<TResult>(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<TResult>[] ReadDatas<TResult>(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<float> Readfloat(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<int> ReadInt(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<short> ReadShort(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<string> ReadString(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<uint> ReadUInt(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<ushort> ReadUShort(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public int WriteBool(DeviceAddress deviceAddress, bool datas)
        {
            throw new NotImplementedException();
        }

        public int WriteBools(DeviceAddress deviceAddress, bool[] datas)
        {
            throw new NotImplementedException();
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

        public int WriteFloat(DeviceAddress deviceAddress, float datas)
        {
            throw new NotImplementedException();
        }

        public int WriteFloats(DeviceAddress deviceAddress, float[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteInt(DeviceAddress deviceAddress, int datas)
        {
            throw new NotImplementedException();
        }

        public int WriteInts(DeviceAddress deviceAddress, int[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteShort(DeviceAddress deviceAddress, short datas)
        {
            throw new NotImplementedException();
        }

        public int WriteShorts(DeviceAddress deviceAddress, short[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteString(DeviceAddress deviceAddress, string datas)
        {
            throw new NotImplementedException();
        }

        public int WriteStrings(DeviceAddress deviceAddress, string[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUInt(DeviceAddress deviceAddress, uint datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUInts(DeviceAddress deviceAddress, uint[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUShort(DeviceAddress deviceAddress, ushort datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
