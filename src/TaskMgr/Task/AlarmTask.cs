using DataServer.Config;
using DataServer.Points;
using DataServer.Alarm;
using DataServer.Task;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using DataServer;
using DataServer.Log;
using DBHandler_EF.Services;
using Utillity.Data;
using Unity;

namespace TaskMgr.Task
{
    public class AlarmTask : AbstractTask,IAlarmTask
    {
        private IPointMapping _pointMapping;
        private AlarmsConfig _alarmsConfig;
        private Timer _timeRecord;
        private IHisAlarmRecordCRUD _hisAlarmRecord;
        private ConcurrentQueue<HistoryAlarm> _historyAlarmQueue;
        private Dictionary<string,AlarmInstance> _alarmInstanceDic;
        private Dictionary<string, IPoint<bool>> _alarmPointDic;
        private List<string> _pointNames;
        private ILog _log;

        public AlarmsConfig AlarmsConfig { get { return _alarmsConfig; } set { _alarmsConfig = value; } }

        private Dictionary<string,AlarmPointCondition> _alarmPointConditionDic;
        public AlarmTask(IPointMapping pointMapping, IHisAlarmRecordCRUD hisAlarmRecord, ILog log)
        {
            _pointMapping = pointMapping;
            _hisAlarmRecord = hisAlarmRecord;
            _log = log;
            _taskName = "AlarmServer";
            _initLevel = 4;
        }
        //public AlarmTask(IPointMapping pointMapping,  ILog log)
        //{
        //    _pointMapping = pointMapping;
        //    //_hisAlarmRecord = hisAlarmRecord;
        //    _log = log;
        //    _taskName = "AlarmServer";
        //    _initLevel = 4;
        //}
        public override bool OnInit()
        {

            bool result = false;
            _log.InfoLog($"{_taskName}: Init => Initing ");
            //try
            //{
            int timeSpan = 3000;
            _timeRecord = new Timer(timeSpan);
            _timeRecord.Elapsed += _timeRecord_Elapsed; ;
            _timeRecord.AutoReset = true;

            InitAlarmDictionary();
            _historyAlarmQueue = new ConcurrentQueue<HistoryAlarm>();
            //_hisAlarmRecord =new LogHistoryAlarmSerivce();
            _log.InfoLog($"{_taskName}: Initing=>Inited");
            result = true;
            //}
            //catch (Exception e)
            //{
            //    _log.ErrorLog($"{_taskName}: Init error '{e.Message}'");
            //}
            return result;
        }

        void InitAlarmDictionary()
        {
            _alarmInstanceDic = new Dictionary<string, AlarmInstance>();
            _alarmPointDic = new Dictionary<string, IPoint<bool>>();
            _alarmPointConditionDic = new Dictionary<string, AlarmPointCondition>();
            _pointNames = new List<string>();
            foreach (var alarmItem in _alarmsConfig.AlarmGroup)
            {
                AlarmInstance alarmInstance = Convert(alarmItem.Value);
                _alarmInstanceDic.Add(alarmInstance.Name, alarmInstance);
                IPoint<bool> alarmPoint = new VirtulPoint<bool>(alarmItem.Key, false);
                alarmPoint.UpdataEvent += AlarmPoint_UpdataEvent;

                _pointMapping.Register(alarmPoint.Name, alarmPoint);
                _alarmPointDic.Add(alarmPoint.Name, alarmPoint);


                var pointNameGroup = StringHandler.SplitEndWith(alarmItem.Value.TagName);
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
                _alarmPointConditionDic.Add(alarmPoint.Name, new AlarmPointCondition() { Name = alarmPoint.Name, ParentPointName = pointName, ParentPointIndex = index, Condition = alarmItem.Value.ConditionType, ConditionValue = alarmItem.Value.ConditionValue });
                ///防止重复订阅同一点位变化
                if (!_pointNames.Exists(s => s == pointName))
                {
                    _pointNames.Add(pointName);
                }
            }

        }

