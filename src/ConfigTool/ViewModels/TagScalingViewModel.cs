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
using DataServer;

namespace ConfigTool.ViewModels
{
    public class TagScalingViewModel:BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private TagConfig _config;
        private IConfigDataServer _configDataServer;

        private string currentScalingType;

        public string CurrentScalingType
        {
            get { return currentScalingType; }
            set { SetProperty(ref currentScalingType, value, "CurrentScalingType"); }
        }

        private List<string> scalingTypes;

        public List<string> ScalingTypes
        {
            get { return scalingTypes; }
            set { SetProperty(ref scalingTypes, value, "ScalingTypes"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
        private string currentDataType;

        public string CurrentDataType
        {
            get { return currentDataType; }
            set { SetProperty(ref currentDataType, value, "CurrentDataType"); }
        }

        private List<string> dataTypes;

        public List<string> DataTypes
        {
            get { return dataTypes; }
            set { SetProperty(ref dataTypes, value, "DataTypes"); }
        }

        private int rawLow;

        public int RawLow
        {
            get { return rawLow; }
            set { SetProperty(ref rawLow, value, "RawLow"); }
        }

        private int rawHigh=1000;

        public int RawHigh
        {
            get { return rawHigh; }
            set { SetProperty(ref rawHigh, value, "RawHigh"); }
        }

        private int scaledLow;

        public int ScaledLow
        {
            get { return scaledLow; }
            set { SetProperty(ref scaledLow, value, "ScaledLow"); }
        }

        private int scaledHigh=1000;

        public int ScaledHigh
        {
            get { return scaledHigh; }
            set { SetProperty(ref scaledHigh, value, "ScaledHigh"); }
        }


        private bool isFristIn = true;
        public TagScalingViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);
            _configDataServer = configDataServer;

            scalingTypes = new List<string>();
            foreach (var type in Enum.GetNames(typeof(ScaleType)))
            {
                scalingTypes.Add(type);
            }
            CurrentScalingType = scalingTypes[0];

            dataTypes = new List<string>();
            foreach (var type in Enum.GetNames(typeof(DataType)))
            {
                dataTypes.Add(type);
            }
            CurrentDataType = dataTypes[0];
        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                if (Enum.TryParse(CurrentScalingType, out ScaleType scaleType))
                {
                    _config.Scaling.ScaleType = scaleType;

                }
                if (Enum.TryParse(CurrentDataType, out DataType dataType))
                {
                    _config.Scaling.DataType = dataType;
                }
                _config.Scaling.RawLow = RawLow;
                _config.Scaling.RawHigh = RawHigh;
                _config.Scaling.ScaledLow = ScaledLow;
                _config.Scaling.ScaledHigh = ScaledHigh;

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
                _config = navigationContext.Parameters.GetValue<TagConfig>("TagConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    CurrentScalingType = _config.Scaling.ScaleType.ToString();
                    CurrentDataType = _config.Scaling.DataType.ToString();
                    RawLow = _config.Scaling.RawLow;
                    RawHigh = _config.Scaling.RawHigh;
                    ScaledLow = _config.Scaling.ScaledLow;
                    ScaledHigh = _config.Scaling.ScaledHigh;
                }
                isFristIn = false;
            }
        }
        #endregion
    }
}
