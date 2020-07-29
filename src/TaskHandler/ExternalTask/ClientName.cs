using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHandler
{
    public enum ClientName
    {
        ModbusTCPClient,
        ModbusRTUClient,
        DL645_1997Client,
        DL645_2007Client,
        S7CommClient
    }
}
