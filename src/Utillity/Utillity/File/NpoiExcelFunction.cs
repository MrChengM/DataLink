using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Data;
using NPOI.SS.UserModel;
using System.IO;

namespace Utillity.File
{
    public static class NpoiExcelFunction
    {
        /// <summary>
        /// 读取Excel文件，返回Dataset类型数据
        /// </summary>
        /// <param name="filePath">文件目录</param>
        /// <param name="log">log记录，继承ILog接口</param>
        /// <returns></returns>
        public static DataSet ExcelRead(string filePath)
        {
            DataSet da = new DataSet();
            DataTable dt = null;
            IWorkbook workbook = null;
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    string fileType = Path.GetExtension(filePath);
                    if (fileType == ".xls")
                        workbook = new HSSFWorkbook(file);
                    else if (fileType == ".xlsx")
                        workbook = new XSSFWorkbook(file);
                    else if (fileType == ".xlsm")
                        workbook = new XSSFWorkbook(file);
                    file.Close();
                }
                if (workbook != null)
                {
                    for (int index = 0; index < workbook.NumberOfSheets; index++)
                    {
                        dt = new DataTable(workbook.GetSheetName(index) + "$");
                        ISheet sheet = workbook.GetSheetAt(index);

                        //获取DataSet列表数据
                        IRow row = sheet.GetRow(0);
                        foreach (ICell cell in row.Cells)
                        {
                            DataColumn col = new DataColumn(cellToString(cell));
                            dt.Columns.Add(col);
                        }

                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            row = sheet.GetRow(i);
                            DataRow dtrow = dt.NewRow();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (row == null)
                                    continue;
                                ICell cell = row.GetCell(j);
                                dtrow[j] = cellToString(cell);
                            }
                            dt.Rows.Add(dtrow);
                        }
                        da.Tables.Add(dt);
                    }
                }
                return da;
            }
            catch (Exception e)
            {
                throw new Exception($"Excel Read error:{e.Message}");
            }
            finally
            {
                da?.Dispose();
                dt?.Dispose();
                workbook?.Close();
            }

        }

        public static List<T> ExcelRead<T>(string filePath, string sheetName, CellParse<T> cellParse) where T : new()
        {
            var result = new List<T>();

            IWorkbook workbook = null;
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    string fileType = Path.GetExtension(filePath);
                    if (fileType == ".xls")
                        workbook = new HSSFWorkbook(file);
                    else if (fileType == ".xlsx")
                        workbook = new XSSFWorkbook(file);
                    else if (fileType == ".xlsm")
                        workbook = new XSSFWorkbook(file);
                    file.Close();
                }
                else
                {
                    throw new Exception($"Excel Read error:File not exsit!");
                }
                if (workbook!=null)
                {
                    ISheet sheet = workbook.GetSheet(sheetName);
                    for(int i = 0; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);

                        List<string> ls = new List<string>();
                        foreach (ICell cell in row.Cells)
                        {
                            var s = cellToString(cell);
                            ls.Add(s);
                        }
                        T t= cellParse(ls);
                        result.Add(t);
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception($"Excel Read error:{e.Message}!");
            }
            workbook?.Close();
            return result;
        }
        /// <summary>
        /// 单元处理委托函数
        /// </summary>
        /// <typeparam name="T">可实例化的泛型类型</typeparam>
        /// <param name="obj">具体的实例化</param>
        /// <param name="soures">表类型数据源</param>
        public delegate T CellParse<T>( List<string> soures) where T : new();
        private static string cellToString(ICell cell)
        {
            string result = "";
            if (cell == null)
                result = "";
            else
                switch (cell.CellType)
                {
                    case CellType.Formula:
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.Numeric:
                                result = cell.NumericCellValue.ToString();
                                break;
                            case CellType.String:
                                result = cell.StringCellValue;
                                break;
                            case CellType.Boolean:
                                result = cell.BooleanCellValue.ToString();
                                break;
                            case CellType.Error:
                                result = "";
                                break;
                            default:
                                result = "";
                                break;
                        }
                        break;
                    case CellType.Numeric:
                        result = cell.NumericCellValue.ToString();
                        break;
                    case CellType.Boolean:
                        result = cell.BooleanCellValue.ToString();
                        break;
                    case CellType.String:
                        result = cell.StringCellValue;
                        break;
                    default:
                        result = "";
                        break;
                }
            return result;
        }
        public static void ExcelWrite<T>(string filename, string sheetName, List<T> sources, IColumns columns) where T : IEnumerable<string>
        {
            IWorkbook workbook = null;
            IRow row = null;
            ICell cell = null;
            ICellStyle columnStyle = null;
            IFont columnFont = null;

            ICellStyle cellStyle = null;
            IFont cellFont = null;

            int rowIndex = 0;
            int columnIndex = 0;
            string fileType = Path.GetExtension(filename);
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    FileStream infile = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    if (fileType == ".xls")
                    {
                        workbook = new HSSFWorkbook(infile);
                    }
                    else if (fileType == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(infile);
                    }
                    infile.Close();

                }
                else
                {
                    if (fileType == ".xls")
                    {
                        workbook = new HSSFWorkbook();
                    }
                    else if (fileType == ".xlsx")
                    {
                        workbook = new XSSFWorkbook();
                    }
                }

                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    if (sheetName == workbook.GetSheetName(i))
                    {
                        workbook.RemoveSheetAt(i);
                        break;
                    }
                }
                ISheet sheet = workbook.CreateSheet(sheetName);

                //写标题，为了精简化，暂时未开放
                //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, 0, column.Count() - 1));
                //row = sheet.CreateRow(rowIndex);
                //cell = row.CreateCell(columnIndex);
                //cell.SetCellValue("FAT TEST LIST");
                //rowIndex++;

                //Column单元格式
                columnFont = workbook.CreateFont();
                columnFont.IsBold = true;
                columnFont.FontHeightInPoints = 12;
                columnFont.FontName = "Times New Roman";
                columnStyle = workbook.CreateCellStyle();
                columnStyle.SetFont(columnFont);

                //Cell单元格式
                cellFont = workbook.CreateFont();
                cellFont.FontName = "Times New Roman";
                cellStyle = workbook.CreateCellStyle();
                cellStyle.SetFont(cellFont);

                //写列数据
                row = sheet.CreateRow(rowIndex);
                foreach (var str in columns)
                {
                    cell = row.CreateCell(columnIndex);
                    cell.SetCellValue(str);
                    cell.CellStyle = columnStyle;
                    columnIndex++;
                }
                rowIndex++;
                //写数据
                foreach (var ls in sources)
                {
                    columnIndex = 0;
                    row = sheet.CreateRow(rowIndex);
                    foreach (string s in ls)
                    {
                        cell = row.CreateCell(columnIndex);
                        if (s != null)
                        {
                            cell.SetCellValue(s);
                            cell.CellStyle = cellStyle;
                        }
                        columnIndex++;
                    }
                    rowIndex++;
                }
                //自动设置column宽度
                for (int i = 0; i < columns.Count(); i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                FileStream outfile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                workbook.Write(outfile);
                workbook.Close();
                outfile.Close();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("OutPut file error:{0}", e.Message));
            }
        }
    }
    public interface IColumns : IEnumerable<string>
    {
        int Count();
    }
}
 