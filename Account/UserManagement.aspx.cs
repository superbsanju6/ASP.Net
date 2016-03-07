using System;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;

namespace Thinkgate.Account
{
	public partial class UserManagement : System.Web.UI.Page
	{
		private MembershipUser _user;

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void LoadLists(String listname)
		{
			switch (listname)
			{
				case "Roles":
					string[] roles = Roles.GetAllRoles();
					UsersRoleList.DataSource = roles;
					UsersRoleList.DataBind();
					dvUserRoles.Visible = true;
					break;
			}
		}


		protected void btnSubmitAccountChanges_Click(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(Username.Text.ToString())) return;
			GetUser(Username.Text);
			if (CheckForUserError()) return;
			_user.Email = (String.IsNullOrEmpty(txtEmailAddress.Text) ? _user.Email : txtEmailAddress.Text);
			if(!_user.IsApproved && cboIsApproved.Checked)
			{
				//TODO Wire Up the You Are Approved email notification
			}
			_user.IsApproved = cboIsApproved.Checked;
			Membership.UpdateUser(_user);
			if (_user.IsLockedOut && !cboLockedOut.Checked) _user.UnlockUser();

			if (cboResetPassword.Checked)
			{
				_user.ChangePassword(_user.GetPassword(), AppSettings.DefaultPassword);
				//TODO Wire Up the email notification
			}
		}

		protected void btnSubmitUserRoleChanges_Click(object sender, EventArgs e)
		{
		}

		protected void cbolUserRoles_DataBinding(object sender, EventArgs e)
		{

		}

		protected void btnSelectUser_Click(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(Username.Text)) return;
			GetUser(Username.Text);
			if (CheckForUserError()) return;


			txtEmailAddress.Text = _user.Email;
			cboIsApproved.Checked = _user.IsApproved;
			cboLockedOut.Checked = _user.IsLockedOut;
			cboLockedOut.Enabled = _user.IsLockedOut;
			StringBuilder userInfo = new StringBuilder();
			userInfo.AppendLine("Last Login: " + _user.LastLoginDate);
			userInfo.AppendLine("Last Activity: " + _user.LastActivityDate);
			userInfo.AppendLine("Is Online?: " + _user.IsOnline);
			userInfo.AppendLine("Last Lockout Date: " + (_user.LastLockoutDate < new DateTime(2011, 1, 1) ? "N/A" : _user.LastLockoutDate.ToString()));
			userInfo.AppendLine("Created Date: " + _user.CreationDate);
			txtUserInfo.Text = userInfo.ToString();
		}

		protected void GetUser(string username)
		{
			_user = Membership.GetUser(username);
			LoadLists("Roles");
			CheckRolesForSelectedUser(username);
		}

		protected bool CheckForUserError()
		{
			if (_user == null)
			{
				lblResultMessage.Text = String.Format("{0} Not Found", Username.Text);
				return true;
			}
			return false;
		}

		private void CheckRolesForSelectedUser(string username)
		{
			// Determine what roles the selected user belongs to 
			string[] selectedUsersRoles = Roles.GetRolesForUser(username);

			// Loop through the Repeater's Items and check or uncheck the checkbox as needed 

			foreach (RepeaterItem ri in UsersRoleList.Items)
			{
				// Programmatically reference the CheckBox 
				CheckBox RoleCheckBox = ri.FindControl("RoleCheckBox") as CheckBox;
				// See if RoleCheckBox.Text is in selectedUsersRoles 
				if (selectedUsersRoles.Contains<string>(RoleCheckBox.Text))
					RoleCheckBox.Checked = true;
				else
					RoleCheckBox.Checked = false;
			}
		}

		protected void RoleCheckBox_CheckChanged(object sender, EventArgs e)
		{
			CheckBox RoleCheckBox = sender as CheckBox;

			string selectedUserName = Username.Text;
			string roleName = RoleCheckBox.Text;

			// Determine if we need to add or remove the user from this role 
			if (RoleCheckBox.Checked)
			{
				// Add the user to the role 
				Roles.AddUserToRole(selectedUserName, roleName);
				// Display a status message 
				ActionStatus.Text = string.Format("User {0} was added to role {1}.", selectedUserName, roleName);
			}
			else
			{
				// Remove the user from the role 
				Roles.RemoveUserFromRole(selectedUserName, roleName);
				// Display a status message 
				ActionStatus.Text = string.Format("User {0} was removed from role {1}.", selectedUserName, roleName);

			}
		}
	}
}