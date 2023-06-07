using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Config;
using System.Collections.ObjectModel;

namespace ConfigTool.Models
{
    public static class GlobalVar
    {
        public static Dictionary<string,DriverInfo> DriverInfos = new Dictionary<string, DriverInfo>();
        public static ProjectConfig ProjectConfig = new ProjectConfig();
    }
}
