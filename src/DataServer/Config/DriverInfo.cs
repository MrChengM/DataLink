using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataServer;

namespace DataServer.Config
{
    public  class DriverInfo
    {
        private string fullName;

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }
        private CommunicationType commType;

        public CommunicationType CommType
        {
            get { return commType; }
            set { commType = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        private List<PropertyInfo> devicePropertyDes=new List<PropertyInfo>();

        public List<PropertyInfo> DevicePropertyDes
        {
            get { return devicePropertyDes; }
            set { devicePropertyDes = value; }
        }


    }
}
