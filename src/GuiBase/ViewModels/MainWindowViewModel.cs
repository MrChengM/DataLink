using GuiBase.Views;
using GuiBase.Services;
using GuiBase.Models;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;


namespace GuiBase.ViewModels
{
    public class MainWindowViewModel:BindableBase
    {
        private ILocalizationService _localizationService;
        private string _title ;
        public string Title {
            get { return _title; }
            set 
            {
                SetProperty(ref _title,value); 
            }
        }

        public MainWindowViewModel( IRegionManager regionManager,ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            translate();
            regionManager.RegisterViewWithRegion("MenuListRegion", typeof(Menu));
            regionManager.RegisterViewWithRegion("NavigtionRegion", typeof(NavigationList));
            regionManager.RegisterViewWithRegion("HeaderRegion", typeof(Header));
            regionManager.RegisterViewWithRegion("AlarmViewRegion", typeof(AlarmLiteView));
            //regionManager.RegisterViewWithRegion("BaseViewRegion", typeof(ViewA));
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }

        private void translate()
        {
            Title = _localizationService.Translate(TranslateCommonId.MainViewId);
        }
        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;
        }
    }
}
