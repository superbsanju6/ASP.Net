using System;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_Item_AddendumText : System.Web.UI.Page
    {
        public SessionObject SessionObject;
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject) Session["SessionObject"];
            if (!IsPostBack)
            {
                LoadAddedendumText();
            }
        }
        protected void LoadAddedendumText()
        {
            string _sBy = "" + Request.QueryString["by"];
            string _sItemID = "" + Request.QueryString["xID"];
            string _sAssessmentID = "" + Request.QueryString["AssessmentID"];

            if (_sItemID == "")
                return;

            if (_sBy.Trim().Equals("addendum", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadAddedendumContent(_sItemID);
                return;
            }
            
            if (_sAssessmentID == "")
                LoadAddedendumItemBankText(_sItemID);
            else
                LoadAddedendumAssessmentItemText(_sItemID, _sAssessmentID);

        }

        protected void LoadAddedendumItemBankText(string ItemID) {

            int _iItemID = Cryptography.GetDecryptedIDFromEncryptedValue(ItemID, SessionObject.LoggedInUser.CipherKey);
            Thinkgate.Base.Classes.Addendum ItemAddendum = Thinkgate.Base.Classes.Addendum.GetAddendumByBankItemID(_iItemID);
            if (ItemAddendum != null)
            {
                lblAddendumText.Text = ItemAddendum.Addendum_Text;
                PageTitle.Text = ItemAddendum.Addendum_Name;
            }
        }
        protected void LoadAddedendumAssessmentItemText(string ItemID, string AssessmentID)
        {
            int _iItemID = Cryptography.GetDecryptedIDFromEncryptedValue(ItemID, SessionObject.LoggedInUser.CipherKey);
            int _iAssessmentID = Cryptography.GetDecryptedIDFromEncryptedValue(AssessmentID, SessionObject.LoggedInUser.CipherKey);
            Thinkgate.Base.Classes.Addendum ItemAddendum = Thinkgate.Base.Classes.Addendum.GetAddendumByAssessmentIDAndItemID(_iItemID, _iAssessmentID);

            if (ItemAddendum != null)
            {
                lblAddendumText.Text = ItemAddendum.Addendum_Text;
                PageTitle.Text = ItemAddendum.Addendum_Name;
            }
        }

        protected void LoadAddedendumContent(string addendumID)
        {
            int _iItemID = Cryptography.GetDecryptedIDFromEncryptedValue(addendumID, SessionObject.LoggedInUser.CipherKey);
            Thinkgate.Base.Classes.Addendum ItemAddendum = Thinkgate.Base.Classes.Addendum.GetAddendumByAddendumID(_iItemID);
            if (ItemAddendum != null)
            {
                lblAddendumText.Text = ItemAddendum.Addendum_Text;
                PageTitle.Text = ItemAddendum.Addendum_Name;
            }
        }
    }
}