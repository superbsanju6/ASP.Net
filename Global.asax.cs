using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Thinkgate.Base.Classes;
using GemBox.Spreadsheet;
using Thinkgate.Base.Enums;
using Thinkgate.Core.Extensions;

using Thinkgate.ExceptionHandling;

namespace Thinkgate
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            AppSettings.AppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
            AppSettings.ConnectionStringName = AppSettings.DetermineConnectionStringName(AppSettings.AppVirtualPath);
            AppSettings.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ToString();
            AppSettings.IsLoggingEnabled = Standpoint.Core.Utilities.DataIntegrity.ConvertToBool(System.Configuration.ConfigurationManager.AppSettings.Get("IsLoggingEnabled"));
            AppSettings.OTCUrl = System.Configuration.ConfigurationManager.AppSettings.Get("OTCUrl");

            string clientName = System.Configuration.ConfigurationManager.AppSettings.Get("Client");
            string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("Application");
            string environmentName = System.Configuration.ConfigurationManager.AppSettings.Get("Environment");
            string configConnectionString = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationPropertiesConnectionString");

            AppSettings.LoadAppSettings(clientName, applicationName, environmentName, configConnectionString);
            //AppSettings.LoadAppSettings(clientName, applicationName, environmentName, AppSettings.ConnectionStringName);

            SqlDependency.Start(AppSettings.ConnectionString);      // enables ability to do SQLDependencies on this database
            SpreadsheetInfo.SetLicense(System.Configuration.ConfigurationManager.AppSettings.Get("GemboxLicenseKey"));

            ExceptionHandler.CreateExceptionPoliciesFromConfig();
            
            ThinkgateEventSource.SessionID = "0";
            ThinkgateEventSource.UserID = "";
            ThinkgateEventSource.Agent = "";
            ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Application Start", string.Empty);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //PLH - Brute Force last option. If authenticated but no session kick back to loginurl
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                if (!Response.IsRequestBeingRedirected)
                {
                    Response.Redirect(FormsAuthentication.LoginUrl, true);
                }
                HttpContext.Current.Response.End();
            }

            // Use SiteRouterEnabled or ShibbolethSsoEnabled settings in DistrictParms to enable SSO,
            // but allow authMethodOverride="NonSso"|"SiteRouterSso"|"ShibbolethSso" in web.config to override it
            string authMethodOverride = ConfigurationManager.AppSettings["AuthMethodOverride"];
            AuthMethod configuredAuthMethod = AuthMethod.NonSso;
            if (!authMethodOverride.IsNullOrEmpty())
            {
                switch (authMethodOverride)
                {
                    case "NonSso":
                        configuredAuthMethod = AuthMethod.NonSso;
                        break;
                    case "SiteRouterSso":
                        configuredAuthMethod = AuthMethod.SiteRouterSso;
                        break;
                    case "ShibbolethSso":
                        configuredAuthMethod = AuthMethod.ShibbolethSso;
                        break;
                }
            }
            else if (DistrictParms.LoadDistrictParms().SiteRouterEnabled)
            {
                configuredAuthMethod = AuthMethod.SiteRouterSso;
            }
            else if (DistrictParms.LoadDistrictParms().ShibbolethSsoEnabled)
            {
                configuredAuthMethod = AuthMethod.ShibbolethSso;
            }

            //Create new Thinkgate Session Object for all objects that get passed around in Session
            var sessionObject = new Classes.SessionObject();
            Session["SessionObject"] = sessionObject;
            
            ThinkgateEventSource.SessionID = Session.SessionID;
            ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Session " + Session.SessionID + " Start", string.Empty);

            Session.Timeout = (AppSettings.SessionTimeout > 0 ? AppSettings.SessionTimeout : 20);

            Session["configuredAuthMethod"] = configuredAuthMethod;

            //Response.Redirect("~/TGLogin.aspx");
            //Server.Transfer("~/TGLogin.aspx");
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {


        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception exc = Server.GetLastError();

            ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Application Error", exc.ToString());
        }

        protected void Session_End(object sender, EventArgs e)
        {
            ThinkgateEventSource.SessionID = Session.SessionID;
            ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Session " + Session.SessionID + " End", string.Empty);
        }

        protected void Application_End(object sender, EventArgs e)
        {

            ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Application End", string.Empty);
            SqlDependency.Stop(AppSettings.ConnectionString);
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            if (Context.Handler is IRequiresSessionState)
            {
                var SessionObject = (Thinkgate.Classes.SessionObject)Session["SessionObject"];
                if (SessionObject == null) return;

                ThinkgateEventSource.SessionID = Session.SessionID;
                if (SessionObject.LoggedInUser == null)
                { ThinkgateEventSource.UserID = ""; }
                else
                { ThinkgateEventSource.UserID = SessionObject.LoggedInUser.UserId.ToString(); }
						
                SessionObject.LogGuid = SessionObject.LogGuid++;
            }
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {          
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {               
                HttpCookie authenticationCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authenticationCookie != null)
                {
                    FormsAuthenticationTicket authenticationTicket = FormsAuthentication.Decrypt(authenticationCookie.Value);
                    if (!authenticationTicket.Expired)
                    {                       
                        if (Session["SessionObject"] == null)
                        {                          
                            FormsAuthentication.SignOut();
                            Response.Redirect(FormsAuthentication.LoginUrl, true);
                            return;
                        }
                    }
                }
            }
        }
    }
}
