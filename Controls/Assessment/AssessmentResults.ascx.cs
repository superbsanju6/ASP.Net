using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Enums.AssessmentScheduling;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentResults : TileControlBase
    {
        // Level will be one of State, District, Teacher, School, Class;
        protected Thinkgate.Base.Enums.EntityTypes _level;
        // Will be districtID, schoolID, teacherID, classID.
        protected Int32 _levelID;
        // Will be Assessment or Reporting.
        protected String _folder;
        // This parameter is optional and set to classID if a class is selected in the teacher screen.
        protected Int32? _selectID;
        // True if this is a postback.
        protected Boolean _isPostBack;
//#if false // Unused for now.
		// This parameter is optional and set to the single category to display instead of having the
		// tabs at the bottom of the tile.
		protected String _category;
//#endif
        // Only one of these will be non-null.
        protected Thinkgate.Base.Classes.Class _class;
        protected Thinkgate.Base.Classes.District _district;
        protected Thinkgate.Base.Classes.School _school;
        protected Thinkgate.Base.Classes.Teacher _teacher;
        private CourseList _currCourseList;

        /*
         Assessment filters are a row of buttons with dropdowns to select subsets of the assessments.
         Assessment category tabs are shown at the bottom of the tile.
         Not all filter buttons and tabs are shown for a given screen.
         Here is a summary of filter button and tab visibility:
                Level				Folder  			Filters													Tabs
                ------------------------------------------------------------------------
                State - not yet implemented
                District		Reporting			Grade, Subject, Term, Type			State, District, Classroom
                School			Reporting			Grade, Subject, Term, Type			State, District, Classroom
                Teacher			Classes				Term, Type											State, District, Classroom
                Teacher			Reporting			Subject, Term, Type							State, District, Classroom
        */

        // Which filter buttons are visible.
        protected Boolean _gradeVisible, _subjectVisible, _termVisible, _testTypeVisible, _editItemMode;

        // View state keys.
        const String _currentViewIdxKey = "ArCurrentViewIdx", _gradeFilterKey = "ArGradeFilterIdx", _subjectFilterKey = "ArSubjectFilter";
        const String _termFilterKey = "ArTermFilter", _testTypeFilterKey = "ArTestTypeFilter";

        // The current user.
        protected Int32 _userID;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            LoadReportScripts();

            if (Tile == null) return;

            _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = (Int32)Tile.TileParms.GetParm("levelID");
            _class = Tile.TileParms.GetParm("class") != null ? (Base.Classes.Class)Tile.TileParms.GetParm("class") : new Base.Classes.Class();
            _folder = Tile.TileParms.GetParm("folder").ToString();
            _selectID = (Int32?)Tile.TileParms.GetParm("selectID");
            _category = Tile.TileParms.GetParm("category") != null ? Tile.TileParms.GetParm("category").ToString() : string.Empty;

            AssessmentResults_level.Value = _level.ToString();
            AssessmentResults_levelID.Value = Standpoint.Core.Classes.Encryption.EncryptInt(_levelID);
            
            // set default tab
            SetDefaultTab();            
        }

        /// <summary>
        /// Sets the selected tab based on the passed category.
        /// </summary>
        private void SetDefaultTab()
        {
            if (_category.ToLower() == "state")
            {
                stateRadTab.Selected = true;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            // Simulate IsPostBack.
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");
            
            // Create the initial viewstate values.
            if (ViewState[_currentViewIdxKey] == null)
            {
                ViewState.Add(_currentViewIdxKey, 0);
                ViewState.Add(_gradeFilterKey, "All");
                ViewState.Add(_subjectFilterKey, "All");
                ViewState.Add(_termFilterKey, "All");
                ViewState.Add(_testTypeFilterKey, "All");
            }

            _userID = SessionObject.LoggedInUser.Page;

            // Set the current filter visibility.
            SetFilterVisibility();
            SetGridHeaderText();
            // Set the tab visibility.
            stateRadTab.Visible = UserHasPermission(Base.Enums.Permission.Tab_State_AssessmentResults);
            districtRadTab.Visible = UserHasPermission(Base.Enums.Permission.Tab_District_AssessmentResults);
            classroomRadTab.Visible = UserHasPermission(Base.Enums.Permission.Tab_Classsroom_AssessmentResults);

            switch (AssessmentResults_RadTabStrip.SelectedTab.Text)
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

            if (!_isPostBack)
            {
                var testSubjectDT = Subject.GetSubjectsByTests();
                _currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
                _currCourseList.RemoveAll(course => { return testSubjectDT.Select("Subject = '" + course.Subject.DisplayText + "'").Length == 0; });

                BuildGrades();
                BuildSubjects();
                BuildTerms();
                BuildTestTypes();
            }
            BuildAssessments();

        }

        private void LoadReportScripts()
        {
            if (Page.Master != null)
            {
                var scriptManager = Page.Master.FindControl("RadScriptManager1");
                if (scriptManager != null)
                {
                    RadScriptManager radScriptManager = (RadScriptManager)scriptManager;

                    radScriptManager.Scripts.Add(
                        new ScriptReference("~/Scripts/AssessmentResults.js"));
                }
            }

            if (!string.IsNullOrEmpty(DistrictParms.LoadDistrictParms().SuggestedResources) && DistrictParms.LoadDistrictParms().SuggestedResources.Trim().Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
            {
                IsSuggestedResourcesVisible.Value = "true";
            }
        }

        private void SetGridHeaderText()
        {
            if (_level == EntityTypes.Student)
            {
                rgAssessmentResults.MasterTableView.Columns[1].HeaderText = "Score";
            }
            else
            {
                string assessmentResultsTileLabel = DistrictParms.LoadDistrictParms().AssessmentResultsTileLabel;

                rgAssessmentResults.MasterTableView.Columns[1].HeaderText = assessmentResultsTileLabel;
            }
        }

        protected void BuildGrades()
        {
            cmbGrade.Visible = _gradeVisible;
            if (_gradeVisible)
            {
                var gradeList = _currCourseList.GetGradeList();
                var dtGrade = new DataTable();
                dtGrade.Columns.Add("Grade");
                dtGrade.Columns.Add("CmbText");
                foreach(var grade in gradeList)
                {
                    var row = dtGrade.NewRow();
                    row["Grade"] = grade.DisplayText;
                    row["CmbText"] = grade.DisplayText;
                    dtGrade.Rows.Add(row);
                }

                DataRow newRow = dtGrade.NewRow();
                newRow["Grade"] = "All";
                newRow["CmbText"] = "Grade";
                dtGrade.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbGrade.DataTextField = "CmbText";
                cmbGrade.DataValueField = "Grade";
                cmbGrade.DataSource = dtGrade;
                cmbGrade.DataBind();

                // Initialize the current selection. Sometimes the filter item no longer exists when changing
                // tabs from School, District, Classroom.
                RadComboBoxItem item = this.cmbGrade.Items.FindItemByValue((String)this.ViewState[_gradeFilterKey], true) ?? this.cmbGrade.Items[0];
                ViewState[_gradeFilterKey] = item.Value;
                Int32 selIdx = cmbGrade.Items.IndexOf(item);
                cmbGrade.SelectedIndex = selIdx;
            }
        }

        protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_gradeFilterKey] = e.Value;

            SetFilterVisibility();
            BuildAssessments();
        }

        protected void BuildSubjects()
        {
            cmbSubject.Visible = _subjectVisible;
            if (_subjectVisible)
            {
                // Now load the filter button tables and databind.
                var subjectList = _currCourseList.GetSubjectList();
                var dtSubject = new DataTable();
                dtSubject.Columns.Add("Subject");
                dtSubject.Columns.Add("DropdownText");
                dtSubject.Columns.Add("CmbText");
                foreach (var subject in subjectList)
                {
                    var row = dtSubject.NewRow();
                    row["Subject"] = subject.DisplayText;
                    row["DropdownText"] = subject.DisplayText + (!string.IsNullOrEmpty(subject.Abbreviation) && subject.Abbreviation != subject.DisplayText ? " (" + subject.Abbreviation + ")" : "");
                    row["CmbText"] = subject.Abbreviation;
                    dtSubject.Rows.Add(row);
                }

                DataRow newRow = dtSubject.NewRow();
                newRow["Subject"] = "All";
                newRow["DropdownText"] = "All";
                newRow["CmbText"] = "Subject";
                dtSubject.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbSubject.DataTextField = "CmbText";
                cmbSubject.DataValueField = "Subject";
                cmbSubject.DataSource = dtSubject;
                cmbSubject.DataBind();

                // Initialize the current selection. Sometimes the filter item no longer exists when changing
                // tabs from School, District, Classroom.
                RadComboBoxItem item = this.cmbSubject.Items.FindItemByValue((String)this.ViewState[_subjectFilterKey], true) ?? this.cmbSubject.Items[0];
                ViewState[_subjectFilterKey] = item.Value;
                Int32 selIdx = cmbSubject.Items.IndexOf(item);
                cmbSubject.SelectedIndex = selIdx;
            }
        }

        protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_subjectFilterKey] = e.Value;

            SetFilterVisibility();
            BuildAssessments();
        }

        protected void BuildTerms()
        {
            cmbTerm.Visible = _termVisible;
            if (_termVisible)
            {
                DataTable dtTerm = Thinkgate.Base.Classes.Assessment.LoadTerms(_userID);
                // The only existing column is 'Term'. We must add a column for 'CmbText'.
                dtTerm.Columns.Add("CmbText", typeof(String));
                foreach (DataRow row in dtTerm.Rows)
                    row["CmbText"] = "Term " + row["Term"];
                DataRow newRow = dtTerm.NewRow();
                newRow["Term"] = "All";
                newRow["CmbText"] = "Term";
                dtTerm.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbTerm.DataTextField = "CmbText";
                cmbTerm.DataValueField = "Term";
                cmbTerm.DataSource = dtTerm;
                cmbTerm.DataBind();

                // Initialize the current selection. Sometimes the filter item no longer exists when changing
                // tabs from School, District, Classroom.
                RadComboBoxItem item = this.cmbTerm.Items.FindItemByValue((String)this.ViewState[_termFilterKey], true) ?? this.cmbTerm.Items[0];
                ViewState[_termFilterKey] = item.Value;
                Int32 selIdx = cmbTerm.Items.IndexOf(item);
                cmbTerm.SelectedIndex = selIdx;
                // Set the hidden field used for navigation.
                AssessmentResults_term.Value = item.Text;
            }
        }

        protected void cmbTerm_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBoxItem item = cmbTerm.Items.FindItemByText(e.Text, true);
            ViewState[_termFilterKey] = item.Value;

            SetFilterVisibility();
            BuildAssessments();


            Int32 selIdx = cmbTerm.Items.IndexOf(item);
            cmbTerm.SelectedIndex = selIdx;
            AssessmentResults_term.Value = item.Value;
        }

        protected void BuildTestTypes()
        {
            cmbTestType.Visible = _testTypeVisible;
            if (_testTypeVisible)
            {
                List<TestType> lstTestType = TestTypes.GetTestTypesForDropDownsOnAssessmentResultsTile(AssessmentResults_RadTabStrip.SelectedTab.Text);
                
                if(!UserHasPermission(Permission.Access_SecureTesting) || !UserHasPermission(Permission.Access_SecureTesting_viewAssessmentResults))
                {
                    lstTestType = lstTestType.Where(x => !x.Secure).ToList();
                }
                
                // The existing columns are 'Type' and 'Abbreviation'.
                // Add a column for 'DropdownText'.
               
                // Data bind the combo box.
                cmbTestType.DataTextField = "Abbreviation";
                cmbTestType.DataValueField = "Type";
                cmbTestType.DataSource = lstTestType;
                cmbTestType.DataBind();

                // Initialize the current selection. Sometimes the filter item no longer exists when changing
                // tabs from School, District, Classroom.
                RadComboBoxItem item = this.cmbTestType.Items.FindItemByValue((string)this.ViewState[_testTypeFilterKey], true) ?? this.cmbTestType.Items[0];
                ViewState[_testTypeFilterKey] = item.Value;
                Int32 selIdx = cmbTestType.Items.IndexOf(item);
                cmbTestType.SelectedIndex = selIdx;

                // Set the hidden field used for navigation.
                AssessmentResults_type.Value = item.Text;
            }
        }

        protected void cmbTestType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBoxItem item = cmbTestType.Items.FindItemByText(e.Text, true);
            ViewState[_testTypeFilterKey] = item.Value;

            SetFilterVisibility();
            BuildAssessments();

            Int32 selIdx = cmbTestType.Items.IndexOf(item);
            cmbTestType.SelectedIndex = selIdx;
            AssessmentResults_type.Value = item.Value;
        }

        protected void BuildAssessments()
        {
            System.Data.SqlClient.SqlParameterCollection parms = new System.Data.SqlClient.SqlCommand().Parameters;
            parms.AddWithValue("UserID", _userID);
            parms.AddWithValue("Level", _level.ToString());
            parms.AddWithValue("LevelID", _levelID);
            parms.AddWithValue("ClassID", _class.ID);
            parms.AddWithValue("TestCategory", AssessmentResults_RadTabStrip.SelectedTab.Text);
            parms.AddWithValue("TestType", (String)ViewState[_testTypeFilterKey]);
            parms.AddWithValue("Term", (String)ViewState[_termFilterKey]);
            parms.AddWithValue("Grade", (String)ViewState[_gradeFilterKey]);
            parms.AddWithValue("Subject", (String)ViewState[_subjectFilterKey]);

            bool hasSecurePermission = UserHasPermission(Permission.Access_SecureTesting)
                                       && UserHasPermission(Permission.Access_SecureTesting_viewAssessmentResults);
            parms.AddWithValue("HasSecurePermission", hasSecurePermission);

            DataTable dtAssessment = ThinkgateDataAccess.FetchDataTable(AppSettings.ConnectionString,
                                                      Thinkgate.Base.Classes.Data.StoredProcedures.E3_ASSESSMENT_RESULTS,
                                                      CommandType.StoredProcedure,
                                                      parms);

            rgAssessmentResults.DataSource = dtAssessment;
            rgAssessmentResults.DataBind();

        }

        /// <summary>
        /// Set the current filter visibility.
        /// </summary>
        protected void SetFilterVisibility()
        {
            // See top of file for visibility definitions.
            _subjectVisible = (_level == Base.Enums.EntityTypes.Teacher && String.Compare(_folder, "Reporting", true) == 0) ||
                                                                                    _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;
            _gradeVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;
            _termVisible = true;
            _testTypeVisible = true;
            _editItemMode = false;
        }

        protected void rgAssessmentResults_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (!(e.Item is GridDataItem)) return;
            Label description = (Label)e.Item.FindControl("lblDescription");
            HyperLink lnkProficient = (HyperLink)e.Item.FindControl("lnkProficient");
            string value = description.Text;

            if (value.Length > 32)
                description.Text = value.Substring(0, 28) + "...";

            if (_level == EntityTypes.Class)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                lnkProficient.ForeColor = Color.Blue;
                lnkProficient.Font.Underline = true;
                lnkProficient.Style["cursor"] = "pointer";
                var lastModifiedDate = row["LastUpdatedDate"] == null
                    ? ""
                    : "as of " + row["LastUpdatedDate"];
                var levelIDEncrypted = Cryptography.EncryptString(_levelID.ToString(), SessionObject.LoggedInUser.CipherKey);

                var steIDs = String.Join(
                    ",",
                    _class.GetStudentTestEventsForTest(Convert.ToInt32(row["TestID"])));
                lnkProficient.Attributes.Add("onclick",
                                       "ShowEditForm('" + AssessmentResults_RadTabStrip.SelectedTab.Text + "', '" +
                                       _level + "','" + _levelID + "','" +
                                       levelIDEncrypted + "','" +
                                       row["TestID"] + "','" + cmbTerm.SelectedItem.Text + "','" +
                                       row["Year"] +
                                       "','" + cmbTestType.SelectedItem.Text + "','" + _levelID + "','" +
                                       "null" + "','" +
                                       "null" + "', '<h1>" +
                                       row["TestName"] + "</h1><br/>" +
                                       lastModifiedDate  + "', 'tportalreport1test', null, '" + steIDs + "', '" + "" + "', '" + (row["TestID"].ToString().Length + 3) + _levelID + row["TestID"] + "', '" + CheckTestSchedule(row["TestID"].ToString()) + "', '" + "../" + "')");
            }
        }

        private string CheckTestSchedule(string testId)
        {
            var isContentLocked = "false";
            DataRow row = Thinkgate.Base.Classes.Assessment.GetAssessmentSchedule(Convert.ToInt32(testId));
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
                    string contentLock = row["CONTENT"].ToString();
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
            return isContentLocked;
        }

        protected void RadTabStrip2_TabClick(object sender, RadTabStripEventArgs e)
        {
            String category = AssessmentResults_RadTabStrip.SelectedTab.Text;
            Tile.TileParms.AddParm("category", category);
            BuildTestTypes();
            BuildAssessments();
        }

        protected void rgAssessmentResults_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DataTable dtAssessment = ThinkgateDataAccess.FetchDataTable("E3_AssessmentResults",
                                                                             new object[] { _userID, _level.ToString(), _levelID, AssessmentResults_RadTabStrip.SelectedTab.Text,
																			(String)ViewState[_testTypeFilterKey], (String)ViewState[_termFilterKey],
																			(String)ViewState[_gradeFilterKey], (String)ViewState[_subjectFilterKey] });
            rgAssessmentResults.DataSource = dtAssessment;
        }

    }
}
