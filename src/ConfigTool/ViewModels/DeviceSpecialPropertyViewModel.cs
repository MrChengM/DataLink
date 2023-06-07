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
        private DeviceConfig _deviceConfig;
        private IEventAggregator _ea;

        private ObservableCollection<DeviceSpecailPropertyMVVM> specialProperties;

        public ObservableCollection<DeviceSpecailPropertyMVVM> SpecialProperties
        {
            get { return specialProperties; }
            set { SetProperty(ref specialProperties, value, "SpecialProperties"); }
        }


        public DeviceSpecialPropertyViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            specialProperties = new ObservableCollection<DeviceSpecailPropertyMVVM>();

            _ea.GetEvent<PubSubEvent<ButtonResult>>().Subscribe(r =>
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
            _deviceConfig = navigationContext.Parameters.GetValue<DeviceConfig>("DeviceConfig");
            
            if (_deviceConfig != null)
            {
                SpecialProperties = new ObservableCollection<DeviceSpecailPropertyMVVM>();
                foreach (var sp in _deviceConfig.SpecialProperties)
                {
                    SpecialProperties.Add(DeviceSpecailPropertyMVVM.Convert(sp));
                }
            }
        }
    }
}
