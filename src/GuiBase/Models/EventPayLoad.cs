using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Models
{
    public enum AccoutLogOnResult
    {
        Success = 1,
        Fail = 2,
        Exit = 4
    }
    public enum LoadResult
    {
        Success = 1,
        Fail = 2,
    }

    public class ViewInfor
    {
        public string ViewName { get; set; }
        public ViewType ViewType { get; set; }
    }
    public enum ViewType
    {
        OverView = 1,
        L1View = 2,
        L2View = 4,
        Command = 8,
        Other = 16
    }
}
