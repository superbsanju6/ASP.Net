using System;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.ControlHost
{
    public partial class PreviewAddendums : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }

            int xID = GetDecryptedEntityId(X_ID);

            Assessment _assessment;
            string addendumHTML = string.Empty;
            int count = 1;

            _assessment = Assessment.GetAssessmentAndQuestionsByID(xID);
            if (_assessment == null)
            {
                SessionObject.RedirectMessage = "Could not find the assessment.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }

            if (_assessment.Addendums.Count == 0)
            {
                addendumPreviewDiv.InnerHtml = "No addendums found.";
            }
            else
            {
                foreach (Addendum oAddendum in _assessment.Addendums)
                {
                    ImageButton closeButton = new ImageButton();
                    closeButton.ImageUrl = "~/Images/X.png";
                    string closeButtonImgURL = closeButton.ResolveClientUrl(closeButton.ImageUrl);

 addendumHTML +="<div>" + count.ToString() + ": <a href=\"javascript:void(0);\" onclick=\"displayFullDescription(this); return false;\">" + oAddendum.Addendum_Name                   + "</a>" +
                      "<div class=\"fullText\">" +
                       "<img src=\"" + closeButtonImgURL + "\" onclick=\"this.parentNode.style.display='none';\" style=\"position:relative; float:right; cursor:pointer;\" /><p>" 
                                + oAddendum.Addendum_Text + 
                      "</p></div>" +
               "</div>";
                    count++;
                }

                addendumPreviewDiv.InnerHtml = addendumHTML;
            }
        }

    }
}