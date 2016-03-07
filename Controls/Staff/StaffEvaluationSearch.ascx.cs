using System;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Data;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Linq;

namespace Thinkgate.Controls.Teacher
{
		public partial class StaffEvaluationSearch : TileControlBase
	{
		public static Base.Enums.EntityTypes _level;
		public static int _levelID;
		protected Boolean _isPostBack;
		protected EvaluationTypes _evalType;

		protected Boolean _yearVisible, _schoolVisible, _gradeVisible, _nameVisible, _permLinkActive;

		// View state keys.
		protected String _yearFilterKey = "evalYearFilter";
		protected String _schoolFilterKey = "evalSchoolFilter";
		protected String _gradeFilterKey = "evalGradeFilter";
		protected String _nameFilterKey = "evalNameFilter";

		protected new void Page_Init(object sender, EventArgs e)
		{
			base.Page_Init(sender, e);

			if (Tile == null)
				return;

			// _level is Thinkgate.Base.Enums.EntityTypes.School or Thinkgate.Base.Enums.EntityTypes.District.
			_level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
			// LevelID is the school or district id.
			_levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));
			_evalType = (Thinkgate.Base.Enums.EvaluationTypes)Tile.TileParms.GetParm("evalType");
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Simulate IsPostBack.
			String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
			_isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            /* Depending on the permissions that the user has, the staff name will be a hyperlink
             * taking the user to the staff profile page if clicked. If no permissions, then do
             * not display as hyperlink
             */
		    switch (_evalType)
		    {
                case Thinkgate.Base.Enums.EvaluationTypes.TeacherClassroom:
                    _permLinkActive = (UserHasPermission(Permission.Hyperlink_TeacherName_CIS));
                    break;

                case Thinkgate.Base.Enums.EvaluationTypes.TeacherNonClassroom:
                    _permLinkActive = (UserHasPermission(Permission.Hyperlink_TeacherName_NCIS));
                    break;

                case Thinkgate.Base.Enums.EvaluationTypes.Administrator:
		            _permLinkActive = (UserHasPermission(Permission.Hyperlink_AdminName_SBAS));
		            break;

                default:
		            _permLinkActive = false;
		            break;
		    }


			// Create the initial viewstate values.
			if (ViewState[_gradeFilterKey] == null)
			{               
				ViewState.Add(_yearFilterKey, "All");
                ViewState.Add(_schoolFilterKey, "0"); //BJC 7/27/2012: Change requested - Set to 0 since user is not forced to select a school. They do not want search loaded for all schools due to performance.
                //ViewState.Add(_schoolFilterKey, "All");
				ViewState.Add(_gradeFilterKey, "All");
				ViewState.Add(_nameFilterKey, "All");
			}

			SetFilterVisibility();

