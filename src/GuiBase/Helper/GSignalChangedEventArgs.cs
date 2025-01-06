using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Common;

namespace GuiBase.Helper
{
    public class GSignalChangedEventArgs: EventArgs
    {
        private GSignalSet _signals;
        public GSignalChangedEventArgs(GSignalSet signals)
        {
            _signals = signals;
        }
        public GSignal Signal1 => _signals[0];
        public GSignal Signal2 => _signals[1];
        public GSignal Signal3 => _signals[2];
        public GSignal Signal4 => _signals[3];
        public GSignal Signal5 => _signals[4];
        public GSignal Signal6 => _signals[5];
        public GSignal Signal7 => _signals[6];
        public GSignal Signal8 => _signals[7];
        public GSignal Signal9 => _signals[8];
        public GSignal Signal10 => _signals[9];
        public GSignal Signal11 => _signals[10];
        public GSignal Signal12 => _signals[11];
        public GSignal Signal13 => _signals[12];
        public GSignal Signal14 => _signals[13];
        public GSignal Signal15 => _signals[14];
        public GSignal Signal16 => _signals[15];
        public GSignal Signal17 => _signals[16];
        public GSignal Signal18 => _signals[17];
        public GSignal Signal19 => _signals[18];
        public GSignal Signal20 => _signals[19];
        public string Signal1Value => GetSignalValue(Signal1);
        public string Signal2Value => GetSignalValue(Signal2);
        public string Signal3Value => GetSignalValue(Signal3);
        public string Signal4Value => GetSignalValue(Signal4);
        public string Signal5Value => GetSignalValue(Signal5);
        public string Signal6Value => GetSignalValue(Signal6);
        public string Signal7Value => GetSignalValue(Signal7);
        public string Signal8Value => GetSignalValue(Signal8);
        public string Signal9Value => GetSignalValue(Signal9);
        public string Signal10Value => GetSignalValue(Signal10);
        public string Signal11Value => GetSignalValue(Signal11);
        public string Signal12Value => GetSignalValue(Signal12);
        public string Signal13Value => GetSignalValue(Signal13);
        public string Signal14Value => GetSignalValue(Signal14);
        public string Signal15Value => GetSignalValue(Signal15);
        public string Signal16Value => GetSignalValue(Signal16);
        public string Signal17Value => GetSignalValue(Signal17);
        public string Signal18Value => GetSignalValue(Signal18);
        public string Signal19Value => GetSignalValue(Signal19);
        public bool Signal1Connected => GetSignalConnection(Signal1);
        public bool Signal2Connected => GetSignalConnection(Signal2);
        public bool Signal3Connected => GetSignalConnection(Signal3);
        public bool Signal4Connected => GetSignalConnection(Signal4);
        public bool Signal5Connected => GetSignalConnection(Signal5);
        public bool Signal6Connected => GetSignalConnection(Signal6);
        public bool Signal7Connected => GetSignalConnection(Signal7);
        public bool Signal8Connected => GetSignalConnection(Signal8);
        public bool Signal9Connected => GetSignalConnection(Signal9);
        public bool Signal10Connected => GetSignalConnection(Signal10);
        public bool Signal11Connected => GetSignalConnection(Signal11);
        public bool Signal12Connected => GetSignalConnection(Signal12);
        public bool Signal13Connected => GetSignalConnection(Signal13);
        public bool Signal14Connected => GetSignalConnection(Signal14);
        public bool Signal15Connected => GetSignalConnection(Signal15);
        public bool Signal16Connected => GetSignalConnection(Signal16);
        public bool Signal17Connected => GetSignalConnection(Signal17);
        public bool Signal18Connected => GetSignalConnection(Signal18);
        public bool Signal19Connected => GetSignalConnection(Signal19);
        public GSignalSet Signals => _signals;
        private string GetSignalValue(GSignal signal)
        {
            return (signal != null) ? signal.Value : null;
        }

        private bool GetSignalConnection(GSignal signal)
        {
            return signal != null && signal.Connected;
        }
        public bool IsConnected
        {
            get
            {
                if (_signals!=null)
                {
                    return _signals.All(s => s.Connected);
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
