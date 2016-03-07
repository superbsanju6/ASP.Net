using System;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.ControlHost
{
    public partial class PreviewRubrics : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }

            int xID = GetDecryptedEntityId(X_ID);
            Assessment _assessment;
            string rubricHtml = string.Empty;
            int count = 1;

            _assessment = Assessment.GetAssessmentAndQuestionsByID(xID);
            if (_assessment == null)
            {
                SessionObject.RedirectMessage = "Could not find the assessment.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }


            foreach (TestQuestion tq in _assessment.Items)
            {
                if (tq.Rubric != null)
                {
                    ImageButton closeButton = new ImageButton();
                    closeButton.ImageUrl = "~/Images/X.png";
                    string closeButtonImgUrl = closeButton.ResolveClientUrl(closeButton.ImageUrl);

                    rubricHtml += "<div>" + count.ToString() + ": <a href=\"javascript:void(0);\" onclick=\"displayFullDescription(this); return false;\">" + tq.Rubric.Name
                        + "</a><div class=\"fullText\"><img src=\"" + closeButtonImgUrl + "\" onclick=\"this.parentNode.style.display='none';\" style=\"position:relative; float:right; cursor:pointer;\" />"
                        + tq.Rubric.Content + "</div></div>";
                    count++;
                }
            }
            rubricPreviewDiv.InnerHtml = rubricHtml;
        }
    }
}