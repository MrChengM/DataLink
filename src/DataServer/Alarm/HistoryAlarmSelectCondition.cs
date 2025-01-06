using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Alarm
{
    public class HistoryAlarmSelectCondition 
    {


        public string AlarmName
        {
            get;
            set;
        }
        public string AlarmLevel
        {
            get ;
            set;
        }
        public string AlarmGroup
        {
            get ;
            set;
        }
        public string L1View
        {
            get ;
            set;
        }
        public string L2View
        {
            get ;
            set;
        }

        public DateTime StartDate
        {
            get ;
            set;
        }
        public DateTime EndDate
        {
            get ;
            set;
        }

    }
}
