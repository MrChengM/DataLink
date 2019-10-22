using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusDrivers
{
    public abstract class ModbusSalve: IPLCDriver
    {
        // 内部成员定义
        private DriverType _driverType;
        private TimeOut _timeOut;
        private bool _isConnect = false;
        private ILog _log;
        private int _pdu = 252;

        public ModbusSalve() { }

        public DriverType DriType
        {
            get
            {
                return _driverType;
            }
            protected set
            {
                _driverType = value;
            }
        }

        public virtual bool IsClose
        {
            get
            {
               return default(bool); 
            }
        }
        public bool IsConnect
        {
            get
            {
                return _isConnect;
            }
            protected set
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

        public abstract bool Connect();
        public abstract bool DisConnect();

        /// <summary>
        /// 读数据报文头
        /// 由设备地址，功能码，其实地址，数量构成
        /// 返回带校验8位字节数组
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="func"></param>
        /// <param name="startAddress"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        protected abstract byte[] readHeader(byte slaveId, byte func, ushort startAddress, ushort byteCount);
       
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
        protected abstract byte[] writeSigHeader(byte slaveID, ushort startAddress, byte funcCode, byte[] datas);

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
        protected abstract byte[] writeMulHeader(byte slaveID, ushort startAddress, byte funcCode, byte[] datas);
        
        /// <summary>
        /// 读数据
        /// 0x01读线圈，地址：00001-09999，类型：bit
        /// 0x02读输入状态，地址：10001-19999，类型：bit
        /// 0x03读保持寄存器，地址：40001-49999，类型：Word
        /// 0x04读输入寄存器，地址：30001-39999，类型：Word
        /// </summary>
        /// <returns>返回带CRC校验8位字节数组</returns>
        protected abstract byte[] readBytes(byte slaveID, ushort startAddress, byte funcCode, ushort count);

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
        protected abstract int writeDatas(byte slaveID, byte funcCode, ushort startAddress, byte[] datas, ushort count, GetWriteHeader getHeader);
            
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
            return writeDatas((byte)deviceAddress.SlaveID, (byte)deviceAddress.FuctionNumber, (ushort)deviceAddress.Address, sendBytes, (ushort)sendBytes.Length, getHeader);
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

        public abstract void Dispose();
      
    }

}
