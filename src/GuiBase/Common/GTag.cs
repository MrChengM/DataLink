using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Points;
using Utillity.Data;

namespace GuiBase.Common
{
    public class GTag : ITag
    {
        public event Action<GTag> SignalChangeEvent;

        public string Name { get ; set ; }
        public DataType Type { get; set; }
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaiseSignalChanged();
                }
            }
        }
        private QUALITIES quality;
        public QUALITIES Quality
        {
            get { return quality; }
            set
            {
                if (quality != value)
                {
                    quality = value;
                    RaiseSignalChanged();
                }
            }
        }
       
        public DateTime TimeStamp { get; set; }

        public int SignalSubCounts => DelegateFunction.SubsCounts(SignalChangeEvent);

        protected virtual void RaiseSignalChanged()
        {
            var handler = SignalChangeEvent;
            handler?.Invoke(this);
        }

    
    }
}
