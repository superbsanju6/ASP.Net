
using System;
using System.Data;
using System.Web.UI;
using Thinkgate.Classes;
using System.Collections;
using System.Linq;
using System.Web;



namespace Thinkgate.Controls.Credentials
{
    public partial class AddUrl : BasePage
    {
        private SessionObject _sessionObject;
        private Int32 CommentedBy;
        private Int32 CredentialId;
        private string Comments;
        private Int32 CommentId;
        DataSet dSCredentials = new DataSet();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            radUrl.Focus();
            base.OnPreRender(e);
        }

        protected void RadButtonsave_Click(object sender, EventArgs e)
        {
            /* Uncomment For Url Validation:
            bool isValidUrl = IsValidUrl(radUrl.Text.Trim());
            if (!isValidUrl)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UrlValidationFail", "urlValidationFailMessage();", true);
                return;
            }
            */
            if (AddCredentialUrl())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "AddUrl", "closeUrlSave();", true);
            }
        }

        protected DataSet GetCredentials()
        {
            int credentialId = 0;
            if (Request.QueryString["crdId"] != null)
                Int32.TryParse(Request.QueryString["crdId"].ToString(), out credentialId);

            if (Session["Credential"] != null)
            {
                dSCredentials = (DataSet)Session["Credential"];
            }
            else
            {
                dSCredentials = Base.Classes.Credentials.GetCredentials("ALL", "DETAIL", credentialId);
            }

            return dSCredentials;
        }

        private bool AddCredentialUrl()
        {
            bool flag = false;
            DataSet ds = GetCredentials();
            try
            {
                if (ds != null)
                {
                    if (ds.Tables.Count == 3)
                    {
                        DataTable dtUrls = ds.Tables[2];
                        int nextUrlId = 1;
                        if (dtUrls.Rows.Count > 0)
                            nextUrlId = (dtUrls.AsEnumerable().Max(r => r.Field<int>("ID")) + 1);
                        DataRow dr = dtUrls.NewRow();
                        dr["ID"] = nextUrlId;
                        dr["URL"] = GetCorrectUrl(Server.HtmlEncode(radUrl.Text));
                        dtUrls.Rows.Add(dr);
                    }
                }
                Session["Credential"] = dSCredentials;
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public string GetCorrectUrl(string url)
        {
            Uri uri = null;

            if (!Uri.TryCreate(url, UriKind.Absolute, out uri) || null == uri)
            {
                return url = "http://" + url; ;
            }
            else
            {
                return url;
            }
        }

        private bool IsValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
    }
}