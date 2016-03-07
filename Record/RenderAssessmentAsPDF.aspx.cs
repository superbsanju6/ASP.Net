using System;
using System.Reflection;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using WebSupergoo.ABCpdf9;
using Standpoint.Core.Utilities;
using System.Collections.Generic;
using System.Data;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Record
{
    public partial class RenderAssessmentAsPDF : BasePage
    {
        private enum _printListFields
        {
            FormID,
            AssessmentCount,
            AnswerKeyCount,
            RubricCount
        }
      
        private const string _externalTestType = "External";
        private const string _assementFile = "AssessmentFile";
        private const string _reviewerFile = "ReviewerFile";
        private const string _answerKeyFile = "AnswerKeyFile";

        protected void Page_Load(object sender, EventArgs e)
        {
            int assessmentID = 0;
            int formID;
            string answerKey;
            string rawPrintList;
            Boolean isAdminInst = false;


            
         
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                assessmentID = GetDecryptedEntityId(X_ID);
            }

            rawPrintList = Request.QueryString["print_list"];
            if(!string.IsNullOrEmpty(Request.QueryString["isAdminInst"]))
            {
                isAdminInst = Convert.ToBoolean(DataIntegrity.ConvertToInt(Request.QueryString["isAdminInst"]));
            }

            if (String.IsNullOrEmpty(rawPrintList))
            {
                isAdminInst = true; /* For All Preview Administration instructions will be visible ,it will be turned on/off while printing */
                answerKey = Request.QueryString["answerKey"];
                formID = DataIntegrity.ConvertToInt(Request.QueryString["formID"]);
                if (formID == 0)
                {
                    SessionObject.RedirectMessage = "No entity ID provided in URL.";
                    Response.Redirect("~/PortalSelection.aspx", true);
                }

                Doc doc = Assessment.RenderAssessmentToPdf(assessmentID, formID, String.IsNullOrEmpty(answerKey) ? PdfRenderSettings.PrintTypes.Assessment : PdfRenderSettings.PrintTypes.AnswerKey, ConfigHelper.GetImagesUrl(),isAdminInst);

                byte[] theData = doc.GetData();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
                Response.AddHeader("content-length", theData.Length.ToString());
                Response.BinaryWrite(theData);

                ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Finished rendering assessment " + assessmentID, string.Empty);
            }
            else
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var newItems = serializer.Deserialize<object[]>(rawPrintList);

                var assessmentPrintBatchHelpers = new List<AssessmentPrintBatchHelper>();
				foreach (var rawset in newItems)
                {
                    var set = ((object[]) rawset);
                    assessmentPrintBatchHelpers.Add(new AssessmentPrintBatchHelper(DataIntegrity.ConvertToInt(set[(int)_printListFields.FormID]), DataIntegrity.ConvertToInt(set[(int)_printListFields.AssessmentCount]), DataIntegrity.ConvertToInt(set[(int)_printListFields.AnswerKeyCount]), DataIntegrity.ConvertToInt(set[(int)_printListFields.RubricCount])));

                    /*Doc subdoc = Assessment.RenderAssessmentToPdf(assessmentID, DataIntegrity.ConvertToInt(set[(int)_printListFields.FormID]), (PdfRenderSettings.PrintTypes) set[(int)_printListFields.Type]);
                    for (var j = 0; j < (int)set[(int)_printListFields.Count]; j++)
                    {
                        doc.Append(subdoc);
                    }*/
                }
                    

                Doc doc = new Doc();
                doc = CreateTestDoc(assessmentID, assessmentPrintBatchHelpers, doc, ConfigHelper.GetImagesUrl(), isAdminInst);


                byte[] theData = doc.GetData();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
                Response.AddHeader("content-length", theData.Length.ToString());
                Response.BinaryWrite(theData);

                ThinkgateEventSource.Log.ApplicationEvent(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, "Finished rendering assessment " + assessmentID, string.Empty);
            }
/*
#if DEBUG
            // having problem with adobe opening file in browser on my machine, simple work around for development
            try
            {
                doc.Save(@"c:\temp\debug_pdf.pdf");
            } catch
            {
                
            }
#endif
  */           
        }

		private Doc CreateTestDoc(int assessmentID, List<AssessmentPrintBatchHelper> assessmentPrintBatchHelpers, Doc doc, string imagesUrl,bool isAdminInst=false)
        {
            /*  Modified to add uploaded document */
            int userID = SessionObject.LoggedInUser.Page;
            var assessmentInfo = Assessment.GetConfigurationInformation(assessmentID, userID);
            var contentType = assessmentInfo.ContentType;
            bool stringCompareResult = contentType.Equals(_externalTestType, StringComparison.OrdinalIgnoreCase);
            bool noDataFlag = true;

            if (!contentType.Equals(_externalTestType, StringComparison.OrdinalIgnoreCase))    // Is this an External Test
            {
                doc = Assessment.RenderAssessmentToPdf_Batch(assessmentID, assessmentPrintBatchHelpers, imagesUrl, isAdminInst); // Not an External Document
                noDataFlag = false;
            }
            else
            {
                foreach (var assessmentPrintBatchHelper in assessmentPrintBatchHelpers)  //Test Can have multiple Forms (Example: Form 201, Form 101, form 301, etc.)
                {
                    DataTable tbl = Assessment.GetDocs(assessmentID);
                    string tempFile = string.Empty;
                    string AnswerKeyTempFile = tbl.Rows[0][_answerKeyFile].ToString();
                    string[] attachmentTypes = { _assementFile, _reviewerFile, _answerKeyFile };  //rename formNames Attacment types
                   
                    /** Print Uploaded Assessment file **/
                    foreach (string attachmentType in attachmentTypes)
                    {
                        if (!string.IsNullOrEmpty(tbl.Rows[0][attachmentType].ToString()))
                        {
                            tempFile = tbl.Rows[0][attachmentType].ToString();
                            using (Doc externalDoc = new Doc())
                            {
                                int docCount = 0;
                                switch (attachmentType)
                                {
                                    case _assementFile:
                                        docCount = assessmentPrintBatchHelper.AssessmentCount;
                                        break;
                                    case _reviewerFile:
                                        docCount = assessmentPrintBatchHelper.AssessmentCount;
                                        break;
                                    case _answerKeyFile:
                                        docCount = assessmentPrintBatchHelper.AnswerKeyCount;
                                        break;
                                }
                                externalDoc.Read(Server.MapPath("~/upload/" + tempFile));

                                for (int j = 0; j < docCount; j++)
                                {
                                    doc.Append(externalDoc);
                                    noDataFlag = false;
                                }
                            }
                             
                        }                        
                    }


                    //Ashley Reeves said that the Answer Key must Print if there is no enternal file loaded - Reference TFS Bug 1350
                    if (string.IsNullOrEmpty(AnswerKeyTempFile) && assessmentPrintBatchHelper.AnswerKeyCount > 0)
                    {
                        Doc answerKeyDoc = new Doc();
                        answerKeyDoc = Assessment.RenderAssessmentToPdf(assessmentID, assessmentPrintBatchHelper.FormID, PdfRenderSettings.PrintTypes.AnswerKey, isAdminInst);
                        //answerKeyDoc = Assessment.RenderAssessmentAnswerKeyToPdf_Batch(assessmentID, assessmentPrintBatchHelpers); // Not an External Document
                        for (int j = 0; j < assessmentPrintBatchHelper.AnswerKeyCount; j++)
                        {
                            doc.Append(answerKeyDoc);
                            noDataFlag = false;
                        }
                        answerKeyDoc.Dispose();
                    }
                    //1-30-2014 Bug 13579:Print on External Assessment with Rubric should print Rubric and not blank pdf
                    if (assessmentPrintBatchHelper.RubricCount > 0)
                    {
                        Doc RubricDoc = new Doc();
                        RubricDoc = Assessment.RenderAssessmentToPdf(assessmentID, assessmentPrintBatchHelper.FormID, PdfRenderSettings.PrintTypes.Rubric, isAdminInst);
                        for (int j = 0; j < assessmentPrintBatchHelper.RubricCount; j++)
                        {
                            doc.Append(RubricDoc);
                            noDataFlag = false;
                        }
                        RubricDoc.Dispose();
                    }
                    //END 1-30-2014 Bug 13579
                }
                #region JavaScript
                //string scriptName = "MasterJavaScript";
                //string myScriptName = "CustomMessage";
                //if (noDataFlag && !ClientScript.IsClientScriptBlockRegistered(scriptName) && !ClientScript.IsClientScriptBlockRegistered(myScriptName))  //Verify script isn't already registered
                //{

                //    string masterJavaScriptText = string.Empty;
                //    string filePath = Server.MapPath("~/Scripts/master.js");
                //    using (StreamReader MasterJavaScriptFile = new StreamReader(filePath))
                //    {
                //        masterJavaScriptText = MasterJavaScriptFile.ReadToEnd();
                //    }
                //    if (!string.IsNullOrEmpty(masterJavaScriptText))
                //    {
                //        ClientScript.RegisterClientScriptBlock(this.GetType(), scriptName, masterJavaScriptText);

                //        string myScript = "var confirmDialogText = 'There are no external test or answer sheets loaded. Please upload a test and/or answer sheet.';" +
                //                            "\n customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'No Uploaded Documents', content: confirmDialogText, dialog_style: 'confirm' }," +
                //                            "\n [{ title: 'OK', callback: cancelCallbackFunction }, { title: 'Cancel'}]);";

                        

                //        ClientScript.RegisterStartupScript(this.GetType(), myScriptName, myScript,true);
                //    }


                //}
                #endregion //JavaScript
                
                if (noDataFlag) doc = Assessment.NoExternalFiles();  // Prints if there are no external files to print
            }

            

            /** End of adding the uploaded document **/
            return doc;
        }
    }
}