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
        private ISecurityService _securityService;
        private ILocalizationService _localizationService;

        private RoleWrapper roleWrapper;
        public ObservableCollection<ResourceWrapperEx> ResourceExs { get; set; }

        public DelegateCommand<ResourceWrapperEx> CheckBtnCommand { get; set; }
        public DelegateCommand<string> ConfrimBtnCommand { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }



        public event Action<IDialogResult> RequestClose;

        private ResourceCaptions columns;

        public ResourceCaptions Columns
        {
            get { return columns; }
            set { SetProperty(ref columns, value, "Columns"); }
        }

        private string confirmText;

        public string ConfirmText
        {
            get { return confirmText; }
            set { SetProperty(ref confirmText, value, "ConfirmText"); }
        }

        private string cancelText;

        public string  CancelText
        {
            get { return cancelText; }
            set { SetProperty(ref cancelText, value, "CancelText"); }
        }

        public ResourceAssignViewModel(ISecurityService securityService,ILocalizationService localizationService)
        {
            _securityService = securityService;
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Columns = new ResourceCaptions(_localizationService);
            translate();
            CheckBtnCommand = new DelegateCommand<ResourceWrapperEx>(checkBtn);
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            ResourceExs = new ObservableCollection<ResourceWrapperEx>();
         
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Columns.GetContent();
            Title = _localizationService.Translate(TranslateCommonId.ResourceAssignId);
            ConfirmText = _localizationService.Translate(TranslateCommonId.ConfirmId);
            CancelText = _localizationService.Translate(TranslateCommonId.CancelId);
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
            Clear();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            roleWrapper = parameters.GetValue<RoleWrapper>("roleInfo");
            initResourExList();
        }

        public void Clear()
        {
            ResourceExs.Clear();
            _localizationService.LanguageChanged -= onLanguageChanged;

        }
    }
}
