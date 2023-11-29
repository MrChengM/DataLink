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
    public class AlarmListViewModel : BindableBase,INavigationAware, IRegionMemberLifetime
    {

        private AlarmsConfig _alarmsConfig;

        private IRegionManager _regionManager;

        private IDialogService _dialogService;

        private IConfigDataServer _configDataServer;

        #region Property
        private ObservableCollection<AlarmListItem> alarmList = new ObservableCollection<AlarmListItem>();

        public ObservableCollection<AlarmListItem> AlarmList
        {
            get { return alarmList; }
            set { SetProperty(ref alarmList, value, "AlarmList"); }
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
        public AlarmListViewModel(IRegionManager regionManager,IDialogService dialogService,IConfigDataServer configDataServer)
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

            var item = obj as AlarmListItem;
            //TagList.Remove(tag);
            _configDataServer.Remove_AlarmItem(item.AlarmTag);
            //_tagGroupConfig.Tags.Remove(tag.Name);
        }

        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.CurreNodeType == NodeType.AlarmItem)
            {
                var config = e.ParentConfigNode as AlarmsConfig;
                if (_alarmsConfig==config)
                {
                    switch (e.OperateMode)
                    {
                        case ConfigOperate.AddNode:
                            if (convertToAlarmInfor(config.AlarmGroup[e.CurreNodeName], out AlarmListItem infor))
                            {
                                AlarmList.Add(infor);
                            }
                            break;
                        case ConfigOperate.RemoveNode:
                            var tagInfor = AlarmList.First(r => r.AlarmTag == e.CurreNodeName);
                            AlarmList.Remove(tagInfor);
                            break;
                        default:
                            break;
                    }
                }
              
            }
        }

        private void showProperty(object obj)
        {
            var  tagInfor = obj as AlarmListItem;
            var alarmItem =_alarmsConfig.AlarmGroup[tagInfor.AlarmTag];
            AlarmItemViewDespatcher viewDespatcher = new AlarmItemViewDespatcher(_regionManager, _configDataServer,alarmItem);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", viewDespatcher);
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    if (convertToAlarmInfor(alarmItem, out AlarmListItem infor))
                    {
                        tagInfor.AlarmTag = infor.AlarmTag;
                        tagInfor.AlarmType = infor.AlarmType;
                        tagInfor.ConditionName = infor.ConditionName;
                        tagInfor.AlarmDescription = infor.AlarmDescription;
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
            _alarmsConfig = navigationContext.Parameters.GetValue<AlarmsConfig>("AlarmsConfig");
            if (_alarmsConfig != null)
            {
                foreach (var config in _alarmsConfig.AlarmGroup)
                {
                    if (convertToAlarmInfor(config.Value, out AlarmListItem infor))
                    {
                        AlarmList.Add(infor);
                    }
                }
            }
        }

        private bool convertToAlarmInfor(AlarmItemConfig alarmItemConfig,out AlarmListItem alarmInfo)
        {
            bool result;
            var infor = new AlarmListItem();

            if (alarmItemConfig != null)
            {
                infor.AlarmTag = alarmItemConfig.AlarmTag;
                infor.ConditionName = alarmItemConfig.ConditionType.ToString() + alarmItemConfig.ConditionValue;
                infor.AlarmType = alarmItemConfig.AlarmType.ToString();
                infor.AlarmDescription = alarmItemConfig.AlarmDescription;
                result = true;
            }
            else
            {
                result = false;

            }
            alarmInfo = infor;
            return result;
        }


    }
}
