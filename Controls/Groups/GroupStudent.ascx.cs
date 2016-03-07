using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Services.Contracts.Groups;

namespace Thinkgate.Controls.Groups
{
    /// <summary>
    /// This tile displays groups that a student is a member of.  It summarizes the group data by 
    /// diplaying the group name, number of members, and date created for the group.
    /// </summary>
    public partial class GroupStudent : TileControlBase
    {
        #region Variables

        readonly GroupsProxy _groupProxy = new GroupsProxy();
        private List<GroupDataContract> _groupsWithStudents;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handler for the PageLoad event.  It handles verifying that the user has permission to view this control
        /// then pulls in the EnvironmentParametersViewModel for use within the Page.
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">The EventArgs object passed to the event handler</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyUserAccess();
        }

        /// <summary>
        /// This event handler handles the Rad Grid "OnNeedDataSource" event.  It retreives the 
        /// groups that the current student belongs to and binds that data set to the grid to display them.
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">GridNeedDataSourceEventArgs passed to the eventhandler</param>
        protected void LoadGroupsForCurrentStudent(object sender, GridNeedDataSourceEventArgs e)
        {
            var studentId = (int)Tile.TileParms.GetParm("studentId");

            if (_groupsWithStudents == null)
            {
                _groupsWithStudents = _groupProxy.GetGroupsForStudent(studentId,
                    DistrictParms.LoadDistrictParms().ClientID);
            }

            radGridStudentGroups.DataSource = _groupsWithStudents;
        }

        #endregion

        #region Private

        /// <summary>
        /// This method looks at the Roles of the Logged in User and makes sure that they have permission
        /// to view the data on this control.  If their permissions are not correct, they will be redirected to the
        /// UnAuthorizedAccess page.
        /// </summary>
        private void VerifyUserAccess()
        {
            var rolePortal =
                (RolePortal)
                    SessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);
            if (rolePortal != RolePortal.Teacher)
            {
                Server.Transfer(@"~/UnauthorizedAccess.aspx");
            }
        }

        #endregion
    }
}