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
    public class RoleManagerViewModel : BindableBase, INavigationAware
    {
        private ISecurityService _securityService;
        private IDialogService _dialogService;

        private string filterName;

        public string FilterName
        {
            get { return filterName; }
            set { SetProperty(ref filterName, value, "FilterName"); }
        }

        private ObservableCollection<RoleWrapper> roleWrappers;

        public ObservableCollection<RoleWrapper> RoleWrappers
        {
            get { return roleWrappers; }
            set { SetProperty(ref roleWrappers, value, "RoleWrappers"); }
        }
        public DelegateCommand SearchRolesCommand { get; set; }

        public DelegateCommand AddRoleCommand { get; set; }
        public DelegateCommand ImportRolesCommand { get; set; }
        public DelegateCommand ExportRolesCommand { get; set; }

        public DelegateCommand<RoleWrapper> EnableOpertCommand { get; set; }
        public DelegateCommand<RoleWrapper> EditRoleCommand { get; set; }
        public DelegateCommand<RoleWrapper> DeleteRoleCommand { get; set; }
        public DelegateCommand<RoleWrapper> AssignResourceCommand { get; set; }


        public RoleManagerViewModel(ISecurityService securityService,IDialogService dialogService)
        {
            _securityService = securityService;
            _dialogService = dialogService;
            searchRoles();
            SearchRolesCommand = new DelegateCommand(searchRoles);
            AddRoleCommand = new DelegateCommand(addRole);
            ImportRolesCommand = new DelegateCommand(importRoles);
            ExportRolesCommand = new DelegateCommand(exportRoles);
            EnableOpertCommand = new DelegateCommand<RoleWrapper>(enableOpert);
            EditRoleCommand = new DelegateCommand<RoleWrapper>(editRole);
            DeleteRoleCommand = new DelegateCommand<RoleWrapper>(deleteRole);
            AssignResourceCommand = new DelegateCommand<RoleWrapper>(assignResource);
    }

        private void assignResource(RoleWrapper wrapper)
        {
            var dParam = new DialogParameters();
            dParam.Add("roleInfo", wrapper);
            _dialogService.ShowDialog("ResourceAssignView", dParam,null);
        }

        private void deleteRole(RoleWrapper wrapper)
        {
            var btnResult = MessageBox.Show("确定要删除该角色吗？", "提示", MessageBoxButton.OKCancel);
            if (btnResult == MessageBoxResult.OK)
            {
                var resources = _securityService.GetAllResources();
                if (_securityService.DeleteRole(RoleWrapper.Convert(wrapper, resources)))
                {
                    RoleWrappers.Remove(wrapper);
                }
                else
                {
                    MessageBox.Show("删除角色失败！", "提示");
                }
            }
        }

        private void editRole(RoleWrapper wrapper)
        {
            var dParam = new DialogParameters();
            dParam.Add("roleInfo",wrapper);
            dParam.Add("isBuild", false);
            _dialogService.ShowDialog("RoleEditView", dParam, s =>
            {
                if (s.Result == ButtonResult.OK)
                {
                    searchRoles();
                }

            });
        }

        private void enableOpert(RoleWrapper wrapper)
        {
            var resources = _securityService.GetAllResources();
            if (!wrapper.Status)
            {
                var btnResult = MessageBox.Show("确定要禁用该角色吗？", "提示", MessageBoxButton.OKCancel);

                if (btnResult == MessageBoxResult.OK)
                {
                    if (_securityService.UpdateRole(RoleWrapper.Convert(wrapper, resources)))
                    {
                        wrapper.Status = false;
                    }
                    else
                    {
                        MessageBox.Show("禁用角色失败!", "提示");
                        wrapper.Status = true;

                    }
                }
                else
                {
                    wrapper.Status = true;
                }
            }
            else
            {
                if (_securityService.UpdateRole(RoleWrapper.Convert(wrapper, resources)))
                {
                    wrapper.Status = true;
                }
                else
                {
                    MessageBox.Show("启用角色失败！", "提示");
                    wrapper.Status = false;

                }
            }
        }

        private void exportRoles()
        {
        }

        private void importRoles()
        {
        }

        private void addRole()
        {
            var dParam = new DialogParameters();
            dParam.Add("isBuild", true);
            _dialogService.ShowDialog("RoleEditView", dParam, s =>
            {
                if (s.Result == ButtonResult.OK)
                {
                    searchRoles();
                }

            });
        }

        private void searchRoles()
        {
            RoleWrappers = new ObservableCollection<RoleWrapper>();
            var roles = _securityService.GetAllRoles();
            if (FilterName != null && FilterName != "")
            {
                var rs = from a in roles
                         where a.Name.Contains(FilterName)
                         select a;
                foreach (var r in rs)
                {
                    RoleWrappers.Add(RoleWrapper.Convert(r));
                }
            }
            else
            {
                foreach (var role in roles)
                {
                    RoleWrappers.Add(RoleWrapper.Convert(role));
                }
            }
           
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
