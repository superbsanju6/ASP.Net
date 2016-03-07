using System;
using System.Web;

namespace Thinkgate
{
    public partial class Framebuster : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Abandon();
            HttpContext.Current.Session["AuthenticatedUser"] = null;
        }
    }
}