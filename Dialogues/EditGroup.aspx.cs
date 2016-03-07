using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using Thinkgate.Services.Contracts.Groups;

namespace Thinkgate.Dialogues
{
    /// <summary>
    /// This dialog allows the user to modify the name and/or description of a group
    /// </summary>
    public partial class EditGroup : Page
    {
        #region Constants

        private const String GroupIdParameterName = "groupId";

        #endregion

        #region Variables

        private GroupDataContract _groupDataContract;
        private readonly GroupsProxy _groupsProxy = new GroupsProxy();

        private SessionObject SessionObject
        {
            get
            {
                return (SessionObject)Session["SessionObject"];
            }
        }

        #endregion

        #region Properties

        private int GroupId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Request.QueryString[GroupIdParameterName]))
                {
                    throw new Exception("GroupId Missing; Expected in QueryString");
                }

                return Int32.Parse(Request.QueryString[GroupIdParameterName]);
            }
        }

        #endregion

        /// <summary>
        /// Fires when the page loads.  If the page is not being loaded by a postback
        /// then the data loads and UI initialization occurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadGroupDataContract();
                InitializeUserInterface();
            }
        }

        /// <summary>
        /// This method loads the _groupDataContract object that is used to populate the UI with data
        /// </summary>
        private void LoadGroupDataContract()
        {
            _groupDataContract = _groupsProxy.GetGroup(GroupId, DistrictParms.LoadDistrictParms().ClientID);
        }

        /// <summary>
        /// This method populates the UI with the appropriate data from the GroupDataContract object
        /// we load in the LoadGroupDataContract method
        /// </summary>
        private void InitializeUserInterface()
        {
            txtName.Text = _groupDataContract.Name;
            txtDescription.Text = _groupDataContract.Description;
        }

        /// <summary>
        /// This method fires when the OK button is clicked.  It saves the changes made to the currently displayed Group  
        /// </summary>
        /// <param name="sender">btnOK</param>
        /// <param name="e">EventArgs passed to the eventhandler</param>
        protected void Save(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                _groupsProxy.UpdateGroup(GroupId, txtName.Text, txtDescription.Text,
                    DistrictParms.LoadDistrictParms().ClientID);

                ScriptManager.RegisterStartupScript(Page, typeof (Page), "closeScript", "CloseDialog('Saved');", true);
            }
        }

        /// <summary>
        /// This eventhandler handles the OnServerValidate event fired from the "DupeValidator" object.
        /// It checks the text input into the txtName control and determines if a group by that name already exists
        /// </summary>
        /// <param name="source">The object that fired this event</param>
        /// <param name="args">ServerValidateEventArgs passed to the eventhandler</param>
        protected void CheckForDuplicateGroupName(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = !IsGroupNameDuplicate();
            }
        }

        private bool IsGroupNameDuplicate()
        {
            List<GroupDataContract> userGroups = _groupsProxy.GetGroupsForUser(SessionObject.LoggedInUser.Page,
                DistrictParms.LoadDistrictParms().ClientID);

            bool isNameDuplicate = false;

            foreach (GroupDataContract group in userGroups)
            {
                if (group.Name.ToLower().Trim() == txtName.Text.ToLower().Trim())
                {
                    if (group.ID != GroupId)
                    {
                        isNameDuplicate = true;
                    }
                }
            }

            return isNameDuplicate;
        }
    }
}