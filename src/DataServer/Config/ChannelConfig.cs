using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;

namespace DataServer.Config
{
   public  class ChannelConfig
    {

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private DriverInfo driverInformation;

        public DriverInfo DriverInformation
        {
            get { return driverInformation; }
            set { driverInformation = value; }
        }

        private int initTimeOut;

        public int InitTimeOut
        {
            get { return initTimeOut; }
            set { initTimeOut = value; }
        }

        private int initLevel;

        public int InitLevel
        {
            get { return initLevel; }
            set { initLevel = value; }
        }

        private Dictionary<string, DeviceConfig> devices=new Dictionary<string, DeviceConfig>();

        public Dictionary<string,DeviceConfig> Devices
        {
            get { return devices; }
            set { devices = value; }
        }
        private object comunicationSetUp;

        public object ComunicationSetUp
        {
            get { return comunicationSetUp; }
            set { comunicationSetUp = value; }
        }
    }
}
