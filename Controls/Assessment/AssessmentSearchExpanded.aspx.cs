using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria.CriteriaControls;
using Thinkgate.Controls.Reports;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Utilities;
using Thinkgate.Services.Contracts.LearningMedia;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentSearchExpanded : ExpandedSearchPage
    {
        public DataTable GridDataTable;
        public DataTable dtPrintDateAllTest_district;
        public DataTable dtPrintDateAllTest_school;
        public string PageGuid;
        public string Category;
        public string DataTableCount;
        public string ProofedTestCount;
        public static string _loggedOnUserRoleName = string.Empty;
        public static int _schoolId = 0;
        private EntityTypes _level;
        private DataTable dtLearningResources;
        private DataTable dtEducationalUse;
        private DataTable dtEndUsers;
        private DataTable dtCreativeCommons;
        private DataTable dtMediaTypes;
        private DataTable dtLanguages;
        private DataTable dtAgeAppropriate;
        private DataTable dtActivity;
        private DataTable dtStandardSet = new DataTable();
        private Guid _userId;
        private int _userPage;
        private Criteria _lrmiSearchValues;
        protected LrmiTagsSearch lrmiSearch;

        private Guid UserId;
        private int UserPage;
        private Criteria lrmiSearchValues;
        private string _category = string.Empty;
        private bool isSecuredFlag;
        protected Dictionary<string, bool> dictionaryItem;

        public List<int> ListOfTests
        {
            get { return (List<int>)Session["ListOfTests_" + PageGuid]; }
            set { Session["ListOfTests_" + PageGuid] = value; }
        }

        public List<int> ListOfSelectedTests
        {
            get { return (List<int>)Session["ListOfSelectedTests"]; }
            set { Session["ListOfSelectedTests"] = value; }
        }

        #region Properties

        public string HiddenGuid { get; set; }
        public string HiddenGuidLRMI { get; set; }
        public string clientFolder { get; set; }
        protected bool permAssessNameHyperLinksActive;

        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

#if DEBUG
            //clientFolder = "";
            clientFolder = !string.IsNullOrEmpty(AppSettings.AppVirtualPath) ? AppSettings.AppVirtualPath + "/" : "";
#else
            clientFolder = !string.IsNullOrEmpty(AppSettings.AppVirtualPath) ? AppSettings.AppVirtualPath + "/" : "";
#endif

            LoadSearchScripts();

        }

        private void CheckLrmiDisplay()
        {
            if (DistrictParms.LoadDistrictParms().LRMITagging != "true")
            {
                RadTabStripSearch.Tabs[1].Visible = false;
            }
            else
            {

                dtLearningResources =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.LearningResourceType, Enums.SelectionType.Assessment.ToString());
                dtEducationalUse =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.EducationalUse, Enums.SelectionType.Assessment.ToString());
                dtEndUsers =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.EndUser, Enums.SelectionType.Assessment.ToString());
                dtCreativeCommons = Base.Classes.Assessment.LoadCreativeCommonsForAssessmentLrmiSearch();
                dtMediaTypes =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.MediaType, Enums.SelectionType.Assessment.ToString());
                dtLanguages = GetLanguages();
                dtAgeAppropriate =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.AgeAppropriate, Enums.SelectionType.Assessment.ToString());
                dtActivity = Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.Activity, Enums.SelectionType.Assessment.ToString());

                drGeneric_String_String globalInputs;
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                {
                    Response.Redirect("~/PortalSelection.aspx", true);
                }
                else
                {
                    globalInputs = (drGeneric_String_String)HttpContext.Current.Session["GlobalInputs"];
                    foreach (var globalInput in globalInputs)
                    {
                        if (globalInput.Name.ToUpper() == "USERID")
                        {
                            UserId = new Guid(globalInput.Value);
                        }
                        else
                        {
                            if (globalInput.Name.ToUpper() == "USERPAGE")
                            {
                                UserPage = int.Parse(globalInput.Value);
                    }
                }
            }
        }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["category"] == null) return;

            CheckLrmiDisplay();

            if (!string.IsNullOrEmpty(hiddenSelectedCount.Text))
            {
                lblSelectedCount.InnerText = "Records selected: " + hiddenSelectedCount.Text;
                radBtnLock.Enabled = radBtnUnlock.Enabled = radBtnActivate.Enabled = radBtnDeactivate.Enabled = radBtnViewTestEvents.Enabled = true;
            }

            _level = !string.IsNullOrEmpty(Request.QueryString["level"]) ? (EntityTypes)Enum.Parse(typeof(EntityTypes), Request.QueryString["level"]) : EntityTypes.Teacher;

            Category = Request.QueryString["category"];

            if (!string.IsNullOrEmpty(hiddenGuidBox.Text))
            {
                PageGuid = hiddenGuidBox.Text;
            }
            else
            {
                PageGuid = Guid.NewGuid().ToString().Replace("-", "");
                hiddenGuidBox.Text = PageGuid;
            }

            switch (Category.ToLower())
            {
                case "classroom":
                    permAssessNameHyperLinksActive = UserHasPermission(Permission.Hyperlink_AssessmentNameClassroom);
                    break;

                case "district":
                    permAssessNameHyperLinksActive = UserHasPermission(Permission.Hyperlink_AssessmentNameDistrict);
                    break;

                case "state":
                    permAssessNameHyperLinksActive = UserHasPermission(Permission.Hyperlink_AssessmentNameState);
                    break;
            }

            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "anything", BuildStartupScript(exportGridImgBtn.ClientID, "../..", PageGuid), false);
                // get role name from the session object
                Thinkgate.Classes.SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                _loggedOnUserRoleName = sessionObject.LoggedInUser.Roles[0].RoleName;
                _schoolId = sessionObject.LoggedInUser.School;
            }
            ControlIconVisibility();

            ParseRequestQueryString();

            LoadSearchCriteriaControl();
            LoadLRMISearchControl();

            if (radGridResults.DataSource == null && string.IsNullOrEmpty(hiddenSelectedCount.Text))
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
            }

        }

        private void LoadSearchScripts()
        {
            if (Master != null)
            {
                RadScriptManager radScriptManager;
                var scriptManager = Master.FindControl("RadScriptManager2");
                if (scriptManager != null)
                {
                    radScriptManager = (RadScriptManager)scriptManager;

                    radScriptManager.Scripts.Add(new ScriptReference("~/Scripts/AssessmentSearch.js"));

                }

            }
            else
            {
                return;
            }
        }

        private void BindDataToGrid()
        {
            if (RadMultiPageSearch.SelectedIndex == 0)
            {
                BindDataToGridBasic();
            }
            else
            {
                BindDataToGridLrmi();
            }
        }

        private DataTable GetLanguages()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Enum");
            dt.Columns.Add("Description");
            var languageType = Enum.GetValues(typeof(LanguageType))
                .Cast<LanguageType>()
                .Select(v => v.ToString())
                .ToList();

            int i = 0;
            foreach (var lt in languageType)
            {
                DataRow dr = dt.NewRow();
                dr["Enum"] = i;
                dr["Description"] = lt;
                dt.Rows.Add(dr);
                i++;
            }

            return dt;
        }

        private void BindDataToGridLrmi()
        {
            
            GridDataTable = lrmiSearch.DoAssessmentSearchLrmi(UserId, UserPage);


            if (GridDataTable.Rows.Count > 0)
            {
                GridDataTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(GridDataTable, "TestID", "EncryptedID");
                ListOfTests = new List<int>();
                foreach (DataRow row in GridDataTable.Rows)
                {
                    ListOfTests.Add(DataIntegrity.ConvertToInt(row["TestID"]));
                }
            }

            dtPrintDateAllTest_district = Thinkgate.Base.Classes.Assessment.GetPrintSecurity_Secutity_Status_AllTest("Assessment", "District", ListOfTests);
            dtPrintDateAllTest_school = Thinkgate.Base.Classes.Assessment.GetPrintSecurity_Secutity_Status_AllTest("school", _schoolId.ToString(), ListOfTests);

            DataTableCount = GridDataTable.Rows.Count.ToString();
            ProofedTestCount = (from DataRow row in GridDataTable.Rows
                                where row["STATUS"].ToString() == "Proofed"
                                select row).Count().ToString();
            radGridResults.DataSource = GridDataTable;
            radGridResults.DataBind();
            initialDisplayText.Visible = false;
        }

        private string GetSearchCriteriaStringWithTable(DataTable lookupTable, string key, string tableTextField = "Description", string tableValuefield = "Enum")
        {

            List<string> values = lrmiSearchValues.CriterionList.FindAll(r => r.Key.Contains(key) && r.Object != null && !string.IsNullOrEmpty(r.ReportStringVal)).Select(criterion => criterion.Object.ToString()).ToList();
            StringBuilder sbValue = new StringBuilder();
            foreach (var specificValue in values)
            {
                DataRow[] records = lookupTable.Select("[" + tableTextField + "] = '" + specificValue + "'");
                if (records.Length > 0)
                {
                for (int rowId = 0; rowId < records.Length; rowId++)
                {
                    DataRow row = records[rowId];
                    if (sbValue.ToString() != string.Empty)
                    {
                        sbValue.Append(",");
                    }
                    sbValue.Append(row[tableValuefield]);
                }
            }
            }
            return sbValue.ToString();
        }

        private string GetSearchCriteriaString(string key)
        {

            List<string> values = lrmiSearchValues.CriterionList.FindAll(r => r.Key.Contains(key) && r.Object != null && !string.IsNullOrEmpty(r.ReportStringVal)).Select(criterion => criterion.Object.ToString()).ToList();
            StringBuilder sbValue = new StringBuilder();
            foreach (var specificValue in values)
            {
                if (sbValue.ToString() != string.Empty)
                {
                    sbValue.Append(",");
                }
                sbValue.Append(specificValue);
            }
            return sbValue.ToString();
        }

        private void BindDataToGridBasic()
        {
            var searchParms = (Criteria)Session["Criteria_" + HiddenGuid];

            List<string> testTypes = new List<string>();

            foreach (var criterion in searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Type") && r.Object != null && !string.IsNullOrEmpty(r.ReportStringVal)))
            {
                testTypes.Add(criterion.Object.ToString());

            }

            List<string> testTerms = new List<string>();

            foreach (var criterion in searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Terms") && r.Object != null && !string.IsNullOrEmpty(r.ReportStringVal)))
            {
                testTerms.Add(criterion.Object.ToString());

            }


            var year = searchParms.CriterionList.Find(r => r.Key == "Year" && !string.IsNullOrEmpty(r.ReportStringVal)) != null ?
                searchParms.CriterionList.Find(r => r.Key == "Year" && !string.IsNullOrEmpty(r.ReportStringVal)).Object.ToString() : string.Empty;

            List<string> grades = searchParms.CriterionList.FindAll(r => r.Key.Contains("Grade") && r.Object != null && !string.IsNullOrEmpty(r.ReportStringVal)).Select(criterion => criterion.Object.ToString()).ToList();

            List<string> subjects = searchParms.CriterionList.FindAll(r => r.Key.Contains("Subject") && r.Object != null && !string.IsNullOrEmpty(r.ReportStringVal)).Select(criterion => criterion.Object.ToString()).ToList();

            #region Get: CourseList
            var currCourseIDs = new CourseList();

            var selectedCurriculumCourse = searchParms.CriterionList.Find(r => r.Key == "Curriculum" && r.Empty == false && !string.IsNullOrEmpty(r.ReportStringVal));

            if (selectedCurriculumCourse != null)
            {
                currCourseIDs = new CourseList() { Thinkgate.Base.Classes.CourseMasterList.GetCurrCourseById(Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(selectedCurriculumCourse.ReportStringVal)) };
            }
            else
            {
                currCourseIDs = (grades.Count > 0 || subjects.Count > 0)
                                    ? Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(
                                        SessionObject.LoggedInUser).
                                          FilterByGradesAndSubjects(grades.Count > 0 ? grades : null,
                                                                    subjects.Count > 0 ? subjects : null)
                                    : Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(
                                        SessionObject.LoggedInUser);
            }
            #endregion


            var status = searchParms.CriterionList.Find(r => r.Header == "Status").ReportStringVal ?? string.Empty;

            var dateCreatedStart = searchParms.CriterionList.Find(r => r.Key == "CreatedDateStart").ReportStringVal ?? string.Empty;
            var dateCreatedEnd = searchParms.CriterionList.Find(r => r.Key == "CreatedDateEnd").ReportStringVal ?? string.Empty;

            var textSearch = searchParms.CriterionList.Find(r => r.Key == "TextSearch" && !string.IsNullOrEmpty(r.ReportStringVal)) != null ? searchParms.CriterionList.Find(r => r.Key == "TextSearch").Object.ToString() : string.Empty;
            bool textSearchExists = new bool();
            string searchOption = string.Empty;
            string searchText = string.Empty;

            if (textSearch.ToString().Contains(":"))
            {
                var tempArray = textSearch.ToString().Split(':');

                if (tempArray.Length == 2)
                {
                    textSearchExists = true;

                    searchText = tempArray[1];

                    switch (tempArray[0])
                    {
                        case "Any Words":
                            searchOption = "any";
                            break;
                        case "All Words":
                            searchOption = "all";
                            break;
                        case "Exact Phrase":
                            searchOption = "exact";
                            break;
                        case "Keywords":
                            searchOption = "key";
                            break;
                        case "Author":
                            searchOption = "author";
                            break;
                    }

                }
            }

            bool hasSecurePermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);

            GridDataTable = Thinkgate.Base.Classes.Assessment.SearchAssessments(currCourseIDs, SessionObject.LoggedInUser.HasPermission(Thinkgate.Base.Enums.Permission.User_Cross_Schools) ? "Yes" : "No",
                //CourseIDs
                Category, //Category
                testTypes.Count > 0 ? testTypes : new List<string>(),
                testTerms, //terms
                status, //status
                textSearchExists ? searchText : string.Empty, //textWords
                textSearchExists ? searchOption : string.Empty, //textWordsOpt
                string.IsNullOrEmpty(year) ? DistrictParms.LoadDistrictParms().Year : year, //yearList
                dateCreatedStart, //createdDateBegin
                dateCreatedEnd, //createdDateEnd
                null, //option
                SessionObject.LoggedInUser.Roles.Select(r => r.RoleName.ToString()).ToList(),
                hasSecurePermission); //userType

            if (GridDataTable.Rows.Count > 0)
            {
                GridDataTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(GridDataTable, "TestID", "EncryptedID");
                ListOfTests = new List<int>();
                foreach (DataRow row in GridDataTable.Rows)
                {
                    ListOfTests.Add(DataIntegrity.ConvertToInt(row["TestID"]));
                }
            }

            dtPrintDateAllTest_district = Thinkgate.Base.Classes.Assessment.GetPrintSecurity_Secutity_Status_AllTest("Assessment", "District", ListOfTests);
            dtPrintDateAllTest_school = Thinkgate.Base.Classes.Assessment.GetPrintSecurity_Secutity_Status_AllTest("school", _schoolId.ToString(), ListOfTests);
            
            
            
            DataTableCount = GridDataTable.Rows.Count.ToString();
            ProofedTestCount = (from DataRow row in GridDataTable.Rows
                                where row["STATUS"].ToString() == "Proofed"
                                select row).Count().ToString();
            radGridResults.DataSource = GridDataTable;
            radGridResults.DataBind();
            initialDisplayText.Visible = false;
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

            if (!string.IsNullOrEmpty(hiddenSelectedCount.Text))
            {
                lblSelectedCount.InnerText = "Records selected: " + hiddenSelectedCount.Text;
                radBtnLock.Enabled = radBtnUnlock.Enabled = radBtnActivate.Enabled = radBtnDeactivate.Enabled = radBtnViewTestEvents.Enabled = true;
            }
            else
            {
                lblSelectedCount.InnerText = "";
                radBtnLock.Enabled = radBtnUnlock.Enabled = radBtnActivate.Enabled = radBtnDeactivate.Enabled = radBtnViewTestEvents.Enabled = false;
            }
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
            var gradesByCurrCourses =
                Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetGradeList();
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
                ServiceUrl = "../../Services/GradeSubjectCourse.svc/GetCurrSubjectsAndCourses",
                ServiceOnSuccess = "getCurrGradesSubjectsAndCourses",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("Grade", "Grades"),
                    Criterion.CreateDependency("Subject", "Subjects"),
                    Criterion.CreateDependency("Curriculum", "Courses")
                }
            });
            #endregion

            #region Criterion: Subject
            var subjectsDT = new DataTable();
            var subjectsByCurrCourses =
                Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetSubjectList();
            subjectsDT.Columns.Add("Subject");

            foreach (var subjectText in subjectsByCurrCourses.Select(s => s.DisplayText).Distinct())
            {
                subjectsDT.Rows.Add(subjectText);
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
                ServiceUrl = "../../Services/GradeSubjectCourse.svc/GetCurrCourses",
                ServiceOnSuccess = "getCurrCoursesFromSubjects",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("Grade", "Grades"),
                    Criterion.CreateDependency("Subject", "Subjects"),
                    Criterion.CreateDependency("Curriculum", "Courses")
                }
            });
            #endregion

            #region Criterion: Course
            var currCourses =
                Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            var currcourseDt = new DataTable();
            currcourseDt.Columns.Add("CourseName");
            currcourseDt.Columns.Add("CourseID");
            currCourses.Sort((x, y) => String.Compare(x.Grade.DisplayText, y.Grade.DisplayText));
            foreach (var c in currCourses)
            {
                currcourseDt.Rows.Add(c.Grade + "-" + c.CourseName, c.ID);
            }

            criteria.Add(new Criterion
            {
                Header = "Course",
                Key = "Curriculum",
                DataSource = currcourseDt,
                DataTextField = "CourseName",
                DataValueField = "CourseID",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.DropDownList
            });
            #endregion

            #region Criterion: Category
            criteria.Add(new Criterion
            {
                Header = "Category",
                Key = "Category",
                Type = "String",
                Description = string.Empty,
                Locked = true,
                Removable = false,
                Object = Category,
                UIType = UIType.DropDownList,
                DataSource = Thinkgate.Base.Classes.Assessment.GetCategories(SessionObject.LoggedInUser.UserId),
                DataTextField = "TestCategory",
                DataValueField = "TestCategory"
            });
            #endregion

            #region Criterion: Type
            //CREATE TESTTYPE DT FOR DATASOURCE
            var typeDT = new DataTable();

            typeDT.Columns.Add("TypeText");
            typeDT.Columns.Add("TypeVal");

            
            List<string> typeListTable = new List<string>();
            dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(Category,true);
            isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            if (dictionaryItem != null && dictionaryItem.Select(c => c.Key).Any())
            {
                if (isSecuredFlag && UserHasPermission(Permission.Access_SecureTesting))
                    typeListTable = dictionaryItem.Select(c => c.Key).ToList();
                else
                    typeListTable = dictionaryItem.Where(c => !c.Value).Select(c => c.Key).ToList();
            }
            foreach (var testType in typeListTable)
            {
                typeDT.Rows.Add(testType, testType);
            }

            criteria.Add(new Criterion
            {
                Header = "Type",
                Key = "Type",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.CheckBoxList,
                DataSource = typeDT,
                DataTextField = "TypeText",
                DataValueField = "TypeVal",
                Removable = true
            });
            #endregion

            DataTable terms = new DataTable();
            terms.Columns.Add("Term");

            foreach (var str in Thinkgate.Base.Classes.Assessment.GetTerms())
            {
                // this is ridiculous, but will ultimatly be replaced so I'm adding this in so I can store the terms in cache as a list, not a datatable
                terms.Rows.Add(str);
            }

            #region Criterion: Terms
            criteria.Add(new Criterion
            {
                Header = "Terms",
                Key = "Terms",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.CheckBoxList,
                DataSource = terms,
                DataTextField = "Term",
                DataValueField = "Term",
                Removable = true
            });
            #endregion

            #region Criterion: Status
            var statusDT = new DataTable();
            var defaultStatusValue = "All";

            statusDT.Columns.Add("StatusText");
            statusDT.Columns.Add("StatusVal");

            if (Category.ToLower() == "district" && !UserHasPermission(Permission.Edit_AssessmentDistrict_Unproofed)
                || Category.ToLower() == "state" && !(DistrictParms.LoadDistrictParms().isStateSystem))
            {
                defaultStatusValue = "Proofed";
                statusDT.Rows.Add("Proofed", "Proofed");
            }
            else
            {
                statusDT.Rows.Add("All", "All");
                statusDT.Rows.Add("Proofed", "Proofed");
                statusDT.Rows.Add("Unproofed", "Unproofed");
            }

            criteria.Add(new Criterion
            {
                Header = "Status",
                Key = "Status",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.DropDownList,
                DataSource = statusDT,
                DataTextField = "StatusText",
                DataValueField = "StatusVal",
                Removable = false,
                Object = defaultStatusValue,
                ReportStringVal = defaultStatusValue
            });
            #endregion

            #region Criterion: Text Search
            var textSearchDT = new DataTable();

            textSearchDT.Columns.Add("SearchFieldKey");

            textSearchDT.Rows.Add("Any Words");
            textSearchDT.Rows.Add("All Words");
            textSearchDT.Rows.Add("Exact Phrase");
            textSearchDT.Rows.Add("Keywords");
            textSearchDT.Rows.Add("Author");

            criteria.Add(new Criterion
            {
                Header = "Text Search",
                Key = "TextSearch",
                Type = "String",
                UIType = UIType.AssessmentTextSearch,
                DataSource = textSearchDT,
                DataTextField = "SearchFieldKey",
                DataValueField = "SearchFieldKey",
                Removable = true
            });
            #endregion

            #region Criterion: Year
            criteria.Add(new Criterion
            {
                Header = "Year",
                Key = "Year",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.DropDownList,
                DataSource = Thinkgate.Base.Classes.Assessment.GetYears(),
                DataTextField = "Year",
                DataValueField = "Year",
                Removable = false,
                Object = DistrictParms.LoadDistrictParms().Year
            });
            #endregion

            #region Criterion: Created Date
            criteria.Add(new Criterion
            {
                Header = "Created Date",
                Key = "CreatedDate",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                UIType = UIType.DatePicker,
                Removable = true,
                IsHeader = true
            });

            criteria.Add(new Criterion
            {
                Header = "Created Date",
                Key = "CreatedDateStart",
                Type = "String",
                Description = string.Empty,
                IsHeader = false,
                Removable = true

            });

            criteria.Add(new Criterion
            {
                Header = "Created Date",
                Key = "CreatedDateEnd",
                Type = "String",
                Description = string.Empty,
                IsHeader = false,
                Removable = true

            });
            #endregion
            return criteria;
        }

        private Criteria LoadLRMISearchCriteria()
        {
            var criteria = new Criteria();

            #region Criterion: Grade
            var gradesDT = new DataTable();
            var gradesByCurrCourses =
                CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetGradeList();
            gradesDT.Columns.Add("Grades");

            foreach (var g in gradesByCurrCourses)
            {
                gradesDT.Rows.Add(g.DisplayText);
            }

            criteria.Add(new Criterion
            {
                Header = "Grades",
                Key = "Grades",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Grades",
                DataValueField = "Grades",
                Removable = true,
                DataSource = gradesDT,
                UIType = UIType.CheckBoxList,
                ServiceUrl = "../../Services/GradeSubjectCourse.svc/GetCurrSubjectsAndCourses",
                ServiceOnSuccess = "getCurrGradesSubjectsAndCourses",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("Grades", "Grades"),
                    Criterion.CreateDependency("Subjects", "Subjects")
                }
            });
            #endregion

            #region Criterion: Subject
            var subjectsDT = new DataTable();
            var subjectsByCurrCourses =
                Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetSubjectList();
            subjectsDT.Columns.Add("Subjects");

            foreach (var subjectText in subjectsByCurrCourses.Select(s => s.DisplayText).Distinct())
            {
                subjectsDT.Rows.Add(subjectText);
            }

            criteria.Add(new Criterion
            {
                Header = "Educational Subject",
                Key = "Subjects",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Subjects",
                DataValueField = "Subjects",
                Removable = true,
                DataSource = subjectsDT,
                UIType = UIType.CheckBoxList,
                ServiceUrl = "../../Services/GradeSubjectCourse.svc/GetCurrCourses",
                ServiceOnSuccess = "getCurrCoursesFromSubjects",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("Grades", "Grades"),
                    Criterion.CreateDependency("Subject", "Subjects"),
                    Criterion.CreateDependency("Curriculum", "Courses")
                }
            });
            #endregion

            criteria.Add(new Criterion
            {
                Header = "Learning Resource:",
                Key = "LearningResource",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Description",
                DataValueField = "Description",
                Removable = true,
                DataSource = dtLearningResources,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null
            });

            criteria.Add(new Criterion
            {
                Header = "Educational Use",
                Key = "EducationalUse",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Description",
                DataValueField = "Description",
                Removable = false,
                DataSource = dtEducationalUse,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null,
                DefaultValue = ((DataRow)dtEducationalUse.Rows[0])["Description"].ToString()
            });

            criteria.Add(new Criterion
            {
                Header = "End User",
                Key = "EndUser",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Description",
                DataValueField = "Description",
                Removable = false,
                DataSource = dtEndUsers,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null,
                DefaultValue = ((DataRow)dtEndUsers.Rows[0])["Description"].ToString()
            });

            criteria.Add(new Criterion
            {
                Header = "Creator",
                Key = "Creator",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.TextBox
            });

            criteria.Add(new Criterion
            {
                Header = "Publisher",
                Key = "Publisher",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.TextBox
            });

            criteria.Add(new Criterion
            {
                Header = "Usage Rights",
                Key = "UsageRights",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "SelectDescription",
                DataValueField = "SelectDescription",
                Removable = true,
                DataSource = dtCreativeCommons,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null
            });

            criteria.Add(new Criterion
            {
                Header = "Media Type",
                Key = "MediaType",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Description",
                DataValueField = "Description",
                Removable = true,
                DataSource = dtMediaTypes,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null
            });

            criteria.Add(new Criterion
            {
                Header = "Language",
                Key = "Language",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Description",
                DataValueField = "Description",
                Removable = true,
                DataSource = dtLanguages,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null
            });

            criteria.Add(new Criterion
            {
                Header = "Age Appropriate",
                Key = "AgeAppropriate",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Description",
                DataValueField = "Description",
                Removable = true,
                DataSource = dtAgeAppropriate,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null
            });

            //criteria.Add(new Criterion
            //{
            //    Header = "Time Required",
            //    Key = "Duration",
            //    Type = "String",
            //    Description = string.Empty,
            //    Locked = false,
            //    Removable = true,
            //    UIType = UIType.Duration
            //});

            //Assessed (Standard) search
            //criteria.Add(new Criterion
            //{
            //    UIType = UIType.GradeSubjectStandards,
            //    Header = "Assessed",
            //    Key = "Assessed",
            //    Locked = false,
            //    Removable = true,
            //    Description = string.Empty,
            //    Type = "String"
            //});

            criteria.Add(new Criterion
            {
                Header = "Text Complexity",
                Key = "TextComplexity",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.TextBox
            });

            criteria.Add(new Criterion
            {
                Header = "Reading Level",
                Key = "ReadingLevel",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.TextBox
            });

            criteria.Add(new Criterion
            {
                Header = "Interactivity Type:",
                Key = "Activity",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Description",
                DataValueField = "Description",
                Removable = true,
                DataSource = dtActivity,
                UIType = UIType.CheckBoxList,
                ServiceUrl = null,
                ServiceOnSuccess = null,
                Dependencies = null
            });

            return criteria;
        }

        private void LoadLRMISearchControl()
        {
            //AssessmentLRMITagSearch.ascx
            //LRMISearchPlaceholder
            lrmiSearch = (LrmiTagsSearch)LoadControl("~/Controls/E3Criteria/CriteriaControls/LrmiTagsSearch.ascx");
            lrmiSearch.ID = "ctlLRMISearchResultsCriteria";

            if (string.IsNullOrEmpty(hiddenLrmiTextBox.Text))
            {
                HiddenGuidLRMI = Guid.NewGuid().ToString();
                hiddenLrmiTextBox.Text = HiddenGuidLRMI;
                lrmiSearch.Guid = HiddenGuidLRMI;
                //lrmiSearch.Criteria = LoadLRMISearchCriteria();
                lrmiSearch.FirstTimeLoaded = true;
            }
            else
            {
                HiddenGuidLRMI = hiddenLrmiTextBox.Text;
                lrmiSearch.Guid = hiddenLrmiTextBox.Text;
                lrmiSearch.FirstTimeLoaded = false;
            }
            lrmiSearch.SearchType = LrmiTagsSearch.Type.Assessement;
            lrmiSearch.SearchCategory = Category;
            lrmiSearch.InitialButtonText = "Search";
            lrmiSearch.ReloadReport += ReportCriteria_ReloadReport;

            LRMISearchPlaceholder.Controls.Add(lrmiSearch);
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
            ClearSelection();
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

        protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
        {

            if (e.Item.ItemType == GridItemType.Header)
            {
                System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Item.FindControl("chkAll");
                chk.Attributes["onclick"] = "selectAll(this,'" + radGridResults.ClientID + "'," + ProofedTestCount + ");";

                if (hiddenChkAll.Text == "1" && string.IsNullOrEmpty(hiddenDeSelected.Text))
                    chk.Checked = true;
            }

            var item = e.Item as GridDataItem;
            if (item != null)
            {
                GridItem gridItem = e.Item;
                DataRowView row = (DataRowView)(gridItem).DataItem;
                GridDataItem gridDataItem = item;

                bool isSecureAssessment = Convert.ToBoolean(row["Secure"].ToString());

                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                Dictionary<string, bool> dictionaryItem;
                dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(row["TestCategory"].ToString());
                bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                if (hasPermission && isSecuredFlag && isSecureAssessment)
                {
                        var img = item.FindControl("imgIconSecure");
                        img.Visible = true;
                }

                System.Web.UI.HtmlControls.HtmlInputCheckBox chkBox = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("chkRowInput");
                if (row["STATUS"].ToString() == "Unproofed")
                    chkBox.Disabled = true;

                    IEnumerable<DataRow> tmpRow_distict = (from DataRow row1 in dtPrintDateAllTest_district.Rows
                                      where row1["TestID"].ToString() == row["TestID"].ToString()
                                      select row1);

                    IEnumerable<DataRow> tmpRow_school = (from DataRow row1 in dtPrintDateAllTest_school.Rows
                                     where row1["TestID"].ToString() == row["TestID"].ToString()
                                     select row1);
                // 6158:  Hide assessment in Teacher and School Portal if assessment security is inactive and does not have a date range
                // if TestEvents_DisplayInactiveDisctrictAssessments parm is turned on then no need to worry about
                // hiding assessments of inactive security status
                // if parm is turned off then check the security status and date range of the assessment

                if (DistrictParms.LoadDistrictParms().TestEvents_DisplayInactiveDistrictAssessments.Trim().ToLower() == "no")
                {
                    DataTable dtPrintEndDate = new DataTable();
                    // 7044: Hide assessment for teacher portal only
                    if (_level == EntityTypes.Teacher || (_level == EntityTypes.Class && (SessionObject.CurrentPortal == EntityTypes.Teacher)))
                    {
                        // scheduler level is school for teacher and school scheduler
                        if (tmpRow_school.ToList().Any())
                        dtPrintEndDate = tmpRow_school.CopyToDataTable<DataRow>();

                        // At this point, user has not set the security at the school level, now check the security status at assessment level
                        if (dtPrintEndDate.Rows.Count == 0 && tmpRow_distict.ToList().Any() )
                        {
                            dtPrintEndDate = tmpRow_distict.CopyToDataTable<DataRow>();
                        }
                        //}
                        //else
                        //{
                        //    // scheduler level is Assessment for district role
                        //    dtPrintEndDate = Thinkgate.Base.Classes.Assessment.GetSecurityStatus("Assessment", "District", Convert.ToInt32(row["TestID"].ToString()));
                        //}

                        if (dtPrintEndDate.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtPrintEndDate.Rows)
                            {
                                // If admin security is inactive and no date range
                                if (dr["admin_lock"].ToString() == "True" && (dr["admin_begin"].ToString() == string.Empty) && (dr["admin_end"].ToString() == string.Empty))
                                {
                                    gridDataItem.Display = false;
                                    return;
                                }

                            }
                        }
                    }
                }

                // 6362: the print icon for District Assessments( and State Assessments - NCCTE Only) 
                // should be controlled by the print window security settings.
                bool bPrintFlag = true;

                DataTable dtPrintDate = new DataTable();
                // print window is derived from district level for school and teacher portal
                if (tmpRow_distict.Any())
                {
                    dtPrintDate = tmpRow_distict.CopyToDataTable<DataRow>();
                }
                if (dtPrintDate.Rows.Count > 0 && !UserHasPermission(Permission.Icon_PrintIcon_SecurityOverride))
                {
                    foreach (DataRow dr in dtPrintDate.Rows)
                    {
                        // If print security status is inactive then print icon should be disbaled
                        if (dr["print_lock"].ToString() == "True")
                            bPrintFlag = false;
                        else if (dr["print_end"].ToString() != string.Empty || dr["print_begin"].ToString() != string.Empty)
                        {
                            DateTime printEndDate = Convert.ToDateTime(dr["print_end"].ToString()).Date;
                            DateTime printBeginDate = Convert.ToDateTime(dr["print_begin"].ToString()).Date;
                            // If the print end date is in past then gray out print icon
                            if (printEndDate < DateTime.Today.Date)
                                bPrintFlag = false;

                            if (printBeginDate > DateTime.Today.Date)
                                bPrintFlag = false;
                        }
                    }
                }

                #region Link Creation for Opening Assessment

                String testLinkUrl = "~/Record/AssessmentObjects.aspx?xID=" + (String)row["EncryptedID"];
                HyperLink lnk;
                bool isSloTeacherAndNonSloAssessment = false;
                var sloTeacher = SessionObject.LoggedInUser.Roles.Find(x => x.RoleName == "SLOTeach");
                if (sloTeacher != null && (row["ItemClassId"].ToString() != "2") && (Category.ToUpper() == "DISTRICT"))
                {
                    isSloTeacherAndNonSloAssessment = true;
                }
                /***********************************************************
                 * if user has permission then display link to assessment.  
                 * Otherwise only display label of assessment name.
                 * ********************************************************/
                if (permAssessNameHyperLinksActive && !isSloTeacherAndNonSloAssessment && (!Convert.ToBoolean(row["Targetted"]) || row["Author"].ToString().ToLower().Trim() == SessionObject.LoggedInUser.UserFullName.ToLower().Trim() || SessionObject.LoggedInUser.Roles.Find(r => r.RoleName.ToString().ToLower().Trim() == "school administrator") != null || SessionObject.LoggedInUser.Roles.Find(r => r.RoleName.ToString().ToLower().Trim() == "district administrator") != null))
                {
                    if ((lnk = (HyperLink)gridItem.FindControl("lnkListTestName")) != null)
                    {
                        lnk.Visible = true;
                        lnk.NavigateUrl = testLinkUrl;
                    }
                }
                else
                {
                    Label assessmentNameLabel;
                    if ((assessmentNameLabel = (Label)gridItem.FindControl("lblListTestName")) != null)
                    {
                        assessmentNameLabel.Visible = true;
                    }
                }

                System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("chkRowInput");
                chk.Attributes["onclick"] = "selectThisRow(this,'" + radGridResults.ClientID + "'," + item.ItemIndex + "," + ProofedTestCount + "," + row["TestID"] + ");";
                chk.Attributes.Add("rowIndex", item.ItemIndex.ToString());

                string[] arrUnChecked = hiddenDeSelected.Text.Split(',').ToArray();
                var foundUnChecked = arrUnChecked.FirstOrDefault(x => x.ToString() == row["TestID"].ToString());
                if (hiddenChkAll.Text == "1" && string.IsNullOrEmpty(foundUnChecked) && !(chk.Disabled))
                {
                    gridItem.Selected = true;
                    chk.Checked = true;
                }

                string[] arrChecked = hiddenSelected.Text.Split(',').ToArray();
                var foundChecked = arrChecked.FirstOrDefault(x => x.ToString() == row["TestID"].ToString());
                if (!string.IsNullOrEmpty(foundChecked) && !(chk.Disabled))
                {
                    gridItem.Selected = true;
                    chk.Checked = true;
                }

                #endregion

                string assessmentTitle = "Term " + row["Term"] + " " + row["TestType"] + " - " + row["Grade"] + " Grade " + row["Subject"] + (row["Course"].ToString() == row["Subject"].ToString() ? string.Empty : " " + row["Course"]);
                //ShowAndHideIcons
                #region Show Administration Button
                Base.Enums.Permission administPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Icon_Administration" + row["TestCategory"].ToString(), true);
                Boolean showAdministBtn = UserHasPermission(administPerm) && string.Compare(row["STATUS"].ToString(), "Proofed", true) == 0;
                System.Web.UI.HtmlControls.HtmlImage imgAdminBtn;
                if ((imgAdminBtn = (System.Web.UI.HtmlControls.HtmlImage)item.FindControl("imgGraphicAdmin")) != null)
                {
                    string onClientClickAdmin = "searchAsssessment_adminClick(" + "'" + row["EncryptedID"].ToString() + "'"
                                                                                                         + "," + "'" + assessmentTitle + "','" + Category + "','" + isSecureAssessment + "'" + ")";

                    imgAdminBtn.Visible = showAdministBtn;
                    if (showAdministBtn && row["DisplayDashboard"].ToString() == "No" && !UserHasPermission(Permission.Icon_AdministrationIcon_SecurityOverride))
                    {
                        imgAdminBtn.Attributes["onclick"] = "return false;";
                        imgAdminBtn.Attributes["style"] = "cursor:default; opacity:.3; filter:alpha(opacity=30);";
                    }
                    else
                    {
                        imgAdminBtn.Attributes["onclick"] = onClientClickAdmin;
                    }
                }
                #endregion

                #region Show Print Button
                Base.Enums.Permission assessmentPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_Assessment" + row["TestCategory"].ToString(), true);
                Base.Enums.Permission answerKeyPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_AnswerKey" + row["TestCategory"].ToString(), true);
                String printonclick = @"searchAsssessment_printClick(" + "'" + Standpoint.Core.Classes.Encryption.EncryptInt(DataIntegrity.ConvertToInt(row["TestID"])) + "'"
                                                                                                         + "," + "'" + row["TestName"].ToString() + "'" + ")";
                Boolean showPrint = (UserHasPermission(assessmentPerm) || UserHasPermission(answerKeyPerm)) && (row["ContentType"].ToString() != "No Items/Content");

                System.Web.UI.HtmlControls.HtmlImage imgBtn;
                //TFS: 6362
                imgBtn = (System.Web.UI.HtmlControls.HtmlImage)item.FindControl("imgGraphicPrint");
                //if ((imgBtn = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("imgGraphicPrint")) != null)
                if (imgBtn != null)
                {
                    if (bPrintFlag)
                    {
                        imgBtn.Attributes["onclick"] = printonclick;
                        imgBtn.Visible = showPrint;
                    }
                    else
                    {
                        imgBtn.Attributes["onclick"] = "return false;";
                        imgBtn.Attributes["style"] = "cursor:default; opacity:.3; filter:alpha(opacity=30);";
                    }
                }
                #endregion

                #region Show Edit Button
                Boolean editAssessment = UserHasPermission((Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Edit_Assessment" + row["TestCategory"].ToString(), true)) && string.Compare(row["STATUS"].ToString(), "Proofed", true) != 0;
                HyperLink imgOnclickLink = new HyperLink();
                string imgOnclickLinkString;
                imgOnclickLink.NavigateUrl = "~/" + clientFolder + "Record/AssessmentPage.aspx?xID=" + (string)row["EncryptedID"];
                imgOnclickLinkString = imgOnclickLink.ResolveClientUrl(imgOnclickLink.NavigateUrl);

                string onClientClick = @"searchAsssessment_editClick('" + row["EncryptedID"].ToString() + "','" + assessmentTitle + "', '" + imgOnclickLinkString + "'); ";

                System.Web.UI.HtmlControls.HtmlImage editBtn;
                if ((editBtn = (System.Web.UI.HtmlControls.HtmlImage)item.FindControl("btnGraphicEdit1")) != null)
                {
                    editBtn.Visible = editAssessment;
                    editBtn.Attributes["onclick"] = onClientClick;
                }
                #endregion

            }
        }

        public void ExportToExcel(DataTable dt)
        {
            ListOfSelectedTests = new List<int>();
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }
            }

            if (selectAll)
            {
                ListOfSelectedTests = ListOfTests;
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                string[] chkDeSelected = hiddenDeSelected.Text.Split(',');
                foreach (var item in ListOfTests)
                {
                    if (chkDeSelected.Where(s => s.Contains(item.ToString())).ToArray().Length <= 0)
                        ListOfSelectedTests.Add(item);
                }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                string[] chkSelected = hiddenSelected.Text.Split(',');
                foreach (var item in chkSelected)
                {
                    ListOfSelectedTests.Add(int.Parse(item));
                }
            }
            else
            {
                foreach (GridItem item in radGridResults.SelectedItems)
                {

                    if (item.FindControl("hiddenEncryptedID") != null)
                    {
                        var id = ((RadTextBox)item.FindControl("hiddenEncryptedID")).Text;
                        ListOfSelectedTests.Add(Cryptography.GetDecryptedIDFromEncryptedValue(id, SessionObject.LoggedInUser.CipherKey));
                    }
                }
            }

            if (ListOfSelectedTests.Count > 0)
            {
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (!ListOfSelectedTests.Contains(int.Parse(dt.Rows[i]["TestID"].ToString())))
                    {
                        dt.Rows[i].Delete();
                    }
                }
                dt.AcceptChanges();
            }

            // Create the workbook
            XLWorkbook workbook = ConvertDataTableToSingleSheetWorkBook(dt, "Results");

            //Prepare the response

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

        protected void LockAssessments_Click(object sender, EventArgs e)
        {

            List<int> idList = new List<int>();
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }

            }

            if (selectAll)
            {
                idList = ListOfTests;
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                string[] chkDeSelected = hiddenDeSelected.Text.Split(',');
                foreach (var item in ListOfTests)
                {
                    if (chkDeSelected.Where(s => s.Contains(item.ToString())).ToArray().Length <= 0)
                        idList.Add(item);
                }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                string[] chkSelected = hiddenSelected.Text.Split(',');
                foreach (var item in chkSelected)
                {
                    idList.Add(int.Parse(item));
                }
            }
            else
            {
                foreach (GridItem item in radGridResults.SelectedItems)
                {

                    if (item.FindControl("hiddenEncryptedID") != null)
                    {
                        var id = ((RadTextBox)item.FindControl("hiddenEncryptedID")).Text;
                        idList.Add(Cryptography.GetDecryptedIDFromEncryptedValue(id, SessionObject.LoggedInUser.CipherKey));
                    }


                }
            }


            if (idList.Count > 0)
            {
                var updatedCount = Thinkgate.Base.Classes.Assessment.ToggleAssessmentLock(idList, true);

                DisplayMessageInAlert(updatedCount + " assessment" + AddPlurality(updatedCount) + " been locked.");
            }

            ClearSelection();
            BindDataToGrid();

        }

        protected void RadGrid_NeedDataSource(object sender, EventArgs e)
        {
            HideGridColumns();
        }

        #region Control UI For Permissions
        public void HideGridColumns()
        {
            var columnPermissions = new Dictionary<string, Permission>() {
                {"Author",Permission.Column_Author_AssessmentSearch}, 
                {"DateCreated",Permission.Column_DateCreated_AssessmentSearch},
                {"DateUpdated",Permission.Column_LastUpdated_AssessmentSearch}
            };

            foreach (var pair in columnPermissions.Where(p => !SessionObject.LoggedInUser.HasPermission(p.Value)))
            {
                var col = radGridResults.Columns.FindByUniqueNameSafe(pair.Key);

                if (col != null)
                    col.Visible = false;
            }
        }

        public void ControlIconVisibility()
        {
            var hasActivatePermission = false;
            var hasLockPermission = false;

            switch (Category)
            {
                case "District":
                    hasActivatePermission = SessionObject.LoggedInUser.HasPermission(Permission.ActivateDeactivate_DistrictAssessment_AssessmentSearch);
                    hasLockPermission = SessionObject.LoggedInUser.HasPermission(Permission.LockUnlockContent_DistrictAssessment_AssessmentSearch);
                    break;

                case "Classroom":
                    hasActivatePermission = SessionObject.LoggedInUser.HasPermission(Permission.ActivateDeactivate_ClassroomAssessment_AssessmentSearch);
                    hasLockPermission = SessionObject.LoggedInUser.HasPermission(Permission.LockUnlockContent_ClassroomAssessment_AssessmentSearch);
                    break;

            }

            if (!hasActivatePermission)
            {
                divBtnActivate.Visible = false;
                divBtnDeactivate.Visible = false;
            }

            if (!hasLockPermission)
            {
                divBtnLock.Visible = false;
                divBtnUnlock.Visible = false;
            }
        }
        #endregion

        protected void UnlockAssessments_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }

            }

            if (selectAll)
            {
                idList = ListOfTests;
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                string[] chkDeSelected = hiddenDeSelected.Text.Split(',');
                foreach (var item in ListOfTests)
                {
                    if (chkDeSelected.Where(s => s.Contains(item.ToString())).ToArray().Length <= 0)
                        idList.Add(item);
                }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                string[] chkSelected = hiddenSelected.Text.Split(',');
                foreach (var item in chkSelected)
                {
                    idList.Add(int.Parse(item));
                }
            }
            else
            {
                foreach (GridItem item in radGridResults.SelectedItems)
                {

                    if (item.FindControl("hiddenEncryptedID") != null)
                    {
                        var id = ((RadTextBox)item.FindControl("hiddenEncryptedID")).Text;
                        idList.Add(Cryptography.GetDecryptedIDFromEncryptedValue(id, SessionObject.LoggedInUser.CipherKey));
                    }


                }
            }



            if (idList != null && idList.Count > 0)
            {
                var updatedCount = Thinkgate.Base.Classes.Assessment.ToggleAssessmentLock(idList, false);
                DisplayMessageInAlert(updatedCount + " assessment" + AddPlurality(updatedCount) + " been unlocked.");

            }

            ClearSelection();
            BindDataToGrid();
        }

        protected void ActivateAssessments_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }

            }

            if (selectAll)
            {
                idList = ListOfTests;
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                string[] chkDeSelected = hiddenDeSelected.Text.Split(',');
                foreach (var item in ListOfTests)
                {
                    if (chkDeSelected.Where(s => s.Contains(item.ToString())).ToArray().Length <= 0)
                        idList.Add(item);
                }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                string[] chkSelected = hiddenSelected.Text.Split(',');
                foreach (var item in chkSelected)
                {
                    idList.Add(int.Parse(item));
                }
            }
            else
            {
                foreach (GridItem item in radGridResults.SelectedItems)
                {

                    if (item.FindControl("hiddenEncryptedID") != null)
                    {
                        var id = ((RadTextBox)item.FindControl("hiddenEncryptedID")).Text;
                        idList.Add(Cryptography.GetDecryptedIDFromEncryptedValue(id, SessionObject.LoggedInUser.CipherKey));
                    }


                }
            }

            if (idList.Count > 0)
            {
                var updatedCount = Thinkgate.Base.Classes.Assessment.ToggleAssessmentActivation(idList, true);
                DisplayMessageInAlert(updatedCount + " assessment" + AddPlurality(updatedCount) + " been activated.");

            }

            ClearSelection();
            BindDataToGrid();
        }

        protected void DeactivateAssessments_Click(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }

            }

            if (selectAll)
            {
                idList = ListOfTests;
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                string[] chkDeSelected = hiddenDeSelected.Text.Split(',');
                foreach (var item in ListOfTests)
                {
                    if (chkDeSelected.Where(s => s.Contains(item.ToString())).ToArray().Length <= 0)
                        idList.Add(item);
                }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                string[] chkSelected = hiddenSelected.Text.Split(',');
                foreach (var item in chkSelected)
                {
                    idList.Add(int.Parse(item));
                }
            }
            else
            {
                foreach (GridItem item in radGridResults.SelectedItems)
                {

                    if (item.FindControl("hiddenEncryptedID") != null)
                    {
                        var id = ((RadTextBox)item.FindControl("hiddenEncryptedID")).Text;
                        idList.Add(Cryptography.GetDecryptedIDFromEncryptedValue(id, SessionObject.LoggedInUser.CipherKey));
                    }


                }
            }

            if (idList.Count > 0)
            {
                var updatedCount = Thinkgate.Base.Classes.Assessment.ToggleAssessmentActivation(idList, false);
                DisplayMessageInAlert(updatedCount + " assessment" + AddPlurality(updatedCount) + " been deactivated.");

            }

            ClearSelection();
            BindDataToGrid();
        }

        protected void ViewTestEvents_Click(object sender, EventArgs e)
        {
            ListOfSelectedTests = new List<int>();
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }
            }

            if (selectAll)
            {
                ListOfSelectedTests = ListOfTests;
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                string[] chkDeSelected = hiddenDeSelected.Text.Split(',');
                foreach (var item in ListOfTests)
                {
                    if (chkDeSelected.Where(s => s.Contains(item.ToString())).ToArray().Length <= 0)
                        ListOfSelectedTests.Add(item);
                }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                string[] chkSelected = hiddenSelected.Text.Split(',');
                foreach (var item in chkSelected)
                {
                    ListOfSelectedTests.Add(int.Parse(item));
                }
            }
            else
            {
                foreach (GridItem item in radGridResults.SelectedItems)
                {

                    if (item.FindControl("hiddenEncryptedID") != null)
                    {
                        var id = ((RadTextBox)item.FindControl("hiddenEncryptedID")).Text;
                        ListOfSelectedTests.Add(Cryptography.GetDecryptedIDFromEncryptedValue(id, SessionObject.LoggedInUser.CipherKey));
                    }
                }
            }

            if (ListOfSelectedTests.Count > 0)
            {
                RadButton btnViewMultipleTestEvents = (RadButton)sender;
                if ((btnViewMultipleTestEvents = (RadButton)this.divBtnViewTestEvents.FindControl("radBtnViewTestEvents")) != null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(string), "ViewMultipleTestEvents",
                        "parent.customDialog({ title: \"Assessment Assignments: Test Events for multiple Assessments\", maxheight: 675, maxwidth: 950, maximize:true, url: \"../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?encrypted=true&multipleAssessment=true&testcategory= " + Category.ToString() +
                        "&assessmentID= " + Standpoint.Core.Classes.Encryption.EncryptInt(ListOfSelectedTests[0]).ToString() + "\" });; ", true);
                }
            }
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
                    ((RadWindowManager)radWinManager).RadAlert(text, null, 100, "Message", "");

                }
            }

        }

        public string AddPlurality(int count)
        {
            return count == 0 || count > 1 ? "s have" : " has";
        }

        public void ClearSelection()
        {
            // Clear checked-unchecked status of the checkboxes
            lblSelectedCount.InnerText = string.Empty;
            hiddenChkAll.Text = string.Empty;
            hiddenSelected.Text = string.Empty;
            hiddenDeSelected.Text = string.Empty;
            hiddenSelectedCount.Text = string.Empty;
            hiddenTotalCount.Text = string.Empty;

            // Disable action buttons
            radBtnLock.Enabled = false;
            radBtnUnlock.Enabled = false;
            radBtnActivate.Enabled = false;
            radBtnDeactivate.Enabled = false;
            radBtnViewTestEvents.Enabled = false;
        }

        
    }
}
