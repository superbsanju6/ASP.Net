using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Security;
using Thinkgate.Classes;
using System.Configuration;
using System.Net;

using Telerik.Web.UI;

using Thinkgate.Base.Classes;

namespace Thinkgate
{
    public partial class SubmitHelpRequest : System.Web.UI.Page
    {   

    #region Constants
        const string FILES = "Files";
        const string FID = "FID";
        const string UPLOADFOLDER = "//FTP//SubmitHelpRequest//";
    #endregion

    #region Properties
        protected List<UploadFileDetail> FileList
        {
            get
            {
                return (List<UploadFileDetail>)ViewState[FILES];
            }
            set
            {
                ViewState[FILES] = value;
            }
        }
        protected string TargetFolder
        {
            get
            {
                return Server.MapPath("~") + UPLOADFOLDER;
            }

        }
        protected int FileID
        {
            get
            {
                return Convert.ToInt32(ViewState[FID]);
            }
            set
            {
                ViewState[FID] = value;
            }
        }
    #endregion

    #region Events Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SessionObject obj = (SessionObject)Session["SessionObject"];
                txtEmail.Text = ((System.Web.Security.MembershipUser)(obj.LoggedInUser)).Email;

                LoadComponent();
            }
        }

        protected void gvFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (gvFiles.EditIndex >= 0)
            { return; }

            //Check row state of gridview whether it is data row or not
            if ((e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate) && (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header))
            {
                e.Row.Cells[0].Visible = false;//FileID
                e.Row.Cells[3].Visible = false;//Description
            }
        }
        
        protected void gvFiles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ClientScript.RegisterStartupScript(typeof(Page), "ChangeValue", "<script> SubjectValueChange();  </script>", false);
            try
            {
                if (e.CommandName.ToString() == "delete")
                { FileID = Convert.ToInt16(e.CommandArgument); }
            }
            catch (Exception ex)
            {}
        }

        protected void gvFiles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string filePath = string.Empty;
            List<UploadFileDetail> uploadFileList;
           
            try
            {
                uploadFileList = FileList.ToList().Where(p => p.FileID != FileID).ToList();
                filePath = TargetFolder + (FileList.ToList().Where(s => s.FileID == FileID).FirstOrDefault().FileName);
                FileList = uploadFileList;
                gvFiles.DataSource = uploadFileList;
                gvFiles.DataBind();

                if (File.Exists(filePath))
                { File.Delete(filePath); }

                DeleteFileFromFTP(new FileInfo(filePath).Name);
            }
            catch //(Exception ex)
            {
                // todo: took out ex to kill warning, but this shouldn't be swallowing an exception...
            }
        }

        protected void rbSubmit_Click(object sender, EventArgs e)
        {
            string ftpUrl = ConfigurationManager.AppSettings.Get("FtpUrl");
            string fileName = string.Empty;

            if (FileList != null && FileList.Count > 0)
            {
                fileName = FileList.ToList().FirstOrDefault().FileName;
                ftpUrl = ftpUrl + fileName;
            }
            else
            { ftpUrl = ""; }

            SubmitForm(ftpUrl);
        }

        protected void rbCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "CloseHelpRequest", "CloseConfirmation();", true);
        }

        protected void rbClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "CloseUploadFiledialog", "FileUploadClose();", true);
        }

        protected void rbOk_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(typeof(Page), "ChangeValue", "<script> SubjectValueChange();  </script>", false);
            try
            {
                CheckAndCreateFolder();

                if (ruFileUpload.UploadedFiles.Count <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "EmptyFile", "MessageAlert('Please select any file type of .pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.jpg,.jpeg,.png,.gif,.bmp.');", true);
                    return;
                }

                //file size limit check(4MB size =4194304 bytes)
                Int64 uploadFileSize = Convert.ToInt64(ConfigurationManager.AppSettings.Get("UploadFileSize"));

                if (ruFileUpload.UploadedFiles[0].ContentLength >= uploadFileSize)
                {
                    ClientScript.RegisterStartupScript(typeof(Page), "UploadFileSize", "<script> MessageAlert('Please upload file upto 4MB.');  </script>", false);
                    return;
                }
                else
                {
                    List<UploadFileDetail> uploadFileList = new List<UploadFileDetail>();

                    if (FileList != null)
                    { uploadFileList = (List<UploadFileDetail>)FileList; }

                    if (uploadFileList.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "FileLimit", "MessageAlert('More than one file can not be uploaded!');", true);
                        return;
                    }
                    
                    UploadFileDetail uploadFileDetail = new UploadFileDetail();

                    uploadFileDetail.FileID = uploadFileList.Count + 1;
                    
                    string oldFileName = string.Empty;
                    string newFileName = string.Empty;

                    oldFileName = new FileInfo(ruFileUpload.UploadedFiles[0].FileName).Name;
                    newFileName = oldFileName.Substring(0, oldFileName.IndexOf('.'));
                    newFileName = newFileName + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString();
                    
                    string fileExtension = oldFileName.Substring(oldFileName.IndexOf('.'), oldFileName.Length - oldFileName.IndexOf('.'));

                    newFileName = newFileName.Replace("-", "").Replace(":", "").Replace(" ", "").Replace("/","_").Replace(@"\","_") + fileExtension;

                    uploadFileDetail.FileName = newFileName;
                    uploadFileDetail.ClientFileName = new FileInfo(ruFileUpload.UploadedFiles[0].FileName).Name;
                    uploadFileDetail.Description = TargetFolder + uploadFileDetail.FileName;

                    uploadFileList.Add(uploadFileDetail);

                    FileList = uploadFileList;
                    gvFiles.DataSource = uploadFileList;
                    gvFiles.DataBind();

                    ruFileUpload.UploadedFiles[0].SaveAs(TargetFolder + (uploadFileDetail.FileName));

                    string usingFtp = ConfigurationManager.AppSettings.Get("UsingFTP");

                    if (usingFtp == "1")
                    {
                        string ftpUrl = ConfigurationManager.AppSettings.Get("FtpUrl");
                        string ftpUid = ConfigurationManager.AppSettings.Get("FtpUid");
                        string ftpPwd = ConfigurationManager.AppSettings.Get("FtpPwd");

                        UploadFileUsingFTP(ftpUrl, ftpUid, ftpPwd, TargetFolder + (uploadFileDetail.FileName), uploadFileDetail.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "ErrMsgAlert", "ErrorMessageAlert('File upload failed.');", true);
            }
        }
       
    #endregion

    #region Private Methods
        protected void LoadComponent()
        {
            List<string> components = GetComponents();

            foreach (string component in components)
            {
                using (RadComboBoxItem radComboBoxItem = new RadComboBoxItem())
                {
                    radComboBoxItem.Text = component;
                    radComboBoxItem.Value = component;
                    cmbComp.Items.Add(radComboBoxItem);
                }
            }
        }
        //TODO: If list will come from DB then need to write DB logic to get the same info
        private List<string> GetComponents()
        {
            List<string> components = new List<string>();

            components.Add("Standards");
            components.Add("Class");
            components.Add("Student");
            components.Add("Staff");
            components.Add("Login");
            components.Add("Instruction");
            components.Add("Assessment");
            components.Add("Reporting");
            components.Add("Evaluation");
            components.Add("Other");

            return components;
        }

        private void SubmitForm(string fileUrl)
        {
            HttpContext.Current.Response.Write("<form name='newForm' method=post action='https://www.salesforce.com/servlet/servlet.WebToCase?encoding=UTF-8'>");

            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"00N50000001nN9u\" value=\"{0}\">", ""));

            //For Debuugging purpose: Uncomment these below lines to debug the posted info.
            //HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"debug\" value=\"{0}\">", "1"));
            //HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"debugEmail\" value=\"{0}\">", "mkrue@thinkgate.net"));            

            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"00N50000001njVv\" value=\"{0}\">", ""));

            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"00N50000001njVw\" value=\"{0}\">", ""));

            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"email\" value=\"{0}\">", txtEmail.Text));//email.Value

            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"orgid\" value=\"{0}\">", "00D500000007Aji"));
                        
            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"phone\" value=\"{0}\">", txtPhone.Text + (!string.IsNullOrEmpty(txtExtn.Text) ? " Extn - " + txtExtn.Text : "")));
            //-----------
            HttpContext.Current.Response.Write(string.Format("<input id='00N50000001nN9z' type='hidden' title='Component' maxlength='60' name=\"orgid\" value=\"{0}\">", ""));
            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"Component__c\" value=\"{0}\">", cmbComp.SelectedItem.Text));
            //---------------
            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"subject\" value=\"{0}\">", txtSubject.Text));

            HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"description\" value=\"{0}\">", txtDetails.Text));
            if (fileUrl.Length > 0)
            {
                HttpContext.Current.Response.Write(string.Format("<input type=hidden name=\"AttachmentLink__c\" value=\"{0}\">", fileUrl));
            }

            HttpContext.Current.Response.Write("</form>");
            HttpContext.Current.Response.Write("</body>");
            ScriptManager.RegisterStartupScript(this, typeof(string), "InformAndClose", " ConfirmAndClose();", true);
            //Response.Write("<SCRIPT LANGUAGE='JavaScript'>document.forms[0].submit();</SCRIPT>");

        }

        protected void CheckAndCreateFolder()
        {
            try
            {
                string folderMainPath = Server.MapPath("~");
                if (Directory.Exists(folderMainPath + UPLOADFOLDER))
                { return; }

                if (!Directory.Exists(folderMainPath + "\\FTP"))
                {
                    Directory.CreateDirectory(folderMainPath + "\\FTP");
                    Directory.CreateDirectory(folderMainPath + UPLOADFOLDER);
                }

                if (!Directory.Exists(folderMainPath + UPLOADFOLDER))
                { Directory.CreateDirectory(folderMainPath + UPLOADFOLDER); }
            }
            catch (Exception ex)
            { }
        }

        protected void UploadFileUsingFTP(string ftpUrl, string ftpUid, string ftpPwd, string serverFilePath, string fileNameToUpload)
        {
            try
            {
                string ftpFullPath = ftpUrl + fileNameToUpload; //create ftp request

                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(ftpFullPath);
                ftpWebRequest.Credentials = new NetworkCredential(ftpUid, ftpPwd);

                ftpWebRequest.KeepAlive = true;
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.UsePassive = true;
                
                //select method as upload file (STOR command)
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                using(FileStream fileStream = File.OpenRead(serverFilePath))
                {    
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    fileStream.Close();                   
                    //get request stream
                    Stream stream = ftpWebRequest.GetRequestStream();
                    stream.Write(buffer, 0, buffer.Length); stream.Close();
                }                
                return;
            }
            catch (Exception ex)
            {
                string excp = ex.Message;
                ClientScript.RegisterStartupScript(typeof(Page), "MessagePopUp", "alert('Image not uploaded successfully." + excp + "')", true);                
                return;
            }
        }

        protected bool DeleteFileFromFTP(string fileName)
        {
            // The serverUri parameter should use the ftp:// scheme.
            // It contains the name of the server file that is to be deleted.

            string ftpUid = ConfigurationManager.AppSettings.Get("FtpUid");
            string ftpPwd = ConfigurationManager.AppSettings.Get("FtpPwd");
            string ftphost = ConfigurationManager.AppSettings.Get("FtpUrl") + fileName;

            Uri serverUri = new Uri(ftphost);

            if (serverUri.Scheme != Uri.UriSchemeFtp)
            { return false; }

            // Get the object used to communicate with the server.
            FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(serverUri);
            ftpWebRequest.Credentials = new NetworkCredential(ftpUid, ftpPwd);
            ftpWebRequest.KeepAlive = true;
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.UsePassive = true;

            ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
            ftpWebResponse.Close();
            return true;
        }
    #endregion
    
    }

    [Serializable]
    public class UploadFileDetail
    {
        public int FileID
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string ClientFileName { get; set; }
    }
    
}