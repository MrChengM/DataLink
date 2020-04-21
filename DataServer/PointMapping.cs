using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public class PointMapping
    {
        private ILog log;

        public static PointMapping Instance;

        private static readonly object locker = new object();
        private PointMapping()
        {
         IPointMapping<bool> boolMapping =new BasePointMapping<bool>(log);
         IPointMapping<byte> byteMapping = new BasePointMapping<byte>(log);
         IPointMapping<ushort> ushortMapping = new BasePointMapping<ushort>(log);
         IPointMapping<short> shortMapping = new BasePointMapping<short>(log);
         IPointMapping<uint> uintMapping = new BasePointMapping<uint>(log);
         IPointMapping<int> intMapping = new BasePointMapping<int>(log);
         IPointMapping<float> floatMapping = new BasePointMapping<float>(log);
         IPointMapping<double> doubleMapping = new BasePointMapping<double>(log);
         IPointMapping<string> stringMapping = new BasePointMapping<string>(log);
    }
        public static PointMapping GetInstance()
        {
            if (Instance == null)
            {
                lock (locker)
                {
                    if (Instance == null)
                    {
                        Instance = new PointMapping();
                    }
                }
            }
            return Instance;
        }
        public ILog Log
        {
            get { return log; }
            set { log = value; }
        }
        IPointMapping<bool> boolMapping = new BasePointMapping<bool>();
        IPointMapping<byte> byteMapping = new BasePointMapping<byte>();
        IPointMapping<ushort> ushortMapping = new BasePointMapping<ushort>();
        IPointMapping<short> shortMapping = new BasePointMapping<short>();
        IPointMapping<uint> uintMapping = new BasePointMapping<uint>();
        IPointMapping<int> intMapping = new BasePointMapping<int>();
        IPointMapping<float> floatMapping = new BasePointMapping<float>();
        IPointMapping<double> doubleMapping = new BasePointMapping<double>();
        IPointMapping<string> stringMapping = new BasePointMapping<string>();
    }
    public class BasePointMapping<T> : IPointMapping<T>
    {
        Dictionary<string,IPoint<T>> mapping = new Dictionary<string, IPoint<T>>();
        private ILog log;

        public BasePointMapping()
        {

        }
        public BasePointMapping(ILog log)
        {
            this.log = log;
        }
        public ILog Log
        {
            get { return log; }
            set { log = value; }
        }
        public bool Find(string key)
        {
           return mapping.ContainsKey(key);
        }

        public bool Find(string key, out string type)
        {
            if (Find(key))
            {
                type = mapping[key].ValueType;
                return true;
            }
            else
            {
                type = null;
                return false;
            };
        }

        public IPoint<T> GetPoint(string key)
        {
            if (Find(key))
            {
               return mapping[key];
            }
            else
            {
                return null;
            }
        }

        public T[] GetValue(string key)
        {
            if (Find(key))
            {
                return GetPoint(key).GetValues();
            }
            else
            {
                return null;
            }
        }

        public void Register(string key, IPoint<T> point)
        {
            if(!Find(key))
            {
                mapping.Add(key, point);
            }
        }

        public void Remove(string key)
        {
            if (Find(key))
            {
                mapping.Remove(key);
            }
        }

        public bool SetQuality(string key, QUALITIES quality)
        {
            if (Find(key))
            {
               return mapping[key].SetQuality(quality);
            }
            else
            {
              return  false;
            }
        }

        public int SetValue(string key, T[] value)
        {
            if (Find(key))
            {
              return  mapping[key].SetValue(value)?1:-1;
            }
            else
            {
                return -1;
            }
        }
    }
}
