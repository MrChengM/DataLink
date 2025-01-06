using Prism.Mvvm;
using System;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataServer.Log;

namespace GuiBase.ViewModels
{
    public class HistoryAlarmViewModel : BindableBase, IDialogAware
    {
        private ILog _log;
        private IHistoryAlarmService _historyAlarmService;
        private ILocalizationService _localizationService;
        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }
        public event Action<IDialogResult> RequestClose;


        public HistoryAlarmSelectConditionWrapper SelectCondition { get; set; } = new HistoryAlarmSelectConditionWrapper();

        private List<HistoryAlarmWrapper> historyAlarms = new List<HistoryAlarmWrapper>();

        public List<HistoryAlarmWrapper> HistoryAlarms
        {
            get { return historyAlarms; }
            set { SetProperty(ref historyAlarms, value, "HistoryAlarms"); }
        }

        private bool topDrawerEnable;

        public bool TopDrawerEnable
        {
            get { return topDrawerEnable; }
            set { SetProperty(ref topDrawerEnable, value, "TopDrawerEnable"); }
        }

        private int counts;

        public int Counts
        {
            get { return counts; }
            set { SetProperty(ref counts, value, "Counts"); }
        }
        private bool isWaiting;

        public bool IsWaiting
        {
            get { return isWaiting; }
            set { SetProperty(ref isWaiting, value, "IsWaiting"); }
        }
        private string filterCondition;

        public string FilterCondition
        {
            get { return filterCondition; }
            set { SetProperty(ref filterCondition, value, "FilterCondition"); }
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

        private string textTotal;

        public string TextTotal
        {
            get { return textTotal; }
            set { SetProperty(ref textTotal, value, "TextTotal"); }
        }

        public DelegateCommand<string> TopDrawerOperationCommand { get; set; }
        //public DelegateCommand DrawerCloseCommand { get; set; }

        public HistoryAlarmViewModel(IHistoryAlarmService historyAlarmService,ILog log,ILocalizationService localizationService)
        {
            _historyAlarmService = historyAlarmService;
            _log = log;
            TopDrawerOperationCommand = new DelegateCommand<string>(topDrawerOperation);
            //DrawerCloseCommand = new DelegateCommand(drawerClose);
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Columns = new AlarmCaptions(localizationService);
            translate();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.AlarmId);
            FilterCondition = _localizationService.Translate(TranslateCommonId.FilterConditionId);
            ButtonOK = _localizationService.Translate(TranslateCommonId.OKId);
            ButtonCancel = _localizationService.Translate(TranslateCommonId.CancelId);
            TextTotal = _localizationService.Translate(TranslateCommonId.TotalId);
            foreach (var hisAlarm in HistoryAlarms)
            {
                hisAlarm.LocalizationDescrible= _localizationService.Translate(hisAlarm.AlarmNumber);
            }
            Columns.GetContent();
        }

        private void topDrawerOperation(string topDrawerParam)
        {
            var temp = new List<HistoryAlarmWrapper>();
            if (topDrawerParam == "Open")
            {
                TopDrawerEnable = true;
            }
            else if (topDrawerParam == "Close")
            {
                Task.Run(() =>
                {
                    IsWaiting = true;
                    //Thread.Sleep(3000);
                    HistoryAlarms = _historyAlarmService.Select(SelectCondition);
                    if (HistoryAlarms != null)
                    {
                        Counts = HistoryAlarms.Count;
                    }
                    IsWaiting = false;
                    TopDrawerEnable = false;
                });

                //App.Current.Dispatcher.Invoke(() =>
                //{

                //    HistoryAlarms = temp;
                //});
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
            HistoryAlarms.Clear();
            _localizationService.LanguageChanged -= onLanguageChanged;

        }
    }
}
