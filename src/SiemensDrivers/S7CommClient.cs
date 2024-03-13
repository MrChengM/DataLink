using System;
using DataServer;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Globalization;
using Utillity.Data;

namespace SiemensDriver
{
    [DriverDescription("Siemens S7",CommunicationType.Ethernet)]
    public class S7CommClient : IPLCDriver
    {
        //private TimeOut _timeOut;
        private bool _isConnect = false;
        private ILog _log;
        private int _pdu;
        private EthernetSetUp _ethernetSetUp = new EthernetSetUp();
        private Socket _socket;
        private int _slotNo;

        private int _handler;

        public string Name { get; set; }
        public S7CommClient()
        {
            _handler = 0;
            ConnectTimeOut = 3000;
            RequestTimeOut = 1000;
            RetryTimes = 1;
        }
      
        public S7CommClient(EthernetSetUp ethernetSetUp)
        {
            _ethernetSetUp = ethernetSetUp;
            _handler = 0;
            ConnectTimeOut = 3000;
            RequestTimeOut = 1000;
            RetryTimes = 1;
        }
        //public S7CommClient(EthernetSetUp ethernetSetUp, TimeOut timeOut, ILog log,int slotNo)
        //{
        //    _ethernetSetUp = ethernetSetUp;
        //    _timeOut = timeOut;
        //    _log = log;
        //    _slotNo = slotNo;
        //    _handler = 0;
        //}

        [DeviceMark]
        public int SlotNo
        {
            get
            {
                return _slotNo;
            }
            set
            {
                _slotNo = value;
            }
        }

