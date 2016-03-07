using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Domain;
using Thinkgate.Domain.Classes;

namespace Thinkgate.Dialogues
{
    public partial class DeleteGroup : Page
    {
        private GroupViewModel _groupViewModel;
        private EnvironmentParametersViewModel _environmentParametersViewModel;
        private RolePortal _rolePortalId;
        private const string DisplayNameShortColumnName = "DisplayNameShortColumn";
        private const string DescriptionShortColumnName = "DescriptionShortColumn";
        private const string DisplayNameDataKey = "DisplayName";
        private const string DescriptionDataKey = "Description";

        protected void Page_Load(object sender, EventArgs e)
        {
            _environmentParametersViewModel =
                new EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            _rolePortalId =
                (RolePortal)
                SessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);

            _groupViewModel = GetGroupViewModel();
        }

        protected void PopulateGroupDetail(object sender, GridNeedDataSourceEventArgs e)
        {
            if (_groupViewModel == null)
                throw new Exception(string.Format("The RoleId:'{0}' does not exist or the currently logged on user is not allowed to see it", RoleId));
            
            grdDetail.DataSource = new List<GroupViewModel> { _groupViewModel };
            
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

        private void MarkGroupAsDeleted()
        {
            var grouping = new Grouping(_environmentParametersViewModel);
            grouping.DeleteGroup(_groupViewModel);
        }

        protected void OkClick(object sender, EventArgs e)
        {
            if (optYes.Checked)
            {
                MarkGroupAsDeleted();
                ScriptManager.RegisterStartupScript(Page, typeof (Page), "closeScript", "CloseDialog('Deleted');",
                                                    true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeScript", "CloseDialog('Cancel');",
                                                    true);
            }

            
        }

        protected void ApplyToolTips(object sender, GridItemEventArgs e)
        {
            if (!(e.Item is GridDataItem)) return;

            var gridItem = e.Item as GridDataItem;
            foreach (GridColumn column in grdDetail.MasterTableView.RenderColumns)
            {
                if (column is GridBoundColumn && column.UniqueName == DisplayNameShortColumnName)
                {
                    object displayName = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex][DisplayNameDataKey];
                    gridItem[column.UniqueName].ToolTip = displayName == null ? string.Empty : displayName.ToString();              
                }
                else if (column is GridBoundColumn && column.UniqueName == DescriptionShortColumnName)
                {
                    object description = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex][DescriptionDataKey];
                    gridItem[column.UniqueName].ToolTip = description == null ? string.Empty : description.ToString();
                }
            }
        }

    }
}