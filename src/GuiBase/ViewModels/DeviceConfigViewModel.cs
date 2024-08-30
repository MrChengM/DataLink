using Prism.Mvvm;
using System;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;

namespace GuiBase.ViewModels
{
    public class DeviceConfigViewModel : BindableBase, IDialogAware
    {
        public string Title => "Operation Record";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return  true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
