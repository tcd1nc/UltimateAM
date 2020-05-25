using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using OfficeOpenXml;
using AssetManager.Models;

namespace AssetManager
{
    public class ExcelLib
    {
              
        private void FormatCustomTable(DataTable dt, ref ExcelWorksheet ws, int firstrow)
        {
            try
            {
                int dtcolumncount = dt.Columns.Count;
                int dtrowcount = dt.Rows.Count;                
                for (int i = 0; i < dtcolumncount; i++)
                {                   
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                        ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();

                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                            ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        else
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                            ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        else
                            ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    ws.Cells[firstrow, i + 1].Value = dt.Columns[i].Caption;
                    ws.Cells[firstrow, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[firstrow, i + 1].Style.Numberformat.Format = "";
                    ws.Column(i + 1).AutoFit();                                           
                }
                var rangeheader = ws.Cells[firstrow, 1, firstrow, dtcolumncount].Style;
                rangeheader.Font.Bold = true;
                rangeheader.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rangeheader.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rangeheader.Font.Color.SetColor(Color.Black);
                rangeheader.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                rangeheader.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Border.Right.Color.SetColor(Color.LightGray);
                rangeheader.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Border.Top.Color.SetColor(Color.LightGray);
                ws.Row(1).Height = 30;
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in FormatCustomTable", "Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }
        
       
        public void MakeCustomReport(Window winref, DataSet ds, CustomReportModel reportdetails)
        {
            try
            {
                using (var xl = new ExcelPackage())
                {
                    ExcelWorksheet ws;
                    ws = xl.Workbook.Worksheets.Add(reportdetails.Name);

                    int firstrow = 1;
                    int firstdatarow = firstrow + 1;
                    int lastdatarow = firstrow;
                    int dtcolumncount = 0;
                    int dtrowcount = 0;
                    int tblctr = 0;
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (!reportdetails.CombineTables)
                        {
                            if (tblctr == 0)
                                ws.Name = dt.TableName;
                            else
                                ws = xl.Workbook.Worksheets.Add(dt.TableName);
                        }

                        tblctr++;
                        dtcolumncount = dt.Columns.Count;
                        dtrowcount = dt.Rows.Count;

                        for (int i = 0; i < dtrowcount; i++)
                            for (int j = 0; j < dtcolumncount; j++)
                                ws.Cells[firstrow + i + 1, j + 1].Value = dt.Rows[i][j];

                        FormatCustomTable(dt, ref ws, firstrow);

                        if (reportdetails.CombineTables)
                        {
                            firstrow = dtrowcount + firstrow + 2; //2 rows between tables
                            firstdatarow = firstrow + 1;
                        }
                    }

                    try
                    {
                        string filename = SaveAs(winref, reportdetails.Name);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            xl.SaveAs(new System.IO.FileInfo(filename));
                            Process.Start(filename);
                        }
                    }
                    catch (Exception e)
                    {
                        IMessageBoxService msg = new MessageBoxService();
                        msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                        msg = null;
                    }
                }               
            }
            catch 
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in MakeCustomReport","Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }
        
             
        private string SaveAs(Window winref, string filename)
        {
            IMessageBoxService dlg = new MessageBoxService();
            if(winref == null)
                winref = Application.Current.Windows[0];
            string result = dlg.SaveFileDlg("Select File Name to Save As", "Excel Files(*.xlsx)| *.xlsx", filename, winref);
            dlg = null;
            return result;
        }

    }
}
