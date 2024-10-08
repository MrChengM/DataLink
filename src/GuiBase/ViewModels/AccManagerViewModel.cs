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
        public string Title => "System Managerment";

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
        private NavigationItem selectedItem;

        public NavigationItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem"); }
        }
        public AccManagerViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            initNavigationViews();
            _navigationToViewCommand = new DelegateCommand(navigationToView);
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
                Title = "User Manager",
                SelectedIcon = PackIconKind.Account,
                UnselectedIcon = PackIconKind.AccountOutline,
                ViewName = "UserManagerView",

            });
            NavigationViews.Add(new NavigationItem()
            {
                Title = "Role Manager",
                SelectedIcon = PackIconKind.AccountMultiple,
                UnselectedIcon = PackIconKind.AccountMultipleOutline,
                ViewName = "RoleManagerView",

            });
            NavigationViews.Add(new NavigationItem()
            {
                Title = "Resource Manager",
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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
