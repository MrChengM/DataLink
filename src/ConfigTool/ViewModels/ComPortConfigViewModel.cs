using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Regions;
using Prism.Events;
using DataServer;
using System.IO.Ports;
using Prism.Services.Dialogs;
using DataServer.Config;
using ConfigTool.Models;

namespace ConfigTool.ViewModels
{
  public class ComPortConfigViewModel: BindableBase,INavigationAware, IRegionMemberLifetime
    {
        private IEventAggregator _ea;
        private SerialportSetUp _serialportSetUp;
        private ComPhyLayerSetting _comunicationSetUp;

        #region Property
        #region combox binding
        private ObservableCollection<string> comPorts;

        public ObservableCollection<string> ComPorts
        {
            get { return comPorts; }
            set { comPorts = value; }
        }

        private ObservableCollection<string> oddEventChecks;

        public ObservableCollection<string> OddEventChecks
        {
            get { return oddEventChecks; }
            set { oddEventChecks = value; }
        }

        private ObservableCollection<int> baudRates;

        public ObservableCollection<int> BaudRates
        {
            get { return baudRates; }
            set { baudRates = value; }
        }
        #endregion

        #region SetUp Data Binding
        private string comPort = "COM1";
        public string ComPort
        {
            get { return comPort; }
            set { SetProperty(ref comPort, value ,"ComPort"); }
        }

        private int baudRate=9600;

        public int BaudRate
        {
            get { return baudRate; }
            set { SetProperty(ref baudRate, value, "BaudRate"); 
            }
        }
        private int dataBit=8;
        public int DataBit
        {
            get { return dataBit; }
            set { SetProperty(ref dataBit, value, "DataBit"); }
        }

        private int stopBit=1;

        public int StopBit
        {
            get { return stopBit; }
            set { SetProperty(ref stopBit, value, "StopBit") ; }
        }

        private string oddEventCheck= "None";

        public string OddEventCheck
        {
            get { return oddEventCheck; }
            set{ SetProperty(ref oddEventCheck, value,"OddEventCheck");}
        }
        public bool isFristIn=true;

        #endregion
        private bool buildMode;
        public bool KeepAlive => !buildMode;

        #endregion

        public ComPortConfigViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            comPorts = new ObservableCollection<string>() { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6" };
            baudRates = new ObservableCollection<int>() { 1200, 2400, 4800, 9600, 19200, 115200 };
            oddEventChecks = new ObservableCollection<string> { "None", "Odd", "Even", "Mark", "Space" };
            _serialportSetUp = new SerialportSetUp();
            eventAggregator.GetEvent<ButtonConfrimEvent>().Subscribe(r =>
            {
                if (r==ButtonResult.OK)
                {
                    _serialportSetUp.ComPort = comPort;
                    _serialportSetUp.BuadRate = baudRate;
                    _serialportSetUp.DataBit = (byte)dataBit;
                    _serialportSetUp.StopBit = (StopBits)stopBit;
                    Parity temp;
                    Enum.TryParse(oddEventCheck, out temp);
                    _serialportSetUp.OddEvenCheck = temp;
                    _comunicationSetUp.SerialportSet = _serialportSetUp;
                }
            });
        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //sendMessage();
            if (isFristIn)
            {
                _comunicationSetUp = navigationContext.Parameters.GetValue<ComPhyLayerSetting>("ComunicationSetUp");
                buildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!buildMode)
                {
                    _serialportSetUp = _comunicationSetUp.SerialportSet;
                    ComPort = _serialportSetUp.ComPort;
                    BaudRate = _serialportSetUp.BuadRate;
                    DataBit = _serialportSetUp.DataBit;
                    StopBit = (byte)_serialportSetUp.StopBit;
                    OddEventCheck = _serialportSetUp.OddEvenCheck.ToString();
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
    }
}
