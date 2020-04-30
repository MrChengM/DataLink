using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServers
{
   public class BufferMangment
    {
        private Queue<byte> temp;
        private bool isEmpty;

        /// <summary>
        /// 报文头长度
        /// </summary>
        private int headLength;
        /// <summary>
        /// 报文体长度
        /// </summary>
        private int bodyLength;
        /// <summary>
        /// 读取报文缓存
        /// </summary>
        private byte[] readBuffer;
        /// <summary>
        /// 报文头缓存
        /// </summary>
        private byte[] headBuffer;
        /// <summary>
        /// 报文数据缓存
        /// </summary>
        private byte[] bodyBuffer;
        /// <summary>
        /// 发送报文缓存
        /// </summary>
        private byte[] sendBuffer;
        //private const int readSize = 256;
        //private const int cacheSize = 1024;
        public bool IsEmpty
        {
            get { return temp.Count == 0; }
        }

        /// <summary>
        /// 报文头长度
        /// </summary>
        public int HeadLength
        {
            get { return headLength; }
            set { headLength = value; }
        }
        /// <summary>
        /// 报文体长度
        /// </summary>
        public int BodyLength
        {
            get { return bodyLength; }
            set { bodyLength = value; }
        }
        /// <summary>
        /// 读取报文缓存
        /// </summary>
        public byte[] ReadBuffer
        {
            get { return readBuffer; }
            set { readBuffer = value; }
        }
        /// <summary>
        /// 报文头缓存
        /// </summary>
        public byte[] HeadBuffer
        {
            get {
                if (headBuffer == null)
                {
                    headBuffer = DeCache(headLength);
                }
                return headBuffer;
            }
            set { headBuffer = value; }
        }
        /// <summary>
        /// 报文本体数据缓存
        /// </summary>
        public byte[] BodyBuffer
        {
            get
            {
                if (bodyBuffer == null)
                {
                    bodyBuffer = DeCache(bodyLength);
                }
                return bodyBuffer;
            }
            set { bodyBuffer = value; }
        }
        /// <summary>
        /// 发送报文缓存
        /// </summary>
        public byte[] SendBuffer
        {
            get { return sendBuffer; }
            set { sendBuffer = value; }
        }
        public BufferMangment()
        {
            temp = new Queue<byte>();
            readBuffer = null;
            headBuffer = null;
            bodyBuffer = null;
            sendBuffer = null;
        }
        public BufferMangment(int size)
        {
            temp = new Queue<byte>(size * 2);
            readBuffer = new byte[size];
            headBuffer = null;
            bodyBuffer = null;
            sendBuffer = null;
        }
        public bool EnCache(int count)
        {
            if (readBuffer.Length >= count)
            {
                for (int i = 0; i < count; i++)
                {
                    temp.Enqueue(readBuffer[i]);
                }
                return true;
            }
                return false;
           
        }
        public bool EnCache(byte[] buffer,int count)
        {
            if (buffer.Length >= count)
            {
                for (int i = 0; i < count; i++)
                {
                    temp.Enqueue(buffer[i]);
                }
                return true;
            }
            return false;
          
        }
        /// <summary>
        /// 缓存读取
        /// 当缓存数量不足时，返回值为null；
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] DeCache(int count)
        {
            var buffer = new byte[count];
            if (temp.Count >= count)
            {
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = temp.Dequeue();
                }
                return buffer;
            }
            else
            {
                return null;
            }
           
        }
        /// <summary>
        /// 清空除主队列外的缓存数据
        /// </summary>
        public void clear()
        {
            headBuffer = null;
            bodyBuffer = null;
            sendBuffer = null;
        }
        /// <summary>
        /// 清空所有数据
        /// </summary>
        public void ClearAll()
        {
            temp = new Queue<byte>();
            readBuffer = null;
            headBuffer = null;
            bodyBuffer = null;
            sendBuffer = null;
        }
    }
}
