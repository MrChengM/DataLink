using DataServer;
using DataServer.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Data;

namespace ModbusDrivers.Client
{
    public abstract class ModbusClient: IPLCDriver,IDisposable
    {
        // 内部成员定义
        ////private TimeOut _timeOut;
        //private int _connectTimeOut;
        //private int _requestTimeOut;
        //private int _retryTimes;

        private bool _isConnect = false;
        private ILog _log;
        private int _pdu = 252;

        

        public ModbusClient() { }

        public ByteOrder Order { get; set; }

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

        public int Id { get; set; } = 1;
        public int ConnectTimeOut { get ; set ; }
        public int RequestTimeOut { get ; set ; }
        public int RetryTimes { get ; set; }
        public string Name { get ; set; }

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

        public DeviceAddress GetDeviceAddress(string address) 
        {
            var result = new DeviceAddress(Id, int.Parse(address));
            return result;
        }

        //public byte[] ReadBytes(DeviceAddress deviceAddress, ushort length)
        //{
        //    throw new NotImplementedException();
        //}
        private byte[] readBool(DeviceAddress deviceAddress, ushort length)
        {
            byte salveID = (byte)deviceAddress.AreaID;
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
            return datas == null ? Item<bool>.CreateDefault() :
                new Item<bool>() { Vaule = NetConvert.GetBit(datas[0], 0), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readBool(deviceAddress, length);
            var bdatas = NetConvert.BytesToBools(datas, length);
            return NetConvertExtension.ToItems(bdatas, 0, length);
        }

        private byte[] readRegister(DeviceAddress deviceAddress, int lenght)
        {
            byte salveID = (byte)deviceAddress.AreaID;
            ushort startAddress;
            byte funcCode = (byte)Function.GetReadFunctionCode(deviceAddress.Address, out startAddress);
            if (funcCode == 3 || funcCode == 4 )
            {
                //若byteOrder为bigEndian不需要颠倒word高低位
                   return(Order == ByteOrder.BigEndian)? readBytes(salveID, startAddress, funcCode, (ushort)lenght)
                    : UnsafeNetConvert.BytesPerversion(readBytes(salveID, startAddress, funcCode, (ushort)lenght));
            }
            else
            {
                return null;
            }
        }

        public Item<short> ReadShort(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 1);
            return datas == null ? Item<short>.CreateDefault() :
                new Item<short>() { Vaule = UnsafeNetConvert.BytesToShort(datas, 0, Order), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length)
        {
            
            var datas = readRegister(deviceAddress, length);
            var bdatas = UnsafeNetConvert.BytesToShorts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(bdatas, 0, length);
        }
        public Item<ushort> ReadUShort(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 1);
            return datas == null ? Item<ushort>.CreateDefault() :
                new Item<ushort>() { Vaule = UnsafeNetConvert.BytesToUShort(datas, 0, Order), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, length);
            var bdatas = UnsafeNetConvert.BytesToUShorts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(bdatas, 0, length);
        }
        public Item<int> ReadInt(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 2);
            return datas == null ? Item<int>.CreateDefault() :
                new Item<int>()
                {
                    Vaule = UnsafeNetConvert.BytesToInt(datas, 0, Order),
                    UpdateTime = DateTime.Now,
                    Quality = QUALITIES.QUALITY_GOOD
                };
        }

