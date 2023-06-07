using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ConfigTool.ViewModels;
using ConfigTool.Views;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Prism.Unity;
using ConfigTool.Models;
using Prism.Modularity;
using Prism.Regions;
using Prism.Events;
using DataServer.Config;

namespace ConfigTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<BuildChannelDialog>();
            containerRegistry.RegisterDialog<RegisterDialog>();
            containerRegistry.RegisterDialog<BuildDeviceDialog>();
            containerRegistry.RegisterDialog<BuildTagGroupDialog>();
            containerRegistry.RegisterDialog<PropertyDialog>();

            containerRegistry.RegisterForNavigation<ComPortConfigView>();
            containerRegistry.RegisterForNavigation<EthernetConfigView>();
            containerRegistry.RegisterForNavigation<MemoryCommConfigView>();
            containerRegistry.RegisterForNavigation<FileCommConfigView>();
            containerRegistry.RegisterForNavigation<ChannelGeneralView>();
            containerRegistry.RegisterForNavigation<DeviceGeneralView>();
            containerRegistry.RegisterForNavigation<DeviceSpecialPropertyView>();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IDialogService, DialogServiceWithRegionManager>();
            //containerRegistry.RegisterSingleton<Dictionary<string, DriverInfo>>();
        }

    }
}
