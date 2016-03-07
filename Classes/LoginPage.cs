using System;
using System.Reflection;
using Standpoint.Core.Data;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Thinkgate.Core.Extensions;
using Thinkgate.ExceptionHandling;
using System.Data.SqlClient;
using System.Data;


namespace Thinkgate.Classes
{
    public class LoginPage : System.Web.UI.Page
    {
        public static DistrictParms DParms = DistrictParms.LoadDistrictParms();
        public string GetCurrentLDAPUser()
        {
            return User.Identity.Name;
        }

        public TgLdapResponce ValidateLDAPUser(string username, string pwd)
        {
            TgLdapResponce tgLdapResponse = new TgLdapResponce();

            if (pwd.Length == 0)
            {
                tgLdapResponse.ExceptionMessage = "Please specify a password";
                tgLdapResponse.IsErrored = true;
                tgLdapResponse.IsLDAPAuthenticated = false;
                return tgLdapResponse;
            }

            using (LdapConnection con = new LdapConnection(new LdapDirectoryIdentifier(DParms.LDAPDomain, DParms.LDAPDomainPort)))
            {
                con.SessionOptions.SecureSocketLayer = DParms.LDAPUseSSL;
                if (DParms.LDAPUseSSLCert)
                {
                    con.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(ServerCallback);
                }

                NetworkCredential theNetworkCredentials = new NetworkCredential();
                theNetworkCredentials.UserName = DParms.LDAPUseNetworkAccount ? DParms.LDAPNetworkCredentialsUsername : username;
                theNetworkCredentials.Password = DParms.LDAPUseNetworkAccount ? Standpoint.Core.Classes.Encryption.DecryptString(DParms.LDAPNetworkCredentialsPassword) : pwd;
                con.Credential = theNetworkCredentials;

                //authTypes = Anonymous, Basic, Negotiate, Ntlm, Digest, Sicily, Dpa, Msn, External, Kerberos
                con.AuthType = (AuthType)Enum.Parse(typeof(AuthType), DParms.LDAPAuthType);
                //protocolVersions = 2, 3
                con.SessionOptions.ProtocolVersion = DParms.LDAPProtocolVersion;
                //ReferralChasingTypes = None, Subordinate, External, All 
                con.SessionOptions.ReferralChasing = (ReferralChasingOptions)Enum.Parse(typeof(ReferralChasingOptions), DParms.LDAPReferralChasing);

                try
                {
                    con.Bind();
                }
                catch (LdapException ex)
                {
                    string errorMessage = "LDAP Exception: " + ex.Message;
                    ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, errorMessage, ex.ToString());
                    tgLdapResponse.ExceptionMessage = errorMessage;
                    tgLdapResponse.IsErrored = true;
                    tgLdapResponse.IsLDAPAuthenticated = false;
                    return tgLdapResponse;
                }
                catch (DirectoryOperationException ex)
                {
                    string errorMessage = "Directory Operation Exception: " + ex.Message;
                    ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, errorMessage, ex.ToString());
                    tgLdapResponse.ExceptionMessage = errorMessage;
                    tgLdapResponse.IsErrored = true;
                    tgLdapResponse.IsLDAPAuthenticated = false;
                    return tgLdapResponse;
                }

