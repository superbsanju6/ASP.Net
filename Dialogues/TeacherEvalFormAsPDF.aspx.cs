using System;
using Standpoint.Core.Classes;
using System.Reflection;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using WebSupergoo.ABCpdf9;
using Thinkgate.ExceptionHandling;
using log4net;

namespace Thinkgate.Dialogues
{
    public partial class TeacherEvalFormAsPDF : BasePage
    {
        private int _rubricResultID;

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _rubricResultID = Encryption.DecryptStringToInt(Request.QueryString["xID"]);
            }
            */
            _rubricResultID = 1;

            if (_rubricResultID <= 0 || Session["TeacherEvalFormHTML"] == null) return;

            WebSupergoo.ABCpdf9.Doc doc = new WebSupergoo.ABCpdf9.Doc();
            doc.HtmlOptions.HideBackground = true;
            doc.HtmlOptions.PageCacheEnabled = false;
            doc.HtmlOptions.UseScript = true;
            doc.HtmlOptions.Timeout = 36000;
            doc.HtmlOptions.BreakZoneSize = 100;

            var url = Request.Url.ToString().Replace("TeacherEvalFormAsPDF", "TeacherEvalForm") + "?exportToPdf=true";

            var html = Session["TeacherEvalFormHTML"].ToString();

            int theID = doc.AddImageHtml(html);

            for (int i = 1; i <= doc.PageCount; i++)
            {
                doc.PageNumber = i;
                doc.Flatten();
            }
                        
            byte[] theData = doc.GetData();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);

            ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Finished rendering PDF for evaluation " + _rubricResultID, string.Empty);
        }
    }
}