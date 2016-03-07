using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using System.Web.Security;
using Thinkgate.Base.Classes;
using System.ComponentModel;

namespace Thinkgate
{
    public partial class reset : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

      

       

        protected void Reset_Click(object sender, EventArgs e)
        {
            //string clientID = Request["txtClientID"];
            string userName = Request["txtUserName"];
            //if (!string.IsNullOrEmpty(clientID) && !string.IsNullOrEmpty(userName))
            if (!string.IsNullOrEmpty(userName))
            {
                MembershipUser user = Membership.GetUser(userName);
                //this.Response.Clear();
                lblMessage.Text = "";
                if (user == null)
                {

                    lblMessage.Text = "Unknown User Name '" + userName + "'.  Please contact an administrator for assistance.";
                    return;
                }
                //if (clientID.ToUpper() != DistrictParms.LoadDistrictParms().ClientID.ToUpper()) 
                //{
                //    lblMessage.Text = "Client ID:'" + clientID + "' was not found,please try again.";
                //    //lblMessage.Text = "Please enter the correct Client ID.";
                //    return;
                //}

                if (!Membership.EnablePasswordReset)
                {
                    lblMessage.Text = string.Format("Password resets are not currently allowed.  Please contact an adminstrator for assistance.");
                    return;
                }

                if (user.IsLockedOut)
                {
                    lblMessage.Text = "This account has been locked out.  Please contact an administrator for assistance.";
                    return;
                }

                string generatedPassword = user.ResetPassword();
                bool ret = ThinkgateUser.SetLastPasswordChangedDateToNull(user.UserName, AppSettings.ApplicationName);
                if (string.IsNullOrEmpty(user.Email))
                {
                    // give them the system default password
                    user.ChangePassword(generatedPassword, AppSettings.DefaultPassword);
                    lblMessage.Text = string.Format("Your password has been reset. There is no email account on record, please contact an adminstrator for assistance.");
                    return;
                }
                else
                {
                    // send them the generated password
                    Thinkgate.Base.Classes.Messaging.SendResetPasswordMessage(user.Email, generatedPassword);
                    lblMessage.Text = "An email has been sent with a temporary password to the email account on record. Once you receive it, please login and change your password to something meaningful to you.";
                  
                }
                
            }

        }
    }

   
}