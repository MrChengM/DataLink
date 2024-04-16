using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Alarm
{
    public class HistoryAlarm
    {
        public string Name { get; set; }
        public string PartName { get; set; }

        public string AlarmDesc { get; set; }
        public AlarmType AlarmLevel { get; set; }

        public string AlarmNumber { get; set; }
        public string L1View { get; set; }
        public string L2View { get; set; }
        public string AlarmGroup { get; set; }
        public DateTime AppearTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
