using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Web.Script.Serialization;
using ClosedXML.Excel;
using System.IO;
using Thinkgate.Controls.E3Criteria;
using System.Drawing;

namespace Thinkgate.Controls.Reports
{
    public partial class ProficiencyReportPageV2 : BasePage
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

        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += SearchHandler;
            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _option = Request.QueryString["Archive"];

            // Create the initial viewstate values.
            if (ViewState[_gridDisplayFilterKey] == null)
            {
                ViewState.Add(_gridDisplayFilterKey, "Level");
                ViewState.Add(_domainFilterKey, "All");
            }

            if (!IsPostBack)
            {
                LoadGridDisplayOptions();
                LoadDomains();
                //LoadReport();
                var serializer = new JavaScriptSerializer();
                var data = Thinkgate.Base.Classes.Reports.GetCriteriasForStateReporting();
                var schoolsForLoggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
                
                ctrlTestYearGradeSubject.JsonDataSource = serializer.Serialize(TestYearGradeSubject.BuildJsonArray(data));
                //schoolsForLoggedInUser = schoolsForLoggedInUser.FindAll(s => s.Grades.IndexOf("03") > -1);
                cmbSchool.JsonDataSource = serializer.Serialize(BuildSchoolGradeJsonArray(schoolsForLoggedInUser));
                cmbSchool.OnClientLoad = "InitialLoadOfSchoolList";         // I've decided here to not put the school in with TestYeraGradeSubject control. Wanted to investigate linking an external control. These two lines are how I've gone about setting that dependency up
                cmbSchool.OnChange = "OnSchoolChange(comboBox);";
                ctrlTestYearGradeSubject.OnChange = "ChangeSchoolBasedOnGrade();";
                //cmbTeacher.WebServiceSettings = new WebServiceSettings { Path = "~/Services/StandardsAJAX.svc", Method = "LoadTeachers" };
                cmbTeacher.OnClientLoad = "OnSchoolChange";
                cmbTeacher.ComboBox.WebServiceSettings.Path = "~/Services/StandardsAJAX.svc";
                cmbTeacher.ComboBox.WebServiceSettings.Method = "LoadTeachers";
                cmbTeacher.ComboBox.EnableLoadOnDemand = true;
                cmbTeacher.ComboBox.EnableVirtualScrolling = true;
                cmbTeacher.ComboBox.ShowMoreResultsBox = true;
                cmbTeacher.ComboBox.OnClientItemsRequesting = "OnClientItemsRequesting_Teachers";
                cmbTeacher.ComboBox.OnClientItemsRequested = "OnClientItemsRequested_Teachers";
                cmbTeacher.ToolTip.OnClientBeforeHide = "TeacherTooltipHide";
                cmbTeacher.ComboBox.OnClientDropDownClosed = "OnClientDropDownClosed_Teachers";
                cmbTeacher.ComboBox.MaxHeight = 200;
                //cmbTeacher.JsonDataSource = serializer.Serialize(BuildSchoolTeacherJsonArray());
                //cmbTeacher.OnClientLoad = "InitialLoadOfTeacherList"; 
                
                var _dp = DistrictParms.LoadDistrictParms();
                btnFLDOE.Visible = _dp.FCAT_FLDOE;
            }
        }

        /*public ArrayList BuildSchoolTeacherJsonArray()
        {
            var dtTeachers = Thinkgate.Base.Classes.Teacher.GetTeachersForReportingDropdown();
            var arry = new ArrayList();
            string lastID = "";
            ArrayList currSchoolList = new ArrayList();
  
            foreach (DataRow row in dtTeachers.Rows)
            {
                string thisId = row["ID"].ToString();
                if (thisId != lastID)
                {
                    currSchoolList = new ArrayList {DataIntegrity.ConvertToInt(row["SchoolID"])};
                    arry.Add(new object[]
                                 {
                                     row["ID"], row["TeacherName"], currSchoolList
                                 });
                    
                } else
                {
                    currSchoolList.Add(DataIntegrity.ConvertToInt(row["SchoolID"]));
                }
                lastID = thisId;
            }
            return arry;
        }*/
        protected void RemoteReportReload(object sender, EventArgs e)
        {
            LoadReport();
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
        
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            LoadReport();
        }

        public DataSet GetDataSet()
        {
            gridResults.Visible = true;
            cmbGridDisplay.Visible = true;
            lblInitialText.Visible = false;

            var criteriaController = Master.CurrentCriteria();      // just going to go ahead and pull this from master instead of from search handler so it will work on tree updates

            //Assessment            
            string selectedTestType = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("AssessmentType").Select(x => x.Text).FirstOrDefault();

            //Year
            string selectedYear = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Year").Select(x => x.Text).FirstOrDefault();
            if (String.IsNullOrEmpty(selectedYear)) selectedYear = DistrictParms.LoadDistrictParms().Year;
            
            //Grade
            string selectedGrade = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Grade").Select(x => x.Text).FirstOrDefault();

            //Subject
            string selectedSubject = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Subject").Select(x => x.Text).FirstOrDefault();

            //School
            int selectedSchoolId = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("School").Select(x => x.Value).FirstOrDefault());
            
            //Class
            //int classID = DataIntegrity.ConvertToInt(criteria.FindCriterionHeaderByText("Class").ReportStringVal);

            //Teacher
            int selectedTeacherId = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Teacher").Select(x => x.Value).FirstOrDefault());

            var criteriaStr = "@@RR=none@@TEAID=" + (selectedTeacherId == 0 ? "" : selectedTeacherId.ToString())
                //+ "@@CID=" + (classID == 0 ? "" : classID.ToString()) 
                            + "@@S=" + (selectedSubject == "Reading" ? "Language Arts" : selectedSubject)
                            + "@@G=" + selectedGrade
                            + "@@SCH=" + (selectedSchoolId == 0 ? "" : selectedSchoolId.ToString())
                            + "@@@@TYRS="
                            + selectedYear + "@@TTERMS=All@@TTYPES="
                            + selectedTestType + "@@";

            return Thinkgate.Base.Classes.Reports.GetProficiencyReport(0, selectedYear, "normal", criteriaStr, _option);
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
            // Create the workbook
            XLWorkbook workbook = Master.ConvertDataTableToSingleSheetWorkBook(dt, "Results");

            //Prepare the response

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");

                // Flush the workbook to the Response.OutputStream

                Response.BinaryWrite(memoryStream.ToArray());

                Response.End();
            }
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
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

        public ArrayList BuildSchoolGradeJsonArray(List<Base.Classes.School> schools)
        {
            var arry = new ArrayList();
            foreach (var s in schools)
            {
                arry.Add(new object[]
                                 {
                                     s.Name, s.ID, s.Grades.Replace(" ", "").Split(',')
                                 });
            }
            return arry;
        }
    }
}
