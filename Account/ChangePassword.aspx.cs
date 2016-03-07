using System;
using System.Web.Security;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Text.RegularExpressions;

namespace Thinkgate.Account
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        private SessionObject _sessionObj;
        private string _userName;
        private DistrictParms _dParms;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null || User == null)
            {
                Services.Service2.KillSession();
            }
            _sessionObj = (SessionObject)Session["SessionObject"];
            _dParms = DistrictParms.LoadDistrictParms();
            if (_sessionObj.LoggedInUser.IsSuperAdmin || _dParms.PasswordConfigurationRequired == "No")
                h1MinimumPassword.Visible = false;
            else if (_dParms.PasswordConfigurationRequired == "Yes")
                displayMsgChild.InnerHtml = MinimumPasswordRequirementHelper.GetMinumumPasswordRequirementMsg();

            _userName = _sessionObj.LoggedInUser.UserName;

        }

        private void DisplayError(string err)
        {
            txtMessages.Text = err;
            txtMessages.Visible = true;
        }

        protected void btnSubmitPasswordChanges_Click(object sender, EventArgs e)
        {
            txtMessages.Visible = false;
            dvSuccess.Visible = false;

            if (String.IsNullOrEmpty(txtOldPassword.Text))
            {
                DisplayError("Please enter your old password");
                return;
            }

            if (String.IsNullOrEmpty(txtNewPassword.Text))
            {
                DisplayError("Please enter a new password");
                return;
            }
            else
            {
                String passwordConfReq;
                if (_sessionObj.LoggedInUser.IsSuperAdmin)
                    passwordConfReq = "No";
                else
                    passwordConfReq = _dParms.PasswordConfigurationRequired;
                if (passwordConfReq == "Yes")
                {
                    var regx = new Regex(_dParms.PasswordFormatReg);
                    if (!regx.IsMatch(txtNewPassword.Text))
                    {
                        DisplayError(_dParms.PasswordValidationMsg);
                        return;
                    }
                }
                else if (txtNewPassword.Text.Trim().Length < 6 || txtNewPassword.Text.Trim().Length > 128)
                {
                    DisplayError("New password cannot be less than 6 and greater than 128 in size");
                    return;
                }
            }
            if (String.IsNullOrEmpty(txtConfirmNewPassword.Text))
            {
                DisplayError("Please confirm your new password");
                return;
            }
            if (!txtConfirmNewPassword.Text.Equals(txtNewPassword.Text))
            {
                DisplayError("Password and confirmation do not match");
                return;
            }
            if (txtOldPassword.Text.Trim().Equals(txtNewPassword.Text.Trim()))
            {
                DisplayError("New password cannot be same as old password");
                return;
            }

            if (Membership.ValidateUser(_userName, txtOldPassword.Text))
            {
                MembershipUser user = Membership.GetUser(_userName);
                if (user == null)
                {
                    Services.Service2.KillSession();
                }
                else
                {
                    if (user.ChangePassword(txtOldPassword.Text, txtNewPassword.Text))
                    {
                        // PBI:1989 : Sync passwords between E3 and Legacy applications
                        string encryptedPassword =
                            LegacyPwdEncryption.EncryptLegacyPassword(txtNewPassword.Text);
                        int userPage = Convert.ToInt32(_sessionObj.GlobalInputs[1].Value);
                        ThinkgateUser.ChangedLegacyPassword(userPage, encryptedPassword);

                        if (user.ProviderUserKey != null)
                            ThinkgateUser.SetRestriction((Guid)user.ProviderUserKey, "None");
                        FormsAuthentication.SetAuthCookie(_userName, true);
                        dvSuccess.Visible = true;

                        //Dan - UserSync - Queue a UserSync Message here!
                        //TODO: Michael Rue - complete user sync functionality
                        //UserSyncHelperFactory.GetMsmqHelper().UpdateUser(user.UserName, txtNewPassword.Text);

                        return;
                    }
                }

                DisplayError("There was an error updating your acount. Please contact an administrator.");
            }
            else
            {
                DisplayError("Old password is not correct.");
            }
        }

        protected void hlSuccess_Click(object sender, EventArgs e)
        {
            FormsAuthentication.RedirectFromLoginPage(_userName, false);
        }


    }
}
