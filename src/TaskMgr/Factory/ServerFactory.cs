using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Log;
using DataServer.Config;
using ModbusServer;
using FreedomDriversV2;
using DataServer.Points;
namespace TaskMgr.Factory
{
    public class ServerFactory
    {

        IPointMapping _pointMapping;
        ILog _log;
        public ServerFactory(IPointMapping pointMapping,ILog log)
        {
            _pointMapping = pointMapping;
            _log = log;
        }
        public IServerDrivers CreateInstance(ServerItemConfig serverItemConfig)
        {
            switch (serverItemConfig.Option)
            {
                case ServerOption.ModbusTCP:
                    return new ModbusTCPServer(serverItemConfig,_log) { PointMapping = _pointMapping};
                case ServerOption.ModbusRTU:
                    return null;
                case ServerOption.Freedom:
                    return new FreedomServer(serverItemConfig,_log) { PointMapping = _pointMapping};
                default:
                    return null;
            }
        }
    }
}
