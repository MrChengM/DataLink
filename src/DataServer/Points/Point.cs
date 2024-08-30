using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace DataServer.Points
{
    public interface IWrite<T>
    {
        event Func<DevicePoint<T>, int,int> WriteEvent;

    }
    public interface IUpdate<T>
    {
        event Action<IPoint<T>,int> UpdataEvent;

    }
    //public interface ISinglPoint<T>
    //{
    //    string Name { get; }
    //    string Index { get; }

    //}
    public interface IPoint<T> : IUpdate<T>
    {
        T this[int index] { get;set; }
        string Name { get; }

        //string DataType { get; }
        int Length { get; }
        T GetValue( int index);

        T [] GetValues();

        void SetQuality(QUALITIES qualitity, int index);
        QUALITIES GetQuality();

        //void SetQuaility(QUALITIES qualitues);
        bool SetValue( T value, int index);

        bool SetValue(T[] value);

        bool IsVirtual();

    }
    public interface IDevicePoint<T>: IPoint<T>, IWrite<T>
    {
        string DeviceName { get; set; }


    }
    public class DevicePoint<T> : IDevicePoint<T>
    {

        private string _name;

        private DeviceAddress _address;

        private int _length;

        private Item<T>[] _value;

        private ReadWriteWay rw;


        private string _deviceName;

        public string DeviceName { get { return _deviceName; } set { _deviceName = value; } }

        public ReadWriteWay RW
        {
            get { return rw; }
            set { rw = value; }
        }


        public T this[int index]
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
        public Scaling Scaling { get; set; }
        public Item<T>[] Value
        {
            private get
            {
                return _value;
            }
            set
            {

                for (int i = 0; i < _length; i++)
                {
                    if (!_value[i].Equals(value[i]))
                    {
                        _value[i] = value[i];
                        UpdataEvent?.Invoke(this, i);
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
        public DevicePoint(string name,int length, DeviceAddress address,string deviceName)
        {
            _name = name;
            _length = length;
            _address = address;
            _deviceName = deviceName;
            _value = new Item<T>[Length];
            for(int i = 0; i < length; i++)
            {
                _value[i] = Item<T>.CreateDefault();
            }

        }


        public QUALITIES GetQuality()
        {
            return _value[0].Quality;
        }


        public bool IsVirtual()
        {
            return false;
        }
        public T GetValue(int index)
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
            for (int i = 0; i < _value.Length; i++)
            {
                value[i] = _value[i].Vaule;
            }
            return value;
        }

        public bool SetValue( T value, int index)
        {
            if (RW ==ReadWriteWay.Read)
            {
                return false;
            }
            if (index < _length)
            {
                if (!_value[index].Vaule.Equals(value))
                {
                    _value[index].Vaule = value;
                    RaisWriteEvent(this, index);
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
            if (RW == ReadWriteWay.Read)
            {
                return false;
            }
            if (value.Length <= _value.Length)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!_value[i].Vaule.Equals(value))
                    {
                        _value[i].Vaule = value[i];
                    }
                }
                if (RaisWriteEvent(this, -1)==-1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        int RaisWriteEvent(DevicePoint<T> point, int index)
        {
            if (WriteEvent != null)
            {
                return WriteEvent.Invoke(point, index);
            }
            else
            {
                return -1;
            }
        }
        public void SetQuality(QUALITIES qualitity, int index)
        {
            if (index<Length)
            {
                Value[index].Quality = qualitity;
            }
        }

        public event Action<IPoint<T>, int> UpdataEvent;
        public event Func<DevicePoint<T>, int,int> WriteEvent;
    }
    public class VirtulPoint<T>: IPoint<T>
    {
        private string _name;

        private T[] _value;
        private QUALITIES _qualitiy = QUALITIES.QUALITY_GOOD;
        private int _length;

        public event Action<IPoint<T>, int> UpdataEvent;

        public T this[int index]
        {
            get { return GetValue(index); }
            set { SetValue(value,index); }
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

        public VirtulPoint(string name, T[] Value)
        {
            _name = name;
            _value = Value;
            _length = Value.Length;
        }
        public VirtulPoint(string name, T Value)
        {
            _name = name;
            _value = new T[] { Value};
            _length =1;
        }
        public T[] GetValues()
        {
            return _value;
        }

        public QUALITIES GetQuality()
        {
            return _qualitiy;
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
                    }
                }
                UpdataEvent?.Invoke(this, -1);
                return true;
            }
            return false;
        }

        public bool IsVirtual()
        {
            return true;
        }

        public T GetValue(int index)
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
        public bool SetValue(T value, int index)
        {
            if (index < _length)
            {
                if (!_value[index].Equals(value))
                {
                    _value[index] = value;
                    UpdataEvent?.Invoke(this, index);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetQuality(QUALITIES qualitity, int index)
        {
            _qualitiy = qualitity;
        }
    }
}
