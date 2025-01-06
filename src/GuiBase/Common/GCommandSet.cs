using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using GuiBase.Services;
using DataServer.Log;

namespace GuiBase.Common
{
    public class GCommandSet : IGCommandSet
    {
        private ILog _log;
        private Dictionary<string, Dictionary<string, List<IGCommand>>> _commandSets = new Dictionary<string, Dictionary<string, List<IGCommand>>>();
        private Dictionary<string, GeneralCommand> _generalCommands = new Dictionary<string, GeneralCommand>();

        public GCommandSet(IGeneralCommandBuilder commandBulder,ILog log)
        {
            _log = log;
            foreach (var generalCommand in commandBulder.GeneralCommands)
            {
                AddGeneralCommandForType(generalCommand.Key, generalCommand.Value);
            }
        }
        public List<IGCommand> this[string key]
        {
            get
            {
                string[] keys = key.Split(new char[]
               {
                    '#'
               });
                string type = keys[0];
                string signalName = keys[1];
                return GetGCommands(type, signalName);
            }
        }

        public void AddGeneralCommandForType(string type, GeneralCommand generalCommand)
        {
            if (_commandSets.ContainsKey(type))
            {
                _commandSets.Remove(type);
            }
            _commandSets.Add(type, new Dictionary<string, List<IGCommand>>());
            if (_generalCommands.ContainsKey(type))
            {
                _generalCommands.Remove(type);
            }
            _generalCommands.Add(type, generalCommand);
        }
        
        public void CleanUp()
        {
            _commandSets = null;
            _generalCommands = null;
        }

        public List<IGCommand> GetGCommands(string type, string signalName)
        {
            if (_commandSets.ContainsKey(type))
            {
                var typeMapping = _commandSets[type];
                if (typeMapping.ContainsKey(signalName))
                {
                    return typeMapping[signalName];
                }
            }
            return null;
        }


        public void RegisterCommand(string type, string signalName)
        {
            if (_generalCommands.TryGetValue(type, out GeneralCommand general))
            {
                var commands = general(signalName);
                if (!_commandSets.ContainsKey(type))
                {
                    _commandSets.Add(type, new Dictionary<string, List<IGCommand>>());
                }
                if (!_commandSets[type].ContainsKey(signalName))
                {
                    _commandSets[type].Add(signalName, commands);
                }
            }
        }
    }
}
