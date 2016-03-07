using System;
using System.Linq;
using System.Reflection;
using Standpoint.Core.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using System.Web.Security;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Account
{
    public partial class UserImpersonation : LoginPage
    {
        public SessionObject SessObj;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SessObj = (SessionObject)Session["SessionObject"];
                if (SessObj == null || SessObj.LoggedInUser == null)
                {
                    Session.Abandon();
                    return;
                }
                frmImpersonate.Visible = true;
            }
        }

        protected void BtnUnImpersonateClick(object sender, EventArgs e)
        {
            Standpoint.Core.Security.UserImpersonation.Deimpersonate();
        }

        protected void BtnImpersonateClick(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblError.Visible = false;

            SessObj = (SessionObject)Session["SessionObject"];
            if (SessObj == null || SessObj.LoggedInUser == null)
            {
                Session.Abandon();
                return;
            }
            string message = "";

            if (String.IsNullOrEmpty(UserToImpersonate.Text) || String.IsNullOrEmpty(ImpersonatePW.Text))
            {
                lblError.Text = "Please enter a valid username and password";
                lblError.Visible = true;
            }else{
                var distParms = DistrictParms.LoadDistrictParms();
                string uid = UserToImpersonate.Text;

                
                lblError.Text = "";
                lblError.Visible = false;

                if (SessObj.LoggedInUser.HasPermission(Permission.Access_ImpersonateUserAccess) && distParms.ImpersonateUserAccess && Encryption.EncryptString(ImpersonatePW.Text).Equals(distParms.ImpersonateUserAccessPW))
                {
                    message = "User:" + SessObj.LoggedInUser.UserName + " is now impersonating user:" + uid;

                    drGeneric_String_String gi = new drGeneric_String_String();
                    //******* 20121029 DHB Start code changes. 
                    //Session["GlobalInputs"] is no longer referenced in the code. Please use AppSettings.GlobalInputs instead.
                    SessObj.GlobalInputs = gi;
                    Session["GlobalInputs"] = gi;
                    //******* 20121029 DHB Stop code changes. 
                    MembershipUser user = Membership.GetUser(uid);
                    if (user == null)
                    {
                        lblError.Text = "Username " + uid + " not found";
                        lblError.Visible = true;
                        return;
                    }
                    SessObj.LoggedInUser = new ThinkgateUser(user);

                    gi.Add("UserID", SessObj.LoggedInUser.UserId.ToString());
                    gi.Add("UserPage", SessObj.LoggedInUser.Page.ToString());
                    gi.Add("UserName", SessObj.LoggedInUser.UserName);

                    Session["SessionObject"] = SessObj;
                    
                    ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, message, "IMPERSONATION");

                    Standpoint.Core.Security.UserImpersonation.ImpersonateUser(uid, "~/PortalSelection.aspx");
                    Response.Redirect("~/PortalSelection.aspx", true);
                }
                else
                {
                    lblError.Text = "Error, you do not have access to impersonate.";
                    lblError.Visible = true;
                    message = "User:" + SessObj.LoggedInUser.UserName + " just attempted to impersonate user:" + uid;

                    ThinkgateEventSource.Log.ApplicationWarning(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, message);
                }
            }
        }
    }
}
