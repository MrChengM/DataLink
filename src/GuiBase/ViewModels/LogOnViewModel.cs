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

        private string name;

        public event Action<IDialogResult> RequestClose;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string title="LogOn";

        public string Title
        {
            get { return title="LogOn"; }
            set { SetProperty(ref title, value, "Title"); }
        }


        //private string password;

        //public string Password
        //{
        //    get { return password; }
        //    set { SetProperty(ref password, value, "Password"); }
        //}

        public DelegateCommand<object> LogOnCommand { get; set; }

        public DelegateCommand ExitCommand { get; set; }


        public LogOnViewModel(IEventAggregator ea,ISecurityService ss)
        {
            _ea = ea;
            _ss = ss;
            LogOnCommand = new DelegateCommand<object>(logOn);
            ExitCommand = new DelegateCommand(exit);
        }

       
        private void logOn(object obj)
        {
            var passwordBox = obj as PasswordBox;

            if (_ss.IsValidLogin(Name,passwordBox.Password))
            {
                _ea.GetEvent<PubSubEvent<AccoutLogOnResult>>().Publish(AccoutLogOnResult.Success);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));

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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
