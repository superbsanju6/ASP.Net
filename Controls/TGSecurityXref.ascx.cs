using System;
using System.Collections;
using System.Data;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Controls
{
	public partial class TGSecurityXref : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if(!Page.IsPostBack)
			{
				LoadLists();
			}
		}

		protected void LoadLists()
		{
			string method = Request.QueryString["Method"];
			switch (method)
			{
                case "LoadRolePermissions":
                    LoadRolePermissions();
			        rtlRolePermissionsForEditing.Visible = true;
					rtlPricingModulePermissionsForEditing.Visible = false;
					rgSchoolPricingModulesForEditing.Visible = false;
					break;
				case "LoadSchoolPricingModules":
					LoadSchoolPricingModules();
                    rtlRolePermissionsForEditing.Visible = false;
					rtlPricingModulePermissionsForEditing.Visible = false;
					rgSchoolPricingModulesForEditing.Visible = true;
					break;
				case "LoadPricingModulePermissions":
					LoadPricingModulePermissions();
                    rtlRolePermissionsForEditing.Visible = false;
					rtlPricingModulePermissionsForEditing.Visible = true;
					rgSchoolPricingModulesForEditing.Visible = false;
					break;
			}
		}
		
		protected void LoadRolePermissions()
		{
		    rtlRolePermissionsForEditing_NeedDataSource(rtlRolePermissionsForEditing,
                                                        new TreeListNeedDataSourceEventArgs(TreeListRebindReason.PostBackEvent));
            rtlRolePermissionsForEditing.DataBind();	
		}

		protected void LoadPricingModulePermissions()
		{
			rtlPricingModulePermissionsForEditing_NeedDataSource(rtlPricingModulePermissionsForEditing, new TreeListNeedDataSourceEventArgs(TreeListRebindReason.PostBackEvent));
			rtlPricingModulePermissionsForEditing.DataBind();
		}

		protected void LoadSchoolPricingModules()
		{
			rgSchoolPricingModulesForEditing_NeedDataSource(rgSchoolPricingModulesForEditing, new GridNeedDataSourceEventArgs(GridRebindReason.PostBackEvent));
			rgSchoolPricingModulesForEditing.DataBind();
			
		}

		protected DataTable ConvertToRolePermissionsBindingTable(ThinkgatePermissionsCollection permissions, ThinkgateRole role)
		{
			DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("ID", typeof(System.Guid)));
            table.Columns.Add(new DataColumn("ParentID", typeof(System.Guid)));
			table.Columns.Add(new DataColumn("Name", typeof(string)));
			table.Columns.Add(new DataColumn("Member", typeof(bool)));
			table.Columns.Add(new DataColumn("PermissionLevelValue", typeof(int)));
			table.Columns.Add(new DataColumn("Description", typeof(string)));

			foreach (ThinkgatePermission permission in permissions)
			{
				DataRow row = table.NewRow();
                row["ID"] = permission.PermissionId;
				row["ParentID"] = permission.ParentPermissionId;
				row["Name"] = permission.PermissionName;
				row["Member"] = role.PermissionsHT.Contains(permission);
				row["PermissionLevelValue"] = role.PermissionsHT.Contains(permission) ? role.PermissionsHT.GetPermission(permission.ToString()).PermissionLevelValue : DataIntegrity.ConvertToInt(ThinkgatePermission.PermissionLevelValues.NoValue.ToString());
				row["Description"] = permission.Description;
				table.Rows.Add(row);
			}
			return table;
		}

		protected DataTable ConvertToPricingModulePermissionsBindingTable(ThinkgatePermissionsCollection permissions, ThinkgatePricingModule pricingModule)
		{
			DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("ID", typeof(System.Guid)));
            table.Columns.Add(new DataColumn("ParentID", typeof(System.Guid)));
            table.Columns.Add(new DataColumn("Name", typeof(string)));
            table.Columns.Add(new DataColumn("Member", typeof(bool)));
            table.Columns.Add(new DataColumn("PermissionLevelValue", typeof(int)));
            table.Columns.Add(new DataColumn("Description", typeof(string)));

			foreach (ThinkgatePermission permission in permissions)
			{
				DataRow row = table.NewRow();
				row["ID"] = permission.PermissionId;
                row["ParentID"] = permission.ParentPermissionId;
				row["Name"] = permission.PermissionName;
				row["Member"] = pricingModule.Permissions.Contains(permission);
				row["PermissionLevelValue"] = pricingModule.Permissions.Contains(permission) ? pricingModule.Permissions.GetPermission(permission.PermissionId).PermissionLevelValue : DataIntegrity.ConvertToInt(ThinkgatePermission.PermissionLevelValues.NoValue.ToString());
				row["Description"] = permission.Description;
				table.Rows.Add(row);
			}
			return table;
		}

		protected DataTable ConvertToSchoolPricingModulesBindingTable(DataTable pricingModules, ThinkgateSchool school)
		{
			pricingModules.Columns.Add(new DataColumn("Member", typeof(bool)));
			pricingModules.Columns.Add(new DataColumn("Description", typeof(string)));

			foreach (DataRow row in pricingModules.Rows)
			{
				row["Member"] = school.ContainsPricingModule(DataIntegrity.ConvertToInt(row["ID"].ToString()));
			}
			return pricingModules;
		}

        protected void rtlRolePermissionsForEditing_UpdateCommand(Object source, TreeListCommandEventArgs e)
        {
            var editedItem = e.Item as TreeListDataItem;
            if (editedItem == null) return;

            Guid newRoleID = new Guid();
            Guid.TryParse(lblHeaderItemID.Text, out newRoleID);
            if (newRoleID == default(Guid)) return;

            ThinkgateRole role = new ThinkgateRole(newRoleID);
            if (String.IsNullOrEmpty(role.RoleName)) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTreeList.ExtractValuesFromItem(newValues, editedItem, true);
            Guid permissionID = new Guid();
            Guid.TryParse(newValues["ID"].ToString(), out permissionID);
            if (permissionID == default(Guid)) return;

            string permissionName = newValues["Name"] == null ? string.Empty : newValues["Name"].ToString();
            int permissionLevel = newValues["PermissionLevelValue"] == null ? 0 : DataIntegrity.ConvertToInt(newValues["PermissionLevelValue"].ToString());
            bool hasPermission = newValues["Member"] != null && DataIntegrity.ConvertToBool(newValues["Member"]);

            ThinkgatePermission permission = new ThinkgatePermission(permissionID);
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            ThinkgateUser user = sessionObject.LoggedInUser;
                
            if (hasPermission)
            {
                permission.PermissionLevel = (ThinkgatePermission.PermissionLevelValues)Enum.ToObject(typeof(ThinkgatePermission.PermissionLevelValues), permissionLevel);
                
                //pricingModule.Permissions.Add(permission);        // Not sure what point of this was
                role.UpdateRolePermissions(permissionID, permissionLevel, user.UserName);
                lblResultMessage.Text = string.Format("Permission {0} was added to Role {1}.", permissionName, role.RoleName);

                // WSH: Going to loop through the children and deal with them. In theory, you could devise a strategy that involved fewer calls to the DB and did a mass update. However you have to do a uniqueness check on each child insert or do a complete delete of all children recs to then do a mass insert. Since this will be a vary rare update, I decided performance was not important.
                foreach (TreeListDataItem item in editedItem.ChildItems)
                {
                    permissionID = new Guid();
                    Guid.TryParse(item.GetDataKeyValue("ID").ToString(), out permissionID);
                    if (permissionID == default(Guid)) return;

                    role.UpdateRolePermissions(permissionID, permissionLevel, user.UserName);
                }
            }
            else
            {
                //pricingModule.Permissions.Remove(permission);   // WSH : Not sure what point of this was
                role.DeleteRolePermissions(permissionID);
                lblResultMessage.Text = string.Format("Permission {0} was revoked from Role {1}.", permissionName, role.RoleName);

                // WSH: Going to loop through the children and deal with them. In theory, you could devise a strategy that involved fewer calls to the DB and did a mass update. However you have to do a uniqueness check on each child insert or do a complete delete of all children recs to then do a mass insert. Since this will be a vary rare update, I decided performance was not important.
                foreach (TreeListDataItem item in editedItem.ChildItems)
                {
                    permissionID = new Guid();
                    Guid.TryParse(item.GetDataKeyValue("ID").ToString(), out permissionID);
                    if (permissionID == default(Guid)) return;

                    role.DeleteRolePermissions(permissionID);
                }
            }

        }

        protected void rtlRolePermissionsForEditing_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            Guid roleID = new Guid();
            Guid.TryParse(Request.QueryString["ID"], out roleID);
            if (roleID == default(Guid)) return;

            ThinkgatePermissionsCollection permissions = new ThinkgatePermissionsCollection();
            permissions.GetPermissionsCollection(PermissionCollectionTypes.All, 1);
            ThinkgateRole role = new ThinkgateRole(roleID);
            lblHeaderItemID.Text = role.RoleId.ToString();
            lblHeaderItemName.Text = String.Format("Permissions in {0}  Role:", role.RoleName);

            DataTable test  = ConvertToRolePermissionsBindingTable(permissions, role);
            rtlRolePermissionsForEditing.DataSource = test;
        }

        protected void rtlPricingModulePermissionsForEditing_UpdateCommand(Object source, TreeListCommandEventArgs e)
        {
            var editedItem = e.Item as TreeListDataItem;
            if (editedItem == null) return;

            int newPricingModuleID = DataIntegrity.ConvertToInt(lblHeaderItemID.Text);
            if (newPricingModuleID == default(int)) return;

            ThinkgatePricingModule pricingModule = new ThinkgatePricingModule(newPricingModuleID);
            if (String.IsNullOrEmpty(pricingModule.Name)) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTreeList.ExtractValuesFromItem(newValues, editedItem, true);
            Guid permissionID = new Guid();
            Guid.TryParse(newValues["ID"].ToString(), out permissionID);
            if (permissionID == default(Guid)) return;

            string permissionName = newValues["Name"] == null ? string.Empty : newValues["Name"].ToString();
            bool hasPermission = newValues["Member"] != null && DataIntegrity.ConvertToBool(newValues["Member"]);

            //ThinkgatePermission permission = new ThinkgatePermission(permissionID);       // WSH : Not sure what point of this was

            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            ThinkgateUser user = sessionObject.LoggedInUser;
            if (hasPermission)
            {
                //pricingModule.Permissions.Add(permission);        // Not sure what point of this was
                pricingModule.UpdatePricingModulePermissions(permissionID, 0, user.UserName);
                lblResultMessage.Text = string.Format("Permission {0} was added to Pricing Module {1}.", permissionName,
                                                      pricingModule.Name);

                // WSH: Going to loop through the children and deal with them. In theory, you could devise a strategy that involved fewer calls to the DB and did a mass update. However you have to do a uniqueness check on each child insert or do a complete delete of all children recs to then do a mass insert. Since this will be a vary rare update, I decided performance was not important.
                foreach (TreeListDataItem item in editedItem.ChildItems)
                {
                    permissionID = new Guid();
                    Guid.TryParse(item.GetDataKeyValue("ID").ToString(), out permissionID);
                    if (permissionID == default(Guid)) return;

                    pricingModule.UpdatePricingModulePermissions(permissionID, 0, user.UserName);
                }
            }
            else
            {
                //pricingModule.Permissions.Remove(permission);   // WSH : Not sure what point of this was
                pricingModule.DeletePricingModulePermissions(permissionID);
                lblResultMessage.Text = string.Format("Permission {0} was revoked from Pricing Module {1}.", permissionName, pricingModule.Name);

                // WSH: Going to loop through the children and deal with them. In theory, you could devise a strategy that involved fewer calls to the DB and did a mass update. However you have to do a uniqueness check on each child insert or do a complete delete of all children recs to then do a mass insert. Since this will be a vary rare update, I decided performance was not important.
                foreach (TreeListDataItem item in editedItem.ChildItems)
                {
                    permissionID = new Guid();
                    Guid.TryParse(item.GetDataKeyValue("ID").ToString(), out permissionID);
                    if (permissionID == default(Guid)) return;

                    pricingModule.DeletePricingModulePermissions(permissionID);
                }
            }
        }

        protected void rtlPricingModulePermissionsForEditing_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            int pricingModuleID = DataIntegrity.ConvertToInt(Request.QueryString["ID"]);
            if (pricingModuleID == 0) return;

            ThinkgatePermissionsCollection permissions = new ThinkgatePermissionsCollection();
            permissions.GetPermissionsCollection(PermissionCollectionTypes.All, 1);
            ThinkgatePricingModule pricingModule = new ThinkgatePricingModule(pricingModuleID);
            lblHeaderItemID.Text = pricingModule.Id.ToString();
            lblHeaderItemName.Text = String.Format("Permissions in {0}  Pricing Module:", pricingModule.Name);

            rtlPricingModulePermissionsForEditing.DataSource = ConvertToPricingModulePermissionsBindingTable(permissions, pricingModule);

        }

		protected void rgSchoolPricingModulesForEditing_UpdateCommand(Object source, GridCommandEventArgs e)
		{
			var editedItem = e.Item as GridEditableItem;
			if (editedItem == null) return;

			int newSchoolID = DataIntegrity.ConvertToInt(lblHeaderItemID.Text);
			if (newSchoolID == default(int)) return;

			ThinkgateSchool school = new ThinkgateSchool(newSchoolID, true);
			if (String.IsNullOrEmpty(school.Name)) return;

			//Get the new values:
			var newValues = new Hashtable();
			e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
			int pricingModuleID = DataIntegrity.ConvertToInt(editedItem.GetDataKeyValue("ID").ToString());
			if (pricingModuleID == default(int)) return;

			bool hasPermission = newValues["Member"] != null && DataIntegrity.ConvertToBool(newValues["Member"]);

			ThinkgatePricingModule pricingModule = new ThinkgatePricingModule(pricingModuleID);

			if (hasPermission)
			{
				school.PricingModules.Add(pricingModule);
				SessionObject sessionObject = (SessionObject)Session["SessionObject"];
				ThinkgateUser user = sessionObject.LoggedInUser;
				school.UpdateSchoolPricingModules(pricingModuleID, user.UserName);
				lblResultMessage.Text = string.Format("Pricing Module {0} was added to School {1}.", pricingModule.Name, school.Name);
			}
			else
			{
				school.PricingModules.Remove(pricingModule);
				school.DeleteSchoolPricingModules(pricingModuleID);
				lblResultMessage.Text = string.Format("Pricing Module {0} was revoked from School {1}.", pricingModule.Name, school.Name);
			}

		}

		protected void rgSchoolPricingModulesForEditing_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			int schoolID = DataIntegrity.ConvertToInt(Request.QueryString["ID"]);
			if (schoolID == 0) return;

			DataTable allPricingModules = ThinkgatePricingModule.GetAllPricingModuleIdsAndNames();

			ThinkgateSchool school = new ThinkgateSchool(schoolID, true);
			lblHeaderItemID.Text = school.Id.ToString();
			lblHeaderItemName.Text = String.Format("Pricing Modules in {0}  School:", school.Name);

			rgSchoolPricingModulesForEditing.DataSource = ConvertToSchoolPricingModulesBindingTable(allPricingModules, school);

		}


	}
}
