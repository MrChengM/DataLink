using DataServer.Alarm;
using DataServer.Points;
using DataServer.Task;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRSelfHost.Hubs
{
    /// <summary>
    /// SignalsHub 类外部调用客户端方法代理类，防止多次调用客户端方法
    /// </summary>
    public interface IAlarmHubProxy
    {
        IAlarmTask AlarmTask { get; }

        void OnAlarmStatusChangeEvent(AlarmInstance instance, AlarmRefresh status);


    }
}
