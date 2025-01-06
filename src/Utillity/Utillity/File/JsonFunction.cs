using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.File
{
    public static class JsonFunction
    {
        /// <summary>
        /// 将类反序列化成Json格式
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T Load<T>(string filename)
        {
            T result = default;
            try
            {

                if (System.IO.File.Exists(filename))
                {
                    string content = "";
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        content = sr.ReadToEnd();
                        sr.Close();
                    }
                    result = JsonConvert.DeserializeObject<T>(content);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Json File Load fail : {e.Message}");
            }
            return result;


        }

        /// <summary>
        /// 将类序列化成Json保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool Save<T>(string filename, T source)
        {
            string content = JsonConvert.SerializeObject(source, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(content);
                sw.Close();
            }
            return true;
        }
    }
}
