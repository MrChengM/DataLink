using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WCFRestFullAPI.Models
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RestAPIResult
    {
        [EnumMember]
        UNKNOWN=0,
        [EnumMember]
        OK = 1,
        [EnumMember]
        FAIL = 2
    }
}
