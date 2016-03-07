using System;
using System.Globalization;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Account
{
    public partial class PwdChgCtrl : UserControl
    {
        private SessionObject _sessionObject;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            DistrictParms dParms = DistrictParms.LoadDistrictParms();
            pwdChgCtrl_hdnPasswordFormatReg.Value = dParms.PasswordFormatReg;
            pwdChgCtrl_hdnPasswordConfReq.Value = dParms.PasswordConfigurationRequired;
            pwdChgCtrl_hdnValidationMsg.Value = dParms.PasswordValidationMsg;
            _sessionObject = (SessionObject)Session["SessionObject"];

            if (Request.Form["__EVENTTARGET"] == "pwdChgCtrl_SubmitBtn")
            {
                UpdatePassword(this, new EventArgs());
            }
            if ((_sessionObject.LoggedInUser.IsSuperAdmin) || (dParms.PasswordConfigurationRequired == "No"))
            {
                pwdChgCtrl_hdnPasswordConfReq.Value = "No";
                h1MinimumPassword.Visible = false;
            }
            else
            {
                displayMsgChild.InnerHtml = MinimumPasswordRequirementHelper.GetMinumumPasswordRequirementMsg();
            }
        }

        private void UpdatePassword(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(Request.QueryString["xID"].ToString(CultureInfo.CurrentCulture)))
            {
                //try
                //{
                int userID = Cryptography.GetDecryptedID(_sessionObject.LoggedInUser.CipherKey);
                var oStaff = Staff.GetStaffByID(userID);
                var oUser = ThinkgateUser.GetThinkgateUserByID(oStaff.UserID);
                if (oUser.IsLockedOut) oUser.UnlockUser();      // user cannot be locked out or reset password will throw an error
                var generatedPwd = oUser.ResetPassword();
                string encPwd = LegacyPwdEncryption.EncryptLegacyPassword(pwdChgCtrl_TextBoxNewPwd.Text);
                ThinkgateUser.ChangedLegacyPassword(oUser.Page, encPwd);
                pwdChgCtrl_LblResultMessage.Text = (!oUser.UpdateUserPassword(generatedPwd, pwdChgCtrl_TextBoxNewPwd.Text)) ? "Attempt to change password failed.  Please contact your system administrator." : "Password change successful.";

                //Dan - UserSync - Queue a UserSync Message here!
                //TODO: Michael Rue - complete user sync functionality
                //UserSyncHelperFactory.GetMsmqHelper().UpdateUser(oUser.UserName, pwdChgCtrl_TextBoxNewPwd.Text);
                //}
                //catch (Exception)
                //{
                //    pwdChgCtrl_LblResultMessage.Text = "Attempt to change password failed.  Please contact your system administrator.";
                //}
            }
            else
            {
                pwdChgCtrl_LblResultMessage.Text = "Attempt to change password failed.  Please contact your system administrator.";
            }

            resultPanel.Visible = true;
            pwdChgCtrl_EditPanel.Visible = false;
        }
    }
}