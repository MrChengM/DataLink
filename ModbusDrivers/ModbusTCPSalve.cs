using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusDrivers
{
    public sealed class ModbusTCPSalve:ModbusSalve
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
        public ModbusTCPSalve() { }
        public ModbusTCPSalve(EthernetSetUp ethernetSetUp, TimeOut timeOut,ILog log)
        {
            _ethernetSetUp = ethernetSetUp;
            TimeOut = timeOut;
            Log = log;
            DriType = DriverType.Ethernet;
        }

        public override bool Connect()
        {
            try
            {
                if (_socket == null)
                    _socket = new Socket(SocketType.Stream, _ethernetSetUp.ProtocolType);
                if (TimeOut.TimeOutSet < 1000)
                    TimeOut.TimeOutSet = 1000;
                _socket.SendTimeout = (int)TimeOut.TimeOutSet;
                _socket.ReceiveTimeout = (int)TimeOut.TimeOutSet;
                IPAddress ipaddress;
                if (IPAddress.TryParse(_ethernetSetUp.IPAddress, out ipaddress))
                {
                    _socket.Connect(ipaddress, _ethernetSetUp.PortNumber);
                    return IsConnect = true;
                }
                else
                {
                    Log.ErrorLog("IP地址无效");
                    return IsConnect = false;
                }
            }
            catch(Exception ex)
            {
                Log.ErrorLog("ModbusTCP Connect Error:" + ex.Message);
                return IsConnect = false;
            }
        }

        public override bool DisConnect()
        {
            _socket.Disconnect(false);
             IsConnect = false;
            return true;
        }

        protected override byte[] readHeader(byte slaveId, byte func, ushort startAddress, ushort count)
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
            try
            {
                if (IsConnect)
                {
                    byte byteCount = Function.GetReadBytesCount(funcCode, count);
                    if (byteCount == 0)
                    {
                        Log.ErrorLog("Modbus 读功能码错误");
                        return null;
                    }
                    byte[] sendBytes = readHeader(slaveID, funcCode, startAddress, count);
                    lock (_async)
                    {
                        byte[] receiveBytes = new byte[3 + byteCount];
                        byte[] dataBytes = new byte[byteCount];
                        byte errorFuncCode = (byte)(0x80 + funcCode);
                        _socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
                        Thread.Sleep(10);
                        int index = 0;
                        bool continueFlag = true;
                        TimeOut.Init();
                        /*----------------------------------------
                        *循环找头：
                        * 先读一个字节判断是否为SlaveID
                        * 如果是,则开启循环
                        * 则再读一个字节判断是否为功能码或者是错误码
                        * 判断是，则指针index+2并跳出循环
                        * 若第二个字节等于SlaveID则复制给头
                        * 否则将头置0
                     ------------------------------------------ */
                        while (TimeOut.TimeOutFlag & continueFlag)
                        {
                            if (index < 2)
                            {
                                _socket.Receive(receiveBytes,0, 1, SocketFlags.None);
                                while (receiveBytes[0] == slaveID)
                                {
                                    _socket.Receive(receiveBytes,1,1, SocketFlags.None);
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
                                    index += _socket.Receive(receiveBytes, index, byteCount +1, SocketFlags.None);
                                    continueFlag = index == receiveBytes.Length ? false : true;
                                }
                                else if (receiveBytes[1] == errorFuncCode)
                                {
                                    index += _socket.Receive(receiveBytes, index,3, SocketFlags.None);
                                    continueFlag = index == 5 ? false : true;
                                }
                            }
                            TimeOut.EndTime = DateTime.Now;
                        }
                        //判断是否超时，并复位
                        if (TimeOut.TimeOutFlag)
                        {
                            TimeOut.LogTimeOutError();
                            return null;
                        }
                        //获取正确报文并处理
                        if (receiveBytes[1] == funcCode)
                        {
                            Array.ConstrainedCopy(receiveBytes, 3, dataBytes, 0, byteCount);
                            return dataBytes;
                        }
                        else if (receiveBytes[1] == errorFuncCode)
                        {
                            Log.ErrorLog(string.Format("Modbus {0} ", Function.GetErrorString(receiveBytes[2])));
                        }
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(string.Format("Modbus {0} ", ex.Message));
                return null;
            }
        }
        protected override byte[] writeSigHeader(byte slaveID, ushort startAddress, byte funcCode, byte[] datas)
        {
            byte[] sendBytes = new byte[12];
            sendBytes[5] = 6;
            sendBytes[6] = slaveID;
            sendBytes[7] = funcCode;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[8] = addressBytes[1];//高位在前
            sendBytes[9] = addressBytes[0];//低位在后
            sendBytes[10] = datas[1];
            sendBytes[11] = datas[0];
            return sendBytes;
        }

        protected override byte[] writeMulHeader(byte slaveID, ushort startAddress, byte funcCode, byte[] datas)
        {
            byte[] sendBytes = new byte[13 + datas.Length];
            sendBytes[5] = 6;
            sendBytes[6] = slaveID;
            sendBytes[7] = funcCode;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[8] = addressBytes[1];//高位在前
            sendBytes[9] = addressBytes[0];//低位在后
            byte[] CountBytes = BitConverter.GetBytes((ushort)datas.Length);
            sendBytes[10] = CountBytes[1];
            sendBytes[11] = CountBytes[0];

            sendBytes[12] = (byte)datas.Length;
            Array.ConstrainedCopy(datas, 0, sendBytes, 13, datas.Length);

            return sendBytes;
        }

        protected override int writeDatas(byte slaveID, byte funcCode, ushort startAddress, byte[] datas, ushort count, GetWriteHeader getHeader)
        {
            try
            {
                if (IsConnect)
                {
                    byte errorFuncCode = (byte)(0x80 + funcCode);
                    byte[] sendBytes = getHeader(slaveID, startAddress, funcCode, datas);

                    lock (_async)
                    {
                        _socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
                        Thread.Sleep(10);

                        int index = 0;
                        bool continueFlag = true;
                        TimeOut.Init();
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

                        while (TimeOut.TimeOutFlag & continueFlag)
                        {
                            if (index < 2)
                            {
                                _socket.Receive(receiveBytes, 0, 1, SocketFlags.None);
                                while (receiveBytes[0] == slaveID)
                                {
                                    _socket.Receive(receiveBytes, 1, 1, SocketFlags.None);
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
                                    continueFlag = index == 4 ? false : true;
                                    return 1;
                                }
                                else if (receiveBytes[1] == errorFuncCode)
                                {
                                    index += _socket.Receive(receiveBytes, index, 1, SocketFlags.None);
                                    continueFlag = index == 3 ? false : true;
                                    Log.ErrorLog(string.Format("Modbus {0} ", Function.GetErrorString(receiveBytes[2])));
                                    return -1;
                                }
                            }
                            TimeOut.EndTime = DateTime.Now;
                        }
                        if (TimeOut.TimeOutFlag)
                        {
                            TimeOut.LogTimeOutError();
                            return -1;
                        }
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(string.Format("Modbus {0} ", ex.Message));
                return -1;
            }
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
