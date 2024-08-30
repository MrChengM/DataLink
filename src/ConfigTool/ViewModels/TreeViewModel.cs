using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigTool.Models;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using System.Windows;
using Prism.Services.Dialogs;
using Prism.Regions;
using Prism.Ioc;
using ConfigTool.Views;
using DataServer.Config;
using ConfigTool.Service;
using Utillity.File;
using DataServer;
using DataServer.Alarm;

namespace ConfigTool.ViewModels
{
    public class TreeViewModel : BindableBase
    {
        #region Private Field

        private readonly Geometry projectIcon = (Geometry)Application.Current.FindResource("ProjectIcon1");
        private readonly Geometry connectivityIcon = (Geometry)Application.Current.FindResource("ConnectivityIcon1");
        private readonly Geometry channelIcon = (Geometry)Application.Current.FindResource("DeviceIcon2");
        private readonly Geometry channelComPortIcon = (Geometry)Application.Current.FindResource("ComPortIcon1");
        private readonly Geometry channelEthentIcon = (Geometry)Application.Current.FindResource("EternetIcon2");
        private readonly Geometry deviceIcon = (Geometry)Application.Current.FindResource("DeviceIcon1");
        private readonly Geometry tagGroupIcon = (Geometry)Application.Current.FindResource("TagGroupIcon1");
        private readonly Geometry tagIcon = (Geometry)Application.Current.FindResource("TagIcon2");
        private readonly Geometry serverIcon = (Geometry)Application.Current.FindResource("ServerIcon1");
        private readonly Geometry serverItemIcon = (Geometry)Application.Current.FindResource("SingleServerIcon1");
        private readonly Geometry tagBindingIcon = (Geometry)Application.Current.FindResource("TagBindingIcon1");
        private readonly Geometry alarmIcon = (Geometry)Application.Current.FindResource("AlarmIcon1");
        private readonly Geometry alarmItemIcon = (Geometry)Application.Current.FindResource("AlarmIcon2");
        private readonly Geometry recordIcon = (Geometry)Application.Current.FindResource("Record");
        private readonly Geometry recordItemIcon = (Geometry)Application.Current.FindResource("RecordItem");


        private Stack<int> idBuffer;

        private Dictionary<int, TreeNode> _treeNodeCollcet;

        private IRegionManager _regionManager;
        private IConfigDataServer _configDataServer;
        private ISingleLoggerServer _singleLoggerServer;
        #endregion
        #region Property
        private string newNodeName;

        public string NewNodeName
        {
            get { return newNodeName; }
            set { SetProperty(ref newNodeName, value, "NewNodeName"); }
        }

        private Geometry newNodeIcon;

        public Geometry NewNodeIcon
        {
            get { return newNodeIcon; }
            set { SetProperty(ref newNodeIcon, value, "NewNodeIcon"); }
        }

        #endregion
        #region ICommand

        private ICommand nodeMouseRightCommand;

        public ICommand NodeMouseRightCommand
        {
            get { return nodeMouseRightCommand; }
            set { nodeMouseRightCommand = value; }
        }

        private ICommand nodeMouseLeftCommand;

        public ICommand NodeMouseLeftCommand
        {
            get { return nodeMouseLeftCommand; }
            set { nodeMouseLeftCommand = value; }
        }


        private ICommand addNodeCommand;

        public ICommand AddNodeCommand
        {
            get { return addNodeCommand; }
            set { addNodeCommand = value; }
        }

        private ICommand deleteNodeCommand;

        public ICommand DeleteNodeCommand
        {
            get { return deleteNodeCommand; }
            set { deleteNodeCommand = value; }
        }
        private ICommand openPropertyCommand;

        public ICommand OpenPropertyCommand
        {
            get { return openPropertyCommand; }
            set { openPropertyCommand = value; }
        }

        private ICommand importCommand;

        public ICommand ImportCommand
        {
            get { return importCommand; }
            set { importCommand = value; }
        }

        private ICommand exportCommand;

        public ICommand ExportCommand
        {
            get { return exportCommand; }
            set { exportCommand = value; }
        }
        #endregion

        private ObservableCollection<TreeNode> items;

        public ObservableCollection<TreeNode> Items
        {
            get { return items; }
            set { SetProperty(ref items, value, "Items"); }
        }
        private IDialogService _dialogService;

        public TreeViewModel(IRegionManager regionManager, IDialogService dialogService,IConfigDataServer configDataServer,ISingleLoggerServer singleLoggerServer)
        {
            nodeMouseRightCommand = new DelegateCommand<object>(nodeMouseRight);
            nodeMouseLeftCommand = new DelegateCommand<object>(nodeMouseLeft);

            addNodeCommand = new DelegateCommand<object>(addNode);
            deleteNodeCommand = new DelegateCommand<object>(deleteNode);
            openPropertyCommand = new DelegateCommand<object>(openProperty);
            importCommand= new DelegateCommand<object>(import);
            exportCommand = new DelegateCommand<object>(export);
            _dialogService = dialogService;
            _configDataServer = configDataServer;
            _configDataServer.ConfigChangeEvent += _configDataServer_ConfigChangeEvent;

            _regionManager = regionManager;
            _singleLoggerServer = singleLoggerServer;


            loadTreeNode(_configDataServer.ProjectConfig);

        }

