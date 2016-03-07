using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Core.Extensions;
using Thinkgate.Services.Contracts.Kentico;

namespace Thinkgate.Classes
{
    public class KenticoBusiness
    {
        private const string AdminUserName = "Admin";

        static public void AddUserAndRoles(string newStaffUserName)
        {
            var environmentParametersViewModel = new Thinkgate.Domain.Classes.EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            var clientId = environmentParametersViewModel.ClientId;
            var kenticoEnabled = environmentParametersViewModel.IsKenticoEnabledSite;

            if (kenticoEnabled)
            {
                SqlParameterCollection parmsUersInRoles = new SqlCommand().Parameters;
                parmsUersInRoles.AddWithValue("ApplicationName", AppSettings.ApplicationName);
                parmsUersInRoles.AddWithValue("UserName", newStaffUserName);

                var findRolesForUser = ThinkgateDataAccess.FetchDataTable(AppSettings.ConnectionString,
                                                                     Thinkgate.Base.Classes.Data.StoredProcedures.ASPNET_USERS_IN_ROLES_GET_ROLES_FOR_USER,
                                                                     System.Data.CommandType.StoredProcedure,
                                                                     parmsUersInRoles);

                var roleservice = new RoleServiceProxy();
                var nameCalculator = new Thinkgate.Domain.Classes.KenticoNameCalculator();

                for (int i = 0; i < findRolesForUser.Rows.Count; i++)
                {
                    string newStaffRoleName = findRolesForUser.Rows[i]["RoleName"].ToString();
                    var kenticoUserName = nameCalculator.CalculateUserName(clientId, newStaffUserName);
                    var kenticoRoleName = nameCalculator.CalculateSystemGroupName(clientId, AdminUserName, newStaffRoleName);
                    var roleuser = new RoleUser();
                    roleuser.UserName = kenticoUserName;
                    roleservice.AddUsersToRole(kenticoRoleName, EnumerableExtensions.SingleItemAsEnumerable<RoleUser>(roleuser));
                }
            }
        }
    }
}