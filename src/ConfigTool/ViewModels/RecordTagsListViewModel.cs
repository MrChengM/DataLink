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
    public class RecordTagsListViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private RecordItemConfig _recordItemConfig;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        #region Property
        private ObservableCollection<string> recordTagsList = new ObservableCollection<string>();

        public ObservableCollection<string> RecordTagsList
        {
            get { return recordTagsList; }
            set { SetProperty(ref recordTagsList, value, "RecordTagsList"); }
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
        public RecordTagsListViewModel(IRegionManager regionManager, IDialogService dialogService, IConfigDataServer configDataServer)
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
            var item = obj as string;
            _configDataServer.RemoveRecordTags(_recordItemConfig.Name,new List<string>{ item});
        }

        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.CurreNodeType == NodeType.RecordTag)
            {
                var config = e.ParentConfigNode as RecordItemConfig;
                if (_recordItemConfig == config)
                {
                    RecordTagsList = new ObservableCollection<string>(config.TagNames);
                }
            }
        }

        private void showProperty(object obj)
        {
            if (_recordItemConfig != null)
            {
                var tagNames = _recordItemConfig.TagNames;
                var dialogPara = new DialogParameters
            {
                { "RecordItem", _recordItemConfig.Name },
                { "TagNames",tagNames }
            };
                _dialogService.ShowDialog("RecordTagsManagerView", dialogPara, r =>
                {
                    //if (r.Result == ButtonResult.OK)
                    //{
                    //}
                }
                );
            }

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
            _recordItemConfig = navigationContext.Parameters.GetValue<RecordItemConfig>("RecordItemConfig");
            if (_recordItemConfig != null)
            {
                RecordTagsList =new ObservableCollection<string>( _recordItemConfig.TagNames);
            }
        }
    }
}
