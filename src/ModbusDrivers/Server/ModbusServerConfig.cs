using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ModbusDrivers.Server
{
    public class ModbusServerConfig
    {
        /***************************
       点文件路径必须配置
       其他参数可以设置为默认值
       ****************************/
        private string _ipAddress = "127.0.0.1";
        private int _port = 502;
        private int _timeOut = 1000;
        private int _maxConnect = 5;
        private int _salveID = 1;
        private string _signalFile = "";
        public ModbusServerConfig() { }
        public string IpAddress { get { return _ipAddress; } set {_ipAddress=value; } }
        public int Port { get { return _port; } set {_port=value; } }
        public int TimeOut { get {return _timeOut ; } set {_timeOut=value ; } }
        public int MaxConnect { get {return _maxConnect; } set {_maxConnect=value ; } }
        public int SalveId { get {return _salveID; } set {_salveID=value; } }
        public string SignalListFilePath { get {return _signalFile; } set { _signalFile=value; } }



        public static void ReadConfig(ModbusServerConfig serverConfig, XmlReader reader)
        {
                 serverConfig.IpAddress = reader["address"];
                int port;
                if(int.TryParse(reader["port"],  out port))
                serverConfig.Port = port;
                int timeOut;
                if(int.TryParse(reader["timeout"],out timeOut))
                    serverConfig.TimeOut = timeOut;
                int maxconnect;
                if (int.TryParse(reader["maxconnect"], out maxconnect))
                    serverConfig.MaxConnect=maxconnect;
                int salveId;
                if (int.TryParse(reader["salveid"], out salveId))
                    serverConfig.SalveId = salveId;
                serverConfig.SignalListFilePath = reader["signallist"];
        }
    }
}