        private void export(object obj)
        {
            TreeNode curreNode = (TreeNode)obj;
            switch (curreNode.Type)
            {
                case NodeType.Project:
                    break;
                case NodeType.Connectivity:
                    break;
                case NodeType.Channel:
                    break;
                case NodeType.Device:
                    break;
                case NodeType.TagGroup:
                    exportTags(curreNode);
                    break;
                case NodeType.Tag:
                    break;
                case NodeType.Servers:
                    break;
                case NodeType.ServerItem:
                    exportTagBindings(curreNode);
                    break;
                case NodeType.TagBinding:
                    break;
                case NodeType.Alarms:
                    exportAlarmsBindings(curreNode);
                    break;
                case NodeType.AlarmItem:
                    break;
                default:
                    break;
            }
        }

     

        private void exportTags(TreeNode curreNode)
        {
            TreeNode channelNode, deviceNode;
            deviceNode = _treeNodeCollcet[curreNode.ParentId];
            channelNode = _treeNodeCollcet[deviceNode.ParentId];
            var tagGroup = _configDataServer.GetTagGroup(channelNode.NodeName, deviceNode.NodeName, curreNode.NodeName);
            var tags = tagGroup.Tags;
            List<TagConfig> tagConfigs = new List<TagConfig>();
            foreach (var kvp in tags)
            {
                tagConfigs.Add(kvp.Value);
            }
            string fileName = "";
            string fileStyle = "CSV|*.csv";
            if (!FileDialog.OutputFile(ref fileName, fileStyle))
            {
                return;
            }
            CsvFunction.CsvWirte(fileName, tagConfigs,
                (s) =>
                {
                    return new string[]
                    {
                        s.Name,
                        s.Address,
                        s.DataType.ToString(),
                        s.Length.ToString(),
                        s.Operate.ToString(),
                        s.Scaling.ScaleType.ToString(),
                        s.Scaling.DataType.ToString(),
                        s.Scaling.RawLow.ToString(),
                        s.Scaling.RawHigh.ToString(),
                        s.Scaling.ScaledLow.ToString(),
                        s.Scaling.ScaledHigh.ToString()
                    };
                },
                new string[]
                {
                    "Name",
                    "Address",
                    "DataType",
                    "Length",
                    "R/W",
                    "Scaling Type",
                    "Scaling Data Type",
                    "RawLow",
                    "RawHigh",
                    "ScaledLow",
                    "ScaledHigh"
                }
                );
        }
        private void exportTagBindings(TreeNode curreNode)
        {
            var serverItem = _configDataServer.GetServerItem(curreNode.NodeName);
            var dTagBindings = serverItem.TagBindingList;
            List<TagBindingConfig> lTagBindings = new List<TagBindingConfig>();
            foreach (var kvp in dTagBindings)
            {
                lTagBindings.Add(kvp.Value);
            }
            string fileName = "";
            string fileStyle = "CSV|*.csv";
            if (FileDialog.OutputFile(ref fileName, fileStyle))
            {
                CsvFunction.CsvWirte(fileName, lTagBindings,
                                (s) =>
                                {
                                    return new string[]
                                    {
                                        s.DestTagName,
                                        s.SourceTagName,

                                    };
                                },
                                new string[]
                                {
                                    "Name",
                                    "TagName",
                                }
                                );
            }


        }
        private void exportAlarmsBindings(TreeNode curreNode)
        {

            var alarms = _configDataServer.GetAlarms();
            var dAlarmItems = alarms.AlarmGroup;
            List<AlarmItemConfig> lAlarmItems = new List<AlarmItemConfig>();
            foreach (var kvp in dAlarmItems)
            {
                lAlarmItems.Add(kvp.Value);
            }
            string fileName = "";
            string fileStyle = "CSV|*.csv";
            if (!FileDialog.OutputFile(ref fileName, fileStyle))
                return;
            CsvFunction.CsvWirte(fileName, lAlarmItems,
                (s) =>
                {
                    return new string[]
                    {
                        s.AlarmTag,
                        s.TagName,
                        s.PartName,
                        s.AlarmDescription,
                        s.AlarmType.ToString(),
                        s.ALNumber,
                        s.Level1View,
                        s.Level2View,
                        s.ConditionType.ToString(),
                        s.ConditionValue.ToString(),
                        s.AlarmGroup
                    };
                },
                new string[]
                {
                    "Alarm Name",
                    "Tag Name",
                    "Part Name",
                    "Alarm Description",
                    "Alarm Type",
                    "ALNumber",
                    "Level1 View",
                    "Level2 View",
                    "Condition Type",
                    "Condition Value",
                    "Alarm Group"
                }
                );
        }
        private void import(object obj)
        {
            TreeNode curreNode = (TreeNode)obj;
            switch (curreNode.Type)
            {
                case NodeType.Project:
                    break;
                case NodeType.Connectivity:
                    break;
                case NodeType.Channel:
                    break;
                case NodeType.Device:
                    break;
                case NodeType.TagGroup:
                    importTags(curreNode);
                    break;
                case NodeType.Tag:
                    break;
                case NodeType.Servers:
                    break;
                case NodeType.ServerItem:
                    importTagBindings(curreNode);
                    break;
                case NodeType.TagBinding:
                    break;
                case NodeType.Alarms:
                    importAlarms(curreNode);
                    break;
                case NodeType.AlarmItem:
                    break;
                default:
                    break;
            }
        }

