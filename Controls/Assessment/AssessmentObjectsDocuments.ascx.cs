using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Enums;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentObjectsDocuments : TileControlBase
    {
        private Int32 _assessmentId;
        private Base.Classes.Assessment _assessment;
        // True if this is a postback.
        private Boolean _isPostBack;
        private String _uploadPath;
        private Base.Classes.Assessment _selectedAssessment;
        DistrictParms dParms;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            if (Tile == null) return;
            _assessmentId = (Int32)Tile.TileParms.GetParm("assessmentID");
            _assessment = (Base.Classes.Assessment)Tile.TileParms.GetParm("assessment");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            String controlId = GetControlThatCausedPostBack(Parent.Page);
            // Simulate IsPostBack.
            _isPostBack = (controlId != null);

            // Initial load.
            dParms = DistrictParms.LoadDistrictParms();
            _selectedAssessment = Base.Classes.Assessment.GetAssessmentAndQuestionsByID(_assessmentId);
            BuildUi();
            hiddenAssessmentID.Value = _assessmentId.ToString();
            hiddenUploadPath.Value = AppSettings.UploadFolderPhysicalPath + "\\";
           
            // Path to upload directory.
            _uploadPath = AppSettings.UploadFolderPhysicalPath + "\\";

            foreach (RepeaterItem ritem in rptAssessment.Items)
            {
                //PBI: 2937
                if (UserHasPermission(Permission.Icon_Delete_Document))
                    ((RadButton)ritem.FindControl("btnDeleteAssessment")).Click += new EventHandler(btnDeleteAssessment_Click);
                else
                {
                    ((RadButton)ritem.FindControl("btnDeleteAssessment")).Visible = false;
                }

                if(!UserHasPermission(Permission.Icon_Upload_Document))
                {
                     ((RadButton)ritem.FindControl("btnUploadAssessment")).Visible = false;
                }
            }

            foreach (RepeaterItem ritem in rptAnswerKey.Items)
            {
                // PBI: 2937
                if (!UserHasPermission(Permission.Icon_Delete_Document))
                {
                    ((RadButton)ritem.FindControl("btnDeleteAnswerKey")).Visible = false;
                }
                else
                    ((RadButton)ritem.FindControl("btnDeleteAnswerKey")).Click += new EventHandler(btnDeleteAnswerKey_Click);

                if (!UserHasPermission(Permission.Icon_Upload_Document))
                {
                    ((RadButton)ritem.FindControl("btnUploadAnswerKey")).Visible = false;
                }
            }

            foreach (RepeaterItem ritem in rptReview.Items)
            {
                 // PBI: 2937
                if (!UserHasPermission(Permission.Icon_Delete_Document))
                {
                    ((RadButton)ritem.FindControl("btnDeleteReview")).Visible = false;
                }
                
                else
                    ((RadButton)ritem.FindControl("btnDeleteReview")).Click += new EventHandler(btnDeleteReview_Click);

                if (!UserHasPermission(Permission.Icon_Upload_Document))
                {
                    ((RadButton)ritem.FindControl("btnUploadReview")).Visible = false;
                }
            }
        }


        protected void BuildUi()
        {           
            DataTable tbl = Base.Classes.Assessment.GetDocs(_assessmentId);
            bool hasReviewDoc = false;
            bool hasAnswerKeyDoc = false;
            bool hasAssessmentDoc = false;

            DataColumn hasAssessmentDocCol = tbl.Columns.Add("HasAssessmentDoc", typeof(Boolean));
            DataColumn hasAnswerKeyDocCol = tbl.Columns.Add("HasAnswerKeyDoc", typeof(Boolean));
            DataColumn hasReviewDocCol = tbl.Columns.Add("HasReviewDoc", typeof(Boolean));
            DataColumn canUploadCol = tbl.Columns.Add("CanUploadReview", typeof(Boolean));
            foreach (DataRow row in tbl.Rows)
            {
                row[hasAssessmentDocCol] = !String.IsNullOrEmpty(row["AssessmentFile"].ToString());
                hasAssessmentDoc = !String.IsNullOrEmpty(row["AssessmentFile"].ToString());
                row[hasAnswerKeyDocCol] = !String.IsNullOrEmpty(row["AnswerKeyFile"].ToString());
                hasAnswerKeyDoc = !String.IsNullOrEmpty(row["AnswerKeyFile"].ToString());
                row[hasReviewDocCol] = !String.IsNullOrEmpty(row["ReviewerFile"].ToString());
                hasReviewDoc = !String.IsNullOrEmpty(row["ReviewerFile"].ToString());
                row[canUploadCol] = !_assessment.IsProofed;
            }

            rptAssessment.DataSource = tbl;
            rptAssessment.DataBind();

            rptAnswerKey.DataSource = tbl;
            rptAnswerKey.DataBind();

            rptReview.DataSource = tbl;
            rptReview.DataBind();

            Control reviewHeaderTemplate = rptReview.Controls[0].Controls[0];
            Control answerHeaderTemplate = rptAnswerKey.Controls[0].Controls[0];
            Control assessmentHeaderTemplate = rptAssessment.Controls[0].Controls[0];

            ImageButton btnDeleteAllAnswer = ((ImageButton)answerHeaderTemplate.FindControl("btnDeleteAllAnswerKey"));
            ImageButton btnDeleteAllAssessment = ((ImageButton)assessmentHeaderTemplate.FindControl("btnDeleteAllAssessment"));
            ImageButton btnDeleteAllReview = ((ImageButton)reviewHeaderTemplate.FindControl("btnDeleteAllReview"));
            
            btnDeleteAllReview.Style["visibility"] = _selectedAssessment.IsProofed || (!_selectedAssessment.IsProofed && !hasReviewDoc) ? "hidden" : "visible";
            btnDeleteAllAnswer.Style["visibility"] = hasAnswerKeyDoc ? "visible" : "hidden";
            btnDeleteAllAssessment.Style["visibility"] = hasAssessmentDoc ? "visible" : "hidden";

            // Assessment and Answer Key documents do not show on unproofed tests.
            if (!_assessment.IsProofed)
            {
                rptAssessment.Visible = false;
                rptAnswerKey.Visible = false;
                // Only the Review documents are shown so we can remove the checkbox.
                cbxReview.Visible = false;
            }
            else if ((_selectedAssessment.TestCategory.Equals("district", StringComparison.InvariantCultureIgnoreCase) && dParms.DistrictReviewAssessment == false)
                        && _assessment.IsProofed
                    ||
                    (_selectedAssessment.TestCategory.Equals("state", StringComparison.InvariantCultureIgnoreCase) && dParms.StateReviewAssessment == false)
                        && _assessment.IsProofed
                    ||
                    (_selectedAssessment.TestCategory.Equals("classroom", StringComparison.InvariantCultureIgnoreCase) && dParms.ClassroomReviewAssessment == false)
                        && _assessment.IsProofed)
            {
                cbxReview.Visible = false;
                rptReview.Visible = false;
            }
        }
        
        protected void btnDeleteAllAssessment_Click(object sender, EventArgs e)
        {
            Base.Classes.Assessment.DeleteDoc(_assessmentId, 0, "AssessmentFile");
            BuildUi();
        }


        void btnDeleteAssessment_Click(object sender, EventArgs e)
        {
            Int32 formId = DataIntegrity.ConvertToInt(((RadButton)sender).Value);
            Base.Classes.Assessment.DeleteDoc(_assessmentId, formId, "AssessmentFile");
            BuildUi();
        }


        protected void btnDeleteAllAnswerKey_Click(object sender, EventArgs e)
        {
            Base.Classes.Assessment.DeleteDoc(_assessmentId, 0, "AnswerKeyFile");
            BuildUi();
        }

        void btnDeleteAnswerKey_Click(object sender, EventArgs e)
        {
            Int32 formId = DataIntegrity.ConvertToInt(((RadButton)sender).Value);
            Base.Classes.Assessment.DeleteDoc(_assessmentId, formId, "AnswerKeyFile");
            BuildUi();
        }

        protected void btnDeleteAllReview_Click(object sender, EventArgs e)
        {
            Base.Classes.Assessment.DeleteDoc(_assessmentId, 0, "ReviewerFile");
            BuildUi();
        }

        void btnDeleteReview_Click(object sender, EventArgs e)
        {
            Int32 formId = DataIntegrity.ConvertToInt(((RadButton)sender).Value);
            Base.Classes.Assessment.DeleteDoc(_assessmentId, formId, "ReviewerFile");
            BuildUi();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BuildUi();
        }
    }
}