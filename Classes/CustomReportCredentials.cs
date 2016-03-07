using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Security;
using System.Security.Principal;

namespace Thinkgate.Classes
{
    [Serializable]
    public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {

        // local variable fornetwork credential.
        private string _UserName;
        private string _PassWord;
        private string _DomainName;
    
        public CustomReportCredentials(string UserName, string PassWord, string DomainName)
        { 
            _UserName = UserName;
            _PassWord = PassWord;
            _DomainName = DomainName; 
        }

        public WindowsIdentity ImpersonationUser
        {
            get 
            { 
                return null; // not use ImpersonationUser
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                // use NetworkCredentials
                return new NetworkCredential(_UserName,_PassWord,_DomainName);
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            // not use FormsCredentials unless you have implements acustom autentication.
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }

}