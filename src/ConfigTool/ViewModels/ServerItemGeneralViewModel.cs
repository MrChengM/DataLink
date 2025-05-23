﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Ioc;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using DataServer.Config;
using System.Collections.ObjectModel;
using ConfigTool.Models;
using ConfigTool.Service;
using DataServer;
using System.ComponentModel;
using Utillity.Data;

namespace ConfigTool.ViewModels
{
    class ServerItemGeneralViewModel : BindableBase, INavigationAware,IDataErrorInfo
    {
        private IEventAggregator _ea;
        private ServerItemConfig _config;
        private IConfigDataServer _configDataServer;

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        private int id = 1;

        public int ID
        {
            get { return id; }
            set { SetProperty(ref id, value, "ID"); }
        }

        private int timeOut = 1000;

        public int TimeOut
        {
            get { return timeOut; }
            set { SetProperty(ref timeOut, value, "TimeOut"); }
        }

        private int maxConnect = 100;

        public int MaxConnect
        {
            get { return maxConnect; }
            set { SetProperty(ref maxConnect, value, "MaxConnect"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }

        private List<string> serverOptions;

        public List<string> ServerOptions
        {
            get { return serverOptions; }
            set { SetProperty(ref serverOptions, value, "ServerOptions"); }
        }

        private string currentServer;

        public string CurrentServer
        {
            get { return currentServer; }
            set { SetProperty(ref currentServer, value, ()=> {
                if (BuildMode)
                {
                    if (Enum.TryParse(value, out ServerOption option))
                    {
                        _config.Option = option;
                    }
                }
            }, "CurrentServer"); }
        }

        private bool isFristIn = true;
        #region IDataErrorInfo
        public string Error => null;
        private string[] errorMsgBuffer = new string[7];

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == "Name")
                {

                    if (string.IsNullOrEmpty(Name))
                    {
                        result = "Server Name can not null or empty !";
                    }
                    else if (!RegexCheck.IsString(Name.ToString()))
                    {
                        result = "Server Name include special character !";
                    }
                    else if (_configDataServer.IsExit_ServerItem(Name) && BuildMode)
                    {
                        result = "Server Name is Exit! ";
                    }
                    errorMsgBuffer[0] = result;
                }
                else if (columnName == "TimeOut")
                {
                    if (TimeOut < 0)
                    {
                        result = "Value can not less than 0 !";

                    }
                    errorMsgBuffer[1] = result;
                }
                else if (columnName == "MaxConnect")
                {
                    if (MaxConnect < 1)
                    {
                        result = "Value can not less than 0 !";

                    }
                    errorMsgBuffer[2] = result;
                }
                judgeHasError();
                return result;
            }

        }
        void judgeHasError()
        {
            bool hasError = false;
            foreach (var errorMsg in errorMsgBuffer)
            {
                if (errorMsg != string.Empty && errorMsg != null)
                {
                    hasError = true;
                    break;
                }
            }
            _ea.GetEvent<PubSubEvent<bool>>().Publish(hasError);

        }
        #endregion

        public ServerItemGeneralViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);
            _configDataServer = configDataServer;

            serverOptions = new List<string>();
            foreach (var type in Enum.GetNames(typeof(ServerOption)))
            {
                serverOptions.Add(type);
            }
            currentServer = serverOptions[0];
        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                if (buildMode)
                {
                    _config.Name = Name;
                    if (Enum.TryParse(CurrentServer, out ServerOption option))
                    {
                        _config.Option = option;
                    }
                }
                _config.ID = ID;
                _config.MaxConnect = MaxConnect;
                _config.TimeOut = TimeOut;

            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (isFristIn)
            {
                _config = navigationContext.Parameters.GetValue<ServerItemConfig>("ServerItemConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    Name = _config.Name;
                    ID = _config.ID;
                    CurrentServer = _config.Option.ToString();
                    MaxConnect = _config.MaxConnect;
                    TimeOut = _config.TimeOut;
                }
                isFristIn = false;
            }
        }
    }
}
