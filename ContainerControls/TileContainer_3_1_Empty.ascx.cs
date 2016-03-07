using System;
using Thinkgate.Classes;
using Telerik.Web.UI;

namespace Thinkgate.ContainerControls
{
    public partial class TileContainer_3_1_Empty : Container
    {
        string defaultMsg = "";
        Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

        protected void Page_Load(object sender, EventArgs e)
        {
            NumberOfTilesPerPage = 0;
            if (sessionObject.DefaultEmptyMessage != string.Empty)
            {
                defaultMsgDiv.InnerText = sessionObject.DefaultEmptyMessage;
                Session.Remove("DefaultEmptyMessage");
            }
        }
    }
}