			if (!_isPostBack)
			{
				BuildSchools();

                //BJC 7/27/2012: Change requested - force user to make initial search due to performance issues with the staff filter taking too long to load for all schools.
                if (!_schoolVisible)
                {
                    BuildYears();
                    BuildGrades();
                    BuildNames();
                    BuildUI();
                }
			}

		}

		protected void SetFilterVisibility()
		{
			_yearVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;
			_schoolVisible = _level == Base.Enums.EntityTypes.District;
			_gradeVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;
			_nameVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;
						
			cmbYear.Visible = _yearVisible;
			cmbSchool.Visible = _schoolVisible;
			cmbGrade.Visible = _gradeVisible;
			cmbName.Visible = _nameVisible;       
		}

		protected void BuildYears()
		{
			if (_yearVisible)
			{
				DataTable dtYears = Thinkgate.Base.Classes.Assessment.GetYears();
				DataColumn ddCol = dtYears.Columns.Contains("DropdownText") ? dtYears.Columns[1] : dtYears.Columns.Add("DropdownText", typeof(String));
				foreach(DataRow row in dtYears.Rows)
					row[ddCol] = row["Year"];					
                /* BJC 7/27/2012: Change request - Removed "All" selection.
				DataRow newRow = dtYears.NewRow();
				newRow["Year"] = "Year";
				newRow[ddCol] = "All";
				dtYears.Rows.InsertAt(newRow, 0);
                */

				// Data bind the combo box.
				cmbYear.DataTextField = "Year";
				cmbYear.DataValueField = "Year";
				cmbYear.DataSource = dtYears;
				cmbYear.DataBind();
                cmbYear.SelectedIndex = cmbYear.Items[cmbYear.Items.Count - 1].Index; //BJC 7/27/2012: Change request - Set default selection to lowest year.
			    ViewState[_yearFilterKey] = cmbYear.Items[cmbYear.Items.Count - 1].Value;
			}
		}

		protected void cmbYear_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			// Sets to "All, 11-12, etc".
			ViewState[_yearFilterKey] = e.Value;
			cmbYear.Text = e.Text;
			BuildNames();
			BuildUI();
		}

		protected void BuildSchools()
		{
			if (_schoolVisible)
			{
				List<ThinkgateSchool> allSchools;
				if(_level == EntityTypes.District)
					allSchools = ThinkgateSchool.GetSchoolCollectionForApplication();
				else
                    allSchools = ThinkgateSchool.GetSchoolCollectionForUser(SessionObject.LoggedInUser, UserHasPermission(Permission.User_Cross_Schools) ? 1 : 0);

				allSchools = (from s in allSchools orderby s.Name select s).ToList();

				DataTable dtSchools = new DataTable();
				DataColumn nameCol = dtSchools.Columns.Add("SchoolName", typeof(String));
				DataColumn abbrevCol = dtSchools.Columns.Add("Abbrev", typeof(String));
				DataColumn idCol = dtSchools.Columns.Add("ID", typeof(Int32));
				foreach(ThinkgateSchool school in allSchools)
				{
					DataRow row = dtSchools.NewRow();
					row[nameCol] = school.Name;
					row[abbrevCol] = school.Abbreviation;
					row[idCol] = school.Id;
					dtSchools.Rows.Add(row);
				}

                /*
				DataRow newRow = dtSchools.NewRow();
				newRow[nameCol] = "All";
				newRow[abbrevCol] = "School";
				newRow[idCol] = 0;
				dtSchools.Rows.InsertAt(newRow, 0);
                */

				// Data bind the combo box.
				cmbSchool.DataTextField = "Abbrev";
				cmbSchool.DataValueField = "ID";
				cmbSchool.DataSource = dtSchools;
				cmbSchool.DataBind();
			}
            else
			{
                List<ThinkgateSchool> allSchools;
                allSchools = ThinkgateSchool.GetSchoolCollectionForUser(SessionObject.LoggedInUser, UserHasPermission(Permission.User_Cross_Schools) ? 1 : 0);
                allSchools = (from s in allSchools orderby s.Name select s).ToList();
			    ViewState[_schoolFilterKey] = allSchools.Count > 0 ? allSchools[0].Id.ToString() : "0";
			}
		}

		protected void cmbSchool_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			// Sets to school id as a string.
			ViewState[_schoolFilterKey] = e.Value;
			cmbSchool.Text = e.Text;
            BuildYears();
            BuildGrades();
			BuildNames();
			BuildUI();
		}

		protected void BuildGrades()
		{
			if (_gradeVisible)
			{
				List<Grade> grades = null;
				// We only want the grades for this school.
                if (_level == EntityTypes.School)
                {
                    ThinkgateSchool school = new ThinkgateSchool(_levelID, false);
                    
                    grades = CourseMasterList.GradesForSchoolsDict[_levelID];

                    //If the school type is not high school then remove high school grades from the dropdown.
                    //MKR 07/30/2012 - We need to uncomment this but didn't want this to go to Sarasota yet.
                    /*if (school.SchoolType == "Middle" || school.SchoolType == "Elementary")
                    {
                        grades.RemoveAll(g => g.ToString().Equals("9_12") || g.ToString().Equals("09") || g.ToString().Equals("10") || g.ToString().Equals("11") || g.ToString().Equals("12"));
                    }*/
                }
                // All grades for district.
                else if (_level == EntityTypes.District)
                    grades = (from c in CourseMasterList.ClassCourseDict.Values select c.Grade).Distinct().ToList();

				if(grades == null)
					return;

				DataTable dtGrade = new DataTable();
				DataColumn gradeCol = dtGrade.Columns.Add("Grade", typeof(String));

				// Add the grade names to the data table (making sure that they are unique).
				IEnumerable<String> gradeNames = (from g in grades
																					select g.DisplayText).Distinct();
				// Sort the grades.
				gradeNames = gradeNames.OrderBy(n => n, new GradeDisplayTextComparer());

				foreach (String gradeName in gradeNames)
				{
					DataRow row = dtGrade.NewRow();
					row["Grade"] = gradeName;
					dtGrade.Rows.Add(row);
				}

				// The only existing column is 'Grade'. We must add a column for 'CmbText'.
				dtGrade.Columns.Add("CmbText", typeof(String));
				foreach (DataRow row in dtGrade.Rows)
					row["CmbText"] = row["Grade"];
				DataRow newRow = dtGrade.NewRow();
				newRow["Grade"] = "All";
				newRow["CmbText"] = "Grade";
				dtGrade.Rows.InsertAt(newRow, 0);

				// Data bind the combo box.
				cmbGrade.DataTextField = "CmbText";
				cmbGrade.DataValueField = "Grade";
				cmbGrade.DataSource = dtGrade;
				cmbGrade.DataBind();

				// Initialize the current selection.
				RadComboBoxItem item = cmbGrade.Items.FindItemByValue((String)ViewState[_gradeFilterKey], true);
				Int32 selIdx = cmbGrade.Items.IndexOf(item);
				cmbGrade.SelectedIndex = selIdx;
			}
		}

		protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			// Sets to "06, 07, etc".
			ViewState[_gradeFilterKey] = e.Value;

			cmbGrade.Text = e.Text;
			BuildNames();
			BuildUI();
		}

		protected void BuildNames()
		{
			if (_nameVisible)
			{
                DataTable dtStaff = Thinkgate.Base.Classes.Staff.GetStaffForEvaluations(_evalType, ViewState[_schoolFilterKey].ToString(), ViewState[_gradeFilterKey].ToString(), ViewState[_yearFilterKey].ToString());
				DataColumn pageStr = dtStaff.Columns.Add("PageStr", typeof(String));
				DataColumn dropdownStr = dtStaff.Columns.Add("DropdownText", typeof(String));
				foreach (DataRow row in dtStaff.Rows)
				{
					row[pageStr] = row["Page"].ToString();
					row[dropdownStr] = row["User_Full_Name"].ToString();
				}

				DataRow newRow = dtStaff.NewRow();
				newRow[dropdownStr] = "All";
				newRow["User_Full_Name"] = "Name";
				newRow[pageStr] = "All";
				dtStaff.Rows.InsertAt(newRow, 0);

				// Data bind the combo box.
				cmbName.DataTextField = "User_Full_Name";
				cmbName.DataValueField = "PageStr";
				cmbName.DataSource = dtStaff;
				cmbName.DataBind();
			}
		}

		protected void cmbName_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			ViewState[_nameFilterKey] = e.Value;
			cmbName.Text = e.Text;
			BuildUI();
		}

		private void BuildUI()
		{
			// Get the evaluations table. There are three user types that may have evaluations, Classroom Teacher (CI), Non-Classroom Teacher (NCI),
			// and School-Based Administrator (SA).
			//
			// It has columns:
			// Int32 ID	(evaluation id)
			// String Type (One of EvaluationTypes as string).
			// String EvalName
			// String User_Full_Name
		    lblInitialText.Visible = false;
			DataTable dtEval = Thinkgate.Base.Classes.Staff.GetStaffEvaluations(_evalType, ViewState[_yearFilterKey].ToString(),
																				ViewState[_schoolFilterKey].ToString(), ViewState[_gradeFilterKey].ToString(), ViewState[_nameFilterKey].ToString());
			if(dtEval.Rows.Count > 0)
			{
				dtEval = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtEval, "ID", "ID_Encrypted");
				dtEval = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtEval, "UserPage", "UID_Encrypted");
				DataColumn saCol = dtEval.Columns.Add("IsSA", typeof(Boolean));
				foreach (DataRow row in dtEval.Rows)
				{
					// Limit the name length.
//					row["User_Full_Name"] = row["User_Full_Name"].ToString().Substring(0, 20);
					row[saCol] = (String.Compare(row["Type"].ToString(), EvaluationTypes.Administrator.ToString(), true) == 0);
				}

				dtEval.DefaultView.Sort = "User_Full_Name, EvalName";
				lblNoResults.Visible = false;
				grdEval.Visible = true;
				grdEval.DataSource = dtEval;
				grdEval.DataBind();	
			}
			else
			{
				grdEval.DataSource = null;
				grdEval.Visible = false;
				lblNoResults.Visible = true;
			}
		}


	}
}