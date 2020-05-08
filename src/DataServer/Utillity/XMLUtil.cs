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

        public static List<T> ReadXMLConfig<T>(string filePath, CellParse<T> cellParse, string startElement) where T:new()
        {
            List<T> result = new List<T>();
            Stream sFileSteam = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            XmlReader xmlReader = XmlReader.Create(sFileSteam);
            while (xmlReader.Read())
            {
                xmlReader.MoveToContent();
                while (xmlReader.IsStartElement(startElement))
                {
                    T element = new T();
                    cellParse(element, xmlReader);
                    result.Add(element);
                    xmlReader.Read();
                }
                xmlReader.Read();
            }
            return result;
        }
        public delegate void CellParse<T>(T obj, XmlReader reader) where T : new();
    }
}


