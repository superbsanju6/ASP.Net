using System;
using System.Globalization;
using Thinkgate.Classes;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using System.Web.UI;
using Thinkgate.Base.DataAccess;
using Standpoint.Core.Utilities;
using Thinkgate.Domain.Classes;
using System.Linq;
using Thinkgate.Services.Contracts.UserSyncService;

namespace Thinkgate.Controls.Staff
{
    using Telerik.Web.UI;
	using System.Text;
	using Newtonsoft.Json;

    public partial class AddStaff : BasePage
    {
        private List<Base.Classes.School> _schools;
        private List<ThinkgateRole> _roles;
        public enum Restrictions
        {
            None = 0,
            Revoked = 1,
            LockedOut = 2,
            ChangePassword = 3
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _schools = SchoolMasterList.GetSchoolsAll();
                _roles = ThinkgateRole.GetRolesCollectionForApplication().OrderBy(x => x.RoleHierarchyLevel).ToList();
                LoadSchoolDropdown();
                LoadUserTypeDropdown();
            }

            if (Request.Form["__EVENTTARGET"] == "RadButtonOk")
            {
                AddNewStaff(this, new EventArgs());
            }
        }

        private void LoadSchoolDropdown()
        {
            schoolDropdown.Items.Clear();
            
            //Build school ListBox
            foreach (var school in _schools)
            {
                var schoolListItem = new RadComboBoxItem
                    {
                        Text = school.Name,
                        Value = school.ID.ToString(CultureInfo.InvariantCulture)
                    };

                schoolDropdown.Items.Add(schoolListItem);

                var itemLabel = (Label)schoolListItem.FindControl("schoolLabel");

                if (itemLabel != null)
                {
                    itemLabel.Text = school.Name;
                }
            }

            var findButton = new RadComboBoxItem();
            schoolDropdown.Items.Add(findButton);

            var findButtonCheckbox = (CheckBox)findButton.FindControl("schoolCheckbox");
            if (findButtonCheckbox != null)
            {
                findButtonCheckbox.InputAttributes["style"] = "display:none;";
            }
        }

        private void LoadUserTypeDropdown()
        {
            userTypeDropdown.Items.Clear();
            int maxHierarchyLevel = SessionObject.LoggedInUser.Roles.OrderByDescending(y => y.RoleHierarchyLevel).First().RoleHierarchyLevel;
            
            //Build role ListBox
            foreach (var role in _roles)
            {
                //PLH - 11/12/2012 - Temporary fix to not allow admins to add teachers or teach plus. Also eliminates district superintendent. Currently this causes duplicates to appear after roster loads and causes
                //more issues than it solves
                if (role.RoleName.ToLower() != "teacher" && role.RoleName.ToLower() != "teach plus" && role.RoleName.ToLower() != "district superintendent" && role.RoleName.ToLower() != "student" && role.RoleHierarchyLevel >= maxHierarchyLevel)
                {
                    var userTypeListItem = new RadComboBoxItem {Text = role.RoleName, Value = role.RoleId.ToString()};

                    userTypeDropdown.Items.Add(userTypeListItem);

                    var itemLabel = (Label)userTypeListItem.FindControl("userTypeLabel");

                    if (itemLabel != null)
                    {
                        itemLabel.Text = role.RoleName;
                    }
                }
            }

            var findButton = new RadComboBoxItem();
            userTypeDropdown.Items.Add(findButton);

            var findButtonCheckbox = (CheckBox)findButton.FindControl("userTypeCheckbox");
            if (findButtonCheckbox != null)
            {
                findButtonCheckbox.InputAttributes["style"] = "display:none;";
            }
        }

