using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreedomDriversV2;
using DataServer;
using DataServer.Log;
using System.Timers;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using DataServer.Points;

namespace SignalMonitor
{
    public class SignalServer:IDisposable
    {
        #region 字段
        FreedomClientAsync _client;
        EthernetSetUp _setUp;
        ILog _log;
        TimeOut _timeOut;
        Timer _reConnectTimer;
        //MainWindow listView 信号读取集合
        ObservableCollection<SignalDisplay> _mainSignalList = new ObservableCollection<SignalDisplay>();
        //元数据信号读取集合
        ObservableCollection<SignalDisplay> _sourceSignalList = new ObservableCollection<SignalDisplay>();
        //Window _currentWindow;
        #endregion

        #region 属性
        public EthernetSetUp SetUp
        {
            get
            {
                return _setUp;
            }
            set
            {
                _setUp = value;
            }
        }
        public ILog Log
        {
            get
            {
                return _log;
            }
             set
            {
                _log = value;
            }
        }
        public TimeOut TimeOut
        {
            get
            {
                return _timeOut;
            }
            set
            {
                _timeOut = value;
            }
        }

        //public Window CurrentWindow
        //{
        //    set
        //    {
        //        _currentWindow = value;
        //    }
        //    get
        //    {
        //        return _currentWindow;
        //    }
        //}
        public ObservableCollection<SignalDisplay> MainSignalList
        {
            get { return _mainSignalList; }
            set{ _mainSignalList = value; }
        }
        public ObservableCollection<SignalDisplay> SourceSignalList
        {
            get { return _sourceSignalList; }
            set { _sourceSignalList = value; }
        }
        #endregion
        private static SignalServer Instance;
        private static readonly object locker = new object();
        public static SignalServer GetInstance()
        {
            if (Instance == null)
            {
                lock (locker)
                {
                    if (Instance == null)
                    {
                        Instance =new SignalServer();
                    }
                }
            }
            return Instance;
        }
        private SignalServer( )
        {
            //_currentWindow = currentWindow;
            _setUp = new EthernetSetUp("127.0.0.1", 9527);
            _log = new Log4netWrapper();
            _log.Init("SignalMonitorLogger");
            _timeOut = new TimeOut();
            _client = new FreedomClientAsync("SignalMonitor",_setUp, _timeOut, _log) ;
            //断线重连事件
            _reConnectTimer = new Timer(3000);
            _reConnectTimer.Elapsed += ReTime_Elapsed;
            _reConnectTimer.Enabled = false;
            //读服务器元数据
            _client.AsyncReadMetaData += asyncReadMetaData;
            //订阅异步客户端事件
            _client.AsyncReadOrSubsEvent += asyncReadOrSubsEvent;
            _client.AsyncWriteEvent += ayncWriteEvent;
            _client.DisconnectEvent += disconnectEvent;

            //第一次连接
            ReadMetaData();


        }
        public void Reconnect()
        {
            _client.EthernetSetUp = _setUp;
            _client.DisConnect();
        }
        public void Disconnect()
        {
            _reConnectTimer.Enabled = false;
            _client.DisconnectEvent-= disconnectEvent;
            _client.DisConnect();
        }
        public void ReadMetaData()
        {
            if (_client.Connect())
            {
                _client.ReadMetaData();
            }
        }

        public void Subscribe(List<SignalDisplay> items)
        {
            List<string> subList = new List<string>();
            foreach (var s in items)
            {
                if (!_mainSignalList.Contains(s))
                {
                    _mainSignalList.Add(s);
                    subList.Add(s.SignalName);
                }
            }
            while (subList.Count > 1000)
            {
                var send = subList.GetRange(0, 100);
                subList.RemoveRange(0, 100);
                _client.Subscribe(send.ToArray());
            }
            _client.Subscribe(subList.ToArray());
        }

        public void CancelSubscribe(List<SignalDisplay> items)
        {
            List<string> cancelList = new List<string>();
            foreach (var s in items)
            {
                _mainSignalList.Remove(s);

                cancelList.Add(s.SignalName);
            }
            _client.CancelSubscribe(cancelList.ToArray());
        }

        public void WriteAsync( SignalDisplay item,string value) 
        {
            ITag data = new Tag { Name = item.SignalName, Type =(DataType)Enum.Parse(typeof(DataType), item.Type), Value = value };
            _client.WriteAsync(new ITag[] { data });
        }
        private void asyncReadMetaData(List<PointMetadata> obj)
        {
            foreach (var metaData in obj)
            {
                //if (metaData.Length == 1)
                //{
                //    SignalDisplay s = new SignalDisplay { SignalName = metaData.Name, Type = metaData.ValueType.ToString(), IsVirtual = metaData.IsVirtual };
                //    _sourceSignalList.Add(s);
                //}
                //else
                //{
                for (int i = 0; i < metaData.Length; i++)
                {
                    string name = string.Concat(metaData.Name, "[", i, "]");
                    SignalDisplay s = new SignalDisplay { SignalName = name, Type = metaData.ValueType.ToString(), IsVirtual = metaData.IsVirtual };
                    _sourceSignalList.Add(s);
                }
                //}
            }
        }

        private void disconnectEvent(FreedomClientAsync obj)
        {
            Application.Current.Dispatcher.Invoke(() => {
                _sourceSignalList.Clear();
                _mainSignalList.Clear();
            });
            if (!_reConnectTimer.Enabled)
            {
                _reConnectTimer.Enabled = true;
            }
        }
        public event Action<bool,string[]> WriteFreedBack;
        private void ayncWriteEvent(bool result,string[] message)
        {
            WriteFreedBack?.Invoke(result, message);
        }

        private void asyncReadOrSubsEvent(List<ITag> obj)
        {
            foreach (var result in obj)
            {
                //IEnumerable<SignalDisplay> signalDisplays;
                //if (_mainSignalList.Contains(new SignalDisplay { SignalName = result.Name }, new SignalDisplayCompare()))
                //{
                //    signalDisplays = _mainSignalList.Select((a) => a.SignalName == result.Name ? a : null);

                //}
                //else
                //{
                //    signalDisplays = _mainSignalList.Select((a) => a.SignalName == result.Name ? a : null);
                //}
                var signalDisplays = from s in _mainSignalList
                                     where s.SignalName == result.Name
                                     select s;

                foreach (var signalDisplay in signalDisplays)
                {
                    if (signalDisplay != null)
                    {
                        signalDisplay.Quality = result.Quality.ToString();
                        signalDisplay.Value = result.Value;
                    }
                }
            }
        }

        private void ReTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timer = sender as Timer;
            timer.Enabled = false;
            if (_client.Connect())
            {
                _client.ReadMetaData();
            }
            else
            {
                timer.Enabled = true;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _reConnectTimer?.Close();
                    _client?.Dispose();
                    Instance = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SignalServer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
