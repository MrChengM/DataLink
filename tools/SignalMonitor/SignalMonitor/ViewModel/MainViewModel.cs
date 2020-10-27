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
        #region field
        SignalServer server;
        MyCommand<RoutedEventArgs> closeWindowCommand;

        MyCommand<KeyEventArgs> deleteCommand;
        MyCommand<KeyEventArgs> inputCommand;
        MyCommand<MouseEventArgs> searchClickCommand;
        MyCommand<MouseEventArgs> connectClickCommand;
        MyCommand<MouseEventArgs> saveClickCommand;
        MyCommand<MouseEventArgs> clearClickCommand;
        MyCommand<MouseEventArgs> loadClickCommand;
        SearchWindow searchWindow;
        //MainWindow mainWindow;
        ConnectWindow connectWindow;

        ObservableCollection<SignalDisplay> mainSignalDisplay;

        #endregion

        #region property
        public ObservableCollection<SignalDisplay> MainSignalDisplay
        {
            get
            {
                return mainSignalDisplay;
            }
            set
            {
                mainSignalDisplay = value;
            }
        }
        public MyCommand<RoutedEventArgs> CloseWindowCommand
        {
            get
            {
                return closeWindowCommand;
            }
            set
            {
                closeWindowCommand = value;
            }
        }

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
                return server.MainSignalList;
            }
            set
            {
                server.MainSignalList = value;
            }
        }
        #endregion

        #region method
        public MainViewModel()
        {
            server = SignalServer.GetInstance();
            mainSignalDisplay = server.MainSignalList;
            server.WriteFreedBack += writeFreedBack;
            
            initCommand();
        }

        private void writeFreedBack(bool obj)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    if (obj)
                        MessageBox.Show("Sucessfull!", "Write Value feedBack");
                    else
                        MessageBox.Show("Fail!", "Write Value feedBack");
                });
        }

        private void initCommand()
        {
            closeWindowCommand = new MyCommand<RoutedEventArgs>(new Func<RoutedEventArgs, bool>(s => true), new Action<object, object, RoutedEventArgs>(closeWindow));
            deleteCommand = new MyCommand<KeyEventArgs>(new Func<KeyEventArgs, bool>(s => true), new Action<object,object,KeyEventArgs>(deleteItems));
            inputCommand = new MyCommand<KeyEventArgs>(new Func<object, bool>(s =>true), new Action<object, object,KeyEventArgs>(inputSignalValue));
            searchClickCommand = new MyCommand<MouseEventArgs>(new Func<MouseEventArgs, bool>(s => true), new Action<object, object, MouseEventArgs>(searchClick));
            connectClickCommand = new MyCommand<MouseEventArgs>(new Func<MouseEventArgs, bool>(s => true), new Action<object, object, MouseEventArgs>(connectClick));
        }

        private void closeWindow(object sender,object parameter, RoutedEventArgs e)
        {
            server.Dispose();
            Application.Current.Shutdown(); //关闭当前程序
            Environment.Exit(0);//立即中断程序运行
        }

        private void deleteItems(object sender, object parameter,KeyEventArgs e)
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
                server.CancelSubscribe(deleteList);
            }
        }
        private void inputSignalValue(object sender, object parameter,KeyEventArgs e)
        {
            TextBox inputTeBox = e.Source as TextBox;
            if (e.Key == Key.Enter)
            {
                var parent = VisualTreeHelper.GetParent(inputTeBox) as ContentPresenter;
                SignalDisplay item = parent.Content as SignalDisplay;
                server.WriteAsync(item, inputTeBox.Text);
            }
        }
        private void searchClick(object sender, object parameter,MouseEventArgs e)
        {
            
            if (searchWindow==null || !searchWindow.IsVisible)
            {
                searchWindow = new SearchWindow();
                searchWindow.Show();
            }
        }
        private void connectClick(object sender, object parameter, MouseEventArgs e)
        {
            if (connectWindow == null || !connectWindow.IsVisible)
            {
                connectWindow = new ConnectWindow();
                connectWindow.Show();
            }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
