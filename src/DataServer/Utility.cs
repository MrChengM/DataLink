using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Data;

namespace DataServer
{
    public class NetConvertExtension
    {
        public static Item<T>[] ToItems<T>( T[] datas, int offset, int count)
        {
            Item<T>[] result = new Item<T>[count];
            if (datas == null)
            {
                for (int i = 0; i < count; i++)
                    result[i] = Item<T>.CreateDefault();
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (i + offset < datas.Length)
                        result[i] = new Item<T>()
                        {
                            Vaule = datas[i + offset],
                            UpdateTime = DateTime.Now,
                            Quality = QUALITIES.QUALITY_GOOD
                        };
                    else
                        result[i] = Item<T>.CreateDefault();
                }
            }
            return result;
        }
        public static T[] ToDatas<T>(Item<T>[] datas, int offset, int count)
        {
            T[] result = new T[count];
            if (datas==null)
            {
                result = null;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (i + offset < datas.Length)
                        result[i] = datas[i].Vaule;
                 }
            }
            return result;
        }
    }
}
