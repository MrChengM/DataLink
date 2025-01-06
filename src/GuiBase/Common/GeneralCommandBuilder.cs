using DataServer;
using GuiBase.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Log;
namespace GuiBase.Common
{
    public class GeneralCommandBuilder:IGeneralCommandBuilder
    {
        private ISecurityService _securityService;
        private ILog _log;
        private IOperateRecordService _operateRecordService;
        public Dictionary<string,GeneralCommand> GeneralCommands { get; private set; }
        public GeneralCommandBuilder(ISecurityService securityService,IOperateRecordService operateRecordService, ILog log)
        {
            _securityService = securityService;
            _operateRecordService = operateRecordService;
            _log = log;
            initCommands();
        }

        void initCommands()
        {
            GeneralCommands = new Dictionary<string, GeneralCommand>();
            GeneralCommands.Add("PowerBoxCommand", PowerBoxCommand);
            GeneralCommands.Add("Conv_47Command", Conv_47Command);

        }
        public BitGCommand CreateBitCommand(string name,string translateId, string signalName, int bit, bool value)
        {
            return new BitGCommand(signalName, translateId, bit, name, value, _securityService, _operateRecordService);
        }
        public List<IGCommand> PowerBoxCommand(string signalName)
        {
            List<IGCommand> result = new List<IGCommand>();
            result.Add(CreateBitCommand("Auto","G_Text_Auto",signalName,0,true));
            result.Add(CreateBitCommand("Manual", "G_Text_Manual", signalName,1, true));
            result.Add(CreateBitCommand("Reset", "G_Text_Reset", signalName, 2, true));
            return result;
        }
        public List<IGCommand> Conv_47Command(string signalName)
        {
            List<IGCommand> result = new List<IGCommand>();
            result.Add(CreateBitCommand("Start", "G_Text_Start", signalName, 0, true));
            result.Add(CreateBitCommand("Stop", "G_Text_Stop", signalName, 1, true));
            result.Add(CreateBitCommand("Reset", "G_Text_Reset", signalName, 2, true));
            return result;
        }
    }
}
