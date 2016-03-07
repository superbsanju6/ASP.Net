using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Classes.Serializers;
using System.Reflection;
using Thinkgate.Base.DataAccess;
using System.Web.Script.Serialization;
using Standpoint.Core.Utilities;
using System.Linq;
using Thinkgate.Base.Enums;

namespace Thinkgate.Record
{
    public partial class AssessmentPage : RecordPage
    {
        #region Properties
        private int _assessmentID;
        public Assessment selectedAssessment;
        public static string ImageWebFolder;
        public string QA_Text_Visible_Style;
        public string ItemThumbnailWebPath_Content;
        public string SelectedStandardOutline = "Search Standards";
        protected int currUserID;
		public string lbl_ImagesUrl;
        private int _selectedStandardOutline;
        private DataTable _standardsSetListTable;
        private DataTable _dtStandardsWithSetInTree;
        private DataTable _dtStandards;
        private JavaScriptSerializer _serializer;
        public string ItemDefaultDirections;   
        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(selectedAssessment.TestType);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }
   
        private string CacheKey
        {
            get { return 'A' + _assessmentID.ToString(CultureInfo.CurrentCulture); }
        }

        #endregion

        protected override String TypeKey
        {
            get { return EntityTypes.Assessment + "_"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _assessmentID = GetDecryptedEntityId(X_ID);
                lbl_TestID.Value = _assessmentID.ToString(CultureInfo.CurrentCulture);
            }
            currUserID = SessionObject.LoggedInUser != null ? (SessionObject.LoggedInUser.Page > 0 ? SessionObject.LoggedInUser.Page : AppSettings.Demo_TeacherID) : AppSettings.Demo_TeacherID;

            ImageWebFolder = (Request.ApplicationPath.Equals("/") ? "" : Request.ApplicationPath) + "/Images/";
            ItemThumbnailWebPath_Content = AppSettings.ItemThumbnailWebPath_Content;
            DistrictParms = DistrictParms.LoadDistrictParms();
            lbl_OTCUrl.Value = AppSettings.OTCUrl + DistrictParms.ClientID;

            ClientConfigHelper clientConfigHelper = new ClientConfigHelper(DistrictParms.ClientID);
            lbl_ImagesUrl = clientConfigHelper.GetImagesUrl();

            txtAddNumItems.Attributes.Add("onclick", "javascript:event.stopPropagation()");

            if (!IsPostBack)
            {
                // txtKeyword.EmptyMessage = "&lt; Enter text to search for items &gt;";   // Commented for Bug Id: #8945; 

                if (!String.IsNullOrEmpty(SessionObject.GenericPassThruParm) && SessionObject.GenericPassThruParm.StartsWith("CLONE:") && SessionObject.GenericPassThruParm.Contains(Request.QueryString["xID"]))
                {
                    if (SessionObject.GenericPassThruParm.Contains("ContainedExpiredContent"))
                        ScriptManager.RegisterStartupScript(this, typeof(string), "cloneMessage", "postCloneRemovedExpiredContentDialog();", true);
                    else
                    ScriptManager.RegisterStartupScript(this, typeof(string), "cloneMessage", "postCloneDialog();", true);
                    SessionObject.GenericPassThruParm = null;
                }
				LoadAssessmentObject(lbl_ImagesUrl);
                LoadCorrectNewDialog();
                var currCourse = CourseMasterList.GetCurrCourseById(selectedAssessment.currCourseID);

                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                Dictionary<string, bool> dictionaryItem;
                dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(selectedAssessment.TestCategory);
                bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                hiddenAccessSecureTesting.Value = hasPermission.ToString();
                hiddenIsSecuredFlag.Value = isSecuredFlag.ToString();
                hiddenSecureType.Value = SecureType.ToString();

                ScriptManager.RegisterStartupScript(this, typeof(string), "encryptedUserID", "var encryptedUserID='" + Standpoint.Core.Classes.Encryption.EncryptInt(currUserID) + "';" +
                                                                                         " var FieldTestFormId=" + AssessmentForm.FieldTestFormId + ";" +
                                                                                         " var SessionBridge='" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "';" +
                                                                                         " var Enums = new Object(); Enums.UpdatedDataAttached=" + (int)ActionResult.GenericStatus.UpdatedDataAttached + ";" +
                                                                                         " Enums.ProofStatus_base=" + (int)ActionResult.ProofStatus._base + "; Enums.ProofStatusDistractorCountMismatch = " + ((int)ActionResult.ProofStatus.DistractorCountMismatch - (int)ActionResult.ProofStatus._base) + "; " +
                                                                                         " Enums.ProofStatusMultipleChoiceAlignmentProblem=" + ((int)ActionResult.ProofStatus.MultipleChoiceAlignmentProblem - (int)ActionResult.ProofStatus._base) + ";" +
                                                                                         " var ContentType='" + selectedAssessment.ContentType + "';" +
                                                                                         " AssessmentIsProofed=" + (selectedAssessment.IsProofed ? "true" : "false") + ";" +
                                                                                         " var dok = '" + DistrictParms.DOK + "';" +
                                                                                         " var grade='" + currCourse.Grade + "';" +
                                                                                         " var subject='" + currCourse.Subject + "';" +
                                                                                         " var coursename='" + currCourse.CourseName + "';" +
                                                                                         " var TestYear='" + selectedAssessment.Year + "';" +
                                                                                         " var TestCurrCourseID=" + selectedAssessment.currCourseID + ";" +
                                                                                         " var MultiForm='" + selectedAssessment.MultiForm + "';" +
                                                                                         " var TestCategory='" + selectedAssessment.Category + "';" +
                                                                                         " var xID = '" + Standpoint.Core.Classes.Encryption.EncryptString(selectedAssessment.Category.ToString()) + "'; " +
                                                                                         " var yID = '" + Standpoint.Core.Classes.Encryption.EncryptString(selectedAssessment.Category.ToString()) + "'; " +
                                                                                         " var zID = '" + Standpoint.Core.Classes.Encryption.EncryptString(selectedAssessment.AssessmentID.ToString()) + "'; " +
                                                                                         " var hasReviewerFile = '" + (!string.IsNullOrEmpty(selectedAssessment.Forms[0].ReviewerFile)) + "'; " + 
                                                                                         " var hasAssessmentFile = '" + (!string.IsNullOrEmpty(selectedAssessment.Forms[0].AssessmentFile)) + "'; " +
                                                                                         " var isSecureAssessment='"+ SecureType.ToString() +"';"+
                                                                                         " var AssessmentType='" + selectedAssessment.TestType + "';" +
                                                                                         " var hasSecurePermission='" + hasPermission.ToString() + "';" +
                                                                                         " var isSecureFlag='" + isSecuredFlag.ToString() + "';"
                                                                                         , true);


                RefreshQuestionsPanel();
                PopulateProfileLabels(selectedAssessment);
                BindStandardsSetList();
                BindRadtab_Forms();
                BindComboBoxes();
                BindCommonStandardTree();
                if (selectedAssessment.IncludeFieldTest)
                {
                    radtab_FieldTest.Visible = true;
                }

                mainTabStrip.FindTabByValue("Preview").Visible = !selectedAssessment.IsExternal;
                var isNoItemsContent = selectedAssessment.ContentType.ToUpper() == "NO ITEMS/CONTENT" ? true : false;
                mainToolBar.FindItemByValue("OnlinePreview", true).Visible = !isNoItemsContent;

                if (UserHasPermission(Permission.Icon_Test_PrintHTML) && UserHasPermission(Permission.Icon_Test_PrintWord))
                {
                    if (DistrictParms.LRMITagging == "true")
                    {
                    rtbBtnHtml.Visible = true;
                    rtbBtnWord.Visible = true;
                        rtbBtnLRMI.Visible = true;
                        ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                            "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'HTML', 'Word','Tags','Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                            true);
                }
                else
                {
                        rtbBtnHtml.Visible = true;
                        rtbBtnWord.Visible = true;
                        rtbBtnLRMI.Visible = false;
                        ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                            "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'HTML', 'Word','Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                            true);
                    }
                }
                else
                {
                    if (!UserHasPermission(Permission.Icon_Test_PrintHTML) && !UserHasPermission(Permission.Icon_Test_PrintWord))
                    {
                        if (DistrictParms.LRMITagging == "true")
                        {
                            rtbBtnLRMI.Visible = true;
                            ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                                "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'Tags', 'Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                                true);
                        }
                        else
                        {
                            rtbBtnLRMI.Visible = false;
                            ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                                "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                                true);
                        }
                    }
                    else
                    {
                        if (UserHasPermission(Permission.Icon_Test_PrintHTML))
                        {
                            if (DistrictParms.LRMITagging == "true")
                            {
                            rtbBtnHtml.Visible = true;
                                rtbBtnLRMI.Visible = true;
                                ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                                    "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'HTML', 'Tags', 'Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                                    true);
                            }
                            else
                            {
                                rtbBtnHtml.Visible = false;
                                rtbBtnLRMI.Visible = true;
                                ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                                    "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'HTML','Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                                    true);
                            }
                        }
                        else
                        {
                            if (DistrictParms.LRMITagging == "true")
                            {
                                rtbBtnWord.Visible = true;
                                rtbBtnLRMI.Visible = true;
                                ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                                    "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'Word' , 'Tags', 'Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                                    true);
                        }
                        else
                        {
                            rtbBtnWord.Visible = true;
                                rtbBtnLRMI.Visible = false;
                                ScriptManager.RegisterStartupScript(this, typeof(string), "toolbar",
                                    "var mainToolBar_buttonVisibility = { 'Edit': ['Undo', 'Print', 'Word' ,'Configure', 'Proof', 'Reorder', 'OnlinePreview', 'AddItem', 'DeleteAssessment', 'Copy', 'Generate', 'sep'], 'Preview': ['Print', 'Format', 'Header'], 'Documents': [] };",
                                    true);
                            }
                        }
                    }

                }

                rtbBtnHtml.Visible = UserHasPermission(Permission.Icon_Test_PrintHTML);
                rtbBtnWord.Visible = UserHasPermission(Permission.Icon_Test_PrintWord);


                string documentTableHolderCSSClass = "";
                if (!selectedAssessment.IsProofed)
                {
                    if ((selectedAssessment.Category == AssessmentCategories.Classroom && DistrictParms.ClassroomUnproofedUpload) ||
                        (selectedAssessment.Category == AssessmentCategories.District && DistrictParms.DistrictUnproofedUpload) ||
                        (selectedAssessment.Category == AssessmentCategories.State && DistrictParms.StateUnproofedUpload) ||
                        (selectedAssessment.Category == AssessmentCategories.Summative && DistrictParms.SummativeUnproofedUpload))
                    {
                        mainTabStrip.FindTabByValue("Documents").Visible = true;
                        if ((selectedAssessment.Category == AssessmentCategories.Classroom && DistrictParms.ClassroomReviewAssessment) ||
                            (selectedAssessment.Category == AssessmentCategories.District && DistrictParms.DistrictReviewAssessment) ||
                            (selectedAssessment.Category == AssessmentCategories.State && DistrictParms.StateReviewAssessment) ||
                            (selectedAssessment.Category == AssessmentCategories.Summative && DistrictParms.SummativeReviewAssessment))
                        {
                            documentTableHolderCSSClass += " doc_HideAA doc_EditReviewer doc_HideCheck";    //4
                        }
                        else
                        {
                            documentTableHolderCSSClass += " doc_ShowAA doc_HideReviewer doc_HideCheck ";   //2
                        }
                    }
                    else
                    {
                        mainTabStrip.FindTabByValue("Documents").Visible = false;   // 1, 3
                    }
                }
                else
                {
                    //proofed
                    mainTabStrip.FindTabByValue("Documents").Visible = true;
                    if ((selectedAssessment.Category == AssessmentCategories.Classroom && DistrictParms.ClassroomReviewAssessment) ||
                        (selectedAssessment.Category == AssessmentCategories.District && DistrictParms.DistrictReviewAssessment) ||
                        (selectedAssessment.Category == AssessmentCategories.State && DistrictParms.StateReviewAssessment) ||
                        (selectedAssessment.Category == AssessmentCategories.Summative && DistrictParms.SummativeReviewAssessment))
                    {
                        if ((selectedAssessment.Category == AssessmentCategories.Classroom && DistrictParms.ClassroomUnproofedUpload) ||
                        (selectedAssessment.Category == AssessmentCategories.District && DistrictParms.DistrictUnproofedUpload) ||
                        (selectedAssessment.Category == AssessmentCategories.State && DistrictParms.StateUnproofedUpload) ||
                        (selectedAssessment.Category == AssessmentCategories.Summative && DistrictParms.SummativeUnproofedUpload))
                        {
                            documentTableHolderCSSClass += " doc_ShowAA " + (chkDisplayReview.Checked ? " doc_ShowReviewer " : " doc_HideReviewer ") + " doc_ShowCheck ";    // 7.2, 7.3
                        }
                        else
                        {
                            documentTableHolderCSSClass += " doc_ShowAA doc_HideReviewer doc_HideCheck ";       // 8
                        }
                    }
                    else
                    {
                        documentTableHolderCSSClass += " doc_ShowAA doc_HideReviewer doc_HideCheck "; // 5
                    }
                }


                documentTableHolder.Attributes.Add("class", documentTableHolderCSSClass);

                bankColumn.Visible = bankSlider.Visible = !selectedAssessment.IsExternal;
                imgAddNewForm.Visible = selectedAssessment.MultiForm && !selectedAssessment.IsExternal;


#if DEBUG
                ScriptManager.RegisterStartupScript(this, typeof(string), "debug_message", "function debug_message(message) {/*THIS IS NOT SAFE, BREAKS BANK DRAG*/ /*$('#bankScroller').html($('#bankScroller').html() + message + '<br/>');*/}", true);
#else
                ScriptManager.RegisterStartupScript(this, typeof(string), "debug_message", "function debug_message(message) {}", true);
#endif

                ConfigureForProofedAssessment(selectedAssessment);
                SetupToolBar(selectedAssessment);
                BindStandardFilter();
                BindOutlineStandardTree();

            }
        }

