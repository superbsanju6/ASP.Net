using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.SiteProvider;
using Standpoint.Core.ExtensionMethods;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Data;
using Thinkgate.Domain.Exceptions;
using System.IO;
using System.Drawing;
using Thinkgate.Enums;
using CultureInfo = System.Globalization;
using Thinkgate.ExceptionHandling;

namespace Thinkgate
{
    public partial class TGLogin : LoginPage
    {
        #region Variables

        private string _passedInUserName;
        private string _passedInPassword;
        private DateTime? _authExpirationDateTime;
        private bool _authenticatedViaSso;
        private bool _doNotContinue;
        private SessionObject _currentSession;
        public string ClientLoginCss = Thinkgate.SiteMaster.GetClientCss("Login/TGLogin");
        public string ClientSiteCss = Thinkgate.SiteMaster.GetClientCss("Site");

        #endregion

        #region Constants

        private const string KenticoCookieName = ".KENTICOAUTH";
        private const float BackgroundBrightnessThreshhold = .73f;

        private const string SHIB_LEA_ID_ATTRIBUTE_NAME = "orgdn"; // eduPersonOrgDN
        private const string SHIB_TEACHER_ID_ATTRIBUTE_NAME = "eppn"; // eduPersonPrincipalName
        private const string SHIB_DEFAULT_LEA_QUALIFIER = "Shibboleth";


        #endregion

        #region Properties

        /// <summary>
        /// Accessor to give us our current session object without having to duplicate code
        /// </summary>
        private SessionObject CurrentSession
        {
            get
            {
                if (_currentSession == null)
                {
                    _currentSession = (SessionObject)Session["SessionObject"];
                }

                return _currentSession;
            }
        }

        /// <summary>
        /// Accessor to give us the Username we were passed without duplicating code
        /// and without re retreiving the data if it has already been loaded
        /// </summary>
        private String PassedInUserName
        {
            get
            {
                if (String.IsNullOrEmpty(_passedInUserName))
                {
                    _passedInUserName = Request["passedInUserName"];
                }

                return _passedInUserName;
            }
        }

