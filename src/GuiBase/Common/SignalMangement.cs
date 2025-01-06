using DataServer;
using DataServer.Log;
using DataServer.Points;
using GuiBase.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Services;
using Utillity.Data;

namespace GuiBase.Common
{
    public class SignalMangement : ISignalMangement
    {
        private Dictionary<string, ITag> tagsMapping;

        public event Action<SignalMappingtState, ITag> SignalMappingChangeEvent;

        private ILog _log;
        private ISignalService _signalService;
        public SignalMangement(ILog log, ISignalService signalService)
        {
            _log = log;
            _signalService = signalService;
            _signalService.ConnectStatusChangeEvent += onConnectStatusChangeEvent;
            _signalService.ReceiveTagEvent += onReceiveTagEvent;
            _signalService.ReceiveTagsEvent += onReceiveTagsEvent;
            tagsMapping = new Dictionary<string, ITag>();
        }

        private void onReceiveTagsEvent(List<Tag> tags)
        {
            foreach (var tag in tags)
            {
                UpdataTag(tag);
            }
        }

        private void onReceiveTagEvent(Tag tag)
        {
            UpdataTag(tag);
        }

        private void onConnectStatusChangeEvent(bool isconnected)
        {

            if (isconnected)
            {
                _signalService.Subscribe(GetAllTagNames());
            }
        }

        private List<string> GetAllTagNames()
        {
            var tags = GetAllTag();
            var tagNames = new List<string>();
            foreach (var tag in tags)
            {
                tagNames.Add(tag.Name);
            }
            return tagNames;
        }
        private bool find(string tagName)
        {
            return tagsMapping.ContainsKey(tagName);
        }
        public List<ITag> GetAllTag()
        {
            var result = new List<ITag>();
            foreach (var tagPair in tagsMapping)
            {
                result.Add(tagPair.Value);
            }
            return result;
        }

        public ITag GetTag(string name)
        {
            if (find(name))
            {
                return tagsMapping[name];
            }
            else
            {
                return default(ITag);
            }
        }
        public void Register(ISignalChanged host, string signalName)
        {
            registerSignal(host, signalName);
        }
        public void Register(ISignalChanged host, GSignal signal)
        {
            registerSignal(host, signal.SignalName);
            registerKeepAliveSignal(host, signal);
        }

        private void registerSignal(ISignalChanged host, string signalName)
        {
            if (!find(signalName))
            {
                GTag tag = new GTag() { Name = signalName };
                tagsMapping.Add(signalName, tag);
                tag.SignalChangeEvent += host.OnSignalChanged;
                host.OnSignalChanged(tag);
                RaiseSignalMappingChanged(SignalMappingtState.Add, tag);
            }
            else
            {
                var tag = tagsMapping[signalName] as GTag;
                tag.SignalChangeEvent += host.OnSignalChanged;
                host.OnSignalChanged(tag);
            }
        }
        private void registerKeepAliveSignal(ISignalChanged host, GSignal signal)
        {
            if (!find(signal.KeepAliveSignalName))
            {
                KeepAliveTag tag = new KeepAliveTag() { Name = signal.KeepAliveSignalName };
                if (!tag.SignalBindings.Contains(signal.SignalName))
                {
                    tag.SignalBindings.Add(signal.SignalName);
                }

                tagsMapping.Add(signal.KeepAliveSignalName, tag);
                tag.KeepAliveSignalChange += host.OnKeepAliveSignalChanged;

                //绑定后获取第一次数据
                host.OnKeepAliveSignalChanged(tag.SignalBindings, tag.Connected);
                RaiseSignalMappingChanged(SignalMappingtState.Add, tag);
            }
            else
            {
                var tag = tagsMapping[signal.KeepAliveSignalName] as KeepAliveTag;
                tag.KeepAliveSignalChange += host.OnKeepAliveSignalChanged;
                host.OnKeepAliveSignalChanged(tag.SignalBindings, tag.Connected);

            }
        }
        public void UnRegister(ISignalChanged host, string signalName)
        {
            unRegisterSignal(host, signalName);
        }
        public void Register(ISignalChanged host, GSignalSet signalSet)
        {
            foreach (var signal in signalSet)
            {
                Register(host, signal);
            }
        }

