using Thinkgate.Classes;
using System;
using Thinkgate.Base.Classes;

namespace Thinkgate
{
    public partial class LumenixBridge : System.Web.UI.Page
    {
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

            //frmSessionBridge.Action = AppSettings.LumenixBridgeUrl;
            frmSessionBridge.Action = dp.LumenixBridgeUrl;
            UserID.Value = user.UserName;
            Password.Value = user.GetEncryptedPwd();
            ClientID.Value = (AppSettings.AppVirtualPath == "/") ? "TM2011" : AppSettings.AppVirtualPath;
            
            //ReturnURL.Value = Request.QueryString["ReturnURL"].ToString(); //Not implemented
        }
    }
}