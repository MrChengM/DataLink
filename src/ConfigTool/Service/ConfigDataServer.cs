using DataServer.Config;
using DataServer.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Service
{
    public class ConfigDataServer : IConfigDataServer
    {

        private Dictionary<string, DriverInfo> _driverInfos;
        private ProjectConfig _projectConfig ;


        public ConfigDataServer()
        {
            _driverInfos = new Dictionary<string, DriverInfo>();
            _projectConfig = new ProjectConfig();
        }

        public Dictionary<string, DriverInfo> DriverInfos => _driverInfos;

        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="channnel"></param>
        public void AddChannel(ChannelConfig channnel)
        {
            if (_projectConfig.Client.Channels?.ContainsKey(channnel.Name)==false)
            {
                _projectConfig.Client.Channels.Add(channnel.Name,channnel);
            };
        }
        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="device"></param>
        public void AddDevice(string channelName, DeviceConfig device)
        {
            var channel = GetChannel(channelName);
            if (channel!=null)
            {
                if (!channel.Devices.ContainsKey(device.Name))
                {
                    channel.Devices.Add(device.Name, device);
                }
            }
        }
        /// <summary>
        /// 添加驱动信息
        /// </summary>
        /// <param name="info"></param>
        public void AddDriverInfo(DriverInfo info)
        {
            if (!_driverInfos.ContainsKey(info.Description))
            {
                _driverInfos.Add(info.Description, info);
            }
        }
        /// <summary>
        /// 添加标签组
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroup"></param>
        public void AddTagGroup(string channelName, string deviceName, TagGroupConfig tagGroup)
        {
            var device = GetDevice(channelName, deviceName);
            if (device != null)
            {
                if (!device.TagGroups.ContainsKey(tagGroup.Name))
                {
                    device.TagGroups.Add(tagGroup.Name,tagGroup);
                }
            }
        }
        public bool ConnectServer()
        {
            throw new NotImplementedException();
        }

        public bool DisconnectServer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取通道配置
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public ChannelConfig GetChannel(string channelName)
        {
            ChannelConfig result;
            _projectConfig.Client.Channels.TryGetValue(channelName, out result);
            return result;
        }

        public DeviceConfig GetDevice(string channelName, string deviceName)
        {
            throw new NotImplementedException();
        }

        public DriverInfo GetDriverInfo(string name)
        {
            DriverInfo result;
            _driverInfos.TryGetValue(name, out result);
            return result;
        }

        //private bool isExit(string driverName)
        //{
        //    return _driverInfos.ContainsKey(driverName);
        //}
        public TagGroupConfig GetTagGroup(string channelName, string deviceName, string TagGroupName)
        {
            throw new NotImplementedException();
        }

        public bool Load()
        {
            throw new NotImplementedException();
        }

        public bool Open(string path)
        {
            throw new NotImplementedException();
        }

        public void RemoveChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public void RemoveDevice(string channelName, string DeviceName)
        {
            throw new NotImplementedException();
        }

        public void RemoveTagGroup(string channelName, string DeviceName, string TagGroupName)
        {
            throw new NotImplementedException();
        }

        //public Dictionary<string,DriverInfo> ResigterDLL(string path)
        //{
        //   return _driverInfos = ReflectionOperate.GetInfos(path);
        //}

        public bool Save(string path)
        {
            throw new NotImplementedException();
        }

        public bool SaveAs(string path)
        {
            throw new NotImplementedException();
        }

        public bool Updata()
        {
            throw new NotImplementedException();
        }
    }
}
