using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_Item_StandardText : System.Web.UI.Page
    {
        public SessionObject SessionObject;
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject) Session["SessionObject"];
            if (!IsPostBack)
            {
                LoadStandardText();
            }
        }
        protected void LoadStandardText()
        {

            string _sStandardID = "" + Request.QueryString["xID"];

            if (_sStandardID == "")
                return;

            int _iStandardID = Cryptography.GetDecryptedIDFromEncryptedValue(_sStandardID, SessionObject.LoggedInUser.CipherKey);
            Thinkgate.Base.Classes.Standards ItemStandards = Thinkgate.Base.Classes.Standards.GetStandardByID(_iStandardID);
            if (ItemStandards != null)
            {
                lblStandardText.Text = ItemStandards.Desc;
                PageTitle.Text = ItemStandards.StandardName;
            }
        }
    }
}