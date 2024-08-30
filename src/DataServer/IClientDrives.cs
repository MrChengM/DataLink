using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.Serialization;
using Utillity.Data;

namespace DataServer
{
    #region
    public interface IRead
    {
        //读取单个数据
        Item<bool> ReadBool(DeviceAddress deviceAddress);
        Item<byte> ReadByte(DeviceAddress deviceAddress);
        Item<short> ReadShort(DeviceAddress deviceAddress);
        Item<ushort> ReadUShort(DeviceAddress deviceAddress);
        Item<int> ReadInt(DeviceAddress deviceAddress);
        Item<uint> ReadUInt(DeviceAddress deviceAddress);
        Item<float> Readfloat(DeviceAddress deviceAddress);
        Item<string> ReadString(DeviceAddress deviceAddress);
        /// <summary>
        /// 泛型方法，暂不实现
        /// </summary>
        /// <typeparam name="TResult">数据类型</typeparam>
        /// <param name="deviceAddress">设备地址</param>
        /// <returns></returns>
        //Item<TResult> ReadData<TResult>(DeviceAddress deviceAddress);

        //读取连续数据
        Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length);
        Item<byte>[] ReadBytes(DeviceAddress deviceAddress, ushort length);
        Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length);
        Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length);
        Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length);
        Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length);
        Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length);
        Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length);
        /// <summary>
        /// 泛型方法，暂不实现
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="deviceAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        //Item<TResult>[] ReadDatas<TResult>(DeviceAddress deviceAddress, ushort length);
    }
    public interface IWrite
    {
        //写单个数据
        //int WriteBool(DeviceAddress deviceAddress, bool datas);
        //int WriteByte(DeviceAddress deviceAddress, byte datas);
        //int WriteShort(DeviceAddress deviceAddress, short datas);
        //int WriteUShort(DeviceAddress deviceAddress, ushort datas);
        //int WriteInt(DeviceAddress deviceAddress, int datas);
        //int WriteUInt(DeviceAddress deviceAddress, uint datas);
        //int WriteFloat(DeviceAddress deviceAddress, float datas);

        int WriteBool(DeviceAddress deviceAddress, bool datas, int offset = 0);
        int WriteByte(DeviceAddress deviceAddress, byte datas, int offset = 0);
        int WriteShort(DeviceAddress deviceAddress, short datas, int offset = 0);
        int WriteUShort(DeviceAddress deviceAddress, ushort datas, int offset = 0);
        int WriteInt(DeviceAddress deviceAddress, int datas, int offset = 0);
        int WriteUInt(DeviceAddress deviceAddress, uint datas, int offset = 0);
        int WriteFloat(DeviceAddress deviceAddress, float datas, int offset = 0);
        int WriteString(DeviceAddress deviceAddress, string datas,int offset=0);
        ///// <summary>
        ///// 泛型方法，暂不实现
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="deviceAddress"></param>
        ///// <param name="datas"></param>
        ///// <returns></returns>
        //int WriteData<T>(DeviceAddress deviceAddress, T datas);
        //写多个数据
        int WriteBools(DeviceAddress deviceAddress, bool[] datas);
        int WriteBytes(DeviceAddress deviceAddress, byte[] datas);
        int WriteShorts(DeviceAddress deviceAddress, short[] datas);
        int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas);
        int WriteInts(DeviceAddress deviceAddress, int[] datas);
        int WriteUInts(DeviceAddress deviceAddress, uint[] datas);
        int WriteFloats(DeviceAddress deviceAddress, float[] datas);
        int WriteStrings(DeviceAddress deviceAddress, string[] datas);
        ///// <summary>
        ///// 泛型方法，暂不实现
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="deviceAddress"></param>
        ///// <param name="datas"></param>
        ///// <returns></returns>
        //int WriteDatas<T>(DeviceAddress deviceAddress, T[] datas);
    }
    public interface IDriver : IDisposable
    {

        //驱动类型
        //DriverType DriType { get; }
        //状态判断
        string Name { get; set; }
        bool IsConnect { get; }
        bool Connect();
        bool DisConnect();
        bool IsClose { get; }
        //TimeOut TimeOut { get; set; }
        ILog Log { get; set; }

        int ConnectTimeOut { get; set; }

        int RequestTimeOut { get; set; }

        int RetryTimes { get; set; }

    }
    public interface IPLCDriver : IRead, IWrite, IDriver
    {
        /// <summary>
        /// 报文数据长度
        /// </summary>
        int PDU { get; set; }
        /// <summary>
        /// 字节顺序
        /// </summary>
        ByteOrder Order { get; set; }
        int Id { get; set; }
        DeviceAddress GetDeviceAddress(string address);

    }
    public interface IComPortDriver : IPLCDriver
    {
        SerialportSetUp PortSetUp { get; set; }
    }
    public interface IEthernetPLCDriver: IPLCDriver
    {
        EthernetSetUp EthSetUp { get; set; }
    }
    #endregion
    //[DataContract]
    //[JsonConverter(typeof(StringEnumConverter))]
    public enum CommunicationType
    {
        //[EnumMember]
        Serialport = 0x01,
        //[EnumMember]
        Ethernet = 0x02,
        //[EnumMember]
        File = 0x03,
        //[EnumMember]
        Memory = 0x04,
    }
    [DataContract]
    /// <summary>
    /// 串口类型设置
    /// </summary>
    public class SerialportSetUp
    {
        [DataMember]
        public string ComPort { get; set; }
        [DataMember]
        public int BuadRate { get; set; }
        [DataMember]
        public byte DataBit { get; set; }
        [DataMember]
        public StopBits StopBit { get; set; }
        [DataMember]
        public Parity OddEvenCheck { get; set; }
        public SerialportSetUp() { }
        public SerialportSetUp(string comPort, int buadRate, byte dataBit, StopBits stopBit, Parity oddEvenCheck = Parity.None)
        {
            ComPort = comPort;
            BuadRate = buadRate;
            DataBit = dataBit;
            StopBit = stopBit;
            OddEvenCheck = oddEvenCheck;
        }

        public SerialportSetUp(string comPort, int buadRate, StopBits stopBit)
        {
            ComPort = comPort;
            BuadRate = buadRate;
            StopBit = stopBit;
        }

        public override string ToString()
        {
            return $"SerialPort:{ComPort}";
        }
    }
    [DataContract]
    /// <summary>
    /// 以太网端口设置
    /// </summary>
    public class EthernetSetUp
    {
        [DataMember]
        public string LocalNetworkAdpt { get; set; }
        [DataMember]
        public string IPAddress { get; set; }
        [DataMember]
        public int PortNumber { get; set; }
        [DataMember]
        public ProtocolType ProtocolType { get; set; }
        public EthernetSetUp() { }
        public EthernetSetUp(string ipAddress, int portNumber, ProtocolType protocolType = ProtocolType.Tcp)
        {
            IPAddress = ipAddress;
            PortNumber = portNumber;
            ProtocolType = protocolType;
        }
        public override string ToString()
        {
            return $"Ethernet:{PortNumber}";
        }
        public static EthernetSetUp Clone(EthernetSetUp source)
        {
            return new EthernetSetUp(source.IPAddress, source.PortNumber, source.ProtocolType) { LocalNetworkAdpt = source.LocalNetworkAdpt };
        }
    }

    [DataContract]
    /// <summary>
    /// 进程间通讯 DCOM通讯等（OPC/DDE)
    /// </summary>
    
    public class MemorySetUp
    {
        [DataMember]
        public string IPAddress { get; set; }
        [DataMember]
        public string ServerName { get; set; }
        [DataMember]
        public string TopicName { get; set; }
        public MemorySetUp() { }
        public MemorySetUp(string ipAddress, string serverName, string topicName)
        {
            IPAddress = ipAddress;
            ServerName = serverName;
            TopicName = topicName;
        }
        public override string ToString()
        {
            return $"Memory:{ServerName}";
        }
    }
    /// <summary>
    /// 地址
    /// </summary>
    public struct DeviceAddress
    {
        public int AreaID { get; set; }
        public int FuctionCode { get; set; }
        public int Address { get; set; }
        public int BitAddress { get; set; }
        public DeviceAddress(int area, int address, int bitAddress=0,int fuctioncode=0x00)
        {
            AreaID = area;
            FuctionCode = fuctioncode;
            Address = address;
            BitAddress = bitAddress;
        }

    }

    /// <summary>
    /// 数据单元
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Item<T> 
    {
        public T Vaule { get; set; }
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// DL通讯协议变量发生时间专用
        /// </summary>
        public DateTime AppearTime { get; set; }
        public QUALITIES Quality { get; set; }
        public static Item<T> CreateDefault() => new Item<T> { Vaule = default(T), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_BAD,AppearTime=new DateTime(1990,01,01,00,00,00) };
        // override object.Equals
        public bool Equals(Item<T> soure)
        {
            if (Vaule.Equals(soure.Vaule) && Quality == soure.Quality)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
    /// <summary>
    /// 通信质量
    /// </summary>
    public enum QUALITIES : byte
    {
        LIMIT_CONST = 3,
        LIMIT_HIGH = 2,
        LIMIT_LOW = 1,
        QUALITY_BAD = 0,
        QUALITY_COMM_FAILURE = 0x18,
        QUALITY_CONFIG_ERROR = 4,
        QUALITY_DEVICE_FAILURE = 12,
        QUALITY_EGU_EXCEEDED = 0x54,
        QUALITY_GOOD = 0xc0,
        QUALITY_LAST_KNOWN = 20,
        QUALITY_LAST_USABLE = 0x44,
        QUALITY_LOCAL_OVERRIDE = 0xd8,
        QUALITY_MASK = 0xc0,
        QUALITY_NOT_CONNECTED = 8,
        QUALITY_OUT_OF_SERVICE = 0x1c,
        QUALITY_SENSOR_CAL = 80,
        QUALITY_SENSOR_FAILURE = 0x10,
        QUALITY_SUB_NORMAL = 0x58,
        QUALITY_UNCERTAIN = 0x40,
        QUALITY_WAITING_FOR_INITIAL_DATA = 0x20,
        STATUS_MASK = 0xfc,
    }
    public enum DataType : byte
    {
        Bool = 0x01,
        Byte = 0x02,
        Short = 0x03,
        UShort = 0x04,
        Int = 0x05,
        UInt = 0x06,
        Float = 0x07,
        //Double = 0x08,
        String = 0x09

    }
    /// <summary>
    /// 字节组合高低位，word高低位，dword高低位，float高低位
    /// </summary>
    public class Scaling
    {

        public ScaleType ScaleType { get; set; }
        public DataType DataType { get; set; }

        public int RawLow { get; set; }
        public int RawHigh{ get; set; }

        public int ScaledLow { get; set; }

        public int ScaledHigh { get; set; }

        //public IEnumerator<string> GetEnumerator()
        //{
        //    yield return ScaleType.ToString();
        //    yield return DataType.ToString();
        //    yield return RawLow.ToString();
        //    yield return RawHigh.ToString();
        //    yield return ScaledLow.ToString();
        //    yield return ScaledHigh.ToString();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}
    }

    public enum ScaleType
    {
        None,
        Linear,
        SquareRoot,
    }
    public enum ReadWriteWay
    {
        Read,
        Write,
        ReadAndWrite
    }
   
}
