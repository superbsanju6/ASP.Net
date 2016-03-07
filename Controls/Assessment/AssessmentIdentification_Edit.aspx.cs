using System;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using System.Data;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentIdentification_Edit : BasePage
	{
		protected Int32 _userID, _assessmentID, _teacherID;
		protected Thinkgate.Base.Classes.Assessment _assessment;
		protected DataTable _dtGrade, _dtSubject, _dtCourse, _dtTestType, _dtTerm;
		protected String cacheKey;

		protected void Page_Load(object sender, EventArgs e)
		{
			_userID = SessionObject.LoggedInUser.Page;

			if(_assessment == null)
				LoadAssessment();

			lblAuthor.Text = _assessment.CreatedByName;
			lblCreated.Text = _assessment.DateCreated.ToShortDateString();

			if(!IsPostBack)
			{
				tbxDescription.Text = _assessment.Description;
				BuildGrades();
			}
		}

		private void LoadAssessment()
		{
			if (Request.QueryString["xID"] == null ||
                    (_assessmentID = GetDecryptedEntityId(X_ID)) <= 0)
			{
				SessionObject.RedirectMessage = "No assessment ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else if (Request.QueryString["yID"] == null ||
							 (_teacherID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "yID")) <= 0)
			{
				SessionObject.RedirectMessage = "No teacher ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else
			{
				cacheKey = "Assessment_" + Request.QueryString["xID"]; 

				if(!RecordExistsInCache(cacheKey))
				{
					_assessment = Base.Classes.Assessment.GetAssessmentAndQuestionsByID(_assessmentID);
					if(_assessment != null)
						Thinkgate.Base.Classes.Cache.Insert(cacheKey, _assessment);
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

		protected void BuildGrades()
		{
            _dtGrade = Thinkgate.Base.Classes.Data.TeacherDB.GetGradesForTeacher(_teacherID, _userID);
			cmbGrade.DataSource = _dtGrade;
			cmbGrade.DataBind();

			// Set text to null so that the "EmptyMessage" will show. In this case it is <Select One>.
			// Setting the current index to -1 does not seem to work.
			cmbGrade.Text = null;
			for (Int32 i = 0; i < _dtGrade.Rows.Count; i++)
			{
				if(String.Compare(_assessment.Grade, (String)_dtGrade.Rows[i]["Grade"], true) == 0)
				{
					cmbGrade.SelectedIndex = i;
					break;
				}
			}

			BuildSubjects();
		}

		protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessment.Grade = e.Text;
			// Set the subject to null so that the "EmptyMessage" will be shown.
			_assessment.Subject = null;
			BuildSubjects();
		}


		protected void BuildSubjects()
		{
            _dtSubject = Thinkgate.Base.Classes.Data.TeacherDB.GetSubjectsForTeacher(_teacherID, _assessment.Grade, _userID);
			cmbSubject.DataSource = _dtSubject;
			cmbSubject.DataBind();

			cmbSubject.Text = null;
			for (Int32 i = 0; i < _dtSubject.Rows.Count; i++)
			{
				if (String.Compare(_assessment.Subject, (String)_dtSubject.Rows[i]["SubjectValue"], true) == 0)
				{
					cmbSubject.SelectedIndex = i;
					break;
				}
			}

			BuildCourses();
		}

		protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessment.Subject = e.Text;
			_assessment.Course = null;
			BuildCourses();
		}


		protected void BuildCourses()
		{
            _dtCourse = Thinkgate.Base.Classes.Data.TeacherDB.GetCoursesForTeacher(_teacherID, _assessment.Grade, _assessment.Subject, _userID);
			cmbCourse.DataSource = _dtCourse;
			cmbCourse.DataBind();

			cmbCourse.Text = null;
			for (Int32 i = 0; i < _dtCourse.Rows.Count; i++)
			{
				if (String.Compare(_assessment.Course, (String)_dtCourse.Rows[i]["CourseValue"], true) == 0)
				{
					cmbCourse.SelectedIndex = i;
					break;
				}
			}

			BuildTestTypes();
		}

		protected void cmbCourse_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessment.Course = e.Text;
			_assessment.TestType = null;
			BuildTestTypes();
		}

		protected void BuildTestTypes()
		{
            _dtTestType = Base.Classes.Assessment.GetTestTypes("Classroom", 0);
			cmbTestType.DataSource = _dtTestType;
			cmbTestType.DataBind();

			cmbTestType.Text = null;
			for (Int32 i = 0; i < _dtTestType.Rows.Count; i++)
			{
				if (String.Compare(_assessment.TestType, (String)_dtTestType.Rows[i]["Type"], true) == 0)
				{
					cmbTestType.SelectedIndex = i;
					break;
				}
			}

			BuildTerms();
		}

		protected void cmbTestType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessment.TestType = e.Text;
			_assessment.Term = null;
			BuildTerms();
		}

		protected void BuildTerms()
		{
			_dtTerm = Thinkgate.Base.Classes.Assessment.GetTerms(_assessmentID, _userID);
			cmbTerm.DataSource = _dtTerm;
			cmbTerm.DataBind();

			cmbTerm.Text = null;
			for (Int32 i = 0; i < _dtTerm.Rows.Count; i++)
			{
				if (String.Compare(_assessment.Term, _dtTerm.Rows[i]["Term"].ToString(), true) == 0)
				{
					cmbTerm.SelectedIndex = i;
					break;
				}
			}

			SetOkState();
		}

		protected void cmbTerm_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessment.Term = e.Text;
			SetOkState();
		}

		protected void tbxDescription_TextChanged(object sender, EventArgs e)
		{
			_assessment.Description = tbxDescription.Text;
		}

		protected void SetOkState()
		{
			okButton.Enabled = !String.IsNullOrEmpty(_assessment.Grade) && !String.IsNullOrEmpty(_assessment.Subject) &&
												 !String.IsNullOrEmpty(_assessment.Course) && !String.IsNullOrEmpty(_assessment.TestType) &&
												 !String.IsNullOrEmpty(_assessment.Term);
		}

		protected void okButton_Click(object sender, EventArgs e)
		{
			Thinkgate.Base.Classes.Assessment.SaveIdentificationInformation(_assessment, _userID);
			Thinkgate.Base.Classes.Cache.Insert(cacheKey, _assessment);
		}

		protected void cancelButton_Click(object sender, EventArgs e)
		{
		}


	}
}