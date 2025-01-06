using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Log;
using dbm=DBHandler_EF.Modules;

namespace DBHandler_EF.Services
{
    public class OperateRecoredSerivce : IOperateRecordCRUD
    {
        public int Insert(OperateRecord record)
        {
            int result;
            using (var content = new dbm.OperateRecordContent())
            {
                content.OperateRecords.Add(Convert(record));
                result = content.SaveChanges();
            }
            return result;
        }

        public int Insert(List<OperateRecord> records)
        {
            int result;
            using (var content = new dbm.OperateRecordContent())
            {
                foreach (var record in records)
                {
                    content.OperateRecords.Add(Convert(record));
                }
                result = content.SaveChanges();
            }
            return result;
        }

        public List<OperateRecord> Select(string userName)
        {
            List<OperateRecord> result = new List<OperateRecord>();
            using (var content = new dbm.OperateRecordContent())
            {
                var records = from a in content.OperateRecords
                                where a.UserName.Contains(userName)
                                select a;
                foreach (var record in records)
                {
                    result.Add(Convert(record));
                }
            }
            return result;
        }

        public List<OperateRecord> Select(DateTime startTime, DateTime endTime)
        {
            List<OperateRecord> result = new List<OperateRecord>();
            using (var content = new dbm.OperateRecordContent())
            {
                var records = from a in content.OperateRecords
                              where a.Time > startTime && a.Time < endTime
                              select a;
                foreach (var record in records)
                {
                    result.Add(Convert(record));
                }
            }
            return result;
        }

        public List<OperateRecord> Select(string userName, DateTime startTime, DateTime endTime)
        {
            List<OperateRecord> result = new List<OperateRecord>();
            using (var content = new dbm.OperateRecordContent())
            {
                var records = from a in content.OperateRecords
                              where a.UserName.Contains(userName) && a.Time > startTime && a.Time < endTime
                              select a;
                foreach (var record in records)
                {
                    result.Add(Convert(record));
                }
            }
            return result;
        }
        public List<OperateRecord> Select(OperateRecordSelectCondition selectCondition)
        {
            if (selectCondition.UserName==null)
            {
                return Select(selectCondition.StartTime, selectCondition.EndTime);
            }
            else
            {
                return Select(selectCondition.UserName, selectCondition.StartTime, selectCondition.EndTime);
            }
        }
        public OperateRecord Convert(dbm.OperateRecord dRecord)
        {
            return new OperateRecord()
            {
                UserName = dRecord.UserName,
                Id = dRecord.Id,
                Time = dRecord.Time,
                ComputerInfor = dRecord.ComputerInfor,
                Message = dRecord.Message,
                Transcode=dRecord.Transcode,
            };
        }
        public dbm.OperateRecord Convert(OperateRecord record)
        {
            return new dbm.OperateRecord()
            {
                UserName = record.UserName,
                Id = record.Id,
                Time = record.Time,
                ComputerInfor = record.ComputerInfor,
                Message = record.Message,
                Transcode = record.Transcode,
            };
        }
    }
}
