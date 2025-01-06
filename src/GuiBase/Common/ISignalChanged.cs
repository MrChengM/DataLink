using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Common
{
    public interface ISignalChanged
    {
         void OnKeepAliveSignalChanged(List<string> signalNames, bool value);

        void OnSignalChanged(GTag tag);
        
    }
}
