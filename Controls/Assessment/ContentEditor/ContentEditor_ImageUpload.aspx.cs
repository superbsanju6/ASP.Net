using System;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using System.IO;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;


namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_ImageUpload : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (RadUpload1.InvalidFiles.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(string), "fileUploadError", "parent.customDialog({ title: \"Error Uploading Files\", height: 100, width: 500, animation: \"None\", dialog_style: \"alert\", content: \"Error uploading file: " + RadUpload1.InvalidFiles[0].GetName() + ". The only allowed file type is PDF.\"   }, [{ title: \"Ok\"}]);; ", true);
                return;
            }
            if (RadUpload1.UploadedFiles.Count > 0)
            {
                if (Request.QueryString["xID"] == null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(string), "fileUploadError", "parent.customDialog({ title: \"Error Uploading Files\", height: 100, width: 500, animation: \"None\", dialog_style: \"alert\", content: \"Invalid or missing Assessment ID.\"   }, [{ title: \"Ok\"}]);; ", true);
                    return;
                }

                int itemImageID = GetDecryptedEntityId(X_ID);
                string targetFolder = AppSettings.UploadFolderPhysicalPath;
                try
                {
                    // we should only have a single file in this case, just throwing this here to show how in other places
                    foreach (UploadedFile newFile in RadUpload1.UploadedFiles)
                    {
                        string currentFileName = newFile.GetName();
                        string targetFileName = (Path.GetRandomFileName()).Replace(".", "") + newFile.GetExtension();
                        while (System.IO.File.Exists(Path.Combine(targetFolder, targetFileName)))
                        {
                            targetFileName = (Path.GetRandomFileName()).Replace(".", "") + newFile.GetExtension();
                        }
                        newFile.SaveAs(Path.Combine(targetFolder, targetFileName));
                        SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                        object returnPayload;
                        returnPayload = Thinkgate.Base.Classes.ItemImage.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page,itemImageID, "FileName", currentFileName);
                        returnPayload = Thinkgate.Base.Classes.ItemImage.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page,itemImageID, "File", targetFileName);

                    }
                }
                catch (System.Exception ex)
                {
                    object returnPayload;
                    returnPayload = ex.Message;
                }
                ScriptManager.RegisterStartupScript(this.Page, typeof(string), "fileUploadJS", "parent.upload_completed(); ", true);
            }


        }
    }
}