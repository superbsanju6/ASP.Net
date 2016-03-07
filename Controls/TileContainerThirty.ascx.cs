using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Thinkgate.Controls
{
    public partial class TileContainerThirty : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public void SetTitles(List<string> titleList)
        {
            var counter = 1;
            foreach (string title in titleList)
            {
                var titleDiv = FindControl("tileTitleDiv" + counter);
                if (titleDiv == null) continue;
                ((System.Web.UI.HtmlControls.HtmlGenericControl)titleDiv).InnerHtml = title;

                counter++;
            }
        }

        public void SetHTMLToContent(List<string> htmlList)
        {
            var counter = 1;

            foreach (string html in htmlList)
            {
                var contentDiv =  (System.Web.UI.HtmlControls.HtmlGenericControl)FindControl("tileContentDiv" + counter);
                if (contentDiv == null) continue;
                contentDiv.InnerHtml = html;

                counter++;
            }

            for (var i = counter; i <= 30; i++)
            {
                var containterDiv = FindControl("tileContainerDiv" + i);
                if (containterDiv == null) continue;
                ((System.Web.UI.HtmlControls.HtmlGenericControl)containterDiv).Style.Value = "display:none";
            }
        }
    }
}