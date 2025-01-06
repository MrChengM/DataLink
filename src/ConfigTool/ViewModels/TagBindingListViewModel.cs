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
    public class TagBindingListViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        private ServerItemConfig _serverItemConfig;


        private ObservableCollection<TagBindingListItem> tagBindingList = new ObservableCollection<TagBindingListItem>();

        public ObservableCollection<TagBindingListItem> TagBindingList 
        {
            get { return tagBindingList; }
            set { SetProperty(ref tagBindingList, value, "TagBindingList"); }
        }

        public TagBindingListViewModel(IRegionManager regionManager, IDialogService dialogService, IConfigDataServer configDataServer)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _configDataServer = configDataServer;
            _configDataServer.ConfigChangeEvent += _configDataServer_ConfigChangeEvent;
            mouseDoubleClickCommand = new DelegateCommand<object>(showProperty);
            deleteItemCommand = new DelegateCommand<object>(deleteItem);
        }
        public bool KeepAlive => false;
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
        private void deleteItem(object obj)
        {
            var tagBinding = obj as TagBindingListItem;
            //TagList.Remove(tag);
            _configDataServer.RemoveTagBinding(_serverItemConfig.Name,tagBinding.Name);
        }

        private void showProperty(object obj)
        {
            var tagBindingInfor = obj as TagBindingListItem;
            var tagBindingConfig = _serverItemConfig.TagBindingList[tagBindingInfor.Name];
            TagBindingViewDespatcher viewDespatcher = new TagBindingViewDespatcher(_regionManager, _configDataServer, _serverItemConfig.Name,tagBindingConfig);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToTagBindingInfor(tagBindingConfig, out TagBindingListItem infor))
                    {
                        tagBindingInfor.Name = infor.Name;
                        tagBindingInfor.SourceTag = infor.SourceTag;

                    }
                }
            }
            );
        }

        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.CurreNodeType == NodeType.TagBinding)
            {
                var config = e.ParentConfigNode as ServerItemConfig;
                if (_serverItemConfig == config)
                {
                    switch (e.OperateMode)
                    {
                        case ConfigOperate.AddNode:
                            if (convertToTagBindingInfor(config.TagBindingList[e.CurreNodeName], out TagBindingListItem infor))
                            {
                                TagBindingList.Add(infor);
                            }
                            break;
                        case ConfigOperate.RemoveNode:
                            var tagInfor = TagBindingList.First(r => r.Name == e.CurreNodeName);
                            TagBindingList.Remove(tagInfor);
                            break;
                        default:
                            break;
                    }
                }

            }
        }
        private bool convertToTagBindingInfor(TagBindingConfig tagBindingConfig, out TagBindingListItem tagBindingInfor)
        {
            bool result;
            var infor = new TagBindingListItem();

            if (tagBindingConfig != null)
            {
                infor.Name = tagBindingConfig.DestTagName;
                infor.SourceTag = tagBindingConfig.SourceTagName;

                result = true;
            }
            else
            {
                result = false;

            }
            tagBindingInfor = infor;
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

            _serverItemConfig = navigationContext.Parameters.GetValue<ServerItemConfig>("ServerItemConfig");
            if (_serverItemConfig != null)
            {
                foreach (var config in _serverItemConfig.TagBindingList)
                {
                    if (convertToTagBindingInfor(config.Value, out TagBindingListItem infor))
                    {
                        TagBindingList.Add(infor);
                    }
                }
            }
        }
    }
}
