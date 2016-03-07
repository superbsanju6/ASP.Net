using System.Web.Hosting;
using CMS.Synchronization;

namespace Thinkgate
{
	using System;
	using System.Reflection;
	using System.Web;
	using Thinkgate.Base.Enums;
	using Thinkgate.Classes;
	using Thinkgate.Interfaces;

	public partial class SiteMaster : System.Web.UI.MasterPage
	{
		public SessionObject SessionObject;

		public Banner Banner = new Banner();

		public BannerType BannerType { get; set; }
		public IBannerControl BannerControl;
	    public string ClientSiteCss = GetClientCss("Site");

		protected void Page_Init(object sender, EventArgs e)
		{
			SessionObject = (SessionObject)Session["SessionObject"];
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.User.Identity.IsAuthenticated || Session.IsNewSession || Session == null || Session["SessionObject"] == null)
			{
				Services.Service2.KillSession();
				return;
			}
            // We are configured with sliding auth timeout window for non-SSO, so we need to manually enforce auth timeouts for SSO
		    if ((SessionObject != null) &&
                (SessionObject.IsSsoAuthTimeoutSpecified) &&
                (DateTime.Compare(DateTime.Now, SessionObject.SsoAuthTimeoutDateTime) >= 0))
            {
                Services.Service2.KillSession();
                return;               
		    }

			ConfigureHeaderSettings();

			//var bodyClass = SessionObject.BackgroundClass;
			//var bodyColor = string.Empty;

			bodyTag.Attributes.Add("class", "default");

			//if (bodyClass == "waterfall")
			//{
			//    bgimg.Visible = false;
			//    bgimg2.Visible = false;

			//}
			//else if (bodyClass == "radial")
			//{
			//    bgimg.Visible = false;
			//    bgimg2.Visible = true;

			//}
			//else if (bodyClass == "stripes")
			//{
			//    bgimg.Visible = true;
			//    bgimg2.Visible = true;
			//}

			//bodyColor = String.IsNullOrEmpty(SessionObject.BackgroundColor) ? "#28628F" : SessionObject.BackgroundColor;

			//bodyTag.Attributes.Add("class", bodyClass);
			//bodyTag.Style.Add("background-color", bodyColor);

			if (!string.IsNullOrEmpty(SessionObject.RedirectMessage))
			{
				ShowFatalRadNotification(SessionObject.RedirectMessage);
				SessionObject.RedirectMessage = null;
			}

			if (!string.IsNullOrEmpty(SessionObject.RecoverableRedirectMessage))
			{
				ShowGenericRadNotification(SessionObject.RecoverableRedirectMessage);
				SessionObject.RecoverableRedirectMessage = null;
			}

			//if (!Page.IsPostBack)
			//{
			//    lblLoggedInUser.Text = (Page.User != null && Page.User.Identity.IsAuthenticated
			//                                ? String.Format("Welcome {0}", Page.User.Identity.Name)
			//                                : lblLoggedInUser.Text);
			//    //hlAdministration.Visible = Page.User.IsInRole("Administrators");
			//}

			if (BannerType != Base.Enums.BannerType.None)
			{
				var control = LoadControl(string.Format("~/Controls/Banner/{0}Banner.ascx", BannerType));
				BannerControl = control as IBannerControl;
				if (BannerControl != null)
				{
					foreach (var contextMenu in Banner.Menu.Keys)
					{
						BannerControl.AddMenu(contextMenu, Banner.Menu[contextMenu]);
					}
				}

				PlaceHolderBanner.Controls.Add(control);
			}

			if (Assembly.GetExecutingAssembly() != null)
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				poweredByLogo.ToolTip = String.Format("Version: {0} | Dated: {1} | TGS: {2}",
					assembly.GetName().Version.ToString(),
					System.IO.File.GetLastWriteTime(assembly.Location).ToShortDateString(),
					Server.MachineName);
			}
		}

		private void ConfigureHeaderSettings()
		{
			string showLogoDisplaySetting = System.Configuration.ConfigurationManager.AppSettings.Get("ShowTgLogo");

			if (!string.IsNullOrEmpty(showLogoDisplaySetting))
			{
				bool showlogo = Standpoint.Core.Utilities.DataIntegrity.ConvertToBool(showLogoDisplaySetting);
				if (!showlogo)
				{
					poweredByLogo.Style.Add("display", "none");
				}
			}


			if (HttpContext.Current.Request.UserAgent != null)
			{
				var userAgent = HttpContext.Current.Request.UserAgent.ToLower();
				if (userAgent.Contains("ipad"))
				{
					mainDiv.Style.Add("padding-left", "45px");
					mainDiv.Style.Add("padding-right", "75px");
					headerDiv.Style.Add("margin-left", "45px");
				}
			}
		}

		protected void ShowGenericRadNotification(string message)
		{
			genericRadNotification.Title = "Notification";
			genericRadNotification.Text = message;
			genericRadNotification.Show();
		}

		public void ShowFatalRadNotification(string message)
		{
			fatalNotification.Title = "Fatal Error";
			fatalNotification.ContentIcon = "Warning";
			fatalNotification.TitleIcon = "Warning";
			fatalNotification.Text = message;
			fatalNotification.Show();
		}

		public new static string ResolveUrl(string originalUrl)
		{
			if (!string.IsNullOrEmpty(originalUrl) && '~' == originalUrl[0])
			{
				int index = originalUrl.IndexOf('?');
				string queryString = (-1 == index) ? null : originalUrl.Substring(index);
				if (-1 != index) originalUrl = originalUrl.Substring(0, index);
				originalUrl = VirtualPathUtility.ToAbsolute(originalUrl) + queryString;
			}

			return originalUrl;
		}

	    public static string GetClientCss(string cssName)
	    {
	        var clientId = ((HttpRuntime.AppDomainAppVirtualPath == "/") ? string.Empty : HttpRuntime.AppDomainAppVirtualPath + "/").Replace("/", "");
	        var clientState = KenticoHelper.GetKenticoMainFolderName(clientId);

	        if (string.IsNullOrEmpty(clientId) && string.IsNullOrEmpty(clientState)) return string.Empty;

	        var cssFileName = string.Empty;

            // check for client specific customization
            cssFileName = "~/Styles/Clients/" + clientState + "/" + clientId + "/" + cssName + ".css";
	        if (HostingEnvironment.VirtualPathProvider.FileExists(cssFileName))
	        {
	            return cssFileName;
	        }

            // if no client specific customization found, check for state customization
            cssFileName = "~/Styles/Clients/" + clientState + "/" + cssName + ".css";
            if (HostingEnvironment.VirtualPathProvider.FileExists(cssFileName))
            {
                return cssFileName;
            }

	        return string.Empty;
	    }
	}
}
