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
using System.ComponentModel;
using Utillity.Data;

namespace ConfigTool.ViewModels
{
    public class TagGroupGeneralViewModel : BindableBase, INavigationAware,IDataErrorInfo
    {
        private IEventAggregator _ea;
        private TagGroupConfig _config;
        private IConfigDataServer _configDataServer;
        private DeviceConfig _deviceConfig;

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
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

                if (columnName == "Name")
                {

                    if (string.IsNullOrEmpty(Name))
                    {
                        result = "TagGroup Name can not null or empty !";
                    }
                    else if (!RegexCheck.IsString(Name.ToString()))
                    {
                        result = "TagGroup Name include special character !";
                    }
                    else if (_deviceConfig.TagGroups.ContainsKey(Name) && BuildMode)
                    {
                        result = "TagGroup Name is Exit! ";
                    }
                    errorMsgBuffer[0] = result;
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
                _deviceConfig = navigationContext.Parameters.GetValue<DeviceConfig>("DeviceConfig");
                _config = navigationContext.Parameters.GetValue<TagGroupConfig>("TagGroupConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    Name = _config.Name;
                }
                isFristIn = false;
            }
        }
        #endregion

    }
}
