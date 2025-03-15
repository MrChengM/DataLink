using DataServer.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Utillity.File;
using Utillity.Reflection;
using System.Reflection;
using DataServer;
using System.IO;

namespace TaskMgr
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,MaxItemsInObjectGraph = 2147483647)]
    public class ConfigService : IConfigServer
    {
        #region Field
        private ProjectConfig config;
        private Dictionary<string, DriverInfo> driverInfos;
        private Dictionary<string, Type> driverTypes;
        private const string CONFIGPATH = "../../../../conf";
        private const string PROJECTFILE_DEFAULT = "/ProjectConfig.Json";
        private const string DLLPATH = "../../../../dll";
        private const string DRIVERINFOS_DEFAULT = "/DriverInformation.Json";

        public event Action<ProjectConfig> ProConfRefreshEvent;
        //public event Action<ChannelConfig> ChlConfRefreshEvent;
        ////public event Action<AlarmsConfig> AlmConfRefreshEvent;
        ////public event Action<RecordItemConfig> RcrdConfRefreshEvent;
        #endregion
        #region Property
        public ProjectConfig Config
        {
            get { return config; }
            set
            {
                if (config != value)
                {
                    config = value;
                    ProConfRefreshEvent?.Invoke(config);
                }
            }
        }
        public Dictionary<string, Type> DriverTypes
        {
            get { return driverTypes; }
            //set { config = value; }
        }
        public Dictionary<string, DriverInfo> DriverInfos
        {
            get { return driverInfos; }
            //set { driverInfos = value; }
        }


        #endregion
        #region Method

        public ConfigService()
        {
            config = new ProjectConfig();
            driverInfos = new Dictionary<string, DriverInfo>();
            driverTypes = new Dictionary<string, Type>();
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

                var types = ReflectionFunction.GetTypesLoad(path);
                foreach (var t in types)
                {
                    if (!driverTypes.ContainsKey(t.FullName))
                    {
                        driverTypes.Add(t.FullName, t);
                    }
                }

                var infos = getInfos(types);
                foreach (var info in infos)
                {
                    if (!driverInfos.ContainsKey(info.Key))
                    {
                        driverInfos.Add(info.Key, info.Value);
                    }
                }
            }
            SaveDrivierInfos();
        }

        public void SaveDrivierInfos()
        {
            JsonFunction.Save(CONFIGPATH + DRIVERINFOS_DEFAULT, driverInfos);
        }
        public void SaveProjectConfig()
        {
            JsonFunction.Save(CONFIGPATH + PROJECTFILE_DEFAULT, config);
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
        public Dictionary<string,DriverInfo> SaveDriverDll(string fileName, Stream stream,out string errorMsg)
        {
            try
            {
                string filePath = DLLPATH + "//" + fileName;
                FileBinary.Write(filePath, stream);

                var types = ReflectionFunction.GetTypesLoad(filePath);
                foreach (var t in types)
                {
                    if (!driverTypes.ContainsKey(t.FullName))
                    {
                        driverTypes.Add(t.FullName, t);
                    }
                }

                var infos = getInfos(types);
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
                SaveDrivierInfos();
                errorMsg = null;
                return driverInfos;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
                return null;
            }
        }
        #endregion

    }
}
