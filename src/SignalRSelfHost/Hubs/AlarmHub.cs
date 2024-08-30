using DataServer.Alarm;
using DataServer.Task;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRSelfHost.Hubs
{
    [HubName("Alarm")]
    public class AlarmHub : Hub
    {
        private IAlarmTask _alarmTask;

        public AlarmHub(IAlarmTask alarmTask)
        {
            _alarmTask = alarmTask;
            _alarmTask.AlarmStatusChangeEvent += _alarmTask_AlarmStatusChangeEvent;
        }
        private void _alarmTask_AlarmStatusChangeEvent(AlarmInstance instance, AlarmRefresh status)
        {
            Clients.Caller.receiveAlarmMessage(instance, status);
        }

        public List<AlarmInstance> GetExitAlarms()
        {
            return _alarmTask.GetExitAlarms();
        }

        public void AlarmConfrim(string alarmName)
        {
            _alarmTask.AlarmConfrim(alarmName);
        }
    }
}
