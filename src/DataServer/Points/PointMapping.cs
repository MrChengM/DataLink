using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Points
{
    public interface IPointMapping
    {

        void Register(string key, IPoint<bool> point);
        void Register(string key, IPoint<byte> point);
        void Register(string key, IPoint<short> point);
        void Register(string key, IPoint<ushort> point);
        void Register(string key, IPoint<int> point);
        void Register(string key, IPoint<uint> point);
        void Register(string key, IPoint<float> point);
        //void Register(string key, IPoint<double> point);
        void Register(string key, IPoint<string> point);
        /// <summary>
        /// 从Mapping中移除点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="point"></param>
        void Remove(string key);
        /// <summary>
        /// 通过键索引查找点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Find(string key);
        PointMetadata GetPointMetaData(string key);

        IPoint<bool> GetBoolPoint(string key);
        IPoint<byte> GetBytePoint(string key);
        IPoint<short> GetShortPoint(string key);
        IPoint<ushort> GetUShortPoint(string key);
        IPoint<int> GetIntPoint(string key);
        IPoint<uint> GetUIntPoint(string key);
        IPoint<float> GetFloatPoint(string key);
        //IPoint<double> GetDoublePoint(string key);
        IPoint<string> GetStringPoint(string key);

        ITag GetTag(PointNameIndex pointNameIndex);
        ITag[] GetTags(string pointName);

        Dictionary<string, PointMetadata> GetPointMetadatas();
    }
    public class PointMapping : IPointMapping
    {
        private Dictionary<string, IPoint<bool>> _boolPointMapping = new Dictionary<string, IPoint<bool>>();
        private Dictionary<string, IPoint<byte>> _bytePointMapping = new Dictionary<string, IPoint<byte>>();
        private Dictionary<string, IPoint<short>> _shortPointMapping = new Dictionary<string, IPoint<short>>();
        private Dictionary<string, IPoint<ushort>> _ushortPointMapping = new Dictionary<string, IPoint<ushort>>();
        private Dictionary<string, IPoint<int>> _intPointMapping = new Dictionary<string, IPoint<int>>();
        private Dictionary<string, IPoint<uint>> _uintPointMapping = new Dictionary<string, IPoint<uint>>();
        private Dictionary<string, IPoint<float>> _floatPointMapping = new Dictionary<string, IPoint<float>>();
        //private Dictionary<string, IPoint<double>> _doublePointMapping = new Dictionary<string, IPoint<double>>();
        private Dictionary<string, IPoint<string>> _stringPointMapping = new Dictionary<string, IPoint<string>>();
        private Dictionary<string, PointMetadata> _pointMetaMapping = new Dictionary<string, PointMetadata>();

        public bool Find(string key)
        {
          return _pointMetaMapping.ContainsKey(key);
        }

        public IPoint<bool> GetBoolPoint(string key)
        {
            _boolPointMapping.TryGetValue(key, out IPoint<bool> result);
            return result;
        }

        public IPoint<byte> GetBytePoint(string key)
        {
            _bytePointMapping.TryGetValue(key, out IPoint<byte> result);
            return result;
        }

        //public IPoint<double> GetDoublePoint(string key)
        //{
        //    _doublePointMapping.TryGetValue(key, out IPoint<double> result);
        //    return result;
        //}

        public IPoint<float> GetFloatPoint(string key)
        {
            _floatPointMapping.TryGetValue(key, out IPoint<float> result);
            return result;
        }

        public IPoint<int> GetIntPoint(string key)
        {
            _intPointMapping.TryGetValue(key, out IPoint<int> result);
            return result;
        }
        public PointMetadata GetPointMetaData(string key)
        {
            if (Find(key))
            {
                return _pointMetaMapping[key];
            }
            else
            {
                return null;
            }
        }

        //public IPoint<bool> GetPoint(string key)
        //{
        //    IPoint<bool> result;
        //    _boolPointMapping.TryGetValue(key, out result);
        //    return result;
        //}

        public Dictionary<string,PointMetadata> GetPointMetadatas()
        {
            return _pointMetaMapping;
        }

        public IPoint<short> GetShortPoint(string key)
        {
            _shortPointMapping.TryGetValue(key, out IPoint<short> result);
            return result;
        }

        public IPoint<string> GetStringPoint(string key)
        {
            _stringPointMapping.TryGetValue(key, out IPoint<string> result);
            return result;
        }
        public IPoint<uint> GetUIntPoint(string key)
        {
            _uintPointMapping.TryGetValue(key, out IPoint<uint> result);
            return result;
        }

        public IPoint<ushort> GetUShortPoint(string key)
        {
            _ushortPointMapping.TryGetValue(key, out IPoint<ushort> result);
            return result;
        }
        public ITag GetTag(PointNameIndex pointNameIndex)
        {
            ITag result;   
            var metaData = GetPointMetaData(pointNameIndex.PointName);
            string pointName = metaData.Name;
            int index = pointNameIndex.Index;
            var type = metaData.ValueType;
            if (type == DataType.Bool)
            {
                var point =GetBoolPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
            }
            else if (type == DataType.Byte)
            {
                var point = GetBytePoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };

            }
            else if (type == DataType.UShort)
            {
                var point = GetUShortPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };


            }
            else if (type == DataType.Short)
            {
                var point = GetShortPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };

            }
            else if (type == DataType.UInt)
            {
                var point = GetUIntPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };

            }
            else if (type == DataType.Int)
            {
                var point = GetIntPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };


            }
            else if (type == DataType.Float)
            {
                var point = GetFloatPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
            }
            else if (type == DataType.String)
            {
                var point = GetStringPoint(pointName);
                var value = point[index];
                var quality = point.GetQuality();
                result = new Tag { Name = pointName + $"[{index}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value};
            }
            else
            {
                return null;
            }
            return result;
        }
        public ITag[] GetTags(string pointName)
        {
            ITag[] result ;

            var metaData = GetPointMetaData(pointName);
            var length = metaData.Length;
            var type = metaData.ValueType;

            result = new ITag[length];
            if (type == DataType.Bool)
            {
                var point = GetBoolPoint(pointName);
                
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }
            }
            else if (type == DataType.Byte)
            {
                var point = GetBytePoint(pointName);
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }
            }
            else if (type == DataType.UShort)
            {
                var point = GetUShortPoint(pointName);
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }

            }
            else if (type == DataType.Short)
            {
                var point = GetShortPoint(pointName);
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }
            }
            else if (type == DataType.UInt)
            {
                var point = GetUIntPoint(pointName);
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }
            }
            else if (type == DataType.Int)
            {
                var point = GetIntPoint(pointName);
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }

            }
            else if (type == DataType.Float)
            {
                var point = GetFloatPoint(pointName);
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }
            }
            else if (type == DataType.String)
            {
                var point = GetStringPoint(pointName);
                var quality = point.GetQuality();
                for (int i = 0; i < length; i++)
                {
                    var value = point[i];
                    result[i] = new Tag { Name = pointName + $"[{i}]", Quality = quality, TimeStamp = DateTime.Now, Type = type, Value = value.ToString() };
                }
            }
            else
            {
                return null;
            }
            return result;
        }
        public void Register(string key, IPoint<bool> point)
        {
            if (!Find(key))
            {
                _boolPointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Bool, point.Length, point.IsVirtual()));
            }
        }

        public void Register(string key, IPoint<byte> point)
        {

            if (!Find(key))
            {
                _bytePointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Byte, point.Length, point.IsVirtual()));
            }
        }

        public void Register(string key, IPoint<short> point)
        {

            if (!Find(key))
            {
                _shortPointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Short, point.Length, point.IsVirtual()));

            }
        }

        public void Register(string key, IPoint<ushort> point)
        {
            if (!Find(key))
            {
                _ushortPointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.UShort, point.Length, point.IsVirtual()));

            }
        }

        public void Register(string key, IPoint<int> point)
        {
            if (!Find(key))
            {
                _intPointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Int, point.Length, point.IsVirtual()));

            }
        }

        public void Register(string key, IPoint<uint> point)
        {
            if (!Find(key))
            {
                _uintPointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.UInt, point.Length, point.IsVirtual()));

            }
        }

        public void Register(string key, IPoint<float> point)
        {
            if (!Find(key))
            {
                _floatPointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Float, point.Length, point.IsVirtual()));

            }
        }

        //public void Register(string key, IPoint<double> point)
        //{
        //    if (!Find(key))
        //    {
        //        _doublePointMapping.Add(key, point);
        //        _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Double, point.Length, point.IsVirtual()));

        //    }
        //}

        public void Register(string key, IPoint<string> point)
        {
            if (!Find(key))
            {
                _stringPointMapping.Add(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.String, point.Length, point.IsVirtual()));
            }
        }

        public void Remove(string key)
        {
            var meteData = GetPointMetaData(key);
            if (meteData!=null)
            {
                switch (meteData.ValueType)
                {
                    case DataType.Bool:
                        _boolPointMapping.Remove(key);
                        break;
                    case DataType.Byte:
                        _bytePointMapping.Remove(key);
                        break;
                    case DataType.Short:
                        _shortPointMapping.Remove(key);
                        break;
                    case DataType.UShort:
                        _ushortPointMapping.Remove(key);
                        break;
                    case DataType.Int:
                        _intPointMapping.Remove(key);
                        break;
                    case DataType.UInt:
                        _uintPointMapping.Remove(key);
                        break;
                    case DataType.Float:
                        _floatPointMapping.Remove(key);
                        break;
                    //case DataType.Double:
                    //    _doublePointMapping.Remove(key);
                    //    break;
                    case DataType.String:
                        _stringPointMapping.Remove(key);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public class PointMetadata
    {
        public PointMetadata(string name, DataType valueType,int length,bool isVirtual)
        {
            Name = name;
            ValueType = valueType;
            Length = length;
            IsVirtual = isVirtual;
        }

        public string Name { get; set; }
        public DataType ValueType { get; set; }
        public int Length { get; set; }
        public bool IsVirtual { get; set; }
    }
    public class PointNameIndex
    {
        public PointNameIndex(string name, int index)
        {
            PointName = name;
            Index = index;
        }
        public string PointName { get; set; }

        public int Index { get; set; }
    }
}
