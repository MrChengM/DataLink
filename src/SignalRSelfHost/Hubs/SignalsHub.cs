using DataServer;
using DataServer.Log;
using DataServer.Points;
using DataServer.Task;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRSelfHost.Hubs
{
    [HubName("Signals")]
    public class SignalsHub : Hub
    {
        private ISignalsHubProxy _signalProxy;

        private ILog _log;

        public SignalsHub(ILog log, ISignalsHubProxy hubProxy)
        {
            _log = log;
            _signalProxy = hubProxy;
        }

        public override Task OnConnected()
        {
            _log.InfoLog("SignalServer: Client have Connected ,ConnectId '{0}',HubName 'Signals'", Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {

            _log.InfoLog("SignalServer: Client have Reconnected ,connectId '{0}',HubName 'Signals'", Context.ConnectionId);
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            removeCurrentIdTags();
            _log.InfoLog("SignalServer: Client have Disconnected ,connectId '{0}',HubName 'Signals'", Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        private void removeCurrentIdTags()
        {
            var mapping = _signalProxy.SubscribeTagMapping;
            var connectId = Context.ConnectionId;
            mapping.TryRemove(connectId, out _);
        }
        public void Subscribe(string tagName)
        {
            string connectId = Context.ConnectionId;
            var mapping = _signalProxy.SubscribeTagMapping;
            if (mapping.ContainsKey(connectId))
            {
                var tags = _signalProxy.SubscribeTagMapping[connectId];
                if (tags == null)
                {
                    tags = new List<string>();
                    tags.Add(tagName);
                    mapping[connectId] = tags;
                    fristSubSendValue(tagName);
                }
                else
                {
                    if (!tags.Contains(tagName))
                    {
                        tags.Add(tagName);
                        fristSubSendValue(tagName);
                    }
                }
            }
            else
            {
                if (mapping.TryAdd(connectId, new List<string>() { tagName }))
                {
                    fristSubSendValue(tagName);
                }
            }
        }
        public void SubscribeMultiple(List<string> tagNames)
        {
            string connectId = Context.ConnectionId;
            var mapping = _signalProxy.SubscribeTagMapping;
            if (mapping.ContainsKey(connectId))
            {
                var tags = mapping[connectId];
                if (tags == null)
                {
                    mapping[connectId] = tagNames;
                    fristSubSendValue(tagNames);
                }
                else
                {
                    foreach (var tagName in tagNames)
                    {
                        if (!tags.Contains(tagName))
                        {
                            tags.Add(tagName);
                            fristSubSendValue(tagName);
                        }
                    }
                }
            }
            else
            {
                if (mapping.TryAdd(connectId, tagNames))
                {
                    fristSubSendValue(tagNames);
                }
            }
        }

        private void fristSubSendValue(string tagName)
        {
            _signalProxy.SendTag(Context.ConnectionId, tagName);
        }
        private void fristSubSendValue(List<string> tagNames)
        {
            _signalProxy.SendTags(Context.ConnectionId, tagNames);
        }
        public void Unsubscribe(string tagName)
        {
            string connectId = Context.ConnectionId;
            var mapping = _signalProxy.SubscribeTagMapping;
            if (mapping.ContainsKey(connectId))
            {
                var tags = mapping[connectId];
                if (tags != null && tags.Contains(tagName))
                {
                    tags.Remove(tagName);
                }
            }

        }
        public void UnsubscribeMultiple(List<string> tagNames)
        {
            foreach (var tagName in tagNames)
            {
                Unsubscribe(tagName);
            }

        }

        public Tag ReadTag(string tagName)
        {
            return _signalProxy.GetTag(tagName);

        }
        public List<Tag> ReadTags(List<string> tagNames)
        {
            var result = new List<Tag>();
            foreach (var tagName in tagNames)
            {
                result.Add(_signalProxy.GetTag(tagName));
            }
            return result;
        }
        public WriteResult WriteTag(Tag tag)
        {
            return _signalProxy.WriteTag(tag);

        }
        public WriteResult WriteTags(List<Tag> tags)
        {
            var result = new WriteResult();
            foreach (var tag in tags)
            {
                result = _signalProxy.WriteTag(tag);
                if (result.Result == OperateResult.NG)
                {
                    return result;
                }

            }
            return result;
        }
    }
}
