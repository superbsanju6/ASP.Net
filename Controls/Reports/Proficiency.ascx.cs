using System;
using System.Data;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Drawing;
using System.Web.UI;
using Telerik.Web.UI;
using ClosedXML.Excel;
using System.IO;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Reports
{
    public partial class Proficiency : TileControlBase
    {
        public string Guid;
        public int FormID;
        private int _addedColumns = 0;
        private string _displayOption { get { return ViewState[_gridDisplayFilterKey].ToString(); } }
        private string _option;

        protected String _gridDisplayFilterKey = "GridDisplayFilter";
        protected String _domainFilterKey = "DomainFilter";

        private DataTable _proficiencyData;
        private DataTable _demographicData;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _option = Tile.TileParms.GetParm("Archive") != null && (bool)Tile.TileParms.GetParm("Archive") ? "archive" : string.Empty;

            // Create the initial viewstate values.
            if (ViewState[_gridDisplayFilterKey] == null)
            {
                ViewState.Add(_gridDisplayFilterKey, "Level");
                ViewState.Add(_domainFilterKey, "All");
            }

            LoadCriteria();
            var _dp = DistrictParms.LoadDistrictParms();
            btnFLDOE.Visible = _dp.FCAT_FLDOE;
                
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "anything", BuildStartupScript(exportGridImgBtn.ClientID, "..", Guid), false);
            }

            if (!IsPostBack)
            {
                LoadGridDisplayOptions();
                LoadDomains();
                //LoadReport();
            }
        }

        protected void RemoteReportReload(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlProficiencyCriteria";

            if (Tile.TileParms.GetParm("CriteriaGUID") == null) return;
            Guid = (string)Tile.TileParms.GetParm("CriteriaGUID");

            if (Session["Criteria_" + Guid] == null) return;

            ctlReportCriteria.Guid = Guid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            criteriaPlaceholder.Controls.Add(ctlReportCriteria);

            ctlReportCriteria.HideCriterionSection("Demographics");
            ctlReportCriteria.HideCriterionSection("Class");
        }

        private void LoadReport()
        {
            var ds = GetDataSet();

            if (ds == null || ds.Tables.Count < 5) return;

            _proficiencyData = ds.Tables[0];
            _demographicData = ds.Tables[4];

            //Add Student Counts Row?
            DataRow allStudentRow = ds.Tables[3].NewRow();
            allStudentRow["RecType"] = 4;
            allStudentRow["LevelText"] = "Student Counts";
            allStudentRow["LevelIndex"] = 0;
            allStudentRow["LevelColor"] = "White";
            ds.Tables[3].Rows.InsertAt(allStudentRow, 0);

            AddColumnsToGrid(_demographicData);

            gridResults.DataSource = ds.Tables[3]; //Row Data
            gridResults.DataBind();

            if (ds.Tables[2].Rows.Count > 0)
            {
                var html = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
                html.InnerHtml = ds.Tables[2].Rows[0]["ChartTitle2"].ToString();
                pnlProficiencyLevels.Controls.Add(html);
            }
        }

        public DataSet GetDataSet()
        {
            gridResults.Visible = true;
            cmbGridDisplay.Visible = true;
            lblInitialText.Visible = false;

            var criteria = (Criteria)Session["Criteria_" + Guid];

            //Assessment
            string testType = criteria.FindCriterionHeaderByText("Assessment").ReportStringVal;
            
            //Year
            var yearCriterion = criteria.FindCriterionHeaderByText("Year");
            string year = (yearCriterion == null || string.IsNullOrEmpty(yearCriterion.ReportStringVal)) ? DistrictParms.LoadDistrictParms().Year : string.Join(",", yearCriterion.ReportStringVal);

            //Grade
            var gradeControl = criteria.FindCriterionHeaderByText("Grade");
            string grades = (gradeControl == null || string.IsNullOrEmpty(gradeControl.ReportStringVal)) ? "All" : string.Join(",", gradeControl.ReportStringVal);

            //Subject
            var subjectControl = criteria.FindCriterionHeaderByText("Subject");
            string subjects = (subjectControl == null || string.IsNullOrEmpty(subjectControl.ReportStringVal)) ? "All" : string.Join(",", subjectControl.ReportStringVal);

            //School
            int schoolID = DataIntegrity.ConvertToInt(criteria.FindCriterionHeaderByText("School").ReportStringVal);
            
            //Class
            //int classID = DataIntegrity.ConvertToInt(criteria.FindCriterionHeaderByText("Class").ReportStringVal);

            //Teacher
            int teacherID = DataIntegrity.ConvertToInt(criteria.FindCriterionHeaderByText("Teacher").ReportStringVal);

            var criteriaStr = "@@RR=none@@TEAID=" + (teacherID == 0 ? "" : teacherID.ToString())
                            //+ "@@CID=" + (classID == 0 ? "" : classID.ToString()) 
                            + "@@S=" + subjects 
                            + "@@G=" + grades
                            + "@@SCH=" + (schoolID == 0 ? "" : schoolID.ToString()) 
                            + "@@@@TYRS=" 
                            + year + "@@TTERMS=All@@TTYPES=" 
                            + testType + "@@";

            return Thinkgate.Base.Classes.Reports.GetProficiencyReport(0, year, "normal", criteriaStr, _option);
        }

        private void AddColumnsToGrid(DataTable columnsDT)
        {
            gridResults.Columns.Clear();

            //Level Column
            var levelColumn = new GridBoundColumn();
            gridResults.Columns.Add(levelColumn);
            levelColumn.DataField = "LevelText";
            levelColumn.HeaderText = "Level";
            levelColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;

            //Label Column
            var labelColumn = new GridBoundColumn();
            gridResults.Columns.Add(labelColumn);
            labelColumn.HeaderText = "";
            labelColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;

            switch (_displayOption)
            {
                case "Level":
                    labelColumn.DataFormatString = "Counts<br/>Distribution%";
                    break;

                case "Score":
                    labelColumn.DataFormatString = "Counts<br/>Scores";
                    break;

                case "All":
                    labelColumn.DataFormatString = "Counts<br/>Distribution%<br/>Scores";
                    break;
            }

            //Column for each demographic
            foreach (DataRow r in columnsDT.Rows)
            {
                var column = new GridNumericColumn();
                gridResults.Columns.Add(column);
                column.HeaderText = r["DemoLabel"].ToString();

                _addedColumns++;
            }
        }

        protected void gridResults_DataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var dataItem = (DataRowView)((GridItem)e.Item).DataItem;
                var cellIndex = e.Item.Cells.Count - _addedColumns;

                if (cellIndex < 0) return;

                var levelIndex = dataItem["LevelIndex"].ToString();
                var recType = DataIntegrity.ConvertToInt(dataItem["RecType"]);
                var levelColor = dataItem["LevelColor"].ToString();

                for (var i = 0; i < e.Item.Cells.Count; i++)
                {
                    e.Item.Cells[i].BackColor = Color.FromName(levelColor);
                }

                string cellText = "";

                foreach (DataRow r in _demographicData.Rows)
                {
                    //Find row for the correct DemoLabel
                    var rows = _proficiencyData.Select("DemoLabel='" + r["DemoLabel"] + "'");
                    if (rows == null || rows.Length == 0)
                    {
                        e.Item.Cells[cellIndex].Text = "&nbsp;";
                        cellIndex++;
                        continue;
                    }

                    var row = rows[0];

                    switch (recType)
                    {
                        case 0: //A particular Level   
                            cellText = SafeGridValue(row["PLCNT" + levelIndex].ToString()) + "<br/>";

                            if (_displayOption == "Level" || _displayOption == "All") cellText += SafeGridValue(row["PLDIS" + levelIndex].ToString()) + "<br/>";
                            if (_displayOption == "Score" || _displayOption == "All") cellText += SafeGridValue(row["PLAVG" + levelIndex].ToString());

                            break;

                        case 1: //Proficient row
                            cellText = SafeGridValue(row["PLCNT0"].ToString()) + "<br/>";

                            if (_displayOption == "Level" || _displayOption == "All") cellText += SafeGridValue(row["PLDIS0"].ToString()) + "<br/>";
                            if (_displayOption == "Score" || _displayOption == "All") cellText += SafeGridValue(row["PLAVG0"].ToString());
                            break;

                        case 2: //% Learning Gain
                            cellText = SafeGridValue(row["ALT1CNT"].ToString()) + "<br/>";

                            if (_displayOption == "Level" || _displayOption == "All") cellText += SafeGridValue(row["ALT1DIS"].ToString()) + "<br/>";
                            if (_displayOption == "Score" || _displayOption == "All") cellText += SafeGridValue(row["ALT1AVG"].ToString());
                            break;

                        case 3: //% At Expectation
                            cellText = SafeGridValue(row["ALT2CNT"].ToString()) + "<br/>";

                            if (_displayOption == "Level" || _displayOption == "All") cellText += SafeGridValue(row["ALT2DIS"].ToString()) + "<br/>";
                            if (_displayOption == "Score" || _displayOption == "All") cellText += SafeGridValue(row["ALT2AVG"].ToString());
                            break;

                        case 4: //All Students - always displays count and score
                            cellText = SafeGridValue(row["PLCNTALL"].ToString()) + "<br/>";

                            if (_displayOption == "Level" || _displayOption == "All") cellText += "-<br/>"; //don't have distribution for All Students
                            if (_displayOption == "Score" || _displayOption == "All") cellText += SafeGridValue(row["PLAVGALL"].ToString());
                            break;
                    }

                    e.Item.Cells[cellIndex].Text = cellText;

                    cellIndex++;
                }
            }
        }

        private string SafeGridValue(string value)
        {
            if (String.IsNullOrEmpty(value)) return "&nbsp;";
            return value;
        }

        private void LoadGridDisplayOptions()
        {
            // Initialize the current selection.
            RadComboBoxItem item = cmbGridDisplay.Items.FindItemByValue((String)ViewState[_gridDisplayFilterKey], true);
            Int32 selIdx = cmbGridDisplay.Items.IndexOf(item);
            cmbGridDisplay.SelectedIndex = selIdx;
        }

        private void LoadDomains()
        {
            //TODO: Load Domains Data adding an all option


            // Initialize the current selection.
            RadComboBoxItem item = cmbDomain.Items.FindItemByValue((String)ViewState[_domainFilterKey], true);
            Int32 selIdx = cmbDomain.Items.IndexOf(item);
            cmbDomain.SelectedIndex = selIdx;
        }

        protected void cmbGridDisplay_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_gridDisplayFilterKey] = e.Value;
            LoadReport();
        }

        protected void cmbDomain_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_domainFilterKey] = e.Value;
            LoadReport();
        }

        public void ExportToExcel(DataTable dt)
        {
            XLWorkbook workbook = ConvertDataTableToSingleSheetWorkBook(dt, "Results");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                Session["FileExport_Content" + Guid] = memoryStream.ToArray();
            }
        }

        protected void ExportGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            LoadReport();
            ExportToExcel(BuildDataTableForExport());
        }

        private DataTable BuildDataTableForExport()
        {
            var ds = GetDataSet();
            if (ds == null || ds.Tables.Count < 5) return null;

            DataRow allStudentRow = ds.Tables[3].NewRow();
            allStudentRow["RecType"] = 4;
            allStudentRow["LevelText"] = "Student Counts";
            allStudentRow["LevelIndex"] = 0;
            allStudentRow["LevelColor"] = "White";
            ds.Tables[3].Rows.InsertAt(allStudentRow, 0);

            var dt = new DataTable();
            dt.Columns.Add("LevelText");

            //Column for each demographic
            foreach (DataRow r in ds.Tables[4].Rows)
            {                
                dt.Columns.Add(r["DemoLabel"].ToString() + ": Counts");
                dt.Columns.Add(r["DemoLabel"].ToString() + ": Distribution%");
                dt.Columns.Add(r["DemoLabel"].ToString() + ": Scores");
            }

            //Now Add Rows
            foreach (DataRow row in ds.Tables[3].Rows)
            {
                var dr = dt.NewRow();

                var recType = DataIntegrity.ConvertToInt(row["RecType"]);
                var levelIndex = row["LevelIndex"].ToString();
                var levelColor = row["LevelColor"].ToString();

                dr["LevelText"] = row["LevelText"];

                foreach (DataRow r in ds.Tables[4].Rows)
                {
                    var matchRows = ds.Tables[0].Select("DemoLabel='" + r["DemoLabel"] + "'");
                    if (matchRows == null || matchRows.Length == 0)
                    {
                        continue;
                    }
                    var matchRow = matchRows[0];

                    switch (recType)
                    {
                        case 0: //A particular Level   
                            dr[r["DemoLabel"].ToString() + ": Counts"] = matchRow["PLCNT" + levelIndex].ToString();
                            dr[r["DemoLabel"].ToString() + ": Distribution%"] = matchRow["PLDIS" + levelIndex].ToString();
                            dr[r["DemoLabel"].ToString() + ": Scores"] = matchRow["PLAVG" + levelIndex].ToString();
                            break;
                        case 1: //Proficient row
                            dr[r["DemoLabel"].ToString() + ": Counts"] = matchRow["PLCNT0"].ToString();
                            dr[r["DemoLabel"].ToString() + ": Distribution%"] = matchRow["PLDIS0"].ToString();
                            dr[r["DemoLabel"].ToString() + ": Scores"] = matchRow["PLAVG0"].ToString();
                            break;
                        case 2: //% Learning Gain
                            dr[r["DemoLabel"].ToString() + ": Counts"] = matchRow["ALT1CNT"].ToString();
                            dr[r["DemoLabel"].ToString() + ": Distribution%"] = matchRow["ALT1DIS"].ToString();
                            dr[r["DemoLabel"].ToString() + ": Scores"] = matchRow["ALT1AVG"].ToString();
                            break;
                        case 3: //% At Expectation
                            dr[r["DemoLabel"].ToString() + ": Counts"] = matchRow["ALT2CNT"].ToString();
                            dr[r["DemoLabel"].ToString() + ": Distribution%"] = matchRow["ALT2DIS"].ToString();
                            dr[r["DemoLabel"].ToString() + ": Scores"] = matchRow["ALT2AVG"].ToString();
                            break;
                        case 4: //All Students - always displays count and score
                            dr[r["DemoLabel"].ToString() + ": Counts"] = matchRow["PLCNTALL"].ToString();
                            dr[r["DemoLabel"].ToString() + ": Distribution%"] = "";
                            dr[r["DemoLabel"].ToString() + ": Scores"] = matchRow["PLAVGALL"].ToString();
                            break;
                    }
                }


                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}