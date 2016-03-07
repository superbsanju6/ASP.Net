using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.LearningMedia;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class LrmiTagsSearch : System.Web.UI.UserControl
    {
        public SessionObject SessionObject;
        public string Guid { get; set; }
        public bool FirstTimeLoaded { get; set; }
        public string InitialButtonText { get; set; }
        public event EventHandler ReloadReport;
        public Type SearchType { get; set; }
        private char _commaSep = Convert.ToChar(",");
        public string SearchCategory { get; set; }

        public enum Type
        {
            Item,
            Assessement
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject) Session["SessionObject"];
            if (!IsPostBack)
            {
                SetControls();
            }
            else
            {
                HideExpandedDivs();
            }
        }

        private void HideExpandedDivs()
        {
            HeaderDivExpandGrades.Style["display"] = "none";
        }

        private void SetControls()
        {
            InitializeControls();
            LoadControls();
        }
        #region Load methods
        private void LoadControls()
        {
            LoadGrades();
            LoadSubjects();
            LoadLearningResources();
            LoadEducationalUses();
            LoadEndUsers();
            tbCreator.Text = "";
            tbPublisher.Text = "";
            LoadUsageRights();
            LoadMediaTypes();
            LoadLanguages();
            LoadAgeAppropriate();
            tbTimeRequiredDays.Text = "";
            LoadStandardSets();
            rlbAssessedGrade.Items.Add(new ListItem("Select Grade"));
            rlbAssessedSubject.Items.Add(new ListItem("Select Subject"));
            rlbAssessedCourse.Items.Add(new ListItem("Select Course"));
            rlbAssessedStandard.Items.Add(new ListItem("Select Standard"));
            tbTextComplexity.Text = "";
            tbReadingLevel.Text = "";
            LoadInteractivityTypes();
        }

        private void InitializeControls()
        {
            rlbGrades.OnClientItemChecked = "onItemCheckedLRMI";
            rlbSubjects.OnClientItemChecked = "onItemCheckedLRMI";
            rlbLearningResources.OnClientItemChecked = "onItemCheckedLRMI";
            rlbEducationalUse.OnClientItemChecked = "onItemCheckedLRMI";
            rlbEndUser.OnClientItemChecked = "onItemCheckedLRMI";
            rlbUsageRights.OnClientItemChecked = "onItemCheckedLRMI";
            rlbMediaType.OnClientItemChecked = "onItemCheckedLRMI";
            rlbLanguage.OnClientItemChecked = "onItemCheckedLRMI";
            rlbAgeAppropriate.OnClientItemChecked = "onItemCheckedLRMI";
            rlbInteractivity.OnClientItemChecked = "onItemCheckedLRMI";
            SetTimeRequiredOptions();
            rlbAssessedStandardSet.Attributes.Add("onchange", "PopulateGrade(rlbAssessedStandardSet)");
            rlbAssessedGrade.Attributes.Add("onchange", "PopulateSubject(rlbAssessedGrade)");
            rlbAssessedSubject.Attributes.Add("onchange", "PopulateCourse(rlbAssessedSubject)");
            rlbAssessedCourse.Attributes.Add("onchange", "PopulateStandard(rlbAssessedCourse)");
            rlbAssessedStandard.Attributes.Add("onchange", "selectStandardsCriteriaValue(rlbAssessedStandard)");
            hdnGrades.Value = string.Empty;
            hdnSubjects.Value = string.Empty;
            hdnLearningResources.Value = string.Empty;
            hdnLanguages.Value = string.Empty;
            hdnEducationalUses.Value = string.Empty;
            hdnEndUsers.Value = string.Empty;
            hdnCreator.Value = string.Empty;
            hdnPublisher.Value = string.Empty;
            hdnUsageRights.Value = string.Empty;
            hdnMediaTypes.Value = string.Empty;
            hdnLanguages.Value = string.Empty;
            hdnAgeAppropriate.Value = string.Empty;
            hdnTimeRequired.Value = "00:00:00";
            hdnAssessedStandardSet.Value = string.Empty;
            hdnAssessedGrade.Value = string.Empty;
            hdnAssessedSubject.Value = string.Empty;
            hdnAssessedCourse.Value = string.Empty;
            hdnAssessed.Value = string.Empty;
            hdnTextComplexity.Value = string.Empty;
            hdnReadingLevel.Value = string.Empty;
            hdnInteractivity.Value = string.Empty;
        }

        private void SetTimeRequiredOptions()
        {
            for (int i = 0; i < 24; i++)
            {
                rdlTimeRequiredHours.Items.Add(new ListItem(i.ToString(CultureInfo.InvariantCulture)));
            }
            for (int j = 0; j < 60; j+=5)
            {
                rdlTimeRequiredMinutes.Items.Add(new ListItem(j.ToString(CultureInfo.InvariantCulture)));
            } 
        }

        private void LoadGrades()
        {
            var gradesDataSource = new DataTable();
            var gradesByCurrCourses =
                CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetGradeList();
            gradesDataSource.Columns.Add("Key");
            gradesDataSource.Columns.Add("Value");
            foreach (var g in gradesByCurrCourses)
            {
                var li = new RadListBoxItem(g.DisplayText, g.DisplayText);
                rlbGrades.Items.Add(li);
            }
        }

        private void LoadSubjects()
        {
            var subjectsByCurrCourses = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetSubjectList();
            foreach (var subjectText in subjectsByCurrCourses.Select(s => s.DisplayText).Distinct())
            {
                var li = new RadListBoxItem(subjectText, subjectText);
                rlbSubjects.Items.Add(li);
            }
        }

        private void LoadLearningResources()
        {
            DataTable dtLearningResources = new DataTable();
            if (SearchType == Type.Assessement)
            {
                dtLearningResources =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.LearningResourceType, Enums.SelectionType.Assessment.ToString());
            }

            foreach (DataRow lrRow in dtLearningResources.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString(), lrRow[2].ToString());
                rlbLearningResources.Items.Add(li);
            }
        }

        private void LoadEducationalUses()
        {
            DataTable dtEducationalUse =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.EducationalUse, Enums.SelectionType.Assessment.ToString());

            foreach (DataRow lrRow in dtEducationalUse.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString(), lrRow[0].ToString());
                rlbEducationalUse.Items.Add(li);
            }
        }

        private void LoadEndUsers()
        {
            DataTable dtEndUsers =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.EndUser, Enums.SelectionType.Assessment.ToString());

            foreach (DataRow lrRow in dtEndUsers.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString(), lrRow[0].ToString());
                rlbEndUser.Items.Add(li);
            }
        }

        private void LoadUsageRights()
        {
            DataTable dtCreativeCommons = Base.Classes.Assessment.LoadCreativeCommonsForAssessmentLrmiSearch();

            foreach (DataRow lrRow in dtCreativeCommons.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString(), lrRow[0].ToString());
                rlbUsageRights.Items.Add(li);
            }
        }

        private void LoadMediaTypes()
        {
            DataTable dtMediaTypes =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.MediaType, Enums.SelectionType.Assessment.ToString());


            foreach (DataRow lrRow in dtMediaTypes.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString().Replace("/", "__"), lrRow[0].ToString());
                rlbMediaType.Items.Add(li);
            }
        }

        private void LoadLanguages()
        {
            DataTable dtLanguages = GetLanguages();

            foreach (DataRow lrRow in dtLanguages.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString(), lrRow[0].ToString());
                rlbLanguage.Items.Add(li);
            }
        }

        private void LoadAgeAppropriate()
        {
            DataTable dtAgeAppropriate =
                    Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.AgeAppropriate, Enums.SelectionType.Assessment.ToString());


            foreach (DataRow lrRow in dtAgeAppropriate.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString(), lrRow[2].ToString());
                rlbAgeAppropriate.Items.Add(li);
            }
        }

        private void LoadStandardSets()
        {
            var tempStandardSet = Thinkgate.Classes.Search.CriteriaHelper.GetStandardSetList();
            var liSelect = new ListItem("Select Standard Set", "0");
            rlbAssessedStandardSet.Items.Add(liSelect);

            foreach (var tempSs in tempStandardSet)
            {
                var li = new ListItem(tempSs.Value, tempSs.Key);
                rlbAssessedStandardSet.Items.Add(li);
            }    
        }

        private void LoadInteractivityTypes()
        {
            DataTable dtActivity = Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.Activity, Enums.SelectionType.Assessment.ToString());

            foreach (DataRow lrRow in dtActivity.Rows)
            {
                var li = new RadListBoxItem(lrRow[1].ToString(), lrRow[0].ToString());
                rlbInteractivity.Items.Add(li);
            }
        }

        private static DataTable GetLanguages()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Enum");
            dt.Columns.Add("Description");
            var languageType = Enum.GetValues(typeof(LanguageType))
                .Cast<LanguageType>()
                .Select(v => v.ToString())
                .ToList();

            int i = 0;
            foreach (var lt in languageType)
            {
                DataRow dr = dt.NewRow();
                dr["Enum"] = i;
                dr["Description"] = lt;
                dt.Rows.Add(dr);
                i++;
            }

            return dt;
        }

        #endregion

        protected void RadButtonClear_Click(object sender, EventArgs e)
        {
            foreach (RadListBoxItem item in rlbGrades.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbSubjects.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbEducationalUse.Items)
            {
                item.Selected = false;
            } 
            foreach (RadListBoxItem item in rlbEndUser.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbLearningResources.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbMediaType.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbUsageRights.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbLanguage.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbAgeAppropriate.Items)
            {
                item.Selected = false;
            }
            foreach (RadListBoxItem item in rlbInteractivity.Items)
            {
                item.Selected = false;
            }
            tbCreator.Text = string.Empty;
            tbPublisher.Text = string.Empty;
            tbReadingLevel.Text = string.Empty;
            tbTextComplexity.Text = string.Empty;
            tbTimeRequiredDays.Text = string.Empty;
            rdlTimeRequiredHours.SelectedValue = "0";
            rdlTimeRequiredMinutes.SelectedValue = "0";
            hdnCreator.Value = string.Empty;
            hdnPublisher.Value = string.Empty;
            hdnTimeRequired.Value = "00:00:00";
            hdnAgeAppropriate.Value = string.Empty;
            hdnAssessed.Value = string.Empty;
            hdnAssessedCourse.Value = string.Empty;
            hdnAssessedGrade.Value = string.Empty;
            hdnAssessedStandardSet.Value = string.Empty;
            hdnAssessedSubject.Value = string.Empty;
            hdnGrades.Value = string.Empty;
            hdnSubjects.Value = string.Empty;
            hdnEducationalUses.Value = string.Empty;
            hdnEndUsers.Value = string.Empty;
            hdnInteractivity.Value = string.Empty;
            hdnLanguages.Value = string.Empty;
            hdnLearningResources.Value = string.Empty;
            hdnMediaTypes.Value = string.Empty;
            hdnReadingLevel.Value = string.Empty;
            hdnTextComplexity.Value = string.Empty;
            hdnUsageRights.Value = string.Empty;
        }

        private string GetListOfStandards()
        {
            string stringListOfStandards = string.Empty;

            if (!string.IsNullOrEmpty(hdnAssessedStandardSet.Value))
            {
                var standardSetList = new drGeneric_String();
                if (!String.IsNullOrEmpty(hdnAssessedStandardSet.Value))
                {
                    standardSetList.Add(hdnAssessedStandardSet.Value.Trim());
                }


                var gradeList = new drGeneric_String();
                if (!String.IsNullOrEmpty(hdnAssessedGrade.Value))
                {
                    gradeList.Add(hdnAssessedGrade.Value.Trim());
                }

                var subjectList = new drGeneric_String();
                if (!String.IsNullOrEmpty(hdnAssessedSubject.Value))
                {
                    subjectList.Add(hdnAssessedSubject.Value.Trim());
                }

                var courseList = new drGeneric_String();
                if (!String.IsNullOrEmpty(hdnAssessedCourse.Value))
                {
                    courseList.Add(hdnAssessedCourse.Value.Trim());
                }
                DataTable filteredStandards = Base.Classes.Standards.SearchStandards(false,
                    "", //TextSearch
                    "", //TextSearchVal
                    null, //itemBanks
                    null, //StandardCourses
                    null,
                    standardSetList.Count > 0 ? standardSetList : null,
                    gradeList.Count > 0 ? gradeList : null,
                    subjectList.Count > 0 ? subjectList : null,
                    courseList.Count > 0 ? courseList : null);
                List<object> listOfStandards = (from lststd in filteredStandards.DataSet.Tables[0].AsEnumerable()
                                                select lststd["StandardID"]).ToList();
                stringListOfStandards = string.Join(",", listOfStandards);
                //if (stringListOfStandards == "") stringListOfStandards = "0";
            }
            return stringListOfStandards;
        }

        public DataTable DoAssessmentSearchLrmi(Guid userGuid, int userPage)
        {
            string selectedStandardIds = hdnAssessed.Value;
            if (selectedStandardIds == string.Empty)
            {
                selectedStandardIds = GetListOfStandards();
            }
            DataTable results = Base.Classes.Assessment.SearchAssessmentsByLrmiTags(
            userGuid,
            userPage,
            hdnGrades.Value,//Grades
            hdnSubjects.Value,//Subjects
            hdnLearningResources.Value,//Learing Resource Types
            hdnEducationalUses.Value,//Educational Uses
            hdnEndUsers.Value, //Target Audience
            hdnCreator.Value,//Creator
            hdnPublisher.Value,//Publisher
            hdnUsageRights.Value, //Usage Rights URL
            hdnMediaTypes.Value,//Media Types
            hdnLanguages.Value,//Languages
            hdnAgeAppropriate.Value,//Age Appropriate
            hdnTimeRequired.Value,//Duration
            selectedStandardIds,//Assessed
            hdnTextComplexity.Value,//TextComplexity
            hdnReadingLevel.Value,//Reading Level
            hdnInteractivity.Value,//Activity
            SearchCategory,
            SessionObject.LoggedInUser.HasPermission(Thinkgate.Base.Enums.Permission.User_Cross_Schools) ? "Yes" : "No"
            );

            //SetControls();

            return results;
        }

        protected void RadButtonSearch_Click(object sender, EventArgs e)
        {
            ReloadReport(null, new EventArgs());
        }
    }
}