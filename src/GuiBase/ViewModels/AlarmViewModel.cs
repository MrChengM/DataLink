using Prism.Mvvm;
using System;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;
using System.Collections.ObjectModel;
using DataServer.Alarm;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace GuiBase.ViewModels
{
    public class AlarmViewModel : BindableBase, IDialogAware
    {

        private IAlarmService _alarmService;
        private ILocalizationService _localizationService;

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }

        private AlarmCaptions columns;

        public AlarmCaptions Columns
        {
            get { return columns; }
            set { SetProperty(ref columns, value, "Columns"); }
        }
        private string filterCondition;

        public string FilterCondition
        {
            get { return filterCondition; }
            set { SetProperty(ref filterCondition, value, "FilterCondition"); }
        }


        public event Action<IDialogResult> RequestClose;

        public AlarmFilterCondition Filter { get; set; }

        private bool topDrawerEnable;

        public bool TopDrawerEnable
        {
            get { return topDrawerEnable; }
            set { SetProperty(ref topDrawerEnable, value, "TopDrawerEnable"); }
        }
      
       private ObservableCollection<AlarmWrapper> alarms;

        public ObservableCollection<AlarmWrapper> Alarms
        {
            get { return alarms; }
            set { SetProperty(ref alarms, value, "Alarms"); }
        }

        private int counts;

        public int Counts
        {
            get { return counts; }
            set { SetProperty(ref counts, value, "Counts"); }
        }
        private int totalCounts;

        public int TotalCounts
        {
            get { return totalCounts; }
            set { SetProperty(ref totalCounts, value, "TotalCounts"); }
        }

        public DelegateCommand<string> TopDrawerOperationCommand { get; set; }
        public DelegateCommand DrawerCloseCommand { get; set; }

        public DelegateCommand ConfirmAllCommand { get; set; }

        public AlarmViewModel(IAlarmService alarmService,ILocalizationService localizationService)
        {
            _alarmService = alarmService;
            _alarmService.AlarmRefreshEvent += _alarmService_AlarmStatusChangeEvent;
            _alarmService.ConnectStatusChangeEvent += onConnectStatusChanged;
            Filter = new AlarmFilterCondition();
            TopDrawerOperationCommand = new DelegateCommand<string>(topDrawerOperation);
            ConfirmAllCommand = new DelegateCommand(confirmAll);
            DrawerCloseCommand = new DelegateCommand(drawerClose);

            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Columns = new AlarmCaptions(_localizationService);

            translate();
            filterAllCondition();
            //Alarms = alarmService.AllEnableAlarms;
           
        }

        private void onConnectStatusChanged(bool isConnected)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (isConnected)
                {
                    filterAllCondition();
                }
                else
                {
                    Alarms.Clear();
                }
            });
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.AlarmId);
            FilterCondition = _localizationService.Translate(TranslateCommonId.FilterConditionId);
            Columns.GetContent();
        }
        private void drawerClose()
        {
            filterAllCondition();
        }

        private void _alarmService_AlarmStatusChangeEvent(AlarmWrapper newAlarm, AlarmRefresh status)
        {

            App.Current.Dispatcher.Invoke(() =>
            {
                if (matchFilterCondition(newAlarm))
                {
                    if (status == AlarmRefresh.Add)
                    {
                        Alarms.Add(newAlarm);
                    }
                    else
                    {
                        var oldaAlarm = Alarms.ToList().Find(s => s.AlarmName == newAlarm.AlarmName);
                        if (status == AlarmRefresh.Updata)
                        {
                            if (oldaAlarm != null)
                            {
                                oldaAlarm.CopyFrom(newAlarm);
                            }
                            else
                            {
                                Alarms.Add(newAlarm);
                            }
                        }

                        if (status == AlarmRefresh.Remove)
                        {
                            Alarms.Remove(oldaAlarm);
                        }
                    }
                    
                }
                updataCounts();
            });
         
        }
        private bool matchFilterCondition(AlarmWrapper alarmWrapper)
        {
            bool matchALFilter, matchAGFilter, matchL1VFilter, matchL2VFilter;
            if (Filter.AlarmLevel != "All" && alarmWrapper.AlarmLevel.ToString() != Filter.AlarmLevel)
            {
                matchALFilter = false;
            }
            else
            {

                matchALFilter = true;
            }

            if (Filter.AlarmGroup != null && Filter.AlarmGroup != "" && Filter.AlarmGroup != "*" && alarmWrapper.AlarmGroup!= Filter.AlarmGroup)
            {
                 matchAGFilter = false;

            }
            else
            {
                matchAGFilter = true;
            }
            if (Filter.L1View != null && Filter.L1View != "" && Filter.L1View != "*" && alarmWrapper.L1View != Filter.L1View)
            {
                matchL1VFilter = false;

            }
            else
            {
                matchL1VFilter = true;
            }
            if (Filter.L2View != null && Filter.L2View != "" && Filter.L2View != "*" && alarmWrapper.L2View != Filter.L2View)
            {
                matchL2VFilter = false;

            }
            else
            {
                matchL2VFilter = true;
            }
            return matchALFilter && matchAGFilter && matchL1VFilter && matchL2VFilter;
        }
        private void filterAllCondition()
        {
            var alarm1 = filterByAlarmLevel(_alarmService.AllExitAlarms);
            var alarm2= filterByAlarmGroup(alarm1);
            var alarm3 = filterByL1View(alarm2);
            var alarm4 = filterByL2View(alarm3);

            Counts = alarm4.Count();
            TotalCounts = _alarmService.AllExitAlarms.Count();
            Alarms = new ObservableCollection<AlarmWrapper>(alarm4);
            updataCounts();
        }

        private void updataCounts()
        {
            Counts = Alarms.Count();
            TotalCounts = _alarmService.AllExitAlarms.Count();

        }

        private IEnumerable<AlarmWrapper> filterByAlarmLevel(IEnumerable<AlarmWrapper> alarms)
        {
            if (Filter.AlarmLevel != "All")
            {
                var result = from s in alarms
                             where s.AlarmLevel.ToString() == Filter.AlarmLevel
                             select s;
                             
                return result;
            }
            else
            {
                return alarms;
            }
        }
        private IEnumerable<AlarmWrapper> filterByAlarmGroup(IEnumerable<AlarmWrapper> alarms)
        {
            if (Filter.AlarmGroup != null&& Filter.AlarmGroup != ""&& Filter.AlarmGroup != "*")
            {
                var result = from s in alarms
                             where s.AlarmGroup == Filter.AlarmGroup
                             select s;

                return result;
            }
            else
            {
                return alarms;
            }
        }
        private IEnumerable<AlarmWrapper> filterByL1View(IEnumerable<AlarmWrapper> alarms)
        {
            if (Filter.L1View != null && Filter.L1View != "" && Filter.L1View != "*")
            {
                var result = from s in alarms
                             where s.L1View == Filter.L1View
                             select s;

                return result;
            }
            else
            {
                return alarms;
            }
        }
        private IEnumerable<AlarmWrapper> filterByL2View(IEnumerable<AlarmWrapper> alarms)
        {
            if (Filter.L2View != null && Filter.L2View != "" && Filter.L2View != "*")
            {
                var result = from s in alarms
                             where s.L2View == Filter.L2View
                             select s;

                return result;
            }
            else
            {
                return alarms;
            }
        }
        private void confirmAll()
        {
            foreach (var alarm in Alarms)
            {
                alarm.Confrim();
            }
        }

        private void topDrawerOperation(string topDrawerParam)
        {
            if (topDrawerParam == "Open")
            {
                TopDrawerEnable = true;
            }
            else if (topDrawerParam == "Close")
            {
                TopDrawerEnable = false;
            }
        }
        public bool CanCloseDialog()
        {
            return  true;
        }

        public void OnDialogClosed()
        {
            Clear();

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
        public void Clear()
        {
            Alarms.Clear();
            _alarmService.AlarmRefreshEvent -= _alarmService_AlarmStatusChangeEvent;
            _alarmService.ConnectStatusChangeEvent -= onConnectStatusChanged;
            _localizationService.LanguageChanged -= onLanguageChanged;
        }
    }
}
