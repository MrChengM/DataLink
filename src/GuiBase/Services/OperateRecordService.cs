using DataServer.Log;
using GuiBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Communication;

namespace GuiBase.Services
{
    public class OperateRecordService : IOperateRecordService
    {
        private readonly string ServerUrl = "http://localhost:3051/api/OperateRecord";
        private ILog _log;
        private string _taskName = "OperateRecordService";
        private ISecurityService _securityService;
        private string _computerInfor;
        
        public OperateRecordService(ILog log, ISecurityService securityService)
        {
            _log = log;
            _securityService = securityService;
            _computerInfor = $"MachineName:'{ Environment.MachineName}'; UserName:'{Environment.UserName}';";
        }
        public bool Insert(string transcode,  string message)
        {
            var result = false;
            var url = ServerUrl + "/Record";
            var currentUser = _securityService.GetCurrentUser();
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            var record = new OperateRecord()
            {
                UserName = currentUser.Account,
                Id = currentUser.Id,
                ComputerInfor = _computerInfor,
                Time = DateTime.Now,
                Transcode= transcode,
                Message = message
            };
            try
            {
                result = RestAPIClient.PostFuncJson<OperateRecord, bool>(url, record, aut);
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: { ServerUrl} insert records error':{e.Message}'!");
            }
            return result;
        }

        public List<OperateRecord> Select(OperateRecordSelectConditionWrapper conditionWrapper)
        {
            List<OperateRecord> result = new List<OperateRecord>();
            var url = ServerUrl + "/Record";
            var currentUser = _securityService.GetCurrentUser();
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            try
            {
                result = RestAPIClient.PutFuncJson<OperateRecordSelectCondition, List<OperateRecord>>(url, conditionWrapper.Convert(), aut);
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: { ServerUrl} select records error':{e.Message}'!");
            }
            return result;
        }
    }
}
