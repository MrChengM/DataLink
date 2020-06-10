using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataServer;

namespace TaskHandler.Config
{
    public static class ConfigUtilly
    {
        public static void ReadConfig(TCPClientConfig config, XmlReader reader,string handler) ///可优化用统一静态类进行处理
        {
            int temp;
            config.IpAddress = reader["address"];
            if (int.TryParse(reader["port"], out temp))
                config.Port = temp;
            if (int.TryParse(reader["timeout"], out temp))
                config.TimeOut = temp;
            if (int.TryParse(reader["timer"], out temp))
                config.PollingTime = temp;
            config.SignalListFilePath = reader["signallist"];
        }
        public static void ReadConfig(ComClientConfig config, XmlReader reader)
        {
            int temp;
            byte temp1;
            config.ComPort = reader["comport"];
            if (int.TryParse(reader["buadrate"], out temp))
                config.BuadRate = temp;
            if (byte.TryParse(reader["databit"], out temp1))
                config.DataBit = temp1;
            if (byte.TryParse(reader["stopbit"], out temp1))
                config.StopBit = temp1;
            config.OddEvenCheck = reader["oddevencheck"];
            if (int.TryParse(reader["timeout"], out temp))
                config.TimeOut = temp;
            if (int.TryParse(reader["timer"], out temp))
                config.PollingTime = temp;
            config.SignalListFilePath = reader["signallist"];
        }
        public static void ReadConfig(TCPServerConfig config, XmlReader reader)
        {
            config.IpAddress = reader["address"];
            int port;
            if (int.TryParse(reader["port"], out port))
                config.Port = port;
            int timeOut;
            if (int.TryParse(reader["timeout"], out timeOut))
                config.TimeOut = timeOut;
            int maxconnect;
            if (int.TryParse(reader["maxconnect"], out maxconnect))
                config.MaxConnect = maxconnect;
            int salveId;
            if (int.TryParse(reader["salveid"], out salveId))
                config.SalveId = salveId;
            config.SignalListFilePath = reader["signallist"];
        }
        public static void ReadConfig(ComServerConfig config, XmlReader reader)
        {
            int temp;
            byte temp1;
            config.ComPort = reader["comport"];
            if (int.TryParse(reader["buadrate"], out temp))
                config.BuadRate = temp;
            if (byte.TryParse(reader["databit"], out temp1))
                config.DataBit = temp1;
            if (byte.TryParse(reader["stopbit"], out temp1))
                config.StopBit = temp1;
            config.OddEvenCheck = reader["oddevencheck"];
            int timeOut;
            if (int.TryParse(reader["timeout"], out timeOut))
                config.TimeOut = timeOut;
            int maxconnect;
            if (int.TryParse(reader["maxconnect"], out maxconnect))
                config.MaxConnect = maxconnect;
            int salveId;
            if (int.TryParse(reader["salveid"], out salveId))
                config.SalveId = salveId;
            config.SignalListFilePath = reader["signallist"];
        }
        public static void ReadConfig(TaskConfig config,XmlReader reader)
        {
            int temp;
            string str;

            config.Id = reader["id"];
            config.TaskName = reader["taskname"];
            if(int.TryParse(reader["createtimeOut"],out temp))
            {
                config.CreatedTimeOut = temp;
            }
            if(int.TryParse(reader["inittimeOut"],out temp))
            {
                config.InitTimeOut = temp;
            }
            if(int.TryParse(reader["starttimeOut"],out temp))
            {
                config.StartTimeOut = temp;
            }
            if (int.TryParse(reader["stoptimeOut"], out temp))
            {
                config.StopTimeOut = temp;
            }
            if (int.TryParse(reader["initlevel"], out temp))
            {
                config.InitLevel = temp;
            }
            str = reader["tasktype"];
            if (str!=null)
            {
                TaskType tsType;
                if(Enum.TryParse(str, out tsType))
                    config.TsType = tsType;
            }
            str = reader["drivertype"];
            if (str != null)
            {
                 DriverType drType;
                if (Enum.TryParse(str, out drType))
                    config.DrType = drType;
            }
        }
    }
}
