using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Standpoint.Core.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using System.Linq;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentTargets : TileControlBase
    {
        private int _testID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];

            if (Tile == null) return;

            _testID = Convert.ToInt32(Tile.TileParms.GetParm("assessmentID"));

            LoadTargetsGrid();
        }

        private void LoadTargetsGrid()
        {
            grdTargets.DataSource = Thinkgate.Base.Classes.Assessment.GetTargetsByTestID(_testID);
            grdTargets.DataBind();
        }

        protected void grdTargets_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                //GridDataItem dataItem = (GridDataItem)e.Item;
                //HyperLink hyperLink = (HyperLink)dataItem["link"].Controls[0];
                //hyperLink.NavigateUrl = "http://thinkgate.net/e3qa2011/display.asp?key=7099&fo=basic display&rm=page&xID=4773&??category=Classroom&??xID=" + dataItem["TestID"].Text;  
            }
        }

    }
}