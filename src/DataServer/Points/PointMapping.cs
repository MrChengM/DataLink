using System;
using System.Collections.Concurrent;
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

        WriteResult WritePoint(PointMetadata pointMeta,int index,string value);
        WriteResult WritePoint(string pointName, int index, string value);

        Dictionary<string, PointMetadata> GetPointMetadatas();

        event Action<Tag> PointChangeEvent;
    }
    public class PointMapping : IPointMapping
    {
        private ConcurrentDictionary<string, IPoint<bool>> _boolPointMapping = new ConcurrentDictionary<string, IPoint<bool>>();
        private ConcurrentDictionary<string, IPoint<byte>> _bytePointMapping = new ConcurrentDictionary<string, IPoint<byte>>();
        private ConcurrentDictionary<string, IPoint<short>> _shortPointMapping = new ConcurrentDictionary<string, IPoint<short>>();
        private ConcurrentDictionary<string, IPoint<ushort>> _ushortPointMapping = new ConcurrentDictionary<string, IPoint<ushort>>();
        private ConcurrentDictionary<string, IPoint<int>> _intPointMapping = new ConcurrentDictionary<string, IPoint<int>>();
        private ConcurrentDictionary<string, IPoint<uint>> _uintPointMapping = new ConcurrentDictionary<string, IPoint<uint>>();
        private ConcurrentDictionary<string, IPoint<float>> _floatPointMapping = new ConcurrentDictionary<string, IPoint<float>>();
        //private Dictionary<string, IPoint<double>> _doublePointMapping = new Dictionary<string, IPoint<double>>();
        private ConcurrentDictionary<string, IPoint<string>> _stringPointMapping = new ConcurrentDictionary<string, IPoint<string>>();
        private Dictionary<string, PointMetadata> _pointMetaMapping = new Dictionary<string, PointMetadata>();

        public event Action<Tag> PointChangeEvent;

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
                _boolPointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Bool, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnBoolPointUpdataEvent;
            }
        }

        private void OnBoolPointUpdataEvent(IPoint<bool> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.Bool, Value = point[index].ToString() };
                RaisePointChange(tag);
            }

        }

        public void Register(string key, IPoint<byte> point)
        {

            if (!Find(key))
            {
                _bytePointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Byte, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnBytePointUpdataEvent;

            }
        }
        private void OnBytePointUpdataEvent(IPoint<byte> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.Byte, Value = point[index].ToString() };
                RaisePointChange(tag);
            }

        }
        public void Register(string key, IPoint<short> point)
        {

            if (!Find(key))
            {
                _shortPointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Short, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnShortPointUpdataEvent;

            }
        }
        private void OnShortPointUpdataEvent(IPoint<short> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.Short, Value = point[index].ToString() };
                RaisePointChange(tag);
            }

        }
        public void Register(string key, IPoint<ushort> point)
        {
            if (!Find(key))
            {
                _ushortPointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.UShort, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnUShortPointUpdataEvent;

            }
        }
        private void OnUShortPointUpdataEvent(IPoint<ushort> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.UShort, Value = point[index].ToString() };
                RaisePointChange(tag);
            }

        }
        public void Register(string key, IPoint<int> point)
        {
            if (!Find(key))
            {
                _intPointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Int, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnIntPointUpdataEvent;

            }
        }
        private void OnIntPointUpdataEvent(IPoint<int> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.Int, Value = point[index].ToString() };
                RaisePointChange(tag);
            }

        }
        public void Register(string key, IPoint<uint> point)
        {
            if (!Find(key))
            {
                _uintPointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.UInt, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnUIntPointUpdataEvent;

            }
        }
        private void OnUIntPointUpdataEvent(IPoint<uint> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.UInt, Value = point[index].ToString() };
                RaisePointChange(tag);
            }

        }
        public void Register(string key, IPoint<float> point)
        {
            if (!Find(key))
            {
                _floatPointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.Float, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnFloatPointUpdataEvent;

            }
        }
        private void OnFloatPointUpdataEvent(IPoint<float> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.Float, Value = point[index].ToString() };
                RaisePointChange(tag);
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
                _stringPointMapping.TryAdd(key, point);
                _pointMetaMapping.Add(key, new PointMetadata(key, DataType.String, point.Length, point.IsVirtual()));
                point.UpdataEvent += OnStringPointUpdataEvent;

            }
        }
        private void OnStringPointUpdataEvent(IPoint<string> point, int index)
        {
            if (index > -1 && index < point.Length)
            {
                var tag = new Tag { Name = point.Name + $"[{index}]", Quality = point.GetQuality(), TimeStamp = DateTime.Now, Type = DataType.String, Value = point[index] };
                RaisePointChange(tag);
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
                        _boolPointMapping[key].UpdataEvent -= OnBoolPointUpdataEvent;
                        _boolPointMapping.TryRemove(key,out _);
                        break;
                    case DataType.Byte:
                        _bytePointMapping[key].UpdataEvent -= OnBytePointUpdataEvent;
                        _bytePointMapping.TryRemove(key, out _);
                        break;
                    case DataType.Short:
                        _shortPointMapping[key].UpdataEvent -= OnShortPointUpdataEvent;
                        _shortPointMapping.TryRemove(key, out _);
                        break;
                    case DataType.UShort:
                        _ushortPointMapping[key].UpdataEvent -= OnUShortPointUpdataEvent;
                        _ushortPointMapping.TryRemove(key, out _);
                        break;
                    case DataType.Int:
                        _intPointMapping[key].UpdataEvent -= OnIntPointUpdataEvent;
                        _intPointMapping.TryRemove(key,out _);
                        break;
                    case DataType.UInt:
                        _uintPointMapping[key].UpdataEvent -= OnUIntPointUpdataEvent;
                        _uintPointMapping.TryRemove(key,out _);
                        break;
                    case DataType.Float:
                        _floatPointMapping[key].UpdataEvent -= OnFloatPointUpdataEvent;
                        _floatPointMapping.TryRemove(key,out _);
                        break;
                    //case DataType.Double:
                    //    _doublePointMapping.Remove(key);
                    //    break;
                    case DataType.String:
                        _stringPointMapping[key].UpdataEvent -= OnStringPointUpdataEvent;
                        _stringPointMapping.TryRemove(key,out _);
                        break;
                    default:
                        break;
                }
            }
        }

        void RaisePointChange(Tag tag)
        {
            PointChangeEvent?.Invoke(tag);
        }
        public WriteResult WritePoint(string pointName, int index, string value) 
        {
            var pointMeta = GetPointMetaData(pointName);
            return WritePoint(pointMeta, index, value);
        }

        public WriteResult WritePoint(PointMetadata pointMeta, int index, string value)
        {

            string pointName = pointMeta.Name;
            var type = pointMeta.ValueType;
            WriteResult result = new WriteResult();
            string msgheader=string.Concat("the point name:", pointName, " type : ", type," information: ");

            if (type == DataType.Bool)
            {
                bool temp;
                if (bool.TryParse(value, out temp))
                {
                    var point = GetBoolPoint(pointName);
                    if (point is DevicePoint<bool> dp)
                    {
                        result = dp.Write(index, temp);
                    }
                    else
                    {
                        if (point.SetValue(temp, index))
                        {
                            result.Result = OperateResult.OK;
                        }
                        else
                        {
                            result.Result = OperateResult.NG;
                            result.Messages = string.Concat(msgheader, "set value error");

                        }

                    }
                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = string.Concat(msgheader, "value type not match bool");

                }


            }
            else if (type == DataType.Byte)
            {
                byte temp;
                if (byte.TryParse(value, out temp))
                {
                    var point = GetBytePoint(pointName);
                    if (point is DevicePoint<byte> dp)
                    {
                        result = dp.Write(index, temp);
                    }
                    else
                    {
                        if (point.SetValue(temp, index))
                        {
                            result.Result = OperateResult.OK;
                        }
                        else
                        {
                            result.Result = OperateResult.NG;
                            result.Messages = string.Concat(msgheader, "set value error");

                        }

                    }
                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = string.Concat(msgheader, "value type not match byte");
                }
            }
            else if (type == DataType.UShort)
            {
                ushort temp;
                if (ushort.TryParse(value, out temp))
                {
                    var point = GetUShortPoint(pointName);
                    if (point is DevicePoint<ushort> dp)
                    {
                        result = dp.Write(index, temp);
                    }
                    else
                    {
                        if (point.SetValue(temp, index))
                        {
                            result.Result = OperateResult.OK;
                        }
                        else
                        {
                            result.Result = OperateResult.NG;
                            result.Messages = string.Concat(msgheader, "set value error");
                        }

                    }
                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = string.Concat(msgheader, "value type not match ushort");
                }
            }
            else if (type == DataType.Short)
            {
                short temp;
                if (short.TryParse(value, out temp))
                {
                    var point = GetShortPoint(pointName);
                    if (point is DevicePoint<short> dp)
                    {
                        result = dp.Write(index, temp);
                    }
                    else
                    {
                        if (point.SetValue(temp, index))
                        {
                            result.Result = OperateResult.OK;
                        }
                        else
                        {
                            result.Result = OperateResult.NG;
                            result.Messages = string.Concat(msgheader, "set value error");
                        }

                    }
                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = string.Concat(msgheader, "value type not match short");
                }
            }
            else if (type == DataType.UInt)
            {
                uint temp;
                if (uint.TryParse(value, out temp))
                {
                    var point = GetUIntPoint(pointName);
                    if (point is DevicePoint<uint> dp)
                    {
                        result = dp.Write(index, temp);
                    }
                    else
                    {
                        if (point.SetValue(temp, index))
                        {
                            result.Result = OperateResult.OK;
                        }
                        else
                        {
                            result.Result = OperateResult.NG;
                            result.Messages = string.Concat(msgheader, "set value error");
                        }

                    }
                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = string.Concat(msgheader, "value type not match uint");
                }
            }
            else if (type == DataType.Int)
            {
                int temp;
                if (int.TryParse(value, out temp))
                {
                    var point = GetIntPoint(pointName);
                    if (point is DevicePoint<int> dp)
                    {
                        result = dp.Write(index, temp);
                    }
                    else
                    {
                        if (point.SetValue(temp, index))
                        {
                            result.Result = OperateResult.OK;
                        }
                        else
                        {
                            result.Result = OperateResult.NG;
                            result.Messages = string.Concat(msgheader, "set value error");
                        }

                    }
                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = string.Concat(msgheader, "value type not match int");
                }
            }
            else if (type == DataType.Float)
            {
                float temp;
                if (float.TryParse(value, out temp))
                {
                    var point = GetFloatPoint(pointName);
                    if (point is DevicePoint<float> dp)
                    {
                        result = dp.Write(index, temp);
                    }
                    else
                    {
                        if (point.SetValue(temp, index))
                        {
                            result.Result = OperateResult.OK;
                        }
                        else
                        {
                            result.Result = OperateResult.NG;
                            result.Messages = string.Concat(msgheader, "set value error");
                        }

                    }
                }
                else
                {
                    result.Result = OperateResult.NG;
                    result.Messages = string.Concat(msgheader, "value type not match uint");
                }
            }
            return result;
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
