using DataServer;
using ds = DataServer;
using DataServer.Serialization;
using DataServer.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Utillity;
using System.Xml;
using System.Threading;
using Timer=System.Timers;

namespace ModbusDrivers.Client
{
    public class ModbusTCPClientHanderTask:IDisposable
    {
        ModbusTCPClient _client;
        ModbusTCPClientConfig _config; 
        PointDeviceCollcet _points;
        XMLWorkbook _workbook;
        EthernetSetUp _setup;
        TimeOut _timeout;
        Timer.Timer _timeRead;
        ILog _log;

        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        //public ModbusTCPClientHanderTask()
        //{
            
        //}
        public ModbusTCPClientHanderTask(ILog log)
        {
            this._log = log;
        }
        public bool OnInit()
        {
            _log.NormalLog(string.Format("ModbusTCPClientHanderTask:Init=>Initing "));
            try
            {
                //获取配置
                _config = ReaderXMLUtil.ReadXMLConfig<ModbusTCPClientConfig>("../../../../conf/Configuration.xml", ModbusTCPClientConfig.ReadConfig, "setup", "ModbusTCPClientHandlerTask")[0];
                if (_config.IpAddress == "" || _config.SignalListFilePath == "")
                {
                    _log.ErrorLog(string.Format("ModbusTCPClientHanderTask Init fail:{0}","IP address or Signal file path is null!"));
                    return false;
                }
                _setup = new EthernetSetUp(_config.IpAddress, _config.Port);
                _timeout = new TimeOut("ModbusTCPClientHanderTask", _config.TimeOut, _log);

                _timeRead = new Timer.Timer(_config.PollingTime);

                _client = new ModbusTCPClient(_setup, _timeout, _log);

                //获取点数据
                _workbook = XmlSerialiaztion.XmlDeserial<XMLWorkbook>(_config.SignalListFilePath, _log);
                _points = PointsCollcetCreate.Create(_workbook,_log);
                //点数据注册
                PointsRegister.Register( ref _points, _log);
                _log.NormalLog(string.Format("ModbusTCPClientHanderTask:Initing=>Inited "));
                return true;

            }
            catch(Exception e)
            {
                _log.ErrorLog(string.Format("ModbusTCPClientHanderTask Init fail:{0}", e.Message));
                return false;
            }


        }

        public bool OnStart()
        {
            _log.NormalLog(string.Format("ModbusTCPClientHanderTask:Start=>Starting"));
            if (_client!=null&& _timeRead!=null)
            {
                _client.Connect();
                
                _timeRead.Elapsed += TimeRead_Elapsed;
                _timeRead.AutoReset = true;
                _timeRead.Start();
                subscribeSendData();
                _log.NormalLog(string.Format("ModbusTCPClientHanderTask:Starting=>Started"));
                return true;
            }
            else
            {
                return false;
            }

        }
        private readonly static object _locker = new object();
        private readonly static object _locker1 = new object();
        private readonly static object _locker2 = new object();
        private readonly static object _locker3 = new object();
        private readonly static object _locker4 = new object();
        private readonly static object _locker5 = new object();

