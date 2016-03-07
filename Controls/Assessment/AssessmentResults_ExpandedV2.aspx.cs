using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Text.RegularExpressions;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Web.Script.Serialization;
using Thinkgate.Controls.E3Criteria;
using System.Text;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Enums.AssessmentScheduling;
using Thinkgate.Domain.Classes;
using Thinkgate.Utilities;
using Thinkgate.Services.Contracts.Groups;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentResults_ExpandedV2 : BasePage
    {        
        private int _userPage = 119;
        private int _userPageForProcedure;        
        private CourseList _currCourseList;
        private List<string> _curriculumIDList = new List<string>();
        //private bool is1Curriculum;

        private string _level;
        private int _levelID;
        private string _category;
        private string _term;
        private string _type;
        private List<string> _years;
        private string _groups;
        private int _classID;
        private string _testID;
        private EnvironmentParametersViewModel _enviromentParameter;
        private DistrictParms _dp;
        private RolePortal _roleportalID;

        private CourseList CurrCourseList
        {
            get
            {
                if (_currCourseList == null) LoadCurrCourseList();
                return _currCourseList;
            }
        }

        private DistrictParms _districtParms
        {
            get
            {
                if (_dp == null) _dp = DistrictParms.LoadDistrictParms();
                return _dp;
            }
        }
        public string ListTypes ; 
        protected new void Page_Init(object sender, EventArgs e)
        {
            _enviromentParameter = new EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            Master.Search += SearchHandler;
            base.Page_Init(sender, e);

            LoadReportScripts();

            //_dp = DistrictParms.LoadDistrictParms();

            //Defaults
            if (!IsPostBack)
            {
                _category = "District";
                _term = "All";
                _groups = "All";
                _type = "All";
            }

            //Set hidden input to current district year for javascript logic
            districtYear.Value = _districtParms.Year;
            
            _levelID = 0;

            if (!IsPostBack && Request.QueryString["category"] != null && !String.IsNullOrEmpty(Request.QueryString["category"]))
                _category = Request.QueryString["category"].ToString();

            if (Request.QueryString["term"] != null && !String.IsNullOrEmpty(Request.QueryString["term"]) && Request.QueryString["term"].ToString() != "Term")
                _term = Request.QueryString["term"].ToString();

            if (Request.QueryString["groups"] != null && !String.IsNullOrEmpty(Request.QueryString["groups"]) && Request.QueryString["groups"].ToString() != "Groups")
                _groups = Request.QueryString["groups"].ToString();

            if (Request.QueryString["type"] != null && !String.IsNullOrEmpty(Request.QueryString["type"]) && Request.QueryString["type"].ToString() != "Type")
                _type = Request.QueryString["type"].ToString();

            if (Request.QueryString["level"] != null && !String.IsNullOrEmpty(Request.QueryString["level"]))
                _level = Request.QueryString["level"];

            if (Request.QueryString["levelID"] != null && !String.IsNullOrEmpty(Request.QueryString["levelID"]))
                _levelID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "levelID");

            if (Request.QueryString["testID"] != null && !String.IsNullOrEmpty(Request.QueryString["testID"]))
                _testID = Request.QueryString["testID"];

            Base.Classes.Course currCourse = null;

            if (!IsPostBack)
            {
                LoadCurrCourseList();
                var serializer = new JavaScriptSerializer();
               //Removing condition for multi course id for a user. Multi course id could not show the selection. In this case user don't know the class course/subject.
                if (_curriculumIDList.Count>0){
                    CourseMasterList.CurrCourseDict.TryGetValue(DataIntegrity.ConvertToInt(_curriculumIDList[0]), out currCourse);
                    if (currCourse != null)
                    {
                        ctrlGradeSubjectCourseStandardSet.ChkGrade.DefaultTexts = new List<string>()
                                                                                      {currCourse.Grade.ToString()};

                        ctrlGradeSubjectCourseStandardSet.ChkSubject.DefaultTexts = new List<string>() { currCourse.Subject.ToString() };

                        ctrlGradeSubjectCourseStandardSet.CmbCourse.DefaultTexts = new List<string>()
                                                                                       {currCourse.CourseName};
                    }
                }
                
                ctrlGradeSubjectCourseStandardSet.JsonDataSource = serializer.Serialize(CurrCourseList.BuildJsonArray(false));
                ctrlGradeSubjectCourseStandardSet.ChkStandardSet.Visible = false;           // don't need StandardSet for curriculum
                ctrlGradeSubjectCourseStandardSet.CmbCourse.Text = "Curriculum";
                ctrlGradeSubjectCourseStandardSet.ChkGrade.Required = true;
                ctrlGradeSubjectCourseStandardSet.ChkSubject.Required = true;
                ctrlGradeSubjectCourseStandardSet.CmbCourse.Required = true;
                ctrlGradeSubjectCourseStandardSet.ChkGrade.RestrictValueCount = CriteriaBase.RestrictValueOptions.OnlyOneAppliedAtATime;
                
                cblYear.DataSource = Base.Classes.Assessment.GetYears();
                cblYear.DefaultTexts = new List<string> { _districtParms.Year };

                cblTerm.DataSource = new List<string> {"All"}.Concat(Base.Classes.Assessment.GetTerms());
                cmbCategory.DataSource = new List<string> {"Classroom", "District", "State"};
                cmbCategory.DefaultTexts = new List<string> {_category};

                 _roleportalID = (RolePortal)(SessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection));
                 GroupsProxy prxGroups = new GroupsProxy();
                 List<GroupDataContract> lstGroups =
                     prxGroups.GetGroupsForUser(
                         SessionObject.LoggedInUser.Page,
                         DistrictParms.LoadDistrictParms().ClientID);

                 if (lstGroups != null && lstGroups.Count > 0)
                 {
                     cblGrps.DataSource = lstGroups;
                     cblGrps.DataTextField = "Name";
                     cblGrps.DataValueField = "ID";
                 }
                 else
                 {
                     cblGrps.EmptyMessage = "No groups available";
                 }

                 
                 var allCategories = Base.Classes.TestTypes.GetTypes();
                ListTypes = GetTypeList(allCategories);
                Dictionary<string, bool> typeDictionary = TestTypes.TypeWithSecureFlag((string.IsNullOrEmpty(_category) ? "District" : _category));
                List<string> typeListTable = new List<string>();
                if (typeDictionary != null && typeDictionary.Any())
                {
                    if (!UserHasPermission(Permission.Access_SecureTesting)
                        || !UserHasPermission(Permission.Access_SecureTesting_viewAssessmentResults))
                    {
                        typeListTable =
                            typeDictionary.Where(x => !x.Value).Select(x => x.Key).ToList();
                    }
                    else
                    {
                        typeListTable = typeDictionary.Select(x => x.Key).ToList();
                    }
                }
                if (typeListTable != null &&  typeListTable.Any())
                {
                    typeListTable.Add("All");
                    typeListTable.Sort();
                }
                cmbType.DataSource = typeListTable;
                cmbType.DefaultTexts = new List<string> { _type };

             }

            /* end new code *****/

            if ((currCourse == null || (_level != "Teacher" && _level != "Class"))
                && string.IsNullOrEmpty(_testID))
            {
                return;
            }
            SearchAndClear.RunSearchOnPageLoad = true;
        }

         

        private void LoadReportScripts()
        {
            if (Page.Master != null)
            {
                var scriptManager = Page.Master.FindControl("RadScriptManager2");
                if (scriptManager != null)
                {
                    RadScriptManager radScriptManager = (RadScriptManager)scriptManager;

                    radScriptManager.Scripts.Add(
                        new ScriptReference("~/Scripts/AssessmentResults.js"));
                }
            }
        }

        public string GetTypeList(List<string> types)
        {
            string sType = string.Empty;
            foreach (string type in types)
            {
                if(string.IsNullOrEmpty(sType))
                    sType = type;
                else
                    sType = sType + "^" + type;
            }
            return sType;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_districtParms.SuggestedResources) && _districtParms.SuggestedResources.Trim().Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
            {
                IsSuggestedResourcesVisible.Value = "true";
            }
            try
            {
                if (IsPostBack) SetPrivateVariables(Master.CurrentCriteria());
            } catch (CriteriaNotLoaded err)
            {
                
            }

            string js = "toggleLockedColumnDisplay(" + chkUnlockedColumns.Checked.ToString().ToLower() + ");";
            var rndNumber = new Random();
            string jsScriptName = "AssessmentResultsReload" + rndNumber.Next();
            System.Web.UI.ScriptManager.RegisterStartupScript(Page, this.GetType(), jsScriptName, js, true);
        }

        private void SetPrivateVariables(CriteriaController criteriaController)
        {
            /* Year */
            _years = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Year").Select(x => x.Text).ToList();
            if (_years.Count < 1) _years.Add(_districtParms.Year);

            if (IsPostBack)
            {
                _term = String.Join(",", criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Term").Select(x => x.Text)); // multiple terms allowed
                _category = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Category").Select(x => x.Text).FirstOrDefault();  // only 1 category
                _type = String.Join(",", criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Type").Select(x => x.Text)); // multiple types allowed
                _groups = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Groups").Select(x => x.Value).FirstOrDefault(); // only 1 group
            }
        }

        private void ExecuteResults()
        {
            string environmentName = System.Configuration.ConfigurationManager.AppSettings.Get("Environment");

            var dev = environmentName == "Dev" ? true : false;

            var criteriaController = Master.CurrentCriteria();      // just going to go ahead and pull this from master instead of from search handler so it will work on tree updates

            SetPrivateVariables(criteriaController);

            /* Grade */
            var selectedGrades = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Grade").Select(x => x.Text).ToList();

            /* Subject */
            var selectedSubjects = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Subject").Select(x => x.Text).ToList();

            /* Curriculum */
            var selectedCourseNames = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Course").Select(x => x.Text).ToList();
            if (selectedGrades.Count == 0 || selectedSubjects.Count==0 || selectedCourseNames.Count == 0)
            {
                Master.NotifyOfMissingCritiera();
                resultsPanel.Visible = false;
                initMessage.Visible = true;
                return;
            }

            //var selectedGroups = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Groups").Select(x => x.Value).ToList();

            //var sbGrps = new StringBuilder();
            //foreach (var grp in selectedGroups)
            //{
            //    sbGrps.Append(grp + ",");
            //}
            //string grps = selectedGroups.Count > 0 ? sbGrps.Remove(sbGrps.Length - 1, 1).ToString() : string.Empty;

            var selectedCurriculum = CurrCourseList.FilterByGradesSubjectsStandardSetsAndCourse(selectedGrades, selectedSubjects, null, selectedCourseNames);
            var curriculum = selectedCurriculum[0].ID;
            
            //Demographics
            var sb = new StringBuilder();
            var selectedDemographics = criteriaController.ParseCriteria<E3Criteria.Demographics.ValueObject>("Demographics");
            foreach (var demo in selectedDemographics)
            {
                sb.Append("@@D" + demo.DemoField + "=" + demo.DemoValue + "@@");
            }
            string demoString = sb.ToString();

            if (String.IsNullOrEmpty(_term)) _term = "All";
            if (String.IsNullOrEmpty(_type)) _type = "All";
            if (String.IsNullOrEmpty(_category)) _category = Request.QueryString["category"];
            if (String.IsNullOrEmpty(_groups)) _groups = Request.QueryString["group"];

            switch (_category)
            {
                case "State":
                    IsStudentResponseVisible.Value = UserHasPermission(Permission.Reports_StudentResponses_State) ? "true" : "false";
                    break;
                case "District":
                    IsStudentResponseVisible.Value = UserHasPermission(Permission.Reports_StudentResponses_District) ? "true" : "false";
                    break;
                case "Classroom":
                    IsStudentResponseVisible.Value = UserHasPermission(Permission.Reports_StudentResponses_Classroom) ? "true" : "false";
                    break;
            }

            var School = "All";
            var Student = "slist";
            var userPage = dev ? _userPage : _userPageForProcedure;
            var levelLabel = "none";

            switch(_level)
            {
                case "School":
                    levelLabel = "SCH";
                    break;
                case "Teacher":
                    levelLabel = "TEAID";
                    break;
                case "Class":
                    levelLabel = "CID";
                    break;
            }

            string years = string.Join(",", _years.ToArray());

            string CritOrides = dev ? "@@ADMINYEAR=" + years + "@@" + levelLabel + "=" + _levelID + "@@TYRS=" + years + "@@TTERMS=" + _term + "@@TTYPES=" + _type + "@@CURR=" + curriculum +
                "@@TLISTADD=" + lockedTestIDs.Value + "@@" + demoString + "@@TestID=" + _testID + "@@" + "@@GRP=" + _groups  + "@@"
                : "@@ADMINYEAR=" + years + "@@@@" + levelLabel + "=" + _levelID + "@@TYRS=" + years + "@@TTERMS=" + _term + "@@TTYPES=" + _type + "@@CURR=" + curriculum +
                "@@TLISTADD=" + lockedTestIDs.Value + "@@" + demoString + "@@TestID=" + _testID + "@@" + "@@GRP=" + _groups + "@@";

            yearList.Value = years; //Set hidden input for javascript logic when determining whether to re-roster or not.

            var dataSet = new DataSet();
            switch(_level)
            {
                case "District":
                case "School":
                case "Assessment":
                    dataSet = AssessmentResultsPortal.LoadResults(userPage, _category, CritOrides);
                    break;
                case "Teacher":
                case "Class":
                    dataSet = AssessmentResultsPortal.LoadResults(years, School, Student, _classID, userPage, _category, CritOrides);
                    break;
            }

            if (dataSet == null || dataSet.Tables.Count < 4) return;

            SessionObject.TeacherPortal_assessmentRollup = dataSet.Tables[0];
            SessionObject.TeacherPortal_resultsData = dataSet.Tables[1];
            SessionObject.TeacherPortal_chartData = dataSet.Tables[2];
            SessionObject.TeacherPortal_heirarchyData = dataSet.Tables[3]; //2
             
        }

        private void LoadCurrCourseList()
        {
            if (_currCourseList != null) return;
            Base.Classes.Staff teacherObj;
            ThinkgateUser oUser;
            var testSubjectDT = Subject.GetSubjectsByTests();

            switch(_level)
            {
                case "Class":
                    _classID = _levelID;
                    var classObj = Base.Classes.Class.GetClassByID(_levelID, SessionObject.LoggedInUser.Page);
                    _curriculumIDList = classObj.CurriculumIDList;
                    //is1Curriculum = _curriculumIDList.Count == 1;
                    classObj.LoadTeachers();
                    var classTeacher = classObj.Teachers.Find(t => t.IsPrimary);
                    if (classTeacher != null)
                    {
                        teacherObj = Base.Classes.Staff.GetStaffByID(classTeacher.PersonID);
                        oUser = ThinkgateUser.GetThinkgateUserByID(teacherObj.UserID);
                        _currCourseList = CourseMasterList.GetCurrCoursesForUser(oUser);
                        _userPageForProcedure = classTeacher.PersonID;
                    }
                    else
                    {
                        _currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
                        _userPageForProcedure = SessionObject.LoggedInUser.Page;
                    }
                    break;
                case "Teacher":
                    teacherObj = Base.Classes.Staff.GetStaffByID(_levelID);
                    oUser = ThinkgateUser.GetThinkgateUserByID(teacherObj.UserID);
                    _currCourseList = CourseMasterList.GetCurrCoursesForUser(oUser);
                    _userPageForProcedure = _levelID;
                    break;
                default:
                    _currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
                    _userPageForProcedure = SessionObject.LoggedInUser.Page;
                    portalType.Value = "Admin"; //Set hidden input for javascript logic when determining whether to re-roster or not.
                    
                    break;
            }

            _currCourseList.RemoveAll(course => { return testSubjectDT.Select("Subject = '" + course.Subject.DisplayText + "'").Length == 0; });
        }

        /// <summary>
        /// Handler for the Search Button
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            DoSearch();
        }

        private void DoSearch(bool excelExport = false)
        {
            resultsPanel.Visible = true;
            initMessage.Visible = false;

            SessionObject.TeacherPortal_assessmentRollup = null;
            SessionObject.TeacherPortal_resultsData = null;
            SessionObject.TeacherPortal_chartData = null;
            SessionObject.TeacherPortal_heirarchyData = null;

            radTreeResults.Columns.Clear();
            AddInitialColumnsToTree();
            
            radTreeResults.DataSource = null;
            radTreeResults.DataSource = GetDataTable();
            AddTestColumns();
            radTreeResults.DataBind();

            switch (_level)
            {
                case "State":
                case "District":
                    radTreeResults.ExpandToLevel(1);
                    break;
                case "School":
                    radTreeResults.ExpandToLevel(2);
                    break;
                default:
                    radTreeResults.ExpandToLevel(3);
                    break;
            }
            
            SetChartTitles();
        }

        private void AddInitialColumnsToTree()
        {
            var levelColumn = new TreeListBoundColumn();
            radTreeResults.Columns.Add(levelColumn);
            levelColumn.DataField = "LevelName";
            levelColumn.UniqueName = "LevelName";
            levelColumn.HeaderText = "Name";
            levelColumn.HeaderStyle.Width = 200;
            levelColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            var multiSelectColumn = new TreeListBoundColumn();
            radTreeResults.Columns.Add(multiSelectColumn);
            multiSelectColumn.UniqueName = "MultiSelect";
            multiSelectColumn.HeaderText = "";
            multiSelectColumn.HeaderStyle.Width = 30;
        }

        private void SetChartTitles()
        {
            if (SessionObject.TeacherPortal_chartData == null) return;
            var chartData = SessionObject.TeacherPortal_chartData;
            if (chartData.Rows.Count == 0) return;
            var chartDataRow = chartData.Rows[0];

            if (chartDataRow["PerformanceLevels"].ToString().Length > 0)
            {
                performanceLevelDiv.InnerHtml = "<table><tr>" + chartDataRow["PerformanceLevels"].ToString()
                    + "</tr></table>";
            }
        }

        public DataTable GetDataTable()
        {
            if (SessionObject.TeacherPortal_heirarchyData == null)
                ExecuteResults();

            return SessionObject.TeacherPortal_heirarchyData;
        }


        private void AddTestColumns()
        {
            if (SessionObject.TeacherPortal_assessmentRollup == null) return;

            var assessmentData2 = SessionObject.TeacherPortal_assessmentRollup;

            foreach (DataRow test in assessmentData2.Rows)
            {
                var dateTimeCalculated = GetTestDateTimeCalculated(test["TestID"].ToString());
                if (string.IsNullOrEmpty(test["TestID"].ToString())) continue;
                var column = new TreeListNumericColumn();
                var lockedTestIDsList = "," + lockedTestIDs.Value + ",";
                var isLockedColumn = lockedTestIDsList.IndexOf("," + test["TestID"] + ",") > -1;
                var lockImageName = isLockedColumn ? "lock" : "unlock";

                radTreeResults.Columns.Add(column);
                column.DataField = "test_" + test["TestID"].ToString();
                column.HeaderText = "<table style=\"display:inline;\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td><a title='" + test["Description"] + " as of " + dateTimeCalculated + "'>"
                    + test["TestName"].ToString() + "<br/>as of " + dateTimeCalculated + 
                    "</a></td><td style=\"vertical-align:top;\"><img src=\"../../Images/" + lockImageName + ".gif\" style=\"cursor:pointer;\" onclick=\"toggleColumnLock(this, '" + test["TestID"] + "'); return false;\" /></td>"
                    + "</tr></table>";
                column.NumericType = NumericType.Percent;
                column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                column.MinWidth = Unit.Parse("100");
                column.MaxWidth = Unit.Parse("200");
            }
        }

        private string GetTestDateTimeCalculated(string testID)
        {
            if (SessionObject.TeacherPortal_resultsData == null) return "N/A";
            var resultsData = SessionObject.TeacherPortal_resultsData;
            if (resultsData.Rows.Count == 0) return "N/A";

            foreach (DataRow r in resultsData.Rows)
                if (r["TestID"].ToString().Equals(testID) && r["DateTimeCalculated"].ToString().Length > 0 && (r["Level"].ToString().Equals("State") || r["Level"].ToString().Equals("District")))
                    return (Standpoint.Core.Utilities.DataIntegrity.ConvertToDate(r["DateTimeCalculated"])).ToShortDateString();

            return "N/A";
        }

        #region Control Page Events

        protected void radTreeResults_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {
            if(e.Item is TreeListHeaderItem)
            {
                var header = (TreeListHeaderItem)e.Item;
                var lockedTestIDsArr = lockedTestIDs.Value.Split(',');
                foreach(TableCell cell in header.Cells)
                {
                    foreach(string testID in lockedTestIDsArr)
                    {
                        if (cell.Text.IndexOf("'" + testID + "'") > -1)
                        {
                            cell.Text = cell.Text.Replace("unlock.gif", "lock.gif");
                        }
                    }
                }
            }
            if (!(e.Item is TreeListDataItem)) return;
            if (SessionObject.TeacherPortal_resultsData == null) return;

            var resultsData = SessionObject.TeacherPortal_resultsData;
            var assessmentData2 = SessionObject.TeacherPortal_assessmentRollup;
            var levelData = SessionObject.TeacherPortal_heirarchyData;
            var cellIndex = e.Item.Cells.Count - assessmentData2.Rows.Count;
            var nameCellIndex = cellIndex-1;
            var dataItem = (DataRowView)((TreeListDataItem)e.Item).DataItem;
            var multiSelectTestIDs = "";
            var levelIDEncrypted = "";
            var lockedTestIDsList = "," + lockedTestIDs.Value + ",";
            string ClassID_TrackedDown;
            string isContentLocked = "False";
            string contentLock;

            if (cellIndex < 0) return;

            e.Item.Cells[cellIndex].Attributes["style"] = "text-align:center;";

            for (var i = 0; i < cellIndex; i++ )
            {
                e.Item.Cells[i].HorizontalAlign = HorizontalAlign.Left;
            }

            //Add link to open student
            if (dataItem["Level"].ToString() == "Student")
            {
                e.Item.Cells[cellIndex - 1].Attributes.Add("onclick", "window.open('../../Record/Student.aspx?childPage=yes&xID=" + Standpoint.Core.Classes.Encryption.EncryptString(dataItem["LevelID"].ToString()) + "');");
                e.Item.Cells[cellIndex - 1].Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");
            }

                foreach (DataRow test in assessmentData2.Rows)
                {
                    // TFS 1123 : Check the content is locked or unlocked for specific Test ID
                    //TFS 11310: The below code was moved out of the 2nd foreach call so that we only make one data call per test to 
                    //get schedule info instead of a data call for every cell in the report.
                    if (!string.IsNullOrEmpty(test["TestID"].ToString()))
                    {
                        int testId = Convert.ToInt32(test["TestID"]);
                        DataRow row = Thinkgate.Base.Classes.Assessment.GetAssessmentSchedule(testId);
                        if (row != null)
                        {
                            string content = row["CONTENT"].ToString();
                            if (content == AssessmentScheduleStatus.Disabled.ToString())
                            {
                                isContentLocked = "True";
                            }
                            else if (content == AssessmentScheduleStatus.Enabled.ToString())
                            {
                                isContentLocked = "False";
                            }
                            else if (row["CONTENT"].ToString().Split(' ')[0] ==
                                     AssessmentScheduleStatus.Enabled.ToString())
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
                                {
                                    isContentLocked = "False";
                                }
                                else
                                {
                                    isContentLocked = "True";
                                }
                            }
                        }
                    }

                    foreach (DataRow r in resultsData.Rows)
                    {
                        if (dataItem["Level"].Equals(r["Level"])
                                && dataItem["LevelID"].Equals(r["LevelID"])
                                && r["TestID"].Equals(test["TestID"]))
                        {
                            e.Item.Cells[cellIndex].Text = r["Score"].ToString();

                            levelIDEncrypted = Cryptography.EncryptString(r["LevelID"].ToString(), SessionObject.LoggedInUser.CipherKey);
                            
                            if ((_level == "Class" || _level == "Teacher") && (r["Level"].ToString() == "Class" || r["Level"].ToString() == "Student" || r["Level"].ToString() == "Teacher"))
                            {
                                var dateTimeCalculated = GetTestDateTimeCalculated(test["TestID"].ToString());

                                if (dataItem["Level"].ToString() != "State" && dataItem["Level"].ToString() != "District" && dataItem["Level"].ToString() != "School")
                                {
                                    //BJC-6/27/2012: If level=Class, build string list of STEID's to use for legacy link to print student responses.
                                    //If level=Student, pass student's STEID through.
                                    var steIDs = String.Empty;
                                    ClassID_TrackedDown = _classID.ToString();
                                    if(dataItem["Level"].ToString() == "Class")
                                    {
                                        List<DataRow> studentList = levelData.Select("Level = 'Student' and ParentLevelID = " + r["LevelID"]).ToList();
                                        List<DataRow> steIDList = new List<DataRow>();
                                        foreach(DataRow studentRow in studentList)
                                        {
                                            var tempList = resultsData.Select("LevelID = " + studentRow["LevelID"] + " and TestID = " + r["TestID"]).ToList();
                                            if (tempList.Count == 0) continue;
                                            steIDList.Add(tempList[0]);
                                        }
                                        steIDs = String.Join(",", from student in steIDList
                                                                    select student["STEID"]);
                                    }
                                    else if(dataItem["Level"].ToString() == "Student")
                                    {
                                        steIDs = r["STEID"].ToString();
                                        if (ClassID_TrackedDown == "0" && dataItem["ParentLevel"].ToString() == "Class") ClassID_TrackedDown = dataItem["ParentLevelID"].ToString();
                                    }
                                    //BJC-6/27/2012: End of change.
                                    //---------------------------------
                                    //@@GRP=" + grps + "@@"
                                    e.Item.Cells[cellIndex].Attributes.Add("onclick",
                                                                           "ShowEditForm('" + _category + "', '" +
                                                                           r["Level"] + "','" + r["LevelID"] + "','" +
                                                                           levelIDEncrypted + "','" +
                                                                           test["TestID"] + "','" + _term + "','" +
                                                                           r["Year"] +
                                                                           "','" + _type + "','" + ClassID_TrackedDown + "','" +
                                                                           dataItem["ParentLevel"] + "','" +
                                                                           dataItem["ParentLevelID"] + "', '<h1>" +
                                                                           test["TestName"] + "</h1><br/>as of " +
                                                                           dateTimeCalculated + "', 'tportalreport1test', null, '" + steIDs + "', '" + _groups + "', '" + r["TestEventID"] + "', '" + isContentLocked + "', '" + "../../" + "')");
                                    e.Item.Cells[cellIndex].Attributes.Add("style", "cursor: pointer;");
                                }

                                //BJC - 6/12/2012: Collect test IDs for multiple assessment selection.
                                multiSelectTestIDs += test["TestID"] + ",";
                            }
                            else
                            {
                                var dateTimeCalculated = GetTestDateTimeCalculated(test["TestID"].ToString());

                                if ((_level == "State" || _level == "District" || _level == "School") ||
                                    (dataItem["Level"].ToString() != "State" && dataItem["Level"].ToString() != "District" && dataItem["Level"].ToString() != "School"))
                                {
                                    e.Item.Cells[cellIndex].Attributes.Add("onclick",
                                                                           "ShowEditForm('" + _category + "', '" +
                                                                           r["Level"] + "','" + r["LevelID"] + "','" +
                                                                           levelIDEncrypted + "','" +
                                                                           test["TestID"] + "','" + _term + "','" +
                                                                           r["Year"] +
                                                                           "','" + _type + "','" + _classID + "','" +
                                                                           dataItem["ParentLevel"] + "','" +
                                                                           dataItem["ParentLevelID"] + "', '<h1>" +
                                                                           test["TestName"] + "</h1><br/>as of " +
                                                                           dateTimeCalculated + "', 'tportalreport1test', null, '', '" + _groups + "', " + "null, '" + isContentLocked + "', '" + "../../" + "')");
                                    e.Item.Cells[cellIndex].Attributes.Add("style", "cursor: pointer;");
                                }

                                //BJC - 6/12/2012: Collect test IDs for multiple assessment selection.
                                multiSelectTestIDs += test["TestID"] + ",";

                                if ((_level == "District" || _level == "School") && r["Level"].ToString() == "Teacher")
                                {
                                    var js = string.Empty;
                                    var style = "opacity:.3; filter:alpha(opacity=30);";
                                    if (UserHasPermission(Permission.Icon_Adminportal_Teacherweeble) && (_years.Count == 1) && (_years.Contains(_districtParms.Year.Trim())))
                                    {
                                        js = "window.open('../../Controls/Assessment/AssessmentResults_ExpandedV2.aspx?category=" + _category + "&term=" + _term +
                                            "&type=" + _type + "&level=Teacher&levelID=" + Standpoint.Core.Classes.Encryption.EncryptInt(DataIntegrity.ConvertToInt(r["LevelID"])) + "');";
                                        style = "cursor:pointer;";
                                    }
                                    e.Item.Cells[nameCellIndex].Text = "<img src=\"../../Images/user2_16x16.gif\" style=\"" + style + "\" onclick=\"" +
                                        js + "return false;\" />" + dataItem["LevelName"];
                                }
                            }

                            //If performance levels checkbox is checked, show level colors on each assessment score cell.
                            if (chkPerformanceLevels.Checked)
                            {
                                //BJC - 6/14/2012: The noShade class is used to prevent the Telerik odd/even row shading on assessment score cells.
                                e.Item.Cells[cellIndex].CssClass = "noShade";
                                e.Item.Cells[cellIndex].BackColor = System.Drawing.Color.FromName(r["PLevel"].ToString());
                            }

                            //BJC - 6/14/2012: The bgStyle attribute is used to retain the cell color when performance levels are hidden on client-side.
                            e.Item.Cells[cellIndex].Attributes["bgStyle"] = r["PLevel"].ToString();

                            break;
                        }
                    }

                    //BJC - 6/14/2012: If test ID is not in the locked test IDs list, add unlockedCell attribute for client-side indicator that cell is unlocked.
                    if (lockedTestIDsList.IndexOf("," + test["TestID"] + ",") == -1)
                    {
                        e.Item.Cells[cellIndex].Attributes["unlockedCell"] = "yes";
                    }
                    if (cellIndex > (e.Item.Cells.Count - assessmentData2.Rows.Count))
                    {
                        e.Item.Cells[cellIndex].Attributes["scoreCell"] = "yes";
                    }

                    cellIndex++;
                }

            //BJC - 6/12/2012: Multiple assessment selection functionality. If multiSelectTestIDs contains any test IDs, add functionality. Otherwise, add static image.
            cellIndex = e.Item.Cells.Count - assessmentData2.Rows.Count;
            multiSelectTestIDs = String.IsNullOrEmpty(multiSelectTestIDs) ? "" : multiSelectTestIDs.Substring(0, multiSelectTestIDs.Length - 1);



            var multiSelectJS =
                                                                           "ShowEditForm('" + _category + "', '" +
                                                                           dataItem["Level"] + "','" + dataItem["LevelID"] + "','" +
                                                                           levelIDEncrypted + "','','" + _term + "','" +
                                                                           dataItem["Year"] +
                                                                           "','" + _type + "','" + _classID + "','" +
                                                                           dataItem["ParentLevel"] + "','" +
                                                                           dataItem["ParentLevelID"] + "', '<h1>" +
                                                                           Regex.Replace(dataItem["LevelName"].ToString(), "'", "\\'") + "</h1>'"
                                                                           + ",'tportalreportmtest', '" + multiSelectTestIDs + "', '', '" + _groups + "', '', '', '../../')";


            /*var multiSelectJS = "ShowEditForm('" + _category + "','" + dataItem["Level"] + "','" + dataItem["LevelID"] + "','','" + levelIDEncrypted + "','" +
                               _term + "','" + dataItem["Year"] + "','" + _type + "','" +
                               _classID + "','" + dataItem["ParentLevel"] + "','" + dataItem["ParentLevelID"] +
                               "', '<h1>" + Regex.Replace(dataItem["LevelName"].ToString(), "'", "\\'") + "</h1>', 'tportalreportmtest', '" + multiSelectTestIDs + "', '', '" + _groups + "', '" + "../../" + "')";*/

            if (((_level == "Teacher" || _level == "Class") && (dataItem["Level"].ToString() == "State" || dataItem["Level"].ToString() == "District"
                || dataItem["Level"].ToString() == "School")) || String.IsNullOrEmpty(multiSelectTestIDs))
            {
                //DHB - 8/7/2012: Design team ask that "Report Selection" page icon not even show for teacher role if at school, district or state level.
                //e.Item.Cells[cellIndex].Text = "<img src=\"../../Images/report.jpg\" />";
            }
            else
            {
                e.Item.Cells[cellIndex].Text = "<img src=\"../../Images/report.jpg\" style=\"cursor:pointer;\" onclick=\"" + multiSelectJS + "; return false;\" />";
            }
        }

        protected void radTreeResults_NeedDataSource(object source, TreeListNeedDataSourceEventArgs e)
        {
            radTreeResults.DataSource = GetDataTable();
          
            SetChartTitles();
        }

        protected void chkPerformanceLevels_CheckedChanged(object sender, EventArgs e)
        {
            DoSearch();
        }

        protected void chkUnlockedColumns_CheckedChanged(object sender, EventArgs e)
        {
            DoSearch();
        }

        #endregion

    }
}