        public EthernetSetUp EthSetUp
        {
            get
            {
                return _ethernetSetUp;
            }
            set
            {
                _ethernetSetUp = value;
            }
        }
        public bool IsClose
        {
            get
            {
                return _socket==null||!_socket.Connected;
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
                _log=value;
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

        //public TimeOut TimeOut
        //{
        //    get
        //    {
        //        return _timeOut;
        //    }

        //    set
        //    {
        //       _timeOut=value;
        //    }
        //}

        public ByteOrder Order { get ; set ; }
        public int Id { get ; set; }
        public int ConnectTimeOut { get ; set ; }
        public int RequestTimeOut { get ; set ; }
        public int RetryTimes { get; set; }

        public bool Connect()
        {
            try
            {
                if (_socket == null)
                    _socket = new Socket(SocketType.Stream, _ethernetSetUp.ProtocolType);
               
                _socket.SendTimeout = RequestTimeOut;
                _socket.ReceiveTimeout = RequestTimeOut;
                IPAddress ipaddress;
                if (IPAddress.TryParse(_ethernetSetUp.IPAddress, out ipaddress))
                {
                    _socket.Connect(ipaddress, _ethernetSetUp.PortNumber);
                    int initState = -1;
                    byte[] receiveBytes = new byte[16384];

                    /************************************
                    第一次握手，主要是需确认CPUSlot
                    PC发出报文: ( [A18]=0x01 =CPUSlot)
                    03 00  00 16 11 E0 00 00 00 01  00 C1 02 01 00 C2 02 01 01 C0 01 09
                    PLC回复报文:( B[10]=0x06 可能 是西门子的小型号  B[22]=0x01=CPUSlot)
                    03 00 00 16 11 D0 00 01 00 06 00 C0 01 09 C1 02 01 00 C2 02 01 01   
                    *************************************/
                    string first = string.Concat("0300001611E00000000100C1020100C20201", _slotNo.ToString("X").PadLeft(2), "C00109");
                    _log.DebugLog($"{Name}:Tx => 0x {first}");
                    _socket.Send(NetConvert.GetHexBytes(first));

                    Thread.Sleep(10);
                    int readNumber = 0;

                    _socket.Receive(receiveBytes, 4, SocketFlags.None);
                    _socket.Receive(receiveBytes, 4, receiveBytes[3] - 4, SocketFlags.None);
                    readNumber = receiveBytes[3];
                    if (readNumber == 22)
                        initState = 0;
                    //报文数据记录
                    var logBytes = new byte[readNumber];
                    Array.Copy(receiveBytes, 0, logBytes, 0, readNumber);
                    _log.DebugLog($"{Name}:Re <= 0x {NetConvert.GetHexString(receiveBytes)}");
                    /************************************
                   第二次握手
                   PC发出报文：
                   03 00 00 19 02 F0 80 32 01 00 00 FF FF 00 08 00 00 F0 00 00 01 00 01 07 80     
                   PLC回复报文：
                   03 00 00 1B 02 F0 80 32 03 00 00 FF FF 00 08 00 00 00 00 F0 00 00 01 00 01 00 F0      
                  *************************************/
                    if (initState == 0)
                    {
                        string second = "0300001902F08032010000FFFF00080000F000000100010780";
                        _log.DebugLog($"{Name}:Tx => 0x {second}");
                        _socket.Send(NetConvert.GetHexBytes(second));
                        Thread.Sleep(10);

                        _socket.Receive(receiveBytes, 4, SocketFlags.None);
                        _socket.Receive(receiveBytes, 4, receiveBytes[3] - 4, SocketFlags.None);
                        readNumber = receiveBytes[3];
                        if (readNumber >= 27)
                        {
                            initState = 1;
                        }

                        //报文数据记录
                        logBytes = new byte[readNumber];
                        Array.Copy(receiveBytes, 0, logBytes, 0, readNumber);
                        _log.DebugLog($"{Name}:Re <= 0x {NetConvert.GetHexString(receiveBytes)}");
                    }
                    return _isConnect = initState == 1 ? true : false;
                }
                else
                {
                    Log.ErrorLog( string.Format( "{0}:IP地址无效",Name));
                    return _isConnect = false;
                }
            }
            catch (Exception ex)
            {
                DisConnect();
                Log.ErrorLog(Name + ":S7Comm Connect Error:" + ex.Message);
                return _isConnect = false;
            }
        }
        

      
        public bool DisConnect()
        {
            if (IsConnect)
            {
                _socket.Close();

            }
            _socket?.Dispose();
            _socket = null;
            _isConnect = false;
            return true;
        }

        public void Dispose()
        {
            _socket?.Dispose();
            _socket = null;
        }
        /// <summary>
        /// A[1]~A[2]: 03 00  固定报文头；
        /// A[3] ~A[4]: 00 1F  整个读取请求长度为0x1F= 31 ;
        /// A[5]~A[11]: 02 F0 80 32  01 00 00 固定6个字节；
        /// A[12] ~A[13]: 两个字节，标识序列号，回复报文相同位置和这个完全一样；范围是0 ~65535；
        /// A[14] ~A[23]:00 0E 00 00 04 01 12 0A 10 02   固定10个字节
        /// A[24]~A[25]:两个字节,访问数据的个数，以byte为单位；
        /// A[26] ~A[27]: DB块的编号，比如DB50, 就是0x32=50, 两个字节，范围是0 ~65535（也许是一个1个字节，因为没有设置估DB255以上的数据块，所以不知道到底是几个字节，姑且认为是2个字节）；
        /// A[28]: 访问数据块的类型：0x81-input ,0x82-output ,0x83-flag , 0x84-DB(这个最常见);
        /// A[29] ~A[31]: 访问DB块的偏移量offset(地址+1以byte为单位)并乘以8; 3个字节，范围是0 ~16777216（一般 用不到这么大）
        /// 程序设计的时候，其实主要关注最后4个信息，即：
        /// 1. A[24] ~A[25]: 访问byte个数
        /// 2. A[26] ~A[27]: DB块编号
        /// 3. A[28]: 数据块类型
        /// 4.A[29] ~A[31]  :访问地址偏移量;相当于首地址编号
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private byte[] createReadHeader(DeviceAddress address,int handler,int length)
        {
            string header = string.Concat("0300001F02F08032010000", 
                handler.ToString("X").PadLeft(4,'0'), 
                "000E00000401120A1002", 
                length.ToString("X").PadLeft(4,'0'),
                address.AreaID.ToString("X").PadLeft(4,'0'),
                address.FuctionCode.ToString("X").PadLeft(2,'0'),
                (address.Address*8).ToString("X").PadLeft(6,'0')
                );

           return NetConvert.GetHexBytes(header);
        }
        /// <summary>
        /// 根据以上4个报文的demo，摸索出大致规律如下(未必完全正确，但是应付项目可以了)；
        /// A[1] ~A[2]: 03 00  固定报文头；
        /// A[3] ~A[4]: 整个报文长度：35+写入长度;
        /// A[5] ~A[11]:  02 F0 80 32  01 00 00 固定6个字节(和读取的完全一样)
        /// A[12] ~A[13]: 两个字节，标识序列号，回复报文相同位置和这个完全一样；范围是0 ~65535；
        /// A[14] ~A[15]:00 0E 固定2个字节；
        /// A[16] ~A[17]:写入长度+4；
        /// A[18] ~A[22]: 05 01 12 0A 10 固定5个自己
        /// A[23]: 写入方式： 01-按bit写入； 02-按byte写入；
        /// A[24] ~A[25]:两个字节,写入数据的个数（可能是byte或bit, 按A[23] 来区分）
        /// A[26] ~A[27]: DB块的编号
        /// A[28]: 写入数据块的类型：0x81-input ,0x82-output ,0x83-flag , 0x84-DB(这个最常见);
        /// A[29] ~A[31]: 写入DB块的偏移量offset(地址+1以byte为单位)*8; 3个字节，范围是0 ~16777216（一般 用不到这么大）
        /// A[32] ~A[33]：写入方式为： 03-按bit写入; 04-按byte写入；  
        /// A[34] ~A[35]：写入bit的个数(bit为单位)
        /// A[36] ~最后 ： 连续的写入值；
 
        /// 注意点：
        /// 1.写入可以按byte和bit两种方法去操作；
        /// 2.对于byte，可以一口气写连续多个byte, 理论上一条指令连续写bit也可以，但是实践下来，发现有问题，所以对于bit操作，我们就一个一个写吧；
        /// </summary>
        /// <param name="address"></param>
        /// <param name="handler"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private byte[] createWriteHeader(DeviceAddress address, int handler, byte[] source)
        {
            var length = source.Length;

            byte[] result = new byte[35 + length];

            string header = string.Concat("0300",
                (35 + length).ToString("X").PadLeft(4, '0'),
                "02F08032010000",
                handler.ToString("X").PadLeft(4, '0'),
                "000E",
                (length + 4).ToString("X").PadLeft(4, '0'),
                "0501120A10",
                "02",
                length.ToString("X").PadLeft(4, '0'),
                address.AreaID.ToString("X").PadLeft(4, '0'),
                address.FuctionCode.ToString("X").PadLeft(2, '0'),
                (address.Address*8).ToString("X").PadLeft(6, '0'),
                "0004",
                (length * 8).ToString("X").PadLeft(4, '0')
                );

            var headerbyte = NetConvert.GetHexBytes(header);
            Array.Copy(headerbyte, result, 35);
            Array.Copy(source, 0, result, 35, length);
            return result;

        }
        private byte[] createWriteHeader(DeviceAddress address, int handler, bool source)
        {
            var length = 1;

            byte[] result = new byte[35 + length];

            string header = string.Concat("0300",
                (35 + length).ToString("X").PadLeft(4, '0'),
                "02F08032010000",
                handler.ToString("X").PadLeft(4, '0'),
                "000E",
                (length + 4).ToString("X").PadLeft(4, '0'),
                "0501120A10",
                "01",
                length.ToString("X").PadLeft(4, '0'),
                address.AreaID.ToString("X").PadLeft(4, '0'),
                address.FuctionCode.ToString("X").PadLeft(2, '0'),
                (address.Address*8+address.BitAddress).ToString("X").PadLeft(6, '0'),
                "0003",
                length.ToString("X").PadLeft(4, '0')
                );
            var headerbyte = NetConvert.GetHexBytes(header);
            Array.Copy(headerbyte, result, 35);
            if (source)
            {
                result[35] = 1;
            }
            return result;
        }
       
        /// <summary>
        /// 将字符串地址转换成数据结构地址
        /// </summary>
        /// <param name="address">字符串地址，例如：DB200.DBW0</param>
        /// <returns></returns>
        public DeviceAddress GetDeviceAddress(string address)
        {
            DeviceAddress result = new DeviceAddress();
            var strArrary = address.Split('.');
            if (strArrary.Length >= 2)
            {
                if (strArrary[0].Contains("DB"))
                {
                    result.FuctionCode = 0x84;
                    result.AreaID = int.Parse(strArrary[0].Substring(2));
                    result.Address = int.Parse(strArrary[1].Substring(3));
                }
            }
            else if (strArrary.Length == 3)
            {
                if (strArrary[0].Contains("DB"))
                {
                    result.BitAddress = int.Parse(strArrary[2].Substring(0));
                }
            }
            return result;
        }

        private static readonly object _async = new object();

        /// <summary>
        /// B[1]~B[2]: 03 00 固定报文头
        /// B[3] ~B[4]: 整个读取回复报文长度：25+读取长度;
        /// B[5] ~B[11]:  02 F0 80 32  03 00 00 固定6个字节,和读取请求相同的位置几乎一样，就 B[9] = 0x03; A[9]=0x01;
        /// B[12] ~B[13]: 两个字节，标识序列号，回复报文相同位置和这个完全一样；范围是0 ~65535；
        /// B[14] ~B[15]: 两个字节，固定为00 02；对应读取位置是 00 0E；正好 02+0E=10 ;有点补码的感觉，其实不需要关注规律，反正是固定的；
        /// B[16] ~B[17]:两个字节，=请求读取的字节数+4；
        /// B[18] ~B[23]:6个字节，固定为:00 00 04 01 FF 04 ;
        /// B[24] ~B[25]:两个字节, 请求访问的byte个数*8 ；其实就是以二进制为单位的个数；由此可以看出，一口气最多访问的地址个数是8192；
        /// B[26] ~最后一个   ：以offset作为首地址，所对应的各个byte的值；
        /// 程序设计的时候，其实只要关注两个信息：
        /// 1.校验B[3] ~B[4]:校验长度正确；
        /// 2.B[26] ~最后一个  :获取对应的值；
        /// 到这里读的处理就算结束了；
        /// 几个小注意点:
        /// 1.对于不同信号的PLC,除了初始化的CPUSolt不同；正常读/写指令是一样的；
        /// 2.读的时候，都是以byte为单位的，如果程序只需要bit，那么还是以Byte为单位去读，将读出的部分按bit再去分解；
        /// 3.flag类型到底是什么，不是很清楚，有点类似三菱里的M点；这个也不需要去深究，一般项目里主要就是用DB块；
        /// 4.读取的长度如果是N(以byte为单位)，那么返回的长度就是N*8(以bit为单位）；怎么判断长度是否要*8；主要看后面是不是紧挨着数据，如果是数据，就需要*8；offset都是以bit为单位的；
        /// 5.正常读的操作都是DB块，所以在A[26] ~A[27]这个字节写入DB块的编号，但是对于input,output,flags这三个类型，是不需要数据块编号的不过我们可以随便写一个DB编号；
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] readyBytes(DeviceAddress address,int length)
        {
            byte[] result = null;
            if (IsConnect)
            {
                if (address.FuctionCode == 0x81 || address.FuctionCode == 0x82 || address.FuctionCode == 0x83 || address.FuctionCode == 0x84)
                {
                    lock (_async)
                    {
                        int times = 0;
                        while (result == null && times < RetryTimes)
                        {
                            _handler++;
                            if (_handler > 65535)
                                _handler = 0;
                            byte[] sendBytes = createReadHeader(address, _handler, length);
                            byte[] receiveBytes = new byte[25 + length];
                            byte[] dataBytes = new byte[length];
                            _log.DebugLog($"{Name}:Tx => 0x {NetConvert.GetHexString(sendBytes)}");
                            try
                            {
                                _socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);

                                //Thread.Sleep(10);
                                int readNumber = 0;

                                _socket.Receive(receiveBytes, 4, SocketFlags.None);
                                _socket.Receive(receiveBytes, 4, receiveBytes[3] - 4, SocketFlags.None);
                                readNumber = receiveBytes[3];

                                //报文数据记录
                                var logBytes = new byte[readNumber];
                                Array.Copy(receiveBytes, 0, logBytes, 0, readNumber);
                                _log.DebugLog($"{Name}:Re <= 0x {NetConvert.GetHexString(logBytes)}");

                                if (readNumber == receiveBytes.Length)
                                {
                                    Array.Copy(receiveBytes, 25, dataBytes, 0, length);
                                    result = dataBytes;
                                }
                                else
                                {
                                    _log.ErrorLog(Name + ": S7Comm read data error:" + Convert.ToString(receiveBytes));
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ErrorLog(string.Format("{0}: S7Comm {1} ",Name, ex.Message));
                            }
                        }
                    }
                }
                else
                {
                    _log.ErrorLog(Name + ": S7Comm read function error:" + address.FuctionCode);
                }
            }
            else
            {
                _log.ErrorLog(Name + ": S7Comm  read data error:" + "No connect");
            }
            return result;
        }

        public Item<bool> ReadBool(DeviceAddress deviceAddress)
        {
            var datas = readyBytes(deviceAddress, 1);
            return datas == null ? Item<bool>.CreateDefault() :
                new Item<bool>() { Vaule = NetConvert.ByteToBool(datas[0], deviceAddress.BitAddress), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<bool>[] ReadBools(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readyBytes(deviceAddress, (int)Math.Ceiling((double)(deviceAddress.BitAddress+ length)/8));
            var bools = NetConvert.BytesToBools(datas,deviceAddress.BitAddress, length);
            return NetConvertExtension.ToItems(bools, 0, length);

        }
        public Item<byte> ReadByte(DeviceAddress deviceAddress)
        {
            var datas = readyBytes(deviceAddress, 1);
            return datas==null? Item<byte>.CreateDefault() : new Item<byte>() { Vaule =datas[0], UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }
        public Item<byte>[] ReadBytes(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readyBytes(deviceAddress, length);
            return NetConvertExtension.ToItems(datas, 0, length);
        }

        public Item<TResult> ReadData<TResult>(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<TResult>[] ReadDatas<TResult>(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<float> Readfloat(DeviceAddress deviceAddress)
        {
            var datas = readyBytes(deviceAddress, 4);
            return datas == null ? Item<float>.CreateDefault() : 
                new Item<float>() { Vaule = UnsafeNetConvert.BytesToFloat(datas, 0, Order),UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readyBytes(deviceAddress, 4* length);
            var floats = UnsafeNetConvert.BytesToFloats(datas, 0, length, Order);
            return NetConvertExtension.ToItems(floats, 0, length);
        }

        public Item<int> ReadInt(DeviceAddress deviceAddress)
        {
            var datas = readyBytes(deviceAddress, 4);
            return datas == null ? Item<int>.CreateDefault() :
                new Item<int>() { Vaule = UnsafeNetConvert.BytesToInt(datas, 0, Order), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<int>[] ReadInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readyBytes(deviceAddress, 4*length);
            var ints = UnsafeNetConvert.BytesToInts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(ints, 0, length);
        }

        public Item<short> ReadShort(DeviceAddress deviceAddress)
        {
            var datas = readyBytes(deviceAddress, 2);
            return datas == null ? Item<short>.CreateDefault() :
                new Item<short>() { Vaule = UnsafeNetConvert.BytesToShort(datas, 0, Order), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<short>[] ReadShorts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readyBytes(deviceAddress, 2 * length);
            var shorts = UnsafeNetConvert.BytesToShorts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(shorts, 0, length);
        }

        public Item<string> ReadString(DeviceAddress deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Item<string>[] ReadStrings(DeviceAddress deviceAddress, ushort length)
        {
            throw new NotImplementedException();
        }

        public Item<uint> ReadUInt(DeviceAddress deviceAddress)
        {
            var datas = readyBytes(deviceAddress, 4);
            return datas == null ? Item<uint>.CreateDefault() :
                new Item<uint>() { Vaule = UnsafeNetConvert.BytesToUInt(datas, 0, Order), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<uint>[] ReadUInts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readyBytes(deviceAddress, 4 * length);
            var Uints = UnsafeNetConvert.BytesToUInts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(Uints, 0, length);
        }

        public Item<ushort> ReadUShort(DeviceAddress deviceAddress)
        {
            var datas = readyBytes(deviceAddress, 2);
            return datas == null ? Item<ushort>.CreateDefault() :
                new Item<ushort>() { Vaule = UnsafeNetConvert.BytesToUShort(datas, 0, Order), UpdateTime = DateTime.Now, Quality = QUALITIES.QUALITY_GOOD };
        }

        public Item<ushort>[] ReadUShorts(DeviceAddress deviceAddress, ushort length)
        {
            var datas = readyBytes(deviceAddress, 2 * length);
            var ushorts = UnsafeNetConvert.BytesToUShorts(datas, 0, length, Order);
            return NetConvertExtension.ToItems(ushorts, 0, length);
        }
        /// B[1] ~B[2]: 03 00 固定报文头；
        /// B[22]: FF 标识写入正常；
        private int writedata(DeviceAddress deviceAddress,byte[] source)
        {
            int result = -1;

            if (IsConnect)
            {
                if (deviceAddress.FuctionCode == 0x81 || deviceAddress.FuctionCode == 0x82 || deviceAddress.FuctionCode == 0x83 || deviceAddress.FuctionCode == 0x84)
                {
                    int times = 0;
                    while (result != 1 && times < RetryTimes)
                    {
                        try
                        {
                            byte[] receiveBytes = new byte[22];
                            _log.DebugLog($"{Name}:Tx => 0x {NetConvert.GetHexString(source)}");
                            _socket.Send(source, source.Length, SocketFlags.None);

                            //Thread.Sleep(10);
                            int readNumber = 0;

                            _socket.Receive(receiveBytes, 4, SocketFlags.None);
                            _socket.Receive(receiveBytes, 4, receiveBytes[3] - 4, SocketFlags.None);
                            readNumber = receiveBytes[3];

                            //报文数据记录
                            var logBytes = new byte[readNumber];
                            Array.Copy(receiveBytes, 0, logBytes, 0, readNumber);
                            _log.DebugLog($"{Name}:Re <= 0x {NetConvert.GetHexString(logBytes)}");

                            if (readNumber == receiveBytes.Length)
                            {
                                result = receiveBytes[21] == 0xFF ? 1 : -1;
                            }
                            else
                            {
                                //string byteSteamString = "";
                                //foreach (byte bt in receiveBytes)
                                //{
                                //    byteSteamString += string.Format("{0:x2}", bt) + " ";
                                //}
                                _log.ErrorLog(Name + ":S7Comm write data error: receive byte " + NetConvert.GetHexString(receiveBytes));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.ErrorLog(Name + string.Format(":S7Comm {0} ", ex.Message));
                        }
                    }
                }
                else
                {
                    _log.ErrorLog(Name + ":S7Comm write function error:" + deviceAddress.FuctionCode);
                }
            }
            else
            {
                _log.ErrorLog(Name + ":S7Comm  write data error:" + "No connect");
            }
            return result;
        }

        public int WriteBool(DeviceAddress deviceAddress, bool datas, int offset = 0)
        {
            if (deviceAddress.AreaID==0x84)
            {
                var length = deviceAddress.BitAddress + offset;
                deviceAddress.BitAddress = length % 8;
                deviceAddress.Address = length / 8;
            }
            lock (_async)
            {
                _handler=_handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, datas);
                return writedata(deviceAddress, source);
            }
        }

        public int WriteBools(DeviceAddress deviceAddress, bool[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteByte(DeviceAddress deviceAddress, byte datas,int offset=0)
        {
            if (deviceAddress.AreaID == 0x84)
            {
                deviceAddress.Address += offset;
            }
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler,new byte[] { datas });
                return writedata(deviceAddress, source);
            }
        }

        public int WriteBytes(DeviceAddress deviceAddress, byte[] datas)
        {
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, datas);
                return writedata(deviceAddress, source);
            }
        }

        public int WriteFloat(DeviceAddress deviceAddress, float datas,int offset =0)
        {
            if (deviceAddress.AreaID == 0x84)
            {
                deviceAddress.Address += offset * 4;
            }
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler,UnsafeNetConvert.FloatToBytes( datas,ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteFloats(DeviceAddress deviceAddress, float[] datas)
        {
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.FloatsToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteInt(DeviceAddress deviceAddress, int datas, int offset = 0)
        {
            if (deviceAddress.AreaID == 0x84)
            {
                deviceAddress.Address += offset*4;
            }
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.IntToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteInts(DeviceAddress deviceAddress, int[] datas)
        {
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.IntsToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteShort(DeviceAddress deviceAddress, short datas, int offset = 0)
        {
            if (deviceAddress.AreaID == 0x84)
            {
                deviceAddress.Address += offset*2;
            }
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.ShortToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteShorts(DeviceAddress deviceAddress, short[] datas)
        {
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.ShortsToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteString(DeviceAddress deviceAddress, string datas , int offset = 0)
        {
            throw new NotImplementedException();
        }

        public int WriteStrings(DeviceAddress deviceAddress, string[] datas)
        {
            throw new NotImplementedException();
        }

        public int WriteUInt(DeviceAddress deviceAddress, uint datas, int offset = 0)
        {
            if (deviceAddress.AreaID == 0x84)
            {
                deviceAddress.Address += offset * 4;
            }
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.UIntToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteUInts(DeviceAddress deviceAddress, uint[] datas)
        {
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.UIntsToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteUShort(DeviceAddress deviceAddress, ushort datas, int offset = 0)
        {
            if (deviceAddress.AreaID == 0x84)
            {
                deviceAddress.Address += offset * 2;
            }
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.UShortToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

        public int WriteUShorts(DeviceAddress deviceAddress, ushort[] datas)
        {
            lock (_async)
            {
                _handler = _handler++ >= 65535 ? 0 : _handler;
                var source = createWriteHeader(deviceAddress, _handler, UnsafeNetConvert.UShortsToBytes(datas, ByteOrder.BigEndian));
                return writedata(deviceAddress, source);
            }
        }

      
    }
}
