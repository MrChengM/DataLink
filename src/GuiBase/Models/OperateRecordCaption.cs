using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Services;
using Prism.Mvvm;

namespace GuiBase.Models
{
 public   class OperateRecordCaption:BindableBase
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

        private string time;

        public string Time
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

        private string confirm;
        public string Confirm
        {
            get { return confirm; }
            set { SetProperty(ref confirm, value, "Confirm"); }
        }

        private string cancel;

        public string Cancel
        {
            get { return cancel; }
            set { SetProperty(ref cancel, value, "Cancel"); }
        }
        private string appearTime;

        public string AppearTime
        {
            get { return appearTime; }
            set { SetProperty(ref appearTime, value, "AppearTime"); }
        }


        public OperateRecordCaption(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        
        public void GetContent()
        {
            UserName = _localizationService.Translate(TranslateCommonId.UserNameId);
            Id = _localizationService.Translate(TranslateCommonId.IdId);
            Time = _localizationService.Translate(TranslateCommonId.TimeId);
            ComputerInfor = _localizationService.Translate(TranslateCommonId.ComputerInforId);
            Message = _localizationService.Translate(TranslateCommonId.MessageId);
            Confirm = _localizationService.Translate(TranslateCommonId.ConfirmId);
            Cancel= _localizationService.Translate(TranslateCommonId.CancelId);
            AppearTime = _localizationService.Translate(TranslateCommonId.AppearTimeId);
        }
    }
}
