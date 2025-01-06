using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Services;
using Prism.Mvvm;

namespace GuiBase.Models
{
    public  class RoleCaptions:BindableBase
    {

        private ILocalizationService _localizationService;

        public RoleCaptions(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value, "Id"); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        private string status;

        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value, "Description"); }
        }

        private string createId;

        public string CreateId
        {
            get { return createId; }
            set { SetProperty(ref createId, value, "CreateId"); }
        }

        private string createTime;

        public string CreateTime
        {
            get { return createTime; }
            set { SetProperty(ref createTime, value, "CreateTime"); }
        }

        private string operate;

        public string Operate
        {
            get { return operate; }
            set { SetProperty(ref operate, value, "Operate"); }
        }

        private string add;

        public string Add
        {
            get { return add; }
            set { SetProperty(ref add, value, "Add"); }
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

        private string edit;

        public string Edit
        {
            get { return edit; }
            set { SetProperty(ref edit, value, "Edit"); }
        }

        private string delete;

        public string Delete
        {
            get { return delete; }
            set { SetProperty(ref delete, value, "Delete"); }
        }
        private string assign;

        public string Assign
        {
            get { return assign; }
            set { SetProperty(ref assign, value, "Assign"); }
        }



        public void GetContent()
        {
            Id = _localizationService[TranslateCommonId.IdId];
            Name = _localizationService[TranslateCommonId.NameId];
            Status = _localizationService[TranslateCommonId.StatusId];
            CreateId = _localizationService[TranslateCommonId.CreateIdId];
            CreateTime = _localizationService[TranslateCommonId.CreateTimeId];
            Operate = _localizationService[TranslateCommonId.OperateId];
            Add = _localizationService[TranslateCommonId.AddId];
            Confirm = _localizationService[TranslateCommonId.ConfirmId];
            Cancel = _localizationService[TranslateCommonId.CancelId];
            Edit = _localizationService[TranslateCommonId.EditId];
            Delete = _localizationService[TranslateCommonId.DeleteId];
            Assign = _localizationService[TranslateCommonId.AssignId];
        }
    }
}
