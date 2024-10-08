﻿using System;
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
    public class UserEditViewModel:BindableBase,IDialogAware
    {
        private ISecurityService _securityService;
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
        public string Title => "User Edit";

        public UserEditViewModel (ISecurityService securityService)
        {
            _securityService = securityService;
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            SsexS =new List<string>( Enum.GetNames(typeof(SSex)));
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
                    if (!_securityService.CreateUser(user))
                    {
                        MessageBox.Show("Create user fail!");
                        return;
                    }
                }
                else
                {
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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            BuildMode = parameters.GetValue<bool>("isBuild");
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

      
    }
}
