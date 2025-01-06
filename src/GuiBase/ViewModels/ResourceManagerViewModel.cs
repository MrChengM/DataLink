using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using Prism.Services.Dialogs;
using Prism.Events;
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
        private ILocalizationService _localizationService;
        private IEventAggregator _ea;
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
        private ResourceCaptions captions;

        public ResourceCaptions Captions
        {
            get { return captions; }
            set { SetProperty(ref captions, value, "Captions"); }
        }

        private string addText;

        public string AddText
        {
            get { return addText; }
            set { SetProperty(ref addText, value, "AddText"); }
        }

        private string editText;

        public string EditText
        {
            get { return editText; }
            set { SetProperty(ref editText, value, "EditText"); }
        }

        private string deleteText;

        public string DeleteText
        {
            get { return deleteText; }
            set { SetProperty(ref deleteText, value, "DeleteText"); }
        }



        public DelegateCommand SearchResourcesCommand { get; set; }

        public DelegateCommand AddResourceCommand { get; set; }
        public DelegateCommand ImportResourcesCommand { get; set; }
        public DelegateCommand ExportResourcesCommand { get; set; }

        public DelegateCommand<ResourceWrapper> EnableOpertCommand { get; set; }
        public DelegateCommand<ResourceWrapper> EditResourceCommand { get; set; }
        public DelegateCommand<ResourceWrapper> DeleteResourceCommand { get; set; }


        public ResourceManagerViewModel(ISecurityService securityService,IDialogService dialogService,ILocalizationService localizationService, IEventAggregator ea)
        {
            _securityService = securityService;
            _dialogService = dialogService;
            _ea = ea;
            _ea.GetEvent<PubSubEvent<DialogClosedResult>>().Subscribe(onDialogClosed);
            searchResources();

            _localizationService = localizationService;
            Captions = new ResourceCaptions(_localizationService);
            _localizationService.LanguageChanged += onLanguageChanged;
            translate();
            SearchResourcesCommand = new DelegateCommand(searchResources);
            AddResourceCommand = new DelegateCommand(addResource);
            ImportResourcesCommand = new DelegateCommand(importRoles);
            ExportResourcesCommand = new DelegateCommand(exportRoles);
            EnableOpertCommand = new DelegateCommand<ResourceWrapper>(enableOpert);
            EditResourceCommand = new DelegateCommand<ResourceWrapper>(editResource);
            DeleteResourceCommand = new DelegateCommand<ResourceWrapper>(deleteResource);
    }

        private void onDialogClosed(DialogClosedResult result)
        {
            if (result.ViewName == "AccManagerView")
            {
                Clear();
            }
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Captions.GetContent();
            AddText = _localizationService.Translate(TranslateCommonId.AddId);
            EditText = _localizationService.Translate(TranslateCommonId.EditId);
            DeleteText = _localizationService.Translate(TranslateCommonId.DeleteId);
        }
        private void deleteResource(ResourceWrapper wrapper)
        {
            string caption = _localizationService.Translate(TranslateCommonId.WaringId);
            string message = _localizationService.Translate(TranslateCommonId.DeleteResourceWarningId);
            string errorMessage = _localizationService.Translate(TranslateCommonId.DeleteResourceFailId);

            var btnResult = MessageBox.Show(message, caption, MessageBoxButton.OKCancel);
            if (btnResult == MessageBoxResult.OK)
            {
                if (_securityService.DeleteResource(ResourceWrapper.Convert(wrapper)))
                {
                    ResourceWrappers.Remove(wrapper);
                }
                else
                {
                    MessageBox.Show(errorMessage, caption);
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
            string caption = _localizationService.Translate(TranslateCommonId.WaringId);
            string message = _localizationService.Translate(TranslateCommonId.DisableResourceWarningId);
            string errorMessage1 = _localizationService.Translate(TranslateCommonId.DisableResourceFailId);
            string errorMessage2 = _localizationService.Translate(TranslateCommonId.EnableResourceFailId);
            if (wrapper.Disable)
            {
                var btnResult = MessageBox.Show(message, caption, MessageBoxButton.OKCancel);

                if (btnResult == MessageBoxResult.OK)
                {
                    if (_securityService.UpdateResource(ResourceWrapper.Convert(wrapper)))
                    {
                        wrapper.Disable = true;
                    }
                    else
                    {
                        MessageBox.Show(errorMessage1, caption);
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
                    MessageBox.Show(errorMessage2, caption);
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
        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;
            _ea.GetEvent<PubSubEvent<DialogClosedResult>>().Unsubscribe(onDialogClosed);

        }
    }
}
