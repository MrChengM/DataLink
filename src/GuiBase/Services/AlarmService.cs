using DataServer;
using DataServer.Alarm;
using GuiBase.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Services
{
    public class AlarmService : IAlarmService
    {
        private IHubProxy hubProxy;
        private readonly string ServerUrl = "http://localhost:3051/";
        private ILog _log;
        //定义一个连接对象
        public HubConnection connection;
        private string _taskName = "AlarmService";
        public List<AlarmWrapper> AllExitAlarms { get;private set; }

        public event Action<AlarmWrapper, AlarmRefresh> AlarmRefreshEvent;
        public event Action<bool> ConnectStatusChangeEvent;
        public bool IsConnect { get; set; }
        public AlarmService(ILog log)
        {
            _log = log;
            initConnection();
        }

        private void initConnection()
        {
            connection = new HubConnection(ServerUrl);
            hubProxy = connection.CreateHubProxy("Alarm");
            hubProxy.On<AlarmInstance, AlarmRefresh>("receiveAlarmMessage", receiveAlarmMessage);
            ServicePointManager.DefaultConnectionLimit = 10;
            connection.Closed += Connection_Closed;
            connection.Reconnected += Connection_Reconnected;
            connection.Error += Connection_Error;
            connection.StateChanged += Connection_StateChanged;
            AllExitAlarms = new List<AlarmWrapper>();
        }

        private void Connection_StateChanged(StateChange state)
        {
            if (state.OldState== ConnectionState.Disconnected&& state.NewState== ConnectionState.Connecting)
            {
                _log.InfoLog($"{_taskName}: Start to connecting { ServerUrl}" );

            }
            else if (state.OldState == ConnectionState.Connecting && state.NewState == ConnectionState.Connected)
            {
                getAllExitAlarmsAsync();
                IsConnect = true;
                ConnectStatusChangeEvent?.Invoke(true);
                _log.InfoLog($"{_taskName}: Connect to { ServerUrl} succefully");

            }
            else if (state.OldState == ConnectionState.Connecting && state.NewState == ConnectionState.Disconnected)
            {
                IsConnect = false;
                ConnectStatusChangeEvent?.Invoke(false);
                _log.InfoLog($"{_taskName}: Connect to { ServerUrl} failed");

            }
            else if (state.OldState == ConnectionState.Connected && state.NewState == ConnectionState.Disconnected)
            {
                IsConnect = false;
                ConnectStatusChangeEvent?.Invoke(false);
                _log.InfoLog($"{_taskName}: Disconnect to { ServerUrl} succefully");
            }
        }

        private void Connection_Error(Exception e)
        {
            _log.ErrorLog($"{_taskName}: Connect { ServerUrl} error'{e.Message}'!");
            IsConnect = false;
            ConnectStatusChangeEvent?.Invoke(false);
        }

        public bool Start()
        {
            bool result;
            try
            {
                connection.Start().Wait();
                result = true;
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: connect to { ServerUrl} fail,{e.Message}!");
                IsConnect = false;
                ConnectStatusChangeEvent?.Invoke(false);
                result = false;
            }
            return result;
        }
        public bool Stop()
        {
            bool result;
            try
            {
                connection.Stop();
                _log.InfoLog($"{_taskName}: Start to disconnect { ServerUrl}");
                result = true;
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: Disconnect to { ServerUrl} fail '{e}'!");
                IsConnect = false;
                ConnectStatusChangeEvent?.Invoke(false);
                result = false;
            }
            return result;
        }
        private void Connection_Reconnected()
        {
            IsConnect = true;
            ConnectStatusChangeEvent?.Invoke(true);
            getAllExitAlarmsAsync();
        }

        private void Connection_Closed()
        {
            IsConnect = false;
            ConnectStatusChangeEvent?.Invoke(false);
        }

        private void receiveAlarmMessage(AlarmInstance instance, AlarmRefresh status)
        {
            ///更新
            var newAlarm = AlarmWrapper.Convert(instance);
            updateAlarms(newAlarm, status);

        }

        private void updateAlarms(AlarmWrapper newAlarm, AlarmRefresh status)
        {
            var oldaAlarm = AllExitAlarms.Find(s => s.AlarmName == newAlarm.AlarmName);
            if (status == AlarmRefresh.Updata)
            {
                if (oldaAlarm != null)
                {
                    oldaAlarm.CopyFrom(newAlarm);
                    AlarmRefreshEvent?.Invoke(newAlarm, AlarmRefresh.Updata);
                }
                else
                {
                    AllExitAlarms.Add(newAlarm);
                    newAlarm.AlarmConfrimEvent += AlarmConfrimEvent;
                    AlarmRefreshEvent?.Invoke(newAlarm, AlarmRefresh.Add);

                }
            }
            else
            {
                if (oldaAlarm != null)
                {
                    AllExitAlarms.Remove(oldaAlarm);
                    oldaAlarm.AlarmConfrimEvent -= AlarmConfrimEvent;
                    AlarmRefreshEvent?.Invoke(newAlarm, AlarmRefresh.Remove);

                }
            }
        }
        private async void getAllExitAlarmsAsync()
        {
            var alarmInstances = await hubProxy.Invoke<List<AlarmInstance>>("GetExitAlarms");

            foreach (var alarm in AllExitAlarms)
            {
                alarm.AlarmConfrimEvent -= AlarmConfrimEvent;
            }
            AllExitAlarms.Clear();
            foreach (var instance in alarmInstances)
            {
                var newAlarm = AlarmWrapper.Convert(instance);
                newAlarm.AlarmConfrimEvent += AlarmConfrimEvent;
                AllExitAlarms.Add(newAlarm);
            }
        }

        private void AlarmConfrimEvent(string name)
        {
            SendConfrim(name);
        }

        public async void SendConfrim(string name)
        {
           await hubProxy.Invoke("AlarmConfrim",name);
        }

    }
}
