using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiemensDriver;
using DataServer;
using System.Threading;

namespace SiemensClient
{
    class Program
    {
        static void Main(string[] args)
        {
            EthernetSetUp setup = new EthernetSetUp("192.168.1.240", 102);
            ILog log = new DefaultLog("S7CommomClient") { ByteSteamLogSwicth = true, Handle = LogHandle.debug };
            TimeOut timeout = new TimeOut("S7CommomClient", 1000,log);
            S7CommClient client = new S7CommClient(setup, timeout, log, 1);
            if (client.Connect())
            {
                //DB块编号，地址，访问数据块的类型：0x81-input ,0x82-output ,0x83-flag , 0x84-DB;
                var address = new DeviceAddress(30, 0, 0x84,ByteOrder.BigEndian);
               
                var temp=client.ReadUShorts(address,2);
                Thread.Sleep(1000);
                var temp1=client.ReadBytes(address,3);

                var address1 = new DeviceAddress(14, 0, 0x84, ByteOrder.BigEndian);
                var result = client.WriteUShort(address1, 12);
                var temp3 = client.ReadUShorts(address1, 1);


            }
            Console.ReadKey();
        }
    }
}
