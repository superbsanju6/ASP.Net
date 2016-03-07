using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using System.Data.SqlClient;

namespace Thinkgate
{
	public partial class CTExml : LoginPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string _u = !String.IsNullOrEmpty(Request["u"]) ? Request["u"] : string.Empty;
			string _p = !String.IsNullOrEmpty(Request["p"]) ? Request["p"] : string.Empty;
			string _x = !String.IsNullOrEmpty(Request["x"]) ? Request["x"] : string.Empty;
			string _teid = !String.IsNullOrEmpty(Request["teid"]) ? Request["teid"] : string.Empty;

			if (_teid != string.Empty && _u != string.Empty && _x == "EI")
			{
				GetExternalTestEvent(_teid);
			}
			else if (_u != string.Empty && _x == "Halo")
			{
				string userPage = CheckLogin(_u, _p);
				if (!string.IsNullOrEmpty(userPage) && userPage != "0")
				{
					Response.Write(SignInToLegacy(_u, _p));
				}
				else
				{
					Response.Write("Signin failed.");
				}
			}
			else if (_u != string.Empty && _x == "RenLe@rnP@ds1")
			{
				string userPage = CheckLogin(_u, _p);
				Response.Write(userPage);
			}
            else if (_x == "check")
            {
                Response.Write("found");
            }
		}

		private string CheckLogin(string username, string password)
		{
			bool _isUserAuthenticated = false;
			string userPage = "0";

			DistrictParms DParms = DistrictParms.LoadDistrictParms();

			if (DParms.LDAPEnabled)
			{
				TgLdapResponce tgLdapResponse = ValidateLDAPUser(username, password);
				_isUserAuthenticated = tgLdapResponse.IsLDAPAuthenticated;
			}
			else
			{
				_isUserAuthenticated = Membership.ValidateUser(username, password);
			}

			if (_isUserAuthenticated)
			{
				MembershipUser mUser = Membership.GetUser(username);
				SessionObject obj = (SessionObject)Session["SessionObject"];
				if (obj == null)
				{
					obj = new Classes.SessionObject();
					Session["SessionObject"] = obj;
					Session.Timeout = (AppSettings.SessionTimeout > 0 ? AppSettings.SessionTimeout : 20);
				}

				drGeneric_String_String gi = new drGeneric_String_String();
				Session["GlobalInputs"] = gi;
				obj.GlobalInputs = gi;
				MembershipUser user = Membership.GetUser(username);
				if (user == null) return "0";

				obj.LoggedInUser = new ThinkgateUser(user);
				obj.EncryptedProvidedPwd = Standpoint.Core.Classes.Encryption.EncryptString(password);
				gi.Add("UserID", obj.LoggedInUser.UserId.ToString());
				gi.Add("UserPage", obj.LoggedInUser.Page.ToString());
				gi.Add("UserName", obj.LoggedInUser.UserName);

				Session["SessionObject"] = obj;
				FormsAuthentication.SetAuthCookie(username, true);

				if (obj.LoggedInUser.Page != 0)
				{
					userPage = obj.LoggedInUser.Page.ToString();
				}
			}

			return userPage;
		}

		private string SignInToLegacy(string username, string password)
		{
			VersaITv2r5.Services legacyService = new VersaITv2r5.Services();

			DistrictParms DParms = DistrictParms.LoadDistrictParms();
			SessionObject SessionObject = (SessionObject)Session["SessionObject"];
			string ArenaName = DParms.ArenaName;
			string ArenaDBConnect = DParms.ArenaDBConnect;
			string ArenaLogPath = DParms.ArenaLogPath;
			string ArenaXMLFilePath = DParms.ArenaXMLFilePath;
			string ArenaXSLDirectory = Server.MapPath(DParms.ArenaXSLDirectory);
			int ArenaMenuPage = DParms.ArenaMenuPage;
			int ArenaDefaultFormatProfilePage = DParms.ArenaDefaultFormatProfilePage;
			int ArenaDesignFormatProfilePage = DParms.ArenaDesignFormatProfilePage;
			int ArenaMaxSearchResults = DParms.ArenaMaxSearchResults;
			int ArenaGuestPage = DParms.ArenaGuestPage;
			string ArenaGuestStartMenu = DParms.ArenaGuestStartMenu;
			string ArenaUploadDirectory = Server.MapPath(DParms.ArenaUploadDirectory);
			int ArenaControlApplPage = DParms.ArenaControlApplPage;
			int ArenaPersonalApplPage = DParms.ArenaPersonalApplPage;
			string ArenaQueueDBConnect = DParms.ArenaQueueDBConnect;
			string ArenaUploadWebDirectory = Server.MapPath(DParms.ArenaUploadWebDirectory);
			string ArenaImageWebDirectory = Server.MapPath(DParms.ArenaImageWebDirectory);
			string ArenaScriptLogPath = DParms.ArenaScriptLogPath;
			string ArenaSecurity = DParms.ArenaSecurity;
			int ArenaSessionTimeout = DParms.ArenaSessionTimeout;
			string ArenaReptTemplDirectory = Server.MapPath(DParms.ArenaReptTemplDirectory);
			string ArenaDTSDirectory = Server.MapPath(DParms.ArenaDTSDirectory);
			string ArenaReportsManager = DParms.ArenaReportsManager;
			string ArenaTempDirectory = Server.MapPath(DParms.ArenaTempDirectory);
            if (string.IsNullOrEmpty(legacyService.GetArenaName()))
            {
                legacyService.InitArena(ArenaName, ArenaDBConnect, ArenaLogPath, ArenaXMLFilePath, ArenaXSLDirectory, ArenaMenuPage, ArenaDefaultFormatProfilePage, ArenaDesignFormatProfilePage,
                                        ArenaMaxSearchResults, ArenaGuestPage, ArenaGuestStartMenu, ArenaUploadDirectory, ArenaControlApplPage, ArenaPersonalApplPage, ArenaQueueDBConnect, ArenaUploadWebDirectory,
                                        ArenaImageWebDirectory, ArenaScriptLogPath, ArenaSecurity, ArenaSessionTimeout, ArenaReptTemplDirectory, ArenaDTSDirectory, ArenaReportsManager, ArenaTempDirectory, "", null);
            }

            int sessID = 0;

            if (Request.Cookies[ArenaName] == null)
            {
                sessID = legacyService.SessionRefresh(0);
                Response.Cookies.Add(new HttpCookie(sessID.ToString()));
            }
            else
            {
                sessID = Convert.ToInt32(Request.Cookies[ArenaName].Value);
                legacyService.SessionRefresh(sessID);
            }

            if (legacyService.UserLogin(username, SessionObject.LoggedInUser.GetLegacyEncryptedPwd(), Request.ServerVariables["REMOTE_ADDR"], "ssl=no"))
            {
                sessID = legacyService.SessionSave();
                Response.Cookies[ArenaName].Value = sessID.ToString();
            }

            string returnValue = legacyService.SubServices.ctlMessage;
            legacyService = null;
			return returnValue;
		}

		private void GetExternalTestEvent(string testeventid)
		{
			SqlParameterCollection parms = new SqlCommand().Parameters;
			parms.AddWithValue("TestEventID", testeventid);

			System.Data.DataRow dr = ThinkgateDataAccess.FetchDataRow(AppSettings.ConnectionString,
											 "TM_ExternalTestEvent",
											 System.Data.CommandType.StoredProcedure,
											 parms);

			Response.Write(dr["testevents"]);
		}
	}
}