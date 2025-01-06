using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Alarm;
namespace DataServer.Log
{
    public interface IHisAlarmRecordCRUD
    {
        /// <summary>
        /// 添加一条新的记录
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Insert(HistoryAlarm historyAlarm);
        /// <summary>
        /// 批量添加历史报警记录
        /// </summary>
        /// <param name="historyAlarms"></param>
        /// <returns></returns>
        int Insert(List<HistoryAlarm> historyAlarms);
        /// <summary>
        /// 删除包含报警名的所有记录
        /// </summary>
        /// <param name="alarmName"></param>
        /// <returns></returns>
        int Delete(string alarmName);
        /// <summary>
        /// 删除包含报警名时间范围内的所有记录
        /// </summary>
        /// <param name="alarmName"></param>
        /// <returns></returns>
        int Delete(string alarmName, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 查找包含报警名称的所有记录
        /// </summary>
        /// <param name="alarmName"></param>
        /// <returns></returns>
        List<HistoryAlarm> Select(string alarmName);
        /// <summary>
        /// 查找指定时间范围内的包含报警名称的所有历史记录
        /// </summary>
        /// <param name="alarmName"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<HistoryAlarm> Select(string alarmName, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 查找指定时间范围内的包含所有历史报警记录
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<HistoryAlarm> Select( DateTime startTime, DateTime endTime);
        /// <summary>
        /// 查找指定条件下历史报警记录
        /// </summary>
        /// <param name="selectCondition"></param>
        /// <returns></returns>
        List<HistoryAlarm> Select(HistoryAlarmSelectCondition selectCondition);
    }
}
