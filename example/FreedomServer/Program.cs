using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Points;
using FreedomDrivers;
using DataServer;



namespace FreedomServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog log = new DefaultLog("FreedomServer");

            PointMapping<bool> boolMaping = PointMapping<bool>.GetInstance(log);
            PointMapping<byte> byteMaping = PointMapping<byte>.GetInstance(log);
            PointMapping<ushort> ushortMaping = PointMapping<ushort>.GetInstance(log);
            PointMapping<short> shortMaping = PointMapping<short>.GetInstance(log);
            PointMapping<int> intMaping = PointMapping<int>.GetInstance(log);
            PointMapping<uint> uintMaping = PointMapping<uint>.GetInstance(log);
            PointMapping<float> floatMaping = PointMapping<float>.GetInstance(log);
            for (int i = 0; i < 1000; i++)
            {
                var bpoint = new VirtulPoint<bool>(string.Concat("boolSignal", i), DataServer.ValueType.Bool, new bool[] { true });
                boolMaping.Register(bpoint.Name, bpoint);
                var bypoint = new VirtulPoint<byte>(string.Concat("byteSignal", i), DataServer.ValueType.Byte, new byte[] {10});
                byteMaping.Register(bypoint.Name, bypoint);
                var ushortpoint = new VirtulPoint<ushort>(string.Concat("ushortSignal", i), DataServer.ValueType.UInt16,new ushort[] {Convert.ToUInt16(i) });
                ushortMaping.Register(ushortpoint.Name, ushortpoint);
                var shortpoint = new VirtulPoint<short>(string.Concat("shortSignal", i), DataServer.ValueType.Int16, new short[] { Convert.ToInt16(i) });
                shortMaping.Register(shortpoint.Name, shortpoint);
                var intpoint = new VirtulPoint<int>(string.Concat("intSignal", i), DataServer.ValueType.Int32,new int[] { i});
                intMaping.Register(intpoint.Name, intpoint);
                var uintpoint = new VirtulPoint<uint>(string.Concat("uintSignal", i), DataServer.ValueType.UInt32,new uint[] { Convert.ToUInt32(i)});
                uintMaping.Register(uintpoint.Name, uintpoint);
            }

            EthernetSetUp setup = new EthernetSetUp("127.0.0.1", 9527);
            TimeOut timeout = new TimeOut("FreedomeServer", 1000, log);
            FreedomDrivers.FreedomServer fs = new FreedomDrivers.FreedomServer(setup, timeout, log);
            if (fs.Init())
            {
                fs.Start();
            }
            Console.ReadKey();
        }
    }
}
