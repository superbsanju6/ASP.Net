using System;

namespace Thinkgate.Account
{
	public partial class AccountMenu : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			dvMenu.Visible = this.Page.User.IsInRole("Administrators");
		}

		protected void btnChangePassword_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Account/ChangePassword.aspx");
		}

		protected void btnResetPassword_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Account/ResetPassword.aspx");
		}

		protected void btnRegistration_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Account/Registration.aspx");
		}

		protected void btnRoleManagement_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Account/RoleManagement.aspx");
		}

		protected void btnUserManagemnt_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Account/UserManagement.aspx");
		}

		protected void btnHome_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/PortalSelection.aspx");
		}
	}
}