using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusDrivers;
using DataServer;

namespace DriverTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EthernetSetUp netsetup = new EthernetSetUp("127.0.0.1", 502);
            ILog log = new TESTLOG();
            TimeOut timeout = new TimeOut("salve", 3000, log);
            ModbusTCPMaster salve = new ModbusTCPMaster(netsetup,timeout,log);
            salve.Connect();
            DeviceAddress da = new DeviceAddress(1, 40001,2, DataType.UShort,ByteOrder.None);
            if(da.Length == 1)
            {
                switch (da.VarType)
                {
                    case DataType.Bool:
                        salve.WriteBool(da, true);
                        Item<bool> resultbool = salve.ReadBool(da);
                        break;
                    case DataType.Short:
                        salve.WriteShort(da, -11);
                        Item<short> resultShort = salve.ReadShort(da);
                        break;
                    case DataType.UShort:
                        salve.WriteUShort(da, 11);
                        Item<ushort> resultUShort = salve.ReadUShort(da);
                        break;
                    case DataType.Dword:
                        salve.WriteInt(da, -1111111111);
                        Item<int> resultInt = salve.ReadInt(da);
                        break;
                }
            }
            else if(da.Length > 1)
            {
                switch (da.VarType)
                {
                    case DataType.Bool:
                        salve.WriteBools(da, new bool[] { true, true });
                        Item<bool>[] resultbools = salve.ReadBools(da,(ushort)da.Length);
                        break;
                    case DataType.Short:
                        salve.WriteShorts(da,new short[] { -11,-12});
                        Item<short>[] resultShorts = salve.ReadShorts(da, (ushort)da.Length);
                        break;
                    case DataType.UShort:
                        salve.WriteUShorts(da, new ushort[] { 11, 12 });
                        Item<ushort>[] resultUShort = salve.ReadUShorts(da, (ushort)da.Length);
                        break;
                    case DataType.Dword:
                        salve.WriteInts(da, new int[] { -1111111111 ,-22222});
                        Item<int>[] resultInts = salve.ReadInts(da, (ushort)da.Length);
                        break;
                    case DataType.UDword:
                        salve.WriteUInts(da, new uint[] { 1111111111, 22222 });
                        Item<uint>[] resultUInts = salve.ReadUInts(da, (ushort)da.Length);
                        break;
                }
            }

            salve.DisConnect();
            Console.ReadKey();
        }
    }
    class TESTLOG : ILog
    {
        public void ByteSteamLog(ActionType action, byte[] bytes)
        {
            string byteSteamString="";
            foreach(byte bt in bytes)
            {
                byteSteamString += string.Format("{0:x2}",bt)+" ";
            }
            switch (action)
            {
                case ActionType.RECEIVE:
                    Console.WriteLine(string.Format("Rx:{0}", byteSteamString));
                    break;
                case ActionType.SEND:
                    Console.WriteLine(string.Format("Tx:{0}", byteSteamString));
                    break;
            }
        }

        public void DebugLog(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void ErrorLog(string format, params object[] parameters)
        {
            Console.WriteLine(format);
        }

        public LogLevel GetLogLevel()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public void NormalLog(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void SetLogLevel(LogLevel level)
        {
            throw new NotImplementedException();
        }

        public void WarningLog(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
