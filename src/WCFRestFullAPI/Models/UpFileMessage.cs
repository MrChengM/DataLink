using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DataServer.Config;

namespace WCFRestFullAPI.Models
{
    //[DataContract]
    //public class UpFileMessage
    //{
    //    [DataMember]
    //    public string FileName { get; set; }
    //    [DataMember]
    //    public Stream FileStream { get; set; }

    //}
    [DataContract]
    public class UpDllFileResult
    {
        [DataMember]
        public RestAPIResult Result { get; set; }
        [DataMember]
        public string ErrorMsg { get; set; }
        [DataMember]
        public Dictionary<string,DriverInfo> DriverInfos { get; set; }
    }
}