        private void AlarmPoint_UpdataEvent(IPoint<bool> point, int index)
        {
            if (_alarmInstanceDic.TryGetValue(point.Name, out AlarmInstance instance))
            {
                if (point[0])
                {
                    instance.IsEnable = true;
                    instance.IsCheck = false;
                    instance.Count++;
                    instance.AppearTime = DateTime.Now;
                    AlarmStatusChangeEvent?.Invoke(instance, AlarmRefresh.Updata);
                }
                else
                {
                    instance.IsEnable = false;
                    instance.EndTime = DateTime.Now;
                    if (instance.ConfirmMode == ConfirmMode.Auto||instance.IsCheck)
                    {
                        instance.Count = 0;
                        AlarmStatusChangeEvent?.Invoke(instance, AlarmRefresh.Remove);
                    }
                    else
                    {
                        AlarmStatusChangeEvent?.Invoke(instance, AlarmRefresh.Updata);
                    }
                    _historyAlarmQueue.Enqueue(Convert(instance));
                }

            }
        }
        public void AlarmConfrim(string alarmName)
        {
            if (_alarmInstanceDic.ContainsKey(alarmName))
            {
                var instance = _alarmInstanceDic[alarmName];
                instance.IsCheck = true;
                if (instance.IsEnable)
                {
                    AlarmStatusChangeEvent?.Invoke(instance, AlarmRefresh.Updata);
                }
                else
                {
                    AlarmStatusChangeEvent?.Invoke(instance, AlarmRefresh.Remove);

                }
            }

        }
        public event Action<AlarmInstance, AlarmRefresh> AlarmStatusChangeEvent;
        HistoryAlarm Convert(AlarmInstance alarmInstance)
        {
            return new HistoryAlarm()
            {
                Name = alarmInstance.Name,
                PartName = alarmInstance.PartName,
                AlarmDesc = alarmInstance.AlarmDesc,
                AlarmLevel = alarmInstance.AlarmLevel,
                AlarmNumber = alarmInstance.AlarmNumber,
                L1View = alarmInstance.L1View,
                L2View = alarmInstance.L2View,
                AlarmGroup = alarmInstance.AlarmGroup,
                AppearTime = alarmInstance.AppearTime,
                EndTime = alarmInstance.EndTime
            };
        }

