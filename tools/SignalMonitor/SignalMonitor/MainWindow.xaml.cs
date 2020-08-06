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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FreedomDrivers;
using DataServer;
using DataServer.Points;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace SignalMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<SignalDisplay> signalList = new ObservableCollection<SignalDisplay>();
        List<PointMetadata> pointMetaData;
        //ObservableCollection<SignalDisplay> commSignalList;

        SearchWindow searchWin;
        ConnectWindow connectWindow ;

        System.Timers.Timer reTime;
        FreedomClientAsync clientAsync;
        public MainWindow()
        {
            InitializeComponent();
            listView.ItemsSource = signalList;
            Closed += mainWindowClosed;

            init();
        }

        private void init()
        {

            //数据配置
            pointMetaData = new List<PointMetadata>();

            //Meu窗口配置
            searchWin = new SearchWindow() {SignalDisplayList= signalList, ClientAsync = clientAsync};
            connectWindow = new ConnectWindow();
            SignalMeItem.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(signalMeItemMouseLeftButtonDown), true);
            ConnectMeItem.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(ConnectMeItemMouseLeftButtonDown), true);


            ///连接到服务端配置
            EthernetSetUp setUp = new EthernetSetUp("127.0.0.1", 9527);
            ILog log = new DefaultLog("SignalMonitor");
            TimeOut timeOut = new TimeOut("SignalMonitor", 1000, log);
            clientAsync = new FreedomClientAsync(setUp, timeOut, log);
           

            //读服务器元数据
            clientAsync.AsyncReadMetaData += asyncReadMetaData;

            //订阅client操作事件
            clientAsync.AsyncReadOrSubsEvent += asyncReadOrSubsEvent;
            clientAsync.AsyncWriteEvent += ayncWriteEvent;
            clientAsync.DisconnectEvent += disconnectEvent;

            //断线重连事件
            reTime = new System.Timers.Timer(3000);
            reTime.Elapsed += ReTime_Elapsed;

            if (clientAsync.Connect())
            {
                clientAsync.ReadMetaData();
            }
        }

        //定时触发重连
        private void ReTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = sender as System.Timers.Timer;
            timer.Enabled = false;
            if (clientAsync.Connect())
            {
                clientAsync.ReadMetaData();
            }
            else
            {
                timer.Enabled = true;
            }
        }

        private void ayncWriteEvent(bool obj)
        {
            Dispatcher.Invoke(() => {
                if (obj)
                {
                    MessageBox.Show("Sucessful", "Write Status", MessageBoxButton.OK);

                }
                else
                {
                    MessageBox.Show("Fail", "Write Status", MessageBoxButton.OK);
                }
            });
            
        }
        private void asyncReadOrSubsEvent(List<AsyncResult> obj)
        {
            foreach (var result in obj)
            {
                string name = string.Concat(result.Name, "[", result.Index,"]");
                IEnumerable<SignalDisplay> signalDisplays;
                if (signalList.Contains(new SignalDisplay { SignalName = name }, new SignalDisplayCompare()))
                {
                    signalDisplays = signalList.Select((a) => a.SignalName == name ? a : null);

                }
                else
                {
                    signalDisplays = signalList.Select((a) => a.SignalName == result.Name ? a : null);
                }
                foreach (var signalDisplay in signalDisplays)
                {
                    if (signalDisplay != null)
                    {
                        signalDisplay.Quality = result.Quality;
                        signalDisplay.Value = result.Value;
                    }
                }
            }
        }

        private static readonly object locker = new object();
        private void asyncReadMetaData(List<PointMetadata> obj)
        {
            //lock (locker)
            //{
                pointMetaData.AddRange(obj);
                updateSearchList();
            //}
        }

        private void disconnectEvent(FreedomClientAsync obj)
        {
            pointMetaData.Clear();
            Dispatcher.Invoke(() =>
            {
                signalList.Clear();
                searchWin.SourcePointNames = new ObservableCollection<SignalDisplay>();
            }); 
            if (!reTime.Enabled)
            {
                reTime.Enabled = true;
            }
        }


        private void mainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown(); //关闭当前程序
            //Environment.Exit(0);立即中断程序运行
        }

        private void signalMeItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!searchWin.IsVisible)
            {
                searchWin = new SearchWindow() { SignalDisplayList = signalList ,ClientAsync=clientAsync};
                searchWin.Show();
                updateSearchList();
            }
        }

        private void ConnectMeItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!connectWindow.IsVisible)
            {
                connectWindow = new ConnectWindow();
                connectWindow.ReConnectEvent += reConnectEvent;
                connectWindow.Show();
            }
        }

        private void reConnectEvent(string arg1, short arg2)
        {
            clientAsync.EthernetSetUp = new EthernetSetUp(arg1, arg2);
            clientAsync.DisConnect();
        }

        private void updateSearchList()
        {
            if (searchWin != null && searchWin.IsVisible)
            {
                searchWin.SourcePointNames = getSourcePointNames();
            }
        }

        //获取搜索界面ListBox 点名数据
        private ObservableCollection<SignalDisplay> getSourcePointNames()
        {
            //pointMetaData.Sort((a,b)=> a.Name.CompareTo(b.Name));
            ObservableCollection<SignalDisplay> pointNames = new ObservableCollection<SignalDisplay>();
            foreach (var metaData in pointMetaData)
            {
                if (metaData.Length == 1)
                {
                    SignalDisplay s = new SignalDisplay { SignalName = metaData.Name, Type = metaData.ValueType, IsVirtual = metaData.IsVirtual };
                    pointNames.Add(s);
                }
                else
                {
                    for (int i = 0; i < metaData.Length; i++)
                    {
                        string name = string.Concat(metaData.Name, "[", i, "]");
                        SignalDisplay s = new SignalDisplay { SignalName = name, Type = metaData.ValueType, IsVirtual = metaData.IsVirtual };
                        pointNames.Add(s);
                    }
                }
            }
            return pointNames;
        }
        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                List<SignalDisplay> deleteList = new List<SignalDisplay>();
                List<Address> cancelList = new List<Address>();
                var items = listView.SelectedItems;
                foreach(var item in items)
                {
                    SignalDisplay s = item as SignalDisplay;
                    deleteList.Add(s);
                }
                foreach(var s in deleteList)
                {
                    int index = 0;
                    var nameGroup = s.SignalName.Split('[');
                    if (nameGroup.Length > 1)
                    {
                        nameGroup[1] = nameGroup[1].Replace("]", "");
                        int.TryParse(nameGroup[1], out index);
                    }
                    signalList.Remove(s);
                    cancelList.Add(new Address() { Name = nameGroup[0], Index = index.ToString(), Type = s.Type });
                }
                while (cancelList.Count > 1000)
                {
                    var send = cancelList.GetRange(0, 100);
                    cancelList.RemoveRange(0, 100);
                    clientAsync.CancelSubscribe(send);
                }
                clientAsync.CancelSubscribe(cancelList);
            }
        }

        private void wirteTeBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox inputTeBox = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                var parent = VisualTreeHelper.GetParent(inputTeBox) as ContentPresenter;
                SignalDisplay item = parent.Content as SignalDisplay;
                int index = 0;
                var nameGroup = item.SignalName.Split('[');
                if (nameGroup.Length > 1)
                {
                    nameGroup[1] = nameGroup[1].Replace("]", "");
                    int.TryParse(nameGroup[1], out index);
                }
                WriteData data = new WriteData { Name = nameGroup[0], Index = index.ToString(), Type = item.Type, Value = inputTeBox.Text };
                clientAsync.WriteAsync(new List<WriteData> { data });
            }
        }
    }
   

}
