using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.Data
{
    public class NetConvert
    {
        #region 数据转换 
        public static byte[] ShortToBytes(short data, ByteOrder byteOrder)
        {
            byte[] result;
            switch (byteOrder)
            {
                case ByteOrder.LittleEndian:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.None:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.BigEndianAndRervseWord:
                case ByteOrder.BigEndian:
                    var bytes = BitConverter.GetBytes(data);
                    int count = bytes.Length;
                    result = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[count - i] = bytes[i];
                    }
                    return result;
            }
            return null;
        }
        public static byte[] ShortsToBytes(short[] datas, ByteOrder byteOrder)
        {
            List<byte> byteList=new List<byte>();

            foreach (var data in datas)
            {
                var byteVaules = ShortToBytes(data, byteOrder);
                if (byteVaules != null) 
                {
                    foreach (var byteVaule in byteVaules)
                    {
                        byteList.Add(byteVaule);
                    }
                }
                else
                {
                    return null;
                }
            }
            return byteList.ToArray();
        }
        /// <summary>
        /// 32整形转换为字节数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="byteOrder"></param>
        /// <returns></returns>
        public static byte[] IntToBytes(int data, ByteOrder byteOrder)
        {
            byte[] result;
            switch (byteOrder)
            {
                case ByteOrder.LittleEndian:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.None:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.BigEndian:
                case ByteOrder.BigEndianAndRervseWord:
                    var bytes = BitConverter.GetBytes(data);
                    int count = bytes.Length;
                    result = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[count - i] = bytes[i];
                    }
                    return result;
            }
            return null;
        }
        public static byte[] LongToBytes(long data, ByteOrder byteOrder)
        {
            byte[] result;
            switch (byteOrder)
            {
                case ByteOrder.LittleEndian:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.None:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.BigEndian:
                case ByteOrder.BigEndianAndRervseWord:
                    var bytes = BitConverter.GetBytes(data);
                    int count = bytes.Length;
                    result = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[count - i] = bytes[i];
                    }
                    return result;
            }
            return null;
        }

        public static byte[] SingleToBytes(float data, ByteOrder byteOrder)
        {
            byte[] result;
            switch (byteOrder)
            {
                case ByteOrder.LittleEndian:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.None:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.BigEndian:
                case ByteOrder.BigEndianAndRervseWord:
                    var bytes = BitConverter.GetBytes(data);
                    int count = bytes.Length;
                    result = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[count - i] = bytes[i];
                    }
                    return result;
            }
            return null;
        }
        public static byte[] DoubleToBytes(double data, ByteOrder byteOrder)
        {
            byte[] result;
            switch (byteOrder)
            {
                case ByteOrder.LittleEndian:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.None:
                    return result = BitConverter.GetBytes(data);
                case ByteOrder.BigEndian:
                case ByteOrder.BigEndianAndRervseWord:
                    var bytes = BitConverter.GetBytes(data);
                    int count = bytes.Length;
                    result = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[count - i] = bytes[i];
                    }
                    return result;
            }
            return null;
        }

        /// <summary>
        /// 求字节指定位的状态
        /// </summary>
        /// <param name="sourceByte">源数据字节,</param>
        /// <param name="positon">定位,小于等于8</param>
        /// <returns></returns>
        public static bool ByteToBool(byte sourceByte, int positon)
        {
            if (positon > 8)
                throw new NotImplementedException();
            byte a = 1;
            return (sourceByte >> positon & a) != 0;
        }
        public static byte BooltoByte(bool source, int positon)
        {
            if (positon > 8)
                throw new NotImplementedException();
            int a = 0;
            if (source)
                a = 1;
            return (byte)(a << positon);
        }
        public static byte[] BoolstoBytes(bool[] source)
        {
            if (source == null)
                return null;
            int count = 1;
            if (source.Length > 8)
            {
                count = source.Length % 8 == 0 ? source.Length / 8 : source.Length / 8 + 1;
            }
            byte[] result = new byte[count];
            int soucreIdex = 0;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (soucreIdex >= source.Length)
                        break;
                    result[i] = (byte)(result[i] | BooltoByte(source[soucreIdex], j));
                    soucreIdex++;
                }
            }
            return result;
        }
        /// <summary>
        /// 字节数组转bool数组
        /// </summary>
        /// <param name="sourceBytes">源目标字节数组</param>
        /// <param name="length">转换长度，最大值为byte[].length*8</param>
        /// <returns></returns>
        public static bool[] BytesToBools(byte[] sourceBytes, int length)
        {
            if (sourceBytes == null || length > sourceBytes.Length * 8)
                return null;
            bool[] result = new bool[length];
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                index1 = 8 * i;
                for (int j = 0; j < 8; j++)
                {
                    index2 = index1 + j;
                    if (index2 >= length)
                        return result;
                    result[index2] = ByteToBool(sourceBytes[i], j);
                }
            }
            return result;
        }
        /// <summary>
        /// 字节数组转bool数组
        /// </summary>
        /// <param name="sourceBytes">源目标字节数组</param>
        /// <param name="length">转换长度，最大值为byte[].length*8</param>
        /// <returns></returns>
        public static bool[] BytesToBools(byte[] sourceBytes, int positon, int length)
        {
            if (sourceBytes == null || length + positon > sourceBytes.Length * 8)
                return null;
            bool[] result = new bool[length];
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                index1 = 8 * i;
                for (int j = 0; j < 8; j++)
                {
                    if ((index2 = index1 + j - positon) >= 0)
                    {
                        if (index2 >= length)
                            return result;
                        result[index2] = ByteToBool(sourceBytes[i], j);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 字节数组转换为16位整型
        /// </summary>
        /// <param name="data"> 输入值</param>
        /// <param name="startIndex"> 起始位置，小于数组长度</param>
        /// <param name="byteOrder"></param>
        /// <returns></returns>
        public static short BytesToShort(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(short);
            short result;
            byte[] bytes = new byte[2];
            if (startIndex > data.Length - 2)
            {
                bytes[0] = data[startIndex];
            }
            else
            {
                bytes[0] = data[startIndex];
                bytes[1] = data[startIndex + 1];
            }
            switch (byteOrder)
            {
                case ByteOrder.None:
                    return result = BitConverter.ToInt16(bytes, 0);
                case ByteOrder.LittleEndian:
                    return result = BitConverter.ToInt16(bytes, 0);
                case ByteOrder.BigEndian:
                case ByteOrder.BigEndianAndRervseWord:
                    byte[] value = new byte[] { bytes[1], bytes[0] };
                    return result = BitConverter.ToInt16(value, 0);
            }
            return default(short);
        }
        /// <summary>
        /// 字节数组转换为16位整型数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="byteOrder"></param>
        /// <returns></returns>
        public static short[] BytesToShorts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            short[] result = new short[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToShort(data, startIndex + i * 2, byteOrder);
            }
            return result;
        }
        public static int BytesToInt(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(int);
            int result;
            byte[] bytes = new byte[4];
            if (startIndex > data.Length - 4)
            {

                for (int i = 0; i < data.Length - startIndex; i++)
                {
                    bytes[i] = data[startIndex + i];
                }
            }
            else
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = data[startIndex + i];
                }
            }
            switch (byteOrder)
            {
                case ByteOrder.None:
                    return result = BitConverter.ToInt32(bytes, 0);
                case ByteOrder.LittleEndian:
                    return result = BitConverter.ToInt32(bytes, 0);
                case ByteOrder.BigEndian:
                case ByteOrder.BigEndianAndRervseWord:
                    byte[] value = new byte[4];
                    return result = BitConverter.ToInt32(value, 0);
            }
            return default(int);
        }
        public static int[] BytesToInts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                result[count] = BytesToInt(data, startIndex + i * 4, byteOrder);
            }
            return result;
        }

        /// <summary>
        /// Hex字符串按2个字符转换成字节数组
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] GetHexBytes(string source)
        {
            byte[] result;
            var temp = source.Length % 2;
            if (temp == 0)
            {
                result = new byte[source.Length / 2];
            }
            else
            {
                source = string.Concat(source, "0");//补齐
                result = new byte[source.Length / 2 + 1];
            }
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = byte.Parse(source.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return result;

        }
        /// <summary>
        /// 字节数组转换为16进制字符串
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public static string GetHexString(byte[] soure)
        {

            string result=null;

            foreach (var data in soure)
            {
                if (result==null)
                {
                    result = string.Format("{0:X2}", data);

                }
                else
                {
                    result.Contains(string.Format("{0:X2}", data));

                }
            }
            return result;
        }
        #endregion
    }
}
