using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Classes.Data;

namespace Thinkgate
{
    public partial class LegacyExcelExport : System.Web.UI.Page
    {
        private const string newExportID = "newExportID";

        protected void Page_Load(object sender, EventArgs e)
        {
            string exportID = Request.QueryString[newExportID];
            SqlParameterCollection parms = new SqlCommand().Parameters;
            parms.AddWithValue("ReportDataID", Convert.ToInt32(exportID));
            DataRow dr = ThinkgateDataAccess.FetchDataRow(AppSettings.ConnectionString,
                                                          StoredProcedures.E3_LEGACY_EXPORT_TO_E3_GET,
                                                          CommandType.StoredProcedure,
                                                          parms);
            parms = null;

            //PLH - 8/07/2013 - Had to hard code theses replaces due to bad formatting from Legacy which Gembox can't read. 
            string exportTable = dr[0].ToString();
            exportTable = exportTable.Replace("7pt;\r\n\t\t\t\t\t\t\t", "7pt;");
            exportTable = exportTable.Replace("\r\n\t\t\t\t\t\t\t\t\t\t\t\t\tjavascript", "javascript:");
            exportTable = exportTable.Replace("void(0); \r\n\t\t\t\t\t\t\t\t\t\t\t", "void(0);");
            exportTable = exportTable.Replace("tableid=\"tbl1\"", "");
            exportTable = exportTable.Replace("aref", "a href");
            

            byte[] byteArray = Encoding.ASCII.GetBytes(exportTable);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                ExcelFile ef = ExcelFile.Load(stream, LoadOptions.HtmlDefault);
                ExcelWorksheet ws = ef.Worksheets.Add("DataSheet1");
                
                int columnCount = ws.CalculateMaxUsedColumns();
                for (int i = 0; i < columnCount; i++)
                {
                    ws.Columns[i].AutoFit(1, ws.Rows[1], ws.Rows[ws.Rows.Count - 1]);
                }

                ef.Save(Response, "Thinkgate_export.xls");
                byteArray = null;
            }
        }
    }
}