using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Thinkgate.Controls.E3Criteria
{
    public class ExportToCSV
    {
        /// <summary>
        /// ConvertXLToCSV
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ConvertXLToCSV(XLWorkbook workbook)
        {
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
        /// ConvertXLToCSV
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ConvertXLToCSV(byte[] data)
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