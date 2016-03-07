using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using Thinkgate.Classes;
using assessmentAlias = Thinkgate.Base.Classes;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class ImportTestResponseCard : System.Web.UI.Page
    {
        Int32 _assessmentID;
        SessionObject sessionObject;
        Int32 recordCount;

        public string TargetFolder 
        {
            get { return ViewState["targetFolder"].ToString(); }
            set { ViewState["targetFolder"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            sessionObject = (SessionObject)Session["SessionObject"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["xID"] == null || !Int32.TryParse(Request.QueryString["xID"], out _assessmentID))
            {
                sessionObject.RedirectMessage = "No assessment ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }

            if (!IsPostBack)
            {
                TargetFolder = Server.MapPath("~/DBACCESS/"); 
                LoadAssessment();
            }
        }

        /// <summary>
        /// Loads the Assessment Information
        /// </summary>
        private void LoadAssessment()
        {
            // fill out assessment related info                        
            assessmentAlias.Assessment assessment = assessmentAlias.Assessment.GetAssessmentByID(_assessmentID);

            lblGrade.Text = assessment.Grade;
            lblSubject.Text = assessment.Subject;
            lblCourse.Text = assessment.Course;
            lblAssessment.Text = assessment.TestType + assessment.Term;
        }

        /// <summary>
        /// Check And Create Folder
        /// </summary>
        protected void CheckAndCreateFolder()
        {
            try
            {
                if (!Directory.Exists(TargetFolder))
                { Directory.CreateDirectory(TargetFolder); }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// Upload Test Response Card File at server location
        /// </summary>
        /// <param name="cardFileName"></param>
        protected void UploadCardFile(string cardFileName)
        {
            try
            {
                CheckAndCreateFolder();
                radFileUpload.UploadedFiles[0].SaveAs(TargetFolder + cardFileName, true);
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "UploadFailed", "MessageAlert('File import failed due to communications error. Please try again.');", true);
            }
        }

        /// <summary>
        /// Get Data from Test Response Card file
        /// </summary>
        /// <param name="cardFileName"></param>
        /// <returns>CardFileData</returns>
        protected string GetCardFileData(string cardFileName)
        {
            string cardFileData = string.Empty; ;
            string lineData = string.Empty; ;

            try
            {
                // Upload Test Response Card file at server
                UploadCardFile(cardFileName);

                // Read Test Response Card file line by line
                StreamReader file = new StreamReader(TargetFolder + cardFileName);
                while ((lineData = file.ReadLine()) != null)
                {
                    // Skip record which contains word 'Seq'
                    if (!lineData.Contains("Seq"))
                    {
                        cardFileData += "START," + lineData + ",END ";
                        recordCount++;
                    }
                }
                file.Close();

                //Dont know why this was deleting. -PLH
                // Delete Test Response Card file from server
                //DeleteCardFile(cardFileName.Trim());
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "UploadFailed", "MessageAlert('File import failed due to communications error. Please try again.');", true);
            }

            return cardFileData;
        }

        /// <summary>
        /// Delete Test Response Card File from server location
        /// </summary>
        /// <param name="cardFileName"></param>
        protected void DeleteCardFile(string cardFileName)
        {
            try
            {
                if (File.Exists(TargetFolder + cardFileName))
                    File.Delete(TargetFolder + cardFileName);
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "UploadFailed", "MessageAlert('File import failed due to communications error. Please try again.');", true);
            }
        }

        protected void radImport_Click(object sender, EventArgs e)
        {
            if (radFileUpload.UploadedFiles.Count <= 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "InvalidFile", "MessageAlert('Please browse a specified file type (.csv)');", true);
                return;
            }

            Random rand = new Random();

            string cardFileName = rand.Next(100000) + "_" + radFileUpload.UploadedFiles[0].GetName();
            string cardFileData = GetCardFileData(cardFileName);
            if (recordCount <= 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "EmptyFile", "MessageAlert('TestResponseCardImport Error:<br /> Input file is empty.');", true);
                return;
            }
            else
            {
                string procName = cardFileName.Contains("_PPS") ? "E3_SaveTestResponseCardPPS" : "E3_SaveTestResponseCardHALO";
                int user = sessionObject.LoggedInUser.Page;
                string fileName = cardFileName;
                string fileData = cardFileData;
                int testID = cardFileName.Contains("_PPS") ? 0 : _assessmentID;
                string memo = string.Empty;
                int uploadJobId = 0;

                try
                {
                    DataTable dtImport = Thinkgate.Base.Classes.Assessment.ImportTestResponseCard(procName, user, fileName, fileData, testID, memo, uploadJobId);

                    //dtImport.Rows[0]["Messages"].ToString();
                    int cardsRead  = Convert.ToInt32(dtImport.Rows[0]["CardsRead"]);
                    int cardsImported = Convert.ToInt32(dtImport.Rows[0]["CardsImported"]);
                    int errorsFound = cardsRead - cardsImported;

                    string message = cardsRead + " test response cards read;<br /> " + cardsImported + " successfully imported;<br /> " + errorsFound + " errors.";
                    ScriptManager.RegisterStartupScript(this, typeof(string), "ImportSuccess", "MessageAlert('TestResponseCardImport Success:<br /> " + message + "');", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "ImportFailed", "MessageAlert('TestResponseCardImport (ExecSP Error):<br /> '" + ex.Message + "');", true);
                }
            }
        }
    }
}