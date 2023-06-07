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
using DataServer;

namespace ConfigTool.ViewModels
{
   public class BuildDeviceDialogViewModel:BindableBase, IDialogAware
    {
        #region Filed

        private DeviceConfig _deviceConfig;
        private IRegionManager _regionmanager;

        private ChannelConfig _channelConfig;
        #endregion
        #region Property
        private string _title = "Add Device Wizard";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, "Title"); }
        }



        private string deviceName="Device1";

        public string DeviceName
        {
            get { return deviceName; }
            set { SetProperty(ref deviceName, value, "DeviceName"); }
        }


        private string deviceId;

        public string DeviceId
        {
            get { return deviceId; }
            set { SetProperty(ref deviceId, value, "DeviceId"); }
        }

        private int connectTimeOut;

        public int ConnectTimeOut
        {
            get { return connectTimeOut; }
            set { SetProperty(ref connectTimeOut, value, "ConnectTimeOut"); }
        }


        private int requestTimeOut;

        public int RequestTimeOut
        {
            get { return requestTimeOut; }
            set { SetProperty(ref requestTimeOut, value, "RequestTimeOut"); }
        }


        private int retryTimes;

        public int RetryTimes
        {
            get { return retryTimes; }
            set { SetProperty(ref retryTimes, value, "RetryTimes"); }
        }

        private ObservableCollection<DeviceSpecailPropertyMVVM> specialProperties;

        public ObservableCollection<DeviceSpecailPropertyMVVM> SpecialProperties
        {
            get { return specialProperties; }
            set { SetProperty(ref specialProperties, value, "SpecialProperties"); }
        }

        private string warnInfo;
        public string WarnInfo
        {
            get { return warnInfo; }
            set { SetProperty(ref warnInfo, value, "WarnInfo"); }
        }

        private string currentByteOrder;

        public string CurrentByteOrder
        {
            get { return currentByteOrder; }
            set { SetProperty(ref currentByteOrder, value, "CurrentByteOrder"); }
        }

        private List<string> byteOrders;

        public List<string> ByteOrders
        {
            get { return byteOrders; }
            set { SetProperty(ref byteOrders, value, "ByteOrders"); }
        }
        #endregion

        public BuildDeviceDialogViewModel(IRegionManager regionManager)
        {
            _regionmanager = regionManager;
            _deviceConfig = new DeviceConfig();
            byteOrders = new List<string> { ByteOrder.BigEndian.ToString(), ByteOrder.BigEndianAndRervseWord.ToString(), ByteOrder.LittleEndian.ToString(), ByteOrder.None.ToString() };
            currentByteOrder = ByteOrder.None.ToString();
        }
        #region ICommand
        
        public ICommand CloseDialogCommand { get => new DelegateCommand<string>(closeDialog); }

        private void closeDialog(string parameter)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                result = ButtonResult.OK;
                if (checkData())
                {
                    _deviceConfig.Name = deviceName;
                    _deviceConfig.ID = deviceId;
                    _deviceConfig.ConnectTimeOut = connectTimeOut;
                    _deviceConfig.RequestTimeOut = requestTimeOut;
                    _deviceConfig.RetryTimes = retryTimes;
                    ByteOrder temp;
                    Enum.TryParse(CurrentByteOrder, out temp);
                    _deviceConfig.ByteOrder = temp;
                    _deviceConfig.SpecialProperties = new List<DeviceSpecialProperty>();
                    foreach (var sp in SpecialProperties)
                    {
                        _deviceConfig.SpecialProperties.Add(sp.Convert());
                    }
                    param.Add("DeviceConfig",_deviceConfig);
                }
                else
                {
                    return;
                }
            }
            else if (parameter?.ToLower() == "cancel")
                result = ButtonResult.Cancel;
            RequestClose(new DialogResult(result, param));
        }

        private bool checkData()
        {

            if (deviceName == null || _channelConfig.Devices.ContainsKey(deviceName))
            {
                WarnInfo = "警告：请确认通道名称设置是否正确！！！";
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region IDialogAware


        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var currentNode = parameters.GetValue<TreeNode>("CurreNode");
            GlobalVar.ProjectConfig.Client.Channels.TryGetValue(currentNode.NodeName, out _channelConfig);
            switch (_channelConfig.DriverInformation.CommType)
            {
                case CommunicationType.Serialport:
                    DeviceId="1";
                    break;
                case CommunicationType.Ethernet:
                    DeviceId = "255.255.255.0";
                    break;
                case CommunicationType.File:
                    break;
                case CommunicationType.Memory:
                    break;
                default:
                    break;
            }
            var DevicePropertyDes = _channelConfig.DriverInformation.DevicePropertyDes;
            SpecialProperties = new ObservableCollection<DeviceSpecailPropertyMVVM>();
            foreach (var item in DevicePropertyDes)
            {
                SpecialProperties.Add(new DeviceSpecailPropertyMVVM { PropertyName = item.Name ,PropertyValue="0"});
            }
        }
        #endregion

    }
}
