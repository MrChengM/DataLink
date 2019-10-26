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
        protected abstract byte[] getReadHeader(byte slaveId, byte func, ushort startAddress, ushort byteCount);

        /// <summary>
        /// 强制单个线圈,
        /// 功能码为05
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="funcCode"></param>
        /// <param name="datas"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected abstract byte[] getWriteSigCoilHeader(byte slaveID, ushort startAddress,bool value);

        /// <summary>
        /// 写单个寄存器,
        /// 功能码为05
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="value">byte数组长度为2</param>
        /// <returns></returns>
        protected abstract byte[] getWriteSigRegisterHeader(byte slaveID, ushort startAddress, byte[] value);

        /// <summary>
        /// 强制多个线圈
        /// 功能码为0x10（16)
        /// </summary>
        /// <param name="slaveID">子站地址</param>
        /// <param name="startAddress">开始地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        protected abstract byte[] getWriteMulCoilHeader(byte slaveID, ushort startAddress, bool[] value);
        /// <summary>
        /// 写多个寄存器数据,
        /// 功能码为0x10（16)
        /// </summary>
        /// <param name="slaveID">子站地址</param>
        /// <param name="startAddress">开始地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        protected abstract byte[] getWriteMulRegisterHeader(byte slaveID, ushort startAddress, byte[] value);
     
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
        /// 写报文数组
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <returns></returns>
        protected abstract int writeBytes(byte[] sendBytes);
            
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
            throw new NotImplementedException();
        }
        private byte[] readBool(DeviceAddress deviceAddress, ushort length)
        {
            byte salveID = (byte)deviceAddress.Area;
            ushort startAddress;
            byte funcCode = (byte)Function.GetReadFunctionCode(deviceAddress.Address, out startAddress);
            byte[] datas = null;
            if (funcCode == 1 || funcCode == 2)
            {
                datas = readBytes(salveID, startAddress, funcCode, length);
            }
            return datas;
        }
        public Item<bool> ReadBool(DeviceAddress deviceAddress)
        {
            var datas = readBool(deviceAddress, 1);
            return datas == null ? Item<bool>.Default :
                new Item<bool>() { Vaule = NetConvert.ByteToBool(datas[0], 0), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readBool(deviceAddress, 1);
            var bdatas = NetConvert.BytesToBools(datas, length);
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

        private byte[] readRegister(DeviceAddress deviceAddress, int lenght)
        {
            byte salveID = (byte)deviceAddress.Area;
            ushort startAddress;
            byte funcCode = (byte)Function.GetReadFunctionCode(deviceAddress.Address, out startAddress);
            if (funcCode == 3 || funcCode == 4)
            {

                return UnsafeNetConvert.HiLoBytesPerversion( readBytes(salveID, startAddress, funcCode, (ushort)lenght));
            }
            else
            {
                return null;
            }
        }

        public Item<short> ReadShort(DeviceAddress deviceAddress)
        {

            var datas = readRegister(deviceAddress, 1);
            return datas == null ? Item<short>.Default :
                new Item<short>() { Vaule = UnsafeNetConvert.BytesToShort(datas, 0, deviceAddress.ByteOrder), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length)
        {
            
            var datas = readRegister(deviceAddress, length);
            var bdatas = UnsafeNetConvert.BytesToShorts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }
        public Item<ushort> ReadUShort(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 1);
            return datas == null ? Item<ushort>.Default :
                new Item<ushort>() { Vaule = UnsafeNetConvert.BytesToUShort(datas, 0, deviceAddress.ByteOrder), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, 1);
            var bdatas = UnsafeNetConvert.BytesToUShorts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }
        public Item<int> ReadInt(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 2);
            return datas == null ? Item<int>.Default :
                new Item<int>()
                {
                    Vaule = UnsafeNetConvert.BytesToInt(datas, 0, deviceAddress.ByteOrder),
                    UpdateTime = DateTime.Now,
                    Quality = QUALITIES.QUALITY_GOOD
                };
        }

        public Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, 2*length);
            var bdatas = UnsafeNetConvert.BytesToInts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }
        public Item<uint> ReadUInt(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 2);
            return datas == null ? Item<uint>.Default :
                new Item<uint>()
                {
                    Vaule = UnsafeNetConvert.BytesToUInt(datas, 0, deviceAddress.ByteOrder),
                    UpdateTime = DateTime.Now,
                    Quality = QUALITIES.QUALITY_GOOD
                };
        }

        public Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, 2*length);
            var bdatas = UnsafeNetConvert.BytesToUInts(datas, 0, length, deviceAddress.ByteOrder);
            return NetConvert.ToItems(bdatas, 0, length);
        }

        public Item<float> Readfloat(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 2);
            return datas == null ? Item<float>.Default :
                      new Item<float>()
                      {
                          Vaule = UnsafeNetConvert.BytesToFloat(datas, 0, deviceAddress.ByteOrder),
                          UpdateTime = DateTime.Now,
                          Quality = QUALITIES.QUALITY_GOOD
                      };
        }

        public Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, 2*length);
            var bdatas = UnsafeNetConvert.BytesToFloats(datas, 0, length, deviceAddress.ByteOrder);
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
            ushort startAddress;
            if (Function.EnableWriteCoil(deviceAddress.Address,out startAddress))
            {
                byte[] sendBytes = getWriteSigCoilHeader((byte)deviceAddress.Area, startAddress, datas);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }

        }

        public int WriteBools(DeviceAddress deviceAddress, bool[] datas)
        {
            ushort startAddress;
            if (Function.EnableWriteCoil(deviceAddress.Address, out startAddress))
            {
                byte[] sendBytes = getWriteMulCoilHeader((byte)deviceAddress.Area, (ushort)deviceAddress.Address, datas);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
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
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.ShortToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteSigRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteUShort(DeviceAddress deviceAddress, ushort datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.UShortToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteSigRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteShorts(DeviceAddress deviceAddress, short[] datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.ShortsToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.UShortsToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }


        public int WriteInt(DeviceAddress deviceAddress, int datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.IntToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteInts(DeviceAddress deviceAddress, int[] datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.IntsToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteUInt(DeviceAddress deviceAddress, uint datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.UIntToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteUInts(DeviceAddress deviceAddress, uint[] datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.UIntsToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteFloat(DeviceAddress deviceAddress, float datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.FloatToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteFloats(DeviceAddress deviceAddress, float[] datas)
        {
            ushort startAddress;
            if (Function.EnableWriteRegister(deviceAddress.Address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.FloatsToBytes(datas, deviceAddress.ByteOrder);
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.Area, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
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
