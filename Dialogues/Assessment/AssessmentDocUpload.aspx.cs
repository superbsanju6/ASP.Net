using System;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf;
using Standpoint.Core.Utilities;
using Telerik.Web;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Collections.Generic;
using System.IO;


namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentDocUpload : BasePage
	{
		Int32  _assessmentID = -1, _formID = -1;
		SessionObject sessionObject;
		String _doctype;

		protected void Page_Load(object sender, EventArgs e)
		{
			sessionObject = (SessionObject)Session["SessionObject"];

			if (Request.QueryString["xID"] == null || !Int32.TryParse(Request.QueryString["xID"], out _assessmentID))
			{
				sessionObject.RedirectMessage = "No assessment ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else if (Request.QueryString["yID"] == null ||  !Int32.TryParse(Request.QueryString["yID"], out _formID))
			{
				sessionObject.RedirectMessage = "No form ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else if (Request.QueryString["doctype"] == null || String.IsNullOrEmpty((_doctype = Request.QueryString["doctype"])))
			{
				sessionObject.RedirectMessage = "No document type provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else
			{
				if(!IsPostBack)
				{
				}
			}
		}

		protected void btnUpload_Click(object sender, EventArgs e)
		{
			if (RadUpload1.InvalidFiles.Count > 0)
			{
                hdnAssessmentFileUploadFlag.Value = "E";
                hdnAssessmentFileUploadMsg.Value = "Error uploading file: " + RadUpload1.InvalidFiles[0].GetName() + ". The only allowed file type is PDF.";  
				return;
			}
			if (RadUpload1.UploadedFiles.Count > 0)
			{
				if (_assessmentID < 0 || _formID < 0 || String.IsNullOrEmpty(_doctype))
				{
                    hdnAssessmentFileUploadFlag.Value = "E";
                    hdnAssessmentFileUploadMsg.Value = "Invalid or missing Assessment ID or form ID.";              
					return;
				}
								
				string targetFolder = AppSettings.UploadFolderPhysicalPath;
				UploadedFile newFile = RadUpload1.UploadedFiles[0];
                var rasterizer = new Rasterizer();
                
                string targetFileName = (Path.GetRandomFileName()).Replace(".", "") + newFile.GetExtension();
				newFile.SaveAs(Path.Combine(targetFolder, targetFileName));
				
                Thinkgate.Base.Classes.Assessment.AddDoc(_assessmentID, _formID, _doctype, targetFileName);
                
                //Split PDF into images, 1 per page
                List<string> imageNamesList = rasterizer.CreateImagesFromPDF(Path.Combine(targetFolder, targetFileName));

                //Inset records into AssessmentPDFImages
                Thinkgate.Base.Classes.Assessment.AddImagesFromPDF(_assessmentID, _formID, _doctype, imageNamesList);

                hdnAssessmentFileUploadFlag.Value = "S";
                hdnAssessmentFileUploadMsg.Value = "File uploaded successfully.";
			}
		}
	}
}