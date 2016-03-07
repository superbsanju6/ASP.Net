using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;
using System.Drawing;
using System.Web.UI;
using System.Reflection;
using WebSupergoo.ABCpdf9;
using Thinkgate.Base.Classes;
using System.IO;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using ClosedXML.Excel;
using Thinkgate.Base.Enums.AssessmentScheduling;
using GemBox.Spreadsheet;
using Thinkgate.ExceptionHandling;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Reports
{
    public enum AnalysisType
    {
        ItemAnalysis,
        StandardAnalysis
    }

    public partial class ItemAnalysis : TileControlBase
    {
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";
        public AnalysisType? AnalysisType;
        public string Guid;
        public int FormID;
        public string _groupFilterName;
        public DataSet ReportDs = new DataSet();
        protected bool _isExcel;
        private Criteria _reportCriteria;
        private bool _isContentLocked;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            if (Tile.TileParms.GetParm("AnalysisType") == null) return;

            if (Request.QueryString["selectedReport"] != null && !String.IsNullOrEmpty(Request.QueryString["selectedReport"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["selectedReport"].ToString() + "' report", Request.QueryString["selectedReport"].ToString(), sessionObject.LoggedInUser.UserName);
                if (Request.QueryString["selectedReport"].ToString().ToLower() == "item analysis")
                {
                    // TFS 1123 : Check the content is locked or unlocked for specific Test ID
                    if (Request.QueryString["testID"] != null)
                    {
                        int testID = Convert.ToInt32(Request.QueryString["testID"]);
                        DataRow row = Thinkgate.Base.Classes.Assessment.GetAssessmentSchedule(testID);

                        if (row != null)
                        {
                            string contentLock;
                            string content = row["CONTENT"].ToString();
                            if (content == AssessmentScheduleStatus.Disabled.ToString())
                            { _isContentLocked = true; }
                            else if (content == AssessmentScheduleStatus.Enabled.ToString())
                            { _isContentLocked = false; }
                            else if (row["CONTENT"].ToString().Split(' ')[0] == AssessmentScheduleStatus.Enabled.ToString())
                            {
                                contentLock = row["CONTENT"].ToString();
                                DateTime dateFrom = DateTime.MinValue;
                                DateTime dateTo = DateTime.MaxValue;

                                if (contentLock.IndexOf(" - ") > -1) //Enabled 10/15/2013 - 10/31/2013
                                {
                                    dateFrom = Convert.ToDateTime(contentLock.ToString().Split(' ')[1].Trim());
                                    dateTo = Convert.ToDateTime(contentLock.ToString().Split(' ')[3].Trim());
                                }

                                else if (contentLock.IndexOf("Starting") > -1) //Enabled Starting 10/15/2013
                                {
                                    dateFrom = Convert.ToDateTime(contentLock.ToString().Split(' ')[2]);
                                }

                                else if (contentLock.IndexOf("Until") > -1) //Enabled Until 10/15/2013
                                {
                                    dateTo = Convert.ToDateTime(contentLock.ToString().Split(' ')[2]);
                                }

                                if (dateFrom <= DateTime.Today && DateTime.Today <= dateTo)
                                { _isContentLocked = false; }
                                else
                                { _isContentLocked = true; }
                            }
                        }
                    }
                }
            }

            AnalysisType = (AnalysisType)Tile.TileParms.GetParm("AnalysisType");
            analysisReportType.Value = AnalysisType.ToString();
            LoadCriteria();
            LoadReport();
            LoadForms();
            //btnFormSelection.Attributes.Add("dropdownListID", ctxForm.ClientID);
        }

        private void LoadForms()
        {
            rcbItemAnalysisForms.Items.Clear();
            rcbItemAnalysisForms.Items.Add(new RadComboBoxItem() { Text = "All Students", Value = "0" });

            var forms = (from DataRow sRow in ReportDs.Tables[1].Rows
                         select new
                         {
                             formID = sRow["FormID"].ToString(),
                             formName = sRow["FormIDDisplay"].ToString()
                         }).Distinct();
            foreach (var form in forms)
            {
                rcbItemAnalysisForms.Items.Add(new RadComboBoxItem() { Text = form.formName, Value = form.formID });
            }
            //rcbItemAnalysisForms.DataSource = forms;
            //rcbItemAnalysisForms.DataBind();

            rcbItemAnalysisForms.Enabled = (rcbItemAnalysisForms.Items.Count > 1);
        }

        protected void RemoteReportReload(object sender, EventArgs e)
        {
            Session["ItemAnalysisData_" + AnalysisType + "_" + Guid] = null;
            if (Session["Criteria_" + Guid] == null) return;
            _reportCriteria = Guid == null ? null : (Criteria)Session["Criteria_" + Guid];
            LoadReport();
        }

        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlItemAnalysisCriteria";

            if (Tile.TileParms.GetParm("CriteriaGUID") == null) return;
            Guid = (string)Tile.TileParms.GetParm("CriteriaGUID");
            guidValue.Value = Guid;

            if (Session["Criteria_" + Guid] == null) return;
            _reportCriteria = Guid == null ? null : (Criteria)Session["Criteria_" + Guid];

            ctlReportCriteria.Guid = Guid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            criteriaPlaceholder.Controls.Add(ctlReportCriteria);
        }

        private DataSet GetDataTable()
        {

            //Do switch here on report type
            if (!AnalysisType.HasValue) AnalysisType = Reports.AnalysisType.ItemAnalysis;

            var dev = System.Configuration.ConfigurationManager.AppSettings.Get("Environment") == "Dev" ? true : false;

            //var userPage = dev ? 119 : 60528;
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
            var selectedLevelID = selectedLevelCriterion != null ? DataIntegrity.ConvertToInt(selectedLevelCriterion.ReportStringVal) : Cryptography.DecryptionToInt(Request.QueryString["levelID"], SessionObject.LoggedInUser.CipherKey);
            var selectedClass = (level == "Class") ? selectedLevelID.ToString() : "";
            var criteriaOverrides = (reportCriteria).GetCriteriaOverrideString();
            var itemAnalysisData = new DataSet();
            levelValue.Value = level;
            levelIDValue.Value = selectedLevelID.ToString();

            switch (AnalysisType)
            {
                case Reports.AnalysisType.ItemAnalysis:
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
                                                                                                    "@@Product=none@@@@RR=none" + criteriaOverrides + "1test=yes@@@@PT=1@@@@FormID=" + FormID.ToString() + "@@",
                                                                                                    FormID.ToString());

                    itemAnalysisData.Tables.Add(Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(itemAnalaysisDataSet.Tables[0].Copy(),
                                                                                    "Question", "xID").Copy());
                    itemAnalysisData.Tables.Add(itemAnalaysisDataSet.Tables[1].Copy());

                    break;

                case Reports.AnalysisType.StandardAnalysis:
                    var standardAnalaysisDataSet = Thinkgate.Base.Classes.Reports.GetStandardAnaylsisData(year, selectedTest,
                                                                                                level,
                                                                                                selectedLevelID, userPage,
                                                                        _permissions, "", "", "", 0, selectedClass,
                                                                       "@@Product=none@@@@RR=none" +
                                                                                                criteriaOverrides +
                                                                                                "1test=yes@@@@PT=1@@@@FormID=" + FormID.ToString() + "@@",
                                                                                                FormID.ToString(), "SS");




                    itemAnalysisData.Tables.Add(Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(standardAnalaysisDataSet.Tables[0].Copy(), "StandardID", "xID"));
                    itemAnalysisData.Tables.Add(standardAnalaysisDataSet.Tables[1].Copy());

                    break;
            }

            Session["ItemAnalysisData_" + AnalysisType + "_" + Guid] = itemAnalysisData;
            return itemAnalysisData;
        }

        private void LoadReport()
        {
            ReportDs = (DataSet)(Session["ItemAnalysisData_" + AnalysisType + "_" + Guid] ?? GetDataTable());

            if (ReportDs == null || ReportDs.Tables.Count < 2 || ReportDs.Tables[0].Rows.Count < 1) return;

            if (ReportDs.Tables[0].Rows.Count > 0) SetLabels(ReportDs.Tables[0].Rows[0]);

            //Get and Bind Top "Boxes" for each series
            var seriesRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                              select new
                              {
                                  chartItem = sRow["ChartItem"].ToString(),
                                  color = sRow["ChartColor"].ToString(),
                                  asOfDate = !String.IsNullOrEmpty(sRow["DateTimeCalculated"].ToString()) ? "As of " + sRow["DateTimeCalculated"].ToString() : ""
                              }).Distinct();

            chartItemRepeater.DataSource = seriesRows;
            chartItemRepeater.DataBind();

            if (ReportDs.Tables[0].Columns.Contains("FieldTest"))
            {
                var itemRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                                select new
                                {
                                    ChartGroup = sRow["ChartGroup"].ToString(),
                                    xID = sRow["xID"].ToString(),
                                    Sort = (AnalysisType == Thinkgate.Controls.Reports.AnalysisType.ItemAnalysis) ? sRow["ChartGroup"].ToString() : sRow["StandardNbr"].ToString(),
                                    FieldTest = sRow["FieldTest"].ToString()
                                }).Distinct();

                chartSeriesRepeater.DataSource = itemRows;
                chartSeriesRepeater.DataBind();
            }
            else
            {
                var itemRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                                select new
                                {
                                    ChartGroup = sRow["ChartGroup"].ToString(),
                                    xID = sRow["xID"].ToString(),
                                    Sort = (AnalysisType == Thinkgate.Controls.Reports.AnalysisType.ItemAnalysis) ? sRow["ChartGroup"].ToString() : sRow["StandardNbr"].ToString()
                                }).Distinct();

                chartSeriesRepeater.DataSource = itemRows;
                chartSeriesRepeater.DataBind();
            }

        }

        protected void chartSeriesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {

                var chartGroup = ((HtmlGenericControl)e.Item.FindControl("chartGroup")).InnerHtml;
                var itemID = ((HtmlGenericControl)e.Item.FindControl("itemIdentifier")).InnerHtml;

                var itemLabel = ((System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("phItemLabel"));
                var itemLabelCell = ((System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("phItemLabelCell"));

                Type dataObjectType = e.Item.DataItem.GetType();
                PropertyInfo dataObjectProperty = dataObjectType.GetProperty("FieldTest");
                if (dataObjectProperty != null)
                {
                    object dataObjectValue = dataObjectProperty.GetValue(e.Item.DataItem, null);
                    if (dataObjectValue != null && dataObjectValue.ToString() == "1") itemLabelCell.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#FFFF00");
                }

                // TFS 1123: Disable item preview link for Teacher and School Admin login if assessment content is locked
                ReportHelper reportHelper = new ReportHelper();
                reportHelper.UserRoles = SessionObject.LoggedInUser.Roles;
                if (_isContentLocked && reportHelper.DisableItemLinks())
                {
                    //itemLabel.Enabled = false;
                    itemLabel.Attributes["style"] = "font-weight: bold; disabled: true;";
                }
                else
                {
                    itemLabel.Style.Add(HtmlTextWriterStyle.Color, "#034AF3");
                    itemLabel.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                    itemLabel.Style.Add(HtmlTextWriterStyle.TextDecoration, "underline");

                    if (itemLabel != null)
                    {
                        var controlUrl = AnalysisType == Reports.AnalysisType.ItemAnalysis ? "../ControlHost/PreviewTestQuestion.aspx" : "../ControlHost/PreviewStandard.aspx";
                        itemLabel.Attributes.Add("onclick", "openItemPreview('" + controlUrl + "?xID=" + itemID + "'); return false;");
                    }
                }

                var dt = (DataSet)(Session["ItemAnalysisData_" + AnalysisType + "_" + Guid] ?? GetDataTable());

                var barRows = (from DataRow dRow in dt.Tables[0].Rows
                               select new
                               {
                                   chartAmount = 100 * DataIntegrity.ConvertToDouble(dRow["ChartAmt"]),
                                   formattedChartAmount = String.Format("{0:p}", DataIntegrity.ConvertToDouble(dRow["ChartAmt"])),
                                   color = dRow["ChartColor"].ToString(),
                                   chartGroup = dRow["ChartGroup"].ToString()
                               }).Where(t => t.chartGroup == chartGroup);

                ((Repeater)e.Item.FindControl("barLineRepeater")).DataSource = barRows;
                ((Repeater)e.Item.FindControl("barLineRepeater")).DataBind();
            }
        }

        private void SetLabels(DataRow row)
        {
            double possible = DataIntegrity.ConvertToDouble(row["Possible"]) == 0 ? 1 : DataIntegrity.ConvertToDouble(row["Possible"]);
            string testScoreType = row["TestScoreType"].ToString();
            if (testScoreType == "S")
            {
                lblHigh.Text = Math.Round(Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Highest"]), 2).ToString();
                lblLow.Text = Math.Round(Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Lowest"]), 2).ToString();
                lblMedian.Text = Math.Round(Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Median"]), 2).ToString();
                lblMean.Text = Math.Round(Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Mean"]), 2).ToString();
            }
            else
            {
                lblHigh.Text = Math.Round((Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Highest"]) * 100), 2).ToString() + '%';
                lblLow.Text = Math.Round((Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Lowest"]) * 100), 2).ToString() + '%';
                lblMedian.Text = Math.Round((Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Median"]) * 100), 2).ToString() + "%";
                lblMean.Text = Math.Round((Standpoint.Core.Utilities.DataIntegrity.ConvertToDouble(row["Mean"]) * 100), 2).ToString() + "%";
            }


            lblSchoolCount.Text = row["SchoolCount"].ToString();
            lblTeacherCount.Text = row["TeacherCount"].ToString();
            lblClassCount.Text = row["ClassCount"].ToString();
            lblStudentCount.Text = row["StudentCount"].ToString();
            lblClassStudentCount.Text = row.Table.Columns.Contains("ClassStudentCount") ? row["ClassStudentCount"].ToString() : "";
            lblClassStudentCount.Visible = row.Table.Columns.Contains("ClassStudentCount") && Request.QueryString["level"] == "Class";
            classStudentCountTitle.Visible = row.Table.Columns.Contains("ClassStudentCount") && Request.QueryString["level"] == "Class";
        }

        protected void btnSelectChartItem_Click(object sender, EventArgs e)
        {
            var chartItem = itemAnalysis_selectedChartItem.Value;
            if (chartItem != "")
            {
                radWindowDetail.ContentContainer.Controls.Clear();

                if (AnalysisType == Reports.AnalysisType.ItemAnalysis)
                {

                    var ctlDetailControl = (Reports.ItemAnalysisDetailV2)LoadControl("~/Controls/Reports/ItemAnalysisDetailV2.ascx");
                    //TODO: DHB - When we are convinced that they will not want to roll back to ItemAnalysisDetail.ascx, then rename V2 and get rid of unused ascx.
                    //Reports.ItemAnalysisDetail ctlDetailControl =
                    //    (Reports.ItemAnalysisDetail)LoadControl("~/Controls/Reports/ItemAnalysisDetail.ascx");

                    ctlDetailControl.ID = "ctlDetailControl" + AnalysisType.ToString();
                    ctlDetailControl.Guid = Guid;
                    ctlDetailControl.Tile = Tile;
                    ctlDetailControl.Tile.TileParms.AddParm("Level", chartItem);
                    ctlDetailControl.Tile.TileParms.AddParm("FormID", FormID);
                    radWindowDetail.ContentContainer.Controls.Add(ctlDetailControl);
                }
                else
                {
                    Reports.StandardAnalysisDetail ctlDetailControl =
                        (Reports.StandardAnalysisDetail)LoadControl("~/Controls/Reports/StandardAnalysisDetail.ascx");
                    ctlDetailControl.Guid = Guid;
                    ctlDetailControl.Tile = Tile;
                    ctlDetailControl.Tile.TileParms.AddParm("Level", chartItem);
                    ctlDetailControl.Tile.TileParms.AddParm("FormID", FormID);
                    radWindowDetail.ContentContainer.Controls.Add(ctlDetailControl);
                }

                radWindowDetail.Title = chartItem + " Details";

                radWindowDetail.VisibleOnPageLoad = true;
            }
        }

        protected void ctxForm_ItemClick(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {


            FormID = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(e.Value);
            RemoteReportReload(this, null);
            rcbItemAnalysisForms.Text = e.Text;

        }

        public override void ExportToExcel()
        {
            _isExcel = true;
            // Create the workbook
            _isExcel = true;
            // Create the workbook
            ExcelFile ef = new ExcelFile();
            ef.DefaultFontName = "Calibri";
            ExcelWorksheet ws = ef.Worksheets.Add("DataSheet");
            FormatItemAnalysis(ws);
            ef.Save(Response, "ExportData.xlsx");
        }

        public void FormatItemAnalysis(ExcelWorksheet ws)
        {
            ReportDs = (DataSet)(Session["ItemAnalysisData_" + AnalysisType + "_" + Guid] ?? GetDataTable());
            if (ReportDs == null || ReportDs.Tables.Count < 2) return;

            ws.PrintOptions.PrintGridlines = true;
            //Write second rows first so that we can calculate width of for the headers
            //var rowCount = 11;
            var rowCount = 2;
            var level = Request.QueryString["level"] ?? "Class";
            /*
            workbook.Worksheet(1).Cell(1, 1).RichText.AddText(AnalysisType == Reports.AnalysisType.ItemAnalysis ? "Item Analysis" : "Standard Analysis").SetBold(true).SetFontSize(12);
            workbook.Worksheet(1).Cell(1, 51).RichText.AddText(AppSettings.ApplicationName).SetBold(true).SetFontSize(12);

            workbook.Worksheet(1).Cell(2, 1).RichText.AddText("Test").SetBold(true);
            workbook.Worksheet(1).Cell(2, 11).Value = ReportDs.Tables[0].Rows[0]["TestName"].ToString();
            workbook.Worksheet(1).Cell(2, 1).RichText.AddText("School").SetBold(true);
            workbook.Worksheet(1).Cell(2, 51).Value = level == "School" && _reportCriteria != null
                                          ? ((Criteria)_reportCriteria).CriterionList.FindAll(
                                              r => r.Key.Contains(level) && r.Object != null).Select(
                                                  criterion => criterion.Object.ToString()).ToList().ToString()
                                          : ReportDs.Tables[0].Rows[0]["SchoolName"].ToString();
            workbook.Worksheet(1).Cell(2, 62).RichText.AddText("# Schools").SetBold(true);
            workbook.Worksheet(1).Cell(2, 88).Value = ReportDs.Tables[0].Rows[0]["SchoolCount"].ToString();
            workbook.Worksheet(1).Cell(2, 99).RichText.AddText("High").SetBold(true);
            workbook.Worksheet(1).Cell(2, 105).Value = ReportDs.Tables[0].Rows[0]["Highest"].ToString();
            */

            var seriesRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                              select new
                              {
                                  chartItem = sRow["ChartItem"].ToString(),
                                  color = sRow["ChartColor"].ToString(),
                                  asOfDate = !String.IsNullOrEmpty(sRow["DateTimeCalculated"].ToString()) ? "As of " + sRow["DateTimeCalculated"].ToString() : ""
                              }).Distinct();

            var cellPosition = 1;
            foreach (var row in seriesRows)
            {
                ws.Cells[0, cellPosition].Value = row.chartItem + Environment.NewLine + row.asOfDate;
                ws.Cells[0, cellPosition].Style.Font.Weight = ExcelFont.BoldWeight;
                ws.Cells[0, cellPosition].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                ws.Cells[0, cellPosition].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                ws.Cells[0, cellPosition].Style.WrapText = true;
                ws.Cells[0, cellPosition].Style.FillPattern.SetSolid(ColorTranslator.FromHtml(row.color));
                ws.Rows[0].Height = 1024;

                ws.Cells[0, cellPosition].Style.Borders.SetBorders(MultipleBorders.Left, Color.Black, LineStyle.Thin);
                for (var i = cellPosition; i < (cellPosition + 15); i++)
                {
                    ws.Cells[0, i].Style.Borders.SetBorders(MultipleBorders.Horizontal, Color.Black, LineStyle.Thin);
                }
                ws.Cells[0, cellPosition + 15].Style.Borders.SetBorders(MultipleBorders.Right, Color.Black, LineStyle.Thin);
                ws.Cells.GetSubrangeAbsolute(0, cellPosition, 0, cellPosition + 15).Merged = true;
                ws.Cells[0, cellPosition].Style.Locked = true;
                cellPosition = cellPosition + 18;
            }

            if (ReportDs.Tables[0].Columns.Contains("FieldTest"))
            {
                var itemRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                                select new
                                {
                                    ChartGroup = sRow["ChartGroup"].ToString(),
                                    xID = sRow["xID"].ToString(),
                                    Sort = (AnalysisType == Reports.AnalysisType.ItemAnalysis) ? sRow["ChartGroup"].ToString() : sRow["StandardNbr"].ToString(),
                                    FieldTest = sRow["FieldTest"].ToString()
                                }).Distinct();

                foreach (var row in itemRows)
                {
                    var levelRowCount = rowCount;
                    var barsRowCount = ReportDs.Tables[0].Select("ChartGroup = '" + row.ChartGroup + "'").Count();
                    var barsRowLoopCount = 0;

                    ws.Cells[rowCount, 0].Value = row.Sort;
                    ws.Cells[rowCount, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    ws.Cells[rowCount, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    ws.Cells[rowCount, 0].Style.Borders.SetBorders(MultipleBorders.Top, Color.Black, LineStyle.Thin);
                    ws.Cells[rowCount, 0].Style.Borders.SetBorders(MultipleBorders.Left, Color.Black, LineStyle.Thin);

                    if (row.FieldTest == "1") ws.Cells[rowCount, 0].Style.FillPattern.SetSolid(Color.Yellow);

                    foreach (DataRow barsRow in ReportDs.Tables[0].Select("ChartGroup = '" + row.ChartGroup + "'"))
                    {
                        var chartAmount = Math.Round(100 * DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]), 0);
                        var formattedChartAmount = String.Format("{0:p}", DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]));

                        if (row.FieldTest == "1") ws.Cells[levelRowCount, 0].Style.FillPattern.SetSolid(Color.Yellow);

                        ws.Cells[levelRowCount, 0].Style.Borders.SetBorders(MultipleBorders.Bottom, Color.Black, barsRowCount == barsRowLoopCount ? LineStyle.Thin : LineStyle.None);
                        ws.Cells[levelRowCount, 0].Style.Borders.SetBorders(MultipleBorders.Left, Color.Black, LineStyle.Thin);

                        for (var i = 1; i < 101; i++)
                        {
                            if (i <= chartAmount) ws.Cells[levelRowCount, i].Style.FillPattern.SetSolid(ColorTranslator.FromHtml(barsRow["ChartColor"].ToString()));
                            ws.Cells[levelRowCount, i].Style.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
                            ws.Cells[levelRowCount, i].Style.Borders.SetBorders(MultipleBorders.Left, Color.Black, i == 1 ? LineStyle.Thin : LineStyle.None);
                            ws.Cells[levelRowCount, i].Style.Borders.SetBorders(MultipleBorders.Right, Color.Black, i == 100 ? LineStyle.Thin : LineStyle.None);
                        }

                        ws.Cells[levelRowCount, 101].Value = formattedChartAmount;
                        ws.Cells[levelRowCount, 101].Style.Font.Weight = ExcelFont.BoldWeight;
                        ws.Cells[levelRowCount, 101].Style.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
                        levelRowCount++;
                        barsRowLoopCount++;
                    }

                    rowCount = levelRowCount + 1;
                }

                ws.Columns[0].Width = AnalysisType == Reports.AnalysisType.ItemAnalysis ? 2 : 15;

                for (var x = 1; x < 101; x++)
                {
                    ws.Columns[x].Width = 256;
                }
            }
            else
            {
                var itemRows = (from DataRow sRow in ReportDs.Tables[0].Rows
                                select new
                                {
                                    ChartGroup = sRow["ChartGroup"].ToString(),
                                    xID = sRow["xID"].ToString(),
                                    Sort = (AnalysisType == Reports.AnalysisType.ItemAnalysis) ? sRow["ChartGroup"].ToString() : sRow["StandardNbr"].ToString()
                                }).Distinct();

                foreach (var row in itemRows)
                {
                    var levelRowCount = rowCount;
                    var barsRowCount = ReportDs.Tables[0].Select("ChartGroup = '" + row.ChartGroup + "'").Count();
                    var barsRowLoopCount = 0;

                    ws.Cells[rowCount, 0].Value = row.Sort;
                    ws.Cells[rowCount, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    ws.Cells[rowCount, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    ws.Cells[rowCount, 0].Style.Borders.SetBorders(MultipleBorders.Top, Color.Black, LineStyle.Thin);
                    ws.Cells[rowCount, 0].Style.Borders.SetBorders(MultipleBorders.Left, Color.Black, LineStyle.Thin);

                    foreach (DataRow barsRow in ReportDs.Tables[0].Select("ChartGroup = '" + row.ChartGroup + "'"))
                    {
                        var chartAmount = Math.Round(100 * DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]), 0);
                        var formattedChartAmount = String.Format("{0:p}", DataIntegrity.ConvertToDouble(barsRow["ChartAmt"]));

                        ws.Cells[levelRowCount, 0].Style.Borders.SetBorders(MultipleBorders.Bottom, Color.Black, barsRowCount == barsRowLoopCount ? LineStyle.Thin : LineStyle.None);
                        ws.Cells[levelRowCount, 0].Style.Borders.SetBorders(MultipleBorders.Left, Color.Black, LineStyle.Thin);

                        for (var i = 1; i < 101; i++)
                        {
                            if (i <= chartAmount) ws.Cells[levelRowCount, i].Style.FillPattern.SetSolid(ColorTranslator.FromHtml(barsRow["ChartColor"].ToString()));
                            ws.Cells[levelRowCount, i].Style.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
                            ws.Cells[levelRowCount, i].Style.Borders.SetBorders(MultipleBorders.Left, Color.Black, i == 1 ? LineStyle.Thin : LineStyle.None);
                            ws.Cells[levelRowCount, i].Style.Borders.SetBorders(MultipleBorders.Right, Color.Black, i == 100 ? LineStyle.Thin : LineStyle.None);
                        }

                        ws.Cells[levelRowCount, 101].Value = formattedChartAmount;
                        ws.Cells[levelRowCount, 101].Style.Font.Weight = ExcelFont.BoldWeight;
                        ws.Cells[levelRowCount, 101].Style.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
                        levelRowCount++;
                        barsRowLoopCount++;
                    }

                    rowCount = levelRowCount + 1;
                }

                ws.Columns[1].Width = AnalysisType == Reports.AnalysisType.ItemAnalysis ? 2 : 15;

                for (var x = 1; x < 101; x++)
                {
                    ws.Columns[x].Width = 256;
                }
            }
        }
    }
}