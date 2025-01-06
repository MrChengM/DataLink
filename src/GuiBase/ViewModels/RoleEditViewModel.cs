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
    public class RoleEditViewModel :BindableBase, IDialogAware
    {
        private ISecurityService _securityService;
        private ILocalizationService _localizationService;

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }


        public event Action<IDialogResult> RequestClose;

        private RoleWrapper roleW;

        
        public RoleWrapper RoleW
        {
            get { return roleW; }
            set { SetProperty(ref roleW, value, "RoleW"); }
        }

        private RoleCaptions captions;

        public RoleCaptions Captions
        {
            get { return captions; }
            set { SetProperty(ref captions, value, "Captions"); }
        }


        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
        public DelegateCommand<string> ConfrimBtnCommand { get; set; }

        public RoleEditViewModel(ISecurityService securityService,ILocalizationService localizationService)
        {
            _securityService = securityService;
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);

            _localizationService = localizationService;
            Captions = new RoleCaptions(_localizationService);
            _localizationService.LanguageChanged += onLanguageChanged;
            translate();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.RoleEditId);
            Captions.GetContent();
        }
        private void confrimBtn(string param)
        {
            var btnResult = new ButtonResult();
            if (param == "OK")
            {

                var resources = _securityService.GetAllResources();
                var role = RoleWrapper.Convert(RoleW, resources);
                btnResult = ButtonResult.OK;
                if (BuildMode)
                {
                    role.CreateId = _securityService.GetCurrentUser().Id;
                    role.CreateTime = DateTime.Now;
                    role.Status = 0;
                    if (!_securityService.CreateRole(role))
                    {
                        MessageBox.Show("Create role fail!");
                        return;
                    }
                }
                else
                {
                    if (!_securityService.UpdateRole(role))
                    {
                        MessageBox.Show("Update role fail!");
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
          return  true;
        }

        public void OnDialogClosed()
        {
            Clear();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            BuildMode = parameters.GetValue<bool>("isBuild");
            if (BuildMode)
            {
                RoleW = new RoleWrapper()
                {
                    Id = Guid.NewGuid().ToString(),
                    ResourceIds = new List<string>()
                };
            }
            else
            {
                RoleW = parameters.GetValue<RoleWrapper>("roleInfo")?.CopyTo();
            }
        }
        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;

        }
    }
}
