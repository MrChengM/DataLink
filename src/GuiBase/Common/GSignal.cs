using DataServer;
using DataServer.Points;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuiBase.Common
{
    public class GSignal : DependencyObject, INotifyPropertyChanged
    {

        public bool Initialized { get; set; }
        public DataType Type { get; set; }
        public QUALITIES Quality { get; set; }
        public bool Connected { get; set; }
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Initialized = true;
            }
        }
        public SignalIdEnum Id
        {
            get { return (SignalIdEnum)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(SignalIdEnum), typeof(GSignal), new PropertyMetadata(SignalIdEnum.Signal1));


        public string SignalName
        {
            get { return (string)GetValue(SignalNameProperty); }
            set { SetValue(SignalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SignalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SignalNameProperty =
            DependencyProperty.Register("SignalName", typeof(string), typeof(GSignal), new PropertyMetadata(string.Empty));
        public string KeepAliveSignalName
        {
            get { return (string)GetValue(KeepAliveSignalNameProperty); }
            set { SetValue(KeepAliveSignalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeepAliveSignalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeepAliveSignalNameProperty =
            DependencyProperty.Register("KeepAliveSignalName", typeof(string), typeof(GSignal), new PropertyMetadata(string.Empty));


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void OnSignalNameChange()
        {
            RaisePropertyChanged("SignalName");
        }


        public GSignal Clone()
        {
            return new GSignal()
            {
                Id = this.Id,
                SignalName = this.SignalName,
                KeepAliveSignalName = this.KeepAliveSignalName,
                Type = this.Type,
                Quality = this.Quality,
                Value = this.Value,
                Connected = this.Connected
            };

        }
    }
    public enum SignalIdEnum
    {
        Signal1, 
        Signal2,
        Signal3,
        Signal4,
        Signal5,
        Signal6,
        Signal7,
        Signal8,
        Signal9,
        Signal10,
        Signal11,
        Signal12,
        Signal13,
        Signal14,
        Signal15,
        Signal16,
        Signal17,
        Signal18,
        Signal19,
        Signal20,
    }
}
