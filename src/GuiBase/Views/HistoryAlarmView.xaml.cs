using GuiBase.ViewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GuiBase.Views
{
    /// <summary>
    /// HistoryAlarmView.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryAlarmView : UserControl
    {
        public HistoryAlarmView()
        {
            InitializeComponent();
        }

        //public void CombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        //{
        //    StartCombinedCalendar.SelectedDate = ((HistoryAlarmViewModel)DataContext).SelectCondition.StartDate;
        //    StartCombinedClock.Time = ((HistoryAlarmViewModel)DataContext).SelectCondition.StartDate;
        //}

        //public void CombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        //{
        //    if (Equals(eventArgs.Parameter, "1") &&
        //        StartCombinedCalendar.SelectedDate is DateTime selectedDate)
        //    {
        //        //var combined = selectedDate.AddSeconds(CombinedClock.Time.TimeOfDay.TotalSeconds);
        //        //((HistoryAlarmViewModel)DataContext).StartDate = combined;
        //        ((HistoryAlarmViewModel)DataContext).SelectCondition.StartDate = selectedDate;
        //    }
        //}
        private void StartCombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            StartCombinedCalendar.SelectedDate = ((HistoryAlarmViewModel)DataContext).SelectCondition.StartDate;
            StartCombinedClock.Time = ((HistoryAlarmViewModel)DataContext).SelectCondition.StartDate;

        }

        private void StartCombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1") &&
               StartCombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                var combined = selectedDate.Date.AddSeconds(StartCombinedClock.Time.TimeOfDay.TotalSeconds);
                ((HistoryAlarmViewModel)DataContext).SelectCondition.StartDate = combined;
            }
        }

        private void EndCombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1") &&
               EndCombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                var combined = selectedDate.Date.AddSeconds(EndCombinedClock.Time.TimeOfDay.TotalSeconds);
                ((HistoryAlarmViewModel)DataContext).SelectCondition.EndDate = combined;
            }
        }

        private void EndCombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
           

            EndCombinedCalendar.SelectedDate = ((HistoryAlarmViewModel)DataContext).SelectCondition.EndDate;
            EndCombinedClock.Time = ((HistoryAlarmViewModel)DataContext).SelectCondition.EndDate;

        }
    }
}
