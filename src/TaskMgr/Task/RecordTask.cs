using DataServer.Config;
using DataServer.Points;
using DataServer.Task;
using System;
using System.Collections.Generic;
using Timer=System.Timers;
using DataServer;
using DBHandler_EF.Serivces;
using Utillity.Data;
using System.Collections.Concurrent;
using System.Threading;
using Unity;

namespace TaskMgr.Task
{
    public class RecordTaskOnTime : AbstractTask
    {
        private IPointMapping _pointMapping;
        private RecordItemConfig _recordItemConfig;
        private Timer.Timer _timeRecord;
        private ITagRecord _tagRecord;
        private ILog _log;
        [Dependency]
        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        public RecordTaskOnTime(IPointMapping pointMapping, RecordItemConfig recordItemConfig,ILog log)
        {
            _pointMapping = pointMapping;
            _recordItemConfig = recordItemConfig;
            _taskName = recordItemConfig.Name;
            _initLevel = 4;
            _log = log;
        }
        public override bool OnInit()
        {
            bool result = false;
            _log.InfoLog($"{_taskName}: Init => Initing ");
            try
            {
                _timeRecord = new Timer.Timer(_recordItemConfig.TimeSpan);
                _timeRecord.Elapsed += _timeRecord_Elapsed; ;
                _timeRecord.AutoReset = true;
                _tagRecord = new LogTagSerivce();
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

            _timeRecord.Start();
            //_log.InfoLog($"{_taskName}: Time start: {DateTime.Now} ");

            _log.InfoLog($"{_taskName}: Starting=>Started ");
            return true;
        }

        private void _timeRecord_Elapsed(object sender, Timer.ElapsedEventArgs e)
        {
            var t = sender as Timer.Timer;
            //_log.InfoLog($"{_taskName}: _timeRecord_Elapsed inovke: {DateTime.Now} ");

            t.Stop();
            List<ITag> recordTags = new List<ITag>(); 
            foreach (var tagName in _recordItemConfig.TagNames)
            {
                var pointNameGroup = StringHandler.SplitEndWith(tagName);
                if (pointNameGroup.Length > 1)
                {
                    var pointName = pointNameGroup[0];
                    var index = int.Parse(pointNameGroup[1]);
                    var pointNameIndex = new PointNameIndex(pointName, index);
                    var tag = _pointMapping.GetTag(pointNameIndex);
                    recordTags.Add(tag);
                }
                else
                {
                    var tags =_pointMapping.GetTags(pointNameGroup[0]);
                    recordTags.AddRange(tags);
                }
            }
            _tagRecord.Insert(recordTags);
            //_log.InfoLog($"{_taskName}: _timeRecord_Elapsed end: {DateTime.Now} ");

            t.Start();
        }


        public override bool OnStop()
        {
            _log.InfoLog($"{_taskName}: Stop=>Stopping");
            _timeRecord.Stop();
            Thread.Sleep(3000);
            _log.InfoLog($"{_taskName}: Stopping=>Stopped");
            return true;
        }

        public override bool Restart()
        {
            _log.InfoLog($"{_taskName}: Restart => Restarting ");
            OnStop();
            if (OnInit())
            {
                if (OnStart())
                {
                    _log.InfoLog($"{_taskName}: Restarting => Restarted ");
                    return true;
                }
                else
                {
                    _log.InfoLog($"{_taskName}: Restarting => Restart failed ");
                    return false;
                };
            }
            else
            {
                _log.InfoLog($"{_taskName}: Restarting => Restart failed ");
                return false;
            }
        }
    }
    public class RecordTaskOnChange : AbstractTask
    {
        private IPointMapping _pointMapping;
        private RecordItemConfig _recordItemConfig;
        private Timer.Timer _timeRecord;
        private ITagRecord _tagRecord;
        private ConcurrentQueue<ITag> _recoredTagsQueue;
        private List<string> _pointNames;
        private List<PointNameIndex> _pointNamesCollection;
        private ILog _log;
        [Dependency]
        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        public RecordTaskOnChange(IPointMapping pointMapping, RecordItemConfig recordItemConfig, ILog log)
        {
            _pointMapping = pointMapping;
            _recordItemConfig = recordItemConfig;
            _log = log;
            _taskName = recordItemConfig.Name;
            _initLevel = 4;
        }
        public override bool OnInit()
        {
            bool result = false;
            _log.InfoLog($"{_taskName}: Init => Initing ");
            try
            {
                int timeSpan = 1000;
                if (_recordItemConfig.TimeSpan>1000)
                {
                    timeSpan = _recordItemConfig.TimeSpan;
                }
                _timeRecord = new Timer.Timer(timeSpan);
                _timeRecord.Elapsed += _timeRecord_Elapsed; ;
                _timeRecord.AutoReset = true;
                _recoredTagsQueue = new ConcurrentQueue<ITag>();
                _pointNamesCollection = new List<PointNameIndex>();
                _tagRecord = new LogTagSerivce();
                _pointNames = new List<string>();
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
            subscribePoint();
            _timeRecord.Start();
            _log.InfoLog($"{_taskName}: Starting=>Started ");
            //_log.InfoLog($"{_taskName}: Time start: {DateTime.Now} ");

            return true;
        }

        void subscribePoint()
        {
            foreach (var tagName in _recordItemConfig.TagNames)
            {
                var pointNameGroup = StringHandler.SplitEndWith(tagName);
                string pointName;
                int index;
                if (pointNameGroup.Length > 1)
                {
                     pointName = pointNameGroup[0];
                     index = int.Parse(pointNameGroup[1]);
                }
                else
                {
                    pointName = pointNameGroup[0];
                    index = -1;
                }
                _pointNamesCollection.Add(new PointNameIndex(pointName, index));

                ///防止重复订阅同一点位变化
                if (!_pointNames.Exists(s => s == pointName))
                {
                    _pointNames.Add(pointName);
                }
                else
                {
                    continue;
                }

                var pointMete = _pointMapping.GetPointMetaData(pointName);
                switch (pointMete.ValueType)
                {
                    case DataType.Bool:
                        var boolPoint = _pointMapping.GetBoolPoint(pointName);
                        boolPoint.UpdataEvent += BoolPoint_UpdataEvent;
                        break;
                    case DataType.Byte:
                        var bytePoint = _pointMapping.GetBytePoint(pointName);
                        bytePoint.UpdataEvent += BytePoint_UpdataEvent;
                        break;
                    case DataType.Short:
                        var shortPoint = _pointMapping.GetShortPoint(pointName);
                        shortPoint.UpdataEvent += ShortPoint_UpdataEvent; 
                        break;
                    case DataType.UShort:
                        var ushortPoint= _pointMapping.GetUShortPoint(pointName);
                        ushortPoint.UpdataEvent += UshortPoint_UpdataEvent;

                        break;
                    case DataType.Int:
                        var intPoint = _pointMapping.GetIntPoint(pointName);
                        intPoint.UpdataEvent += IntPoint_UpdataEvent;
                        break;
                    case DataType.UInt:
                        var uintPoint = _pointMapping.GetUIntPoint(pointName);
                        uintPoint.UpdataEvent += UintPoint_UpdataEvent;
                        break;
                    case DataType.Float:
                        var floatPoint = _pointMapping.GetFloatPoint(pointName);
                        floatPoint.UpdataEvent += FloatPoint_UpdataEvent;
                        break;
                    case DataType.String:
                        var stringPoint = _pointMapping.GetStringPoint(pointName);
                        stringPoint.UpdataEvent += StringPoint_UpdataEvent;
                        break;
                    default:
                        break;
                }
            }
        }
        void cancenlSubscribePoint()
        {
            foreach (var pointName in _pointNames)
            {
                var pointMete = _pointMapping.GetPointMetaData(pointName);
                switch (pointMete.ValueType)
                {
                    case DataType.Bool:
                        var boolPoint = _pointMapping.GetBoolPoint(pointName);
                        boolPoint.UpdataEvent -= BoolPoint_UpdataEvent;
                        break;
                    case DataType.Byte:
                        var bytePoint = _pointMapping.GetBytePoint(pointName);
                        bytePoint.UpdataEvent -= BytePoint_UpdataEvent;
                        break;
                    case DataType.Short:
                        var shortPoint = _pointMapping.GetShortPoint(pointName);
                        shortPoint.UpdataEvent -= ShortPoint_UpdataEvent;
                        break;
                    case DataType.UShort:
                        var ushortPoint = _pointMapping.GetUShortPoint(pointName);
                        ushortPoint.UpdataEvent -= UshortPoint_UpdataEvent;

                        break;
                    case DataType.Int:
                        var intPoint = _pointMapping.GetIntPoint(pointName);
                        intPoint.UpdataEvent -= IntPoint_UpdataEvent;
                        break;
                    case DataType.UInt:
                        var uintPoint = _pointMapping.GetUIntPoint(pointName);
                        uintPoint.UpdataEvent -= UintPoint_UpdataEvent;
                        break;
                    case DataType.Float:
                        var floatPoint = _pointMapping.GetFloatPoint(pointName);
                        floatPoint.UpdataEvent -= FloatPoint_UpdataEvent;
                        break;
                    case DataType.String:
                        var stringPoint = _pointMapping.GetStringPoint(pointName);
                        stringPoint.UpdataEvent -= StringPoint_UpdataEvent;
                        break;
                    default:
                        break;
                }
            }
        }
        private void StringPoint_UpdataEvent(IPoint<string> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp=DateTime.Now,
                    Type=DataType.String
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void FloatPoint_UpdataEvent(IPoint<float> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp = DateTime.Now,
                    Type = DataType.Float
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void UintPoint_UpdataEvent(IPoint<uint> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp = DateTime.Now,
                    Type = DataType.UInt
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void IntPoint_UpdataEvent(IPoint<int> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp = DateTime.Now,
                    Type = DataType.Int
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void UshortPoint_UpdataEvent(IPoint<ushort> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp = DateTime.Now,
                    Type = DataType.UShort
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void ShortPoint_UpdataEvent(IPoint<short> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp = DateTime.Now,
                    Type = DataType.Short
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void BytePoint_UpdataEvent(IPoint<byte> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp = DateTime.Now,
                    Type = DataType.Byte
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void BoolPoint_UpdataEvent(IPoint<bool> point, int index)
        {
            if (_pointNamesCollection.Exists(s => s.PointName == point.Name && (s.Index == -1 || s.Index == index)))
            {
                var value = point.GetValue(index);
                var quality = point.GetQuality();
                var tag = new Tag()
                {
                    Name = point.Name + $"[{index}]",
                    Quality = quality,
                    Value = value.ToString(),
                    TimeStamp = DateTime.Now,
                    Type = DataType.Bool
                };
                _recoredTagsQueue.Enqueue(tag);
            }
        }

        private void _timeRecord_Elapsed(object sender, Timer.ElapsedEventArgs e)
        {
            _log.InfoLog($"{_taskName}: _timeRecord_Elapsed inovke : {DateTime.Now} ");

            var t = sender as Timer.Timer;
            t.Stop();
            List<ITag> recordTags = new List<ITag>();
            while (!_recoredTagsQueue.IsEmpty)
            {
                _recoredTagsQueue.TryDequeue(out ITag tag);
                recordTags.Add(tag);
            }
            _tagRecord.Insert(recordTags);
            _log.InfoLog($"{_taskName}: _timeRecord_Elapsed  end: {DateTime.Now} ");

            t.Start();
        }


        public override bool OnStop()
        {
            _log.InfoLog($"{_taskName}: Stop=>Stopping");
            _timeRecord.Stop();
            cancenlSubscribePoint();
            _log.InfoLog($"{_taskName}: Stopping=>Stopped");
            return true;
        }

        public override bool Restart()
        {
            _log.InfoLog($"{_taskName}: Restart => Restarting ");
            OnStop();
            if (OnInit())
            {
                if (OnStart())
                {
                    _log.InfoLog($"{_taskName}: Restarting => Restarted ");
                    return true;
                }
                else
                {
                    _log.InfoLog($"{_taskName}: Restarting => Restart failed ");
                    return false;
                };
            }
            else
            {
                _log.InfoLog($"{_taskName}: Restarting => Restart failed ");
                return false;
            }
        }
    }
}
