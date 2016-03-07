using System.Text.RegularExpressions;
using Standpoint.Core.Classes;
using Thinkgate.Classes;
using System;
using System.Reflection;
using Thinkgate.Base.Classes;
using Thinkgate.Utilities;
using Thinkgate.ExceptionHandling;

namespace Thinkgate
{
    public partial class SessionBridge : System.Web.UI.Page
    {
        private static Regex _decryptRegex = new Regex(@"ENC\[([^\[]*)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase );
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null || Session["SessionObject"].ToString() == "")
            {
                Services.Service2.KillSession();
            }

            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            
            if (sessionObject.LoggedInUser == null || String.IsNullOrEmpty(sessionObject.EncryptedProvidedPwd))
            {
                Services.Service2.KillSession();
            }

            ThinkgateUser user = sessionObject.LoggedInUser;
            var dp = DistrictParms.LoadDistrictParms();

            //frmSessionBridge.Action = AppSettings.SessionBridgeURL;
            frmSessionBridge.Action = dp.SessionBridgeUrl;
            UserID.Value = user.UserName;
            UserPage.Value = user.Page.ToString();
            Password.Value = Convert.ToString(Encryption.DecryptString(sessionObject.EncryptedProvidedPwd));
            ClientID.Value = (AppSettings.AppVirtualPath == "/") ? "TM2011" : AppSettings.AppVirtualPath;
            
            if (Request.QueryString["selectedReport"] != null && !String.IsNullOrEmpty(Request.QueryString["selectedReport"]))
            {
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["selectedReport"].ToString() + "' report", Request.QueryString["selectedReport"].ToString(), sessionObject.LoggedInUser.UserName);
            }

            ReturnURL.Value = _decryptRegex.Replace(Request.QueryString["ReturnURL"].ToString(),
                                                    match => Cryptography.GetDecryptedIDFromEncryptedValue(match.Groups[1].ToString(), sessionObject.LoggedInUser.CipherKey).ToString());
        }
    }
}