        private void importTagBindings(TreeNode curreNode)
        {
            //TreeNode serverItemNode=_treeNodeCollcet[curreNode.ParentId];
            string fileName = "";
            string fileStyle = "CSV|*.csv";
            FileDialog.InputFile(ref fileName, fileStyle);
            var tagBindings = CsvFunction.CsvRead(fileName, (s) => {
                return new TagBindingConfig()
                {
                     DestTagName=s[0],
                     SourceTagName=s[1]
                };
            }, true);
            foreach (var tagBinding in tagBindings)
            {
                _configDataServer.AddTagBinding(curreNode.NodeName,tagBinding);
            }
        }

      
        private void importTags(TreeNode curreNode)
        {
            TreeNode channelNode, deviceNode;
            deviceNode = _treeNodeCollcet[curreNode.ParentId];
            channelNode = _treeNodeCollcet[deviceNode.ParentId];
            //var tagGroup = _configDataServer.GetTagGroup(channelNode.NodeName, deviceNode.NodeName, curreNode.NodeName);
            string fileName = "";
            string fileStyle = "CSV|*.csv";
            FileDialog.InputFile(ref fileName, fileStyle);
            var tags = CsvFunction.CsvRead(fileName, (s)=> {
                return new TagConfig()
                {
                    Name = s[0],
                    Address = s[1],
                    DataType = (DataType)Enum.Parse(typeof(DataType), s[2]),
                    Length = Convert.ToInt32(s[3]),
                    Scaling = new Scaling()
                    {
                        ScaleType = (ScaleType)Enum.Parse(typeof(ScaleType), s[4]),
                        DataType = (DataType)Enum.Parse(typeof(DataType), s[5]),
                        RawLow = Convert.ToInt32(s[6]),
                        RawHigh = Convert.ToInt32(s[7]),
                        ScaledLow = Convert.ToInt32(s[8]),
                        ScaledHigh = Convert.ToInt32(s[9])
                    }
                };
            }, true);
            foreach (var tag in tags)
            {
                _configDataServer.AddTag(channelNode.NodeName, deviceNode.NodeName, curreNode.NodeName, tag);
            }
        }
        private void importAlarms(TreeNode curreNode)
        {
            string fileName = "";
            string fileStyle = "CSV|*.csv";
            FileDialog.InputFile(ref fileName, fileStyle);
            var alarmItems = CsvFunction.CsvRead(fileName, (s) => {
                return new AlarmItemConfig()
                {
                    AlarmTag = s[0],
                    TagName = s[1],
                    PartName = s[2],
                    AlarmDescription = s[3],
                    AlarmType=(AlarmType)Enum.Parse(typeof(AlarmType),s[4]),
                    ALNumber=s[5],
                    Level1View=s[6],
                    Level2View=s[7],
                    ConditionType=(ConditionType)Enum.Parse(typeof(ConditionType),s[8]),
                    ConditionValue =Convert.ToInt32(s[9]),
                    AlarmGroup=s[10]
                };
            }, true);
            foreach (var alarmItem in alarmItems)
            {
                _configDataServer.AddAlarmItemConfig(alarmItem);
            }

        }
        private void _configDataServer_ConfigChangeEvent(object sender, ConfigEventArgs e)
        {
            if (e.OperateMode == ConfigOperate.ReloadAll)
            {
                var config = _configDataServer.ProjectConfig;
                loadTreeNode(config);
            }
        }

        private void loadTreeNode(ProjectConfig projectConfig)
        {
            idBuffer = new Stack<int>(1000);
            Items = new ObservableCollection<TreeNode>();
            _treeNodeCollcet = new Dictionary<int, TreeNode>();
            for (int i = 0; i < 1000; i++)
            {
                idBuffer.Push(i);
            }
            var rootNode = new TreeNode(idBuffer.Pop(), -1, "Project", NodeType.Project, projectIcon)
            {
                MenuNewNodeVbt = Visibility.Collapsed,
                MenuDeleteVbt = Visibility.Collapsed,
                MenuPropertyVbt = Visibility.Collapsed,
                MenuImportVbt = Visibility.Collapsed,
                MenuExportVbt = Visibility.Collapsed,
            };
            items.Add(rootNode);
            _treeNodeCollcet.Add(rootNode.NodeId, rootNode);

            var connectNode = loadClientNode(rootNode.NodeId, projectConfig.Client);
            rootNode.ChildNodes.Add(connectNode);

            var serversNode = loadServerNode(rootNode.NodeId, projectConfig.Server);
            rootNode.ChildNodes.Add(serversNode);

            var alarmsNode = loadAlarmsNode(rootNode.NodeId, projectConfig.Alarms);
            rootNode.ChildNodes.Add(alarmsNode);

            var recordsNode = loadRecordsNode(rootNode.NodeId, projectConfig.Records);
            rootNode.ChildNodes.Add(recordsNode);


        }


