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
    public class SearchViewModel : INotifyPropertyChanged
    {
        DataExchangeTask dataTask;
        public SearchViewModel(DataExchangeTask task)
        {
            dataTask = task;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
