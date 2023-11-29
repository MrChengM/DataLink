using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Ioc;
using Prism.Events;
using Prism.Regions;
using DataServer.Config;
using DataServer;
using Prism.Services.Dialogs;
using ConfigTool.Models;
using System.Collections.ObjectModel;

namespace ConfigTool.ViewModels
{
    class DeviceSpecialPropertyViewModel : BindableBase, INavigationAware
    {
        private ChannelConfig _channelConfig;
        private DeviceConfig _deviceConfig;
        private IEventAggregator _ea;

        private ObservableCollection<DeviceSpecailPropertyMVVM> specialProperties;
        private bool isFristIn=true;

        public ObservableCollection<DeviceSpecailPropertyMVVM> SpecialProperties
        {
            get { return specialProperties; }
            set { SetProperty(ref specialProperties, value, "SpecialProperties"); }
        }

        public bool IsBuild { get; private set; }

        public DeviceSpecialPropertyViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            specialProperties = new ObservableCollection<DeviceSpecailPropertyMVVM>();

            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(r =>
            {
                if (r == ButtonResult.OK)
                {
                    _deviceConfig.SpecialProperties = new List<DeviceSpecialProperty>();
                    foreach (var sp in SpecialProperties)
                    {

                        _deviceConfig.SpecialProperties.Add(sp.Convert());
                    }
                }
            }
                );
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
                _channelConfig = navigationContext.Parameters.GetValue<ChannelConfig>("ChannelConfig");
                _deviceConfig = navigationContext.Parameters.GetValue<DeviceConfig>("DeviceConfig");
                IsBuild= navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!IsBuild )
                {
                    SpecialProperties = new ObservableCollection<DeviceSpecailPropertyMVVM>();
                    foreach (var sp in _deviceConfig.SpecialProperties)
                    {
                        SpecialProperties.Add(DeviceSpecailPropertyMVVM.Convert(sp));
                    }
                }
                else
                {
                    var DevicePropertyDes = _channelConfig.DriverInformation.DevicePropertyInfos;
                    SpecialProperties = new ObservableCollection<DeviceSpecailPropertyMVVM>();
                    foreach (var item in DevicePropertyDes)
                    {
                        SpecialProperties.Add(new DeviceSpecailPropertyMVVM { PropertyName = item.Name, PropertyValue = "0" });
                    }
                }
                isFristIn = false;
            }
          
        }
    }
}
