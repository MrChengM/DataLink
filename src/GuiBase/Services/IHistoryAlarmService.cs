using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Models;

namespace GuiBase.Services
{
    public interface IHistoryAlarmService
    {
        List<HistoryAlarmWrapper> Select(HistoryAlarmSelectConditionWrapper conditionWrapper);
    }
}
