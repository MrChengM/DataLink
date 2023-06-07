﻿using System;
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

namespace ConfigTool.ViewModels
{
    public class DeviceGeneralViewModel : BindableBase, INavigationAware
    {
        private IEventAggregator _ea;
        private ChannelConfig _channelConfig;
        private DeviceConfig _deviceConfig;

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


        private string deviceName = "Device1";

        public string DeviceName
        {
            get { return deviceName; }
            set { SetProperty(ref deviceName, value, "DeviceName"); }
        }


        private string deviceId="1";

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


        private int requestTimeOut=1000;

        public int RequestTimeOut
        {
            get { return requestTimeOut; }
            set { SetProperty(ref requestTimeOut, value, "RequestTimeOut"); }
        }


        private int retryTimes=3;

        public int RetryTimes
        {
            get { return retryTimes; }
            set { SetProperty(ref retryTimes, value, "RetryTimes"); }
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

        public DeviceGeneralViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            _ea.GetEvent<PubSubEvent<ButtonResult>>().Subscribe(r => 
            {
                if (r == ButtonResult.OK)
                {
                    _deviceConfig.ConnectTimeOut = ConnectTimeOut;
                    _deviceConfig.RequestTimeOut = RequestTimeOut;
                    _deviceConfig.RetryTimes = RetryTimes;
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
            _channelConfig = navigationContext.Parameters.GetValue<ChannelConfig>("ChannelConfig");
            _deviceConfig = navigationContext.Parameters.GetValue<DeviceConfig>("DeviceConfig");
            if (_channelConfig != null)
            {
                ChannelName = _channelConfig.Name;
                DriverInfo = _channelConfig.DriverInformation.FullName;
            }
            if (_deviceConfig!=null)
            {
                DeviceName = _deviceConfig.Name;
                DeviceId = _deviceConfig.ID;
                ConnectTimeOut = _deviceConfig.ConnectTimeOut;
                RequestTimeOut = _deviceConfig.RequestTimeOut;
                RetryTimes = _deviceConfig.RetryTimes;
                CurrentByteOrder = _deviceConfig.ByteOrder.ToString();
            }
        }
        #endregion

    }
}