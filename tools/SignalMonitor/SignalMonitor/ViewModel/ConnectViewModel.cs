using DataServer.Points;
using FreedomDrivers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SignalMonitor
{
    public class ConnectViewModel:INotifyPropertyChanged
    {
        SignalServer dataTask;
        ConnectWindow connectWindow;

        public ConnectViewModel(ConnectWindow window)
        {
            dataTask = SignalServer.GetInstance();
            connectWindow = window;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
