using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate
{
	public partial class Security : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.User.Identity.IsAuthenticated || Session.IsNewSession || Session == null || (SessionObject)Session["SessionObject"] == null)
			{
                Services.Service2.KillSession();
			}

			dvMenu.Visible = Page.User.IsInRole("Administrators");
			if (!Page.IsPostBack)
			{
				//CheckSessionTimeout();
				lblLoggedInUser.Text = (Page.User != null && Page.User.Identity.IsAuthenticated
				                        	? String.Format("Welcome {0}", Page.User.Identity.Name)
				                        	: lblLoggedInUser.Text);
			}

		}

		protected void HomeButton_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/PortalSelection.aspx", true);
		}

		protected void SignOffButton_Click(object sender, EventArgs e)
		{
			FormsAuthentication.SignOut();
			Response.Redirect("~/Logout.aspx", true);
		}

		protected void ShowGenericRadNotification(string message)
		{
			genericRadNotification.Title = "Notification";
			genericRadNotification.Text = message;
			genericRadNotification.Show();
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

		protected void btnLogout_Click(object sender, EventArgs e)
		{
			FormsAuthentication.SignOut();
			Session.Abandon();
			Response.Redirect("~/Logout.aspx");
		}

	}
}