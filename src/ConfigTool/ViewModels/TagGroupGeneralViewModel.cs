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

namespace ConfigTool.ViewModels
{
    public class TagGroupGeneralViewModel : BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private TagGroupConfig _config;
        private IConfigDataServer _configDataServer;

        private string name="Group1";

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        private int scanTimes=100;

        public int ScanTimes
        {
            get { return scanTimes; }
            set { SetProperty(ref scanTimes, value, "ScanTimes"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
        private bool isFristIn = true;
        public TagGroupGeneralViewModel(IEventAggregator eventAggregator,IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig );

            _configDataServer = configDataServer;
        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                _config.Name = Name;
                _config.ScanTimes = ScanTimes;
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
                _config = navigationContext.Parameters.GetValue<TagGroupConfig>("TagGroupConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    Name = _config.Name;
                    ScanTimes = _config.ScanTimes;
                }
                isFristIn = false;
            }
        }
        #endregion

    }
}
