using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Log;
using Utillity.Data;
using System.Threading;

namespace DL645Driver
{
    public abstract class DLDriver : IComPortDriver
    {
        private CommunicationType _driverType;
        private TimeOut _timeOut;
        private bool _isConnect;
        private ILog _log;
        private int _pdu ;
        protected SerialportSetUp _setUp;
        protected SerialPort _serialPort;

        public DLDriver(SerialportSetUp setUp, TimeOut timeOut,ILog log )
        {
            _setUp = setUp;
            _log = log;
            _timeOut = timeOut;

            _driverType = CommunicationType.Serialport;
            _serialPort = new SerialPort();
            _isConnect = false;
        }
        public CommunicationType DriType
        {
            get
            {
                return _driverType;
            }
        }

        public bool IsClose
        {
            get
            {
                return _serialPort == null || _serialPort.IsOpen == false;
            }
        }

        public bool IsConnect
        {
            get
            {
                return _isConnect;
            }
        }

        public ILog Log
        {
            get
            {
                return _log;
            }

            set
            {
                _log =value;
            }
        }

        public int PDU
        {
            get
            {
                return _pdu;
            }

            set
            {
                _pdu=value;
            }
        }

        public TimeOut TimeOut
        {
            get
            {
               return _timeOut;
            }

            set
            {
                _timeOut =value;
            }
        }

        public ByteOrder Order { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int ConnectTimeOut { get; set; }
        public int RequestTimeOut { get; set; }
        public int RetryTimes { get; set; }
        public SerialportSetUp PortSetUp { get => _setUp; set => _setUp = value; }

        public bool Connect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                if (TimeOut.TimeOutSet < 1000)
                    TimeOut.TimeOutSet = 1000;
                _serialPort.PortName = _setUp.ComPort;
                _serialPort.BaudRate = _setUp.BuadRate;
                _serialPort.DataBits = _setUp.DataBit;
                _serialPort.StopBits = _setUp.StopBit;
                _serialPort.Parity = _setUp.OddEvenCheck;
                _serialPort.WriteTimeout = (int)TimeOut.TimeOutSet;
                _serialPort.ReadTimeout = (int)TimeOut.TimeOutSet;
                _serialPort.Open();
                _isConnect = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorLog("ModbusRTU Connect Error:" + ex.Message);
                _isConnect = false;
                return false;
            }
        }

