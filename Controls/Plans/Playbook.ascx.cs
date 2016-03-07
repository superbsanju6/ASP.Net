using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;

namespace Thinkgate.Controls.Plans
{
    public partial class Playbook : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadPlaybookGrid();
            BtnAdd.Attributes["onclick"] = "window.open('" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "' + escape('fastsql_v2_direct.asp?ID=7266|search_documents_add&??x=Playbook&??action=add'), '_blank');";
            BtnAdd.Visible = UserHasPermission(Permission.Create_Playbook);
        }

        private void LoadPlaybookGrid()
        {
            grdPlaybooks.DataSource = Thinkgate.Base.Classes.Playbook.GetPlaybooks();
            grdPlaybooks.DataBind();
        }

        protected void grdPlaybooks_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = (GridDataItem)e.Item;
                HyperLink hyperLink = (HyperLink)dataItem["link"].Controls[0];
                hyperLink.NavigateUrl = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode("display.asp?key=7266&fo=basic display&rm=page&xID=" + dataItem["ID"].Text);
            }
        }
    }
}