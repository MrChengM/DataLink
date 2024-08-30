using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Alarm;
using DataServer.Config;

namespace DataServer.Task
{
    public interface IAlarmTask
    {
        AlarmsConfig AlarmsConfig { get; set; }
        List<AlarmInstance> GetExitAlarms();
        event Action<AlarmInstance, AlarmRefresh> AlarmStatusChangeEvent;
        void AlarmConfrim(string alarmName);
    }
}
