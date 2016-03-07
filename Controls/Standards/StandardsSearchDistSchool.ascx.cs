using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Classes.Data;
using System.Collections.Generic;

namespace Thinkgate.Controls.Standards
{
    public partial class StandardsSearchDistSchool : TileControlBase
    {
        // Level will be one of District, Teacher, School, Class;
        protected Thinkgate.Base.Enums.EntityTypes _level;
        protected Int32 _userID;
        protected Int32 _levelID;
        // True if this is a postback.
        protected Boolean _isPostBack;

        // Which filter buttons are visible.
        protected Boolean _standardSetVisible, _gradeVisible, _subjectVisible;

        // View state keys.
        const String _standardSetFilterKey = "StandardSetFilter", _gradeFilterKey = "GradeFilter", _subjectFilterKey = "SubjectFilter";

        // We only want to show the first 100 standards.
        private const Int32 _maxStandards = 100;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null) return;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            // Simulate IsPostBack.
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder");

            _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = (Int32)Tile.TileParms.GetParm("levelID");
            _userID = SessionObject.LoggedInUser.Page;

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            if (!_isPostBack)
            {
                taskList.Add(new AsyncPageTask(BuildStandardSets));
            }
            taskList.Add(new AsyncPageTask(BuildStandards));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "StandardsSearch", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            // Create the initial viewstate values.
            if (ViewState[_standardSetFilterKey] == null)
            {
                ViewState.Add(_standardSetFilterKey, "All");
                ViewState.Add(_gradeFilterKey, "All");
                ViewState.Add(_subjectFilterKey, "All");
            }

            // Set the current filter visibility.
            SetFilterVisibility();