        private TreeNode loadClientNode(int parentId ,ClientConfig clientConfig)
        {
            var connectNode = new TreeNode(idBuffer.Pop(), parentId, "Connectivity", NodeType.Connectivity, connectivityIcon)
            {
                MenuNewNodeVbt = Visibility.Visible,
                MenuDeleteVbt = Visibility.Collapsed,
                MenuPropertyVbt = Visibility.Collapsed,
                MenuImportVbt = Visibility.Collapsed,
                MenuExportVbt = Visibility.Collapsed,
            };
            _treeNodeCollcet.Add(connectNode.NodeId, connectNode);

            foreach (var channel in clientConfig.Channels)
            {
                var icon = channelIcon;
                switch (channel.Value.DriverInformation.CommType)
                {
                    case CommunicationType.Serialport:
                        icon = channelComPortIcon;
                        break;
                    case CommunicationType.Ethernet:
                        icon = channelEthentIcon;
                        break;
                    case CommunicationType.File:
                        break;
                    case CommunicationType.Memory:
                        break;
                    default:
                        break;
                }
                var channelNode = new TreeNode(idBuffer.Pop(), connectNode.NodeId, channel.Key, NodeType.Channel, icon) 
                {
                    MenuNewNodeVbt = Visibility.Visible,
                    MenuDeleteVbt = Visibility.Visible,
                    MenuPropertyVbt = Visibility.Visible,
                    MenuImportVbt = Visibility.Collapsed,
                    MenuExportVbt = Visibility.Collapsed,
                };
                connectNode.ChildNodes.Add(channelNode);
                _treeNodeCollcet.Add(channelNode.NodeId, channelNode);
                foreach (var device in channel.Value.Devices)
                {
                    var deviceNode = new TreeNode(idBuffer.Pop(), channelNode.NodeId, device.Key, NodeType.Device, deviceIcon)
                    {
                        MenuNewNodeVbt = Visibility.Visible,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Collapsed,
                        MenuExportVbt = Visibility.Collapsed,
                    };
                    channelNode.ChildNodes.Add(deviceNode);
                    _treeNodeCollcet.Add(deviceNode.NodeId, deviceNode);
                    foreach (var tagGroup in device.Value.TagGroups)
                    {
                        var tagGroupNode = new TreeNode(idBuffer.Pop(), deviceNode.NodeId, tagGroup.Key, NodeType.TagGroup, tagGroupIcon)
                        {
                            MenuNewNodeVbt = Visibility.Visible,
                            MenuDeleteVbt = Visibility.Visible,
                            MenuPropertyVbt = Visibility.Visible,
                            MenuImportVbt = Visibility.Visible,
                            MenuExportVbt = Visibility.Visible,
                        };
                        deviceNode.ChildNodes.Add(tagGroupNode);
                        _treeNodeCollcet.Add(tagGroupNode.NodeId, tagGroupNode);
                    }
                }
            }

            return connectNode;
        }

        private  TreeNode loadServerNode(int parentId,ServersConfig severConfig)
        {
            var serversNode = new TreeNode(idBuffer.Pop(), parentId, "Servers", NodeType.Servers, serverIcon)
            {
                MenuNewNodeVbt = Visibility.Visible,
                MenuDeleteVbt = Visibility.Collapsed,
                MenuPropertyVbt = Visibility.Collapsed,
                MenuImportVbt = Visibility.Collapsed,
                MenuExportVbt = Visibility.Collapsed,
            };
            _treeNodeCollcet.Add(serversNode.NodeId, serversNode);
            foreach (var server in severConfig.Items)
            {
                switch (server.Value.Option)
                {
                    case ServerOption.ModbusTCP:
                        var mdbusTCP = new TreeNode(idBuffer.Pop(), serversNode.NodeId, server.Key, NodeType.ServerItem, serverItemIcon)
                        {
                            MenuNewNodeVbt = Visibility.Visible,
                            MenuDeleteVbt = Visibility.Visible,
                            MenuPropertyVbt = Visibility.Visible,
                            MenuImportVbt = Visibility.Visible,
                            MenuExportVbt = Visibility.Visible,
                        };
                        serversNode.ChildNodes.Add(mdbusTCP);
                        break;
                    case ServerOption.ModbusRTU:
                        var mdbusRTU = new TreeNode(idBuffer.Pop(), serversNode.NodeId, server.Key, NodeType.ServerItem, serverItemIcon)
                        {
                            MenuNewNodeVbt = Visibility.Visible,
                            MenuDeleteVbt = Visibility.Visible,
                            MenuPropertyVbt = Visibility.Visible,
                            MenuImportVbt = Visibility.Visible,
                            MenuExportVbt = Visibility.Visible,
                        };
                        serversNode.ChildNodes.Add(mdbusRTU);
                        break;
                    case ServerOption.Freedom:
                        var freedom = new TreeNode(idBuffer.Pop(), serversNode.NodeId, server.Key, NodeType.ServerItem, serverItemIcon)
                        {
                            MenuNewNodeVbt = Visibility.Collapsed,
                            MenuDeleteVbt = Visibility.Visible,
                            MenuPropertyVbt = Visibility.Visible,
                            MenuImportVbt = Visibility.Collapsed,
                            MenuExportVbt = Visibility.Collapsed,
                        };
                        serversNode.ChildNodes.Add(freedom);
                        break;
                    default:
                        break;
                }
            }

            return serversNode;
        }
        private TreeNode loadAlarmsNode(int parentId, AlarmsConfig alarmsConfig)
        {
            var alarmsNode = new TreeNode(idBuffer.Pop(), parentId, "Alarms", NodeType.Alarms, alarmIcon)
            {
                MenuNewNodeVbt = Visibility.Visible,
                MenuDeleteVbt = Visibility.Collapsed,
                MenuPropertyVbt = Visibility.Collapsed,
                MenuImportVbt = Visibility.Visible,
                MenuExportVbt = Visibility.Visible,
            };
            _treeNodeCollcet.Add(alarmsNode.NodeId, alarmsNode);
         

            return alarmsNode;
        }
        private TreeNode loadRecordsNode(int parentId, RecordsConfig recordsConfig)
        {
            var recordNode = new TreeNode(idBuffer.Pop(), parentId, "Records", NodeType.Records, recordIcon)
            {
                MenuNewNodeVbt = Visibility.Visible,
                MenuDeleteVbt = Visibility.Collapsed,
                MenuPropertyVbt = Visibility.Collapsed,
                MenuImportVbt = Visibility.Collapsed,
                MenuExportVbt = Visibility.Collapsed,
            };
            _treeNodeCollcet.Add(recordNode.NodeId, recordNode);

            foreach (var record in recordsConfig.RecordGroup.Values)
            {
                var recordItem = new TreeNode(idBuffer.Pop(), recordNode.NodeId, record.Name, NodeType.RecordItem, recordItemIcon);
                recordNode.ChildNodes.Add(recordItem);
                _treeNodeCollcet.Add(recordItem.NodeId, recordItem);
            }

            return recordNode;
        }

