using System;
using Standpoint.Core.Classes;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Xsl;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using WebSupergoo.ABCpdf9;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Record
{
    public partial class RenderItemAsPDF : BasePage
    {
        private Boolean _testQuestion;
        private int _itemID;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _itemID = GetDecryptedEntityId(X_ID);
                Boolean success;
                if (Boolean.TryParse(Encryption.DecryptString(Request.QueryString["TestQuestion"]),out success))
                {
                    _testQuestion = success;
                }
                else
                {
                    SessionObject.RedirectMessage = "No Test/Bank parameter provided in URL.";
                    Response.Redirect("~/PortalSelection.aspx", true);
                }
            }


            Doc doc = new Doc();
            Session["PDFimageURL"] = ConfigHelper.GetImagesUrl();  //Bug 16910 fix - images not displaying in PDF
			doc = Assessment.RenderAssessmentItemToPdf(_itemID, _testQuestion, ConfigHelper.GetImagesUrl());

            byte[] theData = doc.GetData();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);

            Session.Remove("PDFimageURL"); //Bug 16910 fix - images not displaying in PDF
           
            ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Finished rendering PDF for" + (_testQuestion ? "test question" : " bank question") + _itemID, string.Empty);
        }
    }
}