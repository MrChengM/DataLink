using System;
using System.Collections.Generic;
using DataServer.Points;
using DataServer;
using DataServer.Log;
using DataServer.Config;
using Timer = System.Timers;
using System.Threading;
using Utillity.Data;
using DataServer.Task;
using Unity;

namespace TaskMgr.Task
{
    public class ComChannelTask : AbstractTask
    {

        private IPLCDriver _client;

        private Timer.Timer _timeRead;

        private IPointMapping _pointMapping;
        private ChannelConfig _channelConfig;
        private Dictionary<string, DevicePointsBuffer> _pointsPool;

        private object _locker = new object();

        private Type _driverType;

        private ILog _log;
        [Dependency]
        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        /// <summary>
        /// 所有设备点集合，按设备分类
        /// </summary>
        public ComChannelTask(ChannelConfig channelConfig, IPointMapping pointMapping,Type driverType)
        {
            _channelConfig = channelConfig;
            _taskName = _channelConfig.Name;
            _pointMapping = pointMapping;
            _driverType = driverType;
            _initLevel = 1;

        }
        public override bool OnInit()
        {
            bool result = false;
            _log.InfoLog($"{_taskName}: Init => Initing ");
            try
            {
                _client = createClient();
                registerPoints();
                _timeRead = new Timer.Timer(_channelConfig.ScanTimes);
                _timeRead.Elapsed += TimeRead_Elapsed;
                _timeRead.AutoReset = true;
                _log.InfoLog($"{_taskName}: Initing=>Inited");
                result = true;
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: Init error '{e.Message}'");
            }
            return result;
        }

        public override bool OnStart()
        {
            _log.InfoLog($"{_taskName}: Start=>Starting ");
            if (_client != null && _timeRead != null)
            {
                _client.Connect();
                _timeRead.Start();
                _log.InfoLog($"{_taskName}: Starting=>Started ");
                return true;
            }
            else
            {
                _log.InfoLog($"{_taskName}: Start Failed ");
                return false;
            }
        }

        public override bool OnStop()
        {
            _log.InfoLog($"{_taskName}: Stop=>Stopping");
            _timeRead.Stop();
            Thread.Sleep(3000);
            var result = _client.DisConnect();
            if (result)
            {
                _log.InfoLog($"{_taskName}: Stopping=>Stopped");

            }
            else
            {
                _log.ErrorLog($"{_taskName}: Stopp Failed");

            }
            return result;
        }
        public override bool Restart()
        {
            bool result = false;
            _log.InfoLog($"{_taskName}: Restart => Restarting ");
            if (OnStop())
            {
                RemovePoints();
                registerPoints();
                ///Updata Com port setting;
                Type type = _client.GetType();
                type.GetProperty("PortSetUp").SetValue(_client, _channelConfig.ComunicationSetUp.SerialportSet);
                ///Updata scan time;
                _timeRead.Interval = _channelConfig.ScanTimes;
                if (OnStart())
                {
                    _log.InfoLog($"{_taskName}: Restart => Restarted ");
                    result = true;
                }
            }
            else
            {
                _log.InfoLog($"{_taskName}: Restart => Failed ");
                result = false;
            };

            return result;

        }

