using System;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Staff
{
    public partial class StaffIdentification : TileControlBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;

			var staffMember = (Thinkgate.Base.Classes.Staff)Tile.TileParms.GetParm("staff");

            lblName.Text = String.Concat(staffMember.FirstName, " ", staffMember.MiddleName, " ", staffMember.LastName);
            lblEmail.Text = staffMember.Email;
            if (staffMember.Email.Length > 0) anchorEmail.HRef = "mailto:" + staffMember.Email;

            foreach(var school in staffMember.SchoolList)
            {
                lblSchool.Text += String.Concat(school.Name, "<br/>");
            }

            if (staffMember.SchoolList.Count == 0) lblSchool.Text = "N/A";

            lblUserID.Text = staffMember.LoginID;

            foreach (var role in staffMember.RoleList)
            {
                lblUserType.Text += String.Concat(role.RoleName, "<br/>");
            }

            if (UserHasPermission(Permission.Icon_Reset_Password))
            {
                btnResetPassword.Visible = true;
                /* width: 385, height: 300 */
                btnResetPassword.Attributes.Add("onclick", "javascript:customDialog({ url: '../Account/ChgTeacherPasswordAdmin.aspx?xID=" + Request.QueryString["xID"] + "', maximize: true, maxwidth: 385, maxheight: 300, title: 'Change Password'}); return false;");
            }
            else
            {
                btnResetPassword.Visible = false;
            }
		}
	}
}