using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.ObjectModel;
using DataServer.Alarm;
using GuiBase.Services;

namespace GuiBase.Models
{
    public class AlarmWrapper : BindableBase
    {
        #region Private Field

        private readonly SolidColorBrush ALARM25COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm25");
        private readonly SolidColorBrush ALARM50COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm50");
        private readonly SolidColorBrush ALARM75COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm75");
        private readonly SolidColorBrush ALARM100COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm100");
        private readonly SolidColorBrush ALARMDISUNCHECK = (SolidColorBrush)Application.Current.FindResource("AlarmDisapperWithUncheck");
        private readonly SolidColorBrush ALARENABLECHECKED = (SolidColorBrush)Application.Current.FindResource("AlarmEnableWithChecked");

        public ILocalizationService Localization { get; }
        public IOperateRecordService RecordService { get; }

        #endregion
        public event Action<string> AlarmConfrimEvent;
        #region Property
        public string AlarmName { get; set; }
        public string PartName { get; set; }

        public AlarmType AlarmLevel { get; set; }

        public string AlarmGroup { get; set; }

        public  string AlarmNumber{ get; private set; }

        private string alarmDescrible;

        public string AlarmDescrible
        {
            get { return alarmDescrible; }
            set { SetProperty(ref alarmDescrible, value, "AlarmDescrible"); }
        }

        private string localizationDescrible;

        public string LocalizationDescrible
        {
            get { return localizationDescrible; }
            set { SetProperty(ref localizationDescrible, value, "LocalizationDescrible"); }
        }


        public string L1View { get; set; }
        public string L2View { get; set; }
        public DateTime AppearTime { get; set; }

        public AlarmDisplayStatus Status { get; set; }

        private int counts;

        public int Counts
        {
            get { return counts; }
            set { SetProperty(ref counts, value, "Counts"); }
        }

        private SolidColorBrush rowColor;

        public SolidColorBrush RowColor
        {
            get { return rowColor; }
            set { SetProperty(ref rowColor, value, "RowColor"); }
        }

        private bool confirmEnable;

        public bool ConfirmEnable
        {
            get { return confirmEnable; }
            set { SetProperty(ref confirmEnable, value, "ConfirmEnable"); }
        }
        //private bool isConfrim;

        //public bool IsConfrim
        //{
        //    get { return isConfrim; }
        //    set { SetProperty(ref isConfrim, value, "IsConfrim"); }
        //}


        public DelegateCommand  ConfirmCommand { get; set; }
        #endregion

        public AlarmWrapper()
        {
            ConfirmCommand = new DelegateCommand(Confrim);
            Localization = ContainerLocator.Container?.Resolve<ILocalizationService>();
                Localization.LanguageChanged += onLanguageChanged;
            RecordService = ContainerLocator.Container?.Resolve<IOperateRecordService>();

        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
                LocalizationDescrible = Localization[AlarmNumber];
        }

        public void Confrim()
        {
            if (ConfirmEnable)
            {
                AlarmConfrimEvent?.Invoke(AlarmName);
                var trancode = $"idm={TranslateCommonId.AlarmConfirmOperDescId}||ntr={AlarmName}||ids={AlarmNumber}";
                var mes = $"Alarm is confirmed,alarm point:{AlarmName},alarm des:{AlarmDescrible}";
                RecordService.Insert(trancode, mes);
            }
        }

        public void RefreshRowColor()
        {
            if (Status== AlarmDisplayStatus.Enable)
            {
                switch (AlarmLevel)
                {
                    case AlarmType.Information:
                        RowColor = ALARM25COLOR;
                        break;
                    case AlarmType.Trivial:
                        RowColor = ALARM50COLOR;
                        break;
                    case AlarmType.Minor:
                        RowColor = ALARM75COLOR;
                        break;
                    case AlarmType.Major:
                        RowColor = ALARM100COLOR;
                        break;
                    default:
                        break;
                }
            }
            else if (Status == AlarmDisplayStatus.DisappearWithUnCheck)
            {
                RowColor = ALARMDISUNCHECK;
            }
            else if (Status == AlarmDisplayStatus.EnableWithChecked)
            {
                RowColor = ALARENABLECHECKED;
            }
        }
        public static AlarmWrapper Convert(AlarmInstance alarmInstance)
        {
            AlarmWrapper result = new AlarmWrapper();
            result.AlarmName = alarmInstance.Name;
            result.AlarmLevel = alarmInstance.AlarmLevel;
            result.AlarmGroup = alarmInstance.AlarmGroup;
            result.AlarmDescrible = alarmInstance.AlarmDesc;
            result.PartName = alarmInstance.PartName;
            result.L1View = alarmInstance.L1View;
            result.L2View = alarmInstance.L2View;
            result.AppearTime = alarmInstance.AppearTime;
            result.Counts = alarmInstance.Count;
            result.AlarmNumber = alarmInstance.AlarmNumber;
            result.LocalizationDescrible = result.Localization[result.AlarmNumber];

            if (alarmInstance.ConfirmMode == ConfirmMode.Normal)
            {
                result.ConfirmEnable = true;
            }
            if (alarmInstance.IsEnable && !alarmInstance.IsCheck)
            {
                result.Status = AlarmDisplayStatus.Enable;
            }
            else if (alarmInstance.IsEnable && alarmInstance.IsCheck)
            {
                result.Status = AlarmDisplayStatus.EnableWithChecked;
            }
            else if (!alarmInstance.IsEnable && !alarmInstance.IsCheck)
            {
                result.Status = AlarmDisplayStatus.DisappearWithUnCheck;
            }
            result.RefreshRowColor();
            return result;
        }
        public void  Refresh(AlarmInstance alarmInstance)
        {
            AppearTime = alarmInstance.AppearTime;
            Counts = alarmInstance.Count;
            if (alarmInstance.ConfirmMode == ConfirmMode.Normal)
            {
                ConfirmEnable = true;
            }
            if (alarmInstance.IsEnable && !alarmInstance.IsCheck)
            {
                Status = AlarmDisplayStatus.Enable;
            }
            else if (alarmInstance.IsEnable && alarmInstance.IsCheck)
            {
                Status = AlarmDisplayStatus.EnableWithChecked;
            }
            else if (!alarmInstance.IsEnable && !alarmInstance.IsCheck)
            {
                Status = AlarmDisplayStatus.DisappearWithUnCheck;
            }
            RefreshRowColor();
        }
        public void CopyFrom(AlarmWrapper alarm)
        {
            AlarmName = alarm.AlarmName;
            AlarmLevel = alarm.AlarmLevel;
            AlarmNumber = alarm.AlarmNumber;
            AlarmGroup = alarm.AlarmGroup;
            AlarmDescrible = alarm.AlarmDescrible;
            LocalizationDescrible = alarm.LocalizationDescrible;
            PartName = alarm.PartName;
            L1View = alarm.L1View;
            L2View = alarm.L2View;
            AppearTime = alarm.AppearTime;
            Counts = alarm.Counts;
            Status = alarm.Status;
            RowColor = alarm.rowColor;
            ConfirmEnable = alarm.confirmEnable;

        }

        public void Clear()
        {

            Localization.LanguageChanged -= onLanguageChanged;
        }
    }

    public enum AlarmDisplayStatus
    {
        Enable,
        DisappearWithUnCheck,
        EnableWithChecked
    }
}
