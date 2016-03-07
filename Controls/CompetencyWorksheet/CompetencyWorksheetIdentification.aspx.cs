using CMS.GlobalHelper;
using CMS.SettingsProvider;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Controls.CompetencyWorksheet
{
    public partial class CompetencyWorksheetIdentification : System.Web.UI.Page
    {
        private SessionObject session;
        private CourseList _curriculumCourseList;
        protected string subject;
        protected int course=0;
        protected string courseName;
        protected string grade;
        protected int _teacherID;
        private int nodeid { get { return Convert.ToInt32(hdnNodeId.Value); } }
        private string type { get { return Convert.ToString(hdnType.Value); } }
        public Int32 worksheetId { get { return Convert.ToInt32(hdnWorksheetId.Value == "" ? ((Session["docId"] == "" || Session["docId"] == null) ? "0" : Session["docId"]) : hdnWorksheetId.Value); } }
        public string _isNew { get { return QueryHelper.GetString("IsNew", ""); } }
        public Int32 classid;
        static List<CompetencyRubrics> competencyRubricsPageList;
        enum RubricsName
        {
            DistrictDefault,
            StateDefault
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            session = (SessionObject)Session["SessionObject"];
            _curriculumCourseList = Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(session.LoggedInUser);
            _teacherID = DataIntegrity.ConvertToInt(session.LoggedInUser.Page);

            hdnNodeId.Value = QueryHelper.GetString("id", "-1");
            hdnWorksheetId.Value = QueryHelper.GetString("WorksheetID", "");
            hdnType.Value = Convert.ToString(QueryHelper.GetString("type", ""));
            if (!IsPostBack)
            {
                ///Only for Teacher set default value of grade ,subject, course.
                //subject = session.AssessmentBuildParms.ContainsKey("Subject") ? session.AssessmentBuildParms["Subject"] : string.Empty;
                //course = session.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(session.AssessmentBuildParms["Course"]) : 0;
                //grade = session.AssessmentBuildParms.ContainsKey("Grade") ? session.AssessmentBuildParms["Grade"] : string.Empty;

                //BindClassTypes();

                competencyRubricsPageList = CompetencyWorkSheet.CompetencyRubricsPage();
                if (Request.QueryString["type"] != null)
                {
                    if (type == "Edit")
                    {
                        btnContinue.Visible = false;
                        btnSave.Visible = true;
                        btnCancel.OnClientClick = "CloseDialog('" + type.ToLower() + "');";
                        cmbRubric.Enabled = false;
                        gradeDropdown.Enabled = false;
                        subjectDropdown.Enabled = false;
                        courseDropdown.Enabled = false;
                    }
                    else if (type.ToLower() == "copy" || type.ToLower() == "'update'")
                    {
                        btnCancel.OnClientClick = "CloseDialog('" + type.ToLower().Replace("'", "") + "');";
                        btnContinue.Text = "  Save  ";
                        btnSave.Visible = false;
                    }
                    BindCompetencyWorkSheet(worksheetId);
                }
                else
                {
                    txtName.Text = QueryHelper.GetString("name", "");
                    txtDescription.Text = QueryHelper.GetString("desc", "");
                    subject = session.clickedClass.Subject.DisplayText != "" ? session.clickedClass.Subject.DisplayText : string.Empty;
                    courseName = session.clickedClass.Course.CourseName != "" ? session.clickedClass.Course.CourseName : string.Empty;
                     grade = session.clickedClass.Grade.DisplayText != "" ? session.clickedClass.Grade.DisplayText : string.Empty;
                    LoadGradeButtonFilter();
                    LoadSubjectsButtonFilter();
                    LoadCoursesButtonFilter();
                }
                BindRubricsItems(grade, subject, Convert.ToString(course));
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


        [System.Web.Services.WebMethod]
        public static string[] LoadSubjectsButtonFilterByGrade(string gradeVal)
        {
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            CourseList curriculumCourses = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            var subjectsByCurriculumCourses = curriculumCourses.FilterByGrade(gradeVal).GetSubjectList();

            List<string> subjects = new List<string>();
            foreach (var subjectText in subjectsByCurriculumCourses.Select(s => s.DisplayText).Distinct())
            {
                subjects.Add(subjectText);
            }
            return subjects.ToArray();
        }

        [System.Web.Services.WebMethod]
        public static string[] LoadCoursesButtonFilterBySubject(string subject, string gradeVal)
        {
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            CourseList curriculumCourses = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            var coursesByCurriculumCourses = curriculumCourses.FilterByGradeAndSubject(gradeVal, subject);
            coursesByCurriculumCourses.Sort((x, y) => string.Compare(x.CourseName, y.CourseName));

            List<string> courses = new List<string>();

            foreach (var c in coursesByCurriculumCourses)
            {
                courses.Add(c.ID.ToString() + "/" + c.CourseName);
            }

            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemTotals.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemNames.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardRigorLevel.Clear();
            SessionObject.ItemBanks.Clear();

            return courses.ToArray();
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

                if (c.CourseName == courseName)
                {
                    item.Selected = true;
                    course = c.ID;
                }
                courseDropdown.Items.Add(item);
            }

            initCourse.Value = courseDropdown.SelectedIndex.ToString();

            if (courseDropdown.SelectedValue.Length == 0 && courseDropdown.Items.Count == 1)
            {
                courseDropdown.Items[0].Selected = true;
            }
        }


        ///// <summary>
        ///// BindGroupTypes
        ///// </summary>
        //private void BindGroupTypes()
        //{
        //    DataTable dtGroup = new DataTable();
        //    dtGroup.Columns.Add("CmbText", typeof(String));
        //    dtGroup.Columns.Add("GroupId", typeof(String));

        //    List<CompetencyRole> groupList = CompetencyWorkSheet.CompetencyRoleList(session.LoggedInUser.UserId);
        //    //session.LoggedInUser.UserId
        //    //Base.Classes.Teacher selectedTeacher = (Base.Classes.Teacher)Base.Classes.Cache.Get(key);
        //    foreach (Base.Classes.CompetencyRole oClass in groupList)
        //    {
        //        dtGroup.Rows.Add(oClass.DisplayName, oClass.RoleId);
        //    }

        //    ///Data bind the combo box.
        //    CmbGroupName.DataTextField = "CmbText";
        //    CmbGroupName.DataValueField = "GroupId";
        //    CmbGroupName.DataSource = dtGroup;
        //    CmbGroupName.DataBind();

        //    if (groupList.Count == 0)
        //    {
        //        CmbGroupName.EmptyMessage = "No groups available";
        //        CmbGroupName.Enabled = false;
        //    }
        //}

        private void BindRubricsItems(string grade, string subject, string course)
        {
            List<CompetencyRubrics> competencyRubricsList;
            competencyRubricsList = (from oCompetencyRubrics in competencyRubricsPageList.Where(p => p.CurrCourse == course) select oCompetencyRubrics).ToList();

            if (competencyRubricsList.Count == 0)
            {
                competencyRubricsList = (from oCompetencyRubrics in competencyRubricsPageList.Where(p => p.Name.Replace(" ",string.Empty) == RubricsName.DistrictDefault.ToString()) select oCompetencyRubrics).ToList();
            }
            if (competencyRubricsList.Count == 0)
            {
                competencyRubricsList = (from oCompetencyRubrics in competencyRubricsPageList.Where(p => p.Name.Replace(" ", string.Empty) == RubricsName.StateDefault.ToString()) select oCompetencyRubrics).ToList();
            }

            foreach (var oCompetencyRubrics in competencyRubricsList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = oCompetencyRubrics.Name;
                item.Value = oCompetencyRubrics.RubricID.ToString();
                
                cmbRubric.Items.Add(item);
            }

            intRubric.Value = cmbRubric.SelectedIndex.ToString();

            if (cmbRubric.SelectedValue.Length == 0 && cmbRubric.Items.Count == 1)
            {
                cmbRubric.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// LoadRubricsFilterByGradeSubjectCourse
        /// </summary>
        [System.Web.Services.WebMethod]
        public static string[] LoadRubricsFilterByGradeSubjectCourse(string gradeVal, string subjectVal, string courseVal)
        {
          
            List<CompetencyRubrics> competencyRubricsList;
            competencyRubricsList = (from oCompetencyRubrics in competencyRubricsPageList.Where(p => p.CurrCourse == courseVal) select oCompetencyRubrics).ToList();

            if (competencyRubricsList.Count == 0)
            {
                competencyRubricsList = (from oCompetencyRubrics in competencyRubricsPageList.Where(p => p.Name.Replace(" ", string.Empty) == RubricsName.DistrictDefault.ToString()) select oCompetencyRubrics).ToList();
            }
            if (competencyRubricsList.Count == 0)
            {
                competencyRubricsList = (from oCompetencyRubrics in competencyRubricsPageList.Where(p => p.Name.Replace(" ", string.Empty) == RubricsName.StateDefault.ToString()) select oCompetencyRubrics).ToList();
            }

            List<string> rubricsList = new List<string>();

            foreach (var c in competencyRubricsList)
            {
                rubricsList.Add(c.RubricID.ToString() + "/" + c.Name);
            }
   
            return rubricsList.ToArray();
        }

        /// <summary>
        /// BindGroupTypes
        /// </summary>
        private void BindCompetencyWorkSheet(Int32 workSheetId)
        {
            CompetencyWorkSheet competencyWorkSheetItems = new CompetencyWorkSheet();
            competencyWorkSheetItems = CompetencyWorkSheet.CompetencyWorkSheetItems(workSheetId);
            txtName.Text = competencyWorkSheetItems.Name;
            txtDescription.Text = competencyWorkSheetItems.Description;
            if (competencyWorkSheetItems.CompetencyRubricItemID != "")
            {
                if (cmbRubric.FindItemByValue(competencyWorkSheetItems.CompetencyRubricItemID) != null)
                {
                    cmbRubric.FindItemByValue(competencyWorkSheetItems.CompetencyRubricItemID).Selected = true;
                }
            }
            // cmbRubric.SelectedItem.Value = competencyWorkSheetItems.CompetencyRubricItemID;

            subject = competencyWorkSheetItems.Subject;
            courseName = competencyWorkSheetItems.ClassName;
            grade = competencyWorkSheetItems.Grade;
            LoadGradeButtonFilter();
            LoadSubjectsButtonFilter();
            LoadCoursesButtonFilter();
        }

        public bool RecordExistsInCache(string key)
        {
            return (Thinkgate.Base.Classes.Cache.Get(key) != null);
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {

            if (_isNew == "1")
            {
                Session["docId"] = null;
            }

            var standardids = GetStandardsbyNodeId(nodeid);
            string returnValue = CompetencyWorkSheet.AddWorkSheet(txtName.Text.Length > 100 ? txtName.Text.Substring(0, 100) : txtName.Text, session.LoggedInUser.UserId,
                Convert.ToInt32(courseDropdown.SelectedItem.Value), Convert.ToInt32(cmbRubric.SelectedItem.Value),
                DistrictParms.LoadDistrictParms().Year, txtDescription.Text.Length > 200 ? txtDescription.Text.Substring(0, 200) : txtDescription.Text, standardids,
                worksheetId, type.ToLower() != "copy" ? 0 : 1);

            Int32 worksheetid = Convert.ToInt32(returnValue.Split('-')[0]);
            if (worksheetid == -1)
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(string), "alert", "DuplicateRecords();", true);
                btnContinue.Enabled = false;
            }
            else
            {

                if (type.Trim().ToLower() != "copy")
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(string), "standardassociation", "CallPage('" + worksheetid + "','" + _isNew + "');", true);
                    session.CheckNewWorksheetCreated = "1";
                }
                else
                {
                    classid = session.clickedClass.ID;
                    ScriptManager.RegisterStartupScript(this.Page, typeof(string), "alert", "copyMessage('" + Encryption.EncryptInt(worksheetid) + "'," + returnValue.Split('-')[1] + ");", true);
                    Session["CheckForWorksheetCopied"] = "copied";
                    session.CheckNewWorksheetCreated = "1";
                }
            }
            //session.WorksheetPostboxStatus = true;
        }


        protected Thinkgate.Base.DataAccess.dtGeneric_Int GetStandardsbyNodeId(int nodeid)
        {
            var lststandards = new Thinkgate.Base.DataAccess.dtGeneric_Int();
            string expression = string.Empty;
            DataTable dsstandard;
            try
            {
                Base.DataAccess.dtGeneric_Int _standardids = new Base.DataAccess.dtGeneric_Int();
                _standardids.Add(nodeid);
                dsstandard = CompetencyWorkSheet.GetCurrStabdardsById_Kentico(_standardids, true);

            }
            catch (Exception ex)
            {

                throw new Exception(String.Format("The Query did not give any results {0}\n{1}", expression, ex.Message));

            }

            if (dsstandard != null)
            {
                foreach (DataRow row in dsstandard.Rows)
                {
                    lststandards.Add(Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(row[0]));
                }

            }
            return lststandards;
        }

        //<summary>
        //GroupClassSelectedIndexChanged
        //</summary>
        [System.Web.Services.WebMethod]
        public static string StudentForClass(string Grade, string Subject, string currCourseName)
        {
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            CourseList curriculumCourseList = new CourseList();
            curriculumCourseList = Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            var coursesByGradeAndSubject = curriculumCourseList.FilterByGradeAndSubject(Grade, Subject);
            coursesByGradeAndSubject.Sort((x, y) => string.Compare(x.CourseName, y.CourseName));
            int currCourseID = 0;
            foreach (var c in coursesByGradeAndSubject)
            {
                if (c.CourseName == currCourseName)
                {
                    currCourseID = c.ID;
                }
            }

            string groupName = "groupName";
            Guid? myGroupName = groupName == "groupName" ? Guid.Empty : new Guid(groupName);
            return CompetencyWorkSheet.GetCompetencyWorksheetStudendCount(currCourseID, myGroupName, SessionObject.LoggedInUser.UserId).ToJSON(false);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            session.CheckNewWorksheetCreated = "1";
            var standardids = GetStandardsbyNodeId(nodeid);
            string workSheetid = CompetencyWorkSheet.AddWorkSheet(txtName.Text, session.LoggedInUser.UserId, Convert.ToInt32(courseDropdown.SelectedItem.Value), Convert.ToInt32(cmbRubric.SelectedItem.Value), DistrictParms.LoadDistrictParms().Year, txtDescription.Text, standardids,
                worksheetId);
            //  session.WorksheetPostboxStatus = true;
            ScriptManager.RegisterStartupScript(this.Page, typeof(string), "Close", "CloseDialog('save');", true);
        }

    }
}