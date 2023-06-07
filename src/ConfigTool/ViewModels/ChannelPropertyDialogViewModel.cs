using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using DataServer.Config;
using ConfigTool.Models;
using System.Collections.ObjectModel;
using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ViewModels
{
   public class ChannelPropertyDialogViewModel : PropertyDialogViewModel, IDialogAware
    {
        IRegionManager _regionManager;
        IEventAggregator _eventAggregator;

        private ChannnelConfig channnelConfig;
        public ChannelPropertyDialogViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager, eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        internal override void closeDialog(string parameter)
        {
            base.closeDialog(parameter);
            RequestClose?.Invoke(new DialogResult());
        }

        internal override void OptionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url);
        }
        #region IDialogAware

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            string channelName;

            parameters.TryGetValue("ChannelName", out channelName);
            OptionItems = new ObservableCollection<PropertyOptionItem>()
            {
                new PropertyOptionItem{Content="General",Url="ChannelGeneralOptionView"},
            };
            if (GlobalVar.ProjectConfig.Client.Channnels.TryGetValue(channelName, out channnelConfig))
            {
                switch (channnelConfig.DriverInformation.CommType)
                {
                    case CommunicationType.Serialport:
                        OptionItems.Add(new PropertyOptionItem { Content = "Serial Commniucation", Url = "ComPortConfigView" });
                        break;
                    case CommunicationType.Ethernet:
                        OptionItems.Add(new PropertyOptionItem { Content = "Ethernet Commniucation", Url = "EthernetConfigView" });
                        break;
                    case CommunicationType.File:
                        OptionItems.Add(new PropertyOptionItem { Content = "File Commniucation", Url = "FileCommConfigView" });
                        break;
                    case CommunicationType.Memory:
                        OptionItems.Add(new PropertyOptionItem { Content = "Memory Commniucation", Url = "MemoryCommConfigView" });
                        break;
                    default:
                        break;
                }

            } 
       }
        #endregion

    }
}
