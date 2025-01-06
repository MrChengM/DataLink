using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Services;
using GuiBase.Models;
using GuiBase.Common;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DataServer.Alarm;

namespace GuiBase.ViewModels
{
    public class L3BaseViewModel : BindableBase, IDialogAware
    {
        private IRegionManager _regionManager;
        private IAlarmService _alarmService;
        private IHistoryAlarmService _historyAlarmService;
        private ILocalizationService _localizationService;

        private GSignalSet _signals;
        private List<IGCommand> _commands;
        private string _elementView;
        private string _elementName;

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }
        private string tableMain;

        public string TableMain
        {
            get { return tableMain; }
            set { SetProperty(ref tableMain, value, "TableMain"); }
        }
        private string tableHistoryAlarm;

        public string TableHistoryAlarm
        {
            get { return tableHistoryAlarm; }
            set { SetProperty(ref tableHistoryAlarm, value, "TableHistoryAlarm"); }
        }

        public event Action<IDialogResult> RequestClose;
        public AlarmFilterCondition Filter { get; set; }= new AlarmFilterCondition();
        private ObservableCollection<AlarmWrapper> alarms;
        public ObservableCollection<AlarmWrapper> Alarms
        {
            get { return alarms; }
            set { SetProperty(ref alarms, value, "Alarms"); }
        }
        private AlarmCaptions columns;

        public AlarmCaptions Columns
        {
            get { return columns; }
            set { SetProperty(ref columns, value, "Columns"); }
        }
        private string buttonOK;

        public string ButtonOK
        {
            get { return buttonOK; }
            set { SetProperty(ref buttonOK, value, "ButtonOK"); }
        }

        private string buttonCancel;

        public string ButtonCancel
        {
            get { return buttonCancel; }
            set { SetProperty(ref buttonCancel, value, "ButtonCancel"); }
        }
        private string buttonQuery;

        public string ButtonQuery
        {
            get { return buttonQuery; }
            set { SetProperty(ref buttonQuery, value, "ButtonQuery"); }
        }
        public HistoryAlarmSelectConditionWrapper SelectCondition { get; set; } = new HistoryAlarmSelectConditionWrapper();

        private ObservableCollection<HistoryAlarmWrapper> historyAlarms = new ObservableCollection<HistoryAlarmWrapper>();

        public ObservableCollection<HistoryAlarmWrapper> HistoryAlarms
        {
            get { return historyAlarms; }
            set { SetProperty(ref historyAlarms, value, "HistoryAlarms"); }
        }

        public ICommand QueryDataCommand { get; set; }


        public L3BaseViewModel(IRegionManager regionManager, IAlarmService alarmService, IHistoryAlarmService historyAlarmService,ILocalizationService localizationServices)
        {
            _regionManager = regionManager;
            _alarmService = alarmService;
            _historyAlarmService = historyAlarmService;
            _localizationService = localizationServices;
            _localizationService.LanguageChanged += onLanguageChanged;
            Columns = new AlarmCaptions(localizationServices);
            QueryDataCommand = new DelegateCommand(queryData);
            translate();

        }
        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.L3ViewId);
            TableMain = _localizationService.Translate(TranslateCommonId.MainViewId);
            TableHistoryAlarm = _localizationService.Translate(TranslateCommonId.HistoryAlarmId);
            ButtonOK = _localizationService.Translate(TranslateCommonId.OKId);
            ButtonCancel = _localizationService.Translate(TranslateCommonId.CancelId);
            ButtonQuery = _localizationService.Translate(TranslateCommonId.QueryId);

            foreach (var hisAlarm in HistoryAlarms)
            {
                hisAlarm.LocalizationDescrible= _localizationService.Translate(hisAlarm.AlarmNumber);
            }
            Columns.GetContent();
        }
        private void queryData()
        {
            HistoryAlarms.Clear();
            foreach (var signal in _signals)
            {
                SelectCondition.AlarmName = signal.SignalName;
                var hisAlarms = _historyAlarmService.Select(SelectCondition);
                HistoryAlarms.AddRange(hisAlarms);
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            Clear();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _elementView = parameters.GetValue<string>("L3ViewName");
            _elementName = parameters.GetValue<string>("ElementName");
            _signals = parameters.GetValue<GSignalSet>("Signals");
            _commands = parameters.GetValue<List<IGCommand>>("Commands");
            var param = new NavigationParameters();
            param.Add("ElementName", _elementName);
            param.Add("Signals", _signals);
            param.Add("Commands", _commands);
            _regionManager.RequestNavigate("ElementShowRegion", _elementView, param);

            getAlarms();
            _alarmService.AlarmRefreshEvent += onAlarmRefreshEvent;
            _alarmService.ConnectStatusChangeEvent += onConnectStatusChanged;
        }

        private void onConnectStatusChanged(bool isConnected)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (isConnected)
                {
                    getAlarms();
                }
                else
                {
                    Alarms.Clear();
                }
            });
        }

        private void getAlarms()
        {
            Alarms = new ObservableCollection<AlarmWrapper>();
               var allAlarms = _alarmService.AllExitAlarms;
            foreach (var alarm in allAlarms)
            {
                if (match(alarm.AlarmName))
                {
                    Alarms.Add(alarm);
                }
            }

        }
        private void onAlarmRefreshEvent(AlarmWrapper newAlarm, AlarmRefresh status)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (match(newAlarm.AlarmName))
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

            });
        }
        private bool match(string alarmName)
        {
            foreach (var signal in _signals)
            {
                if (alarmName.Contains(signal.SignalName))
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            Alarms.Clear();
            HistoryAlarms.Clear();
            _alarmService.AlarmRefreshEvent -= onAlarmRefreshEvent;
            _alarmService.ConnectStatusChangeEvent -= onConnectStatusChanged;
            _localizationService.LanguageChanged -= onLanguageChanged;

        }
    }
}
