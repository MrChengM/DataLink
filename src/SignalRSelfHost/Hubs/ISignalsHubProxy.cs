using DataServer;
using DataServer.Points;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRSelfHost.Hubs
{
    /// <summary>
    /// SignalsHub 类外部调用客户端方法代理类，防止多次调用客户端方法
    /// </summary>
    public interface ISignalsHubProxy
    {
        ConcurrentDictionary<string, List<string>> SubscribeTagMapping { get; }
        void SendTag(string connectId, string tagName);
        void SendTags(string connectId, List<string> tagNames);

        Tag GetTag(string tagName);
        WriteResult WriteTag(Tag tag);
        
    }
}
