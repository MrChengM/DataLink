using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Prism.Services.Dialogs;
using GuiBase.Views;
using GuiBase.Common;
using GuiBase.Helper;
using GuiBase.Services;
using DataServer;
using DataServer.Log;
using DataServer.Permission;
using GuiBase.Views.L2View;
using GuiBase.Views.L1View;
using GuiBase.Views.L3View;
using GuiBase.ViewModels;

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
            securityService.ResgisterResource("Admin", ResourceType.System);//管理员

            securityService.ResgisterResource("Header", ResourceType.Menu);//TOP菜单
            securityService.ResgisterResource("NavigationList", ResourceType.Menu);//导航菜单
            securityService.ResgisterResource("Menu_Bottom", ResourceType.Menu);//底部菜单

            securityService.ResgisterResource("OverView", ResourceType.ViewGourp);//OverView画面组
            securityService.ResgisterResource("L1View", ResourceType.ViewGourp);
            securityService.ResgisterResource("L2View", ResourceType.ViewGourp);
            securityService.ResgisterResource("Command", ResourceType.ViewGourp);
            securityService.ResgisterResource("Other", ResourceType.ViewGourp);

            securityService.ResgisterResource("AccManagerView", ResourceType.View);//用户管理画面
            securityService.ResgisterResource("AlarmView", ResourceType.View);//实时报警画面
            securityService.ResgisterResource("DeviceConfig", ResourceType.View);//设备配置画面
            securityService.ResgisterResource("HistoryAlarmView", ResourceType.View);//历史报警画面
            securityService.ResgisterResource("OperRecordView", ResourceType.View);//操作报警画面
            securityService.ResgisterResource("SignalMonitor", ResourceType.View);//信号监控画面

            securityService.ResgisterResource("L1ViewA", ResourceType.View);
            securityService.ResgisterResource("ViewA", ResourceType.View);
            securityService.ResgisterResource("ViewB", ResourceType.View);
            securityService.ResgisterResource("ViewC", ResourceType.View);
            securityService.ResgisterResource("ViewD", ResourceType.View);


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
            containerRegistry.RegisterDialog<L3BaseView>();

            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<ViewB>();
            containerRegistry.RegisterForNavigation<ViewC>();
            containerRegistry.RegisterForNavigation<ViewD>();
            containerRegistry.RegisterForNavigation<L1ViewA>();
            containerRegistry.RegisterForNavigation <PowerBox001,DefaultElementL3ViewModel>();
            

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
            containerRegistry.RegisterSingleton<ISignalService, SignalService>();
            containerRegistry.RegisterSingleton<ILocalizationService, LocalizationService>();
            containerRegistry.RegisterSingleton<IOperateRecordService, OperateRecordService>();

            containerRegistry.RegisterSingleton<ISignalMangement, SignalMangement>();
            containerRegistry.RegisterSingleton<IGeneralCommandBuilder, GeneralCommandBuilder>();
            containerRegistry.RegisterSingleton<IGCommandSet, GCommandSet>();

        }
    }
}
