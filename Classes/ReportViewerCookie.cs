using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinkgate.Domain.Classes;
using Thinkgate.Base.Classes;
using System.Web.Security;
using System.Net;
using System.Configuration;
using Standpoint.Core.Classes;

namespace Thinkgate.Classes
{
    public class ReportViewerCookie
    {
        //Constants (Change to name)
        private const string EndpointName = "ReportingService2010Service";
        private const int serverIndex = 2;

        //Report Server Business Layer
        private ReportingServices ReportServerBL = null;

        //User Environment
        private EnvironmentParametersFactory UserEnvFactory = null;
        private EnvironmentParametersViewModel UserEnvParms = null;
        
        //Cookie Variables
        private string cookieName = null;
        private string cookieValue = null;
        private string cookiePath = null;
        private string cookieDomain = null;

        //Report Server URL Property
        public string reportServerURL { get; private set; }

        //Default Constructor
        public ReportViewerCookie()
        {
            UserEnvFactory = new EnvironmentParametersFactory(AppSettings.ConnectionStringName);
            UserEnvParms = UserEnvFactory.GetEnvironmentParameters();
            
            getServiceEndPoint();
        }

        //Generates Cookie to pass to Report Viewer control
        public Cookie getReportViewerCookie(HttpCookie E3cookie)
        {
            //Decrypt E3 Cookie
            FormsAuthenticationTicket E3authTicket = FormsAuthentication.Decrypt(E3cookie.Value);

            //Duplicate cookie and add custom client format (client - username)
            var DupeAuthTicket = new FormsAuthenticationTicket(E3authTicket.Version,
                                                                    string.Concat(UserEnvParms.ClientId, @"-", E3authTicket.Name),
                                                                    E3authTicket.IssueDate,
                                                                    E3authTicket.Expiration,
                                                                    E3authTicket.IsPersistent,
                                                                    UserEnvParms.ClientId,
                                                                    E3authTicket.CookiePath);

            //Logon to report server to get SSRS auth cookie
            ReportServerBL = new ReportingServices(UserEnvParms, FormsAuthentication.Encrypt(DupeAuthTicket));
            
            //Parse Cookie values from SSRS auth string
            setCookiePaths(ReportServerBL.LogonUserToReportingServer());

            //Build cookie for Report Viewer
            if (cookieName != null && cookieValue != null && cookieDomain != null)
            {
                Cookie ReportViewerCookie = new Cookie(cookieName, cookieValue, FormsAuthentication.Decrypt(cookieValue).CookiePath, cookieDomain);
                ReportViewerCookie.HttpOnly = true;
                return ReportViewerCookie;
            }
            else return null; //
        }

        //Returns SSRS reports that user has access to view
        public IEnumerable<Services.Contracts.SSRS.CatalogItem> getReportPaths(HttpCookie E3cookie)
        {
            if (ReportServerBL != null)
            {
                return ReportServerBL.GetReportList();
            }
            else
            {
                //Logon
                this.getReportViewerCookie(E3cookie);
                if (ReportServerBL != null)
                    return ReportServerBL.GetReportList();
                else return null; //Verify
            }
            
        }
        //Returns Encrypted report paths
        public IEnumerable<Services.Contracts.SSRS.CatalogItem> getEncryptedReportPaths(HttpCookie E3cookie)
        {
            if (ReportServerBL != null)
            {
                return ReportServerBL.GetReportList();
            }
            else
            {
                //Logon
                this.getReportViewerCookie(E3cookie);
                if (ReportServerBL != null)
                {
                    var list = ReportServerBL.GetReportList().ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Path = String.Concat("~/Record/SSRSReports.aspx?xID=",Encryption.EncryptString(list[i].Path));
                    }
                    return list;
                }
                else return null;
            }

        }
        //Parses cookie values from ssrs cookie string
        private void setCookiePaths(string _ssrsCookieString)
        {
            string[] cookieArray = null;
            
            //Create Cookie Parts from encrypted SSRS cookie
            cookieArray = _ssrsCookieString.Split(';');
            cookieName = cookieArray[0].Substring(0, cookieArray[0].IndexOf("="));
            int startPos = cookieArray[0].IndexOf("=") + 1; //Encrypted cookie value
            cookieValue = cookieArray[0].Substring(startPos, cookieArray[0].Length - startPos);

            //Decrypt SSRS cookie to get cookie path
            FormsAuthenticationTicket ssrsauthCookieTicket = FormsAuthentication.Decrypt(cookieValue);
            cookiePath = ssrsauthCookieTicket.CookiePath;

        }

        //Gets SSRS server name and path from web.config
        private void getServiceEndPoint()
        {
            //Variables
            System.ServiceModel.Configuration.ChannelEndpointElement _endpoint = null;
            string[] endpointArray = null;
            string endpointAddress = null;
            
            //Find SSRS servername in web.config to be used as Report Viewer cookie domain
            System.ServiceModel.Configuration.ClientSection wconfig = (System.ServiceModel.Configuration.ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
            
            foreach (System.ServiceModel.Configuration.ChannelEndpointElement endpoint in wconfig.Endpoints)
            {
                if (endpoint.Name.ToString() ==  EndpointName)
                {
                    _endpoint = endpoint;
                    break;
                }
                //Throw exception end point not found
            }

            //Find Report Server URL and Servername for Reportviewer cookie domain
            endpointAddress = _endpoint.Address.ToString();
            endpointArray = endpointAddress.Split('/');
            reportServerURL = endpointAddress.Substring(0, endpointAddress.LastIndexOf('/'));
            cookieDomain = endpointArray[serverIndex];
            
        }
        


    }
}