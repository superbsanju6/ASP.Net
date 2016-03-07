using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Credentials
{
    public partial class AddCredential : BasePage
    {
        private SessionObject _sessionObject;
        public DataSet dSCredentials = new DataSet();
        public int credentialID = 0;
        public int _userID = 0;
        public int assignedCount = 0;
        public int isActive = 0;
        public string credentialName = "";
        public string alignments = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["crdId"] != null)
            {
                Int32.TryParse(Request.QueryString["crdId"].ToString(), out credentialID);
            }
            if (!IsPostBack)
            {
                if (_sessionObject == null)
                {
                    _sessionObject = (SessionObject)Session["SessionObject"];
                    _userID = _sessionObject.LoggedInUser.Page;
                }

                Session.Remove("Credential");

                if (SessionObject.IsAlignmentDataAvailable == null)
                {
                    SessionObject.IsAlignmentDataAvailable = Base.Classes.Credentials.IsAlignmentDataAvailable();
                }

                if (SessionObject.IsAlignmentDataAvailable == true)
                {
                    trAlignment.Visible = true;
                }
                else
                {
                    trAlignment.Visible = false;
                }

                if (Request.QueryString["crdId"] != null)
                {
                    Int32.TryParse(Request.QueryString["crdId"].ToString(), out credentialID);
                    RadButtonDelete.Visible = true;                    
                    trActivate.Visible = true;
                    trSpacer.Visible = false;
                }
                else
                {
                    RadButtonDelete.Visible = false;                    
                    trActivate.Visible = false;
                    trSpacer.Visible = !trAlignment.Visible;
                }
                BindPageControls();
            }
        }

        private void BindPageControls()
        {
            dSCredentials = GetCredentials();

            if (dSCredentials != null && dSCredentials.Tables.Count == 3)
            {
                if (dSCredentials.Tables[0].Rows.Count > 0)
                {
                    assignedCount = Convert.ToInt32(dSCredentials.Tables[0].Rows[0]["AssignedToCount"]);
                    isActive = Convert.ToInt32(dSCredentials.Tables[0].Rows[0]["IsActive"]);
                    txtCredentialName.Text = Convert.IsDBNull(dSCredentials.Tables[0].Rows[0]["Name"]) ? "" : Server.HtmlDecode(dSCredentials.Tables[0].Rows[0]["Name"].ToString().Trim());
                    hdcredentialName.Value = txtCredentialName.Text.Replace("'", "").Trim();
                    credentialName = txtCredentialName.Text.Replace("'","").Trim();                    
                    ViewState["CredentialUniqueName"] = credentialName;
                    rdActivateDeactivate.Checked = Convert.ToInt32(dSCredentials.Tables[0].Rows[0]["IsActive"]) == 1 ? false : true;
                }

                if (dSCredentials.Tables[1].Rows.Count > 0)
                {
                    alignments = dSCredentials.Tables[1].Rows[0]["Alignments"].ToString().Trim();
                    hdnAlignments.Value = alignments;
                }

                if (dSCredentials.Tables[2].Rows.Count > 0)
                {
                    dtlURLs.DataSource = dSCredentials.Tables[2];
                    dtlURLs.DataBind();
                }
                Session["Credential"] = dSCredentials;
            }
        }       

        protected void imgDeleteUrl_Command(object sender, CommandEventArgs e)
        {
            dSCredentials = GetCredentials();

            if (dSCredentials != null && dSCredentials.Tables.Count == 3)
            {
                DataTable dtUrls = dSCredentials.Tables[2];
                if (dtUrls.Rows.Count > 0)
                {
                    DataRow[] foundRows;
                    foundRows = dtUrls.Select("ID =" + e.ToString());
                    if (foundRows.Length > 0)
                        dtUrls.Rows.Remove(foundRows[0]);

                    dtlURLs.DataSource = dtUrls;
                    dtlURLs.DataBind();
                }
                Session["Credential"] = dSCredentials;
            }
        }

        protected DataSet GetCredentials()
        {           
            if (Session["Credential"] != null)
            {
                dSCredentials = (DataSet)Session["Credential"];
            }
            else
            {
                dSCredentials = Base.Classes.Credentials.GetCredentials("ALL", "DETAIL", credentialID);
            }
            return dSCredentials;
        }

        protected void imgRemoveUrl_Command(object sender, CommandEventArgs e)
        {
            dSCredentials = GetCredentials();

            if (dSCredentials != null && dSCredentials.Tables.Count == 3)
            {
                DataTable dtUrls = dSCredentials.Tables[2];
                if (dtUrls.Rows.Count > 0 && e != null && e.CommandArgument != null)
                {
                    DataRow[] foundRows;
                    foundRows = dtUrls.Select("ID =" + e.CommandArgument.ToString());
                    if (foundRows.Length > 0)
                        dtUrls.Rows.Remove(foundRows[0]);

                    dtlURLs.DataSource = dtUrls;
                    dtlURLs.DataBind();
                }
                Session["Credential"] = dSCredentials;
            }
        }

        protected void btnHiddenBind_ServerClick(object sender, EventArgs e)
        {
            dSCredentials = GetCredentials();

            if (dSCredentials != null && dSCredentials.Tables.Count == 3)
            {
                if (dSCredentials.Tables[2].Rows.Count > 0)
                {
                    dtlURLs.DataSource = dSCredentials.Tables[2];
                    dtlURLs.DataBind();
                }                
            }
        }

        protected void RadButtonUpdate_Click(object sender, EventArgs e)
        {
                   
            DataSet dsCredentialList = Base.Classes.Credentials.GetCredentials("ALL", "SUMMARY");
            if (dsCredentialList != null && dsCredentialList.Tables.Count > 0 && dsCredentialList.Tables[0].Rows.Count > 0)
            {
                DataRow[] foundRows;
                foundRows = dsCredentialList.Tables[0].Select("NAME ='" + Server.HtmlEncode(txtCredentialName.Text.Trim()) + "'");
                if ((foundRows.Length > 0 && ViewState["CredentialUniqueName"]==null)||(foundRows.Length > 0 && rdActivateDeactivate.Checked == false && ViewState["CredentialUniqueName"].ToString().Trim().ToLower() != txtCredentialName.Text.Trim().ToLower()))
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "ExistingCredential", "existingCredentialAlert();", true);
                    return;
                }

            }

            _sessionObject = (SessionObject)Session["SessionObject"];
            var credentialName = Server.HtmlEncode(txtCredentialName.Text.Trim());
            
            dSCredentials = (DataSet)Session["Credential"];
            if (Request.QueryString["crdId"] != null)
            {
                Int32.TryParse(Request.QueryString["crdId"].ToString(), out credentialID);
            }
            else
            {
                if (dSCredentials != null && dSCredentials.Tables.Count == 3 && dSCredentials.Tables[0].Rows.Count == 0)
                {
                    dSCredentials.Tables[0].Rows.Add(dSCredentials.Tables[0].NewRow());
                    dSCredentials.Tables[0].Rows[0]["Name"] = Server.HtmlEncode(txtCredentialName.Text.Trim());
                    dSCredentials.Tables[0].Rows[0]["IsActive"] = 1;
                }
            }

            if(dSCredentials != null && dSCredentials.Tables.Count == 3 && dSCredentials.Tables[0].Rows.Count > 0)
                dSCredentials.Tables[0].Rows[0]["Name"] = Server.HtmlEncode(txtCredentialName.Text.Trim());

            if (dSCredentials != null && dSCredentials.Tables.Count == 3 &&  dSCredentials.Tables[1].Rows.Count > 0)
            {
                dSCredentials.Tables[1].Rows[0]["Alignments"] = hdnAlignments.Value.Trim();
            }


            bool isSuccess = Base.Classes.Credentials.AddNewCredential(dSCredentials, credentialID, _sessionObject.LoggedInUser.Page, rdActivateDeactivate.Checked ? 0 : 1);
            if (isSuccess)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "ClodeEditCredential", "closeCredentialsWindow();", true);
            }
        }

        protected void RadButtonDelete_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["crdId"] != null)
            {
                Int32.TryParse(Request.QueryString["crdId"].ToString(), out credentialID);
            }

            _sessionObject = (SessionObject)Session["SessionObject"];
            Base.Classes.Credentials.DeleteCredential(credentialID,_sessionObject.LoggedInUser.Page);
            System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "ClodeDeleteCredential", "closeCredentialsWindow();", true);
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

        protected void hypUrlDelete_Click(object sender, EventArgs e)
        {
            HtmlButton btn = (HtmlButton)sender;
            string urlId = btn.Attributes["argument"];
          
            // post back target
            dSCredentials = GetCredentials();

            if (dSCredentials != null && dSCredentials.Tables.Count == 3)
            {
                DataTable dtUrls = dSCredentials.Tables[2];
                if (dtUrls.Rows.Count > 0 && e != null && urlId != null)
                {
                    DataRow[] foundRows;
                    foundRows = dtUrls.Select("ID =" + urlId);
                    if (foundRows.Length > 0)
                        dtUrls.Rows.Remove(foundRows[0]);

                    dtlURLs.DataSource = dtUrls;
                    dtlURLs.DataBind();
                }
                Session["Credential"] = dSCredentials;
            }
        }

    }
}