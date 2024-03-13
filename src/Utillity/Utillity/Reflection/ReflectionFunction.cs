using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.Reflection
{
    public static class ReflectionFunction
    {
        /// <summary>
        /// 获取dll中集成指定接口非抽象类
        /// 非强制占用DLL
        /// </summary>
        /// <param name="assemblyPath">dll路径名称</param>
        /// <returns></returns>
        public static List<Type> GetTypesOnlyLoad(string assemblyPath, string interfaceName = "IPLCDriver")
        {
            var result = new List<Type>();

            //LoadForm函数会强制锁定DLL文件至应用结束
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
            //非强制占用DLL，不会加载依赖项
            Assembly ass = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            Type[] types = ass.GetTypes();

            foreach (var t in types)
            {

                if (t.GetInterface(interfaceName) != null && !t.IsAbstract)
                {

                    result.Add(t);
                }
            }
            return result;
        }
        public static List<Type> GetTypesLoad(string assemblyPath, string interfaceName = "IPLCDriver")
        {
            var result = new List<Type>();
            //会强制占用DLL并加载相关依赖项
            Assembly ass = Assembly.LoadFrom(assemblyPath);
            Type[] types = ass.GetTypes();

            foreach (var t in types)
            {
                if (t.GetInterface(interfaceName) != null && !t.IsAbstract)
                {
                    result.Add(t);
                }
            }
            return result;
        }

        private static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
          return  Assembly.ReflectionOnlyLoad(args.Name);
        }
    }
}
