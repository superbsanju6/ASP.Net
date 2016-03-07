using System;
using System.Data;
using System.Web.UI.WebControls;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Collections.Generic;
using System.Linq;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Class
{
    public partial class ClassSummary_Edit : BasePage
    {
        private Thinkgate.Base.Classes.Class _selectedClass;
        private CourseList _classCourseList;
        private IEnumerable<Grade> _gradeList;
        private bool _permissionIdentification;
        private bool _permissionTeacher;
        private bool _permissionRoster;
        private bool _permissionDeleteClass;
        public string PageTitle;
        private bool _userCrossSchools;
        private List<Base.Classes.School> _schools;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblTeacherMessage.Text = string.Empty;
            _classCourseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
            _gradeList = _classCourseList.GetGradeList();
            LoadClass();
            /* Determine if user has permissions to access each section of edit page */
            _permissionIdentification = UserHasPermission(Base.Enums.Permission.Section_Identification_EditClass);
            _permissionTeacher = UserHasPermission(Base.Enums.Permission.Section_Teacher_EditClass);
            _permissionRoster = UserHasPermission(Base.Enums.Permission.Section_Roster_EditClass);
            _permissionDeleteClass = UserHasPermission(Base.Enums.Permission.Delete_Class);

            bool reloadTeacherGrid = false;

            if (!string.IsNullOrEmpty(addTeacherList.Value))
            {
                _selectedClass.LoadTeachers();
                List<string> teacherUserNames = addTeacherList.Value.Split(',').ToList();
                foreach (Base.Classes.Teacher teacher in _selectedClass.Teachers)
                {
                    if (teacherUserNames.Contains(teacher.EmployeeID))
                    {
                        teacherUserNames.Remove(teacher.EmployeeID);
                    }
                }

                Thinkgate.Base.Classes.Class.AddTeachersToClass(teacherUserNames, _selectedClass.ID);
                addTeacherList.Value = string.Empty;
                reloadTeacherGrid = true;
                LoadClass();
            }

            if (!string.IsNullOrEmpty(isSaveClass.Value))
            {
                //SaveClass();
                isSaveClass.Value = string.Empty;
            }


            if(!UserHasPermission(Base.Enums.Permission.Edit_Class))
            {
                return;
            }

            /* Determine if user has permissions to access each section of edit page */
            _permissionIdentification = UserHasPermission(Base.Enums.Permission.Section_Identification_EditClass);
            _permissionTeacher = UserHasPermission(Base.Enums.Permission.Section_Teacher_EditClass);
            _permissionRoster = UserHasPermission(Base.Enums.Permission.Section_Roster_EditClass);
            _permissionDeleteClass = UserHasPermission(Base.Enums.Permission.Delete_Class);

            //Determine if user should have ability to delete class or save changes to the class.
            saveButton.Enabled = _permissionIdentification || _permissionTeacher || _permissionRoster;
            deleteButton.Enabled = _permissionDeleteClass;
            deleteButton.Visible = _permissionDeleteClass;

            if (_selectedClass == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }

            _selectedClass.LoadTeachers();
            _selectedClass.LoadStudents();

            rosterGrid.DataSource = _selectedClass.Students;
            rosterGrid.DataBind();

            // Disable Add Student button if user does not have permission to edit.
            if (!_permissionRoster) addStudentButton.Enabled = false;

            studentCountSpan.InnerHtml = _selectedClass.Students.Count.ToString() + " students";

            if (!IsPostBack || reloadTeacherGrid)
            {
                teachersGrid.DataSource = _selectedClass.Teachers;
                teachersGrid.DataBind();
            }

            // Disable Add Student button if user does not have permission to edit.
            if (!_permissionTeacher)
            {
                btnTeacherAdd.Enabled = false;
                btnTeacherRemove.Enabled = false;
                teachersGrid.MasterTableView.GetColumn("TeacherGridRemove").Visible = false;
            }

            LoadIdentificationDropdowns();

            //PageTitle = _selectedClass.PrimaryTeacher + ": " + _selectedClass.Grade.GetFriendlyName() + " Grade " + _selectedClass.Subject.DisplayText + " - Period " + _selectedClass.Period;
            Page.Title = "Edit Class (" + _selectedClass.PrimaryTeacher + ": " + _selectedClass.Grade.GetFriendlyName() + " Grade " + _selectedClass.Subject.DisplayText + " - Period " + _selectedClass.Period + ")";
            deleteButton.Attributes["classID"] = (_permissionDeleteClass) ? _selectedClass.ID.ToString() : "0";
            saveButton.Attributes["classID"] = _selectedClass.ID.ToString();
        }

        private void LoadClass()
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                var decryptedClassID = GetDecryptedEntityId(X_ID);
                _selectedClass = Thinkgate.Base.Classes.Class.GetClassByID(decryptedClassID);                
            }
        }

        private void LoadIdentificationDropdowns()
        {
            var subjectListTable = _classCourseList.FilterByGrade(_selectedClass.Grade.DisplayText).GetSubjectList();
            var courseListTable = _classCourseList.FilterByGradeAndSubject(_selectedClass.Grade.DisplayText, _selectedClass.Subject.DisplayText);
            var periodListTable = _selectedClass.GetClassPeriods();
            var semesterListTable = _selectedClass.GetClassSemesters();
            
            //Load grades
            gradeDropdown.Items.Clear();
            foreach (Grade grade in _gradeList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = grade.DisplayText;
                item.Value = grade.DisplayText;
                if (grade.DisplayText == _selectedClass.Grade.DisplayText)
                {
                    item.Selected = true;
                    gradeDropdown.Attributes["initialValue"] = grade.DisplayText;
                }

                gradeDropdown.Items.Add(item);
            }

            //Load subjects
            subjectDropdown.Items.Clear();
            foreach(var s in subjectListTable)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = s.DisplayText;
                item.Value = s.DisplayText;
                if (s.DisplayText == _selectedClass.Subject.DisplayText)
                {
                    item.Selected = true;
                    subjectDropdown.Attributes["initialValue"] = s.DisplayText;
                }

                subjectDropdown.Items.Add(item);
            }

            //Load courses
            courseDropdown.Items.Clear();
            foreach (var c in courseListTable)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = c.CourseName;
                item.Value = c.ID.ToString();
                if (c.CourseName == _selectedClass.Course.CourseName)
                {
                    item.Selected = true;
                    courseDropdown.Attributes["initialValue"] = c.ID.ToString();
                }

                courseDropdown.Items.Add(item);
            }

            //Load section
            sectionTextBox.Text = _selectedClass.Section;
            sectionTextBox.Attributes["initialValue"] = _selectedClass.Section;

            //Load periods
            foreach (DataRow row in periodListTable.Rows)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = row["Period"].ToString();
                item.Value = row["Period"].ToString();
                if (row["Period"].ToString() == _selectedClass.Period.ToString())
                {
                    item.Selected = true;
                    periodDropdown.Attributes["initialValue"] = row["Period"].ToString();
                }

                periodDropdown.Items.Add(item);
            }

            //Load semesters
            foreach (DataRow row in semesterListTable.Rows)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = row["Semester"].ToString();
                item.Value = row["Semester"].ToString();
                if (row["Semester"].ToString() == _selectedClass.Semester)
                {
                    item.Selected = true;
                    semesterDropdown.Attributes["initialValue"] = row["Semester"].ToString();
                }

                semesterDropdown.Items.Add(item);
            }

            //Load year
            lblYear.Text = _selectedClass.Year;

            //Load block
            blockTextBox.Text = _selectedClass.Block;
            blockTextBox.Attributes["initialValue"] = _selectedClass.Block;

            //Load schools
            _userCrossSchools = UserHasPermission(Base.Enums.Permission.User_Cross_Schools);

            if (_userCrossSchools)
            {
                _schools = SchoolMasterList.GetSchoolsAll();

                foreach (var school in _schools)
                {
                    RadComboBoxItem item = new RadComboBoxItem();
                    item.Text = school.Name;
                    item.Value = school.ID.ToString();
                    if (school.ID == _selectedClass.School.ID)
                    {
                        item.Selected = true;
                        schoolDropdown.Attributes["initialValue"] = school.ID.ToString();
                    }

                    schoolDropdown.Items.Add(item);
                }
            }

            else
            {
                foreach (ThinkgateSchool school in SessionObject.LoggedInUser.Schools)
                {
                    RadComboBoxItem item = new RadComboBoxItem();
                    item.Text = school.Name;
                    item.Value = school.Id.ToString();
                    if (school.Id == _selectedClass.School.ID)
                    {
                        item.Selected = true;
                        schoolDropdown.Attributes["initialValue"] = school.Id.ToString();
                    }

                    schoolDropdown.Items.Add(item);
                }
            }

            //Load Retain On Resync radio buttons
            if (_selectedClass.RetainOnResync) rbRetainOnResyncYes.Checked = true; 
            else rbRetainOnResyncNo.Checked = true;

            //Disable controls if user does not have rights to edit this section.
            if (!_permissionIdentification)
            {
                gradeDropdown.Enabled = false;
                subjectDropdown.Enabled = false;
                courseDropdown.Enabled = false;
                sectionTextBox.Enabled = false;
                periodDropdown.Enabled = false;
                semesterDropdown.Enabled = false;
                blockTextBox.Enabled = false;
                schoolDropdown.Enabled = false;
                rbRetainOnResyncYes.Enabled = false;
                rbRetainOnResyncNo.Enabled = false;
            }

        }

        protected void Teacher_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                HyperLink teacherNameLink = (HyperLink)item.FindControl("teacherLink");
                //CheckBox removeTeacherCheckbox = (CheckBox)item.FindControl("removeTeacherCheckbox");
                CheckBox primaryCheckbox = (CheckBox)item.FindControl("primaryCheckbox");
                Thinkgate.Base.Classes.Teacher teacher = (Thinkgate.Base.Classes.Teacher)item.DataItem;

                if (teacherNameLink != null)
                {
                    string xID = Encryption.EncryptInt(teacher.PersonID);
                    teacherNameLink.NavigateUrl = "~/Record/Teacher.aspx?childPage=yes&xID=" + xID;
                    teacherNameLink.Attributes["onclick"] = "window.open('" + teacherNameLink.ResolveClientUrl(teacherNameLink.NavigateUrl) + "'); return false;";
                    teacherNameLink.Attributes["style"] = "color:#00F;";
                    teacherNameLink.Text = teacher.TeacherName;
                }

                //if (removeTeacherCheckbox != null)
                //{
                //    removeTeacherCheckbox.Attributes["value"] = teacher.PersonID.ToString();
                //    removeTeacherCheckbox.InputAttributes["teacherID"] = teacher.PersonID.ToString();

                //    //Disable checkbox if user does not have permission to edit.
                //    if (!_permissionTeacher) removeTeacherCheckbox.Enabled = false;
                //}

                if (primaryCheckbox != null)
                {
                    primaryCheckbox.Checked = teacher.IsPrimary;
                    //primaryCheckbox.Attributes["onclick"] = "primarySelect(this);";
                    primaryCheckbox.InputAttributes["teacherID"] = teacher.PersonID.ToString();

                    if (teacher.IsPrimary)
                    {
                        primaryTeacherID.Value = teacher.PersonID.ToString();
                        primaryTeacherID.Attributes.Remove("initialValue");
                        primaryTeacherID.Attributes.Add("initialValue", primaryTeacherID.Value);
                    }

                    //Disable checkbox if user does not have permission to edit.
                    if (!_permissionTeacher) primaryCheckbox.Enabled = false;
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void Roster_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                HyperLink studentNameLink = (HyperLink)item.FindControl("studentLink");
                //CheckBox removeStudentCheckbox = (CheckBox)item.FindControl("removeStudentCheckbox");
                Thinkgate.Base.Classes.Student student = (Thinkgate.Base.Classes.Student)item.DataItem;

                if (studentNameLink != null)
                {
                    string xID = Encryption.EncryptInt(student.ID);
                    studentNameLink.NavigateUrl = "~/Record/Student.aspx?childPage=yes&xID=" + xID;
                    studentNameLink.Attributes["onclick"] = "window.open('" + studentNameLink.ResolveClientUrl(studentNameLink.NavigateUrl) + "'); return false;";
                    studentNameLink.Attributes["style"] = "color:#00F;";
                    studentNameLink.Text = student.StudentName;
                }

                //if(removeStudentCheckbox != null)
                //{
                //    removeStudentCheckbox.InputAttributes["studentID"] = student.ID.ToString();

                //    //Disable checkbox if user does not have permission to edit.
                //    if (!_permissionRoster) removeStudentCheckbox.Enabled = false;
                //}
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        //private void SaveClass()
        //{
        //    LoadClass();
        //    _selectedClass.SaveClassChanges(_selectedClass.ID,
        //                                    DataIntegrity.ConvertToInt(courseDropdown.SelectedValue),
        //                                    sectionTextBox.Text,
        //                                    periodDropdown.SelectedValue,
        //                                    semesterDropdown.SelectedValue,
        //                                    blockTextBox.Text,
        //                                    rbRetainOnResyncYes.Checked,
        //                                    DataIntegrity.ConvertToInt(schoolDropdown.SelectedValue),
        //                                    SessionObject.LoggedInUser.Page);

        //    Thinkgate.Base.Classes.Cache.Remove("Class_" + _selectedClass.ID.ToString());
        //}

        protected void btnTeacherRemove_Click(object sender, EventArgs e)
        {
            RemoveTeachers();
        }

        private void RemoveTeachers()
        {
            List<int> _lstTeachersRemove = new List<int>();

            //If user does not have permission to edit the "Teacher" section, then skip looping through the check boxes.
            if (_permissionTeacher)
            {
                foreach (GridDataItem item in teachersGrid.Items)
                {
                    CheckBox removeTeacherCheckbox = (CheckBox)item.FindControl("removeTeacherCheckbox");
                    CheckBox primaryCheckbox = (CheckBox)item.FindControl("primaryCheckbox");

                    if (removeTeacherCheckbox.Checked)
                    {
                        if (primaryCheckbox.Checked)
                        {
                            lblTeacherMessage.Text = "Primary teacher cannot be selected for removal";
                            return;
                        }
                        _lstTeachersRemove.Add(DataIntegrity.ConvertToInt(primaryCheckbox.InputAttributes["teacherID"]));
                    }
                }
            }

            Thinkgate.Base.Classes.Class.RemoveTeachers(_lstTeachersRemove, _selectedClass.ID);
            LoadClass();
            _selectedClass.LoadTeachers();
            teachersGrid.DataSource = _selectedClass.Teachers;
            teachersGrid.Rebind();
        }

        protected void primaryCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked && _permissionTeacher)
            {
                foreach (GridDataItem item in teachersGrid.Items)
                {
                    CheckBox primaryCheckbox = (CheckBox)item.FindControl("primaryCheckbox");
                    if (primaryCheckbox != chkBox)
                    {
                        primaryCheckbox.Checked = false;
                    }
                }

                Thinkgate.Base.Classes.Class.SetPrimaryTeacherOnClass(DataIntegrity.ConvertToInt(chkBox.InputAttributes["teacherID"]), _selectedClass.ID);
            }
            else
            {
                chkBox.Checked = true;
            }
        }

        protected void saveButton_Click(object sender, EventArgs e)
        {
            //SaveClass();
        }
    }
}