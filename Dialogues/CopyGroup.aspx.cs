using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;

namespace Thinkgate.Dialogues
{
    public partial class CopyGroup : System.Web.UI.Page
    {
        private GroupViewModel _groupViewModel;
        private EnvironmentParametersViewModel _environmentParametersViewModel;
        private RolePortal _rolePortalId;

        protected void Page_Load(object sender, EventArgs e)
        {
            var session = (SessionObject)Session["SessionObject"];
            _environmentParametersViewModel = new EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            _rolePortalId = (RolePortal)session.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);
        }

        protected void Save(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            _groupViewModel = GetGroupViewModel();
            var groupViewModel = new GroupViewModel
            {
                DisplayName = txtName.Text,
                Description = txtDescription.Text,
                VisibilityType = _groupViewModel.VisibilityType
            };

            var grouping = new Grouping(_environmentParametersViewModel);
            Guid roleId = grouping.CopyGroup(groupViewModel, SessionObject.LoggedInUser.School, RoleId, _rolePortalId);
            string script = string.Format("CloseDialog('{0}');", roleId.ToString());
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeScript", script, true);
        }

        protected void GetDuplicateExists(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { args.IsValid = true; return; }
            var grouping = new Grouping(_environmentParametersViewModel);
            args.IsValid = !grouping.GetDuplicateExists(txtName.Text);
        }

        private SessionObject SessionObject
        {
            get { return (SessionObject)Session["SessionObject"]; }
        }

        private Guid RoleId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Request.QueryString["roleId"]))
                    throw new Exception("RoleId Missing; Expected in QueryString");
                return Guid.Parse(Request.QueryString["roleId"]);
            }
        }

        private GroupViewModel GetGroupViewModel()
        {
            var grouping = new Grouping(_environmentParametersViewModel);
            return grouping.GetGroupDetailForPortalEducator(_rolePortalId, SessionObject.LoggedInUser.School, RoleId);
        }
    }
}