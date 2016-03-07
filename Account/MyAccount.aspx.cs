using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate.Account
{
    public partial class MyAccount : BasePage
    {
        private bool _permissionToEditPhoto;
        private bool _permissionToEditEmail;
        protected bool PermissionToChgPwd = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            /*********************************************
             * Determine whether user has permissions to 
             * edit the following sections:
             *      Email Addresss
             *      Photo
             *      Password
             ********************************************/
            _permissionToEditEmail = UserHasPermission(Permission.Hyperlink_Edit_Email);
            hlEditEmailAddr.Visible = _permissionToEditEmail;

            _permissionToEditPhoto = UserHasPermission(Permission.Hyperlink_Edit_Photo);
            hlEditPhoto.Visible = _permissionToEditPhoto;

            PermissionToChgPwd = UserHasPermission(Permission.Change_Password_MyAccount);

            if (!IsPostBack)
            {
                hdnPasswordFormatReg.Value = DistrictParms.PasswordFormatReg;
                hdnPasswordConfReq.Value = DistrictParms.PasswordConfigurationRequired;
                hdnPasswordValidationMsg.Value = DistrictParms.PasswordValidationMsg;

                Page.Title = "My Account";
                /***********************************************
                 * Populate our text boxes and labels from the 
                 * SessionObject.LoggedInUser object
                 * ********************************************/

                var oUser = SessionObject.LoggedInUser;
                lblName.InnerText = oUser.UserFullName;
                lblUserID.InnerText = oUser.UserName;

                //This code should probably be placed into a private method
                if ((SessionObject.LoggedInUser.IsSuperAdmin) || (DistrictParms.PasswordConfigurationRequired == "No"))
                {
                    hdnPasswordFormatReg.Value = "No";
                    h1MinimumPassword.Visible = false;
                }
                else
                {
                    displayMsgChild.InnerHtml = MinimumPasswordRequirementHelper.GetMinumumPasswordRequirementMsg();
                }

                SetRoleDispayText();

                lblEmailAddr.Text = oUser.Email;
                rtbEmailAddr.Text = oUser.Email;

                hdnPhotoFilename.Value = "";
                hdnPhotoFilename.Attributes.Add("initialValue", hdnPhotoFilename.Value);
                /**********************************************
                 * search for and wire in User's profile image.  
                 * If image does not exist or cannot be found,
                 * then go with a default image.
                 *********************************************/
                if (!string.IsNullOrEmpty(oUser.ImageFilename) &&
                    File.Exists(Server.MapPath(AppSettings.ProfileImageUserWebPath + "/" + oUser.ImageFilename)))
                {
                    imgPhoto.Src = AppSettings.ProfileImageUserWebPath + "/" + oUser.ImageFilename;
                }
                else
                {
                    imgPhoto.Src = "/Images/person.png";
                }

                /**********************************************
                 * Provide configuration for our Telerik Rad 
                 * Upload control with values from AppSettings.
                 *********************************************/
                ruUserImage.MaxFileSize = AppSettings.ProfileImageMaxFileSize;
                ruUserImage.AllowedFileExtensions = AppSettings.ProfileImageAllowedFileTypes.Split(',');
                ruUserImage.TargetPhysicalFolder = Server.MapPath(AppSettings.ProfileImageUserWebPath);

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Request.Form["__EVENTTARGET"]))
                {
                    lblUpdateResults.Text = "Update was successful.";
                    switch (Request.Form["__EVENTTARGET"])
                    {
                        case "rbEmailAddrSave":

                            /***********************************************
                             * If any reason email update should not occur,
                             * set message and break out of case statement.
                             **********************************************/

                            if (!_permissionToEditEmail)
                            {
                                lblUpdateResults.Text = "User does not have permission to edit email address.";
                                break;
                            }

                            if (!Standpoint.Core.Utilities.Validations.IsValidEmail(rtbEmailAddr.Text))
                            {
                                lblUpdateResults.Text = "Please enter a valid email address.";
                                break;
                            }

                            /**********************************************
                             * Update the email addr.
                             *********************************************/

                            UpdateEmailAddr();
                            break;

                        case "rbPasswordSave":

                            /***********************************************
                             * If any reason photo update should not occur,
                             * set message and break out of case statement.
                             **********************************************/
                            if (!PermissionToChgPwd) lblUpdateResults.Text = "User does not have permission to change password.";
                            else UpdatePassword();
                            break;

                        case "rbPhotoSave":

                            /***********************************************
                             * If any reason photo update should not occur,
                             * set message and break out of case statement.
                             **********************************************/
                            if (!_permissionToEditPhoto) lblUpdateResults.Text = "User does not have permission to change photo.";
                            else BtnUploadPhotoClick(this, new EventArgs());
                            break;
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(MyAccount), "roundTripProcessing", "roundTripProcessing();", true);
                }
            }

        }

        private void SetRoleDispayText()
        {
            lblUserRole.InnerText = string.Join(
                ",",
                SessionObject.LoggedInUser.Roles.Select(select => select.RoleName).ToArray());
        }

        protected void BtnUploadPhotoClick(object sender, EventArgs e)
        {
            if (ruUserImage.UploadedFiles.Count > 0)
            {
                var newFile = ruUserImage.UploadedFiles[0];
                /* is the file name of the uploaded file 50 characters or less?  
                    * The database field that holds the file name is 50 chars.
                    * */
                if (newFile.ContentLength > AppSettings.ProfileImageMaxFileSize)
                {
                    lblUpdateResults.Text = "The file "
                                            + newFile.FileName.Substring(
                                                newFile.FileName.LastIndexOf(@"\", StringComparison.CurrentCulture) + 1)
                                            + " is too big. The maximum size limit is "
                                            + AppSettings.ProfileImageMaxFileSize.ToString("0,000")
                                            + " bytes.  Please select another file.";
                }
                else
                {
                    if (newFile.GetName().Length <= AppSettings.ProfileImageMaxFileNameLength)
                    {
                        var uploadFolder = Server.MapPath(AppSettings.ProfileImageUserWebPath);

                        string newFileName;

                        do
                        {
                            newFileName = (Path.GetRandomFileName()).Replace(".", "") + newFile.GetExtension();
                        }
                        while (File.Exists(Path.Combine(uploadFolder, newFileName)));

                        try
                        {
                            newFile.SaveAs(Path.Combine(uploadFolder, newFileName));
                            if (!string.IsNullOrEmpty(hdnPhotoFilename.Value))
                            {
                                var oldFile = Path.Combine(uploadFolder, hdnPhotoFilename.Value);
                                if (File.Exists(oldFile))
                                {
                                    File.Delete(oldFile);
                                }
                            }
                            hdnPhotoFilename.Value = newFileName;
                            var oUser = SessionObject.LoggedInUser;
                            oUser.UpdateUserImageFilename(newFileName);
                            imgPhoto.Src = AppSettings.ProfileImageUserWebPath + "/" + newFileName;
                        }
                        catch
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

        protected void UpdateEmailAddr()
        {
            var oUser = SessionObject.LoggedInUser;
            var successful = oUser.UpdateUserEmailAddr(rtbEmailAddr.Text);
            if (successful)
            {
                lblEmailAddr.Text = rtbEmailAddr.Text;
            }
            else
            {
                lblUpdateResults.Text = "Attempt to update email address was unsuccessful.";
            }
        }

        protected void UpdatePassword()
        {
            var oUser = SessionObject.LoggedInUser;
            string encPwd = LegacyPwdEncryption.EncryptLegacyPassword(rtbNewPwd.Text);
            ThinkgateUser.ChangedLegacyPassword(oUser.Page, encPwd);
            if (!oUser.UpdateUserPassword(rtbOrigPwd.Text, rtbNewPwd.Text))
            {
                lblUpdateResults.Text = "Original password is incorrect.";
            }

            //Dan - UserSync - Queue a UserSync Message here!
            //TODO: Michael Rue - complete user sync functionality
            //UserSyncHelperFactory.GetMsmqHelper().UpdateUser(oUser.UserName, rtbNewPwd.Text);
        }

    }
}