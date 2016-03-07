using System;
using System.Collections.Generic;
using System.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Record
{
    public partial class StateAnalysis : RecordPage
    {
        #region Properties

        public string _level;
        public int _levelID;
        public string _multiTestIDs;
        public string _term;
        public string _year;
        public string _type;
        public string _class;
        public string _parent;
        public int _parentID;
        private string _selectedReport;
        private bool _archive;

        //public TileParms SearchParms { get { return (TileParms)Session["Reports_SearchParms"]; } set { Session["Reports_SearchParms"] = value; } }
        public SearchParms SearchParms;
        public Criteria ReportCriteria;
        private string _guid;

        private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";
        private const int UserPage = 110;

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            _selectedReport = !string.IsNullOrEmpty(Request.QueryString["selectedReport"]) ? Request.QueryString["selectedReport"] : "";
            _archive = !string.IsNullOrEmpty(Request.QueryString["folder"]) && Request.QueryString["folder"].IndexOf("Archive") > -1;
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            _level = Request.QueryString["level"];
            _levelID = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(Request.QueryString["levelID"]);
            _multiTestIDs = Request.QueryString["multiTestIDs"];
            _parentID = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(Request.QueryString["parentID"]);
            _term = !string.IsNullOrEmpty(Request.QueryString["term"]) ? Request.QueryString["term"] : "All";
            _year = !string.IsNullOrEmpty(Request.QueryString["year"]) ? Request.QueryString["year"] : DistrictParms.LoadDistrictParms().Year;
            _type = !string.IsNullOrEmpty(Request.QueryString["type"]) ? Request.QueryString["type"] : "All";
            _class = !string.IsNullOrEmpty(Request.QueryString["cid"]) ? Request.QueryString["cid"] : "0";
            _parent = !string.IsNullOrEmpty(Request.QueryString["parent"]) ? Request.QueryString["parent"] : string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadPageTitle();

            if (!string.IsNullOrEmpty(hiddenTxtBox.Text))
            {
                var guid = hiddenTxtBox.Text;
            }
            else
            {
                LoadSearchCriteria();
            }

            LoadDefaultFolderTiles();
        }

        private void LoadPageTitle()
        {
            pageTitleLabel.Text = "State Assessment Results"; //TODO: Make configurable
        }

        #endregion

        #region Folder Methods

        private void SetupFolders()
        {
            Folders = new List<Folder>();

            if (_archive)
            {
                Folders.Add(new Folder("Multiple FCAT Indicators Archive", "~/Images/new/folder_data_analysis.png", LoadSummativeReportArchive, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                Folders.Add(new Folder("Proficiency Archive", "~/Images/new/folder_data_analysis.png", LoadProficiencyArchive, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                Folders.Add(new Folder("Teacher Student Learning Objectives Archive", "~/Images/new/folder_data_analysis.png", LoadTeacherStudentLearningObjectivesArchive, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
            }
            else
            {
                Folders.Add(new Folder("Multiple FCAT Indicators", "~/Images/new/folder_data_analysis.png", LoadSummativeReport, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                //Folders.Add(new Folder("Performance Summary", "~/Images/new/folder_data_analysis.png", LoadPerformanceSummary, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                //Folders.Add(new Folder("5 Year History", "~/Images/new/folder_data_analysis.png", Load5YearHistory, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                //Folders.Add(new Folder("Trend Analysis", "~/Images/new/folder_data_analysis.png", LoadTrendAnalysis, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                Folders.Add(new Folder("Proficiency", "~/Images/new/folder_data_analysis.png", LoadProficiency, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                if (UserHasPermission(Base.Enums.Permission.Hyperlink_TeacherStudentLearningObjectives))
                {
                    Folders.Add(new Folder("Teacher Student Learning Objectives", "~/Images/new/folder_data_analysis.png", LoadTeacherStudentLearningObjectives, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
                }
            }

            //if (SessionObject.LoggedInUser.HasPermission(Thinkgate.Base.Enums.Permission.Report_StandardAnalysis))

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadReportTiles(string tilePath, TileParms tileParms = null, string reportName = "Results")
        {
            if (tileParms == null) tileParms = new TileParms();

            tileParms.AddParm("CriteriaGUID", hiddenTxtBox.Text);
            tileParms.AddParm("multiTestIDs", _multiTestIDs);

            Rotator1Tiles.Add(new Tile(reportName, tilePath, false, tileParms));
        }

        private void LoadSummativeReport()
        {
            var tileParms = new TileParms();
            tileParms.AddParm("guid", _guid);
            //tileParms.AddParm("AnalysisType", Thinkgate.Controls.Reports.AnalysisType.ItemAnalysis);
            LoadReportTiles("~/Controls/Reports/SummativeReportV2.ascx", tileParms, "Multiple FCAT Indicators");
        }

        private void LoadSummativeReportArchive()
        {
            var tileParms = new TileParms();
            tileParms.AddParm("guid", _guid);
            tileParms.AddParm("Archive", true);
            LoadReportTiles("~/Controls/Reports/SummativeReportV2.ascx", tileParms, "Multiple FCAT Indicators Archive");
        }

        private void LoadPerformanceSummary()
        {
            var tileParms = new TileParms();
            //tileParms.AddParm("AnalysisType", Thinkgate.Controls.Reports.AnalysisType.ItemAnalysis);
            LoadReportTiles("~/Controls/Reports/PerformanceSummary.ascx", tileParms, "Performance Summary");
        }

        private void Load5YearHistory()
        {
            var tileParms = new TileParms();
            //tileParms.AddParm("AnalysisType", Thinkgate.Controls.Reports.AnalysisType.ItemAnalysis);
            LoadReportTiles("~/Controls/Reports/FiveYearHistory.ascx", tileParms, "5 Year History");
        }

        private void LoadTrendAnalysis()
        {
            var tileParms = new TileParms();
            //tileParms.AddParm("AnalysisType", Thinkgate.Controls.Reports.AnalysisType.ItemAnalysis);
            LoadReportTiles("~/Controls/Reports/TrendAnalysis.ascx", tileParms, "Trend Analysis");
        }

        private void LoadProficiency()
        {
            var tileParms = new TileParms();
            //tileParms.AddParm("AnalysisType", Thinkgate.Controls.Reports.AnalysisType.ItemAnalysis);
            LoadReportTiles("~/Controls/Reports/ProficiencyV2.ascx", tileParms, "Proficiency");
        }

        private void LoadProficiencyArchive()
        {
            var tileParms = new TileParms();
            tileParms.AddParm("Archive", true);
            LoadReportTiles("~/Controls/Reports/ProficiencyV2.ascx", tileParms, "Proficiency Archive");
        }

        private void LoadTeacherStudentLearningObjectives()
        {
            var tileParms = new TileParms();
            ScriptManager.RegisterStartupScript(this, typeof(String), "teacherStudentLearningObjectives", "setDocumentTitle('Teacher Student Learning Objectives');", true);
            LoadReportTiles("~/Controls/Reports/TeacherStudentLearningObjectivesControl.ascx", tileParms, "Teacher Student Learning Objectives");
        }

        private void LoadTeacherStudentLearningObjectivesArchive()
        {
            var tileParms = new TileParms();
            ScriptManager.RegisterStartupScript(this, typeof(String), "teacherStudentLearningObjectives", "setDocumentTitle('Teacher Student Learning Objectives');", true);
            LoadReportTiles("~/Controls/Reports/TeacherStudentLearningObjectivesControl.ascx", tileParms, "Teacher Student Learning Objectives");
        }

        #endregion

        private void LoadSearchCriteria()
        {
            _guid = Guid.NewGuid().ToString();
            hiddenTxtBox.Text = _guid;

            //Session["Criteria_" + _guid] = searchCriteria;
            return;
            var yearList = new List<string> { this._year };

            var searchCriteria = new Criteria();

            #region Criterion: Assessment

            var assessmentTypesTable = Thinkgate.Base.Classes.Reports.GetTestTypesForStateReporting();

            //CLEANUP: WSH - Need to strip out all of this old critiera stuff once the new fct and proficiency stuff is verified.

            /*
             * WSH: 8/20/12 - removed for Sarasota demo per meeting request - 
              var _assessmentType = (assessmentTypesTable.Rows.Count > 0) ? assessmentTypesTable.Rows[0]["Type"].ToString() : "";
                foreach (DataRow row in assessmentTypesTable.Rows)
            {
                if (row["Type"].ToString() == "FCAT2")
                {
                    _assessmentType = "FCAT2";
                    break;
                }
                
            }
              */

            searchCriteria.Add(new Criterion()
            {
                Header = "Assessment",
                Locked = false,
                Key = "TestType",
                Object = "",
                Type = "String",
                ReportStringKey = "TID",
                ReportStringVal = "",
                UIType = UIType.DropDownList,
                DataSource = assessmentTypesTable,
                DataTextField = "Type",
                DataValueField = "Type",
                IsRequired = true/*,
                DefaultValue = _assessmentType*/
            });

            #endregion

            #region Criterion: Year

            var yearsTable = Thinkgate.Base.Classes.Reports.GetYearsForStateReporting();
            // WSH: 8/20/12 - removed for Sarasota demo per meeting request - if (yearsTable.Rows.Count > 0) _year = yearsTable.Rows[0]["Year"].ToString();
            searchCriteria.Add(new Classes.Criterion()
            {
                Key = "Year",
                Header = "Year",
                Type = "String",
                Removable = true,
                Locked = false,
                IsHeader = true,
                DataTextField = "Year",
                DataValueField = "Year",
                UIType = UIType.DropDownList,
                DataSource = yearsTable,/*
                DefaultValue = _year,*/
                IsRequired = true,
                ServiceUrl = ResolveUrl("~/Services/GradeSubjectCourse.svc/GetTeachersBySchools"),
                ServiceOnSuccess = "getTeachers",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("Grade", "Grades"),
                    Criterion.CreateDependency("School", "Schools"),
                    Criterion.CreateDependency("Teacher", "Teachers"),
                    Criterion.CreateDependency("Year", "Years")
                }
            });

            #endregion

            #region Criterion: Grade
            searchCriteria.Add(new Criterion
            {
                Header = "Grade",
                Key = "Grade",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataSource = Thinkgate.Base.Classes.Reports.GetGradesForStateReporting(),
                UIType = UIType.DropDownList,
                Removable = true,
                IsRequired = true,
                ServiceUrl = ResolveUrl("~/Services/GradeSubjectCourse.svc/GetSchoolsAndTeachersByGrade"),
                ServiceOnSuccess = "getSchoolsAndTeachers",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("Grade", "Grades"),
                    Criterion.CreateDependency("School", "Schools"),
                    Criterion.CreateDependency("Teacher", "Teachers"),
                    Criterion.CreateDependency("Year", "Years")
                }
            });

            #endregion

            #region Criterion: Subject

            searchCriteria.Add(new Criterion
            {
                Header = "Subject",
                Key = "Subject",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Subject",
                DataValueField = "Subject",
                Removable = true,
                DataSource = Thinkgate.Base.Classes.Reports.GetSubjectsForStateReporting(),
                UIType = UIType.DropDownList,
                IsRequired = true
            });
            #endregion

            #region Criterion: School
            var schoolDataTable = new System.Data.DataTable();
            var schoolsForLoggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolsForLoggedInUser = schoolsForLoggedInUser.FindAll(s => s.Grades.IndexOf("03") > -1);
            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID");

            foreach (var s in schoolsForLoggedInUser)
            {
                schoolDataTable.Rows.Add(s.Name, s.ID);
            }

            searchCriteria.Add(new Criterion
            {
                Header = "School",
                Key = "School",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Name",
                DataValueField = "ID",
                Removable = true,
                DataSource = schoolDataTable,
                UIType = UIType.DropDownList,
                ServiceUrl = ResolveUrl("~/Services/GradeSubjectCourse.svc/GetTeachersBySchools"),
                ServiceOnSuccess = "getTeachers",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("Grade", "Grades"),
                    Criterion.CreateDependency("School", "Schools"),
                    Criterion.CreateDependency("Teacher", "Teachers"),
                    Criterion.CreateDependency("Year", "Years")
                }
            });

            #endregion

            #region Criterion: Teacher

            var teachers = new System.Data.DataTable();//Base.Classes.Teacher.GetTeachersBySchoolsDT(schoolsForLoggedInUser.Select(s => s.ID).ToList());
            teachers.Columns.Add("ID");
            teachers.Columns.Add("TeacherName");

            searchCriteria.Add(new Criterion
            {
                Header = "Teacher",
                Key = "Teacher",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "TeacherName",
                DataValueField = "ID",
                Removable = true,
                DataSource = teachers,
                UIType = UIType.DropDownList

            });

            #endregion

            #region Criterion: Class
            var classCourseList = Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
            List<Int32> schoolIDs = new List<int>();
            schoolIDs.Add(0);
            var classesForLooggedInUser = Thinkgate.Base.Classes.Class.SearchClasses(classCourseList, null, schoolIDs, "");

            searchCriteria.Add(new Criterion
            {
                Header = "Class",
                Key = "Class",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "ClassName",
                DataValueField = "ClassID",
                Removable = true,
                DataSource = classesForLooggedInUser,
                UIType = UIType.DropDownList
            });
            #endregion

            #region Criterion: Demographics
            searchCriteria.Add(new Criterion()
            {
                Header = "Demographics",
                Key = "Demographics",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.Demographics
            });

            #endregion

            SearchParms = new SearchParms();
            SearchParms.AddParm("reportCriteria", searchCriteria);

            _guid = Guid.NewGuid().ToString();
            hiddenTxtBox.Text = _guid;

            Session["Criteria_" + _guid] = searchCriteria;
        }
    }
}
