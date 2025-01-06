using DataServer;
using DataServer.Log;
using DataServer.Alarm;
using GuiBase.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            ///SignalR消息日志开启
            connection.TraceLevel = TraceLevels.All;
            connection.TraceWriter = Console.Out;
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
                reconnectTimes = 0;
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
            //else if (state.OldState == ConnectionState.Connected && state.NewState == ConnectionState.Reconnecting)
            //{
            //    IsConnect = false;
            //    ConnectStatusChangeEvent?.Invoke(false);
            //    _log.InfoLog($"{_taskName}: Start to Reconnecting { ServerUrl}");
            //}
            //else if (state.OldState == ConnectionState.Reconnecting && state.NewState == ConnectionState.Connected)
            //{
            //    IsConnect = true;
            //    ConnectStatusChangeEvent?.Invoke(true);
            //    _log.InfoLog($"{_taskName}:Reconnect to { ServerUrl} succefully");
            //}
            //else if (state.OldState == ConnectionState.Reconnecting && state.NewState == ConnectionState.Disconnected)
            //{
            //    IsConnect = false;
            //    ConnectStatusChangeEvent?.Invoke(false);
            //    _log.InfoLog($"{_taskName}:Reconnect to { ServerUrl} failed");
            //}
            //else if (state.OldState == ConnectionState.Connected && state.NewState == ConnectionState.Disconnected)
            //{
            //    IsConnect = false;
            //    ConnectStatusChangeEvent?.Invoke(false);
            //    _log.InfoLog($"{_taskName}: Disconnect to { ServerUrl} succefully");
            //}
        }

        private void Connection_Error(Exception e)
        {
            _log.ErrorLog($"{_taskName}: Connect to { ServerUrl} error,'{e.Message}'!");
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
                _log.ErrorLog($"{_taskName}: Stop connecting to { ServerUrl} error,'{e.Message}'!");
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
                stopFlag = true;
                connection.Stop();
                _log.InfoLog($"{_taskName}: Start to disconnect { ServerUrl}!");
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
        private bool stopFlag = false;
        private void Connection_Reconnected()
        {
            IsConnect = true;
            ConnectStatusChangeEvent?.Invoke(true);
            _log.InfoLog($"{_taskName}: Reconnect to { ServerUrl} succefully !");
            getAllExitAlarmsAsync();
        }

        private void Connection_Closed()
        {
            IsConnect = false;
            ConnectStatusChangeEvent?.Invoke(false);
            _log.InfoLog($"{_taskName}: Connection have closed,Url '{ ServerUrl}' !");
            riseReconnect();
        }
        int reconnectTimes = 0;
        /// <summary>
        /// 触发断线重连,10S重连一次
        /// </summary>
        private void riseReconnect()
        {
            if (!stopFlag)
            {
                var task = Task.Factory.StartNew
                (() =>
                {

                    reconnectTimes++;
                    Thread.Sleep(10000);
                    _log.InfoLog($"{_taskName}:Reconnect to '{ServerUrl}' by manual,times:{reconnectTimes}");
                    Start();

                }
                );
            }
        }
        private void receiveAlarmMessage(AlarmInstance instance, AlarmRefresh status)
        {
            ///更新
            var newAlarm = AlarmWrapper.Convert(instance);
            updateAlarms(newAlarm, status);

        }

        private void updateAlarms(AlarmWrapper newAlarm, AlarmRefresh status)
        {
            var oldAlarm = AllExitAlarms.Find(s => s.AlarmName == newAlarm.AlarmName);
            if (status == AlarmRefresh.Updata)
            {
                if (oldAlarm != null)
                {
                    oldAlarm.CopyFrom(newAlarm);
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
                if (oldAlarm != null)
                {
                    AllExitAlarms.Remove(oldAlarm);
                    oldAlarm.AlarmConfrimEvent -= AlarmConfrimEvent;
                    oldAlarm.Clear();
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
                alarm.Clear();
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

        public void SendConfrim(string name)
        {
            hubProxy.Invoke("AlarmConfrim", name);
        }

    }
}
