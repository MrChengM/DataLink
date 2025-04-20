using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DataServer.Config;
using System.IO;

namespace WCFRestFullAPI.Service
{
    /// <summary>
    /// 1、Get为Url带参数获取资源
    /// 2、Post 当获取资源使用时候body传入参数（比Get更安全），由于不会覆盖掉前一个值，为新增参数
    /// 3、Put 会覆盖掉前一个值，为更新参数
    /// 4、Delete删除资源,参数放在URL（与Get相同）
    /// 5、PUT/POST使用使用BodyStyle = WebMessageBodyStyle.BareWrapped格式，Body Json必须包含参数名
    /// </summary>
    [ServiceContract]
    public interface IConfigRestService
    {
        #region Get Method
        [OperationContract]
        [WebGet(UriTemplate = "Project", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ProjectConfig GetProject();

        [OperationContract]
        [WebGet(UriTemplate = "Project/Client", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ClientConfig GetClient();

        [OperationContract]
        [WebGet(UriTemplate = "Project/Server", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServersConfig GetServer();

        [OperationContract]
        [WebGet(UriTemplate = "Project/Client_ID={channelName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ChannelConfig GetChannel(string channelName);

        [OperationContract]
        [WebGet(UriTemplate = "Project/Client_ID={channelName}_{deviceName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DeviceConfig GetDevice(string channelName, string deviceName);

        [OperationContract]
        [WebGet(UriTemplate = "Project/Client_ID={channelName}_{deviceName}_{tagGroupName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        TagGroupConfig GetTagGroup(string channelName, string deviceName, string tagGroupName);

        [OperationContract]
        [WebGet(UriTemplate = "Project/Client_ID={channelName}_{deviceName}_{tagGroupName}_{tagName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        TagConfig GetTag(string channelName, string deviceName, string tagGroupName, string tagName);
        [OperationContract]
        [WebGet(UriTemplate = "DriverInformation", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string,DriverInfo> GetDriverInfos();
        #endregion
        #region Post Method
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client/Channel", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PostChannel(ChannelConfig channelConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PostDevice(string channelName,DeviceConfig deviceConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}_{deviceName}", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PostTagGroup(string channelName, string deviceName,TagGroupConfig tagGroupConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}_{deviceName}_{tagGroupName}", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PostTag(string channelName, string deviceName, string tagGroupName,TagConfig tagConfig);
        //[OperationContract]
        //[WebInvoke(UriTemplate = "DriverInformation", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //Dictionary<string, DriverInfo> PostDriverDll(Stream stream);
        [OperationContract]
        [WebInvoke(UriTemplate = "DriverInformation/{fileName}",BodyStyle = WebMessageBodyStyle.Bare, Method = "POST",  ResponseFormat = WebMessageFormat.Json)]
        UpDllFileResult UpDriverDll(string fileName, Stream stream);
        #endregion
        #region Put Method
        [OperationContract]
        [WebInvoke(UriTemplate = "Project", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PutProject(ProjectConfig projectConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client",Method ="PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PutClient(ClientConfig clientConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client/Channel", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PutChannel(ChannelConfig channelConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}", Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PutDevice(string channelName, DeviceConfig deviceConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}_{deviceName}", Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PutTagGroup(string channelName, string deviceName, TagGroupConfig tagGroupConfig);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}_{deviceName}_{tagGroupName}", Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult PutTag(string channelName, string deviceName, string tagGroupName, TagConfig tagConfig);
        #endregion

        #region Delete Method
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}", Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult DeleteChannel(string channelName);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}_{deviceName}", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult DeleteDevice(string channelName, string deviceName);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}_{deviceName}_{tagGroupName}", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult DeleteTagGroup(string channelName, string deviceName, string tagGroupName);
        [OperationContract]
        [WebInvoke(UriTemplate = "Project/Client_ID={channelName}_{deviceName}_{tagGroupName}_{tagName}", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RestAPIResult DeleteTag(string channelName, string deviceName, string tagGroupName, string tagName);
        #endregion

    }
   
}
