using DataServer.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigTool.Models;
using Utillity.File;
using Prism.Ioc;
using log4net;
using Utillity.Reflection;
using System.Reflection;
using DataServer;
using WCFRestFullAPI.Client;
using WCFRestFullAPI.Models;
using System.IO;

namespace ConfigTool.Service
{
    public class ConfigDataServer : IConfigDataServer
    {

        private Dictionary<string, DriverInfo> _driverInfos;

        private ProjectConfig _projectConfig ;
        private IContainerProvider _containerProvider;
        private ISingleLoggerServer _singleLoggerServer;
        private const string CONFIGPATH = "../../../../conf";
        private string sFilePath = "";
        private string sFilter = "json|*.json";
        private string basUrl = "http://127.0.0.1:3271/ConfigService";

        private const string DRIVERFILE_DEFAULT = "/DriverInformation.Json";

        private const string PROJECTFILE_DEFAULT = "/ProjectConfig.Json";

        private const string CLIENTFILE_DEFAULT = "/ClientConfig.Json";

        private const string SEVERFILE_DEFAULT = "/ServerConfig.Jsosn";

        private const string ALARMFILE_DEFAULT = "/AlarmConfig.Jsosn";

        private const string DLLPATH = "../../../../dll";


        private log4net.ILog _log; 

        //public string DllPath => DLLPATH;
        public event EventHandler<ConfigEventArgs> ConfigChangeEvent;

        public ConfigDataServer()
        {
            _driverInfos = new Dictionary<string, DriverInfo>();
            _projectConfig = new ProjectConfig();
            _containerProvider = ContainerLocator.Container;
            _singleLoggerServer = _containerProvider.Resolve<ISingleLoggerServer>();
            _log = _singleLoggerServer.Log;
            Load();
        }

        public Dictionary<string, DriverInfo> DriverInfos => _driverInfos;

