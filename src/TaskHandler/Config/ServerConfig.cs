using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TaskHandler.Config
{
    public abstract class ServerConfig
    {
        /***************************
      点文件路径必须配置
      其他参数可以设置为默认值
      ****************************/
        private int _timeOut;
        private int _maxConnect;
        private int _salveID;
        private string _signalFile;
        public ServerConfig()
        {
            _timeOut = 1000;
            _maxConnect = 5;
            _salveID = 1;
            _signalFile = "";
        }
        public int TimeOut { get { return _timeOut; } set { _timeOut = value; } }
        public int MaxConnect { get { return _maxConnect; } set { _maxConnect = value; } }
        public int SalveId { get { return _salveID; } set { _salveID = value; } }
        public string SignalListFilePath { get { return _signalFile; } set { _signalFile = value; } }



        //public static void ReadConfig(ServerConfig serverConfig, XmlReader reader)
        //{
        //    serverConfig.IpAddress = reader["address"];
        //    int port;
        //    if (int.TryParse(reader["port"], out port))
        //        serverConfig.Port = port;
        //    int timeOut;
        //    if (int.TryParse(reader["timeout"], out timeOut))
        //        serverConfig.TimeOut = timeOut;
        //    int maxconnect;
        //    if (int.TryParse(reader["maxconnect"], out maxconnect))
        //        serverConfig.MaxConnect = maxconnect;
        //    int salveId;
        //    if (int.TryParse(reader["salveid"], out salveId))
        //        serverConfig.SalveId = salveId;
        //    serverConfig.SignalListFilePath = reader["signallist"];
        //}
    }
    public class TCPServerConfig:ServerConfig
    {
        private string _ipAddress;
        private int _port;
        
        public TCPServerConfig():base()
        {
            _ipAddress = "127.0.0.1";
            _port = 502;
        }
        public string IpAddress { get { return _ipAddress; } set { _ipAddress = value; } }
        public int Port { get { return _port; } set { _port = value; } }

    }
    public class ComServerConfig: ServerConfig
    {
         /***************************
         com端口和点文件路径必须配置
         其他参数可以设置为默认值
         ****************************/
        string _comport ; //主键 默认为空 必须配置
        int _buadRate;
        byte _dataBit;
        byte _stopBit;
        string _OddEvenCheck;
        public ComServerConfig() : base()
        {
            _comport = "";
            _buadRate = 9600;
            _dataBit = 8;
            _stopBit = 1;
            _OddEvenCheck = Parity.None.ToString();
        }

        public string ComPort { get { return _comport; } set { _comport = value; } }
        public int BuadRate { get { return _buadRate; } set { _buadRate = value; } }
        public byte DataBit { get { return _dataBit; } set { _dataBit = value; } }
        public byte StopBit { get { return _stopBit; } set { _stopBit = value; } }
        public string OddEvenCheck { get { return _OddEvenCheck; } set { _OddEvenCheck = value; } }

    }

}
