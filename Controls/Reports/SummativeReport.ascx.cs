using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Reports
{
    public partial class SummativeReport : TileControlBase
    {              
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null || Tile.TileParms.GetParm("guid") == null) return;
            summativeReportFrame.Attributes["src"] = ResolveUrl("~/Controls/Reports/SummativeReportPage.aspx") + "?xID=" + Tile.TileParms.GetParm("guid").ToString();
        }
    }
}