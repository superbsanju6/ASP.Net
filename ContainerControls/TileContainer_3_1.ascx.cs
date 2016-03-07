using System;
using Telerik.Web.UI;
using Thinkgate.Classes;

namespace Thinkgate.ContainerControls
{
    public partial class TileContainer_3_1 : Container
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NumberOfTilesPerPage = 3;
        }

        protected void RadDockLayout1_SaveDockLayout(object sender, DockLayoutEventArgs e)
        {
            DockPositionChanged(sender, e);
        }
    }
}
