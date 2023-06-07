using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DataServer.Config;
using ConfigTool.Models;
using Prism.Regions;
using System.Windows.Input;
using Prism.Commands;
using Prism.Ioc;
using Prism.Events;
using DataServer;

namespace ConfigTool.ViewModels
{


    public class PropertyDialogViewModel : BindableBase, IDialogAware
    {

        //private TreeNode currentNode;
        //private TreeNode parentNode;

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _ea;
        private NodeType nodeType;

        private ChannelConfig _channelConfig;
        private DeviceConfig _deviceConfig;

        #region Property
        private string _title = "Property";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, "Title"); }
        }

        private ObservableCollection<PropertyOptionItem> optionItems;
        /// <summary>
        /// Property选项卡名称集合
        /// </summary>
        public ObservableCollection<PropertyOptionItem> OptionItems
        {
            get { return optionItems; }
            set { SetProperty(ref optionItems, value, "OptionItems"); }
        }
        //}
        //private ICommand optionSelectCommand;

        //public ICommand OptionSelectCommand
        //{
        //    get { return optionSelectCommand; }
        //    set { optionSelectCommand = value; }
        //}

        private ICommand closeDialogCommand;

        public event Action<IDialogResult> RequestClose;

        public ICommand CloseDialogCommand
        {
            get { return closeDialogCommand; }
            set { closeDialogCommand = value; }
        }

        #endregion

        public PropertyDialogViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _ea = eventAggregator;

            //optionSelectCommand = new DelegateCommand<string>(OptionSelect);
            closeDialogCommand = new DelegateCommand<string>(closeDialog);
        }
        #region ICommand
        void channelOptionSelect(string url)
        {
            var para = new NavigationParameters();
            para.Add("ChannelConfig", _channelConfig);
            _regionManager.RequestNavigate("PropertyRegion", url, para);
        }
        void deviceOptionSelect(string url)
        {
            var para = new NavigationParameters();
            para.Add("ChannelConfig", _channelConfig);
            para.Add("DeviceConfig", _deviceConfig);
            _regionManager.RequestNavigate("PropertyRegion", url, para);
        }
        void closeDialog(string parameter)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                result = ButtonResult.OK;
                _ea.GetEvent<ButtonConfrimEvent> ().Publish(result);
            }
            else if (parameter?.ToLower() == "cancel")
                result = ButtonResult.Cancel;
            RequestClose(new DialogResult(result, param));
            //_regionManager.Regions.Remove("PropertyRegion");
            _ea.GetEvent<ButtonConfrimEvent>().Clear();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

            nodeType = parameters.GetValue<NodeType>("NodeType");
            string channelName;
            string deviceName;
            string tagName;
            switch (nodeType)
            {
                case NodeType.Channel:
                    parameters.TryGetValue("ChannelName", out channelName);
                    initAsChannel(channelName);
                    break;
                case NodeType.Device:
                    parameters.TryGetValue("ChannelName", out channelName);
                    parameters.TryGetValue("DeviceName", out deviceName);
                    initAsDevice(channelName, deviceName);
                    break;
                case NodeType.Tags:
                    parameters.TryGetValue("ChannelName", out channelName);
                    parameters.TryGetValue("DeviceName", out deviceName);
                    parameters.TryGetValue("TagName", out tagName);
                    initAsTag(channelName, deviceName, tagName);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 初始化为Channel属性
        /// </summary>
        /// <param name="channelName"></param>
        private void initAsChannel(string channelName)
        {
            Title = "Property Connectivity.Channel";

            if (GlobalVar.ProjectConfig.Client.Channels.TryGetValue(channelName, out _channelConfig))
            {
                OptionItems = new ObservableCollection<PropertyOptionItem>()
            {
                new PropertyOptionItem
                {
                    Content="General",
                    Url="ChannelGeneralView",
                    OptionSelectCommand =new DelegateCommand<string>(channelOptionSelect)
                }
            };
                switch (_channelConfig.DriverInformation.CommType)
                {
                    case CommunicationType.Serialport:
                        OptionItems.Add(new PropertyOptionItem
                        {
                            Content = "Serial Commniucation",
                            Url = "ComPortConfigView",
                            OptionSelectCommand = new DelegateCommand<string>(channelOptionSelect)
                        }
                        );
                        break;
                    case CommunicationType.Ethernet:
                        OptionItems.Add(new PropertyOptionItem { 
                            Content = "Ethernet Commniucation",
                            Url= "EthernetConfigView",
                            OptionSelectCommand = new DelegateCommand<string>(channelOptionSelect)
                        });
                        break;
                    case CommunicationType.File:
                        break;
                    case CommunicationType.Memory:
                        break;
                    default:
                        break;
                }
                channelOptionSelect("ChannelGeneralView");
            }


        }
       /// <summary>
       /// 初始化为Device属性
       /// </summary>
       /// <param name="channelName"></param>
       /// <param name="deviceName"></param>
        private void initAsDevice(string channelName, string deviceName)
        {
            Title = "Property Connectivity.Channel.Device";
            if (GlobalVar.ProjectConfig.Client.Channels.TryGetValue(channelName, out _channelConfig))
            {
                if (_channelConfig.Devices.TryGetValue(deviceName, out _deviceConfig)) 
                {
                    OptionItems = new ObservableCollection<PropertyOptionItem>
                    {
                        new PropertyOptionItem
                        {
                            Content="General",
                            Url="DeviceGeneralView",
                            OptionSelectCommand = new DelegateCommand<string>(deviceOptionSelect)
                        },
                        new PropertyOptionItem
                        {
                            Content="Special Property",
                            Url="DeviceSpecialPropertyView",
                            OptionSelectCommand = new DelegateCommand<string>(deviceOptionSelect)
                        }
                    };
                    deviceOptionSelect("DeviceGeneralView");
                };
            }
        }
        /// <summary>
        /// 初始化为Tag属性
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagName"></param>
        private void initAsTag(string channelName, string deviceName, string tagName)
        {
        }
        #endregion


    }
}
