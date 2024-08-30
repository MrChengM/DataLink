using ConfigTool.Models;
using ConfigTool.Service;
using DataServer.Alarm;
using DataServer.Config;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Data;

namespace ConfigTool.ViewModels
{
    class AlarmItemGeneral2ViewModel: BindableBase, INavigationAware,IDataErrorInfo
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

        private ConfirmMode currentConfirmMode;

        public ConfirmMode CurrentConfirmMode
        {
            get { return currentConfirmMode; }
            set { SetProperty(ref currentConfirmMode, value, "CurrentConfirmMode"); }
        }

        private ObservableCollection<ConfirmMode> confirmModes;

        public ObservableCollection<ConfirmMode> ConfirmModes
        {
            get { return confirmModes; }
            set { SetProperty(ref confirmModes, value, "ConfirmModes"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
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

                if (columnName == "CurrentAlarmType")
                {

                    if (string.IsNullOrEmpty(CurrentAlarmType))
                    {
                        result = "Alarm Type can not null or empty !";
                    }
                    errorMsgBuffer[0] = result;
                }
                else if (columnName == "Level1View")
                {
                    if (string.IsNullOrEmpty(Level1View))
                    {
                        result = "Level1 View can not null or empty !";
                    }
                    errorMsgBuffer[1] = result;
                }
                else if (columnName == "Level2View")
                {
                    if (string.IsNullOrEmpty(Level2View))
                    {
                        result = "Level2  View can not null or empty !";
                    }
                    errorMsgBuffer[2] = result;
                }
                else if (columnName == "AlarmDescription")
                {
                    if (string.IsNullOrEmpty(AlarmDescription))
                    {
                        result = "Alarm Description can not null or empty !";
                    }
                    errorMsgBuffer[3] = result;
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

        public AlarmItemGeneral2ViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);

            //_configDataServer = configDataServer;
            alarmTypes = new ObservableCollection<string>( Enum.GetNames(typeof(AlarmType)));
            currentAlarmType = alarmTypes[0];
            confirmModes = new ObservableCollection<ConfirmMode>() { ConfirmMode.Normal,ConfirmMode.Auto};
            currentConfirmMode = confirmModes[0];
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
                _config.ConfirmMode = CurrentConfirmMode;
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
                    CurrentConfirmMode = _config.ConfirmMode;
                }
                isFristIn = false;
            }

        }
        #endregion
    }
}
