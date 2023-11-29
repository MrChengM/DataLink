using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
     public interface IServerDrivers:IDisposable
    {
        bool Init();
        bool Start();
        bool Stop();
        bool IsRunning { get;}
    }
    public enum ServerOption
    {
        [DriverDescription("ModbusTCPServer",CommunicationType.Ethernet)]
        ModbusTCP,
        [DriverDescription("ModbusRTUServer", CommunicationType.Serialport)]
        ModbusRTU,
        [DriverDescription("FreedomServer", CommunicationType.Ethernet)]
        Freedom

    }
}
