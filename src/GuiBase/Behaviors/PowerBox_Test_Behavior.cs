using GuiBase.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Data;

namespace GuiBase.Behaviors
{
    public class PowerBox_Test_Behavior : BaseElementDefaultBahavior
    {
        public override string GetToolTip(GSignalChangedEventArgs e)
        {
            StringBuilder toolTip = new StringBuilder();
            if (e.Signal1 != null && int.TryParse(e.Signal1.Value, out int signal1))
            {
                if (NetConvert.GetBit(signal1, 0))
                {
                    toolTip.Append(GetStatus("AL4001"));
                    toolTip.Append(";");

                }
                if (NetConvert.GetBit(signal1, 1))
                {
                    toolTip.Append(GetStatus("AL4002"));
                    toolTip.Append(";");
                }
                if (NetConvert.GetBit(signal1, 2))
                {
                    toolTip.Append(GetStatus("AL4003"));
                    toolTip.Append(";");
                }
                if (NetConvert.GetBit(signal1, 3))
                {
                    toolTip.Append(GetStatus("AL4004"));
                    toolTip.Append(";");
                }
                if (toolTip.Length == 0)
                {
                    toolTip.Append(GetStatus("Unknown"));
                    toolTip.Append(";");
                }
            }
            return toolTip.ToString();

        }

        public override void OnSignalChanged(GSignalChangedEventArgs e)
        {

            if (e.Signal1 != null && int.TryParse(e.Signal1.Value, out int signal1))
            {
                string newStatus;
                if (NetConvert.GetBit(signal1, 0))
                {
                    newStatus = "AL4001";
                }
                else if (NetConvert.GetBit(signal1, 1))
                {
                    newStatus = "AL4002";

                }
                else if (NetConvert.GetBit(signal1, 2))
                {
                    newStatus = "AL4003";
                }
                else if (NetConvert.GetBit(signal1, 3))
                {
                    newStatus = "AL4004";
                }
                else
                {
                    newStatus = "G_Text_Unknown";
                }
                if (Element.HasSignalElement)
                {
                    Element.SignalBrush = GetBrush(newStatus);
                }
                if (Element.HasTextElement)
                {
                    Element.Text = GetStatus(newStatus);
                }
            }
        }
    }
}
