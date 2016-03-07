using System;
using Telerik.Web.UI;
using Thinkgate.Classes;

namespace Thinkgate.ContainerControls
{
    public partial class TileContainerDockSix : Container
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NumberOfTilesPerPage = 6;
        }

        protected void RadDockLayout1_SaveDockLayout(object sender, DockLayoutEventArgs e)
        {
            DockPositionChanged(sender, e);
        }
    }
}
