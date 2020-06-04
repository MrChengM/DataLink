using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using ds=DataServer;
using DataServer.Serialization;
using DataServer.Points;

namespace ModbusDrivers.Server
{
    public class ModbusPointsRegister
    {
        public static void Register(PointVirtualCollcet points, ILog log)
        {
            var mapping = ModbusPointMapping.GetInstance(log);
            foreach (var point in points.BoolPoints)
            {
                mapping.Register(point.Name, point);
            }
            foreach (var point in points.UshortPoints)
            {
                mapping.Register(point.Name, point);
            }
        }
    }

}
