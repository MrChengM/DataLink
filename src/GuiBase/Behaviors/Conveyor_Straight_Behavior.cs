using GuiBase.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Behaviors
{
    public class Conveyor_Straight_Behavior : BaseElementDefaultBahavior
    {
        public override string GetToolTip(GSignalChangedEventArgs e)
        {
            return "Test1";
        }

        public override void OnSignalChanged(GSignalChangedEventArgs e)
        {
            
        }
    }
}
