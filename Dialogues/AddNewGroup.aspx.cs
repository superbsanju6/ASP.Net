using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using Thinkgate.Services.Contracts.Groups;

namespace Thinkgate.Dialogues
{
    /// <summary>
    /// This Dialog window opens when the "Add New" link is clicked on the Grouping Tile.
    /// It has two input controls where a user can enter a name for the group and a description
    /// of the group.
    /// </summary>
    public partial class AddNewGroup : Page
    {
        #region Variables

        private EnvironmentParametersViewModel _environmentParametersViewModel;
        private readonly GroupsProxy _groupsProxy = new GroupsProxy();
        private SessionObject _sessionObject;

        #endregion

        #region Event Handlers

        /// <summary>
        /// This event handler catches the event that fires when this Dialog is loaded.
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">EventArgs passed to the eventhandler</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _environmentParametersViewModel = new EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            _sessionObject = (SessionObject)Session["SessionObject"];

            if (!IsPostBack)
            {
                txtName.Focus();
            }
        }

        /// <summary>
        /// This eventhandler handles the OnClientClicking event fired from btnOK.
        /// Pending successful page validation the code then creates a group from the data entered into
        /// the UI.
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">EventArgs passed to the eventhandler</param>
        protected void Save(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                _groupsProxy.CreateGroup(txtName.Text, txtDescription.Text, _sessionObject.LoggedInUser.Page,
                    DistrictParms.LoadDistrictParms().ClientID);
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeScript", "CloseDialog('Saved');", true);
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
                var grouping = new Grouping(_environmentParametersViewModel);
                args.IsValid = !grouping.GetDuplicateExists(txtName.Text);
            }
        }

        #endregion
    }
}