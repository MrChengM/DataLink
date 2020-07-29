using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;

namespace SignalMonitor
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        //private string _ipAddrass;
        //private int _port;

        //public string IpAddress
        //{
        //    get { return _ipAddrass; }
        //    set { _ipAddrass = value; }
        //}
        //public int Port
        //{
        //    get { return _port; }
        //    set { _port = value; }
        //}
        public ConnectWindow()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            IPAddress temp;
            short temp1;
            if (IPAddress.TryParse(newAddressTeBox.Text,out temp)&& short.TryParse(newPortTeBox.Text, out temp1))
            {
                if(curAddressTeBox.Text.Equals(newAddressTeBox.Text)|| curPortTeBox.Text.Equals(newPortTeBox.Text))
                {
                    curAddressTeBox.Text = newAddressTeBox.Text;
                    curPortTeBox.Text = newPortTeBox.Text;
                    ReConnectEvent?.Invoke(newAddressTeBox.Text, temp1);
                }
            }
            else
            {
                MessageBox.Show("Ip address or port input error!");
            }
            Close();
          
        }
        public event Action<string, short> ReConnectEvent;
    }
}
