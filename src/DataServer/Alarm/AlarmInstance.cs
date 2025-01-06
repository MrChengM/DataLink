using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Alarm
{
    public class AlarmInstance
    {
        public string Name { get; set; }
        public bool IsEnable { get; set; }
        public bool IsCheck { get; set; }
        public string PartName { get; set; }
        public string AlarmDesc { get; set; }
        public AlarmType AlarmLevel { get; set; }
        public string AlarmNumber { get; set; }
        public string L1View { get; set; }
        public string L2View { get; set; }
        public string AlarmGroup { get; set; }
        public DateTime AppearTime { get; set; }
        public DateTime EndTime { get; set; }
        public ConfirmMode ConfirmMode { get; set; }
        public int Count { get; set; }
    }
    public enum AlarmType
    {
        Information = 25,
        Trivial = 50,
        Minor = 75,
        Major = 100,

    }
    public enum ConfirmMode
    {
        Normal,
        Auto,
    }
    public enum ConditionType
    {
        Bit,
        MoreThan,
        LessThan,
        Equals,
        NotEquals
    }

    public enum AlarmRefresh
    {
        Add,
        Updata,
        Remove
    }
}
