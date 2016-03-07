using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate
{
    public partial class MainNav : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!Request.IsAuthenticated)
                    // This is an unauthorized, authenticated request...
                    Response.Redirect("~/UnauthorizedAccess.aspx");
            }
         }

        protected void teacherImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var teacherID = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_TeacherID.ToString());
            Response.Redirect("~/Record/Teacher.aspx?xID=" + teacherID, true);
        }

        protected void classImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var classID = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_ClassID.ToString());
            Response.Redirect("~/Record/Class.aspx?xID=" + classID, true);
        }

        protected void districtImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var districtID = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_DistrictID.ToString());
            Response.Redirect("~/Record/District.aspx?xID=" + districtID, true);
        }

        protected void studentImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var studentID = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_StudentID.ToString());
            Response.Redirect("~/Record/Student.aspx?xID=" + studentID, true);
        }

        protected void schoolImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var schoolID = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_SchoolID.ToString());
            Response.Redirect("~/Record/School.aspx?xID=" + schoolID, true);
        }

        protected void assessmentImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var assessmentID = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_TestID.ToString());
            Response.Redirect("~/Record/AssessmentPage.aspx?xID=" + assessmentID, true);
        }
        
    }
}