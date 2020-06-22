using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusDrivers;
using DataServer;
using ModbusDrivers.Client;
using ModbusDrivers.Server;


namespace ModbusClient
{
    class Program
    {
        static void Main(string[] args)
        {
            EthernetSetUp _setup  = new EthernetSetUp("127.0.0.1", 502);
            ILog _log = new DefaultLog("ModbusTCPClient");
            TimeOut _timeout = new TimeOut("ModbusTCPClient", 1000, _log);
            ModbusTCPClient _client = new ModbusTCPClient(_setup,_timeout,_log);
            if (_client.Connect())//先判断是否能连接到客户端
            {
                var boolvalue = _client.ReadBool(new DeviceAddress(1, 00001));
                var boolvalues = _client.ReadBools(new DeviceAddress(1, 00001),10);

                var boolvalue1 = _client.ReadBool(new DeviceAddress(1, 10001));
                var boolvalue1s = _client.ReadBools(new DeviceAddress(1, 10001), 10);

                var shortvalue = _client.ReadShort(new DeviceAddress(1, 30001));
                var shortvalues = _client.ReadShorts(new DeviceAddress(1, 30001), 10);

                var shortvalue1 = _client.ReadUShort(new DeviceAddress(1, 40001));
                var ushortvalue1s = _client.ReadUShorts(new DeviceAddress(1, 40001), 10);

                var intvalue = _client.ReadInt(new DeviceAddress(1, 30001));
                var intvalues = _client.ReadInts(new DeviceAddress(1, 30001), 10);

                var intvalue1 = _client.ReadInt(new DeviceAddress(1, 40001, 0x00, ByteOrder.BigEndian));
                var intvalue1s = _client.ReadInts(new DeviceAddress(1, 40001, 0x00, ByteOrder.BigEndian), 10);

                var floatvalue = _client.Readfloat(new DeviceAddress(1, 30001));
                var floatvalues = _client.Readfloats(new DeviceAddress(1, 30001),10);

                var floatvalue1 = _client.Readfloat(new DeviceAddress(1, 40001));
                var floatvalue1s = _client.Readfloats(new DeviceAddress(1, 40001), 10);
            }

            Console.ReadKey();
        }
    }
   
}
