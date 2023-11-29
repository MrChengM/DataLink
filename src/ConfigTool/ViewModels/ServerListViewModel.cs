using ConfigTool.Models;
using ConfigTool.Service;
using DataServer.Config;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConfigTool.ViewModels
{
    public class ServerListViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        private ServersConfig _serversConfig;
        #region Property
        private ObservableCollection<ServerListItem> serverList = new ObservableCollection<ServerListItem>();

        public ObservableCollection<ServerListItem> ServerList
        {
            get { return serverList; }
            set { SetProperty(ref serverList, value, "ServerList"); }
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

        public ServerListViewModel(IRegionManager regionManager,IDialogService dialogService,IConfigDataServer configDataServer )
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _configDataServer = configDataServer;
            _configDataServer.ConfigChangeEvent += _configDataServer_ConfigChangeEvent;
            mouseDoubleClickCommand = new DelegateCommand<object>(showProperty);
            deleteItemCommand = new DelegateCommand<object>(deleteItem);
        }

        private void deleteItem(object obj)
        {
            var server = obj as ServerListItem;
            //TagList.Remove(tag);
            _configDataServer.RemoveServerItem(server.Name);
        }

        private void showProperty(object obj)
        {
            var serverInfor = obj as ServerListItem;
            var serverItemConfig = _serversConfig.Items[serverInfor.Name];
            ServerItemViewDespatcher viewDespatcher = new ServerItemViewDespatcher(_regionManager,_configDataServer,serverItemConfig);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToServerInfor(serverItemConfig, out ServerListItem infor))
                    {
                        serverInfor.Name = infor.Name;
                        serverInfor.Address = infor.Address;
                        serverInfor.Option = infor.Option;
                        serverInfor.PhySetting = infor.PhySetting;
                    }
                }
            }
            );
        }

        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.CurreNodeType == NodeType.ServerItem)
            {
                var config = e.ParentConfigNode as ServersConfig;
                if (_serversConfig == config)
                {
                    switch (e.OperateMode)
                    {
                        case ConfigOperate.AddNode:
                            if (convertToServerInfor(config.Items[e.CurreNodeName], out ServerListItem infor))
                            {
                                ServerList.Add(infor);
                            }
                            break;
                        case ConfigOperate.RemoveNode:
                            var tagInfor = ServerList.First(r => r.Name == e.CurreNodeName);
                            ServerList.Remove(tagInfor);
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        private bool convertToServerInfor(ServerItemConfig serverItemConfig, out ServerListItem serverInfor)
        {
            bool result;
            var infor = new ServerListItem();

            if (serverItemConfig != null)
            {
                infor.Name = serverItemConfig.Name;
                infor.Address = serverItemConfig.Address;
                infor.Option = serverItemConfig.Option.ToString();
                switch (serverItemConfig.Option)
                {
                    case DataServer.ServerOption.ModbusTCP:
                        infor.PhySetting = serverItemConfig.ComunicationSetUp.EthernetSet.ToString();

                        break;
                    case DataServer.ServerOption.ModbusRTU:
                        infor.PhySetting = serverItemConfig.ComunicationSetUp.SerialportSet.ToString();
                        break;
                    case DataServer.ServerOption.Freedom:
                        infor.PhySetting = serverItemConfig.ComunicationSetUp.EthernetSet.ToString();
                        break;
                    default:
                        break;
                }
                result = true;
            }
            else
            {
                result = false;

            }
            serverInfor = infor;
            return result;
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

            _serversConfig = navigationContext.Parameters.GetValue<ServersConfig>("ServersConfig");
            if (_serversConfig != null)
            {
                foreach (var config in _serversConfig.Items)
                {
                    if (convertToServerInfor(config.Value, out ServerListItem infor))
                    {
                        ServerList.Add(infor);
                    }
                }
            }
        }
    }
}
