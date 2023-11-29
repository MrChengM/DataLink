using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.StructOperation
{
    public class UnsafeNetConvert
    {
        #region 值类型转字节数组
        /// <summary>
        /// 根据字节指针，大小，转换至字节组
        /// </summary>
        /// <param name="bytePtr">字节指针</param>
        /// <param name="size">字节组大小</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        private unsafe static byte[] ToBytes(byte* bytePtr, int size, ByteOrder byteOrder)
        {
            byte* p = bytePtr;
            byte[] result = new byte[size];
            fixed (byte* q = result)
            {
                if (byteOrder == ByteOrder.None || byteOrder == ByteOrder.LittleEndian)
                {
                    for (int i = 0; i < size; i++)
                    {
                        q[i] = *(p + i);
                    }
                }
                if (byteOrder == ByteOrder.BigEndian || byteOrder == ByteOrder.BigEndianAndRervseWord)
                {
                    for (int i = 0; i < size; i++)
                    {
                        q[size - i - 1] = *(p + i);
                    }
                }

            }
            return result;
        }
        /// <summary>
        /// 高低位互换
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public unsafe static byte[] BytesPerversion(byte[] source)
        {
            if (source != null && source.Length % 2 == 0)
            {
                fixed (byte* p = &source[0])
                {
                    for (int i = 0; i < source.Length; i = i + 2)
                    {
                        byte cache = p[i];
                        p[i] = p[i + 1];
                        p[i + 1] = cache;
                    }
                }
                return source;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 16位整型转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] ShortToBytes(short data, ByteOrder byteOrder)
        {
            int count = sizeof(short);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        /// <summary>
        /// 16位无符号整型转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] UShortToBytes(ushort data, ByteOrder byteOrder)
        {

            int count = sizeof(ushort);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }

        /// <summary>
        /// 32位整型转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] IntToBytes(int data, ByteOrder byteOrder)
        {
            int count = sizeof(int);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        /// <summary>
        /// 32位无符号整型转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] UIntToBytes(uint data, ByteOrder byteOrder)
        {
            int count = sizeof(uint);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        /// <summary>
        /// 64位整型转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] LongToBytes(long data, ByteOrder byteOrder)
        {
            int count = sizeof(long);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        /// <summary>
        /// 64位无符号整型转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] ULongToBytes(ulong data, ByteOrder byteOrder)
        {

            int count = sizeof(ulong);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        /// <summary>
        /// 单精度浮点转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] FloatToBytes(float data, ByteOrder byteOrder)
        {

            int count = sizeof(float);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        /// <summary>
        /// 双精度转为字节组
        /// </summary>
        /// <param name="data">源数据</param>
        /// <param name="byteOrder">编码方式</param>
        /// <returns></returns>
        public unsafe static byte[] DoubleToBytes(double data, ByteOrder byteOrder)
        {

            int count = sizeof(double);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        #endregion
        #region 值类型数组转字节数组
        public unsafe static byte[] ShortsToBytes(short[] data, ByteOrder byteOrder)
        {
            int size = sizeof(short);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(ShortToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }

        public unsafe static byte[] UShortsToBytes(ushort[] data, ByteOrder byteOrder)
        {
            int size = sizeof(ushort);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(UShortToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }
        public unsafe static byte[] IntsToBytes(int[] data, ByteOrder byteOrder)
        {
            int size = sizeof(int);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(IntToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }
        public unsafe static byte[] UIntsToBytes(uint[] data, ByteOrder byteOrder)
        {
            int size = sizeof(uint);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(UIntToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }
        public unsafe static byte[] LongsToBytes(long[] data, ByteOrder byteOrder)
        {
            int size = sizeof(long);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(LongToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }
        public unsafe static byte[] ULongsToBytes(ulong[] data, ByteOrder byteOrder)
        {
            int size = sizeof(ulong);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(ULongToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }
        public unsafe static byte[] FloatsToBytes(float[] data, ByteOrder byteOrder)
        {
            int size = sizeof(float);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(FloatToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }
        public unsafe static byte[] DoublesToBytes(double[] data, ByteOrder byteOrder)
        {
            int size = sizeof(double);
            byte[] result = new byte[size * data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(DoubleToBytes(data[i], byteOrder), 0, result, i * size, size);
            }
            return result;
        }
        #endregion
        #region 字节数组转值类型
        private unsafe static void BytesToStruct(byte* bytePtr, int size, byte[] data, int startIndex, ByteOrder byteOrder)
        {
            fixed (byte* q = &data[startIndex])
            {
                if (data.Length - startIndex < size)
                    size = data.Length - startIndex;
                if (byteOrder == ByteOrder.None || byteOrder == ByteOrder.LittleEndian)
                {
                    for (int i = 0; i < size; i++)
                    {
                        *(bytePtr + i) = q[i];
                    }
                }
                if (byteOrder == ByteOrder.BigEndian || byteOrder == ByteOrder.BigEndianAndRervseWord)
                {
                    for (int i = 0; i < size; i++)
                    {
                        *(bytePtr + i) = q[size - i - 1];
                    }
                }
            }
        }
        public unsafe static short BytesToShort(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(short);
            short result;
            int size = sizeof(short);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static ushort BytesToUShort(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(ushort);
            ushort result;
            int size = sizeof(ushort);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }

        public unsafe static int BytesToInt(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(int);
            int result;
            int size = sizeof(int);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static uint BytesToUInt(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(int);
            uint result;
            int size = sizeof(uint);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static long BytesToLong(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(long);
            long result;
            int size = sizeof(long);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static ulong BytesToULong(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(ulong);
            ulong result;
            int size = sizeof(ulong);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static float BytesToFloat(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(float);
            float result;
            int size = sizeof(float);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static double BytesToDouble(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                return default(double);
            double result;
            int size = sizeof(double);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        #endregion
        #region 字节数组转值类型数组
        public static short[] BytesToShorts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            if (data == null)
            {
                return null;
            }
            short[] result = new short[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToShort(data, startIndex + i * 2, byteOrder);
            }
            return result;
        }
        public static ushort[] BytesToUShorts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            if (data == null)
            {
                return null;
            }
            ushort[] result = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToUShort(data, startIndex + i * 2, byteOrder);
            }
            return result;
        }
        public static int[] BytesToInts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            if (data == null)
            {
                return null;
            }
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToInt(data, startIndex + i * 4, byteOrder);
            }
            return result;
        }
        public static uint[] BytesToUInts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            if (data == null)
            {
                return null;
            }
            uint[] result = new uint[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToUInt(data, startIndex + i * 4, byteOrder);
            }
            return result;
        }
        public static long[] BytesToLongs(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            long[] result = new long[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToLong(data, startIndex + i * 8, byteOrder);
            }
            return result;
        }
        public static ulong[] BytesToULongs(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            if (data == null)
            {
                return null;
            }
            ulong[] result = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToULong(data, startIndex + i * 8, byteOrder);
            }
            return result;
        }
        public static float[] BytesToFloats(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            float[] result = new float[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToFloat(data, startIndex + i * 4, byteOrder);
            }
            return result;
        }
        public static double[] BytesToDoubls(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            if (data == null)
            {
                return null;
            }
            double[] result = new double[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToFloat(data, startIndex + i * 8, byteOrder);
            }
            return result;
        }
        #endregion
        #region 值类型转16位整型数组
        public static short UShortToShort(ushort data)
        {
            return BytesToShort(UShortToBytes(data, ByteOrder.None), 0, ByteOrder.None);
        }
        public static short[] IntToShorts(int data)
        {
            return BytesToShorts(IntToBytes(data, ByteOrder.None), 0, 2, ByteOrder.None);
        }
        #endregion
    }
    
}
