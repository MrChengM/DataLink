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
using ConfigTool.Service;

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
            containerRegistry.RegisterDialog<RegisterDialog>();
            containerRegistry.RegisterDialog<BaseBuildDialog>();
            containerRegistry.RegisterDialog<PropertyDialog>();
            containerRegistry.RegisterDialog<SearchTagDialog>();

            containerRegistry.RegisterForNavigation<ComPortConfigView>();
            containerRegistry.RegisterForNavigation<EthernetConfigView>();
            containerRegistry.RegisterForNavigation<MemoryCommConfigView>();
            containerRegistry.RegisterForNavigation<FileCommConfigView>();
            containerRegistry.RegisterForNavigation<ChannelGeneralView>();
            containerRegistry.RegisterForNavigation<DeviceGeneralView>();
            containerRegistry.RegisterForNavigation<DeviceSpecialPropertyView>();
            containerRegistry.RegisterForNavigation<TagGroupGeneralView>();
            containerRegistry.RegisterForNavigation<TagGeneralView>();
            containerRegistry.RegisterForNavigation<TagScalingView>();
            containerRegistry.RegisterForNavigation<ChannelListView>();
            containerRegistry.RegisterForNavigation<DeviceListView>();
            containerRegistry.RegisterForNavigation<TagGroupListView>();
            containerRegistry.RegisterForNavigation<TagListView>();
            containerRegistry.RegisterForNavigation<ServerItemGeneralView>();
            containerRegistry.RegisterForNavigation<ServerListView>();
            containerRegistry.RegisterForNavigation<TagBindingGenralView>();
            containerRegistry.RegisterForNavigation<TagBindingListView>();
            containerRegistry.RegisterForNavigation<AlarmItemGeneralView>();
            containerRegistry.RegisterForNavigation<AlarmItemGeneral2View>();
            containerRegistry.RegisterForNavigation<AlarmListView>();
            containerRegistry.RegisterForNavigation<RecordGeneralView>();
            containerRegistry.RegisterForNavigation<RecordListView>();
            containerRegistry.RegisterForNavigation<RecordTagsManagerView>();
            containerRegistry.RegisterForNavigation<RecordTagsListView>();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IDialogService, DialogServiceWithRegionManager>();
            //containerRegistry.RegisterSingleton<Dictionary<string, DriverInfo>>();

            containerRegistry.RegisterSingleton<IConfigDataServer, ConfigDataServer>();
            containerRegistry.RegisterSingleton<ISingleLoggerServer, SingleLoggerServer>();
        }

    }
}
