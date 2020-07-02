﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace ComparadorListasDeMateriais
{
    internal class ExcelCloseEventArgs : EventArgs
    {
        public ExcelCloseEventArgs(bool saveWorkbooks = true)
        {
            this.SaveWorkbooks = saveWorkbooks;
        }

        public bool SaveWorkbooks { get; private set; }
    }

    internal static class ExcelInterop
    {
        private static Microsoft.Office.Interop.Excel.Application _app;
        public static Microsoft.Office.Interop.Excel.Application App
        {
            get
            {
                if (_app == null || !ExcelProcessRunning)
                {
                    _app = RegisterAndGetApp();
                }
                if (_showOnStartup)
                    _app.Visible = true;

                return _app;
            }
        }

        private static bool _showOnStartup = true;
        public static bool ShowOnStartup
        {
            get { return _showOnStartup; }
            set { _showOnStartup = value; }
        }

        public static Application RegisterAndGetApp()
        {
            Application excel = null;

            // get excel, throw exception if it is not
            var officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
                throw new Exception("Excel nao instalado");

            try
            {
                excel = (Microsoft.Office.Interop.Excel.Application)Marshal.GetActiveObject("Excel.Application");
            }
            catch (COMException e)
            {
                // 0x800401E3 - the excel process simply was not running, we continue if we
                // encounter this exception

                if (!e.ToString().Contains("0x800401E3"))
                {
                    throw new Exception("Erro321");
                }
            }
            catch (Exception)
            {
                // An exception "The URI prefix is not recognized" will be
                // thrown out for the first run, no idea why that happen, so
                // just swallow this exception and try to create an new excel
                // instance.

                // Sometimes a FileNotFoundException occurs with DynamoCore.XmlSerializers
                // which is not clear why. This exception also makes Excel tests very flaky
                // It is found that just swallowing these exceptions and creating a new excel instance 
                // below causes the test to continue and successfuly pass
            }

            if (excel == null)
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
            }

            excel.Visible = ShowOnStartup;
            excel.DisplayAlerts = false;

            return excel;
        }

        /// <summary>
        /// Check if the excel process is running
        /// </summary>
        public static bool ExcelProcessRunning
        {
            get
            {
                return Process.GetProcessesByName("EXCEL").Length != 0;
            }
        }

        /// <summary>
        /// Check if this object holds a reference to Excel
        /// </summary>
        public static bool HasValidExcelReference
        {
            get { return _app != null; }
        }

        /// <summary>
        /// Close all Excel workbooks and provide SaveAs dialog if needed.  Also, perform
        /// garbage collection and remove references to Excel App
        /// </summary>
        private static void TryQuitAndCleanup(bool saveWorkbooks)
        {
            if (HasValidExcelReference)
            {
                if (ExcelProcessRunning)
                {
                    App.Workbooks.Cast<Workbook>().ToList().ForEach((wb) => wb.Close(saveWorkbooks));
                    App.Quit();
                }

                while (Marshal.ReleaseComObject(_app) > 0)
                {

                }

                _app = null;
            }
        }

        internal static void OnProcessExit(object sender, EventArgs eventArgs)
        {
            if (eventArgs != null)
            {
                var args = eventArgs as ExcelCloseEventArgs;
                if (args != null)
                {
                    TryQuitAndCleanup(args.SaveWorkbooks);
                }
            }
            else
                TryQuitAndCleanup(true);
        }
    }

    public class WorkSheet
    {
        #region Helper methods

        private static object[][] ConvertToJaggedArray(object[,] input, bool convertToString = false)
        {
            int rows = input.GetUpperBound(0);
            int cols = input.GetUpperBound(1);

            object[][] output = new object[rows][];

            for (int i = 0; i < rows; i++)
            {
                output[i] = new object[cols];

                for (int j = 0; j < cols; j++)
                {
                    if (convertToString)
                    {
                        if (input[i + 1, j + 1] == null)
                            output[i][j] = null;
                        else
                            output[i][j] = input[i + 1, j + 1].ToString();
                    }
                    else
                        output[i][j] = input[i + 1, j + 1];
                }
            }

            return output;
        }

        private static object[,] ConvertToDimensionalArray(object[][] input, out int rows, out int cols)
        {
            if (input == null)
            {
                rows = cols = 1;
                return new object[,] { { "" } };
            }

            rows = input.GetUpperBound(0) + 1;
            cols = 0;
            for (int i = 0; i < rows; i++)
            {
                if (input[i] != null)
                    cols = Math.Max(cols, input[i].GetUpperBound(0) + 1);
            }

            // if the input data is an empty list or a list of nested empty lists
            // return an empty cell
            if (rows == 0 || cols == 0)
            {
                rows = cols = 1;
                return new object[,] { { "" } };
            }

            object[,] output = new object[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (input[i] == null || j > input[i].GetUpperBound(0))
                        output[i, j] = "";
                    else
                    {
                        var item = input[i][j];

                        if (item is double)
                        {
                            output[i, j] = ((double)item).ToString(CultureInfo.InvariantCulture);
                        }
                        else if (item is float)
                        {
                            output[i, j] = ((float)item).ToString(CultureInfo.InvariantCulture);
                        }
                        else if (item is DateTime)
                        {
                            output[i, j] = ((DateTime)item).ToString(CultureInfo.InvariantCulture);
                        }
                        else if (item == null)
                        {
                            output[i, j] = "";
                        }

                        else
                        {
                            output[i, j] = item.ToString();
                        }
                    }

                }
            }

            return output;
        }

        #endregion
        /// <summary>
        /// Returns data from given worksheet (GetDataFromExcelWorksheet node)
        /// </summary>
        internal object[][] Data
        {
            get
            {
                return GetData();
            }
        }

        internal object[][] GetData(bool convertToString = false)
        {
            var vals = ws.UsedRange.get_Value();

            // if worksheet is empty
            if (vals == null)
                return new object[0][];

            // if worksheet contains a single value
            if (!vals.GetType().IsArray)
                return new object[][] { new object[] { vals } };

            return ConvertToJaggedArray((object[,])vals, convertToString);
        }

        private WorkBook wb = null;
        private Worksheet ws = null;

        /// <summary>
        /// create new worksheet from given workbook and name (AddExcelWorksheetToWorkbook node)
        /// </summary>
        /// <param name="wbook"></param>
        /// <param name="sheetName"></param>
        /// <param name="overWrite"></param>
        internal WorkSheet(WorkBook wbook, string sheetName, bool overWrite = false)
        {
            wb = wbook;

            // Look for an existing worksheet
            WorkSheet[] worksheets = wbook.WorkSheets;
            WorkSheet wSheet = worksheets.FirstOrDefault(n => n.ws.Name == sheetName);

            if (wSheet == null)
            {
                // If you don't find one, create one.
                ws = (Worksheet)wb.Add();
                ws.Name = sheetName;
                wb.Save();
                return;
            }

            // If you find one, then use it.
            if (overWrite)
            {
                // if there is only one worksheet, we need to add one more
                // before we can delete the first one
                ws = (Worksheet)wb.Add();
                wSheet.ws.Delete();
                ws.Name = sheetName;
                wb.Save();

            }
            else
                ws = wSheet.ws;
        }

        internal WorkSheet(Worksheet ws, WorkBook wb)
        {
            this.ws = ws;
            this.wb = wb;
        }

        /// <summary>
        /// instance method, write data to existing worksheet, (WriteDataToExcelWorksheet node)
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal WorkSheet WriteData(int startRow, int startColumn, object[][] data)
        {
            startRow = Math.Max(0, startRow);
            startColumn = Math.Max(0, startColumn);
            int numRows, numColumns;

            object[,] rangeData = ConvertToDimensionalArray(data, out numRows, out numColumns);

            if (rangeData == null)
                return this;

            var c1 = (Range)ws.Cells[startRow + 1, startColumn + 1];
            var c2 = (Range)ws.Cells[startRow + numRows, startColumn + numColumns];
            var range = ws.Range[c1, c2];
            range.Value = rangeData;

            wb.Save();
            return this;
        }

    }

    public class WorkBook
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// (GetWorksheetsFromExcelWorkbook node)
        /// </summary>
        internal WorkSheet[] WorkSheets
        {
            get
            {
                return wb.Worksheets.Cast<Worksheet>().Select(n => new WorkSheet(n, this)).ToArray();
            }
        }

        private Workbook wb = null;

        internal object Add()
        {
            return wb.Worksheets.Add();
        }

        internal void Save()
        {
            if (!String.IsNullOrEmpty(wb.Path))
                wb.Save();
        }

        /// <summary>
        /// Creates a new Workbook with filepath and sheet name as input
        /// </summary>
        internal WorkBook(string filePath)
        {
            Name = filePath;

            if (!String.IsNullOrEmpty(filePath))
            {
                try
                {
                    // Look for an existing open workbook
                    var workbookOpen = ExcelInterop.App.Workbooks.Cast<Workbook>()
                        .FirstOrDefault(e => e.FullName == filePath);

                    // Use the existing workbook.
                    if (workbookOpen != null)
                    {
                        wb = workbookOpen;
                    }
                    // If you can't find an existing workbook at
                    // the specified path, then create a new one.
                    else
                    {
                        Workbook workbook = ExcelInterop.App.Workbooks.Open(filePath);
                        wb = workbook;
                        wb.Save();
                    }
                }
                catch (Exception)
                {
                    // Exception is thrown when there is no existing workbook with the given filepath
                    wb = ExcelInterop.App.Workbooks.Add();
                    wb.SaveAs(filePath);
                }
            }
            else
                wb = ExcelInterop.App.Workbooks.Add();
        }

        /// <summary>
        /// (SaveAsExcelWorkbook node)
        /// </summary>
        /// <param name="wbook"></param>
        /// <param name="filename"></param>
        internal WorkBook(WorkBook wbook, string filename)
        {
            Name = filename;
            wb = wbook.wb;

            if (wb.FullName == filename)
                wb.Save();
            else
            {
                try
                {
                    Workbook workbook = ExcelInterop.App.Workbooks.Open(filename);
                    workbook.Close(false);
                }
                catch (Exception)
                {
                }

                wb.SaveAs(filename);
            }

        }

        private WorkBook(Workbook wb, string filePath)
        {
            this.wb = wb;
            Name = filePath;
        }

        /// <summary>
        /// (ReadExcelFile node)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static WorkBook ReadExcelFile(string path)
        {
            var workbookOpen = ExcelInterop.App.Workbooks.Cast<Workbook>()
                    .FirstOrDefault(e => e.FullName == path);

            if (workbookOpen != null)
                return new WorkBook(workbookOpen, path);

            if (File.Exists(path))
                return new WorkBook(ExcelInterop.App.Workbooks.Open(path, true, false), path);

            throw new FileNotFoundException("File path not found.", path);
        }

        /// <summary>
        /// instance method, (GetExcelWorksheetByName node)
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        internal WorkSheet GetWorksheetByName(string sheetName)
        {
            var ws = wb.Worksheets.Cast<Worksheet>().FirstOrDefault(sheet => sheet.Name == sheetName);

            if (ws == null)
                throw new ArgumentException("No worksheet matches the given string.", "sheetName");

            return new WorkSheet(ws, this);
        }

    }

    /// <summary>
    ///     Methods for Import/Export category.
    /// </summary>
    public static class Data
    {
        /// <summary>
        ///     Write a list of lists into a file using a comma-separated values 
        ///     format. Outer list represents rows, inner lists represent columns. 
        /// </summary>
        /// <param name="filePath">Path to write to</param>
        /// <param name="data">List of lists to write into CSV</param>
        /// <search>write,text,file</search>
        public static void ExportCSV(string filePath, object[][] data)
        {

            using (var writer = new StreamWriter(filePath))

            {
                foreach (var line in data)
                {
                    int count = 0;
                    foreach (var entry in line)
                    {
                        writer.Write(entry);
                        if (++count < line.Length)
                            writer.Write(",");
                    }
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        ///     Imports data from a CSV (comma separated values) file, put the items into a list and 
        ///     transpose it if needed.
        /// </summary>
        /// <param name="filePath">The CSV file to be converted into a list.</param>
        /// <param name="transpose">Whether the resulting list should be transposed.</param>
        /// <returns name="list">The list containing the items in the CSV file.</returns>
        /// <search>import,csv,comma,file,list,separate,transpose</search>
        public static IList ImportCSV(string filePath, bool transpose = false)
        {

            if (string.IsNullOrEmpty(filePath))
            {
                // File not existing.
                throw new FileNotFoundException();
            }
            // Open the file to read from.
            List<object[]> CSVdatalist = new List<object[]>();
            int colNum = 0;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using (var sr = new StreamReader(fileStream))
            {
                while (!sr.EndOfStream)
                {
                    string[] lineStr = sr.ReadLine().Split(',');
                    int count = 0;

                    // Convert each line into an array of objects (int, double, null or string)
                    // and append them into CSVdatalist. 
                    object[] line = new object[lineStr.Length];
                    foreach (string elementStr in lineStr)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(elementStr) || string.IsNullOrWhiteSpace(elementStr))
                                line[count] = null;
                            else if (elementStr.Contains("."))
                                line[count] = Double.Parse(elementStr);
                            else line[count] = Int32.Parse(elementStr);
                        }
                        catch (Exception)
                        {
                            line[count] = elementStr;
                        }
                        count++;
                    }
                    colNum = System.Math.Max(colNum, line.Length);
                    CSVdatalist.Add(line);
                }
            }

            // The length of all arrays in CSVdatalist must be the same. If the length of the array
            // is less than colNum, null is appended to the array to achieve the required length.  
            for (int row = 0; row < CSVdatalist.Count(); row++)
            {
                int count = CSVdatalist[row].Count();
                if (count < colNum)
                {
                    object[] newRow = new object[colNum];
                    Array.Copy(CSVdatalist[row], newRow, count);
                    for (int j = count; j < colNum; j++)
                    {
                        newRow[j] = null;
                    }
                    CSVdatalist[row] = newRow;
                }
            }

            // Judge whether the array needed to be transposed (when transpose is false) or not (when transpose is true)
            return CSVdatalist;
        }


    }
}
