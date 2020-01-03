using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusDrivers
{
    public enum FunctionCode
    {
        /// <summary>
        /// 读线圈，地址：00001-09999，类型：bit
        /// </summary>
        ReadCoil = 0x01,
        /// <summary>
        /// 读输入状态，地址：10001-19999，类型：bit
        /// </summary>
        ReadInputStatus = 0x02,
        /// <summary>
        /// 读保持寄存器，地址：40001-49999，类型：Word
        /// </summary>
        ReadHoldRegister = 0x03,
        /// <summary>
        /// 读输入寄存器，地址：30001-39999，类型：Word
        /// </summary>
        ReadInputRegister = 0x04,
        /// <summary>
        /// 强制线圈，地址：00001-09999，类型：bit
        /// </summary>
        ForceSingleCoil = 0x05,
        /// <summary>
        /// 写单个寄存器，地址：40001-49999，类型：Word
        /// </summary>
        WriteSingleRegister = 0x06,
        /// <summary>
        /// 读不正常状态
        /// </summary>
        ReadAbNormalStatus = 0x07,
        /// <summary>
        /// 诊断
        /// </summary>
        Diagnose = 0x08,
        /// <summary>
        /// 程序
        /// </summary>
        Progame = 0x09,
        /// <summary>
        /// 查询
        /// </summary>
        Query = 0x0A,
        /// <summary>
        /// 通讯事件控制
        /// </summary>
        ControlComEvent = 0x0B,
        /// <summary>
        /// 通讯事件记录
        /// </summary>
        RecordComEvent = 0x0C,
        /// <summary>
        /// 强制多个线圈，地址：00001-9999，类型：bit
        /// </summary>
        ForceMulCoil = 0x0F,
        /// <summary>
        /// 写多个寄存器地址：40001-49999，类型：Word
        /// </summary>
        WriteMulRegister = 0x10,
        /// <summary>
        /// 报告从机ID
        /// </summary>
        ReportSlaveID = 0x11,

        Unkown=0x0,
    }
    public enum ErrorCode : byte
    {
        /// <summary>
        /// 不合法功能代码
        /// </summary>
        LllegalFuctionCode = 0x01,
        /// <summary>
        /// 不合法数据地址
        /// </summary>
        LllegalDataAddress = 0x02,
        /// <summary>
        /// 不合法数据
        /// </summary>
        LllegalData = 0x03,
        /// <summary>
        /// 从机设备故障
        /// </summary>
        SlaveDeviceFault = 0x04,
        /// <summary>
        /// 确认
        /// </summary>
        Cofirm = 0x05,
        /// <summary>
        /// 从机设备忙
        /// </summary>
        SlaveDeviceBuss = 0x06,
        /// <summary>
        /// 确认
        /// </summary>
        Deny = 0x07,
        /// <summary>
        /// 内存奇偶校验错误
        /// </summary>
        OddEvenError = 0x08,
    }
    public static class Function
    {
        public static string GetErrorString(byte errorCode)
        {

            switch (errorCode)
            {
                case (byte)ErrorCode.LllegalFuctionCode:
                    return "Illegal function code";
                case (byte)ErrorCode.LllegalDataAddress:
                    return "Invalid data address";
                case (byte)ErrorCode.LllegalData:
                    return "Invalid data";
                case (byte)ErrorCode.SlaveDeviceFault:
                    return "Salve Address fault";
                case (byte)ErrorCode.SlaveDeviceBuss:
                    return "Salve device busy";
                case (byte)ErrorCode.Cofirm:
                    return "Affirm";
                case (byte)ErrorCode.Deny:
                    return "Deny";
                case (byte)ErrorCode.OddEvenError:
                    return "Memory parity error";
            }
            return "Unknow";
        }
        /// <summary>
        /// 通过功能码及数量获取对应的字节数
        /// 0x01,0x02属于Bit类型,最大readCount为2000
        /// 0x03,0x04属于Word类型,最大readCount为125
        /// 其他功能代码返回0值
        /// </summary>
        /// <param name="funcCod">功能码</param>
        /// <param name="readCount">读取数量</param>
        /// <returns></returns>
        public static byte GetReadBytesCount(byte funcCod, ushort readCount)
        {
            byte byteCount = 0;
            switch (funcCod)
            {
                case (byte)FunctionCode.ReadCoil:
                    return byteCount = (byte)((readCount % 8 == 0) ? readCount / 8 : (readCount / 8 + 1));
                case (byte)FunctionCode.ReadInputStatus:
                    return byteCount = (byte)((readCount % 8 == 0) ? readCount / 8 : (readCount / 8 + 1));
                case (byte)FunctionCode.ReadHoldRegister:
                    return byteCount = (byte)(readCount * 2);
                case (byte)FunctionCode.ReadInputRegister:
                    return byteCount = (byte)(readCount * 2);
            }
            return byteCount;
        }
        public static FunctionCode GetReadFunctionCode(int address,  out ushort startAddress)
        {

            string addressStr = string.Format("{0:D5}", address);
            string headStr = addressStr.Substring(0, 1);
            startAddress = Convert.ToUInt16(addressStr.Substring(1, 4));
            switch (headStr)
            {
                case "0":
                    return FunctionCode.ReadCoil;
                case "1":
                    return FunctionCode.ReadInputStatus;
                case "4":
                    return FunctionCode.ReadHoldRegister;
                case "3":
                    return FunctionCode.ReadInputRegister;
                default:
                    return FunctionCode.Unkown;
            }

        }

        public static bool EnableWriteCoil(int address, out ushort startAddress)
        {
            string addressStr = string.Format("{0:D5}", address);
            string headStr = addressStr.Substring(0, 1);
            startAddress = Convert.ToUInt16(addressStr.Substring(1, 4));
            if (headStr == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool EnableWriteRegister(int address, out ushort startAddress)
        {
            string addressStr = string.Format("{0:D5}", address);
            string headStr = addressStr.Substring(0, 1);
            startAddress = Convert.ToUInt16(addressStr.Substring(1, 4));
            if (headStr == "4")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}



