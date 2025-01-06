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
using System.Collections.ObjectModel;
using DataServer.Permission;

namespace GuiBase.ViewModels
{
    public class HeaderViewModel:BindableBase
    {
        private IEventAggregator _ea;
        private IDialogService _dialogService;
        private ISecurityService _securityService;
        private IContainerProvider _container;
        private ILocalizationService _localizationService;
        public ObservableCollection<NavigationItem> DialogList { get; set; }

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

        private string languageSelect;

        public string LanguageSelect
        {
            get { return languageSelect; }
            set { SetProperty(ref languageSelect, value, "LanguageSelect"); }
        }

        private string language;

        public string Language
        {
            get { return language; }
            set { SetProperty(ref language, value, onLanguageNameChanged, "Language"); }
        }

        private void onLanguageNameChanged()
        {
            if (Language=="中文简体")
            {
                _localizationService.Language = "zh";
            }
            else if(Language == "English")
            {
                _localizationService.Language = "en";
            }
        }

        
        public HeaderViewModel(IContainerProvider container ,IDialogService dialogService,ISecurityService securityService,IEventAggregator ea, ILocalizationService localizationService)
        {
            _container = container;
            _dialogService = dialogService;
            _securityService = securityService;
            _ea = ea;
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;

            Language = "中文简体";
            DialogList = new ObservableCollection<NavigationItem>();
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
            translate();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }

        private void translate()
        {
            LanguageSelect = _localizationService.Translate(TranslateCommonId.LanguageSelectId);
            foreach (var nm in DialogList)
            {
                nm.Title = _localizationService.Translate(nm.TitelId);
            }
        }
        private void UserClick()
        {
            _dialogService.ShowDialog("UserDetailedView");
        }

        private void _securityService_UserChangeEvent(User user)
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
            initNavigationItems();
            translate();
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
            if (SelectedItem != null)
            {
                if (SelectedItem.TitelId == TranslateCommonId.ExitId)
                {
                    var alarmService = _container.Resolve<IAlarmService>();
                    alarmService.Stop();
                    var signalServer = _container.Resolve<ISignalService>();
                    signalServer.Stop();

                    App.Current.Shutdown();

                }
                else if (SelectedItem.TitelId ==TranslateCommonId.LogonId)
                {
                    //var startWindow = _container.Resolve<StartUpWindow>((typeof(string),"LogOnView"));
                    //var regionManager = _container.Resolve<IRegionManager>();
                    //RegionManager.SetRegionManager(startWindow, regionManager);
                    //RegionManager.UpdateRegions();
                    //startWindow.Show();
                    //var windows = App.Current.Windows;
                    //foreach (var window in windows)
                    //{
                    //    if (window is MainWindow)
                    //    {
                    //        MainWindow mw = window as MainWindow;
                    //        mw.Close();
                    //        regionManager.Regions.Remove("MenuListRegion");
                    //        regionManager.Regions.Remove("NavigtionRegion");
                    //        regionManager.Regions.Remove("HeaderRegion");
                    //        regionManager.Regions.Remove("AlarmViewRegion");
                    //        regionManager.Regions.Remove("BaseViewRegion");

                    //    }
                    //}
                    _dialogService.ShowDialog(selectedItem.ViewName, s => SelectedItem = null);

                }
                else
                {
                    _dialogService.Show(selectedItem.ViewName, s => SelectedItem = null);
                }

            }
        }

        void initNavigationItems()
        {
            DialogList.Clear();
            DialogList.Add(new NavigationItem()
            {
                TitelId = TranslateCommonId.LogonId,
                SelectedIcon = PackIconKind.Account,
                UnselectedIcon = PackIconKind.AccountOutline,
                ViewName = "LogOnView",
            }); ;
            if (_securityService.HasPermission("AlarmView",ResourceType.View))
            {
                DialogList.Add(new NavigationItem()
                {
                    TitelId = TranslateCommonId.AlarmId,
                    SelectedIcon = PackIconKind.AlarmLight,
                    UnselectedIcon = PackIconKind.AlarmLightOutline,
                    ViewName = "AlarmView",

                });
            }
            if (_securityService.HasPermission("HistoryAlarmView", ResourceType.View))
            {
                DialogList.Add(new NavigationItem()
                {
                    TitelId = TranslateCommonId.HistoryAlarmId,
                    SelectedIcon = PackIconKind.AlarmPanel,
                    UnselectedIcon = PackIconKind.AlarmPanelOutline,
                    ViewName = "HistoryAlarmView",

                });
            }
            if (_securityService.HasPermission("OperRecordView", ResourceType.View))
            {
                DialogList.Add(new NavigationItem()
                {
                    TitelId = TranslateCommonId.OperRecordId,
                    SelectedIcon = PackIconKind.BookOpen,
                    UnselectedIcon = PackIconKind.BookOpenOutline,
                    ViewName = "OperRecordView",

                });
            }
            DialogList.Add(new NavigationItem()
            {
                TitelId = TranslateCommonId.ExitId,
                SelectedIcon = PackIconKind.Power,
                UnselectedIcon = PackIconKind.Power,
                ViewName = "",

            });
           
        }
        public void Clear()
        {
            DialogList.Clear();
            _localizationService.LanguageChanged -= onLanguageChanged;
            _securityService.UserChangeEvent -= _securityService_UserChangeEvent;

        }
    }
}
