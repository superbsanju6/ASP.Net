using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Reports
{
    public partial class CompetencyTrackingReport : TileControlBase
    {              
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null || Tile.TileParms.GetParm("guid") == null) return;
			competencyTrackingReportFrame.Attributes["src"] = ResolveUrl("~/Controls/Reports/CompetencyTrackingReportPage.aspx") + "?xID=" + Tile.TileParms.GetParm("guid").ToString();
        }
    }
}