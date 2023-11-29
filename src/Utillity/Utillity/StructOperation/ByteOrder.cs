using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.StructOperation
{
    public enum ByteOrder
    {
        None = 0,
        /// <summary>
        /// 大端法，高位存储在字节低位，低位存储在字节高位
        /// </summary>
        BigEndian = 1,
        /// <summary>
        /// 大端法，高位存储在字节低位，低位存储在字节高位（发送报文时，字需要调换字节高低位）
        /// </summary>
        BigEndianAndRervseWord = 2,
        /// <summary>
        /// 小端法，低位存储在字节低位，高位存储在字节高位
        /// </summary>
        LittleEndian = 4,
        //Network = 4,
        //Host = 8
    }
}
