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
    public class AlarmHubProxy:IAlarmHubProxy
    {
        private readonly object locker = new object();

        private IHubContext _hubContext;
        private IAlarmTask _alarmTask;
        public IAlarmTask AlarmTask => _alarmTask;
      
        public AlarmHubProxy(IAlarmTask alarmTask )
        {
            _alarmTask = alarmTask;
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<AlarmHub>();
            _alarmTask.AlarmStatusChangeEvent += OnAlarmStatusChangeEvent;

        }

        public void OnAlarmStatusChangeEvent(AlarmInstance instance, AlarmRefresh status)
        {
            lock (locker)
            {
                 _hubContext.Clients.All.receiveAlarmMessage(instance, status);
            }

        }


    }
}