        private void nodeMouseLeft(object obj)
        {
            TreeNode channelNode, deviceNode;

            TreeNode curreNode = (TreeNode)obj;

            ChannelConfig channel;
            DeviceConfig device;
            TagGroupConfig tagGroup;
            //TagConfig tag;

            NavigationParameters np = new NavigationParameters();
            switch (curreNode.Type)
            {
                case NodeType.Connectivity:
                   var client= _configDataServer.GetClient();
                    np.Add("ClientConfig", client);
                    _regionManager.RequestNavigate("DetailedListRegion", "ChannelListView", np);
                    break;
                case NodeType.Channel:
                    channel = _configDataServer.GetChannel(curreNode.NodeName);
                    np.Add("ChannelConfig", channel);
                    _regionManager.RequestNavigate("DetailedListRegion", "DeviceListView", np);
                    break;
                case NodeType.Device:
                    channelNode = _treeNodeCollcet[curreNode.ParentId];
                    channel = _configDataServer.GetChannel(channelNode.NodeName);
                    device = _configDataServer.GetDevice(channelNode.NodeName, curreNode.NodeName);
                    np.Add("ChannelConfig", channel);
                    np.Add("DeviceConfig", device);
                    _regionManager.RequestNavigate("DetailedListRegion", "TagGroupListView", np);
                    break;
                case NodeType.TagGroup:
                    deviceNode = _treeNodeCollcet[curreNode.ParentId];
                    channelNode= _treeNodeCollcet[deviceNode.ParentId];
                    channel = _configDataServer.GetChannel(channelNode.NodeName);
                    device = _configDataServer.GetDevice(channelNode.NodeName, deviceNode.NodeName);
                    tagGroup = _configDataServer.GetTagGroup(channelNode.NodeName, deviceNode.NodeName, curreNode.NodeName);
                    np.Add("ChannelConfig", channel);
                    np.Add("DeviceConfig", device);
                    np.Add("TagGroupConfig", tagGroup);
                    _regionManager.RequestNavigate("DetailedListRegion", "TagListView", np);
                    break;
                case NodeType.Servers:
                    var servers = _configDataServer.GetServers();
                    np.Add("ServersConfig", servers);
                    _regionManager.RequestNavigate("DetailedListRegion", "ServerListView", np);
                    break;
                case NodeType.ServerItem:
                    var serverItem = _configDataServer.GetServerItem(curreNode.NodeName);
                    np.Add("ServerItemConfig", serverItem);
                    _regionManager.RequestNavigate("DetailedListRegion", "TagBindingListView", np);
                    break;
                case NodeType.Alarms:
                    var alarms = _configDataServer.GetAlarms();
                    np.Add("AlarmsConfig", alarms);
                    _regionManager.RequestNavigate("DetailedListRegion", "AlarmListView", np);
                    break;
                case NodeType.Records:
                    var records = _configDataServer.GetRecords();
                    np.Add("RecordsConfig", records);
                    _regionManager.RequestNavigate("DetailedListRegion", "RecordListView", np);
                    break;
                case NodeType.RecordItem:
                    var recordItem = _configDataServer.GetRecordItem(curreNode.NodeName);
                    np.Add("RecordItemConfig", recordItem);
                    _regionManager.RequestNavigate("DetailedListRegion", "RecordTagsListView", np);
                    break;
                default:
                    break;
            }
        }
        private void nodeMouseRight(object obj)
        {

            NodeType curreNode = (NodeType)obj;
            switch (curreNode)
            {
                case NodeType.Connectivity:
                    NewNodeName = "Add Channel";
                    NewNodeIcon = channelIcon;
                    break;
                case NodeType.Channel:
                    NewNodeName = "Add Device";
                    NewNodeIcon = deviceIcon;

                    break;
                case NodeType.Device:
                    NewNodeName = "Add TagGroup";
                    NewNodeIcon = tagGroupIcon;
                    break;
                case NodeType.TagGroup:
                    NewNodeName = "Add Tag";
                    NewNodeIcon = tagIcon;
                    break;
                case NodeType.Servers:
                    NewNodeName = "Add Server";
                    NewNodeIcon = serverItemIcon;
                    break;
                case NodeType.ServerItem:
                    NewNodeName = "Add TagBinding";
                    NewNodeIcon = tagBindingIcon;
                    break;
                case NodeType.Alarms:
                    NewNodeName = "Add Alarm";
                    NewNodeIcon = alarmItemIcon;
                    break;
                case NodeType.Records:
                    NewNodeName = "Add Record";
                    NewNodeIcon = recordItemIcon;
                    break;
                case NodeType.RecordItem:
                    NewNodeName = "TagsManager";
                    NewNodeIcon = tagGroupIcon;
                    break;
                default:
                    break;
            }
        }
     
