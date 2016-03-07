using Thinkgate.Utilities;

namespace Thinkgate.Controls.Student
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Standpoint.Core.Classes;
    using Standpoint.Core.Utilities;

    using Telerik.Web.UI;

    using Thinkgate.Base.Classes;
    using Thinkgate.Base.Enums;
    using Thinkgate.Classes;
    using Thinkgate.Controls.Reports;
    using System.Collections.Generic;
    using ClosedXML.Excel;
    using GemBox.Spreadsheet;

    public partial class StudentSearch_Expanded : ExpandedSearchPage
    {
		//TODO:Consider removing "static" from these two properties - otherwise one user could be effected by prior user's level & id. - DHB 3/27/13
        public EntityTypes _level;
        public int _levelID;

        private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";

        private const int UserPage = 110;
        private string _selectedGrade;
        private string _selectedSchoolType;
        private string _selectedClass;
        protected string gradeListFilterKey = "GradeListFilter";
        private DataTable dataTableStudents;

        #region Properties

        public string HiddenGuid { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ParseRequestQueryString();

            switch (_level)
            {
                case EntityTypes.Teacher:
                case EntityTypes.District:
                    _selectedClass = SessionObject.StudentSearchParms.GetParm("PieChartValue") != null ? SessionObject.StudentSearchParms.GetParm("PieChartValue").ToString() : null;
                    _selectedSchoolType = SessionObject.StudentSearchParms.GetParm("PieChartValue") != null ? SessionObject.StudentSearchParms.GetParm("PieChartValue").ToString().Replace(" ", "-s-") : null;
                    break;
                case EntityTypes.School:
                    Nullable<int> gradePos = DataIntegrity.ConvertToNullableInt(SessionObject.StudentSearchParms.GetParm("PieChartValue"));
                    if (gradePos != null)
                    {
                    var gradeList = (List<string>)SessionObject.StudentSearchParms.GetParm("GradeListFilter");
                        _selectedGrade = gradeList != null && gradeList.Count > 0 ? gradeList[(int)gradePos] : null;
                    }
                    break;
            }

            LoadStudentSearchCriteriaControl();
        }

        //******* 2012-8-30 DHB Start code changes.
        //QA178 - Remove Tile view of search results defaulting to grid view only
        //        Have added Visible="false" to the icon buttons that display 
        //        grid/tile views, so the two event handlers below are no longer
        //        needed.
        //
        //protected void imgIcon_Click(object sender, EventArgs e)
        //{
        //    SessionObject.Students_SearchResultSize = "Small";
        //    SearchStudents("Small");
        //}

        //protected void imgGrid_Click(object sender, EventArgs e)
        //{
        //    SessionObject.Students_SearchResultSize = "Grid";
        //    SearchStudents("Grid");
        //}
        //******* 2012-8-30 DHB Stop code changes.

        protected void ImgBtnNameSearchClick(object sender, ImageClickEventArgs e)
        {
            SearchStudents();
        }

        protected void ImgBtnAdvancedCriteriaSearchClick(object sender, ImageClickEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "SEARCH_PARENT", "clickSearch()", true);
        }

        protected void GotoStudentBtnClick(object sender, EventArgs eventArgs)
        {
            var studentID = Standpoint.Core.Classes.Encryption.EncryptString(selectedStudentInput.Text);
            ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", "parent.window.open('/Record/Student.aspx?childPage=yes&xID=" + studentID + "');", true);

            // Since this causes a postback, reload search tiles
            // TODO: Paging resets. Have it go back to last page.
        }

        #region Private Methods

        private Criteria LoadSearchCriteria()
        {
            var criteria = new Criteria();

            criteria.Add(new Criterion
            {
                Header = "Name",
                Key = "Name",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.TextBox,
                Removable = true
            });

            criteria.Add(new Criterion
            {
                Header = "Student ID",
                Key = "StudentID",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.TextBox,
                Removable = true
            });

            criteria.Add(new Criterion
            {
                Header = "RTI",
                Key = "RTI",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.RTI
            });

            criteria.Add(new Criterion
            {
                Header = "Cluster",
                Key = "Cluster",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.CheckBoxList,
                DataSource = Base.Classes.Cluster.GetClusterListForDropDown(UserPage),
                DataTextField = "Cluster",
                DataValueField = "Cluster",
                Removable = true
            });

            // School Type
            var schoolTypeDataTable = new DataTable();
            var schoolTypesForLoggedInUser = SchoolTypeMasterList.GetSchoolTypeListForUser(SessionObject.LoggedInUser);
            schoolTypeDataTable.Columns.Add("SchoolType");

            foreach (var s in schoolTypesForLoggedInUser)
            {
                schoolTypeDataTable.Rows.Add(s);
            }

            criteria.Add(new Criterion
            {
                Header = "School Type",
                Key = "SchoolType",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.CheckBoxList,
                DataSource = schoolTypeDataTable,
                DataTextField = "SchoolType",
                DataValueField = "SchoolType",
                DefaultValue = _selectedSchoolType,
                Removable = true,
                ServiceUrl = "../../Services/School.svc/GetAllSchoolsFromSchoolTypes",
                ServiceOnSuccess = "getAllSchoolsFromSchoolTypes",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("SchoolType", "SchoolTypes"),
                    Criterion.CreateDependency("School", "Schools")
                }
            });

            // School
            var schoolDataTable = new DataTable();
            var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID");

            foreach (var s in schoolsForLooggedInUser)
            {
                schoolDataTable.Rows.Add(s.Name, s.ID);
            }

            criteria.Add(new Criterion
            {
                Header = "School",
                Key = "School",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.DropDownList,
                DataSource = schoolDataTable,
                DataTextField = "Name",
                DataValueField = "ID",
                Removable = true
            });

            // School
            var gradeDataTable = new DataTable();
            gradeDataTable.Columns.Add("Grade");

            foreach (Grade g in Thinkgate.Base.Classes.Grade.GetGradesForStudents())
            {
                gradeDataTable.Rows.Add(g.DisplayText);
            }

            criteria.Add(new Criterion
            {
                Header = "Grade",
                Key = "Grade",
                Type = "String",
                DataTextField = "Grade",
                DataValueField = "Grade",
                Description = string.Empty,
                Locked = false,
                DefaultValue = _selectedGrade,
                DataSource = gradeDataTable,
                UIType = UIType.CheckBoxList,
                Removable = true
            });

            criteria.Add(new Criterion
            {
                Header = "Demographics",
                Key = "Demographics",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.Demographics
            });

            // Enrollment
            var enrollmentDataTable = new DataTable();
            enrollmentDataTable.Columns.Add("Enrollment");

            enrollmentDataTable.Rows.Add("Current Year");
            enrollmentDataTable.Rows.Add("Prior Years");
            enrollmentDataTable.Rows.Add("All Years");


            criteria.Add(new Criterion
            {
                Header = "Enrollment",
                Key = "Enrollment",
                Type = "String",
                Description = string.Empty,
                DefaultValue = enrollmentDataTable.Rows[0].ItemArray[0].ToString(),
                DataSource = enrollmentDataTable,
                Locked = false,
                DataTextField = "Enrollment",
                DataValueField = "Enrollment",
                UIType = UIType.DropDownList
            });

            return criteria;
        }

        private void LoadStudentSearchCriteriaControl()
        {
            var ctlReportCriteria = (ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlStudentSearchResultsCriteria";

            if (string.IsNullOrEmpty(hiddenTextBox.Text))
            {
                HiddenGuid = Guid.NewGuid().ToString();
                hiddenTextBox.Text = HiddenGuid;
                ctlReportCriteria.Guid = HiddenGuid;
                ctlReportCriteria.Criteria = LoadSearchCriteria();
                ctlReportCriteria.FirstTimeLoaded = true;
            }
            else
            {
                HiddenGuid = hiddenTextBox.Text;
                ctlReportCriteria.Guid = hiddenTextBox.Text;
                ctlReportCriteria.FirstTimeLoaded = false;
            }

            ctlReportCriteria.ReloadReport += ReportCriteria_ReloadReport;

            criteraDisplayPlaceHolder.Controls.Add(ctlReportCriteria);
        }

        private void ReportCriteria_ReloadReport(object sender, EventArgs e)
        {
            this.SearchStudents("Grid");
        }

        private void ParseRequestQueryString()
        {
            if (string.IsNullOrEmpty(Request.QueryString["level"]))
            {
                SessionObject.RedirectMessage = "No level provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }

            _level = (EntityTypes)EnumUtils.enumValueOf(Request.QueryString["level"], typeof(EntityTypes));

            if (string.IsNullOrEmpty(Request.QueryString["levelID"]))
            {
                SessionObject.RedirectMessage = "No levelID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }

            _levelID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "levelID");
        }

        private void SearchStudents(string resultType = "Grid")
        {
            var studentSearchParms = (Criteria)Session["Criteria_" + HiddenGuid];

            var studentName = studentSearchParms.FindCriterionHeaderByText("Name").ReportStringVal ?? string.Empty;
            var studentID = studentSearchParms.FindCriterionHeaderByText("Student ID").ReportStringVal ?? string.Empty;

            var cluster = string.Join(",", studentSearchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Cluster") && !string.IsNullOrEmpty(r.ReportStringVal)).Select(t => t.Object));
            var schoolType = string.Join(",", studentSearchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("SchoolType") && !string.IsNullOrEmpty(r.ReportStringVal)).Select(t => t.Object));

            var schoolID = DataIntegrity.ConvertToInt(studentSearchParms.FindCriterionHeaderByText("School").ReportStringVal);

            var grades = string.Join(",", studentSearchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Grade") && !string.IsNullOrEmpty(r.ReportStringVal)).Select(t => t.Object));

            // OLD CODE

            //var studentName = studentSearchParms.GetParm("Student Name") != null ? studentSearchParms.GetParm("Student Name").ToString() : string.Empty;
            //var studentID = studentSearchParms.GetParm("Student ID") != null ? studentSearchParms.GetParm("Student ID").ToString() : string.Empty;
            //var loginID = studentSearchParms.GetParm("Login ID") != null ? studentSearchParms.GetParm("Login ID").ToString() : string.Empty;
            //var grades = studentSearchParms.GetParm("Grades") != null ? string.Join(",", (List<string>)studentSearchParms.GetParm("Grades")) : string.Empty;
            //var cluster = string.Empty;
            //var schoolType = studentSearchParms.GetParm("School Type") != null ? string.Join(",", (List<string>)studentSearchParms.GetParm("School Type")) : string.Empty;
            //var schoolID = studentSearchParms.GetParm("School ID") != null ? DataIntegrity.ConvertToInt(studentSearchParms.GetParm("School ID")) : 0;

            var tier2RTI = string.Empty;
            var tier3RTI = string.Empty;
            var inactiveRTI = string.Empty;

            var rtiControl = studentSearchParms.FindCriterionHeaderByText("RTI");

            if (rtiControl != null && !string.IsNullOrEmpty(rtiControl.ReportStringVal))
            {
                var rtiSerializer = new JavaScriptSerializer();
                var rtiObject = rtiSerializer.Deserialize<ReportCriteria.RTIJsonObject>(rtiControl.ReportStringVal);
                if (rtiObject != null)
                {
                    foreach (var tier in rtiObject.items)
                    {
                        switch (tier.text)
                        {
                            case "Former Year":
                                inactiveRTI = "yes";
                                break;

                            case "Current Tier 2":
                                tier2RTI = "yes";
                                break;

                            case "Current Tier 3":
                                tier3RTI = "yes";
                                break;
                        }
                    }
                }
            }

            var demoString = string.Empty;
            var demographicControl = studentSearchParms.FindCriterionHeaderByText("Demographics");

            if (demographicControl != null && !string.IsNullOrEmpty(demographicControl.ReportStringVal))
            {
                var serializer = new JavaScriptSerializer();
                var demographicObject = serializer.Deserialize<ReportCriteria.DemographicJsonObject>(demographicControl.ReportStringVal);
                if (demographicObject != null)
                {
                    demoString = demographicObject.items.Aggregate(string.Empty, (current, demographic) => current + ("@@D" + demographic.demoField + "=" + demographic.value + "@@"));
                }
            }

            var enrollment = studentSearchParms.FindCriterionHeaderByText("Enrollment").ReportStringVal.Replace("-s-", " ");

            dataTableStudents = Base.Classes.Data.StudentDB.Search(0, _level.ToString(), _levelID, studentName, studentID, grades, cluster, schoolType, schoolID, tier2RTI, tier3RTI, inactiveRTI, enrollment, demoString);
            const string template = "<a class='searchLink' href='javascript: openStudent(\"@@##ID##@@\")'><div align='center'>"
                                    + "<img width='50px' src='../../images/new/search_student_@@Demo6@@.png' border='0'/></div> "
                                    + "@@StudentName@@<br/>@@StudentID@@<br/>@@Grade@@, @@Abbreviation@@</a>";
            const string templateNoLink = "<div align='center'>"
                                    + "<img width='50px' src='../../images/new/search_student_@@Demo6@@.png' border='0'/></div> "
                                    + "@@StudentName@@<br/>@@StudentID@@<br/>@@Grade@@, @@Abbreviation@@";

            lblSearchResultCount.Text = "Results Found: " + (dataTableStudents == null ? 0 : dataTableStudents.Rows.Count);

            LoadResults(dataTableStudents, buttonsContainer1, tileDiv1, tileResultsPanel, gridResultsPanel, radGridResults, resultType, 
                UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName) ? template : templateNoLink);
        }

        #endregion

        protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            SearchStudents("Grid");
        }

        protected void radGridResults_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;

            if (item != null)
            {
                var link = (HyperLink)item["ViewHyperLinkColumn"].Controls[0];

                var id = string.Format("{0}", DataBinder.Eval(item.DataItem, "ID"));
                var encryptedId = Encryption.EncryptString(id);

                if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                {
                    link.NavigateUrl = string.Format("~/Record/Student.aspx?childPage=yes&xID={0}", encryptedId);
                }
            }
        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            this.SearchStudents("Grid");
        }

        protected void btnExport_Click(object sender, ImageClickEventArgs e)
        {            // Create the workbook
            SearchStudents();
            ExcelFile ef = new ExcelFile();
            ef.DefaultFontName = "Calibri";
            ExcelWorksheet ws = ef.Worksheets.Add("Results");
            FormatExcelExport(ws, dataTableStudents);
            ef.Save(Response, "ExportData.xlsx");
        }

        private void FormatExcelExport(ExcelWorksheet ws, DataTable dt)
        {
            int rowCount = 1;
            int colCount = 0;

            foreach (DataRow row in dt.Rows)
            {
                colCount = 0;
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ColumnName == "StudentName" || column.ColumnName == "StudentID" || column.ColumnName == "FormattedGrade" || column.ColumnName == "SchoolName")
                    {
                        ws.Cells[rowCount, colCount].Value = row[column].ToString();
                        colCount++;
                    }
                }
                rowCount++;
            }

            colCount = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName == "StudentName" || dc.ColumnName == "StudentID" || dc.ColumnName == "FormattedGrade" || dc.ColumnName == "SchoolName")
                {
                    ws.Cells[0, colCount].Value = (dc.ColumnName == "ViewHyperLinkColumn") ? "Name" : dc.ColumnName;
                    ws.Cells[0, colCount].Style.Font.Weight = ExcelFont.BoldWeight;
                    ws.Columns[colCount].AutoFit();
                    colCount++;
                }
            }
        }
    }
}
