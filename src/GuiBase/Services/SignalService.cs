using DataServer;
using DataServer.Log;
using DataServer.Alarm;
using DataServer.Points;
using GuiBase.Models;
using GuiBase.Common;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GuiBase.Services
{
   public class SignalService: ISignalService
    {
        private IHubProxy hubProxy;
        private readonly string ServerUrl = "http://localhost:3051/";
        private ILog _log;
        //定义一个连接对象
        public HubConnection connection;
        private string _taskName = "SignalService";
        //private ISignalMangement _signalMangement;

        public event Action<bool> ConnectStatusChangeEvent;
        public event Action<Tag> ReceiveTagEvent;
        public event Action<List<Tag>> ReceiveTagsEvent;

        public bool IsConnect { get; set; }
        public SignalService(ILog log)
        {
            _log = log;
            initConnection();
        }
        //public SignalService(ILog log,ISignalMangement signalMangement)
        //{
        //    _log = log;
        //    _signalMangement = signalMangement;
        //    _signalMangement.SignalMappingChangeEvent += onSignalMappingChangeEvent;
        //    initConnection();
        //}

        //private void onSignalMappingChangeEvent(SignalMappingtState state, ITag tag)
        //{
        //    if (state == SignalMappingtState.Add)
        //    {
        //        subscribe(tag.Name);
        //    }
        //    else if (state == SignalMappingtState.Remove)
        //    {
        //        unsubscribe(tag.Name);
        //    }
        //}

        private void initConnection()
        {
            connection = new HubConnection(ServerUrl);

            ///SignalR消息日志开启
            connection.TraceLevel = TraceLevels.All;
            connection.TraceWriter = Console.Out;
            hubProxy = connection.CreateHubProxy("Signals");
            ///方法必须满足驼峰原则
            hubProxy.On<Tag>("receiveTag", receiveTag); 
            hubProxy.On<List<Tag>>("receiveTags", receiveTags);

            ServicePointManager.DefaultConnectionLimit = 10;
            connection.Closed += Connection_Closed;
            connection.Reconnected += Connection_Reconnected;
            connection.Error += Connection_Error;
            connection.StateChanged += Connection_StateChanged;
        }

        private void Connection_StateChanged(StateChange state)
        {
            if (state.OldState == ConnectionState.Disconnected && state.NewState == ConnectionState.Connecting)
            {
                _log.InfoLog($"{_taskName}: Start to connecting { ServerUrl}");

            }
            else if (state.OldState == ConnectionState.Connecting && state.NewState == ConnectionState.Connected)
            {
                reconnectTimes = 0;

                IsConnect = true;
                ConnectStatusChangeEvent?.Invoke(true);
                //subscribeAllTags();
                _log.InfoLog($"{_taskName}: Connect to { ServerUrl} succefully");

            }
            else if (state.OldState == ConnectionState.Connecting && state.NewState == ConnectionState.Disconnected)
            {
                IsConnect = false;
                ConnectStatusChangeEvent?.Invoke(false);
                _log.InfoLog($"{_taskName}: Connect to { ServerUrl} failed");
            }
            //else if (state.OldState == ConnectionState.Connected && state.NewState == ConnectionState.Disconnected)
            //{
            //    IsConnect = false;
            //    ConnectStatusChangeEvent?.Invoke(false);
            //    _log.InfoLog($"{_taskName}: Disconnect to { ServerUrl} succefully");
            //}
        }

        private void Connection_Error(Exception e)
        {
            _log.ErrorLog($"{_taskName}: Connect to { ServerUrl} error'{e.Message}'!");
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
                _log.ErrorLog($"{_taskName}: Connect to { ServerUrl} fail,{e.Message}!");
                IsConnect = false;
                ConnectStatusChangeEvent?.Invoke(false);
                result = false;
            }
            return result;
        }
        private bool stopFlag = false;
        public bool Stop()
        {
            bool result;
            try
            {
                stopFlag = true;
                connection.Stop();
                _log.InfoLog($"{_taskName}: Stop the connection,Url '{ServerUrl}'");
                result = true;
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: Stop the connection fail, Url'{ ServerUrl}' Error '{e}'!");
                IsConnect = false;
                ConnectStatusChangeEvent?.Invoke(false);
                result = false;
            }
            return result;
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
        private void Connection_Reconnected()
        {
            _log.InfoLog($"{_taskName}: Reconnect to '{ServerUrl}' succefully!");
            //subscribeAllTags();
            IsConnect = true;
            ConnectStatusChangeEvent?.Invoke(true);
        }

        private void Connection_Closed()
        {
            _log.InfoLog($"{_taskName}: Connection have closed ,Url '{ServerUrl}'!");
            IsConnect = false;
            ConnectStatusChangeEvent?.Invoke(false);
            riseReconnect();
        }

        private void receiveTag(Tag tag)
        {
            ///更新
            App.Current.Dispatcher.InvokeAsync(()=>ReceiveTagEvent?.Invoke(tag));
        }

        private void receiveTags(List<Tag> tags)
        {
            ///更新
            App.Current.Dispatcher.InvokeAsync(() => ReceiveTagsEvent?.Invoke(tags));

        }
        public void Subscribe(string tagName)
        {
             hubProxy.Invoke("Subscribe", tagName);
        }

        public void Unsubscribe(string tagName)
        {
             hubProxy.Invoke("Unsubscribe", tagName);
        }
        public void Subscribe(List<string> tagNames)
        {
            hubProxy.Invoke("SubscribeMultiple", tagNames);
        }
        public void Unsubscribe(List<string> tagNames)
        {
            hubProxy.Invoke("UnsubscribeMultiple", tagNames);
        }
        //private void subscribeAllTags()
        //{
        //    var tags = _signalMangement.GetAllTag();
        //    if (tags!=null)
        //    {
        //        var tagNames = new List<string>();

        //        foreach (var tag in tags)
        //        {
        //            tagNames.Add(tag.Name);
        //        }
        //        subscribeMult(tagNames);
        //    }
        //}
        public Task<Tag> ReadAsync(string tagName)
        {
           return hubProxy.Invoke<Tag>("ReadTag", tagName);
        }
        public Task<List<Tag>> ReadAsync(List<string> tagNames)
        {
            return hubProxy.Invoke<List<Tag>>("ReadTags", tagNames);
        }
        public Task<WriteResult> WriteAsync(Tag tag)
        {
            return hubProxy.Invoke<WriteResult>("WriteTag", tag);
        }

        public Task<WriteResult> WriteAsync(List<Tag> tags)
        {
            return hubProxy.Invoke<WriteResult>("WriteTags", tags);
        }

        public Tag Read(string tagName)
        {
            return ReadAsync(tagName).Result;
        }

        public List<Tag> Read(List<string> tagNames)
        {
            return ReadAsync(tagNames).Result;
        }

        public WriteResult Write(Tag tag)
        {
            return WriteAsync(tag).Result;
        }

        public WriteResult Write(List<Tag> tags)
        {
            return WriteAsync(tags).Result;
        }
    }
}
