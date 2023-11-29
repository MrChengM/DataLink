using DataServer.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCFRestFullAPI.Models;
using Utillity.File;
using Utillity.Reflection;
using System.Reflection;
using DataServer;
using System.IO;

namespace WCFRestFullAPI.Service
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,MaxItemsInObjectGraph = 2147483647)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ConfigRestService : IConfigRestService
    {
        #region Field
        private ProjectConfig config;
        private Dictionary<string, DriverInfo> driverInfos;
        private const string CONFIGPATH = "../../../../conf";
        private const string PROJECTFILE_DEFAULT = "/ProjectConfig.Json";
        private const string DLLPATH = "../../../../dll";

        #endregion
        #region Property
        public ProjectConfig Config
        {
            get { return config; }
            //set { config = value; }
        }

        public Dictionary<string, DriverInfo> DriverInfos
        {
            get { return driverInfos; }
            //set { driverInfos = value; }
        }


        #endregion
        #region Method

        public ConfigRestService()
        {
            config = new ProjectConfig();
            driverInfos = new Dictionary<string, DriverInfo>();
            loadProjectConfig();
            loadDriverInfos();
        }
        private void loadProjectConfig()
        {
            config = JsonFunction.Load<ProjectConfig>(CONFIGPATH + PROJECTFILE_DEFAULT);

        }
        private void loadDriverInfos()
        {
            string[] driversPath = Directory.GetFiles(DLLPATH);
            foreach (var path in driversPath)
            {
                if (path.Contains("DataServer.dll"))
                {
                    continue;
                }
                var infos = getInfos(ReflectionFunction.GetTypesLoad(path));
                foreach (var info in infos)
                {
                    if (!driverInfos.ContainsKey(info.Key))
                    {
                        driverInfos.Add(info.Key, info.Value);
                    }
                }
            }
        }
        public bool isExit_Channel(string channelName)
        {
            if (config != null)
            {
                return config.Client.Channels.ContainsKey(channelName);

            }
            else
            {
                return false;
            }
        }

        bool isExit_Device(string channelName, string deviceName)
        {
            if (isExit_Channel(channelName))
            {
                return config.Client.Channels[channelName].Devices.ContainsKey(deviceName);
            }
            else
            {
                return false;
            }
        }

        bool isExit_TagGroup(string channelName, string deviceName, string TagGroupNmae)
        {
            if (isExit_Device(channelName, deviceName))
            {
                return config.Client.Channels[channelName].Devices[deviceName].TagGroups.ContainsKey(TagGroupNmae);
            }
            else
            {
                return false;
            }
        }

        bool isExit_Tag(string channelName, string deviceName, string tagGroupName, string tagName)
        {

            if (isExit_TagGroup(channelName, deviceName, tagGroupName))
            {
                return config.Client.Channels[channelName].Devices[deviceName].TagGroups[tagGroupName].Tags.ContainsKey(tagName);
            }
            else
            {
                return false;
            }
        }


        #region Get Method
        public ChannelConfig GetChannel(string channelName)
        {
            if (config.Client.Channels.TryGetValue(channelName, out ChannelConfig channel))
            {
                return channel;
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
        }

        public ClientConfig GetClient()
        {
            return config.Client;
        }

        public DeviceConfig GetDevice(string channelName, string deviceName)
        {
            DeviceConfig device = null;
            var channel = GetChannel(channelName);
            if (channel != null)
            {
                if (!channel.Devices.TryGetValue(deviceName, out device))
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            return device;
        }

        public ProjectConfig GetProject()
        {
            return config;
        }

        public ServersConfig GetServer()
        {
            return config.Server;
        }

        public TagConfig GetTag(string channelName, string deviceName, string tagGroupName, string tagName)
        {
            TagConfig tag = null;
            var tagGroup = GetTagGroup(channelName, deviceName, tagGroupName);
            if (tagGroup != null)
            {
                if (!tagGroup.Tags.TryGetValue(tagName, out tag))
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            return tag;
        }

        public TagGroupConfig GetTagGroup(string channelName, string deviceName, string tagGroupName)
        {
            TagGroupConfig tagGroup = null;
            var device = GetDevice(channelName, deviceName);
            if (device != null)
            {
                if (!device.TagGroups.TryGetValue(tagGroupName, out tagGroup))
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            return tagGroup;
        }

        public Dictionary<string, DriverInfo> GetDriverInfos()
        {
            return driverInfos;
        }

        #endregion
        #region Post Method

        public RestAPIResult PostChannel(ChannelConfig channelConfig)
        {
            if (channelConfig == null | channelConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            if (isExit_Channel(channelConfig.Name))
            {
                return RestAPIResult.FAIL;
            }
            else
            {
                config.Client.Channels.Add(channelConfig.Name, channelConfig);
                return RestAPIResult.OK;
            }
        }

        public RestAPIResult PostDevice(string channelName, DeviceConfig deviceConfig)
        {
            if (deviceConfig == null || deviceConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            else if (isExit_Device(channelName, deviceConfig.Name))
            {
                return RestAPIResult.FAIL;
            }
            else if (isExit_Channel(channelName))
            {
                config.Client.Channels[channelName].Devices.Add(deviceConfig.Name, deviceConfig);
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult PostTag(string channelName, string deviceName, string tagGroupName, TagConfig tagConfig)
        {
            if (tagConfig == null || tagConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            else if (isExit_Tag(channelName, deviceName, tagGroupName, tagConfig.Name))
            {
                return RestAPIResult.FAIL;
            }
            else if (isExit_TagGroup(channelName, deviceName, tagGroupName))
            {
                config.Client.Channels[channelName].Devices[deviceName].TagGroups[tagGroupName].Tags.Add(tagConfig.Name, tagConfig);
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult PostTagGroup(string channelName, string deviceName, TagGroupConfig tagGroupConfig)
        {
            if (tagGroupConfig == null || tagGroupConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            else if (isExit_TagGroup(channelName, deviceName, tagGroupConfig.Name))
            {
                return RestAPIResult.FAIL;
            }
            else if (isExit_Device(channelName, deviceName))
            {
                config.Client.Channels[channelName].Devices[deviceName].TagGroups.Add(tagGroupConfig.Name, tagGroupConfig);
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }
        #endregion
        private Dictionary<string, DriverInfo> getInfos(List<Type> types)
        {
            var result = new Dictionary<string, DriverInfo>();
            foreach (var t in types)
            {
                var ddAttribute = t.GetCustomAttribute<DriverDescriptionAttribute>();
                string des = ddAttribute.Description;
                DriverInfo info = new DriverInfo()
                {
                    Description = des,
                    FullName = t.FullName,
                    CommType = ddAttribute.CommType,
                };
                foreach (PropertyInfo p in t.GetRuntimeProperties())
                {
                    if (p.GetCustomAttribute<DeviceMarkAttribute>() != null)
                    {
                        info.DevicePropertyInfos.Add(new DevicePropertyInfo() { Name = p.Name, PropertyType = p.PropertyType });
                    }
                }
                result.Add(des, info);
            }
            return result;
        }
        #region Put Method
        public RestAPIResult PutChannel(ChannelConfig channelConfig)
        {
            if (channelConfig == null || channelConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            if (isExit_Channel(channelConfig.Name))
            {
                config.Client.Channels[channelConfig.Name] = channelConfig;
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult PutClient(ClientConfig clientConfig)
        {
            if (clientConfig == null)
            {
                return RestAPIResult.FAIL;
            }
            config.Client = clientConfig;
            return RestAPIResult.OK;
        }

        public RestAPIResult PutDevice(string channelName, DeviceConfig deviceConfig)
        {
            if (deviceConfig == null || deviceConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            if (isExit_Device(channelName, deviceConfig.Name))
            {
                config.Client.Channels[channelName].Devices[deviceConfig.Name] = deviceConfig;
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult PutProject(ProjectConfig projectConfig)
        {
            if (projectConfig == null)
            {
                return RestAPIResult.FAIL;
            }
            config = projectConfig;
            return RestAPIResult.OK;
        }

        public RestAPIResult PutTag(string channelName, string deviceName, string tagGroupName, TagConfig tagConfig)
        {
            if (tagConfig == null || tagConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            if (isExit_Tag(channelName, deviceName, tagGroupName, tagConfig.Name))
            {
                config.Client.Channels[channelName].Devices[deviceName].TagGroups[tagGroupName].Tags[tagConfig.Name] = tagConfig;
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult PutTagGroup(string channelName, string deviceName, TagGroupConfig tagGroupConfig)
        {
            if (tagGroupConfig == null || tagGroupConfig.Name == null)
            {
                return RestAPIResult.FAIL;
            }
            if (isExit_TagGroup(channelName, deviceName, tagGroupConfig.Name))
            {
                config.Client.Channels[channelName].Devices[deviceName].TagGroups[tagGroupConfig.Name] = tagGroupConfig;
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }
        #endregion
        #region Delete Method
        public RestAPIResult DeleteChannel(string channelName)
        {
            if (isExit_Channel(channelName))
            {
                config.Client.Channels.Remove(channelName);
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult DeleteDevice(string channelName, string deviceName)
        {
            if (isExit_Device(channelName, deviceName))
            {
                config.Client.Channels[channelName].Devices.Remove(deviceName);
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult DeleteTagGroup(string channelName, string deviceName, string tagGroupName)
        {
            if (isExit_TagGroup(channelName, deviceName, tagGroupName))
            {
                config.Client.Channels[channelName].Devices[deviceName].TagGroups.Remove(tagGroupName);
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public RestAPIResult DeleteTag(string channelName, string deviceName, string tagGroupName, string tagName)
        {
            if (isExit_Tag(channelName, deviceName, tagGroupName, tagName))
            {
                config.Client.Channels[channelName].Devices[deviceName].TagGroups[tagGroupName].Tags.Remove(tagName);
                return RestAPIResult.OK;
            }
            else
            {
                return RestAPIResult.FAIL;
            }
        }

        public UpDllFileResult UpDriverDll(string fileName, Stream stream)
        {
            try
            {
                string filePath = DLLPATH + "//" + fileName;
                FileBinary.Write(filePath, stream);

                var infos = getInfos(ReflectionFunction.GetTypesLoad(filePath));
                foreach (var info in infos)
                {
                    if (driverInfos.ContainsKey(info.Key))
                    {
                        driverInfos[info.Key] = info.Value;
                    }
                    else
                    {
                        driverInfos.Add(info.Key, info.Value);
                    }
                }
                return new UpDllFileResult() { DriverInfos = driverInfos, Result = RestAPIResult.OK, ErrorMsg = null };
            }
            catch (Exception e)
            {
                return new UpDllFileResult() { DriverInfos = null, Result = RestAPIResult.FAIL, ErrorMsg = e.Message };
            }
        }
        #endregion
        #endregion

    }
}
