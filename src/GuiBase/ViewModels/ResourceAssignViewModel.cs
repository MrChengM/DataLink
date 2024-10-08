using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using Prism.Services.Dialogs;
using GuiBase.Services;
using System.Collections.ObjectModel;
using GuiBase.Models;
using DataServer.Permission;
using System.Windows;

namespace GuiBase.ViewModels
{
    public class ResourceAssignViewModel : BindableBase,IDialogAware
    {
        public ISecurityService _securityService;

        public RoleWrapper roleWrapper;
        public ObservableCollection<ResourceWrapperEx> ResourceExs { get; set; }

        public DelegateCommand<ResourceWrapperEx> CheckBtnCommand { get; set; }
        public DelegateCommand<string> ConfrimBtnCommand { get; set; }
        public string Title => "Resource Assign";

        public event Action<IDialogResult> RequestClose;

        public ResourceAssignViewModel(ISecurityService securityService)
        {
            _securityService = securityService;
            CheckBtnCommand = new DelegateCommand<ResourceWrapperEx>(checkBtn);
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            ResourceExs = new ObservableCollection<ResourceWrapperEx>();
         
        }

        private void initResourExList()
        {
            var resources = _securityService.GetAllResources();
            foreach (var resource in resources)
            {
                if (roleWrapper.ResourceIds.Count > 0)
                {
                    ResourceExs.Add(new ResourceWrapperEx() { IsChecked = roleWrapper.ResourceIds.Contains(resource.Id), Wrapper = ResourceWrapper.Convert(resource) });
                }
                else
                {
                    ResourceExs.Add(new ResourceWrapperEx() { IsChecked = false, Wrapper = ResourceWrapper.Convert(resource) });

                }
            }
        }
        private void confrimBtn(string param)
        {
            var btnResult = new ButtonResult();
            if (param == "OK")
            {
                var resources = _securityService.GetAllResources();
                roleWrapper.ResourceIds.Clear();
                foreach (var resourceEx in ResourceExs)
                {
                    if (resourceEx.IsChecked)
                    {
                        roleWrapper.ResourceIds.Add(resourceEx.Wrapper.Id);
                    }
                }
                if (!_securityService.UpdateRole(RoleWrapper.Convert(roleWrapper, resources)))
                {
                    MessageBox.Show("Update Role failed!");
                    return;
                }
                btnResult = ButtonResult.OK;
            }
            else if (param == "Cancel")
            {
                btnResult = ButtonResult.Cancel;
            }
            RequestClose?.Invoke(new DialogResult(btnResult));
        }

        private void checkBtn(ResourceWrapperEx ex)
        {
            foreach (var resourceEx in ResourceExs)
            {
                if (ex.IsChecked)
                {
                    if (resourceEx.Wrapper.ParentId == ex.Wrapper.Id)
                    {
                        resourceEx.IsChecked = true;
                        checkBtn(resourceEx);
                    }

                }
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
            roleWrapper = parameters.GetValue<RoleWrapper>("roleInfo");
            initResourExList();
        }
    }
}
