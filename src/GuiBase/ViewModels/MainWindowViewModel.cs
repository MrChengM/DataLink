using GuiBase.Views;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;


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

        public MainWindowViewModel( IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("MenuListRegion", typeof(Menu));
            regionManager.RegisterViewWithRegion("NavigtionRegion", typeof(NavigationList));
            regionManager.RegisterViewWithRegion("HeaderRegion", typeof(Header));
            regionManager.RegisterViewWithRegion("AlarmViewRegion", typeof(AlarmLiteView));
            //regionManager.RegisterViewWithRegion("BaseViewRegion", typeof(ViewA));
        }
    }
}
