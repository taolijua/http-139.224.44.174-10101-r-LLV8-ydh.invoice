using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Data;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace YDH.Finace.Common
{
    /// <summary>
    /// 高性能Excel导入导出
    /// </summary>
    public class ExcelExtensions : IDisposable
    {

        string sheetName = "sheet1";
        SpreadsheetDocument xl;//表格文档类
        OpenXmlWriter oxw;
        WorksheetPart wsp;//对应sheet*.xml文档建立的一个类，处理该文档与外部的关系。
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="path">Excel文件名称(全路径)</param>
        /// <param name="rowCount">表格列数量</param>
        /// <param name="sheetName">表格名称</param>

        public ExcelExtensions(Stream stream, string sheetName = "sheet1")
        {
            this.sheetName = sheetName ?? "sheet1";
            xl = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);//先创建一个.xlsx类型
            xl.AddWorkbookPart();//创建了 WorkbookPart 并将其添加到此文档。
            wsp = xl.WorkbookPart.AddNewPart<WorksheetPart>();
            oxw = OpenXmlWriter.Create(wsp);
            oxw.WriteStartElement(new Worksheet());
            oxw.WriteStartElement(new SheetData());
        }

        #region 导出数据 到 Excel 

        /// <summary>
        /// 写入表格数据
        /// 使用方法 using (var oxExt = new ExcelExport("地址"))
        ///           {
        ///              oxExt.Write(数据源);
        ///            }
        /// </summary>
        /// <param name="datas"></param>
        public void Write(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            oxw.WriteStartElement(new Row());
            // 表头
            foreach (DataColumn item in dt.Columns)
            {
                var oxa = new List<OpenXmlAttribute>();

                oxa.Add(new OpenXmlAttribute("t", null, "str"));

                oxw.WriteStartElement(new Cell() { DataType = CellValues.InlineString }, oxa);

                oxw.WriteElement(new CellValue($"{item.ColumnName}"));

                oxw.WriteEndElement();
            }
            oxw.WriteEndElement();
            // 内容
            foreach (DataRow dgvr in dt.Rows)
            {
                oxw.WriteStartElement(new Row());
                foreach (DataColumn item in dt.Columns)
                {
                    var oxa = new List<OpenXmlAttribute>();

                    oxa.Add(new OpenXmlAttribute("t", null, "str"));

                    oxw.WriteStartElement(new Cell() { DataType = CellValues.InlineString }, oxa);

                    oxw.WriteElement(new CellValue($"{dgvr[item.ColumnName]}"));

                    oxw.WriteEndElement();
                }
                oxw.WriteEndElement();
            }
        }

        /// <summary>
        /// 写入表格数据
        /// 使用方法
        ///    using (var oxExt = new ExcelExport("地址"))
        ///          {
        ///              Dictionary<string, string> keyValues = new Dictionary<string, string>();
        ///              keyValues.Add("数据源字段", "Excel的列名");
        ///              oxExt.Write(数据源, keyValues);
        ///          }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">数据</param>
        /// <param name="keyValues">表头对应数据的列名</param>
        public void Write<T>(List<T> items, Dictionary<string, string> keyValues)
        {
            if (items == null || items.Count == 0) return;

            oxw.WriteStartElement(new Row());

            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            // 表头
            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);

                if (!keyValues.ContainsKey(prop.Name)) continue;

                string name = keyValues[prop.Name];

                var oxa = new List<OpenXmlAttribute>();

                oxa.Add(new OpenXmlAttribute("t", null, "str"));

                oxw.WriteStartElement(new Cell() { DataType = CellValues.InlineString }, oxa);

                oxw.WriteElement(new CellValue($"{name}"));

                oxw.WriteEndElement();
            }
            oxw.WriteEndElement();
            // 内容
            foreach (T item in items)
            {
                oxw.WriteStartElement(new Row());

                foreach (PropertyInfo prop in props)
                {
                    if (!keyValues.ContainsKey(prop.Name)) continue;
                    var oxa = new List<OpenXmlAttribute>();

                    oxa.Add(new OpenXmlAttribute("t", null, "str"));

                    oxw.WriteStartElement(new Cell() { DataType = CellValues.InlineString }, oxa);

                    oxw.WriteElement(new CellValue($"{ prop.GetValue(item)}"));

                    oxw.WriteEndElement();
                }

                oxw.WriteEndElement();
            }
        }


        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))

            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        void Close()
        {
            // this is for SheetData
            oxw.WriteEndElement();
            // this is for Worksheet
            oxw.WriteEndElement();
            oxw.Close();
            oxw = OpenXmlWriter.Create(xl.WorkbookPart);
            oxw.WriteStartElement(new Workbook());
            oxw.WriteStartElement(new Sheets());
            // you can use object initialisers like this only when the properties
            // are actual properties. SDK classes sometimes have property-like properties
            // but are actually classes. For example, the Cell class has the CellValue
            // "property" but is actually a child class internally.
            // If the properties correspond to actual XML attributes, then you're fine.
            oxw.WriteElement(new Sheet()
            {
                Name = sheetName,
                SheetId = 1,
                Id = xl.WorkbookPart.GetIdOfPart(wsp)
            });
            // this is for Sheets
            oxw.WriteEndElement();
            // this is for Workbook
            oxw.WriteEndElement();
            oxw.Close();
            xl.Close();
        }
        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    Close();
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~OpenXmlExt() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }
        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion


        #endregion

        #region ExcelCSV

        /// <summary>
        /// 将CSV文件中的数据读出到DataTable中(csv)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static DataTable ExcelToTableForCSV(string file, int iIndex)
        {
            DataTable dt = new DataTable();
            bool isFirst = true;
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
                {
                    string strLine = string.Empty;
                    int columnCount = 0;
                    while (!string.IsNullOrWhiteSpace((strLine = streamReader.ReadLine())))
                    {
                        if (isFirst == true)
                        {
                            string[] tableHead = strLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            isFirst = false;
                            columnCount = tableHead.Length;
                            //创建列
                            for (int i = 0; i < columnCount; i++)
                            {
                                string strColumnName = tableHead[i].Trim().Replace("\t", string.Empty);
                                if (dt.Columns.Contains(strColumnName))
                                {
                                    throw new Exception(string.Format("表格中列名【{0}】重复", strColumnName));
                                }
                                DataColumn dc = new DataColumn(strColumnName, typeof(string));
                                dt.Columns.Add(dc);
                            }
                        }
                        else
                        {
                            string[] aryLine = strLine.Split(',');
                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < columnCount; j++)
                            {
                                dr[j] = aryLine[j].Trim().Replace("\t", string.Empty);
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 将DataTable数据导出到CSV文件中(csv)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file"></param>
        private static void TableToExcelForCSV(DataTable dt, string file)
        {
            if (dt == null) return;

            StreamWriter writer;
            bool comma = false;
            int columns = dt.Columns.Count;

            using (writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (!comma) comma = true;
                    else writer.Write(',');

                    writer.Write(QuoteValue(col.ColumnName));
                }
                writer.WriteLine();

                foreach (DataRow row in dt.Rows)
                {
                    comma = false;
                    for (int c = 0; c < columns; c++)
                    {
                        if (!comma) comma = true;
                        else writer.Write(',');

                        writer.Write(QuoteValue(row[c].ToString()));
                    }
                    writer.WriteLine();
                }
            }
        }

        private static string QuoteValue(string value)
        {
            return String.Concat(value.Replace("\t", string.Empty).Replace(",", string.Empty), string.Empty);
        }

        #endregion




        #region  Excel 导出 DataTable

        /// <summary>
        /// Excel 导出 DataTable
        /// 使用方式  DataTable dtImportData = ExcelExtensions.GetDataTableFromExcel("文件地址");
        /// </summary>
        /// <param name="excelFile">文件地址</param>
        /// <returns></returns>
        public static DataTable GetDataTableFromExcel(Stream stream)
        {
            var result = new DataTable();
            using (var excel = SpreadsheetDocument.Open(stream, false))
            {
                var bookPart = excel.WorkbookPart;
                var sharedPart = bookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                var sharedArray = sharedPart?.SharedStringTable.Elements<DocumentFormat.OpenXml.Spreadsheet.SharedStringItem>().Select(i =>
                                  i.Text != null ? i.Text.Text : i.InnerText ?? (i.InnerXml ?? null)).ToArray() ?? new string[0];
                var styleArray = bookPart.WorkbookStylesPart.Stylesheet.CellFormats.OfType<DocumentFormat.OpenXml.Spreadsheet.CellFormat>().ToArray();


                var sheetArray = bookPart.WorksheetParts.ToArray();

                WorksheetPart wsPart = sheetArray.Where(i => i.Uri.ToString().Contains("sheet1")).ToList()[0];

                // WorksheetPart wsPart = (WorksheetPart)(bookPart.GetPartById(iSheetsId));


                using (var sheetPartReader = OpenXmlReader.Create(wsPart, true))
                {
                    while (sheetPartReader.Read())
                    {
                        if (sheetPartReader.ElementType == typeof(DocumentFormat.OpenXml.Spreadsheet.Worksheet))
                            sheetPartReader.ReadFirstChild();
                        if (sheetPartReader.ElementType == typeof(DocumentFormat.OpenXml.Spreadsheet.Row))
                        {
                            if (!(sheetPartReader.LoadCurrentElement() is DocumentFormat.OpenXml.Spreadsheet.Row row)) continue;
                            if (result.Columns.Count == 0)
                            {
                                int columnIndex = 1;
                                foreach (var header in row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                                {
                                    int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(header.CellReference));
                                    if (columnIndex < cellColumnIndex)
                                    {
                                        int index = columnIndex;
                                        for (int i = 0; i < cellColumnIndex - index; i++)
                                        {
                                            result.Columns.Add(new DataColumn("", typeof(string)));
                                            columnIndex++;
                                        }
                                    }
                                    else
                                    {
                                        var headValue = GetCellValue(sharedArray, styleArray, header);
                                        //if (string.IsNullOrEmpty(headValue)) break;
                                        result.Columns.Add(new DataColumn(headValue, typeof(string)));
                                        columnIndex++;
                                    }
                                }
                                continue;
                            }
                            int dataIndex = 0;
                            int columnIndex2 = 1;
                            var dataRow = result.NewRow();
                            foreach (var cell in row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                            {
                                int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
                                if (columnIndex2 < cellColumnIndex)
                                {
                                    int index = columnIndex2;
                                    for (int i = 0; i < cellColumnIndex - index; i++)
                                    {
                                        dataRow[dataIndex++] = string.Empty;
                                        columnIndex2++;
                                        if (dataIndex >= result.Columns.Count) break;
                                    }
                                }
                                var cellValue = GetCellValue(sharedArray, styleArray, cell);
                                dataRow[dataIndex++] = cellValue;
                                columnIndex2++;
                                if (dataIndex >= result.Columns.Count) break;
                            }
                            result.Rows.Add(dataRow);
                        }
                    }
                }
            }
            return result;
        }

        public static int GetColumnIndexFromName(string columnNameOrCellReference)
        {
            int columnIndex = 0;
            int factor = 1;
            for (int pos = columnNameOrCellReference.Length - 1; pos >= 0; pos--)   // R to L
            {
                if (Char.IsLetter(columnNameOrCellReference[pos]))  // for letters (columnName)
                {
                    columnIndex += factor * ((columnNameOrCellReference[pos] - 'A') + 1);
                    factor *= 26;
                }
            }
            return columnIndex;
        }

        public static string GetColumnName(string cellReference)
        {
            /* Advance from L to R until a number, then return 0 through previous position*/
            for (int lastCharPos = 0; lastCharPos <= 3; lastCharPos++)
                if (Char.IsNumber(cellReference[lastCharPos]))
                    return cellReference.Substring(0, lastCharPos);

            throw new ArgumentOutOfRangeException("cellReference");
        }

        private static string GetCellValue(string[] sharedArray, DocumentFormat.OpenXml.Spreadsheet.CellFormat[] styleArray, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            var cellValue = string.Empty;
            if (cell.CellReference != null && cell.CellReference.HasValue)
            {
                cellValue = cell.InnerText;
                if (cell.DataType != null && cell.DataType.HasValue)
                {
                    switch (cell.DataType.Value)
                    {
                        case DocumentFormat.OpenXml.Spreadsheet.CellValues.SharedString:
                            if (int.TryParse(cellValue, out var valueId))
                                cellValue = sharedArray[valueId];
                            break;
                        case DocumentFormat.OpenXml.Spreadsheet.CellValues.Boolean:
                            cellValue = bool.TryParse(cellValue, out var boolValue) ?
                                        boolValue.ToString() : "False";
                            break;
                        case DocumentFormat.OpenXml.Spreadsheet.CellValues.Date:
                            if (double.TryParse(cellValue, out var dateValue))
                                cellValue = DateTime.FromOADate(dateValue).ToString();
                            break;
                    }
                }
                else if (cell.StyleIndex != null &&
                        cell.StyleIndex.HasValue &&
                        styleArray.Length > cell.StyleIndex)
                {
                    var style = styleArray[cell.StyleIndex];
                    if (style.NumberFormatId.HasValue)
                    {
                        var formatId = style.NumberFormatId.Value;
                        if (formatId >= 14 && formatId <= 22)
                        {
                            if (double.TryParse(cellValue, out var dateValue))
                                cellValue = DateTime.FromOADate(dateValue).ToString();
                        }
                        else
                        {
                            if (Regex.IsMatch(cellValue, @"\d+\.\d{16,}$"))
                                cellValue = double.Parse(cellValue).ToString();
                        }
                    }
                }

            }

            return cellValue;
        }

        /// <summary>
        /// LIST转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        #endregion
    }
}
