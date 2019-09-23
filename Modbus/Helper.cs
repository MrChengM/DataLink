using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus
{
    public enum FucthionCode
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
        WriteSingleCoil = 0x06,
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
        Progame=0x09,
        /// <summary>
        /// 查询
        /// </summary>
        Query=0x0A,
        /// <summary>
        /// 通讯事件控制
        /// </summary>
        ControlComEvent=0x0B,
        /// <summary>
        /// 通讯事件记录
        /// </summary>
        RecordComEvent=0x0C,
        /// <summary>
        /// 强制多个寄存器，地址：30001-39999，类型：Word
        /// </summary>
        ForceMulRegister = 0x0F,
        /// <summary>
        /// 写多个寄存器地址：40001-49999，类型：Word
        /// </summary>
        WriteMulRegister = 0x10,
        /// <summary>
        /// 报告从机ID
        /// </summary>
        ReportSlaveID=0x11
    }
    public enum ErrorCode:byte
    {
        /// <summary>
        /// 不合法功能代码
        /// </summary>
        LllegalFuctionCode=0x01,
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
        SlaveDeviceFault=0x04,
        /// <summary>
        /// 确认
        /// </summary>
        Cofirm=0x05,
        /// <summary>
        /// 从机设备忙
        /// </summary>
        SlaveDeviceBuss = 0x06,
        /// <summary>
        /// 确认
        /// </summary>
        Deny=0x07,
        /// <summary>
        /// 内存奇偶校验错误
        /// </summary>
        OddEvenError=0x08,
    }
    public static class Function
    {
        public static string GetErrorString(byte Code)
        {

            switch (Code)
            {
                case (byte)ErrorCode.LllegalFuctionCode:
                    return "不合法功能代码";
                case (byte)ErrorCode.LllegalDataAddress:
                    return "不合法数据地址";
                case (byte)ErrorCode.LllegalData:
                    return "不合法数据";
                case (byte)ErrorCode.SlaveDeviceFault:
                    return "从机地址故障";
                case (byte)ErrorCode.SlaveDeviceBuss:
                    return "从机设备忙";
                case (byte)ErrorCode.Cofirm:
                    return "确认";
                case (byte)ErrorCode.Deny:
                    return "否认";
                case (byte)ErrorCode.OddEvenError:
                    return "内存奇偶校验错误";
            }
            return default(string);
        }
    }
}
