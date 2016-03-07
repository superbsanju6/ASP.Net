using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Reports
{
    public class ReportHelper : TileControlBase
    {
        /// <summary>
        /// Set the logged-in user roles
        /// </summary>
        public List<ThinkgateRole> UserRoles { get; set; }


        /// <summary>
        ///     Disable the  item click through link for school portal and teacher portal selection
        /// </summary>
        /// <returns>
        ///     true  - if logged-in user selects school portal or teacher portal
        ///     false - if logged-in user selects other than school portal and teacher portal
        /// </returns>
        public bool DisableItemLinks()
        {
            // RolePortalSelection Value 2 indicates School portal and 3 indicates teacher portal
            if (UserRoles[0].RolePortalSelection == 2 || UserRoles[0].RolePortalSelection == 3)
            { return true; }
            else
            { return false; }
        }
    }
}