using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Security;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Core.Identity;

namespace Thinkgate
{
    public static class ThinkgateIdentityContext
    {
        /// <summary>
        /// Based upon the currently logged in forms authentication user, set the the current logged in user to a new GenericPrincipal with SAML claims
        /// </summary>
        public static void SetUserIdentity(SessionObject sessionObject, HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return;

            var genericIdentity = new GenericIdentity(context.User.Identity.Name);
            genericIdentity.AddClaims(GetThinkgateSpecificClaims(sessionObject));

            var genericPrincipal = new GenericPrincipal(genericIdentity, Roles.GetRolesForUser());
            Thread.CurrentPrincipal = genericPrincipal;
            HttpContext.Current.User = genericPrincipal;

        }

        /// <summary>
        /// Using values from multiple sources, build custom Thinkgate claims
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Claim> GetThinkgateSpecificClaims(SessionObject sessionObject)
        {
            var primarySecurityRoleName = string.Empty;
            var primarySecurityRolePortalSelection = 0;
            var userId = GetUserId(sessionObject.LoggedInUser);
            var primarySecurityRole = GetPrimarySecurityRoleName(sessionObject.LoggedInUser);
            var userPage = GetUserPage(sessionObject);

            if (primarySecurityRole != null)
            {
                primarySecurityRoleName = primarySecurityRole.RoleName;
                primarySecurityRolePortalSelection = primarySecurityRole.RolePortalSelection;
            }

            return new List<Claim>
            {
                new Claim(ThinkgateClaimTypes.UserId, userId.ToString()),
                new Claim(ThinkgateClaimTypes.PrimarySecurityRole, primarySecurityRoleName),
                new Claim(ThinkgateClaimTypes.RolePortalSelection, primarySecurityRolePortalSelection.ToString(CultureInfo.InvariantCulture)),
                new Claim(ThinkgateClaimTypes.UserPage, userPage.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Locality, AppSettings.ClientID),
                new Claim(ClaimTypes.StateOrProvince, DistrictParms.LoadDistrictParms().State),
                new Claim(ThinkgateClaimTypes.SchoolYear, DistrictParms.LoadDistrictParms().Year),
                new Claim(ThinkgateClaimTypes.IsKenticoEnabledSite, DistrictParms.LoadDistrictParms().IsKenticoEnabledSite)
            };
        }

        private static int GetUserPage(SessionObject sessionObject)
        {

            if (sessionObject == null)
                return 0;

            if (sessionObject.LoggedInUser == null)
                return 0;

            return sessionObject.LoggedInUser.Page;
        }

        private static Guid GetUserId(ThinkgateUser thinkgateUser)
        {
            if (thinkgateUser == null)
                return Guid.Empty;

            return thinkgateUser.UserId;

        }

        private static ThinkgateRole GetPrimarySecurityRoleName(ThinkgateUser thinkgateUser)
        {
            if (thinkgateUser == null)
                return null;

            if (!thinkgateUser.Roles.Any())
                return null;

            var roles = thinkgateUser.Roles;
            return roles.First(w => w.RolePortalSelection == roles.Min(m => m.RolePortalSelection));

        }
    }
}