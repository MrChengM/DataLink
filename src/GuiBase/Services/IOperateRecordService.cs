using System;
using DataServer.Log;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Models;

namespace GuiBase.Services
{
    public interface IOperateRecordService
    {
        List<OperateRecord> Select(OperateRecordSelectConditionWrapper conditionWrapper);
        bool Insert(string transcode, string message);

    }
}
