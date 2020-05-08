using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Serialization
{
    [XmlRoot("Workbook")]
    public class Workbook : IXmlSerializable, IOperation<Workbook>
    {
        public Workbook() { }
        public Workbook(List<List<string>> llstrings) { _llStrings = llstrings; }
        private List<List<string>> _llStrings = new List<List<string>>();

        public List<List<string>> llStrings { get { return _llStrings; } set { _llStrings = value; } }
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.IsStartElement("Row"))
                {
                    List<string> ls = new List<string>();
                    reader.Read();
                    while (reader.IsStartElement("Cell"))
                    {
                        reader.Read();
                        ls.Add(reader.ReadElementContentAsString());
                        reader.Read();
                    }
                    _llStrings.Add(ls);
                }
                reader.MoveToContent();
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            //Namespace.
            writer.WriteAttributeString("xmlns", null, null, "urn:schemas-microsoft-com:office:spreadsheet");
            writer.WriteAttributeString("xmlns", "o", null, "urn:schemas-microsoft-com:office:spreadsheet");
            writer.WriteAttributeString("xmlns", "x", null, "urn:schemas-microsoft-com:office:excel");
            writer.WriteAttributeString("xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet");
            writer.WriteAttributeString("xmlns", "html", null, "http://www.w3.org/TR/REC-html40");

            //Workbook Property Setup
            //"DocumentProperties"
            writer.WriteStartElement(null, "DocumentProperties", "urn:schemas-microsoft-com:office:office");
            writer.WriteElementString("Author", "BGBuild");
            writer.WriteElementString("LastAuthor", "BGBuild");
            writer.WriteElementString("Created", DateTime.Now.ToString());
            writer.WriteElementString("LastSaved", DateTime.Now.ToString());
            writer.WriteElementString("Version", "14.0");
            writer.WriteEndElement();
            //OfficeDocumentSettings 
            writer.WriteStartElement(null, "OfficeDocumentSettings", "urn:schemas-microsoft-com:office:office");
            writer.WriteElementString("AllowPNG", null);
            writer.WriteEndElement();
            //ExcelWorkbook 
            writer.WriteStartElement(null, "ExcelWorkbook", "urn:schemas-microsoft-com:office:excel");
            writer.WriteElementString("WindowHeight", "12660");
            writer.WriteElementString("WindowWidth", "15180");
            writer.WriteElementString("WindowTopX", "480");
            writer.WriteElementString("WindowTopY", "120");
            writer.WriteElementString("ProtectStructure", "False");
            writer.WriteElementString("ProtectWindows", "False");
            writer.WriteEndElement();
            //Styles
            writer.WriteStartElement("Styles");
            writer.WriteStartElement("Style");
            writer.WriteAttributeString("ss", "ID", null, "Default");
            writer.WriteAttributeString("ss", "Name", null, "Normal");
            writer.WriteStartElement("Alignment");
            writer.WriteAttributeString("ss", "Vertical", null, "Bottom");
            writer.WriteEndElement();
            writer.WriteElementString("Borders", null);
            writer.WriteStartElement("Font");
            writer.WriteAttributeString("ss", "FontName", null, "Arial");
            writer.WriteEndElement();
            writer.WriteElementString("Interior", null);
            writer.WriteElementString("NumberFormat", null);
            writer.WriteElementString("Protection", null);
            writer.WriteEndElement();
            writer.WriteStartElement("Style");
            writer.WriteAttributeString("ss", "ID", null, "s62");
            writer.WriteStartElement("Font");
            writer.WriteAttributeString("ss", "FontName", null, "Arial");
            writer.WriteAttributeString("x", "Family", null, "Swiss");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("Style");
            writer.WriteAttributeString("ss", "ID", null, "s63");
            writer.WriteStartElement("Font");
            writer.WriteAttributeString("ss", "FontName", null, "Arial");
            writer.WriteAttributeString("x", "Family", null, "Swiss");
            writer.WriteAttributeString("x", "Color", null, "#33333");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            //Worksheet
            writer.WriteStartElement("Worksheet");
            writer.WriteAttributeString("ss", "Name", null, "Sheet1");
            //Table
            writer.WriteStartElement("Table");
            foreach (List<string> ls in _llStrings)
            {
                //Row
                writer.WriteStartElement("Row");
                foreach (string s in ls)
                {
                    //Cell
                    writer.WriteStartElement("Cell");
                    writer.WriteStartElement("Data");
                    writer.WriteAttributeString("ss", "Type", null, "String");
                    writer.WriteString(s);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            //WorksheetOptions
            writer.WriteStartElement("WorksheetOptions", "urn:schemas-microsoft-com:office:excel");
            writer.WriteElementString("Unsynced", null);
            writer.WriteStartElement("Print");
            writer.WriteElementString("ValidPrinterInfo", null);
            writer.WriteElementString("PaperSizeIndex", "9");
            writer.WriteElementString("HorizontalResolution", "600");
            writer.WriteElementString("VerticalResolution", "300");
            writer.WriteEndElement();
            writer.WriteElementString("Selected", null);
            writer.WriteElementString("TopRowVisible", "0");
            writer.WriteElementString("ProtectObjects", "False");
            writer.WriteElementString("ProtectScenarios", "False");
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public Workbook Add(Workbook T1, Workbook T2)
        {
            T2._llStrings.RemoveAt(0);
            T1._llStrings.AddRange(T2._llStrings);
            return T1;
        }

        public Workbook Subtract(Workbook T1, Workbook T2)
        {
            throw new NotImplementedException();
        }

        public Workbook Multiply(Workbook T1, Workbook T2)
        {
            throw new NotImplementedException();
        }

        public Workbook Div(Workbook T1, Workbook T2)
        {
            throw new NotImplementedException();
        }

        public Workbook Add(Workbook T1)
        {
            T1._llStrings.RemoveAt(0);
            _llStrings.AddRange(T1._llStrings);
            return this;
        }

        public Workbook Subtract(Workbook T1)
        {
            throw new NotImplementedException();
        }

        public Workbook Multiply(Workbook T1)
        {
            throw new NotImplementedException();
        }

        public Workbook Div(Workbook T1)
        {
            throw new NotImplementedException();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        /*public static Workbook operator +(Workbook a, Workbook b)
        {
            Workbook c = new Workbook();
            b.llStrings.RemoveAt(0);
            a.llStrings.AddRange(b.llStrings);
            c.llStrings.AddRange(a.llStrings);
            return c;
        }*/
    }
    public interface IOperation<T>
    {

        T Add(T T1, T T2);
        T Subtract(T T1, T T2);
        T Multiply(T T1, T T2);
        T Div(T T1, T T2);

        T Add(T T1);
        T Subtract(T T1);
        T Multiply(T T1);
        T Div(T T1);
    }
}
