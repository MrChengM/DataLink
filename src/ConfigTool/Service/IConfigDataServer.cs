using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigTool.Models;
using DataServer;
using DataServer.Config;
namespace ConfigTool.Service
{
    public interface IConfigDataServer: IClientConfigOpertor, IDriverInfosOpertor,IServerConfigOpertor, IAlarmsConfigOpertor
    {
        void Load();
        bool Open();
        bool Save();
        bool SaveAs();
        //bool ConnectServer();
        //bool DisconnectServer();
        bool Updata();
        bool DownLoad();
        //Dictionary<string, DriverInfo> ResigterDLL(string path);

    }
   //public delegate void ConfigChangeHandler();

    public interface IClientConfigOpertor
    {


        ProjectConfig ProjectConfig { get; }

        event EventHandler<ConfigEventArgs> ConfigChangeEvent;

        ClientConfig GetClient();
        ChannelConfig GetChannel(string channelName);
        DeviceConfig GetDevice(string channelName, string deviceName);
        TagGroupConfig GetTagGroup(string channelName, string deviceName, string tagGroupName);

        TagConfig GetTag(string channelName, string deviceName, string tagGroupName, string tagName);

        List<TagListItem> GetAllTags();
        bool AddDevice(string channelName, DeviceConfig device);
        bool AddChannel(ChannelConfig channnel);
        bool AddTagGroup(string channelName, string deviceName, TagGroupConfig tagGroup);

        bool AddTag(string channelName, string deviceName, string tagGroupName,TagConfig tagConfig);

        bool IsExit_Channel(string channelName);

        bool IsExit_Deveice(string channelName, string deviceName);

        bool IsExit_TagGroup(string channelName, string deviceName,string tagGroupName);
        bool IsExit_Tag(string channelName, string deviceName, string tagGroupName,string tagName);

        void RemoveChannel(string channelName);
        void RemoveDevice(string channelName, string deviceName);
        void RemoveTagGroup(string channelName, string deviceName, string tagGroupName);
        void RemoveTag(string channelName, string deviceName, string tagGroupName,string tagName);

    }
    public interface IDriverInfosOpertor
    {
        //string DllPath { get; }
        void AddDriverInfo(DriverInfo info);
        DriverInfo GetDriverInfo(string name);
        Dictionary<string, DriverInfo> DriverInfos { get; }
       void RegisterDriver(string filePath);
    }


    public interface IServerConfigOpertor
    {
        ServersConfig GetServers();
        ServerItemConfig GetServerItem(string serverItemName);
        TagBindingConfig GetTagBinding(string serverItemName, string tagBindingName);
        bool AddServerItem(ServerItemConfig serverItem);

        bool AddTagBinding(string serverName, TagBindingConfig tagBinding);
        bool IsExit_ServerItem(string serverName);
        bool IsExit_ServerOption(ServerOption option);

        bool IsExit_TagBinding(string serverName, string tagBinding);

        void RemoveServerItem(string serverName);
        void RemoveTagBinding(string serverName, string tagBindingName);
    }
    public interface IAlarmsConfigOpertor
    {
        AlarmsConfig GetAlarms();
        AlarmItemConfig GetAlarmItem(string alarmTag);

        bool AddAlarmItemConfig(AlarmItemConfig alarmItem);

        bool IsExit_AlarmItem(string alarmTag);

        void Remove_AlarmItem(string alarmTag);

    }
    public enum ConfigOperate
    {
        AddNode,
        RemoveNode,
        ReloadAll
    }

    public class ConfigEventArgs
    {
        public object ParentConfigNode { get; set; }
        public string CurreNodeName { get; set; }
        public NodeType CurreNodeType { get; set; }
        public ConfigOperate OperateMode { get; set; }

    }
}
