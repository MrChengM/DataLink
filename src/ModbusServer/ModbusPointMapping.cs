using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ds = DataServer;
using DataServer;
using DataServer.Log;
using DataServer.Points;
using Utillity.Data;
namespace ModbusServer
{
    public class ModbusPointMapping ///Point Length=1;
    {
        private ILog log;

        //private string _serverName;
        private IPointMapping _pointMapping;


        public IPointMapping PointMapping { get=> _pointMapping; set=> _pointMapping=value; }
        Dictionary<string, PointNameIndex> _modbusMapping;
        public ILog Log
        {
            get { return log; }
            set { log = value; }
        }
        //#region 单例，确保只存在一个Moubus点表
        //private static ModbusPointMapping instance;

        //private static readonly object locker = new object();

        public ModbusPointMapping(ILog log)
        {
            //_serverName = serverName;
            this.log = log;
            _modbusMapping = new Dictionary<string, PointNameIndex>(10000);
        }
        //public static ModbusPointMapping GetInstance(ILog log)
        //{
        //    if (instance == null)
        //    {
        //        lock (locker)
        //        {
        //            if (instance == null)
        //            {
        //                instance = new ModbusPointMapping(log);
        //            }
        //        }
        //    }
        //    return instance;
        //}
        //#endregion
        #region Method
        /// <summary>
        /// 注册bool类型modbus点
        /// </summary>
        /// <param name="key">00001～09999，10001～19999</param>
        /// <param name="point">长度为1的bool型</param>
        public void Register(string key, PointNameIndex nameIndex)
        {
            if (!Find(key))
            {
                if (key.Substring(0, 1) == "0" || key.Substring(0, 1) == "1" || key.Substring(0, 1) == "3" || key.Substring(0, 1) == "4")
                {
                    _modbusMapping.Add(key, nameIndex);
                }
                else
                {
                    log.ErrorLog("ModbusMapping Register Error: Key value not match modbus  standard,Key : {0} ", key);
                }
            }
            else
            {
                log.ErrorLog("ModbusMapping Register Error: Key have exist,Key : {0} ", key);

            }
        }

        public void Remove(string key)
        {
            if(Find(key))
            {
                _modbusMapping.Remove(key);
            }
        }
        public bool Find(string key)
        {
           //string type;
           return _modbusMapping.ContainsKey(key);
        }

