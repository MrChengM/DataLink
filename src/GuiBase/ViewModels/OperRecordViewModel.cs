using Prism.Mvvm;
using System;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GuiBase.ViewModels
{
    public class OperRecordViewModel : BindableBase, IDialogAware
    {
        private IOperateRecordService _operateRecordService;
        private ILocalizationService _localizationService;
        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }
        public event Action<IDialogResult> RequestClose;


        public OperateRecordSelectConditionWrapper SelectCondition { get; set; } = new OperateRecordSelectConditionWrapper();

        private ObservableCollection<OperateRecordWrapper> operateRecords = new ObservableCollection<OperateRecordWrapper>();

        public ObservableCollection<OperateRecordWrapper> OperateRecords
        {
            get { return operateRecords; }
            set { SetProperty(ref operateRecords, value, "OperateRecords"); }
        }

        private bool topDrawerEnable;

        public bool TopDrawerEnable
        {
            get { return topDrawerEnable; }
            set { SetProperty(ref topDrawerEnable, value, "TopDrawerEnable"); }
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
        private OperateRecordCaption caption;

        public OperateRecordCaption Caption
        {
            get { return caption; }
            set { SetProperty(ref caption, value, "Caption"); }
        }

       
        public DelegateCommand<string> TopDrawerOperationCommand { get; set; }
        //public DelegateCommand DrawerCloseCommand { get; set; }

        public OperRecordViewModel(IOperateRecordService operateRecordService, ILocalizationService localizationService)
        {
            _operateRecordService = operateRecordService;
            TopDrawerOperationCommand = new DelegateCommand<string>(topDrawerOperation);
            //DrawerCloseCommand = new DelegateCommand(drawerClose);
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Caption = new OperateRecordCaption(localizationService);
            translate();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.OperRecordId);
            FilterCondition = _localizationService.Translate(TranslateCommonId.FilterConditionId);
            foreach (var record in OperateRecords)
            {
                record.Message = _localizationService.TranslateBaseOnRules(record.Transcode);
            }
            Caption.GetContent();
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
                IsWaiting = true;
                //Thread.Sleep(3000);
                var records = _operateRecordService.Select(SelectCondition);
                OperateRecords.Clear();
                foreach (var record in records)
                {
                    OperateRecords.Add(OperateRecordWrapper.Convert(record, _localizationService));
                }
                IsWaiting = false;
                TopDrawerEnable = false;
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            //RequestClose?.Invoke(new DialogResult());
            Clear();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public void Clear()
        {
            OperateRecords.Clear();
            _localizationService.LanguageChanged -= onLanguageChanged;

        }
    }
}