        protected void AddNewStaff(object sender, EventArgs e)
        {

            var environmentParametersViewModel = new EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            var staffManagement = new StaffManagement(environmentParametersViewModel);

            var doesUserExist = staffManagement.DoesUserExist(loginID.Text);
            staffManagement.Dispose();

            if (doesUserExist)
            {
                var radalertscript = "<script language='javascript'>function f(){radalert('" + string.Format("Cannot Add User, this user already exists.  User: {0}", loginID.Text) + "', 300, 300, 'Duplicate User Detected'); Sys.Application.remove_load(f);}; Sys.Application.add_load(f);</script>";
                Page.ClientScript.RegisterStartupScript(GetType(), "radalert", radalertscript); 
                return;
            }

            var staffIdentificationTable = new dtGeneric_String_String();
            var schoolIdTable = new dtGeneric_Int();
            var userTypeIdTable = new dtGeneric_String();
            string restrictionsValue = string.Empty ;

            staffIdentificationTable.Rows.Add("FirstName", firstName.Text);
            staffIdentificationTable.Rows.Add("MiddleName", middleName.Text);
            staffIdentificationTable.Rows.Add("LastName", lastName.Text);
            staffIdentificationTable.Rows.Add("Email", email.Text);
            staffIdentificationTable.Rows.Add("LoginID", loginID.Text);
            
            switch (DataIntegrity.ConvertToInt(restrictionsDropdown.SelectedIndex))     // using index is not best way to accomplish this... but easy to fix in the future if we need to reorder dropdown
            {
                case (int)Restrictions.None:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "false");
                    staffIdentificationTable.Rows.Add("IsApproved", "true");
                    staffIdentificationTable.Rows.Add("Restrictions", "None");
                    restrictionsValue = "None";
                    break;
                case (int)Restrictions.Revoked:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "false");
                    staffIdentificationTable.Rows.Add("IsApproved", "false");
                    staffIdentificationTable.Rows.Add("Restrictions", "None");
                    restrictionsValue = "None";
                    break;
                case (int)Restrictions.LockedOut:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "true");
                    staffIdentificationTable.Rows.Add("IsApproved", "true");
                    staffIdentificationTable.Rows.Add("Restrictions", "None");
                    restrictionsValue = "None";
                    break;
                case (int)Restrictions.ChangePassword:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "false");
                    staffIdentificationTable.Rows.Add("IsApproved", "true");
                    staffIdentificationTable.Rows.Add("Restrictions", ThinkgateUser.ChangePasswordRestrictionValue);
                    restrictionsValue = ThinkgateUser.ChangePasswordRestrictionValue.ToString(CultureInfo.InvariantCulture);
                    break;
            }
            foreach (RadComboBoxItem item in schoolDropdown.Items)
            {
                var itemCheckbox = (CheckBox)item.FindControl("schoolCheckbox");
                var itemLabel = (Label)item.FindControl("schoolLabel");

                if (itemCheckbox != null && itemCheckbox.Checked && itemLabel.Text != @"All" && itemLabel.Text.ToLower().IndexOf("<img", StringComparison.Ordinal) == -1)
                {
                    schoolIdTable.Add(DataIntegrity.ConvertToInt(item.Value));
                }
            }
			StringBuilder userSyncRoles = new StringBuilder();
            foreach (RadComboBoxItem item in userTypeDropdown.Items)
            {
                var itemCheckbox = (CheckBox)item.FindControl("userTypeCheckbox");
                var itemLabel = (Label)item.FindControl("userTypeLabel");

                if (itemCheckbox != null && itemCheckbox.Checked && itemLabel.Text != @"All" && itemLabel.Text.ToLower().IndexOf("<img", StringComparison.Ordinal) == -1)
                {
                    userTypeIdTable.Add(item.Text);
					userSyncRoles.Append(itemLabel.Text);
                }
            }

            /*  Create the user record  */
            /* Validate Results - if error, give message and go back to user */
            SqlParameterCollection parms = new SqlCommand().Parameters;
            parms.AddWithValue("ApplicationName", AppSettings.ApplicationName);
            parms.AddWithValue("UserName", loginID.Text);
            parms.AddWithValue("FirstName", firstName.Text);
            parms.AddWithValue("MiddleName", middleName.Text);
            parms.AddWithValue("LastName", lastName.Text);
            parms.AddWithValue("Password", DistrictParms.LoadDistrictParms().DefaultPasswordEncrypted);
            parms.AddWithValue("PasswordSalt", DistrictParms.LoadDistrictParms().DefaultPasswordEncryptedSalt);
            parms.AddWithValue("Email", email.Text);
            parms.Add(ThinkgateDataAccess.GetParmFromTable(userTypeIdTable.ToSql(), "Roles"));
            parms.Add(ThinkgateDataAccess.GetParmFromTable(schoolIdTable.ToSql(), "Schools"));
            parms.AddWithValue("PrimarySchool", DataIntegrity.ConvertToInt(cmbPrimarySchool.SelectedItem.Value));
            parms.AddWithValue("PrimaryUser", cmbPrimaryUser.SelectedItem.Text);
            parms.AddWithValue("TeacherID", string.Empty);
            parms.AddWithValue("Restrictions", restrictionsValue);   

            var drNewStaffUserPage = ThinkgateDataAccess.FetchDataRow(AppSettings.ConnectionString,
                                                                      Base.Classes.Data.StoredProcedures.ASPNET_TG_SECURITY_USER_CREATE_USER,
                                                                      System.Data.CommandType.StoredProcedure,
                                                                      parms,
                                                                      SessionObject.GlobalInputs);
            /*
             * Extract UserPage ID out of recordset and put in hidden field so that when we return to the client side, we
             * can offer user (through javascript) the opportunity to bring up Staff Object Page with new staff in it.
            */
            if (drNewStaffUserPage != null)
            {
                hdnNewStaffIDEncrypted.Value = Standpoint.Core.Classes.Encryption.EncryptString(drNewStaffUserPage["UserPage"].ToString());
                KenticoBusiness.AddUserAndRoles(loginID.Text);
            }

			//Dan - UserSync - Queue a UserSync Message here!
            //TODO: Michael Rue - complete user sync functionality
			//UserSyncHelperFactory.GetMsmqHelper().AddOrUpdateUser(loginID.Text, loginID.Text, null, email.Text, JsonConvert.SerializeObject(userSyncRoles));

            ScriptManager.RegisterStartupScript(this, typeof(AddStaff), "AddedStaff", "autoSizeWindow();", true);

            resultPanel.Visible = true;
            addPanel.Visible = false;
            lblResultMessage.Text = @"Staff successfully added!";
        }
		
    }
}