        private void openProperty(object obj)
        {
            TreeNode curreNode = (TreeNode)obj;
            TreeNode channelNode,deviceNode,serverNode;
            var dialogPara = new DialogParameters();

            switch (curreNode.Type)
            {
                case NodeType.Channel:
                    var channelViewDespatcher = new ChannelViewDespatcher(_regionManager, _configDataServer, _configDataServer.GetChannel(curreNode.NodeName));
                    dialogPara.Add("ViewDespatcher", channelViewDespatcher);
                    break;
                case NodeType.Device:
                    channelNode  = _treeNodeCollcet[curreNode.ParentId];
                    var deviceViewDespatcher = new DeviceViewDespatcher(_regionManager, _configDataServer, _configDataServer.GetChannel(channelNode.NodeName), _configDataServer.GetDevice(channelNode.NodeName, curreNode.NodeName));
                    dialogPara.Add("ViewDespatcher", deviceViewDespatcher);
                    break;
                case NodeType.TagGroup:
                    deviceNode = _treeNodeCollcet[curreNode.ParentId];
                    channelNode = _treeNodeCollcet[deviceNode.ParentId];
                    var tagGroupConfig = _configDataServer.GetTagGroup(channelNode.NodeName, deviceNode.NodeName, curreNode.NodeName);
                    var tagsViewDespatcher = new TagGroupViewDespatcher(_regionManager, _configDataServer, channelNode.NodeName, deviceNode.NodeName, tagGroupConfig);

                    dialogPara.Add("ViewDespatcher", tagsViewDespatcher);
                    break;
                case NodeType.ServerItem:
                    var serverItemConfig = _configDataServer.GetServerItem(curreNode.NodeName);
                    var serverViewDespatcher = new ServerItemViewDespatcher(_regionManager, _configDataServer, serverItemConfig);
                    dialogPara.Add("ViewDespatcher", serverViewDespatcher);

                    break;
                case NodeType.TagBinding:
                    serverNode = _treeNodeCollcet[curreNode.ParentId];
                    var tagBindingConfig = _configDataServer.GetTagBinding(serverNode.NodeName, curreNode.NodeName);
                    var tagBindingViewDespatcher = new TagBindingViewDespatcher(_regionManager, _configDataServer, serverNode.NodeName, tagBindingConfig);
                    dialogPara.Add("ViewDespatcher", tagBindingViewDespatcher);
                    break;
                case NodeType.RecordItem:
                    var recordItemConfig = _configDataServer.GetRecordItem(curreNode.NodeName);
                    var recordItemViewDespatcher = new RecordItemViewDespatcher(_regionManager, _configDataServer,recordItemConfig);
                    dialogPara.Add("ViewDespatcher", recordItemViewDespatcher);
                    break;
                default:
                    break;
            }
            _dialogService.ShowDialog("PropertyDialog", dialogPara, r => { });
        }
        private void addNode(object obj)
        {
            TreeNode curreNode = (TreeNode)obj;
            switch (curreNode.Type)
            {
                case NodeType.Connectivity:
                    addChannelNode(curreNode);
                    break;
                case NodeType.Channel:
                    addDeviceNode(curreNode);
                    break;
                case NodeType.Device:
                    addTagGroupNode(curreNode);
                    break;
                case NodeType.TagGroup:
                    addTagNode(curreNode);
                    break;
                case NodeType.Servers:
                    addServerItemNode(curreNode);
                    break;
                case NodeType.ServerItem:
                    addTagBinding(curreNode);
                    break;
                case NodeType.Alarms:
                    addAlarmItem(curreNode);
                    break;
                case NodeType.Records:
                    addRecordItem(curreNode);
                    break;
                case NodeType.RecordItem:
                    RecordTagManager(curreNode);
                    break;
                default:
                    break;
            }
        }

    

