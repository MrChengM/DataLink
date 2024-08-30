using System;
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
using Utillity.Data;
using System.ComponentModel;

namespace ConfigTool.ViewModels
{
    public class ChannelGeneralViewModel : BindableBase, INavigationAware, IDataErrorInfo
    {
        private IEventAggregator _ea;
        private ChannelConfig _config;
        private IConfigDataServer _configDataServer;

        private string name;
        

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string driverInfo;

        public string DriverInfo
        {
            get { return driverInfo; }
            set
            {
                SetProperty(ref driverInfo, value, () =>
                {
                    if (BuildMode)
                    {
                        _config.DriverInformation = _configDataServer.GetDriverInfo(value);
                    }
                }, "DriverInfo"); }
        }


        private ObservableCollection<string> driverInfos;

        public ObservableCollection<string> DriverInfos
        {
            get { return driverInfos; }
            set { SetProperty(ref driverInfos, value, "DriverInfos"); }
        }

        private int initLevel=1;

        public int InitLevel
        {
            get { return initLevel; }
            set { SetProperty(ref initLevel, value, "InitLevel"); }
        }

        private int scanTime = 100;

        public int ScanTime
        {
            get { return scanTime; }
            set { SetProperty(ref scanTime, value, "ScanTime"); }
        }

        private bool buildMode ;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
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
                        result = "Channel Name can not null or empty !";
                    }
                    else if (!RegexCheck.IsString(Name.ToString()))
                    {
                        result = "Channel Name include special character !";
                    }
                    else if (_configDataServer.IsExit_Channel(Name) && BuildMode)
                    {
                        result = "Channel Name is Exit! ";
                    }
                    errorMsgBuffer[0] = result;
                }
                else if (columnName == "DriverInfo")
                {
                    if (string.IsNullOrEmpty(DriverInfo))
                    {
                        result = "DriverInfo can not null or empty !";
                    }
                    errorMsgBuffer[1] = result;
                }
                else if (columnName == "ScanTime")
                {
                    if (ScanTime < 0)
                    {
                        result = "ScanTime can not less than 0 !";

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


        private bool isFristIn = true;
        public ChannelGeneralViewModel(IEventAggregator eventAggregator,IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig );

            _configDataServer = configDataServer;
            driverInfos = new ObservableCollection<string>();
            foreach (var info in _configDataServer.DriverInfos)
            {
                driverInfos.Add(info.Key);
            }
        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                if (BuildMode)
                {

                    _config.Name = Name;
                    _config.DriverInformation = _configDataServer.GetDriverInfo(driverInfo);
                }
                _config.InitLevel = InitLevel;
                _config.ScanTimes = ScanTime;
            }
        }
        #region INavigationAware
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
                _config = navigationContext.Parameters.GetValue<ChannelConfig>("ChannelConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    Name = _config.Name;
                    DriverInfo = _config.DriverInformation.Description;
                    InitLevel = _config.InitLevel;
                    ScanTime = _config.ScanTimes;
                }
                isFristIn = false;
            }
          
        }
        #endregion

    }
}
