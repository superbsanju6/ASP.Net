using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Documents
{
    public partial class ReportInterfaces : TileControlBase
    {   
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
        }

        protected void ClassResultsClick(object sender, EventArgs e)
        {

        }
    }
}