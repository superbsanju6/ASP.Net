using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using Standpoint.Core.ExtensionMethods;
using Thinkgate.Base.Classes;

namespace Thinkgate
{
    public partial class LoggedOut : Page
    {
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

            // Use SsoLogoutRedirectUrl setting in DistrictParms as an optional SSO Logout Redirect URL,
            // but allow ssoLogoutRedirectUrlOverride in web.config to override it
            var ssoLogoutRedirectUrl = DistrictParms.LoadDistrictParms().SsoLogOutRedirectUrl;
            string ssoLogoutRedirectUrlOverride =
                ConfigurationManager.AppSettings["SsoLogoutRedirectUrlOverride"];
            if (!ssoLogoutRedirectUrlOverride.IsNull())
                ssoLogoutRedirectUrl = ssoLogoutRedirectUrlOverride;
            if (!ssoLogoutRedirectUrl.IsNullOrEmpty())
                Response.Redirect(ssoLogoutRedirectUrl, true);
        }
    }
}