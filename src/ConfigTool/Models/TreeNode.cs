using System.Collections.ObjectModel;
using System.Windows.Media;
using Prism.Mvvm;
using System.Windows;

namespace ConfigTool.Models
{
    public class TreeNode:BindableBase
    {

        private int nodeId;

        public int NodeId
        {
            get { return nodeId; }
            set { nodeId = value; }
        }

        private int parentId;

        public int ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        private string nodeName;

        public string NodeName
        {
            get { return nodeName; }
            set {
                SetProperty(ref nodeName,value,"NodeName");
               }
        }
        private NodeType type;

        public NodeType Type
        {
            get { return type; }
            set { type = value; }
        }

        private ObservableCollection<TreeNode> childNodes;

        public ObservableCollection<TreeNode> ChildNodes
        {
            get { return childNodes; }
            set
            {
                SetProperty(ref childNodes, value, "ChildNodes");
            }
        }

        private Geometry icon;


        public Geometry Icon
        {
            get { return icon; }
            set {
                SetProperty(ref icon, value, "Icon");
            }
        }
        #region MenuItem Visibility Property
        private Visibility menuNewNodeVbt;

        public Visibility MenuNewNodeVbt
        {
            get { return menuNewNodeVbt; }
            set
            {
                SetProperty(ref menuNewNodeVbt, value, "MenuNewNodeVbt");
            }
        }

        private Visibility menuDeleteVbt;

        public Visibility MenuDeleteVbt
        {
            get { return menuDeleteVbt; }
            set
            {
                SetProperty(ref menuDeleteVbt, value, "MenuDeleteVbt");
            }
        }
        private Visibility menuPropertyVbt;

        public Visibility MenuPropertyVbt
        {
            get { return menuPropertyVbt; }
            set
            {
                SetProperty(ref menuPropertyVbt, value, "MenuPropertyVbt");
            }
        }
        
        private Visibility menuImportVbt;

        public Visibility MenuImportVbt
        {
            get { return menuImportVbt; }
            set
            {
                SetProperty(ref menuImportVbt, value, "MenuImportVbt");
            }
        }
        private Visibility menuExportVbt;

        public Visibility MenuExportVbt
        {
            get { return menuExportVbt; }
            set
            {
                SetProperty(ref menuExportVbt, value, "MenuExportVbt");
            }
        }
        #endregion

        public TreeNode(int nodeId,int parentId,string nodeName,NodeType nodeType, Geometry icon) 
        {
            this.nodeId = nodeId;
            this.parentId = parentId;
            this.nodeName = nodeName;
            this.type = nodeType;
            this.icon = icon;
            childNodes = new ObservableCollection<TreeNode>();
        }
    }
}
