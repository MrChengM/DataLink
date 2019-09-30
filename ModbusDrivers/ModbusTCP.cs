using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModbusDrivers
{
    public sealed class ModbusTCP:IPLCDriver
    {
        private DriverType _driverType = DriverType.Ethernet;
        private EthernetSetUp _ethernetSetUp =new EthernetSetUp();
        private Socket _socket;
        private TimeOut _timeOut;
        private bool _isConnect = false;
        // private bool _isClose = true;
        private ILog _log;
        private int _pdu = 252;

        public int PDU
        {
            get
            {
               return _pdu;
            }

            set
            {
                _pdu=value;
            }
        }

        public DriverType DriType
        {
            get
            {
               return _driverType;
            }
        }

        public bool IsConnect
        {
            get
            {
                return _isConnect;
            }
        }

        public bool IsClose
        {
            get
            {
                return _socket==null||_socket.Connected==false;
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
                _timeOut=value;
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
        public ModbusTCP() { }
        public ModbusTCP(EthernetSetUp ethernetSetUp, TimeOut timeOut,ILog log)
        {
            _ethernetSetUp = ethernetSetUp;
            _timeOut = timeOut;
            _log = log;
        }

        public bool Connect()
        {
            if(_socket==null)
               _socket=new Socket(SocketType.Stream, _ethernetSetUp.ProtocolType) ;
            _socket.SendTimeout = (int)TimeOut.TimeOutSet;
            _socket.ReceiveTimeout = (int)TimeOut.TimeOutSet;
            IPAddress ipaddress;
            if (IPAddress.TryParse(_ethernetSetUp.IPAddress,out ipaddress))
            {
                _socket.Connect(ipaddress, _ethernetSetUp.PortNumber);
                return _isConnect = true;
            }
            else
            {
                _log.ErrorLog("IP地址无效");
                return _isConnect = false;
            }
        }

        public bool DisConnect()
        {
            _socket.Disconnect(false);
             _isConnect = false;
            return true;
        }

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
            return sendBytes;
        }
        public DeviceAddress GetDeviceAddress(string address)
        {
            throw new NotImplementedException();
        }

        public string GetAddress(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<bool> ReadBool(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<short> ReadShort(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<ushort> ReadUShort(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<int> ReadInt(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<uint> ReadUInt(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<float> Readfloat(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<string> ReadString(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<TResult> ReadData<TResult>(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<TResult>[] ReadDatas<TResult>(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public int WriteBool(DeviceAddress deviceAddress, bool datas)
        {
            throw new NotImplementedException();
        }

        public int WriteByte(DeviceAddress deviceAddress, byte datas)
        {
            throw new NotImplementedException();
        }

        public int WriteShort(DeviceAddress deviceAddress, short datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUShort(DeviceAddress deviceAddress, ushort datas)
        {
            throw new NotImplementedException();
        }

        public int WriteInt(DeviceAddress deviceAddress, int datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUInt(DeviceAddress deviceAddress, uint datas)
        {
            throw new NotImplementedException();
        }

        public int WriteFloat(DeviceAddress deviceAddress, float datas)
        {
            throw new NotImplementedException();
        }

        public int WriteString(DeviceAddress deviceAddress, string datas)
        {
            throw new NotImplementedException();
        }

        public int WriteData<T>(DeviceAddress deviceAddress, T datas)
        {
            throw new NotImplementedException();
        }

        public int WriteBools(DeviceAddress deviceAddress, bool[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteBytes(DeviceAddress deviceAddress, byte[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteShorts(DeviceAddress deviceAddress, short[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteInts(DeviceAddress deviceAddress, int[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUInts(DeviceAddress deviceAddress, uint[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteFloats(DeviceAddress deviceAddress, float[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteStrings(DeviceAddress deviceAddress, string[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteDatas<T>(DeviceAddress deviceAddress, T[] datas)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
