using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Reports
{
    public partial class EdwinAnalyticsReport : TileControlBase
    {
        public List<ExternalLink> Links = new List<ExternalLink>();

        protected void Page_Load(object sender, EventArgs e)
        {
            tileContainerEdwinAnalytics.Visible = UserHasPermission(Base.Enums.Permission.Tile_ExternalLinks_EdwinAnalytics);
            
            // do not query database if user does not have permission
            if (!tileContainerEdwinAnalytics.Visible) return;

            var clientId = DistrictParms.LoadDistrictParms().ClientID;
            Links = ExternalLink.GetExternalLinks(clientId, "EdwinAnalytics", true);
        }
    }
}