        /// <summary>
        /// Accessor to give us the Password we were passed without duplicating code
        /// and without re retreiving the data if it has already been loaded
        /// </summary>
        private string PassedInPassword
        {
            get
            {
                if (String.IsNullOrEmpty(_passedInPassword))
                {
                    _passedInPassword = Request["passedInPassword"];
                }

                return _passedInPassword;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// This handles the Load event of this page.  It performs page initialization
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">EventArgs passed to the eventhandler</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _authenticatedViaSso = false;
            bool bypassSso = !string.IsNullOrEmpty(Request.QueryString["BypassSSO"]);

            // TODO:  Move this "Kentico Enabled" functionality to some type of application configuration manager.  It should not be in the login page.
            SetIsKenticoEnabled();

            Session["MessageCenterPopupVisited"] = false;

            var configuredAuthMethod = (AuthMethod)Session["configuredAuthMethod"];

            if ((configuredAuthMethod != AuthMethod.NonSso) && !bypassSso)
            {
                RedirectForSso(configuredAuthMethod);
            }

            SetVersionInfo();

            if (!string.IsNullOrEmpty(DParms.HomePageMessage))
            {
                DisplayHomepageMessage();
            }

            SetBackgroundColor();

            // if LoginPageHideGradiant is true then 'hide' the gradiant mask (set bgimg visibility to false)
            if (DParms.LoginPageHideGradiant)
            {
                bgimg.Visible = false;
            }

            if (!String.IsNullOrEmpty(PassedInUserName) && !String.IsNullOrEmpty(PassedInPassword))
            {
                if (Request["donotcontinue"] != "false")
                {
                    //If we've gotten here. it means that tgLogin was requested by 
                    //Thinkgate.Net's SignInIFrameVersion.asp page instead of the 
                    //user hitting tgLogin.aspx directly. We'll set the 
                    //_DoNotContinue flag to true here so that later on when
                    //we return to the SignInIFrameVersion.asp, it will
                    //open a new window and take the user to his/her portal page.
                    _doNotContinue = true;
                    myLogin_Authenticate(null, null);
                }
                else
                {
                    myLogin_Authenticate(null, null);
                }
            }

            SetFocusToUsernameInput();
        }

        /// <summary>
        /// This method 
        /// </summary>
        /// <param name="sender">myLogin Control</param>
        /// <param name="e">EventArgs passed to the handler</param>
        protected void myLogin_Authenticate(object sender, AuthenticateEventArgs e)
        {
            bool authenticatedUser = false;

            //swap  out the below two lines for production:
            var providedUserName = GetUserName();
            var providedUserPassword = GetPassword();

            if (_authenticatedViaSso)
            {
                authenticatedUser = true;
            }
            else if (!ShouldWeBypassLdap(providedUserName) && DParms.LDAPEnabled) //is this an LDAP enabled district, and a value in the BypassLDAP URL request param IS NOT present?
            {
                TgLdapResponce tgLdapResponse = ValidateLDAPUser(providedUserName, providedUserPassword);

                if (tgLdapResponse.IsErrored)
                {
                    myLogin.FailureText = tgLdapResponse.ExceptionMessage;
                }
                authenticatedUser = tgLdapResponse.IsLDAPAuthenticated;
            }
            else
            {

                if (ConfigurationManager.AppSettings.AllKeys.Contains("isLoggingEnabled"))
                {

                    bool isLoggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["isLoggingEnabled"].ToString().ToLower());
                    if (isLoggingEnabled)
                    {
                        int ID = LogUserLoginDetails(providedUserName, providedUserPassword, false);
                        authenticatedUser = Membership.ValidateUser(providedUserName, providedUserPassword);

                        if (authenticatedUser == false)
                        {
                            UpdateLogUserLoginDetails(ID, "Invalid Password", authenticatedUser);
                        }
                        else
                        {
                            UpdateLogUserLoginDetails(ID, "Successfully Logged In", authenticatedUser);
                        }
                    }
                    else
                    {
                        authenticatedUser = Membership.ValidateUser(providedUserName, providedUserPassword);
                    }
                }
                else
                {
                    authenticatedUser = Membership.ValidateUser(providedUserName, providedUserPassword);
                }
            }

            if (authenticatedUser)
            {
               
                string kenticoUserName = null;
                if ((bool)Session["KenticoEnabled"]) { kenticoUserName = KenticoHelper.GetKenticoUser(providedUserName); }
                LogUserLogin(providedUserName, true);
                SessionObject obj = (SessionObject)Session["SessionObject"];

                if (obj == null)
                {
                    //ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                    //Create new Thinkgate Session Object for all objects that get passed around in Session
                    obj = new SessionObject();
                    Session["SessionObject"] = obj;

                    ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Session " + Session.SessionID + " Start", string.Empty);
                    Session.Timeout = (AppSettings.SessionTimeout > 0 ? AppSettings.SessionTimeout : 20);
                }
                else
                {
                    obj.CheckNewWorksheetCreated = "0";
                }

                drGeneric_String_String gi = new drGeneric_String_String();
                Session["GlobalInputs"] = gi;
                obj.GlobalInputs = gi;
                MembershipUser user = Membership.GetUser(providedUserName);
                myLogin_LoginError(sender, e);
                if (user == null) return;

                obj.LoggedInUser = new ThinkgateUser(user);

                gi.Add("UserID", obj.LoggedInUser.UserId.ToString());
                gi.Add("UserPage", obj.LoggedInUser.Page.ToString());
                gi.Add("UserName", obj.LoggedInUser.UserName);

                obj.EncryptedProvidedPwd = Standpoint.Core.Classes.Encryption.EncryptString(providedUserPassword);
                DateTime resetDate = new DateTime(1999, 6, 22);

                Session["SessionObject"] = obj;

                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.UserID = sessionObject.LoggedInUser.UserId.ToString();
                ThinkgateEventSource.Log.LoggedUserPortalLogin(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has logged in", sessionObject.LoggedInUser.Roles[0].RoleName, sessionObject.LoggedInUser.UserName);

                if (!DParms.LDAPEnabled && (providedUserPassword == AppSettings.DefaultPassword ||
                    obj.LoggedInUser.LastPasswordChangedDate.ToShortDateString() == resetDate.ToShortDateString() ||
                    obj.LoggedInUser.Restrictions == ThinkgateUser.ChangePasswordRestrictionValue
                    || ThinkgateUser.IsPasswordExpired(obj.LoggedInUser.UserId)//--90 days password.
                   ))
                {
                    CreateAuthCookie(providedUserName, true);
                    if ((bool)Session["KenticoEnabled"])
                    {
                        CreateCookieClone(kenticoUserName);
                    }

                    if (_doNotContinue)
                    {
                        string script = " DoLogin('ResetPassword'); ";
                        ScriptManager.RegisterStartupScript(this, typeof(string), "LoginFromSplashPage", script, true);
                        return;
                    }
                    else
                    {
                        Response.Redirect("~/Account/ChangePassword.aspx", true);
                    }
                }
                CreateAuthCookie(providedUserName, true);
                if ((bool)Session["KenticoEnabled"])
                {
                    CreateCookieClone(kenticoUserName);
                }

                if (_doNotContinue && !DParms.LDAPEnabled)
                {
                    string script = " DoLogin('" + myLogin.FailureText + "'); ";
                    ScriptManager.RegisterStartupScript(this, typeof(string), "LoginFromSplashPage", script, true);
                    return;
                }

                if (!_authenticatedViaSso)
                    FormsAuthentication.RedirectFromLoginPage(providedUserName, false);
                else
                {
                    if (_authExpirationDateTime == null)
                        obj.IsSsoAuthTimeoutSpecified = false;
                    else
                    {
                        obj.SsoAuthTimeoutDateTime = _authExpirationDateTime.Value;
                        obj.IsSsoAuthTimeoutSpecified = true;
                    }
                }
            }
            else
            {                
                LogUserLogin(providedUserName, false);
                if (_doNotContinue)
                {
                    myLogin.FailureText = "Invalid login. Signin failed for " + providedUserName + ".";
                    MembershipUser usrInfo = Membership.GetUser(providedUserName);
                    if (usrInfo != null)
                    {
                        // Is this user locked out?
                        if (usrInfo.IsLockedOut)
                        {
                            myLogin.FailureText =
                                "Access revoked-password attempts.  Signin failed for " + providedUserName + ".";
                        }
                        else if (!usrInfo.IsApproved)
                        {
                            myLogin.FailureText =
                                "Your account (" + providedUserName + ") has not yet been approved. You cannot login until an administrator has approved your account.";
                        }
                        else
                        {
                            myLogin.FailureText = "Password is invalid. Signin failed for " + providedUserName + ".";
                        }
                    }

                    string script = " DoLogin('" + myLogin.FailureText + "'); ";
                    ScriptManager.RegisterStartupScript(this, typeof(string), "LoginFromSplashPage", script, true);
                }
            }
        }

        /// <summary>
        /// This event fires when a login error occurs
        /// </summary>
        /// <param name="sender">myLogin control</param>
        /// <param name="e">EventArgs passed to the handler</param>
        protected void myLogin_LoginError(object sender, EventArgs e)
        {
            // Determine why the user could not login...        
            if (!DParms.LDAPEnabled && !String.IsNullOrEmpty(myLogin.FailureText))
            {
                myLogin.FailureText = "Your login attempt was not successful. Please try again.";
            }

            var providedUserName = GetUserName();

            // Does there exist a User account for this user?
            MembershipUser usrInfo = Membership.GetUser(providedUserName);
            if (usrInfo != null)
            {
                // Is this user locked out?
                if (usrInfo.IsLockedOut)
                {
                    myLogin.FailureText =
                        "Access revoked-password attempts.  Signin failed for " + providedUserName + ".";
                }
                else if (!usrInfo.IsApproved)
                {
                    myLogin.FailureText =
                        "Your account (" + providedUserName + ") has not yet been approved. You cannot login until an administrator has approved your account.";
                }
                else
                {
                    myLogin.FailureText = "";
                }
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Some of our clients use Federated Security which isn't directly supported by E3;
        /// If this is one of those clients, then SiteRouter will have created a .TGSSOAUTH cookie for us
        /// and we will pull the authentication info from that cookie
        /// </summary>
        /// <returns>
        /// Redirection URL string, either for reauthentication or for the next page following successful login
        /// </returns>
        private string ProcessSiteRouterLogin()
        {
            const string ssoAuthCookieName = ".TGSSOAUTH";
            string reauthenticateUrl = DParms.SiteRouterUrl;
            string redirectUrl;

            try
            {
                // If SSO enabled and accessing TGLogin.aspx page when already authenticated, force re-authentication
                if (Request.IsAuthenticated)
                {
                    Services.Service2.KillSession();
                    LogSiteRouterSsoRedirect("Session already active, killing session", reauthenticateUrl);
                    //return reauthenticateUrl;
                }
                // Process the SSO Authentication cookie, or force re-authentication if no cookie
                HttpCookie ssoAuthCookie = Request.Cookies[ssoAuthCookieName];
                if (ssoAuthCookie == null)
                {
                    LogSiteRouterSsoRedirect("No ssoAuthCookie", reauthenticateUrl);
                    return reauthenticateUrl;
                }

                FormsAuthenticationTicket ssoAuthenticationTicket = FormsAuthentication.Decrypt(ssoAuthCookie.Value);
                if (ssoAuthenticationTicket == null)
                {
                    LogSiteRouterSsoRedirect("No ssoAuthenticationTicket", reauthenticateUrl);
                    return reauthenticateUrl;
                }

                string userName = ssoAuthenticationTicket.Name;
                string userData = ssoAuthenticationTicket.UserData;
                // parse '&' delimited name=value pairs from the FormsAuthenticationTicket UserData
                Dictionary<string, string> userDataPairs =
                    userData.Split('&').Select(value => value.Split('=')).ToDictionary(pair => pair[0], pair => pair[1]);
                string userDataClientId = userDataPairs["ClientID"];
                DateTime expirationDateTime = ssoAuthenticationTicket.Expiration;

                // Remove the cookie so it will not be used again
                Services.Service2.expireCookie(ssoAuthCookieName);

                // Check if the authentication has already expired
                if (DateTime.Compare(DateTime.Now, expirationDateTime) >= 0)
                    return reauthenticateUrl;

                // Check if the cookie is for a different virtual path
                if (String.Compare(userDataClientId, AppSettings.AppVirtualPath.Replace("/", ""), StringComparison.OrdinalIgnoreCase) != 0)
                    return reauthenticateUrl;

                if (Membership.GetUser(userName) == null)
                    return reauthenticateUrl;

                // Initialize _passedInUserName and _passedInPassword for use by myLogin_Authenticate()
                // The _passedInPassword value will not be used for SSO, but must be non-null and non-empty.
                _passedInUserName = userName;
                _passedInPassword = "Unknown";
                _authExpirationDateTime = expirationDateTime;
                _authenticatedViaSso = true;
            }
            catch (Exception e)
            {
                Services.Service2.expireCookie(ssoAuthCookieName);
                return reauthenticateUrl;
            }

            // Perform the required session setup, then redirect
            myLogin_Authenticate(null, null);

            if (!string.IsNullOrEmpty(Request.QueryString["ReturnURL"]))
            {
                redirectUrl = Request.QueryString["ReturnURL"];
            }
            else
            {
                redirectUrl = "PortalSelection.aspx";
            }
            return redirectUrl;
        }

        /// <summary>
        /// Some of our clients use Shibboleth for SSO login;
        /// If this is one of those clients, we will have request header attributes specifying the user's ID and the LEA with which the user is associated
        /// </summary>
        /// <returns>
        /// Redirection URL string, either for reauthentication or for the next page following successful login.
        /// Returns null if no redirection is desired and login page s/b displayed.
        /// NOTE: If returning null, the login page will have been updated to display an error code if/as appropriate.
        /// </returns>
        private string ProcessShibbolethLogin()
        {
            string redirectUrl;

            // If SSO enabled and accessing TGLogin.aspx page when already authenticated, force re-authentication
            // NOTE:  Services.Service2.KillSession() performs a redirect to ~/TGLogin.aspx which will generate
            //        a System.Threading.ThreadAbortException
            //        We do not want to catch that exception, so perform this check outside of the try/catch.
            if (Request.IsAuthenticated)
            {
                redirectUrl = GetLoginUrlForClientId(AppSettings.AppVirtualPath.Replace("/", ""));
                LogShibbolethSsoRedirect("Session already active, killing session", redirectUrl);
                Services.Service2.KillSessionSSO();
                return redirectUrl;
            }

            try
            {
                // Process the Shibboleth attribute containing the LEA Identifier information

                string leaIdAttributeValue;
                string clientId;
                if (!TryGetClientIdFromShibAttribute(SHIB_LEA_ID_ATTRIBUTE_NAME, out leaIdAttributeValue, out clientId))
                {
                    return null;
                }

                // If the authentication is for a different clientId, redirect to that client
                if (String.Compare(clientId, AppSettings.AppVirtualPath.Replace("/", ""), StringComparison.OrdinalIgnoreCase) != 0)
                {
                    redirectUrl = GetLoginUrlForClientId(clientId);
                    LogShibbolethSsoRedirect(
                        "Attribute '" + SHIB_LEA_ID_ATTRIBUTE_NAME + "' ("
                        + leaIdAttributeValue + ") maps to clientId '" + clientId + "'",
                        redirectUrl);
                    return redirectUrl;
                }

                // The authentication is for our clientId...
                // Process the Shibboleth attribute containing the Teacher Identifier information
                string userName;
                if (!TryGetUserNameFromShibAttribute(SHIB_TEACHER_ID_ATTRIBUTE_NAME, out userName))
                {
                    return null;
                }

                // Initialize _passedInUserName and _passedInPassword for use by myLogin_Authenticate()
                // The _passedInPassword value will not be used for SSO, but must be non-null and non-empty.
                _passedInUserName = userName;
                _passedInPassword = "Unknown";
                _authExpirationDateTime = null;
                _authenticatedViaSso = true;
            }
            catch (Exception e)
            {
                LogAndDisplaySsoErrorOnLoginPage(LoginErrorCode.UnspecifiedError, e.ToString());
                return null;
            }

            // Perform the required session setup, then redirect
            myLogin_Authenticate(null, null);

            if (!string.IsNullOrEmpty(Request.QueryString["ReturnURL"]))
            {
                redirectUrl = Request.QueryString["ReturnURL"];
            }
            else
            {
                redirectUrl = "PortalSelection.aspx";
            }
            return redirectUrl;
        }


        /// <summary>
        /// Try to get the ClientId from the specified Shibboleth attribute
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <param name="clientId"></param>
        /// <returns>
        /// Returns true if ClientId obtained successfully
        /// Returns false otherwise
        /// NOTE: If returning false, the login page will have been updated to display an error code if/as appropriate.
        /// </returns>
        private bool TryGetClientIdFromShibAttribute(
            string attributeName,
            out string attributeValue,
            out string clientId)
        {
            attributeValue = Request.Headers[attributeName];
            clientId = null;
            if (attributeValue.IsNull())
            {
                LogAndDisplaySsoErrorOnLoginPage(
                    LoginErrorCode.MissingClaimLeaIdentifier,
                    "Missing Shibboleth attribute '" + attributeName + "'");
                return false;
            }
            var leaIdentifier = GetLeaIdentifierFromShibAttribute(attributeValue);
            if (leaIdentifier.IsNullOrEmpty())
            {
                LogAndDisplaySsoErrorOnLoginPage(
                    LoginErrorCode.EmptyClaimLeaIdentifier,
                    "Shibboleth attribute '" + attributeName + "' ("
                    + attributeValue + ") specifies an empty LEA identifier");
                return false;
            }
            // Determine the clientId based on the LEA Identifier information
            string leaQualifier = ConfigurationManager.AppSettings["ShibbolethSsoLeaQualifier"];
            if (leaQualifier.IsNullOrEmpty())
                leaQualifier = SHIB_DEFAULT_LEA_QUALIFIER;
            clientId = LookupClientIdForLeaIdentifier(leaQualifier, leaIdentifier);
            if (clientId.IsNullOrEmpty())
            {
                LogAndDisplaySsoErrorOnLoginPage(
                    LoginErrorCode.NotFoundLeaIdentifier,
                    "Shibboleth attribute '" + attributeName + "' ("
                    + attributeValue + ") specifies a non-existent LEA identifier ("
                    + leaIdentifier + ") for LEA Qualifier (" + leaQualifier + ")");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Try to get the UserName from the specified Shibboleth attribute
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="userName"></param>
        /// <returns>
        /// Returns true if UserName obtained successfully
        /// Returns false otherwise
        /// NOTE: If returning false, the login page will have been updated to display an error code if/as appropriate.
        /// </returns>
        private bool TryGetUserNameFromShibAttribute(
            string attributeName,
            out string userName)
        {
            userName = null;

            string attributeValue = Request.Headers[attributeName];
            if (attributeValue.IsNull())
            {
                LogAndDisplaySsoErrorOnLoginPage(
                    LoginErrorCode.MissingClaimTeacherIdentifier,
                    "Missing Shibboleth attribute '" + attributeName + "'");
                return false;
            }

            var teacherId = GetTeacherIdFromShibAttribute(attributeValue);
            if (teacherId.IsNullOrEmpty())
            {
                LogAndDisplaySsoErrorOnLoginPage(
                    LoginErrorCode.EmptyClaimTeacherIdentifier,
                    "Shibboleth attribute '" + attributeName + "' ("
                    + attributeValue + ") specifies an empty TeacherID");
                return false;
            }

            // Verify the specified TeacherID is valid in this LEA
            userName = LookupUserNameForTeacherId(teacherId);
            if (userName.IsNullOrEmpty() || (Membership.GetUser(userName) == null))
            {
                LogAndDisplaySsoErrorOnLoginPage(LoginErrorCode.NotFoundTeacherIdentifier,
                    "Shibboleth attribute '" + attributeName + "' ("
                    + attributeValue + ") specifies a non-existent TeacherID ("
                    + teacherId + ")");
                return false;
            }
            return true;
        }


        /// <summary>
        /// Log and display specified error on the TGLogin page
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="logMessage"></param>
        private void LogAndDisplaySsoErrorOnLoginPage(LoginErrorCode errorCode, string logMessage)
        {
            string ssoErrorPrefixText = ConfigurationManager.AppSettings["SsoErrorPrefixText"];
            string errorCodeText = string.Format("Error code: {0}{1:D4}", ssoErrorPrefixText, (int)errorCode);

            // Enable display of the error on the login page
            ssoErrorMessageDiv.Visible = true;
            lblSsoErrorMessage.Text = "&nbsp;&nbsp;" + errorCodeText;

            // Log the error
            ThinkgateEventSource.Log.LoggedUserPortalLoginSsoError(
                MethodBase.GetCurrentMethod().DeclaringType + "->"
                + MethodBase.GetCurrentMethod().Name,
                errorCodeText + " Error details: " + logMessage,
                "",
                "");

        }

        /// <summary>
        /// Log redirect from SiteRouter-protected TgLogin.aspx page
        /// </summary>
        /// <param name="reasonMessage"></param>
        /// <param name="redirectUrl"></param>
        private static void LogSiteRouterSsoRedirect(string reasonMessage, string redirectUrl)
        {
            ThinkgateEventSource.Log.LoggedUserPortalLoginSsoRedirect(
                MethodBase.GetCurrentMethod().DeclaringType + "->"
                + MethodBase.GetCurrentMethod().Name,
                "Redirecting from TgLogin to '" + redirectUrl + "' (" + reasonMessage + ")",
                "",
                "");
        }

        /// <summary>
        /// Log redirect from Shibboleth-protected TgLogin.aspx page
        /// </summary>
        /// <param name="reasonMessage"></param>
        /// <param name="redirectUrl"></param>
        private static void LogShibbolethSsoRedirect(string reasonMessage, string redirectUrl)
        {
            ThinkgateEventSource.Log.LoggedUserPortalLoginSsoRedirect(
                MethodBase.GetCurrentMethod().DeclaringType + "->"
                + MethodBase.GetCurrentMethod().Name,
                "Redirecting from TgLogin to '" + redirectUrl + "' (" + reasonMessage + ")",
                "",
                "");
        }

        /// <summary>
        /// Get leaIdentifier from attributes provided by Shibboleth
        /// Returns leaIdentifier in the form "districtX.org" parsed from the eduPersonOrgDN attribute
        /// which is received from Shibboleth in the form "O=organization,DC=districtX,DC=org"
        /// </summary>
        /// <returns>
        /// Returns null if Shibboleth header is not present
        /// </returns>
        private static string GetLeaIdentifierFromShibAttribute(string shibLeaIdentifierAttribute)
        {
            const string separator = ",DC="; // "O=o,DC=districtX,DC=org"
            var separators = new[] { separator };

            // If the attribute is not present, return null
            if (shibLeaIdentifierAttribute.IsNull())
                return null;

            // if no ,DC= parameters are present, return empty string
            var firstIndex = shibLeaIdentifierAttribute.IndexOf(separator, StringComparison.Ordinal);
            if (firstIndex < 0)
                return string.Empty;

            // convert "...,DC=x,DC=y,DC=etc" to "x.y.etc" for return
            // (also trims the individual DC= component parts)
            var leaIdentifierSubstring = shibLeaIdentifierAttribute.Substring(firstIndex + separator.Length);
            var leaIdentifier = String.Join(
                ".",
                leaIdentifierSubstring.Split(separators, StringSplitOptions.None).Select(s => s.Trim()).ToList());
            return leaIdentifier;
        }

        /// <summary>
        /// Get TeacherId from attributes provided by Shibboleth
        /// Returns teacherID in the form "user" parsed from eduPersonPrincipalName attribute
        /// which is received from Shibboleth in the form "user@scope"
        /// </summary>
        /// <returns>
        /// Returns null if Shibboleth header is not present
        /// </returns>
        private static string GetTeacherIdFromShibAttribute(string shibTeacherIdAttribute)
        {
            const char separator = '@'; // "user@scope"

            // if the attribute is not present, return null
            if (shibTeacherIdAttribute.IsNull())
                return null;

            // if no "user@" is present, return empty string
            var firstIndex = shibTeacherIdAttribute.IndexOf(separator);
            if (firstIndex < 0)
                return string.Empty;

            // convert "user@scope" to "user" for return
            var teacherId = shibTeacherIdAttribute.Substring(0, firstIndex);
            return teacherId;
        }

        /// <summary>
        /// Perform database lookup of ClientID matching the specified locality
        /// </summary>
        /// <param name="leaQualifier"></param>
        /// <param name="leaIdentifier"></param>
        /// <returns>
        /// Returns null if not found
        /// </returns>
        private static string LookupClientIdForLeaIdentifier(string leaQualifier, string leaIdentifier)
        {
            if (leaQualifier.IsNullOrEmpty() || leaQualifier.IsNullOrEmpty())
                return null;

            var thinkgateConfigConnectionString = ConfigurationManager.ConnectionStrings["Thinkgate_config"].ConnectionString;

            using (var connection = new SqlConnection(thinkgateConfigConnectionString))
            {
                const string sqlCmdTxt =
                    "SELECT TOP 1 ClientID FROM SSO_LEAMapping WHERE LEAQualifier = @leaQualifier and LEAIdentifier = @leaIdentifier;";

                connection.Open();
                using (var cmd = new SqlCommand(sqlCmdTxt, connection))
                {
                    cmd.Parameters.Add(
                        new SqlParameter("@leaQualifier", SqlDbType.NVarChar, 20)
                        {
                            Value = leaQualifier,
                            Direction = ParameterDirection.Input
                        });
                    cmd.Parameters.Add(
                        new SqlParameter("@leaIdentifier", SqlDbType.NVarChar, 20)
                        {
                            Value = leaIdentifier,
                            Direction = ParameterDirection.Input
                        });

                    string clientId = Convert.ToString(cmd.ExecuteScalar());
                    return clientId.Trim();
                }
            }
        }

        /// <summary>
        /// Perform database lookup of username matching the specified TeacherId
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns>
        /// returns null if TeacherId not found
        /// </returns>
        private static string LookupUserNameForTeacherId(string teacherId)
        {
            string userName = null;
            if (!teacherId.IsNullOrEmpty())
            {
                var clientId = AppSettings.ConnectionStringName;
                var unitOfWork = new UnitOfWork(clientId);
                try
                {
                    var users =
                        unitOfWork.AspnetUsersRepository.Get(w => w.TeacherID == teacherId).ToList();
                    if (users.Any())
                        userName = users.First().UserName;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            return userName;
        }

        /// <summary>
        /// Build an absolute login url for the specified clientId
        /// </summary>
        /// <param name="targetClientId"></param>
        /// <returns></returns>
        private string GetLoginUrlForClientId(string targetClientId)
        {
            return string.Format("{0}://{1}/{2}/{3}",
                Request.IsSecureConnection ? "https" : "http",
                Request.Headers["Host"],
                targetClientId,
                "TGLogin.aspx");
        }


        /// <summary>
        /// Performs a URL redirect based on whether or not authenticated via SSO
        /// </summary>
        /// <param name="authMethod"></param>
        private void RedirectForSso(AuthMethod authMethod)
        {
            string redirectUrl = null;
            switch (authMethod)
            {
                case AuthMethod.NonSso:
                    break;
                case AuthMethod.ShibbolethSso:
                    redirectUrl = ProcessShibbolethLogin();
                    break;
                case AuthMethod.SiteRouterSso:
                    redirectUrl = ProcessSiteRouterLogin();
                    break;
            }
            if (redirectUrl != null)
            {
                Response.Redirect(redirectUrl);
            }
        }

        /// <summary>
        /// Sets the IsKenticoEnabled property by looking for an app setting in the web.config.  If the app setting 
        /// does not exist in the web.config it defaults to 'true'.
        /// </summary>
        private void SetIsKenticoEnabled()
        {
            bool isKenticoEnabled = true;
            isKenticoEnabled = bool.Parse(DParms.IsKenticoEnabledSite);
            Session["KenticoEnabled"] = isKenticoEnabled;
        }

        /// <summary>
        /// If LoginPageBackgroundColor parm exists, set the login page background to that color, else default to #28628F
        /// </summary>
        private void SetBackgroundColor()
        {
            if (!string.IsNullOrEmpty(DParms.LoginPageBackgroundColor))
            {
                bodyTag.Style.Add("background-color", DParms.LoginPageBackgroundColor);
                AdjustBackgroundBrightness();
            }
            else
            {
                bodyTag.Style.Add("background-color", "#28628F");
            }
        }

        /// <summary>
        /// If the brightness of the background is >= .73 then we will use a black font otherwise it will keep a white font as set on the aspx page
        /// </summary>
        private void AdjustBackgroundBrightness()
        {
            // if the brightness is >= .73 then we will use a black font otherwise it will keep a white font as set on the aspx page
            float brightness = ColorTranslator.FromHtml(DParms.LoginPageBackgroundColor).GetBrightness();

            if (brightness >= BackgroundBrightnessThreshhold)
            {
                myLogin.ForeColor = Color.Black;
                lblVersion.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Displays the appropriate Homepage message when called
        /// </summary>
        private void DisplayHomepageMessage()
        {
            lblMessage.Visible = true;
            lblMessage.BackColor = System.Drawing.Color.Orange;
            lblMessage.Text = DParms.HomePageMessage;
        }

        /// <summary>
        /// Gets the Password that the current user is using
        /// </summary>
        /// <returns>The Password that the current user is using</returns>
        private string GetPassword()
        {
            return !String.IsNullOrEmpty(_passedInPassword) ? _passedInPassword : myLogin.Password;
        }

        /// <summary>
        /// Gets the Username that the current user is using
        /// </summary>
        /// <returns>The Username that the current user is using</returns>
        private string GetUserName()
        {
            return !String.IsNullOrEmpty(_passedInUserName) ? _passedInUserName : myLogin.UserName;
        }

        /// <summary>
        /// Allows us to determine if we can bypass
        /// </summary>
        /// <param name="providedUserName">The Username of the current user</param>
        /// <returns>true if the QueryString does not contain "BypassLDAP" or if the User is a SuperAdmin </returns>
        private bool ShouldWeBypassLdap(string providedUserName)
        {
            return !string.IsNullOrEmpty(Request.QueryString["BypassLDAP"]) || IsUserSuperAdmin(providedUserName);
        }

        /// <summary>
        /// Determines if the current user is a "SuperAdmin"
        /// </summary>
        /// <param name="username">The username supplied from the current user</param>
        /// <returns>true if the user a SuperAdmin; false otherwise </returns>
        private bool IsUserSuperAdmin(string username)
        {
            bool isSuperAdmin = false;
            try
            {
                MembershipUser user = Membership.GetUser(username);
                if (user != null)
                {
                    CurrentSession.LoggedInUser = new ThinkgateUser(user);
                    if (CurrentSession.LoggedInUser.IsSuperAdmin)
                    {
                        isSuperAdmin = true;
                    }
                }
            }
            catch (Exception ex)
            { }

            return isSuperAdmin;
        }

        /// <summary>
        /// Sets the website version number
        /// the last time the assembly was updated
        /// and the Server Name to a label for display
        /// </summary>
        private void SetVersionInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            lblVersion.Text = String.Format("Version: {0}", assembly.GetName().Version);
            lblModifiedDate.Text = String.Format("Dated: {0}", File.GetLastWriteTime(assembly.Location).ToShortDateString());
            lblServerName.Text = Server.MachineName;
        }

        /// <summary>
        /// Causes focus to be set to the "Username" textbox automatically
        /// </summary>
        private void SetFocusToUsernameInput()
        {
            var myloginUserName = myLogin.FindControl("UserName");
            if (myloginUserName != null)
            {
                SetFocus(myloginUserName.ClientID);
            }
        }

        /// <summary>
        /// Creates an authentication cookie for Kentico so we dont have to reacuthenticate with kentico
        /// </summary>
        /// <param name="username">E3 Username</param>
        /// <param name="isPersistent">Determines if the Cookie being created is persistant</param>
        private void CreateAuthCookie(string username, bool isPersistent)
        {
            if (_authExpirationDateTime == null)
            {
                FormsAuthentication.SetAuthCookie(username, isPersistent);
            }
            else
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, username, DateTime.Now,
                    _authExpirationDateTime.Value, isPersistent, "", FormsAuthentication.FormsCookiePath);
                string hash = FormsAuthentication.Encrypt(ticket);

                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
                cookie.HttpOnly = true;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        private void CreateCookieClone(string username)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, username, DateTime.Now, DateTime.Now.AddMinutes(60), true, "", FormsAuthentication.FormsCookiePath);

            string hash = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(KenticoCookieName, hash);
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
            SetKenticoContext(username);
        }

        /// <summary>
        /// The Kintico username is different from the E3 username so this method
        /// sets the Kintico Username
        /// </summary>
        /// <param name="username">Username that comes from E3</param>
        private void SetKenticoContext(string username)
        {
            CMSContext.Init();

            try
            {
                UserInfo userInfo = UserInfoProvider.GetUserInfo(username);
                CurrentUserInfo currentUserInfo = new CurrentUserInfo(userInfo, true);
                CMSContext.CurrentUser = currentUserInfo;
                Session["KenticoUserInfo"] = userInfo;
            }
            catch (Exception e)
            {
                Guard.FailIf(true, "User " + username + " does not exist in the content management system: " + e.Message);
            }
        }

        #endregion
    }
}
