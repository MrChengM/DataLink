using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Points;

namespace DataServer.Log
{
    public interface ITagRecordCRUD
    {
        /// <summary>
        /// 添加一条新的记录
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Insert(ITag tag);
        /// <summary>
        /// 更新按点名找到的第一条记录
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Updata(ITag tag);
        /// <summary>
        /// 删除按照点名、值、时间查找都一致的第一条记录
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Delete(ITag tag);
        /// <summary>
        /// 删除包含点名的所有记录
        /// </summary>
        /// <param name="pointName"></param>
        /// <returns></returns>
        int Delete(string pointName);
        /// <summary>
        /// 删除包含点名时间范围内的所有记录
        /// </summary>
        /// <param name="pointName"></param>
        /// <returns></returns>
        int Delete(string pointName, DateTime stratTime, DateTime endTime);
        /// <summary>
        /// 查找包含点名的所有记录
        /// </summary>
        /// <param name="pointName"></param>
        /// <returns></returns>
        List<ITag> Select(string pointName);
        /// <summary>
        /// 查找指定时间范围内的包含点名的所有记录
        /// </summary>
        /// <param name="pointName"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<ITag> Select(string pointName, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 批量添加点位信息
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Insert(List<ITag> tags);
        /// <summary>
        /// 批量更新按点名找到的第一条记录
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        int Updata(List<ITag> tags);
        /// <summary>
        /// 删除按照点名、值、时间查找都一致的第一条记录
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Delete(List<ITag> tags);

    }
}
