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
    /// OperRecordView.xaml 的交互逻辑
    /// </summary>
    public partial class OperRecordView : UserControl
    {
        public OperRecordView()
        {
            InitializeComponent();
        }
        private void StartCombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            StartCombinedCalendar.SelectedDate = ((OperRecordViewModel)DataContext).SelectCondition.StartTime;
            StartCombinedClock.Time = ((OperRecordViewModel)DataContext).SelectCondition.StartTime;

        }

        private void StartCombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1") &&
               StartCombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                var combined = selectedDate.Date.AddSeconds(StartCombinedClock.Time.TimeOfDay.TotalSeconds);
                ((OperRecordViewModel)DataContext).SelectCondition.StartTime = combined;
            }
        }

        private void EndCombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1") &&
               EndCombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                var combined = selectedDate.Date.AddSeconds(EndCombinedClock.Time.TimeOfDay.TotalSeconds);
                ((OperRecordViewModel)DataContext).SelectCondition.EndTime = combined;
            }
        }

        private void EndCombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {


            EndCombinedCalendar.SelectedDate = ((OperRecordViewModel)DataContext).SelectCondition.EndTime;
            EndCombinedClock.Time = ((OperRecordViewModel)DataContext).SelectCondition.EndTime;

        }
    }
}
