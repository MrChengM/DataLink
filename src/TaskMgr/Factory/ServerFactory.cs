using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Config;
using ModbusServer;
using FreedomDriversV2;
namespace TaskMgr.Factory
{
    public class ServerFactory
    {
        public IServerDrivers CreateInstance(ServerOption serverOption)
        {
            switch (serverOption)
            {
                case ServerOption.ModbusTCP:
                    return new ModbusTCPServer();
                case ServerOption.ModbusRTU:
                    return null;
                case ServerOption.Freedom:
                    return new FreedomServer();
                default:
                    return null;
            }
        }
    }
}
