using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes
{
    public class MinimumPasswordRequirementHelper
    {

        public static string GetMinumumPasswordRequirementMsg()
        {
            StringBuilder sb = new StringBuilder(); ;
            DistrictParms dParms = DistrictParms.LoadDistrictParms();
    

            if(dParms.PasswordConfigurationRequired == "Yes")
            {
                sb.Append("<ul><li>Password must have minimum of " + dParms.PasswordMinLength + " and maximum of " + dParms.PasswordMaxLength + " characters. </li>");
               if (dParms.PasswordNumericAllowed == "1") 
               {
                   sb.Append("<li>Password must include at least one numerical character (0-9).</li>");
               }
               if (dParms.PasswordUpperCaseAllowed == "1") 
               {
                   sb.Append("<li>Password must include at least one upper case character (A-Z).</li>");
               }

               if (dParms.PasswordLowerCaseAllowed == "1") 
               {
                   sb.Append("<li>Password must include at least one lower case character (a-z).</li>");
               }

               if (dParms.PasswordSpecialCharAllowed == "1") 
               {
                   sb.Append("<li>Password must include at least one special character (! % ~!@#$&*-_).</li>");
               }

               sb.Append("</ul>");
            }
            return sb.ToString();
        }
    }
}