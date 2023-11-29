using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DataServer;

namespace DataServer.Config
{
    [DataContract]
    public  class DriverInfo
    {
        private string fullName;
        [DataMember]
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }
        private CommunicationType commType;
        [DataMember]
        public CommunicationType CommType
        {
            get { return commType; }
            set { commType = value; }
        }

        private string description;
        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        private List<DevicePropertyInfo> devicePropertyInfos =new List<DevicePropertyInfo>();
        [DataMember]
        public List<DevicePropertyInfo> DevicePropertyInfos
        {
            get { return devicePropertyInfos ; }
            set { devicePropertyInfos  = value; }
        }


    }
    [DataContract]
   public class DevicePropertyInfo
    {
        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Type propertyType;
        [DataMember]
        public Type PropertyType
        {
            get { return propertyType; }
            set { propertyType = value; }
        }

    }
}
