using System;
using System.Web;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.ControlHost
{
    public partial class AssessmentCoverPageEditor : BasePage
    {
        private int _assessmentID;
        private string _cacheKey;
        private AssessmentInfo _assessmentInfo;
        private SessionObject _sessionObject;
        private int _currUserID;
        public string clientFolder { get; set; }

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            _sessionObject = (SessionObject)Page.Session["SessionObject"];
            _currUserID = _sessionObject.LoggedInUser.Page;

#if DEBUG
            clientFolder = "";
#else
            clientFolder = AppSettings.AppVirtualPath;
#endif
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadAssessmentInfo();

            if (!IsPostBack)
            {
                lblCoverSheetCkeditor.Text = _assessmentInfo.CoverPageHTML.Length > 0 ? _assessmentInfo.CoverPageHTML : "<div align=\"center\"><br/><br/><br/><br/>Assessment Cover page Workspace<br/><br/>Click to Edit</div>";
                //coverPagePDFFrame.Attributes["src"] = "AssessmentCoverPagePDFView.aspx?xID=" + Request.QueryString["xID"];
                assessmentIDEncrypted.Value = EntityIdEncrypted;
            }
        }

        private void LoadAssessmentInfo()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }

            _assessmentID = GetDecryptedEntityId(X_ID);
            _cacheKey = "AssessmentInfo_" + _assessmentID.ToString();

            if (Base.Classes.Cache.Get(_cacheKey) == null)
            {
                _assessmentInfo = Assessment.GetConfigurationInformation(_assessmentID, _currUserID);
                if (_assessmentInfo != null)
                    Base.Classes.Cache.Insert(_cacheKey, _assessmentInfo);
                else
                {
                    RedirectToPortalSelectionScreenWithCustomMessage("Could not find assessment");
                }
            }
            else
            {
                _assessmentInfo = (AssessmentInfo)Cache[_cacheKey];
            }

            if (_assessmentInfo == null)
            {
                RedirectToPortalSelectionScreenWithCustomMessage("Assessment does not exist.");
            }
        }

        public void UpdateCoverPage(object sender, EventArgs e)
        {
            string editorContent = HttpUtility.UrlDecode(hdnEditorContent.Value);
            Assessment.UpdateAssessmentCoverPage(_assessmentID, editorContent);
            _assessmentInfo.CoverPageHTML = editorContent;
            Base.Classes.Cache.Remove(_cacheKey);
            Base.Classes.Cache.Insert(_cacheKey, _assessmentInfo);

            string js = "if(modalWin) modalWin.SetUrl('../Dialogues/Assessment/AssessmentConfiguration.aspx?xID=" + Request.QueryString["xID"] + "');"
                + "var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);var winHeight = adjustedHeight < 675 ? adjustedHeight : 675;modalWin.SetSize(800, winHeight);"
                + "modalWin.Center();modalWin.SetTitle('Assessment Configuration');";

            ScriptManager.RegisterStartupScript(Page, typeof(Page), "AssessmentCoverPage", js, true);
        }

        public void PDFView(object sender, EventArgs e)
        {
            string editorContent = HttpUtility.UrlDecode(hdnEditorContent.Value);
            Assessment.UpdateAssessmentCoverPage(_assessmentID, editorContent);
            _assessmentInfo.CoverPageHTML = editorContent;
            Base.Classes.Cache.Remove(_cacheKey);
            Base.Classes.Cache.Insert(_cacheKey, _assessmentInfo);

            string js = "$('#coverPagePDFFrame').attr('src', 'AssessmentCoverPagePDFView.aspx?xID=" + Request.QueryString["xID"] + "');";
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "AssessmentPDFView", js, true);
        }

    }
}