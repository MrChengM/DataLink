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
    public class TagGeneralViewModel:BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private TagConfig _config;
        private IConfigDataServer _configDataServer;

        private string name = "Tag1";

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

        private string address;

        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value, "Address"); }
        }

        private int length;

        public int Length
        {
            get { return length; }
            set { SetProperty(ref length, value, "Length"); }
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

        private List<string> operateWays;

        public List<string> OperateWays
        {
            get { return operateWays; }
            set { SetProperty(ref operateWays, value, "OperateWays"); }
        }

        private string currentOperateWay;

        public string CurrentOperateWay
        {
            get { return currentOperateWay; }
            set { SetProperty(ref currentOperateWay, value, "CurrentOperateWay"); }
        }

        private bool isFristIn = true;
        public TagGeneralViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);
            _configDataServer = configDataServer;

            dataTypes = new List<string>();
            foreach (var type in Enum.GetNames(typeof(DataType)))
            {
                dataTypes.Add(type);
            }
            CurrentDataType = dataTypes[0];

            operateWays = new List<string>();
            foreach (var way in Enum.GetNames(typeof(OperateWay)))
            {
                operateWays.Add(way);
            }
            CurrentOperateWay = operateWays[0];
        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                _config.Name = Name;
                _config.Address = Address;
                _config.Length = Length;
                if (Enum.TryParse(CurrentDataType, out DataType type))
                {
                    _config.DataType = type;
                }
                if (Enum.TryParse(CurrentOperateWay, out OperateWay  way))
                {
                    _config.Operate = way;
                }
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
                    Name = _config.Name;
                    Address = _config.Address;
                    Length = _config.Length;
                    CurrentDataType = _config.DataType.ToString();
                    CurrentOperateWay = _config.Operate.ToString();
                }
                isFristIn = false;
            }
        }
        #endregion
    }
}
