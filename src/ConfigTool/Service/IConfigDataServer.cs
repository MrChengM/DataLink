using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Config;
namespace ConfigTool.Service
{
    public interface IConfigDataServer: IClinetDataOpertor, IDriverInfosOpertor
    {
        bool Load();
        bool Open(string path);
        bool Save(string path);

        bool SaveAs(string path);
        bool ConnectServer();
        bool DisconnectServer();
        bool Updata();

        //Dictionary<string, DriverInfo> ResigterDLL(string path);

    }

    public interface IClinetDataOpertor
    {
        ChannelConfig GetChannel(string channelName);
        DeviceConfig GetDevice(string channelName, string deviceName);
        TagGroupConfig GetTagGroup(string channelName, string deviceName, string TagGroupName);
        void AddDevice(string ChannelName, DeviceConfig device);
        void AddChannel(ChannelConfig channnel);
        void AddTagGroup(string ChannelName, string deviceName, TagGroupConfig tagGroup);
        void RemoveChannel(string channelName);
        void RemoveDevice(string channelName, string DeviceName);
        void RemoveTagGroup(string channelName, string DeviceName, string TagGroupName);
    }
    public interface IDriverInfosOpertor
    {
        void AddDriverInfo(DriverInfo info);
        DriverInfo GetDriverInfo(string name);

        Dictionary<string, DriverInfo> DriverInfos { get; }

    }
}
