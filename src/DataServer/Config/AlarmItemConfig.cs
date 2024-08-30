using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using DataServer.Alarm;

namespace DataServer.Config
{
    [DataContract]
    public class AlarmItemConfig
    {
        private string tagName;

        [DataMember]
        public string TagName
        {
            get { return tagName; }
            set { tagName = value; }
        }

        private string alarmTag;
        [DataMember]
        public string AlarmTag
        {
            get { return alarmTag; }
            set { alarmTag = value; }
        }

        private string partName;
        [DataMember]
        public string PartName
        {
            get { return partName; }
            set { partName = value; }
        }

        private string alarmDescription;
        [DataMember]
        public string AlarmDescription
        {
            get { return alarmDescription; }
            set { alarmDescription = value; }
        }

        private AlarmType alarmType;
        [DataMember]
        public AlarmType AlarmType
        {
            get { return alarmType; }
            set { alarmType = value; }
        }
        private string alNumber;
        [DataMember]
        public string ALNumber
        {
            get { return alNumber; }
            set { alNumber = value; }
        }

        private string level1View;
        [DataMember]
        public string Level1View
        {
            get { return level1View; }
            set { level1View = value; }
        }
        private string level2View;
        [DataMember]
        public string Level2View
        {
            get { return level2View; }
            set { level2View = value; }
        }
        private ConditionType conditionType;
        [DataMember]
        public ConditionType ConditionType
        {
            get { return conditionType; }
            set { conditionType = value; }
        }

        
        private float conditionValue;
        [DataMember]
        public float ConditionValue
        {
            get { return conditionValue; }
            set { conditionValue = value; }
        }

        private string alarmGroup;
        [DataMember]
        public string AlarmGroup
        {
            get { return alarmGroup; }
            set { alarmGroup = value; }
        }

        private ConfirmMode confirmMode;

        public ConfirmMode ConfirmMode
        {
            get { return confirmMode; }
            set { confirmMode = value; }
        }

    }
}
