using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace DataServer.Points
{
    public interface ISend<T>
    {
        event Action<IPoint<T>, int, bool> SendEvent;

    }
    public interface IUpdate<T>
    {
        void ValueUpdate(T value, int index,OperateSource source);

    }
    public interface IPoint<T>:INotifyPropertyChanged
    {
        T this[byte index] { get;set; }
        string Name { get; }

        //string DataType { get; }
        int Length { get; }
        T GetValue( byte index);
        string ValueType { get; }

        T [] GetValues();

        byte GetQuality(int index);

        string GetQualityString(int index);
        bool SetQuality(QUALITIES quality, int index=0);
        bool SetValue( T value, byte index);

        bool SetValue(T[] value);

        bool IsVirtual();

        void ValueUpdate(T value, int index);

        event Action<IPoint<T>, int,bool> SendEvent;

    }


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
            set { SetValue(value, index); }
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

                for (int i = 0; i < _length; i++)
                {
                    if (_value[i] == null)
                    {
                        _value[i] = value[i];
                        OnValueChange(i);
                    }
                    else
                    {
                        if (!_value[i].Vaule.Equals(value[i].Vaule))
                        {
                            _value[i].Vaule = value[i].Vaule;
                            OnValueChange(i);
                        }
                        _value[i].Quality = value[i].Quality;
                        _value[i].UpdateTime = value[i].UpdateTime;
                    }

                }
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
            for(int i = 0; i < length; i++)
            {
                _value[i] = Item<T>.CreateDefault();
            }
        }


        public byte GetQuality(int index)
        {
            return (byte)_value[index].Quality;
        }

        public string GetQualityString(int index)
        {
            return _value[index].Quality.ToString();
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
                
                if (!_value[index].Vaule.Equals(value))
                {
                    _value[index].Vaule = value;
                    OnValueChange(index);
                    SendEvent?.Invoke(this, index, false);
                }
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
                    if (!_value[i].Vaule.Equals(value))
                    {
                        _value[i].Vaule = value[i];
                        OnValueChange(i);
                        SendEvent?.Invoke(this, i, false);
                    }
                }
                return true;
            }
            return false;
        }

        public bool SetQuality( QUALITIES quality, int index=0)
        {
            _value[index].Quality = quality;
            return true;
        }
        #region INotifyPropertyChanged接口

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<IPoint<T>, int,bool> SendEvent;

        /// <summary>
        /// 设置数据时触发绑定（用户设置或读取设备值）
        /// </summary>
        /// <param name="index"></param>                                         
        protected void OnValueChange(int index)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(index.ToString()));
        }
        /// <summary>
        /// 绑定的数据源数据变化触发
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public void ValueUpdate(T value, int index)
        {
            if (index < _length)
            {

                if (!_value[index].Vaule.Equals(value))
                {
                    _value[index].Vaule = value;
                    SendEvent?.Invoke(this, index, false);
                }
            }
        }

        #endregion

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
                for (int i = 0; i < value.Length; i++)
                {
                    if (!value[i].Equals(_value[i]))
                    {
                        _value[i] = value[i];
                        OnValueChange(i);
                    }
                }
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
        public VirtulPoint(string name,string type, T[] Value)
        {
            _name = name;
            _value = Value;
            _length = Value.Length;
            _valueType = type;
        }
        public VirtulPoint(string name, string type,int length=1)
        {
            _name = name;
            _length = length;
            _valueType = type;
            _value = new T[length];
            for(int i = 0; i < length; i++)
            {
                _value[i] = default(T);
            }
        }
        public T[] GetValues()
        {
            return _value;
        }

        public byte GetQuality(int index)
        {
            return (byte)_qualitiy;
        }

        public string GetQualityString(int index)
        {
            return _qualitiy.ToString();
        }

        public bool SetValue(T[] value)
        {
            if (value.Length <= _value.Length)
            {
                for(int i = 0; i < value.Length;i++)
                {
                    if (!value[i].Equals(_value[i]))
                    {
                        _value[i] = value[i];
                        OnValueChange(i);
                    }
                }
               
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
                if (!_value[index].Equals(value))
                {
                    _value[index] = value;
                    OnValueChange(index);
                }
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public bool SetQuality(QUALITIES quality, int index=0)
        {
            _qualitiy = quality;
            return true;
        }
        #region INotifyPropertyChanged 接口实现
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<IPoint<T>, int, bool> SendEvent;
        /// <summary>
        /// 设置数据时触发绑定（用户设置或读取设备值）
        /// </summary>
        /// <param name="index"></param>
        protected void OnValueChange(int index)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(index.ToString()));
        }

        /// <summary>
        /// 绑定的数据源数据变化触发
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public void ValueUpdate(T value, int index)
        {
            if (index < _length)
            {
                if (!_value[index].Equals(value))
                {
                    _value[index] = value;
                    SendEvent?.Invoke(this, index, false);
                }
            }
        }
        #endregion
    }

    public enum OperateSource
    {
        FromDevice,
        FromPointBinding,
        FormUserSet
    }
}
