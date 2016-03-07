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
using Thinkgate.Domain.Extensions;
using Thinkgate.Utilities;
using System.Collections.Generic;

namespace Thinkgate.Controls.Reports
{
    public partial class StarReport : BasePage
    {
        public SessionObject SessionObject;
        protected string grade;
        private CourseList _curriculumCourseList;
        protected int _teacherID;
        protected int _schoolID;
        protected int course;
        protected string subject;

        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];
            _curriculumCourseList = Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
        }

        protected void Page_Load(object sender, EventArgs e)
        {   
            if (!IsPostBack)
            {
                LoadSchoolButtonFilter();
                LoadGradeButtonFilter();
                LoadSubjectsButtonFilter();
                LoadCoursesButtonFilter();
            }
        }

        private void LoadSchoolButtonFilter()
        {
            var schoolList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolDropDown.Items.Clear();
            schoolDropDown.Attributes["schoolID"] = _schoolID.ToString();
            foreach (var school in schoolList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = school.Name;
                item.Value = school.ID.ToString();

                //item.Attributes["gradeOrdinal"] = g.GetFriendlyName();
                schoolDropDown.Items.Add(item);
            }
            initSchool.Value = schoolDropDown.SelectedIndex.ToString();
            if (schoolDropDown.SelectedValue.Length == 0 && schoolDropDown.Items.Count == 1)
            {
                schoolDropDown.Items[0].Selected = true;
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

    }
}