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
    public class OperateRecordController : ApiController
    {

        private IOperateRecordCRUD _recordCRUD;

        public OperateRecordController(IOperateRecordCRUD recordCRUD)
        {
            _recordCRUD = recordCRUD;
        }
        [BasicAuthenticationFilter]
        [HttpGet]
        public List<OperateRecord> Get()
        {
           return _recordCRUD.Select(DateTime.Now - TimeSpan.FromDays(1), DateTime.Now);
        }
        [BasicAuthenticationFilter]
        [HttpGet]
        public List<OperateRecord> Get(string userName)
        {
            return _recordCRUD.Select(userName);
        }
        //[BasicAuthenticationFilter]
        //[HttpPut]
        //public List<OperateRecord> Put([FromBody] DateTime startTime, [FromBody] DateTime endTime)
        //{
        //    return _recordCRUD.Select(startTime,endTime);

        //}
        [BasicAuthenticationFilter]
        [ActionName("Record")]
        [HttpPut]
        public List<OperateRecord> Put([FromBody]OperateRecordSelectCondition selectCondition)
        {
            return _recordCRUD.Select(selectCondition);

        }
        [BasicAuthenticationFilter]
        [ActionName("Record")]
        [HttpPost]
        public bool CreateRecord([FromBody] OperateRecord record)
        {
            if (_recordCRUD.Insert(record) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
