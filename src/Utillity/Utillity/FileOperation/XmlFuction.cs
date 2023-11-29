using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utillity.FileOperation
{
    public class XmlFuction
    {
 
        //private static string _sFileName;
        //private List<List<string>> _lDataList;

        //节点名定义
        private  string _sMainNode = "/a:Workbook/a:Worksheet/a:Table";
        private  string _sLimbNode= "Row";
        private  string _sItemNode= "Cell";
        private  string _sDataNode= "Data";

        public XmlFuction()
        {
            //xmlDoc = new XmlDocument();
        }
       /* public string sFileName
        {
            get { return _sFileName; }
            set  {  _sFileName=value; }
        }
        */
        public string sMainNode
        {
            get { return _sMainNode; }
            set { _sMainNode = value; }
        }
        public string sLimbNode
        {
            get { return _sLimbNode; }
            set { _sLimbNode = value; }
        }
        public string sItemNode
        {
            get { return _sItemNode; }
            set { _sItemNode = value; }
        }
        public string sDataNode
        {
            get { return _sDataNode; }
            set { _sDataNode = value; }
        }
        public void XmlWrite(string _sFileName,List<List<string>> _lDataList)
        {
            int iCounts = (_lDataList.Count / 1000);
            for(int i=0; i <= iCounts; i++)
            {
                 XmlDocument xmlDoc = new XmlDocument();
                int iStartIndex = i * 1000;
                int iLength;
                if (i == iCounts)
                    iLength = _lDataList.Count - iStartIndex;
                else
                    iLength = 1000;
                List<List<string>> _lDataListSmall = _lDataList.GetRange(iStartIndex, iLength);
                xmlDoc.Load(_sFileName);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("a", "urn:schemas-microsoft-com:office:spreadsheet");
                XmlNode root = xmlDoc.SelectSingleNode(_sMainNode, nsmgr);
                if( i==0)
                root.RemoveAll();
                foreach (List<string> ls in _lDataListSmall)
                {
                    XmlElement xeLimb = xmlDoc.CreateElement(_sLimbNode, nsmgr.LookupNamespace("a"));
                    foreach (string ss in ls)
                    {
                        XmlElement xeItem = xmlDoc.CreateElement(_sItemNode, nsmgr.LookupNamespace("a"));
                        XmlElement xeData = xmlDoc.CreateElement(_sDataNode, nsmgr.LookupNamespace("a"));
                        XmlAttribute xa = xmlDoc.CreateAttribute("ss", "Type", nsmgr.LookupNamespace("a"));
                        xa.Value = "String";
                        xeData.Attributes.Append(xa);
                        xeData.InnerText = ss;
                        xeItem.AppendChild(xeData);
                        xeLimb.AppendChild(xeItem);
                    }
                    root.AppendChild(xeLimb);
                }
                xmlDoc.Save(_sFileName);
            }
        }
    }
    public class ReaderXMLUtil
    {
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
        public static List<T> ReadXMLConfig<T>(string filePath, CellParse<T> cellParse, string nodeElement, string handler,ILog log) where T : new()
        {
            List<T> result = new List<T>();
            string fileType = Path.GetExtension(filePath);
            if (fileType == ".xml")
            {
                if (File.Exists(filePath))
                {
                    Stream sFileSteam = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                    XmlReader xmlReader = XmlReader.Create(sFileSteam);
                    while (xmlReader.Read())
                    {
                        xmlReader.MoveToContent();
                        while (xmlReader.IsStartElement(nodeElement))
                        {
                            if (handler == "All" || xmlReader["handler"] == handler)
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
                }
                else
                {
                    log.ErrorLog("Read configration error: file not exsit!");
                }
            }
            else
            {
                log.ErrorLog("Read configration error: file type error!");
            }
            return result;
        }
        public delegate void CellParse<T>(T obj, XmlReader reader) where T : new();
        /// <summary>
        /// 根据委托函数判断根节点 父子节点 获取参数配置
        /// </summary>
        /// <typeparam name="T">泛型数据类型</typeparam>
        /// <param name="filePath">文件目录</param>
        /// <param name="cellParse">委托函数，具体逻辑判断</param>
        /// <returns></returns>
        public static List<T> ReadXMLConfig<T>(string filePath, CellParse1<T> cellParse,ILog log) 
        {
            List<T> result = new List<T>();
            string fileType = Path.GetExtension(filePath);
            if (fileType == ".xml")
            {
                if (File.Exists(filePath))
                {
                    Stream sFileSteam = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                    XmlReader xmlReader = XmlReader.Create(sFileSteam);
                    while (xmlReader.Read())
                    {
                        xmlReader.MoveToContent();
                        result.AddRange(cellParse(xmlReader));
                        xmlReader.Read();
                    }
                    sFileSteam.Flush();
                    sFileSteam.Close();
                }
                else
                {
                    log.ErrorLog("Read configration error: file not exsit!");
                }
            }
            else
            {
                log.ErrorLog("Read configration error: file type error!");
            }
            return result;
        }
        /// <summary>
        /// 实现根节点，父子节点判断逻辑的函数委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public delegate List<T> CellParse1<T>(XmlReader reader);
    }
}
