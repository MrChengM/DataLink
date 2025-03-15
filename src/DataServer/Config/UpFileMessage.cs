using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DataServer.Config;

namespace DataServer.Config
{

    public class UpDllFileResult
    {
        public RestAPIResult Result { get; set; }
        public string ErrorMsg { get; set; }
        public Dictionary<string,DriverInfo> DriverInfos { get; set; }
    }
}
