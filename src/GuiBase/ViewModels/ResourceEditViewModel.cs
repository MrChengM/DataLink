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
    public class ResourceEditViewModel : BindableBase, IDialogAware
    {
        private IDialogService _dialogService;
        private ISecurityService _securityService;
        public string Title => "Resource Edit";

        public event Action<IDialogResult> RequestClose;

        private ResourceWrapper resourceW;

        
        public ResourceWrapper ResourceW
        {
            get { return resourceW; }
            set { SetProperty(ref resourceW, value, "ResourceW"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }
        public DelegateCommand<string> ConfrimBtnCommand { get; set; }
        public DelegateCommand SearchParentBtnCommand { get; set; }
        public DelegateCommand SearchNameBtnCommand { get; set; }

        public ResourceEditViewModel(IDialogService dialogService, ISecurityService securityService)
        {
            _dialogService = dialogService;
            _securityService = securityService;
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            SearchParentBtnCommand = new DelegateCommand(SearchParentBtn);
            SearchNameBtnCommand= new DelegateCommand(SearchNameBtn);
        }

        private void SearchParentBtn()
        {
            _dialogService.ShowDialog("ResourceListView", s =>
            {
                if (s.Result == ButtonResult.OK)
                {
                    var resource = s.Parameters.GetValue<Resource>("Resource");
                    if (ResourceW.Id == resource.Id)
                    {
                        MessageBox.Show("不能将资源本身设为资源的父类");
                    }
                    else
                    {
                        ResourceW.ParentId = resource.Id;
                        ResourceW.ParentName = resource.Name;
                    }

                }
            });
        }

        private void SearchNameBtn()
        {
            _dialogService.ShowDialog("ResourceNameListView", s =>
            {
                if (s.Result==ButtonResult.OK)
                {
                    var resourceName = s.Parameters.GetValue<string>("ResourceName");
                    ResourceW.Name = resourceName;
                }
            });
        }

        private void confrimBtn(string param)
        {
            var btnResult = new ButtonResult();
            if (param == "OK")
            {

                var resource = ResourceWrapper.Convert(ResourceW);
                btnResult = ButtonResult.OK;
                if (BuildMode)
                {
                    resource.CreateUserId = _securityService.GetCurrentUser().Id;
                    resource.CreateTime = DateTime.Now;
                    resource.Disable = false;
                    if (!_securityService.CreateResource(resource))
                    {
                        MessageBox.Show("Create Resource fail!");
                        return;
                    }
                }
                else
                {
                    resource.UpdateTime = DateTime.Now;
                    resource.UpdateUserId = _securityService.GetCurrentUser().Id;
                    if (!_securityService.UpdateResource(resource))
                    {
                        MessageBox.Show("Update Resource fail!");
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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            BuildMode = parameters.GetValue<bool>("isBuild");
            if (BuildMode)
            {
                ResourceW = new ResourceWrapper()
                {
                    Id = Guid.NewGuid().ToString(),
                };
            }
            else
            {
                ResourceW = parameters.GetValue<ResourceWrapper>("resourceInfo")?.CopyTo();
            }
        }
    }
}
