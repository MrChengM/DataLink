using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using GuiBase.Models;
using Prism.Ioc;
using GuiBase.Views;
using System.Windows;
using Prism.Regions;

namespace GuiBase.ViewModels
{
    public class StartUpWindowViewModel : BindableBase
    {
        private IEventAggregator _ea;
        private IContainerExtension _container;
        private IRegionManager _regionManager;
        private string _title = "StartUp";
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }
     public StartUpWindowViewModel(IContainerExtension container, IRegionManager regionManager, IEventAggregator ea)
        {
            _container = container;
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("LoadRegion", "LoadView");
            _ea = ea;
            _ea.GetEvent<PubSubEvent<AccoutLogOnResult>>().Subscribe(logResultCallBack);
            _ea.GetEvent<PubSubEvent<LoadResult>>().Subscribe(loadResultCallBack);

        }

        private void loadResultCallBack(LoadResult result)
        {
            if (result == LoadResult.Success)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _regionManager.RequestNavigate("LoadRegion", "LogOnView");
                });
            }
        }

        private void logResultCallBack(AccoutLogOnResult result)
        {
            if (result==AccoutLogOnResult.Success)
            {
                //App.Current.
                var mainWindow = _container.Resolve<MainWindow>();
                var regionManager = _container.Resolve<IRegionManager>();
                RegionManager.SetRegionManager(mainWindow, regionManager);
                RegionManager.UpdateRegions();
                mainWindow.Show();
                App.Current.MainWindow.Close();
                _ea.GetEvent<PubSubEvent<AccoutLogOnResult>>().Unsubscribe(logResultCallBack);

            }
            else
            {
               Application.Current.Shutdown();
            }
        }
    }
}
