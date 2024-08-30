using DataServer.Points;
using FreedomDriversV2;
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
using System.Collections.Specialized;

namespace SignalMonitor
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        SignalServer server;

        private ObservableCollection<SignalDisplay> targetPoinNames = new ObservableCollection<SignalDisplay>();
        bool deviceBoxChecked =true;
        bool virtualBoxChecked=true;
        string filterData;
        string signalNumberDiscribe;


        MyCommand<RoutedEventArgs> filterClickCommand;

        MyCommand<KeyEventArgs> filterKeyCommand;

        MyCommand<RoutedEventArgs> addSignalCommand;




        public MyCommand<RoutedEventArgs> FilterClickCommand
        {
            get { return filterClickCommand; }
            set { filterClickCommand = value; }
        }

       
        public MyCommand<KeyEventArgs> FilterKeyCommand
        {
            get { return filterKeyCommand; }
            set { filterKeyCommand = value; }
        }

        public MyCommand<RoutedEventArgs> AddSignalCommand
        {
            get { return addSignalCommand; }
            set { addSignalCommand = value; }
        }
   
        public bool DeviceBoxChecked
        {
            get { return deviceBoxChecked; }
            set
            {
                if (deviceBoxChecked != value)
                {
                    deviceBoxChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeviceBoxChecked"));
                }
            }
        }
        public bool VirtualBoxChecked
        {
            get { return virtualBoxChecked; }
            set
            {
                if (virtualBoxChecked != value)
                {
                    virtualBoxChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VirtualBoxChecked"));
                }
            }
        }
        public string FilterData
        {
            get { return filterData; }
            set
            {
                if (filterData != value)
                {
                    filterData = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilterData"));
                }
            }
        }
        public string SignalNumberDiscribe
        {
            get { return signalNumberDiscribe; }
            set
            {
                if (signalNumberDiscribe != value)
                {
                    signalNumberDiscribe = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SignalNumberDiscribe"));
                }
            }
        }

        public ObservableCollection<SignalDisplay> TargetPoinNames
        {
            get { return targetPoinNames; }
            set { targetPoinNames = value; }
        }
        
        public SearchViewModel()
        {
            server = SignalServer.GetInstance();
            pointListUpdata();
            server.SourceSignalList.CollectionChanged += SourceSignalList_CollectionChanged;
            initCommand();
        }

        private void SourceSignalList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            pointListUpdata();
        }

        private void initCommand()
        {
            filterClickCommand = new MyCommand<RoutedEventArgs>(new Func<RoutedEventArgs, bool>(s => true), new Action<object, object, RoutedEventArgs>(filterClick));
            filterKeyCommand = new MyCommand<KeyEventArgs>(new Func<KeyboardEventArgs, bool>(s => true), new Action<object, object, KeyEventArgs>(filterKey));
            addSignalCommand = new MyCommand<RoutedEventArgs>(new Func<RoutedEventArgs, bool>(s => true), new Action<object, object, RoutedEventArgs>(addClick));
        }

        private void addClick(object sender, object paramenter, RoutedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                listBox = paramenter as ListBox;
            }
            if (listBox == null)
            {
                return;
            }
            var items = listBox.SelectedItems;
            List<SignalDisplay> subList = new List<SignalDisplay>();
            foreach (var s in items)
            {
                var item = (SignalDisplay)s;
                subList.Add(item);
            }
            server.Subscribe(subList);
        }

        private void filterKey(object sender, object paramenter, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pointListUpdata();
            }
        }

        private void filterClick(object sender, object paramenter, RoutedEventArgs e)
        {
            pointListUpdata();
        }

        private void pointListUpdata()
        {
            Application.Current.Dispatcher.Invoke(() => {
                if ((deviceBoxChecked == false) && (virtualBoxChecked == true))
                {
                    targetPoinNames.Clear();
                    var items = server.SourceSignalList.Select(a => a.IsVirtual ? a : null);
                    var seacrhItems = items.Select(find);
                    foreach (var item in seacrhItems)
                    {
                        if (item != null)
                        {
                            targetPoinNames.Add(item);
                        }
                    }
                }
                else if ((deviceBoxChecked == true) && (virtualBoxChecked == false))
                {
                    targetPoinNames.Clear();
                    var items = server.SourceSignalList.Select(a => a.IsVirtual ? null : a);
                    var seacrhItems = items.Select(find);
                    foreach (var item in seacrhItems)
                    {
                        if (item != null)
                        {
                            targetPoinNames.Add(item);
                        }
                    }
                }
                else if ((deviceBoxChecked == true) && (virtualBoxChecked == true))
                {
                    targetPoinNames.Clear();
                    var seacrhItems = server.SourceSignalList.Select(find);
                    foreach (var item in seacrhItems)
                    {
                        if (item != null)
                        {
                            targetPoinNames.Add(item);
                        }
                    }
                }
                else
                {
                    targetPoinNames.Clear();
                }
                SignalNumberDiscribe = string.Concat("Showing ", targetPoinNames.Count, " of ", server.SourceSignalList.Count, " Signals");
            });

        }

        private SignalDisplay find(SignalDisplay target)
        {
            string item = filterData;
            if (item != null && item != "" && target != null)
            {
                var group = item.Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);

                int position = 0;
                foreach (var s in group)
                {
                    position = target.SignalName.IndexOf(s, position);
                    if (position == -1)
                    {
                        return null;
                    }
                }
                return target;

            }
            else
            {
                return target;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
