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

namespace ConfigTool.ViewModels
{
    public class TreeViewModel : BindableBase
    {
        #region Private Field

        private ObservableCollection<TreeNode> items;
        private readonly Geometry connectivityIcon = (Geometry)App.Current.FindResource("ConnectivityIcon1");
        private readonly Geometry channelComPortIcon = (Geometry)App.Current.FindResource("ComPortIcon1");
        private readonly Geometry channelEthentIcon = (Geometry)App.Current.FindResource("EternetIcon2");
        private readonly Geometry deviceIcon = (Geometry)App.Current.FindResource("DeviceIcon1");
        private readonly Geometry tagIcon = (Geometry)App.Current.FindResource("TagGroupIcon1");

        private Stack<int> idBuffer;
        #endregion

        #region ICommand
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

        private ICommand importTagsCommand;

        public ICommand ImportTagsCommand
        {
            get { return importTagsCommand; }
            set { importTagsCommand = value; }
        }

        private ICommand exportTagsCommand;

        public ICommand ExportTagsCommand
        {
            get { return exportTagsCommand; }
            set { exportTagsCommand = value; }
        }
        #endregion



        public ObservableCollection<TreeNode> Items
        {
            get { return items; }
            set { items = value; }
        }


        private IDialogService _dialogService;

        public TreeViewModel(IDialogService dialogService)
        {
            idBuffer = new Stack<int>(300);
            for (int i = 0; i < 1000; i++)
            {
                idBuffer.Push(i);
            }
            Items = new ObservableCollection<TreeNode>
            {
                new TreeNode(idBuffer.Pop(), -1, "Connectivity", NodeType.Connectivity, connectivityIcon)
                {   
                    MenuNewNodeVbt=Visibility.Visible,
                    MenuDeleteVbt=Visibility.Collapsed,
                    MenuPropertyVbt=Visibility.Collapsed,
                    MenuImportVbt=Visibility.Collapsed,
                    MenuExportVbt=Visibility.Collapsed,
                },
            };

            addNodeCommand = new DelegateCommand<object>(addNode);
            deleteNodeCommand = new DelegateCommand<object>(deleteNode);
            openPropertyCommand = new DelegateCommand<object>(openProperty);
            importTagsCommand= new DelegateCommand<object>(importTags);
            exportTagsCommand = new DelegateCommand<object>(exportTags);
            _dialogService = dialogService;
        }

        private void exportTags(object obj)
        {
        }

        private void importTags(object obj)
        {
        }

        private void openProperty(object obj)
        {
            TreeNode curreNode = (TreeNode)obj;
            var dialogPara = new DialogParameters();

            switch (curreNode.Type)
            {
                case NodeType.Channel:
                    dialogPara.Add("NodeType", NodeType.Channel);
                    dialogPara.Add("ChannelName", curreNode.NodeName);
                    break;
                case NodeType.Device:
                    var parentNode = FindNodeByValue(items[0], curreNode.ParentId);
                    dialogPara.Add("NodeType", NodeType.Device);
                    dialogPara.Add("DeviceName", curreNode.NodeName);
                    dialogPara.Add("ChannelName", parentNode.NodeName);
                    break;
                case NodeType.Tags:
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
                    addTagNode(curreNode);
                    break;
                case NodeType.Tags:
                    break;
                default:
                    break;
            }
        }

        private void addChannelNode(TreeNode curreNode)
        {
            var dialogPara = new DialogParameters();
            dialogPara.Add("CurreNode", curreNode);
            _dialogService.ShowDialog("BuildChannelDialog", dialogPara , r => 
            {
                if (r .Result==ButtonResult.OK)
                {
                    var config = r.Parameters.GetValue<ChannelConfig>("ChannelConfig");
                    GlobalVar.ProjectConfig.Client.Channels.Add(config.Name,config);
                    var name = config.Name;
                    Geometry icon= channelComPortIcon;
                    switch (config.DriverInformation.CommType)
                    {
                        case DataServer.CommunicationType.Serialport:
                            icon = channelComPortIcon;
                            break;
                        case DataServer.CommunicationType.Ethernet:
                            icon = channelEthentIcon;
                            break;
                        case DataServer.CommunicationType.File:
                            break;
                        case DataServer.CommunicationType.Memory:
                            break;
                        default:
                            break;
                    }
                    curreNode.ChildNodes.Add(new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.Channel, icon)
                    {
                        MenuNewNodeVbt = Visibility.Visible,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Collapsed,
                        MenuExportVbt = Visibility.Collapsed,
                    });
                }
            }
            );
        }
        private void addDeviceNode(TreeNode curreNode)
        {
            var dialogPara = new DialogParameters();
            dialogPara.Add("CurreNode", curreNode);
            _dialogService.ShowDialog("BuildDeviceDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var config = r.Parameters.GetValue<DeviceConfig>("DeviceConfig");
                    GlobalVar.ProjectConfig.Client.Channels[curreNode.NodeName].Devices.Add(config.Name, config);
                    curreNode.ChildNodes.Add(new TreeNode(idBuffer.Pop(), curreNode.NodeId, config.Name, NodeType.Device, deviceIcon)
                    {
                        MenuNewNodeVbt = Visibility.Visible,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Visible,
                        MenuExportVbt = Visibility.Visible,
                    });
                }
            }
            );
        }
        private void addTagNode(TreeNode curreNode)
        {
            var dialogPara = new DialogParameters();
            dialogPara.Add("CurreNode", curreNode);
            _dialogService.ShowDialog("BuildTagDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    curreNode.ChildNodes.Add(new TreeNode(idBuffer.Pop(), curreNode.NodeId, "Tag1", NodeType.Tags, channelComPortIcon)
                    {
                        MenuNewNodeVbt = Visibility.Collapsed,
                        MenuDeleteVbt = Visibility.Visible,
                        MenuPropertyVbt = Visibility.Visible,
                        MenuImportVbt = Visibility.Visible,
                        MenuExportVbt = Visibility.Visible,
                    });
                }
            }
            );

        }
        private void deleteNode(object obj)
        {
            TreeNode curreNode = (TreeNode)obj;
            GlobalVar.ProjectConfig.Client.Channels.Remove(curreNode.NodeName);
            var parentNode= FindNodeByValue(items[0], curreNode.ParentId);
            parentNode?.ChildNodes.Remove(curreNode);
        }

        private TreeNode FindNodeByValue(TreeNode tnParent, int  nodeId)
        {

            if (tnParent == null) return null;

            if (tnParent.NodeId == nodeId) return tnParent;

            TreeNode tnRet = null;

            foreach (TreeNode tn in tnParent.ChildNodes)
            {

                tnRet = FindNodeByValue(tn, nodeId);

                if (tnRet != null) break;

            }

            return tnRet;

        }
    }
}
