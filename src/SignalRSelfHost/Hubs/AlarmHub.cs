using DataServer;
using DataServer.Log;
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
        private ILog _log;
        private IAlarmHubProxy _hubProxy;
        public AlarmHub(IAlarmHubProxy hubProxy, ILog log)
        {
            _hubProxy = hubProxy;
            _alarmTask = _hubProxy.AlarmTask;
            _log = log;
        }
        public override Task OnConnected()
        {
            _log.InfoLog("SignalServer: Client have Connected ,ConnectId '{0}',HubName 'Alarm'", Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            _log.InfoLog("SignalServer: Client have Reconnected ,connectId '{0}',HubName 'Alarm'", Context.ConnectionId);
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            _log.InfoLog("SignalServer: Client have Disconnected ,connectId '{0}',HubName 'Alarm'", Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
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