        private void LoadCorrectNewDialog()
        {
            itemChange_dialog.InnerHtml = "<br /><div style=\"font-size: 12px; border: 1px solid black; padding-left: 10px; padding-right: 10px;\">" +
                                          "<p>This item has been changed and Elements needs more information on how to handle the update.</p>" +
                                          "<p>Select <b>\"Correction\"</b> if this change should result in an update to the original item.This correction will be made to the item in the item bank" +
                                          ((DistrictParms.LoadDistrictParms().CorrectUnproofedTests && selectedAssessment.TestCategory == "District") ? " and will be applied to the item on any district assessments that are not already Proofed - Ready to print." : ".") +
                                          "</p><p>Select <b>\"New\"</b> if you want this item to be added as a new item.</p></div>";
        }

		private void LoadAssessmentObject(string imagesUrl = "")
        {
            if (selectedAssessment != null) return; // no need to load twice
            selectedAssessment = Assessment.GetAssessmentAndQuestionsByID(_assessmentID, imagesUrl, false);
            formCount.Value = selectedAssessment.Forms.Count.ToString();
            //Page.Title = "Term " + selectedAssessment.Term + " " + selectedAssessment.TestType + " - " + selectedAssessment.Grade + " Grade " + selectedAssessment.Subject + (selectedAssessment.Course == selectedAssessment.Subject ? string.Empty : " " + selectedAssessment.Course);
            TestQuestion.LoadStandardsForTestQuestions(selectedAssessment.Items);

            Cache.Insert('A' + _assessmentID.ToString(), selectedAssessment, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(2));
            string assessmentTitle = "Term " + selectedAssessment.Term + " " + selectedAssessment.TestType + " - " + selectedAssessment.Grade + " Grade " + selectedAssessment.Subject + (selectedAssessment.Course.ToString() == selectedAssessment.Subject.ToString() ? string.Empty : " " + selectedAssessment.Course);
            //if(SecureType)
            //{
            //    assessmentTitle = "Term " + selectedAssessment.Term + " " + selectedAssessment.TestType + " - " + selectedAssessment.Grade + " Grade " + selectedAssessment.Subject + (selectedAssessment.Course.ToString() == selectedAssessment.Subject.ToString() ? string.Empty : " " + selectedAssessment.Course);
            //}
            
            ScriptManager.RegisterStartupScript(this, typeof(string), "Title", "setTitle( '" + assessmentTitle + "');", true);
        }

