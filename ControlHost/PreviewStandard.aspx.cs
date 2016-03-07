using System;
using Thinkgate.Classes;

namespace Thinkgate.ControlHost
{
    public partial class PreviewStandard : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }

            int xID = GetDecryptedEntityId(X_ID);

            Thinkgate.Base.Classes.Standards standard = SessionObject.PreviewStandardsDialogParms.ContainsKey(xID) ? SessionObject.PreviewStandardsDialogParms[xID] : Base.Classes.Standards.GetStandardByID(xID);

            string standardID = Request.QueryString["xID"];
            string standardName = standard.StandardName;
            string standardText = standard.Desc;

            if (String.IsNullOrEmpty(standardID))
            {
                RedirectToPortalSelectionScreen();
            }

            standardPreviewDiv.InnerHtml = "<a href=\"javascript:void(0);\" onclick=\"window.open('../Record/StandardsPage.aspx?xID=" + standardID + "');\">" + standardName + "</a><br/><br/>" + standardText;
        }

    }
}