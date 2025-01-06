using Prism.Mvvm;
using System;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;
using System.Collections.Generic;
using MaterialDesignThemes.Wpf;
using Prism.Regions;

namespace GuiBase.ViewModels
{
    public class AccManagerViewModel : BindableBase, IDialogAware
    {
        private IRegionManager _regionManager;
        private ILocalizationService _localizationService;
        private IEventAggregator _ea;

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }


        public event Action<IDialogResult> RequestClose;
        private DelegateCommand _navigationToViewCommand;
        public List<NavigationItem> NavigationViews { get; set; }

        public DelegateCommand NavigationToViewCommand
        {
            get { return _navigationToViewCommand; }
            set
            {
                _navigationToViewCommand = value;
            }
        }

        private string systemMangagementText;

        public string SystemMangagementText
        {
            get { return systemMangagementText; }
            set { SetProperty(ref systemMangagementText, value, "SystemMangagementText"); }
        }


        private NavigationItem selectedItem;

        public NavigationItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem"); }
        }
        public AccManagerViewModel(IRegionManager regionManager,ILocalizationService localizationService, IEventAggregator ea)
        {
            _regionManager = regionManager;
            _localizationService = localizationService;
            _ea = ea;

            _localizationService.LanguageChanged += onLanguageChanged;
            initNavigationViews();
            translate();
            _navigationToViewCommand = new DelegateCommand(navigationToView);
        }
        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }

        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.AccManagerId);
            SystemMangagementText = _localizationService.Translate(TranslateCommonId.SystemManagementId);
            foreach (var nm in NavigationViews)
            {
                nm.Title = _localizationService.Translate(nm.TitelId);
            }
        }
        private void navigationToView()
        {
            var viewName = SelectedItem.ViewName;
            _regionManager.RequestNavigate("ManagerViewRegion", viewName);

        }
        void initNavigationViews()
        {
            NavigationViews = new List<NavigationItem>();
            NavigationViews.Add(new NavigationItem()
            {
                TitelId = TranslateCommonId.UserManagementId,
                SelectedIcon = PackIconKind.Account,
                UnselectedIcon = PackIconKind.AccountOutline,
                ViewName = "UserManagerView",

            });
            NavigationViews.Add(new NavigationItem()
            {
                TitelId = TranslateCommonId.RoleManagementId,
                SelectedIcon = PackIconKind.AccountMultiple,
                UnselectedIcon = PackIconKind.AccountMultipleOutline,
                ViewName = "RoleManagerView",

            });
            NavigationViews.Add(new NavigationItem()
            {
                TitelId = TranslateCommonId.ResourceManagementId,
                SelectedIcon = PackIconKind.Ballot,
                UnselectedIcon = PackIconKind.BallotOutline,
                ViewName = "ResourceManagerView",

            });
        }
        public bool CanCloseDialog()
        {
            return  true;
        }

        public void OnDialogClosed()
        {
            _ea.GetEvent<PubSubEvent<DialogClosedResult>>().Publish(new DialogClosedResult() { ViewName = "AccManagerView", IsClosed = true });
            Clear();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;
            NavigationViews.Clear();
        }
    }
}
