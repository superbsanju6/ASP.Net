using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using CMS.GlobalHelper;
using Standpoint.Core.ExtensionMethods;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Domain.Classes;
using Thinkgate.Base.DataAccess;
using System.Web.UI.HtmlControls;
using System.Text;



namespace Thinkgate.Controls.Reports
{
    public enum ViewBy
    {
        District, School, Teacher, Class, Group, Student, Demographics, Alignment, Year
    }

    public partial class CredentialTrackingReportPage : BasePage
    {
        public string PageGuid;
        public int FormID;
        private readonly List<string> _excelIgnoredColumns = new List<string>();

        private static string _loggedOnUserRoleName = string.Empty;
        private static ThinkgateUser _loggedOnUser = null;
        private EnvironmentParametersViewModel _enviromentParameter;

        private string _currentViewByValue = string.Empty;
        private static readonly string DefaultStandardSets = DistrictParms.LoadDistrictParms().AddRemove_Competency_DefaultStandardSetList;
        public DataTable GridDataTable;
        public int DataTableCount;
        private int _distinctRowsCount;
        private int _distinctObjectivesCount;

        public string _viewBy;

        private const string CredentialTrackingReport = "Credential Tracking Report";
        private const string CredentialTrackingReport_NoSpaces = "CredentialExport";

        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += SearchHandler;
            base.Page_Init(sender, e);
            _currentViewByValue = string.Empty;
            _enviromentParameter = new EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            _loggedOnUserRoleName = SessionObject.LoggedInUser.Roles[0].RoleName;
            _loggedOnUser = SessionObject.LoggedInUser;

            _excelIgnoredColumns.Add("EncryptedID");
            _excelIgnoredColumns.Add("WorksheetId");
            _excelIgnoredColumns.Add("StandardId");
            _excelIgnoredColumns.Add("Included");
            _excelIgnoredColumns.Add("Total");
            _excelIgnoredColumns.Add("Excluded");
            _excelIgnoredColumns.Add("StandardLevel");
            _excelIgnoredColumns.Add("ScoreColumnA");
            _excelIgnoredColumns.Add("ScoreColumnB");
            _excelIgnoredColumns.Add("ScoreColumnC");
            _excelIgnoredColumns.Add("ScoreColumnD");
            _excelIgnoredColumns.Add("ScoreColumnE");

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateViewByDropdown();
            PopulateSchoolDropdown();
            PopulateGroupsDropdown();
            if (SessionObject.IsAlignmentDataAvailable == null)
            {
                SessionObject.IsAlignmentDataAvailable = Base.Classes.Credentials.IsAlignmentDataAvailable();
            }

            if (SessionObject.IsAlignmentDataAvailable != true)
            {
                cmbalignment.Visible = false;
            }
            else
            {
                cmbalignment.Visible = true;
                PopulateAlignmentDropdown();
            }
            PopulateYearDropdown();
        }

        private void PopulateSchoolDropdown()
        {
            List<Base.Classes.School> schoolsList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);

            cmbSchool.DataTextField = "Name";
            cmbSchool.DataValueField = "Id";
            cmbSchool.DataSource = schoolsList;

