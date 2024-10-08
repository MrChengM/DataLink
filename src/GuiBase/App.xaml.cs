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
using DataServer.Permission;

namespace GuiBase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var securityService = Container.Resolve<ISecurityService>();
            RegisterResourceNames(securityService);
            return Container.Resolve<StartUpWindow>((typeof(string), "LoadView"));
        }

        protected void RegisterResourceNames(ISecurityService securityService)
        {
            securityService.ResgisterResourceName("Admin", ResourceType.System);//管理员

            securityService.ResgisterResourceName("Header", ResourceType.Menu);//TOP菜单
            securityService.ResgisterResourceName("NavigationList", ResourceType.Menu);//导航菜单
            securityService.ResgisterResourceName("Menu_Bottom", ResourceType.Menu);//底部菜单

            securityService.ResgisterResourceName("OverView", ResourceType.ViewGourp);//OverView画面组
            securityService.ResgisterResourceName("L1View", ResourceType.ViewGourp);
            securityService.ResgisterResourceName("L2View", ResourceType.ViewGourp);
            securityService.ResgisterResourceName("Command", ResourceType.ViewGourp);
            securityService.ResgisterResourceName("Other", ResourceType.ViewGourp);

            securityService.ResgisterResourceName("AccManagerView", ResourceType.View);//用户管理画面
            securityService.ResgisterResourceName("AlarmView", ResourceType.View);//实时报警画面
            securityService.ResgisterResourceName("DeviceConfig", ResourceType.View);//设备配置画面
            securityService.ResgisterResourceName("HistoryAlarmView", ResourceType.View);//历史报警画面
            securityService.ResgisterResourceName("OperRecordView", ResourceType.View);//操作报警画面
            securityService.ResgisterResourceName("SignalMonitor", ResourceType.View);//信号监控画面

            securityService.ResgisterResourceName("L1ViewA", ResourceType.View);
            securityService.ResgisterResourceName("ViewA", ResourceType.View);
            securityService.ResgisterResourceName("ViewB", ResourceType.View);
            securityService.ResgisterResourceName("ViewC", ResourceType.View);


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
            containerRegistry.RegisterDialog<UserDetailedView>();
            containerRegistry.RegisterDialog<UserEditView>();
            containerRegistry.RegisterDialog<RoleEditView>();
            containerRegistry.RegisterDialog<ResourceAssignView>();
            containerRegistry.RegisterDialog<ResourceEditView>();
            containerRegistry.RegisterDialog<ResourceListView>();
            containerRegistry.RegisterDialog<ResourceNameListView>();

            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<ViewB>();
            containerRegistry.RegisterForNavigation<ViewC>();
            containerRegistry.RegisterForNavigation<ViewD>();
            containerRegistry.RegisterForNavigation<L1ViewA>();

            containerRegistry.RegisterForNavigation<LoadView>();
            containerRegistry.RegisterForNavigation<LogOnView>();

            containerRegistry.RegisterForNavigation<UserManagerView>();
            containerRegistry.RegisterForNavigation<RoleManagerView>();
            containerRegistry.RegisterForNavigation<ResourceManagerView>();


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
