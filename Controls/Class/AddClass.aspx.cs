namespace Thinkgate.Controls.Class
{
		using System;
		using System.Web.UI;
		using Telerik.Web.UI;
		using Thinkgate.Classes;

		public partial class AddClass : BasePage
		{
				private const int UserPage = 110;
                SessionObject _sessionObject;
				private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";

				protected void Page_Load(object sender, EventArgs e)
				{
						if (!IsPostBack)
						{
								BindPageControls();
						}

						if (Request.Form["__EVENTTARGET"] == "RadButtonOk")
						{
								ButtonOkClick(this, new EventArgs());
						}
				}

				protected void ButtonOkClick(object sender, EventArgs e)
				{
						var schoolID = RadComboBoxSchool.SelectedValue;
						var grade = RadComboBoxGrade.SelectedValue;
						var subject = RadComboBoxSubject.SelectedValue;
						var course = RadComboBoxCourse.SelectedValue;
						var year = RadComboBoxYear.SelectedValue;
						var semester = RadComboBoxSemester.SelectedValue;
						var period = RadComboBoxPeriod.SelectedValue;

						var newClass = Base.Classes.Class.AddClass(schoolID, grade, subject, course, semester, period, UserPage);

						if (newClass == null)
						{
								LabelGenericErrorMessage.Text = "There was an error adding this class. Please check your data and try again.";

								ScriptManager.RegisterStartupScript(this, typeof(AddClass), "ErrorMessage", "autoSizeWindow();", true);
								return;
						}

						ScriptManager.RegisterStartupScript(this, typeof(AddClass), "AddedSchool", "autoSizeWindow();", true);

						resultPanel.Visible = true;
						addPanel.Visible = false;

						TextBoxHiddenEncryptedClassID.Text = Standpoint.Core.Classes.Encryption.EncryptString(string.Format("{0}", newClass.ID));

						lblResultMessage.Text = "Class successfully added";
				}

				protected void RadComboBoxGradeSelectedIndexChanged(object sender, EventArgs e)
				{
						LoadSubjectDropDownItems();
						RegisterAutoResizeWindow();
				}

				protected void RadComboBoxSubjectSelectedIndexChanged(object sender, EventArgs e)
				{
						LoadCourseDropDownItems();
						RegisterAutoResizeWindow();
				}

				private void BindPageControls()
				{
						// School
                        _sessionObject = (SessionObject)Session["SessionObject"]; //Code Added for TFS #17991/19169
                        RadComboBoxSchool.DataSource = Base.Classes.School.GetSchoolListForDropDown(_sessionObject.LoggedInUser.Page);
						RadComboBoxSchool.DataTextField = "TXT";
						RadComboBoxSchool.DataValueField = "VAL";
						RadComboBoxSchool.DataBind();
						RadComboBoxSchool.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));

						// Grade                    
						RadComboBoxGrade.DataSource = Base.Classes.Grade.GetGradeListForDropDown(UserPage, Permissions);
						RadComboBoxGrade.DataTextField = "Grade";
                        RadComboBoxGrade.DataValueField = "Grade";
						RadComboBoxGrade.DataBind();
						RadComboBoxGrade.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));

						// Subject
						LoadSubjectDropDownItems();

						// Course
						LoadCourseDropDownItems();

						// Year
						String year = Thinkgate.Base.Classes.DistrictParms.LoadDistrictParms().Year;
						RadComboBoxYear.Items.Insert(0, (new RadComboBoxItem { Text = year, Value = year }));

						// Semester
						RadComboBoxSemester.DataTextField = "Semester";
						RadComboBoxSemester.DataValueField = "Semester";
						RadComboBoxSemester.DataSource = Base.Classes.Semester.GetSemesterListForDropDown();
						RadComboBoxSemester.DataBind();
						RadComboBoxSemester.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));

						// Period
						RadComboBoxPeriod.DataTextField = "Period";
						RadComboBoxPeriod.DataValueField = "Period";
						RadComboBoxPeriod.DataSource = Base.Classes.PeriodMasterList.GetPeriodDataTableForDropDown("Period");
						RadComboBoxPeriod.DataBind();
						RadComboBoxPeriod.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));
				}

				private void LoadSubjectDropDownItems()
				{
						RadComboBoxSubject.Items.Clear();
						RadComboBoxSubject.DataSource = Base.Classes.Subject.GetSubjectListForDropDown(RadComboBoxGrade.SelectedValue, UserPage, Permissions);
						RadComboBoxSubject.DataTextField = "ListVal";
						RadComboBoxSubject.DataValueField = "ListVal";
						var item = new RadComboBoxItem { Text = "Select", Value = "0" };
						RadComboBoxSubject.DataBind();
						RadComboBoxSubject.Items.Insert(0, item);
				}

				private void LoadCourseDropDownItems()
				{
						RadComboBoxCourse.Items.Clear();
						RadComboBoxCourse.DataSource = Base.Classes.Course.GetCourseListForDropDown(RadComboBoxGrade.SelectedValue, RadComboBoxSubject.SelectedValue, UserPage, Permissions);
						RadComboBoxCourse.DataTextField = "ListVal";
						RadComboBoxCourse.DataValueField = "ListVal";
						var item = new RadComboBoxItem { Text = "Select", Value = "0" };
						RadComboBoxCourse.DataBind();
						RadComboBoxCourse.Items.Insert(0, item);
				}

				private void RegisterAutoResizeWindow()
				{
						ScriptManager.RegisterStartupScript(this, typeof(AddClass), "ResizeWindow", "autoSizeWindow();", true);
				}
		}
}
