using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ds = DataServer;
using DataServer;
using DataServer.Points;
namespace ModbusDrivers.Server
{
    /// <summary>
    /// Modbus点表
    /// 第一次实例化需要初始化Init（）；
    /// </summary>
    public class ModbusPointMapping : IPointMapping<bool>, IPointMapping<ushort>///Point Length=1;
    {
        private ILog _log;

        /// <summary>
        /// Mapping点存储索引
        /// </summary>
        private List<ModbusPointMeta> indexList= new List<ModbusPointMeta>();//未使用
        /// <summary>
        /// 线圈地址空间 key=00001~09999
        /// </summary>
        Dictionary<string, IPoint<bool>> coilMapping= new Dictionary<string, IPoint<bool>>(10000);
        /// <summary>
        /// 输入状态地址空间 key=10001～19999
        /// </summary>
        Dictionary<string, IPoint<bool>> inputStatusMapping= new Dictionary<string, IPoint<bool>>(10000);
        /// <summary>
        /// 输入寄存器空间 key=30001～39999
        /// </summary>
        Dictionary<string, IPoint<ushort>> inPutRegisterMapping= new Dictionary<string, IPoint<ushort>>(10000);
        /// <summary>
        /// 保持寄存器空间 key=40001～49999
        /// </summary>
        Dictionary<string, IPoint<ushort>> holdRegisterMapping= new Dictionary<string, IPoint<ushort>>(10000);

        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        #region 单例，确保只存在一个Moubus点表
        private static ModbusPointMapping instance;

        private static readonly object locker = new object();

