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
    public class ResourceNameListViewModel : BindableBase,IDialogAware
    {
        public ISecurityService _securityService;
        public ILocalizationService _localizationService;

        public ObservableCollection<string> ResourceNames { get; set; }

        public DelegateCommand<string> ConfrimBtnCommand { get; set; }

        private string selectName;

        public string SelectName
        {
            get { return selectName; }
            set { SetProperty(ref selectName, value, "SelectName"); }
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

        public ResourceNameListViewModel(ISecurityService securityService,ILocalizationService localizationService)
        {
            _securityService = securityService;
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Captions = new ResourceCaptions(_localizationService);
            translate();
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            initList();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.ResourceNameListId);
            Captions.GetContent();

        }
        void initList()
        {
            ResourceNames = new ObservableCollection<string>();
            var allResourceNames = _securityService.GetResourceNames();
            var resources = _securityService.GetAllResources();
            if (resources == null)
            {
                ResourceNames.AddRange(allResourceNames);
                return;
            }
            foreach (var name in allResourceNames)
            {
                if (resources.Find(s => s.Name == name) == null)
                {
                    ResourceNames.Add(name);
                }
            }
        }


        private void confrimBtn(string param)
        {
            var message = _localizationService.Translate(TranslateCommonId.ResourceNameSetErrorId);
            var dialogParam = new DialogParameters();
            var btnResult = new ButtonResult();
            if (param == "OK")
            {
                if (SelectName != null)
                {
                    dialogParam.Add("ResourceName", SelectName);
                    btnResult = ButtonResult.OK;
                }
                else
                {
                    MessageBox.Show(message);
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
            ResourceNames.Clear();
        }
    }
}
