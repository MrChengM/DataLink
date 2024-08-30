using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Services.Dialogs;
using Prism.Events;
using System.Windows.Controls;
using GuiBase.Models;
using GuiBase.Services;

namespace GuiBase.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        private IDialogService _dialogService;
        public DelegateCommand<string> DialogClickCommand { get; set; }


        private bool menuIsChecked;

        public bool MenuIsChecked
        {
            get { return menuIsChecked; }
            set { SetProperty(ref menuIsChecked, value, "MenuIsChecked"); }
        }

        public MenuViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            DialogClickCommand = new DelegateCommand<string>(dialogClick);
        }

        private void dialogClick(string btnName)
        {
            var viewName = string.Concat(btnName, "View");
            MenuIsChecked = false;
            if (viewName=="LogOnView")
            {
                _dialogService.ShowDialog(viewName);
            }
            else
            {
                _dialogService.Show(viewName);

            }
        }
    }
}
