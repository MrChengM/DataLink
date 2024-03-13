using DataServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Utillity.Data;

namespace ModbusDrivers.Client
{
    /// <summary>
    /// Modbus TCP Client 协议
    /// </summary>
    [DriverDescription("Modbus TCP",CommunicationType.Ethernet)]
    public sealed class ModbusTCPClient:ModbusClient
    {
        private EthernetSetUp _ethernetSetUp =new EthernetSetUp();
        private Socket _socket;

        public override bool IsClose
        {
            get
            {
                return _socket==null||_socket.Connected==false;
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
        public ModbusTCPClient() 
        {
            ConnectTimeOut = 3000;
            RequestTimeOut = 1000;
            RetryTimes = 1;
        }
        public ModbusTCPClient(EthernetSetUp ethernetSetUp)
        {
            ConnectTimeOut = 3000;
            RequestTimeOut = 1000;
            RetryTimes = 1;
            _ethernetSetUp = ethernetSetUp;
        }
        //public ModbusTCPClient(EthernetSetUp ethernetSetUp, TimeOut timeOut,ILog log)
        //{
        //    _ethernetSetUp = ethernetSetUp;
        //    TimeOut = timeOut;
        //    Log = log;
        //}

        public override bool Connect()
        {
            try
            {
                if (_socket == null)
                    _socket = new Socket(SocketType.Stream, _ethernetSetUp.ProtocolType);
                //if (TimeOut.TimeOutSet < 1000)
                //    TimeOut.TimeOutSet = 1000;
                _socket.SendTimeout = RequestTimeOut;
                _socket.ReceiveTimeout = RequestTimeOut;
                IPAddress ipaddress;
                if (IPAddress.TryParse(_ethernetSetUp.IPAddress, out ipaddress))
                {
                    _socket.Connect(ipaddress, _ethernetSetUp.PortNumber);
                    return IsConnect = true;
                }
                else
                {
                    Log.ErrorLog($"{Name}: IP地址无效");
                    return IsConnect = false;
                }
            }
            catch(Exception ex)
            {
                Log.ErrorLog($"{Name}: ModbusTCP Connect Error:" + ex.Message);
                return IsConnect = false;
            }
        }
        public override bool DisConnect()
        {
            if (IsConnect)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket.Dispose();
                _socket = null;
                IsConnect = false;
            }
            return true;

        }

        protected override byte[] getReadHeader(byte slaveId, byte func, ushort startAddress, ushort count)
        {
            byte[] sendBytes = new byte[12];

            //TCP报文头5个0，1个6
            for(int i = 0; i < 5; i++)
            {
                sendBytes[i] = 0;
            }
            sendBytes[5] = 6;

            //具体的请求数据，无校验位
            sendBytes[6] = slaveId;
            sendBytes[7] = func;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);//起始位为0，偏移1位
            sendBytes[8] = addressBytes[1];//高位在前
            sendBytes[9] = addressBytes[0];//低位在后
            byte[] countBytes = BitConverter.GetBytes(count);
            sendBytes[10] = countBytes[1];//高位在前
            sendBytes[11] = countBytes[0];//低位在后
            return sendBytes;
        }
        protected override byte[] getWriteSigCoilHeader(byte slaveID, ushort startAddress, bool value)
        {
            byte[] sendBytes = new byte[12];
            sendBytes[5] = 6;
            sendBytes[6] = slaveID;
            sendBytes[7] = (byte)FunctionCode.ForceSingleCoil;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[8] = addressBytes[1];//高位在前
            sendBytes[9] = addressBytes[0];//低位在后
            if(value)
            sendBytes[10] = 0xFF;
            return sendBytes;
        }

        /// <summary>
        /// 获取写多个寄存器报文头
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="value">数组长度为2</param>
        /// <returns></returns>
        protected override byte[] getWriteSigRegisterHeader(byte slaveID, ushort startAddress, byte[] value)
        {
            byte[] sendBytes = new byte[12];
            sendBytes[5] = 6;
            sendBytes[6] = slaveID;
            sendBytes[7] = (byte)FunctionCode.WriteSingleRegister;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[8] = addressBytes[1];//高位在前
            sendBytes[9] = addressBytes[0];//低位在后
            sendBytes[10] = value[1];
            sendBytes[11] = value[0];
            return sendBytes;
        }

