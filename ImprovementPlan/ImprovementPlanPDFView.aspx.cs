using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Enums.ImprovementPlan;

namespace Thinkgate.ImprovementPlan
{


    public partial class ImprovementPlanPDFView : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            int ImprovementID = 0;
            if (Request.QueryString["impID"] != null)
                ImprovementID = int.Parse(Request.QueryString["impID"]);
            else
            {
                return;
            }

            
            
            WebSupergoo.ABCpdf9.Doc doc = new WebSupergoo.ABCpdf9.Doc();
            doc.MediaBox.String = "A4";
            doc.Rect.String = doc.MediaBox.String;
            doc.Rect.Inset(20, 20);
            doc.HtmlOptions.HideBackground = true;
            doc.HtmlOptions.PageCacheEnabled = false;
            doc.HtmlOptions.UseScript = true;
            doc.HtmlOptions.Timeout = 36000;
            doc.HtmlOptions.BreakZoneSize = 100;
            string html;
            using (var writer = new StringWriter())
            {
                Server.Execute("~/ImprovementPlan/ImprovementPlanViewMode.aspx?impID=" +
                       ImprovementID + "&actType=" + ActionType.View + "&isPDF=Yes", writer);
                html = writer.GetStringBuilder().ToString();
            }
           
            int theID = doc.AddImageHtml(html);

            while (true)
            {
                doc.FrameRect(); // add a black border
                if (!doc.Chainable(theID))
                    break;
                doc.Page = doc.AddPage();
                theID = doc.AddImageToChain(theID);
            }

            for (int i = 1; i <= doc.PageCount; i++)
            {
                doc.PageNumber = i;
              //  doc.AddText("Page : "+ i + "/" + doc.PageCount);
                doc.Flatten();
            }
            //reset back to page 1 so the pdf starts displaying there
            if (doc.PageCount > 0)
                doc.PageNumber = 1;

            byte[] theData = doc.GetData();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename=ImprovementPlan.PDF");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);
          
        }
    }
}