using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Alarm;
using DBHandler_EF.Modules;
using DataServer.Log;

namespace DBHandler_EF.Services
{
    public class LogHistoryAlarmSerivce : IHisAlarmRecordCRUD
    {
        public int Delete(string alarmName)
        {
            int result;
            using (var hisContent = new LogContent())
            {
                var hisAlarms= from a in hisContent.HistoryAlarms
                                       where a.AlarmName == alarmName
                                       select a;
                hisContent.HistoryAlarms.RemoveRange(hisAlarms);
                result = hisContent.SaveChanges();
            }
            return result;
        }

        public int Delete(string alarmName, DateTime startTime, DateTime endTime)
        {
            int result;
            using (var hisContent = new LogContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where a.AlarmName == alarmName && a.AppearTime > startTime && a.EndTime < endTime
                                select a;
                hisContent.HistoryAlarms.RemoveRange(hisAlarms);
                result = hisContent.SaveChanges();
            }
            return result;
        }

        public int Insert(HistoryAlarm historyAlarm)
        {
            int result;
            using (var hisContent = new LogContent())
            {
                hisContent.HistoryAlarms.Add(Convert(historyAlarm));
                result = hisContent.SaveChanges();
            }
            return result;
        }

        public int Insert(List<HistoryAlarm> historyAlarms)
        {
            int result;
            using (var hisContent = new LogContent())
            {
                foreach (var historyAlarm in historyAlarms)
                {
                    hisContent.HistoryAlarms.Add(Convert(historyAlarm));
                }
                result = hisContent.SaveChanges();
            }
            return result;
        }

        public List<HistoryAlarm> Select(string alarmName)
        {
            List<HistoryAlarm> result =new List<HistoryAlarm>();
            using (var hisContent = new LogContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where a.AlarmName.Contains(alarmName) 
                                select a;
                foreach (var hisAlarm in hisAlarms)
                {
                    result.Add(Convert(hisAlarm));
                }
            }
            return result;
        }

        public List<HistoryAlarm> Select(string alarmName, DateTime startTime, DateTime endTime)
        {
            List<HistoryAlarm> result = new List<HistoryAlarm>();
            using (var hisContent = new LogContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where a.AlarmName.Contains(alarmName) && ((a.AppearTime > startTime && a.AppearTime < endTime)|| (a.EndTime > startTime && a.EndTime < endTime))
                                select a;
                foreach (var hisAlarm in hisAlarms)
                {
                    result.Add(Convert(hisAlarm));
                }
            }
            return result;
        }

        public List<HistoryAlarm> Select(DateTime startTime, DateTime endTime)
        {
            List<HistoryAlarm> result = new List<HistoryAlarm>();
            using (var hisContent = new LogContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where ((a.AppearTime > startTime && a.AppearTime < endTime) || (a.EndTime > startTime && a.EndTime < endTime))
                                select a;
                foreach (var hisAlarm in hisAlarms)
                {
                    result.Add(Convert(hisAlarm));
                }
            }
            return result;
        }
        public List<HistoryAlarm> Select(HistoryAlarmSelectCondition selectCondition)
        {
            List<HistoryAlarm> result = new List<HistoryAlarm>();
            using (var hisContent = new LogContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where (selectCondition.AlarmName == null || a.AlarmName.Contains(selectCondition.AlarmName)) &&
                                (selectCondition.AlarmGroup == null || a.AlarmGroup == selectCondition.AlarmGroup) &&
                                (selectCondition.AlarmLevel == "All" || ((AlarmType)a.AlarmLevel).ToString() == selectCondition.AlarmLevel) &&
                                (selectCondition.L1View == null || a.L1View == selectCondition.L1View) &&
                                (selectCondition.L2View == null || a.L2View == selectCondition.L2View) &&
                                ((a.AppearTime > selectCondition.StartDate && a.AppearTime < selectCondition.EndDate) || (a.EndTime > selectCondition.StartDate && a.EndTime < selectCondition.EndDate))
                                select a;
                foreach (var hisAlarm in hisAlarms)
                {
                    result.Add(Convert(hisAlarm));
                }
            }
            return result;
        }
        public HistoryAlarm Convert(LogHistoryAlarm dhistoryAlarm)
        {
            return new HistoryAlarm()
            {
                Name=dhistoryAlarm.AlarmName,
                PartName=dhistoryAlarm.PartName,
                AlarmDesc=dhistoryAlarm.AlarmDescrible,
                AlarmLevel=(AlarmType)dhistoryAlarm.AlarmLevel,
                AlarmNumber=dhistoryAlarm.AlarmNumber,
                L1View=dhistoryAlarm.L1View,
                L2View=dhistoryAlarm.L2View,
                AlarmGroup=dhistoryAlarm.AlarmGroup,
                AppearTime=dhistoryAlarm.AppearTime,
                EndTime=dhistoryAlarm.EndTime

            };
        }
        public LogHistoryAlarm Convert(HistoryAlarm historyAlarm)
        {
            return new LogHistoryAlarm()
            {
                AlarmName = historyAlarm.Name,
                PartName = historyAlarm.PartName,
                AlarmDescrible = historyAlarm.AlarmDesc,
                AlarmLevel =(int)historyAlarm.AlarmLevel,
                AlarmNumber = historyAlarm.AlarmNumber,
                L1View = historyAlarm.L1View,
                L2View = historyAlarm.L2View,
                AlarmGroup = historyAlarm.AlarmGroup,
                AppearTime = historyAlarm.AppearTime,
                EndTime = historyAlarm.EndTime

            };
        }

    
    }
}
