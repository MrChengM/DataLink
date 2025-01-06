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
  public class RecordListViewModel: BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private RecordsConfig _recordsConfig;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        #region Property
        private ObservableCollection<RecordListItem> recordList = new ObservableCollection<RecordListItem>();

        public ObservableCollection<RecordListItem> RecordList
        {
            get { return recordList; }
            set { SetProperty(ref recordList, value, "RecordList"); }
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
        public RecordListViewModel(IRegionManager regionManager, IDialogService dialogService, IConfigDataServer configDataServer)
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

            var item = obj as RecordListItem;
            _configDataServer.RemoveRecordItem(item.Name);
        }

        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.CurreNodeType == NodeType.RecordItem)
            {
                var config = e.ParentConfigNode as RecordsConfig;
                if (_recordsConfig == config)
                {
                    switch (e.OperateMode)
                    {
                        case ConfigOperate.AddNode:
                            if (convertToRecordInfor(config.RecordGroup[e.CurreNodeName], out RecordListItem infor))
                            {
                                RecordList.Add(infor);
                            }
                            break;
                        case ConfigOperate.RemoveNode:
                            var tagInfor = RecordList.First(r => r.Name == e.CurreNodeName);
                            RecordList.Remove(tagInfor);
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        private void showProperty(object obj)
        {
            var recordInfor = obj as RecordListItem;
            var recordItem = _recordsConfig.RecordGroup[recordInfor.Name];
            var viewDespatcher = new RecordItemViewDespatcher(_regionManager, _configDataServer, recordItem);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToRecordInfor(recordItem, out RecordListItem infor))
                    {
                        recordInfor.Name = infor.Name;
                        recordInfor.Option = infor.Option;
                        recordInfor.Times = infor.Times;
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
            _recordsConfig = navigationContext.Parameters.GetValue<RecordsConfig>("RecordsConfig");
            if (_recordsConfig != null)
            {
                foreach (var config in _recordsConfig.RecordGroup)
                {
                    if (convertToRecordInfor(config.Value, out RecordListItem infor))
                    {
                        RecordList.Add(infor);
                    }
                }
            }
        }

        private bool convertToRecordInfor(RecordItemConfig recordItemConfig, out RecordListItem recordInfo)
        {
            bool result;
            var infor = new RecordListItem();

            if (recordItemConfig != null)
            {
                infor.Name = recordItemConfig.Name;
                infor.Option = recordItemConfig.Option.ToString();
                infor.Times = recordItemConfig.TimeSpan;
                result = true;
            }
            else
            {
                result = false;

            }
            recordInfo = infor;
            return result;
        }
    }
}
