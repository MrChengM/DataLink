using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using DataServer.Config;
using ConfigTool.Models;
using Prism.Regions;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;

namespace ConfigTool.ViewModels
{
    public class BuildNodeDialogViewModel : BindableBase, IDialogAware
    {

        #region Property
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _ea;
        private NodeType nodeType;

        private ChannelConfig _channelConfig;
        private DeviceConfig _deviceConfig;


        private ObservableCollection<PropertyOptionItem> optionItems;
        /// <summary>
        /// Property选项卡名称集合
        /// </summary>
        public ObservableCollection<PropertyOptionItem> OptionItems
        {
            get { return optionItems; }
            set { SetProperty(ref optionItems, value, "OptionItems"); }
        }
        //}
        //private ICommand optionSelectCommand;

        //public ICommand OptionSelectCommand
        //{
        //    get { return optionSelectCommand; }
        //    set { optionSelectCommand = value; }
        //}

        private ICommand closeDialogCommand;

        public ICommand CloseDialogCommand
        {
            get { return closeDialogCommand; }
            set { closeDialogCommand = value; }
        }

        #endregion

        #region IDialogAware
        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            var currentNode = parameters.GetValue<TreeNode>("CurrentNod");
            switch (currentNode.Type)
            {
                case NodeType.Channel:
                    break;
                case NodeType.Device:
                    break;
                case NodeType.Tags:
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
