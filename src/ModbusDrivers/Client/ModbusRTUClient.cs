using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Utillity;
using System.IO.Ports;
using System.Threading;
 
namespace ModbusDrivers.Client
{
    /// <summary>
    /// ModbusRTU 协议 IPLCDriver:IRead IWrite IDriver IDisposable
    /// </summary>
    [DriverDescription("Modbus RTU",CommunicationType.Serialport)]
    public sealed class ModbusRTUClient : ModbusClient
    {
        // 内部成员定义

        private SerialportSetUp _portSetUp ;
        private SerialPort _serialPort = new SerialPort();

        public ModbusRTUClient() { }

        public ModbusRTUClient(SerialportSetUp portSetUp, TimeOut timeOut, ILog log)
        {
            _portSetUp = portSetUp;
            DriType = CommunicationType.Serialport;
            TimeOut = timeOut;
            Log = log;
        }


        public SerialportSetUp PortSetUp
        {
            get
            {
                return _portSetUp;
            }
            set
            {
                _portSetUp = value;
            }
        }
       
        public override bool IsClose
        {
            get
            {
                return _serialPort ==null|| _serialPort.IsOpen == false;
            }
        }

        /// <summary>
        /// COM口设置并打开
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                if (TimeOut.TimeOutSet < 1000)
                    TimeOut.TimeOutSet = 1000;
                _serialPort.PortName = _portSetUp.ComPort;
                _serialPort.BaudRate = _portSetUp.BuadRate;
                _serialPort.DataBits = _portSetUp.DataBit;
                _serialPort.StopBits = _portSetUp.StopBit;
                _serialPort.Parity = _portSetUp.OddEvenCheck;
                _serialPort.WriteTimeout = (int)TimeOut.TimeOutSet;
                _serialPort.ReadTimeout = (int)TimeOut.TimeOutSet;
                _serialPort.Open();
                IsConnect = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorLog("ModbusRTU Connect Error:" + ex.Message);
                IsConnect = false;
                return false;
            }
        }

        public override bool DisConnect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                IsConnect = false;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorLog("Modbus DisConnect Error:" + ex.Message);
                IsConnect = false;
                return false;
            }
        }
        /// <summary>
        /// 读数据报文头
        /// 由设备地址，功能码，其实地址，数量构成
        /// 返回带校验8位字节数组
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="func"></param>
        /// <param name="startAddress"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        protected override  byte[] getReadHeader(byte slaveId, byte func, ushort startAddress, ushort byteCount)
        {
            byte[] sendBytes = new byte[8];
            sendBytes[0] = slaveId;
            sendBytes[1] = func;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);//起始位为0，偏移1位
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            byte[] countBytes = BitConverter.GetBytes(byteCount);
            sendBytes[4] = countBytes[1];//高位在前
            sendBytes[5] = countBytes[0];//低位在后
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            sendBytes[6] = CRCBytes[0];
            sendBytes[7] = CRCBytes[1];
            return sendBytes;
        }
        protected override byte[] getWriteSigCoilHeader(byte slaveID, ushort startAddress, bool value)
        {
            byte[] sendBytes = new byte[8];
            sendBytes[0] = slaveID;
            sendBytes[1] = (byte)FunctionCode.ForceSingleCoil;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            if (value)
                sendBytes[4] = 0xFF;
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            sendBytes[6] = CRCBytes[0];
            sendBytes[7] = CRCBytes[1];
            return sendBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="value">数组长度为2</param>
        /// <returns></returns>
        protected override byte[] getWriteSigRegisterHeader(byte slaveID, ushort startAddress, byte[] value)
        {
            byte[] sendBytes = new byte[8];
            sendBytes[0] = slaveID;
            sendBytes[1] = (byte)FunctionCode.WriteSingleRegister;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            sendBytes[4] = value[1];
            sendBytes[5] = value[0];
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            sendBytes[6] = CRCBytes[0];
            sendBytes[7] = CRCBytes[1];
            return sendBytes;
        }

        protected override byte[] getWriteMulCoilHeader(byte slaveID, ushort startAddress, bool[] value)
        {
            byte[] datas = NetConvert.BoolstoBytes(value);
            byte[] sendBytes = new byte[9 + datas.Length];
            sendBytes[0] = slaveID;
            sendBytes[1] = (byte)FunctionCode.ForceMulCoil;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            byte[] CountBytes = BitConverter.GetBytes((ushort)value.Length);
            sendBytes[4] = CountBytes[1];
            sendBytes[5] = CountBytes[0];
            sendBytes[6] = (byte)datas.Length;
            Array.Copy(datas, 0, sendBytes, 7, datas.Length);
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            Array.Copy(CRCBytes, 0, sendBytes, sendBytes.Length-3, 2);
            return sendBytes;
        }
        protected override byte[] getWriteMulRegisterHeader(byte slaveID, ushort startAddress, byte[] value)
        {
            byte[] sendBytes = new byte[9 + value.Length];
            sendBytes[0] = slaveID;
            sendBytes[1] = (byte)FunctionCode.WriteMulRegister;
            byte[] addressBytes = BitConverter.GetBytes(startAddress - 1);
            sendBytes[2] = addressBytes[1];//高位在前
            sendBytes[3] = addressBytes[0];//低位在后
            byte[] CountBytes = BitConverter.GetBytes((ushort)value.Length/2);
            sendBytes[4] = CountBytes[1];
            sendBytes[5] = CountBytes[0];
            sendBytes[6] = (byte)value.Length;
            //value = UnsafeNetConvert.BytesPerversion(value); //前面已进行高低位翻转判断
            Array.Copy(value, 0, sendBytes, 7, value.Length);
            byte[] CRCBytes = Utility.CalculateCrc(sendBytes, sendBytes.Length - 2);
            Array.Copy(CRCBytes, 0, sendBytes, sendBytes.Length - 3, 2);

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
                        Log.ErrorLog("Modbus 读取功能码不正常");
                        return null;
                    }
                    byte[] sendBytes = getReadHeader(slaveID, funcCode, startAddress, count);
                    lock (_async)
                    {
                        
                        byte[] receiveBytes = new byte[3 + byteCount + 2];
                        byte[] dataBytes = new byte[byteCount];
                        byte errorFuncCode = (byte)(0x80 + funcCode);
                        List<byte> reciveBytesLog = new List<byte>();

                        _serialPort.Write(sendBytes, 0, sendBytes.Length);
                        Log.ByteSteamLog(ActionType.SEND, sendBytes);

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
                                _serialPort.Read(receiveBytes, 0, 1);
                                reciveBytesLog.Add(receiveBytes[0]);
                                while (receiveBytes[0] == slaveID)
                                {
                                    _serialPort.Read(receiveBytes, 1, 1);
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
                                    //只能一个字节一个字节读取
                                    index += _serialPort.Read(receiveBytes, index, 1);
                                    continueFlag = index == receiveBytes.Length ? false : true;

                                    for (int i = index; i < receiveBytes.Length; i++)
                                    {
                                        reciveBytesLog.Add(receiveBytes[i]);
                                    }
                                }
                                else if (receiveBytes[1] == errorFuncCode)
                                {
                                    index += _serialPort.Read(receiveBytes, index, 1);
                                    continueFlag = index == 5 ? false : true;
                                    for (int i = index; i < 5; i++)
                                    {
                                        reciveBytesLog.Add(receiveBytes[i]);
                                    }
                                }
                            }

                        }
                        Log.ByteSteamLog(ActionType.RECEIVE, reciveBytesLog.ToArray());
                        //获取正确报文并处理
                        if (receiveBytes[1] == funcCode)
                        {
                            if (!Utility.CheckSumCRC(receiveBytes, receiveBytes.Length))
                            {
                                Log.ErrorLog("Modbus CRC校验错误");
                                return null;
                            }
                            Array.ConstrainedCopy(receiveBytes, 3, dataBytes, 0, byteCount);
                            return dataBytes;
                        }
                        else if (receiveBytes[1] == errorFuncCode)
                        {
                            if (!Utility.CheckSumCRC(receiveBytes, 5))
                            {
                                Log.ErrorLog("Modbus CRC校验错误");
                                return null;
                            }
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
        /// <summary>
        /// 0x05强制单个线圈
        /// 0x06预置单个寄存器
        /// 0x0F强制多个线圈
        /// 0x10预置多个寄存器
        /// </summary>
        /// <param name="slaveID"></param>
        /// <param name="startAddress"></param>
        /// <param name="funcCode"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        protected override int writeBytes(byte[] sendBytes)
        {
            try
            {
                if (IsConnect)
                {
                    byte slaveID = sendBytes[0];
                    byte funcCode = sendBytes[1];
                    byte errorFuncCode = (byte)(0x80 + funcCode);
                    List<byte> reciveBytesLog = new List<byte>();

                    lock (_async)
                    {
                        _serialPort.Write(sendBytes, 0, sendBytes.Length);
                        Log.ByteSteamLog(ActionType.SEND, sendBytes);
                        Thread.Sleep(10);

                        int index = 0;
                        bool continueFlag = true;
                        byte[] receiveBytes = new byte[8];

                        /*----------------------------------------
                         *循环找头：
                         * 先读一个字节判断是否为SlaveID
                         * 如果是,则开启循环
                         * 则再读一个字节判断是否为功能码或者是错误码
                         * 判断是，则指针index+2并跳出循环
                         * 若第二个字节等于SlaveID则复制给头
                         * 否则将头置0
                         ------------------------------------------ */

                        while (!TimeOut.TimeOutFlag & continueFlag)
                        {
                            if (index < 2)
                            {
                                _serialPort.Read(receiveBytes, 0, 1);
                                reciveBytesLog.Add(receiveBytes[0]);

                                while (receiveBytes[0] == slaveID)
                                {
                                    _serialPort.Read(receiveBytes, 1, 1);

                                    //接受到报文记录
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
                                    index += _serialPort.Read(receiveBytes, index, 6);
                                    continueFlag = index == 8 ? false : true;

                                    //接受到报文记录
                                    for (int i = 2; i < 8; i++)
                                    {
                                        reciveBytesLog.Add(receiveBytes[i]);
                                    }
                                }
                                else if (receiveBytes[1] == errorFuncCode)
                                {
                                    index += _serialPort.Read(receiveBytes, index, 3);
                                    continueFlag = index == 5 ? false : true;

                                    //接受到报文记录
                                    for (int i = 2; i < 5; i++)
                                    {
                                        reciveBytesLog.Add(receiveBytes[i]);
                                    }
                                }
                            }
                        }
                        Log.ByteSteamLog(ActionType.RECEIVE, receiveBytes);
                        //获取正确报文并处理
                        if (receiveBytes[1] == funcCode)
                        {
                            if (Utility.CheckSumCRC(receiveBytes, receiveBytes.Length))
                            {
                                return 1;
                            }
                            else
                            {
                                Log.ErrorLog("Modbus CRC校验错误");
                                return -1;
                            }

                        }
                        else if (receiveBytes[1] == errorFuncCode)
                        {
                            if (Utility.CheckSumCRC(receiveBytes, 5))
                            {
                                Log.ErrorLog(string.Format("Modbus {0} ", Function.GetErrorString(receiveBytes[2])));
                            }
                            else
                            {
                                Log.ErrorLog("Modbus CRC校验错误");
                            }
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
            _portSetUp = null;
            _serialPort.Close();
            _serialPort.Dispose();
            TimeOut = null;
            Log = null;

        }

    }
}
