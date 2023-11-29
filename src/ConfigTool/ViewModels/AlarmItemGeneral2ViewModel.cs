using ConfigTool.Models;
using ConfigTool.Service;
using DataServer;
using DataServer.Config;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ViewModels
{
    class AlarmItemGeneral2ViewModel: BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private AlarmItemConfig _config;
        //private IConfigDataServer _configDataServer;

        private ObservableCollection<string> alarmTypes;

        public ObservableCollection<string> AlarmTypes
        {
            get { return alarmTypes; }
            set { SetProperty(ref alarmTypes, value, "AlarmTypes"); }
        }


        private string currentAlarmType;

        public string CurrentAlarmType
        {
            get { return currentAlarmType; }
            set { SetProperty(ref currentAlarmType, value, "CurrentAlarmType"); }
        }

        private string level1View;

        public string Level1View
        {
            get { return level1View; }
            set { SetProperty(ref level1View, value, "Level1View"); }
        }

        private string level2View;

        public string Level2View
        {
            get { return level2View; }
            set { SetProperty(ref level2View, value, "Level2View"); }
        }

        private string alarmDescription;

        public string AlarmDescription
        {
            get { return alarmDescription; }
            set { SetProperty(ref alarmDescription, value, "AlarmDescription"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }


        private bool isFristIn = true;
        public AlarmItemGeneral2ViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);

            //_configDataServer = configDataServer;
            alarmTypes = new ObservableCollection<string>( Enum.GetNames(typeof(AlarmType)));
            currentAlarmType = alarmTypes[0];

        }



        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                if (Enum.TryParse(CurrentAlarmType, out AlarmType type))
                {
                    _config.AlarmType = type;
                }
                _config.Level1View = Level1View;
                _config.Level2View = Level2View;
                _config.AlarmDescription = AlarmDescription;
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
                _config = navigationContext.Parameters.GetValue<AlarmItemConfig>("AlarmItemConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    CurrentAlarmType = _config.AlarmType.ToString();
                    Level1View = _config.Level1View;
                    Level2View = _config.Level2View;
                    AlarmDescription = _config.AlarmDescription;
                }
                isFristIn = false;
            }

        }
        #endregion
    }
}
