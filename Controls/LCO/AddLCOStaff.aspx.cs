using System;
using Thinkgate.Classes;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using System.Web.UI;
using Thinkgate.Base.DataAccess;
using Standpoint.Core.Utilities;
using System.Data;
using System.Data.SqlClient;
using Thinkgate.Domain.Classes;

namespace Thinkgate.Controls.LCO
{
    using Telerik.Web.UI;
    using Thinkgate.Core.Extensions;

    public partial class AddLCOStaff : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserTypeDropdown();
                LoadRegionDropDown();
                LoadLEADropDown();
                LoadSectionDropDown();
            }

            if (Request.Form["__EVENTTARGET"] == "RadButtonOk")
            {
                AddNewLcoStaff(this, new EventArgs());
            }
        }

        private void LoadLEADropDown()
        {
            cmbLEA.Items.Clear();

            foreach (DataRow r in Base.Classes.LCO.LoadLEAs().Rows)
            {
                RadComboBoxItem imcItem = new RadComboBoxItem();
                imcItem.Text = r["LEAName"].ToString();
                imcItem.Value = r["LEAID"].ToString();
                cmbLEA.Items.Add(imcItem);
            }
        }

        private void LoadRegionDropDown()
        {
            cmbRegion.Items.Clear();

            foreach (DataRow r in Base.Classes.LCO.LoadRegions().Rows)
            {
                RadComboBoxItem regionItem = new RadComboBoxItem();
                regionItem.Text = r["RegionName"].ToString();
                regionItem.Value = r["RegionID"].ToString();
                cmbRegion.Items.Add(regionItem);
            }
        }

        private void LoadUserTypeDropdown()
        {
            userTypeDropdown.Items.Clear();

            foreach (DataRow r in Base.Classes.LCO.LoadLCORoles().Rows)
            {
                RadComboBoxItem userItem = new RadComboBoxItem();
                userItem.Text = r["RoleName"].ToString();
                userItem.Value = r["RoleID"].ToString();
                userTypeDropdown.Items.Add(userItem);
            }
        }

        private void LoadSectionDropDown()
        {
            cmbSection.Items.Clear();

            foreach (DataRow r in Base.Classes.LCO.LoadProgramAreas().Rows)
            {
                RadComboBoxItem sectionItem = new RadComboBoxItem();
                cmbSection.Items.Add(sectionItem);
                CheckBox chk = (CheckBox)sectionItem.FindControl("chkPA");
                chk.Text = r["ProgramAreaName"].ToString();
                sectionItem.Value = r["ProgramID"].ToString();
            }
        }

        protected void AddNewLcoStaff(object sender, EventArgs e)
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
            var schoolIDTable = new dtGeneric_Int();
            var userTypeIDTable = new dtGeneric_String();
            var sectionChiefTable = new dtGeneric_Int();

            staffIdentificationTable.Rows.Add("FirstName", firstName.Text);
            staffIdentificationTable.Rows.Add("MiddleName", middleName.Text);
            staffIdentificationTable.Rows.Add("LastName", lastName.Text);
            staffIdentificationTable.Rows.Add("Email", email.Text);
            staffIdentificationTable.Rows.Add("LoginID", loginID.Text);
            staffIdentificationTable.Rows.Add("Restrictions", restrictionsDropdown.Text);
            
            schoolIDTable.Add(0);
            userTypeIDTable.Add(userTypeDropdown.SelectedItem.Text);
	        var userSyncRoles = userTypeDropdown.SelectedItem.Value;

            foreach (RadComboBoxItem item in cmbSection.Items)
            {
                CheckBox itemCheckbox = (CheckBox)item.FindControl("chkPA");

                if (itemCheckbox != null && itemCheckbox.Checked)
                {
                    sectionChiefTable.Add(DataIntegrity.ConvertToInt(item.Value));
                }
            }

            SqlParameterCollection parms = new SqlCommand().Parameters;
            parms.AddWithValue("ApplicationName", AppSettings.ApplicationName);
            parms.AddWithValue("UserName", loginID.Text);
            parms.AddWithValue("FirstName", firstName.Text);
            parms.AddWithValue("MiddleName", middleName.Text);
            parms.AddWithValue("LastName", lastName.Text);
            parms.AddWithValue("Password", DistrictParms.LoadDistrictParms().DefaultPasswordEncrypted);
            parms.AddWithValue("PasswordSalt", DistrictParms.LoadDistrictParms().DefaultPasswordEncryptedSalt);
            parms.AddWithValue("Email", email.Text);
            parms.Add(ThinkgateDataAccess.GetParmFromTable(userTypeIDTable.ToSql(), "Roles"));
            parms.Add(ThinkgateDataAccess.GetParmFromTable(schoolIDTable.ToSql(), "Schools"));
            parms.AddWithValue("PrimarySchool", DataIntegrity.ConvertToInt(0));
            parms.AddWithValue("PrimaryUser", userTypeDropdown.SelectedItem.Text);
            parms.AddWithValue("TeacherID", string.Empty);

            var drNewStaffUserPage = ThinkgateDataAccess.FetchDataRow(AppSettings.ConnectionString,
                                                                      Thinkgate.Base.Classes.Data.StoredProcedures.ASPNET_TG_SECURITY_USER_CREATE_USER,
                                                                      System.Data.CommandType.StoredProcedure,
                                                                      parms,
                                                                      SessionObject.GlobalInputs);
            /*
             * Extract UserPage ID out of recordset and put in hidden field so that when we return to the client side, we
             * can offer user (through javascript) the opportunity to bring up Staff Object Page with new staff in it.
            */

            int leaID = cmbLEA.SelectedItem == null ? 0 : DataIntegrity.ConvertToInt(cmbLEA.SelectedItem.Value);
            int regionID = cmbRegion.SelectedItem == null ? 0 : DataIntegrity.ConvertToInt(cmbRegion.SelectedItem.Value);
            
            Base.Classes.LCO.AddLCOMapping(DataIntegrity.ConvertToInt(drNewStaffUserPage["UserPage"].ToString()), leaID, regionID, sectionChiefTable.ToSql());

            if (drNewStaffUserPage != null) 
            {
                hdnNewStaffIDEncrypted.Value = Standpoint.Core.Classes.Encryption.EncryptString(drNewStaffUserPage["UserPage"].ToString());
                KenticoBusiness.AddUserAndRoles(loginID.Text);
				//Dan - UserSync - Queue a UserSync Message here!
                //TODO: Michael Rue - complete user sync functionality
				//UserSyncHelperFactory.GetMsmqHelper().AddOrUpdateUser(loginID.Text, loginID.Text, null, email.Text, userSyncRoles);
            }

            ScriptManager.RegisterStartupScript(this, typeof(AddLCOStaff), "AddedLCOStaff", "autoSizeWindow();", true);

            resultPanel.Visible = true;
            addPanel.Visible = false;
            lblResultMessage.Text = "Staff successfully added!";
        }

        public void userTypeDropdown_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            switch (((RadComboBox)sender).SelectedItem.Text.ToLower().Trim())
            {
                case "imc":
                    divLEA.Visible = true;
                    divRegion.Visible = false;
                    divSection.Visible = false;
                    break;
                case "regionalcoordinator":
                    divLEA.Visible = false;
                    divRegion.Visible = true;
                    divSection.Visible = false;
                    break;
                case "sectionchief":
                    divLEA.Visible = false;
                    divRegion.Visible = false;
                    divSection.Visible = true;
                    break;
                default:
                    break;
            }
        }

    }
}
