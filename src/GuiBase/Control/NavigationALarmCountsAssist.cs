using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuiBase.Control
{
    public static class NavigationALarmCountsAssist
    {
        public static int GetAlarm25Counts(DependencyObject element)
            => (int)element.GetValue(Alarm25CountsProperty);
        public static void SetAlarm25Counts(DependencyObject element, int value)
            => element.SetValue(Alarm25CountsProperty, value);

        // Using a DependencyProperty as the backing store for Alarm25Counts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Alarm25CountsProperty =
            DependencyProperty.RegisterAttached("Alarm25Counts", typeof(int), typeof(NavigationALarmCountsAssist), new PropertyMetadata(0));

        public static int GetAlarm50Counts(DependencyObject element)
           => (int)element.GetValue(Alarm50CountsProperty);
        public static void SetAlarm50Counts(DependencyObject element, int value)
            => element.SetValue(Alarm50CountsProperty, value);

        // Using a DependencyProperty as the backing store for Alarm25Counts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Alarm50CountsProperty =
            DependencyProperty.RegisterAttached("Alarm50Counts", typeof(int), typeof(NavigationALarmCountsAssist), new PropertyMetadata(0));
        public static int GetAlarm75Counts(DependencyObject element)
         => (int)element.GetValue(Alarm75CountsProperty);
        public static void SetAlarm75Counts(DependencyObject element, int value)
            => element.SetValue(Alarm75CountsProperty, value);

        // Using a DependencyProperty as the backing store for Alarm25Counts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Alarm75CountsProperty =
            DependencyProperty.RegisterAttached("Alarm75Counts", typeof(int), typeof(NavigationALarmCountsAssist), new PropertyMetadata(0));
        public static int GetAlarm100Counts(DependencyObject element)
       => (int)element.GetValue(Alarm100CountsProperty);
        public static void SetAlarm100Counts(DependencyObject element, int value)
            => element.SetValue(Alarm100CountsProperty, value);

        // Using a DependencyProperty as the backing store for Alarm25Counts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Alarm100CountsProperty =
            DependencyProperty.RegisterAttached("Alarm100Counts", typeof(int), typeof(NavigationALarmCountsAssist), new PropertyMetadata(0));
    }
}
