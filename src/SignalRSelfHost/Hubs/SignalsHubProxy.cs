using DataServer;
using DataServer.Points;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
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
    public class SignalsHubProxy : ISignalsHubProxy
    {

        private IHubContext _hubContext;
        public ConcurrentDictionary<string, List<string>> SubscribeTagMapping { get; private set; }
        private IPointMapping _pointMapping;
        public SignalsHubProxy(IPointMapping pointMapping)
        {
            ///采用IOC容器，GlobalHost失效
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalsHub>();
            //_hubContext = connectionManager.GetHubContext<SignalsHub>();

            SubscribeTagMapping = new ConcurrentDictionary<string, List<string>>();
            _pointMapping = pointMapping;
            _pointMapping.PointChangeEvent += OnPointChange;
        }

        private readonly object locker = new object();
        public void OnPointChange(Tag tag)
        {
            lock (locker)
            {
                foreach (var tags in SubscribeTagMapping)
                {
                    if (tags.Value.Contains(tag.Name))
                    {
                        _hubContext.Clients.Client(tags.Key).receiveTag(tag);
                    }
                }
            }
        }
        private readonly object locker2 = new object();
        public void SendTag(string connectId, string tagName)
        {
            lock (locker2)
            {

                var pointNameIndex = StringHandlerExtension.SplitEndWith(tagName);
                if (pointNameIndex != null)
                {
                    var tag = _pointMapping.GetTag(pointNameIndex) as Tag;
                    _hubContext.Clients.Client(connectId).receiveTag(tag);

                }

            }
        }
        private readonly object locker3 = new object();

        public void SendTags(string connectId, List<string> tagNames)
        {
            lock (locker3)
            {
                var tags = new List<Tag>(); 
                foreach (var tagName in tagNames)
                {
                    var pointNameIndex = StringHandlerExtension.SplitEndWith(tagName);
                    if (pointNameIndex != null)
                    {
                        var tag = _pointMapping.GetTag(pointNameIndex) as Tag;
                        tags.Add(tag);
                    }
                }
                if (tags.Count>0)
                {
                    _hubContext.Clients.Client(connectId).receiveTags(tags);
                }
            }
        }
        public Tag GetTag(string tagName)
        {
            Tag result = null;
            var pointNameIndex = StringHandlerExtension.SplitEndWith(tagName);
            if (pointNameIndex != null)
            {
                result = _pointMapping.GetTag(pointNameIndex) as Tag;
            }
            return result;
        }
        public WriteResult WriteTag(Tag tag)
        {
            WriteResult result=new WriteResult();
            var pointNameIndex = StringHandlerExtension.SplitEndWith(tag.Name);
            if (pointNameIndex != null)
            {
                result = _pointMapping.WritePoint(pointNameIndex.PointName, pointNameIndex.Index, tag.Value);
                
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"Tag:'{tag.Name}', name type error!";
            }
            return result;
        }
    }
}
