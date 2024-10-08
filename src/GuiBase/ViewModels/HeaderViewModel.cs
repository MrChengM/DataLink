using System;
using System.Collections.Generic;
using System.Text;
using GuiBase.ViewModels;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Commands;
using GuiBase.Models;
using GuiBase.Services;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Threading.Tasks;
using Prism.Ioc;
using GuiBase.Views;
using Prism.Events;
using Prism.Regions;

namespace GuiBase.ViewModels
{
    public class HeaderViewModel:BindableBase
    {
        private IEventAggregator _ea;
        private IDialogService _dialogService;
        private ISecurityService _securityService;
        private IContainerProvider _container;
        public List<NavigationItem> DialogList { get; set; }

        private NavigationItem selectedItem;

        //private Timer timer;
        public DelegateCommand DialogClickCommand { get; set; }
        public DelegateCommand UserClickCommand { get; set; }

        public NavigationItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem"); }
        }

        
        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value, "SelectedIndex"); }
        }


        private string currentTime;

        public string CurrentTime
        {
            get { return currentTime; }
            set { SetProperty(ref currentTime, value, "CurrentTime"); }
        }
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value, "UserName"); }
        }
        private bool userBtnEnable;

        public bool UserBtnEnable
        {
            get { return userBtnEnable; }
            set { SetProperty(ref userBtnEnable, value, "UserBtnEnable"); }
        }

        public HeaderViewModel(IContainerProvider container ,IDialogService dialogService,ISecurityService securityService,IEventAggregator ea)
        {
            _container = container;
            _dialogService = dialogService;
            _securityService = securityService;
            _ea = ea;
            _securityService.UserChangeEvent += _securityService_UserChangeEvent;
            initNavigationItems();
            DialogClickCommand = new DelegateCommand(DialogClick);
            UserClickCommand = new DelegateCommand(UserClick);
            var task = new TaskFactory().StartNew(refresh);
            if (securityService.GetCurrentUser()!=null)
            {
                UserName = securityService.GetCurrentUser().Account;
                UserBtnEnable = true;
            }
        }

        private void UserClick()
        {
            _dialogService.ShowDialog("UserDetailedView");
        }

        private void _securityService_UserChangeEvent(DataServer.Permission.User user)
        {
            if (user!=null)
            {
                UserName = user.Account;
                UserBtnEnable = true;
            }
            else
            {
                UserName = null;
                UserBtnEnable = false;
            }
        }

        private void refresh()
        {
            while (true)
            {
                CurrentTime = DateTime.Now.ToString("G"); //"yy/mm/dd hh:mm:ss"
            }
        }

        private void DialogClick()
        {
            if (SelectedItem!=null)
            {
                if (SelectedItem.Title == "Eixt")
                {
                    App.Current.Shutdown();

                }
                
                else if (SelectedItem.Title == "LogOff")
                {
                    var startWindow = _container.Resolve<StartUpWindow>((typeof(string),"LogOnView"));
                    var regionManager = _container.Resolve<IRegionManager>();
                    RegionManager.SetRegionManager(startWindow, regionManager);
                    RegionManager.UpdateRegions();
                    startWindow.Show();
                    var windows = App.Current.Windows;
                    foreach (var window in windows)
                    {
                        if (window is MainWindow)
                        {
                            MainWindow mw = window as MainWindow;
                            mw.Close();
                            regionManager.Regions.Remove("MenuListRegion");
                            regionManager.Regions.Remove("NavigtionRegion");
                            regionManager.Regions.Remove("HeaderRegion");
                            regionManager.Regions.Remove("AlarmViewRegion");
                            regionManager.Regions.Remove("BaseViewRegion");

                        }
                    }
                    //_dialogService.ShowDialog(selectedItem.ViewName, s => SelectedItem = null);

                }
                else
                {
                    _dialogService.Show(selectedItem.ViewName, s => SelectedItem = null);
                }

            }
        }

        void initNavigationItems()
        {
            DialogList = new List<NavigationItem>();
            DialogList.Add(new NavigationItem()
            {
                Title = "LogOff",
                SelectedIcon = PackIconKind.AccountOff,
                UnselectedIcon = PackIconKind.AccountOffOutline,
                ViewName = "LogOnView",

            });
            DialogList.Add(new NavigationItem()
            {
                Title = "Alarm",
                SelectedIcon = PackIconKind.AlarmLight,
                UnselectedIcon = PackIconKind.AlarmLightOutline,
                ViewName = "AlarmView",

            });
            DialogList.Add(new NavigationItem()
            {
                Title = "HistoryAlarm",
                SelectedIcon = PackIconKind.AlarmPanel,
                UnselectedIcon = PackIconKind.AlarmPanelOutline,
                ViewName = "HistoryAlarmView",

            });
            DialogList.Add(new NavigationItem()
            {
                Title = "OperRecord",
                SelectedIcon = PackIconKind.BookOpen,
                UnselectedIcon = PackIconKind.BookOpenOutline,
                ViewName = "OperRecordView",

            });
            //NavigationList.Add(new NavigationItem()
            //{
            //    Title = "Legend",
            //    SelectedIcon = PackIconKind.Motion,
            //    UnselectedIcon = PackIconKind.MotionOutline,
            //    NavigtionViewName = "",

            //});
            DialogList.Add(new NavigationItem()
            {
                Title = "Eixt",
                SelectedIcon = PackIconKind.Power,
                UnselectedIcon = PackIconKind.Power,
                ViewName = "",

            });
           
        }
    }
}
