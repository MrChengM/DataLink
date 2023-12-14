using ConfigTool.Service;
using DataServer;
using DataServer.Config;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Models
{
    public interface IBuildViewDespatcher
    {
        string Title { get; }
        //string NodeName { get; }

        bool Navigate(int pageNumber, out string header);

        int MaxPageNumber { get; }

        /// <summary>
        /// 返回首页，并初始化页面号
        /// </summary>
        /// <returns></returns>
        bool ReturnHomePage(out int pageNubmer,out string header);

        bool AddConfig();
    }

    public interface IPropertyViewDespatcher
    {
        string Title { get; }

        ObservableCollection<PropertyOptionItem> OptionItems { get; }

    }
    /// <summary>
    /// 通道画面配置协调类
    /// </summary>
    public class ChannelViewDespatcher : IBuildViewDespatcher, IPropertyViewDespatcher
    {
        private ChannelConfig _channelConfig;
        private readonly IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private NavigationParameters _np;
        public int MaxPageNumber => 2;
        public ChannelConfig ChannelConfig => _channelConfig;
        
        //string title;
        public string Title => "Channel";

        private ObservableCollection<PropertyOptionItem> propertyOptionItems;
        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        public ChannelViewDespatcher(IRegionManager regionManager,IConfigDataServer configDataServer)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelConfig = new ChannelConfig();
            _np = new NavigationParameters();
            _np.Add("ChannelConfig", _channelConfig);
            _np.Add("ComunicationSetUp", _channelConfig.ComunicationSetUp);

            _np.Add("isBuild", true);
        }

        public ChannelViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, ChannelConfig channelConfig)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelConfig = channelConfig;
            _np = new NavigationParameters();
            _np.Add("ChannelConfig", _channelConfig);
            _np.Add("ComunicationSetUp", _channelConfig.ComunicationSetUp);

            _np.Add("isBuild", false);
            initOptionItems();
        }
        #region IPropertyViewDespatcher
        private void initOptionItems()
        {

            if (_channelConfig != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>()
            {
                new PropertyOptionItem
                {
                    Content="General",
                    Url="ChannelGeneralView",
                    OptionSelectCommand =new DelegateCommand<string>(channelOptionSelect)
                }
            };
                switch (_channelConfig.DriverInformation.CommType)
                {
                    case CommunicationType.Serialport:
                        OptionItems.Add(new PropertyOptionItem
                        {
                            Content = "Serial Commniucation",
                            Url = "ComPortConfigView",
                            OptionSelectCommand = new DelegateCommand<string>(channelOptionSelect)
                        }
                        );
                        break;
                    case CommunicationType.Ethernet:
                        OptionItems.Add(new PropertyOptionItem
                        {
                            Content = "Ethernet Commniucation",
                            Url = "EthernetConfigView",
                            OptionSelectCommand = new DelegateCommand<string>(channelOptionSelect)
                        });
                        break;
                    case CommunicationType.File:
                        break;
                    case CommunicationType.Memory:
                        break;
                    default:
                        break;
                }
                //channelOptionSelect("ChannelGeneralView");
            }
        }
        void channelOptionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }
        #endregion


        #region IBuildViewDespatcher
        public bool Navigate(int pageNumber,out string header)
        {
            bool result = false;
            header = "default";

            if (pageNumber == 1)
            {
                header = "General";
                _regionManager.RequestNavigate("BuildBaseRegion", "ChannelGeneralView", _np);
                result = true;
            }
            else if (pageNumber == 2)
            {
                if (_channelConfig.DriverInformation != null)
                {
                    switch (_channelConfig.DriverInformation.CommType)
                    {
                        case CommunicationType.Serialport:
                            header = "ComPort Communication";
                            _regionManager.RequestNavigate("BuildBaseRegion", "ComPortConfigView", _np);
                            result = true;
                            break;
                        case CommunicationType.Ethernet:
                            header = "Ethernet Communication";
                            _regionManager.RequestNavigate("BuildBaseRegion", "EthernetConfigView", _np);
                            result = true;
                            break;
                        case CommunicationType.File:
                            header = "File Communication";
                            break;
                        case CommunicationType.Memory:
                            header = "Memory Communication";
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;
        }
        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            _regionManager.RequestNavigate("BuildBaseRegion", "ChannelGeneralView", _np);
            pageNubmer = 1;
            header = "General";
            return true;
        }
        public bool AddConfig()
        {
            bool result=false;
            if (_channelConfig.Name!=null)
            {
                if (_configDataServer.AddChannel(_channelConfig))
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion
    }
    public class DeviceViewDespatcher : IBuildViewDespatcher,IPropertyViewDespatcher
    {
        private ChannelConfig _channelConfig;
        private DeviceConfig _deviceConfig;
        private readonly IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private NavigationParameters _np;

        public ChannelConfig ChannelConfig => _channelConfig;
        public DeviceConfig DeviceConfig => _deviceConfig;
        public int MaxPageNumber => 2;
        public string Title => "Device";

        private ObservableCollection<PropertyOptionItem> propertyOptionItems;
        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        public DeviceViewDespatcher(IRegionManager regionManager,IConfigDataServer configDataServer, ChannelConfig channelConfig)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelConfig = channelConfig;
            _deviceConfig = new DeviceConfig();
            _np = new NavigationParameters();
            _np.Add("ChannelConfig", _channelConfig);
            _np.Add("DeviceConfig", _deviceConfig);
            _np.Add("isBuild", true);
        }
        public DeviceViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer,ChannelConfig channelConfig,DeviceConfig deviceConfig)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelConfig = channelConfig;
            _deviceConfig = deviceConfig;
            _np = new NavigationParameters();
            _np.Add("ChannelConfig", _channelConfig);
            _np.Add("DeviceConfig", _deviceConfig);
            _np.Add("isBuild", false);
            initOptionItems();
        }
        #region IPropertyViewDespatcher
        private void initOptionItems()
        {
            if (_deviceConfig != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>
                    {
                        new PropertyOptionItem
                        {
                            Content="General",
                            Url="DeviceGeneralView",
                            OptionSelectCommand = new DelegateCommand<string>(deviceOptionSelect)
                        },
                        new PropertyOptionItem
                        {
                            Content="Special Property",
                            Url="DeviceSpecialPropertyView",
                            OptionSelectCommand = new DelegateCommand<string>(deviceOptionSelect)
                        }
                    };
            }
        }
        void deviceOptionSelect(string url)
        {
            //var para = new NavigationParameters();
            //para.Add("ChannelConfig", _channelConfig);
            //para.Add("DeviceConfig", _deviceConfig);
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }
        #endregion
        #region IBuildViewDespatcher
        public bool AddConfig()
        {
            bool result = false;
            if (_deviceConfig.Name != null)
            {
                if (_configDataServer.AddDevice(_channelConfig.Name, _deviceConfig))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool Navigate(int pageNumber, out string header)
        {
            bool result = false;
            header = "default";

            if (pageNumber == 1)
            {
                header = "General";
                _regionManager.RequestNavigate("BuildBaseRegion", "DeviceGeneralView", _np);
                result = true;
            }
            else if (pageNumber == 2)
            {
                header = "Special Property";
                _regionManager.RequestNavigate("BuildBaseRegion", "DeviceSpecialPropertyView", _np);
                result = true;

            }
            return result;
        }
        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            pageNubmer = 1;
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "DeviceGeneralView", _np);
            return true;
        }
        #endregion

    }
    public class TagGroupViewDespatcher : IBuildViewDespatcher,IPropertyViewDespatcher
    {
        private IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private string _channelName;
        private string _deviceName;
        private TagGroupConfig _tagGroupConfig;
        private NavigationParameters _np;
        private ObservableCollection<PropertyOptionItem> propertyOptionItems;

        public int MaxPageNumber => 1;

        public string Title => "TagGroup";

        public TagGroupConfig Config => _tagGroupConfig;

        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        /// <summary>
        /// 新建画面初始化函数
        /// Build使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        public TagGroupViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, string channelName, string deviceName)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelName = channelName;
            _deviceName = deviceName;
            _tagGroupConfig = new TagGroupConfig();
            _np = new NavigationParameters();
            _np.Add("TagGroupConfig", _tagGroupConfig);
            _np.Add("isBuild", true);
        }

        /// <summary>
        /// 属性画面初始化函数
        /// Property使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupConfig"></param>
        public TagGroupViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, string channelName, string deviceName,TagGroupConfig tagGroupConfig)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelName = channelName;
            _deviceName = deviceName;
            _tagGroupConfig = tagGroupConfig;
            _np = new NavigationParameters();
            _np.Add("TagGroupConfig", _tagGroupConfig);
            _np.Add("isBuild", false);
            initOptionItems();
        }
       
        private void initOptionItems()
        {
            if (_tagGroupConfig != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>
                    {
                        new PropertyOptionItem
                        {
                            Content="General",
                            Url="TagGroupGeneralView",
                            OptionSelectCommand = new DelegateCommand<string>(tagGroupOptionSelect)
                        }
                };
            }
        }

        private void tagGroupOptionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }

        public bool AddConfig()
        {
           return _configDataServer.AddTagGroup(_channelName, _deviceName, _tagGroupConfig);
        }

        public bool Navigate(int pageNubmer, out string header)
        {
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "TagGroupGeneralView", _np);
            return true;

        }

        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            pageNubmer = 1;
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "TagGroupGeneralView", _np);
            return true;
        }
    }
    public class TagViewDespatcher : IBuildViewDespatcher, IPropertyViewDespatcher
    {
        private IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private string _channelName;
        private string _deviceName;
        private string _tagGroupName;
        private TagConfig _tagConfig;
        private NavigationParameters _np;
        private ObservableCollection<PropertyOptionItem> propertyOptionItems;
        public int MaxPageNumber => 2;

        public string Title => "Tag";
        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        public TagConfig Config => _tagConfig;
        /// <summary>
        /// 新建Tag画面使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupName"></param>
        public TagViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, string channelName, string deviceName,string tagGroupName)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelName = channelName;
            _deviceName = deviceName;
            _tagGroupName = tagGroupName;
            _tagConfig = new TagConfig();
            _np = new NavigationParameters();
            _np.Add("TagConfig", _tagConfig);
            _np.Add("isBuild", true);
        }

        /// <summary>
        /// 编辑Property使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupConfig"></param>
        public TagViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, string channelName, string deviceName, string tagGroupName,TagConfig tagConfig)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _channelName = channelName;
            _deviceName = deviceName;
            _tagGroupName = tagGroupName;
            _tagConfig = tagConfig;
            _np = new NavigationParameters();
            _np.Add("TagConfig", _tagConfig);
            _np.Add("isBuild", false);
            initOptionItems();
        }

        private void initOptionItems()
        {
            if (_tagConfig != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>
                    {
                        new PropertyOptionItem
                        {
                            Content="General",
                            Url="TagGeneralView",
                            OptionSelectCommand = new DelegateCommand<string>(tagOptionSelect)
                        },
                        new PropertyOptionItem
                        {
                            Content="Scaling",
                            Url="TagScalingView",
                            OptionSelectCommand = new DelegateCommand<string>(tagOptionSelect)
                        }
                };
            }
        }

        private void tagOptionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }

        public bool AddConfig()
        {
            return _configDataServer.AddTag(_channelName, _deviceName, _tagGroupName,_tagConfig);
        }

        public bool Navigate(int pageNumber, out string header)
        {
            bool result = false;
            header = "default";

            if (pageNumber == 1)
            {
                header = "General";
                _regionManager.RequestNavigate("BuildBaseRegion", "TagGeneralView", _np);
                result = true;
            }
            else if (pageNumber == 2)
            {
                header = "Scaling";
                _regionManager.RequestNavigate("BuildBaseRegion", "TagScalingView", _np);
                result = true;

            }
            return result;
        }

        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            pageNubmer = 1;
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "TagGeneralView", _np);
            return true;
        }

    }

    public class ServerItemViewDespatcher : IBuildViewDespatcher, IPropertyViewDespatcher
    {
        private IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private ServerItemConfig _serverItem;
        private NavigationParameters _np;
        private ObservableCollection<PropertyOptionItem> propertyOptionItems;
        public int MaxPageNumber => 2;

        public string Title => "Server Item";
        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        public ServerItemConfig Config => _serverItem;
        /// <summary>
        /// 新建Server画面使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupName"></param>
        public ServerItemViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _serverItem = new ServerItemConfig();
            _np = new NavigationParameters();
            _np.Add("ServerItemConfig", _serverItem);
            _np.Add("ComunicationSetUp", _serverItem.ComunicationSetUp);
            _np.Add("isBuild", true);
        }

        /// <summary>
        /// 编辑Property使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupConfig"></param>
        public ServerItemViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, ServerItemConfig serverItem)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _serverItem = serverItem;
            _np = new NavigationParameters();
            _np.Add("ServerItemConfig", _serverItem);
            _np.Add("ComunicationSetUp", _serverItem.ComunicationSetUp);
            _np.Add("isBuild", false);
            initOptionItems();
        }

        private void initOptionItems()
        {
            if (_serverItem != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>()
                    {
                       new PropertyOptionItem
                {
                    Content="General",
                    Url="ServerItemGeneralView",
                    OptionSelectCommand =new DelegateCommand<string>(severOptionSelect)
                }
                };
                switch (_serverItem.Option)
                {
                    case ServerOption.ModbusRTU:
                        OptionItems.Add(new PropertyOptionItem
                        {
                            Content = "Serial Commniucation",
                            Url = "ComPortConfigView",
                            OptionSelectCommand = new DelegateCommand<string>(severOptionSelect)
                        });
                        break;
                    case ServerOption.ModbusTCP :
                        OptionItems.Add(new PropertyOptionItem
                        {
                            Content = "Ethernet Commniucation",
                            Url = "EthernetConfigView",
                            OptionSelectCommand = new DelegateCommand<string>(severOptionSelect)
                        });
                        break;
                    case ServerOption.Freedom:
                        OptionItems.Add(new PropertyOptionItem
                        {
                            Content = "Ethernet Commniucation",
                            Url = "EthernetConfigView",
                            OptionSelectCommand = new DelegateCommand<string>(severOptionSelect)
                        });
                        break;
                    default:
                        break;
                }
            };
        }

        private void severOptionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }

        public bool AddConfig()
        {
            return _configDataServer.AddServerItem(_serverItem);
        }

        public bool Navigate(int pageNumber, out string header)
        {

            bool result = false;
            header = "default";

            if (pageNumber == 1)
            {
                header = "General";
                _regionManager.RequestNavigate("BuildBaseRegion", "ServerItemGeneralView", _np);
                result = true;
            }
            else if (pageNumber == 2)
            {
                switch (_serverItem.Option)
                {
                    case ServerOption.ModbusTCP:
                        header = "Ethernet Communication";
                        _regionManager.RequestNavigate("BuildBaseRegion", "EthernetConfigView", _np);
                        result = true;
                        break;
                    case ServerOption.ModbusRTU:
                        header = "ComPort Communication";
                        _regionManager.RequestNavigate("BuildBaseRegion", "ComPortConfigView", _np);
                        result = true;
                        break;
                    case ServerOption.Freedom:
                        header = "Ethernet Communication";
                        _regionManager.RequestNavigate("BuildBaseRegion", "EthernetConfigView", _np);
                        result = true;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            pageNubmer = 1;
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "ServerItemGeneralView", _np);
            return true;
        }

    }
    public class TagBindingViewDespatcher : IBuildViewDespatcher, IPropertyViewDespatcher
    {
        private IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private string _serverName;
        private TagBindingConfig _tagBinding;
        private NavigationParameters _np;
        private ObservableCollection<PropertyOptionItem> propertyOptionItems;
        public int MaxPageNumber => 1;

        public string Title => "Tag Binding";
        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        public TagBindingConfig Config => _tagBinding;
        /// <summary>
        /// 新建画面使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupName"></param>
        public TagBindingViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer,string serverName)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _serverName = serverName;
            _tagBinding = new TagBindingConfig();
            _np = new NavigationParameters();
            _np.Add("TagBindingConfig", _tagBinding);
            _np.Add("isBuild", true);
        }

        /// <summary>
        /// 编辑Property使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupConfig"></param>
        public TagBindingViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, string serverName,TagBindingConfig tagBinding)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _serverName = serverName;
            _tagBinding = tagBinding;
            _np = new NavigationParameters();
            _np.Add("TagBindingConfig", _tagBinding);
            _np.Add("isBuild", false);
            initOptionItems();
        }

        private void initOptionItems()
        {
            if (_tagBinding != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>()
                    {
                       new PropertyOptionItem
                {
                    Content="General",
                    Url="TagBindingGenralView",
                    OptionSelectCommand =new DelegateCommand<string>(optionSelect)
                }
                };
            };
        }

        private void optionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }

        public bool AddConfig()
        {
            return _configDataServer.AddTagBinding(_serverName,_tagBinding);
        }

        public bool Navigate(int pageNumber, out string header)
        {

            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "TagBindingGenralView", _np);
            return true;
        }

        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            pageNubmer = 1;
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "TagBindingGenralView", _np);
            return true;
        }

    }
    public class AlarmItemViewDespatcher : IBuildViewDespatcher, IPropertyViewDespatcher
    {
        private IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private AlarmItemConfig  _alarmItem;
        private NavigationParameters _np;
        private ObservableCollection<PropertyOptionItem> propertyOptionItems;
        public int MaxPageNumber => 2;

        public string Title => "Alarm Item";
        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        public AlarmItemConfig Config => _alarmItem;
        /// <summary>
        /// 新建Server画面使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupName"></param>
        public AlarmItemViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _alarmItem = new AlarmItemConfig();
            _np = new NavigationParameters();
            _np.Add("AlarmItemConfig", _alarmItem);
            _np.Add("isBuild", true);
        }

        /// <summary>
        /// 编辑Property使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupConfig"></param>
        public AlarmItemViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, AlarmItemConfig alarmItem)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _alarmItem = alarmItem;
            _np = new NavigationParameters();
            _np.Add("AlarmItemConfig", _alarmItem);
            _np.Add("isBuild", false);
            initOptionItems();
        }

        private void initOptionItems()
        {
            if (_alarmItem != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>()
                    {
                       new PropertyOptionItem
                {
                    Content="General",
                    Url="AlarmItemGeneralView",
                    OptionSelectCommand =new DelegateCommand<string>(alarmOptionSelect)
                },        new PropertyOptionItem
                {
                    Content="General2",
                    Url="AlarmItemGeneral2View",
                    OptionSelectCommand =new DelegateCommand<string>(alarmOptionSelect)
                }
                };
               
            };
        }

        private void alarmOptionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }

        public bool AddConfig()
        {
            return _configDataServer.AddAlarmItemConfig(_alarmItem);
        }

        public bool Navigate(int pageNumber, out string header)
        {

            bool result = false;
            header = "default";

            if (pageNumber == 1)
            {
                header = "General";
                _regionManager.RequestNavigate("BuildBaseRegion", "AlarmItemGeneralView", _np);
                result = true;
            }
            else if (pageNumber == 2)
            {
                header = "General2";
                _regionManager.RequestNavigate("BuildBaseRegion", "AlarmItemGeneral2View", _np);
                result = true;
            }
            return result;
        }

        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            pageNubmer = 1;
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "AlarmItemGeneralView", _np);
            return true;
        }

    }
    public class RecordItemViewDespatcher : IBuildViewDespatcher, IPropertyViewDespatcher
    {
        private IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private RecordItemConfig _recordItem;
        private NavigationParameters _np;
        private ObservableCollection<PropertyOptionItem> propertyOptionItems;
        public int MaxPageNumber => 1;

        public string Title => "Record Item";
        public ObservableCollection<PropertyOptionItem> OptionItems => propertyOptionItems;

        public RecordItemConfig Config => _recordItem;
        /// <summary>
        /// 新建Server画面使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupName"></param>
        public RecordItemViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _recordItem = new RecordItemConfig();
            _np = new NavigationParameters();
            _np.Add("RecordItemConfig", _recordItem);
            _np.Add("isBuild", true);
        }

        /// <summary>
        /// 编辑Property使用
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="configDataServer"></param>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroupConfig"></param>
        public RecordItemViewDespatcher(IRegionManager regionManager, IConfigDataServer configDataServer, RecordItemConfig recordItem)
        {
            _regionManager = regionManager;
            _configDataServer = configDataServer;
            _recordItem = recordItem;
            _np = new NavigationParameters();
            _np.Add("RecordItemConfig", _recordItem);
            _np.Add("isBuild", false);
            initOptionItems();
        }

        private void initOptionItems()
        {
            if (_recordItem != null)
            {
                propertyOptionItems = new ObservableCollection<PropertyOptionItem>()
                    {
                       new PropertyOptionItem
                {
                    Content="General",
                    Url="RecordGeneralView",
                    OptionSelectCommand =new DelegateCommand<string>(recordOptionSelect)
                }
                };

            };
        }

        private void recordOptionSelect(string url)
        {
            _regionManager.RequestNavigate("PropertyRegion", url, _np);
        }

        public bool AddConfig()
        {
            return _configDataServer.AddRecordItem(_recordItem);
        }

        public bool Navigate(int pageNumber, out string header)
        {

            bool result = false;
            header = "default";

            if (pageNumber == 1)
            {
                header = "General";
                _regionManager.RequestNavigate("BuildBaseRegion", "RecordGeneralView", _np);
                result = true;
            }
            return result;

        }

        public bool ReturnHomePage(out int pageNubmer, out string header)
        {
            pageNubmer = 1;
            header = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "RecordGeneralView", _np);
            return true;
        }

    }


}
