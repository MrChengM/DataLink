using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Common;

namespace GuiBase.Helper
{
    public interface IToolTiProvider
    {
        object GetToolTip(GSignalSet signals);
    }
}
