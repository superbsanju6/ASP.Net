using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Reports
{
    public partial class CredentialTrackingReport : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null || Tile.TileParms.GetParm("guid") == null) return;
            credentialTrackingReportFrame.Attributes["src"] = ResolveUrl("~/Controls/Reports/CredentialTrackingReportPage.aspx") + "?xID=" + Tile.TileParms.GetParm("guid").ToString();
        }
    }
}