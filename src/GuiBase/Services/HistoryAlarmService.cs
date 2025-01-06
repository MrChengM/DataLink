using DataServer;
using DataServer.Log;
using DataServer.Alarm;
using GuiBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Communication;

namespace GuiBase.Services
{
    public class HistoryAlarmService : IHistoryAlarmService
    {
        private readonly string ServerUrl = "http://localhost:3051/api/HistoryAlarms";
        private ILog _log;
        private string _taskName = "HistoryAlarmService";
        public HistoryAlarmService(ILog log)
        {
            _log = log;
        }
        public List<HistoryAlarmWrapper> Select(HistoryAlarmSelectConditionWrapper conditionWrapper)
        {
            try
            {
                var url = ServerUrl + "/Put";
                var result = new List<HistoryAlarmWrapper>();
                var hisAlarms = RestAPIClient.PutFuncJson<HistoryAlarmSelectConditionWrapper, List<HistoryAlarm>>(url, conditionWrapper);
                foreach (var hisAlarm in hisAlarms)
                {
                    result.Add(HistoryAlarmWrapper.Convert(hisAlarm));
                }

                return result;
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: Post { ServerUrl} error':{e.Message}'!");

                return null;
            }


        }
    }
}
