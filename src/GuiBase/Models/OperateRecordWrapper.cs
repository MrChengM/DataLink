using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Log;
using GuiBase.Services;
using Prism.Mvvm;

namespace GuiBase.Models
{
 public class OperateRecordWrapper : BindableBase
    {
        private ILocalizationService _localizationService;

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value, "UserName"); }
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value, "Id"); }
        }

        private DateTime time;

        public DateTime Time
        {
            get { return time; }
            set { SetProperty(ref time, value, "Time"); }
        }

        private string computerInfor;

        public string ComputerInfor
        {
            get { return computerInfor; }
            set { SetProperty(ref computerInfor, value, "ComputerInfor"); }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value, "Message"); }
        }
        private string transcode;

        public string Transcode
        {
            get { return transcode; }
            set { SetProperty(ref transcode, value, "Transcode"); }
        }


        public static OperateRecordWrapper Convert(OperateRecord record,ILocalizationService service)
        {
            OperateRecordWrapper result = new OperateRecordWrapper();
            result.UserName = record.UserName;
            result.Id = record.Id;
            result.Time = record.Time;
            result.ComputerInfor = record.ComputerInfor;
            result.Transcode = record.Transcode;
            result.Message = service.TranslateBaseOnRules(record.Transcode);
            return result;
           
        }
    }
}
