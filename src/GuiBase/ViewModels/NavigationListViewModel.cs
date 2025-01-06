using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;
using GuiBase.Models;
using MaterialDesignThemes.Wpf;
using GuiBase.Services;
using GuiBase.Helper;
using DataServer.Alarm;
using DataServer.Permission;
using System.Linq;
using System;

namespace GuiBase.ViewModels
{
    public class NavigationListViewModel:BindableBase
    {
        private IRegionManager _regionManager;
        private IEventAggregator _ea;
        private IAlarmService _alarmService;
        private ISecurityService _securityService;
        private ILocalizationService _localizationService;
        //private IRegionNavigationJournal _journal;

        private List<ViewInfor> viewInfors;
        public ObservableCollection<NavigationItem> OverViews { get; set; }

        public ObservableCollection<NavigationItem> L1Views { get; set; }

        public ObservableCollection<NavigationItem> L2Views { get; set; }
        public ObservableCollection<NavigationItem> CommandViews { get; set; }
        public ObservableCollection<NavigationItem> OtherViews { get; set; }


        private NavigationItem overViewSelectedItem;

        public NavigationItem OverViewSelectedItem
        {
            get { return overViewSelectedItem; }
            set { SetProperty(ref overViewSelectedItem, value, "OverViewSelectedItem"); }
        }
        private NavigationItem l1ViewSelectedItem;

        public NavigationItem L1ViewSelectedItem
        {
            get { return l1ViewSelectedItem; }
            set { SetProperty(ref l1ViewSelectedItem, value, "L1ViewSelectedItem"); }
        }
        private NavigationItem l2ViewSelectedItem;

        public NavigationItem L2ViewSelectedItem
        {
            get { return l2ViewSelectedItem; }
            set { SetProperty(ref l2ViewSelectedItem, value, "L2ViewSelectedItem"); }
        }
        private NavigationItem commandViewSelectedItem;

        public NavigationItem CommandViewSelectedItem
        {
            get { return commandViewSelectedItem; }
            set { SetProperty(ref commandViewSelectedItem, value, "CommandViewSelectedItem"); }
        }
        private NavigationItem otherViewSelectedItem;

        public NavigationItem OtherViewSelectedItem
        {
            get { return otherViewSelectedItem; }
            set { SetProperty(ref otherViewSelectedItem, value, "OtherViewSelectedItem"); }
        }

        private string overViewText;

        public string OverViewText
        {
            get { return overViewText; }
            set { SetProperty(ref overViewText, value, "OverViewText"); }
        }


        private bool overViewEnable;

        public bool OverViewEnable
        {
            get { return overViewEnable; }
            set { SetProperty(ref overViewEnable, value, "OverViewEnable"); }
        }

        private string l1ViewText;

        public string L1ViewText
        {
            get { return l1ViewText; }
            set { SetProperty(ref l1ViewText, value, "L1ViewText"); }
        }


        private bool l1ViewEnable;

        public bool L1ViewEnable
        {
            get { return l1ViewEnable; }
            set { SetProperty(ref l1ViewEnable, value, "L1ViewEnable"); }
        }

        private string l2ViewText;

        public string L2ViewText
        {
            get { return l2ViewText; }
            set { SetProperty(ref l2ViewText, value, "L2ViewText"); }
        }
        private bool l2ViewEnable;
        public bool L2ViewEnable
        {
            get { return l2ViewEnable; }
            set { SetProperty(ref l2ViewEnable, value, "L2ViewEnable"); }
        }

        private string commandText;

        public string CommandText
        {
            get { return commandText; }
            set { SetProperty(ref commandText, value, "CommandText"); }
        }
        private bool commandEnable;
        public bool CommandEnable
        {
            get { return commandEnable; }
            set { SetProperty(ref commandEnable, value, "CommandEnable"); }
        }

        private string otherText;

        public string OtherText
        {
            get { return otherText; }
            set { SetProperty(ref otherText, value, "OtherText"); }
        }
        private bool otherEnable;
        public bool OtherEnable
        {
            get { return otherEnable; }
            set { SetProperty(ref otherEnable, value, "OtherEnable"); }
        }
        private string goBackText;

