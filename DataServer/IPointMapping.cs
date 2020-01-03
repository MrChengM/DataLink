using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public interface IPointMapping<T>
    {
        /// <summary>
        /// 注册点到Mapping中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键索引</param>
        /// <param name="point">点名</param>
        void Register(string key,IPoint<T> point);
        ///// <summary>
        ///// 批量注册点到Mapping中
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key">键索引</param>
        ///// <param name="point">点名</param>
        //void Register<T>(string key,IPoint<T>[] points);
        /// <summary>
        /// 从Mapping中移除点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="point"></param>
        void Remove(string key,IPoint<T> point);
        ///// <summary>
        ///// 从Mapping中批量移除点
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="point"></param>
        //void Remove<T>(string key,IPoint<T>[] point);

        /// <summary>
        /// 通过点名查找点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point"></param>
        /// <returns></returns>
        bool Find(IPoint<T> point);
        /// <summary>
        /// 通过点名批量查找点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="points"></param>
        /// <returns></returns>
        //bool Find<T>(IPoint<T>[] points);
        ///// <summary>
        ///// 通过键索引查找点
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        bool Find(string key);
        /// <summary>
        /// 通过键索引批量查找点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool Find(string key, int count);

        /// <summary>
        /// 通过键索引获取点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        IPoint<T> Get(string key);
        ///// <summary>
        ///// 通过键索引连续批量获取点
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //IPoint<T>[] Get<T>(string key, int count);
        ///// <summary>
        ///// 通过键索引获取值
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //T GetValue(string key);
        /// <summary>
        /// 通过键索引连续批量获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        T[] GetValue(string key, int count);


        //int Set(string key, T value);
        /// <summary>
        /// 设置piont的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int Set(string key, T[] value);

    }
}
