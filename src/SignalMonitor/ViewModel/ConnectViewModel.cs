using DataServer.Points;
using FreedomDriversV2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
        #region field
        SignalServer server;
        string inputAddress;
        string inputPort;
        string currentAddress;
        string currentPort;
        MyCommand<RoutedEventArgs> acceptClickCommand;
        MyCommand<RoutedEventArgs> cancelClickCommand;
        #endregion

        #region Property
        public MyCommand<RoutedEventArgs> AcceptClickCommand
        {
            get
            {
                return acceptClickCommand;
            }
            set
            {
                acceptClickCommand = value;
            }
        }
        public MyCommand<RoutedEventArgs> CancelClickCommand
        {
            get
            {
                return cancelClickCommand;
            }
            set
            {
                cancelClickCommand = value;
            }
        }
        public string InputAddress
        {
            get
            {
                return inputAddress;
            }
            set
            {
                if( inputAddress!=value)
                {
                    inputAddress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputAddress"));
                }

            }
        }
        public string InputPort
        {
            get
            {
                return inputPort;
            }
            set
            {
                if (inputPort != value)
                {
                    inputPort = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputPort"));
                }

            }
        }
        public string CurrentAddress
        {
            get
            {
                return currentAddress;
            }
            set
            {
                if (currentAddress != value)
                {
                    currentAddress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentAddress"));
                }

            }
        }
        public string CurrentPort
        {
            get
            {
                return currentPort;
            }
            set
            {
                if (currentPort != value)
                {
                    currentPort = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPort"));
                }

            }
        }
        #endregion
        //ConnectWindow connectWindow;

        public ConnectViewModel()
        {
            server = SignalServer.GetInstance();
            currentAddress = server.SetUp.IPAddress;
            currentPort = server.SetUp.PortNumber.ToString();
            inputAddress = currentAddress;
            inputPort = currentPort;
            //connectWindow = window;
            initCommand();
        }

        private void initCommand()
        {
            acceptClickCommand= new MyCommand<RoutedEventArgs>(new Func<RoutedEventArgs, bool>(s => true), new Action<object, object, RoutedEventArgs>(acceptClick));
            cancelClickCommand = new MyCommand<RoutedEventArgs>(new Func<RoutedEventArgs, bool>(s => true), new Action<object, object, RoutedEventArgs>(cancelClick));
        }

        private void cancelClick(object sender, object paramenter, RoutedEventArgs e)
        {
            Button acceptButton = sender as Button;
            Window currentWindow = Window.GetWindow(acceptButton);
            currentWindow.Close();
        }

        private void acceptClick(object sender, object paramenter, RoutedEventArgs e)
        {
            IPAddress temp;
            short temp1;
            if (IPAddress.TryParse(inputAddress, out temp) && short.TryParse(InputPort, out temp1))
            {
                if (inputAddress!=currentAddress || inputPort!=currentPort)
                {
                    CurrentAddress = inputAddress;
                    CurrentPort = inputPort;
                    server.SetUp = new DataServer.EthernetSetUp(inputAddress, temp1);
                    server.Reconnect();
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Ip address or port input error!");
                    return;
                });
              
            }
            Button acceptButton = sender as Button;
            Window currentWindow = Window.GetWindow(acceptButton);
            currentWindow.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