        public bool DisConnect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _isConnect = false;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorLog("Modbus DisConnect Error:" + ex.Message);
                _isConnect = false;
                return false;
            }
        }

        public  void Dispose()
        {
            _setUp = null;
            _serialPort.Close();
            _serialPort.Dispose();
            TimeOut = null;
            Log = null;
        }
        /// <summary>
        /// 读电表数据 
        /// 功能码 fuction = 11H
        /// 数据域长度 1997：L=02H；2007：L=04H
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataArea"></param>
        /// <returns></returns>
        protected virtual byte[] createReadHeader(byte[] id, byte[] dataArea)
        {
            byte function = 0x11;
            int length = 16 + dataArea.Length;
            byte[] data = new byte[length];
            data[0] = 0xFE;              //唤醒字符 
            data[1] = 0xFE;
            data[2] = 0xFE;
            data[3] = 0xFE;
            data[4] = 0x68;             //启始字符
            for (int i = 0; i < 6; i++)
            {
                data[i + 5] = id[i]; //A0-A5
            }
            data[11] = 0x68;                            //启始字符
            data[12] = function; //控制码
            data[13] = (byte)dataArea.Length;//数据长度
            for (int i = 0; i < dataArea.Length; i++)                 //数据域（地址码）
            {
                data[14 + i] = (byte)(dataArea[i] + 0x33);
            }
            byte[] CSdata = new byte[length-6];
            Array.Copy(data, 4, CSdata, 0, CSdata.Length);
            data[length-2] = ByteCheck.CSCheck(CSdata);         //CS校验
            data[length-1] = 0x16;                            //结束字符
            return data;

        }
        object _async = new object();

        protected virtual byte[] readBytes(DeviceAddress address, ushort length)
        {
            try
            {
                var savleId = getSalveID(address.AreaID);
                var dataArea = getDataArea(address.Address);
                var SendData = createReadHeader(savleId, dataArea);
                lock (_async)
                {
                    _serialPort.Write(SendData, 0, SendData.Length);
                    Thread.Sleep(50);
                    int count = 0;
                    byte[] framedatas = new byte[_serialPort.BytesToRead];
                    while (_serialPort.BytesToRead > 0)
                    {
                        count += _serialPort.Read(framedatas, count, 1);
                    }
                    Queue<byte> dataQueue = new Queue<byte>(framedatas);//接收到的数组存入队列
                    List<byte[]> dataList = new List<byte[]>(); //接收到的数据按照每条报文格式存入list
                    while (dataQueue.Count != 0)
                    {
                        List<byte> lsbyte = new List<byte>();
                        var temp = dataQueue.Dequeue();
                        if (temp == 0x68)
                        {
                            lsbyte.Add(temp);
                            while (temp != 0x16 && dataQueue.Count != 0)
                            {
                                temp = dataQueue.Dequeue();
                                lsbyte.Add(temp);
                            }
                        }
                        if (lsbyte.Count != 0)
                        {
                            dataList.Add(lsbyte.ToArray());
                        }
                    }

                    List<byte> values = new List<byte>();
                    ///遍历list中每个数组，拿到转换好的值数据
                    foreach (var array in dataList)
                    {
                        byte[] receivedatas;
                        ///数据判断处理
                        receivedatas = new byte[array.Length];
                        Array.Copy(array, receivedatas, array.Length);
                        var value = getBytesValue(receivedatas, savleId, dataArea);
                        if (value != null)
                        {
                            foreach (var bt in value)
                            {
                                values.Add(bt);
                            }
                        }
                    }
                    return values == null ? null : values.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取数据域数组（地址码）
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected abstract byte[] getDataArea(int address);
        /// <summary>
        /// 根据整形变换为数量为6的字节数组
        /// 100000000000=>00,00,00,00,00,10
        /// 不足12位时自动补0
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual byte[] getSalveID(int id)
        {
            byte[] ids = new byte[6];
            var sids = id.ToString();
            if (sids.Length < 2 * ids.Length)
            {
                for (int i = 0; i < 2 * ids.Length - sids.Length; i++)
                {
                    sids = "0" + sids;
                }
            }
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = byte.Parse(sids.Substring(sids.Length - 2 * (i + 1), 2));
            }
            return ids;
        }
        /// <summary>
        /// 根据返回的数据进行数据处理得到具体的值数据
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="ids"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        protected abstract byte[] getBytesValue(byte[] datas, byte[] ids, byte[] address);
        /// <summary>
        /// 字节数据转换为float数组
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="postion">定位点</param>
        /// <returns></returns>
        protected float getFloat(byte[] source, int postion)
        {
            string s = "";
            for (int i = 1; i <= source.Length; i++)
            {
                s += source[source.Length - i].ToString("0:00");
            }
            //s = s.Substring(0, s.Length - postion) + "." + s.Substring(s.Length - postion, postion);
            s.Insert(postion, ".");
            return float.Parse(s);
        }
        /// <summary>
        /// 通过字节数组，根据数据长度及点位转换为指定格式的单元数组
        /// </summary>
        /// <param name="length">数据字节长度</param>
        /// <param name="postion">从左到右小数点定位</param>
        /// <param name="data">源数据字节数组</param>
        /// <returns></returns>
        protected Item<float>[] getItems(int length,int postion,byte[] data)
        {
            Item<float>[] value = null;
           
            if (data.Length % length == 0)
            {
                value = new Item<float>[data.Length / length];
                for (int i = 0; i < value.Length; i++)
                {
                    byte[] temp = new byte[length];
                    Array.Copy(data, length * i, temp, 0, temp.Length);
                    value[i] = new Item<float> { Vaule = getFloat(temp, postion), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
                }
            }
            return value;
        }
        /// <summary>
        /// 通过字节数组，根据数据长度及点位转换为指定格式的单元数组
        /// item value用float储存整形数据
        /// </summary>
        /// <param name="data">源数据字节数组</param>
        /// <returns></returns>
        protected Item<float>[] getItems(byte[] data)
        {
            Item<float>[] values = null;
            string s = "";
            foreach (var b in data)
            {
                s = b.ToString() + s;
            }
            var value = new Item<float>() { Vaule = float.Parse(s), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
            values = new Item<float>[] { value };
            return values;
        }
        /// <summary>
        /// 通过字节数组，根据数据长度及点位转换为指定格式的单元数组
        /// item value用float储存整形数据
        /// </summary>
        /// <param name="data">源数据字节数组</param>
        /// <returns></returns>
        protected virtual Item<float>[] getItemsTime(byte[] data)
        {
            Item<float>[] values = null;
            var value = Item<float>.CreateDefault();
            value.AppearTime = new DateTime(1900, data[3], data[2], data[1], data[0], 0);
            values = new Item<float>[] { value };
            return values;
        }

        public virtual Item<byte> ReadByte(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public virtual Item<byte>[] ReadBytes(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteBool(DeviceAddress deviceAddress, bool datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteByte(DeviceAddress deviceAddress, byte datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteShort(DeviceAddress deviceAddress, short datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteUShort(DeviceAddress deviceAddress, ushort datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteInt(DeviceAddress deviceAddress, int datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteUInt(DeviceAddress deviceAddress, uint datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteFloat(DeviceAddress deviceAddress, float datas, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteString(DeviceAddress deviceAddress, string datas, int offset = 0)
        {
            throw new NotImplementedException();
        }
       
        public virtual DeviceAddress GetDeviceAddress(string address)
        {
            throw new NotImplementedException();
        }

        public virtual Item<bool> ReadBool(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public virtual Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public virtual Item<float> Readfloat(DeviceAddress deviceAddress)
        {

            throw new NotImplementedException();
        }
        public virtual Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public virtual Item<int> ReadInt(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public virtual Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public virtual Item<short> ReadShort(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public virtual Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public virtual Item<string> ReadString(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public virtual Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public virtual Item<uint> ReadUInt(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public virtual Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public virtual Item<ushort> ReadUShort(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public virtual Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }


        public virtual int WriteBools(DeviceAddress deviceAddress, bool[] datas)
        {
            throw new NotImplementedException();
        }

    

        public virtual int WriteBytes(DeviceAddress deviceAddress, byte[] datas)
        {
            throw new NotImplementedException();
        }
        public virtual int WriteFloats(DeviceAddress deviceAddress, float[] datas)
        {
            throw new NotImplementedException();
        }
        public virtual int WriteInts(DeviceAddress deviceAddress, int[] datas)
        {
            throw new NotImplementedException();
        }
        public virtual int WriteShorts(DeviceAddress deviceAddress, short[] datas)
        {
            throw new NotImplementedException();
        }
     
        public virtual int WriteStrings(DeviceAddress deviceAddress, string[] datas)
        {
            throw new NotImplementedException();
        }
        public virtual int WriteUInts(DeviceAddress deviceAddress, uint[] datas)
        {
            throw new NotImplementedException();
        }
        public virtual int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas)
        {
            throw new NotImplementedException();
        }
    }
}
