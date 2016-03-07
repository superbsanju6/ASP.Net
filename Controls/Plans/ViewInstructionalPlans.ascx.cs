using System;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Plans
{
    public partial class ViewInstructionalPlans : TileControlBase
    {
        protected Thinkgate.Base.Enums.EntityTypes Level; // Level will be one of Teacher, Class;
        protected Int32 LevelID;
        protected Boolean _isPostBack;
        protected Boolean GradeVisible;
        protected Boolean SubjectVisible;
        protected Boolean CourseVisible;
        protected String CurrentViewIdxKey = "CurrentViewIdx";
        protected String GradeFilterKey = "GradeFilter";
        protected String SubjectFilterKey = "SubjectFilter";
        protected String CourseFilterKey = "CourseFilter";
        
        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null) return;

            Level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            LevelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            AttatchLevelToKeys();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            // Simulate IsPostBack.
            var postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            // Create the initial viewstate values.
            if (ViewState[CurrentViewIdxKey] == null)
            {
                ViewState.Add(CurrentViewIdxKey, 0);
                ViewState.Add(GradeFilterKey, "All");
                ViewState.Add(SubjectFilterKey, "All");
                ViewState.Add(CourseFilterKey, "All");
            }

            SetFilterVisibility();

            if (!_isPostBack)
            {
                BuildGrades();
                BuildSubjects();
                BuildCourses();
            }

            BuildPlans();
        }

        private void AttatchLevelToKeys()
        {
            CurrentViewIdxKey += Tile.Title.Replace(" ", string.Empty);
            GradeFilterKey += Tile.Title.Replace(" ", string.Empty);
            SubjectFilterKey += Tile.Title.Replace(" ", string.Empty);
            CourseFilterKey += Tile.Title.Replace(" ", string.Empty);
        }

        protected void SetFilterVisibility()
        {
            cmbGrade.Visible = Level == Base.Enums.EntityTypes.District || Level == Base.Enums.EntityTypes.School;
            cmbSubject.Visible = Level == Base.Enums.EntityTypes.District || Level == Base.Enums.EntityTypes.School;
            cmbCourse.Visible = Level == Base.Enums.EntityTypes.District || Level == Base.Enums.EntityTypes.School;
        }

        protected void BuildPlans()
        {
            DataTable dtPlans = Base.Classes.InstructionalPlan.LoadPlans(Level, LevelID, 
                                                (String)ViewState[GradeFilterKey], (String)ViewState[SubjectFilterKey], (String)ViewState[CourseFilterKey]);

            dtPlans = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtPlans, "ID", "ID_Encrypted");

            Boolean isEmpty = dtPlans.Rows.Count == 0;

            lbxGraphic.DataSource = dtPlans;
            lbxGraphic.DataBind();
            lbxGraphic.Visible = !isEmpty;
            pnlGraphicNoResults.Visible = isEmpty;
        }

        protected void lbxGraphic_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            var listBoxItem = e.Item;
            var row = (DataRowView)(listBoxItem).DataItem;
            HyperLink lnk;

            String testLinkUrl = "~/Record/InstructionalPlan.aspx?xID=" + (String)row["ID_Encrypted"];

            if ((lnk = (HyperLink)listBoxItem.FindControl("lnkGraphicTestName")) != null)
            {
                lnk.NavigateUrl = testLinkUrl;
            }

            var graphicLine2Summary = (Panel)listBoxItem.FindControl("graphicLine2Summary");
            var numberTerms = DataIntegrity.ConvertToInt(row["NumTerms"]);

            for (var i = 1; i <= numberTerms; i++)
            {
                var termLnk = new HyperLink
                                  {
                                      NavigateUrl = testLinkUrl + "&folder=Term " + i,
                                      Target = "_blank",
                                      Text = string.Format("Term {0}", i),
                                      CssClass = "termLink"
                                  };
                graphicLine2Summary.Controls.Add(termLnk);
            }
        }

        #region Build Filters and Filter Change Methods

        protected void BuildGrades()
        {
            if (GradeVisible)
            {
                // The only existing column is 'Grade'.
                DataTable dtGrade = Base.Classes.Assessment.LoadMockGrades(LevelID);
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
                RadComboBoxItem item = cmbGrade.Items.FindItemByValue((String)ViewState[GradeFilterKey], true);
                Int32 selIdx = cmbGrade.Items.IndexOf(item);
                cmbGrade.SelectedIndex = selIdx;
            }
        }

        protected void BuildSubjects()
        {
            if (SubjectVisible)
            {
                // Now load the filter button tables and databind.
                var dtSubject = Base.Classes.Assessment.LoadMockSubjects(LevelID);
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
                RadComboBoxItem item = cmbSubject.Items.FindItemByValue((String)ViewState[SubjectFilterKey], true);
                Int32 selIdx = cmbSubject.Items.IndexOf(item);
                cmbSubject.SelectedIndex = selIdx;
            }
        }
        
        protected void BuildCourses()
        {
            if (CourseVisible)
            {                
                var gradeFilter = (String)ViewState[GradeFilterKey];
                var subjectFilter = (String)ViewState[SubjectFilterKey];
                var courses = Base.Classes.CourseMasterList.CurrCourseList;
                courses = courses.FilterByGradeAndSubject((gradeFilter == "All") ? null : gradeFilter, (subjectFilter == "All") ? null : subjectFilter);

                // Data bind the combo box.
                cmbCourse.DataTextField = "CourseName";
                cmbCourse.DataValueField = "ID";
                cmbCourse.DataSource = courses;
                cmbCourse.DataBind();

                // Initialize the current selection.                
                cmbCourse.SelectedValue = ViewState[CourseFilterKey].ToString();
            }
        }

        protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[GradeFilterKey] = e.Value;
            SetFilterVisibility();
            BuildPlans();
            cmbGrade.Text = e.Text;
        }

        protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[SubjectFilterKey] = e.Value;
            SetFilterVisibility();
            BuildPlans();
            cmbSubject.Text = e.Text;
        }

        protected void cmbCourse_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[CourseFilterKey] = e.Value;
            SetFilterVisibility();
            BuildPlans();            
            cmbCourse.Text = e.Text;
        }

        #endregion
    }
}
