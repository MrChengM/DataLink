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
using Timer = System.Timers;
using ModbusDrivers.Client;
using TaskHandler.Factory;
using TaskHandler.Builder;
using TaskHandler.Config;

namespace TaskHandler
{
    public class ClientHandlerTask:AbstractTask
    {
        ClientTaskBuilder _clientBuilder;
        IPLCDriver _client;
        ClientConfig _config;
        PointDeviceCollcet _points;
        Timer.Timer _timeRead;

        public IPLCDriver Client { get { return _client; }set { _client = value; } }
        public ClientConfig Config { get { return _config; } set { _config = value; } }
        public PointDeviceCollcet Points { get { return _points; } set { _points = value; } }

        public ClientHandlerTask()
        {

        }
        public ClientHandlerTask(ClientTaskBuilder builder)
        {
            _clientBuilder = builder;
        }
        public override bool OnInit()
        {
            //try
            //{
                if (_clientBuilder.BuildTaskName())
                {
                    if (_clientBuilder.BuildLog())
                    {
                        _log.NormalLog(string.Format("{0}:Init=>Initing ", "OnInit()"));
                        if (_clientBuilder.BuildConfig())
                        {
                            if (_clientBuilder.BuildPoints())
                            {
                                if (_clientBuilder.BuildClient())
                                {
                                    _timeRead = new Timer.Timer(_config.PollingTime);
                                    _log.NormalLog(string.Format("{0}:Initing=>Inited ", "OnInit()"));
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;

            //}
            //catch (Exception e)
            //{
            //    _log.ErrorLog(string.Format("Init fail:{0}", e.Message));
            //    return false;
            //}


        }

        public override bool OnStart()
        {
            _log.NormalLog(string.Format("{0}:Start=>Starting ", "OnStart()"));
            if (_client != null && _timeRead != null)
            {
                _client.Connect();

                _timeRead.Elapsed += TimeRead_Elapsed;
                _timeRead.AutoReset = true;
                _timeRead.Start();
                subscribeSendData();
                _log.NormalLog(string.Format("{0}:Starting=>Started ", "OnStart()"));
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
                    point.SendEvent += (s, p, q) =>
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
                    point.Value = _client.ReadBools(point.Address, (ushort)point.Length);
                }
                //foreach (var point in points.BytePoints)
                //{
                //    var devicePoint = point as DevicePoint<byte>;
                //    client.ReadBytes(devicePoint.Address, (ushort)devicePoint.Length);
                //}
                foreach (var point in _points.UshortPoints)
                {
                    point.Value = _client.ReadUShorts(point.Address, (ushort)point.Length);
                }
                foreach (var point in _points.ShortPoints)
                {
                    point.Value = _client.ReadShorts(point.Address, (ushort)point.Length);
                }
                foreach (var point in _points.IntPoints)
                {
                    point.Value = _client.ReadInts(point.Address, (ushort)point.Length);

                }
                foreach (var point in _points.UintPoints)
                {
                    point.Value = _client.ReadUInts(point.Address, (ushort)point.Length);
                }
                foreach (var point in _points.FloatPoints)
                {
                    point.Value = _client.Readfloats(point.Address, (ushort)point.Length);
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

        public override bool OnStop()
        {
            return _client.DisConnect();

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
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
        #endregion
    }
}