        public ProjectConfig ProjectConfig => _projectConfig;



        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="channnel"></param>
        public bool AddChannel(ChannelConfig channnel)
        {
            bool result = false;
            if (_projectConfig.Client.Channels?.ContainsKey(channnel.Name)==false)
            {
                var client = GetClient();
                _projectConfig.Client.Channels.Add(channnel.Name,channnel);
                _log.Info($"Add channel successfully: \'{channnel.Name}\'");
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = client, CurreNodeName = channnel.Name, CurreNodeType = NodeType.Channel, OperateMode = ConfigOperate.AddNode });
                result = true;
            }
            else
            {
                _log.Warn($"Add channel failed: \'{channnel.Name}\' have duplicated! ");
            }
            return result;

        }
        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="device"></param>
        public bool AddDevice(string channelName, DeviceConfig device)
        {
            bool result = false;
            var channel = GetChannel(channelName);
            if (channel!=null)
            {
                if (!channel.Devices.ContainsKey(device.Name))
                {
                    channel.Devices.Add(device.Name, device);
                    _log.Info($"Add Device sucessfully: \'{channelName}.{device.Name}\' .");

                    raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = channel, CurreNodeName=device.Name, CurreNodeType = NodeType.Device, OperateMode = ConfigOperate.AddNode });
                    result = true;
                }
                else
                {
                    _log.Warn($"Add Device failed: \'{channelName}.{device.Name}\' have duplicated! ");
                }
            }
            else
            {
                _log.Warn($"Add Device failed: \'{channelName}\' not exit! ");
            }
            return result;
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
            else
            {
                _driverInfos[info.Description] = info;
            }
            _log.Info($"Driver information register successful : \'{info.FullName}\'");
            //JsonFunction.Save($"{CONFIGPATH}{DRIVERFILE_DEFAULT}", _driverInfos);
    
        }
        /// <summary>
        /// 添加标签组
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="deviceName"></param>
        /// <param name="tagGroup"></param>
        public bool AddTagGroup(string channelName, string deviceName, TagGroupConfig tagGroup)
        {
            bool result=false;
            var device = GetDevice(channelName, deviceName);
            if (device != null)
            {
                if (!device.TagGroups.ContainsKey(tagGroup.Name))
                {
                    device.TagGroups.Add(tagGroup.Name,tagGroup);
                    _log.Info($"Add TagGroup sucessfully : \'{channelName}.{deviceName}.{tagGroup.Name}\'.");

                    raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = device, CurreNodeName = tagGroup.Name,CurreNodeType = NodeType.TagGroup, OperateMode = ConfigOperate.AddNode });
                    result = true;
                }
                else
                {
                    _log.Warn($"Add TagGroup failed : \'{channelName}.{deviceName}.{tagGroup.Name}\' hanve duplicated!");

                }
            }
            else
            {
                _log.Warn($"Add TagGroup failed : \'{channelName}.{deviceName}\' not exit!");

            }
            return result;
        }
        public bool AddTag(string channelName, string deviceName, string tagGroupName, TagConfig tagConfig)
        {

            bool result = false;
            var tagGroup = GetTagGroup(channelName, deviceName, tagGroupName);
            if (tagGroup != null)
            {
                if (!tagGroup.Tags.ContainsKey(tagConfig.Name))
                {
                    tagGroup.Tags.Add(tagConfig.Name, tagConfig);
                    _log.Info($"Add Tag sucessfully : \'{channelName}.{deviceName}.{tagGroup.Name}.{tagConfig.Name}\'.");
                    raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = tagGroup, CurreNodeName = tagConfig.Name,CurreNodeType = NodeType.Tag, OperateMode = ConfigOperate.AddNode });
                    result = true;
                }
                else
                {
                    _log.Warn($"Add Tag failed : \'{channelName}.{deviceName}.{tagGroup.Name}.{tagConfig.Name}\' have duplicated!");
                }
            }
            else
            {
                _log.Warn($"Add Tag failed : \'{channelName}.{deviceName}.{tagGroup.Name}\' have not exit!");
            }
            return result;
        }

        public ClientConfig GetClient()
        {
            return _projectConfig.Client;
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
            DeviceConfig result;
            var channel = GetChannel(channelName);
            if (channel!=null)
            {
                channel.Devices.TryGetValue(deviceName, out result);
                return result;
            }
            else
            {
                return null;
            }
        }

        public DriverInfo GetDriverInfo(string name)
        {
            DriverInfo result;
            _driverInfos.TryGetValue(name, out result);
            return result;
        }

        public void RegisterDriver(string filePath)
        {
            try
            {
                FileInfo info = new FileInfo(filePath);
                string url = basUrl + "/DriverInformation" + $"/{info.Name}";
                //byte[] dllBytes = FileBinary.Read(filePath);
                var bytes = FileBinary.Read(filePath);

                var result = RestAPIOpertor.PostFileFunc<UpDllFileResult>(url, bytes);
                if (result!=null)
                {
                    if (result.Result==RestAPIResult.OK)
                    {
                        _driverInfos = result.DriverInfos;
                        _log.Info($"Dll Register succeefully:DLL name\'{info.Name}\'");
                    }
                    else
                    {
                        _log.Info($"Dll Register failed:\'{result.ErrorMsg}\'");
                    }
                  
                }
                else
                {
                    _log.Error($"Dll Register failed，information:\'value is null!\'");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Dll Register error，information:\'{ex.Message}\'");
            }

        }

        //public Dictionary<string,DriverInfo> RegisterDriver(string filePath,bool isLoad)
        //{
        //    Dictionary<string, DriverInfo> infors = new Dictionary<string, DriverInfo>();
        //    try
        //    {
        //        var types = ReflectionFunction.GetTypesOnlyLoad(filePath);

        //        if (types != null && !isLoad)
        //        {
        //            FileCopy.SingleFileCopy(filePath, DLLPATH);
        //        }

        //        foreach (var t in types)
        //        {
        //            DriverInfo info = new DriverInfo();
        //            var ddAttributes = CustomAttributeData.GetCustomAttributes(t);
        //            var ddAttribute = ddAttributes[0];
        //            info.FullName = t.FullName;
        //            if (ddAttribute.AttributeType.ToString() == typeof(DriverDescriptionAttribute).ToString())
        //            {
        //                foreach (var csa in ddAttribute.ConstructorArguments)
        //                {
        //                    if (csa.ArgumentType == typeof(string))
        //                    {
        //                        info.Description = (string)csa.Value;
        //                    }
        //                    if (csa.ArgumentType.ToString() == typeof(CommunicationType).ToString())
        //                    {
        //                        info.CommType = (CommunicationType)csa.Value;
        //                    }
        //                }
        //            }

        //            var props = t.GetProperties();
        //            foreach (var p in props)
        //            {
        //                var dmAttributes = CustomAttributeData.GetCustomAttributes(p);
        //                if (dmAttributes.Count != 0 && dmAttributes[0].AttributeType.ToString() == typeof(DeviceMarkAttribute).ToString())
        //                {
        //                    var deviceProp = new DevicePropertyInfo() { Name = p.Name, PropertyType = p.PropertyType };
        //                    info.DevicePropertyInfos.Add(deviceProp);
        //                }
        //            }

        //            infors.Add(info.Description, info);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.Error($"Dll Register Failed: \'{e.Message}\' !");
        //    }
        //    return infors;

        //}
        //private bool isExit(string driverName)
        //{
        //    return _driverInfos.ContainsKey(driverName);
        //}
        public TagGroupConfig GetTagGroup(string channelName, string deviceName, string tagGroupName)
        {
            var device = GetDevice(channelName, deviceName);
            if (device!=null)
            {
                TagGroupConfig config;

                device.TagGroups.TryGetValue(tagGroupName, out config);

                return config;
            }
            else
            {
                return null;
            }
        }

        public TagConfig GetTag(string channelName, string deviceName, string tagGroupName, string tagName)
        {
            var tagGroup = GetTagGroup(channelName, deviceName, tagGroupName);
            if (tagGroup != null)
            {
                TagConfig config;

                tagGroup.Tags.TryGetValue(tagName, out config);
                return config;
            }
            else
            {
                return null;
            }
        }
        public List<TagListItem> GetAllTags()
        {
            List<TagListItem> result = new List<TagListItem>();
            //string tagName;
            foreach (var channel in _projectConfig.Client.Channels)
            {
                foreach (var device in channel.Value.Devices)
                {
                    foreach (var tagGroup in device.Value.TagGroups)
                    {
                        foreach (var tag in tagGroup.Value.Tags)
                        {
                            TagListItem tagItemArrary = new TagListItem();
                            tagItemArrary.Name = $"{channel.Key}.{device.Key}.{tagGroup.Key}.{tag.Key}";
                            tagItemArrary.Length = tag.Value.Length;
                            tagItemArrary.DataType = tag.Value.DataType.ToString();
                            tagItemArrary.OperateWay = tag.Value.Operate.ToString();
                            result.Add(tagItemArrary);
                            if (tag.Value.Length > 1)
                            {
                                for (int i = 0; i < tag.Value.Length; i++)
                                {
                                    TagListItem tagItem = new TagListItem();
                                    tagItem.Name = $"{channel.Key}.{device.Key}.{tagGroup.Key}.{tag.Key}[{i}]";
                                    tagItem.Length = 1;
                                    tagItem.DataType = tag.Value.DataType.ToString();
                                    tagItem.OperateWay = tag.Value.Operate.ToString();
                                    result.Add(tagItem);
                                }
                            }
                        }
                          
                    }
                }
            }
            return result;
        }
        public void RemoveChannel(string channelName)
        {
            if (IsExit_Channel(channelName))
            {
                var client = GetClient();
                client.Channels.Remove(channelName);
                _log.Info($"Remove channel sucessfully: \'{channelName}\'have removed.");
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = client, CurreNodeName= channelName, CurreNodeType = NodeType.Channel, OperateMode = ConfigOperate.RemoveNode });
            }
            else
            {
                _log.Warn($"Remove channel Failed: \'{channelName}\' not exit!");
            }
        }

        public void RemoveDevice(string channelName, string deviceName)
        {
            if (IsExit_Deveice(channelName,deviceName))
            {
                var channel = GetChannel(channelName);
                channel.Devices.Remove(deviceName);
                _log.Info($"Remove device sucessfully: \'{channelName}.{deviceName}\' have removed.");
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = channel, CurreNodeName=deviceName, CurreNodeType = NodeType.Device, OperateMode = ConfigOperate.RemoveNode });
            }
            else
            {
                _log.Warn($"Remove device failed: \'{channelName}.{deviceName}\' not exit!");
            }
        }

        public void RemoveTagGroup(string channelName, string deviceName, string tagGroupName)
        {
            if (IsExit_TagGroup(channelName, deviceName,tagGroupName))
            {
                var device = GetDevice(channelName, deviceName);
                device.TagGroups.Remove(tagGroupName);
                _log.Info($"Remove TagGroup sucessfully: \'{channelName}.{deviceName}.{tagGroupName}\' have removed.");
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = device, CurreNodeName = tagGroupName, CurreNodeType = NodeType.TagGroup, OperateMode = ConfigOperate.RemoveNode });
            }
            else
            {
                _log.Warn($"Remove TagGroup failed: \'{channelName}.{deviceName}\' not exit!");

            }
        }
        public void RemoveTag(string channelName, string deviceName, string tagGroupName, string tagName)
        {
            if (IsExit_Tag(channelName, deviceName, tagGroupName, tagName))
            {
                var tagGroup = GetTagGroup(channelName, deviceName,tagGroupName);
                tagGroup.Tags.Remove(tagName);

                _log.Info($"Remove tag sucessfully: \'{channelName}.{deviceName}.{tagGroupName}.{tagName}\' have removed.");

                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = tagGroup, CurreNodeName = tagName,CurreNodeType = NodeType.Tag, OperateMode = ConfigOperate.RemoveNode });
            }
            else
            {
                _log.Warn($"Remove tag failed: \'{channelName}.{deviceName}.{tagGroupName}\' not exit!");

            }
        }


        void raiseConfigChangeEvent(object sender,ConfigEventArgs e)
        {
            ConfigChangeEvent?.Invoke(sender, e);
        }
        //public Dictionary<string,DriverInfo> ResigterDLL(string path)
        //{
        //   return _driverInfos = ReflectionOperate.GetInfos(path);
        //}
        public void Load()
        {
            Updata();
        }

        //private void loadDriverInfo()
        //{
        //    //string[] driversPath = System.IO.Directory.GetFiles(DLLPATH);
        //    ////var temp = new string[] { driversPath[1], driversPath[2] };
        //    //foreach (var path in driversPath)
        //    //{
        //    //    var infos = RegisterDriver(path, true);
        //    //    foreach (var info in infos)
        //    //    {
        //    //        AddDriverInfo(info.Value);
        //    //    }
        //    //}
        //    //_log.Info($"Load Driver information finish from:\'{DLLPATH}\'");
        //}
        //private void loadConfigFile()
        //{
        //    UpdataConfig();
        //}
        private bool Open(string path)
        {
            bool result;
            
            var config= JsonFunction.Load<ProjectConfig>(path);
            if (config != null)
            {
                _projectConfig = config;

                raiseConfigChangeEvent(this, new ConfigEventArgs() { OperateMode = ConfigOperate.ReloadAll });
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool Open()
        {
            if (FileDialog.InputFile(ref sFilePath, sFilter))
            {
             return  Open(sFilePath);
            }
            else
            {
                return false;
            }
        }
        public bool Save()
        {
            if (save($"{CONFIGPATH}{PROJECTFILE_DEFAULT}"))
            {
                _log.Info($"Save project config sucessfully,Path:\'{CONFIGPATH}{PROJECTFILE_DEFAULT}\'");
                return true;
            }
            else
            {
                _log.Warn($"Save project config failed,Path:\'{CONFIGPATH}{PROJECTFILE_DEFAULT}\'");
                return false;
            }

        }
        public bool SaveAs()
        {
            if (!FileDialog.OutputFile(ref sFilePath, sFilter))
            {
                return false;
            }
            if (save(sFilePath))
            {
                _log.Info($"Save project config sucessfully,Path:\'{sFilePath}\'");
                return true;

            }
            else
            {
                _log.Warn($"Save project config sucessfully,Path:\'{sFilePath}\'");
                return false;

            }

        }
        private bool save(string path)
        {
            return JsonFunction.Save(path, _projectConfig);

        }
        public bool Updata()
        {
            return UpdataConfig() & UpdataDLL();
        }
        public bool UpdataConfig()
        {

            string url = basUrl + "/Project";
            try
            {
                var result = RestAPIOpertor.GetFuncJson<ProjectConfig>(url);
                if (result != null)
                {
                    _projectConfig = result;
                    raiseConfigChangeEvent(this, new ConfigEventArgs() { OperateMode = ConfigOperate.ReloadAll });
                    _log.Info($"Updata Project from Server sucessfully! ");

                    return true;
                }
                else
                {
                    _log.Info($"Updata Project from Server failed:the result value is null! ");
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.Error($"Updata Project from Server failed:{e.Message}({url})! ");
                return false;
            }
        }
        public bool UpdataDLL()
        {

            string url = basUrl + "/DriverInformation";
            try
            {
                var result = RestAPIOpertor.GetFuncJson<Dictionary<string,DriverInfo>>(url);
                if (result != null)
                {
                    _driverInfos = result;
                    _log.Info($"Updata drivers information from Server sucessfully! ");

                    return true;
                }
                else
                {
                    _log.Info($"Updata drivers information from Server failed:the result value is null! ");
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.Error($"Updata drivers information from Server failed:{e.Message}({url})! ");
                return false;
            }
        }
        public bool DownLoad()
        {
            string url = basUrl + "/Project";
            try
            {
                var result = RestAPIOpertor.PutFuncJson<ProjectConfig, RestAPIResult>(url, _projectConfig);
                if (result == RestAPIResult.OK)
                {
                    _log.Info($"DownLoad Project to Server sucessfully! ");
                    return true;
                }
                else
                {
                    _log.Info($"DownLoad Project to Server failed! ");

                    return false;
                }
            }
            catch (Exception e)
            {
                _log.Error($"DownLoad Project to Server error,information:{e.Message}({url})! ");
                return false;
            }
           
        }
        //public bool ConnectServer()
        //{
        //    throw new NotImplementedException();
        //}

        //public bool DisconnectServer()
        //{
        //    throw new NotImplementedException();
        //}

        public bool IsExit_Channel(string channelName)
        {
          return _projectConfig.Client.Channels.ContainsKey(channelName);
        }

        public bool IsExit_Deveice(string channelName, string deviceName)
        {
            if (IsExit_Channel(channelName))
            {
              return  _projectConfig.Client.Channels[channelName].Devices.ContainsKey(deviceName);
            }
            else
            {
                return false;
            }
        }

        public bool IsExit_TagGroup(string channelName, string deviceName, string TagGroupNmae)
        {
            if (IsExit_Deveice(channelName,deviceName))
            {
                return _projectConfig.Client.Channels[channelName].Devices[deviceName].TagGroups.ContainsKey(TagGroupNmae);
            }
            else
            {
                return false;
            }
        }

        public bool IsExit_Tag(string channelName, string deviceName, string tagGroupName, string tagName)
        {

            if (IsExit_TagGroup(channelName, deviceName, tagGroupName))
            {
                return _projectConfig.Client.Channels[channelName].Devices[deviceName].TagGroups[tagGroupName].Tags.ContainsKey(tagName);
            }
            else
            {
                return false;
            }
        }

        #region IServerConfigOpertor
        public bool AddServerItem(ServerItemConfig serverItem)
        {
            bool result;
            if (IsExit_ServerItem(serverItem.Name))
            {
                _log.Warn($"Add Server failed:server name \'{serverItem.Name}\' have duplicated! ");
                result = false;
            }
            else if (IsExit_ServerOption(serverItem.Option))
            {
                _log.Warn($"Add Server failed: server option \'{serverItem.Option}\' have duplicated! ");
                result = false;

            }
            else
            {
                _projectConfig.Server.Items.Add(serverItem.Name, serverItem);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = _projectConfig.Server, CurreNodeName = serverItem.Name,CurreNodeType = NodeType.ServerItem, OperateMode = ConfigOperate.AddNode });
                _log.Info($"Add Server successfully: server name \'{serverItem.Name}\' have added! ");

                result = true;
            }
            return result;
        }

        public bool AddTagBinding(string serverName,TagBindingConfig tagBinding)
        {
            bool result;
            if (!IsExit_ServerItem(serverName))
            {
                _log.Warn($"Add TagBinding failed: server name \'{serverName}\' isn't exit! ");
                result = false;
            }
            else if(IsExit_TagBinding(serverName,tagBinding.DestTagName))
            {
                _log.Warn($"Add TagBinding failed: tagbinding name \'{serverName}.{tagBinding.DestTagName}\' have duplicated! ");
                result = false;
            }
            else
            {
                var serverItem = GetServerItem(serverName);
                _projectConfig.Server.Items[serverName].TagBindingList.Add(tagBinding.DestTagName, tagBinding);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = serverItem, CurreNodeName = tagBinding.DestTagName, CurreNodeType = NodeType.TagBinding, OperateMode = ConfigOperate.AddNode });
                _log.Info($"Add TagBinding successfully: tagbinding name \'{serverName}.{tagBinding.DestTagName}\' have added! ");

                result = true;
            }
            return result;
        }

        public bool IsExit_ServerItem(string serverName)
        {
            return _projectConfig.Server.Items.ContainsKey(serverName);
        }

        public bool IsExit_ServerOption(ServerOption option)
        {
            bool result = false;
            foreach (var item in _projectConfig.Server.Items)
            {
                if (item.Value.Option == option)
                {
                    result = true;
                    break;
                } 
            }
            return result;
        }

        public bool IsExit_TagBinding(string serverName, string tagBindingName)
        {
            bool result;
            if (IsExit_ServerItem(serverName))
            {
                result= _projectConfig.Server.Items[serverName].TagBindingList.ContainsKey(tagBindingName);
            }
            else
            {
                result = false;
            }
            return result;
        }

        public void RemoveServerItem(string serverName)
        {
            if (IsExit_ServerItem(serverName))
            {
                _projectConfig.Server.Items.Remove(serverName);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = _projectConfig.Server, CurreNodeName = serverName, CurreNodeType = NodeType.ServerItem, OperateMode = ConfigOperate.RemoveNode });
                _log.Info($"Remove server item successfully: server name \'{serverName}\' have removed ! ");
            }
            else
            {
                _log.Warn($"Remove server item faile: server name \'{serverName}\' isn't exit! ");

            }
        }

        public void RemoveTagBinding(string serverName, string destName)
        {
            if (IsExit_TagBinding(serverName,destName))
            {
                var serverItem = GetServerItem(serverName);

                _projectConfig.Server.Items[serverName].TagBindingList.Remove(destName);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = serverItem, CurreNodeName = destName, CurreNodeType = NodeType.TagBinding, OperateMode = ConfigOperate.RemoveNode });

                _log.Info($"Remove Tag Binding successfully: Tag Binding name \'{serverName}.{destName}\' have removed ! ");
            }
            else
            {
                _log.Warn($"Remove Tag Binding faile: Tag Binding name \'{serverName}.{destName}\' isn't exit! ");

            }
        }

        public ServerItemConfig GetServerItem(string serverItemName)
        {
            
            ServerItemConfig result;
            _projectConfig.Server.Items.TryGetValue(serverItemName, out result);
            return result;
        }

        public TagBindingConfig GetTagBinding(string serverName, string tagBindingName)
        {
            TagBindingConfig result = null;
            if (IsExit_TagBinding(serverName, tagBindingName))
            {
                result = _projectConfig.Server.Items[serverName].TagBindingList[tagBindingName];
            }
            return result;
        }

        public ServersConfig GetServers()
        {
            return _projectConfig.Server;
        }

        #endregion
        #region IAlarmsConfigOpertor
        public AlarmsConfig GetAlarms()
        {
            return  _projectConfig.Alarms;
        }

        public AlarmItemConfig GetAlarmItem(string alarmTag)
        {
            if (IsExit_AlarmItem(alarmTag))
            {
                return _projectConfig.Alarms.AlarmGroup[alarmTag];
            }
            else
            {
                return null;
            }
        }

        public bool AddAlarmItemConfig(AlarmItemConfig alarmItem)
        {
            if (IsExit_AlarmItem(alarmItem.AlarmTag))
            {
                _log.Warn($"Add Alarm Item failed: Alarm Item name \'{alarmItem.AlarmTag}\' have duplicated ! ");
                return false;
            }
            else
            {
                var alarms = _projectConfig.Alarms;

                _projectConfig.Alarms.AlarmGroup.Add(alarmItem.AlarmTag, alarmItem);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = alarms, CurreNodeName = alarmItem.AlarmTag, CurreNodeType = NodeType.AlarmItem, OperateMode = ConfigOperate.AddNode });
                _log.Info($"Add Alarm Item successfully: Alarm Item name \'{alarmItem.AlarmTag}\' have added ! ");

                return true;
            }
        }

        public bool IsExit_AlarmItem(string alarmTag)
        {
          return  _projectConfig.Alarms.AlarmGroup.ContainsKey(alarmTag);
        }

        public void Remove_AlarmItem(string alarmTag)
        {
            if (IsExit_AlarmItem(alarmTag))
            {
                var alarms = _projectConfig.Alarms;
                _projectConfig.Alarms.AlarmGroup.Remove(alarmTag);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = alarms, CurreNodeName = alarmTag, CurreNodeType = NodeType.AlarmItem, OperateMode = ConfigOperate.RemoveNode });
                _log.Info($"Remove Alarm Item successfully: Alarm Item name \'{alarmTag}\' have removed ! ");
            }
            else
            {
                _log.Warn($"Remove Alarm Item failed: Alarm Item name \'{alarmTag}\' isn't exit ! ");
            }
        }
        #endregion

        #region IRecordsConfigOpertor
        public RecordsConfig GetRecords()
        {
            return _projectConfig.Records;
        }

        public RecordItemConfig GetRecordItem(string name)
        {
            if (IsExit_RecordItem(name))
            {
                return _projectConfig.Records.RecordGroup[name];
            }
            else
            {
                return null;
            }
        }

        public bool AddRecordItem(RecordItemConfig recordItem)
        {
            if (IsExit_RecordItem(recordItem.Name))
            {
                _log.Warn($"Add Record Item failed: Record Item name \'{recordItem.Name}\' have duplicated ! ");
                return false;
            }
            else
            {
                var records = _projectConfig.Records;
                records.RecordGroup.Add(recordItem.Name, recordItem);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = records, CurreNodeName = recordItem.Name, CurreNodeType = NodeType.RecordItem, OperateMode = ConfigOperate.AddNode });
                _log.Info($"Add Record Item successfully: Record Item name \'{recordItem.Name}\' have added ! ");
                return true;
            }
        }

        public bool IsExit_RecordItem(string name)
        {
           return _projectConfig.Records.RecordGroup.ContainsKey(name);
        }

        public void RemoveRecordItem(string name)
        {
            if (IsExit_RecordItem(name))

            {    var records = _projectConfig.Records;

                records.RecordGroup.Remove(name);
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = records, CurreNodeName = name, CurreNodeType = NodeType.RecordItem, OperateMode = ConfigOperate.RemoveNode });
                _log.Info($"Remove Record Item successfully: Record Item name \'{name}\' have removed ! ");
            }
            else
            {
                _log.Warn($"Remove Record Item failed: Record Item name \'{name}\' isn't exit ! ");

            }

        }

        public bool AddRecordTags(string recordName, List<string> tags)
        {
            if (IsExit_RecordItem(recordName))
            {
                RecordItemConfig recordItem = _projectConfig.Records.RecordGroup[recordName];
                foreach (var tag in tags)
                {
                    if (!recordItem.TagNames.Contains(tag))
                    {
                        recordItem.TagNames.Add(tag);
                        _log.Info($"Add  Record tag successfully:  Tag name \'{recordItem.Name}\' have added ! ");
                    }
                    else
                    {
                        _log.Warn($"Add Record tag failed: Tag name \'{recordItem.Name}\' have duplicated ! ");

                    }
                }
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = recordItem, CurreNodeName = null, CurreNodeType = NodeType.RecordTag, OperateMode = ConfigOperate.AddNode });

                return true;
            }
            else
            {
                _log.Warn($"Add Record tag failed: Record Item name \'{recordName}\' isn't exit ! ");

                return false;
            }
        }

        public bool RemoveRecordTags(string recordName, List<string> tags)
        {
            if (IsExit_RecordItem(recordName))
            {
                RecordItemConfig recordItem = _projectConfig.Records.RecordGroup[recordName];
                foreach (var tag in tags)
                {
                    if (recordItem.TagNames.Contains(tag))
                    {
                        recordItem.TagNames.Remove(tag);
                        _log.Info($"Remove  Record tag successfully:  Tag name \'{tag}\' have Removed ! ");
                    }
                    else
                    {
                        _log.Warn($"Remove  Record tag failed:  Tag name \'{tag}\' isn't exit ! ");
                    }
                }
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = recordItem, CurreNodeName = null, CurreNodeType = NodeType.RecordTag, OperateMode = ConfigOperate.RemoveNode });
                return true;
            }
            else
            {
                _log.Warn($"Remove Record tag failed: Record Item name \'{recordName}\' isn't exit ! ");

                return false;
            }
        }
        public bool ReplaceRecordTags(string recordName, List<string> tags)
        {
            if (IsExit_RecordItem(recordName))
            {
                RecordItemConfig recordItem = _projectConfig.Records.RecordGroup[recordName];
                recordItem.TagNames = tags;
                _log.Info($"Replace Record Item tags successfully:  Record item \'{recordName}\' tags have Repalced ! ");
                raiseConfigChangeEvent(this, new ConfigEventArgs() { ParentConfigNode = recordItem, CurreNodeName = null, CurreNodeType = NodeType.RecordTag, OperateMode = ConfigOperate.ReplaceNode });
                return true;
            }
            else
            {
                _log.Warn($"Replace Record tag failed: Record Item name \'{recordName}\' isn't exit ! ");

                return false;
            }
        }
        #endregion


    }
}
