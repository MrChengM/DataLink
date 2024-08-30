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
using ConfigTool.Service;
using System.Windows;

namespace ConfigTool.ViewModels
{
    public class BaseBuildDialogViewModel : BindableBase, IDialogAware
    {
        //private readonly IRegionManager _regionManager;
        //private ChannelConfig _channelConfig;
        //private DeviceConfig _deviceConfig;
        //private TagGroupConfig _tagGroupConfig;
        private IBuildViewDespatcher _viewDespatcher;
        
        private IEventAggregator _ea;
        //private IConfigDataServer _configDataServer;

        NavigationParameters _np;
        private const int PAGENUMBERMAX = 2;

        private bool _hasError = false;
        private int pageNumber=1;
        public int PageNumber
        {
            get { return pageNumber; }
            set 
            {
                SetProperty(ref pageNumber, value,
                    updataButtonEnable);
            }
        }

        void updataButtonEnable()
        {
            if (_viewDespatcher.MaxPageNumber > 1)
            {
                if (pageNumber == 1)
                {
                    PreviousEnable = false && !_hasError;
                    NextEnable = true && !_hasError;
                    ConfirmEnable = false && !_hasError;
                }
                else if (pageNumber == _viewDespatcher.MaxPageNumber)
                {
                    PreviousEnable = true && !_hasError;
                    NextEnable = false && !_hasError;
                    ConfirmEnable = true && !_hasError;
                }
                else
                {
                    PreviousEnable = true && !_hasError;
                    NextEnable = true && !_hasError;
                    ConfirmEnable = false && !_hasError;
                }
            }
            else
            {
                PreviousEnable = false && !_hasError;
                NextEnable = false && !_hasError;
                ConfirmEnable = true && !_hasError;
            }
            
        }

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
        public BaseBuildDialogViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            _ea.GetEvent<PubSubEvent<bool>>().Subscribe(getHasError);
            closeDialogCommand = new DelegateCommand<string>(closeDialog);
            navigatePageCommand = new DelegateCommand<string>(navigatePage);
            _np = new NavigationParameters();

        }

        private void getHasError(bool hasError)
        {
            _hasError = hasError;
            updataButtonEnable();
        }

        private void navigatePage(string param)
        {
            string content;
            if (param.ToLower() == "previous")
            {
                if (PageNumber > 1)
                {
                    PageNumber--;

                    if (!_viewDespatcher.Navigate(PageNumber,out content))
                    {
                        PageNumber++;
                    }
                    else
                    {
                        GroupBoxHeader = content;
                    }
                }
            }
            else if (param.ToLower() == "next")
            {
                if (PageNumber < _viewDespatcher.MaxPageNumber)
                {
                    PageNumber++;
                    if (!_viewDespatcher.Navigate(PageNumber,out content))
                    {
                        PageNumber--;
                    }
                    else
                    {
                        GroupBoxHeader = content;
                    }
                }
            }
        }

        private void closeDialog(string parameter)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                result = ButtonResult.OK;
                _ea.GetEvent<ButtonConfrimEvent>().Publish(result);
                if (!_viewDespatcher.AddConfig())
                {
                    MessageBox.Show("Add Config error");
                    return;
                };
            }
            else if (parameter?.ToLower() == "cancel")
            {
                result = ButtonResult.Cancel;
            }

            RequestClose(new DialogResult(result, param));
            //_ea.GetEvent<ButtonConfrimEvent>().Clear();
        }
        #region IDialogAware
        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _ea.GetEvent<ButtonConfrimEvent>().Clear();
            _ea.GetEvent<PubSubEvent<bool>>().Unsubscribe(getHasError);

            //RequestClose?.Invoke(default(IDialogResult));
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            _viewDespatcher = parameters.GetValue<IBuildViewDespatcher>("ViewDespatcher");
            if (_viewDespatcher != null)
            {
                Title = $"Add {_viewDespatcher.Title} Wizard";
                if (_viewDespatcher.ReturnHomePage(out int number, out string content))
                {
                    PageNumber = number;
                    GroupBoxHeader = content;
                }
                updataButtonEnable();

            }
        }
        #endregion
    }
}
