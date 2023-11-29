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
    public class ChannelListViewModel : BindableBase,INavigationAware, IRegionMemberLifetime
    {

        private ClientConfig _client;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        #region Property
        private ObservableCollection<ChannelListItem> channelList = new ObservableCollection<ChannelListItem>();

        public ObservableCollection<ChannelListItem> ChannelList
        {
            get { return channelList; }
            set { SetProperty(ref channelList, value, "ChannelList"); }
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
        public ChannelListViewModel(IRegionManager regionManager,IDialogService dialogService,IConfigDataServer configDataServer)
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
            if (e.CurreNodeType == NodeType.Channel)
            {
                var config = e.ParentConfigNode as ClientConfig;
                switch (e.OperateMode)
                {
                    case ConfigOperate.AddNode:
                        if (convertToChannelInfor(config.Channels[e.CurreNodeName], out ChannelListItem infor))
                        {
                            ChannelList.Add(infor);
                        }
                        break;
                    case ConfigOperate.RemoveNode:
                        var channelInfor = channelList.First(r => r.Name == e.CurreNodeName);
                        channelList.Remove(channelInfor);
                        break;
                    default:
                        break;
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
            var  channelInfor = obj as ChannelListItem;
            var channelConfig = _configDataServer.GetChannel(channelInfor.Name);
            ChannelViewDespatcher viewDespatcher = new ChannelViewDespatcher(_regionManager, _configDataServer, channelConfig);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToChannelInfor(channelConfig, out ChannelListItem infor))
                    {
                        channelInfor.Name = infor.Name;
                        channelInfor.DriverName = infor.DriverName;
                        channelInfor.Connection = infor.Connection;
                        channelInfor.InitLevel = infor.InitLevel;

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
            _client = navigationContext.Parameters.GetValue<ClientConfig>("ClientConfig");
            if (_client!=null)
            {
                foreach (var config in _client.Channels)
                {
                    if (convertToChannelInfor(config.Value, out  ChannelListItem infor))
                    {
                        ChannelList.Add(infor);
                    }
                }
            }
        }

        private bool convertToChannelInfor(ChannelConfig channelConfig,out ChannelListItem channelInfor)
        {
            bool result;
            var infor = new ChannelListItem();

            if (channelConfig != null)
            {
                infor.Name = channelConfig.Name;
                infor.DriverName = channelConfig.DriverInformation.Description;
                switch (channelConfig.DriverInformation.CommType)
                {
                    case DataServer.CommunicationType.Serialport:
                        infor.Connection = channelConfig.ComunicationSetUp.SerialportSet.ToString();
                        break;
                    case DataServer.CommunicationType.Ethernet:
                        infor.Connection = channelConfig.ComunicationSetUp.EthernetSet.ToString();
                        break;
                    case DataServer.CommunicationType.File:
                        //infor.Connection = channelConfig.ComunicationSetUp..ToString();
                        break;
                    case DataServer.CommunicationType.Memory:
                        infor.Connection = channelConfig.ComunicationSetUp.MemorySetUp.ToString();
                        break;
                    default:
                        break;
                }

                infor.InitLevel = channelConfig.InitLevel;
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
