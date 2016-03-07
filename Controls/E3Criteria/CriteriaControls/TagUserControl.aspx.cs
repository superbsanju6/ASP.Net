using System;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class TagUserControl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LRMITags.EnumSelectionType = Enums.SelectionType.Resource.ToString();
            //LRMITags.AssessedVisable = true;
            //LRMITags.TeachesVisable = false;
            //LRMITags.RequiresVisable = true;
 
            LRMITags.TagCriteriaSelectionParameters.EducationalSubject.Add("Mathematics");

            LRMITags.TagCriteriaSelectionParameters.GradeLevel.Add("02");

            LRMITags.TagCriteriaSelectionParameters.ReadingLevel = "Level One";
            LRMITags.TagCriteriaSelectionParameters.Creator = "Daffy Duck";
            LRMITags.TagCriteriaSelectionParameters.UseRightUrl = 3;
            LRMITags.TagCriteriaSelectionParameters.EducationalUse.Add(50);
            LRMITags.TagCriteriaSelectionParameters.EducationalUse.Add(53);
            LRMITags.TagCriteriaSelectionParameters.EndUser.Add(36);
            LRMITags.TagCriteriaSelectionParameters.InteractivityType.Add(11);
            LRMITags.TagCriteriaSelectionParameters.LearningResourceType.Add(18);
            LRMITags.TagCriteriaSelectionParameters.LearningResourceType.Add(23);
            LRMITags.TagCriteriaSelectionParameters.LearningResourceType.Add(31);
            LRMITags.TagCriteriaSelectionParameters.MediaType.Add(39);
            LRMITags.TagCriteriaSelectionParameters.MediaType.Add(49);
            LRMITags.TagCriteriaSelectionParameters.DateCreated = Convert.ToDateTime("2014-01-02");
            LRMITags.TagCriteriaSelectionParameters.TimeRequiredHours = 5;
            LRMITags.TagCriteriaSelectionParameters.Language = 2;

            LRMITags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(5262);
            LRMITags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(5777);
            LRMITags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(6127);

            LRMITags.TagCriteriaSelectionParameters.RequiresStandardIds.Add(9806);
            LRMITags.TagCriteriaSelectionParameters.RequiresStandardIds.Add(9808);

            LRMITags.MediaTypeDisabled = true;

            //LRMITags.TagCriteriaSelectionParameters.UseRightUrlTxt = "http://www.yahoo.com";

            LRMITags.SaveCancelButtonClick += new EventHandler(User_Clicked_Button);
        }
        protected void User_Clicked_Button(object sender, EventArgs e)
        {
            TagCriteriaSelectionParameters tcsp = LRMITags.TagCriteriaSelectionParameters;
            if (!TagCriteriaSelectionParameters.IsEmpty(tcsp))
            {
                int usageRights = (int) tcsp.UseRightUrl;
            }

        }
    }
}