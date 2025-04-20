using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using GuiBase.Models;
using GuiBase.Services;
using Prism.Commands;
using DataServer.Permission;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;

namespace GuiBase.ViewModels
{
    public class UserEditViewModel : BindableBase, IDialogAware
    {
        private ISecurityService _securityService;
        private ILocalizationService _localizationService;

        private UserWrapper userW;


        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
        public UserWrapper UserW
        {
            get { return userW; }
            set { SetProperty(ref userW, value, "UserW"); }
        }

        private List<string> ssexS;

        public List<string> SsexS
        {
            get { return ssexS; }
            set { SetProperty(ref ssexS, value, "SsexS"); }
        }
        public DelegateCommand<string> ConfrimBtnCommand { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }

        private UserCaptions captions;

        public UserCaptions Captions
        {
            get { return captions; }
            set { SetProperty(ref captions, value, "Captions"); }
        }

        private bool resetStatus;

        public bool ResetStatus
        {
            get { return resetStatus; }
            set { SetProperty(ref resetStatus, value,()=>setPasswordEnable() , "ResetStatus"); }
        }

        private bool passwordEnable;

        public bool PasswordEnable
        {
            get { return passwordEnable; }
            set { SetProperty(ref passwordEnable, value, "PasswordEnable"); }
        }

        private Visibility resetVisiblilty;

        public Visibility ResetVisiblilty
        {
            get { return resetVisiblilty; }
            set { SetProperty(ref resetVisiblilty, value, "ResetVisiblilty"); }
        }
        public UserEditViewModel (ISecurityService securityService,ILocalizationService localizationService)
        {
            _securityService = securityService;
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            SsexS =new List<string>( Enum.GetNames(typeof(SSex)));

            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Captions = new UserCaptions(_localizationService);
            translate();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.UserEditId);
            Captions.GetContent();
        }
        private void confrimBtn(string param)
        {
            var btnResult = new ButtonResult();
            if (param == "OK")
            {

                var roles = _securityService.GetAllRoles();
                var user = UserWrapper.Convert(UserW, roles);
                btnResult = ButtonResult.OK;
                if (BuildMode)
                {
                    user.CreateId = _securityService.GetCurrentUser().Id;
                    user.CreateTime = DateTime.Now;
                    user.Status = 0;
                    user.Password = _securityService.PasswordHash(UserW.Password);
                    if (!_securityService.CreateUser(user))
                    {
                        MessageBox.Show("Create user fail!");
                        return;
                    }
                }
                else
                {
                    if (ResetStatus)
                    {
                        user.Password = _securityService.PasswordHash(UserW.Password);
                    }
                    if (!_securityService.UpdateUser(user))
                    {
                        MessageBox.Show("Update user fail!");
                        return;
                    }
                }
            }
            else if (param == "Cancel")
            {
                btnResult = ButtonResult.Cancel;
            }
            RequestClose?.Invoke(new DialogResult(btnResult));
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
            BuildMode = parameters.GetValue<bool>("isBuild");
            setPasswordEnable();
            if (BuildMode)
            {
                UserW = new UserWrapper()
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleNameExs = new ObservableCollection<RoleNameEx>()
                };
                var Roles = _securityService.GetAllRoles();
                foreach (var role in Roles)
                {
                    UserW.RoleNameExs.Add(new RoleNameEx() { Name = role.Name, IsChecked = false });
                }

            }
            else
            {
                UserW = parameters.GetValue<UserWrapper>("userInfo")?.CopyTo();
            }

        }

        public event Action<IDialogResult> RequestClose;
        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;

        }

        private void setPasswordEnable()
        {
            if (BuildMode)
            {
                PasswordEnable = true;
                ResetVisiblilty = Visibility.Hidden;
            }
            else
            {
                ResetVisiblilty = Visibility.Visible;
                PasswordEnable = resetStatus;
            }
        }
    }
}
