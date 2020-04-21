using System;
using System.IO.Ports;
using System.Net.Sockets;

namespace DataServer
{
    #region
    public interface IRead
    {
        byte[] ReadBytes(DeviceAddress deviceAddress, ushort length);
        //读取单个数据
        Item<bool> ReadBool(DeviceAddress deviceAddress);
        //Item<byte> ReadByte(DeviceAddress deviceAddress);
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
        Item<TResult> ReadData<TResult>(DeviceAddress deviceAddress);

        //读取连续数据
        Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length);
        //Item<byte>[] ReadBytes(DeviceAddress deviceAddress, ushort length);
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
        Item<TResult>[] ReadDatas<TResult>(DeviceAddress deviceAddress, ushort length);
    }
    public interface IWrite
    {
        //写单个数据
        int WriteBool(DeviceAddress deviceAddress, bool datas);
        int WriteByte(DeviceAddress deviceAddress, byte datas);
        int WriteShort(DeviceAddress deviceAddress, short datas);
        int WriteUShort(DeviceAddress deviceAddress, ushort datas);
        int WriteInt(DeviceAddress deviceAddress, int datas);
        int WriteUInt(DeviceAddress deviceAddress, uint datas);
        int WriteFloat(DeviceAddress deviceAddress, float datas);
        int WriteString(DeviceAddress deviceAddress, string datas);
        /// <summary>
        /// 泛型方法，暂不实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deviceAddress"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        int WriteData<T>(DeviceAddress deviceAddress, T datas);
        //写多个数据
        int WriteBools(DeviceAddress deviceAddress, bool[] datas);
        int WriteBytes(DeviceAddress deviceAddress, byte[] datas);
        int WriteShorts(DeviceAddress deviceAddress, short[] datas);
        int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas);
        int WriteInts(DeviceAddress deviceAddress, int[] datas);
        int WriteUInts(DeviceAddress deviceAddress, uint[] datas);
        int WriteFloats(DeviceAddress deviceAddress, float[] datas);
        int WriteStrings(DeviceAddress deviceAddress, string[] datas);
        /// <summary>
        /// 泛型方法，暂不实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deviceAddress"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        int WriteDatas<T>(DeviceAddress deviceAddress, T[] datas);
    }
    public interface IDriver : IDisposable
    {

        //驱动类型
        DriverType DriType { get; }
        //状态判断
        bool IsConnect { get; }
        bool Connect();
        bool DisConnect();
        bool IsClose { get; }
        TimeOut TimeOut { get; set; }
        ILog Log { get; set; }

    }
    public interface IPLCDriver : IRead, IWrite, IDriver
    {
        //报文数据长度
        int PDU { get; set; }
        DeviceAddress GetDeviceAddress(string address);
        string GetAddress(DeviceAddress deviceAddress);
    }
    #endregion
    public enum DriverType
    {
        Serialport = 0x01,
        Ethernet = 0x02,
        File = 0x03,
        Memory = 0x04,
    }
    /// <summary>
    /// 串口类型设置
    /// </summary>
    public class SerialportSetUp
    {
        public string ComPort { get; set; }
        public int BuadRate { get; set; }
        public byte DataBit { get; set; }
        public StopBits StopBit { get; set; }
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

        public static SerialportSetUp Default = new SerialportSetUp("COM1", 9600, 8, StopBits.One, Parity.None);
    }
    /// <summary>
    /// 以太网端口设置
    /// </summary>
    public class EthernetSetUp
    {
        public string IPAddress { get; set; }
        public int PortNumber { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public EthernetSetUp() { }
        public EthernetSetUp(string ipAddress, int portNumber, ProtocolType protocolType = ProtocolType.Tcp)
        {
            IPAddress = ipAddress;
            PortNumber = portNumber;
            ProtocolType = protocolType;
        }
    }

    /// <summary>
    /// 进程间通讯 DCOM通讯等（OPC/DDE)
    /// </summary>
    public class MemorySetUp
    {
        public string IPAddress { get; set; }
        public string ServerName { get; set; }
        public string TopicName { get; set; }
        public MemorySetUp() { }
        public MemorySetUp(string ipAddress, string serverName, string topicName)
        {
            IPAddress = ipAddress;
            ServerName = serverName;
            TopicName = topicName;
        }
    }
    /// <summary>
    /// 地址
    /// </summary>
    public struct DeviceAddress
    {
        public int Area { get; set; }
        //public int FuctionNumber { get; set; }
        public int Address { get; set; }
        //public string VarType { get; set; }
        public ByteOrder ByteOrder { get; set; }
        public DeviceAddress(int area, int address,   ByteOrder byteOrder = ByteOrder.None)
        {
            Area = area;
            //FuctionNumber = fuctionNumber;
            Address = address;
            //VarType = varType;
            ByteOrder = byteOrder;
        }

    }

    /// <summary>
    /// 数据单元
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Item<T>
    {
        public T Vaule { get; set; }
        public DateTime UpdateTime { get; set; }
        public QUALITIES Quality { get; set; }
        public static Item<T> Default = new Item<T> { Vaule = default(T), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_BAD };
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
    //public enum DataType : byte
    //{
    //    Bool = 0x01,
    //    Byte = 0x02,
    //    Short = 0x03,
    //    UShort = 0x04,
    //    Word = 0x05,
    //    UWord = 0X06,
    //    Dword = 0x07,
    //    UDword = 0x08,
    //    Folat = 0x09,
    //    UFolat = 0x10,
    //    String = 0x11

    //}
    public static class ValueType
    {
        public static string Bool = "bool";
        public static string Byte = "byte";
        public static string Int16 = "short";
        public static string UInt16 = "ushort";
        public static string Int32 = "int";
        public static string UInt32 = "uint";
        public static string Float = "float";
        public static string String = "string";
    }
    /// <summary>
    /// 字节组合高低位，word高低位，dword高低位，float高低位
    /// </summary>
    [Flags]
    public enum ByteOrder : byte
    {
        None = 0,
        /// <summary>
        /// 大端法，高位存储在字节低位，低位存储在字节高位
        /// </summary>
        BigEndian = 1,
        /// <summary>
        /// 小端法，低位存储在字节低位，高位存储在字节高位
        /// </summary>
        LittleEndian = 2,
        //Network = 4,
        //Host = 8
    }
    public static class EthProtocolType
    {
        public const string TCPIP = "TCP/IP";
        public const string UDP = "UDP";
        public const string SOAP = "SOAP";
    }
}
