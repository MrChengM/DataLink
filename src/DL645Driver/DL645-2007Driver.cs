using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.IO;
using System.Threading;
using DataServer.Utillity;
using System.Globalization;

namespace DL645Driver
{
    [DriverDescription("DL645-2007电能表协议",CommunicationType.Serialport)]
    public sealed class DL645_2007Driver : DLDriver
    {
        #region IDriver
        #endregion
        public DL645_2007Driver(SerialportSetUp setUp, TimeOut timeOut, ILog log) :base(setUp,timeOut, log)
        {
        }
        /// <summary>
        /// 获取数据域数组（地址码）
        /// 2007数据域字节长度：04H
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override byte[] getDataArea(int address)
        {
            byte[] result = BitConverter.GetBytes(address);
            return result;
        }
        /// <summary>
        /// 获取具体值数据
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="ids"></param>
        /// <param name="address"></param>
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
                        Log.ErrorLog(typeof(DL645_2007Driver)+":Ids error");
                        return null;
                    }
                }
                for (int i = 0; i < address.Length; i++)
                {
                    if ((datas[i + 10] - 0x33) != address[i])
                    {
                        Log.ErrorLog(typeof(DL645_2007Driver)+":address code error");

                        return null;
                    }
                }
                if (datas[8] == 0x91 || datas[8]== 0xB1)
                {
                    byte[] result = new byte[datas[9] - 4];
                    Array.Copy(datas, 14, result, 0, result.Length);
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = (byte)(result[i] - 0x33);
                    }
                    return result;
                }
                else if (datas[8] == 0xD1)
                {
                    Log.ErrorLog(string.Format("{0}:Slave Rely error,Code:{1} "),typeof(DL645_2007Driver),datas[10]);
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
        /// 读取浮点型数据，2007协议已实现A1-A3三个表内数据的读取
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override Item<float>[] Readfloats(DeviceAddress deviceAddress, ushort length)
        {
            byte[] readDatas = readBytes(deviceAddress, 0);
            if (readDatas != null)
            {
                var adderss = getDataArea(deviceAddress.Address);
                if (adderss[3] == 0)  //List A.1
                    return relayA1Code(adderss, readDatas);
                else if (adderss[3] == 1) //List A.2
                    return relayA2Code(adderss, readDatas);
                else if (adderss[3] == 2) //List A.3
                    return relayA3Code(adderss, readDatas);
            }
            return new Item<float>[] { Item<float>.CreateDefault() };
        }

        /// <summary>
        /// 表A.1 电能量数据表示编码：
        /// 1.DI3数据标识为；00
        /// 2.数据格式为：XXXXXX.XX
        /// 3.无FF数据标识，数据字节长度：4
        /// 4.DI1为FF时，读数据块，数据字节长度为：4*63
        /// 5.DI0为FF时，读数据块，数据字节长度为：4*13
        /// </summary>
        /// <param name="addrCode">地址码</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        private Item<float>[] relayA1Code(byte[] addrCode,byte[] data)
        {
            return getItems(4, 6, data);
        }
        /// <summary>
        ///  表A.2 电能量数据表示编码：
        /// 1.DI3数据标识为；01
        /// 2.最大需量数据格式为：XX.XXXX(3字节)
        /// 3.最大需量出现时间数据格式为：YYMMDDHHMM(5字节)
        /// 4.无FF数据标识，数据字节长度：8
        /// 5.DI1为FF时，读数据块，数据字节长度：8*63
        /// 6.DI0为FF时，读数据块，数据字节长度：8*13
        /// </summary>
        /// <param name="addrCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Item<float>[] relayA2Code(byte[] addrCode,byte[] data)
        {
            int length = 8;
            int dataLength = 3;
            int timeLength = 5;
            int postion = 2;
            Item<float>[] value = null;

            if (data.Length % length == 0)
            {
                value = new Item<float>[data.Length / length];
                for (int i = 0; i < value.Length; i++)
                {
                    byte[] dataBytes = new byte[dataLength];
                    Array.Copy(data, length * i, dataBytes, 0, dataLength);
                    byte[] datetime = new byte[timeLength];
                    Array.Copy(data, length * i + dataLength, datetime, 0, timeLength);
                    DateTime appearTime = new DateTime(datetime[4], datetime[3], datetime[2], datetime[1], datetime[0], 0);
                    value[i] = new Item<float>
                    {
                        Vaule = getFloat(data, postion),
                        UpdateTime = DateTime.Now,
                        AppearTime = appearTime,
                        Quality = QUALITIES.QUALITY_GOOD
                    };
                }
            }

            return value;
        }
        /// <summary>
        /// 表A.3 电能量数据表示编码
        /// 1.DI3数据标识为:02
        /// 2.当DI2=01时，数据格式为：XXX.X,数据字节长度为：2,
        ///    DI1=0xFF读数据块时，数据字节长度为：2*3
        /// 3.当DI2=02时，数据格式为：XXX.XXX,数据字节长度：3
        ///    DI1=0xFF读数据块时，数据字节长度为：3*3
        /// 4.当DI2=03、04、05时，数据格式为：XX.XXXX,数据字节长度为：3
        ///    DI1=0xFF读数据块时，数据字节长度为：3*3
        /// 5.当DI2=06时，数据格式为：X.XX,数据字节长度为：2
        ///    DI1=0xFF读数据块时，数据字节长度为：2*3
        /// 6.当DI2=07时，数据格式为：XXX.X，数据字节长度：2
        ///   DI1=0xFF读数据块时，数据字节长度为：2*3
        /// 7.当DI2=08、09时，数据格式为：XX.XX，数据字节长度为：2
        ///   DI1=0xFF读数据块时，数据字节长度为：2*3
        /// 8.当DI2=0A、0B时，数据格式为：XX.XX,数据字节长度：2
        ///   DI0=0xFF读数据块时，数据字节长度为：2*21
        /// 9.当DI2=80，DI0=01时，数据格式为XXX.XXX,数据字节长度为：3
        /// 10.当DI2=80,DI0=02-06时，数据格式为：XX.XXXX，数据字节长度为：3
        /// 11.当DI2=80,DI0=07时，数据格式为：XXX.X，数据字节长度为：2
        /// 12.当DI2=80,DI0=08、09时，数据格式为：XX.XX，数据字节长度为：2
        /// </summary>
        /// <param name="addrCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Item<float>[] relayA3Code(byte[] addrCode, byte[] data)
        {
            Item<float>[] value = null;
            switch (addrCode[2])
            {
                case 0x01:
                    value = getItems(2, 3, data);
                    break;
                case 0x02:
                    value = getItems(3, 3, data);
                    break;
                case 0x03:
                case 0x04:
                case 0x05:
                    value = getItems(3, 2, data);
                    break;
                case 0x06:
                    value = getItems(2,1, data);
                    break;
                case 0x07:
                case 0x08:
                case 0x09:
                case 0x0A:
                case 0x0B:
                    value = getItems(2, 2, data);
                    break;
                case 0x80:
                    switch(addrCode[0])
                    {
                        case 0x01:
                            value = getItems(3, 3, data);
                            break;
                        case 0x02:
                        case 0x03:
                        case 0x04:
                        case 0x05:
                        case 0x06:
                            value = getItems(3, 2, data);
                            break;
                        case 0x07:
                            value = getItems(2, 3, data);
                            break;
                        case 0x08:
                        case 0x09:
                            value = getItems(2, 2, data);
                            break;
                    }
                    break;
            }
            return value;

        }
      
    }
}
