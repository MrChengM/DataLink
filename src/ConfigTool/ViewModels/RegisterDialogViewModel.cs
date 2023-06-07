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
using DataServer.Utillity;

namespace ConfigTool.ViewModels
{
   public class RegisterDialogViewModel:BindableBase, IDialogAware
    {

        private string _title = "Driver Register";

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

        private ICommand registerDLLCommand;

        public ICommand RegisterDLLCommand
        {
            get { return registerDLLCommand; }
            set { registerDLLCommand = value; }
        }

        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

       
        #endregion

        public RegisterDialogViewModel()
        {
            registerDLLCommand = new DelegateCommand(registerDLL);
        }

        private void registerDLL()
        {
            string fileStyle = "DLL|*.dll";
            string filePath="";
            if (FileOperate.InputFile(ref filePath, fileStyle))
            {
                AssemblyPath = filePath;
                dllInfors = ReflectionOperate.GetInfos(AssemblyPath);
            } 
        }

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
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "cancel")
                result = ButtonResult.Cancel;
            var param = new DialogParameters
            {
                { "DriverInfors", DLLInfors }
            };
            RaiseRequestClose(new DialogResult(result, param));
        }

        private void RaiseRequestClose(DialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
    }
}
