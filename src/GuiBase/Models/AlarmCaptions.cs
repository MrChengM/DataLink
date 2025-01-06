using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using GuiBase.Services;
namespace GuiBase.Models
{
    public class AlarmCaptions : BindableBase
    {
        private ILocalizationService _localizationService;

        private string alarmName;
        public string AlarmName
        {
            get { return alarmName; }
            set { SetProperty(ref alarmName, value, "AlarmName"); }
        }

        private string partName;

        public string PartName
        {
            get { return partName; }
            set { SetProperty(ref partName, value, "PartName"); }
        }

        private string alarmLevel;

        public string AlarmLevel
        {
            get { return alarmLevel; }
            set { SetProperty(ref alarmLevel, value, "AlarmLevel"); }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value, "Description"); }
        }

        private string group;

        public string Group
        {
            get { return group; }
            set { SetProperty(ref group, value, "Group"); }
        }
        private string appearTime;

        public string AppearTime
        {
            get { return appearTime; }
            set { SetProperty(ref appearTime, value, "AppearTime"); }
        }

        private string endTime;

        public string EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value, "EndTime"); }
        }


        private string counts;

        public string Counts
        {
            get { return counts; }
            set { SetProperty(ref counts, value, "Counts"); }
        }
        private string l1View;

        public string L1View
        {
            get { return l1View; }
            set { SetProperty(ref l1View, value, "L1View"); }
        }
        private string l2View;

        public string L2View
        {
            get { return l2View; }
            set { SetProperty(ref l2View, value, "L2View"); }
        }
        private string confirm;

        public string Confirm
        {
            get { return confirm; }
            set { SetProperty(ref confirm, value, "Confrim"); }
        }
        private string duration;

        public string Duration
        {
            get { return duration; }
            set { SetProperty(ref duration, value, "Duration"); }
        }


        public AlarmCaptions(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public void GetContent()
        {
            AlarmName = _localizationService[TranslateCommonId.AlarmNameId];
            PartName = _localizationService[TranslateCommonId.PartNameId];
            AlarmLevel = _localizationService[TranslateCommonId.AlarmLevelId];
            Description = _localizationService[TranslateCommonId.DescriptionId];
            Group = _localizationService[TranslateCommonId.GroupId];
            AppearTime = _localizationService[TranslateCommonId.AppearTimeId];
            EndTime = _localizationService[TranslateCommonId.EndTimeId];
            Counts = _localizationService[TranslateCommonId.CountsId];
            Confirm = _localizationService[TranslateCommonId.ConfirmId];
            L1View = _localizationService[TranslateCommonId.L1ViewId];
            L2View = _localizationService[TranslateCommonId.L2ViewId];
            Duration = _localizationService[TranslateCommonId.DurationId];
        }
    }
}
