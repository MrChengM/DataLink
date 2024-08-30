using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;
using Prism.Unity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;
using GuiBase.Models;
using MaterialDesignThemes.Wpf;
using GuiBase.Services;
using DataServer.Alarm;
namespace GuiBase.ViewModels
{
    public class NavigationListViewModel:BindableBase
    {
        private IRegionManager _regionManager;
        private IEventAggregator _ea;
        private IAlarmService _alarmService;
        public List<NavigationItem> OverViews { get; set; }

        public List<NavigationItem> L1Views { get; set; }

        public List<NavigationItem> L2Views { get; set; }
        public List<NavigationItem> CommandViews { get; set; }
        public List<NavigationItem> OtherViews { get; set; }


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
        private DelegateCommand<string> _navigationToViewCommand;

        public DelegateCommand<string> NavigationToViewCommand
        {
            get {return _navigationToViewCommand; }
            set {
                _navigationToViewCommand = value;
            }
        }
        public NavigationListViewModel(IRegionManager regionManager, IEventAggregator ea, IAlarmService alarmService)
        {
            _regionManager = regionManager;
            _ea = ea;
            _alarmService = alarmService;
            _navigationToViewCommand = new DelegateCommand<string>(_navigationToView);

            initOverViews();
            initL1Views();
            initL2Views();
            initCommandViews();
            initOtherViews();
            getAlarmCounts();
            _alarmService.AlarmRefreshEvent += _alarmService_AlarmRefreshEvent;

        }

        private void _alarmService_AlarmRefreshEvent(AlarmWrapper alarmWrapper, AlarmRefresh alarmRefresh)
        {
            var l1View = L1Views.Find(s => s.ViewName == alarmWrapper.L1View);
            var l2View = L2Views.Find(s => s.ViewName == alarmWrapper.L2View);
            AlarmCountRefresh(l1View, alarmWrapper, alarmRefresh);
            AlarmCountRefresh(l2View, alarmWrapper, alarmRefresh);
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
                                viewItem.Alarm25Counts--;
                                break;
                            case AlarmType.Trivial:
                                viewItem.Alarm50Counts--;
                                break;
                            case AlarmType.Minor:
                                viewItem.Alarm75Counts--;
                                break;
                            case AlarmType.Major:
                                viewItem.Alarm100Counts--;
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
        private void getAlarmCounts(List<NavigationItem> views)
        {
            foreach (var view in views)
            {
                List<AlarmWrapper> alarmList=new List<AlarmWrapper>();
                if (view.ViewType==ViewType.L1View)
                {
                     alarmList = _alarmService.AllExitAlarms.FindAll(s => s.L1View == view.ViewName);
                }
                else if(view.ViewType == ViewType.L2View)
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
        private void initOverViews()
        {
          
        }
        private void initL1Views()
        {
            L1Views = new List<NavigationItem>();

            var l1View = new NavigationItem()
            {
                ViewName = "L1ViewA",
                ViewType = ViewType.L1View,
                SelectedIcon =PackIconKind.ViewArray,
                UnselectedIcon = PackIconKind.ViewArrayOutline,
            };
            L1Views.Add(l1View);
        }
        private void initL2Views()
        {
            L2Views = new List<NavigationItem>();

            var ViewA = new NavigationItem()
            {
                ViewName = "ViewA",
                ViewType = ViewType.L2View,
                SelectedIcon = PackIconKind.ViewArray,
                UnselectedIcon = PackIconKind.ViewArrayOutline
            };
            var ViewB = new NavigationItem()
            {
                ViewName = "ViewB",
                ViewType = ViewType.L2View,
                SelectedIcon = PackIconKind.ViewArray,
                UnselectedIcon = PackIconKind.ViewArrayOutline,
            };
            var ViewC = new NavigationItem()
            {
                ViewName = "ViewC",
                ViewType = ViewType.L2View,
                SelectedIcon = PackIconKind.ViewArray,
                UnselectedIcon = PackIconKind.ViewArrayOutline
            };
            L2Views.Add(ViewA);
            L2Views.Add(ViewB);
            L2Views.Add(ViewC);
        }
        private void initCommandViews()
        {

        }
        private void initOtherViews()
        {

        }
        private void _navigationToView(string type)
        {
            if (type== ViewType.OverView.ToString())
            {

                L1ViewSelectedItem = null;
                L2ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                OtherViewSelectedItem = null;
                navigate(ViewType.OverView, OverViewSelectedItem.ViewName);
            }
            if (type== ViewType.L1View.ToString())
            {

                OverViewSelectedItem = null;
                L2ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                OtherViewSelectedItem = null;
                navigate(ViewType.L1View, L1ViewSelectedItem.ViewName);
            }
            if (type== ViewType.L2View.ToString())
            {
                OverViewSelectedItem = null;
                L1ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                OtherViewSelectedItem = null;
                navigate(ViewType.L2View, L2ViewSelectedItem.ViewName);
            }
            if (type== ViewType.Command.ToString())
            {
                OverViewSelectedItem = null;
                L1ViewSelectedItem = null;
                L2ViewSelectedItem = null;
                OtherViewSelectedItem = null;
                navigate(ViewType.Command, CommandViewSelectedItem.ViewName);
            }
            if (type== ViewType.Other.ToString())
            {
                OverViewSelectedItem = null;
                L1ViewSelectedItem = null;
                L2ViewSelectedItem = null;
                CommandViewSelectedItem = null;
                navigate(ViewType.Command, CommandViewSelectedItem.ViewName);
            }
        }
        private void navigate( ViewType type, string viewName)
        {
            if (viewName != null)
            {
                _regionManager.RequestNavigate("BaseViewRegion", viewName);
                _ea.GetEvent<PubSubEvent<ViewInfor>>().Publish(new ViewInfor() { ViewType = type, ViewName = viewName });
            }
        }
    }
}
