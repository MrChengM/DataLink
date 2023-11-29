using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Windows.Input;
using DataServer.Config;
using ConfigTool.Models;
using ConfigTool.Service;
using Utillity.File;
using ConfigTool.Helper;

namespace ConfigTool.ViewModels
{
    public class MenuViewModel:BindableBase
    {
        private IDialogService _dialogService;
        private IConfigDataServer _configDataServer;
        public ISingleLoggerServer _singleLoggerServer;


        private LogFilter filter;

        public LogFilter Filter
        {
            get { return filter; }
            set { filter = value; }
        }
        //private Dictionary<string, DriverInfo> _driverInfos;
        private bool isConfigTool = true;

        public bool IsConfigTool
        {
            get { return isConfigTool; }
            set { SetProperty(ref isConfigTool, value, 
                ()=> { _singleLoggerServer.LogFilter.Source = value ? _singleLoggerServer.LogFilter.Source |= LogSource.ConfigTool : _singleLoggerServer.LogFilter.Source ^= LogSource.ConfigTool; },
                "IsConfigTool"); }
        }

        private bool isDataLink=true;

        public bool IsDataLink
        {
            get { return isDataLink; }
            set { SetProperty(ref isDataLink, value,
                                () => { _singleLoggerServer.LogFilter.Source = value ? _singleLoggerServer.LogFilter.Source | LogSource.DataLink : _singleLoggerServer.LogFilter.Source ^= LogSource.DataLink; },
                "IsDataLink"); }
        }

        private bool isInfo=true;

        public bool IsInfo
        {
            get { return isInfo; }
            set { SetProperty(ref isInfo, value, () => {
                if (value)
                {
                    _singleLoggerServer.LogFilter.Level = _singleLoggerServer.LogFilter.Level | LogLevel.Info;

                }
                else
                {
                    _singleLoggerServer.LogFilter.Level ^= LogLevel.Info;

                }
            }, "IsInfo"); }
        }

        private bool isError=true;

        public bool IsError
        {
            get { return isError; }
            set { SetProperty(ref isError, value, () => {
                if (value)
                {
                    _singleLoggerServer.LogFilter.Level = _singleLoggerServer.LogFilter.Level | LogLevel.Error;

                }
                else
                {
                    _singleLoggerServer.LogFilter.Level ^= LogLevel.Error;

                }
                 },
                 "IsError"); }
        }
        #region ICommand

        #region MenuItems Runtime
        private ICommand updataCommand;
        public ICommand UpdataCommand
        {
            get { return updataCommand; }
            set { updataCommand = value; }
        }

        private ICommand downLoadCommand;
        public ICommand DownLoadCommand
        {
            get { return downLoadCommand; }
            set { downLoadCommand = value; }
        }
        //private ICommand connectCommand;

        //public ICommand ConnectCommand
        //{
        //    get { return connectCommand; }
        //    set { connectCommand = value; }
        //}

        //private ICommand disconnectCommand;

        //public ICommand DisConnectCommand
        //{
        //    get { return disconnectCommand; }
        //    set { disconnectCommand = value; }
        //}

        private ICommand registerCommand;

        public ICommand RegisterCommand
        {
            get { return registerCommand; }
            set { registerCommand = value; }
        }

        #endregion
        #region  MenuItems File 
        private ICommand openCommand;
        public ICommand OpenCommand
        {
            get { return openCommand; }
            set { openCommand = value; }
        }
        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set { saveCommand = value; }
        }
        private ICommand saveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return saveAsCommand; }
            set { saveAsCommand = value; }
        }

      
        #endregion
        #endregion

        public MenuViewModel(IDialogService dialogService, IConfigDataServer configDataServer,ISingleLoggerServer singleLoggerServer)
        {
            _dialogService = dialogService;
            _configDataServer = configDataServer;
            _singleLoggerServer = singleLoggerServer;

            filter = new LogFilter();
            //Runtime ICommand
            updataCommand = new DelegateCommand(updata);
            downLoadCommand = new DelegateCommand(downLoad);
            //connectCommand = new DelegateCommand(connect);
            //disconnectCommand = new DelegateCommand(disconnect);
            registerCommand = new DelegateCommand(register);

            //File ICommand
            openCommand = new DelegateCommand(open);
            saveCommand = new DelegateCommand(save);
            saveAsCommand = new DelegateCommand(saveAs);

            //_driverInfos = ContainerLocator.Container.Resolve<Dictionary<string, DriverInfo>>();
        }

     

        private void saveAs()
        {

            _configDataServer.SaveAs();
        }

        private void save()
        {
            _configDataServer.Save();
        }

        private void open()
        {
            _configDataServer.Open();
        }

        private void register()
        {
            _dialogService.ShowDialog("RegisterDialog", new DialogParameters(), r =>
            {
                //if (r.Result==ButtonResult.OK)
                //{
                //    if (r.Parameters.Count!=0)
                //    {
                //        var driverInfors = r.Parameters.GetValue<Dictionary<string,DriverInfo>>("DriverInfors");
                //        foreach (var infor in driverInfors)
                //        {
                //            if (!GlobalVar.DriverInfos.ContainsKey(infor.Key))
                //                GlobalVar.DriverInfos.Add(infor.Key,infor.Value);
                //        }
                //    }
                //}
            }
            );
        }
        //private void disconnect()
        //{
        //    //throw new NotImplementedException();
        //}

        //private void connect()
        //{
        //    //throw new NotImplementedException();
        //}

        private void updata()
        {
            _configDataServer.Updata();
        }
        private void downLoad()
        {
            _configDataServer.DownLoad();
        }
    }
}
