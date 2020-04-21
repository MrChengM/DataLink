using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusDrivers;
using DataServer;
using ModbusDrivers.Server;

namespace ModbusServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog log = new TESTLOG();
            ModbusPointMapping mapping = ModbusPointMapping.GetInstance();
            mapping.Init(log);
            for(int i = 1; i < 1001; i++)
            {
                var key = string.Format("{0:D5}", 40000 + i);
                mapping.Register(key, new VirtulPoint<ushort>(key));
            }
            ModbusTCPServer server = new ModbusTCPServer(log, new TimeOut("ModbusServer", 10000, log), 100, 1);
            server.Init();
            server.Start();
        }
    }
}
