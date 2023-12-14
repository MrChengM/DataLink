using ConfigTool.Models;
using ConfigTool.Service;
using DataServer.Config;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using DataServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ViewModels
{
    public class RecordGeneralViewModel: BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private RecordItemConfig _config;
        private IConfigDataServer _configDataServer;

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string currentOption;

        public string CurrentOption
        {
            get { return currentOption; }
            set { SetProperty(ref currentOption, value, "CurrentOption"); }
        }

        private List<string> options;

        public List<string> Options
        {
            get { return options; }
            set { options = value; }
        }

        private int times;

        public int Times
        {
            get { return times; }
            set { SetProperty(ref times, value, "Times"); }
        }

        private bool buildMode;
        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
   

        private bool isFristIn = true;
        public RecordGeneralViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);
            _configDataServer = configDataServer;

            
            options = new List<string>();
            foreach (var type in Enum.GetNames(typeof(RecordWay)))
            {
                options.Add(type);
            }
            CurrentOption = options[0];
        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                _config.Name = Name;
                if (Enum.TryParse(CurrentOption, out RecordWay way))
                {
                    _config.Option = way;
                }
                _config.TimeSpan = Times;
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
                _config = navigationContext.Parameters.GetValue<RecordItemConfig>("RecordItemConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    Name = _config.Name;
                    CurrentOption = _config.Option.ToString();
                    Times = _config.TimeSpan;
                }
                isFristIn = false;
            }
        }
        #endregion
    }
}
