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

    public class DialogClosedResult
    {
        public string ViewName { get; set; }
        public bool IsClosed { get; set; }

    }


}
