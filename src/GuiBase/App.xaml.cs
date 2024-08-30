using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using GuiBase.Views;
using Prism.Services.Dialogs;
using GuiBase.Helper;
using GuiBase.Services;
using DataServer;

namespace GuiBase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<StartUpWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<LogOnView>();
            containerRegistry.RegisterDialog<AccManagerView>();
            containerRegistry.RegisterDialog<OperRecordView>();
            containerRegistry.RegisterDialog<AlarmView>();
            containerRegistry.RegisterDialog<HistoryAlarmView>();
            containerRegistry.RegisterDialog<SignalMonitorView>();
            containerRegistry.RegisterDialog<DeviceConfigView>();

            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<ViewB>();
            containerRegistry.RegisterForNavigation<ViewC>();
            containerRegistry.RegisterForNavigation<ViewD>();
            containerRegistry.RegisterForNavigation<L1ViewA>();

            containerRegistry.RegisterForNavigation<LoadView>();
            containerRegistry.RegisterForNavigation<LogOnView>();

        }
        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IDialogService, DialogServiceWithRegionManager>();
            //containerRegistry.RegisterSingleton<Dictionary<string, DriverInfo>>();

            containerRegistry.RegisterSingleton<ILog, Log4netWrapper>();
            containerRegistry.RegisterSingleton<ISecurityService, SecurityService>();
            containerRegistry.RegisterSingleton<IAlarmService, AlarmService>();
            containerRegistry.RegisterSingleton<IHistoryAlarmService, HistoryAlarmService>();
        }
    }
}
