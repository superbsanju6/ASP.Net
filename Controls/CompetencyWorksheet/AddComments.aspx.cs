using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using System.Data;


namespace Thinkgate.Controls.CompetencyWorksheet
{
    public partial class AddComments : System.Web.UI.Page
    {
        public SessionObject session;
        private Int32 StandardId;
        private Guid TeacherId;
        private int StudentId;
        private string Comments;
        private Int32 WorksheetId;
        private Int32 CompetencyCommentId;
        private string sb;


        protected void Page_Load(object sender, EventArgs e)
        {
            //btnSave.Enabled = false;
            sb = txtComments.Text;
            session = (SessionObject)Session["SessionObject"];
               if (Request.QueryString["CommentID"] != null)
               {
                   CompetencyCommentId = Convert.ToInt32(Request.QueryString["CommentID"]);
                   DataSet dscomment = CompetencyWorkSheet.GetCommentDetails(CompetencyCommentId);
                   if (dscomment != null && dscomment.Tables[0].Rows.Count > 0)
                   {
                       if (txtComments.Text == "")
                       {                          
                           txtComments.Text = dscomment.Tables[0].Rows[0]["Comment"].ToString().Trim();
                           //btnSave.Enabled = true;
                       }
                   }
               }
            
            else
            {
                if (Request.QueryString["StandardID"] != null && Request.QueryString["StudentID"] != null && Request.QueryString["WorksheetID"] != null)
                {
                    StandardId = Convert.ToInt32(Request.QueryString["StandardID"]);
                    StudentId = Convert.ToInt32(Request.QueryString["StudentID"]);
                    WorksheetId = Convert.ToInt32(Request.QueryString["WorksheetID"]);
                }
            }

        }        

        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (session.LoggedInUser.UserId !=null)
            TeacherId = session.LoggedInUser.UserId;
            Comments = sb.Trim();// txtComments.Text.Trim();
            CompetencyWorkSheet.AddCompetencyComments(StandardId, TeacherId, StudentId, Comments, WorksheetId, CompetencyCommentId);
            //resultPanel.Visible = true;
            addPanel.Visible = false;
            //lblResultMessage.Text = @"Comment successfully added!";
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "UpdateComm", "closeWindow();", true);
        }

        
    }
}