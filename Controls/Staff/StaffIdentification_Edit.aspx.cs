using System;
using System.Data;
using System.Text;
using CMS.GlobalHelper;
using Newtonsoft.Json;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using System.IO;
using System.Web.UI;
using System.Linq;
using Thinkgate.Base.DataAccess;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Staff
{
    public partial class StaffIdentification_Edit : BasePage
	{
        private Base.Classes.Staff _selectedStaff;
        private List<Base.Classes.School> _schools;
        private List<ThinkgateRole> _roles;
        private UploadedFile newFile;
        private bool _fromTeacherPage;
        private SessionObject _sessionObject;
        
        public enum Restrictions
        {
            None = 0,
            Revoked = 1,
            LockedOut = 2,
            ChangePassword = 3
        }

        private enum PrimaryType
        {
            School,
            UserType
        }

		protected void Page_Load(object sender, EventArgs e)
		{
            /********************************************************************************************************
             * We can get to this page from two different app locations/contexts.  You either get here from the 
             * Staff Object page, or the Teacher Object Page.  if/when an update is made, then it will be important
             * to know what needs refreshing in cache, a teacher object or a staff oject.
             * *****************************************************************************************************/
            _fromTeacherPage = (Request.QueryString["type"] != null && Request.QueryString["type"].ToLower() == "teacher");

            LoadStaff();

            if(!IsPostBack)
            {
                // set parms values in hidden field.
                var DParmsNew = DistrictParms.LoadDistrictParms();
                hdnPasswordFormatReg.Value = DParmsNew.PasswordFormatReg;
                hdnPasswordConfReq.Value = DParmsNew.PasswordConfigurationRequired;
                hdnPasswordValidationMsg.Value = DParmsNew.PasswordValidationMsg;
                _sessionObject = (SessionObject)Session["SessionObject"];

                if ((_sessionObject.LoggedInUser.IsSuperAdmin) || (DParmsNew.PasswordConfigurationRequired == "No"))
                {
                    hdnPasswordConfReq.Value = "No";
                    h1MinimumPassword.Visible = false;
                }
                else
                {
                    displayMsgChild.InnerHtml = MinimumPasswordRequirementHelper.GetMinumumPasswordRequirementMsg();
                }

                

                //_schools = SchoolMasterList.schoolMaster;
                _schools = SchoolMasterList.GetSchoolsAll();
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                _roles = ThinkgateRole.GetRolesCollectionForApplication(0, sessionObject.LoggedInUser.Roles[0].RolePortalSelection).OrderBy(x => x.RoleHierarchyLevel).ToList();
                firstName.Text = _selectedStaff.FirstName;
                middleName.Text = _selectedStaff.MiddleName;
                lastName.Text = _selectedStaff.LastName;
                email.Text = _selectedStaff.Email;
                loginID.Text = _selectedStaff.LoginID;
                LoadSchoolDropdown();
                LoadUserTypeDropdown();
                LoadPrimary(PrimaryType.School);
                LoadPrimary(PrimaryType.UserType);
                SetRestrictionsDropdown();

                if (!string.IsNullOrEmpty(_selectedStaff.Picture) &&
                    File.Exists(Server.MapPath(AppSettings.ProfileImageUserWebPath + "/" + _selectedStaff.Picture)))
                {
                    imgPhoto.Src = AppSettings.ProfileImageUserWebPath + "/" + _selectedStaff.Picture;
                }
                else
                {
                    imgPhoto.Src = "/Images/person.png";
                }

                //Provide configuration for our Telerik Rad Upload control with values from AppSettings.
                ruUserImage.MaxFileSize = AppSettings.ProfileImageMaxFileSize;
                ruUserImage.AllowedFileExtensions = AppSettings.ProfileImageAllowedFileTypes.Split(',');
                ruUserImage.TemporaryFolder = Server.MapPath(AppSettings.ProfileImageUserWebPath);

            }
		}

        private void LoadStaff()
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _selectedStaff = Base.Classes.Staff.GetStaffByID(DataIntegrity.ConvertToInt(GetDecryptedEntityId(X_ID))); 
            }
        }

        private void LoadPrimary(PrimaryType type)
        {
            RadComboBox cmbType;
            RadComboBox cmbPrimary;
            string chkText = null;
            switch (type)
            {
                case PrimaryType.School:
                    cmbType = schoolDropdown;
                    cmbPrimary = cmbPrimarySchool;
                    chkText = "schoolCheckBox";
                    break;
                case PrimaryType.UserType:
                    cmbType = userTypeDropdown;
                    cmbPrimary = cmbPrimaryUser;
                    chkText = "userTypeCheckbox";
                    break;
                default:
                    cmbType = null;
                    cmbPrimary = null;
                    chkText = null;
                    break;
            }


            cmbPrimary.Items.Clear();
            foreach (RadComboBoxItem item in cmbType.Items)
            {
                if (((CheckBox)item.FindControl(chkText)).Checked)
                {
                    RadComboBoxItem newItem = new RadComboBoxItem(item.Text, item.Value);
                    cmbPrimary.Items.Add(newItem);
                }
            }

            switch(type)
            {
                case PrimaryType.School:
                    if (cmbPrimary.FindItemByValue(_selectedStaff.LegacySchoolID.ToString()) != null)
                    {
                        cmbPrimary.FindItemByValue(_selectedStaff.LegacySchoolID.ToString()).Selected = true;
                    }
                    break;
                case PrimaryType.UserType:
                    if (cmbPrimary.FindItemByText(_selectedStaff.LegacyRole, true) != null)
                    {
                        cmbPrimary.FindItemByText(_selectedStaff.LegacyRole, true).Selected = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void LoadSchoolDropdown()
        {
            schoolDropdown.Items.Clear();
            int schoolDropdownCheckedItemsTotal = 0;

            RadComboBoxItem schoolListItemAll = new RadComboBoxItem();
            //schoolListItemAll.Text = "All";
            //schoolListItemAll.Value = "Multiple";
            //schoolDropdown.Items.Add(schoolListItemAll);

            CheckBox allItemCheckbox = (CheckBox)schoolListItemAll.FindControl("schoolCheckBox");
            //Label allItemLabel = (Label)schoolListItemAll.FindControl("schoolLabel");
            if (_selectedStaff.SchoolList.Count > 1)
            {
                if (allItemCheckbox != null)
                {
                    allItemCheckbox.Checked = true;
                    schoolDropdown.Text = "Multiple";
                }
            }
            //if (allItemLabel != null)
            //{
            //    allItemLabel.Text = "All";
            //}

            //Build school ListBox
            foreach (var school in _schools)
            {
                RadComboBoxItem schoolListItem = new RadComboBoxItem();
                Label schoolLabel = (Label)schoolListItem.FindControl("schoolLabel");
                schoolListItem.Text = school.Name;
                schoolListItem.Value = school.ID.ToString();

                schoolDropdown.Items.Add(schoolListItem);

                CheckBox itemCheckbox = (CheckBox)schoolListItem.FindControl("schoolCheckbox");
                Label itemLabel = (Label)schoolListItem.FindControl("schoolLabel");

                if (itemCheckbox != null)
                {
                    var isStaffSchool = _selectedStaff.SchoolList.Find(s => s.ID == school.ID);
                    if (isStaffSchool != null)
                    {
                        itemCheckbox.Checked = true;
                        schoolListItem.Selected = true;
                        schoolDropdownCheckedItemsTotal++;
                        schoolDropdown.Text = school.Name;
                    }
                }
                if (itemLabel != null)
                {
                    itemLabel.Text = school.Name;
                }
            }

            if (schoolDropdownCheckedItemsTotal == _schools.Count)
            {
                schoolDropdown.Items[0].Checked = true;
                schoolDropdown.Items[0].Selected = true;
            }
            
            if (schoolDropdownCheckedItemsTotal > 1)
            {
                schoolDropdown.ClearSelection();
                schoolDropdown.Text = "Multiple";
            }

            RadComboBoxItem findButton = new RadComboBoxItem();
            schoolDropdown.Items.Add(findButton);

            CheckBox findButtonCheckbox = (CheckBox)findButton.FindControl("schoolCheckbox");
            if (findButtonCheckbox != null)
            {
                findButtonCheckbox.InputAttributes["style"] = "display:none;";
            }
        }

        private void LoadUserTypeDropdown()
        {
            userTypeDropdown.Items.Clear();
            int userTypeDropdownCheckedItemsTotal = 0;

            RadComboBoxItem userTypeListItemAll = new RadComboBoxItem();
            //userTypeListItemAll.Text = "All";
            //userTypeListItemAll.Value = "Multiple";
            //userTypeDropdown.Items.Add(userTypeListItemAll);

            CheckBox allItemCheckbox = (CheckBox)userTypeListItemAll.FindControl("userTypeCheckBox");
            Label allItemLabel = (Label)userTypeListItemAll.FindControl("userTypeLabel");
            if (_selectedStaff.RoleList.Count > 1)
            {
                if (allItemCheckbox != null)
                {
                    allItemCheckbox.Checked = true;
                    userTypeDropdown.Text = "Multiple";
                }
            }
            if (allItemLabel != null)
            {
                allItemLabel.Text = "All";
            }

            int maxHierarchyLevel = SessionObject.LoggedInUser.Roles.OrderByDescending(y => y.RoleHierarchyLevel).First().RoleHierarchyLevel;

            //Build role ListBox
            foreach (var role in _roles)
            {
                //PLH - 11/12/2012 - Eliminates district superintendent from role dropdown. Temporary bandaid.
                if (role.RoleName.ToLower() != "district superintendent" && role.RoleName.ToLower() != "student")
                {
                    RadComboBoxItem userTypeListItem = new RadComboBoxItem();
                    Label userTypeLabel = (Label)userTypeListItem.FindControl("userTypeLabel");
                    
                    // 7715 - test coordinator is not allowed to change permission to district administrator
                    // rollbacked as requested
                    //SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                    //if (sessionObject.LoggedInUser.Roles[0].RoleName.ToLower() == "test coordinator" && role.RoleName.ToLower() == "district administrator")
                    //{
                    //    continue;
                    //}

                    userTypeListItem.Text = role.RoleName;
                    userTypeListItem.Value = role.RoleId.ToString();

                    userTypeDropdown.Items.Add(userTypeListItem);

                    CheckBox itemCheckbox = (CheckBox)userTypeListItem.FindControl("userTypeCheckbox");
                    itemCheckbox.Enabled = role.RoleHierarchyLevel >= maxHierarchyLevel;
                    Label itemLabel = (Label)userTypeListItem.FindControl("userTypeLabel");
                    itemLabel.ForeColor = role.RoleHierarchyLevel >= maxHierarchyLevel ? System.Drawing.Color.Black : System.Drawing.Color.Gray;

                    if (itemCheckbox != null)
                    {
                        var isStaffRole = _selectedStaff.RoleList.Find(r => r.RoleId == role.RoleId);
                        if (isStaffRole != null)
                        {
                            itemCheckbox.Checked = true;
                            userTypeDropdownCheckedItemsTotal++;
                            userTypeListItem.Selected = true;
                            userTypeDropdown.Text = role.RoleName;
                        }
                    }
                    if (itemLabel != null)
                    {
                        itemLabel.Text = role.RoleName;
                    }
                    
                }
            }

            if (userTypeDropdownCheckedItemsTotal == _roles.Count)
            {
                userTypeDropdown.Items[0].Checked = true;
                userTypeDropdown.Items[0].Selected = true;
            }

            if (userTypeDropdownCheckedItemsTotal > 1)
            {
                userTypeDropdown.ClearSelection();
                userTypeDropdown.Text = "Multiple";
            }

            RadComboBoxItem findButton = new RadComboBoxItem();
            userTypeDropdown.Items.Add(findButton);

            CheckBox findButtonCheckbox = (CheckBox)findButton.FindControl("userTypeCheckbox");
            if (findButtonCheckbox != null)
            {
                findButtonCheckbox.InputAttributes["style"] = "display:none;";
            }
        }

        private void SetRestrictionsDropdown()
        {
            // WSH - going to try using the built in api properties to set the 'restriction' dropdown to the user instead of trying to find a way to keep the restrictions field in sync with a lock out (which I think would require a trigger)
            var oUser = ThinkgateUser.GetThinkgateUserByID(_selectedStaff.UserID);
            
            if (!oUser.IsApproved)
            {
                restrictionsDropdown.SelectedIndex = (int) Restrictions.Revoked; // access revoked
            } else if (oUser.IsLockedOut)
            {
                restrictionsDropdown.SelectedIndex = (int) Restrictions.LockedOut; // locked out
            }
            else if (_selectedStaff.Restrictions == ThinkgateUser.ChangePasswordRestrictionValue)
            {
                restrictionsDropdown.SelectedIndex = (int) Restrictions.ChangePassword;
            } else
            {
                restrictionsDropdown.SelectedIndex = (int) Restrictions.None;
            }
        }

        protected void UpdatePassword(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbNewPwd.Text))
            {
            var oUser = ThinkgateUser.GetThinkgateUserByID(_selectedStaff.UserID);
            if (oUser.IsLockedOut) oUser.UnlockUser();      // user cannot be locked out or reset password will throw an error
            var generatedPwd = oUser.ResetPassword();
            if (oUser.UpdateUserPassword(generatedPwd, rtbNewPwd.Text))
            {
                string encPwd = LegacyPwdEncryption.EncryptLegacyPassword(rtbNewPwd.Text);
                ThinkgateUser.ChangedLegacyPassword(oUser.Page, encPwd);
                lblResultMessage.Text = "Password change successful."; 
				
				//Dan - UserSync - Queue a UserSync Message here!
                //TODO: Michael Rue - complete user sync functionality
				//UserSyncHelperFactory.GetMsmqHelper().UpdateUser(oUser.UserName, rtbNewPwd.Text);
            }
            else
            { 
                lblUpdateResults.Text = "Attempt to change password failed.  Please contact your system administrator."; 
            }
        }
            else lblResultMessage.Text = "Attempt to change password failed."; 
        }

        protected void UpdatePhoto(object sender, EventArgs e)
        {
            if (ruUserImage.UploadedFiles.Count > 0)
            {
                newFile = ruUserImage.UploadedFiles[0];
                /* is the file name of the uploaded file 50 characters or less?  
                    * The database field that holds the file name is 50 chars.
                    * */
                if (newFile.ContentLength > AppSettings.ProfileImageMaxFileSize)
                {
                    lblUpdateResults.Text = "The file being imported is too big. Please select another file.";
                }
                else
                {
                    if (newFile.GetName().Length <= AppSettings.ProfileImageMaxFileNameLength)
                    {
                        var uploadFolder = Server.MapPath(AppSettings.ProfileImageUserWebPath);

                        string newFileName;

                        do
                        {
                            newFileName = (Path.GetRandomFileName()).Replace(".", "") +
                                          newFile.GetExtension();
                        } while (System.IO.File.Exists(Path.Combine(uploadFolder, newFileName)));

                        try
                        {
                            newFile.SaveAs(Path.Combine(uploadFolder, newFileName));

                            var oUser = SessionObject.LoggedInUser;
                            var successful = oUser.UpdateUserImageFilename(newFileName);
                            if (successful)
                            {
                                //attempt to delete the old file
                                if (imgPhoto.Src != "/Images/person.png")
                                {
                                    var oldFilePath = Server.MapPath(imgPhoto.Src);
                                    File.Delete(oldFilePath);
                                }
                                oUser.ImageFilename = newFileName;
                                SessionObject.LoggedInUser = oUser;

                                imgPhoto.Src = AppSettings.ProfileImageUserWebPath + "/" + newFileName;
                            }
                            else
                            {
                                lblUpdateResults.Text = "Database error. Please contact system administrator.";
                            }
                        }
                        catch (Exception ex)
                        {
                            lblUpdateResults.Text = "Image Import unsuccessful. Please contact system adminstrator.";
                        }
                    }
                    else
                    {
                        lblUpdateResults.Text = "Image filename length must be 50 characters or less. Please rename the file and try again.";
                    }
                }
            }
            else
            {
                lblUpdateResults.Text = "Image import unsuccessful. No image specified.";
            }
        }

        protected void UpdateStaff(object sender, EventArgs e)
        {
            lblResultMessage.Text = string.Empty;
            var staffIdentificationTable = new dtGeneric_String_String();
            var schoolIDTable = new dtGeneric_Int();
            var userTypeIDTable = new dtGeneric_String();

		    string currLoginID = _selectedStaff.LoginID;
			string newLoginID = loginID.Text;
            string imageFileName = string.Empty;
            if (imgPhoto.Src != string.Empty)
            {
                imageFileName = Path.GetFileName(imgPhoto.Src);
            }
            staffIdentificationTable.Rows.Add("FirstName", firstName.Text);
            staffIdentificationTable.Rows.Add("MiddleName", middleName.Text);
            staffIdentificationTable.Rows.Add("LastName", lastName.Text);
            staffIdentificationTable.Rows.Add("Email", email.Text);
            staffIdentificationTable.Rows.Add("LoginID", loginID.Text);
            staffIdentificationTable.Rows.Add("Image_FileName", imageFileName);
            //staffIdentificationTable.Rows.Add("Restrictions", restrictionsDropdown.Text);
            switch (DataIntegrity.ConvertToInt(restrictionsDropdown.SelectedIndex))     // using index is not best way to accomplish this... but easy to fix in the future if we need to reorder dropdown
            {
                case (int) Restrictions.None:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "false");
                    staffIdentificationTable.Rows.Add("IsApproved", "true");
                    staffIdentificationTable.Rows.Add("Restrictions", "None");
                    break;
                case (int) Restrictions.Revoked:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "false");
                    staffIdentificationTable.Rows.Add("IsApproved", "false");
                    staffIdentificationTable.Rows.Add("Restrictions", "None");
                    break;
                case (int) Restrictions.LockedOut:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "true");
                    staffIdentificationTable.Rows.Add("IsApproved", "true");
                    staffIdentificationTable.Rows.Add("Restrictions", "None");
                    break;
                case (int) Restrictions.ChangePassword:
                    staffIdentificationTable.Rows.Add("IsLockedOut", "false");
                    staffIdentificationTable.Rows.Add("IsApproved", "true");
                    staffIdentificationTable.Rows.Add("Restrictions", ThinkgateUser.ChangePasswordRestrictionValue);
                    break;
            }

            foreach(RadComboBoxItem item in schoolDropdown.Items)
            {
                CheckBox itemCheckbox = (CheckBox)item.FindControl("schoolCheckbox");
                Label itemLabel = (Label)item.FindControl("schoolLabel");

                if (itemCheckbox != null && itemCheckbox.Checked && itemLabel.Text != "All" && itemLabel.Text.IndexOf("<img") == -1)
                {
                    schoolIDTable.Add(DataIntegrity.ConvertToInt(item.Value));
                }
            }

			List<String> userSyncRoles = new List<string>();

            foreach (RadComboBoxItem item in userTypeDropdown.Items)
            {
                CheckBox itemCheckbox = (CheckBox)item.FindControl("userTypeCheckbox");
                Label itemLabel = (Label)item.FindControl("userTypeLabel");

	            if (itemCheckbox != null && itemCheckbox.Checked && itemLabel.Text != "All" &&
	                itemLabel.Text.IndexOf("<img") == -1)
                {
                    userTypeIDTable.Add(item.Value);
					userSyncRoles.Add(itemLabel.Text);
                }
            }

            Base.Classes.Staff.UpdateStaff(staffIdentificationTable, schoolIDTable, userTypeIDTable, _selectedStaff.UserID.ToString(), DataIntegrity.ConvertToInt(cmbPrimarySchool.SelectedItem.Value), cmbPrimaryUser.SelectedItem.Text.ToLower());

            if (_fromTeacherPage) Base.Classes.Cache.Remove("Teacher_" + GetDecryptedEntityId(X_ID));

            Base.Classes.Cache.Remove("Staff_" + GetDecryptedEntityId(X_ID));

			//Dan - UserSync - Queue a UserSync Message here!
			//string usroles = JsonConvert.SerializeObject(userSyncRoles);

            //TODO: Michael Rue - complete user sync functionality
			//UserSyncHelperFactory.GetMsmqHelper().AddOrUpdateUser(currLoginID, newLoginID, null, email.Text, usroles);

	      
            string js = "parent.window.location.reload();";
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "StaffIdentificationEditSaveAndClose", js, true);
        }
	}
}