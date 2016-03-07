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

namespace Thinkgate.Controls.Assessment
{
    public partial class ViewBlueprints : TileControlBase
    {

        protected Thinkgate.Base.Enums.EntityTypes _level;
        // Category will be one of State, District, Classroom.
        protected String _category;
        protected Int32 _userID;
        protected Int32 _levelID;
        protected Int32 _schoolID;
        // True if this is a postback.
        protected Boolean _isPostBack;

        // Which filter buttons are visible.
        protected Boolean _gradeVisible, _subjectVisible, _yearVisible, _statusVisible;

        // View state keys.
        protected String _currentViewIdxKey = "CurrentViewIdx", _gradeFilterKey = "GradeFilter", _subjectFilterKey = "SubjectFilter", _yearFilterKey = "YearFilter";
        protected String _statusFilterKey = "StatusFilter";

        // We only want to show the first 100 assessments.
        private const Int32 _maxBlueprints = 100;

        private DataTable dtGrade;
        private DataTable dtSubject;
        private DataTable dtYears;
        private DataTable dtBlueprint;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null) return;

            _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            AttatchLevelToKeys();
            _category = (String)Tile.TileParms.GetParm("category");
            _levelID = (Int32)Tile.TileParms.GetParm("levelID");

        }
        #region Asynchronous Tasks
        private void LoadGrades()
        {
            dtGrade = Thinkgate.Base.Classes.Assessment.LoadMockGrades(_userID, SessionObject.GlobalInputs);
        }

        private void LoadSubjects()
        {
            dtSubject = Thinkgate.Base.Classes.Assessment.LoadMockSubjects(_userID, SessionObject.GlobalInputs);
        }

        private void LoadYears()
        {
            dtYears = Thinkgate.Base.Classes.Assessment.GetYears();

        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _userID = SessionObject.LoggedInUser.Page;

            // Simulate IsPostBack.
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);

            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            // Create the initial viewstate values.
            if (ViewState[_currentViewIdxKey] == null)
            {
                ViewState.Add(_currentViewIdxKey, 0);
                ViewState.Add(_gradeFilterKey, "All");
                ViewState.Add(_subjectFilterKey, "All");
                ViewState.Add(_yearFilterKey, "All");
                ViewState.Add(_statusFilterKey, (_category == "District" && !UserHasPermission(Permission.Edit_Blueprint)) ? "Proofed" : "All");
            }

            // Set the current filter visibility.
            SetFilterVisibility();

            // Set the current view.
            SetView();

            LoadGrades();
            LoadSubjects();
            LoadYears();
            if (!_isPostBack)
            {
                BuildGrades();
                BuildSubjects();
                BuildYears();
                BuildStatuses();
            }

            if (!(_level == Base.Enums.EntityTypes.District))
            {
                LoadBlueprints();
            }
            else
            {
                lbxGraphic.Visible = false;
                pnlGraphicNoResults.Visible = false;
                pnlFilterSelection.Visible = true;
            }
           
        }

        private void LoadBlueprints()
        {

            String gradeFilter = (String)ViewState[_gradeFilterKey];
            String subjectFilter = (String)ViewState[_subjectFilterKey];
            String yearFilter = (String)ViewState[_yearFilterKey];
            String statusFilter =( String)ViewState[_statusFilterKey];

            Thinkgate.Base.Classes.CourseList courses = Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser); //.CurrCourseList;

            if (_level == Base.Enums.EntityTypes.District)
                 dtBlueprint = Thinkgate.Base.Classes.Assessment.LoadBlueprints(yearFilter, gradeFilter, subjectFilter, statusFilter,
                                                                                                                     courses.ToSql());
            else if (_level == Base.Enums.EntityTypes.School)
                dtBlueprint = Thinkgate.Base.Classes.Assessment.LoadBlueprints("All", gradeFilter, subjectFilter, "Proofed",
                                                                                                                    courses.ToSql());
            else if(_level == Base.Enums.EntityTypes.Teacher)
                dtBlueprint = Thinkgate.Base.Classes.Assessment.LoadBlueprints("All", "All", "All", "Proofed",
                                                                                                                    courses.ToSql());
            // School and Teacher can see only proofed blueprints
             Boolean isEmpty = dtBlueprint.Rows.Count == 0;
             lbxGraphic.DataSource = dtBlueprint;
             lbxGraphic.DataBind();
             lbxGraphic.Visible = !isEmpty;
             pnlGraphicNoResults.Visible = isEmpty;
             pnlFilterSelection.Visible = false;
             
        }
        
        protected void BuildGrades()
        {
            if (_gradeVisible)
            {
                
                CourseList courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
                DataTable dtGrade = new DataTable();

                dtGrade.Columns.Add("Grade", typeof(String));
                dtGrade.Columns.Add("CmbText", typeof(String));

                if (courseList != null)
                {
                    foreach (var grade in courseList.GetGradeList())
                    {
                        DataRow newGradeRow = dtGrade.NewRow();
                        newGradeRow["Grade"] = grade.DisplayText;
                        newGradeRow["CmbText"] = grade.DisplayText;
                        dtGrade.Rows.Add(newGradeRow);
                    }
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

                // Initialize the current selection.
                RadComboBoxItem item = cmbGrade.Items.FindItemByValue((String)ViewState[_gradeFilterKey], true);
                Int32 selIdx = cmbGrade.Items.IndexOf(item);
                cmbGrade.SelectedIndex = selIdx;
            }
            else
                cmbGrade.Visible = false;
        }

        protected void BuildSubjects()
        {
            if (_subjectVisible)
            {
                
                // Now load the filter button tables and databind.
                // The existing columns are 'Subject' and 'Abbreviation'.
                // Add a column for 'DropdownText'.
                dtSubject.Columns.Add("DropdownText");
                foreach (DataRow row in dtSubject.Rows)
                {
                    row["DropdownText"] = (String)row["Subject"];
                    if ((String)row["Subject"] != (String)row["Abbreviation"])
                        row["DropdownText"] += " (" + (String)row["Abbreviation"] + ")";
                }

                // We will rename Abbreviation to CmbText.
                dtSubject.Columns["Abbreviation"].ColumnName = "CmbText";

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

                // Initialize the current selection.
                RadComboBoxItem item = cmbSubject.Items.FindItemByValue((String)ViewState[_subjectFilterKey], true);
                Int32 selIdx = cmbSubject.Items.IndexOf(item);
                cmbSubject.SelectedIndex = selIdx;
            }
            else
                cmbSubject.Visible = false;
        }

        protected void BuildStatuses()
        {
            if (_statusVisible)
            {
               
                DataTable dtStatus = new DataTable();
                dtStatus.Columns.Add("Status", typeof(String));
                dtStatus.Columns.Add("CmbText", typeof(String));
                dtStatus.Columns.Add("DropdownText", typeof(String));
                DataRow newRow = dtStatus.NewRow();
                newRow["Status"] = "All";
                newRow["CmbText"] = "Status";
                newRow["DropdownText"] = "All";
                dtStatus.Rows.Add(newRow);
                newRow = dtStatus.NewRow();
                newRow["Status"] = "Proofed";
                newRow["CmbText"] = "Proofed";
                newRow["DropdownText"] = "Proofed";
                dtStatus.Rows.Add(newRow);
                newRow = dtStatus.NewRow();
                newRow["Status"] = "Unproofed";
                newRow["CmbText"] = "Unproofed";
                newRow["DropdownText"] = "Unproofed";
                dtStatus.Rows.Add(newRow);

                // Data bind the combo box.
                cmbStatus.DataTextField = "CmbText";
                cmbStatus.DataValueField = "Status";
                cmbStatus.DataSource = dtStatus;
                cmbStatus.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbStatus.Items.FindItemByValue((String)ViewState[_statusFilterKey], true);
                Int32 selIdx = cmbStatus.Items.IndexOf(item);
                cmbStatus.SelectedIndex = selIdx;
            }
            else
                cmbStatus.Visible = false;
        }

        protected void BuildYears()
        {
            if (_yearVisible)
            {
                cmbYear.Items.Clear();
                bool bFlag = false;

               // dtYears = Thinkgate.Base.Classes.Assessment.GetYears();

                DataColumn ddCol = dtYears.Columns.Contains("DropdownText") ? dtYears.Columns[1] : dtYears.Columns.Add("DropdownText", typeof(String));
              
                foreach (DataRow row in dtYears.Rows)
                    row[ddCol] = row["Year"];

                foreach (DataRow rowItem in dtYears.Rows)
                {
                    if (rowItem["Year"].ToString() == "Year")
                        bFlag = true;

                }
                if (!bFlag)
                {
                    DataRow newRow = dtYears.NewRow();
                    newRow["Year"] = "Year";
                    newRow["DropdownText"] = "All";
                    dtYears.Rows.InsertAt(newRow, 0);
                }

                // Data bind the combo box.
                cmbYear.DataTextField = "Year";
                cmbYear.DataValueField = "DropdownText";
                cmbYear.DataSource = dtYears;
                cmbYear.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbYear.Items.FindItemByValue((String)ViewState[_yearFilterKey], true);
                Int32 selIdx = cmbYear.Items.IndexOf(item);
                cmbYear.SelectedIndex = selIdx;
            }
            else
                cmbYear.Visible = false;
        }

        private void AttatchLevelToKeys()
        {
            _currentViewIdxKey += Tile.Title.Replace(" ", string.Empty);
            _gradeFilterKey += Tile.Title.Replace(" ", string.Empty);
            _subjectFilterKey += Tile.Title.Replace(" ", string.Empty);
            _yearFilterKey += Tile.Title.Replace(" ", string.Empty);
            _statusFilterKey += Tile.Title.Replace(" ", string.Empty);
        }

         /// <summary>
        /// Set the current filter visibility.
        /// </summary>
        protected void SetFilterVisibility()
        {
            // Subject and grade filters are visible for these conditions:
            //	District view.
            //	School view.
            _subjectVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School ;
            // Grade is visible only on district and school portal.
            _gradeVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;

            // Year filter is visible only on district portal
            _yearVisible = _level == Base.Enums.EntityTypes.District;

            // Status filter is visible only when Edit_Blueprint permission is set up
            _statusVisible = UserHasPermission(Permission.Edit_Blueprint);

            if (_subjectVisible == false && _gradeVisible == false && _yearVisible == false && _statusVisible == false)
            {
                btnOk.Visible = false;
            }
        }

        /// <summary>
        /// Set the current view to graphical.
        /// </summary>
        protected void SetView()
        {
            divGraphicalView.Style["display"] = "";
        }

        protected void lbxList_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            DataRowView row = (DataRowView)(listBoxItem).DataItem;

            Image imgEdit = (Image)e.Item.FindControl("testImg");

            if (!UserHasPermission(Permission.Edit_Blueprint))
            {
                HyperLink ctlBlueprint = (HyperLink)e.Item.FindControl("lnkGraphicBlueprint");
                ctlBlueprint.Visible = false;

                imgEdit.Visible = false;
            }

            String PGId;
            PGId = ((HiddenField)e.Item.FindControl("inpGraphicAssessmentID")).Value;
            
            String url = String.Empty;
            HyperLink blueprint = (HyperLink)e.Item.FindControl("lnkGraphicBlueprint");
           // blueprint.NavigateUrl = "../../SessionBridge.aspx?ReturnURL=" + System.Web.HttpUtility.UrlEncode("fastsql_v2_direct.asp?ID=7188|ExecutePacingGuideBlueprintViewSeq&??xID=") + PGId + System.Web.HttpUtility.UrlEncode("&popup=y&target=_blank");
            blueprint.NavigateUrl = "../../SessionBridge.aspx?ReturnURL=" + System.Web.HttpUtility.UrlEncode("display.asp?key=7271&fo=basic display&rm=page&xID=") + PGId + System.Web.HttpUtility.UrlEncode("&BP=yes");
            
            HyperLink guide = (HyperLink)e.Item.FindControl("lnkGraphicGuide");
            guide.NavigateUrl = "../../SessionBridge.aspx?ReturnURL=" + System.Web.HttpUtility.UrlEncode("fastsql_v2_direct.asp?ID=7188|ExecutePacingGuideBlueprintViewSeq&??xID=") + PGId + System.Web.HttpUtility.UrlEncode("&popup=y&target=_blank");

            HyperLink summary = (HyperLink)e.Item.FindControl("lnkGraphicSummary");
            summary.NavigateUrl = "../../SessionBridge.aspx?ReturnURL=" + System.Web.HttpUtility.UrlEncode("fastsql_v2_direct.asp?ID=7188|ExecuteAssessmentSummary&??xID=") + PGId + System.Web.HttpUtility.UrlEncode("&popup=y&target=_blank");

            // Edit mode controls how the row is displayed. If null, it shows 'More Results...'.
            if (row["Status"].ToString() == "Proofed")
                imgEdit.ImageUrl = "~/Images/proofed.png";
            else
                imgEdit.ImageUrl  ="~/Images/editable.png";
        }

        protected void cmbStatus_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_statusFilterKey] = e.Value;

            cmbStatus.Text = e.Text;
            RadComboBoxItem item = cmbStatus.Items.FindItemByText(e.Text);
            Int32 selIdx = cmbStatus.Items.IndexOf(item);
            cmbStatus.SelectedIndex = selIdx;
        }

        protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_subjectFilterKey] = e.Value;
            cmbSubject.Text = e.Text;
        }

        protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_gradeFilterKey] = e.Value;
            cmbGrade.Text = e.Text;
        }

        protected void cmbYear_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_yearFilterKey] = e.Value;
            cmbYear.Text = e.Text;
        }

        protected void btnOk_OnClick(object o, EventArgs e)
        {
            SetFilterVisibility();
            LoadBlueprints();
        }
    }
}