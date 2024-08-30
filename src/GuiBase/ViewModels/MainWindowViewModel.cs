using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;

namespace GuiBase.ViewModels
{
    public class MainWindowViewModel:BindableBase
    {
        private string _title = "Gui";
        public string Title {
            get { return _title; }
            set 
            {
                SetProperty(ref _title,value); 
            }
        }
    }
}
