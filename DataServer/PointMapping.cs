using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public class PointMapping<T> : IPointMapping<T>
    {
        private ILog log;

        public static PointMapping<T> Instance;

        private PointMapping()
        {
            if (Instance == null)
            {
                Instance = new PointMapping<T>();
            }
        }

        public ILog Log
        {
            get { return log; }
            set { log = value; }
        }

        Dictionary<string, IPoint<T>> mapping = new Dictionary<string, IPoint<T>>();

        public void Register(string key, IPoint<T> point)
        {
            if (!Find(key) && !Find(point))
            {
                mapping.Add(key, point);
            }
            else
            {
                log.ErrorLog(string.Format("signal Name already exist,name:{0}"), point.Name);
            }
        }

        public void Remove(string key, IPoint<T> point)
        {
            if (Find(key) || Find(point))
            {
                mapping.Remove(key);
            }
            else
            {
                log.ErrorLog(string.Format("signal Name not exist,name:{0}"), point.Name);
            }
        }

        public bool Find(IPoint<T> point)
        {
            return mapping.ContainsValue(point);

        }

        public bool Find(string key)
        {
            return mapping.ContainsKey(key);
        }

        public bool Find(string key, int count)
        {
            throw new NotImplementedException();
        }

        public IPoint<T> Get(string key)
        {
            if (Find(key))
                return mapping[key];
            else
                log.ErrorLog(string.Format("signal Name not exist,name:{0}"), key);
            return null;

        }

        //public T GetValue(string key)
        //{
        //    if (Find(key))
        //        return mapping[key].GetValues()[0];
        //    else
        //        return default(T);
        //}

        public T[] GetValue(string key, int count)
        {
            if (Find(key))
            {
                return mapping[key].GetValues();
            }
            else
                log.ErrorLog(string.Format("signal Name not exist,name:{0}"), key);

            return null;
        }

        //public int Set(string key, T value)
        //{
        //    throw new NotImplementedException();
        //}

        public int Set(string key, T[] value)
        {
            if (Find(key))
            {
                IPoint<T> point = mapping[key];
                point.SetValue(value);
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
