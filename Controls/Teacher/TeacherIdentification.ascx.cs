using System;
using System.Web;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Teacher
{
	public partial class TeacherIdentification : TileControlBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;

			var t = (Thinkgate.Base.Classes.Teacher)Tile.TileParms.GetParm("teacher");

            if (!IsPostBack)
            {
                lblName.Text = t.FirstName + " " + t.LastName;
                lblEmail.Text = t.Email;
                if (t.Email.Length > 0) anchorEmail.HRef = "mailto:" + t.Email;

                lblSchool.Text = (t.Schools.Count > 0) ? t.Schools[0].Name : "N/A";
                lblUserID.Text = t.EmployeeID;
                lblUserType.Text = "Teacher"; //TODO: Can a teacher have a different user type?

                tileTeacherIdentification_btnUploadPicture.Visible = (UserHasPermission(Permission.Icon_Upload_Picture));
                if (UserHasPermission(Permission.Icon_Reset_Password))
                {
                    tileTeacherIdentification_btnResetPassword.Visible = true;
                    /* width: 385, height: 300 */
                    tileTeacherIdentification_btnResetPassword.Attributes.Add("onclick", "javascript:customDialog({ url: '../Account/ChgTeacherPasswordAdmin.aspx?xID=" + HttpUtility.UrlEncode(Request.QueryString["xID"]) + "', maximize: true, maxwidth: 385, maxheight: 300, title: 'Change Password'}); return false;");
                }
                else
                {
                    tileTeacherIdentification_btnResetPassword.Visible = false;
                }

            }
		}
	}
}