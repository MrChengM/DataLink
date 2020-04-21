using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using ds = DataServer;

namespace ModbusDrivers.Server
{
    public class BoolPoint : IPoint<bool>
    {
        /// <summary>
        /// 源数据点
        /// </summary>
        private IPoint<bool> _source;
        /// <summary>
        /// Modbus键值00001～09999.
        /// 10001～19999
        /// </summary>
        private string _key;
        /// <summary>
        /// 源数据点数组索引
        /// </summary>
        private byte _index;

        private int _length = 1;

        public BoolPoint(IPoint<bool> source,string key,byte index)
        {
            _source = source;
            _key = key;
            index = _index;
        }
        public bool this[byte index]
        {
            get
            {
                return GetValue();
            }

            set
            {
                SetValue(value);
            }
        }

        public int Length
        {
            get
            {
               return _length;
            }
        }

        public string Name
        {
            get
            {
                return _key;
            }
        }

        public string ValueType
        {
            get
            {
              return  DataServer.ValueType.Bool;
            }
        }

        public byte GetQuality()
        {
            return _source.GetQuality();
        }

        public string GetQualityString()
        {
            return _source.GetQualityString();
        }

        public bool GetValue(byte index=0)
        {
            return _source[_index];
        }
        public bool[] GetValues()
        {
            bool[] result = new bool[1];
            result[0]=_source[_index];
            return result;
        }

        public bool IsVirtual()
        {
            return _source.IsVirtual();
        }

        public bool SetValue( bool value, byte index = 0)
        {
            return _source.SetValue(value,_index);
        }
        public bool SetValue(bool[] value)
        {
            return SetValue(value[0]);
        }
        public bool SetQuality(QUALITIES quality)
        {
            return _source.SetQuality(quality);
        }
    }
    public class IntPoint : IPoint<short>
    {
        /// <summary>
        /// Modbus键值30001～39999，40001～49999
        /// </summary>
        private string key;
        /// <summary>
        /// 源数据数组索引
        /// </summary>
        private byte _index;
        /// <summary>
        /// 位数高于16位数据偏移量
        /// </summary>
        private byte _offset;

        private int _length=1;

        private byte _quality;

        private string _qualityString;
        private bool _isVirtual;

        #region 源数据
        private IPoint<byte> _bytePoint;
        private IPoint<short> _shortPoint;
        private IPoint<ushort> _ushortPoint;
        private IPoint<int> _intPoint;
        private IPoint<uint> _uintPoint;
        private IPoint<float> _floatPoint;
        private string _soureValueType;
        #endregion

