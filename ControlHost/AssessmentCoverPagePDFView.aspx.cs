using System;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using WebSupergoo.ABCpdf9;
using Telerik.Web.UI;

namespace Thinkgate.ControlHost
{
    public partial class AssessmentCoverPagePDFView : BasePage
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
            _sessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];
            _currUserID = _sessionObject.LoggedInUser.Page;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadAssessmentInfo();
            PDFView();
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
                    RedirectToPortalSelectionScreenWithCustomMessage("Could not find the assessment");
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

        public void PDFView()
        {
            Doc doc = Assessment.RenderAssessmentCoverPageToPdf(_assessmentInfo.CoverPageHTML);

            byte[] theData = doc.GetData();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);
        }

    }
}