using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.IO;
using System.Threading;
using DataServer;
using DataServer.Utillity;

namespace DL645Driver
{
    [Description("DL645-1997电能表协议")]
    //DL645_1997Driver : IPLCDriver         IPLCDriver : IDriver, IReaderWriter              IDriver : IDisposable
    public sealed class DL645_1997Driver:DLDriver
    {
        public DL645_1997Driver(SerialportSetUp setUp,  TimeOut timeOut, ILog log) :base(setUp, timeOut,log)
        {
        }
        /// <summary>
        /// 获取数据域数组（地址码）
        /// 1997数据域字节长度：02H
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override byte[] getDataArea(int address)
        {
            byte[] result = new byte[2];
            byte[] temp = BitConverter.GetBytes(address);
            Array.Copy(temp, result, result.Length);
            return result;
        }
        /// <summary>
        /// 获取具体值数据
        /// </summary>
        /// <param name="datas">源数据</param>
        /// <param name="ids">设备从站地址</param>
        /// <param name="address">地址码</param>
        /// <returns></returns>
        protected override byte[] getBytesValue(byte[] datas, byte[] ids, byte[] address)
        {
            byte[] CSdatas = new byte[datas.Length - 2];
            Array.Copy(datas, CSdatas, CSdatas.Length);
            if (datas[datas.Length - 1] == Utility.CSCheck(CSdatas))
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    if (datas[i + 1] != ids[i])
                    {
                        Log.ErrorLog(typeof(DL645_1997Driver) + ":Ids error");
                        return null;
                    }
                }
                for (int i = 0; i < address.Length; i++)
                {
                    if ((datas[i + 10] - 0x33) != address[i])
                    {
                        Log.ErrorLog(typeof(DL645_1997Driver) + ":address code error");

                        return null;
                    }
                }
                if (datas[8] == 0x81 || datas[8] == 0xA1)
                {
                    byte[] result = new byte[datas[9] - 4];
                    Array.Copy(datas, 14, result, 0, result.Length);
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = (byte)(result[i] - 0x33);
                    }
                    return result;
                }
                else if (datas[8] == 0xc1)
                {
                    Log.ErrorLog(string.Format("{0}:Slave Rely error,Code:{1} "), typeof(DL645_2007Driver), datas[10]);
                }
                return null;
            }
            else
            {
                Log.ErrorLog("CSC check error");
                return null;
            }
        }
        /// <summary>
        /// 获取电表浮点型数据
        /// 实现A1-A4四个list
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            byte[] readDatas = readBytes(deviceAddress, 0);
            if (readDatas != null)
            {
                var dataArea = getDataArea(deviceAddress.Address);
                string dataStr = string.Format("{0:4x}", deviceAddress.Address);
                if (dataStr.Substring(0,1) == "9")  //List A.1
                    return relayA1Code(dataArea, readDatas);
                else if (dataStr.Substring(0, 1) == "A") //List A.2
                    return relayA2Code(dataArea, readDatas);
                else if (dataStr.Substring(0, 1) == "B") //List A.3
                    switch (dataStr.Substring(1, 1))
                    {
                        case "1":
                        case "4":
                        case "5":
                        case "8":
                        case "9":
                            return relayA3Code(dataArea, readDatas);
                        case "2":
                        case "3":
                        case "6":
                            return relayA4Code(dataArea, readDatas);
                    }
            }
            return new Item<float>[] { Item<float>.CreateDefault() };
        }

        /// <summary>
        /// 表A.1 电能量数据表示编码：
        /// 1.DI1数据高位标识为；9
        /// 2.数据格式为：XXXXXX.XX
        /// 3.无FF数据标识，数据字节长度：4
        /// </summary>
        /// <param name="addrCode">地址码</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        private Item<float>[] relayA1Code(byte[] addrCode, byte[] data)
        {
            return getItems(4, 6, data);
        }
        /// <summary>
        ///  表A.2 电能量数据表示编码：
        /// 1.DI1数据高位标识为；A
        /// 2.数据格式为：XX.XXXX
        /// 3.无FF数据标识，数据字节长度：3
        /// </summary>
        /// <param name="addrCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Item<float>[] relayA2Code(byte[] addrCode, byte[] data)
        {
            return getItems(3,2, data);
        }
        /// <summary>
        /// 表A.3 电能量数据表示编码
        /// 1.DI1数据高位标识为；B，低位：1、4、5、8、9
        /// 2.数据格式为：MMDDHHMM,月日时分
        /// 3无FF数据标识，数据字节长度：4
        /// </summary>
        /// <param name="addrCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Item<float>[] relayA3Code(byte[] addrCode, byte[] data)
        {
            Item<float>[] value = new Item < float >[data.Length / 4];

            for(int i = 0; i < data.Length / 4; i++)
            {
                var temp = new byte[4];
                Array.Copy(data, 4 * i, temp, 0, 4);
                value[i] = Item<float>.CreateDefault();
                value[i].AppearTime = new DateTime(1900, temp[3], temp[2], temp[1], temp[0], 0);
            }
            return value;

        }

        /// <summary>
        /// 表A.4 电能量数据表示编码
        /// 1.DI1数据高位标识为；B，低位：2、3、6
        /// 2.DI1低位为2，DI0低位为0、1，数据格式为：MMDDHHMM,月日时分，长度为：4
        /// 3.DI1低位为2，DI0低位为2、3，数据格式为：NNNN,长度为：2
        /// 4.DI1低位为2，DI0低位为4，数据格式为：NNNNNN,长度为：3
        /// 5.DI1低位为3，DI0高位为1，数据格式为：NNNN,长度为：2
        /// 6.DI1低位为3，DI0高位为2，数据格式为：NNNNNN,长度为：3
        /// 7.DI1低位为3，DI0高位为3、4，数据格式为：MMDDHHMM,长度为：4
        /// 8.DI1低位为6，DI0高位为1，数据格式为：XXX,长度为：2
        /// 9.DI1低位为6，DI0高位为2，数据格式为：XX.XX,长度为：2
        /// 10.DI1低位为6，DI0高位为3、低位小于4,数据格式为：XX.XXXX,长度为：3
        /// 11.DI1低位为6，DI0高位为3、低位大于等于4,数据格式为：XX.XX,长度为：2
        /// 11.DI1低位为6，DI0高位为4、5,数据格式为：XX.XX,长度为：2
        /// </summary>
        /// <param name="addrCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Item<float>[] relayA4Code(byte[] addrCode, byte[] data)
        {
            Item<float>[] values = null;
            string DI1 = string.Format("{0:X2}", addrCode[1]);
            string DI0 = string.Format("{0:X2}", addrCode[0]);
            switch (DI1.Substring(1, 1))
            {
                case "2":
                    if (byte.Parse(DI0.Substring(1, 1)) < 2)
                    {
                        values = getItemsTime(data);
                    }
                    else
                    {
                        values = getItems(data);
                    }
                    break;
                case "3":
                    if (byte.Parse(DI0.Substring(0, 1)) < 3)
                    {
                        values = getItems(data);
                    }
                    else
                    {
                        values = getItemsTime(data);
                    }
                    break;
                case "6":
                    if (DI0.Substring(0, 1) == "3")
                    {
                        if (byte.Parse(DI0.Substring(1, 1)) < 4)
                            values = getItems(3, 2, data);
                        else
                            values = getItems(2, 2, data);
                    }
                    else if (DI0.Substring(0, 1) == "4")
                    {
                        values = getItems(2, 2, data);
                    }
                    else if (DI0.Substring(0, 1) == "5")
                    {
                        values = getItems(2, 1, data);
                    }
                    break;
            }
            return values;
        }
    }
}
