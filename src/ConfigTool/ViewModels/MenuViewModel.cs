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

namespace ConfigTool.ViewModels
{
    public class MenuViewModel:BindableBase
    {

        private IDialogService _dialogService;
        //private Dictionary<string, DriverInfo> _driverInfos;

        #region ICommand

        #region MenuItems Runtime
        private ICommand updataCommand;
        public ICommand UpdataCommand
        {
            get { return updataCommand; }
            set { updataCommand = value; }
        }

        private ICommand connectCommand;

        public ICommand ConnectCommand
        {
            get { return connectCommand; }
            set { connectCommand = value; }
        }

        private ICommand disconnectCommand;

        public ICommand DisConnectCommand
        {
            get { return disconnectCommand; }
            set { disconnectCommand = value; }
        }

        private ICommand registerCommand;

        public ICommand RegisterCommand
        {
            get { return registerCommand; }
            set { registerCommand = value; }
        }

        #endregion
        #endregion

        public MenuViewModel(IDialogService dialogService)
        {
            updataCommand = new DelegateCommand(updata);
            connectCommand = new DelegateCommand(connect);
            disconnectCommand = new DelegateCommand(disconnect);
            registerCommand = new DelegateCommand(register);
            _dialogService = dialogService;
            //_driverInfos = ContainerLocator.Container.Resolve<Dictionary<string, DriverInfo>>();
        }

        private void register()
        {
            _dialogService.ShowDialog("RegisterDialog",new DialogParameters(),  r=> 
            {
                if (r.Result==ButtonResult.OK)
                {
                    if (r.Parameters.Count!=0)
                    {
                        var driverInfors = r.Parameters.GetValue<Dictionary<string,DriverInfo>>("DriverInfors");
                        foreach (var infor in driverInfors)
                        {
                            if (!GlobalVar.DriverInfos.ContainsKey(infor.Key))
                                GlobalVar.DriverInfos.Add(infor.Key,infor.Value);
                        }
                    }
                }
            }
            );
        }
        private void disconnect()
        {
            //throw new NotImplementedException();
        }

        private void connect()
        {
            //throw new NotImplementedException();
        }

        private void updata()
        {
            //throw new NotImplementedException();
        }
    }
}
