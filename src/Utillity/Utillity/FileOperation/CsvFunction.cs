using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Utillity.FileOperation
{
    public class CsvFunction
    {
        /// <summary>
        /// 读取Csv文件，根据委托函数生成具体的类型表
        /// </summary>
        /// <typeparam name="T">可实例化泛型类型</typeparam>
        /// <param name="sfilePath">文件地址</param>
        /// <param name="cellParse">委托函数</param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static List<T> CsvRead<T>(string sfilePath,CellParse<T> cellParse, ILog log)where T :new()
        {
            List<T> result = new List<T>();
            string sfilename = @sfilePath;
            string fileType = Path.GetExtension(sfilePath);
            if (fileType == ".csv")
            {
                if (File.Exists(sfilename))
                {
                    FileStream fs = new FileStream(sfilePath, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(sfilePath, Encoding.UTF8);
                    while (!sr.EndOfStream)
                    {
                        var temp = sr.ReadLine();
                        if (temp != null)
                        {
                            var arraryStr = temp.Split(',');
                            T cell = new T();
                            cellParse(cell, arraryStr);
                            result.Add(cell);
                        }
                    }
                    sr.Close();
                    sr.Dispose();
                }
                else
                {
                    log.ErrorLog("Read Csv Error:file not exist!");
                }
            }
            else
            {
                log.ErrorLog("Read Csv Error:file style not match!");
            }

          
            return result;
        }

        /// <summary>
        /// 单元处理委托函数
        /// </summary>
        /// <typeparam name="T">可实例化的泛型类型</typeparam>
        /// <param name="obj">具体的实例化</param>
        /// <param name="soures">数组类型数据源</param>
        public delegate void CellParse<T>(T obj, string[] soures) where T : new();
        /// <summary>
        /// 将datatable表格式写入CSV文件
        /// </summary>
        /// <param name="sfilePath">文件地址</param>
        /// <param name="dt">数据表</param>
        /// <param name="log">log记录，继承ILog接口</param>
        public static void CsvWirte(string sfilePath,DataTable dt,ILog log)
        {
            string sfilename = @sfilePath;
            if (File.Exists(sfilename))
            {
                File.Delete(sfilename);
            }
           // File.Create(sfilename);
            FileStream fs = new FileStream(sfilename,FileMode.Create,FileAccess.Write);
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
                        sWirteLine = sWirteLine == null ? dt.Rows[i][j].ToString():string.Concat(sWirteLine, ",", dt.Rows[i][j].ToString());
                    }
                    sw.WriteLine(sWirteLine);
                }
                sw.Close();
                sw.Dispose();
            }
            catch(Exception ex)
            {
                log.ErrorLog($"Write to CSV Error：{ex.Message} ");
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
        public static void CsvWirte<T>(string sfilePath, List<T> sources, ILog log) where T : IEnumerable<string>
        {
            string sfilename = @sfilePath;
            if (File.Exists(sfilename))
            {
                File.Delete(sfilename);
            }
            // File.Create(sfilename);
            FileStream fs = new FileStream(sfilename, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            try
            {
                foreach(T data in sources)
                {
                    string sWirteLine = null;
                    foreach (string s in data)
                    {
                        sWirteLine = sWirteLine == null ? s : string.Concat(sWirteLine,",",s);
                    }
                    sw.WriteLine(sWirteLine);
                }
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                log.ErrorLog($"Write to CSV Error:{ex.Message} ");
                sw.Close();
                sw.Dispose();
            }
        }
    }
}
