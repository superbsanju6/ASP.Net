using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Web.Security;

namespace Thinkgate
{
    public partial class Administration : System.Web.UI.Page
    {
        private Thinkgate.Base.Classes.ThinkgateUser _user;

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            _user = sessionObject.SelectedUser;
            if (!Page.IsPostBack)
            {
                LoadPermissions();
                LoadRoles();
                LoadPricingModules();
                LoadSchools();
                if (_user != null)
                {
                    LoadUserRolesForEditing(_user.UserId);
                    LoadUserSchoolsForEditing(_user.UserId);
                    LoadUserPermissionsForEditing(_user.UserId);
                }
            }
        }

        protected void LoadRoles()
        {
            List<ThinkgateRole> roles = ThinkgateRole.GetRolesCollectionForApplication(1);
            rgRoles.DataSource = roles;
            rgRoles.DataBind();
        }

        protected void rgRoles_UpdateCommand(Object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;

            if (editedItem == null) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
            Guid roleID = new Guid();
            Guid.TryParse(editedItem.GetDataKeyValue("RoleID").ToString(), out roleID);
            string roleName = newValues["RoleName"] == null ? string.Empty : newValues["RoleName"].ToString();
            string roleDescription = newValues["Description"] == null ? string.Empty : newValues["Description"].ToString();
            bool active = newValues["Active"] != null && DataIntegrity.ConvertToBool(newValues["Active"]);
            int portalSelection = newValues["RolePortalSelection"] == null ? 0 : DataIntegrity.ConvertToInt(newValues["RolePortalSelection"]);

            if (String.IsNullOrEmpty(roleName)) return;

            ThinkgateRole role = new ThinkgateRole(roleID, roleName, roleDescription, active, portalSelection);
            role.UpdateRole();
            LoadRoles();

        }

