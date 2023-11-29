using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DataServer.Config
{
    /// <summary>
    /// Json读取转换；
    /// Channel配置，通信类为Object类型；
    /// 目的是为了实例化反序列化具体的通讯类型
    /// 注意点：请勿在方法中引用Channel类型，会导致死循环。
    /// object=>CommPortSetting类，转换取消
    /// </summary>
    public class ChannelJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ChannelConfig) == objectType;
        }

        ///基础类型可以用Value函数读取（JToken），自定义类型必须用ToObject函数（JObject）；
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var channelConfig = new ChannelConfig();
            var jobj = serializer.Deserialize<JObject>(reader);
            var comJobj = jobj["ComunicationSetUp"];
            channelConfig.Name = jobj.Value<string>("Name");
            channelConfig.InitLevel = jobj.Value<int>("InitLevel");
            channelConfig.InitTimeOut = jobj.Value<int>("InitTimeOut");
            channelConfig.DriverInformation = jobj["DriverInformation"].ToObject<DriverInfo>();
            channelConfig.Devices = jobj["Devices"].ToObject<Dictionary<string,DeviceConfig>>();
            switch (channelConfig.DriverInformation.CommType)
            {
                case CommunicationType.Serialport:
                    channelConfig.ComunicationSetUp.SerialportSet = comJobj.ToObject<SerialportSetUp>();
                    break;
                case CommunicationType.Ethernet:
                    channelConfig.ComunicationSetUp.EthernetSet = comJobj.ToObject<EthernetSetUp>();
                    break;
                case CommunicationType.File:
                    break;
                case CommunicationType.Memory:
                    break;
                default:
                    break;
            }
            return channelConfig;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var channel = value as ChannelConfig;
            JObject o = new JObject();
            o["Name"]=channel.Name;
            o.Add("DriverInformation",JToken.FromObject(channel.DriverInformation));
            o["InitTimeOut"]=channel.InitTimeOut;
            o["InitLevel"]=channel.InitLevel;
            o["Devices"]= JToken.FromObject(channel.Devices);
            o.Add("ComunicationSetUp",JToken.FromObject(channel.ComunicationSetUp));
            serializer.Serialize(writer,o);

        }
    }
}
