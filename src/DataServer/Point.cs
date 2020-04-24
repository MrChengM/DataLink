using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public interface IPoint<T>
    {
        T this[byte index] { get;set; }
        string Name { get; }

        //string DataType { get; }
        int Length { get; }
        T GetValue( byte index);
        string ValueType { get; }

        T [] GetValues();

        byte GetQuality();

        string GetQualityString();
        bool SetQuality(QUALITIES quality);
        bool SetValue( T value, byte index);

        bool SetValue(T[] value);

        bool IsVirtual();
    }
    //public abstract class Point<T>
    //{
    //    public abstract string Name { get;}

    //    //public abstract DeviceAddress Address { get; set; }

    //    public abstract Item<T>[] Value { get; set; }

    //    public abstract PointType Type { get;}

    //}

    public enum PointType
    {
        DevicePoint=0x01,
        VirtualPoint=0x02,

    }
    public class DevicePoint<T> : IPoint<T> 
    {

        private string _name;

        private DeviceAddress _address;

        private string _valueType;

        private int _length;

        private Item<T>[] _value;

        private PointType _type = PointType.DevicePoint;

        public T this[byte index]
        {
            get { return GetValue(index); }
            set { SetValue( value,index); }
        }
        public  DeviceAddress Address
        {
            get
            {
                return _address;
            }

            set
            {
                _address = value;
            }
        }

        public  Item<T>[] Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public string ValueType
        {
            get
            {
               return _valueType;
            }
        }

        public DevicePoint(string name,string valueType,int length, DeviceAddress address)
        {
            _name = name;
            _valueType = valueType;
            _length = length;
            _address = address;
            _value = new Item<T>[Length];
        }


        public byte GetQuality()
        {
            return (byte)_value[0].Quality;
        }

        public string GetQualityString()
        {
            return _value[0].Quality.ToString();
        }

        public bool IsVirtual()
        {
            if (_type == PointType.VirtualPoint)
            {
                return true;
            }
            return false;
        }
        public T GetValue(byte index)
        {

            if (index < _length)
            {
                return _value[index].Vaule;
            }
            else
            {
                return default(T);
            }
        }
        public T[] GetValues()
        {
            T[] value = new T[_value.Length];
           for(int i = 0; i < _value.Length; i++)
            {
                value[i] = _value[i].Vaule;
            }
            return value;
        }

        public bool SetValue( T value, byte index)
        {
            if (index < _length)
            {
                _value[index].Vaule = value;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetValue(T[] value)
        {
            if(value.Length<= _value.Length)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    _value[i].Vaule = value[i];
                }
                return true;
            }
            return false;
        }

        public bool SetQuality(QUALITIES quality)
        {
            foreach(Item<T> item in _value)
            {
                item.Quality = quality;
            }
            return true;
        }
    }
    public class VirtulPoint<T>: IPoint<T>
    {
        private string _name;

        private string _valueType;
        private T[] _value;
        private QUALITIES _qualitiy = QUALITIES.QUALITY_GOOD;
        private int _length;
        private PointType _type = PointType.VirtualPoint;

        public T this[byte index]
        {
            get { return GetValue(index); }
            set { SetValue(value,index); }
        }
        public T[] Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value =value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }
        public string ValueType
        {
            get { return _valueType; }
        }
        public VirtulPoint(string name, T[] Value)
        {
            _name = name;
            _value = Value;
            _length = Value.Length;
        }
        public VirtulPoint(string name, int length=1)
        {
            _name = name;
            _length = length;
            _value = new T[length];
        }
        public T[] GetValues()
        {
            return _value;
        }

        public byte GetQuality()
        {
            return (byte)_qualitiy;
        }

        public string GetQualityString()
        {
            return _qualitiy.ToString();
        }

        public bool SetValue(T[] value)
        {
            if (value.Length <= _value.Length)
            {
                Array.Copy(value, _value, value.Length);
                return true;
            }
            return false;
        }

        public bool IsVirtual()
        {
            if (_type == PointType.VirtualPoint)
                return true;
            return false;
        }

        public T GetValue(byte index)
        {
            if (index < _length)
            {
                return _value[index];
            }
            else
            {
                return default(T);
            }
        }

        public bool SetValue(T value, byte index)
        {
            if (index < _length)
            {
                _value[index] =value;
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool SetQuality(QUALITIES quality)
        {
            _qualitiy = quality;
            return true;
        }
    }
}
