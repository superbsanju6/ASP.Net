using System;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Thinkgate.Account
{
    public partial class RoleManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!Page.IsPostBack)
				DisplayRolesInGrid();
        }

		protected void CreateRoleButton_Click(object sender, EventArgs e)
		{
			string newRoleName = RoleName.Text.Trim();

			if (!Roles.RoleExists(newRoleName))
				Roles.CreateRole(newRoleName);

			RoleName.Text = string.Empty;
			DisplayRolesInGrid();
		}
		
		private void DisplayRolesInGrid()
		{
			RoleList.DataSource = Roles.GetAllRoles();
			RoleList.DataBind();
		}

		protected void RoleList_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			Label RoleNameLabel = RoleList.Rows[e.RowIndex].FindControl("RoleNameLabel") as Label;
			Roles.DeleteRole(RoleNameLabel.Text, false);
			DisplayRolesInGrid();
		}
    }
}