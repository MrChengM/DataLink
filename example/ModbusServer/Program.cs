using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ModbusDrivers;
using DataServer;
using DataServer.Points;
using SocketServers;

namespace ModbusServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //ILog log = new DefaultLog("ModbusTCPServer");
            //ModbusPointMapping mapping = ModbusPointMapping.GetInstance(log);
            //for (int i = 1; i < 1001; i++)
            //{
            //    var key = string.Format("{0:D5}", 40000 + i);
            //    mapping.Register(key, new VirtulPoint<ushort>(key, "ushort", new ushort[] { (ushort)i }));
            //    var key1 = string.Format("{0:D5}", 00000 + i);
            //    mapping.Register(key1, new VirtulPoint<bool>(key1, "bool", new bool[] { true }));
            //    var key2 = string.Format("{0:D5}", 10000 + i);
            //    mapping.Register(key2, new VirtulPoint<bool>(key2, "bool", new bool[] { true }));
            //    var key3 = string.Format("{0:D5}", 30000 + i);
            //    mapping.Register(key3, new VirtulPoint<ushort>(key3, "ushort", new ushort[] { (ushort)(i * 2) }));

            //}
            //ModbusTCPServer server = new ModbusTCPServer(new EthernetSetUp("127.0.0.1", 502), new TimeOut("ModbusServer", 10000, log), log, 5, 1);
            //server.Init();
            //server.Start();
            Console.ReadKey();
        }
    }
}
