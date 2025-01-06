using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Common
{
    public delegate List<IGCommand> GeneralCommand(string signalName);
    public interface IGCommandSet
    {
        void AddGeneralCommandForType(string type, GeneralCommand generalCommand);
        void RegisterCommand(string type, string signalName);
        void CleanUp();
        List<IGCommand> this[string key] { get; }
        List<IGCommand> GetGCommands(string type, string signalName);
    }
}