        protected void rgRoles_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            List<ThinkgateRole> roles = ThinkgateRole.GetRolesCollectionForApplication(1);
            rgRoles.DataSource = roles;
        }

        protected void LoadSchools()
        {
            List<ThinkgateSchool> schools = ThinkgateSchool.GetSchoolCollectionForApplication();
            rgSchools.DataSource = schools;
            rgSchools.DataBind();
        }

        protected void rgSchools_UpdateCommand(Object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;

            if (editedItem == null) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
            int id = DataIntegrity.ConvertToInt(editedItem.GetDataKeyValue("ID").ToString());
            string name = newValues["Name"] == null ? string.Empty : newValues["Name"].ToString();
            int? page = DataIntegrity.ConvertToNullableInt(newValues["Page"]);
            int district = DataIntegrity.ConvertToInt(newValues["District"]);
            string abbreviation = newValues["Abbreviation"] == null ? string.Empty : newValues["Abbreviation"].ToString();
            string phone = newValues["Phone"] == null ? string.Empty : newValues["Phone"].ToString();
            string cluster = newValues["Cluster"] == null ? string.Empty : newValues["Cluster"].ToString();
            string schoolType = newValues["SchoolType"] == null ? string.Empty : newValues["SchoolType"].ToString();
            string schoolId = newValues["SchoolID"] == null ? string.Empty : newValues["SchoolID"].ToString();
            string portalFlag = newValues["PortalFlag"] == null ? string.Empty : newValues["PortalFlag"].ToString();

            if (id < 1) return;

            ThinkgateSchool school = new ThinkgateSchool(id, name, page, district, abbreviation, phone, cluster, schoolType, schoolId, portalFlag);

            school.UpdateSchool();
            LoadSchools();

        }

        protected void rgSchools_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            List<ThinkgateSchool> schools = ThinkgateSchool.GetSchoolCollectionForApplication();
            rgSchools.DataSource = schools;

        }

        protected void LoadPricingModules()
        {
            List<ThinkgatePricingModule> pricingModules = ThinkgatePricingModule.GetPricingModulesCollectionForApplication();
            rgPricingModules.DataSource = pricingModules;
            rgPricingModules.DataBind();
        }

        protected void rgPricingModules_UpdateCommand(Object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;

            if (editedItem == null) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
            int id = DataIntegrity.ConvertToInt(editedItem.GetDataKeyValue("ID").ToString());
            string name = newValues["Name"] == null ? string.Empty : newValues["Name"].ToString();
            bool active = newValues["Active"] != null && DataIntegrity.ConvertToBool(newValues["Active"]);

            if (id < 1) return;

            ThinkgatePricingModule pricingModule = new ThinkgatePricingModule(id, name, active);

            pricingModule.UpdatePricingModule();
            LoadPricingModules();

        }

        protected void rgPricingModules_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            List<ThinkgatePricingModule> pricingModules = ThinkgatePricingModule.GetPricingModulesCollectionForApplication();
            rgPricingModules.DataSource = pricingModules;
        }

        protected void LoadPermissions()
        {
            ThinkgatePermissionsCollection permissions = new ThinkgatePermissionsCollection();
            permissions.GetPermissionsCollection(PermissionCollectionTypes.All, 1);
            rgPermissions.DataSource = permissions.Permissions;
            rgPermissions.DataBind();
        }

        protected void rgPermissions_UpdateCommand(Object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;

            if (editedItem == null) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
            Guid permissionID = new Guid();
            Guid.TryParse(editedItem.GetDataKeyValue("PermissionID").ToString(), out permissionID);
            string permissionName = newValues["PermissionName"] == null ? string.Empty : newValues["PermissionName"].ToString();
            string description = newValues["Description"] == null ? string.Empty : newValues["Description"].ToString();

            if (String.IsNullOrEmpty(permissionName)) return;

            ThinkgatePermission perm = new ThinkgatePermission(permissionID, permissionName, description);
            perm.UpdatePermission();
            LoadPermissions();

        }

        protected void rgPermissions_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            ThinkgatePermissionsCollection permissions = new ThinkgatePermissionsCollection();
            permissions.GetPermissionsCollection(PermissionCollectionTypes.All, 1);
            rgPermissions.DataSource = permissions.Permissions;
        }

        protected void LoadUserRolesForEditing(Guid selectedUserID)
        {
            ThinkgateUser user = ThinkgateUser.GetThinkgateUserByID(selectedUserID);
            DataTable table = user.GetUserRolesForEditing();
            rgUserRolesForEditing.DataSource = table;
            rgUserRolesForEditing.DataBind();
        }

        protected void rgUserRolesForEditing_UpdateCommand(Object source, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem == null) return;

            //Get the new values:
            Hashtable newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
            string roleName = newValues["RoleName"] == null ? string.Empty : newValues["RoleName"].ToString();
            bool hasRole = newValues["HasRole"] != null && DataIntegrity.ConvertToBool(newValues["HasRole"]);
            if (String.IsNullOrEmpty(roleName)) return;

            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            _user = sessionObject.SelectedUser;
            if (hasRole)
            {
                Roles.AddUserToRole(_user.UserName, roleName);
                _user.Roles.Add(new ThinkgateRole(roleName));
                lblResultMessage.Text = string.Format("User {0} was added to role {1}.", _user.UserName, roleName);
            }
            else
            {
                Roles.RemoveUserFromRole(_user.UserName, roleName);
                _user.Roles.Remove(new ThinkgateRole(roleName));
                lblResultMessage.Text = string.Format("User {0} was removed from role {1}.", _user.UserName, roleName);
            }

            sessionObject.UpdateUserObject(_user);
        }

        protected void rgUserRolesForEditing_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            _user = sessionObject.SelectedUser;
            if (_user == null) return;
            ThinkgateUser user = ThinkgateUser.GetThinkgateUserByID(_user.UserId);
            DataTable table = user.GetUserRolesForEditing();
            rgUserRolesForEditing.DataSource = table;
        }

        protected void LoadUserSchoolsForEditing(Guid selectedUserID)
        {
            ThinkgateUser user = ThinkgateUser.GetThinkgateUserByID(selectedUserID);
            DataTable table = user.GetUserSchoolsForEditing();
            rgUserSchoolsForEditing.DataSource = table;
            rgUserSchoolsForEditing.DataBind();
        }

        protected void rgUserSchoolsForEditing_UpdateCommand(Object source, GridCommandEventArgs e)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            _user = sessionObject.SelectedUser;
            if (_user == null) return;

            var editedItem = e.Item as GridEditableItem;
            if (editedItem == null) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
            int? schoolID = DataIntegrity.ConvertToNullableInt(editedItem.GetDataKeyValue("ID").ToString());
            if (schoolID == null) return;

            bool hasSchool = newValues["HasSchool"] != null && DataIntegrity.ConvertToBool(newValues["HasSchool"]);

            ThinkgateSchool school = new ThinkgateSchool((int)schoolID, false);

            if (hasSchool)
            {
                _user.addSchool(school);
                lblResultMessage.Text = string.Format("User {0} was added to school {1}.", _user.UserName, school.Name);
            }
            else
            {
                _user.removeSchool(school);
                lblResultMessage.Text = string.Format("User {0} was removed from school {1}.", _user.UserName, school.Name);
            }
            sessionObject.UpdateUserObject(_user);

        }

        protected void rgUserSchoolsForEditing_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            _user = sessionObject.SelectedUser;

            if (_user == null) return;
            ThinkgateUser user = ThinkgateUser.GetThinkgateUserByID(_user.UserId);
            DataTable table = user.GetUserSchoolsForEditing();
            rgUserSchoolsForEditing.DataSource = table;
        }

        protected void LoadUserPermissionsForEditing(Guid selectedUserID)
        {
            ThinkgateUser user = ThinkgateUser.GetThinkgateUserByID(selectedUserID);
            DataTable table = user.GetUserPermissionsForEditing();

            rtlUserPermissionsForEditing.DataSource = table;
            rtlUserPermissionsForEditing.DataBind();
        }


        protected void rgUserPermissionsForEditing_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                RadComboBox rcb = (RadComboBox)item["PermissionLevel"].FindControl("rcbPermissionLevel");
                try
                {
                    rcb.SelectedValue = "Johnson";
                }
                catch (Exception)
                {
                    rcb.Items.Add(new RadComboBoxItem("Johnson", "Johnson"));
                    rcb.SelectedValue = "Johnson";
                }
            }
        }
        protected void rtlUserPermissionsForEditing_UpdateCommand(Object source, TreeListCommandEventArgs e)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            _user = sessionObject.SelectedUser;
            if (_user == null) return;

            var editedItem = e.Item as TreeListDataItem;
            if (editedItem == null) return;

            //Get the new values:
            var newValues = new Hashtable();
            e.Item.OwnerTreeList.ExtractValuesFromItem(newValues, editedItem, true);
            Guid permissionID = new Guid();
            Guid.TryParse(newValues["PermissionId"].ToString(), out permissionID);
            if (permissionID == default(Guid)) return;

            string permissionName = newValues["PermissionName"] == null ? string.Empty : newValues["PermissionName"].ToString();
            int permissionLevel = newValues["PermissionLevelValue"] == null ? 0 : DataIntegrity.ConvertToInt(newValues["PermissionLevelValue"].ToString());
            bool hasPermission = newValues["HasPermission"] != null && DataIntegrity.ConvertToBool(newValues["HasPermission"]);

            ThinkgatePermission permission = new ThinkgatePermission(permissionID);

            if (hasPermission)
            {
                permission.PermissionLevel = (ThinkgatePermission.PermissionLevelValues)Enum.ToObject(typeof(ThinkgatePermission.PermissionLevelValues), permissionLevel);
                _user.addPermission(permission);
                lblResultMessage.Text = string.Format("Permission {0} was added to user {1}.", permissionName, _user.UserName);
                foreach (TreeListDataItem item in editedItem.ChildItems)
                {
                    Guid.TryParse(item.GetDataKeyValue("PermissionId").ToString(), out permissionID);
                    if (permissionID == default(Guid)) return;
                    permission = new ThinkgatePermission(permissionID);
                    permission.PermissionLevel = (ThinkgatePermission.PermissionLevelValues)Enum.ToObject(typeof(ThinkgatePermission.PermissionLevelValues), permissionLevel);

                    _user.addPermission(permission);
                }
            }
            else
            {
                _user.removePermission(permission);
                lblResultMessage.Text = string.Format("Permission {0} was revoked from user {1}.", permissionName, _user.UserName);
                foreach (TreeListDataItem item in editedItem.ChildItems)
                {
                    Guid.TryParse(item.GetDataKeyValue("PermissionId").ToString(), out permissionID);
                    if (permissionID == default(Guid)) return;
                    permission = new ThinkgatePermission(permissionID);

                    _user.removePermission(permission);
                }
            }
            sessionObject.UpdateUserObject(_user);
        }

        protected void rtlUserPermissionsForEditing_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            _user = sessionObject.SelectedUser;

            if (_user == null) return;
            ThinkgateUser user = ThinkgateUser.GetThinkgateUserByID(_user.UserId);
            DataTable table = user.GetUserPermissionsForEditing();
            rtlUserPermissionsForEditing.DataSource = table;
        }

        protected void GetUser(string username)
        {
            _user = new ThinkgateUser(Membership.GetUser(username));
        }

        protected void btnSelectUser_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Username.Text)) return;
            GetUser(Username.Text);
            if (_user == null) return;
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            sessionObject.SelectedUser = _user;
            Session["SessionObject"] = sessionObject;

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
            LoadUserRolesForEditing(_user.UserId);
            LoadUserSchoolsForEditing(_user.UserId);
            LoadUserPermissionsForEditing(_user.UserId);

        }

        protected void btnSubmitAccountChanges_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Username.Text.ToString())) return;
            GetUser(Username.Text);
            if (_user == null) return;
            _user.Email = (String.IsNullOrEmpty(txtEmailAddress.Text) ? _user.Email : txtEmailAddress.Text);
            if (!_user.IsApproved && cboIsApproved.Checked)
            {
                //TODO Wire Up the You Are Approved email notification
            }
            _user.IsApproved = cboIsApproved.Checked;
            Membership.UpdateUser(_user);
            if (_user.IsLockedOut && !cboLockedOut.Checked) _user.UnlockUser();

            lblResultMessage.Text = "User Updated.";

            if (cboResetPassword.Checked)
            {
                string newPW = newPassword.Value.ToString();
                try
                {
                    // TODO: We need to check the password validity before attempting to set it because otherwise, the reset password will be successful but the change won't and the user won't be able to log in
                    if (_user.ChangePassword(_user.ResetPassword(), newPW))
                    {
                        cboResetPassword.Checked = false;
                        lblResultMessage.Text = lblResultMessage.Text + " Password updated.";
                    }
                    else
                    {
                        lblResultMessage.Text = lblResultMessage.Text +
                                                " The password you provided is invalid. It must be seven characters long and have at least one non-alphanumeric character.";
                    }
                }
                catch (Exception)
                {
                    lblResultMessage.Text = lblResultMessage.Text +
                                                " The password you provided is invalid. It must be seven characters long and have at least one non-alphanumeric character.";
                }
                //_user.ChangePassword(_user.GetPassword(), AppSettings.DefaultPassword);
                //_user.ResetPassword(AppSettings.DefaultPassword);
                //_user.ChangePasswordQuestionAndAnswer(AppSettings.DefaultPassword, "abc", "def");
                //TODO Wire Up the email notification
            }
        }
    }
}