using System.Web.UI;

namespace Thinkgate.Controls.Banner
{
    using System;
    using System.Web.Security;
    using Telerik.Web.UI;
    using Thinkgate.Base.Classes;
    using Thinkgate.Base.Enums;
    using Thinkgate.Classes;
    using Thinkgate.Interfaces;

    public partial class MainBanner : System.Web.UI.UserControl, IBannerControl
    {
        public void AddMenu(Banner.ContextMenu contextMenu, RadContextMenu menu)
        {
            switch (contextMenu)
            {
                case Banner.ContextMenu.Help:
                    break;
            }
        }

        public void HideContextMenu(Banner.ContextMenu contextMenu)
        {
         
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var sessionObject = (SessionObject)Session["SessionObject"];
            var distParms = DistrictParms.LoadDistrictParms();

            bool isLCO = false;
            if (sessionObject.Elements_ActiveFolder != null)
            {
                if (sessionObject.LCOrole == EntityTypes.IMC || sessionObject.LCOrole == EntityTypes.RegionalCoordinator || sessionObject.LCOrole == EntityTypes.SectionChief || sessionObject.LCOrole == EntityTypes.LCOAdministrator)
                {
                    isLCO = true;
                }
            }
      
            RadMenuItem miHelpMenuItem;

            var configuredAuthMethod = (AuthMethod)Session["configuredAuthMethod"];
            if ((configuredAuthMethod == AuthMethod.SiteRouterSso) ||
                (configuredAuthMethod == AuthMethod.ShibbolethSso))
            {
                // Log Out for SSO must display LoggedOut.aspx page if browser window fails to close
                // This requires different OnClick handling than Log Out for non-SSO case
                SignOffLink.OnClientClick = "javaScript:SignOffButtonSso_ClientClick();";
            }

            if (sessionObject.Elements_ActiveFolder != null)
            {
                if (sessionObject.Elements_ActiveFolder.ToLower().Trim() == "course object")
                {
                    isLCO = true;
                    if (!AppSettings.IsIllinois)
                    {
                    HomeButton.Visible = false;
                    AccountLink.Visible = false;
                    HelpLink.Visible = false;
                    SignOffLink.Text = "Close";
                    SignOffLink.OnClientClick = "javaScript:window.close(); return false;";
                }
            }
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_ThinkgateTV))
            {
                miHelpMenuItem = new RadMenuItem("Thinkgate TV");
                miHelpMenuItem.Attributes.Add("action", "window");
                miHelpMenuItem.Attributes.Add("actionUrl", distParms.ThinkgateTVUrl);
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_ThinkgateUniversity))
            {
                miHelpMenuItem = new RadMenuItem("Thinkgate University");
                //miHelpMenuItem.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                //miHelpMenuItem.Value = miHelpMenuItem.ID;
                miHelpMenuItem.Attributes.Add("action", "window");
                miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/LumenixBridge.aspx"));
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_FAQ))
            {
                miHelpMenuItem = new RadMenuItem("FAQ");
                //miHelpMenuItem.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                //miHelpMenuItem.Value = miHelpMenuItem.ID;
                miHelpMenuItem.Attributes.Add("action", "window");
                miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/faq/Thinkgate_FAQ_Document.pdf"));
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_SupportCenter))
            {
                miHelpMenuItem = new RadMenuItem("Support Center");
                //miHelpMenuItem.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                //miHelpMenuItem.Value = miHelpMenuItem.ID;
                miHelpMenuItem.Attributes.Add("action", "customDialog");
                miHelpMenuItem.Attributes.Add("acctionName", "Support Center");
                miHelpMenuItem.Attributes.Add("actionTitle", "Support Center");
                miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "fastsql_v2_direct.asp?ID=7256|support_messages");
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_MessageCenter))
            {
                miHelpMenuItem = new RadMenuItem("Message Center");
                miHelpMenuItem.Attributes.Add("action", "customDialog");
                miHelpMenuItem.Attributes.Add("acctionName", "Message Center");
                miHelpMenuItem.Attributes.Add("actionTitle", "Message Center");
                miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/MessageCenter.aspx"));
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_ReferenceCenter))
            {
                miHelpMenuItem = new RadMenuItem("Reference Center");
                //miHelpMenuItem.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                //miHelpMenuItem.Value = miHelpMenuItem.ID;
                miHelpMenuItem.Attributes.Add("action", "customDialog");
                miHelpMenuItem.Attributes.Add("acctionName", "Reference Center");
                miHelpMenuItem.Attributes.Add("actionTitle", "Reference Center");
                
                 //TFS 4472: If Kentico is disabled use Legacy
                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CMSTreePathToReferences"]))
                    miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/SessionBridge.aspx") + "?ReturnURL=fastsql_v2_direct.asp?ID=E3_ReferenceCenter|reference_center&ppwp=yes");
                else miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/ReferenceCenter.aspx")); //Else Kentico Reference Center
                
                ctxHelpMenu.Items.Add(miHelpMenuItem); 
                
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Submit_Help_Request))
            {
                miHelpMenuItem = new RadMenuItem("Submit Help Request");
                //miHelpMenuItem.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                //miHelpMenuItem.Value = miHelpMenuItem.ID;
                miHelpMenuItem.Attributes.Add("action", "customDialog");
                miHelpMenuItem.Attributes.Add("actionName", "Submit Help Request");
                miHelpMenuItem.Attributes.Add("actionTitle", "Submit Help Request");
                miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/SubmitHelpRequest.aspx")); 
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_HaloForms))
            {
                miHelpMenuItem = new RadMenuItem("Calculate Halo Cost");
                //miHelpMenuItem.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                //miHelpMenuItem.Value = miHelpMenuItem.ID;
                miHelpMenuItem.Attributes.Add("action", "customDialog");
                miHelpMenuItem.Attributes.Add("actionName", "Calculate Halo Cost");
                miHelpMenuItem.Attributes.Add("actionTitle", "Calculate Halo Cost");
                miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/CalculateHaloCost.aspx"));
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (sessionObject.LoggedInUser.HasPermission(Permission.Menu_Help_HaloForms))
            {
                miHelpMenuItem = new RadMenuItem("Submit Halo Order");
                //miHelpMenuItem.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                //miHelpMenuItem.Value = miHelpMenuItem.ID;
                miHelpMenuItem.Attributes.Add("action", "customDialog");
                miHelpMenuItem.Attributes.Add("actionName", "Submit Halo Order");
                miHelpMenuItem.Attributes.Add("actionTitle", "Submit Halo Order");
                miHelpMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/SubmitHaloOrder.aspx"));
                ctxHelpMenu.Items.Add(miHelpMenuItem);
            }

            if (ctxHelpMenu.Items.Count == 0) HelpLink.Visible = false;

            // Task: 12405 - JDW    
            // RadMenuItem declaration stays out of conditional statement 
            RadMenuItem miAccountMenuItem;

            if (isLCO)
            {
                // Task: 12405 - JDW 
                #region ISLE environment test for MainBanner Configuration. Boolean variable declared in AppSettings
                if (!AppSettings.IsIllinois)
                {
                btnRefresh.Visible = true;
                HomeButton.Visible = false;
                HelpLink.Visible = false;
                }
                #endregion

                // Task: 12405 - JDW
                #region ISLE environment test for Account Menu Configuration.
                if (!AppSettings.IsIllinois)
                {
                miAccountMenuItem = new RadMenuItem("My Account");
                miAccountMenuItem.Attributes.Add("action", "customDialog");
                miAccountMenuItem.Attributes.Add("actionName", "My Account");
                miAccountMenuItem.Attributes.Add("actionTitle", "My Account");
                miAccountMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/Account/MyAccount.aspx"));
                ctxAccountMenu.Items.Add(miAccountMenuItem);
                }
                #endregion

                //addImpersonationMenuItem(distParms, sessionObject, miAccountMenuItem);
            }
            else
            {
                if (ctxAccountMenu.Items.Count == 0)
                {
                    #region Task: 12405 JDW: IL environment test, "My Account" never shows for any role in Illinois, 'My Profile' should always show
                    if (!AppSettings.IsIllinois)
                    {
                        miAccountMenuItem = new RadMenuItem("My Account");
                        miAccountMenuItem.Attributes.Add("action", "customDialog");
                        miAccountMenuItem.Attributes.Add("actionName", "My Account");
                        miAccountMenuItem.Attributes.Add("actionTitle", "My Account");
                        miAccountMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/Account/MyAccount.aspx"));
                        ctxAccountMenu.Items.Add(miAccountMenuItem);
                    }
                    #endregion

                    miAccountMenuItem = new RadMenuItem("My Profile");
                    miAccountMenuItem.Attributes.Add("action", "window");
                    miAccountMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/Record/Staff.aspx") + "?xID=" + Standpoint.Core.Classes.Encryption.EncryptInt(sessionObject.LoggedInUser.Page));
                    ctxAccountMenu.Items.Add(miAccountMenuItem);

                    //addImpersonationMenuItem(distParms, sessionObject, miAccountMenuItem);
                }
            }
                
            if (ctxAccountMenu.Items.Count == 0) AccountLink.Visible = false;
           
            /****************************************************************
            * Find the RadScriptManager control and insert service into it.
            * 
            * The RadScriptManager is in the Site.Master masterpage. 
            * Because that master page is used by so many pages, we don't 
            * want to add services to Site.Master's script manager.  The
            * pages that don't need the service don't need the extra overhead
            * that the service causes.  So we must find the scriptManager and
            * add the service to it from here.
            * **************************************************************/
            var pageScriptMgr = Page.Master.FindControl("RadScriptManager1") as RadScriptManager;
            var newSvcRef = new ServiceReference("~/Services/Service2.Svc");
            pageScriptMgr.Services.Add(newSvcRef);

            // Task: 12405 JDW 
            if (AppSettings.IsIllinois) { SetIsleMenuOptions(); }
            // Task: 27814 KNS 
            if (AppSettings.IsMassachusetts) { SetMassachusettsMenuOptions(); }
        }


        private void addImpersonationMenuItem(DistrictParms distParms, SessionObject sessionObject, RadMenuItem miAccountMenuItem)
        {
            if (distParms.ImpersonateUserAccess && sessionObject.LoggedInUser.HasPermission(Permission.Access_ImpersonateUserAccess))
            {
                miAccountMenuItem = new RadMenuItem("Impersonate User");
                miAccountMenuItem.Attributes.Add("action", "window");
                miAccountMenuItem.Attributes.Add("actionUrl", ResolveUrl("~/Account/UserImpersonation.aspx"));
                ctxAccountMenu.Items.Add(miAccountMenuItem);
            }
        }

        protected void HomeButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PortalSelection.aspx", true);
        }

        /// <summary>
        /// This will set up the MainBanner configurations options for Illinois. 
        /// In ISLE, no 'Home' link on the banner. 
        /// </summary>  
        protected void SetIsleMenuOptions()
        {
            HomeButton.Visible = false; // No 'Home' link on banner
            AccountLink.Visible = true;
            HelpLink.Visible = true;
        }

        /// <summary>
        /// This will set up the MainBanner configurations options for Massachusetts. 
        /// In Masschusetts, any 'Home' option should be a 'ET&L Home' option ANYWHERE in site.  
        /// </summary>  
        protected void SetMassachusettsMenuOptions()
        {
            HomeButton.Text = "ET&amp;L Home";  // 'Home' will be 'ET&L Home' for "MA"
        }

        /// <summary>
        /// This is the onClick handler for Log Out; It is only used if SSO is enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SignOffButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LoggedOut.aspx", true);
        }

        //protected void SignOffButton_Click(object sender, EventArgs e)
        //{
        //    Services.Service2.KillSession(true);
        //    Page.ClientScript.RegisterClientScriptBlock(typeof(MainBanner), "closeIt", "window.close(); ", true);
        //}
    }
}