        public IntPoint(IPoint<byte> point,string key,byte index)
        {
            _bytePoint = point;
            this.key = key;
            _index = index;
            _quality = point.GetQuality();
            _qualityString = point.GetQualityString();
            _soureValueType = ds.ValueType.Byte;
            _isVirtual = point.IsVirtual();
            _offset = 0;
        }
        public IntPoint(IPoint<short> point, string key, byte index)
        {
            _shortPoint = point;
            this.key = key;
            _index = index;
            _quality = point.GetQuality();
            _qualityString = point.GetQualityString();
            _soureValueType = ds.ValueType.Int16;
            _isVirtual = point.IsVirtual();
            _offset = 0;

        }
        public IntPoint(IPoint<ushort> point, string key, byte index)
        {
            _ushortPoint = point;
            this.key = key;
            _index = index;
            _quality = point.GetQuality();
            _qualityString = point.GetQualityString();
            _soureValueType = ds.ValueType.UInt16;
            _isVirtual = point.IsVirtual();
            _offset = 0;

        }
        public IntPoint(IPoint<int> point, string key, byte index,byte offset)
        {
            _intPoint = point;
            this.key = key;
            _index = index;
            _quality = point.GetQuality();
            _qualityString = point.GetQualityString();
            _soureValueType = ds.ValueType.Int32;
            _isVirtual = point.IsVirtual();
            _offset = offset;
        }
        public IntPoint(IPoint<uint> point, string key, byte index,byte offset)
        {
            _uintPoint = point;
            this.key = key;
            _index = index;
            _quality = point.GetQuality();
            _qualityString = point.GetQualityString();
            _soureValueType = ds.ValueType.UInt32;
            _isVirtual = point.IsVirtual();
            _offset = offset;
        }
        public IntPoint(IPoint<float> point, string key, byte index,byte offset)
        {
            _floatPoint = point;
            this.key = key;
            _index = index;
            _quality = point.GetQuality();
            _qualityString = point.GetQualityString();
            _soureValueType = ds.ValueType.Float;
            _isVirtual = point.IsVirtual();
            _offset = offset;
        }
        public short this[byte index]
        {
            get
            {
              return  GetValues()[index];
            }

            set
            {
                SetValue(value,index);
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public string Name
        {
            get
            {
                return key;
            }
        }

        public string ValueType
        {
            get
            {
               return ds.ValueType.Int16;
            }
        }

        public byte GetQuality()
        {
            return _quality;
        }

        public string GetQualityString()
        {
            return _qualityString;
        }

        public short GetValue(byte index=0)
        {
            if (_soureValueType == ds.ValueType.Byte)
            {
                byte[] bytes = new byte[2] { _bytePoint[_index], _bytePoint[(byte)(_index + 1)] };
                return UnsafeNetConvert.BytesToShort(bytes, 0, ByteOrder.None);
            }
            else if (_soureValueType == ds.ValueType.Int16)
            {
                return _shortPoint[_index];
            }
            else if (_soureValueType == ds.ValueType.UInt16)
            {
                return UnsafeNetConvert.BytesToShort(UnsafeNetConvert.UShortToBytes(_ushortPoint[_index], ByteOrder.None), 0, ByteOrder.None);
            }
            else if (_soureValueType == ds.ValueType.Int32)
            {
                return UnsafeNetConvert.BytesToShorts(UnsafeNetConvert.IntToBytes(_intPoint[_index], ByteOrder.None), 0,2, ByteOrder.None)[_offset];
            }
            else if (_soureValueType == ds.ValueType.UInt32)
            {
                return UnsafeNetConvert.BytesToShorts(UnsafeNetConvert.UIntToBytes(_uintPoint[_index], ByteOrder.None), 0,2, ByteOrder.None)[_offset];
            }
            else if (_soureValueType == ds.ValueType.Float)
            {
                return UnsafeNetConvert.BytesToShorts(UnsafeNetConvert.FloatToBytes(_floatPoint[_index], ByteOrder.None), 0, 2, ByteOrder.None)[_offset];
            }
            else
            {
                return default(short);
            }
        }

        public short[] GetValues()
        {
           return new short[] { GetValue() };
        }

        public bool IsVirtual()
        {
            return _isVirtual;
        }

        public bool SetValue(short[] value)
        {
           return SetValue(value[0]);
        }

        public bool SetValue(short value, byte index=0)
        {
            if (_soureValueType == ds.ValueType.Byte)
            {
                byte[] bytes = UnsafeNetConvert.ShortToBytes(value, ByteOrder.None);
                _bytePoint.SetValue(bytes[0], _index);
                _bytePoint.SetValue(bytes[0], (byte)(_index + 1));
                return true;
            }
            else if (_soureValueType == ds.ValueType.Int16)
            {
                _shortPoint[_index] = value;
                return true;
            }
            else if (_soureValueType == ds.ValueType.UInt16)
            {
                _ushortPoint[_index] = UnsafeNetConvert.BytesToUShort(UnsafeNetConvert.ShortToBytes(value, ByteOrder.None), 0, ByteOrder.None);
                return true;
            }
            else if (_soureValueType == ds.ValueType.Int32)
            {
                
                short[] shorts= UnsafeNetConvert.BytesToShorts(UnsafeNetConvert.IntToBytes(_intPoint[_index], ByteOrder.None), 0, 2, ByteOrder.None);
                shorts[_offset] = value;
                _intPoint[_index] = UnsafeNetConvert.BytesToInt(UnsafeNetConvert.ShortsToBytes(shorts, ByteOrder.None), 0, ByteOrder.None);
                return true;
            }
            else if (_soureValueType == ds.ValueType.UInt32)
            {
                short[] shorts = UnsafeNetConvert.BytesToShorts(UnsafeNetConvert.UIntToBytes(_uintPoint[_index], ByteOrder.None), 0, 2, ByteOrder.None);
                shorts[_offset] = value;
                _uintPoint[_index] = UnsafeNetConvert.BytesToUInt(UnsafeNetConvert.ShortsToBytes(shorts, ByteOrder.None), 0, ByteOrder.None);
                return true;
            }
            else if (_soureValueType == ds.ValueType.Float)
            {
                short[] shorts = UnsafeNetConvert.BytesToShorts(UnsafeNetConvert.FloatToBytes(_uintPoint[_index], ByteOrder.None), 0, 2, ByteOrder.None);
                shorts[_offset] = value;
                _floatPoint[_index] = UnsafeNetConvert.BytesToFloat(UnsafeNetConvert.ShortsToBytes(shorts, ByteOrder.None), 0, ByteOrder.None);
                return true;
            }
            return false;
        }
        public bool SetQuality(QUALITIES quality)
        {
            if (_soureValueType == ds.ValueType.Byte)
            {
                return _bytePoint.SetQuality(quality);
            }
            else if (_soureValueType == ds.ValueType.Int16)
            {
                return _shortPoint.SetQuality(quality);

            }
            else if (_soureValueType == ds.ValueType.UInt16)
            {
                return _ushortPoint.SetQuality(quality);

            }
            else if (_soureValueType == ds.ValueType.Int32)
            {

                return _intPoint.SetQuality(quality);

            }
            else if (_soureValueType == ds.ValueType.UInt32)
            {
                return _uintPoint.SetQuality(quality);
            }
            else if (_soureValueType == ds.ValueType.Float)
            {
                return _floatPoint.SetQuality(quality);
            }
            return false;
        }
    }
}
