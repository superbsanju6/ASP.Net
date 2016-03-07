
using System;
using System.Data;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class SuggestedResources : BasePage
    {

        #region Constants

        private const string STATUS_OK = "OK";
        private const string STATUS_NOSTANDARD = "NOSTANDARD";
        private const string STATUS_NORECORDSFOUND = "NORECORDSFOUND";
        private const string STATUS_PROFICIENCYMET = "PROFICIENCYMET";

        #endregion

        #region Fields

        private string Category { get; set; }
        public string Level { get; set; }
        private string LevelID { get; set; }
        private string AssessmentID { get; set; }
        private string Year { get; set; }
        private string Term { get; set; }
        private string ParentObjectName { get; set; }
        private string ParentObjectID { get; set; }
        private string TestEventID { get; set; }
        
        public string AssessmentName { get; set; }
        public string AssessmentHoverName { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public int StandardsCount { get; set; }
        public string CurrentStatus { get; set; }
        public string LevelColor { get; set; }        
        public string AssessmentTitleDate { get; set; }
        private DataTable suggestedResourcesDistinctByStudent = new DataTable();
        private DataTable suggestedResourcesDetails = new DataTable();

        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            ParseQueryStringValues();

            InitPage();
            LoadSuggestedResources();
            LoadHeader();
        }

        private void InitPage()
        {
            CurrentStatus = STATUS_NORECORDSFOUND;
            this.LevelNameLabel.Visible = false;
        }

        protected void rptStudents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView studentRow = (DataRowView)e.Item.DataItem;
                string studentId = studentRow.Row["StudentId"].ToString();

                suggestedResourcesDetails.DefaultView.RowFilter = string.Format("[StudentId] = '{0}'", studentId);
                DataTable standardsTable = suggestedResourcesDetails.DefaultView.ToTable();

                Repeater repeaterStandardDetails = (Repeater)e.Item.FindControl("rptStudentProficiency");
                repeaterStandardDetails.DataSource = standardsTable;
                repeaterStandardDetails.DataBind();
            }
        }

        protected void rptStudentProficiency_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView studentRow = (DataRowView)e.Item.DataItem;
                string standardId = studentRow.Row["StandardID"].ToString();
                string standardName = studentRow.Row["StandardName"].ToString();

                string url = "~/Record/StandardsPage.aspx?from=suggestedresources&xID=" + Cryptography.EncryptString(standardId, SessionObject.LoggedInUser.CipherKey);

                HyperLink hyperlink = (HyperLink)e.Item.FindControl("hypStandard");
                hyperlink.NavigateUrl = url;
                hyperlink.Text = standardName;
            }
        }

        #endregion

        #region Methods

        private void LoadHeader()
        {
            Base.Classes.Assessment assessment = Thinkgate.Base.Classes.Assessment.GetAssessmentByID(int.Parse(this.AssessmentID));
            AssessmentName = assessment.TestName;
            AssessmentHoverName = "(" + assessment.Description + " as of " + AssessmentTitleDate + ")";

            if (this.Level.Equals("Class", StringComparison.InvariantCultureIgnoreCase))
            {
                Base.Classes.Class classInfo = Thinkgate.Base.Classes.Class.GetClassByID(int.Parse(this.LevelID));
                LevelNameLabel.Text = string.Format(" Class (Period {0} {1})", classInfo.Period, classInfo.Semester);
                this.LevelNameLabel.Visible = true;
            }
            else if (this.Level.Equals("Student", StringComparison.InvariantCultureIgnoreCase))
            {
                LevelNameLabel.Text = string.Format(" Student ({0})", StudentName);
                this.LevelNameLabel.Visible = true;
            }

            divNoStandards.Visible = CurrentStatus.Equals(STATUS_NOSTANDARD, StringComparison.InvariantCultureIgnoreCase);
            divProficiencyMet.Visible = CurrentStatus.Equals(STATUS_PROFICIENCYMET, StringComparison.InvariantCultureIgnoreCase);

            rptStudents.Visible = !(divProficiencyMet.Visible || divNoStandards.Visible);
        }

        private void ParseQueryStringValues()
        {
            Category = GetQueryStringValue("category");
            Level = GetQueryStringValue("level");
            LevelID = GetDecryptedEntityId("levelID").ToString();
            AssessmentID = GetQueryStringValue("testID");
            Year = GetQueryStringValue("year");
            Term = GetQueryStringValue("term");
            ParentObjectName = GetQueryStringValue("parent");
            ParentObjectID = GetQueryStringValue("parentID");
            TestEventID = GetQueryStringValue("testEventID");
            AssessmentTitleDate = GetQueryStringValue("assessmentTitleDate");
        }

        private string GetQueryStringValue(string parameter)
        {
            if (Request.QueryString[parameter] != null)
            {
                return string.Format("{0}", Request.QueryString[parameter]);
            }
            return string.Empty;
        }

        private void LoadSuggestedResources()
        {
            DataSet suggestedResourceSet = Thinkgate.Base.Classes.Reports.GetSuggestedResources(int.Parse(this.AssessmentID), this.Level, int.Parse(this.LevelID));
            if (suggestedResourceSet != null && suggestedResourceSet.Tables.Count > 0)
            {
                if (suggestedResourceSet.Tables.Count > 1 && suggestedResourceSet.Tables[1].Rows.Count > 0)
                {
                    CurrentStatus = suggestedResourceSet.Tables[1].Rows[0]["Status"].ToString();
                    LevelColor = suggestedResourceSet.Tables[1].Rows[0]["LevelColor"].ToString();
                }
                suggestedResourcesDetails = suggestedResourceSet.Tables[0].Copy();
                suggestedResourcesDistinctByStudent = suggestedResourceSet.Tables[0].DefaultView.ToTable(true, "StudentId", "StudentName");

                if (suggestedResourceSet.Tables.Count > 1 && suggestedResourceSet.Tables[1].Rows.Count > 0)
                {
                    StudentId = suggestedResourceSet.Tables[1].Rows[0]["OBJECTID"].ToString();
                    StudentName = suggestedResourceSet.Tables[1].Rows[0]["OBJECTNAME"].ToString();
                }

                rptStudents.DataSource = suggestedResourcesDistinctByStudent;
                rptStudents.DataBind();
            }
        }

        #endregion
    }
}