        public void UnRegister(ISignalChanged host, GSignal signal)
        {
            unRegisterSignal(host, signal.SignalName);
            unRegisterKeepAliveSignal(host, signal);
        }
        private void unRegisterSignal(ISignalChanged host, string signalName)
        {
            if (find(signalName))
            {
                var tag = tagsMapping[signalName] as GTag;
                if (tag != null)
                {
                    tag.SignalChangeEvent -= host.OnSignalChanged;
                    //if (tag.SignalSubCounts == 0)
                    //{
                    //    tagsMapping.Remove(tag.Name);
                    //    RaiseSignalMappingChanged(SignalMappingtState.Remove, tag);
                    //}
                }
            }
        }
        private void unRegisterKeepAliveSignal(ISignalChanged host, GSignal signal)
        {
            if (find(signal.KeepAliveSignalName))
            {
                var tag = tagsMapping[signal.KeepAliveSignalName] as KeepAliveTag;
                if (tag != null)
                {
                    tag.KeepAliveSignalChange -= host.OnKeepAliveSignalChanged;
                    //tag.SignalBindings.Remove(signal.SignalName);
                    //if (tag.SignalBindings.Count == 0)
                    //{
                    //    tagsMapping.Remove(tag.Name);
                    //    RaiseSignalMappingChanged(SignalMappingtState.Remove, tag);
                    //}
                }
            }
        }
        public void UnRegister(ISignalChanged host, GSignalSet signalSet)
        {
            foreach (var signal in signalSet)
            {
                UnRegister(host, signal);
            }
        }

        public int UpdataTag(ITag tag)
        {
            if (find(tag.Name))
            {
                var resouceTag = tagsMapping[tag.Name];
                resouceTag.Type = tag.Type;
                resouceTag.Quality = tag.Quality;
                resouceTag.Value = tag.Value;
                resouceTag.TimeStamp = tag.TimeStamp;
                return 1;
            }
            else
            {
                return -1;
            }
        }

        protected virtual void RaiseSignalMappingChanged(SignalMappingtState state, ITag tag)
        {

            if (state == SignalMappingtState.Add)
            {
                _signalService.Subscribe(tag.Name);
            }
            else if (state == SignalMappingtState.Remove)
            {
                _signalService.Unsubscribe(tag.Name);
            }
            var handler = SignalMappingChangeEvent;
            handler?.Invoke(state, tag);
        }

        public WriteResult Write(SignalDTO signalDTO)
        {
            return _signalService.Write(GetTag(signalDTO));
        }

        public WriteResult Write(SignalBitDTO signalBitDTO)
        {
            return _signalService.Write(GetTag(signalBitDTO));
        }
        public void WriteAsync(SignalBitDTO signalBitDTO, Action<WriteResult> callback)
        {
            _signalService.WriteAsync(GetTag(signalBitDTO)).ContinueWith(s => { callback(s.Result); });
        }

        public WriteResult Write(List<SignalDTO> signalDTOs)
        {
            var tags = new List<Tag>();
            foreach (var signalDTO in signalDTOs)
            {
                tags.Add(GetTag(signalDTO));
            }
            return _signalService.Write(tags);
        }

        public WriteResult Write(List<SignalBitDTO> signalBitDTOs)
        {
            var tags = new List<Tag>();
            foreach (var signalBitDTO in signalBitDTOs)
            {
                tags.Add(GetTag(signalBitDTO));
            }
            return _signalService.Write(tags);
        }

        private Tag GetTag(SignalDTO signalDTO)
        {
            Tag result;
            var tag = GetTag(signalDTO.Name);
            if (tag != null)
            {
                result = new Tag()
                {
                    Name = tag.Name,
                    Quality = tag.Quality,
                    Type = tag.Type,
                    Value = signalDTO.Value
                };
            }
            else
            {
                result = new Tag()
                {
                    Name = signalDTO.Name,
                    Value = signalDTO.Value
                };
            }
            return result;
        }
        private Tag GetTag(SignalBitDTO signalBitDTO)
        {
            Tag result = null;
            var tag = GetTag(signalBitDTO.Name);
            if (tag == null)
            {
                tag = _signalService.Read(signalBitDTO.Name);
                
            }
            if (tag != null)
            {
                result = new Tag()
                {
                    Name = tag.Name,
                    Type = tag.Type,
                    Quality = tag.Quality,
                    Value = NetConvertExtension.SetBit(tag.Value, tag.Type, signalBitDTO.Bit, signalBitDTO.Value)
                };
            }
            return result;
        }
    }
}
