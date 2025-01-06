using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Services.Dialogs;
using Prism.Events;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;
using DataServer.Permission;

namespace GuiBase.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        private ILocalizationService _localizationService;

        private IDialogService _dialogService;
        public DelegateCommand<string> DialogClickCommand { get; set; }

        private ISecurityService _securityService;


        private bool menuIsChecked;

        public bool MenuIsChecked
        {
            get { return menuIsChecked; }
            set { SetProperty(ref menuIsChecked, value, "MenuIsChecked"); }
        }

        private bool buttonIsChecked;

        public bool ButtonIsChecked
        {
            get { return buttonIsChecked; }
            set { SetProperty(ref buttonIsChecked, value, "ButtonIsChecked"); }
        }


        private bool menuEnable;

        public bool MenuEnable
        {
            get { return menuEnable; }
            set { SetProperty(ref menuEnable, value, "MenuEnable"); }
        }

        private string accManagerText;

        public string AccManagerText
        {
            get { return accManagerText; }
            set { SetProperty(ref accManagerText, value, "AccManagerText"); }
        }


        private bool accManagerEnable;
        public bool AccManagerEnable
        {
            get { return accManagerEnable; }
            set { SetProperty(ref accManagerEnable, value, "AccManagerEnable"); }
        }

        private string operRecordText;

        public string OperRecordText
        {
            get { return operRecordText; }
            set { SetProperty(ref operRecordText, value, "OperRecordText"); }
        }


        private bool operRecordEnable;

        public bool OperRecordEnable
        {
            get { return operRecordEnable; }
            set { SetProperty(ref operRecordEnable, value, "OperRecordEnable"); }
        }

        private string alarmText;

        public string AlarmText
        {
            get { return alarmText; }
            set { SetProperty(ref alarmText, value, "AlarmText"); }
        }


        private bool alarmEnable;

        public bool AlarmEnable
        {
            get { return alarmEnable; }
            set { SetProperty(ref alarmEnable, value, "AlarmEnable"); }
        }

        private string historyAlarmText;

        public string HistoryAlarmText
        {
            get { return historyAlarmText; }
            set { SetProperty(ref historyAlarmText, value, "HistoryAlarmText"); }
        }


        private bool historyAlarmEnable;

        public bool HistoryAlarmEnable
        {
            get { return historyAlarmEnable; }
            set { SetProperty(ref historyAlarmEnable, value, "HistoryAlarmEnable"); }
        }

        private string signalMonitorText;

        public string SignalMonitorText
        {
            get { return signalMonitorText; }
            set { SetProperty(ref signalMonitorText, value, "SignalMonitorText"); }
        }


        private bool signalMonitorEnable;

        public bool SignalMonitorEnable
        {
            get { return signalMonitorEnable; }
            set { SetProperty(ref signalMonitorEnable, value, "SignalMonitorEnable"); }
        }

        private string deviceConfigText;

        public string DeviceConfigText
        {
            get { return deviceConfigText; }
            set { SetProperty(ref deviceConfigText, value, "DeviceConfigText"); }
        }


        private bool deviceConfigEnable;

        public bool DeviceConfigEnable
        {
            get { return deviceConfigEnable; }
            set { SetProperty(ref deviceConfigEnable, value, "DeviceConfigEnable"); }
        }

        public MenuViewModel(IDialogService dialogService,ISecurityService securityService,ILocalizationService localizationService)
        {
            _dialogService = dialogService;
            _securityService = securityService;
            DialogClickCommand = new DelegateCommand<string>(dialogClick);
            _securityService.UserChangeEvent += _securityService_UserChangeEvent;
            _securityService_UserChangeEvent(null);
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            translate();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            AccManagerText = _localizationService.Translate(TranslateCommonId.AccManagerId);
            OperRecordText = _localizationService.Translate(TranslateCommonId.OperRecordId);
            AlarmText = _localizationService.Translate(TranslateCommonId.AlarmId);
            HistoryAlarmText = _localizationService.Translate(TranslateCommonId.HistoryAlarmId);
            SignalMonitorText = _localizationService.Translate(TranslateCommonId.SignalMonitorId);
            DeviceConfigText = _localizationService.Translate(TranslateCommonId.DeviceConfigId);
        }
        private void _securityService_UserChangeEvent(User user)
        {
            MenuEnable = _securityService.HasPermission("Menu_Bottom", ResourceType.Menu);
            AccManagerEnable = _securityService.HasPermission("AccManagerView", ResourceType.View);
            AlarmEnable = _securityService.HasPermission("AlarmView", ResourceType.View);
            HistoryAlarmEnable = _securityService.HasPermission("HistoryAlarmView", ResourceType.View);
            OperRecordEnable = _securityService.HasPermission("OperRecordView", ResourceType.View);
            SignalMonitorEnable = _securityService.HasPermission("SignalMonitor", ResourceType.View);
            DeviceConfigEnable = _securityService.HasPermission("DeviceConfig", ResourceType.View);
        }

        private void dialogClick(string btnName)
        {
            var viewName = string.Concat(btnName, "View");
            MenuIsChecked = false;
            if (viewName == "LogOnView")
            {
                _dialogService.ShowDialog(viewName, new DialogParameters(), null);
            }
            else
            {
                _dialogService.Show(viewName,new DialogParameters(), null);

            }
            ButtonIsChecked = false;
        }
        public void Clear()
        {
            _securityService.UserChangeEvent -= _securityService_UserChangeEvent;
            _localizationService.LanguageChanged -= onLanguageChanged;
        }
    }
}
