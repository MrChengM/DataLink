using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Prism.Mvvm;


namespace GuiBase.Models
{
    public class NavigationItem : BindableBase
    {
        public string Title { get; set; }
        public PackIconKind SelectedIcon { get; set; }
        public PackIconKind UnselectedIcon { get; set; }
        public string ViewName { get; set; }
        public ViewType ViewType { get; set; }

        private int? _notification = null;
        public int? Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, value); }
        }

        private int alarm25Counts;

        public int Alarm25Counts
        {
            get { return alarm25Counts; }
            set { SetProperty(ref alarm25Counts, value, "Alarm25Counts"); }
        }

        private int alarm50Counts;

        public int Alarm50Counts
        {
            get { return alarm50Counts; }
            set { SetProperty(ref alarm50Counts, value, "Alarm50Counts"); }
        }
        private int alarm75Counts;

        public int Alarm75Counts
        {
            get { return alarm75Counts; }
            set { SetProperty(ref alarm75Counts, value, "Alarm75Counts"); }
        }
        private int alarm100Counts;

        public int Alarm100Counts
        {
            get { return alarm100Counts; }
            set { SetProperty(ref alarm100Counts, value, "Alarm100Counts"); }
        }
    }
}
