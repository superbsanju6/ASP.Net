using System;
using System.Reflection;

using Thinkgate.Classes;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.Reports
{
    public partial class SummativeReportV2 : TileControlBase
    {              
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null || Tile.TileParms.GetParm("guid") == null) return;
            if (Request.QueryString["folder"] != null && !String.IsNullOrEmpty(Request.QueryString["folder"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["folder"].ToString() + "' report", Request.QueryString["folder"].ToString(), sessionObject.LoggedInUser.UserName);
            }
            summativeReportFrame.Attributes["src"] = ResolveUrl("~/Controls/Reports/SummativeReportPageV2.aspx") + "?xID=" + Tile.TileParms.GetParm("guid").ToString();
        }
    }
}