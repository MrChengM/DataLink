using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using System.Xml;
using System.IO.Ports;
using System.IO;

namespace ModbusDrivers.Client
{
    public class ModbusTCPClientConfig
    {
        /***************************
       Ip地址和点文件路径必须配置
       其他参数可以设置为默认值
       ****************************/
        string _ipString = ""; //主键 默认为空 必须配置
        int _port = 502;
        int _timeout = 1000;
        int _pollingTime = 1000;
        string _signalListFiePath = "";// 默认为空 必须配置

        public ModbusTCPClientConfig() { }
        public string IpAddress { get {return _ipString ; } set { _ipString=value; } }
        public int Port { get {return _port ; } set {_port=value ; } }
        public int TimeOut { get {return _timeout; } set {_timeout=value ; } }
        public int PollingTime { get {return _pollingTime; } set { _pollingTime=value; } }
        public string SignalListFilePath { get {return _signalListFiePath; } set {_signalListFiePath=value ; } }



        public static void ReadConfig(ModbusTCPClientConfig clientConfig, XmlReader reader)
        {
            int temp;
            clientConfig.IpAddress = reader["address"];
            if (int.TryParse(reader["port"], out temp))
                clientConfig.Port = temp;
            if (int.TryParse(reader["timeout"], out temp))
                clientConfig.TimeOut = temp;
            if (int.TryParse(reader["timer"], out temp))
                clientConfig.PollingTime = temp;
            clientConfig.SignalListFilePath = reader["signallist"];
        }
    }
    public class ModbusRTUClientConfig
    {
        /***************************
        com端口和点文件路径必须配置
        其他参数可以设置为默认值
        ****************************/
        string _comport = ""; //主键 默认为空 必须配置
        int _buadRate = 9600;
        byte _dataBit = 8;
        byte _stopBit = 1;
        string _OddEvenCheck = Parity.None.ToString();
        int _timeOut = 1000;
        int _pollingTime = 1000;
        string _signalListFilePath = "";// 默认为空 必须配置

        public string ComPort { get { return _comport; } set {_comport=value; } }
        public int BuadRate { get {return _buadRate; } set {_buadRate=value; } }
        public byte DataBit { get {return _dataBit; } set {_dataBit=value; } }
        public byte StopBit { get {return _stopBit; } set {_stopBit=value; } }
        public string OddEvenCheck { get { return _OddEvenCheck; } set { _OddEvenCheck=value; } }
        public int TimeOut { get { return _timeOut; } set {_timeOut=value; } }
        public int PollingTime { get {return _pollingTime; } set {_pollingTime=value; } }
        public string SignalListFilePath { get {return _signalListFilePath; } set {_signalListFilePath=value ; } }
        public static void ReadConfig(ModbusRTUClientConfig clientConfig, XmlReader reader)
        {
            int temp;
            byte temp1;
            clientConfig.ComPort = reader["comport"];
            if (int.TryParse(reader["buadrate"], out temp))
                clientConfig.BuadRate = temp;
            if (byte.TryParse(reader["databit"], out temp1))
                clientConfig.DataBit = temp1;
            if (byte.TryParse(reader["stopbit"], out temp1))
                clientConfig.StopBit = temp1;
            clientConfig.OddEvenCheck = reader["oddevencheck"];
            if (int.TryParse(reader["timeout"], out temp))
                clientConfig.TimeOut = temp;
            if (int.TryParse(reader["timer"], out temp))
                clientConfig.PollingTime = temp;
            clientConfig.SignalListFilePath = reader["signallist"];
        }
    }
}
