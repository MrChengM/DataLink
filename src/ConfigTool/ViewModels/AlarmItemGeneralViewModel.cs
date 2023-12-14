using ConfigTool.Models;
using ConfigTool.Service;
using DataServer;
using DataServer.Config;
using Prism.Commands;
using Prism.Events;
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
    class AlarmItemGeneralViewModel: BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private AlarmItemConfig _config;
        private IConfigDataServer _configDataServer;
        private IDialogService _dialogService;
        private string alarmTag;

        public string AlarmTag
        {
            get { return alarmTag; }
            set { SetProperty(ref alarmTag, value, "AlarmTag"); }
        }

        private string tagName;

        public string TagName
        {
            get { return tagName; }
            set { SetProperty(ref tagName, value, "TagName"); }
        }

        private string partName;

        public string PartName
        {
            get { return partName; }
            set { SetProperty(ref partName, value, setAlarmName, "PartName"); }
        }

        private string alNumber;

        public string ALNumber
        {
            get { return alNumber; }
            set { SetProperty(ref alNumber, value, setAlarmName, "ALNumber"); }
        }

        private string currentConditionType;

        public string CurrentConditionType
        {
            get { return currentConditionType; }
            set { SetProperty(ref currentConditionType, value, "CurrentConditionType"); }
        }

        private ObservableCollection<string> conditionTypes;

        public ObservableCollection<string> ConditionTypes
        {
            get { return conditionTypes; }
            set { SetProperty(ref conditionTypes, value, "ConditionTypes"); }
        }

        private int conditionValue;

        public int ConditionValue
        {
            get { return conditionValue; }
            set { SetProperty(ref conditionValue, value, "ConditionValue"); }
        }


        private string alarmGroup;

        public string AlarmGroup
        {
            get { return alarmGroup; }
            set { SetProperty(ref alarmGroup, value, "AlarmGroup"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }

        private void setAlarmName()
        {
            if (partName!=null)
            {
                AlarmTag = $"{TagName}.{PartName}.{ALNumber}";
            }
            else
            {
                AlarmTag = $"{TagName}.{ALNumber}";
            }
        }

        private ICommand openTagsDailogCommand;

        public ICommand OpenTagsDailogCommand
        {
            get { return openTagsDailogCommand; }
            set { openTagsDailogCommand = value; }
        }
        private bool isFristIn = true;
        public AlarmItemGeneralViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer,IDialogService dialogService)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);

            _configDataServer = configDataServer;
            _dialogService = dialogService;

            conditionTypes =new ObservableCollection<string>( Enum.GetNames(typeof(ConditionType)));
            currentConditionType = conditionTypes[0];
            openTagsDailogCommand = new DelegateCommand(openTagsDailog);

        }

        private void openTagsDailog()
        {

            var dialogPara = new DialogParameters();
            var allTags = _configDataServer.GetAllTags();
            dialogPara.Add("AllTags", allTags);
            dialogPara.Add("SourceTag", TagName);
            _dialogService.ShowDialog("SearchTagDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var tagItem = r.Parameters.GetValue<List<TagListItem>>("SelectTags");
                    if (tagItem!=null)
                    {
                        TagName = tagItem[0].Name;

                    }
                }
            }
            );

        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                if (BuildMode)
                {
                    _config.AlarmTag = AlarmTag;
                    _config.TagName = TagName;
                    _config.PartName = PartName;
                    _config.ALNumber = ALNumber;
                    _config.AlarmGroup = AlarmGroup;
                }
                if (Enum.TryParse(CurrentConditionType, out ConditionType type))
                {
                    _config.ConditionType = type;
                }
                _config.ConditionValue = ConditionValue;
            }
        }
        #region INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (isFristIn)
            {
                _config = navigationContext.Parameters.GetValue<AlarmItemConfig>("AlarmItemConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    AlarmTag = _config.AlarmTag;
                    TagName = _config.TagName;
                    PartName =_config. PartName;
                    ALNumber =_config. ALNumber;
                    AlarmGroup = _config.AlarmGroup;
                    CurrentConditionType = _config.ConditionType.ToString();
                    ConditionValue = _config.ConditionValue;
                }
                isFristIn = false;
            }

        }
        #endregion
    }
}
