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

namespace ConfigTool.ViewModels
{
    public class ChannelGeneralViewModel : BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private ChannelConfig _config;


        private string name="Channel1";

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
                        _config.DriverInformation = GlobalVar.DriverInfos[value];
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

        private int initTimeOut=1000;

        public int InitTimeOut
        {
            get { return initTimeOut; }
            set { SetProperty(ref initTimeOut, value, "InitTimeOut"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
        private bool isFristIn = true;
        public ChannelGeneralViewModel(IEventAggregator eventAggregator)
        {
            driverInfos = new ObservableCollection<string>();
            foreach (var info in GlobalVar.DriverInfos)
            {
                driverInfos.Add(info.Key);
            }

            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig );
        }

        private void setConfig(ButtonResult button)
        {
            if (button==ButtonResult.OK)
            {
                if (BuildMode)
                {
                    _config.Name = Name;
                    _config.DriverInformation = GlobalVar.DriverInfos[DriverInfo];
                }
                _config.InitLevel = InitLevel;
                _config.InitTimeOut = InitTimeOut;
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
                BuildMode = navigationContext.Parameters.GetValue<bool>("isNewOne");
                if (!BuildMode)
                {
                    Name = _config.Name;
                    DriverInfo = _config.DriverInformation.Description;
                    InitLevel = _config.InitLevel;
                    InitTimeOut = _config.InitTimeOut;
                }
                isFristIn = false;
            }
          
        }
        #endregion

    }
}