        public string GoBackText
        {
            get { return goBackText; }
            set { SetProperty(ref goBackText, value, "GoBackText"); }
        }

        private string goForwardText;

        public string GoForwardText
        {
            get { return goForwardText; }
            set { SetProperty(ref goForwardText, value, "GoForwardText"); }
        }


        public DelegateCommand<ViewType?> NavigationToViewCommand { get; set; }

        public DelegateCommand GoForwardCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }

        public NavigationListViewModel(IRegionManager regionManager,  IEventAggregator ea, IAlarmService alarmService, ISecurityService securityService,ILocalizationService localizationService)
        {
            _regionManager = regionManager;
            //_journal = regionNavigationService.Journal;
            _ea = ea; 
            _ea.GetEvent<PubSubEvent<ViewInfor>>().Subscribe(refresh);

            _alarmService = alarmService;
            NavigationToViewCommand = new DelegateCommand<ViewType?>(_navigationToView);
            GoForwardCommand = new DelegateCommand(GoForward, CanGoForward);
            GoBackCommand = new DelegateCommand(GoBack, CanGoBack);
            _securityService = securityService;

            viewInfors = new List<ViewInfor>();
            OverViews = new ObservableCollection<NavigationItem>();
            L1Views = new ObservableCollection<NavigationItem>();
            L2Views = new ObservableCollection<NavigationItem>();
            CommandViews = new ObservableCollection<NavigationItem>();
            OtherViews = new ObservableCollection<NavigationItem>();

          

            _alarmService.AlarmRefreshEvent += _alarmService_AlarmRefreshEvent;
            _alarmService.ConnectStatusChangeEvent += onConnectStatusChanged;
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            _securityService.UserChangeEvent += _securityService_UserChangeEvent;
            _securityService_UserChangeEvent(_securityService.GetCurrentUser());
        }

        private void onConnectStatusChanged(bool isConnected)
        {
            getAlarmCounts();
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            OverViewText = _localizationService.Translate(TranslateCommonId.OverViewId);
            L1ViewText = _localizationService.Translate(TranslateCommonId.L1ViewId);
            L2ViewText = _localizationService.Translate(TranslateCommonId.L2ViewId);
            CommandText = _localizationService.Translate(TranslateCommonId.CommandId);
            OtherText = _localizationService.Translate(TranslateCommonId.OtherId);
            GoBackText = _localizationService.Translate(TranslateCommonId.GoBackId);
            GoForwardText = _localizationService.Translate(TranslateCommonId.GoForwardId);
            foreach (var item in OverViews)
            {
                item.Title = _localizationService.Translate(item.TitelId);
            }
            foreach (var item in L1Views)
            {
                item.Title = _localizationService.Translate(item.TitelId);
            }
            foreach (var item in L2Views)
            {
                item.Title = _localizationService.Translate(item.TitelId);
            }
            foreach (var item in CommandViews)
            {
                item.Title = _localizationService.Translate(item.TitelId);
            }
            foreach (var item in OtherViews)
            {
                item.Title = _localizationService.Translate(item.TitelId);
            }
        }
        private bool CanGoBack()
        {
            return ViewOperateHelper.BaseCanBack();
        }

        private void GoBack()
        {
            ViewOperateHelper.BaseNavigationGoBack();
        }

        private bool CanGoForward()
        {

            return  ViewOperateHelper.BaseCanForward();
        }

        private void GoForward()
        {
            ViewOperateHelper.BaseNavigationGoForward();
        }

        private void raiseCanExecuteChanged()
        {
            GoBackCommand.RaiseCanExecuteChanged();
            GoForwardCommand.RaiseCanExecuteChanged();
        }

        private bool goButtonDisplay;

        public bool GoButtonDisplay
        {
            get { return goButtonDisplay; }
            set { SetProperty(ref goButtonDisplay, value, "GoButtonDisplay"); }
        }


        private void _securityService_UserChangeEvent(User user)
        {
            initViewLists();
            translate();
            getAlarmCounts();
            if (user == null)
            {
                ViewOperateHelper.BaseJournalClear();
                GoButtonDisplay = false;
            }
            else
            {
                GoButtonDisplay = true;
            }
        }