        private void addChannelNode(TreeNode curreNode)
        {
            ChannelViewDespatcher channelViewDespatcher = new ChannelViewDespatcher(_regionManager, _configDataServer);

            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", channelViewDespatcher);

            _dialogService.ShowDialog("BaseBuildDialog", dialogPara , r => 
            {
                if (r .Result==ButtonResult.OK)
                {
                    var config = channelViewDespatcher.ChannelConfig;
                    var name = config.Name;
                    Geometry icon = channelComPortIcon;
                    switch (config.DriverInformation.CommType)
                    {
                        case CommunicationType.Serialport:
                            icon = channelComPortIcon;
                            break;
                        case CommunicationType.Ethernet:
                            icon = channelEthentIcon;
                            break;
                        case CommunicationType.File:
                            break;
                        case CommunicationType.Memory:
                            break;
                        default:
                            break;
                    }
                    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.Channel, icon)
                    {
                        MenuNewNodeVbt = Visibility.Visible,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Collapsed,
                        MenuExportVbt = Visibility.Collapsed,
                    };
                    _treeNodeCollcet.Add(node.NodeId, node);
                    curreNode.ChildNodes.Add(node);
                }
            }
            );
        }
        private void addDeviceNode(TreeNode curreNode)
        {
            DeviceViewDespatcher deviceViewDespatcher = new DeviceViewDespatcher(_regionManager, _configDataServer, _configDataServer.GetChannel(curreNode.NodeName));
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", deviceViewDespatcher);
            _dialogService.ShowDialog("BaseBuildDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var config = deviceViewDespatcher.DeviceConfig;
                    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.Device, deviceIcon)
                    {
                        MenuNewNodeVbt = Visibility.Visible,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Collapsed,
                        MenuExportVbt = Visibility.Collapsed,
                    };
                    _treeNodeCollcet.Add(node.NodeId, node);
                    curreNode.ChildNodes.Add(node);
                }
            }

        );
        }
        private void addTagGroupNode(TreeNode curreNode)
        {
            TreeNode channelNode;
            channelNode = _treeNodeCollcet[curreNode.ParentId];
            TagGroupViewDespatcher tagGroupViewDespatcher = new TagGroupViewDespatcher(_regionManager, _configDataServer, channelNode.NodeName,curreNode.NodeName);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", tagGroupViewDespatcher);
            _dialogService.ShowDialog("BaseBuildDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var config = tagGroupViewDespatcher.Config;
                    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.TagGroup, tagGroupIcon)
                    {
                        MenuNewNodeVbt = Visibility.Visible,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Visible,
                        MenuExportVbt = Visibility.Visible,
                    };
                    _treeNodeCollcet.Add(node.NodeId, node);
                    curreNode.ChildNodes.Add(node);
                }
            }
            );
        }
        private void addTagNode(TreeNode curreNode)
        {
            TreeNode channelNode, deviceNode;

            deviceNode = _treeNodeCollcet[curreNode.ParentId];
            channelNode = _treeNodeCollcet[deviceNode.ParentId];

            var tagViewDespatcher = new TagViewDespatcher(_regionManager, _configDataServer, channelNode.NodeName, deviceNode.NodeName,curreNode.NodeName);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", tagViewDespatcher);
            _dialogService.ShowDialog("BaseBuildDialog", dialogPara, r =>
            {
                //if (r.Result == ButtonResult.OK)
                //{
                //    var config = tagGroupViewDespatcher.Config;
                //    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.Tags, tagGroupIcon)
                //    {
                //        MenuNewNodeVbt = Visibility.Collapsed,
                //        MenuDeleteVbt = Visibility.Visible,
                //        MenuPropertyVbt = Visibility.Visible,
                //        MenuImportVbt = Visibility.Visible,
                //        MenuExportVbt = Visibility.Visible,
                //    };
                //    _treeNodeCollcet.Add(node.NodeId, node);
                //    curreNode.ChildNodes.Add(node);
                //}
            }
            );
        }

        private void addServerItemNode(TreeNode curreNode)
        {
            var serverItemViewDespatcher = new ServerItemViewDespatcher( _regionManager, _configDataServer);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", serverItemViewDespatcher);
            _dialogService.ShowDialog("BaseBuildDialog", dialogPara, r =>
            {
                if(r.Result == ButtonResult.OK)
                {
                    var config = serverItemViewDespatcher.Config;
                    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.ServerItem, serverItemIcon);
                    switch (config.Option)
                    {
                        case ServerOption.ModbusTCP:
                            node.MenuNewNodeVbt = Visibility.Visible;
                            node.MenuDeleteVbt = Visibility.Visible;
                            node.MenuPropertyVbt = Visibility.Visible;
                            node.MenuImportVbt = Visibility.Visible;
                            node.MenuExportVbt = Visibility.Visible;
                            break;
                        case ServerOption.ModbusRTU:
                            node.MenuNewNodeVbt = Visibility.Visible;
                            node.MenuDeleteVbt = Visibility.Visible;
                            node.MenuPropertyVbt = Visibility.Visible;
                            node.MenuImportVbt = Visibility.Visible;
                            node.MenuExportVbt = Visibility.Visible;
                            break;
                        case ServerOption.Freedom:
                            node.MenuNewNodeVbt = Visibility.Collapsed;
                            node.MenuDeleteVbt = Visibility.Visible;
                            node.MenuPropertyVbt = Visibility.Visible;
                            node.MenuImportVbt = Visibility.Collapsed;
                            node.MenuExportVbt = Visibility.Collapsed;
                            break;
                        default:
                            break;
                    }

                    _treeNodeCollcet.Add(node.NodeId, node);
                    curreNode.ChildNodes.Add(node);
                }
            }
            );
        }
        private void addTagBinding(TreeNode curreNode)
        {
            var tagBindingViewDespatcher = new TagBindingViewDespatcher(_regionManager, _configDataServer,curreNode.NodeName);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", tagBindingViewDespatcher);
            _dialogService.ShowDialog("BaseBuildDialog", dialogPara, r =>
            {
                //if (r.Result == ButtonResult.OK)
                //{
                //    var config = tagBindingViewDespatcher.Config;
                //    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.DestTagName, NodeType.TagBinding, tagBindingIcon)
                //    {
                //        MenuNewNodeVbt = Visibility.Collapsed,
                //        MenuDeleteVbt = Visibility.Visible,
                //        MenuPropertyVbt = Visibility.Visible,
                //        MenuImportVbt = Visibility.Collapsed,
                //        MenuExportVbt = Visibility.Collapsed
                //    };
                //    _treeNodeCollcet.Add(node.NodeId, node);
                //    curreNode.ChildNodes.Add(node);
                //}
            }
            );
        }
        private void addAlarmItem(TreeNode curreNode)
        {
            var alarmItemViewDespatcher = new AlarmItemViewDespatcher(_regionManager, _configDataServer);
            var dialogPara = new DialogParameters();
            dialogPara.Add("ViewDespatcher", alarmItemViewDespatcher);
            _dialogService.ShowDialog("BaseBuildDialog", dialogPara, r =>
            {
                //if (r.Result == ButtonResult.OK)
                //{
                //    var config = tagBindingViewDespatcher.Config;
                //    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.DestTagName, NodeType.TagBinding, tagBindingIcon)
                //    {
                //        MenuNewNodeVbt = Visibility.Collapsed,
                //        MenuDeleteVbt = Visibility.Visible,
                //        MenuPropertyVbt = Visibility.Visible,
                //        MenuImportVbt = Visibility.Collapsed,
                //        MenuExportVbt = Visibility.Collapsed
                //    };
                //    _treeNodeCollcet.Add(node.NodeId, node);
                //    curreNode.ChildNodes.Add(node);
                //}
            }
            );
        }
        private void addRecordItem(TreeNode curreNode)
        {
            var recordItemViewDespatcher = new RecordItemViewDespatcher(_regionManager, _configDataServer);
            var dialogPara = new DialogParameters
            {
                { "ViewDespatcher", recordItemViewDespatcher }
            };
            _dialogService.ShowDialog("BaseBuildDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var config = recordItemViewDespatcher.Config;
                    var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.RecordItem, recordItemIcon)
                    {
                        MenuNewNodeVbt = Visibility.Visible,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Visible,
                        MenuExportVbt = Visibility.Visible
                    };
                    _treeNodeCollcet.Add(node.NodeId, node);
                    curreNode.ChildNodes.Add(node);
                }
            }
            );
        }
        private void RecordTagManager(TreeNode curreNode)
        {
            var tagNames = _configDataServer.GetRecordItem(curreNode.NodeName).TagNames;
            var dialogPara = new DialogParameters
            {
                { "RecordItem", curreNode.NodeName },
                { "TagNames",tagNames }
            };
            _dialogService.ShowDialog("RecordTagsManagerView", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    //var config = recordItemViewDespatcher.Config;
                    //var node = new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.RecordItem, recordItemIcon)
                    //{
                    //    MenuNewNodeVbt = Visibility.Visible,
                    //    MenuDeleteVbt = Visibility.Visible,
                    //    MenuPropertyVbt = Visibility.Visible,
                    //    MenuImportVbt = Visibility.Visible,
                    //    MenuExportVbt = Visibility.Visible
                    //};
                    //_treeNodeCollcet.Add(node.NodeId, node);
                    //curreNode.ChildNodes.Add(node);
                }
            }
            );
        }
        private void deleteNode(object obj)
        {
            TreeNode curreNode = (TreeNode)obj;
            TreeNode parentNode;
            TreeNode channelNode;
            TreeNode deviceNode;
            TreeNode serverNode;
            switch (curreNode.Type)
            {
                case NodeType.Connectivity:
                    break;
                case NodeType.Channel:
                    _configDataServer.RemoveChannel(curreNode.NodeName);
                    break;
                case NodeType.Device:
                    _treeNodeCollcet.TryGetValue(curreNode.ParentId, out channelNode);
                    _configDataServer.RemoveDevice(channelNode.NodeName, curreNode.NodeName);
                    break;
                case NodeType.TagGroup:
                    _treeNodeCollcet.TryGetValue(curreNode.ParentId, out deviceNode);
                    _treeNodeCollcet.TryGetValue(deviceNode.ParentId, out channelNode);
                    _configDataServer.RemoveTagGroup(channelNode.NodeName, deviceNode.NodeName, curreNode.NodeName);
                    break;
                case NodeType.ServerItem:
                    _configDataServer.RemoveServerItem(curreNode.NodeName);
                    break;
                case NodeType.RecordItem:
                    _configDataServer.RemoveRecordItem(curreNode.NodeName);
                    break;
                //case NodeType.TagBinding:
                //    _treeNodeCollcet.TryGetValue(curreNode.ParentId, out serverNode);
                //    _configDataServer.RemoveTagBinding(serverNode.NodeName, curreNode.NodeName);
                //    break;
                default:
                    break;
            }
            _treeNodeCollcet.TryGetValue(curreNode.ParentId, out parentNode);
            parentNode.ChildNodes.Remove(curreNode);
        }
        ///// <summary>
        ///// 查找树节点
        ///// </summary>
        ///// <param name="tnParent">父节点</param>
        ///// <param name="nodeId">节点Id</param>
        ///// <returns></returns>
        //private TreeNode findNode(TreeNode tnParent, int  nodeId)
        //{

        //    if (tnParent == null) return null;

        //    if (tnParent.NodeId == nodeId) return tnParent;

        //    TreeNode tnRet = null;

        //    foreach (TreeNode tn in tnParent.ChildNodes)
        //    {

        //        tnRet = findNode(tn, nodeId);

        //        if (tnRet != null) break;

        //    }

        //    return tnRet;

        //}
    }
}
