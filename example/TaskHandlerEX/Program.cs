using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHandler.Builder;
using TaskHandler.Factory;
using TaskHandler;

namespace TaskHandlerEX
{
    class Program
    {
        static void Main(string[] args)
        {
            //var builderClient = new TCPClientTaskBuilder(ClientName.ModbusTCPClient);

            //var taskClient = builderClient.GetResult();

            //var builderServer = new TCPServerTaskBuilder(ServerName.ModbusTCPServer);
            //var taskServer = builderServer.GetResult();
            //if (taskClient.OnInit())
            //{
            //    taskClient.OnStart();
            //    if (taskServer.OnInit())
            //    {
            //        taskServer.OnStart();
            //    }
            //}
            TaskMgr.Main(args);
            Console.ReadKey();
        }
    }
}
