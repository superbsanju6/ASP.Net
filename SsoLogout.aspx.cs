using System;
using System.Web;

namespace Thinkgate
{
    public partial class SsoLogout : System.Web.UI.Page
    {
        // This SsoLogout.aspx page is expected to be invoked by a centralized SSO Identity Provider
        // to request that the application be logged out.
        // This page will typically be invoked only if/when the SSO infrastructure is configured to log out
        // all active SSO-authenticated applications upon user logout from the SSO Identity Provider itself.

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            // Note: Our page_Load() is invoked before the Site.Master Page_Load().
            // But, the following code which kills the session must run AFTER the Site.Master Page_Load() runs.
            // Therefore this code must be here in Page_LoadComplete() rather than in Page_Load().
            Services.Service2.expireCookie(".KENTICOAUTH");
            System.Web.Security.FormsAuthentication.SignOut();
            HttpContext.Current.Session["SessionObject"] = null;
            HttpContext.Current.Session.Abandon();
        }
    }
}