using System;
using System.Reflection;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.CommonService;
using WebSupergoo.ABCpdf9;
using Standpoint.Core.Utilities;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Record
{
    public partial class RenderAssessmentItemUsageReportAsPDF : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string _itemBank = Request.QueryString["ItemBank"];
            string _category = Request.QueryString["Category"];
            string _school = Request.QueryString["School"];
            string _grade = Request.QueryString["Grade"];
            string _subject = Request.QueryString["Subject"];
            string _startDate = Request.QueryString["StartDate"];
            string _endDate = Request.QueryString["EndDate"];
            string _currDate = Request.QueryString["CurrDate"];

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
                Server.Execute("~/Record/RenderAssessmentItemUsageReportView.aspx?ItemBank=" + _itemBank + "&Category=" + _category + "&School=" + _school + "&Grade=" + _grade + "&Subject=" + _subject + "&StartDate=" + _startDate + "&EndDate=" + _endDate + "&Currdate=" + _currDate, writer);
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
                doc.Flatten();
            }
            //reset back to page 1 so the pdf starts displaying there
            if (doc.PageCount > 0)
                doc.PageNumber = 1;

            byte[] theData = doc.GetData();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename = AssessmentItemUsageReport.pdf");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);


        }
    }
}