            if (!_isPostBack)
            {
                BuildStandardSets();
                BuildGrades();
                BuildSubjects();
                BuildStandards();
            }
        }

        protected void lbxStandards_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            DataRowView row = (DataRowView)(listBoxItem).DataItem;

            HyperLink lnk;
            Label lbl;
            if ((lnk = (HyperLink)listBoxItem.FindControl("lnkStandard")) != null && (lbl = (Label)listBoxItem.FindControl("lblStandard")) != null)
            {
                lnk.NavigateUrl = "javascript:void();";
                lnk.Attributes["onclick"] = string.Concat("customDialog({ url: '", ResolveUrl("~/Controls/Standards/StandardsSearch_ExpandedV2.aspx?standardSet=" + (String)ViewState[_standardSetFilterKey] + "&standardCourseID=" + row["ID_Encrypted"]),
                    "', maximize: true, maxwidth: 950, maxheight: 675}); return false;");
                lnk.ToolTip = row["Description"].ToString();

                if (UserHasPermission(Base.Enums.Permission.Hyperlink_StandardName))
                {
                    lnk.Visible = true;
                    lbl.Visible = false;
                }
                else
                {
                    lnk.Visible = false;
                    lbl.Visible = true;
                }
            }
        }

        protected void BuildStandardSets()
        {
            cmbStandardSet.Visible = _standardSetVisible;
            if (_standardSetVisible)
            {
                DataTable dtStandardSet = Thinkgate.Base.Classes.Standards.GetStandardSets(SessionObject.GlobalInputs, _userID);

                // The only existing column is 'Standard_Set'. We must add a column for 'CmbText'.
                dtStandardSet.Columns.Add("CmbText", typeof(String));
                foreach (DataRow row in dtStandardSet.Rows)
                    row["CmbText"] = row["Standard_Set"];
                DataRow newRow = dtStandardSet.NewRow();
                newRow["Standard_Set"] = "All";
                newRow["CmbText"] = "Standard Set";
                dtStandardSet.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbStandardSet.DataTextField = "CmbText";
                cmbStandardSet.DataValueField = "Standard_Set";
                cmbStandardSet.DataSource = dtStandardSet;
                cmbStandardSet.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbStandardSet.Items.FindItemByValue((String)ViewState[_standardSetFilterKey], true);
                Int32 selIdx = cmbStandardSet.Items.IndexOf(item);
                cmbStandardSet.SelectedIndex = selIdx;
            }
        }

        protected void cmbStandardSet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_standardSetFilterKey] = e.Value;

            SetFilterVisibility();
            BuildStandards();
        }

        protected void BuildGrades()
        {
            cmbGrade.Visible = _gradeVisible;
            if (_gradeVisible)
            {
                DataTable dtGrade = new DataTable();
                DataColumn gradeCol = dtGrade.Columns.Add("Grade", typeof(String));
                List<Grade> grades = new List<Grade>();

                // District.
                if (_level == Base.Enums.EntityTypes.District)
                    grades = (from c in CourseMasterList.StandCourseDict.Values select c.Grade).Distinct().ToList();
                // School.
                if (_level == Base.Enums.EntityTypes.School && CourseMasterList.GradesForSchoolsDict.ContainsKey(_levelID))
                    grades = CourseMasterList.GradesForSchoolsDict[_levelID];

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
                    row["CmbText"] = "Grade " + row["Grade"];
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
            ViewState[_gradeFilterKey] = e.Value;

            SetFilterVisibility();
            BuildStandards();
        }

        protected void BuildSubjects()
        {
            cmbSubject.Visible = _subjectVisible;
            if (_subjectVisible)
            {
                IEnumerable<String> subjects = null;

                // District.
                if (_level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.State)
                    subjects = (from c in CourseMasterList.StandCourseDict.Values
                                orderby c.Subject.DisplayText
                                select c.Subject.DisplayText).Distinct();
                // School.
                // Since we have no way of knowing exactly which subjects are taught at
                // a given school, we will populate with all subjects that are for the grades
                // taught at the school.
                if (_level == Base.Enums.EntityTypes.School && CourseMasterList.GradesForSchoolsDict.ContainsKey(_levelID))
                {
                    List<Grade> grades = CourseMasterList.GradesForSchoolsDict[_levelID];
                    // Now we must get a list of all subjects for these grades.
                    subjects = (from c in CourseMasterList.StandCourseDict.Values
                                where grades.Contains(c.Grade)
                                orderby c.Subject.DisplayText
                                select c.Subject.DisplayText).Distinct();
                }

                DataTable dtSubject = new DataTable();
                DataColumn subjectCol = dtSubject.Columns.Add("Subject", typeof(String));
                DataColumn abbrevCol = dtSubject.Columns.Add("Abbreviation", typeof(String));
                if (subjects != null)
                {
                    foreach (String subject in subjects)
                    {
                        DataRow row = dtSubject.NewRow();
                        row[subjectCol] = subject;
                        row[abbrevCol] = (CourseMasterList.AbbreviationForSubjectsDict.ContainsKey(subject) && !String.IsNullOrEmpty(CourseMasterList.AbbreviationForSubjectsDict[subject]))
                                                             ? CourseMasterList.AbbreviationForSubjectsDict[subject] : subject;
                        dtSubject.Rows.Add(row);
                    }
                }

                // The existing columns are 'Subject' and 'Abbreviation'.
                // Add a column for 'DropdownText'.
                dtSubject.Columns.Add("DropdownText", typeof(String));
                foreach (DataRow row in dtSubject.Rows)
                {
                    row["DropdownText"] = row["Subject"].ToString();
                    if (row["Subject"].ToString() != row["Abbreviation"].ToString())
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
        }

        protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_subjectFilterKey] = e.Value;

            SetFilterVisibility();
            BuildStandards();
        }


        protected void BuildStandards()
        {

            DataTable dtStandard = ThinkgateDataAccess.FetchDataTable("E3_GetStandardsListBy_StandardsSubjectAndSet",
                                                                                new object[] { _userID, (String)ViewState[_standardSetFilterKey], (String)ViewState[_gradeFilterKey],
																						(String)ViewState[_subjectFilterKey], (_level == Base.Enums.EntityTypes.School) ? _levelID : 0 }, SessionObject.GlobalInputs);

#if false
			// We only want to show the first 100 standards.
			while (dtStandard.Rows.Count > _maxStandards)
				dtStandard.Rows.RemoveAt(dtStandard.Rows.Count - 1);
#endif

            dtStandard = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtStandard, "Course", "Course_Encrypted");
            dtStandard = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtStandard, "ID", "ID_Encrypted");


            // We must sometimes truncate the standard description so that it can fit in the area provided.
            // When truncated we add ellipsis (...).
            DataColumn descCol = dtStandard.Columns.Add("Description");
            DataColumn shortenedDescCol = dtStandard.Columns.Add("ShortenedDescription");

            const Int32 descChars = 45;
            for (Int32 i = 0; i < dtStandard.Rows.Count; i++)
            {
                DataRow row = dtStandard.Rows[i];
                String desc = String.Format("{0} {1} - {2}", row["FriendlyGrade"].ToString(), row["Subject"].ToString(), row["Course"].ToString());
                row[descCol] = desc;

                if (desc.Length > descChars)
                    desc = desc.Substring(0, Math.Max(0, descChars - 3)) + "...";
                row[shortenedDescCol] = desc;
            }

#if false
			// Add an empty row at the end if we have the maximum number of standards.
			// This is used as a placeholder for the 'More Results...' line.
			if (dtStandard.Rows.Count >= _maxStandards)
			{
				DataRow newRow = dtStandard.NewRow();
				dtStandard.Rows.Add(newRow);
			}
#endif

            Boolean isEmpty = dtStandard.Rows.Count == 0;

            lbxStandards.DataSource = dtStandard;
            lbxStandards.DataBind();
            lbxStandards.Visible = !isEmpty;
            pnlNoResults.Visible = isEmpty;
        }

        /// <summary>
        /// Set the current filter visibility.
        /// </summary>
        protected void SetFilterVisibility()
        {
            // All visible for now.
            _standardSetVisible = true;
            _gradeVisible = true;
            _subjectVisible = true;
        }
    }
}