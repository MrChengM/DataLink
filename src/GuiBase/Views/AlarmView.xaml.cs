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
    /// AlarmView.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmView : UserControl
    {
        public AlarmView()
        {
            InitializeComponent();
        }

        private void FilterCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DrawerHost.IsTopDrawerOpen = false;

        }

        private void FilterOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            DrawerHost.IsTopDrawerOpen = true;
            
        }
        //public void CombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        //{
        //    //CombinedCalendar.SelectedDate = ((AlarmViewModel)DataContext).StartDate;
        //    //CombinedClock.Time = ((AlarmViewModel)DataContext).StartTime;
        //}

        //public void CombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        //{
        //    //if (Equals(eventArgs.Parameter, "1") &&
        //    //    CombinedCalendar.SelectedDate is DateTime selectedDate)
        //    //{
        //    //    var combined = selectedDate.AddSeconds(CombinedClock.Time.TimeOfDay.TotalSeconds);
        //    //    ((AlarmViewModel)DataContext).StartTime = combined;
        //    //    ((AlarmViewModel)DataContext).StartDate = combined;
        //    //}
        //}

    }
}