        int maxErrorTimes = 3;
        void TimeRead_Elapsed(object sender, Timer.ElapsedEventArgs e)
        {
            var t = sender as Timer.Timer;
            t.Stop();
            foreach (var item in _pointsPool)
            {

                int errorTimes = 0;
                lock (_locker)
                {
                    var client = getPLCDriver(item.Key);
                    var deviceConfig = _channelConfig.Devices[item.Key];
                    var _points = item.Value;
                    if (!client.IsConnect)
                    {
                        client.Connect();
                    }
                    foreach (var point in _points.BoolPoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.ReadBools(point.Address, (ushort)point.Length); 

                            Thread.Sleep(deviceConfig.Timing); 
                           
                        }
                        
                    }
                    if (errorTimes >= 3)
                    {
                        continue;
                    }
                    foreach (var point in _points.BytePoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.ReadBytes(point.Address, (ushort)point.Length);
                            if (point.GetQuality() != QUALITIES.QUALITY_GOOD)
                            {
                                errorTimes++;
                                if (errorTimes >= maxErrorTimes)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(deviceConfig.Timing);
                        }
                    }
                    if (errorTimes >= 3)
                    {
                        continue;
                    }
                    foreach (var point in _points.UShortPoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.ReadUShorts(point.Address, (ushort)point.Length);
                            if (point.GetQuality() != QUALITIES.QUALITY_GOOD)
                            {
                                errorTimes++;
                                if (errorTimes >= maxErrorTimes)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(deviceConfig.Timing);
                        }
                    }
                    if (errorTimes >= 3)
                    {
                        continue;
                    }
                    foreach (var point in _points.ShortPoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.ReadShorts(point.Address, (ushort)point.Length);
                            if (point.GetQuality() != QUALITIES.QUALITY_GOOD)
                            {
                                errorTimes++;
                                if (errorTimes >= maxErrorTimes)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(deviceConfig.Timing);
                        }
                    }
                    if (errorTimes >= 3)
                    {
                        continue;
                    }
                    foreach (var point in _points.IntPoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.ReadInts(point.Address, (ushort)point.Length);
                            if (point.GetQuality() != QUALITIES.QUALITY_GOOD)
                            {
                                errorTimes++;
                                if (errorTimes >= maxErrorTimes)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(deviceConfig.Timing);
                        }

                    }
                    if (errorTimes >= 3)
                    {
                        continue;
                    }
                    foreach (var point in _points.UIntPoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.ReadUInts(point.Address, (ushort)point.Length);
                            if (point.GetQuality() != QUALITIES.QUALITY_GOOD)
                            {
                                errorTimes++;
                                if (errorTimes >= maxErrorTimes)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(deviceConfig.Timing);
                        }
                    }
                    if (errorTimes >= 3)
                    {
                        continue;
                    }
                    foreach (var point in _points.FloatPoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.Readfloats(point.Address, (ushort)point.Length);
                            if (point.GetQuality() != QUALITIES.QUALITY_GOOD)
                            {
                                errorTimes++;
                                if (errorTimes >= maxErrorTimes)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(deviceConfig.Timing);
                        }
                    }
                    if (errorTimes >= 3)
                    {
                        continue;
                    }
                    foreach (var point in _points.StringPoints)
                    {
                        if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                        {
                            point.Value = client.ReadStrings(point.Address, (ushort)point.Length);
                            Thread.Sleep(deviceConfig.Timing);
                        }
                    }
                }
            }
            t.Start();
        }
        private WriteResult StringPoint_WriteEvent(DevicePoint<string> point, int index,string value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);
        }

        private WriteResult FloatPoint_WriteEvent(DevicePoint<float> point, int index,float value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);

        }
        private WriteResult UintPoint_WriteEvent(DevicePoint<uint> point, int index,uint value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);
        }
        private WriteResult IntPoint_WriteEvent(DevicePoint<int> point, int index, int value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);
        }
        private WriteResult UshortPoint_WriteEvent(DevicePoint<ushort> point, int index,ushort value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);

        }
        private WriteResult ShortPoint_WriteEvent(DevicePoint<short> point, int index,short value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);
        }
        private WriteResult BytePoint_WriteEvent(DevicePoint<byte> point, int index,byte value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);
        }
        private WriteResult BoolPoint_WriteEvent(DevicePoint<bool> point, int index,bool value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteData(client, point, index, value);
        }
        private WriteResult StringPoint_WritesEvent(DevicePoint<string> point, string[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point, value);
        }
        private WriteResult FloatPoint_WritesEvent(DevicePoint<float> point, float[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point,  value);

        }
        private WriteResult UintPoint_WritesEvent(DevicePoint<uint> point, uint[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point,  value);
        }
        private WriteResult IntPoint_WritesEvent(DevicePoint<int> point,  int[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point, value);
        }
        private WriteResult UshortPoint_WritesEvent(DevicePoint<ushort> point,  ushort[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point, value);

        }
        private WriteResult ShortPoint_WritesEvent(DevicePoint<short> point,  short[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point, value);
        }
        private WriteResult BytePoint_WritesEvent(DevicePoint<byte> point, byte[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point, value);
        }
        private WriteResult BoolPoint_WritesEvent(DevicePoint<bool> point,  bool[] value)
        {
            var client = getPLCDriver(point.DeviceName);
            return WritePointHandler.WriteDatas(client, point, value);
        }
        /// <summary>
        /// 由于串口通信特征，同一协议下多设备读取只创建一个协议类；
        /// </summary>
        /// <param name="driverInfo"></param>
        /// <returns></returns>
        private IPLCDriver createClient()
        {
            IPLCDriver pLCDriver = (IPLCDriver)Activator.CreateInstance(_driverType);
            pLCDriver.Log = _log;
            _driverType.GetProperty("PortSetUp").SetValue(pLCDriver, _channelConfig.ComunicationSetUp.SerialportSet);
            return pLCDriver;
        }

        private void updateDriverPropety(DeviceConfig device)
        {
            _client.Name = string.Format("{0}.{1}", _channelConfig.Name, device.Name);
            _client.ConnectTimeOut = device.ConnectTimeOut;
            _client.RequestTimeOut = device.RequestTimeOut;
            _client.RetryTimes = device.RetryTimes;
            _client.Id = int.Parse(device.ID);
            _client.Order = device.ByteOrder;
            Type driverType = _client.GetType();
            foreach (var p in device.SpecialProperties)
            {
                driverType.GetProperty(p.Name).SetValue(_client, p.Value);

            }
        }

        private  IPLCDriver getPLCDriver(string deviceName)
        {
           
            updateDriverPropety(_channelConfig.Devices[deviceName]);
            return _client;
        }

        private void registerPoints()
        {
            _pointsPool = new Dictionary<string, DevicePointsBuffer>();
            foreach (var device in _channelConfig.Devices)
            {
                var deviceName = device.Key;

                DevicePointsBuffer points = new DevicePointsBuffer();
                foreach (var tags in device.Value.TagGroups)
                {
                    foreach (var tag in tags.Value.Tags)
                    {
                        var pointName = string.Format("{0}.{1}.{2}.{3}", _channelConfig.Name, device.Key, tags.Key, tag.Key);
                        var address = getPLCDriver(deviceName).GetDeviceAddress(tag.Value.Address);
                        switch (tag.Value.DataType)
                        {
                            case DataType.Bool:
                                var boolPoint = new DevicePoint<bool>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                boolPoint.WriteEvent += BoolPoint_WriteEvent;
                                boolPoint.WritesEvent += BoolPoint_WritesEvent;
                                _pointMapping.Register(pointName, boolPoint);
                                points.BoolPoints.Add(boolPoint);
                                break;
                            case DataType.Byte:
                                var bytePoint = new DevicePoint<byte>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                bytePoint.WriteEvent += BytePoint_WriteEvent;
                                bytePoint.WritesEvent += BytePoint_WritesEvent;
                                _pointMapping.Register(pointName, bytePoint);
                                points.BytePoints.Add(bytePoint);
                                break;
                            case DataType.Short:
                                var shortPoint = new DevicePoint<short>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                shortPoint.WriteEvent += ShortPoint_WriteEvent; 
                                shortPoint.WritesEvent += ShortPoint_WritesEvent;
                                _pointMapping.Register(pointName, shortPoint);
                                points.ShortPoints.Add(shortPoint);
                                break;
                            case DataType.UShort:
                                var ushortPoint = new DevicePoint<ushort>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                ushortPoint.WriteEvent += UshortPoint_WriteEvent; 
                                ushortPoint.WritesEvent += UshortPoint_WritesEvent;
                                _pointMapping.Register(pointName, ushortPoint);
                                points.UShortPoints.Add(ushortPoint);
                                break;
                            case DataType.Int:
                                var intPoint = new DevicePoint<int>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                intPoint.WriteEvent += IntPoint_WriteEvent;
                                intPoint.WritesEvent += IntPoint_WritesEvent;
                                _pointMapping.Register(pointName, intPoint);
                                points.IntPoints.Add(intPoint);
                                break;
                            case DataType.UInt:
                                var uintPoint = new DevicePoint<uint>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                uintPoint.WriteEvent += UintPoint_WriteEvent;
                                uintPoint.WritesEvent += UintPoint_WritesEvent;
                                _pointMapping.Register(pointName, uintPoint);
                                points.UIntPoints.Add(uintPoint);
                                break;
                            case DataType.Float:
                                var floatPoint = new DevicePoint<float>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                floatPoint.WriteEvent += FloatPoint_WriteEvent;
                                floatPoint.WritesEvent += FloatPoint_WritesEvent;

                                _pointMapping.Register(pointName, floatPoint);
                                points.FloatPoints.Add(floatPoint);
                                break;
                            case DataType.String:
                                var stringPoint = new DevicePoint<string>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                                stringPoint.WriteEvent += StringPoint_WriteEvent;
                                stringPoint.WritesEvent += StringPoint_WritesEvent;
                                _pointMapping.Register(pointName, stringPoint);
                                points.StringPoints.Add(stringPoint);
                                break;
                            default:
                                break;
                        }
                    }
                }
                _pointsPool.Add(deviceName, points);
            }
        }
        private void RemovePoints()
        {
            //移除注册点位
            foreach (var item in _pointsPool)
            {
                foreach (var point in item.Value.BoolPoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= BoolPoint_WriteEvent; 
                    point.WritesEvent -= BoolPoint_WritesEvent;
                }
                foreach (var point in item.Value.BytePoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= BytePoint_WriteEvent;
                    point.WritesEvent -= BytePoint_WritesEvent;

                }
                foreach (var point in item.Value.ShortPoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= ShortPoint_WriteEvent;
                    point.WritesEvent -= ShortPoint_WritesEvent;

                }
                foreach (var point in item.Value.UShortPoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= UshortPoint_WriteEvent;
                    point.WritesEvent -= UshortPoint_WritesEvent;

                }
                foreach (var point in item.Value.IntPoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= IntPoint_WriteEvent;
                    point.WritesEvent -= IntPoint_WritesEvent;

                }
                foreach (var point in item.Value.UIntPoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= UintPoint_WriteEvent;
                    point.WritesEvent -= UintPoint_WritesEvent;

                }
                foreach (var point in item.Value.FloatPoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= FloatPoint_WriteEvent;
                    point.WritesEvent -= FloatPoint_WritesEvent;

                }
                foreach (var point in item.Value.StringPoints)
                {
                    _pointMapping.Remove(point.Name);
                    point.WriteEvent -= StringPoint_WriteEvent;
                    point.WritesEvent -= StringPoint_WritesEvent;

                }
            }
            _pointsPool.Clear();
        }
    }
    public class EthernetChannelTask : AbstractTask
    {
        private ChannelConfig _channelConfig;
        private List<SingleSocketTask> _socketTasks;
        private IPointMapping _pointMapping;
        private Type _driverType;

        private ILog _log;
        [Dependency]
        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        public EthernetChannelTask(ChannelConfig channelConfig,IPointMapping pointMapping,Type driverType)
        {
            _channelConfig = channelConfig;
            _taskName = channelConfig.Name;
            _pointMapping = pointMapping;
            _driverType = driverType;
            _socketTasks = new List<SingleSocketTask>();
            _initLevel = 1;
        }

        public override bool OnInit()
        {

            bool result = false;
            _log.InfoLog($"{_taskName}: Init => Initing ");
            try
            {
                foreach (var device in _channelConfig.Devices)
                {
                    var deveiceEthernetSet = EthernetSetUp.Clone(_channelConfig.ComunicationSetUp.EthernetSet);
                    deveiceEthernetSet.IPAddress = device.Value.ID;
                    var task = new SingleSocketTask(_driverType, _channelConfig.Name, _channelConfig.ScanTimes, _pointMapping, device.Value, deveiceEthernetSet, _log);
                    _socketTasks.Add(task);
                }
                _log.InfoLog($"{_taskName}: Initing => Inited ");
                result = true;
            }
            catch (Exception e)
            {

                _log.ErrorLog($"{_taskName}: Init failed,'{e.Message}'");
            }
            return result;
        }

        public override bool OnStart()
        {
            var result = true;
            _log.InfoLog($"{_taskName}: Start => Starting ");
            foreach (var task in _socketTasks)
            {
                task.Start();
            }
            return result;

        }

        public override bool OnStop()
        {
            var result = true;
            _log.InfoLog($"{_taskName}: Stop => Stopping ");
            foreach (var task in _socketTasks)
            {
                if (!task.Stop())
                {
                    _log.InfoLog($"{_taskName}: Stop Failed ");
                    return result = false;
                }
            }
            _log.InfoLog($"{_taskName}: Stopping => Stoped ");
            return result;
        }

        public override bool Restart()
        {
            bool result;
            _log.InfoLog($"{_taskName}: Restart => Restarting ");
            foreach (var task in _socketTasks)
            {
                if (!task.Clear())
                {
                    _log.InfoLog($"{_taskName}: Restart Failed");
                    return result = false;
                }
            }
            if (OnInit())
            {
                if (OnStart())
                {
                    _log.InfoLog($"{_taskName}: Restarting => Restarted ");
                    result = true;
                }
                else
                {
                    _log.InfoLog($"{_taskName}: Restart Failed");
                    result = false;
                }
            }
            else
            {
                _log.InfoLog($"{_taskName}: Restart Failed");
                result = false;
            }
            return result;
        }
    }
    /// <summary>
    /// 每个以太网设备占用单个Socket；
    /// </summary>
    public class SingleSocketTask
    {
        private string _name;
        private IPLCDriver _client;
        private IPointMapping _pointMapping;
        private DevicePointsBuffer _devicePoints;
        private Timer.Timer _timeRead;
        private static object _locker = new object();


        private string _channelName;
        private Type _driverType;
        private DeviceConfig _deviceConfig;
        private EthernetSetUp _ethernetSetUp;
        private ILog _log;
        private int _pollTime;


        
        public SingleSocketTask(Type drivertype,string channelName,int pollTime, IPointMapping pointMapping, DeviceConfig deviceConfig,EthernetSetUp ethernetSetUp,ILog log)
        {
            _driverType = drivertype;
            _channelName = channelName;
            _pollTime = pollTime;
            _deviceConfig = deviceConfig;
            _ethernetSetUp = ethernetSetUp;
            _log = log;
            _pointMapping = pointMapping;
            _timeRead = new Timer.Timer(_pollTime);
            _name = string.Format("{0}.{1}", _channelName, _deviceConfig.Name);
            _client = createClient();
            registerPoints();
        }

        private void TimeRead_Elapsed(object sender, Timer.ElapsedEventArgs e)
        {
            var t = sender as Timer.Timer;
            t.Stop();
            lock (_locker)
            {
                if (!_client.IsConnect)
                {
                    //if (!_client.Connect())
                    //{
                    //    t.Interval = 10000;
                    //    t.Start();
                    //    return;
                    //} 
                    _client.Connect();
                }
                foreach (var point in _devicePoints.BoolPoints)
                {
                    if (point.RW == ReadWriteWay.Read|| point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.ReadBools(point.Address, (ushort)point.Length);
                        Thread.Sleep(_deviceConfig.Timing);
                    }
                }
                foreach (var point in _devicePoints.BytePoints)
                {
                    if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.ReadBytes(point.Address, (ushort)point.Length);
                        Thread.Sleep(_deviceConfig.Timing);
                    }
                }
                foreach (var point in _devicePoints.UShortPoints)
                {
                    if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.ReadUShorts(point.Address, (ushort)point.Length);
                        Thread.Sleep(_deviceConfig.Timing);
                    }
                }
                foreach (var point in _devicePoints.ShortPoints)
                {
                    if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.ReadShorts(point.Address, (ushort)point.Length);
                        Thread.Sleep(_deviceConfig.Timing);
                    }
                }
                foreach (var point in _devicePoints.IntPoints)
                {
                    if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.ReadInts(point.Address, (ushort)point.Length);
                        if (point.GetQuality() != QUALITIES.QUALITY_GOOD)
                        {
                            break;
                        }
                        Thread.Sleep(_deviceConfig.Timing);
                    }

                }
                foreach (var point in _devicePoints.UIntPoints)
                {
                    if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.ReadUInts(point.Address, (ushort)point.Length);
                        Thread.Sleep(_deviceConfig.Timing);
                    }
                }
                foreach (var point in _devicePoints.FloatPoints)
                {
                    if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.Readfloats(point.Address, (ushort)point.Length);
                        Thread.Sleep(_deviceConfig.Timing);
                    }
                }
                foreach (var point in _devicePoints.StringPoints)
                {
                    if (point.RW == ReadWriteWay.Read || point.RW == ReadWriteWay.ReadAndWrite)
                    {
                        point.Value = _client.ReadStrings(point.Address, (ushort)point.Length);
                        Thread.Sleep(_deviceConfig.Timing);
                    }
                }
            }
            if (_client.IsConnect)
            {
                t.Interval = _pollTime;
            }
            else
            {
                t.Interval = 5000;
            }
            t.Start();
        }

        public bool Start()
        {
            _log.InfoLog($"{_name}: Start=>Starting ");
            if (_client != null && _timeRead != null)
            {
                _client.Connect();
                _timeRead.Elapsed += TimeRead_Elapsed;
                _timeRead.AutoReset = true;
                _timeRead.Start();
                _log.InfoLog($"{_name}: Starting=>Started ");
                return true;

            }
            else
            {
                _log.InfoLog($"{_name} :Start Failed ");
                return false;
            }
        }

        public bool Stop()
        {
            _log.InfoLog($"{_name}: Stop=>Stopping");
            _timeRead.Stop();
            Thread.Sleep(3000);
            var result = _client.DisConnect();
            if (result)
            {
                _log.InfoLog($"{_name}: Stopping=>Stopped");

            }
            else
            {
                _log.InfoLog($"{_name}: Stop Failed");

            }
            return result;
        }

        public bool Clear()
        {
            _log.InfoLog($"{_name}: Clear=>Clearing");
            var result = Stop();
            _timeRead.Close();
            _client = null;
            removePoints();
            if (result)
            {
                _log.InfoLog($"{_name}: Clearing=>Cleared");
            }
            else
            {
                _log.InfoLog($"{_name}: Clear Failed");
            }
            return result;

        }
        private IPLCDriver createClient()
        {
            IPLCDriver pLCDriver = (IPLCDriver)Activator.CreateInstance(_driverType);
            pLCDriver.Name = _name;
            pLCDriver.Log = _log;
            pLCDriver.Order = _deviceConfig.ByteOrder;
            pLCDriver.ConnectTimeOut = _deviceConfig.ConnectTimeOut;
            pLCDriver.RequestTimeOut = _deviceConfig.RequestTimeOut;
            pLCDriver.RetryTimes = _deviceConfig.RetryTimes;
            _driverType.GetProperty("EthSetUp").SetValue(pLCDriver,_ethernetSetUp);
            foreach (var p in _deviceConfig.SpecialProperties)
            {
                _driverType.GetProperty(p.Name).SetValue(_driverType, p.Value);

            }
            return pLCDriver;
        }

        private void registerPoints()
        {
            _devicePoints = new DevicePointsBuffer();
            var deviceName = string.Format("{0}.{1}", _channelName, _deviceConfig.Name);
            foreach (var tags in _deviceConfig.TagGroups)
            {
                foreach (var tag in tags.Value.Tags)
                {
                    var pointName = string.Format("{0}.{1}.{2}.{3}", _channelName, _deviceConfig.Name, tags.Key, tag.Key);
                    var address = _client.GetDeviceAddress(tag.Value.Address);
                    switch (tag.Value.DataType)
                    {
                        case DataType.Bool:
                            var boolPoint = new DevicePoint<bool>(pointName, tag.Value.Length, address, deviceName) { RW =tag.Value.Operate};
                            boolPoint.WriteEvent += BoolPoint_WriteEvent;
                            boolPoint.WritesEvent += BoolPoint_WritesEvent;

                            _pointMapping.Register(pointName, boolPoint);
                            _devicePoints.BoolPoints.Add(boolPoint);
                            break;
                        case DataType.Byte:
                            var bytePoint = new DevicePoint<byte>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                            bytePoint.WriteEvent += BytePoint_WriteEvent;
                            bytePoint.WritesEvent += BytePoint_WritesEvent; ;

                            _pointMapping.Register(pointName, bytePoint);
                            _devicePoints.BytePoints.Add(bytePoint);
                            break;
                        case DataType.Short:
                            var shortPoint = new DevicePoint<short>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                            shortPoint.WriteEvent += ShortPoint_WriteEvent;
                            shortPoint.WritesEvent += ShortPoint_WritesEvent;

                            _pointMapping.Register(pointName, shortPoint);
                            _devicePoints.ShortPoints.Add(shortPoint);
                            break;
                        case DataType.UShort:
                            var ushortPoint = new DevicePoint<ushort>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                            ushortPoint.WriteEvent += UshortPoint_WriteEvent;
                            ushortPoint.WritesEvent += UshortPoint_WritesEvent;

                            _pointMapping.Register(pointName, ushortPoint);
                            _devicePoints.UShortPoints.Add(ushortPoint);
                            break;
                        case DataType.Int:
                            var intPoint = new DevicePoint<int>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                            intPoint.WriteEvent += IntPoint_WriteEvent;
                            intPoint.WritesEvent += IntPoint_WritesEvent;

                            _pointMapping.Register(pointName, intPoint);
                            _devicePoints.IntPoints.Add(intPoint);
                            break;
                        case DataType.UInt:
                            var uintPoint = new DevicePoint<uint>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                            uintPoint.WriteEvent += UintPoint_WriteEvent;
                            uintPoint.WritesEvent += UintPoint_WritesEvent;

                            _pointMapping.Register(pointName, uintPoint);
                            _devicePoints.UIntPoints.Add(uintPoint);
                            break;
                        case DataType.Float:
                            var floatPoint = new DevicePoint<float>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                            floatPoint.WriteEvent += FloatPoint_WriteEvent;
                            floatPoint.WritesEvent += FloatPoint_WritesEvent;

                            _pointMapping.Register(pointName, floatPoint);
                            _devicePoints.FloatPoints.Add(floatPoint);
                            break;
                        case DataType.String:
                            var stringPoint = new DevicePoint<string>(pointName, tag.Value.Length, address, deviceName) { RW = tag.Value.Operate };
                            stringPoint.WriteEvent += StringPoint_WriteEvent;
                            stringPoint.WritesEvent += StringPoint_WritesEvent;

                            _pointMapping.Register(pointName, stringPoint);
                            _devicePoints.StringPoints.Add(stringPoint);
                            break;
                        default:
                            break;
                    }
                }
            }

        }
        private void removePoints()
        {
            //移除注册点位

            foreach (var point in _devicePoints.BoolPoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= BoolPoint_WriteEvent;
            }
            foreach (var point in _devicePoints.BytePoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= BytePoint_WriteEvent;
            }
            foreach (var point in _devicePoints.ShortPoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= ShortPoint_WriteEvent;

            }
            foreach (var point in _devicePoints.UShortPoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= UshortPoint_WriteEvent;
            }
            foreach (var point in _devicePoints.IntPoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= IntPoint_WriteEvent;
            }
            foreach (var point in _devicePoints.UIntPoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= UintPoint_WriteEvent;
            }
            foreach (var point in _devicePoints.FloatPoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= FloatPoint_WriteEvent;
            }
            foreach (var point in _devicePoints.StringPoints)
            {
                _pointMapping.Remove(point.Name);
                point.WriteEvent -= StringPoint_WriteEvent;
            }

        }

        private WriteResult StringPoint_WriteEvent(DevicePoint<string> point, int index, string value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);
        }

        private WriteResult FloatPoint_WriteEvent(DevicePoint<float> point, int index, float value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);
        }

        private WriteResult UintPoint_WriteEvent(DevicePoint<uint> point, int index, uint value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);
        }

        private WriteResult IntPoint_WriteEvent(DevicePoint<int> point, int index, int value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);
        }

        private WriteResult UshortPoint_WriteEvent(DevicePoint<ushort> point, int index,ushort value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);
        }
        private WriteResult ShortPoint_WriteEvent(DevicePoint<short> point, int index,short value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);
        }

        private WriteResult BytePoint_WriteEvent(DevicePoint<byte> point, int index,byte value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);

        }
        private WriteResult BoolPoint_WriteEvent(DevicePoint<bool> point, int index,bool value)
        {
            return WritePointHandler.WriteData(_client, point, index, value);
        }
        private WriteResult StringPoint_WritesEvent(DevicePoint<string> point, string[] value)
        {
            return WritePointHandler.WriteDatas(_client, point,  value);
        }
        private WriteResult FloatPoint_WritesEvent(DevicePoint<float> point,  float[] value)
        {
            return WritePointHandler.WriteDatas(_client, point,  value);
        }

        private WriteResult UintPoint_WritesEvent(DevicePoint<uint> point, uint[] value)
        {
            return WritePointHandler.WriteDatas(_client, point,  value);
        }

        private WriteResult IntPoint_WritesEvent(DevicePoint<int> point,  int[] value)
        {
            return WritePointHandler.WriteDatas(_client, point,  value);
        }

        private WriteResult UshortPoint_WritesEvent(DevicePoint<ushort> point, ushort[] value)
        {
            return WritePointHandler.WriteDatas(_client, point,  value);
        }
        private WriteResult ShortPoint_WritesEvent(DevicePoint<short> point, short[] value)
        {
            return WritePointHandler.WriteDatas(_client, point,value);
        }

        private WriteResult BytePoint_WritesEvent(DevicePoint<byte> point, byte[] value)
        {
            return WritePointHandler.WriteDatas(_client, point, value);

        }
        private WriteResult BoolPoint_WritesEvent(DevicePoint<bool> point,  bool[] value)
        {
            return WritePointHandler.WriteDatas(_client, point, value);
        }
    }

    public static class WritePointHandler
    {
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<bool> point, int index, bool value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteBool(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<bool> point, bool[] value)
        {
            var result = new WriteResult();
            var re = client.WriteBools(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<byte> point, int index, byte value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteByte(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<byte> point, byte[] value)
        {
            var result = new WriteResult();
            var re = client.WriteBytes(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<ushort> point, int index, ushort value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteUShort(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<ushort> point, ushort[] value)
        {
            var result = new WriteResult();
            var re = client.WriteUShorts(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<short> point, int index, short value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteShort(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<short> point, short[] value)
        {
            var result = new WriteResult();
            var re = client.WriteShorts(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<uint> point, int index, uint value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteUInt(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<uint> point, uint[] value)
        {
            var result = new WriteResult();
            var re = client.WriteUInts(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<int> point, int index, int value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteInt(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<int> point, int[] value)
        {
            var result = new WriteResult();
            var re = client.WriteInts(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<float> point, int index, float value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteFloat(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<float> point, float[] value)
        {
            var result = new WriteResult();
            var re = client.WriteFloats(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
        public static WriteResult WriteData(IPLCDriver client, DevicePoint<string> point, int index, string value)
        {
            var result = new WriteResult();

            if (index < 0)
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Index set error ";
            }
            else
            {
                var re = client.WriteString(point.Address, value, index);
                if (re == 1)
                {
                    result.Result = OperateResult.OK;

                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = $"PointName:‘{point.Name}’,Index:'{index}',Write to device error ";
                }

            }
            return result;
        }
        public static WriteResult WriteDatas(IPLCDriver client, DevicePoint<string> point, string[] value)
        {
            var result = new WriteResult();
            var re = client.WriteStrings(point.Address, value);
            if (re == 1)
            {
                result.Result = OperateResult.OK;
            }
            else
            {
                result.Result = OperateResult.NG;
                result.Messages = $"PointName:‘{point.Name}’,Write to device error ";
            }
            return result;
        }
    }
    /// <summary>
    /// 同一设备下所包含的点集合
    /// </summary>
    public class DevicePointsBuffer
    {
        public List<DevicePoint<bool>> BoolPoints { get; set; } = new List<DevicePoint<bool>>();
        public List<DevicePoint<byte>> BytePoints { get; set; } = new List<DevicePoint<byte>>();
        public List<DevicePoint<short>> ShortPoints { get; set; } = new List<DevicePoint<short>>();
        public List<DevicePoint<ushort>> UShortPoints { get; set; } = new List<DevicePoint<ushort>>();
        public List<DevicePoint<int>> IntPoints { get; set; } = new List<DevicePoint<int>>();
        public List<DevicePoint<uint>> UIntPoints { get; set; } = new List<DevicePoint<uint>>();
        public List<DevicePoint<float>> FloatPoints { get; set; } = new List<DevicePoint<float>>();
        public List<DevicePoint<string>> StringPoints { get; set; } = new List<DevicePoint<string>>();

    }
}
