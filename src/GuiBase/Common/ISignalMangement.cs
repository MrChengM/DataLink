using DataServer.Points;
using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Common
{
    public interface ISignalMangement
    {
        void Register(ISignalChanged host, string signalName);
        //void Register(ISignalChanged host, List<string> signalName);

        void Register(ISignalChanged host, GSignal signal);
        void Register(ISignalChanged host,GSignalSet signalSet);
        void UnRegister(ISignalChanged host, string signalName);

        void UnRegister(ISignalChanged host,GSignal signal);
        void UnRegister(ISignalChanged host,GSignalSet signalSet);

        int UpdataTag(ITag tag);

        ITag GetTag(string Name);

        List<ITag> GetAllTag();

        WriteResult Write(SignalDTO signalDTO);
        WriteResult Write(SignalBitDTO signalDTO);
        void WriteAsync(SignalBitDTO signalDTO,Action<WriteResult> callback);

        WriteResult Write(List<SignalDTO> signalDTOs);
        WriteResult Write(List<SignalBitDTO> signalDTOs);

        event Action<SignalMappingtState, ITag> SignalMappingChangeEvent;

    }
    public enum SignalMappingtState
    {
        Add,
        Remove
    }
    public class SignalDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class SignalBitDTO
    {
        public string Name { get; set; }
        public int Bit { get; set; }
        public bool Value { get; set; }

    }
}
