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
using DataServer;
using System.Threading;
using System.Threading.Tasks;

namespace GuiBase.ViewModels
{
    public class HistoryAlarmViewModel : BindableBase, IDialogAware
    {
        private ILog _log;
        private IHistoryAlarmService _historyAlarmService;
        public string Title => "HistoryAlarm";

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

        public DelegateCommand<string> TopDrawerOperationCommand { get; set; }
        //public DelegateCommand DrawerCloseCommand { get; set; }

        public HistoryAlarmViewModel(IHistoryAlarmService historyAlarmService,ILog log)
        {
            _historyAlarmService = historyAlarmService;
            _log = log;
            TopDrawerOperationCommand = new DelegateCommand<string>(topDrawerOperation);
            //DrawerCloseCommand = new DelegateCommand(drawerClose);
        }

        //private void drawerClose()
        //{

        //    App.Current.Dispatcher.Invoke(() =>
        //    {
        //        IsWaiting = true;
        //        HistoryAlarms = _historyAlarmService.Select(SelectCondition);
        //        Counts = HistoryAlarms.Count;
        //        IsWaiting = false;

        //    });
        //}

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
                    Thread.Sleep(3000);
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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
