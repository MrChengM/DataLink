using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using GuiBase.Services;
using Prism.Commands;
using DataServer.Permission;
using Prism.Services.Dialogs;

namespace GuiBase.ViewModels
{
    public class UserDetailedViewModel:BindableBase, IDialogAware
    {
        private ISecurityService _securityService;
        private string account;

        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value, "Account"); }
        }


        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        private string sex;

        public string Sex
        {
            get { return sex; }
            set { SetProperty(ref sex, value, "Sex"); }
        }
        private string  creatTime;

        public string  CreatTime
        {
            get { return creatTime; }
            set { SetProperty(ref creatTime, value, "CreatTime"); }
        }
        private bool isWaiting;

        public bool IsWaiting
        {
            get { return isWaiting; }
            set { SetProperty(ref isWaiting, value, "IsWaiting"); }
        }
        private bool topDrawerEnable;

        public bool TopDrawerEnable
        {
            get { return topDrawerEnable; }
            set { SetProperty(ref topDrawerEnable, value, "TopDrawerEnable"); }
        }
        private string messages;

        public string Messages
        {
            get { return messages; }
            set { SetProperty(ref messages, value, "Messages"); }
        }
        private string oldPassword;

        public string OldPassword
        {
            get { return oldPassword; }
            set { SetProperty(ref oldPassword, value, "OldPassword"); }
        }
        private string newPassword;

        public string NewPassword
        {
            get { return newPassword; }
            set { SetProperty(ref newPassword, value, "NewPassword"); }
        }

        private bool btnEnable;

        public bool BtnEnable
        {
            get { return btnEnable; }
            set { SetProperty(ref btnEnable, value, "BtnEnable"); }
        }

        private string confirmPassword;

        public event Action<IDialogResult> RequestClose;

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { SetProperty(ref confirmPassword, value, "ConfirmPassword"); }
        }
        public DelegateCommand<string> ConfirmCommand { get; set; }
        public DelegateCommand OpenDrawerCommand { get; set; }
        public DelegateCommand CancelLogOnCommand { get; set; }

        public string Title => "用户详情";

        public UserDetailedViewModel(ISecurityService securityService)
        {
            _securityService = securityService;
            ConfirmCommand = new DelegateCommand<string>(confirm);
            OpenDrawerCommand = new DelegateCommand(() => TopDrawerEnable = true);
            CancelLogOnCommand = new DelegateCommand(() =>
            {
                _securityService.CancelLogin();
                Account = null;
                Name = null;
                Sex = null;
                CreatTime = null;
                BtnEnable = false;
            });
            var user = _securityService.GetCurrentUser();
            Account = user.Account;
            Name = user.Name;
            Sex = ((SSex)user.SSex).ToString();
            CreatTime = user.CreateTime.ToString("G");
            BtnEnable = true;
        }

        private void confirm(string param)
        {
            if (param == "OK")
            {
                IsWaiting = true;
                if (NewPassword == ConfirmPassword)
                {
                    if (_securityService.ChangePassword(Account, OldPassword, NewPassword))
                    {
                        TopDrawerEnable = false;
                        OldPassword = null;
                        NewPassword = null; 
                        ConfirmPassword = null;
                        Messages = null;
                    }
                    else
                    {
                        Messages = "请验证正确的密码！";

                    }
                }
                else
                {
                    Messages = "请确认新密码两次输入一致！";
                }
                IsWaiting = false;
            }
            else
            {
                TopDrawerEnable = false;
            }
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
