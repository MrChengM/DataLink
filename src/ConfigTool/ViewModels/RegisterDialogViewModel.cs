using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using System.Windows;
using Prism.Services.Dialogs;
using DataServer.Config;
using ConfigTool.Service;
using Utillity.File;
using Utillity.Reflection;
using System.Reflection;
using DataServer;

namespace ConfigTool.ViewModels
{
   public class RegisterDialogViewModel:BindableBase, IDialogAware
    {

        private string _title = "Driver Register";
        private IConfigDataServer _configDataServer;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, "Title"); }
        }
        private string assemblyPath;

        public string AssemblyPath
        {
            get { return assemblyPath; }
            set { SetProperty(ref assemblyPath, value, "AssemblyPath"); }
        }


        private Dictionary<string,DriverInfo> dllInfors;

        public Dictionary<string,DriverInfo> DLLInfors
        {
            get { return dllInfors; }
            set { dllInfors = value; }
        }

        public event Action<IDialogResult> RequestClose;

        #region ICommand

        private ICommand openDLLCommand;

        public ICommand OpenDLLCommand
        {
            get { return openDLLCommand; }
            set { openDLLCommand = value; }
        }

        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

       
        #endregion

        public RegisterDialogViewModel(IConfigDataServer configDataServer)
        {
            _configDataServer = configDataServer;
            openDLLCommand = new DelegateCommand(openDLL);
        }

        private void openDLL()
        {
            string fileStyle = "DLL|*.dll";
            string filePath="";
            if (FileDialog.InputFile(ref filePath, fileStyle))
            {
                AssemblyPath = filePath;
                //getDriverInfors(filePath);
            } 
        }

        //private void getDriverInfors(string filePath)
        //{
        //    DLLInfors = _configDataServer.RegisterDriver(filePath);
        //}
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
        private void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "ok")
            {
                result = ButtonResult.OK;
                _configDataServer.RegisterDriver(assemblyPath);
            }
            else if (parameter?.ToLower() == "cancel")
                result = ButtonResult.Cancel;
            RaiseRequestClose(new DialogResult(result));
        }

        private void RaiseRequestClose(DialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
    }
}
