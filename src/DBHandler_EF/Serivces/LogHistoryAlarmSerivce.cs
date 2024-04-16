using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Alarm;
using DBM= DBHandler_EF.Modules;

namespace DBHandler_EF.Serivces
{
    public class LogHistoryAlarmSerivce : IHisAlarmRecord
    {
        public int Delete(string alarmName)
        {
            int result;
            using (var hisContent = new DBM.DataLinkContent())
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
            using (var hisContent = new DBM.DataLinkContent())
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
            using (var hisContent = new DBM.DataLinkContent())
            {
                hisContent.HistoryAlarms.Add(Convert(historyAlarm));
                result = hisContent.SaveChanges();
            }
            return result;
        }

        public int Insert(List<HistoryAlarm> historyAlarms)
        {
            int result;
            using (var hisContent = new DBM.DataLinkContent())
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
            using (var hisContent = new DBM.DataLinkContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where a.AlarmName == alarmName 
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
            using (var hisContent = new DBM.DataLinkContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where a.AlarmName == alarmName && a.AppearTime > startTime && a.EndTime < endTime
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
            using (var hisContent = new DBM.DataLinkContent())
            {
                var hisAlarms = from a in hisContent.HistoryAlarms
                                where a.AppearTime > startTime && a.EndTime < endTime
                                select a;
                foreach (var hisAlarm in hisAlarms)
                {
                    result.Add(Convert(hisAlarm));
                }
            }
            return result;
        }

        public HistoryAlarm Convert(DBM.HistoryAlarm dhistoryAlarm)
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
        public DBM.HistoryAlarm Convert(HistoryAlarm historyAlarm)
        {
            return new DBM.HistoryAlarm()
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
