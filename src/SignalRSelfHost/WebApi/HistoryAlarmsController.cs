using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DataServer.Alarm;
using DataServer.Log;
namespace SignalRSelfHost.WebApi
{
    public class HistoryAlarmsController : ApiController
    {

        private IHisAlarmRecordCRUD _hisAlarmRecord;

        public HistoryAlarmsController(IHisAlarmRecordCRUD hisAlarmRecord)
        {
            _hisAlarmRecord = hisAlarmRecord;
        }
        // GET api/values 
        public List<HistoryAlarm> Get()
        {
           return _hisAlarmRecord.Select(DateTime.Now - TimeSpan.FromDays(1), DateTime.Now);
        }

        // GET api/values/5 
        public List<HistoryAlarm> Get(string alarmName)
        {
            return _hisAlarmRecord.Select( alarmName);
        }

        // POST api/values 
        //public List<HistoryAlarm> Post([FromBody] HistoryAlarmSelectCondition selectCondition)
        //{
        //    return _hisAlarmRecord.Select(selectCondition);
        //}

        // PUT api/values/5 
        public List<HistoryAlarm> Put([FromBody] HistoryAlarmSelectCondition selectCondition)
        {
            return _hisAlarmRecord.Select(selectCondition);

        }

        //// DELETE api/values/5 
        //public void Delete(int id)
        //{
        //}
    }
}
