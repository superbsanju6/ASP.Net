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
    public partial class RenderImageAsPDF : BasePage
    {
        private int _imageID;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _imageID = GetDecryptedEntityId(X_ID);
            }

            Doc doc = new Doc();

            doc = Assessment.RenderAssessmentImageToPdf(_imageID);

            byte[] theData = doc.GetData();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);

            ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Finished rendering PDF for image " + _imageID, string.Empty);
        }
    }
}