using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DataServer;
using Prism.Regions;
using Prism.Services.Dialogs;
using DataServer.Config;
using ConfigTool.Models;

namespace ConfigTool.ViewModels
{
    public class EthernetConfigViewModel : BindableBase,INavigationAware, IRegionMemberLifetime
    {
        private IEventAggregator _ea;
        private EthernetSetUp _ethernetSetUp;
        private ComPhyLayerSetting _comunicationSetUp;

        #region Property
        private ObservableCollection<string> networkAdapters;
        public ObservableCollection<string> NetworkAdapters
        {
            get { return networkAdapters; }
            set { networkAdapters = value; }
        }

        private string networkAdapter = "127.0.0.1";

        public string NetworkAdapter
        {
            get { return networkAdapter; }
            set
            {
                SetProperty(ref networkAdapter, value,"NetworkAdapter");
            }
        }

        private int portNumber = 502;

        public int PortNumber
        {
            get { return portNumber; }
            set
            {
                SetProperty(ref portNumber, value,"PortNumber");
            }
        }

        private ObservableCollection<string> protocols;

        public ObservableCollection<string> Protocols
        {
            get { return protocols; }
            set { protocols = value; }
        }


        private string protocol ="Tcp";

        public string Protocol
        {
            get { return protocol; }
            set
            {
                SetProperty(ref protocol, value,"Protocol");
            }
        }

        private bool isFristIn = true;
        private bool buildMode;

        public bool KeepAlive => !buildMode;

        #endregion

        public EthernetConfigViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            _ethernetSetUp = new EthernetSetUp();
            protocols = new ObservableCollection<string>() { ProtocolType.Tcp.ToString(), ProtocolType.Udp.ToString() };
            networkAdapters = new ObservableCollection<string>(getNetworkAdapter());
            networkAdapters.Add("127.0.0.1");
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(r =>
            {
                if (r == ButtonResult.OK)
                {
                    _ethernetSetUp.LocalNetworkAdpt = networkAdapter;
                    _ethernetSetUp.PortNumber = portNumber;
                    ProtocolType temp;
                    if (Enum.TryParse(protocol, out temp))
                    {
                        _ethernetSetUp.ProtocolType = temp;
                    }
                   _comunicationSetUp.EthernetSet = _ethernetSetUp;
                }
               

            });
        }
        /// <summary>
        /// 获取本地网络适配信息
        /// </summary>
        /// <returns></returns>
        private List<string> getNetworkAdapter()
        {
            var result = new List<string>();
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adp in adapters)
            {
                if (adp.NetworkInterfaceType== NetworkInterfaceType.Wireless80211 | adp.NetworkInterfaceType==NetworkInterfaceType.Ethernet)
                {
                    if (adp.OperationalStatus== OperationalStatus.Up)
                    {
                        var ipProperty = adp.GetIPProperties();
                        var ipInfoCollect = ipProperty.UnicastAddresses;
                        foreach (var ipInfo in ipInfoCollect)
                        {
                            if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                result.Add(ipInfo.Address.MapToIPv4().ToString());
                            }
                        }
                    }
                }
            }
            return result;
        }
        #region INavigationAware
        //private void sendMessage()
        //{
        //    _ethernetSetUp.LocalNetworkAdpt = networkAdapter;
        //    _ethernetSetUp.PortNumber = portNumber;
        //    ProtocolType temp;
        //    if (Enum.TryParse(protocol, out temp))
        //    {
        //        _ethernetSetUp.ProtocolType = temp;
        //    }
        //}

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //sendMessage();
            if (isFristIn)
            {
                _comunicationSetUp = navigationContext.Parameters.GetValue<ComPhyLayerSetting>("ComunicationSetUp");
                buildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!buildMode)
                {
                    _ethernetSetUp = _comunicationSetUp.EthernetSet;
                    NetworkAdapter = _ethernetSetUp.LocalNetworkAdpt;
                    PortNumber = _ethernetSetUp.PortNumber;
                    Protocol = _ethernetSetUp.ProtocolType.ToString();
                    isFristIn = false;
                }
            }
         
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return !buildMode;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
        #endregion

    }
}