            if (schoolsList.Count == 1)
            {
                cmbSchool.DefaultTexts = PossibleDefaultTexts(schoolsList[0].Name);
            }
        }

        private List<String> PossibleDefaultTexts(object input)
        {
            if (input == null) return null;
            var list = new List<String>();
            list.AddRange(input.ToString().Split(','));
            return list;
        }

        private void PopulateAlignmentDropdown()
        {
            if (SessionObject.LoggedInUser.Schools != null && SessionObject.LoggedInUser.Schools.Count > 0)
            {
                cmbalignment.DataTextField = "Name";
                cmbalignment.DataValueField = "Id";

                cmbalignment.DataSource = SessionObject.LoggedInUser.Schools;
            }
        }

        private void PopulateYearDropdown()
        {
            DataTable DtYearList = Base.Classes.Credentials.GetYearList();
            cmbyear.DataSource = DtYearList;
            cmbyear.DataTextField = "YEAR_LIST";
            cmbyear.DataValueField = "YEAR_LIST";
            cmbyear.DataBind();

        }

        private DataTable getListOfWorksheets(string teacherName)
        {
            DataTable cvteReportByStudent = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_GetWorksheetList",
                    Connection = conn
                };
                cmd.Parameters.Add(teacherName != null
                    ? new SqlParameter { ParameterName = "TeacherName", Value = teacherName }
                    : new SqlParameter { ParameterName = "TeacherName", Value = DBNull.Value });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(cvteReportByStudent);
            }
            return cvteReportByStudent;
        }

        private void PopulateGroupsDropdown()
        {
           
            DataTable DtGroupList = Base.Classes.Credentials.GetUserGroupList(SessionObject.LoggedInUser.Page);
            if (DtGroupList != null && DtGroupList.Rows.Count > 0)
            {
                cmbGroup.DataSource = DtGroupList;
                cmbGroup.DataTextField = "Name";
                cmbGroup.DataValueField = "ID";
                cmbGroup.DataBind();
            }
            else
            {
                cmbGroup.EmptyMessage = "No groups available";
            }
        }

        private void PopulateViewByDropdown()
        {
            var theList = new List<KeyValuePair<string, string>>();

            // Comment out the following so that only "Student" shows for now
            if (IsDistrictUser())
            {
                theList.Add(new KeyValuePair<string, string>("District", "District"));
                theList.Add(new KeyValuePair<string, string>("School", "School"));
            }
            if (IsSchoolUser())
            {
                theList.Add(new KeyValuePair<string, string>("School", "School"));
            }

            theList.Add(new KeyValuePair<string, string>("Teacher", "Teacher"));
            theList.Add(new KeyValuePair<string, string>("Class", "Class"));
            theList.Add(new KeyValuePair<string, string>("Group", "Group"));
            theList.Add(new KeyValuePair<string, string>("Student", "Student"));
            theList.Add(new KeyValuePair<string, string>("Demographics", "Demographics"));

            cmbViewBy.DataTextField = "Key";
            cmbViewBy.DataValueField = "Value";
            cmbViewBy.DataSource = theList;
        }

        private static bool IsSchoolUser()
        {
            bool isSchoolUser =
                _loggedOnUserRoleName.Equals("School Administrator", StringComparison.InvariantCultureIgnoreCase)
                || _loggedOnUserRoleName.Equals("School Support", StringComparison.InvariantCultureIgnoreCase);
            return isSchoolUser;
        }

        private static bool IsDistrictUser()
        {
            bool isDistrictUser =
                _loggedOnUserRoleName.Equals("District Administrator", StringComparison.InvariantCultureIgnoreCase)
                || _loggedOnUserRoleName.Equals("District Support", StringComparison.InvariantCultureIgnoreCase);
            return isDistrictUser;
        }

        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            BindDataToGrid();
        }

        private DataTable GetClassesForTeacher(int schoolID, string teacherName)
        {
            var classCourseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
            List<Int32> schoolIDs = new List<int>();
            schoolIDs.Add(schoolID);

            return Base.Classes.Class.SearchClasses(classCourseList, null, schoolIDs, "", teacherName);
        }

        protected void RadGridResults_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            BindDataToGrid();

        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }

        private SelectedCriteriaCrd GetCriteriaControlValues()
        {
            var criteriaController = Master.CurrentCriteria();

            SelectedCriteriaCrd selectedCriteria = new SelectedCriteriaCrd();

            //ViewBy
            selectedCriteria.SelectedViewBy = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ViewBy").Select(x => x.Text).FirstOrDefault();
            //School
            selectedCriteria.SelectedSchoolId = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("School").Select(x => x.Value).FirstOrDefault());
            selectedCriteria.SelectedSchoolName = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("School").Select(x => x.Text).FirstOrDefault();
            //Teacher
            selectedCriteria.SelectedTeacher = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Teacher").Select(x => x.Value).FirstOrDefault();
            //Class
            selectedCriteria.SelectedClass = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Class").Select(x => x.Value).FirstOrDefault();
            //Student
            selectedCriteria.SelectedStudent = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Student").Select(x => x.Value).FirstOrDefault();
            //Demographics
            selectedCriteria.SelectedDemographics = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics");
            //Group;
            selectedCriteria.SelectedGroup = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Group").Select(x => x.Value).FirstOrDefault();
            //Alignment
            selectedCriteria.SelectedAlignment = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Alignment").Select(x => x.Value).FirstOrDefault();
            //Year
            selectedCriteria.SelectedYear = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Year").Select(x => x.Value).FirstOrDefault();
            return selectedCriteria;
        }

        public void BindDataToGrid()
        {
            //radGridResults.MasterTableView.Columns.Clear();
            //divDefaultMessage.Visible = false;
            radGridResults.MasterTableView.DataSource = new string[0];
            radGridResults.Visible = true;
            DivBlackMsg.Visible = false;
            lblInitialText.Visible = false;
            initialDisplayText.Visible = false;

            SelectedCriteriaCrd selectedCriteria = GetCriteriaControlValues();

            //ViewBy
            string selectedViewBy = selectedCriteria.SelectedViewBy;
            _viewBy = selectedViewBy;
            //School
            int selectedSchoolID = selectedCriteria.SelectedSchoolId;

            //Teacher
            //The code below forces passing the current teacher to the backend procs, this way a teacher can ONLY see their data.
            int selectedTeacherID = 0;
            if (_loggedOnUserRoleName != null
                && _loggedOnUserRoleName.Equals("Teacher", StringComparison.InvariantCultureIgnoreCase)
                && (_loggedOnUser != null && !string.IsNullOrEmpty(_loggedOnUser.Page.ToString())))
            {
                selectedTeacherID = _loggedOnUser.Page; // force current user
            }
            else
            {
                selectedTeacherID = Convert.ToInt32(selectedCriteria.SelectedTeacher);
            }

            //Class
            int selectedClassID = Convert.ToInt32(selectedCriteria.SelectedClass);
            //Student
            int selectedStudentID = Convert.ToInt32(selectedCriteria.SelectedStudent);

            //Demographics
            List<Demographics.ValueObject> selectedDemographics = selectedCriteria.SelectedDemographics;
            //Group;
            int selectedGroupID = Convert.ToInt32(selectedCriteria.SelectedGroup);

            string selectedYear = selectedCriteria.SelectedYear;

            string selectedAlignments = selectedCriteria.SelectedAlignment;

            //GET THE DATA HERE
            GridDataTable = new DataTable();

            if (selectedViewBy != null)
            {
                GridDataTable = GetCredentialReport(selectedViewBy, selectedSchoolID, selectedTeacherID, selectedClassID, selectedDemographics, selectedGroupID, selectedStudentID, selectedAlignments, selectedYear);
            }
            if (GridDataTable.Rows.Count > 0)
            {
                DataView view = new DataView(GridDataTable);
                radGridResults.DataSource = GridDataTable;
                radGridResults.Visible = true;
                DivBlackMsg.Visible = false;
            }
            else
            {
                radGridResults.Visible = false;
                DivBlackMsg.Visible = true;
            }

            //_viewBy = GridDataTable.AsEnumerable().Select(r => r.Field<string>("LevelName")).Distinct().Count().ToString();
            FormatColumnsForExcel(selectedCriteria);
            radGridResults.DataBind();
            initialDisplayText.Visible = false;
        }

        private void FormatColumnsForExcel(SelectedCriteriaCrd selectedCriteria)
        {
            string selectedViewBy = selectedCriteria.SelectedViewBy;
            DataTable localDataTable = GridDataTable.Copy();

            switch (selectedViewBy)
            {
                case "District":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns.Remove("LevelName");
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";

                    break;
                case "School":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "School";
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";
                    break;
                case "Teacher":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "Teacher";
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";
                    break;
                case "Class":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "Class";
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";
                    break;
                case "Group":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "Group";
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";
                    break;
                case "Student":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "Student";
                    localDataTable.Columns.Remove("CredentialID");

                    // Need to modify for yes/no value.
                    //localDataTable.Columns["EarnedDate"].DataType = typeof(String);
                    DataColumn EarnedCol = localDataTable.Columns.Add("Earned", typeof(String));
                    foreach (DataRow dr in localDataTable.Rows)
                    {
                        string date = dr["EarnedDate"].ToString().Trim();
                        dr["Earned"] = String.IsNullOrEmpty(date) ? "No" : "Yes";
                    }
                    
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns.Remove("StudentCount");

                    break;
                case "Demographics":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "Demographics";
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";
                    break;
                case "Alignment":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "Alignment";
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";
                    break;

                case "Year":
                    localDataTable.Columns.Remove("LevelID");
                    localDataTable.Columns["LevelName"].ColumnName = "Year";
                    localDataTable.Columns.Remove("CredentialID");
                    localDataTable.Columns.Remove("EarnedDate");
                    localDataTable.Columns["CredentialName"].ColumnName = "Credential Name";
                    localDataTable.Columns["StudentCount"].ColumnName = "Student Count";
                    break;

            }

            ReportDataCredential theReportData = new ReportDataCredential
            {
                ReportDataTable = localDataTable,
                ReportSelectedCriteria = selectedCriteria
            };
            SessionObject.CredentialTracking_ReportData = theReportData;
        }

        public static DataTable GetCredentialReport(string selectedViewBy, int selectedSchoolID, int selectedTeacherId, int selectedClassID,
            List<Demographics.ValueObject> selectedDemographics, int selectedGroupID, int selectedStudentID, string selectedAlignments, string selectedYear = null, int credentialID = 0, int levelID = 0)
        {
            DataTable credentialReport = new DataTable();

            drGeneric_Int drAlignments = new drGeneric_Int();
            if (!string.IsNullOrEmpty(selectedAlignments))
            {
                string[] arAlignments = (selectedAlignments).Split(',');
                int alignment = 0;
                foreach (string strAlign in arAlignments)
                    if (Int32.TryParse(strAlign, out alignment))
                        drAlignments.Add(alignment);
            }

            var objparams = new object[] { 
                    selectedViewBy ?? "", 
                    selectedSchoolID, 
                    selectedTeacherId,
                    selectedClassID,
                    selectedStudentID,
                    selectedGroupID,
                    drAlignments.ToSql(),
                    selectedYear??"",
                    selectedDemographics.Any(x=>x.DemoLabel=="Race")?selectedDemographics.Where(x=>x.DemoLabel=="Race").Select(x=>x.DemoValue).First():"",
                    selectedDemographics.Any(x=>x.DemoLabel=="Students With Disabilities")?selectedDemographics.Where(x=>x.DemoLabel=="Students With Disabilities").Select(x=>x.DemoValue).First():"",
                    selectedDemographics.Any(x=>x.DemoLabel=="English Language Learner")?selectedDemographics.Where(x=>x.DemoLabel=="English Language Learner").Select(x=>x.DemoValue).First():"",
                    selectedDemographics.Any(x=>x.DemoLabel=="Economically Disadvantaged")?selectedDemographics.Where(x=>x.DemoLabel=="Economically Disadvantaged").Select(x=>x.DemoValue).First():"",
                    selectedDemographics.Any(x=>x.DemoLabel=="Early Intervention Program")?selectedDemographics.Where(x=>x.DemoLabel=="Early Intervention Program").Select(x=>x.DemoValue).First():"",
                    selectedDemographics.Any(x=>x.DemoLabel=="Gender")?selectedDemographics.Where(x=>x.DemoLabel=="Gender").Select(x=>x.DemoValue).First():"",
                    selectedDemographics.Any(x=>x.DemoLabel=="Gifted")?selectedDemographics.Where(x=>x.DemoLabel=="Gifted").Select(x=>x.DemoValue).First():"",
                    credentialID,
                    levelID
                };

            DataSet dsCredentialReport = ThinkgateDataAccess.FetchDataSet(Thinkgate.Base.Classes.Data.StoredProcedures.E3_CREDENTIALS_TRACKINGREPORT_GET, objparams);
            return dsCredentialReport.Tables[0];
        }

        public void ExportToExcel(ReportDataCredential reportData)
        {

            // Create the workbook
            XLWorkbook workbook = BuildWorkBook(reportData, CredentialTrackingReport);

            //Prepare the response
            System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
            Response.Clear();
            Response.Buffer = true;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (browser.Platform.IndexOf("WinNT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    workbook.SaveAs(memoryStream);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + CredentialTrackingReport_NoSpaces + ".xlsx");
                    Response.BinaryWrite(memoryStream.ToArray());
                }
                else
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "text/csv";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + CredentialTrackingReport_NoSpaces + ".csv");
                    byte[] csv = ExportToCSV.ConvertXLToCSV(workbook);
                    Response.BinaryWrite(csv);
                }

                Response.End();
            }
        }

        private DataTable FormatDataTableForExcelExport(DataTable exportReadyDataTable)
        {
            foreach (string ignoredColumn in _excelIgnoredColumns)
            {
                if (exportReadyDataTable.Columns.Contains(ignoredColumn))
                {
                    exportReadyDataTable.Columns.Remove(ignoredColumn);
                }
            }
            return exportReadyDataTable;
        }

        private void BuildCriteriaSheet(string worksheetName, XLWorkbook workbook, SelectedCriteriaCrd selectedCriteria)
        {
            //You can find info on manipulating the Excel workbook here - https://closedxml.codeplex.com/documentation
            var ws = workbook.Worksheets.Add(worksheetName);

            ws.Range("B2:B4").Style.Font.Bold = true;
            ws.Cell(2, 2).Value = CredentialTrackingReport;
            ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;  //Center the cell
            ws.Range("B2:C2").Row(1).Merge(); //Merge across cells
            ws.Range("B2:C2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            ws.Cell(3, 2).Value = "Standard List";
            ws.Cell(4, 2).Value = "Criteria";
            ws.Cell("B4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;  //Center the cell
            ws.Range("B4:B7").Column(1).Merge(); ; //Merge across columns


            if (string.IsNullOrEmpty(selectedCriteria.SelectedWorksheet))
            {
                ws.Cell(3, 3).Value = selectedCriteria.StandardList;
            }
            else
            {
                ws.Cell(3, 3).Value = selectedCriteria.StandardList + ": \"" + selectedCriteria.SelectedWorksheet + "\"";
            }

            //View By, Demographics, Standard Level, and Data Range
            ws.Cell(4, 3).Value = "View By = " + selectedCriteria.SelectedViewBy;
            ws.Cell(5, 3).Value = "Demographics = " + selectedCriteria.SelectedDemographics.ToArray();
            ws.Cell(6, 3).Value = "Standard Level = " + selectedCriteria.StandardLevel;
            ws.Cell(7, 3).Value = "Date Range = (start:" + selectedCriteria.StartDate + " - end: " + selectedCriteria.EndDate + ")";

            ws.Range("D2:D7").Style.Font.Bold = true;
            var theDistrict = Base.Classes.District.GetDistrictByID(SessionObject.LoggedInUser.District);
            ws.Cell(2, 4).Value = theDistrict.DistrictName + " (" + theDistrict.ClientID + ")";

            ws.Cell("D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;  //Center the cell
            ws.Range("D2:G2").Row(1).Merge(); //Merge across cells
            ws.Range("D2:G2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            ws.Cell(3, 4).Value = "School";
            ws.Cell(4, 4).Value = "Teacher";
            ws.Cell(5, 4).Value = "Class";
            ws.Cell(6, 4).Value = "Group";
            ws.Cell(7, 4).Value = "Student";

            ws.Cell(3, 5).Value = selectedCriteria.SelectedSchoolName;
            ws.Cell(4, 5).Value = selectedCriteria.SelectedTeacher;
            ws.Cell(5, 5).Value = selectedCriteria.SelectedClass;
            ws.Cell(6, 5).Value = selectedCriteria.SelectedGroup;
            ws.Cell(7, 5).Value = selectedCriteria.SelectedStudent;

            ws.Range("F3:F6").Style.Font.Bold = true;
            ws.Cell(3, 6).Value = "# Schools";
            ws.Cell(4, 6).Value = "# Teachers";
            ws.Cell(5, 6).Value = "# Classes";
            ws.Cell(6, 6).Value = "# Groups";
            ws.Cell(7, 6).Value = "# Students";

            ws.Cell(3, 7).Value = "?";
            ws.Cell(4, 7).Value = "?";
            ws.Cell(5, 7).Value = "?";
            ws.Cell(6, 7).Value = "?";
            ws.Cell(7, 7).Value = "?";

            ws.Columns().AdjustToContents();
        }

        private XLWorkbook BuildWorkBook(ReportDataCredential reportData, string sheet = "")
        {
            DataTable dt = reportData.ReportDataTable;
            SelectedCriteriaCrd selectedCriteria = reportData.ReportSelectedCriteria;
            XLWorkbook workbook = new XLWorkbook();

            if (dt.Rows.Count > 0)
            {
                var ws = workbook.Worksheets.Add(sheet ?? "Sheet1");
                // set the header size and alignment
                IXLRows headerRow = ws.Rows(1, 1);
                if (headerRow != null)
                {
                    headerRow.Height = 20;
                    headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRow.Style.Font.SetBold();
                }

                int colCount;




                //Write second rows first so that we can calculate width of for the headers
                var rowCount = 2;
                foreach (DataRow row in dt.Rows) // Loop over the rows.
                {
                    colCount = 1;
                    foreach (var item in row.ItemArray) // Loop over the items.
                    {
                        string theCellValue = item.ToString();
                        //if (colCount == 1)
                        //{
                        //    ws.Cell(rowCount, colCount).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        //    ws.Cell(rowCount, colCount).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //    ws.Cell(rowCount, colCount).Style.Alignment.WrapText = true;

                        //}

                        ws.Cell(rowCount, colCount).Value = theCellValue;
                        colCount++;
                    }
                    rowCount++;
                }
                colCount = 1; //reset columns
                foreach (DataColumn column in dt.Columns)
                {
                    ws.Cell(1, colCount).Value = column.ColumnName;

                    ws.Cell(1, colCount).Style.Font.FontColor = XLColor.White;
                    ws.Cell(1, colCount).Style.Fill.BackgroundColor = XLColor.BlueBell;
                    ws.Column(colCount).AdjustToContents();
                    colCount++;
                }

                return workbook;
            }
            return null;
        }


        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (SessionObject.CredentialTracking_ReportData != null)
            {
                DataTable localDataTable = SessionObject.CredentialTracking_ReportData.ReportDataTable;
                if (localDataTable.Rows.Count > 0)
                {
                    ExportToExcel(SessionObject.CredentialTracking_ReportData);
                }
                else
                {
                    //Export Empty Excel if there is no Data.
                    ExportEmptyExcel(SessionObject.CredentialTracking_ReportData);
                }
            }
        }

        public void ExportEmptyExcel(ReportDataCredential reportData)
        {
            // Create the workbook
            XLWorkbook workbook = BuildEmptyWorkBook(reportData, CredentialTrackingReport);

            //Prepare the response
            System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
            Response.Clear();
            Response.Buffer = true;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (browser.Platform.IndexOf("WinNT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    workbook.SaveAs(memoryStream);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + CredentialTrackingReport_NoSpaces + ".xlsx");
                    Response.BinaryWrite(memoryStream.ToArray());
                }
                else
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "text/csv";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + CredentialTrackingReport_NoSpaces + ".csv");
                    byte[] csv = ExportToCSV.ConvertXLToCSV(workbook);
                    Response.BinaryWrite(csv);
                }

                Response.End();
            }
        }

        private XLWorkbook BuildEmptyWorkBook(ReportDataCredential reportData, string sheet = "")
        {
            DataTable dt = reportData.ReportDataTable;
            SelectedCriteriaCrd selectedCriteria = reportData.ReportSelectedCriteria;
            XLWorkbook workbook = new XLWorkbook();

            int colCount;

            var ws = workbook.Worksheets.Add(sheet ?? "Sheet1");
            // set the header size and alignment
            IXLRows headerRow = ws.Rows(1, 1);
            if (headerRow != null)
            {
                headerRow.Height = 20;
                headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRow.Style.Font.SetBold();
            }
            colCount = 1; //reset columns
            foreach (DataColumn column in dt.Columns)
            {
                ws.Cell(1, colCount).Value = column.ColumnName;

                ws.Cell(1, colCount).Style.Font.FontColor = XLColor.White;
                ws.Cell(1, colCount).Style.Fill.BackgroundColor = XLColor.BlueBell;
                ws.Column(colCount).AdjustToContents();
                colCount++;
            }
            return workbook;
        }

        [System.Web.Services.WebMethod]
        public static string GetTeacherListForSchool(int schoolID)
        {
            if (schoolID < 0) return null;
            DataTable teacherListForSchool = new DataTable();

            //set teacher dropdown to the current user if they are a teacher
            if (_loggedOnUserRoleName != null && _loggedOnUserRoleName.Equals("Teacher", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_loggedOnUser.UserFullName) || !string.IsNullOrEmpty(_loggedOnUser.Page.ToString()))
                {
                    return "[{\"TeacherName\":\"" + _loggedOnUser.UserFullName + "\",\"TeacherPage\":\"" + _loggedOnUser.Page + "\"}]";
                }

                return teacherListForSchool.ToJSON(false); //user was a teacher but SessionObject.LoggedInUser object had null values (return new 'empty' datatable')
            }

            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_GetTeacherListBySchoolId",
                    Connection = conn
                };

                cmd.Parameters.Add(new SqlParameter { ParameterName = "School", Value = schoolID });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(teacherListForSchool);
            }
            return teacherListForSchool.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string GetStudentListBySchoolAndTeacher(int schoolID, int teacherPage)
        {
            if (schoolID < 0) return null;
            DataTable studentListBySchoolAndTeacher = new DataTable();

            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_GetStudentListBySchoolIdTeacherId",
                    Connection = conn
                };

                cmd.Parameters.Add(new SqlParameter { ParameterName = "School", Value = schoolID });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "TeacherPage", Value = teacherPage });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(studentListBySchoolAndTeacher);
            }
            return studentListBySchoolAndTeacher.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string GetStudentListBySchoolTeacherAndClass(int schoolID, int teacherPage, int classId)
        {
            if (schoolID < 0) return null;
            DataTable studentListBySchoolTeacherAndClass = new DataTable();

            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_GetStudentListBySchoolIdTeacherIdAndClassId",
                    Connection = conn
                };

                cmd.Parameters.Add(new SqlParameter { ParameterName = "School", Value = schoolID });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "TeacherPage", Value = teacherPage });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ClassId", Value = classId });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(studentListBySchoolTeacherAndClass);
            }
            return studentListBySchoolTeacherAndClass.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string GetClassListBySchoolAndTeacher(int schoolId, int teacherId)
        {
            if (schoolId < 0) return null;
            DataTable classListBySchoolAndTeacher = new DataTable();

            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_GetCVTEClassesForSchoolTeacherForCTR",
                    Connection = conn
                };

                cmd.Parameters.Add(new SqlParameter { ParameterName = "SchoolId", Value = schoolId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "TeacherId", Value = teacherId });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(classListBySchoolAndTeacher);
            }
            return classListBySchoolAndTeacher.Rows.Count <= 0 ? null : classListBySchoolAndTeacher.ToJSON(false);
        }

        protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                var criteriaController = Master.CurrentCriteria();
                _viewBy = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ViewBy").Select(x => x.Text).FirstOrDefault();

                if (_viewBy.Equals(ViewBy.District.ToString()))
                {
                    radGridResults.MasterTableView.GetColumn("LevelName").Visible = false;
                }
                else
                {
                    radGridResults.MasterTableView.GetColumn("LevelName").Visible = true;
                    GridHeaderItem header = (GridHeaderItem)e.Item;
                    if (radGridResults.DataSource != null && radGridResults.DataSource.GetType().Name.Equals("DataTable") && ((DataTable)radGridResults.DataSource).Columns.Contains("LevelName"))
                        header["LevelName"].Text = _viewBy + " (" + ((DataTable)radGridResults.DataSource).AsEnumerable().Select(r => r.Field<string>("LevelName")).Distinct().Count().ToString() + ")";
                    else
                        header["LevelName"].Text = _viewBy;
                }

                if (_viewBy.Equals(ViewBy.Student.ToString()))
                {
                    GridHeaderItem header = (GridHeaderItem)e.Item;
                    if (radGridResults.DataSource != null && radGridResults.DataSource.GetType().Name.Equals("DataTable")
                        && ((DataTable)radGridResults.DataSource).Columns.Contains("StudentCount"))
                        header["StudentCount"].Text = "Date Earned";
                }
            }
        }

        protected void radGridResults_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "StudentCount")
            {
                string[] cmdArguments = e.CommandArgument.ToString().Split(',');
                int levelID = 0, credentialID = 0;
                string credentialName = "";
                if (cmdArguments.Length >= 3)
                {
                    if (!String.IsNullOrEmpty(cmdArguments[0]))
                        Int32.TryParse(cmdArguments[0], out levelID);
                    if (!String.IsNullOrEmpty(cmdArguments[1]))
                        Int32.TryParse(cmdArguments[1], out credentialID);
                    if (!String.IsNullOrEmpty(cmdArguments[2]))
                        credentialName = cmdArguments[2] ?? "";
                }


                SelectedCriteriaCrd selectedCriteria = GetCriteriaControlValues();

                //ViewBy
                string selectedViewBy = selectedCriteria.SelectedViewBy;
                _viewBy = selectedViewBy;
                //School
                int selectedSchoolID = selectedCriteria.SelectedSchoolId;

                string selectedAlignments = selectedCriteria.SelectedAlignment;
                //Teacher
                //The code below forces passing the current teacher to the backend procs, this way a teacher can ONLY see their data.
                int selectedTeacherID = 0;
                if (_loggedOnUserRoleName != null
                    && _loggedOnUserRoleName.Equals("Teacher", StringComparison.InvariantCultureIgnoreCase)
                    && (_loggedOnUser != null && !string.IsNullOrEmpty(_loggedOnUser.Page.ToString())))
                {
                    selectedTeacherID = _loggedOnUser.Page; // force current user
                }
                else
                {
                    selectedTeacherID = Convert.ToInt32(selectedCriteria.SelectedTeacher);
                }

                //Class
                int selectedClassID = Convert.ToInt32(selectedCriteria.SelectedClass);
                //Student
                int selectedStudentID = Convert.ToInt32(selectedCriteria.SelectedStudent);

                //Demographics
                List<Demographics.ValueObject> selectedDemographics = selectedCriteria.SelectedDemographics;
                //Group;
                int selectedGroupID = Convert.ToInt32(selectedCriteria.SelectedGroup);

                string selectedYear = selectedCriteria.SelectedYear;
                

                //GET THE DATA HERE
                GridDataTable = new DataTable();

                if (selectedViewBy != null)
                {
                    GridDataTable = GetCredentialReport(selectedViewBy, selectedSchoolID, selectedTeacherID, selectedClassID, selectedDemographics, selectedGroupID, selectedStudentID, selectedAlignments, selectedYear, credentialID, levelID);
                }

                if (GridDataTable.Rows.Count > 0)
                {
                    //Concanate the search criteria for Student count PDF page
                    StringBuilder sbSelectedCriteria = new StringBuilder("");

                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedViewBy + ";");
                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedSchoolId + ";");
                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedTeacher + ";");
                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedClass + ";");
                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedGroup + ";");
                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedStudent + ";");
                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedAlignment + ";");
                    sbSelectedCriteria.Append(SessionObject.CredentialTracking_ReportData.ReportSelectedCriteria.SelectedYear + ";");

                    sbSelectedCriteria.Append(selectedDemographics.Any(x => x.DemoLabel == "Race") ? selectedDemographics.Where(x => x.DemoLabel == "Race").Select(x => x.DemoValue).First() : "" + ";");

                    sbSelectedCriteria.Append(selectedDemographics.Any(x => x.DemoLabel == "Students With Disabilities") ? selectedDemographics.Where(x => x.DemoLabel == "Students With Disabilities").Select(x => x.DemoValue).First() : "" + ";");
                    sbSelectedCriteria.Append(selectedDemographics.Any(x => x.DemoLabel == "English Language Learner") ? selectedDemographics.Where(x => x.DemoLabel == "English Language Learner").Select(x => x.DemoValue).First() : "" + ";");
                    sbSelectedCriteria.Append(selectedDemographics.Any(x => x.DemoLabel == "Economically Disadvantaged") ? selectedDemographics.Where(x => x.DemoLabel == "Economically Disadvantaged").Select(x => x.DemoValue).First() : "" + ";");
                    sbSelectedCriteria.Append(selectedDemographics.Any(x => x.DemoLabel == "Early Intervention Program") ? selectedDemographics.Where(x => x.DemoLabel == "Early Intervention Program").Select(x => x.DemoValue).First() : "" + ";");
                    sbSelectedCriteria.Append(selectedDemographics.Any(x => x.DemoLabel == "Gender") ? selectedDemographics.Where(x => x.DemoLabel == "Gender").Select(x => x.DemoValue).First() : "" + ";");
                    sbSelectedCriteria.Append(selectedDemographics.Any(x => x.DemoLabel == "Gifted") ? selectedDemographics.Where(x => x.DemoLabel == "Gifted").Select(x => x.DemoValue).First() : "" + ";");


                    sbSelectedCriteria.Append(credentialID + ";");
                    sbSelectedCriteria.Append(levelID);
                    Session.Remove("selectedCriteria");
                    Session["selectedCriteria"] = sbSelectedCriteria;
                    Session.Remove("dtStudentCount");
                    Session["dtStudentCount"] = GridDataTable;
                    string strStudentCountFunction = "openStudentCount('" + credentialName + "');";
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "OpenStudentCount", strStudentCountFunction, true);
                }
            }
            //initialDisplayText.Visible = false;
        }

        public static DataTable GetStudentCountReport(int credentialID, int levelID, string selectedViewBy, int selectedSchoolID, int selectedTeacherId, int selectedClassID,
            List<Demographics.ValueObject> selectedDemographics, int selectedGroupID, int selectedStudentID, string selectedAlignments, string selectedYear = null)
        {
            DataTable studentCount = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = Thinkgate.Base.Classes.Data.StoredProcedures.E3_CREDENTIALS_TRACKINGREPORTSTUDENT_GET,
                    Connection = conn
                };

                cmd.Parameters.Add(credentialID == null
                    ? new SqlParameter { ParameterName = "CredentialID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "CredentialID", Value = credentialID });

                cmd.Parameters.Add(levelID == null
                    ? new SqlParameter { ParameterName = "LevelID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "LevelID", Value = levelID });

                cmd.Parameters.Add(selectedViewBy == null
                    ? new SqlParameter { ParameterName = "ViewBy", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "ViewBy", Value = selectedViewBy });

                cmd.Parameters.Add((selectedSchoolID == 0)
                    ? new SqlParameter { ParameterName = "SchoolId", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "SchoolId", Value = selectedSchoolID });
                cmd.Parameters.Add((selectedTeacherId == 0)
                    ? new SqlParameter { ParameterName = "TeacherId", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "TeacherId", Value = selectedTeacherId });
                cmd.Parameters.Add((selectedClassID == 0)
                    ? new SqlParameter { ParameterName = "ClassId", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "ClassId", Value = selectedClassID });
                cmd.Parameters.Add((selectedStudentID == 0)
                    ? new SqlParameter { ParameterName = "StudentID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StudentID", Value = selectedStudentID });

                cmd.Parameters.Add((selectedGroupID == 0)
                    ? new SqlParameter { ParameterName = "GroupID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "GroupID", Value = selectedGroupID });


                drGeneric_Int drAlignments = new drGeneric_Int();
                if (!string.IsNullOrEmpty(selectedAlignments))
                {
                    string[] arAlignments = (selectedAlignments).Split(',');
                    int alignment = 0;
                    foreach (string strAlign in arAlignments)
                        if (Int32.TryParse(strAlign, out alignment))
                            drAlignments.Add(alignment);
                }

                //cmd.Parameters.Add(new SqlParameter { ParameterName = "Alignments", Value = drAlignments.ToSql() });

                cmd.Parameters.Add((selectedYear == null || selectedYear == "")
                    ? new SqlParameter { ParameterName = "Year", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "Year", Value = selectedYear });


                if (selectedDemographics != null)
                {
                    foreach (var selectedDemographic in selectedDemographics)
                    {
                        switch (selectedDemographic.DemoLabel)
                        {
                            case "Race":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Race", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                                break;
                            case "Students With Disabilities":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Disability", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                                break;


                            case "English Language Learner":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EnglishLearner", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                                break;
                            case "Economically Disadvantaged":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EconomicDisAdv", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                                break;
                            case "Early Intervention Program":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EarlyInt", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                                break;
                            case "Gender":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gender", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                                break;
                            case "Gifted":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gifted", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                                break;
                        }
                    }

                    if (!cmd.Parameters.Contains("Gender"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("Race"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("EnglishLearner"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("EconomicDisAdv"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("Gifted"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("Disability"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("EarlyInt"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                    }


                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(studentCount);
                }
                return studentCount;
            }
        }




        protected void radGridResults_PreRender(object sender, EventArgs e)
        {
            if (_viewBy.IsNotNullOrWhiteSpace() && !_viewBy.Equals("District", StringComparison.CurrentCultureIgnoreCase))
            {
                    radGridResults.Columns[3].HeaderStyle.Width = 450;
                    radGridResults.Columns[3].ItemStyle.Width = 450;
            }
            
            for (int rowIndex = radGridResults.Items.Count - 2; rowIndex >= 0; rowIndex--)
            {
                GridDataItem row = radGridResults.Items[rowIndex];
                GridDataItem previousRow = radGridResults.Items[rowIndex + 1];

                if (row["LevelName"].Text == previousRow["LevelName"].Text)
                {
                    row["LevelName"].VerticalAlign = VerticalAlign.Top;
                    row["LevelName"].RowSpan = previousRow["LevelName"].RowSpan < 2 ? 2 :
                                           previousRow["LevelName"].RowSpan + 1;
                    previousRow["LevelName"].Visible = false;
                }
            }
        }
    }

    [Serializable]
    public class ReportDataCredential
    {
        public DataTable ReportDataTable { get; set; }
        public SelectedCriteriaCrd ReportSelectedCriteria { get; set; }
    }

    [Serializable]
    public class SelectedCriteriaCrd
    {
        // Properties. 
        public string SelectedViewBy { get; set; }
        public int SelectedSchoolId { get; set; }
        public string SelectedSchoolName { get; set; }
        public string SelectedTeacher { get; set; }
        public string SelectedClass { get; set; }
        public string SelectedStudent { get; set; }
        public string StandardLevel { get; set; }
        public List<Demographics.ValueObject> SelectedDemographics { get; set; }
        public string SelectedGroup { get; set; }
        public string StandardList { get; set; }
        public string SelectedWorksheet { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SelectedAlignment { get; set; }
        public string SelectedYear { get; set; }
    }
}