        protected override byte[] getWriteMulCoilHeader(byte slaveID, ushort startAddress,  bool[] value)
        {
            byte[] datas = NetConvert.BoolstoBytes(value);
            byte[] sendBytes = new byte[13 + datas.Length];
            sendBytes[5] = (byte)(7 + datas.Length); //后续报文字节数
            sendBytes[6] = slaveID;
            sendBytes[7] =(byte) FunctionCode.ForceMulCoil;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[8] = addressBytes[1];//高位在前
            sendBytes[9] = addressBytes[0];//低位在后
            byte[] CountBytes = BitConverter.GetBytes((ushort)value.Length);
            sendBytes[10] = CountBytes[1];
            sendBytes[11] = CountBytes[0];
            sendBytes[12] = (byte)datas.Length;
            Array.Copy(datas, 0, sendBytes, 13, datas.Length);
            return sendBytes;
        }
        protected override byte[] getWriteMulRegisterHeader(byte slaveID, ushort startAddress, byte[] value)
        {
            byte[] sendBytes = new byte[13 + value.Length];
            sendBytes[5] = (byte)(7 + value.Length); //后续报文字节数
            sendBytes[6] = slaveID;
            sendBytes[7] = (byte)FunctionCode.WriteMulRegister;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[8] = addressBytes[1];//高位在前
            sendBytes[9] = addressBytes[0];//低位在后
            byte[] CountBytes = BitConverter.GetBytes((ushort)(value.Length/2));
            sendBytes[10] = CountBytes[1];
            sendBytes[11] = CountBytes[0];
            sendBytes[12] = (byte)value.Length;
            //value = UnsafeNetConvert.BytesPerversion(value);//前面数据已翻转
            Array.Copy(value, 0, sendBytes, 13, value.Length);
            return sendBytes;
        }

