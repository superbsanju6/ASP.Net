using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Reports
{
    public partial class BalancedScorecardControl : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BalancedScorecardFrame.Attributes["src"] = ResolveUrl("~/Controls/Reports/BalancedScorecardPage.aspx");
        }
    }
}


