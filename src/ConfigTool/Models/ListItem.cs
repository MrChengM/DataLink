using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace ConfigTool.Models
{
    public class ChannelListItem : BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string driverName;

        public string DriverName
        {
            get { return driverName; }
            set { SetProperty(ref driverName, value, "DriverName"); }
        }

        private string connection;

        public string Connection
        {
            get { return connection; }
            set { SetProperty(ref connection, value, "Connection"); }
        }

        private int initLevel;

        public int InitLevel
        {
            get { return initLevel; }
            set { SetProperty(ref initLevel, value, "InitLevel"); }
        }
       
    }
    public class DeviceListItem : BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value, "Id"); }
        }

        private int connectionTimeOut;

        public int ConnectionTimeOut
        {
            get { return connectionTimeOut; }
            set { SetProperty(ref connectionTimeOut, value, "ConnectionTimeOut"); }
        }

        private int requsetTimeOut;

        public int RequestTimeOut
        {
            get { return requsetTimeOut; }
            set { SetProperty(ref requsetTimeOut, value, "InitLevel"); }
        }

        private int retryTimes;

        public int RetryTimes
        {
            get { return retryTimes; }
            set { SetProperty(ref retryTimes, value, "RetryTimes"); }
        }

        private string byteOrder;

        public string ByteOrder
        {
            get { return byteOrder; }
            set { SetProperty(ref byteOrder, value, "ByteOrder"); }
        }

    }

    public class TagGroupListItem:BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private int scan;

        public int Scan
        {
            get { return scan; }
            set { SetProperty(ref scan, value, "Scan"); }
        }


    }
    public class TagListItem:BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value, "Address"); }
        }

        private string dataType;

        public string DataType
        {
            get { return dataType; }
            set { SetProperty(ref dataType, value, "DataType"); }
        }

        private int length;

        public int Length
        {
            get { return length; }
            set { SetProperty(ref length, value, "Length"); }
        }

        private string operateWay;

        public string OperateWay
        {
            get { return operateWay; }
            set { SetProperty(ref operateWay, value, "OperateWay"); }
        }


    }

    public class ServerListItem:BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }


        private string option;

        public string Option
        {
            get { return option; }
            set { SetProperty(ref option, value, "Option"); }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value, "Address"); }
        }

        private string phySetting;

        public string PhySetting
        {
            get { return phySetting; }
            set { SetProperty(ref phySetting, value, "PhySetting"); }
        }

    }   
    public class TagBindingListItem : BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string sourceTag;

        public string SourceTag
        {
            get { return sourceTag; }
            set { SetProperty(ref sourceTag, value, "SourceTag"); }
        }

    }
    public class AlarmListItem : BindableBase
    {
        private string alarmTag;

        public string AlarmTag
        {
            get { return alarmTag; }
            set { SetProperty(ref alarmTag, value, "AlarmTag"); }
        }

        private string conditionName;

        public string ConditionName
        {
            get { return conditionName; }
            set { SetProperty(ref conditionName, value, "ConditionName"); }
        }

        private string alarmType;

        public string AlarmType
        {
            get { return alarmType; }
            set { SetProperty(ref alarmType, value, "AlarmType"); }
        }

        private string alarmDescription;

        public string AlarmDescription
        {
            get { return alarmDescription; }
            set { SetProperty(ref alarmDescription, value, "AlarmDescription"); }
        }
    }
}