        private string GetSecureTestCategory(string TestCategory)
        {
            bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            Dictionary<string, bool> dictionaryItem;
            dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(TestCategory);
            bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            string testCategory = TestCategory;
            if (hasPermission && isSecuredFlag && SecureType)
                {
                    testCategory = selectedAssessment.TestType;
                }
            return testCategory;
        }

        private DataTable GetStandardsData(string standardSet = "", bool includeStandardFilter = false)
        {
            if (_dtStandardsWithSetInTree == null || _dtStandards == null)
            {
                string testCategory = GetSecureTestCategory(selectedAssessment.TestCategory);
                dtItemBank itemBanks = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser,
                                                                                        testCategory);
                _dtStandardsWithSetInTree = Thinkgate.Base.Classes.Standards.GetStandardsByCurriculum(null, selectedAssessment.TestCategory, selectedAssessment.TestType, selectedAssessment.currCourseID, null, itemBanks);
                _dtStandards = _dtStandardsWithSetInTree.Copy();
                if (_standardsSetListTable == null)
                {
                    // kind of a kludge. The only for us to be here where the setlist table is empty is on a post back where we are reloading the outine dropdown, in this, case we just need the raw table. Any attempt to access the set list below, will throw an error. 
                    return _dtStandards;
                }
                Dictionary<string, int> ss_Lookup = new Dictionary<string, int>();
                int ss_Count = -1;
                // generate a lookup dictionary for the standard sets
                foreach (DataRow row in _standardsSetListTable.Rows)
                {
                    ss_Lookup.Add(row["Standard_Set"].ToString(), ss_Count--);
                }
                // for any 'root' nodes, adjust parent so they will point at eventual standard set parent
                foreach (DataRow row in _dtStandardsWithSetInTree.Rows)
                {
                    if (row["ParentID"].Equals(0))
                    {
                        row["ParentID"] = ss_Lookup[row["Standard_Set"].ToString()];
                    }
                }

                if (includeStandardFilter && SessionObject.LoggedInUser.StandardFilters.Count > 0 && !string.IsNullOrEmpty(SessionObject.LoggedInUser.StandardFilters.First().Value))
                {
                    List<int> standardsFromFilter = GetListFromDelimitedString(Convert.ToChar(","), SessionObject.LoggedInUser.StandardFilters.Values.First()).Select(s => int.Parse(s)).ToList();

                    DataTable dtStandardsForFilter = Thinkgate.Base.Classes.Standards.GetStandardsByIDs(standardsFromFilter);
                    foreach (DataRow row in dtStandardsForFilter.Rows)
                    {
                        DataRow new_row = _dtStandardsWithSetInTree.NewRow();
                        //PLH - 10/15/2013 - Hate having to do this but appending a negative to standardID to avoid duplicate dictionary entries
                        new_row["StandardID"] = "-" + row["StandardID"].ToString();
                        new_row["StandardName"] = row["Standard_Set"] + "." + row["StandardName"].ToString();
                        new_row["StandardText"] = row["Desc"].ToString();
                        new_row["ParentID"] = 0;
                        new_row["ItemBank"] = string.Empty;
                        new_row["FullStandardName"] = row["Standard_Set"] + "." + row["StandardName"].ToString();
                        new_row["Standard_Set"] = row["Standard_Set"].ToString();
                        _dtStandardsWithSetInTree.Rows.Add(new_row);
                    }

                    DataRow headerRow = _dtStandardsWithSetInTree.NewRow();
                    headerRow["ParentID"] = DBNull.Value;
                    headerRow["StandardID"] = 0;
                    headerRow["StandardName"] = "Filter";
                    headerRow["FullStandardName"] = "Filter";
                    _dtStandardsWithSetInTree.Rows.Add(headerRow);
                }

                // now add new rows for the standard sets, these will act as the roots of the standards. We use a negative number as the StandardID and the JS checks for positive to prevent a user selecting a standard set when a standard is requried
                foreach (DataRow row in _standardsSetListTable.Rows)
                {
                    DataRow new_row = _dtStandardsWithSetInTree.NewRow();
                    new_row["ParentID"] = DBNull.Value;
                    new_row["StandardID"] = ss_Lookup[row["Standard_Set"].ToString()];
                    new_row["StandardName"] = row["Standard_Set"].ToString();
                    new_row["FullStandardName"] = row["Standard_Set"].ToString();
                    _dtStandardsWithSetInTree.Rows.Add(new_row);
                }
            }
            if (string.IsNullOrEmpty(standardSet))
            {
                return _dtStandardsWithSetInTree;
            }
            else
            {
                return _dtStandards;
            }
        }

        private List<string> GetListFromDelimitedString(char delimiter, string valueToParse)
        {
            List<string> lst = new List<string>();
            string[] listItems = valueToParse.Split(delimiter);
            foreach (string item in listItems)
            {
                lst.Add(item);
            }

            return lst;
        }

        #region Populate Controls

        /// <summary>
        /// Populates the profile labels.
        /// </summary>
        /// <param name="a">A.</param>
        private void PopulateProfileLabels(Assessment a)
        {
            lblTestName.Text = a.TestName;
            lblDescription.Text = a.Description;
            //lblHeadingDirectionsPreview.Text = "Description: " + a.Description;
            //lblItemCount.Text = (Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(a.Items.Count)).ToString();
            lblItemCount.Text = "Loading";
            lbl_TestCategory.Value = a.TestCategory;
            lbl_TestType.Value = a.TestType;
            AssessmentDirectionsContent.Text = String.IsNullOrEmpty(a.Directions) ? Assessment.DefaultDirections : a.Directions;
            AdminAssessmentDirectionsContent.Text = String.IsNullOrEmpty(a.AdministrationDirections) ? Assessment.AdministrationDefaultDirections : a.AdministrationDirections;       
            //radtab_Forms.Tabs[0].ImageUrl = "";
        }


        private void SetupToolBar(Assessment a)
        {
            IList<RadToolBarItem> allItems = mainToolBar.GetAllItems();
            foreach (RadToolBarItem item in allItems)
            {
                switch (item.Value)
                {
                    case "Proof":
                        if (a.IsProofed) item.ImageUrl = item.Attributes["ProofedImageUrl"];
                        break;
                    case "Undo":
                    case "AddItem":
                        //case "DeleteAssessment":
                        if (a.IsProofed)
                        {
                            mainToolBar.Items.Remove(item);
                        }
                        break;
                    case "Generate":
                    case "Reorder":
                        if (a.IsProofed || a.IsExternal)
                        {
                            mainToolBar.Items.Remove(item);
                        }
                        break;
                    case "Scheduler":
                        /* Need to consider the following parms/permissions regarding when to hide Scheduler Icon here:
                         * 
                         *		permission - Icon_AssessmentSchedules_Classroom_Single, Icon_AssessmentSchedules_District_Single, Icon_AssessmentScheudules_State_Single:  
                         *		When true, users in the application can schedule the single assessment they are working on (per the category of the assessment).
                         *		parm - AssessmentSchedulerProofedOption - When True, this means Users in the application can set scheduling for unproofed and proofed tests
                         *		parm - AssessmentSchedulerClassAssessments - When True, then Classroom Assesments (category) can be scheduled.
                        */

                        bool UserCanScheduleAssessment = false;
                        switch (selectedAssessment.Category)
                        {
                            case AssessmentCategories.State:
                                UserCanScheduleAssessment = (UserHasPermission(Permission.Icon_AssessmentSchedules_State_Single));
                                break;

                            case AssessmentCategories.District:
                                UserCanScheduleAssessment = (UserHasPermission(Permission.Icon_AssessmentSchedules_State_Single));
                                break;

                            case AssessmentCategories.Classroom:
                                UserCanScheduleAssessment = (UserHasPermission(Permission.Icon_AssessmentSchedules_State_Single));
                                break;
                        }


                        if ((!DistrictParms.AssessmentSchedulerProofedOption && !a.IsProofed) || !UserCanScheduleAssessment)
                        {
                            mainToolBar.Items.Remove(item);
                        }
                        break;
                }
            }
        }

        private void ConfigureForProofedAssessment(Assessment a)
        {
            if (!a.IsProofed)
            {
                ItemDefaultDirections = TestQuestion.DefaultDirections;
                cssProofed.Text = ".showIfProofed { display: none }";
                QA_Text_Visible_Style = "";              
                return;
            }
            ItemDefaultDirections = "";
            imgAddNewItem.Visible = false;
            imgAddNewForm.Visible = false;
            txtAddNumItems.Visible = false;
            QA_Text_Visible_Style = "display: none";

            var tabEdit = mainTabStrip.FindTabByValue("Edit");
            tabEdit.Text = "Update";
            //test.Visible = false;       // this is the primary text editor. For some reason, it has to be called test or it auto expands to extreme heights. I simply don't understand why.
            bankColumn.Visible = false;
            bankSlider.Visible = false;
            AdminAssessmentDirectionCellReadOnly.Visible = true;
            AdminAssessmentDirectionsCell.Visible = false;
            AdminAssessmentDirectionCellReadOnlyLabel.Text = string.IsNullOrEmpty(a.AdministrationDirections) ? Assessment.AdministrationDefaultDirections : a.AdministrationDirections;
            AssessmentDirectionsCell.Visible = false;
            AssessmentDirectionCellReadOnly.Visible = true;
            AssessmentDirectionCellReadOnlyLabel.Text = string.IsNullOrEmpty(a.Directions) ? Assessment.DefaultDirections : a.Directions;
            cssProofed.Text = ".borderIfProofed {border: 1px dashed black} .RadComboBox_Web20 .disabled_dropdown_placeholder { /*background-image: url('../Images/rcbSpriteGray.png') !important */ } .response_letter  { border-left: 1px dashed black; border-right: 1px dashed black; }";

            mathjaxLoad.Visible = true;
        }

        private void BindFormData()
        {
            if (_serializer == null) _serializer = new JavaScriptSerializer();
            ScriptManager.RegisterStartupScript(this, typeof(string), "AssessmentForms", "var AssessmentForms=" + _serializer.Serialize(selectedAssessment.PrepareFormsForJson()) + "; rebuildDocumentsTable();", true);
        }

        #endregion

        #region Bind List Controls

        private void BindComboBoxes()
        {
            comboRigor.DataSource = Rigor.PossibleValueList();
            comboRigor.DataBind();
            comboItemWeight.DataSource = ItemWeight.PossibleValueList();
            comboItemWeight.DataBind();
            comboDifficultyIndex.DataSource = DifficultyIndex.PossibleValueList();
            comboDifficultyIndex.DataBind();
        }

        /// <summary>
        /// Binds the assessment items to the item repeater.
        /// </summary>
        /// <param name="items">The items.</param>
        public void BindAssessmentItems()
        {
            if (_serializer == null) _serializer = new JavaScriptSerializer();
            _serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer(), new AddendumMinSerializer(), new RubricMinSerializer(), });
            /*ScriptManager.RegisterStartupScript(this, typeof(string), "AssessmentItems", "var Rubrics = " + _serializer.Serialize(selectedAssessment.Rubrics) + ";"
                                                                                                + " var Addendums = " + _serializer.Serialize(selectedAssessment.Addendums) + ";"
                                                                                                + " var AssessmentItems=" + _serializer.Serialize(selectedAssessment.Items) + "; buildItemIndex();"
                                                                                                
                                                                                        , true);*/
            //selectedAssessment.LoadRubrics();
            _serializer.MaxJsonLength = int.MaxValue;
            selectedAssessment.LoadAddendums();
            ScriptManager.RegisterStartupScript(this, typeof(string), "AssessmentItems", "var AssessmentItems=" + _serializer.Serialize(selectedAssessment.Items) + "; buildItemIndex();", true);

            //Used for Item Bank security

            List<ItemBank> ibList = Assessment.GetItemBanksForAssessment(selectedAssessment.AssessmentID);
            var itemBankEditTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, selectedAssessment.Category.ToString());
            var itemBankCopyTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankCopy, selectedAssessment.Category.ToString());
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
            string renderIBEditArray = "var IBEditArray = [" + serializer.Serialize(itemBankEditTbl) + "]; ";
            renderIBEditArray += "var IBCopyArray = [" + serializer.Serialize(itemBankCopyTbl) + "]; ";
            renderIBEditArray += "var IBAssessmentArray = [" + serializer.Serialize(ibList) + "]; ";
            ScriptManager.RegisterStartupScript(this, typeof(string), "IBEditArray", renderIBEditArray, true);
            //End
        }

        /// <summary>
        /// Binds the data for the rad tree that will be used as the item standard dropdown. We have only one on a page that we move to the correct position to cut down on load.
        /// </summary>
        /// <param name="objtree">The objtree.</param>
        protected void BindCommonStandardTree()
        {
            RadTreeView objTree = (RadTreeView)comboStandard.Items[0].FindControl("CommonStandardTree");

            objTree.DataSource = GetStandardsData(string.Empty, true);
            objTree.DataBind();
        }

        protected void BindStandardsSetList()
        {
            _standardsSetListTable = Thinkgate.Base.Classes.Standards.GetStandardSets(currUserID);
            OutlineStandardSet.DataSource = _standardsSetListTable;
            OutlineStandardSet.DataBind();
            OutlineStandardSet.SelectedIndex = 0;
        }

        protected void BindStandardFilter()
        {
            if (SessionObject.LoggedInUser.StandardFilters.Values.Count > 0)
            {
                DataTable dtStandards = GetStandardsData();
                DataView dv = new DataView(dtStandards);
                dv.RowFilter = "StandardID in (" + SessionObject.LoggedInUser.StandardFilters.Values.First() + ")";
                comboStandardFilter.DataSource = dv;
                comboStandardFilter.DataBind();
            }
        }

        protected void BindOutlineStandardTree()
        {
            RadTreeView objTree = (RadTreeView)comboStandardOutline.Items[0].FindControl("OutlineStandardTree");
            BindStandardTreeGeneral(ref objTree);
        }

        /// <summary>
        /// Loads and binds the list of standard outline tree on right hand side
        /// </summary>
        protected void BindStandardTreeGeneral(ref RadTreeView objtree, bool applyStandardFilter = false)
        {
            if (selectedAssessment == null)
            {
                selectedAssessment = (Assessment)Cache.Get(this.CacheKey);

                if (selectedAssessment == null)
                {
                    LoadAssessmentObject();
                }
            }
            string standardSet = OutlineStandardSet.SelectedItem.Text;//.Replace("Set: ", "");
            //dtItemBank itemBanks = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, selectedAssessment.TestCategory);
            DataTable dtStandards;
            if (applyStandardFilter)
            {
                dtStandards = GetStandardsData();
                //dtStandards = Thinkgate.Base.Classes.Standards.GetStandardsByCurriculum(null, null, null, null, selectedAssessment.TestCategory, currUserID, selectedAssessment.currCourseID, itemBanks);
            }
            else
            {
                dtStandards = GetStandardsData(standardSet);
            }
            if (!dtStandards.Columns.Contains("StandardNameWithCount"))
            {
                dtStandards.Columns.Add(new DataColumn("StandardNameWithCount", typeof(string)));
            }

            DataView dv = new DataView(dtStandards);
            if (applyStandardFilter)
            {
                if (SessionObject.LoggedInUser.StandardFilters.Values.Count > 0)
                {
                    // Ben has this data broken so I'm just waiting
                    dv.RowFilter = "StandardID in (" + SessionObject.LoggedInUser.StandardFilters.Values.First() + ")";
                }
                // targetData = dtStandards.Select("StandardID in (" + SessionObject.LoggedInUser.StandardFilters + ")");
            }
            else
            {
                dv.RowFilter = "Standard_Set = '" + standardSet + "'";

            }
            foreach (DataRowView r in dv)
            {
                if (r["ParentID"].Equals(0))
                {
                    r["ParentID"] = DBNull.Value;
                }
                r["StandardNameWithCount"] = (applyStandardFilter ? r["FullStandardName"] : r["StandardName"]) + " (" + r["ItemCount"].ToString() + ")";
                if (applyStandardFilter)
                {
                    r["ParentID"] = DBNull.Value;   // this is temporary until I can figure out how to handle nonexistant parents when filtered
                }
            }

            objtree.DataSource = dv;
            objtree.DataBind();
        }


        /// <summary>
        /// Once a standard is selected, need to load the bank of questions. Binds to bank repeater
        /// </summary>
        private string LoadBankQuestions(int assessmentID, Label targetCountLabel, dtGeneric_Int standardList = null, Dictionary<int, string> standard_lookup = null, string keywords = null)
        {
            if (selectedAssessment == null)
            {
                selectedAssessment = (Assessment)Cache.Get(this.CacheKey);

                if (selectedAssessment == null)
                {
                    LoadAssessmentObject();
                }
            }
            var itemList = new List<BankQuestion>();

            string testCategory = GetSecureTestCategory(selectedAssessment.TestCategory);

            dtItemBank itemBanks = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory);

            if (standardList != null)
            {
                if (standardList.Count > 0)
                {
                    itemList = BankQuestion.GetBankQuestionsByStandardList(assessmentID, standardList, itemBanks, true,
                                                                    selectedAssessment.currCourseID,
                                                                    selectedAssessment.Year);
                    foreach (var ques in itemList)
                    {
                        ques.StandardName = standard_lookup[ques.StandardID];
                    }
                }
            }
            else
            {
                itemList = BankQuestion.GetBankQuestionsByKeyword(keywords, itemBanks, true,
                                                                   selectedAssessment.currCourseID,
                                                                   selectedAssessment.Year);
                foreach (var ques in itemList)
                {
                    if (ques.Standards.Count > 0)
                    {
                        //foreach (var standard in ques.Standards)
                        //{
                        //    var row = StandardMasterList.GetStandardRowByID(standard.ID);
                        //    standard.StandardName = row["StandardName"].ToString();
                        //    standard.Desc = row["Desc"].ToString();
                        //}

                        // set initial standard to first one for item
                        ques.StandardID = ques.Standards[0].ID;
                        ques.StandardName = ques.Standards[0].StandardName;
                    }
                }
            }

            if (itemList.Count > 0)
            {
                //Start: Bug#:252 and 247 , 18 Dec 2012: Jeetendra Kumar, When a keyword search is executed from an assessment you cannot end the search.                
                if (itemList.Count > 500)
                {
                    targetCountLabel.Text = "More than 500 items found. Please narrow search criteria and use expanded Item search.";
                    return "[]";
                }
                //End: Bug#:252 and 247 , 18 Dec 2012: Jeetendra Kumar, When a keyword search is executed from an assessment you cannot end the search. 

                targetCountLabel.Text = itemList.Count.ToString() + " items found";

                if (_serializer == null) _serializer = new JavaScriptSerializer();
                _serializer.RegisterConverters(new JavaScriptConverter[] { new BankQuestionSerializerForAssessmentPage(), new StandardsSerializer() });

                return _serializer.Serialize(itemList);
            }
            else
            {
                targetCountLabel.Text = "No items found";

                return "[]";
            }
        }

        /// <summary>
        /// Binds the standards for each bank question to the standards repeater
        /// </summary>
        protected void BindStandardsForQuestions(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem == null) return;

            List<Thinkgate.Base.Classes.Standards> target_item = ((BankQuestion)e.Item.DataItem).Standards;
            Repeater standardRepeater = e.Item.FindControl("standardRepeater") as Repeater;
            standardRepeater.DataSource = target_item;
            standardRepeater.DataBind();
        }

        /// <summary>
        /// The OnNodeDataBound event of the Common Standard tree control. Needed to bind the 3rd field ("button text") as custom attribute
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Thes <see cref="Telerik.Web.UI.RadTreeNodeEventArgs"/> instance containing the event data.</param>
        protected void CommonStandardTree_OnNodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            DataRowView dataSourceRow = (DataRowView)e.Node.DataItem;
            e.Node.Attributes["ButtonText"] = dataSourceRow["FullStandardName"].ToString();
        }

        /// <summary>
        /// The OnNodeDataBound event of the OutlineStandardTree control. Needed to bind the 3rd field ("button text") as custom attribute
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Telerik.Web.UI.RadTreeNodeEventArgs"/> instance containing the event data.</param>
        protected void OutlineStandardTree_OnNodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            DataRowView dataSourceRow = (DataRowView)e.Node.DataItem;
            e.Node.Attributes["ButtonText"] = dataSourceRow["FullStandardName"].ToString();
        }

        protected void BindRadtab_Forms()
        {
            if (selectedAssessment.Forms.Count > 5)
            {
                radtab_Forms.Width = 276;
                radtab_Forms.ScrollChildren = true;
            }

            radtab_Forms.DataSource = selectedAssessment.Forms;
            radtab_Forms.DataBind();

            if (!selectedAssessment.IsProofed && selectedAssessment.Forms.Count > 1)
            {
                foreach (RadTab tab in radtab_Forms.Tabs)
                {
                    tab.CssClass = "widetab";       // in order to get proper scrolling behavior when dealing with images, the width of tab needs to be determined earlier (than an image load), so telerik calculations can occur properly
                    tab.ImageUrl = "~/Images/CrossRedNew.png";
                }
            }
        }

        #endregion

        #region Reloaders

        /// <summary>
        /// Call this when a change has been made to the assessment object we're working with, and we need to rebind the questions
        /// </summary>
        protected void RefreshQuestionsPanel(object sender = null, EventArgs e = null)
        {
            LoadAssessmentObject();
            //BindAddendums();
            //BindRubrics();

            BindAssessmentItems();
            BindFormData();
            if (IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "ajaxPanel_Items_Loaded",
                                                    "ajaxPanel_Items_Loaded();", true);

                ScriptManager.RegisterStartupScript(this, typeof(string), "CkEditorReload",
                                                   "CkEditorReload();", true);
            }
        }

        /// <summary>
        /// Call this when a change has been made to the assessment object we're working with, and we need to rebind the questions
        /// </summary>
        protected void RefreshDocumentsTable(object sender = null, EventArgs e = null)
        {
            LoadAssessmentObject();
            BindFormData();
            documentTableHolder.Attributes["class"] =
                documentTableHolder.Attributes["class"].Replace("ReviewUnchecked ", "").Replace("ReviewChecked ", "") +
                (chkDisplayReview.Checked ? " ReviewChecked" : " ReviewUnchecked");
        }
        #endregion

        #region Event Handlers


        protected void KeywordSearch(object sender, EventArgs e)
        {
            string keywords = txtKeyword.Text;

            if (txtKeyword.Text.Length > 0)
            {
                var itemListStr = LoadBankQuestions(_assessmentID, lblBankCount_keyword, null, null, keywords);

                ScriptManager.RegisterStartupScript(this, typeof(string), "ajaxPanel_Bank_keyword_Loaded", "var Bank_keyword_items=" + itemListStr + "; ajaxPanel_Bank_keyword_Loaded()", true);
            }
            else
            {
                lblBankCount_keyword.Text = "Keyword cannot be blank.";
                ScriptManager.RegisterStartupScript(this, typeof(string), "ajaxPanel_Bank_keyword_Loaded", "ajaxPanel_Bank_keyword_Loaded()", true);
            }
        }

        protected void DeleteForm(object sender, EventArgs e)
        {
            LoadAssessmentObject();
            int targetForm = DataIntegrity.ConvertToInt(txt_TargetForm.Text);
            var activeForm = selectedAssessment.Forms.Count - 1;
            selectedAssessment.DeleteSpecificForm(targetForm);
            formCount.Value = selectedAssessment.Forms.Count.ToString();
            ScriptManager.RegisterStartupScript(this, typeof(string), "delete_message", "fadeMessage();", true);
            BindRadtab_Forms();
            BindFormData();

            if (activeForm>0)
            {
                Telerik.Web.UI.RadTab tab = radtab_Forms.FindTabByValue(activeForm.ToString());
                tab.Selected = true;
                ScriptManager.RegisterStartupScript(this, typeof(string), "activeForm", "renderForm(" + activeForm.ToString() + ");", true);
            }
         
        }

        protected void AddNewForm(object sender, EventArgs e)
        {
            LoadAssessmentObject();

            selectedAssessment.AddForm();
            formCount.Value = selectedAssessment.Forms.Count.ToString();
            BindRadtab_Forms();
            BindFormData();
        }

        protected void AddBlankItem(object sender, EventArgs e)
        {
            int formId = DataIntegrity.ConvertToInt(txt_ActiveForm.Text);
            LoadAssessmentObject();

            selectedAssessment.AddBlankItem(formId, selectedAssessment.TestType);

            RefreshQuestionsPanel(sender, e);
        }

        protected void DeleteDocument(object sender, EventArgs e)
        {
            int formToDelete = DataIntegrity.ConvertToInt(FormToDelete.Text);
            string typeToDelete = TypeToDelete.Text;
            Assessment.DeleteAssessmentDocument(_assessmentID, formToDelete, typeToDelete, SessionObject.LoggedInUser.Page);
            RefreshDocumentsTable(sender, e);
        }

        /// <summary>
        /// Items have been dragged from bank, need to update the assessment in the DB and reload the updated assessment
        /// </summary>
        protected void AddItemsFromBank(object sender, EventArgs e)
        {

            if (_serializer == null) _serializer = new JavaScriptSerializer();
            ItemsToAdd[] newItems = _serializer.Deserialize<ItemsToAdd[]>(txt_QuestionIDsToAdd.Text);

            string item_sort_order = txt_QuestionSort.Text;
            int formId = DataIntegrity.ConvertToInt(txt_ActiveForm.Text);

            var dtItemsToAdd = new Thinkgate.Base.DataAccess.dtGeneric_Int_Int();
            for (var j = 0; j < newItems.Length; j++)
            {
                dtItemsToAdd.Add(Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(newItems[j].id), Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(newItems[j].standardID));
            }

            LoadAssessmentObject();

            selectedAssessment.AddItemsToAssessmentFromBank(formId, dtItemsToAdd, item_sort_order/*, AssessmentForms*/);

            RefreshQuestionsPanel(sender, e);

        }

        protected void OutlineStandardSet_OnChange(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindOutlineStandardTree();
            comboStandardOutline.Text = "Search Standards";
            comboStandardOutline.Items[0].Text = "Search Standards";
            ScriptManager.RegisterStartupScript(this, typeof(string), "ajaxPanel_Bank_Loaded", "var Bank_items=[]; ajaxPanel_Bank_Loaded()", true);
            lblBankCount.Text = "";
        }

        protected void comboStandardFilter_OnSelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            int selectedStandard = DataIntegrity.ConvertToInt(e.Value);
            var standardList = new dtGeneric_Int();
            standardList.Add(selectedStandard);
            Dictionary<int, string> standard_lookup = new Dictionary<int, string>();
            standard_lookup.Add(selectedStandard, e.Text);

            var itemListStr = LoadBankQuestions(_assessmentID, lblBankCount_filter, standardList, standard_lookup);

            ScriptManager.RegisterStartupScript(this, typeof(string), "ajaxPanel_Bank_filter_Loaded", "var Bank_filter_items=" + itemListStr + "; ajaxPanel_Bank_filter_Loaded()", true);
        }

        /// <summary>
        /// User has clicked on a standard in the standard outline tree. Need to built the list of standards for that branch and pass to bank binder
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Telerik.Web.UI.RadTreeNodeEventArgs"/> instance containing the event data.</param>
        protected void OutlineStandardTree_OnClick(object sender, RadTreeNodeEventArgs e)
        {
            Dictionary<int, string> standard_lookup = new Dictionary<int, string>();
            _selectedStandardOutline = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(e.Node.Value);
            standard_lookup.Add(_selectedStandardOutline, e.Node.Attributes["ButtonText"]);

            var standardList = new dtGeneric_Int();
            standardList.Add(_selectedStandardOutline);

            foreach (RadTreeNode node in e.Node.GetAllNodes())
            {
                standardList.Add(DataIntegrity.ConvertToInt(node.Value));
                standard_lookup.Add(DataIntegrity.ConvertToInt(node.Value), node.Text.Substring(0, node.Text.IndexOf("(")));
            }

            RadTreeView tree = (RadTreeView)sender;
            RadComboBoxItem comboItem = (RadComboBoxItem)tree.NamingContainer;
            RadComboBox combo = (RadComboBox)comboItem.NamingContainer;
            combo.Items[0].Text = e.Node.Attributes["ButtonText"];

            e.Node.Selected = true;
            var itemListStr = LoadBankQuestions(_assessmentID, lblBankCount, standardList, standard_lookup);

            ScriptManager.RegisterStartupScript(this, typeof(string), "ajaxPanel_Bank_Loaded", "var Bank_items=" + itemListStr + "; ajaxPanel_Bank_Loaded()", true);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Simple helper class used to pass in the id and selected standard for items being added from the bank
        /// </summary>
        [Serializable]
        public class ItemsToAdd
        {
            public int id;
            public int standardID;
        }

        #endregion

        protected void AddBlankItems(object sender, EventArgs e)
        {
            int formId = DataIntegrity.ConvertToInt(txt_ActiveForm.Text);
            int numItems = !string.IsNullOrEmpty(txtAddNumItems.Text) ? DataIntegrity.ConvertToInt(txtAddNumItems.Text) : 0;
            LoadAssessmentObject();

            for (int i = 0; i < numItems; i++)
            {
                selectedAssessment.AddBlankItem(formId, selectedAssessment.TestType);
            }

            ScriptManager.RegisterStartupScript(this, typeof(string), "ClearTextBox",
                                    "document.getElementById('txtAddNumItems').value = '';", true);

            RefreshQuestionsPanel(sender, e);
        }

        protected override object LoadRecord(int xId)
        {
            return Assessment.GetAssessmentByID(xId);
        }

    }
}
