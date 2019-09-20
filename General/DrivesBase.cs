using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivesHelper
{
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
        Item<TResult> ReadData<TResult>(DeviceAddress deviceAddress);

        //读取连续数据
        Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length);
        Item<byte>[] ReadBytes(DeviceAddress deviceAddress, ushort length);
        Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length);
        Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress,ushort length);
        Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length);
        Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length);
        Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length);
        Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length);
        Item<TResult>[] ReadDatas<TResult>(DeviceAddress deviceAddress,ushort length);

    }
    public interface IWrite
    {
        //写单个数据
        int WriteDatas(DeviceAddress deviceAddress, bool datas);
        int WriteDatas(DeviceAddress deviceAddress, byte datas);
        int WriteDatas(DeviceAddress deviceAddress, short datas);
        int WriteDatas(DeviceAddress deviceAddress, ushort datas);
        int WriteDatas(DeviceAddress deviceAddress, int datas);
        int WriteDatas(DeviceAddress deviceAddress, uint datas);
        int WriteDatas(DeviceAddress deviceAddress, float datas);
        int WriteDatas(DeviceAddress deviceAddress, string datas);
        int WriteDatas<T>(DeviceAddress deviceAddress, T datas);
        //写多个数据
        int WriteDatas(DeviceAddress deviceAddress, bool[] datas);
        int WriteDatas(DeviceAddress deviceAddress, byte[] datas);
        int WriteDatas(DeviceAddress deviceAddress, short[] datas);
        int WriteDatas(DeviceAddress deviceAddress, ushort[] datas);
        int WriteDatas(DeviceAddress deviceAddress, int[] datas);
        int WriteDatas(DeviceAddress deviceAddress, uint[] datas);
        int WriteDatas(DeviceAddress deviceAddress, float[] datas);
        int WriteDatas(DeviceAddress deviceAddress, string[] datas);
        int WriteDatas<T>(DeviceAddress deviceAddress, T[] datas);
    }
    public interface IDriver:IDisposable
    {
        DriverType DriType { get; set; }
        SerialportSetUp SPortSet { get; set; }
        EthernetSetUp EtherSet { get; set; }
    }
    public interface IPLCDriver: IRead, IWrite, IDriver
    {

    }
    public enum DriverType
    {
        Serialport = 0x01,
        Ethernet =0x02,
        File=0x03,
        Memory=0x04,
    }
    /// <summary>
    /// 串口类型设置
    /// </summary>
    public class SerialportSetUp
    {
        public string ComPort { get; set; }
        public int BuadRate { get; set; }
        public byte StartBit { get; set; }
        public byte StopBit { get; set; }
        public string OddEvenChenck { get; set; }
    }
    /// <summary>
    /// 以太网端口设置
    /// </summary>
    public class EthernetSetUp
    {
        public string IPAddress { get; set; }
        public int PortNumber { get; set; }
        public string ProtocolType { get; set; }
    }
    /// <summary>
    /// 设备报文读取格式
    /// </summary>
    public struct DeviceAddress
    {
        public int SlaveID { get; set;  }
        public int FuctionNumber { get; set; }
        public int Address { get; set; }
    }
    public class Item<T>
    {
        public T Vaule { get; set; }
        public DateTime UpdateTime { get; set; }
        public QUALITIES Quality { get; set; }
        
    }
    /// <summary>
    /// 通信质量
    /// </summary>
    public enum QUALITIES : short
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
    public enum DataType:byte
    {
        Bool =0x01,
        Byte=0x02,
        Short=0x03,
        UShort=0x04,
        Word=0x05,
        UWord=0X06,
        Dword=0x07,
        UDword=0x08,
        Folat =0x09,
        //UFolat=0x10,
        String=0x11

    }
}
