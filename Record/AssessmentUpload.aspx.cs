using System;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using System.IO;
using Standpoint.Core.Utilities;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Record
{
    public partial class AssessmentUpload : Page
    {
        protected SessionObject SessionObject;

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject) Session["SessionObject"];
            //RadUpload1.TargetPhysicalFolder = AppSettings.UploadFolderPhysicalPath;
        }

        protected void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (RadUpload1.InvalidFiles.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(string), "fileUploadError", "parent.customDialog({ title: \"Error Uploading Files\", maximize: true, maxwidth: 500, maxheight:100, animation: \"None\", dialog_style: \"alert\", content: \"Error uploading file: " + RadUpload1.InvalidFiles[0].GetName() + ". The only allowed file type is PDF.\"   }, [{ title: \"Ok\"}]);; ", true);
                return;
            }
            if (RadUpload1.UploadedFiles.Count <= 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(string), "FileNotselected", "parent.customDialog({ title: \"File not selected\", maximize: true, maxwidth: 500, maxheight:100, animation: \"None\", dialog_style: \"alert\", content: \"Please browse a valid PDF file before clicking on upload.\"   }, [{ title: \"Ok\"}]);; ", true);
                return;
            }
            if (RadUpload1.UploadedFiles.Count > 0)
            {
                if (Request.QueryString["xID"] == null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(string), "fileUploadError", "parent.customDialog({ title: \"Error Uploading Files\", maximize: true, maxwidth: 500, maxheight:100, animation: \"None\", dialog_style: \"alert\", content: \"Invalid or missing Assessment ID.\"   }, [{ title: \"Ok\"}]);; ", true);
                    return;
                }
                
                int assessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey);
                int formID = DataIntegrity.ConvertToInt(Request.QueryString["formID"]);
                string type = Request.QueryString["type"];

                string targetFolder = AppSettings.UploadFolderPhysicalPath;

                // we should only have a single file in this case, just throwing this here to show how in other places
                foreach (UploadedFile newFile in RadUpload1.UploadedFiles)
                {
                    string targetFileName = (Path.GetRandomFileName()).Replace(".", "") + newFile.GetExtension();
                    while (System.IO.File.Exists(Path.Combine(targetFolder, targetFileName)))
                    {
                        targetFileName = (Path.GetRandomFileName()).Replace(".", "") + newFile.GetExtension();
                    }
                    newFile.SaveAs(Path.Combine(targetFolder, targetFileName));
                    Assessment.UpdateAssessmentDocument(assessmentID, formID, type, targetFileName);
                }
                ScriptManager.RegisterStartupScript(this.Page, typeof(string), "fileUploadJS", "parent.upload_completed(); ", true);
            }

            
        }
    }
}