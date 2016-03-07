using System;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Classes;
using System.Data;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using System.Linq;


namespace Thinkgate.Controls.Student
{
		
		public partial class AddStudent : BasePage
		{
				private const int UserPage = 110;
                private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";
                private bool _userCrossSchools;
                private List<Base.Classes.School> _schools;
                private List<Demographic> _lstDemographics;

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
						var firstName = TextBoxFirstName.Text.Trim();
						var middleName = TextBoxMiddleName.Text.Trim();
						var lastName = TextBoxLastName.Text.Trim();
						var studentId = TextBoxStudentID.Text.Trim();
						var grade = RadComboBoxGrade.SelectedValue.Trim();
						var schoolId = RadComboBoxSchool.SelectedValue.Trim();
                        var gender = cmbGender.SelectedValue.Trim();
                        var race = cmbRace.SelectedValue.Trim();

                        var student = Base.Classes.Data.StudentDB.AddNewStudent(firstName, middleName, lastName, studentId, grade, schoolId, race, gender, SessionObject.LoggedInUser.Page);

						if (student == null)
						{
								LabelStudentIDErrorMessage.Text = "A student already exists with this Student ID.";

								ScriptManager.RegisterStartupScript(this, typeof(AddStudent), "ErrorMessage", "selectTextBoxStudentID();", true);
								return;
						}

						ScriptManager.RegisterStartupScript(this, typeof(AddStudent), "AddedStudent", "autoSizeWindow();", true);

						resultPanel.Visible = true;
						addPanel.Visible = false;

						TextBoxHiddenEncryptedStudentID.Text = Standpoint.Core.Classes.Encryption.EncryptString(string.Format("{0}", student.ID));

						lblResultMessage.Text = "Student successfully added!";
				}

				private void BindPageControls()
				{
                    _lstDemographics = Demographic.GetListOfDemographics();

                    LoadGenders();
                    LoadRaces();

                    RadComboBoxGrade.DataSource = Base.Classes.Grade.GetGradeListForDropDown(UserPage, Permissions);
                    RadComboBoxGrade.DataTextField = "Grade";
                    RadComboBoxGrade.DataValueField = "Grade";
					RadComboBoxGrade.DataBind();
					RadComboBoxGrade.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));


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
                            RadComboBoxSchool.Items.Add(item);
                        }
                    }

                    else
                    {
                        foreach (ThinkgateSchool school in SessionObject.LoggedInUser.Schools)
                        {
                            RadComboBoxItem item = new RadComboBoxItem();
                            item.Text = school.Name;
                            item.Value = school.Id.ToString();
                            RadComboBoxSchool.Items.Add(item);
                        }
                    }
                    
					// Show select if more than one option available.
                    if (RadComboBoxSchool.Items.Count > 1)
						RadComboBoxSchool.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));
				}

                private void LoadRaces()
                {
                    cmbRace.DataSource = _lstDemographics.FindAll(x => x.Label.Trim().ToLower() == "race");
                    cmbRace.DataTextField = "Abbreviation";
                    cmbRace.DataValueField = "Value";
                    cmbRace.DataBind();

                    if (cmbRace.Items.Count > 1)
                        cmbRace.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));
                }

                private void LoadGenders()
                {
                    cmbGender.DataSource = _lstDemographics.FindAll(x => x.Label.Trim().ToLower() == "gender");
                    cmbGender.DataTextField = "Abbreviation";
                    cmbGender.DataValueField = "Value";
                    cmbGender.DataBind();

                    if (cmbGender.Items.Count > 1)
                        cmbGender.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));
                }
		}
}
