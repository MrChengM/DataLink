using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
namespace ModbusDrivers.Server
{
    public class ModbusPointMapping<T>: IPointMapping<T>
    {

        Dictionary<string, IPoint<T>> mapping = new Dictionary<string, IPoint<T>>();
        /// <summary>
        /// 线圈地址空间 key=00001~09999
        /// </summary>
        //Dictionary<string, IPoint<bool>> coil = new Dictionary<string, IPoint<bool>>(10000);
        ///// <summary>
        ///// 输入状态地址空间 key=10001～19999
        ///// </summary>
        //Dictionary<string, IPoint<bool>> inputStatus = new Dictionary<string, IPoint<bool>>(10000);
        ///// <summary>
        ///// 输入寄存器空间 key=30001～39999
        ///// </summary>
        //Dictionary<string, IPoint<short>> inPutRegister = new Dictionary<string, IPoint<short>>(10000);
        ///// <summary>
        ///// 保持寄存器空间 key=40001～49999
        ///// </summary>
        //Dictionary<string, IPoint<short>> holdRegister = new Dictionary<string, IPoint<short>>(10000);

        public static ModbusPointMapping<T> Instance;

        private ModbusPointMapping()
        {
            if (Instance == null)
                Instance = new ModbusPointMapping<T>();
        }

        public void Register(string key, IPoint<T> point)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key, IPoint<T> point)
        {
            throw new NotImplementedException();
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
            if (Find(key))
            {
               IPoint<T> point= Get(key);
            }
        }

        public T[] GetValue(string key, int count)
        {
            throw new NotImplementedException();
        }
        public int Set(string key, T[] value)
        {
            throw new NotImplementedException();
        }

        public IPoint<T> Get(string key)
        {
            throw new NotImplementedException();
        }
    }
}
