using DataServer.Config;
using DataServer.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DataServer.Task
{
    public interface IServerTask
    {
        IPointMapping PointMapping { get; set; }
        ServerItemConfig ServerConfig { get; set; }
    }
}
