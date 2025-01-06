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
    public class TagGroupListViewModel : BindableBase,INavigationAware, IRegionMemberLifetime
    {
        private ChannelConfig _channelConfig;

        private DeviceConfig _deviceConfig;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        #region Property
        private ObservableCollection<TagGroupListItem> tagGroupList = new ObservableCollection<TagGroupListItem>();

        public ObservableCollection<TagGroupListItem> TagGroupList
        {
            get { return tagGroupList; }
            set { SetProperty(ref tagGroupList, value, "TagGroupList"); }
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
        public TagGroupListViewModel(IRegionManager regionManager,IDialogService dialogService,IConfigDataServer configDataServer)
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
            if (e.CurreNodeType == NodeType.TagGroup)
            {
                var config = e.ParentConfigNode as DeviceConfig;
                if (_deviceConfig==config)
                {
                    switch (e.OperateMode)
                    {
                        case ConfigOperate.AddNode:
                            if (convertToTagGroupInfor(config.TagGroups[e.CurreNodeName], out TagGroupListItem infor))
                            {
                                TagGroupList.Add(infor);
                            }
                            break;
                        case ConfigOperate.RemoveNode:
                            var tagGroupInfor = TagGroupList.First(r => r.Name == e.CurreNodeName);
                            TagGroupList.Remove(tagGroupInfor);
                            break;
                        default:
                            break;
                    }
                }
              
            }
        }


        private void showProperty(object obj)
        {
            var  tagGroupInfor = obj as TagGroupListItem;
            var tagGroupConfig =_deviceConfig.TagGroups[tagGroupInfor.Name];
            TagGroupViewDespatcher viewDespatcher = new TagGroupViewDespatcher(_regionManager, _configDataServer,_channelConfig.Name,_deviceConfig.Name ,tagGroupConfig);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToTagGroupInfor(tagGroupConfig, out TagGroupListItem infor))
                    {
                        tagGroupInfor.Name = infor.Name;
                        tagGroupInfor.Scan = infor.Scan;
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
            _deviceConfig = navigationContext.Parameters.GetValue<DeviceConfig>("DeviceConfig");

            if (_deviceConfig != null)
            {
                foreach (var config in _deviceConfig.TagGroups)
                {
                    if (convertToTagGroupInfor(config.Value, out TagGroupListItem infor))
                    {
                        TagGroupList.Add(infor);
                    }
                }
            }
        }

        private bool convertToTagGroupInfor(TagGroupConfig tagGroupConfig,out TagGroupListItem tagGroupInfor)
        {
            bool result;
            var infor = new TagGroupListItem();

            if (tagGroupConfig != null)
            {
                infor.Name = tagGroupConfig.Name;
                //infor.Scan = tagGroupConfig.ScanTimes;
                result = true;
            }
            else
            {
                result = false;

            }
            tagGroupInfor = infor;
            return result;
        }

        //private bool convertToChannelConfig()
        //{

        //}
    }
}
