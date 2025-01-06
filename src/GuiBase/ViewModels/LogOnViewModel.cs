using Prism.Mvvm;
using System;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;

namespace GuiBase.ViewModels
{
    public class LogOnViewModel: BindableBase,IDialogAware
    {
        private IEventAggregator _ea;
        private ISecurityService _ss;
        private ILocalizationService _localizationService;
        private IOperateRecordService _operateRecordService;
        private string name;

        public event Action<IDialogResult> RequestClose;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }

        private string messages;

        public string Messages
        {
            get { return messages; }
            set { SetProperty(ref messages, value, "Messages"); }
        }

        private string buttonLogon;

        public string ButtonLogon
        {
            get { return buttonLogon; }
            set { SetProperty(ref buttonLogon, value, "ButtonOK"); }
        }

        private string buttonExit;

        public string ButtonExit
        {
            get { return buttonExit; }
            set { SetProperty(ref buttonExit, value, "ButtonCancel"); }
        }

        private string passwordHelper;

        public string PasswordHelper
        {
            get { return passwordHelper; }
            set { SetProperty(ref passwordHelper, value, "PasswordHelper"); }
        }


        //private string password;

        //public string Password
        //{
        //    get { return password; }
        //    set { SetProperty(ref password, value, "Password"); }
        //}

        public DelegateCommand<object> LogOnCommand { get; set; }

        public DelegateCommand ExitCommand { get; set; }


        public LogOnViewModel(IEventAggregator ea,ISecurityService ss,ILocalizationService localizationService,IOperateRecordService operateRecordService)
        {
            _ea = ea;
            _ss = ss;
            _localizationService = localizationService;
            _operateRecordService = operateRecordService;
            _localizationService.LanguageChanged += onLanguageChanged;
            LogOnCommand = new DelegateCommand<object>(logOn);
            ExitCommand = new DelegateCommand(exit);
            translate();
        }
        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.LogonId);
            ButtonLogon = _localizationService.Translate(TranslateCommonId.LogonId);
            ButtonExit = _localizationService.Translate(TranslateCommonId.ExitId);
            PasswordHelper = _localizationService.Translate(TranslateCommonId.PasswordHelperId);
        }

        private void logOn(object obj)
        {
            var passwordBox = obj as PasswordBox;

            if (_ss.IsValidLogin(Name, passwordBox.Password))
            {
                _operateRecordService.Insert(TranslateCommonId.LogonId, "Logon");
                _ea.GetEvent<PubSubEvent<AccoutLogOnResult>>().Publish(AccoutLogOnResult.Success);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                
            }
            else
            {
                Messages = _localizationService.Translate(TranslateCommonId.LogonFaliMessageId); ;

            }

        }
        private void exit()
        {
            _ea.GetEvent<PubSubEvent<AccoutLogOnResult>>().Publish(AccoutLogOnResult.Exit);
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));

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
        }
        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;
        }
    }
}
