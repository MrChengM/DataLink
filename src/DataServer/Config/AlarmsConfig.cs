using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    [DataContract]
    public class AlarmsConfig
    {
        private Dictionary<string,AlarmItemConfig> alarmGroup=new Dictionary<string, AlarmItemConfig>();
        [DataMember]
        public Dictionary<string,AlarmItemConfig> AlarmGroup
        {
            get { return alarmGroup; }
            set { alarmGroup = value; }
        }

    }

  
}
