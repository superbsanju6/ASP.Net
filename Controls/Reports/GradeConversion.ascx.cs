using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Reflection;

using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using GemBox.Spreadsheet;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.Reports
{
    public partial class GradeConversion : TileControlBase
    {
        private int _testID;
        private string _level;
        private int _levelID;
        private string _year;
        private string _testType;
        private DataTable _conversionDataTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            if (Request.QueryString["selectedReport"] != null && !String.IsNullOrEmpty(Request.QueryString["selectedReport"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["selectedReport"].ToString() + "' report", Request.QueryString["selectedReport"].ToString(), sessionObject.LoggedInUser.UserName);
            }
            LoadTileParms();
            LoadPerformanceLevels();
            LoadConversionTable();
        }

        private void LoadTileParms()
        {
            _testID = Convert.ToInt32(Tile.TileParms.GetParm("testID").ToString());
            _level = Tile.TileParms.GetParm("level").ToString();
            _levelID = Convert.ToInt32(Tile.TileParms.GetParm("levelID").ToString());
            _year = Tile.TileParms.GetParm("year").ToString();
            _testType = Tile.TileParms.GetParm("testType").ToString();
        }

        private void LoadPerformanceLevels()
        {
            DataTable dt = Base.Classes.Reports.GetGradeConversionPerformanceLevels(_testID, _year);

            TableRow tr = new TableRow();

            TableCell tcPerf = new TableCell();
            tcPerf.Text = "Grade Conversion Performance Levels: ";
            tr.Cells.Add(tcPerf);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TableCell tc = new TableCell();
                if (i == dt.Rows.Count - 1)
                {
                    tc.Text = dt.Rows[i]["PText"].ToString() + " (" + dt.Rows[i]["MinScore"].ToString() + "% - 100%)";
                }
                else
                {
                    tc.Text = dt.Rows[i]["PText"].ToString() + " (" + dt.Rows[i]["MinScore"].ToString() + "% - " + (Convert.ToInt32(dt.Rows[i + 1]["MinScore"].ToString()) - 1) + "%)";
                }
                tc.BackColor = Color.FromName(dt.Rows[i]["Color"].ToString());
                tr.Cells.Add(tc);
            }
            tblPerformanceLevels.Rows.Add(tr);
        }

        private void LoadConversionTable()
        {
            _conversionDataTable = Base.Classes.Reports.GetGradeConversion(_level, _levelID, _testID, _testType, _year);
            rgdGradeConversion.DataSource = _conversionDataTable;
            rgdGradeConversion.DataBind();

            SetControlVisibility();
        }

        private void SetControlVisibility()
        {
            bool hasConversionData = _conversionDataTable.Rows.Count > 0;
            rgdGradeConversion.Visible = hasConversionData;
            tblPerformanceLevels.Visible = hasConversionData;
            lblNoRecords.Visible = !hasConversionData;
            excelImgBtn.Visible = hasConversionData;
        }

        protected void rgdGradeConversion_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                item["RawScore"].BackColor = Color.FromName(item["Color"].Text);
                item["RawScore"].Font.Bold = true;
                item["ScaleScore"].BackColor = Color.FromName(item["Color"].Text);
                item["ScaleScore"].Font.Bold = true;
                item["ConvertedScore"].BackColor = Color.FromName(item["Color"].Text);
                item["ConvertedScore"].Font.Bold = true;
            }
        }

        protected void rgdGradeConversion_ColumnCreated(object sender, Telerik.Web.UI.GridColumnCreatedEventArgs e)
        {
            if (e.Column.UniqueName == "PerformanceLevel" || e.Column.UniqueName == "Color")
            {
                GridBoundColumn col = (GridBoundColumn)e.Column;
                col.Display = false;
            }
        }

        public override void ExportToExcel()
        {
            // Create the workbook
            ExcelFile ef = new ExcelFile();
            ef.DefaultFontName = "Calibri";
            ExcelWorksheet ws = ef.Worksheets.Add("DataSheet");
            FormatGradeConversion(ws);
            ef.Save(Response, "ExportData.xlsx");
        }


        public void FormatGradeConversion(ExcelWorksheet ws)
        {
            foreach (DataColumn dc in _conversionDataTable.Columns)
            {
                if (dc.ColumnName != "PerformanceLevel" && dc.ColumnName != "Color")
                {
                    ws.Cells[0, dc.Ordinal].Value = dc.ColumnName;
                    ws.Cells[0, dc.Ordinal].Style.Font.Weight = ExcelFont.BoldWeight;
                }
            }

            int rowCount = 1;
            foreach (DataRow dr in _conversionDataTable.Rows)
            {
                ws.Cells[rowCount, 0].Value = dr["Student_ID"].ToString();
                ws.Cells[rowCount, 1].Value = dr["Student_Name"].ToString();
                ws.Cells[rowCount, 2].Value = dr["RawScore"].ToString();
                ws.Cells[rowCount, 2].Style.FillPattern.SetSolid(Color.FromName(dr["Color"].ToString()));
                ws.Cells[rowCount, 3].Value = dr["ScaleScore"].ToString();
                ws.Cells[rowCount, 3].Style.FillPattern.SetSolid(Color.FromName(dr["Color"].ToString()));
                ws.Cells[rowCount, 4].Value = dr["ConvertedScore"].ToString();
                ws.Cells[rowCount, 4].Style.FillPattern.SetSolid(Color.FromName(dr["Color"].ToString()));
                rowCount++;
            }

            int columnCount = ws.CalculateMaxUsedColumns();
            for (int i = 0; i < columnCount; i++)
            {
                ws.Columns[i].AutoFit(1, ws.Rows[0], ws.Rows[ws.Rows.Count - 1]);
            }
        }
    }
}