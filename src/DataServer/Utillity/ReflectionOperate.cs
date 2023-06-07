using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DataServer.Config;
using System.ComponentModel;
namespace DataServer.Utillity
{
    public class ReflectionOperate
    {

        /// <summary>
        /// 获取协议驱动类元数据
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static  Dictionary<string,DriverInfo> GetInfos(string assemblyPath,string interfaceName= "IPLCDriver")
        {
            var result = new Dictionary<string, DriverInfo>();

            Assembly ass = Assembly.LoadFrom(assemblyPath);
            Type[] types = ass.GetTypes();

            foreach (var t in types)
            {
                
                if (t.GetInterface(interfaceName) !=null && !t.IsAbstract)
                {
                    var ddAttribute = t.GetCustomAttribute<DriverDescriptionAttribute>();
                    string des = ddAttribute.Description;
                    DriverInfo info = new DriverInfo()
                    {
                        Description = des,
                        FullName = t.FullName,
                        CommType = ddAttribute.CommType,
                    };
                    foreach (PropertyInfo p in t.GetRuntimeProperties())
                    {
                        if(p.GetCustomAttribute<DeviceMarkAttribute>()!=null)
                        {
                            info.DevicePropertyDes.Add(p);
                        }
                    }
                    result.Add(des,info);
                }
            }
            return result;
        }


    }
}
