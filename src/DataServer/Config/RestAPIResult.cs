using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataServer.Config
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RestAPIResult
    {
        UNKNOWN=0,
        OK = 1,
        FAIL = 2
    }
}
