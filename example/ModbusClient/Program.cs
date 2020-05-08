using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusDrivers;
using DataServer;
using ModbusDrivers.Client;


namespace ModbusClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //EthernetSetUp netsetup = new EthernetSetUp("127.0.0.1", 502);
            ILog log = new TESTLOG();
            //TimeOut timeout = new TimeOut("salve", 3000, log);
            //ModbusTCPClient salve = new ModbusTCPClient(netsetup, timeout, log);
            //salve.Connect();
            //DeviceAddress da = new DeviceAddress(1, 40001, ByteOrder.None);
            //Item<short> read = salve.ReadShort(da);
            //salve.DisConnect();
            //Console.ReadKey();
            var client = new ModbusTCPClientHanderTask(log);
            client.OnInit();
            client.OnStart();
            Console.ReadKey();

        }
    }
   
}
