using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    public interface IConfigServer
    {

        ProjectConfig Config { get; set; }

        Dictionary<string,DriverInfo> DriverInfos { get; }
        Dictionary<string, Type> DriverTypes { get; }
        void SaveDrivierInfos();
        void SaveProjectConfig();
        Dictionary<string, DriverInfo> SaveDriverDll(string fileName, Stream stream, out string errorMsg);

        event Action<ProjectConfig> ProConfRefreshEvent;

    }
}