        /// <summary>
        /// 获取bool点位布尔量数组
        /// </summary>
        /// <param name="key">00001-09999;10001-19999</param>
        /// <returns></returns>
        public bool[] GetBools(string key)
        {
            bool[] result;
            if (Find(key))
            {
                var pointIndex = _modbusMapping[key];
                var pointMeta = _pointMapping.GetPointMetaData(pointIndex.PointName);
                if (pointMeta.ValueType == DataType.Bool)
                {
                    IPoint<bool> boolPoint = _pointMapping.GetBoolPoint(key);
                    result = pointIndex.Index > -1
                        ? (new bool[] { boolPoint[pointIndex.Index]})
                        :boolPoint.GetValues();
                }
                else
                {
                    log.ErrorLog("ModbusMapping Get Point Error: Value Type error ,Key : {0} ", key);
                    result = null;
                }
            }
            else
            {
                log.ErrorLog("ModbusMapping Get Point Error: Point not exsit ,Key : {0} ", key);
                result = null;
            }
            return result;

        }
        /// <summary>
        /// 获取Short/Int/Float点位字节数组
        /// </summary>
        /// <param name="key">30001-39999;40001-49999</param>
        /// <returns></returns>
        public byte[] GetBytes(string key)
        {
            byte[] result;
            if (Find(key))
            {
                var pointIndex = _modbusMapping[key];
                var pointMeta = _pointMapping.GetPointMetaData(pointIndex.PointName);
                 if (pointMeta.ValueType == DataType.Short)
                {

                    IPoint<short> shortPoint = _pointMapping.GetShortPoint(pointMeta.Name);
                    result = pointIndex.Index > -1
                        ? UnsafeNetConvert.ShortToBytes( shortPoint[pointIndex.Index],ByteOrder.None)
                        : UnsafeNetConvert.ShortsToBytes(shortPoint.GetValues(), ByteOrder.None);
                }
                else if (pointMeta.ValueType == DataType.UShort)
                {
                    IPoint<ushort> ushortPoint = _pointMapping.GetUShortPoint(pointMeta.Name);
                    result = pointIndex.Index > -1
                        ? UnsafeNetConvert.UShortToBytes(ushortPoint[pointIndex.Index], ByteOrder.None)
                        : UnsafeNetConvert.UShortsToBytes(ushortPoint.GetValues(), ByteOrder.None);
                }
                else if (pointMeta.ValueType == DataType.Int)
                {

                    IPoint<int> intPoint = _pointMapping.GetIntPoint(pointMeta.Name);
                    result = pointIndex.Index > -1
                        ? UnsafeNetConvert.IntToBytes(intPoint[pointIndex.Index], ByteOrder.None)
                        : UnsafeNetConvert.IntsToBytes(intPoint.GetValues(), ByteOrder.None);
                    result = UnsafeNetConvert.BytesPerversion(result);
                }
                else if (pointMeta.ValueType == DataType.UInt)
                {
                    IPoint<uint> uintPoint = _pointMapping.GetUIntPoint(pointMeta.Name);
                    result = pointIndex.Index > -1
                        ? UnsafeNetConvert.UIntToBytes(uintPoint[pointIndex.Index], ByteOrder.None)
                        : UnsafeNetConvert.UIntsToBytes(uintPoint.GetValues(), ByteOrder.None);
                    result = UnsafeNetConvert.BytesPerversion(result);
                }
                else if (pointMeta.ValueType == DataType.Float)
                {
                    IPoint<float> floatPoint = _pointMapping.GetFloatPoint(pointMeta.Name);
                    result = pointIndex.Index > -1
                        ? UnsafeNetConvert.FloatToBytes(floatPoint[pointIndex.Index], ByteOrder.None)
                        : UnsafeNetConvert.FloatsToBytes(floatPoint.GetValues(), ByteOrder.None);
                    result = UnsafeNetConvert.BytesPerversion(result);
                }
                else
                {
                    log.ErrorLog("ModbusMapping Get Point Error: Value Type error ,Key : {0} ", key);
                    result = null;
                }
            }
            else
            {
                log.ErrorLog("ModbusMapping Get Point Error: Point not exsit ,Key : {0} ", key);
                result = null;
            }
            return result;
        }
        /// <summary>
        /// Force Single Coil Value Or Force Multi Coils Rely
        /// </summary>
        /// <param name="key">00001 - 09999</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string key, bool[] value)
        {
            bool result = false;
            if (Find(key))
            {
                var pointIndex = _modbusMapping[key];
                var pointMeta = _pointMapping.GetPointMetaData(pointIndex.PointName);
                if (pointMeta.ValueType == DataType.Bool)
                {
                    IPoint<bool> boolPoint = _pointMapping.GetBoolPoint(key);
                    result = pointIndex.Index > -1 ? boolPoint.SetValue(value[0], pointIndex.Index)
                        : boolPoint.SetValue(value);
                }
                else
                {
                    log.ErrorLog("ModbusMapping Set Point Error: Point Type error,Key : {0} , type ： {1}", key, pointMeta.ValueType);
                }

            }
            else
            {
                log.ErrorLog("ModbusMapping Set Point Error: Point not exsit ,Key : {0} ", key);
            }
            return result;
        }
        public bool SetValue(string key, byte[] value,int count)
        {
            bool result =false;
            if (Find(key))
            {
                var pointIndex = _modbusMapping[key];
                var pointMeta = _pointMapping.GetPointMetaData(pointIndex.PointName);
                if (pointMeta.ValueType == DataType.Short)
                {
                    IPoint<short> shortPoint = _pointMapping.GetShortPoint(key);
                    result = pointIndex.Index > -1 ? shortPoint.SetValue(UnsafeNetConvert.BytesToShort(value, 0, ByteOrder.BigEndian), pointIndex.Index)
                                           : shortPoint.SetValue(UnsafeNetConvert.BytesToShorts(value, 0, count, ByteOrder.BigEndian));
                }
                else if (pointMeta.ValueType == DataType.UShort)
                {
                    IPoint<ushort> ushortPoint = _pointMapping.GetUShortPoint(key);
                   result = pointIndex.Index > -1 ? ushortPoint.SetValue(UnsafeNetConvert.BytesToUShort(value, 0, ByteOrder.BigEndian), pointIndex.Index)
                                           : ushortPoint.SetValue(UnsafeNetConvert.BytesToUShorts(value, 0, count, ByteOrder.BigEndian));
                }
                else if (pointMeta.ValueType == DataType.Int)
                {
                    IPoint<int> intPoint = _pointMapping.GetIntPoint(key);
                    result = pointIndex.Index > -1 ? intPoint.SetValue(UnsafeNetConvert.BytesToInt(value, 0, ByteOrder.BigEndian), pointIndex.Index)
                                            : intPoint.SetValue(UnsafeNetConvert.BytesToInts(value, 0, count / 2, ByteOrder.BigEndian));
                }
                else if (pointMeta.ValueType == DataType.UInt)
                {
                    IPoint<uint> uintPoint = _pointMapping.GetUIntPoint(key);
                    result = pointIndex.Index > -1 ? uintPoint.SetValue(UnsafeNetConvert.BytesToUInt(value, 0, ByteOrder.BigEndian), pointIndex.Index)
                                          : uintPoint.SetValue(UnsafeNetConvert.BytesToUInts(value, 0, count / 2, ByteOrder.BigEndian));
                }
                else if (pointMeta.ValueType == DataType.Float)
                {
                    IPoint<float> floatPoint = _pointMapping.GetFloatPoint(key);
                    result = pointIndex.Index > -1 ? floatPoint.SetValue(UnsafeNetConvert.BytesToFloat(value, 0, ByteOrder.BigEndian), pointIndex.Index)
                                            : floatPoint.SetValue(UnsafeNetConvert.BytesToFloats(value, 0, count / 2, ByteOrder.BigEndian));
                }
                else
                {
                    log.ErrorLog("ModbusMapping Set Point Error: Point Type error,Key : {0} , type ： {1}", key, pointMeta.ValueType);
                }

            }
            else
            {
                log.ErrorLog("ModbusMapping Set Point Error: Point not exsit ,Key : {0} ", key);
            }
            return result;
        }
        #endregion
    }
    public static class ModbusType
    {
        public const string Coil = "coil";
        public const string InputStatus = "inputstatus";
        public const string InputRegister = "inputregister";
        public const string HoldRegister = "holdregister";
    }
   
}
