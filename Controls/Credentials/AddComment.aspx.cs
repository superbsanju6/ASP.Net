using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Credentials
{
    public partial class AddComment : BasePage
    {
        private SessionObject _sessionObject;

        private Int32 CommentedBy;
        public int studentCredentialID, studentID, credentialID;
        public string hashKey;
        private string Comments;
        private Int32 CommentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_sessionObject == null)
            {
                _sessionObject = (SessionObject)Session["SessionObject"];
            }
            if (Request.QueryString["CommentID"] != null && Convert.ToInt32(Request.QueryString["CommentID"]) != 0)
            {
                CommentId = Convert.ToInt32(Request.QueryString["CommentID"]);

                RadButtonsave.Enabled = true;

            }

            studentCredentialID = Request.QueryString["studCrdId"] != null ? Convert.ToInt32(Request.QueryString["studCrdId"]) : 0;
            studentID = Request.QueryString["studId"] != null ? Convert.ToInt32(Request.QueryString["studId"]) : 0;
            credentialID = Request.QueryString["crdId"] != null ? Convert.ToInt32(Request.QueryString["crdId"]) : 0;
            hashKey = studentID.ToString() + "_" + credentialID.ToString();

            if (!this.IsPostBack)
            {
                LoadCommentforEdit();
            }
        }

        protected void LoadCommentforEdit()
        {
            if (CommentId > 0)
            {
                DataSet ds = GetStudentComments();
                if (ds != null && ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[1];
                    DataRow[] foundRows;
                    foundRows = dt.Select("ID =" + CommentId);
                    if (foundRows.Length > 0)
                    {
                        radcomment.Text = HttpUtility.HtmlDecode(foundRows[0]["CommentText"].ToString().Trim());
                        studentCredentialID = Convert.ToInt32(foundRows[0]["StudentCredentialId"]);
                    }
                }
            }
        }

        protected void RadButtonsave_Click(object sender, EventArgs e)
        {
            CommentedBy = _sessionObject.LoggedInUser.Page;
            Comments = radcomment.Text.Trim();
            Comments=  HttpUtility.HtmlEncode(Comments);           
            CommentId = Base.Classes.Credentials.AddNewComments(credentialID, studentID, CommentId, Comments, CommentedBy);
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "UpdateCommentScript", "closeSaveWindow();", true);                
        }       

        private DataSet GetStudentComments(bool flagGetFreshDbData = false)
        {
            DataSet ds = new DataSet();
            ds = Base.Classes.Credentials.GetStudentCredentialsComment(studentID, credentialID);
            return ds;
        }
    }
}