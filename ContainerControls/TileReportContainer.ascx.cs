using System;
using Thinkgate.Classes;
using Telerik.Web.UI;

namespace Thinkgate.ContainerControls
{
    public partial class TileReportContainer : Container
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NumberOfTilesPerPage = 2;
        }
    }
}