using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Ioc;
using Prism.Events;
using Prism.Regions;
using DataServer.Config;
using DataServer;
using Prism.Services.Dialogs;
using ConfigTool.Models;
using Utillity.Data;
using System.ComponentModel;

namespace ConfigTool.ViewModels
{
    public class DeviceGeneralViewModel : BindableBase, INavigationAware, IDataErrorInfo
    {
        private IEventAggregator _ea;
        private ChannelConfig _channelConfig;
        private DeviceConfig _deviceConfig;

        public bool BuildMode { get; private set; }

        private bool isFristIn = true;

        #region Property
        private string channelName;

        public string ChannelName
        {
            get { return channelName; }
            set { SetProperty(ref channelName, value, "ChannelName"); }
        }

        private string driverInfo;

        public string DriverInfo
        {
            get { return driverInfo; }
            set { SetProperty(ref driverInfo, value, "DriverInfo"); }
        }


        private string deviceName;

        public string DeviceName
        {
            get { return deviceName; }
            set { SetProperty(ref deviceName, value, "DeviceName"); }
        }
        private bool deviceIsEnable;

        public bool DeviceIsEnable
        {
            get { return deviceIsEnable; }
            set { SetProperty(ref deviceIsEnable, value, "DeviceIsEnable"); }
        }


        private string deviceId;

        public string DeviceId
        {
            get { return deviceId; }
            set { SetProperty(ref deviceId, value, "DeviceId"); }
        }

        private int connectTimeOut=3000;

        public int ConnectTimeOut
        {
            get { return connectTimeOut; }
            set { SetProperty(ref connectTimeOut, value, "ConnectTimeOut"); }
        }


        private int requestTimeOut = 1000;

        public int RequestTimeOut
        {
            get { return requestTimeOut; }
            set { SetProperty(ref requestTimeOut, value, "RequestTimeOut"); }
        }

        private int timing;

        public int Timing
        {
            get { return timing; }
            set { SetProperty(ref timing, value, "Timing"); }
        }
        private int retryTimes = 3;

        public int RetryTimes
        {
            get { return retryTimes; }
            set { SetProperty(ref retryTimes, value, "RetryTimes"); }
        }

        private string currentByteOrder = "None";

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
        #region IDataErrorInfo
        public string Error => null;


        private string[] errorMsgBuffer = new string[7];
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == "DeviceName")
                {

                    if (string.IsNullOrEmpty(DeviceName))
                    {
                        result = "Device Name can not null or empty !";
                    }
                    else if (!RegexCheck.IsString(DeviceName.ToString()))
                    {
                        result = "Device Name include special character !";
                    }
                    else if (_channelConfig.Devices.ContainsKey(DeviceName) && BuildMode)
                    {
                        result = "Device Name is Exit! ";
                    }
                    errorMsgBuffer[0] = result;
                }
                else if (columnName == "DeviceId")
                {
                    if (string.IsNullOrEmpty(DeviceId))
                    {
                        result = "Device ID can not null or empty !";
                    }
                    else if (_channelConfig?.DriverInformation.CommType == CommunicationType.Serialport)
                    {
                        if (!RegexCheck.IsNumber(DeviceId))
                        {
                            result = "DeviceId must be a number !";
                        }
                        else if (int.Parse(DeviceId) < 0)
                        {
                            result = "Value can not less than 0 !";
                        }
                    }
                    else if (_channelConfig?.DriverInformation.CommType == CommunicationType.Ethernet)
                    {
                        if (!RegexCheck.IsIPAddress(DeviceId))
                        {
                            result = "DeviceId must be a IP address !";
                        }
                    }
                    errorMsgBuffer[1] = result;
                }
                else if (columnName == "ConnectTimeOut")
                {
                    if (ConnectTimeOut < 0)
                    {
                        result = "Value can not less than 0 !";

                    }
                    errorMsgBuffer[2] = result;
                }
                else if (columnName == "RequestTimeOut")
                {
                    if (RequestTimeOut < 0)
                    {
                        result = "Value can not less than 0 !";
                    }
                    errorMsgBuffer[3] = result;
                }
                else if (columnName == "Timing")
                {
                    if (Timing < 0)
                    {
                        result = "Value can not less than 0 !";

                    }
                    errorMsgBuffer[4] = result;
                }
                else if (columnName == "RetryTimes")
                {
                    if (RetryTimes < 0)
                    {
                        result = "Value can not less than 0 !";

                    }
                    errorMsgBuffer[5] = result;
                }
                judgeHasError();
                return result;
            }

        }

        void judgeHasError()
        {
            bool hasError = false;
            foreach (var errorMsg in errorMsgBuffer)
            {
                if (errorMsg != string.Empty && errorMsg != null)
                {
                    hasError = true;
                    break;
                }
            }
            _ea.GetEvent<PubSubEvent<bool>>().Publish(hasError);

        }
        #endregion

        public DeviceGeneralViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(r => 
            {
                if (r == ButtonResult.OK)
                {
                    _deviceConfig.Name = DeviceName;
                    _deviceConfig.ID = DeviceId;
                    _deviceConfig.ConnectTimeOut = ConnectTimeOut;
                    _deviceConfig.RequestTimeOut = RequestTimeOut;
                    _deviceConfig.RetryTimes = RetryTimes;
                    _deviceConfig.Timing = Timing;
                    ByteOrder temp;
                    Enum.TryParse(CurrentByteOrder, out temp);
                    _deviceConfig.ByteOrder = temp;
                }
            });
            byteOrders = new List<string> { ByteOrder.BigEndian.ToString(), ByteOrder.BigEndianAndRervseWord.ToString(), ByteOrder.LittleEndian.ToString(), ByteOrder.None.ToString() };
        }
        #region INavigationAware
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
                _channelConfig = navigationContext.Parameters.GetValue<ChannelConfig>("ChannelConfig");
                _deviceConfig = navigationContext.Parameters.GetValue<DeviceConfig>("DeviceConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                DeviceIsEnable = BuildMode;
                ChannelName = _channelConfig.Name;
                DriverInfo = _channelConfig.DriverInformation.Description;
                if (!BuildMode)
                {
                    DeviceName = _deviceConfig.Name;
                    DeviceId = _deviceConfig.ID;
                    ConnectTimeOut = _deviceConfig.ConnectTimeOut;
                    RequestTimeOut = _deviceConfig.RequestTimeOut;
                    RetryTimes = _deviceConfig.RetryTimes;
                    CurrentByteOrder = _deviceConfig.ByteOrder.ToString();
                    Timing = _deviceConfig.Timing;
                }
                isFristIn = false;
            }
        }
        #endregion

    }
}
