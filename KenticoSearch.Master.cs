using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Thinkgate
{
    public partial class KenticoSearch : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           // Page.Header.Controls.Add(
             //   new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl("~/Scripts/jquery-ui/jquery.ui.all.css") + "\" />"));


         //  ScriptManager.RegisterStartupScript(this, typeof(Page), "css", "var a= createCSSRef('Scripts/jquery-ui/jquery.ui.all.css');document.getElementsByTagName('HEAD')[0].appendChild(a);", true);
            ScriptManager scriptMan = ScriptManager.GetCurrent(Page);
            scriptMan.AsyncPostBackTimeout = 36000;

        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            //HtmlGenericControl csslink = new HtmlGenericControl("link");
            //csslink.ID = "GridStyle";
            //csslink.Attributes.Add("href", "../../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.min.css");
            //csslink.Attributes.Add("type", "text/css");
            //csslink.Attributes.Add("rel", "stylesheet");
            //Page.Header.Controls.Add(csslink);

        }
    }
}