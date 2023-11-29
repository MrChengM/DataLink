using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Ioc;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using DataServer.Config;
using System.Collections.ObjectModel;
using ConfigTool.Models;
using ConfigTool.Service;
using DataServer;
namespace ConfigTool.ViewModels
{
    class ServerItemGeneralViewModel : BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private ServerItemConfig _config;
        private IConfigDataServer _configDataServer;

        private string name="Server01";

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }


        private string address= "127.0.0.1";

        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value, "Address"); }
        }

        private bool buildMode;

        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }

        private List<string> serverOptions;

        public List<string> ServerOptions
        {
            get { return serverOptions; }
            set { SetProperty(ref serverOptions, value, "ServerOptions"); }
        }

        private string currentServer;

        public string CurrentServer
        {
            get { return currentServer; }
            set { SetProperty(ref currentServer, value, ()=> {
                if (BuildMode)
                {
                    if (Enum.TryParse(value, out ServerOption option))
                    {
                        _config.Option = option;
                    }
                }
            }, "CurrentServer"); }
        }

        private bool isFristIn = true;

        public ServerItemGeneralViewModel(IEventAggregator eventAggregator, IConfigDataServer configDataServer)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);
            _configDataServer = configDataServer;

            serverOptions = new List<string>();
            foreach (var type in Enum.GetNames(typeof(ServerOption)))
            {
                serverOptions.Add(type);
            }
            currentServer = serverOptions[0];
        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                if (buildMode)
                {
                    _config.Name = Name;
                    if (Enum.TryParse(CurrentServer, out ServerOption option))
                    {
                        _config.Option = option;
                    }
                }
                _config.Address = Address;
                
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (isFristIn)
            {
                _config = navigationContext.Parameters.GetValue<ServerItemConfig>("ServerItemConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    Name = _config.Name;
                    Address = _config.Address;
                    CurrentServer = _config.Option.ToString();
                }
                isFristIn = false;
            }
        }
    }
}