        /// <summary>
        /// 读数据
        /// 0x01读线圈，地址：00001-09999，类型：bit
        /// 0x02读输入状态，地址：10001-19999，类型：bit
        /// 0x03读保持寄存器，地址：40001-49999，类型：Word
        /// 0x04读输入寄存器，地址：30001-39999，类型：Word
        /// </summary>
        /// <returns>返回带CRC校验8位字节数组</returns>
        object _async = new object();
        protected override byte[] readBytes(byte slaveID, ushort startAddress, byte funcCode, ushort count)
        {
            byte[] result = null;
            if (IsConnect)
            {
                byte byteCount = Function.GetReadBytesCount(funcCode, count);
                if (byteCount == 0)
                {
                    Log.ErrorLog(string.Format("{0},Modbus 读功能码错误", Name));
                }
                else
                {
                    byte[] sendBytes = getReadHeader(slaveID, funcCode, startAddress, count);
                    byte errorFuncCode = (byte)(0x80 + funcCode);
                    lock (_async)
                    {
                        int times = 0;
                        while (result == null && times < RetryTimes)
                        {
                            try
                            {
                                byte[] receiveBytes = new byte[3 + byteCount];
                                byte[] dataBytes = new byte[byteCount];
                                List<byte> reciveBytesLog = new List<byte>();
                                _socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
                                Log.DebugLog($"{Name}:Tx => {NetConvert.GetHexString(sendBytes)}");
                                Thread.Sleep(10);
                                int index = 0;
                                bool continueFlag = true;
                                /*----------------------------------------
                                *循环找头：
                                * 先读一个字节判断是否为SlaveID
                                * 如果是,则开启循环
                                * 则再读一个字节判断是否为功能码或者是错误码
                                * 判断是，则指针index+2并跳出循环
                                * 若第二个字节等于SlaveID则复制给头
                                * 否则将头置0
                             ------------------------------------------ */
                                while (continueFlag)
                                {
                                    if (index < 2)
                                    {
                                        _socket.Receive(receiveBytes, 0, 1, SocketFlags.None);
                                        //接受到数据记录
                                        reciveBytesLog.Add(receiveBytes[0]);
                                        while (receiveBytes[0] == slaveID)
                                        {
                                            _socket.Receive(receiveBytes, 1, 1, SocketFlags.None);
                                            //接受到数据记录
                                            reciveBytesLog.Add(receiveBytes[1]);

                                            if (receiveBytes[1] == funcCode || receiveBytes[1] == errorFuncCode)
                                            {
                                                index += 2;
                                                break;
                                            }
                                            else if (receiveBytes[1] == slaveID)
                                            {
                                                receiveBytes[0] = receiveBytes[1];
                                            }
                                            else
                                            {
                                                receiveBytes[0] = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (receiveBytes[1] == funcCode)
                                        {
                                            index += _socket.Receive(receiveBytes, index, byteCount + 1, SocketFlags.None);
                                            continueFlag = index == receiveBytes.Length ? false : true;
                                            //接受到数据记录
                                            for (int i = 2; i < receiveBytes.Length; i++)
                                            {
                                                reciveBytesLog.Add(receiveBytes[i]);
                                            }
                                        }
                                        else if (receiveBytes[1] == errorFuncCode)
                                        {
                                            index += _socket.Receive(receiveBytes, index, 1, SocketFlags.None);
                                            continueFlag = index == 3 ? false : true;
                                            //接受到数据记录
                                            reciveBytesLog.Add(receiveBytes[3]);
                                        }
                                    }
                                }
                                Log.DebugLog($"{Name}:Re <=> {NetConvert.GetHexString(reciveBytesLog.ToArray())}");

                                //获取正确报文并处理
                                if (receiveBytes[1] == funcCode)
                                {
                                    Array.Copy(receiveBytes, 3, dataBytes, 0, byteCount);
                                    result = dataBytes;
                                }
                                else if (receiveBytes[1] == errorFuncCode)
                                {
                                    Log.ErrorLog(string.Format("{0}: Modbus {1} ",Name, Function.GetErrorString(receiveBytes[2])));
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ErrorLog(string.Format("{0}: Modbus {1} ",Name, ex.Message));
                            }
                        }
                    }
                }
            }
            return result;
        }
        protected override int writeBytes( byte[] sendBytes)
        {
            int result = -1;
            if (IsConnect)
            {
                byte slaveID = sendBytes[6];
                byte funcCode = sendBytes[7];
                byte errorFuncCode = (byte)(0x80 + funcCode);
                lock (_async)
                {
                    int times = 0;
                    while (result != 1 && times < RetryTimes)
                    {
                        List<byte> reciveBytesLog = new List<byte>();
                        try
                        {

                            int reseult = _socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
                            Log.DebugLog($"{Name}:Tx => {NetConvert.GetHexString(sendBytes)}");

                            Thread.Sleep(10);
                            int index = 0;
                            bool continueFlag = true;
                            byte[] receiveBytes = new byte[6];
                            /*----------------------------------------
                             *循环找头：
                             * 先读一个字节判断是否为SlaveID
                             * 如果是,则开启循环
                             * 则再读一个字节判断是否为功能码或者是错误码
                             * 判断是，则指针index+2并跳出循环
                             * 若第二个字节等于SlaveID则复制给头
                             * 否则将头置0
                             ------------------------------------------ */

                            while (continueFlag)
                            {
                                if (index < 2)
                                {
                                    _socket.Receive(receiveBytes, 0, 1, SocketFlags.None);
                                    reciveBytesLog.Add(receiveBytes[0]);
                                    while (receiveBytes[0] == slaveID)
                                    {
                                        _socket.Receive(receiveBytes, 1, 1, SocketFlags.None);
                                        reciveBytesLog.Add(receiveBytes[1]);

                                        if (receiveBytes[1] == funcCode || receiveBytes[1] == errorFuncCode)
                                        {
                                            index += 2;
                                            break;
                                        }
                                        else if (receiveBytes[1] == slaveID)
                                        {
                                            receiveBytes[0] = receiveBytes[1];
                                        }
                                        else
                                        {
                                            receiveBytes[0] = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    if (receiveBytes[1] == funcCode)
                                    {
                                        index += _socket.Receive(receiveBytes, index, 4, SocketFlags.None);
                                        continueFlag = index == 6 ? false : true;
                                        for (int i = 2; i < 6; i++)
                                        {
                                            reciveBytesLog.Add(receiveBytes[i]);
                                        }
                                    }
                                    else if (receiveBytes[1] == errorFuncCode)
                                    {
                                        index += _socket.Receive(receiveBytes, index, 1, SocketFlags.None);
                                        continueFlag = index == 3 ? false : true;
                                        reciveBytesLog.Add(receiveBytes[2]);
                                        Log.ErrorLog(string.Format("{0}: Modbus {0} ",Name, Function.GetErrorString(receiveBytes[2])));
                                    }
                                    Log.DebugLog($"{Name}:Re => {NetConvert.GetHexString(reciveBytesLog.ToArray())}");

                                }
                            }
                            result = receiveBytes[1] == funcCode?1:-1;
                        }
                        catch (Exception ex)
                        {
                            Log.ErrorLog(string.Format("{0}: Modbus {0} ",Name, ex.Message));
                        }
                    }
                }
            }
            return result;
        }
        public override void Dispose()
        {
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            _socket = null;
            _ethernetSetUp = null;
            IsConnect = false;
        }
    }
}