        private ModbusPointMapping(ILog log)
        {
            _log = log;

        }
        //public void Init(ILog log)
        //{
        //    this.log = log;
        //    indexList = new List<ModbusPointMeta>();
        //    coilMapping = new Dictionary<string, IPoint<bool>>(10000);
        //    inputStatusMapping = new Dictionary<string, IPoint<bool>>(10000);
        //    inPutRegisterMapping = new Dictionary<string, IPoint<ushort>>(10000);
        //    holdRegisterMapping = new Dictionary<string, IPoint<ushort>>(10000);
        //}
        public static ModbusPointMapping GetInstance(ILog log)
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new ModbusPointMapping(log);
                    }
                }
            }
            return instance;
        }
        #endregion

        #region IPointMapping:bool,ushort
        /// <summary>
        /// 注册bool类型modbus点
        /// </summary>
        /// <param name="key">00001～09999，10001～19999</param>
        /// <param name="point">长度为1的bool型</param>
        public void Register(string key, IPoint<bool> point)
        {
            if (!Find(key))
            {
                if (key.Substring(0, 1) == "0")
                {
                    coilMapping.Add(key, point);
                }
                else if (key.Substring(0, 1) == "1")
                {
                    inputStatusMapping.Add(key, point);
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Register Error: Key value not match point Type,Key : {0} , type ： {1}", key, point.ValueType);
                }
            }
            else
            {
                _log.ErrorLog("ModbusMapping Register Error: Key have exist,Key : {0} ", key);

            }
        }

        public void Remove(string key)
        {
            string type;
            if(Find(key,out type))
            {
                switch (type)
                {
                    case ModbusType.Coil:
                        coilMapping.Remove(key);
                        break;
                    case ModbusType.InputStatus:
                        coilMapping.Remove(key);
                        break;
                    case ModbusType.InputRegister:
                        coilMapping.Remove(key);
                        break;
                    case ModbusType.HoldRegister:
                        coilMapping.Remove(key);
                        break;
                }
            }
        }
 
        public bool Find(string key)
        {
           string type;
           return Find(key, out type);
        }

        public bool Find(string key, out string type)
        {
            string Head = key.Substring(0, 1);
            switch (Head)
            {
                case "0":
                    type = ModbusType.Coil;
                    return coilMapping.ContainsKey(key);
                case "1":
                    type = ModbusType.InputStatus;
                    return inputStatusMapping.ContainsKey(key);
                case "3":
                    type = ModbusType.InputRegister;
                    return inPutRegisterMapping.ContainsKey(key);
                case "4":
                    type = ModbusType.HoldRegister;
                    return holdRegisterMapping.ContainsKey(key);
                default:
                    type = null;
                    return false;
            }
        }
        public IPoint<bool> GetPoint(string key)
        {
            string type;
            if (Find(key, out type))
            {
                if (type == ModbusType.Coil)
                {
                   return coilMapping[key];
                }
                else if(type == ModbusType.InputStatus)
                {
                    return inputStatusMapping[key];
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Get Point Error: Key value not match point Type,Key : {0} , type ： {1}", key, type);
                    return null;
                }
            }
            else
            {
                _log.ErrorLog("ModbusMapping Get Point Error: Point not exsit ,Key : {0} ", key);
                return null;
            }
            
        }

        public bool[] GetValue(string key)
        {
            IPoint<bool> point = GetPoint(key);
            if (point != null)
            {
                return point.GetValues();
            }
            else
            {
                return null;
            }
        }
        public int SetValue(string key, bool[] value)
        {
            string type;
            if (Find(key, out type))
            {
                if (type == ModbusType.Coil)
                {
                    coilMapping[key].SetValue(value);
                    return 1;
                      
                }
                else if (type == ModbusType.InputStatus)
                {
                    inputStatusMapping[key].SetValue(value);
                    return 1;
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Set Point Error: Key value not match point Type,Key : {0} , type ： {1}", key, type);
                    return -1;
                }
            }
            else
            {
                _log.ErrorLog("ModbusMapping Set Point Error: Point not exsit ,Key : {0} ", key);
                return -1;
            }
        }
        /// <summary>
        /// 注册ushort类型数据
        /// </summary>
        /// <param name="key">30001～39999，40001～49999</param>
        /// <param name="point">长度为1的ushort点</param>
        public void Register(string key, IPoint<ushort> point)
        {
                if (!Find(key))
                {

                    if (key.Substring(0, 1) == "3")
                    {
                        inPutRegisterMapping.Add(key, point);
                    }
                    else if (key.Substring(0, 1) == "4")
                    {
                        holdRegisterMapping.Add(key, point);
                    }
                    else
                    {
                        _log.ErrorLog("ModbusMapping Register Error: Key value not match point Type,Key : {0} , type ： {1}", key, point.ValueType);
                    }
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Register Error: Key have exist,Key : {0} ", key);
                }
        }



        IPoint<ushort> IPointMapping<ushort>.GetPoint(string key)
        {
            string type;
            if (Find(key, out type))
            {
                if (type == ModbusType.InputRegister)
                {
                    return inPutRegisterMapping[key];
                }
                else if (type == ModbusType.HoldRegister)
                {
                    return holdRegisterMapping[key];
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Get Point Error: Key value not match point Type,Key : {0} , type ： {1}", key, type);
                    return null;
                }
            }
            else
            {
                _log.ErrorLog("ModbusMapping Get Point Error: Point not exsit ,Key : {0} ", key);
                return null;
            }
        }

        ushort[] IPointMapping<ushort>.GetValue(string key)
        {

            string type;
            if (Find(key, out type))
            {
                if (type == ModbusType.InputRegister)
                {
                    return inPutRegisterMapping[key].GetValues();
                }
                else if (type == ModbusType.HoldRegister)
                {
                    return holdRegisterMapping[key].GetValues();
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Get Point Error: Key value not match point Type,Key : {0} , type ： {1}", key, type);
                    return null;
                }
            }
            else
            {
                _log.ErrorLog("ModbusMapping Get Point Error: Point not exsit ,Key : {0} ", key);
                return null;
            }
        }

        public int SetValue(string key, ushort[] value)
        {
            string type;
            if (Find(key, out type))
            {
                if (type == ModbusType.InputRegister)
                {
                   return inPutRegisterMapping[key].SetValue(value)?1:-1;
                }
                else if (type == ModbusType.HoldRegister)
                {
                    return holdRegisterMapping[key].SetValue(value)?1:-1;
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Get Point Error: Key value not match point Type,Key : {0} , type ： {1}", key, type);
                    return -1;
                }
            }
            else
            {
                _log.ErrorLog("ModbusMapping Get Point Error: Point not exsit ,Key : {0} ", key);
                return -1;
            }
        }

        public bool SetQuality(string key, QUALITIES quality)
        {
            string type;
            if (Find(key, out type))
            {
                if (type == ModbusType.Coil)
                {
                    return coilMapping[key].SetQuality(quality) ? true : false;
                }
                else if (type == ModbusType.InputStatus)
                {
                    return inputStatusMapping[key].SetQuality(quality) ? true : false;
                }
                if (type == ModbusType.InputRegister)
                {
                    return inPutRegisterMapping[key].SetQuality(quality) ? true :false;
                }
                else if (type == ModbusType.HoldRegister)
                {
                    return holdRegisterMapping[key].SetQuality(quality) ? true : false;
                }
                else
                {
                    _log.ErrorLog("ModbusMapping Get Point Error: Key value not match point Type,Key : {0} , type ： {1}", key, type);
                    return false;
                }
            }
            else
            {
                _log.ErrorLog("ModbusMapping Get Point Error: Point not exsit ,Key : {0} ", key);
                return false;
            }
        }

        public bool GetValue(string key, byte index)
        {
            IPoint<bool> point = GetPoint(key);
            if (point != null)
            {
                return point.GetValue(index);
            }
            else
            {
                return default(bool);
            }
        }

        public int SetValue(string key, bool value, byte index)
        {
            IPoint<bool> point = GetPoint(key);
            if (point != null)
            {
                return point.SetValue(value,index)?1:-1;
            }
            else
            {
                return -1;
            }
        }

        ushort IPointMapping<ushort>.GetValue(string key, byte index)
        {
            IPoint<ushort> point = (this as IPointMapping<ushort>).GetPoint(key);
            if (point != null)
            {
                return point.GetValue(index);
            }
            else
            {
                return default(ushort);
            }
        }

        public int SetValue(string key, ushort value, byte index)
        {
            IPoint<ushort> point = (this as IPointMapping<ushort>).GetPoint(key);
            if (point != null)
            {
                return point.SetValue(value,index)?1:-1;
            }
            else
            {
                return -1;
            }
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
    internal class ModbusPointMeta
    {
        /// <summary>
        /// Modbus点名，例如:00001,10001,30001,40001
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// modbus格式,例如：coil,inputstatus,inputregister,holdregister
        /// </summary>
        public string Type { get; set; }

    }
}
