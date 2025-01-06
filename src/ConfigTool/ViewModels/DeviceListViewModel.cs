using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using Prism.Services.Dialogs;
using ConfigTool.Models;
using System.Collections.ObjectModel;
using DataServer.Config;
using System.Windows.Input;
using ConfigTool.Service;

namespace ConfigTool.ViewModels
{
    public class DeviceListViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {

        private ChannelConfig _channelConfig;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        #region Property
        private ObservableCollection<DeviceListItem> deviceList = new ObservableCollection<DeviceListItem>();

        public ObservableCollection<DeviceListItem> DeviceList
        {
            get { return deviceList; }
            set { SetProperty(ref deviceList, value, "DeviceList"); }
        }

        public bool KeepAlive => false;
        #endregion

        #region ICommand
        private ICommand mouseDoubleClickCommand;

        public ICommand MouseDoubleClickCommand
        {
            get { return mouseDoubleClickCommand; }
            set { mouseDoubleClickCommand = value; }
        }
        private ICommand deleteItemCommand;

        public ICommand DeleteItemCommand
        {
            get { return deleteItemCommand; }
            set { deleteItemCommand = value; }
        }


        #endregion
        public DeviceListViewModel(IRegionManager regionManager, IDialogService dialogService, IConfigDataServer configDataServer)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _configDataServer = configDataServer;
            _configDataServer.ConfigChangeEvent += _configDataServer_ConfigChangeEvent;
            mouseDoubleClickCommand = new DelegateCommand<object>(showProperty);
            //deleteItemCommand = new DelegateCommand<object>(deleteItem);
        }

        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.CurreNodeType == NodeType.Device)
            {
                var config = e.ParentConfigNode as ChannelConfig;
                if (_channelConfig == config)
                {
                    switch (e.OperateMode)
                    {
                        case ConfigOperate.AddNode:
                            if (convertToInfor(_channelConfig.Devices[e.CurreNodeName], out DeviceListItem infor))
                            {
                                DeviceList.Add(infor);
                            }
                            break;
                        case ConfigOperate.RemoveNode:
                            var deviceInfor = DeviceList.First(r => r.Name == e.CurreNodeName);
                            DeviceList.Remove(deviceInfor);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //private void deleteItem(object obj)
        //{
        //    var channel = obj as ChannelInfor;
        //    ChannelList.Remove(channel);
        //    _client.Channels.Remove(channel.Name);
        //}

        private void showProperty(object obj)
        {
            var deviceInfor = obj as DeviceListItem;
            var deviceConfig = _channelConfig.Devices[deviceInfor.Name];
            DeviceViewDespatcher viewDespatcher = new DeviceViewDespatcher(_regionManager, _configDataServer, _channelConfig, deviceConfig);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToInfor(deviceConfig, out DeviceListItem infor))
                    {
                        deviceInfor.Name = infor.Name;
                        deviceInfor.Id = infor.Id;
                        deviceInfor.ConnectionTimeOut = infor.ConnectionTimeOut;
                        deviceInfor.RequestTimeOut = infor.RequestTimeOut;
                        deviceInfor.RetryTimes = infor.RetryTimes;
                        deviceInfor.ByteOrder = infor.ByteOrder;
                    }
                }
            }
            );
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _configDataServer.ConfigChangeEvent -= _configDataServer_ConfigChangeEvent;

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _channelConfig = navigationContext.Parameters.GetValue<ChannelConfig>("ChannelConfig");
            if (_channelConfig != null)
            {
                foreach (var config in _channelConfig.Devices)
                {
                    if (convertToInfor(config.Value, out DeviceListItem infor))
                    {
                        DeviceList.Add(infor);
                    }
                }
            }
        }

        private bool convertToInfor(DeviceConfig deviceConfig, out DeviceListItem channelInfor)
        {
            bool result;
            var infor = new DeviceListItem();

            if (deviceConfig != null)
            {
                infor.Name = deviceConfig.Name;
                infor.Id = deviceConfig.ID;
                infor.ConnectionTimeOut = deviceConfig.ConnectTimeOut;
                infor.RequestTimeOut = deviceConfig.RequestTimeOut;
                infor.RetryTimes = deviceConfig.RetryTimes;
                infor.ByteOrder = deviceConfig.ByteOrder.ToString();
                result = true;
            }
            else
            {
                result = false;

            }
            channelInfor = infor;
            return result;
        }
    }
}
