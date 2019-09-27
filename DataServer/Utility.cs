using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public static class Utility
    {
        private static readonly ushort[] crcTable = {
            0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241,
            0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440,
            0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40,
            0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841,
            0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40,
            0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41,
            0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641,
            0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040,
            0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240,
            0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441,
            0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41,
            0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840,
            0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41,
            0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40,
            0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640,
            0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041,
            0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240,
            0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441,
            0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41,
            0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840,
            0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41,
            0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40,
            0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640,
            0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041,
            0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241,
            0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440,
            0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40,
            0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841,
            0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40,
            0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41,
            0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641,
            0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040
        };
        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="data">CRC校验数据</param>
        /// <returns>CRC value</returns>
        public static byte[] CalculateCrc(byte[] data, int len = 0)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (len == 0) len = data.Length;
            ushort crc = ushort.MaxValue;
            for (int i = 0; i < len; i++)
            {
                byte tableIndex = (byte)(crc ^ data[i]);
                crc >>= 8;
                crc ^= crcTable[tableIndex];
            }

            return BitConverter.GetBytes(crc);
        }
        /// <summary>
        /// 判断CRC校验是否正确
        /// </summary>
        /// <param name="frame">获取字节数组，后两位为校验数据</param>
        /// <returns>CRC校验是否匹配 </returns>
        public static bool CheckSumCRC(byte[] frame, int len)
        {
            //int len = frame.Length;
            byte[] chk = CalculateCrc(frame, len - 2);
            return (chk[0] == frame[len - 2] && chk[1] == frame[len - 1]);
        }
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
        /// <summary>
        /// 字节数组转bool数组
        /// </summary>
        /// <param name="sourceBytes">源目标字节数组</param>
        /// <param name="length">转换长度，最大值为byte[].length*8</param>
        /// <returns></returns>
        public static bool[] BytesToBools(byte[] sourceBytes, int length)
        {
            if (length > sourceBytes.Length * 8)
                throw new NotImplementedException();
            bool[] result = new bool[length];
            int index = 0;
            for (int i = 0; i < sourceBytes.Length; i++)
            {
                index += 8 * i;
                for (int j = 0; j < 8; j++)
                {
                    index += j;
                    if (index >= length)
                        return result;
                    result[index] = ByteToBool(sourceBytes[i], j);
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();

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
                    byte[] value = new byte[4];
                    return result = BitConverter.ToInt32(value, 0);
            }
            return default(short);
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
        #endregion
    }
    public class UnSafeUtility
    {
        
        private unsafe static byte[] ToBytes(byte* bytePtr,int size, ByteOrder byteOrder)
        {
            //int count = sizeof(short);
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
                if (byteOrder == ByteOrder.BigEndian)
                {
                    for (int i = 0; i < size; i++)
                    {
                        q[size - i - 1] = *(p + i);
                    }
                }

            }
            return result;
        }
        public unsafe static byte[] ShortToBytes(short data, ByteOrder byteOrder)
        {
            int count = sizeof(short);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        public unsafe static byte[] UshortToBytes(ushort data, ByteOrder byteOrder)
        {

            int count = sizeof(ushort);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        public unsafe static byte[] IntToBytes(int data, ByteOrder byteOrder)
        {
            int count = sizeof(int);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        public unsafe static byte[] UIntToBytes(uint data, ByteOrder byteOrder)
        {
            int count = sizeof(uint);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        public unsafe static byte[] LongToBytes(long data, ByteOrder byteOrder)
        {
            int count = sizeof(long);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }

        public unsafe static byte[] ULongToBytes(ulong data, ByteOrder byteOrder)
        {

            int count = sizeof(ulong);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        public unsafe static byte[] SingleToBytes(float data, ByteOrder byteOrder)
        {

            int count = sizeof(float);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        public unsafe static byte[] DoubleToBytes(double data, ByteOrder byteOrder)
        {

            int count = sizeof(double);
            byte* p = (byte*)&data;
            return ToBytes(p, count, byteOrder);
        }
        private unsafe static void BytesToStruct(byte* bytePtr,int size, byte[] data, int startIndex, ByteOrder byteOrder)
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
                if (byteOrder == ByteOrder.BigEndian)
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
                throw new NotImplementedException();
            short result;
            int size = sizeof(short);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static ushort BytesToUShort(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                throw new NotImplementedException();
            ushort result;
            int size = sizeof(ushort);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static int BytesToInt(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                throw new NotImplementedException();
            int result;
            int size = sizeof(int);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static uint BytesToUInt(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                throw new NotImplementedException();
            uint result;
            int size = sizeof(uint);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static long BytesToLong(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                throw new NotImplementedException();
            long result;
            int size = sizeof(long);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static ulong BytesToULong(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                throw new NotImplementedException();
            ulong result;
            int size = sizeof(ulong);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static float BytesToFloat(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                throw new NotImplementedException();
            float result;
            int size = sizeof(float);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }
        public unsafe static double BytesToDouble(byte[] data, int startIndex, ByteOrder byteOrder)
        {
            if (data == null || startIndex >= data.Length)
                throw new NotImplementedException();
            double result;
            int size = sizeof(double);
            byte* p = (byte*)&result;
            BytesToStruct(p, size, data, startIndex, byteOrder);
            return result;
        }

        public static short[] BytesToShorts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            short[] result = new short[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToShort(data, startIndex + i * 2, byteOrder);
            }
            return result;
        }
        public static ushort[] BytesToUShorts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            ushort[] result = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BytesToUShort(data, startIndex + i * 2, byteOrder);
            }
            return result;
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
        public static uint[] BytesToUInts(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            uint[] result = new uint[count];
            for (int i = 0; i < count; i++)
            {
                result[count] = BytesToUInt(data, startIndex + i * 4, byteOrder);
            }
            return result;
        }
        public static long[] BytesToLongs(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            long[] result = new long[count];
            for (int i = 0; i < count; i++)
            {
                result[count] = BytesToLong(data, startIndex + i * 8, byteOrder);
            }
            return result;
        }
        public static ulong[] BytesToULongs(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            ulong[] result = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                result[count] = BytesToULong(data, startIndex + i * 8, byteOrder);
            }
            return result;
        }
        public static float[] BytesToFloats(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            float[] result = new float[count];
            for (int i = 0; i < count; i++)
            {
                result[count] = BytesToFloat(data, startIndex + i * 4, byteOrder);
            }
            return result;
        }
        public static double[] BytesToDoubls(byte[] data, int startIndex, int count, ByteOrder byteOrder)
        {
            double[] result = new double[count];
            for (int i = 0; i < count; i++)
            {
                result[count] = BytesToFloat(data, startIndex + i * 8, byteOrder);
            }
            return result;
        }
    }
}
