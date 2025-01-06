using DataServer.Alarm;
using GuiBase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Services
{
    public interface IAlarmService
    {
        List<AlarmWrapper> AllExitAlarms { get; }
        event Action<AlarmWrapper, AlarmRefresh> AlarmRefreshEvent;
        event Action<bool> ConnectStatusChangeEvent;

        bool Start();
        bool Stop();
        bool IsConnect { get; }

        //void SendConfrim(string alarmName);

    }
}
