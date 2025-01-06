using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Models
{
    public class ViewInfor
    {
        public string ViewName { get; set; }
        public ViewType ViewType { get; set; }
    }
    public enum ViewType
    {
        Base_OverView = 1,
        Base_L1View = 2,
        Base_L2View = 4,
        Base_Command = 8,
        Base_Other = 16,
        Sub_ACC = 32,
    }
}
