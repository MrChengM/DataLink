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
    /// L3BaseView.xaml 的交互逻辑
    /// </summary>
    public partial class L3BaseView : UserControl
    {
        public L3BaseView()
        {
            InitializeComponent();
        }
        private void StartCombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            StartCombinedCalendar.SelectedDate = ((L3BaseViewModel)DataContext).SelectCondition.StartDate;
            StartCombinedClock.Time = ((L3BaseViewModel)DataContext).SelectCondition.StartDate;

        }

        private void StartCombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1") &&
               StartCombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                var combined = selectedDate.Date.AddSeconds(StartCombinedClock.Time.TimeOfDay.TotalSeconds);
                ((L3BaseViewModel)DataContext).SelectCondition.StartDate = combined;
            }
        }

        private void EndCombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, "1") &&
               EndCombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                var combined = selectedDate.Date.AddSeconds(EndCombinedClock.Time.TimeOfDay.TotalSeconds);
                ((L3BaseViewModel)DataContext).SelectCondition.EndDate = combined;
            }
        }

        private void EndCombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {


            EndCombinedCalendar.SelectedDate = ((L3BaseViewModel)DataContext).SelectCondition.EndDate;
            EndCombinedClock.Time = ((L3BaseViewModel)DataContext).SelectCondition.EndDate;

        }
    }
}
