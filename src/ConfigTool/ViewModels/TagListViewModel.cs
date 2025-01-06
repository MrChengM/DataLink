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
    public class TagListViewModel : BindableBase,INavigationAware, IRegionMemberLifetime
    {
        private ChannelConfig _channelConfig;

        private DeviceConfig _deviceConfig;
        private TagGroupConfig _tagGroupConfig;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        #region Property
        private ObservableCollection<TagListItem> tagList = new ObservableCollection<TagListItem>();

        public ObservableCollection<TagListItem> TagList
        {
            get { return tagList; }
            set { SetProperty(ref tagList, value, "TagList"); }
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
        public TagListViewModel(IRegionManager regionManager,IDialogService dialogService,IConfigDataServer configDataServer)
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

            var tag = obj as TagListItem;
            //TagList.Remove(tag);
            _configDataServer.RemoveTag(_channelConfig.Name, _deviceConfig.Name, _tagGroupConfig.Name, tag.Name);
            //_tagGroupConfig.Tags.Remove(tag.Name);
        }

        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.CurreNodeType == NodeType.Tag)
            {
                var config = e.ParentConfigNode as TagGroupConfig;
                if (_tagGroupConfig==config)
                {
                    switch (e.OperateMode)
                    {
                        case ConfigOperate.AddNode:
                            if (convertToTagInfor(config.Tags[e.CurreNodeName], out TagListItem infor))
                            {
                                TagList.Add(infor);
                            }
                            break;
                        case ConfigOperate.RemoveNode:
                            var tagInfor = TagList.First(r => r.Name == e.CurreNodeName);
                            TagList.Remove(tagInfor);
                            break;
                        default:
                            break;
                    }
                }
              
            }
        }

        private void showProperty(object obj)
        {
            var  tagInfor = obj as TagListItem;
            var tagConfig =_tagGroupConfig.Tags[tagInfor.Name];
            TagViewDespatcher viewDespatcher = new TagViewDespatcher(_regionManager, _configDataServer,_channelConfig.Name,_deviceConfig.Name ,_tagGroupConfig.Name,tagConfig);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToTagInfor(tagConfig, out TagListItem infor))
                    {
                        tagInfor.Name = infor.Name;
                        tagInfor.Address = infor.Address;
                        tagInfor.Length = infor.Length;
                        tagInfor.DataType = infor.DataType;
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
            _tagGroupConfig = navigationContext.Parameters.GetValue<TagGroupConfig>("TagGroupConfig");
            if (_tagGroupConfig != null)
            {
                foreach (var config in _tagGroupConfig.Tags)
                {
                    if (convertToTagInfor(config.Value, out TagListItem infor))
                    {
                        TagList.Add(infor);
                    }
                }
            }
        }

        private bool convertToTagInfor(TagConfig tagConfig,out TagListItem tagInfor)
        {
            bool result;
            var infor = new TagListItem();

            if (tagConfig != null)
            {
                infor.Name = tagConfig.Name;
                infor.Address = tagConfig.Address;
                infor.Length = tagConfig.Length;
                infor.DataType = tagConfig.DataType.ToString();
                result = true;
            }
            else
            {
                result = false;

            }
            tagInfor = infor;
            return result;
        }


    }
}
