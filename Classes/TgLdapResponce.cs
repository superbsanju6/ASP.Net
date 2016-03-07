using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinkgate.Classes
{
    public class TgLdapResponce
    {
        public bool IsLDAPAuthenticated { get; set; }
        public bool IsErrored { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