        /// <summary>
        /// 订阅数据发送
        /// </summary>
        private void subscribeSendData()
        {
            if (_client.IsConnect)
            {
                foreach (var point in _points.BoolPoints)
                {
                    point.SendEvent += (s, p, q) =>
                    {
                        lock (_locker)
                        {
                            _timeRead.Stop();
                            var temp = s as DevicePoint<bool>;
                            if (!q)//true则进行所有数据发送，false只需发送单个数据
                            {
                                var address = temp.Address;
                                address.Address += p;
                                _client.WriteBool(address, temp[(byte)p]);
                            }
                            else
                            {
                                _client.WriteBools(temp.Address, temp.GetValues());
                            }
                            _timeRead.Start();
                        }
                    };
                }
                foreach (var point in _points.ShortPoints)
                {
                    point.SendEvent += (s, p, q) =>
                    {
                        lock (_locker1)
                        {
                            _timeRead.Stop();
                            var temp = s as DevicePoint<short>;
                            if (!q)
                            {
                                var address = temp.Address;
                                address.Address += p;
                                _client.WriteShort(address, temp[(byte)p]);
                            }
                            else
                            {
                                _client.WriteShorts(temp.Address, s.GetValues());
                            }
                            _timeRead.Start();

                        }

                    };
                }
                foreach (var point in _points.UshortPoints)
                {
                    point.SendEvent += (s, p,q) =>
                    {
                        lock (_locker2)
                        {
                            _timeRead.Stop();
                            var temp = s as DevicePoint<ushort>;
                            if (!q)
                            {
                                var address = temp.Address;
                                address.Address += p;
                                _client.WriteUShort(address, temp[(byte)p]);
                            }
                            else
                            {
                                _client.WriteUShorts(temp.Address, temp.GetValues());
                            }
                            _timeRead.Start();
                        }
                       
                    };
                    
                }
                foreach (var point in _points.IntPoints)
                {

                    point.SendEvent += (s, p, q) =>
                    {
                        lock (_locker3)
                        {
                            _timeRead.Stop();
                            var temp = s as DevicePoint<int>;
                            if (!q)
                            {
                                var address = temp.Address;
                                address.Address += p * 2;
                                _client.WriteInt(address, temp[(byte)p]);
                            }
                            else
                            {
                                _client.WriteInts(temp.Address, s.GetValues());
                            }
                            _timeRead.Start();
                        }
                       
                    };

                }
                foreach (var point in _points.UintPoints)
                {
                    point.SendEvent += (s, p, q) =>
                    {
                        lock (_locker4)
                        {
                            _timeRead.Stop();
                            var temp = s as DevicePoint<uint>;
                            if (!q)
                            {
                                var address = temp.Address;
                                address.Address += p * 2;
                                _client.WriteUInt(address, temp[(byte)p]);
                            }
                            else
                            {
                                _client.WriteUInts(temp.Address, s.GetValues());
                            }
                            _timeRead.Start();
                        }
                    };
                }
                foreach (var point in _points.FloatPoints)
                {
                    point.SendEvent += (s, p, q) =>
                    {
                        lock (_locker5)
                        {
                            _timeRead.Stop();
                            var temp = s as DevicePoint<float>;
                            if (!q)
                            {
                                var address = temp.Address;
                                address.Address += p * 2;
                                _client.WriteFloat(address, temp[(byte)p]);
                            }
                            else
                            {
                                _client.WriteFloats(temp.Address, s.GetValues());
                            }
                            _timeRead.Start();
                        }
                    };

                }
            }
 
        }
        private void TimeRead_Elapsed(object sender, Timer.ElapsedEventArgs e)
        {
            var t = sender as Timer.Timer;
            t.Stop();
            if (_client.IsConnect)
            {
                foreach (var point in _points.BoolPoints)
                {
                    point.Value= _client.ReadBools(point.Address, (ushort)point.Length);
                }
                //foreach (var point in points.BytePoints)
                //{
                //    var devicePoint = point as DevicePoint<byte>;
                //    client.ReadBytes(devicePoint.Address, (ushort)devicePoint.Length);
                //}
                foreach (var point in _points.UshortPoints)
                {
                    point.Value=_client.ReadUShorts(point.Address, (ushort)point.Length);
                }
                foreach (var point in _points.ShortPoints)
                {
                    point.Value=_client.ReadShorts(point.Address, (ushort)point.Length);
                }
                foreach (var point in _points.IntPoints)
                {
                    point.Value = _client.ReadInts(point.Address, (ushort)point.Length);

                }
                foreach (var point in _points.UintPoints)
                {
                    point.Value=_client.ReadUInts(point.Address, (ushort)point.Length);
                }
                foreach (var point in _points.FloatPoints)
                {
                    point.Value=_client.Readfloats(point.Address, (ushort)point.Length);
                }
                //foreach (var point in _points.StringPoints)
                //{
                //    var devicePoint = point as DevicePoint<string>;
                //    devicePoint.Value= _client.ReadStrings(devicePoint.Address, (ushort)devicePoint.Length);
                //}
            }
            else
            {
                _client.Connect();
            }
            t.Start();
        }

        public bool OnStop()
        {
            return _client.DisConnect();
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ModbusTCPClientHanderTask() {
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
