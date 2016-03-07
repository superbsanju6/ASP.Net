using System;
using System.Net.Mail;
using System.Web.Security;
using Thinkgate.Base.Classes;
using System.ComponentModel;

namespace Thinkgate.Account
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckForReset();
        }

        protected void CheckForReset()
        {
            string clientID = Request["txtClientID"];
            string userName = Request["txtUserName"];

            if (!string.IsNullOrEmpty(clientID) && !string.IsNullOrEmpty(userName))
            {
                MembershipUser user = Membership.GetUser(userName);
                this.Response.Clear();
                if (user == null)
                {
                    this.Response.Write("False");
					this.Response.Write(Environment.NewLine);
					this.Response.Write("Unknown User '" + userName + "'.  Please contact an administrator for assistance.");
                    return;
                }

                if (!Membership.EnablePasswordReset || !DistrictParms.LoadDistrictParms().SelfPasswordReset)
                {
                    FailureText.Text = string.Format("Password resets are not currently allowed.  Please contact an administrator for assistance.");
                    this.Response.Write("False");
					this.Response.Write(Environment.NewLine);
					this.Response.Write(FailureText.Text);
                    return;
                }

                if (!user.IsApproved)
                {
                    FailureText.Text = "This account has been locked out.  Please contact an administrator for assistance.";
                    this.Response.Write("False");
					this.Response.Write(Environment.NewLine);
					this.Response.Write(FailureText.Text);
                    return;
                }

                if (user.IsLockedOut && !user.UnlockUser())
                {
                    FailureText.Text = "This account has been locked out.  Please contact an administrator for assistance.";
                    this.Response.Write("False");
					this.Response.Write(Environment.NewLine);
					this.Response.Write(FailureText.Text);
                    return;
                }

                string generatedPassword = user.ResetPassword();
                bool ret = ThinkgateUser.SetLastPasswordChangedDateToNull(user.UserName, AppSettings.ApplicationName);
                if (string.IsNullOrEmpty(user.Email))
                {
                    // give them the system default password
                    user.ChangePassword(generatedPassword, AppSettings.DefaultPassword);
                    FailureText.Text = string.Format("Your password has been reset. There is no email account on record, please contact an administrator for assistance.");
                }
                else
                {
                    // send them the generated password
                    Thinkgate.Base.Classes.Messaging.SendPasswordResetMessage(user.Email, generatedPassword);
                    FailureText.Text = "An email has been sent with a temporary password to the email account on record. Once you receive it, please login and change your password to something meaningful to you.";
                }
                this.Response.Write("True");
				this.Response.Write(Environment.NewLine);
				this.Response.Write(FailureText.Text);
                this.Response.End();
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // email is required
            if (string.IsNullOrEmpty(Email.Text) && Request.QueryString["Email"] == null) return;
            string userName = (Request.QueryString["Email"] != null ? Membership.GetUserNameByEmail(Request.QueryString["Email"]) : Membership.GetUserNameByEmail(Email.Text));
            if (!string.IsNullOrEmpty(userName))
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user == null) return;

                if (!Membership.EnablePasswordReset)
                {
                    FailureText.Text = string.Format("Password resets are not currently allowed.  Please contact an adminstrator for assistance.");
                    return;
                }

                if (user.IsLockedOut)
                {
                    FailureText.Text = "This account has been locked out.  Please contact an administrator for assistance.";
                    this.Response.Write("False");
                    return;
                }

                string generatedPassword = user.ResetPassword();
                bool ret = ThinkgateUser.SetLastPasswordChangedDateToNull(user.UserName, AppSettings.ApplicationName);
                if (string.IsNullOrEmpty(user.Email))
                {
                    // give them the system default password
                    user.ChangePassword(generatedPassword, AppSettings.DefaultPassword);
                    FailureText.Text = string.Format("Your password has been reset. There is no email account on record, please contact an adminstrator for assistance.");
                }
                else
                {
                    // send them the generated password
                    Thinkgate.Base.Classes.Messaging.SendPasswordResetMessage(user.Email, generatedPassword);
                    FailureText.Text = "An email has been sent with a temporary password to the email account on record. Once you receive it, please login and change your password to something meaningful to you.";
                }
            }
            else
            {
                FailureText.Text = "Error";
                Email.Text = "";
            }
        }



    }
}