        private void _alarmService_AlarmRefreshEvent(AlarmWrapper alarmWrapper, AlarmRefresh alarmRefresh)
        {
            var l1View = L1Views.Where(s => s.ViewName == alarmWrapper.L1View);
            var l2View = L2Views.Where(s => s.ViewName == alarmWrapper.L2View);
            AlarmCountRefresh(l1View.FirstOrDefault(), alarmWrapper, alarmRefresh);
            AlarmCountRefresh(l2View.FirstOrDefault(), alarmWrapper, alarmRefresh);
        }
        private void AlarmCountRefresh(NavigationItem viewItem, AlarmWrapper alarmWrapper,AlarmRefresh alarmRefresh)
        {
            if (viewItem != null)
            {
                switch (alarmRefresh)
                {
                    case AlarmRefresh.Add:
                        switch (alarmWrapper.AlarmLevel)
                        {
                            case AlarmType.Information:
                                viewItem.Alarm25Counts++;
                                break;
                            case AlarmType.Trivial:
                                viewItem.Alarm50Counts++;
                                break;
                            case AlarmType.Minor:
                                viewItem.Alarm75Counts++;
                                break;
                            case AlarmType.Major:
                                viewItem.Alarm100Counts++;
                                break;
                            default:
                                break;
                        }
                        break;
                    case AlarmRefresh.Updata:
                        break;
                    case AlarmRefresh.Remove:
                        switch (alarmWrapper.AlarmLevel)
                        {
                            case AlarmType.Information:
                                if (viewItem.Alarm25Counts > 0)
                                {
                                    viewItem.Alarm25Counts--;
                                }
                                break;
                            case AlarmType.Trivial:
                                if (viewItem.Alarm50Counts > 0)
                                {
                                    viewItem.Alarm50Counts--;
                                }

                                break;
                            case AlarmType.Minor:
                                if (viewItem.Alarm75Counts > 0)
                                {
                                    viewItem.Alarm75Counts--;
                                }
                                break;
                            case AlarmType.Major:
                                if (viewItem.Alarm100Counts > 0)
                                {
                                    viewItem.Alarm100Counts--;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void getAlarmCounts()
        {
            getAlarmCounts(L1Views);
            getAlarmCounts(L2Views);

        }
        private void getAlarmCounts(ObservableCollection<NavigationItem> views)
        {
            foreach (var view in views)
            {
                List<AlarmWrapper> alarmList = new List<AlarmWrapper>();
                if (view.ViewType == ViewType.Base_L1View)
                {
                    alarmList = _alarmService.AllExitAlarms.FindAll(s => s.L1View == view.ViewName);
                }
                else if (view.ViewType == ViewType.Base_L2View)
                {
                    alarmList = _alarmService.AllExitAlarms.FindAll(s => s.L2View == view.ViewName);

                }
                if (alarmList == null || alarmList.Count == 0)
                {
                    view.Alarm25Counts = 0;
                    view.Alarm50Counts = 0;
                    view.Alarm75Counts = 0;
                    view.Alarm100Counts = 0;
                }
                else
                {
                    view.Alarm25Counts = alarmList.FindAll(s => s.AlarmLevel == AlarmType.Information).Count;
                    view.Alarm50Counts = alarmList.FindAll(s => s.AlarmLevel == AlarmType.Trivial).Count;
                    view.Alarm75Counts = alarmList.FindAll(s => s.AlarmLevel == AlarmType.Minor).Count;
                    view.Alarm100Counts = alarmList.FindAll(s => s.AlarmLevel == AlarmType.Major).Count;
                }
            }
        }
        private void initViewLists()
        {
            viewInfors.Clear();
            if (OverViewEnable = _securityService.HasPermission("OverView", ResourceType.ViewGourp))
            {
                initOverViews();
            }
            if (L1ViewEnable = _securityService.HasPermission("L1View", ResourceType.ViewGourp))
            {
                initL1Views();
            }
            if (L2ViewEnable = _securityService.HasPermission("L2View", ResourceType.ViewGourp))
            {
                initL2Views();
            }
            if (CommandEnable = _securityService.HasPermission("Command", ResourceType.ViewGourp))
            {
                initCommandViews();
            }
            if (OtherEnable = _securityService.HasPermission("Other", ResourceType.ViewGourp))
            {
                initOtherViews();
            }
            ViewOperateHelper.BaseViewInfors = viewInfors;
        }
        public virtual void  initOverViews()
        {
            OverViews.Clear();
        }
        public virtual void initL1Views()
        {
            L1Views.Clear();
            if (_securityService.HasPermission("L1ViewA",ResourceType.View))
            {
                var l1View = new NavigationItem()
                {
                    TitelId="G_Text_L1ViewA",
                    ViewName = "L1ViewA",
                    ViewType = ViewType.Base_L1View,
                    SelectedIcon = PackIconKind.ViewArray,
                    UnselectedIcon = PackIconKind.ViewArrayOutline,
                };
                L1Views.Add(l1View);
                viewInfors.Add(new ViewInfor() { ViewName = l1View.ViewName, ViewType = ViewType.Base_L1View });
            }
            
        }
        public virtual void initL2Views()
        {
            L2Views.Clear();
            if (_securityService.HasPermission("ViewA", ResourceType.View))
            {
                var ViewA = new NavigationItem()
                {
                    TitelId = "G_Text_ViewA",
                    ViewName = "ViewA",
                    ViewType = ViewType.Base_L2View,
                    SelectedIcon = PackIconKind.ViewArray,
                    UnselectedIcon = PackIconKind.ViewArrayOutline
                };
                L2Views.Add(ViewA);
                viewInfors.Add(new ViewInfor() { ViewName = ViewA.ViewName, ViewType = ViewType.Base_L2View });

            }
            if (_securityService.HasPermission("ViewB", ResourceType.View))
            {
                var ViewB = new NavigationItem()
                {
                    TitelId = "G_Text_ViewB",
                    ViewName = "ViewB",
                    ViewType = ViewType.Base_L2View,
                    SelectedIcon = PackIconKind.ViewArray,
                    UnselectedIcon = PackIconKind.ViewArrayOutline,
                };
                L2Views.Add(ViewB);
                viewInfors.Add(new ViewInfor() { ViewName = ViewB.ViewName, ViewType = ViewType.Base_L2View });

            }
            if (_securityService.HasPermission("ViewC", ResourceType.View))
            {
                var ViewC = new NavigationItem()
                {
                    TitelId = "G_Text_ViewC",
                    ViewName = "ViewC",
                    ViewType = ViewType.Base_L2View,
                    SelectedIcon = PackIconKind.ViewArray,
                    UnselectedIcon = PackIconKind.ViewArrayOutline
                };
                L2Views.Add(ViewC);
                viewInfors.Add(new ViewInfor() { ViewName = ViewC.ViewName, ViewType = ViewType.Base_L2View });
            }
        }
        public virtual void initCommandViews()
        {
            CommandViews.Clear();
        }
        public virtual void initOtherViews()
        {
            OtherViews.Clear();

        }
        public virtual void _navigationToView(ViewType? type)
        {
            if (type == null)
            {
                return;
            }
            if (type == ViewType.Base_OverView)
            {

                L1ViewSelectedItem = null;
                L2ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                OtherViewSelectedItem = null;
                ViewOperateHelper.NavigationBaseRegion(new ViewInfor() { ViewType = ViewType.Base_OverView, ViewName = OverViewSelectedItem.ViewName });
            }
            if (type== ViewType.Base_L1View)
            {

                OverViewSelectedItem = null;
                L2ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                OtherViewSelectedItem = null;
                ViewOperateHelper.NavigationBaseRegion(new ViewInfor() { ViewType = ViewType.Base_L1View, ViewName = L1ViewSelectedItem.ViewName });

            }
            if (type== ViewType.Base_L2View)
            {
                OverViewSelectedItem = null;
                L1ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                OtherViewSelectedItem = null;
                ViewOperateHelper.NavigationBaseRegion(new ViewInfor() { ViewType = ViewType.Base_L2View, ViewName = L2ViewSelectedItem.ViewName });

            }
            if (type== ViewType.Base_Command)
            {
                OverViewSelectedItem = null;
                L1ViewSelectedItem = null;
                L2ViewSelectedItem = null;
                OtherViewSelectedItem = null;
                ViewOperateHelper.NavigationBaseRegion(new ViewInfor() { ViewType = ViewType.Base_Command, ViewName = CommandViewSelectedItem.ViewName });
            }
            if (type== ViewType.Base_Other)
            {
                OverViewSelectedItem = null;
                L1ViewSelectedItem = null;
                L2ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                ViewOperateHelper.NavigationBaseRegion(new ViewInfor() { ViewType = ViewType.Base_OverView, ViewName = OtherViewSelectedItem.ViewName });
            }
        }
        private void refresh(ViewInfor infor)
        {
            refreshSelectedItem(infor);
            raiseCanExecuteChanged();
        }
        private void refreshSelectedItem(ViewInfor infor)
        {
            switch (infor.ViewType)
            {
                case ViewType.Base_OverView:
                    L1ViewSelectedItem = null;
                    L2ViewSelectedItem = null;
                    CommandViewSelectedItem = null;
                    OtherViewSelectedItem = null;
                    var overItem = OverViews.Where(s => s.ViewName == infor.ViewName).ToList();
                    if (overItem != null && overItem.Count > 0)
                    {
                        OverViewSelectedItem = overItem[0];
                    }
                    break;
                case ViewType.Base_L1View:
                    //L1ViewSelectedItem = null;
                    L2ViewSelectedItem = null;
                    CommandViewSelectedItem = null;
                    OtherViewSelectedItem = null;
                    OverViewSelectedItem = null;
                    var l1Item = L1Views.Where(s => s.ViewName == infor.ViewName).ToList();
                    if (l1Item != null && l1Item.Count > 0)
                    {
                        L1ViewSelectedItem = l1Item[0];
                    }
                    break;
                case ViewType.Base_L2View:
                    L1ViewSelectedItem = null;
                    //L2ViewSelectedItem = null;
                    CommandViewSelectedItem = null;
                    OtherViewSelectedItem = null;
                    OverViewSelectedItem = null;
                    var l2Item = L2Views.Where(s => s.ViewName == infor.ViewName).ToList();
                    if (l2Item != null && l2Item.Count > 0)
                    {
                        L2ViewSelectedItem = l2Item[0];
                    }
                    break;
                case ViewType.Base_Command:
                    L1ViewSelectedItem = null;
                    L2ViewSelectedItem = null;
                    //CommandViewSelectedItem = null;
                    OtherViewSelectedItem = null;
                    OverViewSelectedItem = null;
                    var comandItem = CommandViews.Where(s => s.ViewName == infor.ViewName).ToList();
                    if (comandItem != null && comandItem.Count > 0)
                    {
                        CommandViewSelectedItem = comandItem[0];
                    }
                    break;
                case ViewType.Base_Other:
                    L1ViewSelectedItem = null;
                    L2ViewSelectedItem = null;
                    CommandViewSelectedItem = null;
                    //OtherViewSelectedItem = null;
                    OverViewSelectedItem = null;
                    var otherItem = OtherViews.Where(s => s.ViewName == infor.ViewName).ToList();
                    if (otherItem != null && otherItem.Count > 0)
                    {
                        OtherViewSelectedItem = otherItem[0];
                    }
                    break;
                case ViewType.Sub_ACC:
                    break;
                default:
                    break;
            }
        }
        //private void navigate( ViewType type, string viewName)
        //{
        //    if (viewName != null)
        //    {
        //        _regionManager.RequestNavigate("BaseViewRegion", viewName);
        //        _ea.GetEvent<PubSubEvent<ViewInfor>>().Publish(new ViewInfor() { ViewType = type, ViewName = viewName });
        //    }
        //}
        public void Clear()
        {
            OverViews.Clear();
            L1Views.Clear();
            L2Views.Clear();
            CommandViews.Clear();
            OtherViews.Clear();
            viewInfors.Clear();

            _alarmService.AlarmRefreshEvent -= _alarmService_AlarmRefreshEvent;
            _alarmService.ConnectStatusChangeEvent -= onConnectStatusChanged;
            _localizationService.LanguageChanged -= onLanguageChanged;
            _securityService.UserChangeEvent -= _securityService_UserChangeEvent;
           
        }
    }
}
