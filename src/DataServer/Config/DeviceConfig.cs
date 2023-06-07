using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;

namespace DataServer.Config
{
    public class DeviceConfig
    {

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string id;

        /// <summary>
        /// 设备地址,如：Salve ID,IP Address
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private List<DeviceSpecialProperty> specialProperties=new List<DeviceSpecialProperty>();

        public List<DeviceSpecialProperty> SpecialProperties
        {
            get { return specialProperties; }
            set { specialProperties = value; }
        }

        private int connectTimeOut;

        public int ConnectTimeOut
        {
            get { return connectTimeOut; }
            set { connectTimeOut = value; }
        }

        private int requestTimeOut;

        public int RequestTimeOut
        {
            get { return requestTimeOut; }
            set { requestTimeOut = value; }
        }

        private int retryTimes;

        public int RetryTimes
        {
            get { return retryTimes; }
            set { retryTimes = value; }
        }


        private Dictionary<string, TagGroupConfig> tagGroups = new Dictionary<string, TagGroupConfig>();

        public Dictionary<string,TagGroupConfig> TagGroups
        {
            get { return tagGroups; }
            set { tagGroups = value; }
        }

        private ByteOrder byteOrder;

        public ByteOrder ByteOrder
        {
            get { return byteOrder; }
            set { byteOrder = value; }
        }
    }
}
