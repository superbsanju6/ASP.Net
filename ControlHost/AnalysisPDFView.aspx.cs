using System;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using WebSupergoo.ABCpdf9;
using Telerik.Web.UI;
using System.Data;
using Thinkgate.Controls.Reports;
using System.IO;
using System.Web.UI.HtmlControls;
using Standpoint.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Thinkgate.ControlHost
{
    public partial class AnalysisPDFView : BasePage
    {
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";
        public string analysisType;
        public string Guid;
        public string FormID;
        public string level;
        public string levelID;
        public DataSet ReportDs = new DataSet();
        private Criteria _reportCriteria;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            analysisType = Request.QueryString["analysisType"];
            Guid = Request.QueryString["guid"];
            FormID = Request.QueryString["formID"];
            level = Request.QueryString["level"];
            levelID = Request.QueryString["levelID"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadCriteria();
            PDFView();
            //reportHeaderDiv.Controls.Add(LoadPDFHeaderInfo());
            //barGraphLevelsContainerDiv.Controls.Add(LoadBarGraphLevels());
            //barGraphPDFContainerDiv.Controls.Add(LoadBarGraphs());
        }

        private void LoadCriteria()
        {
            _reportCriteria = Guid == null ? null : (Criteria)Session["Criteria_" + Guid];
        }

        private DataSet GetDataTable()
        {
            var userPage = SessionObject.LoggedInUser.Page;
            var year = Thinkgate.Base.Classes.AppSettings.Demo_Year;

            if (string.IsNullOrEmpty(Guid))
            {
                SessionObject.RedirectMessage =
                    "Report criteria could not be found in session. No GUID.";
                Response.Redirect("~/PortalSelection.aspx");
                return null;

            }

            var reportCriteria = (Criteria)Session["Criteria_" + Guid];
            var selectedTest = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(reportCriteria.GetAssessmentCriterion().ReportStringVal);
            var level = Request.QueryString["level"] ?? "Class";
            var selectedLevelCriterion = reportCriteria.GetLevelCriterion(level);
            var selectedLevelID = DataIntegrity.ConvertToInt(selectedLevelCriterion != null ? selectedLevelCriterion.ReportStringVal : Request.QueryString["levelID"]);
            var selectedClass = (level == "Class") ? selectedLevelID.ToString() : "";
            var criteriaOverrides = (reportCriteria).GetCriteriaOverrideString();
            var itemAnalysisData = new DataSet();

            switch (analysisType)
            {
                case "ItemAnalysis":
                    var itemAnalaysisDataSet = Thinkgate.Base.Classes.Reports.GetItemAnaylsisData(year,
                                                                                                    selectedTest,
                                                                                                    level,
                                                                                                    selectedLevelID,
                                                                                                    userPage,
                                                                                                    _permissions,
                                                                                                    "",
                                                                                                    "",
                                                                                                    "",
                                                                                                    0,
                                                                                                    selectedClass,
                                                                                                    "@@Product=none@@@@RR=none" + criteriaOverrides + "1test=yes@@@@PT=1@@@@FormID=" + FormID + "@@",
                                                                                                    FormID);

                    itemAnalysisData.Tables.Add(Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(itemAnalaysisDataSet.Tables[0].Copy(),
                                                                                    "Question", "xID").Copy());
                    itemAnalysisData.Tables.Add(itemAnalaysisDataSet.Tables[1].Copy());

                    break;

                case "StandardAnalysis":
                    var standardAnalaysisDataSet = Thinkgate.Base.Classes.Reports.GetStandardAnaylsisData(year, selectedTest,
                                                                                                level,
                                                                                                selectedLevelID, userPage,
                                                                        _permissions, "", "", "", 0, selectedClass,
                                                                       "@@Product=none@@@@RR=none" +
                                                                                                criteriaOverrides +
                                                                                                "1test=yes@@@@PT=1@@@@FormID=" + FormID + "@@",
                                                                                                FormID, "SS");




                    itemAnalysisData.Tables.Add(Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(standardAnalaysisDataSet.Tables[0].Copy(), "StandardID", "xID"));
                    itemAnalysisData.Tables.Add(standardAnalaysisDataSet.Tables[1].Copy());

                    break;
            }

            Session["ItemAnalysisData_" + analysisType + "_" + Guid] = itemAnalysisData;
            return itemAnalysisData;
        }

        public void PDFView()
        {
            Doc doc = ItemAnalysisToPdfDoc();
            byte[] theData = doc.GetData();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);
        }
        
        private HtmlTable LoadPDFHeaderInfo()
        {
            var table = new HtmlTable();
            var row1 = new HtmlTableRow();
            var cell1 = new HtmlTableCell();
            var cell2 = new HtmlTableCell();
            table.Border = 1;
            table.Attributes["style"] = "border:solid 1px #000; border-collapse:collapse; font-family:Arial, sans-serif; width:900px; font-size:10pt;";
            table.CellPadding = 2;
            ReportDs = (DataSet)(Session["ItemAnalysisData_" + analysisType + "_" + Guid] ?? GetDataTable());

            cell1.InnerHtml = (analysisType == "ItemAnalysis" ? "Item" : "Standard") + " Analysis Multi-Level " + (string.IsNullOrEmpty(FormID) || FormID == "0" ? "" : "(" + FormID + "01)");
            cell2.InnerHtml = AppSettings.ApplicationName;
            cell1.ColSpan = 2;
            cell2.ColSpan = 6;

            cell1.Attributes["style"] = "font-weight:bold; width:40%; background-color:#D0D0D0;font-size:12pt;";
            cell2.Attributes["style"] = "font-weight:bold; width:50%; background-color:#D0D0D0;font-size:12pt;";

            row1.Cells.Add(cell1);
            row1.Cells.Add(cell2);
            table.Rows.Add(row1);

            if (ReportDs != null && ReportDs.Tables.Count >= 2 && ReportDs.Tables[0].Rows.Count > 0)
            {
                string testScoreType = ReportDs.Tables[0].Rows[0]["TestScoreType"].ToString();
                double possible = DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Possible"]) == 0 ? 1 : DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Possible"]);
                var row2 = new HtmlTableRow();
                var row2cell1 = new HtmlTableCell();
                var row2cell2 = new HtmlTableCell();
                var row2cell3 = new HtmlTableCell();
                var row2cell4 = new HtmlTableCell();
                var row2cell5 = new HtmlTableCell();
                var row2cell6 = new HtmlTableCell();
                var row2cell7 = new HtmlTableCell();
                var row2cell8 = new HtmlTableCell();
                row2cell1.InnerHtml = "<b>Test</b> ";
                row2cell2.InnerHtml = ReportDs.Tables[0].Rows[0]["TestName"].ToString();
                row2cell3.InnerHtml = "<b>School</b> ";
                ///Modification 12/5/2013
                /// The true portion of this lambda expression is not displaying the correct data.
                /// The report is expecting a school name so, I am using the false portion of this statement
                /// which returns the school name
                ///      REMOVED

                //row2cell4.InnerHtml = level == "School" && _reportCriteria != null
                //                          ? ((Criteria)_reportCriteria).CriterionList.FindAll(
                //                              r => r.Key.Contains(level) && r.Object != null).Select(
                //                                  criterion => criterion.Object.ToString()).ToList().ToString()
                //                          : ReportDs.Tables[0].Rows[0]["SchoolName"].ToString();
                row2cell4.InnerHtml = ReportDs.Tables[0].Rows[0]["SchoolName"].ToString();
                row2cell5.InnerHtml = "<b># Schools</b> ";
                row2cell6.InnerHtml = ReportDs.Tables[0].Rows[0]["SchoolCount"].ToString();
                row2cell7.InnerHtml = "<b>High</b> ";
                row2cell8.InnerHtml = testScoreType == "S" ? Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Highest"])), 2).ToString()
                : Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Highest"]) * 100), 2).ToString() + "%";
                row2.Cells.Add(row2cell1);
                row2.Cells.Add(row2cell2);
                row2.Cells.Add(row2cell3);
                row2.Cells.Add(row2cell4);
                row2.Cells.Add(row2cell5);
                row2.Cells.Add(row2cell6);
                row2.Cells.Add(row2cell7);
                row2.Cells.Add(row2cell8);

                var row3 = new HtmlTableRow();
                var row3cell1 = new HtmlTableCell();
                var row3cell2 = new HtmlTableCell();
                var row3cell3 = new HtmlTableCell();
                var row3cell4 = new HtmlTableCell();
                var row3cell5 = new HtmlTableCell();
                var row3cell6 = new HtmlTableCell();
                var row3cell7 = new HtmlTableCell();
                var row3cell8 = new HtmlTableCell();
                row3cell1.InnerHtml = "<b>Criteria</b>";

                row3cell2.InnerHtml = "Group: " + _reportCriteria.GetLevelCriterion("group").Object.ToString();
  
                row3cell3.InnerHtml = "<b>Teacher</b> ";
                if (level == "Teacher" && _reportCriteria != null)
                {
                    List<string> teachers = ((Criteria)_reportCriteria).CriterionList.FindAll(
                                  r => r.Key.Contains(level) && r.Object != null).Select(
                                  criterion => criterion.Object.ToString()).ToList();

                    if (teachers.Count < 1)
                    {
                        row3cell4.InnerHtml = ReportDs.Tables[0].Rows[0]["TeacherName"].ToString();
                    }
                    else
                    {
                        foreach (string teacher in teachers)
                        {
                            row3cell4.InnerHtml = row3cell4.InnerHtml + teacher + "<br />";
                        }
                    }
                }
                else row3cell4.InnerHtml = ReportDs.Tables[0].Rows[0]["TeacherName"].ToString();
                row3cell5.InnerHtml = "<b># Teachers</b>";
                row3cell6.InnerHtml = ReportDs.Tables[0].Rows[0]["TeacherCount"].ToString();
                row3cell7.InnerHtml = "<b>Low</b> ";
                row3cell8.InnerHtml = testScoreType == "S" ? Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Lowest"])), 2).ToString()
                : Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Lowest"]) * 100), 2).ToString() + "%";

                row3cell1.Attributes["style"] = "border-bottom:solid 0px #000;";
                row3cell2.Attributes["style"] = "border-bottom:solid 0px #000;";

                row3.Cells.Add(row3cell1);
                row3.Cells.Add(row3cell2);
                row3.Cells.Add(row3cell3);
                row3.Cells.Add(row3cell4);
                row3.Cells.Add(row3cell5);
                row3.Cells.Add(row3cell6);
                row3.Cells.Add(row3cell7);
                row3.Cells.Add(row3cell8);

                var row4 = new HtmlTableRow();
                var row4cell1 = new HtmlTableCell();
                var row4cell2 = new HtmlTableCell();
                var row4cell3 = new HtmlTableCell();
                var row4cell4 = new HtmlTableCell();
                var row4cell5 = new HtmlTableCell();
                var row4cell6 = new HtmlTableCell();
                var row4cell7 = new HtmlTableCell();
                var row4cell8 = new HtmlTableCell();
                row4cell3.InnerHtml = "<b>Class</b> ";
                row4cell4.InnerHtml = level == "Class" && _reportCriteria != null
                                          ? string.Join(",",
                                                        ((Criteria)_reportCriteria).CriterionList.FindAll(
                                                            r => r.Key.Contains(level) && r.Object != null).Select(
                                                                criterion => criterion.Object.ToString()).ToList())
                                          : ReportDs.Tables[0].Rows[0]["ClassName"].ToString();
                row4cell5.InnerHtml = "<b># Classes</b>";
                row4cell6.InnerHtml = ReportDs.Tables[0].Rows[0]["ClassCount"].ToString();
                row4cell7.InnerHtml = "<b>Median</b> ";
                row4cell8.InnerHtml = testScoreType == "S" ? Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Median"])), 2).ToString()
                : Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Median"]) * 100), 2).ToString() + "%";

                row4cell1.Attributes["style"] = "border-bottom:solid 0px #000;";
                row4cell2.Attributes["style"] = "border-bottom:solid 0px #000;";

                row4.Cells.Add(row4cell1);
                row4.Cells.Add(row4cell2);
                row4.Cells.Add(row4cell3);
                row4.Cells.Add(row4cell4);
                row4.Cells.Add(row4cell5);
                row4.Cells.Add(row4cell6);
                row4.Cells.Add(row4cell7);
                row4.Cells.Add(row4cell8);

                var row5 = new HtmlTableRow();
                var row5cell1 = new HtmlTableCell();
                var row5cell2 = new HtmlTableCell();
                var row5cell3 = new HtmlTableCell();
                var row5cell4 = new HtmlTableCell();
                var row5cell5 = new HtmlTableCell();
                var row5cell6 = new HtmlTableCell();
                var row5cell7 = new HtmlTableCell();
                var row5cell8 = new HtmlTableCell();
                row5cell3.InnerHtml = "<b>Student</b> ";
                row5cell4.InnerHtml = level == "Student" && _reportCriteria != null
                                          ? string.Join(",",
                                                        ((Criteria)_reportCriteria).CriterionList.FindAll(
                                                            r => r.Key.Contains(level) && r.Object != null).Select(
                                                                criterion => criterion.Object.ToString()).ToList())
                                          : "";
                row5cell5.InnerHtml = "<b># Students</b>";
                row5cell6.InnerHtml = ReportDs.Tables[0].Rows[0]["StudentCount"].ToString();
                row5cell7.InnerHtml = "<b>Mean</b> ";
                row5cell8.InnerHtml = testScoreType == "S" ? Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Mean"])), 2).ToString()
                    : Math.Round((DataIntegrity.ConvertToDouble(ReportDs.Tables[0].Rows[0]["Mean"]) * 100), 2).ToString() + "%";

                row5.Cells.Add(row5cell1);
                row5.Cells.Add(row5cell2);
                row5.Cells.Add(row5cell3);
                row5.Cells.Add(row5cell4);
                row5.Cells.Add(row5cell5);
                row5.Cells.Add(row5cell6);
                row5.Cells.Add(row5cell7);
                row5.Cells.Add(row5cell8);

                table.Rows.Add(row2);
                table.Rows.Add(row3);
                table.Rows.Add(row4);
                table.Rows.Add(row5);
            }


            return table;
        }

        protected HtmlTable LoadBarGraphLevels()
        {
            var table = new HtmlTable();

            ReportDs = (DataSet)(Session["ItemAnalysisData_" + analysisType + "_" + Guid] ?? GetDataTable());

            if (ReportDs != null && ReportDs.Tables.Count >= 2)
            {
                var seriesRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                                  select new
                                  {
                                      chartItem = sRow["ChartItem"].ToString(),
                                      color = sRow["ChartColor"].ToString(),
                                      asOfDate = !String.IsNullOrEmpty(sRow["DateTimeCalculated"].ToString()) ? "As of " + sRow["DateTimeCalculated"].ToString() : ""
                                  }).Distinct();

                var newRow = new HtmlTableRow();
                foreach (var row in seriesRows)
                {
                    var itemCell = new HtmlTableCell();
                    var cellHTML = "<div style=\"background-color:" + row.color + ";height:60px;width:100px;border:1px solid black;padding:3px;text-align:center;margin:3px;";
                    cellHTML += "font-size:12px;font-style:normal;font-family:Tahoma,Arial,sans-serif;\"><b>" + row.chartItem + "</b><br />";
					cellHTML += "<font style=\"font-size: 8pt\">" + row.asOfDate + "</font><br/></div>";
                    itemCell.InnerHtml = cellHTML;
                    newRow.Cells.Add(itemCell);
                }
                table.Rows.Add(newRow);
            }

            return table;
        }

        protected HtmlTable LoadBarGraphs()
        {
            var table = new HtmlTable();
            table.Width = "800";
            table.CellPadding = 0;
            table.CellSpacing = 0;

            ReportDs = (DataSet)(Session["ItemAnalysisData_" + analysisType + "_" + Guid] ?? GetDataTable());

            if (ReportDs != null && ReportDs.Tables.Count >= 2)
            {
                if (ReportDs.Tables[0].Columns.Contains("FieldTest"))
                {
                    var itemRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                                    select new
                                    {
                                        ChartGroup = sRow["ChartGroup"].ToString(),
                                        xID = sRow["xID"].ToString(),
                                        Sort = (analysisType == "ItemAnalysis") ? sRow["ChartGroup"].ToString() : sRow["StandardNbr"].ToString(),
                                        FieldTest = sRow["FieldTest"].ToString()
                                    }).Distinct();
                    var rowCount = 0;
                    var rowCountPage1 = 0;

                    foreach (var row in itemRows)
                    {
                        rowCount++;
                        rowCountPage1++;
                        var newRow = new HtmlTableRow();
                        var itemCell = new HtmlTableCell();
                        newRow.BgColor = "#FFFFFF";
                        itemCell.Attributes["style"] = "width:20px;vertical-align:top;font-weight:bold;border:1px solid #dadada;border-bottom: 0px;";
                        itemCell.Width = "1%";
                        if (row.FieldTest == "1") itemCell.BgColor = "#FFFF00";
                        itemCell.InnerHtml = row.Sort;

                        var barsCell = new HtmlTableCell();
                        barsCell.Attributes["style"] = "border: 1px solid #dadada; padding-bottom: 2px;";
                        var newTable = new HtmlTable();
                        newTable.CellPadding = 0;
                        newTable.CellSpacing = 0;
                        newTable.Width = "100%";
                        if (rowCountPage1 == 8)
                        {
                            newTable.Attributes["style"] = "page-break-after:always;";
                            rowCount = 0;
                        }
                        else if ((rowCount % 9) == 0) newTable.Attributes["style"] = "page-break-after:always;";

                        foreach (DataRow barsRow in ReportDs.Tables[0].Select("ChartGroup = '" + row.ChartGroup + "'"))
                        {
                            var newBarsRow = new HtmlTableRow();
                            var newBarsCell = new HtmlTableCell();
                            var newChartAmtCell = new HtmlTableCell();
                            var chartAmount = 100 * DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]);
                            var formattedChartAmount = String.Format("{0:p}", DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]));
                            newBarsCell.Attributes["style"] = "border: 1px solid #dadada;";
                            newBarsCell.InnerHtml = "<div style=\"background-color:" + barsRow["ChartColor"] + ";width:" + chartAmount + "%;border:1px solid black;text-align:right;font-weight:bold;\">&nbsp;</div>";
                            newChartAmtCell.Attributes["style"] = "width: 70px;font-weight:bold; font-size: 10pt; border:1px solid #dadada;";
                            newChartAmtCell.InnerHtml = formattedChartAmount + "&nbsp;";
                            newBarsRow.Cells.Add(newBarsCell);
                            newBarsRow.Cells.Add(newChartAmtCell);
                            newTable.Rows.Add(newBarsRow);
                        }
                        var barSpacerRow = new HtmlTableRow();
                        var barSpacerCell1 = new HtmlTableCell();
                        var barSpacerCell2 = new HtmlTableCell();
                        barSpacerCell1.Attributes["style"] = "border: 1px solid #dadada;";
                        barSpacerCell2.Attributes["style"] = "border: 1px solid #dadada;";
                        barSpacerCell1.InnerHtml = "&nbsp;";
                        barSpacerCell2.InnerHtml = "&nbsp;";
                        barSpacerRow.Cells.Add(barSpacerCell1);
                        barSpacerRow.Cells.Add(barSpacerCell2);
                        newTable.Rows.Add(barSpacerRow);

                        barsCell.Controls.Add(newTable);
                        newRow.Cells.Add(itemCell);
                        newRow.Cells.Add(barsCell);
                        table.Rows.Add(newRow);
                    }
                }
                else
                {
                    var itemRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                                    select new
                                    {
                                        ChartGroup = sRow["ChartGroup"].ToString(),
                                        xID = sRow["xID"].ToString(),
                                        Sort = (analysisType == "ItemAnalysis") ? sRow["ChartGroup"].ToString() : sRow["StandardNbr"].ToString()
                                    }).Distinct();
                    var rowCount = 0;
                    var rowCountPage1 = 0;

                    foreach (var row in itemRows)
                    {
                        rowCount++;
                        rowCountPage1++;
                        var newRow = new HtmlTableRow();
                        var itemCell = new HtmlTableCell();
                        newRow.BgColor = "#FFFFFF";
                        itemCell.Attributes["style"] = "width:20px;vertical-align:top;font-weight:bold;border:1px solid #dadada;border-bottom: 0px;";
                        itemCell.Width = "1%";
                        itemCell.InnerHtml = row.Sort;

                        var barsCell = new HtmlTableCell();
                        barsCell.Attributes["style"] = "border: 1px solid #dadada; padding-bottom: 2px;";
                        var newTable = new HtmlTable();
                        newTable.CellPadding = 0;
                        newTable.CellSpacing = 0;
                        newTable.Width = "100%";
                        if (rowCountPage1 == 8)
                        {
                            newTable.Attributes["style"] = "page-break-after:always;";
                            rowCount = 0;
                        }
                        else if ((rowCount % 9) == 0) newTable.Attributes["style"] = "page-break-after:always;";

                        foreach (DataRow barsRow in ReportDs.Tables[0].Select("ChartGroup = '" + row.ChartGroup + "'"))
                        {
                            var newBarsRow = new HtmlTableRow();
                            var newBarsCell = new HtmlTableCell();
                            var newChartAmtCell = new HtmlTableCell();
                            var chartAmount = 100 * DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]);
                            var formattedChartAmount = String.Format("{0:p}", DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]));
                            newBarsCell.Attributes["style"] = "border: 1px solid #dadada;";
                            newBarsCell.InnerHtml = "<div style=\"background-color:" + barsRow["ChartColor"] + ";width:" + chartAmount + "%;border:1px solid black;text-align:right;font-weight:bold;\">&nbsp;</div>";
                            newChartAmtCell.Attributes["style"] = "width: 70px;font-weight:bold;font-size: 10pt;border:1px solid #dadada;";
                            newChartAmtCell.InnerHtml = formattedChartAmount + "&nbsp;";
                            newBarsRow.Cells.Add(newBarsCell);
                            newBarsRow.Cells.Add(newChartAmtCell);
                            newTable.Rows.Add(newBarsRow);
                        }
                        var barSpacerRow = new HtmlTableRow();
                        var barSpacerCell1 = new HtmlTableCell();
                        var barSpacerCell2 = new HtmlTableCell();
                        barSpacerCell1.Attributes["style"] = "border: 1px solid #dadada;";
                        barSpacerCell2.Attributes["style"] = "border: 1px solid #dadada;";
                        barSpacerCell1.InnerHtml = "&nbsp;";
                        barSpacerCell2.InnerHtml = "&nbsp;";
                        barSpacerRow.Cells.Add(barSpacerCell1);
                        barSpacerRow.Cells.Add(barSpacerCell2);
                        newTable.Rows.Add(barSpacerRow);

                        barsCell.Controls.Add(newTable);
                        newRow.Cells.Add(itemCell);
                        newRow.Cells.Add(barsCell);
                        table.Rows.Add(newRow);
                    }
                }
            }

            return table;
        }

        protected Doc ItemAnalysisToPdfDoc(PdfRenderSettings settings = null)
        {
            if (settings == null) settings = new PdfRenderSettings();

            StringWriter sw = new StringWriter();
            HtmlTextWriter w = new HtmlTextWriter(sw);
            reportHeaderDiv.Controls.Add(LoadPDFHeaderInfo());
            barGraphLevelsContainerDiv.Controls.Add(LoadBarGraphLevels());
            barGraphPDFContainerDiv.Controls.Add(LoadBarGraphs());
            contentDiv.RenderControl(w);
            string result_html = sw.GetStringBuilder().ToString();

            int topOffset = settings.HeaderHeight > 0 ? settings.HeaderHeight : 0;
            int bottomOffset = settings.FooterHeight > 0 ? settings.FooterHeight : 0;

            Doc doc = new Doc();
            doc.HtmlOptions.HideBackground = true;
            doc.HtmlOptions.PageCacheEnabled = false;
            doc.HtmlOptions.UseScript = true;
            doc.HtmlOptions.Timeout = 36000;
            doc.HtmlOptions.BreakZoneSize = 100;    // I experiemented with this being 99% instead of 100%, but you end up with passages getting cut off in unflattering ways. This may lead to more blank space... but I think it's the lessor of evils
            doc.HtmlOptions.ImageQuality = 70;

            doc.MediaBox.String = "0 0 " + settings.PageWidth + " " + settings.PageHeight;
            doc.Rect.String = settings.LeftMargin + " " + (0 + bottomOffset).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + (settings.PageHeight - topOffset).ToString();
            doc.HtmlOptions.AddTags = true;
            doc.SetInfo(0, "ApplyOnLoadScriptOnceOnly", "1");

            List<int> forms = new List<int>();

            int theID = doc.AddImageHtml(result_html);

            while (true)
            {
                if (!doc.Chainable(theID))
                    break;
                doc.Page = doc.AddPage();
                theID = doc.AddImageToChain(theID);
                string[] tagIds = doc.HtmlOptions.GetTagIDs(theID);
                if (tagIds.Length > 0 && tagIds[0] == "test_header")
                    forms.Add(doc.PageNumber);					// By using GetTagIDs to find if a test header ended up on this page, we can determine whether the page needs a header

                if (settings.PossibleForcedBreaks)
                {		// only want to take the performance hit if there's a change we're forcing breaks
                    if (String.IsNullOrEmpty(doc.GetText("Text")))
                    {		// WSH Found situation where after I added page break always for multi-form, one test that was already breaking properly, added an extra page that was blank between forms. Almost like some amount of HTML had been put there, even though it wasn't any real text. By checking to make sure there is some actual text on page, we can avoid that problem
                        doc.Delete(doc.Page);
                    }
                }
            }

            if (settings.HeaderHeight > 0 && !String.IsNullOrEmpty(settings.HeaderText))
            {
                /*HttpServerUtility Server = HttpContext.Current.Server;
                headerText = Server.HtmlDecode(headerText);*/
                Doc headerDoc = new Doc();

                headerDoc.MediaBox.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                headerDoc.Rect.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                headerDoc.VPos = 0.5;
                int headerID = headerDoc.AddImageHtml(settings.HeaderText);

                if (!String.IsNullOrEmpty(settings.HeaderText))
                {
                    int form_ref = 0;
                    for (int i = 1; i <= doc.PageCount; i++)
                    {
                        if (form_ref < forms.Count && forms[form_ref] == i)
                        {
                            form_ref++;
                        }
                        else
                        {
                            if (i > 1 || settings.ShowHeaderOnFirstPage)
                            {
                                doc.PageNumber = i;
                                doc.Rect.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                                doc.VPos = 0.5;

                                theID = doc.AddImageDoc(headerDoc, 1, null);
                                theID = doc.AddImageToChain(theID);
                            }
                        }
                    }
                }
            }

            for (int i = 1; i <= doc.PageCount; i++)
            {
                doc.PageNumber = i;
                doc.Flatten();
            }
            return doc;
        }

    }
}
