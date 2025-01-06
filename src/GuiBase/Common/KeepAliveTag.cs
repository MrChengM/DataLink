using DataServer;
using DataServer.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Data;
using System.Windows.Forms;

namespace GuiBase.Common
{
    public class KeepAliveTag : ITag
    {
        private Timer spaceTimer;
        public KeepAliveTag()
        {
            SignalBindings = new List<string>();
            spaceTimer = new Timer();
            spaceTimer.Interval = 10000;
            spaceTimer.Tick += OnTick;
            spaceTimer.Enabled = true;
            spaceTimer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            Connected = false;
        }



        public string Name { get ; set; }
        public DataType Type { get ; set; }
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    spaceTimer.Stop();
                    Connected = true;
                    spaceTimer.Start();
                };
            }
        }
        public QUALITIES Quality { get; set ; }
        public DateTime TimeStamp { get ; set ; }

        private bool connected;

        public bool Connected
        {
            get { return connected; }
            set {
                if (connected != value)
                {
                    connected = value;
                    KeepAliveSignalChange?.Invoke(SignalBindings, connected);
                }
            }
        }
        //public int SignalSubCounts => DelegateFunction.SubsCounts(KeepAliveSignalChange);

        public List<string> SignalBindings;

        public event Action<List<string>, bool> KeepAliveSignalChange;
    }
}
