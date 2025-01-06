using DataServer.Points;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Data;

namespace DataServer
{
    public static class NetConvertExtension
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

        public static string SetBit( string Source, DataType type, int bit, bool flag)
        {
            string result = string.Empty;
            switch (type)
            {
                case DataType.Bool:
                    result = flag.ToString();
                    break;
                case DataType.Byte:
                    if (byte.TryParse(Source, out byte bb))
                    {
                        result = NetConvert.SetBit(bb, bit, flag).ToString();
                    }
                    break;
                case DataType.Short:
                    if (short.TryParse(Source, out short sh))
                    {
                        result = NetConvert.SetBit(sh, bit, flag).ToString();
                    }
                    break;
                case DataType.UShort:
                    if (ushort.TryParse(Source, out ushort us))
                    {
                        result = NetConvert.SetBit(us, bit, flag).ToString();
                    }
                    break;
                case DataType.Int:
                    if (int.TryParse(Source, out int it))
                    {
                        result = NetConvert.SetBit(it, bit, flag).ToString();
                    }
                    break;
                case DataType.UInt:
                    if (int.TryParse(Source, out int uit))
                    {
                        result = NetConvert.SetBit(uit, bit, flag).ToString();
                    }
                    break;
                case DataType.Float:
                    break;
                case DataType.String:
                    break;
                default:
                    break;
            }
            return result;
        }
    }
    public class StringHandlerExtension
    {
        public static PointNameIndex SplitEndWith(string source, string splitStr = "[", string endStr = "]")
        {
            if (source.EndsWith(endStr))
            {

                var index = source.LastIndexOf(splitStr);
                if (index == -1)
                {
                    return null;
                }
                var startStr = source.Substring(0, index);
                var lastStr = source.Substring(index + 1).Replace(endStr, "");
                //var arraryString = new string[] { startStr, lastStr };
                //for (int i = 0; i < arraryString.Length; i++)
                //{
                //    arraryString[i] = arraryString[i].Replace(endStr, "");
                //}
                if (int.TryParse(lastStr, out int sIndex))
                {
                    return new PointNameIndex(startStr, sIndex);
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }

        }

    }
}
