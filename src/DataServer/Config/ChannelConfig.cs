using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using Newtonsoft.Json;

namespace DataServer.Config
{
    [DataContract]
    //[JsonConverter(typeof(ChannelJsonConvert))]
    public  class ChannelConfig
    {

        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private DriverInfo driverInformation;

        [DataMember]
        public DriverInfo DriverInformation
        {
            get { return driverInformation; }
            set { driverInformation = value; }
        }

        private int initTimeOut;

        [DataMember]
        public int InitTimeOut
        {
            get { return initTimeOut; }
            set { initTimeOut = value; }
        }

        private int initLevel;
        [DataMember]
        public int InitLevel
        {
            get { return initLevel; }
            set { initLevel = value; }
        }

        private Dictionary<string, DeviceConfig> devices=new Dictionary<string, DeviceConfig>();
        [DataMember]
        public Dictionary<string,DeviceConfig> Devices
        {
            get { return devices; }
            set { devices = value; }
        }
        private ComPhyLayerSetting comunicationSetUp=new ComPhyLayerSetting();
        [DataMember]
        public ComPhyLayerSetting ComunicationSetUp
        {
            get { return comunicationSetUp; }
            set { comunicationSetUp = value; }
        }
    }
}
