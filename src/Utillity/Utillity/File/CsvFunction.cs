using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
//using Utillity.Interface;
using System.Reflection;

namespace Utillity.File
{
    public class CsvFunction
    {
        /// <summary>
        /// 读取Csv文件，根据委托函数生成具体的类型表
        /// return
        /// </summary>
        /// <typeparam name="T">可实例化泛型类型</typeparam>
        /// <param name="sfilePath">文件地址</param>
        /// <param name="cellParse">委托函数</param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static List<T> CsvRead<T>(string sfilePath, StringArraryToCell<T> cellParse,bool haveColumn=false) where T : new()
        {
            List<T> result = new List<T>();
            string sfilename = @sfilePath;
            string fileType = Path.GetExtension(sfilePath);
            if (fileType == ".csv")
            {
                if (System.IO.File.Exists(sfilename))
                {
                    //FileStream fs = new FileStream(sfilePath, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(sfilePath, Encoding.UTF8);

                    int lineNum = 0;
                    while (!sr.EndOfStream)
                    {
                        lineNum++;
                        var temp = sr.ReadLine();
                        if (lineNum == 1 && haveColumn)
                        {
                            continue;
                        }
                        if (temp != null)
                        {
                            var arraryStr = temp.Split(',');
                            T cell= cellParse(arraryStr);
                            result.Add(cell);
                        }
                    }
                    sr.Close();
                    sr.Dispose();
                }
                else
                {
                    throw new Exception("Read Csv Error: file not exist!");
                }
            }
            else
            {
                throw new Exception("Read Csv Error:file style not match!");
            }
            return result;
        }

        /// <summary>
        /// 数组转换为类托管函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="soures"></param>
        /// <returns></returns>
        public delegate T StringArraryToCell<T>(string[] soures) where T : new();
        public delegate string[] CellToStringArrary<T>(T obj) where T : new();

        /// <summary>
        /// 将datatable表格式写入CSV文件
        /// </summary>
        /// <param name="sfilePath">文件地址</param>
        /// <param name="dt">数据表</param>
        /// <param name="log">log记录，继承ILog接口</param>
        public static void CsvWirte(string sfilePath, DataTable dt)
        {
            string sfilename = @sfilePath;
            if (System.IO.File.Exists(sfilename))
            {
                System.IO.File.Delete(sfilename);
            }
            // File.Create(sfilename);
            FileStream fs = new FileStream(sfilename, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            int iRowCounts = dt.Rows.Count;
            int iColCounts = dt.Columns.Count;
            //string sWirteFirseLine = "";
            string sWirteLine = null;

            try
            {
                ///写第一行（列数据）
                for (int i = 0; i < iColCounts; i++)
                {
                    sWirteLine = sWirteLine == null ? dt.Columns[i].ToString() : string.Concat(sWirteLine, ",", dt.Columns[i].ToString());
                }
                sw.WriteLine(sWirteLine);

                //写具体数据
                for (int i = 0; i < iRowCounts; i++)
                {
                    sWirteLine = null;
                    for (int j = 0; j < iColCounts; j++)
                    {
                        sWirteLine = sWirteLine == null ? dt.Rows[i][j].ToString() : string.Concat(sWirteLine, ",", dt.Rows[i][j].ToString());
                    }
                    sw.WriteLine(sWirteLine);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Write to CSV Error：{ex.Message} ");
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
        }
        /// <summary>
        /// 写Csv文件操作：
        /// 若文件存在，会先进行删除操作
        /// 编码格式为UTF8格式
        /// </summary>
        /// <typeparam name="T">可遍历数据类型，继承IEnumerable<string>接口 </typeparam>
        /// <param name="sfilePath">文件目录</param>
        /// <param name="sources">数据源</param>
        /// <param name="log">log记录，继承ILog接口</param>
        public static void CsvWirte<T>(string sfilePath, List<T> sources, string[] columnHeader = null) where T : IEnumerable<string>
        {
            string sfilename = @sfilePath;
            if (System.IO.File.Exists(sfilename))
            {
                System.IO.File.Delete(sfilename);
            }
            // File.Create(sfilename);
            FileStream fs = new FileStream(sfilename, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            try
            {
                string sWirteLine = null;
                //写列头信息
                if (columnHeader != null)
                {
                    foreach (var s in columnHeader)
                    {
                        sWirteLine = sWirteLine == null ? s : string.Concat(sWirteLine, ",", s);
                    }
                }
                sw.WriteLine(sWirteLine);

                foreach (T data in sources)
                {
                    sWirteLine = null;
                    foreach (string s in data)
                    {
                        sWirteLine = sWirteLine == null ? s : string.Concat(sWirteLine, ",", s);
                    }
                    sw.WriteLine(sWirteLine);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Write to CSV Error：{ex.Message} ");
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

        }
        public static void CsvWirte<T>(string sfilePath, List<T> sources, CellToStringArrary<T> toStringArrary, string[] columnHeader = null) where T : new()
        {
            string sfilename = @sfilePath;
            if (System.IO.File.Exists(sfilename))
            {
                System.IO.File.Delete(sfilename);
            }
            // File.Create(sfilename);
            FileStream fs = new FileStream(sfilename, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            try
            {
                string sWirteLine = null;
                //写列头信息
                if (columnHeader != null)
                {
                    foreach (var s in columnHeader)
                    {
                        sWirteLine = sWirteLine == null ? s : string.Concat(sWirteLine, ",", s);
                    }
                }
                sw.WriteLine(sWirteLine);

                foreach (T data in sources)
                {
                    string[] strings = toStringArrary(data);
                    sWirteLine = null;
                    foreach (string s in strings)
                    {
                        sWirteLine = sWirteLine == null ? s : string.Concat(sWirteLine, ",", s);
                    }
                    sw.WriteLine(sWirteLine);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Write to CSV Error：{ex.Message} ");
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

        }

    }
}
