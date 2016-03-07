using System;
using Thinkgate.Classes;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class CreateAssessmentOptions : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject sessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];

            string level = !string.IsNullOrEmpty(Request.QueryString["level"])
                               ? Request.QueryString["level"]
                               : (sessionObject.AssessmentBuildParms.ContainsKey("level")
                                      ? sessionObject.AssessmentBuildParms["level"]
                                      : string.Empty);
            string testCategory = !string.IsNullOrEmpty(Request.QueryString["testCategory"])
                                      ? Request.QueryString["testCategory"]
                                      : (sessionObject.AssessmentBuildParms.ContainsKey("TestCategory")
                                             ? sessionObject.AssessmentBuildParms["TestCategory"]
                                             : string.Empty);
            sessionObject.AssessmentBuildParms.Clear();
            sessionObject.AssessmentBuildParms.Add("level", level);
            sessionObject.AssessmentBuildParms.Add("TestCategory", testCategory);

            StandardRigorLevels rigorLevels = sessionObject.Standards_RigorLevels_ItemCounts;
            rigorLevels.StandardRigorLevel.Clear();
            rigorLevels.ClearAllStandardItemTotals();
            rigorLevels.ClearAllStandardItemNames();
            rigorLevels.ClearAllBlankItemCounts();
            rigorLevels.ClearAllStandardRigorLevel();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllBlankItemCounts();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardItemNames();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardItemTotals();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardAddendumLevel();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllAddendumCounts();
            sessionObject.ItemBanks.Clear();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
