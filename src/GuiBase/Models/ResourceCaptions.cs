using GuiBase.Services;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Models
{
    /// <summary>
    /// 根据翻译获取资源用名称字段内容
    /// </summary>
    public class ResourceCaptions : BindableBase
    {
        private ILocalizationService _localizationService;
        public ResourceCaptions(ILocalizationService localizationService)
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
        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value, "Description"); }
        }

        private string disable;

        public string Disable
        {
            get { return disable; }
            set { SetProperty(ref disable, value, "Disable"); }
        }

        private string parentName;

        public string ParentName
        {
            get { return parentName; }
            set { SetProperty(ref parentName, value, "ParentName"); }
        }

        private string parentId;

        public string ParentId
        {
            get { return parentId; }
            set { SetProperty(ref parentId, value, "ParentId"); }
        }

        private string operate;

        public string Operate
        {
            get { return operate; }
            set { SetProperty(ref operate, value, "Operate"); }
        }

        private string select;

        public string Select
        {
            get { return select; }
            set { SetProperty(ref select, value, "Select"); }
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



        public void GetContent()
        {
            Id = _localizationService[TranslateCommonId.IdId];
            Name = _localizationService[TranslateCommonId.NameId];
            Description = _localizationService[TranslateCommonId.DescriptionId];
            Disable = _localizationService[TranslateCommonId.DisableId];
            ParentName = _localizationService[TranslateCommonId.ParentNameId];
            ParentId = _localizationService[TranslateCommonId.ParentIdId];
            Operate = _localizationService[TranslateCommonId.OperateId];
            Select = _localizationService[TranslateCommonId.SelectId];
            Confirm = _localizationService[TranslateCommonId.ConfirmId];
            Cancel = _localizationService[TranslateCommonId.CancelId];
            Edit = _localizationService[TranslateCommonId.EditId];
            Delete = _localizationService[TranslateCommonId.DeleteId];
        }
    }
}
