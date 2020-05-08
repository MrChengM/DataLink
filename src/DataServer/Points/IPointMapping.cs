using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    /// <summary>
    /// 点表接口：
    /// 主要通过key值进行增删查改
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPointMapping<T>
    {
        /// <summary>
        /// 注册点到Mapping中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键索引</param>
        /// <param name="point">点名</param>
        void Register(string key,IPoint<T> point);
        /// <summary>
        /// 从Mapping中移除点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="point"></param>
        void Remove(string key);
        /// <summary>
        /// 通过键索引查找点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Find(string key);
        bool Find(string key,out string type);
        /// <summary>
        /// 通过键索引获取点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        IPoint<T> GetPoint(string key);
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T[] GetValue(string key);
        /// <summary>
        /// 设置piont的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int SetValue(string key, T[] value);
        bool SetQuality(string key,QUALITIES quality);

    }
    /// <summary>
    /// 批量操作Mapping的增删查改
    /// </summary>
    public interface IBatchInMapping
    {

    }

}
