using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Points
{
    /// <summary>
    /// 点列表泛型类
      /// </summary>
    /// <typeparam name="T">bool，short，int等数据类型</typeparam>
    public class PointMapping<T> : IPointMapping<T>
    {
        public static PointMapping<T> Instance;
        private static readonly object locker = new object();
        private PointMeDataList _indexList;

        Dictionary<string,IPoint<T>> mapping=new Dictionary<string, IPoint<T>>();
        private ILog _log;


        //public void Init(ILog log)
        //{
        //    mapping = new Dictionary<string, IPoint<T>>();
        //    _log = log;
        //}
        public static PointMapping<T> GetInstance(ILog log)
        {
            if (Instance == null)
            {
                lock (locker)
                {
                    if (Instance == null)
                    {
                        Instance = new PointMapping<T>(log);
                    }
                }
            }
            return Instance;
        }
        private PointMapping(ILog log)
        {
            this._log = log;
            _indexList = PointMeDataList.GetInstance();
        }
        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        public bool Find(string key)
        {
           return _indexList.Find(key);
        }

        public bool Find(string key, out string type)
        {
            return _indexList.Find(key, out type);
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
                _indexList.Add(new PointMetadata(point.Name, point.ValueType,point.Length,point.IsVirtual()));
            }
        }

        public void Remove(string key)
        {
            if (Find(key))
            {
                mapping.Remove(key);
                _indexList.Delete(key);
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

        public T GetValue(string key, byte index)
        {
            if (Find(key))
            {
                return GetPoint(key).GetValue(index);
            }
            else
            {
                return default(T);
            }
        }

        public int SetValue(string key, T value, byte index)
        {
            if (Find(key))
            {
                return mapping[key].SetValue(value, index) ? 1 : -1;
            }
            else
            {
                return -1;
            }
        }
    }
    public class PointMeDataList
    {
        List<PointMetadata> _data;
        #region 单例模式，PointMapping只有一个索引
        public static PointMeDataList Instance;
        private static readonly object locker=new object() ;
        private PointMeDataList()
        {
            _data = new List<PointMetadata>();
        }
        public static PointMeDataList GetInstance()
        {

            if (Instance == null)
            {
                lock (locker)
                {
                    if (Instance == null)
                    {
                        Instance = new PointMeDataList ();
                    }
                }
            }
            return Instance;
        }

        public List<PointMetadata> Data
        {
            get { return _data; }
        }
        #endregion
        #region 增，删，查，改
        public void Add(PointMetadata index)
        {
            if (!Find(index.Name))
                _data.Add(index);
        }

        public void Delete(string name )
        {
                _data.RemoveAll((s)=>s.Name==name);
        }
        public bool Find(string name)
        {
           return _data.Exists((s)=>s.Name==name);
        }
        public bool Find(string name,out string type)
        {
            var index = _data.Find((s) => s.Name == name);
            if(index!=null)
            {
                type = index.ValueType;
                return true;
            }
            else
            {
                type = null;
                return false;
                
            }
        }
        #endregion
    }
    public class PointMetadata
    {
        string _name;
        string _valueType;
        int _length;
        bool _isVirtual;

        public PointMetadata(string name,string valueType,int length,bool isVirtual)
        {
            _name = name;
            _valueType = valueType;
            _length = length;
            _isVirtual = isVirtual;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public bool IsVirtual
        {
            get { return _isVirtual; }
            set
            {
                _isVirtual = value;

            }
        }
    }
}
