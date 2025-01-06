using DataServer;
using DataServer.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Prism.Mvvm;
using GuiBase.Services;
using Prism.Ioc;

namespace GuiBase.Models
{
    public class HistoryAlarmWrapper:BindableBase
    {
        private static readonly SolidColorBrush ALARM25COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm25");
        private static readonly SolidColorBrush ALARM50COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm50");
        private static readonly SolidColorBrush ALARM75COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm75");
        private static readonly SolidColorBrush ALARM100COLOR = (SolidColorBrush)Application.Current.FindResource("Alarm100");
        public string AlarmName { get; set; }
        public string PartName { get; set; }

        public string AlarmDescrible { get; set; }
        public AlarmType AlarmLevel { get; set; }
       
        public string AlarmNumber { get; set; }
        public string L1View { get; set; }
        public string L2View { get; set; }
        public string AlarmGroup { get; set; }
        public DateTime AppearTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeSpan Duration => EndTime - AppearTime;

        public SolidColorBrush RowColor { get; set; }

        private string localizationDescrible;

        public string LocalizationDescrible
        {
            get { return localizationDescrible; }
            set { SetProperty(ref localizationDescrible, value, "LocalizationDescrible"); }
        }


        public static HistoryAlarmWrapper Convert(HistoryAlarm histroyAlarm)
        {
            var histroyAlarmWrapper = new HistoryAlarmWrapper()
            {
                AlarmName = histroyAlarm.Name,
                PartName = histroyAlarm.PartName,
                AlarmDescrible = histroyAlarm.AlarmDesc,
                AlarmLevel = histroyAlarm.AlarmLevel,
                AlarmNumber = histroyAlarm.AlarmNumber,
                L1View = histroyAlarm.L1View,
                L2View = histroyAlarm.L2View,
                AlarmGroup = histroyAlarm.AlarmGroup,
                AppearTime = histroyAlarm.AppearTime,
                EndTime = histroyAlarm.EndTime,
            };
            switch (histroyAlarm.AlarmLevel)
            {
                case AlarmType.Information:
                    histroyAlarmWrapper.RowColor = ALARM25COLOR;
                    break;
                case AlarmType.Trivial:
                    histroyAlarmWrapper.RowColor = ALARM50COLOR;
                    break;
                case AlarmType.Minor:
                    histroyAlarmWrapper.RowColor = ALARM75COLOR;
                    break;
                case AlarmType.Major:
                    histroyAlarmWrapper.RowColor = ALARM100COLOR;
                    break;
                default:
                    break;
            }
            var ls = ContainerLocator.Container?.Resolve<ILocalizationService>();
            if (ls != null && histroyAlarmWrapper.AlarmNumber != null)
            {
                histroyAlarmWrapper.LocalizationDescrible = ls[histroyAlarmWrapper.AlarmNumber];
            }
            return histroyAlarmWrapper;
        }

    }
}
