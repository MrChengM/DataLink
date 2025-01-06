using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace GuiBase.Common
{
    public class GSignalSet : ObservableCollection<GSignal>
    {
        public new GSignal this[int key]
        {
            get
            {
                if (this != null)
                {
                    return this.FirstOrDefault(s => s.Id == (SignalIdEnum)key);

                }
                else
                {
                    return null;
                }
            }
        }
        public GSignalSet Clone()
        {
            GSignalSet signals = new GSignalSet();
            foreach (GSignal signal in this)
            {
                signals.Add(signal.Clone());
            }
            return signals;
        }
    }
}
