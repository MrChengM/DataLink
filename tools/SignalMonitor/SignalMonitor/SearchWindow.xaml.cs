using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DataServer.Points;
using FreedomDrivers;
using System.ComponentModel;

namespace SignalMonitor
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {

        private ObservableCollection<SignalDisplay> sourcePointNames=new ObservableCollection<SignalDisplay>();
        private ObservableCollection<SignalDisplay> targetPoinNames =new ObservableCollection<SignalDisplay>();
        private ObservableCollection<SignalDisplay> signalDisplayList;
        //private ObservableCollection<>
        private FreedomClientAsync clientAsync;
        //private MainWindow mainWidow;
        public SearchWindow()
        {
            InitializeComponent();
        }
        #region 属性
        public ObservableCollection<SignalDisplay> SourcePointNames
        {
            get { return sourcePointNames; }
            set
            {
                sourcePointNames = value;
                pointListUpdata();
            }
        }
        
        public ObservableCollection<SignalDisplay> TargetPoinNames
        {
            get { return targetPoinNames; }
            set
            {
                targetPoinNames = value;
            }
        }
        public FreedomClientAsync ClientAsync
        {
            set { clientAsync = value; }
        }
        public ObservableCollection<SignalDisplay> SignalDisplayList
        {
            set { signalDisplayList = value; }
        }
        #endregion

        private void deviceChBox_Click(object sender, RoutedEventArgs e)
        {
            pointListUpdata();
        }
        private void virtualChBox_Click(object sender, RoutedEventArgs e)
        {
            pointListUpdata();
        }
        private void pointListUpdata()
        {
            Dispatcher.Invoke(() => {
                if ((deviceChBox.IsChecked == false) && (virtualChBox.IsChecked == true))
                {
                    targetPoinNames.Clear();
                    var items = sourcePointNames.Select(a => a.IsVirtual ? a : null);
                    var seacrhItems = items.Select(find);
                    foreach (var item in seacrhItems)
                    {
                        if (item != null)
                        {
                            targetPoinNames.Add(item);
                        }
                    }
                }
                else if ((deviceChBox.IsChecked == true) && (virtualChBox.IsChecked == false))
                {
                    targetPoinNames.Clear();
                    var items = sourcePointNames.Select(a => a.IsVirtual ? null : a);
                    var seacrhItems = items.Select(find);
                    foreach (var item in seacrhItems)
                    {
                        if (item != null)
                        {
                            targetPoinNames.Add(item);
                        }
                    }
                }
                else if ((deviceChBox.IsChecked == true) && (virtualChBox.IsChecked == true))
                {
                    targetPoinNames.Clear();
                    var seacrhItems = sourcePointNames.Select(find);
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
                pointNameLiBox.ItemsSource = targetPoinNames;
                pointNameLiBox.Items.Refresh();

                signalNumberTexBlock.Text = string.Concat("Showing ", targetPoinNames.Count, " of ", sourcePointNames.Count, " Signals");
            });
            
        }

        private SignalDisplay find(SignalDisplay target)
        {
            string item = signalSearchTeBox.Text;
            if (item != null && item != ""&&target!=null)
            {
                var group = item.Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);

                int position=0;
                foreach(var s in group)
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
        private void addPointClick(object sender, RoutedEventArgs e)
        {
            var items = pointNameLiBox.SelectedItems;
            List<Address> subList = new List<Address>();
            foreach (var s in items)
            {
                var item = (SignalDisplay)s;
                if (!signalDisplayList.Contains(item))
                {
                    int index = 0;
                    var nameGroup = item.SignalName.Split('[');
                    if (nameGroup.Length > 1)
                    {
                        nameGroup[1] = nameGroup[1].Replace("]", "");
                        int.TryParse(nameGroup[1], out index);
                    }
                    signalDisplayList.Add(item);
                    Address addAddress = new Address()
                    {
                        Name = nameGroup[0],
                        Index = index.ToString(),
                        Type = item.Type
                    };
                    subList.Add(addAddress);
                }
            }
            while (subList.Count > 1000)
            {
                var send = subList.GetRange(0, 100);
                subList.RemoveRange(0, 100);
                clientAsync.Subscribe(send);
            }
            clientAsync.Subscribe(subList);
        }
        private void signalSearchTeBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pointListUpdata();
            }
        }
    }
}
