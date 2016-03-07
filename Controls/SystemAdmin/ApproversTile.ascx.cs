using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Domain.Facades;

namespace Thinkgate.Controls.SystemAdmin
{
    public partial class ApproversTile : TileControlBase
    {
        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            bool isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            SetFilterVisibility();

            BtnAdd.Visible = true;

            if (!isPostBack)
            {
                LoadDropDownLevels();
                LoadDropDownElements();
            }

            LoadApprovers();
        }

        private void LoadDropDownLevels()
        {
            
        }

        private void LoadDropDownElements()
        {
            
        }

        private void LoadApprovers()
        {
            SystemAdminFacade facade = new SystemAdminFacade(AppSettings.ConnectionString);

            IList<ThinkgateUser> approvers = facade.GetApprovers();

            lbx.DataSource = approvers;
            lbx.DataBind();
            lbx.Visible = approvers.Count > 0;
            pnlNoResults.Visible = approvers.Count == 0;
        }

        protected void SetFilterVisibility()
        {
            cmbLevel.Visible = true;
            cmbElement.Visible = true;
        }

        protected void lbxList_ItemDataBound(Object sender, RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            ThinkgateUser user = (ThinkgateUser)(listBoxItem).DataItem;

            HyperLink hlkApprover = (HyperLink)listBoxItem.FindControl("lnkApprover");
            hlkApprover.Text = user.UserFullName;
        }
    }
}