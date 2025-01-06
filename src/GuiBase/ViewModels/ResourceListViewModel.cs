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
    public class ResourceListViewModel : BindableBase,IDialogAware
    {
        public ISecurityService _securityService;
        public ILocalizationService _localizationService;
        public ObservableCollection<Resource> Resources { get; set; }

        public DelegateCommand<string> ConfrimBtnCommand { get; set; }

        private Resource selectResource;

        public Resource SelectResource
        {
            get { return selectResource; }
            set { SetProperty(ref selectResource, value, "SelectResource"); }
        }
        private ResourceCaptions captions;

        public ResourceCaptions Captions
        {
            get { return captions; }
            set { SetProperty(ref captions, value, "Captions"); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }


        public event Action<IDialogResult> RequestClose;

        public ResourceListViewModel(ISecurityService securityService,ILocalizationService localizationService)
        {
            _securityService = securityService;
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            Resources = new ObservableCollection<Resource>();
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Captions = new ResourceCaptions(_localizationService);
            translate();
            Resources.AddRange(_securityService.GetAllResources());
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Captions.GetContent();
            Title = _localizationService.Translate(TranslateCommonId.ResourceListId);
        }
        private void confrimBtn(string param)
        {
            var dialogParam = new DialogParameters();
            var btnResult = new ButtonResult();
            if (param == "OK")
            {
                if (SelectResource != null)
                {
                    dialogParam.Add("Resource", SelectResource);
                    btnResult = ButtonResult.OK;
                }
                else
                {
                    MessageBox.Show("请选择资源！");
                }
            }
            else if (param == "Cancel")
            {
                btnResult = ButtonResult.Cancel;
            }
            RequestClose?.Invoke(new DialogResult(btnResult, dialogParam));
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
        }

        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;
            Resources.Clear();
        }
    }
}
