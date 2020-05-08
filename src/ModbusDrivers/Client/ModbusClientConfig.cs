using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using System.Xml;
using System.IO.Ports;

namespace ModbusDrivers.Client
{
    public class ModbusTCPClientConfig
    {
        public ModbusTCPClientConfig() { }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int TimeOut { get; set; }
        public int PollingTime { get; set; }
        public string SignalListFilePath { get; set; }



        public static void ReadConfig(ModbusTCPClientConfig clientConfig, XmlReader reader)
        {
            if(reader["handler"]== "ModbusTCPClientHandlerTask")
            {
                clientConfig.IpAddress = reader["address"];
                clientConfig.Port = int.Parse(reader["port"]);
                clientConfig.TimeOut = int.Parse(reader["timeout"]);
                clientConfig.PollingTime = int.Parse(reader["timer"]);
                clientConfig.SignalListFilePath = reader["signallist"];
            }

        }
    }
    public class ModbusRTUClientConfig
    {
        public string ComPort { get; set; }
        public int BuadRate { get; set; }
        public byte DataBit { get; set; }
        public byte StopBit { get; set; }
        public string OddEvenCheck { get; set; }
        public int TimeOut { get; set; }
        public int PollingTime { get; set; }
        public string SignalListFilePath { get; set; }
        public static void ReadConfig(ModbusRTUClientConfig clientConfig, XmlReader reader)
        {
            if (reader["handler"] == "ModbusRTUClientHandlerTask")
            {
                clientConfig.ComPort = reader["comport"];
                clientConfig.BuadRate = int.Parse(reader["buadrate"]);
                clientConfig.DataBit = byte.Parse(reader["databit"]);
                clientConfig.StopBit = byte.Parse(reader["stopbit"]);
                clientConfig.OddEvenCheck = reader["oddevencheck"];
                clientConfig.TimeOut = int.Parse(reader["timeout"]);
                clientConfig.PollingTime = int.Parse(reader["timer"]);
                clientConfig.SignalListFilePath = reader["signallist"];
            }

        }
    }
}
