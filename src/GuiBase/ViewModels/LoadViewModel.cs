﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Ioc;
using Prism.Commands;
using GuiBase.Services;
using DataServer;
using System.Threading;
using Prism.Events;
using GuiBase.Models;

namespace GuiBase.ViewModels
{
    public class LoadViewModel : BindableBase
    {

        private IContainerProvider _container;
        private IEventAggregator _ea;
        private ILog _log;

        private string message="Loading...";

        //public DelegateCommand LoadCommand { get; set; }

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value, "Message"); }
        }


        public LoadViewModel(IContainerProvider container, IEventAggregator ea)
        {
            _container = container;
            //LoadCommand = new DelegateCommand(load);
            _ea = ea;
            Task.Run(load);

        }

        private void load()
        {
            Thread.Sleep(200);
            _log = _container.Resolve<ILog>();
            _log.Init("GuiBaseLogger");
            if (!loadAlarmService())
            {
                Message = "Alarm Service Loading failed";
                _ea.GetEvent<PubSubEvent<LoadResult>>().Publish(LoadResult.Fail);
                return;
            }
            _ea.GetEvent<PubSubEvent<LoadResult>>().Publish(LoadResult.Success);
        }

        private bool loadAlarmService()
        {
            Message = "Alarm Service Loading...";
            bool result;
            
            var alarmService= _container.Resolve<IAlarmService>();
            result = alarmService.Start();
            return result;
        }
    }
}
