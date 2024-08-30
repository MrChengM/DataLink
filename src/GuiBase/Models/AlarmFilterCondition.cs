using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace GuiBase.Models
{
    public class AlarmFilterCondition : BindableBase
    {
        private string alarmLevel = "All";

        public string AlarmLevel
        {
            get { return alarmLevel; }
            set { SetProperty(ref alarmLevel, value, "AlarmLevel"); }
        }

        private string alarmGroup;

        public string AlarmGroup
        {
            get { return alarmGroup; }
            set { SetProperty(ref alarmGroup, value, "AlarmGroup"); }
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
        private string isConfrim = "All";

        public string IsConfrim
        {
            get { return isConfrim; }
            set { SetProperty(ref isConfrim, value, "IsConfrim"); }
        }

    }
}