        public Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, 2*length);
            var bdatas = UnsafeNetConvert.BytesToInts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(bdatas, 0, length);
        }
        public Item<uint> ReadUInt(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 2);
            return datas == null ? Item<uint>.CreateDefault() :
                new Item<uint>()
                {
                    Vaule = UnsafeNetConvert.BytesToUInt(datas, 0, Order),
                    UpdateTime = DateTime.Now,
                    Quality = QUALITIES.QUALITY_GOOD
                };
        }

        public Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, 2*length);
            var bdatas = UnsafeNetConvert.BytesToUInts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(bdatas, 0, length);
        }

        public Item<float> Readfloat(DeviceAddress deviceAddress)
        {
            var datas = readRegister(deviceAddress, 2);
            return datas == null ? Item<float>.CreateDefault() :
                      new Item<float>()
                      {
                          Vaule = UnsafeNetConvert.BytesToFloat(datas, 0, Order),
                          UpdateTime = DateTime.Now,
                          Quality = QUALITIES.QUALITY_GOOD
                      };
        }

        public Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readRegister(deviceAddress, 2*length);
            var bdatas = UnsafeNetConvert.BytesToFloats(datas, 0, length, Order);
            return NetConvertExtension.ToItems(bdatas, 0, length);
        }

        public Item<string> ReadString(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        /***********************************
        单个word数据写入，后面数据进行高低位交换；
        多个word数据写入，在前面进行数据判断，是否进行翻转；
        ***********************************/
        public int WriteBool(DeviceAddress deviceAddress, bool datas, int offset = 0)
        {
            ushort startAddress;
            int address = deviceAddress.Address + offset;
            if (Function.EnableWriteCoil(address, out startAddress))
            {
                byte[] sendBytes = getWriteSigCoilHeader((byte)deviceAddress.AreaID, startAddress, datas);
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
                byte[] sendBytes = getWriteMulCoilHeader((byte)deviceAddress.AreaID, (ushort)deviceAddress.Address, datas);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteByte(DeviceAddress deviceAddress, byte datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public int WriteBytes(DeviceAddress deviceAddress, byte[] datas)
        {
            throw new NotImplementedException();
        }
        public int WriteShort(DeviceAddress deviceAddress, short datas, int offset = 0)
        {
            ushort startAddress;
            int address = deviceAddress.Address + offset;
            if (Function.EnableWriteRegister(address, out startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.ShortToBytes(datas,Order);
                byte[] sendBytes = getWriteSigRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteUShort(DeviceAddress deviceAddress, ushort datas, int offset = 0)
        {
            ushort startAddress;
            int address = deviceAddress.Address + offset;
            if (Function.EnableWriteRegister(address, out  startAddress))
            {
                byte[] valueBytes = UnsafeNetConvert.UShortToBytes(datas, Order);
                byte[] sendBytes = getWriteSigRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
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
                byte[] valueBytes = UnsafeNetConvert.BytesPerversion( UnsafeNetConvert.ShortsToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
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
                byte[] valueBytes =UnsafeNetConvert.BytesPerversion(UnsafeNetConvert.UShortsToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }


        public int WriteInt(DeviceAddress deviceAddress, int datas, int offset = 0)
        {
            
            ushort startAddress;
            int address = deviceAddress.Address + offset * 2;
            if (Function.EnableWriteRegister(address, out startAddress))
            {
                var valueBytes = (Order== ByteOrder.BigEndian) ? UnsafeNetConvert.IntToBytes(datas, Order) //判断数据word字节是否需要翻转，若为BigEndian则不需要翻转
                    : UnsafeNetConvert.BytesPerversion(UnsafeNetConvert.IntToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
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
                var valueBytes = (Order == ByteOrder.BigEndian) ? UnsafeNetConvert.IntsToBytes(datas, Order)
                    : UnsafeNetConvert.BytesPerversion(UnsafeNetConvert.IntsToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteUInt(DeviceAddress deviceAddress, uint datas, int offset = 0)
        {
            ushort startAddress;
            int address = deviceAddress.Address + offset * 2;
            if (Function.EnableWriteRegister(address, out startAddress))
            {
                var valueBytes = (Order== ByteOrder.BigEndian) ? UnsafeNetConvert.UIntToBytes(datas, Order)
                    : UnsafeNetConvert.BytesPerversion(UnsafeNetConvert.UIntToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
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
                var valueBytes = (Order == ByteOrder.BigEndian) ? UnsafeNetConvert.UIntsToBytes(datas, Order)
                    : UnsafeNetConvert.BytesPerversion(UnsafeNetConvert.UIntsToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteFloat(DeviceAddress deviceAddress, float datas, int offset = 0)
        {
            ushort startAddress;
            int address = deviceAddress.Address + offset * 2;
            if (Function.EnableWriteRegister(address, out startAddress))
            {
                var valueBytes = (Order == ByteOrder.BigEndian) ? UnsafeNetConvert.FloatToBytes(datas, Order)
                    : UnsafeNetConvert.BytesPerversion(UnsafeNetConvert.FloatToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
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
                var valueBytes = (Order == ByteOrder.BigEndian) ? UnsafeNetConvert.FloatsToBytes(datas, Order)
                    : UnsafeNetConvert.BytesPerversion(UnsafeNetConvert.FloatsToBytes(datas, Order));
                byte[] sendBytes = getWriteMulRegisterHeader((byte)deviceAddress.AreaID, startAddress, valueBytes);
                return writeBytes(sendBytes);
            }
            else
            {
                return -1;
            }
        }

        public int WriteString(DeviceAddress deviceAddress, string datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public int WriteStrings(DeviceAddress deviceAddress, string[] datas)
        {
            throw new NotImplementedException();
        }
                        
        public abstract void Dispose();

        public Item<byte> ReadByte(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<byte>[] ReadBytes(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }
    }

}
