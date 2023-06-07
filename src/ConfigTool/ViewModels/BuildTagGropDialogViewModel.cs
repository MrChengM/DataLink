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
    public class BuildTagGropDialogViewModel : BindableBase, IDialogAware
    {
        #region IDialogAware
        public string Title => throw new NotImplementedException();

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
