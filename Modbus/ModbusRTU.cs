using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using System.IO.Ports;

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
        private int _timeOut = 3000;
        private bool _isConnect = false;
       // private bool _isClose = true;
        private ILog _log;
        private int _pdu = 248;

        public ModbusRTUSalve() { }

        public ModbusRTUSalve(SerialportSetUp portSetUp,int timeOut, ILog log)
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

        public int TimeOut
        {
            get
            {
                return _timeOut;
            }
            set
            {
                _timeOut = value >= 1000 ? value: 1000 ;
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
                _serialPort.PortName = _portSetUp.ComPort;
                _serialPort.BaudRate = _portSetUp.BuadRate;
                _serialPort.DataBits = _portSetUp.DataBit;
                _serialPort.StopBits = _portSetUp.StopBit;
                _serialPort.Parity = _portSetUp.OddEvenCheck;
                _serialPort.WriteTimeout = _timeOut;
                _serialPort.ReadTimeout = _timeOut;
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
        public void Dispose()
        {
            throw new NotImplementedException();
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

     
    }
}
