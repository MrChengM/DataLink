using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataServer.Utillity
{
    public class ReaderXMLUtil
    {
        public ReaderXMLUtil() { }

        /// <summary>
        /// 根据XML配置文件生成具体的配置类
        /// 根据配置项的多少添加进List表内
        /// 待优化：
        /// 1.无法判断根节点及父子节点。
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <param name="filePath">配置文件目录</param>
        /// <param name="cellParse">托管函数：配置文件=>配置类逻辑</param>
        /// <param name="nodeElement">树状节点集合</param>
        /// <returns></returns>
        public static List<T> ReadXMLConfig<T>(string filePath, CellParse<T> cellParse, string nodeElement, string handler) where T:new()
        {
            List<T> result = new List<T>();
            Stream sFileSteam = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            XmlReader xmlReader = XmlReader.Create(sFileSteam);
            while (xmlReader.Read())
            {
                xmlReader.MoveToContent();
                while (xmlReader.IsStartElement(nodeElement))
                {
                    if (handler=="All"||xmlReader["handler"] == handler)
                    {
                        T element = new T();
                        cellParse(element, xmlReader);
                        result.Add(element);
                    }
                    xmlReader.Read();
                }
                xmlReader.Read();
            }
            sFileSteam.Flush();
            sFileSteam.Close();
            return result;
        }
        public delegate void CellParse<T>(T obj, XmlReader reader) where T : new();
    }
}


