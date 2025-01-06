using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Events;
using GuiBase.Models;
namespace GuiBase.Helper
{
    public static class ViewOperateHelper
    {
        public static List<ViewInfor> BaseViewInfors { get; set; }
        private static IRegionNavigationJournal baseJournal;

        public static void NavigationBaseRegion(string viewName)
        {
            var viewInfro = BaseViewInfors.Find(s => s.ViewName == viewName);
            NavigationBaseRegion(viewInfro);
        }
        public static void NavigationBaseRegion(ViewInfor infor)
        {
            var regionManager = ContainerLocator.Container.Resolve<IRegionManager>();
            var ea = ContainerLocator.Container.Resolve<IEventAggregator>();
            regionManager.RequestNavigate("BaseViewRegion", infor.ViewName, s =>
            {
                if (s.Result == true)
                {
                    ea.GetEvent<PubSubEvent<ViewInfor>>().Publish(infor);
                    baseJournal = s.Context.NavigationService.Journal;
                }
            });
        }
       
        public static void Navigation(string regionName, string viewName)
        {
            var regionManager = ContainerLocator.Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate(regionName, viewName);
        }
        public static bool BaseCanForward()
        {
            return baseJournal != null && baseJournal.CanGoForward;
        }
        public static bool BaseCanBack()
        {
            return baseJournal != null && baseJournal.CanGoBack;
        }
        public static void BaseNavigationGoForward()
        {
            baseJournal.GoForward();
            raisViewChangeEvent();
        }
        public static void BaseNavigationGoBack()
        {
            baseJournal.GoBack();
            raisViewChangeEvent();
        }
        private static void raisViewChangeEvent()
        {
            string viewName = baseJournal.CurrentEntry.Uri.OriginalString;
            var ea = ContainerLocator.Container.Resolve<IEventAggregator>();
            var viewInfro = BaseViewInfors.Find(s => s.ViewName == viewName);
            ea.GetEvent<PubSubEvent<ViewInfor>>().Publish(viewInfro);
        }
        public static void ShowL3View(DialogParameters parameter)
        {
            ShowDialog("L3BaseView", parameter);
        }
        public static void ShowDialog(string viewName, DialogParameters parameter)
        {
            var dalogService = ContainerLocator.Container.Resolve<IDialogService>();

            dalogService.ShowDialog(viewName, parameter);
        }
        public static void BaseJournalClear()
        {
            if (baseJournal != null)
            {
                baseJournal.Clear();
            }
        }
    }
}