        AlarmInstance Convert(AlarmItemConfig alarmItemConfig)
        {
            return new AlarmInstance()
            {
                Name = alarmItemConfig.AlarmTag,
                IsEnable = false,
                IsCheck = false,
                PartName = alarmItemConfig.PartName,
                AlarmDesc = alarmItemConfig.AlarmDescription,
                AlarmLevel = alarmItemConfig.AlarmType,
                AlarmNumber = alarmItemConfig.ALNumber,
                L1View = alarmItemConfig.Level1View,
                L2View = alarmItemConfig.Level2View,
                AlarmGroup = alarmItemConfig.AlarmGroup,
            };
        }
        void subscribePoint(string pointName)
        {
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
                    var ushortPoint = _pointMapping.GetUShortPoint(pointName);
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
        void cancenlSubscribePoint(string pointName)
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
        private void StringPoint_UpdataEvent(IPoint<string> point, int index)
        {
           
        }

        private void FloatPoint_UpdataEvent(IPoint<float> point, int index)
        {
            var alarmConditions = from s in _alarmPointConditionDic
                                  where s.Value.ParentPointName == point.Name
                                  select s.Value;
            foreach (var a in alarmConditions)
            {
                if (a.ParentPointIndex == index || (a.ParentPointIndex == -1 && index == 0))
                {
                    var alarmPoint = _alarmPointDic[a.Name];
                    var data = point.GetValue(index);
                    switch (a.Condition)
                    {
                        case ConditionType.Bit:
                            alarmPoint.SetValue(NetConvert.GetBit(System.Convert.ToInt32(data), System.Convert.ToInt32(a.ConditionValue)),0);
                            break;
                        case ConditionType.MoreThan:
                            alarmPoint.SetValue(data > a.ConditionValue, 0);
                            break;
                        case ConditionType.LessThan:
                            alarmPoint.SetValue(data < a.ConditionValue, 0);
                            break;
                        case ConditionType.Equals:
                            alarmPoint.SetValue(data == a.ConditionValue, 0);
                            break;
                        case ConditionType.NotEquals:
                            alarmPoint.SetValue(data != a.ConditionValue, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void UintPoint_UpdataEvent(IPoint<uint> point, int index)
        {
            var alarmConditions = from s in _alarmPointConditionDic
                                  where s.Value.ParentPointName == point.Name
                                  select s.Value;
            foreach (var a in alarmConditions)
            {
                if (a.ParentPointIndex == index || (a.ParentPointIndex == -1 && index == 0))
                {
                    var alarmPoint = _alarmPointDic[a.Name];
                    var data = point.GetValue(index);
                    switch (a.Condition)
                    {
                        case ConditionType.Bit:
                            alarmPoint.SetValue(NetConvert.GetBit(System.Convert.ToInt32(data), System.Convert.ToInt32(a.ConditionValue)), 0);
                            break;
                        case ConditionType.MoreThan:
                            alarmPoint.SetValue(data > a.ConditionValue, 0);
                            break;
                        case ConditionType.LessThan:
                            alarmPoint.SetValue(data < a.ConditionValue, 0);
                            break;
                        case ConditionType.Equals:
                            alarmPoint.SetValue(data == a.ConditionValue, 0);
                            break;
                        case ConditionType.NotEquals:
                            alarmPoint.SetValue(data != a.ConditionValue, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void IntPoint_UpdataEvent(IPoint<int> point, int index)
        {
            var alarmConditions = from s in _alarmPointConditionDic
                                  where s.Value.ParentPointName == point.Name
                                  select s.Value;
            foreach (var a in alarmConditions)
            {
                if (a.ParentPointIndex == index || (a.ParentPointIndex == -1 && index == 0))
                {
                    var alarmPoint = _alarmPointDic[a.Name];
                    var data = point.GetValue(index);
                    switch (a.Condition)
                    {
                        case ConditionType.Bit:
                            alarmPoint.SetValue(NetConvert.GetBit(data, System.Convert.ToInt32(a.ConditionValue)), 0);
                            break;
                        case ConditionType.MoreThan:
                            alarmPoint.SetValue(data > a.ConditionValue, 0);
                            break;
                        case ConditionType.LessThan:
                            alarmPoint.SetValue(data < a.ConditionValue, 0);
                            break;
                        case ConditionType.Equals:
                            alarmPoint.SetValue(data == a.ConditionValue, 0);
                            break;
                        case ConditionType.NotEquals:
                            alarmPoint.SetValue(data != a.ConditionValue, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void UshortPoint_UpdataEvent(IPoint<ushort> point, int index)
        {
            var alarmConditions = from s in _alarmPointConditionDic
                                  where s.Value.ParentPointName == point.Name
                                  select s.Value;
            foreach (var a in alarmConditions)
            {
                if (a.ParentPointIndex == index || (a.ParentPointIndex == -1 && index == 0))
                {
                    var alarmPoint = _alarmPointDic[a.Name];
                    var data = point.GetValue(index);
                    switch (a.Condition)
                    {
                        case ConditionType.Bit:
                            alarmPoint.SetValue(NetConvert.GetBit(System.Convert.ToInt32(data), System.Convert.ToInt32(a.ConditionValue)), 0);
                            break;
                        case ConditionType.MoreThan:
                            alarmPoint.SetValue(data > a.ConditionValue, 0);
                            break;
                        case ConditionType.LessThan:
                            alarmPoint.SetValue(data < a.ConditionValue, 0);
                            break;
                        case ConditionType.Equals:
                            alarmPoint.SetValue(data == a.ConditionValue, 0);
                            break;
                        case ConditionType.NotEquals:
                            alarmPoint.SetValue(data != a.ConditionValue, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void ShortPoint_UpdataEvent(IPoint<short> point, int index)
        {
            var alarmConditions = from s in _alarmPointConditionDic
                                  where s.Value.ParentPointName == point.Name
                                  select s.Value;
            foreach (var a in alarmConditions)
            {
                if (a.ParentPointIndex == index || (a.ParentPointIndex == -1 && index == 0))
                {
                    var alarmPoint = _alarmPointDic[a.Name];
                    var data = point.GetValue(index);
                    switch (a.Condition)
                    {
                        case ConditionType.Bit:
                            alarmPoint.SetValue(NetConvert.GetBit(System.Convert.ToInt32(data), System.Convert.ToInt32(a.ConditionValue)), 0);
                            break;
                        case ConditionType.MoreThan:
                            alarmPoint.SetValue(data > a.ConditionValue, 0);
                            break;
                        case ConditionType.LessThan:
                            alarmPoint.SetValue(data < a.ConditionValue, 0);
                            break;
                        case ConditionType.Equals:
                            alarmPoint.SetValue(data == a.ConditionValue, 0);
                            break;
                        case ConditionType.NotEquals:
                            alarmPoint.SetValue(data != a.ConditionValue, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void BytePoint_UpdataEvent(IPoint<byte> point, int index)
        {
            var alarmConditions = from s in _alarmPointConditionDic
                                  where s.Value.ParentPointName == point.Name
                                  select s.Value;
            foreach (var a in alarmConditions)
            {
                if (a.ParentPointIndex == index || (a.ParentPointIndex == -1 && index == 0))
                {
                    var alarmPoint = _alarmPointDic[a.Name];
                    var data = point.GetValue(index);
                    switch (a.Condition)
                    {
                        case ConditionType.Bit:
                            alarmPoint.SetValue(NetConvert.GetBit(System.Convert.ToInt32(data), System.Convert.ToInt32(a.ConditionValue)), 0);
                            break;
                        case ConditionType.MoreThan:
                            alarmPoint.SetValue(data > a.ConditionValue, 0);
                            break;
                        case ConditionType.LessThan:
                            alarmPoint.SetValue(data < a.ConditionValue, 0);
                            break;
                        case ConditionType.Equals:
                            alarmPoint.SetValue(data == a.ConditionValue, 0);
                            break;
                        case ConditionType.NotEquals:
                            alarmPoint.SetValue(data != a.ConditionValue, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void BoolPoint_UpdataEvent(IPoint<bool> point, int index)
        {
            var alarmConditions = from s in _alarmPointConditionDic
                                  where s.Value.ParentPointName == point.Name
                                  select s.Value;
            foreach (var a in alarmConditions)
            {
                if (a.ParentPointIndex == index || (a.ParentPointIndex == -1 && index == 0))
                {
                    var alarmPoint = _alarmPointDic[a.Name];
                    var data = point.GetValue(index);
                    switch (a.Condition)
                    {
                        case ConditionType.Bit:
                            //alarmPoint.SetValue(NetConvert.IntToBool(System.Convert.ToInt32(data), System.Convert.ToInt32(a.ConditionValue)), 0);
                            break;
                        case ConditionType.MoreThan:
                            //alarmPoint.SetValue(data > a.ConditionValue, 0);
                            break;
                        case ConditionType.LessThan:
                            //alarmPoint.SetValue(data < a.ConditionValue, 0);
                            break;
                        case ConditionType.Equals:
                            alarmPoint.SetValue(data, 0);
                            break;
                        case ConditionType.NotEquals:
                            alarmPoint.SetValue(!data, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void _timeRecord_Elapsed(object sender, ElapsedEventArgs e)
        {
            var t = sender as Timer;
            t.Stop();
            List<HistoryAlarm> historyAlarms = new List<HistoryAlarm>();
            while (!_historyAlarmQueue.IsEmpty)
            {
                _historyAlarmQueue.TryDequeue(out HistoryAlarm   historyAlarm);
                historyAlarms.Add(historyAlarm);
            }
            _hisAlarmRecord.Insert(historyAlarms);
            t.Start();
        }
        public override bool OnStart()
        {
            _log.InfoLog($"{_taskName}: Start=>Starting ");
            foreach (var pointName in _pointNames)
            {
                subscribePoint(pointName);
            }
            _timeRecord.Start();
            _log.InfoLog($"{_taskName}: Starting=>Started ");
            return true;
        }

        public override bool OnStop()
        {

            _log.InfoLog($"{_taskName}: Stop=>Stopping");
            _timeRecord.Stop();
            foreach (var pointName in _pointNames)
            {
                cancenlSubscribePoint(pointName);
            }

            foreach (var alarmPoint in _alarmPointDic)
            {
                alarmPoint.Value.UpdataEvent -= AlarmPoint_UpdataEvent;
            }
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
        public List<AlarmInstance> GetExitAlarms()
        {
            var result = new List<AlarmInstance>();
            foreach (var item in _alarmInstanceDic)
            {
                if (item.Value.Count > 0)
                    result.Add(item.Value);
            }
            return result;
        }

       
    }

    internal class AlarmPointCondition
    {
        public string Name  { get; set; }

        public string ParentPointName { get; set; }

        public int ParentPointIndex { get; set; }

        public ConditionType Condition { get; set; }

        public float ConditionValue { get; set; }
    }
    
}
