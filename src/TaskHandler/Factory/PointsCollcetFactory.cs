using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Points;
using DataServer.Serialization;
using DataServer;
using ModbusDrivers.Server;

namespace TaskHandler.Factory
{
    public class PointsCollcetFactory
    {

        XMLWorkbook _workbook;
        ILog _log;
         public PointsCollcetFactory(XMLWorkbook workbook,ILog log)
        {
            _workbook = workbook;
            _log = log;
        }
        public PointDeviceCollcet CreatePoints(ClientName name)
        {
            if (name == ClientName.DL645_1997Client|| name == ClientName.DL645_2007Client)
            {
               return PointsCollcetCreate.CreateDL645(_workbook, _log);
            }
            else if (name == ClientName.S7CommClient)
            {
                return PointsCollcetCreate.CreateS7(_workbook, _log);
            }
            else
            {
                return PointsCollcetCreate.Create(_workbook, _log);
            }
        }
      public PointVirtualCollcet CreatePoints(ServerName name)
        {
            PointVirtualCollcet result = new PointVirtualCollcet();
            if (name == ServerName.ModbusTCPServer)
            {
                result = PointsCollcetCreate.CreateMoudbus(_workbook, _log);
                ModbusPointsRegister.Register(result, _log);
            }
            else if (name == ServerName.ModbusRTUServer)
            {
                result = PointsCollcetCreate.CreateMoudbus(_workbook, _log);
                ModbusPointsRegister.Register(result, _log);
            }
            return result;
        }
    }
}
