using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using GuiBase.Services;
namespace GuiBase.Models
{
    public class UserCaptions : BindableBase
    {
        private ILocalizationService _localizationService;

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string account;

        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value, "Account"); }
        }
        private string add;

        public string Add
        {
            get { return add; }
            set { SetProperty(ref add, value, "Add"); }
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value, "Id"); }
        }

        private string sex;

        public string Sex
        {
            get { return sex; }
            set { SetProperty(ref sex, value, "Sex"); }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value, "Status"); }
        }

        private string createId;

        public string CreateId
        {
            get { return createId; }
            set { SetProperty(ref createId, value, "CreateId"); }
        }

        private string createTime;

        public string CreateTime
        {
            get { return createTime; }
            set { SetProperty(ref createTime, value, "CreateTime"); }
        }

        private string confirm;

        public string Confirm
        {
            get { return confirm; }
            set { SetProperty(ref confirm, value, "Confirm"); }
        }

        private string cancel;

        public string Cancel
        {
            get { return cancel; }
            set { SetProperty(ref cancel, value, "Cancel"); }
        }

        private string edit;

        public string Edit
        {
            get { return edit; }
            set { SetProperty(ref edit, value, "Edit"); }
        }

        private string delete;

        public string Delete
        {
            get { return delete; }
            set { SetProperty(ref delete, value, "Delete"); }
        }
        private string assign;

        public string Assign
        {
            get { return assign; }
            set { SetProperty(ref assign, value, "Assign"); }
        }
        private string operate;

        public string Operate
        {
            get { return operate; }
            set { SetProperty(ref operate, value, "Operate"); }
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
        private string confirmPassword;

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { SetProperty(ref confirmPassword, value, "ConfirmPassword"); }
        }

        private string changePassword;

        public string ChangePassword
        {
            get { return changePassword; }
            set { SetProperty(ref changePassword, value, "ChangePassword"); }
        }

        private string logout;

        public string Logout
        {
            get { return logout; }
            set { SetProperty(ref logout, value, "Logout"); }
        }
        private string roles;

        public string Roles
        {
            get { return roles; }
            set { SetProperty(ref roles, value, "Roles"); }
        }
        private string password;

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value, "Password"); }
        }


        public UserCaptions(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        public void GetContent()
        {
            Name = _localizationService[TranslateCommonId.NameId];
            Account = _localizationService[TranslateCommonId.AccountId];
            Add = _localizationService[TranslateCommonId.AddId];
            Id = _localizationService[TranslateCommonId.IdId];
            Sex = _localizationService[TranslateCommonId.SexId];
            Status = _localizationService[TranslateCommonId.StatusId];
            CreateId = _localizationService[TranslateCommonId.CreateIdId];
            CreateTime = _localizationService[TranslateCommonId.CreateTimeId];
            Operate = _localizationService[TranslateCommonId.OperateId];
            Confirm = _localizationService[TranslateCommonId.ConfirmId];
            Cancel = _localizationService[TranslateCommonId.CancelId];
            Edit = _localizationService[TranslateCommonId.EditId];
            Delete = _localizationService[TranslateCommonId.DeleteId];
            Assign = _localizationService[TranslateCommonId.AssignId];
            OldPassword = _localizationService[TranslateCommonId.OldPasswordId];
            NewPassword = _localizationService[TranslateCommonId.NewPasswordId];
            ConfirmPassword = _localizationService[TranslateCommonId.ConfirmPasswordId];
            ChangePassword = _localizationService[TranslateCommonId.ChangePasswordId];
            Logout = _localizationService[TranslateCommonId.LogoutId];
            Roles = _localizationService[TranslateCommonId.RolesId];
            Password = _localizationService[TranslateCommonId.PasswordId];
        }
    }
}
