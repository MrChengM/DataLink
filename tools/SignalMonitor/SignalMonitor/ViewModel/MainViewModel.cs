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
    public class MainViewModel : INotifyPropertyChanged
    {

        DataExchangeTask dataTask;
        MyCommand<KeyEventArgs> deleteCommand;
        MyCommand<KeyEventArgs> inputCommand;
        MyCommand<MouseEventArgs> searchClickCommand;
        MyCommand<MouseEventArgs> connectClickCommand;
        MyCommand<MouseEventArgs> saveClickCommand;
        MyCommand<MouseEventArgs> clearClickCommand;
        MyCommand<MouseEventArgs> loadClickCommand;
        SearchWindow searchWindow;
        MainWindow mainWindow;
        ConnectWindow connectWindow;

        public MyCommand<KeyEventArgs> DeleteCommand
        {
            get
            {
                return deleteCommand;
            }
            set
            {
                deleteCommand = value;
            }
        }
        public MyCommand<KeyEventArgs> InputCommand
        {
            get
            {
                return inputCommand;
            }
            set
            {
                inputCommand = value;
            }
        }
        public MyCommand<MouseEventArgs> SearchClickCommand
        {
            get
            {
                return searchClickCommand;
            }
            set
            {
                searchClickCommand = value;
            }
        }
        public MyCommand<MouseEventArgs> ConnectClickCommand
        {
            get
            {
                return connectClickCommand;
            }
            set
            {
                connectClickCommand = value;
            }
        }
        public MyCommand<MouseEventArgs> SaveClickCommand
        {
            get
            {
                return saveClickCommand;
            }
            set
            {
                saveClickCommand = value;
            }
        }
        public MyCommand<MouseEventArgs> ClearClickCommand
        {
            get
            {
                return clearClickCommand;
            }
            set
            {
                clearClickCommand = value;
            }
        }
        public MyCommand<MouseEventArgs> LoadClickCommand
        {
            get
            {
                return loadClickCommand;
            }
            set
            {
                loadClickCommand = value;
            }
        }

        public ObservableCollection<SignalDisplay> SignalList
        {
            get
            {
                return dataTask.MainSignalList;
            }
            set
            {
                dataTask.MainSignalList = value;
            }
        }

        public MainViewModel(MainWindow currentWindow)
        {
            dataTask = new DataExchangeTask(currentWindow);
            mainWindow = currentWindow;
            searchWindow = new SearchWindow();
            searchWindow.DataContext = new SearchViewModel(dataTask);
            connectWindow = new ConnectWindow();
            connectWindow.DataContext = new ConnectViewModel(dataTask);
            initCommand();
        }

        private void initCommand()
        {
            deleteCommand = new MyCommand<KeyEventArgs>(new Func<KeyEventArgs, bool>(s => true), new Action<object,KeyEventArgs>(deleteItems));
            inputCommand = new MyCommand<KeyEventArgs>(new Func<object, bool>(s =>true), new Action<object,KeyEventArgs>(inputSignalValue));
            searchClickCommand = new MyCommand<MouseEventArgs>(new Func<MouseEventArgs, bool>(s => true), new Action<object, MouseEventArgs>(searchClick));
            connectClickCommand = new MyCommand<MouseEventArgs>(new Func<MouseEventArgs, bool>(s => true), new Action<object, MouseEventArgs>(connectClick));
        }

      
        private void deleteItems(object sender,KeyEventArgs e)
        {
            List<SignalDisplay> deleteList = new List<SignalDisplay>();
            List<Address> cancelList = new List<Address>();
            var element = sender as ListView;
            if (e.Key == Key.Delete)
            {
                var items = element.SelectedItems;
                foreach (var item in items)
                {
                    SignalDisplay s = item as SignalDisplay;
                    deleteList.Add(s);
                }
                dataTask.CancelSubscribe(deleteList);
            }
        }
        private void inputSignalValue(object sender,KeyEventArgs e)
        {
            TextBox inputTeBox = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                var parent = VisualTreeHelper.GetParent(inputTeBox) as ContentPresenter;
                SignalDisplay item = parent.Content as SignalDisplay;
                dataTask.WriteAsync(item, inputTeBox.Text);
            }
        }
        private void searchClick(object sender,MouseEventArgs e)
        {

        }
        private void connectClick(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
