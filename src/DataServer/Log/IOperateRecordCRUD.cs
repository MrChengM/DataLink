using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Log
{
    public interface IOperateRecordCRUD
    { 
        int Insert(OperateRecord record);
        
        int Insert(List<OperateRecord> records);
        
       
        List<OperateRecord> Select(string userName);
        
        List<OperateRecord> Select(DateTime startTime, DateTime endTime);
       
        List<OperateRecord> Select(string userName,DateTime startTime, DateTime endTime);
        List<OperateRecord> Select(OperateRecordSelectCondition selectCondition);

    }

    public class OperateRecord
    {
        public string UserName { get; set; }

        public string Id { get; set; }

        public DateTime Time { get; set; }

        public string ComputerInfor { get; set; }
      
        public string Message { get; set; }
        public string Transcode { get; set; }
    }
    public class OperateRecordSelectCondition
    {
        public string UserName { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

    }
}
