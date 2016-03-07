using System;
using System.IO;
using System.Text;
using ClosedXML.Excel;


namespace Thinkgate
{
    public partial class FileExport : System.Web.UI.Page
    {
        public FileExport()
        {
            GetType();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var guid = Request.QueryString["sessionID"];

            if (string.IsNullOrEmpty(guid)) return;

            Byte[] fileExportContent = null;

            if (Session["FileExport_Content" + guid] is Byte[])
                fileExportContent = (Byte[])Session["FileExport_Content" + guid];

            WriteFile(fileExportContent);

        }

        protected void WriteFile(Byte[] byteArray)
        {
            System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
            Response.Clear();
            Response.Buffer = true;

            if (browser.Platform.IndexOf("WinNT", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");
                Response.BinaryWrite(byteArray);
            }
            else
            {
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "text/csv";
                Response.ContentEncoding = Encoding.UTF8;
                Response.AppendHeader("Content-Disposition", "attachment;filename=ExportData.csv");
                byte[] csv = ConvertXLToCSV(byteArray);
                Response.BinaryWrite(csv);
                Response.Flush();
            }
            Response.End();
        }

        /// <summary>
        /// ConvertXLToCSV
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] ConvertXLToCSV(byte[] data)
        {
            var workbook = new XLWorkbook(new System.IO.MemoryStream(data));
            var worksheet = workbook.Worksheet(1);
            StringBuilder csvStringBuilder = new StringBuilder();

            // Look for the first row used
            var firstRowUsed = worksheet.FirstRowUsed();
            // Narrow down the row so that it only includes the used part
            var exportedRow = firstRowUsed.RowUsed();

            int iCount = exportedRow.CellCount();
            //Verify null value for current column
            while (!IsBlankRow(exportedRow, iCount))
            {
                RenderRow(exportedRow, ref csvStringBuilder, iCount);
                csvStringBuilder.Append("\r\n");
                exportedRow = exportedRow.RowBelow();
            }

            return Encoding.UTF8.GetBytes(csvStringBuilder.ToString());
        }

        /// <summary>
        /// RenderRow
        /// </summary>
        /// <param name="iXLRangeRow"></param>
        /// <param name="builder"></param>
        /// <param name="columnsCount"></param>
        static void RenderRow(IXLRangeRow iXLRangeRow, ref StringBuilder builder, int columnsCount)
        {
            for (int iRow = 1; iRow <= columnsCount; iRow++)
            {
                String columnValue = iXLRangeRow.Cell(iRow).GetString();
                builder.Append("\"" + columnValue + "\"").Append(",");
            }
        }

        /// <summary>
        /// IsBlankRow
        /// </summary>
        /// <param name="iXLRangeRow"></param>
        /// <param name="columnsCount"></param>
        /// <returns></returns>
        static bool IsBlankRow(IXLRangeRow iXLRangeRow, int columnsCount)
        {
            bool isEmpty = true;
            for (int jRow = 1; jRow <= columnsCount; jRow++)
            {
                if (iXLRangeRow.Cell(jRow).IsEmpty() == false)
                {
                    isEmpty = false;
                    break;
                }
            }
            return isEmpty;
        }
    }
}