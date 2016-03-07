using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Credentials.PDF
{
    public partial class StudentCredentialCommentsPdf : System.Web.UI.Page
    {
        private int studentCredentialID, studentID, credentialID;
        DataSet ds = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                studentCredentialID = Request.QueryString["studentCredentialID"] != null ? Convert.ToInt32(Request.QueryString["studentCredentialID"]) : 0;
                studentID = Request.QueryString["studentID"] != null ? Convert.ToInt32(Request.QueryString["studentID"]) : 0;
                credentialID = Request.QueryString["credentialID"] != null ? Convert.ToInt32(Request.QueryString["credentialID"]) : 0;
            
                BindRepeaterData();
            }
        }

        protected void BindRepeaterData()
        {
            ds = Base.Classes.Credentials.GetStudentCredentialCommentsPDF(studentCredentialID, studentID, credentialID);
            if (ds != null)
            {
                if (ds.Tables.Count == 2)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblstudentname.Text = ds.Tables[0].Rows[0]["StudentName"].ToString();
                        lblCredential.Text = ds.Tables[0].Rows[0]["CredentialName"].ToString();
                    }
                    RepDetails.DataSource = ds.Tables[1];
                    RepDetails.DataBind();
                }
            }

        }
    }
}