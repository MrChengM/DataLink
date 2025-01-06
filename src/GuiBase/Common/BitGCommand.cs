using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataServer;
using DataServer.Log;
using DataServer.Permission;
using GuiBase.Services;
using GuiBase.Helper;
using GuiBase.Models;
using Prism.Commands;
namespace GuiBase.Common
{
    public class BitGCommand : IGCommand
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string TranslationId { get; set; }
        public bool HasPermission
        {
            get
            {
                if (_securityService != null)
                {
                    return _securityService.HasPermission(Name, ResourceType.Button);
                }
                else
                {
                    return true;
                }
            }
        }
        public bool Enable => true;
        public ICommand Command { get; set; }

        public string SignalName { get; set; }
        public int Bit { get; set; }
        public bool Value { get; set; }
        private ISecurityService _securityService;
        public IOperateRecordService _operateRecordService;

        public BitGCommand(string signalName,string translateId, int bit, string name, bool value,  ISecurityService securityService, IOperateRecordService operateRecordService)
        {
            SignalName = signalName;
            TranslationId = translateId;
            Bit = bit;
            Name = name;
            Value = value;
            _securityService = securityService;
            _securityService?.ResgisterResource(name, ResourceType.Button);
            _operateRecordService = operateRecordService;
            Command = new DelegateCommand(clickCommand);
        }

        private void clickCommand()
        {
            var signalBitDTO = new SignalBitDTO()
            {
                Name = SignalName,
                Bit = Bit,
                Value = Value
            };
            SignalMangementHelper.WriteAsync(signalBitDTO, s =>

             {
                 App.Current.Dispatcher.Invoke(() =>
                     {
                        string resultId;
                         if (s.Result == OperateResult.NG)
                         {
                             MessageBox.Show(s.Messages);
                              resultId = TranslateCommonId.SuccessfulId;
                         }
                         else
                         {
                              resultId = TranslateCommonId.FailId;

                         }
                         var trancode = $"idm={TranslateCommonId.ButtonOperDescId}||ids={TranslationId}||ntr={SignalName}:{Bit}||ntr={Value}||ids={resultId}";
                         var mes = $"{Name} button is checked,point name:{SignalName}:{Bit},value:{Value},operate result:{s.Result}";
                         _operateRecordService.Insert(trancode, mes);
                     });
             }
            );
        }
    }
}
