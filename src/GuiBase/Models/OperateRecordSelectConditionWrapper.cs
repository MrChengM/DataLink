using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Log;
using Prism.Mvvm;

namespace GuiBase.Models
{
    public class OperateRecordSelectConditionWrapper : BindableBase
    {

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value, "UserName"); }
        }

        private DateTime startTime= DateTime.Now - new TimeSpan(1, 0, 0, 0);

        public DateTime StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value, "StartTime"); }
        }

        private DateTime endTime= DateTime.Now;

        public DateTime EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value, "EndTime"); }
        }

        public OperateRecordSelectCondition Convert()
        {
            return new OperateRecordSelectCondition()
            {
                UserName = UserName,
                StartTime = StartTime,
                EndTime = EndTime
            };
        }
    }
}
