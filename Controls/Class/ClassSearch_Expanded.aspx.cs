using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.Reports;
using Standpoint.Core.Utilities;
using Thinkgate.Utilities;
using Thinkgate.Controls.E3Criteria;

namespace Thinkgate.Controls.Class
{
    public partial class ClassSearch_Expanded : ExpandedSearchPage
    {       
        #region Properties

        public DataTable GridDataTable;
        public string PageGuid;
        public string DataTableCount;
        public string HiddenGuid { get; set; }
        private string _selectedSchoolType;
        private bool userCrossSchools;

        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            LoadSearchScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hiddenGuidBox.Text))
            {
                PageGuid = hiddenGuidBox.Text;
            }
            else
            {
                PageGuid = Guid.NewGuid().ToString().Replace("-", "");
                hiddenGuidBox.Text = PageGuid;
            }

            _selectedSchoolType = SessionObject.SchoolSearchParms.GetParm("SchoolType") != null ? SessionObject.SchoolSearchParms.GetParm("SchoolType").ToString().Replace(" ", "-s-") : null;

            if (!IsPostBack)
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "anything", BuildStartupScript(exportGridImgBtn.ClientID, "../..", PageGuid), false);

            ParseRequestQueryString();

            LoadSearchCriteriaControl();

            if (radGridResults.DataSource == null)
            {
                radGridResults.Visible = false;
                initialDisplayText.Controls.Add(new System.Web.UI.HtmlControls.HtmlGenericControl("div") { InnerText = "Press Search...." });
                initialDisplayText.Visible = true;
            }
            

            if (IsPostBack)
            {
                radGridResults.Visible = true;
                RadScriptManager scriptManager = (RadScriptManager)ScriptManager.GetCurrent(this.Page);
                scriptManager.RegisterPostBackControl(this.exportGridImgBtn);
                userCrossSchools = SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Schools);
            }
        }

        private void LoadSearchScripts()
        {
           // if (Master != null)
           // {
               // var scriptManager = Master.FindControl("RadScriptManager2");
                //if (scriptManager != null)
                //{
                //    var radScriptManager = (RadScriptManager)scriptManager;
                //    radScriptManager.Scripts.Add(new ScriptReference("~/Scripts/SchoolSearch.js"));
                //}
          //  }
        }

        private void BindDataToGrid()
        {
            var searchParms = (Criteria)Session["Criteria_" + HiddenGuid];

            List<string> clusters = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Cluster") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

            List<string> schoolTypes = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("SchoolType") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();
            List<Int32> schoolIDs = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Header == "School" && r.Object != null)
                                                            .Select(criterion => DataIntegrity.ConvertToInt(criterion.ReportStringVal)).ToList();
            //						List<Int32> schoolIDs = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key == "School" && r.Object != null).Select(criterion => DataIntegrity.ConvertToInt(criterion.DataValueField)).ToList();

            if (schoolIDs.Count == 0)
            {
                if (!userCrossSchools && SessionObject.LoggedInUser.Schools != null && SessionObject.LoggedInUser.Schools.Count > 0)
                {
                    foreach (var s in SessionObject.LoggedInUser.Schools)
                    { schoolIDs.Add(s.Id); }
                }
            }

            List<string> grades = searchParms.CriterionList.FindAll(r => r.Key.Contains("Grade") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

            List<string> subjects = searchParms.CriterionList.FindAll(r => r.Key.Contains("Subject") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();
            #region Get: CourseList
            var classCourseIDs = new CourseList();

            List<string> selectedCourses = searchParms.CriterionList.FindAll(r => r.Key.Contains("Course") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();
            classCourseIDs = Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).FilterByGradesSubjectsStandardSetsAndCourse(grades, subjects, null, selectedCourses);

            #endregion

            var teacher = searchParms.CriterionList.Find(r => r.Key == "AssessmentAssignmentTeacher").Object != null ? searchParms.CriterionList.Find(r => r.Key == "AssessmentAssignmentTeacher").Object.ToString() : string.Empty;

            #region Get SemesterList criterion
            List<string> semesterList = new List<string>();

            foreach (var criterion in searchParms.CriterionList.FindAll(r => r.IsHeader && r.Key.Contains("Semester") && r.Object != null))
            {
                semesterList.Add(criterion.Object.ToString());
            }
            #endregion


            #region Get Section criterion
            var section = searchParms.CriterionList.Find(r => r.Key == "Section").ReportStringVal ?? string.Empty;
            #endregion

            #region Get Block criterion
            var block = searchParms.CriterionList.Find(r => r.Key == "Block").ReportStringVal ?? string.Empty;
            #endregion

            #region Get Period criterion
            List<string> periodList = new List<string>();

            foreach (var criterion in searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Period") && r.Object != null))
            {
                periodList.Add(criterion.Object.ToString());
            }
            #endregion


            GridDataTable = Thinkgate.Base.Classes.Class.SearchClasses(classCourseIDs, clusters, schoolIDs, section,
                                                                                                                                 teacher, semesterList, periodList, block,
                                                                                                                                 schoolTypes, userCrossSchools);

            DataTableCount = GridDataTable.Rows.Count.ToString();
            radGridResults.DataSource = GridDataTable;
            radGridResults.DataBind();
            initialDisplayText.Visible = false;
            resultsFoundDiv.InnerText = "Results found: " + DataTableCount.ToString();
        }

        protected void ImageIcon_Click(object sender, EventArgs e)
        {
        }

        protected void ImageGrid_Click(object sender, EventArgs e)
        {
        }

        protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindDataToGrid();
        }

        protected void GotoAssessmentButtonClick(object sender, EventArgs eventArgs)
        {
        }

        #region Private Methods

        private Criteria LoadSearchCriteria()
        {
            var criteria = new Criteria();
            #region Criterion: Grade
            var gradesDT = new DataTable();
            var gradesByCurrCourses = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetGradeList();
            gradesDT.Columns.Add("Grade");

            foreach (var g in gradesByCurrCourses)
            {
                gradesDT.Rows.Add(g.DisplayText);
            }

            criteria.Add(new Criterion
            {
                Header = "Grade",
                Key = "Grade",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Grade",
                DataValueField = "Grade",
                Removable = true,
                DataSource = gradesDT,
                UIType = UIType.CheckBoxList,
                ServiceUrl = "../../Services/GradeSubjectCourse.svc/GetClassSubjectsAndCourses",
                ServiceOnSuccess = "getClassGradesSubjectsAndCourses",
                Dependencies = new[]
								{
										Criterion.CreateDependency("Grade", "Grades"),
										Criterion.CreateDependency("Subject", "Subjects"),
										Criterion.CreateDependency("ClassCourses", "Courses")
								}
            });
            #endregion

            #region Criterion: Subject
            var subjectsDT = new DataTable();
            var subjectsByCurrCourses = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetSubjectList();
            subjectsDT.Columns.Add("Subject");

            foreach (var s in subjectsByCurrCourses)
            {
                subjectsDT.Rows.Add(s.DisplayText);
            }

            criteria.Add(new Criterion
            {
                Header = "Subject",
                Key = "Subject",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Subject",
                DataValueField = "Subject",
                Removable = true,
                DataSource = subjectsDT,
                UIType = UIType.CheckBoxList,
                ServiceUrl = "../../Services/GradeSubjectCourse.svc/GetClassCourses",
                ServiceOnSuccess = "getClassCoursesFromSubjects",
                Dependencies = new[]
								{
										Criterion.CreateDependency("Grade", "Grades"),
										Criterion.CreateDependency("Subject", "Subjects"),
										Criterion.CreateDependency("ClassCourses", "Courses")
								}
            });
            #endregion

            #region Criterion: Course
            var curriculumCourses = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
            var courseDt = new DataTable();
            courseDt.Columns.Add("CourseName");
            courseDt.Columns.Add("CourseID");

            foreach (var c in curriculumCourses)
            {
                courseDt.Rows.Add(c.Grade + "-" + c.CourseName, c.ID);
            }

            criteria.Add(new Criterion
            {
                Header = "Course",
                Key = "ClassCourses",
                DataSource = courseDt,
                DataTextField = "CourseName",
                DataValueField = "CourseID",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.DropDownList
            });
            #endregion

            #region Criterion: Cluster
            DataTable dtCluster = Thinkgate.Base.Classes.SchoolMasterList.GetClustersForUser(SessionObject.LoggedInUser).ToDataTable("Cluster");
            criteria.Add(new Criterion
            {
                Header = "Cluster",
                Key = "Cluster",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Cluster",
                DataValueField = "Cluster",
                Removable = true,
                DataSource = dtCluster,
                UIType = UIType.CheckBoxList
            });
            #endregion

            #region Criterion: School type
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
                DataTextField = "SchoolType",
                DataValueField = "SchoolType",
                DefaultValue = _selectedSchoolType,
                Removable = true,
                DataSource = schoolTypeDataTable,
                UIType = UIType.CheckBoxList,
                ServiceUrl = "../../Services/School.svc/GetAllSchoolsFromSchoolTypes",
                ServiceOnSuccess = "getAllSchoolsFromSchoolTypes",
                Dependencies = new[]
								{
										Criterion.CreateDependency("SchoolType", "SchoolTypes"),
										Criterion.CreateDependency("School", "Schools")
								}
            });
            #endregion

            #region Criterion: School
            var schoolDataTable = new DataTable();
            var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID", typeof(Int32));

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
                DataTextField = "Name",
                DataValueField = "ID",
                Removable = true,
                DataSource = schoolDataTable,
                UIType = UIType.CheckBoxList
            });
            #endregion

            #region Criterion: Teacher

            criteria.Add(new Criterion
            {
                Header = "Teacher",
                Key = "AssessmentAssignmentTeacher",
                //DataTextField = "CourseName",
                //DataValueField = "CourseID",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.TextBox

            });
            #endregion

            #region Criterion: Semester
            criteria.Add(new Criterion
            {
                Header = "Semester",
                Key = "Semester",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                Removable = true,
                DataTextField = "Semester",
                DataValueField = "Semester",
                DataSource = Thinkgate.Base.Classes.Semester.GetSemesterListForDropDown(),
                UIType = UIType.DropDownList
            });
            #endregion

            #region Criterion: Period

            criteria.Add(new Criterion
            {
                Header = "Period",
                Key = "Period",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Period",
                DataValueField = "Period",
                Removable = true,
                DataSource = Thinkgate.Base.Classes.PeriodMasterList.GetPeriodDataTableForDropDown("Period"),
                UIType = UIType.CheckBoxList

            });


            #endregion

            #region Criterion: Section
            criteria.Add(new Criterion
            {
                Header = "Section",
                Key = "Section",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                Removable = true,
                UIType = UIType.TextBox
            });
            #endregion

            #region Criterion: Block

            criteria.Add(new Criterion
            {
                Header = "Block",
                Key = "Block",
                //DataTextField = "CourseName",
                //DataValueField = "CourseID",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.TextBox

            });
            #endregion
            return criteria;
        }

        private void LoadSearchCriteriaControl()
        {
            var controlReportCriteria = (ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            controlReportCriteria.ID = "ctlSearchResultsCriteria";

            if (string.IsNullOrEmpty(hiddenTextBox.Text))
            {
                HiddenGuid = Guid.NewGuid().ToString();
                hiddenTextBox.Text = HiddenGuid;
                controlReportCriteria.Guid = HiddenGuid;
                controlReportCriteria.Criteria = LoadSearchCriteria();
                controlReportCriteria.FirstTimeLoaded = true;
            }
            else
            {
                HiddenGuid = hiddenTextBox.Text;
                controlReportCriteria.Guid = hiddenTextBox.Text;
                controlReportCriteria.FirstTimeLoaded = false;
            }

            controlReportCriteria.InitialButtonText = "Search";
            controlReportCriteria.ReloadReport += ReportCriteria_ReloadReport;

            criteraDisplayPlaceHolder.Controls.Add(controlReportCriteria);
        }

        private void ReportCriteria_ReloadReport(object sender, EventArgs e)
        {
            BindDataToGrid();
        }

        private void ParseRequestQueryString()
        {
            //if (string.IsNullOrEmpty(Request.QueryString["level"]))
            //{
            //    SessionObject.RedirectMessage = "No level provided in URL.";
            //    Response.Redirect("~/PortalSelection.aspx", true);
            //}

            //_level = (EntityTypes)EnumUtils.enumValueOf(Request.QueryString["level"], typeof(EntityTypes));
        }

        #endregion

        public void ExportToExcel(DataTable dt)
        {
            if (dt.Columns["ClassID"]!=null)
            dt.Columns.Remove("ClassID");
            // Create the workbook
            XLWorkbook workbook = ConvertDataTableToSingleSheetWorkBook(dt, "Results");

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
                Response.Clear();
                Response.Buffer = true;
                if (browser.Platform.IndexOf("WinNT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    workbook.SaveAs(memoryStream);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");
                    Response.BinaryWrite(memoryStream.ToArray());
                }
                else
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "text/csv";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=ExportData.csv");
                    byte[] csv = ExportToCSV.ConvertXLToCSV(workbook);
                    Response.BinaryWrite(csv);
                }

                Response.Flush();
                Response.End();

                Session["FileExport_Content" + PageGuid] = memoryStream.ToArray();
            }


        }

        protected void ExportGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            BindDataToGrid();
            if (GridDataTable != null && GridDataTable.Rows.Count > 0)
                ExportToExcel(GridDataTable);
        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }

        public void DisplayMessageInAlert(string text)
        {
            if (Master != null)
            {
                var radWinManager = Master.FindControl("RadWindowManager");

                if (radWinManager != null)
                {
                    ((RadWindowManager)radWinManager).RadAlert(text, null, 100, "Message", string.Empty);
                }
            }
        }

        public string AddPlurality(int count)
        {
            return count == 0 || count > 1 ? "s have" : " has";
        }

        [System.Web.Services.WebMethod]
        public static string GetUrlWithEncryptedIdForClassID(int classId)
        {
            if (classId < 0) return null;

            var encryptedClassId = Cryptography.EncryptIntWithoutCipherKey(classId);
            string url = string.Format("../../Record/Class.aspx?xID={0}", encryptedClassId);


            return url;
        }
    }
}
