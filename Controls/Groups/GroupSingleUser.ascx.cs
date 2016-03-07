using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.Groups;

namespace Thinkgate.Controls.Groups
{
    /// <summary>
    /// This UserControl displays collections of GroupWithStudentCountViewModel objects.  It provides entry points for both 
    /// adding new groups as well as navigating to the summary views of any group displayed.
    /// </summary>
    public partial class GroupSingleUser : TileControlBase
    {
        #region Variables

        readonly GroupsProxy _groupProxy = new GroupsProxy();
        private List<GroupDataContract> _groupsWithStudents;

        #endregion

        #region EventHandlers

        /// <summary>
        /// This event handler catches the event that fires when this control is loaded.
        /// It performs the following initialization steps:
        /// 1. Sets an enumeration representing the portal that the page was accessed from
        /// 2. Instantiates a new EnvironmentParametersViewModel object that contains various system parameters
        /// 3. Handles data loading and data binding if a post-back occurs 
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">EventArgs passed to the event handler</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var ajaxManager = RadAjaxManager.GetCurrent(Page);
            ajaxManager.AjaxRequest += RefreshAfterSave;

            if (IsPostBack)
            {
                LoadGroups(null, null);
                RefreshAfterSave(null, null);
            }
        }

        /// <summary>
        /// This eventhandler calls Rebind() on the RadGrid.  This forces the grid to fire the NeedDataSource event
        /// and then rebinds the data 
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">EventArgs passed to the event handler</param>
        private void RefreshAfterSave(object sender, AjaxRequestEventArgs e)
        {
            grdSingleUserMine.Rebind();
        }

        /// <summary>
        /// Populates the _groupsWithStudents variable with the appropriate Group objects
        /// and sets the data source for the RadGrid
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">EventArgs passed to the event handler</param>
        protected void LoadGroups(object sender, GridNeedDataSourceEventArgs e)
        {
            if (_groupsWithStudents == null)
            {
                _groupsWithStudents = _groupProxy.GetGroupsForUser(SessionObject.LoggedInUser.Page,
                    DistrictParms.LoadDistrictParms().ClientID);
            }
             _groupsWithStudents.Sort(new Comparison<GroupDataContract>((x, y) => DateTime.Compare(x.ModifiedDate,y.ModifiedDate)));
             _groupsWithStudents.Reverse();
             grdSingleUserMine.DataSource = _groupsWithStudents;
        }

        #endregion
    }
}