                try
                {
                    SearchRequest request = new SearchRequest(DParms.LDAPRootDN, String.Format(DParms.LDAPDirectoryFilter, username), SearchScope.Subtree);
                    SearchResponse response = (SearchResponse)con.SendRequest(request);

                    if (response.Entries.Count == 0)
                    {
                        string errorMessage = "No such username.";
                        ThinkgateEventSource.Log.ApplicationWarning(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, errorMessage);
                        tgLdapResponse.ExceptionMessage = errorMessage;
                        tgLdapResponse.IsErrored = true;
                        tgLdapResponse.IsLDAPAuthenticated = false;
                        return tgLdapResponse;
                    }
                    else
                    {
                        SearchResultEntry entry = response.Entries[0];
                        con.Credential = new NetworkCredential(con.AuthType == AuthType.Basic ? entry.DistinguishedName : username, pwd);
                        con.Bind();

                        // If we get this far without an exception, the username and
                        // password are valid. We can now use a SearchRequest to search
                        // for group membership etc, but that's out of scope for this
                        // example.
                    }
                }
                catch (DirectoryOperationException ex)
                {
                    string errorMessage = "Invalid root DN / search filter<br/>" + ex.Response + "<br/>" + ex.ToString();
                    ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, errorMessage, ex.ToString());
                    tgLdapResponse.ExceptionMessage = "Directory Operation Exception: " + ex.Message;
                    tgLdapResponse.IsErrored = true;
                    tgLdapResponse.IsLDAPAuthenticated = false;
                    return tgLdapResponse;
                }
                catch (LdapException ex)
                {
                    ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Invalid password", ex.ToString());
                    tgLdapResponse.ExceptionMessage = ex.Message;
                    tgLdapResponse.IsErrored = true;
                    tgLdapResponse.IsLDAPAuthenticated = false;
                    return tgLdapResponse;
                }
                catch (Exception ex)
                {
                    ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, ex.Message, ex.ToString());
                    tgLdapResponse.ExceptionMessage = ex.Message;
                    tgLdapResponse.IsErrored = true;
                    tgLdapResponse.IsLDAPAuthenticated = false;
                    return tgLdapResponse;
                }
            }

            tgLdapResponse.ExceptionMessage = null;
            tgLdapResponse.IsErrored = false;
            tgLdapResponse.IsLDAPAuthenticated = true;
            return tgLdapResponse;
        }


        public bool ServerCallback(LdapConnection connection, X509Certificate certificate)
        {
            try
            {
                X509Certificate expectedCert =
                    X509Certificate.CreateFromCertFile(DParms.LDAPSSLCertPath);

                if (expectedCert.Equals(certificate))
                {
                    return true;
                }
                else
                {
                    // certificate.ToString(true) provides verbose information about the certificate

                    string errorMessage =
                        String.Format(
                        "Certificate provided does not match certificate returned by server: {0}",
                        certificate.ToString(true));

                    ThinkgateEventSource.Log.ApplicationWarning(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, errorMessage);


                    return false;
                }
            }
            catch (Exception ex)
            {
                ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Cannot validate certificate: " + ex.Message, ex.ToString());
                return false;
            }
        }

        protected void LogUserLogin(string username, bool succesfulLogin)
        {
            DataCriteria criteria = ThinkgateDataAccess.GetDataCriteria(ThinkgateDataAccess.criteriaType.OperationsDataCriteria);
	        string llfl = AppSettings.LoggingLevelForLogins;
	        if (StringExtensions.IsNullOrEmpty(llfl))
	        {
		        llfl = "simple";
	        }
	        else
	        {
		        llfl = llfl.ToLower();
	        }
			switch (llfl)
            {
                case "simple":
                    criteria.CreateCommand = "Security_LoginDetailsCreateSimple";
                    criteria.InputParameters = new object[]
					                           	{
					                           		username,
					                           		succesfulLogin,
					                           		String.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ? Request.
					                           		ServerVariables["REMOTE_ADDR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"],
					                           		Request.Browser.Browser,
					                           		Request.Browser.Version
					                           	};
                    break;
                case "verbose":
                    criteria.CreateCommand = "Security_LoginDetailsCreateVerbose";
                    criteria.InputParameters = new object[]
												{
													username,
													succesfulLogin,
													String.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ? Request.ServerVariables["REMOTE_ADDR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"],
													Request.Browser.Browser,
													Request.Browser.Version,
													Request.Browser.AOL,
													Request.Browser.ActiveXControls,
													Request.Browser.BackgroundSounds,
													Request.Browser.Beta,
													Request.Browser.CDF,
													Request.Browser.CanCombineFormsInDeck,
													Request.Browser.CanInitiateVoiceCall,
													Request.Browser.CanRenderAfterInputOrSelectElement,
													Request.Browser.CanRenderEmptySelects,
													Request.Browser.CanRenderInputAndSelectElementsTogether,
													Request.Browser.CanRenderMixedSelects,
													Request.Browser.CanRenderOneventAndPrevElementsTogether,
													Request.Browser.CanRenderPostBackCards,
													Request.Browser.CanRenderSetvarZeroWithMultiSelectionList,
													Request.Browser.CanSendMail,
													Request.Browser.ClrVersion.ToString(),
													Request.Browser.Cookies,
													Request.Browser.Crawler,
													Request.Browser.DefaultSubmitButtonLimit.ToString(),
													Request.Browser.EcmaScriptVersion.ToString(),
													Request.Browser.Frames,
													Request.Browser.GatewayMajorVersion.ToString(),
													Request.Browser.GatewayMinorVersion.ToString(),
													Request.Browser.GatewayVersion,
													Request.Browser.HasBackButton,
													Request.Browser.HidesRightAlignedMultiselectScrollbars,
													Request.Browser.Id,
													Request.Browser.InputType,
													Request.Browser.IsColor,
													Request.Browser.IsMobileDevice,
													Request.Browser.JScriptVersion.ToString(),
													Request.Browser.JavaApplets,
													Request.Browser.MSDomVersion.ToString(),
													Request.Browser.MajorVersion.ToString(),
													Request.Browser.MaximumHrefLength.ToString(),
													Request.Browser.MaximumRenderedPageSize.ToString(),
													Request.Browser.MaximumSoftkeyLabelLength.ToString(),
													Request.Browser.MinorVersion.ToString(),
													Request.Browser.MinorVersionString,
													Request.Browser.MobileDeviceManufacturer,
													Request.Browser.MobileDeviceModel,
													Request.Browser.NumberOfSoftkeys.ToString(),
													Request.Browser.Platform,
													Request.Browser.PreferredImageMime,
													Request.Browser.PreferredRenderingMime,
													Request.Browser.PreferredRenderingType,
													Request.Browser.PreferredRequestEncoding,
													Request.Browser.PreferredResponseEncoding,
													Request.Browser.RendersBreakBeforeWmlSelectAndInput,
													Request.Browser.RendersBreaksAfterHtmlLists,
													Request.Browser.RendersBreaksAfterWmlAnchor,
													Request.Browser.RendersBreaksAfterWmlInput,
													Request.Browser.RendersWmlDoAcceptsInline,
													Request.Browser.RendersWmlSelectsAsMenuCards,
													Request.Browser.RequiredMetaTagNameValue,
													Request.Browser.RequiresAttributeColonSubstitution,
													Request.Browser.RequiresContentTypeMetaTag,
													Request.Browser.RequiresControlStateInSession,
													Request.Browser.RequiresDBCSCharacter,
													Request.Browser.RequiresHtmlAdaptiveErrorReporting,
													Request.Browser.RequiresLeadingPageBreak,
													Request.Browser.RequiresNoBreakInFormatting,
													Request.Browser.RequiresOutputOptimization,
													Request.Browser.RequiresPhoneNumbersAsPlainText,
													Request.Browser.RequiresSpecialViewStateEncoding,
													Request.Browser.RequiresUniqueFilePathSuffix,
													Request.Browser.RequiresUniqueHtmlCheckboxNames,
													Request.Browser.RequiresUniqueHtmlInputNames,
													Request.Browser.RequiresUrlEncodedPostfieldValues,
													Request.Browser.ScreenBitDepth.ToString(),
													Request.Browser.ScreenCharactersHeight.ToString(),
													Request.Browser.ScreenCharactersWidth.ToString(),
													Request.Browser.ScreenPixelsHeight.ToString(),
													Request.Browser.ScreenPixelsWidth.ToString(),
													Request.Browser.SupportsAccesskeyAttribute,
													Request.Browser.SupportsBodyColor,
													Request.Browser.SupportsBold,
													Request.Browser.SupportsCacheControlMetaTag,
													Request.Browser.SupportsCallback,
													Request.Browser.SupportsCss,
													Request.Browser.SupportsDivAlign,
													Request.Browser.SupportsDivNoWrap,
													Request.Browser.SupportsEmptyStringInCookieValue,
													Request.Browser.SupportsFontColor,
													Request.Browser.SupportsFontName,
													Request.Browser.SupportsFontSize,
													Request.Browser.SupportsIModeSymbols,
													Request.Browser.SupportsImageSubmit,
													Request.Browser.SupportsInputIStyle,
													Request.Browser.SupportsInputMode,
													Request.Browser.SupportsItalic,
													Request.Browser.SupportsJPhoneMultiMediaAttributes,
													Request.Browser.SupportsJPhoneSymbols,
													Request.Browser.SupportsQueryStringInFormAction,
													Request.Browser.SupportsRedirectWithCookie,
													Request.Browser.SupportsSelectMultiple,
													Request.Browser.SupportsUncheck,
													Request.Browser.SupportsXmlHttp,
													Request.Browser.Tables,
													Request.Browser.Type,
													Request.Browser.UseOptimizedCacheKey,
													Request.Browser.VBScript,
													Request.Browser.W3CDomVersion.ToString(),
													Request.Browser.Win16,
													Request.Browser.Win32
													};
                    break;
                default: //Same as simple for now. DSS 20111205
                    criteria.CreateCommand = "Security_LoginDetailsCreateSimple";
                    criteria.InputParameters = new object[]
					                           	{
					                           		username,
					                           		succesfulLogin,
					                           		String.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ? Request.
					                           		ServerVariables["REMOTE_ADDR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"],
					                           		Request.Browser.Browser,
					                           		Request.Browser.Version
					                           	};
                    break;

            }

            DataResult result = ThinkgateDataAccess.InsertRecord(criteria);

        }

        protected int LogUserLoginDetails(string username, string password, bool succesfulLogin)
        {
            int ID = 0;
             DataCriteria criteria = ThinkgateDataAccess.GetDataCriteria(ThinkgateDataAccess.criteriaType.OperationsDataCriteria);
             criteria.CreateCommand = "SaveUserLoggingDetails";
                    criteria.InputParameters = new object[]
					                           	{
					                           		username,
                                                    password,
					                           		succesfulLogin,                                                    
					                           		String.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ? Request.ServerVariables["REMOTE_ADDR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"]					                           	    
					                           	};

                    criteria.OutputParameters = new object[] {"ID"};

            DataResult result = ThinkgateDataAccess.InsertRecord(criteria);

            return Convert.ToInt32(result.OutputParamValues["ID"]);
        }

        protected void UpdateLogUserLoginDetails(int ID, string Description,bool isSuccess)
        {


            SqlParameterCollection parms = new SqlCommand().Parameters;
            parms.AddWithValue("ID", ID);
            parms.AddWithValue("Description", Description);
            parms.AddWithValue("isSuccess", isSuccess);

            ThinkgateDataAccess.ExecuteProcedure(AppSettings.ConnectionString,
                                                 "UpdateUserLoggingDetails",
                                                 CommandType.StoredProcedure,
                                                 parms);


           

      
        }

    }
}