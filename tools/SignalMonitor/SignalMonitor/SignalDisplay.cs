using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SignalMonitor
{
    public class SignalDisplay : INotifyPropertyChanged
    {
        string signalName;
        string type;
        string value;
        string index;
        string quality;
        string inputValue;
        bool isVirtual;
        public string SignalName
        {
            get
            {
                return signalName;
            }
            set
            {
                signalName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SignalName"));
            }
        }
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
            }
        }
        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));

            }
        }
        public string Index
        {
            get { return index; }
            set
            {
                index = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Index"));

            }
        }
        public string Quality
        {
            get { return quality; }
            set
            {
                quality = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Quality"));

            }
        }
        public string InputValue
        {
            get { return inputValue; }
            set
            {
                inputValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputValue"));
            }
        }

        public bool IsVirtual
        {
            get { return isVirtual; }
            set
            {
                isVirtual = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVirtual"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        private static readonly object locker = new object();
        private void raisePropertyChange(object sender,PropertyChangedEventArgs e)
        {
            lock (locker)
            {
                PropertyChanged?.Invoke(sender, e);
            }
        }
    }
   public class SignalDisplayCompare : IEqualityComparer<SignalDisplay>
    {
        public bool Equals(SignalDisplay x, SignalDisplay y)
        {
            return x.SignalName == y.SignalName;
        }

        public int GetHashCode(SignalDisplay obj)
        {
            throw new NotImplementedException();
        }
    }
}
