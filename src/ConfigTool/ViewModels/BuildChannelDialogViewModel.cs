using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DataServer.Config;
using ConfigTool.Models;
using Prism.Regions;
using System.Windows.Input;
using Prism.Commands;
using Prism.Ioc;
using Prism.Events;

namespace ConfigTool.ViewModels
{
    public class BuildChannelDialogViewModel : BindableBase, IDialogAware
    {
        private readonly IRegionManager _regionManager;
        private ChannelConfig _config;
        private IEventAggregator _ea;

        NavigationParameters _np;

        private int pageNumber=0;
        public int PageNumber
        {
            get { return pageNumber; }
            set 
            {
                SetProperty(ref pageNumber, value,
                    ()=> 
                    {
                        if (pageNumber == 0)
                        {
                            PreviousEnable = false;
                            NextEnable = true;
                            ConfirmEnable = false;
                        }
                        else if (pageNumber == PAGENUMBERMAX-1)
                        {
                            PreviousEnable = true;
                            NextEnable = false;
                            ConfirmEnable = true;
                        }
                        else
                        {
                            PreviousEnable = true;
                            NextEnable = true;
                            ConfirmEnable = false;
                        }
                    });
            }
        }


        private const int PAGENUMBERMAX =2;
        #region Property
        private string _title = "Add Channel Wizard";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, "Title"); }
        }

        private string groupBoxHeader;

        public string GroupBoxHeader
        {
            get { return groupBoxHeader; }
            set { SetProperty(ref groupBoxHeader, value, "GroupBoxHeader"); }
        }


        private string warnInfo;

        public string WarnInfo
        {
            get { return warnInfo; }
            set { SetProperty(ref warnInfo, value, "WarnInfo"); }
        }

        #endregion
        #region Command

        private ICommand navigatePageCommand;

        public ICommand NavigatePageCommand
        {
            get { return navigatePageCommand; }
            set { navigatePageCommand = value; }
        }


        private ICommand closeDialogCommand;
        public ICommand CloseDialogCommand
        {
            get { return closeDialogCommand; }
            set { closeDialogCommand = value; }
        }


        private bool previousEnable=false;

        public bool PreviousEnable
        {
            get { return previousEnable; }
            set { SetProperty(ref previousEnable, value, "PreviousEnable"); }
        }

        private bool nextEnable=true;

        public bool NextEnable
        {
            get { return nextEnable; }
            set { SetProperty(ref nextEnable, value, "NextEnable"); }
        }

        private bool confirmEnable=false;

        public bool ConfirmEnable
        {
            get { return confirmEnable; }
            set { SetProperty(ref confirmEnable, value, "ConfirmEnable"); }
        }

        #endregion
        public BuildChannelDialogViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _ea = eventAggregator;
            _config = new ChannelConfig();
            closeDialogCommand = new DelegateCommand<string>(closeDialog);
            navigatePageCommand = new DelegateCommand<string>(navigatePage);
            _np = new NavigationParameters();
            _np.Add("ChannelConfig", _config);
            _np.Add("isNewOne", true);
        }

        private void navigatePage(string param)
        {
            if (param.ToLower() == "previous")
            {
                if (PageNumber > 0)
                {
                    PageNumber--;

                    if (!navigatePage())
                    {
                        PageNumber++;
                    }
                }
            }
            else if (param.ToLower() == "next")
            {
                if (PageNumber < PAGENUMBERMAX - 1)
                {
                    PageNumber++;
                    if (!navigatePage())
                    {
                        PageNumber--;
                    }
                }
            }
        }

        private bool navigatePage()
        {
            bool result=false;
            if (pageNumber == 0)
            {
                GroupBoxHeader = "General";
                _regionManager.RequestNavigate("BuildBaseRegion", "ChannelGeneralView", _np);
                result = true;
            }
            else if (pageNumber == 1)
            {
                if (_config.DriverInformation != null)
                {
                    switch (_config.DriverInformation.CommType)
                    {
                        case DataServer.CommunicationType.Serialport:
                            GroupBoxHeader = "ComPort";
                            _regionManager.RequestNavigate("BuildBaseRegion", "ComPortConfigView", _np);
                            result = true;
                            break;
                        case DataServer.CommunicationType.Ethernet:
                            GroupBoxHeader = "Ethernet";
                            _regionManager.RequestNavigate("BuildBaseRegion", "EthernetConfigView", _np);
                            result = true;
                            break;
                        case DataServer.CommunicationType.File:
                            break;
                        case DataServer.CommunicationType.Memory:
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }
        private void closeDialog(string parameter)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                result = ButtonResult.OK;
                _ea.GetEvent<ButtonConfrimEvent>().Publish(result);
                if (checkData())
                {
                    param.Add("ChannelConfig", _config);
                }
                else
                {
                    return;
                }
            }
            else if (parameter?.ToLower() == "cancel")
                result = ButtonResult.Cancel;
            RequestClose(new DialogResult(result, param));
            //_regionManager.Regions.Remove("BuildBaseRegion");
            _ea.GetEvent<ButtonConfrimEvent>().Clear();
        }
        #region IDialogAware
        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(default(IDialogResult));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            GroupBoxHeader = "General";
            _regionManager.RequestNavigate("BuildBaseRegion", "ChannelGeneralView", _np);
        }
        #endregion

        private bool checkData()
        {
            if (_config.Name == null || GlobalVar.ProjectConfig.Client.Channels.ContainsKey(_config.Name))
            {
                WarnInfo = "警告：请确认通道名称重复或未设置！！！";
                return false;
            }
            else
            {
                return true;
            }


        }
    }
}
