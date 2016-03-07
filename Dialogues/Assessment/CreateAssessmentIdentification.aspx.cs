using System;
using System.Data;
using System.Web.UI;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System.Linq;
using Thinkgate.Domain.Classes;
using Thinkgate.Utilities;
using System.Collections.Generic;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class CreateAssessmentIdentification : BasePage
    {
        public SessionObject SessionObject;
        protected int _teacherID;
        protected int levelID;
        protected string levelToString;
        protected Base.Enums.EntityTypes level;
        protected Class levelObj;
        protected Teacher teacherObj;
        protected string subject;
        protected int course;
        protected string grade;
        protected string type;
        protected int term;
        protected string content;
        protected string scoreType;
        protected string performanceLevelSet;
        protected string keywords;
        protected string assessmentSelection;
        protected string description;
        protected StandardRigorLevels rigorLevels;
        protected int _assessmentID;
        protected String cacheKey;
        protected Base.Classes.Assessment _assessment;
        protected string testCategory;
        protected string year;
        private CourseList _curriculumCourseList;
        private bool IsSecure = false;
        protected Dictionary<string, bool> dictionaryItem;
        protected int count = 0;
        bool isSecuredFlag;

        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(this.type);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];
            testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? SessionObject.AssessmentBuildParms["TestCategory"] : string.Empty;
            levelToString = SessionObject.AssessmentBuildParms.ContainsKey("level") ? SessionObject.AssessmentBuildParms["level"] : string.Empty;
            _curriculumCourseList = Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            if (string.IsNullOrEmpty(Request.QueryString["xID"]))
            {

                switch (SessionObject.CurrentPortal)
                {

                    case EntityTypes.District:
                        level = (Base.Enums.EntityTypes)SessionObject.DistrictTileParms.GetParm("level");
                        levelID = DataIntegrity.ConvertToInt(SessionObject.DistrictTileParms.GetParm("levelID"));
                        levelObj = (Class)SessionObject.DistrictTileParms.GetParm("district");
                        break;

                    case EntityTypes.School:
                        level = Base.Enums.EntityTypes.School;
                        break;

                    case EntityTypes.Teacher:
                        level = (Base.Enums.EntityTypes)SessionObject.TeacherTileParms.GetParm("level");
                        _teacherID = DataIntegrity.ConvertToInt(SessionObject.TeacherTileParms.GetParm("userID"));
                        levelID = DataIntegrity.ConvertToInt(SessionObject.TeacherTileParms.GetParm("levelID"));
                        levelObj = (Class)SessionObject.TeacherTileParms.GetParm("class");
                        teacherObj = Base.Classes.Teacher.GetTeacherByPage(_teacherID);
                        break;
                }

                switch (levelToString)
                {
                    case "Class":
                        subject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : (levelObj != null ? levelObj.Subject.DisplayText : string.Empty);
                        course = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]) : (levelObj != null ? levelObj.Course.ID : 0);
                        grade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : (levelObj != null ? levelObj.Grade.DisplayText : string.Empty);
                        break;
                    case "Teacher":

                        subject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : string.Empty;
                        course = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]) : 0;
                        grade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : string.Empty;
                        break;
                    case "District":

                        subject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : string.Empty;
                        course = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]) : 0;
                        grade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : string.Empty;
                        break;
                }

                type = SessionObject.AssessmentBuildParms.ContainsKey("Type") ? SessionObject.AssessmentBuildParms["Type"] : string.Empty;
                year = SessionObject.AssessmentBuildParms.ContainsKey("Year") ? SessionObject.AssessmentBuildParms["Type"] : string.Empty;
                term = SessionObject.AssessmentBuildParms.ContainsKey("Term") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Term"]) : 0;
                content = SessionObject.AssessmentBuildParms.ContainsKey("Content") ? SessionObject.AssessmentBuildParms["Content"] : string.Empty;
                scoreType = SessionObject.AssessmentBuildParms.ContainsKey("ScoreType") ? SessionObject.AssessmentBuildParms["ScoreType"] : string.Empty;
                performanceLevelSet = SessionObject.AssessmentBuildParms.ContainsKey("PerformanceLevelSet") ? SessionObject.AssessmentBuildParms["PerformanceLevelSet"] : string.Empty;
                keywords = SessionObject.AssessmentBuildParms.ContainsKey("Keywords") ? SessionObject.AssessmentBuildParms["Keywords"] : string.Empty;
                assessmentSelection = SessionObject.AssessmentBuildParms.ContainsKey("AssessmentSelection") ? SessionObject.AssessmentBuildParms["AssessmentSelection"] : string.Empty;
                description = SessionObject.AssessmentBuildParms.ContainsKey("Description") ? SessionObject.AssessmentBuildParms["Description"] : string.Empty;
                rigorLevels = SessionObject.Standards_RigorLevels_ItemCounts;
                Page.Title = "Assessment Identification";
                isTeacher.Value = SessionObject.LoggedInUser.Roles.Count == 1 || SessionObject.LoggedInUser.Roles[0].RoleName.ToLower() == "teacher" || SessionObject.LoggedInUser.Roles[0].RoleName.ToLower() == "sloteach" ? "yes" : "no";
                createAssessmentClassCount.Value = SessionObject.LoggedInUser.Classes.Count.ToString();
                if (SessionObject.LoggedInUser.Classes.Count == 1) classID.Value = SessionObject.LoggedInUser.Classes[0].ID.ToString();
                if (level == EntityTypes.Class) classID.Value = levelID.ToString();
                selectedTestCategory.Value = testCategory;
            }
            else
            {
                if (_assessment == null)
                    LoadAssessment();

                subject = _assessment.Subject;
                course = _assessment.currCourseID;
                grade = _assessment.Grade;
                type = _assessment.TestType;
                term = DataIntegrity.ConvertToInt(_assessment.Term);
                content = _assessment.ContentType;
                assessmentSelection = _assessment.AssessmentSelection;
                description = _assessment.Description;
                year = _assessment.Year;
                assessmentItemsCount.Value = _assessment.Items.Count.ToString();
                testCategory = _assessment.TestCategory;
                OTCNavigationRestricted.Value = _assessment.IsOTCNavigationRestricted;
                Page.Title = "Assessment Identification";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory);
                isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString()) == true).Select(y => y.Key).ToList().Distinct().Count() > 0 ? true : false;
                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                
                if (Request.QueryString["copy"] != null)
                {
                    if (Request.QueryString["copy"].ToString().ToUpper() == "YES"
                        && isSecuredFlag && SecureType)
                    {
                        IsSecure = true;
                        rbtnYES.Checked = true;
                        rbtnYES.Enabled = false;
                        rbtnNO.Enabled = false;
                    }
                    else
                    {
                        IsSecure = false;
                        rbtnYES.Enabled = false;
                        rbtnNO.Enabled = false;
                    }
                }
                else if (Request.QueryString["senderPage"] != null)
                {
                    if (Request.QueryString["senderPage"].ToString().ToUpper() == "CONFIG" )
                    {
                        if (isSecuredFlag && hasPermission && SecureType)
                        {
                            IsSecure = true;
                            rbtnYES.Checked = true;
                            rbtnYES.Enabled = false;
                            rbtnNO.Enabled = false;
                        }
                        else
                        {
                            IsSecure = false;
                            rbtnYES.Enabled = false;
                            rbtnNO.Enabled = false;
                        }
                    }
                    else
                    {
                        if (isSecuredFlag && hasPermission && SecureType)
                        {
                            IsSecure = true;
                            rbtnYES.Checked = true;
                            rbtnYES.Enabled = false;
                            rbtnNO.Enabled = false;
                        }
                    }
                }
                else
                {
                    if (isSecuredFlag)
                    {
                       rbtnNO.Checked = true;
                    }
                }
                //divSecure.Visible = (isSecuredFlag && UserHasPermission(Permission.Access_SecureTesting)) ? true : false;
                //if (Session["IsSecure"]!=null)
                //{
                //    if (hasPermission && (bool)Session["IsSecure"] == true)
                //    {
                //        divSecure.Visible = true;
                //    }
                //}
                if (!string.IsNullOrEmpty(Request.QueryString["showSecure"]))
                {
                    divSecure.Visible = (isSecuredFlag && UserHasPermission(Permission.Access_SecureTesting)) ? true : false;
                    if (Session["IsSecure"] != null)
                    {
                        if (hasPermission && (bool)Session["IsSecure"] == true)
                        {
                            divSecure.Visible = true;
                            rbtnYES.Enabled = true;
                            rbtnNO.Enabled = true;
                        }
                    }
                }
                else
                {
                    divSecure.Visible = true;
                    rbtnYES.Enabled = false;
                    rbtnNO.Enabled = false;
                }

                if (hasPermission)
                {
                    if (SecureType || isSecuredFlag)
                    {
                        divSecure.Visible = true;
                    }
                    else { divSecure.Visible = false; }
                }
                else
                {
                    divSecure.Visible = false;
                }
                //if (hasPermission && testCategory != "Classroom")
                //{
                //    divSecure.Visible = true;
                //}

                assessmentID.Value = _assessmentID.ToString();

                switch (Request.QueryString["headerImg"])
                {
                    case "repairtool":
                        headerImg.Src = "../../Images/repairtool.png";
                        headerImg.Attributes["headerImgName"] = "repairtool";
                        assessmentSelectionFormSpan.Attributes["style"] = "display:none;";
                        senderPage.Value = Request.QueryString["senderPage"];
                        break;
                    case "lightningbolt":
                        headerImg.Src = "../../Images/lightningbolt.png";
                        headerImg.Attributes["headerImgName"] = "lightningbolt";
                        nextButton.OnClientClick = "createAssessment('submitQuickBuildXmlHttpPanel'); return false;";
                        senderPage.Value = Request.QueryString["senderPage"];
                        break;
                    default:
                        headerImg.Visible = false;
                        if (_assessment != null)
                        {
                            backButton.Visible = false;
                            contentDropdown.Attributes["style"] = "display:none;";
                            contentLabel.Visible = false;
                            senderPage.Value = Request.QueryString["senderPage"];
                            copy.Value = Request.QueryString["copy"];
                            nextButton.Text = (copy.Value == "yes") ? "   Copy   " : "  Update  ";
                            nextButton.OnClientClick = (copy.Value == "yes" ? "copyAssessment();" : "updateIdentification();") + " return false;";
                            if (copy.Value != "yes" && _assessment.IsProofed)
                            {
                                gradeDropdown.Enabled = false;
                                subjectDropdown.Enabled = false;
                                courseDropdown.Enabled = false;
                                typeDropdown.Enabled = false;
                                termDropdown.Enabled = false;
                                contentDropdown.Enabled = false;
                                assessmentSelectionDropdown.Enabled = false;
                                descriptionInput.Disabled = true;
                                nextButton.Visible = false;
                                cancelButton.Visible = false;
                                isProofed.Value = "Yes";
                                rbtnYES.Enabled = false;
                                rbtnNO.Enabled = false;
                            }
                        }

                        break;
                }

                LoadGradeButtonFilter();
                LoadSubjectsButtonFilter();
                LoadCoursesButtonFilter();
                LoadTypeButtonFilter(IsSecure);
                LoadTermButtonFilter();
                LoadYearButtonFilter();
                LoadPerformanceLevelSetsFilter();
                SetSelectedContentButtonFilter();
                SetSelectedAssessmentSelectionFilter();

                assessmentID_encrypted.Value = Request.QueryString["xID"];
                descriptionInput.Value = copy.Value == "yes" ? "" : description;

                if (rigorLevels == null)
                {
                    standardsCleared.Value = "true";
                }
                else if (rigorLevels.StandardItemTotals.Count > 0)
                {
                    standardsCleared.Value = "false";
                }

                nextButtonInitialText.Value = nextButton.Text;
            }
        }
        private void LoadAssessment()
        {
            if (Request.QueryString["xID"] == null ||
                    (_assessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey)) <= 0)
            {
                SessionObject.RedirectMessage = "No assessment ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                cacheKey = "Assessment_" + Request.QueryString["xID"];
                if (Base.Classes.Cache.Get(cacheKey) == null)
                {
                    _assessment = Base.Classes.Assessment.GetAssessmentAndQuestionsByID(_assessmentID);
                    if (_assessment != null)
                        Base.Classes.Cache.Insert(cacheKey, _assessment);
                    else
                    {
                        SessionObject.RedirectMessage = "Could not find the assessment.";
                        Response.Redirect("~/PortalSelection.aspx", true);
                    }
                }
                else
                    _assessment = (Base.Classes.Assessment)Cache[cacheKey];
            }
        }

        protected void LoadGradeButtonFilter()
        {
            var gradesByCurriculumCourses = _curriculumCourseList.GetGradeList();

            gradeDropdown.Items.Clear();
            gradeDropdown.Attributes["teacherID"] = _teacherID.ToString();

            foreach (var g in gradesByCurriculumCourses)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = g.DisplayText;
                item.Value = g.DisplayText;

                item.Attributes["gradeOrdinal"] = g.GetFriendlyName();

                if (g.DisplayText == grade)
                {
                    item.Selected = true;
                }

                gradeDropdown.Items.Add(item);
            }

            initGrade.Value = gradeDropdown.SelectedIndex.ToString();

            if (gradeDropdown.SelectedValue.Length == 0 && gradeDropdown.Items.Count == 1)
            {
                gradeDropdown.Items[0].Selected = true;
            }
        }

        protected void LoadSubjectsButtonFilter()
        {
            var selectedGrade = gradeDropdown.SelectedValue.Length > 0 ? gradeDropdown.SelectedValue : "";
            var subjectsByCurriculumCourses = _curriculumCourseList.FilterByGrade(selectedGrade).GetSubjectList();

            subjectDropdown.Items.Clear();
            subjectDropdown.Attributes["teacherID"] = _teacherID.ToString();

            foreach (var subjectText in subjectsByCurriculumCourses.Select(s => s.DisplayText).Distinct())
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = subjectText;
                item.Value = subjectText;

                if (subjectText == subject)
                {
                    item.Selected = true;
                }

                subjectDropdown.Items.Add(item);
            }

            initSubject.Value = subjectDropdown.SelectedIndex.ToString();

            if (subjectDropdown.SelectedValue.Length == 0 && subjectDropdown.Items.Count == 1)
            {
                subjectDropdown.Items[0].Selected = true;
            }
        }

        protected void LoadCoursesButtonFilter()
        {
            var selectedGrade = gradeDropdown.SelectedValue.Length > 0 ? gradeDropdown.SelectedValue : string.Empty;
            var selectedSubject = subjectDropdown.SelectedValue.Length > 0 ? subjectDropdown.SelectedValue : string.Empty;
            var coursesByGradeAndSubject = _curriculumCourseList.FilterByGradeAndSubject(selectedGrade, selectedSubject);
            coursesByGradeAndSubject.Sort((x, y) => string.Compare(x.CourseName, y.CourseName));

            courseDropdown.Items.Clear();
            courseDropdown.Attributes["teacherID"] = _teacherID.ToString();

            foreach (var c in coursesByGradeAndSubject)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = c.CourseName;
                item.Value = c.ID.ToString();

                if (c.ID == course)
                {
                    item.Selected = true;
                }

                courseDropdown.Items.Add(item);
            }

            initCourse.Value = courseDropdown.SelectedIndex.ToString();

            if (courseDropdown.SelectedValue.Length == 0 && courseDropdown.Items.Count == 1)
            {
                courseDropdown.Items[0].Selected = true;
            }
        }

        protected void rbtnNO_OnCheckedChanged(object sender, EventArgs e)
        {
            IsSecure = false;
            LoadTypeButtonFilter(IsSecure);
        }

        protected void rbtnYES_OnCheckedChanged(object sender, EventArgs e)
        {
            IsSecure = true;
            LoadTypeButtonFilter(IsSecure);
        }

        void LoadTypeButtonFilter(bool IsSecure)
        {
            var typeListTable = new List<string>();
            var dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory, true);
            if (dictionaryItem != null)
            {
                if (IsSecure)
                {
                    typeDropdown.Items.Clear();
                    typeDropdown.EmptyMessage = "";
                    typeListTable = dictionaryItem.Where(c => c.Value).Select(c => c.Key).ToList();
                }
                else
                {
                    typeDropdown.Items.Clear();
                    typeListTable = dictionaryItem.Where(c => !c.Value).Select(c => c.Key).ToList();
                }
            typeListTable.Sort();
        }

    foreach (string testType in typeListTable)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = testType;
                item.Value = testType;

                if (testType == type || (String.IsNullOrEmpty(type) && ((level == EntityTypes.District && testType == "Pre-test") || ((level == EntityTypes.Class ) && testType == "Test"))))
                {
                    item.Selected = true;
                }

                typeDropdown.Items.Add(item);
            }

            if (typeDropdown.Items.Count > 0 && String.IsNullOrEmpty(type) && level != EntityTypes.District && level != EntityTypes.Class && level != EntityTypes.Teacher)
            {
                typeDropdown.Items[0].Selected = true;
            }
        }

        protected void LoadTermButtonFilter()
        {
            var terms = Thinkgate.Base.Classes.Assessment.GetTerms();

            termDropdown.Items.Clear();

            foreach (string str in terms)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = str;
                item.Value = str;

                if (copy.Value != "yes")
                {
                    item.Selected = term == DataIntegrity.ConvertToInt(str);
                }

                termDropdown.Items.Add(item);
            }
        }

        protected void LoadYearButtonFilter()
        {
            DataTable allYears = Thinkgate.Base.Classes.Assessment.GetYears();
            DataTable dtYears = new DataTable();
            string currentYear = string.Empty;


            DataColumn ddCol = dtYears.Columns.Contains("DropdownText") ? dtYears.Columns[1] : dtYears.Columns.Add("DropdownText", typeof(String));
            DataColumn ddYear = dtYears.Columns.Add("Year", typeof(String));
            foreach (DataRow row in allYears.Rows)
            {

                if (row["YearTense"].ToString() == "Current" || row["YearTense"].ToString() == "Future")
                {

                    if (row["YearTense"].ToString() == "Current") currentYear = row["Year"].ToString();
                    DataRow newrow = dtYears.NewRow();
                    dtYears.Rows.Add(newrow);
                    newrow[ddCol] = row["Year"];
                    newrow[ddYear] = row["Year"];
                }
            }


            // Data bind the combo box.
            yearDropDown.DataTextField = "Year";
            yearDropDown.DataValueField = "DropdownText";
            yearDropDown.DataSource = dtYears;
            yearDropDown.DataBind();
            if (!string.IsNullOrEmpty(currentYear))
                yearDropDown.SelectedValue = currentYear;


        }

        protected void LoadPerformanceLevelSetsFilter()
        {
            DataTable performanceLevelsTable = Base.Classes.Assessment.GetPerformanceLevelsForDialog();

            foreach (DataRow row in performanceLevelsTable.Rows)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = row["PerformanceLevel"].ToString();
                item.Value = row["PerformanceLevel"].ToString();

                if (copy.Value != "yes")
                {
                    item.Selected = performanceLevelSet == row["PerformanceLevel"].ToString();
                }

                performanceLevelSetsDropdown.Items.Add(item);
            }
        }

        protected void SetSelectedContentButtonFilter()
        {
            RadComboBoxItem item = contentDropdown.Items.FindItemByText(content);

            if (item != null)
            {
                item.Selected = true;
            }
            if (content != "Item Bank" && !string.IsNullOrEmpty(content))
            {
                assessmentSelectionFormSpan.Attributes["style"] = "display:none;";
            }
        }

        protected void SetSelectedAssessmentSelectionFilter()
        {
            RadComboBoxItem item = assessmentSelectionDropdown.Items.FindItemByValue(assessmentSelection);

            if (item != null)
            {
                item.Selected = true;
            }
        }

        protected void UpdateIdentification_Click(object sender, EventArgs e)
        {
            _assessment.Grade = gradeDropdown.SelectedValue;
            _assessment.Subject = subjectDropdown.SelectedValue;
            _assessment.currCourseID = DataIntegrity.ConvertToInt(courseDropdown.SelectedValue);
            Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(DataIntegrity.ConvertToInt(courseDropdown.SelectedValue));
            _assessment.Course = assessmentCourse != null ? assessmentCourse.CourseName : "";
            _assessment.TestType = typeDropdown.SelectedValue;
            _assessment.Term = termDropdown.SelectedValue;
            _assessment.ContentType = contentDropdown.SelectedValue;
            _assessment.AssessmentSelection = assessmentSelectionDropdown.SelectedValue;
            _assessment.Description = descriptionInput.Value;
            _assessment.Year = yearDropDown.SelectedValue;

            Base.Classes.Assessment.SaveIdentificationInformation(_assessment, SessionObject.LoggedInUser.Page);
            if (Base.Classes.Cache.Get(cacheKey) == null)
            {
                Base.Classes.Cache.Insert(cacheKey, _assessment);
            }
            else
            {
                Base.Classes.Cache.Remove(cacheKey);
                Base.Classes.Cache.Insert(cacheKey, _assessment);
            }

            Base.Classes.Assessment.SaveLrmiTagForAssessment(_assessmentID, (int)Enums.LrmiTags.EducationalLevel, _assessment.Grade, false);
            Base.Classes.Assessment.SaveLrmiTagForAssessment(_assessmentID, (int)Enums.LrmiTags.EducationalSubject, _assessment.Subject, false);
            Base.Classes.Assessment.SaveLrmiTagForAssessment(_assessmentID, (int)Enums.LrmiTags.EndUser, "13", false);
            Base.Classes.Assessment.SaveLrmiTagForAssessment(_assessmentID, (int)Enums.LrmiTags.EducationalUse, "50", false);
            Base.Classes.Assessment.SaveLrmiTagForAssessment(_assessmentID, (int)Enums.LrmiTags.LearningResourceType, "71", false);

            /*string js = "parent.window.location.reload();";
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "AssessmentIdentificationRefresh", js, true);*/

            //TODO: Need to get this code working at some point. The alternativte being used right now is to reload the entire screen which is inefficient.

            string js = "parent.$find('assessmentIdentificationRefreshTrigger').click();";
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "AssessmentIdentificationRefresh", js, true);

        }
    }
}