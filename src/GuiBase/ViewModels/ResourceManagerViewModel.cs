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
    public class ResourceManagerViewModel : BindableBase, INavigationAware
    {
        private ISecurityService _securityService;
        private IDialogService _dialogService;

        private string filterName;

        public string FilterName
        {
            get { return filterName; }
            set { SetProperty(ref filterName, value, "FilterName"); }
        }

        private ObservableCollection<ResourceWrapper> resourceWrappers;

        public ObservableCollection<ResourceWrapper> ResourceWrappers
        {
            get { return resourceWrappers; }
            set { SetProperty(ref resourceWrappers, value, "ResourceWrappers"); }
        }
        public DelegateCommand SearchResourcesCommand { get; set; }

        public DelegateCommand AddResourceCommand { get; set; }
        public DelegateCommand ImportResourcesCommand { get; set; }
        public DelegateCommand ExportResourcesCommand { get; set; }

        public DelegateCommand<ResourceWrapper> EnableOpertCommand { get; set; }
        public DelegateCommand<ResourceWrapper> EditResourceCommand { get; set; }
        public DelegateCommand<ResourceWrapper> DeleteResourceCommand { get; set; }


        public ResourceManagerViewModel(ISecurityService securityService,IDialogService dialogService)
        {
            _securityService = securityService;
            _dialogService = dialogService;
            searchResources();
            SearchResourcesCommand = new DelegateCommand(searchResources);
            AddResourceCommand = new DelegateCommand(addResource);
            ImportResourcesCommand = new DelegateCommand(importRoles);
            ExportResourcesCommand = new DelegateCommand(exportRoles);
            EnableOpertCommand = new DelegateCommand<ResourceWrapper>(enableOpert);
            EditResourceCommand = new DelegateCommand<ResourceWrapper>(editResource);
            DeleteResourceCommand = new DelegateCommand<ResourceWrapper>(deleteResource);
    }

     

        private void deleteResource(ResourceWrapper wrapper)
        {
            var btnResult = MessageBox.Show("确定要删除该资源吗？", "提示", MessageBoxButton.OKCancel);
            if (btnResult == MessageBoxResult.OK)
            {
                if (_securityService.DeleteResource(ResourceWrapper.Convert(wrapper)))
                {
                    ResourceWrappers.Remove(wrapper);
                }
                else
                {
                    MessageBox.Show("删除资源失败！", "提示");
                }
            }
        }

        private void editResource(ResourceWrapper wrapper)
        {
            var dParam = new DialogParameters();
            dParam.Add("resourceInfo",wrapper);
            dParam.Add("isBuild", false);
            _dialogService.ShowDialog("ResourceEditView", dParam, s =>
            {
                if (s.Result == ButtonResult.OK)
                {
                    searchResources();
                }

            });
        }

        private void enableOpert(ResourceWrapper wrapper)
        {
            if (wrapper.Disable)
            {
                var btnResult = MessageBox.Show("确定要禁用该资源吗？", "提示", MessageBoxButton.OKCancel);

                if (btnResult == MessageBoxResult.OK)
                {
                    if (_securityService.UpdateResource(ResourceWrapper.Convert(wrapper)))
                    {
                        wrapper.Disable = true;
                    }
                    else
                    {
                        MessageBox.Show("禁用资源失败!", "提示");
                        wrapper.Disable = false;

                    }
                }
                else
                {
                    wrapper.Disable = false;
                }
            }
            else
            {
                if (_securityService.UpdateResource(ResourceWrapper.Convert(wrapper)))
                {
                    wrapper.Disable = false;
                }
                else
                {
                    MessageBox.Show("启用资源失败！", "提示");
                    wrapper.Disable = true;

                }
            }
        }

        private void exportRoles()
        {
        }

        private void importRoles()
        {
        }

        private void addResource()
        {
            var dParam = new DialogParameters();
            dParam.Add("isBuild", true);
            _dialogService.ShowDialog("ResourceEditView", dParam, s =>
            {
                if (s.Result == ButtonResult.OK)
                {
                    searchResources();
                }

            });
        }

        private void searchResources()
        {
            ResourceWrappers = new ObservableCollection<ResourceWrapper>();
            var resources = _securityService.GetAllResources();

            if (FilterName != null && FilterName != "")
            {
                var res = from a in resources
                          where a.Name.Contains(FilterName)
                          select a;
                foreach (var resource in res)
                {
                    ResourceWrappers.Add(ResourceWrapper.Convert(resource));
                }
            }
            else
            {
                foreach (var resource in resources)
                {
                    ResourceWrappers.Add(ResourceWrapper.Convert(resource));
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
