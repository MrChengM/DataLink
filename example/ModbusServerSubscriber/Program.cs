using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using ModbusDrivers.Client;

namespace ModbusServerSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            //ILog log1 = new DefaultLog("ModbusTCPClientHanderTask") {Handle=LogHandle.writeFile};

            //ILog log2 = new DefaultLog("ModbusServerHanderTask") { Handle=LogHandle.debug,ByteSteamLogSwicth=true};
            //var client = new ModbusTCPClientHanderTask(log1);
            //if (client.OnInit())
            //    client.OnStart();

            ////var server = new ModbusServerHanderTask(log2);

            //if (server.OnInit())
            //    server.OnStart();
            //Console.ReadKey();
        }
    }
}
