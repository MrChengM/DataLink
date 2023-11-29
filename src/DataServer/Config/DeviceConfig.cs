using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DataServer;

namespace DataServer.Config
{
    [DataContract]
    public class DeviceConfig
    {

        private string name;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string id;

        /// <summary>
        /// 设备地址,如：Salve ID,IP Address
        /// </summary>
        [DataMember]
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private List<DeviceSpecialProperty> specialProperties=new List<DeviceSpecialProperty>();

        [DataMember]
        public List<DeviceSpecialProperty> SpecialProperties
        {
            get { return specialProperties; }
            set { specialProperties = value; }
        }

        private int connectTimeOut;

        [DataMember]
        public int ConnectTimeOut
        {
            get { return connectTimeOut; }
            set { connectTimeOut = value; }
        }

        private int requestTimeOut;
        [DataMember]
        public int RequestTimeOut
        {
            get { return requestTimeOut; }
            set { requestTimeOut = value; }
        }

        private int retryTimes;
        [DataMember]
        public int RetryTimes
        {
            get { return retryTimes; }
            set { retryTimes = value; }
        }


        private Dictionary<string, TagGroupConfig> tagGroups = new Dictionary<string, TagGroupConfig>();
        [DataMember]
        public Dictionary<string,TagGroupConfig> TagGroups
        {
            get { return tagGroups; }
            set { tagGroups = value; }
        }

        private ByteOrder byteOrder;
        [DataMember]
        public ByteOrder ByteOrder
        {
            get { return byteOrder; }
            set { byteOrder = value; }
        }
    }
}
