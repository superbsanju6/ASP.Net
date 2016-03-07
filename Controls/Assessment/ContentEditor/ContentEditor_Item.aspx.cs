using System.Linq;
using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Utilities;
using Thinkgate.Classes;
using Thinkgate.Utilities;
using CultureInfo = System.Globalization.CultureInfo;

namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public class AssessmentItemResponseWithItemID : AssessmentItemResponse
    {
        public int ItemID { get; set; }
        public string ItemType { get; set; }

        public AssessmentItemResponseWithItemID(int id, string distractorText, char letter)
            : base(id, distractorText, letter)
        {
        }
    }

    public partial class ContentEditor_Item : BasePage
    {
        #region Members

        public int AddendumID;
        public Addendum SelectedAddendum;

        protected bool UseTq;

        private CourseList _standardCourseList;
        private IEnumerable<Grade> _gradeList;
        private dtItemBank _itemBankTbl;
        private IEnumerable<Subject> _subjectList;
        private string _sItemID;
        private string _sQuestionType;
        private int _iAssessmentID;
        private TestQuestion _itemTestQuestion;
        private BankQuestion _itemBankQuestion;
        private JavaScriptSerializer _serializer;
        private string _itemReservation;
        private string _imagesUrl;
        private string _testType = string.Empty;

        #endregion Members

        public int ItemID { get; set; }

        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(_testType);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }

        #region Properties

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public class StandardListForArray
        {
            public string EncID { get; set; }
            public string StandardName { get; set; }
        }

        #region Protected

        /// <summary>
        /// Get the initial values from query string 
        /// Loads the test question or bank question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            _itemBankTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");

            _imagesUrl = ConfigHelper.GetImagesUrl();
            lbl_OTCUrl.Value = AppSettings.OTCUrl + DistrictParms.ClientID.ToString(CultureInfo.CurrentCulture);

            UpdateItem();
            LoadItem();
            _standardCourseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);
            _gradeList = _standardCourseList.GetGradeList();
            _subjectList = _standardCourseList.FilterByGrade(UseTq ? _itemTestQuestion.Grade : _itemBankQuestion.Grade).GetSubjectList();

            var gradeList = new List<string>();
            var subjectList = new List<string>();
            gradeList.Add(UseTq ? _itemTestQuestion.Grade : _itemBankQuestion.Grade);
            subjectList.Add(UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject);

            BindPageControls();
            SetItemPageValues();

            ScriptManager.RegisterStartupScript(this, typeof(string), "appClient", "var appClient = '" + AppClient() + "';", true);
            ScriptManager.RegisterStartupScript(this, typeof (string), "debug_message", "function debug_message(message) {}", true);

            rtbBtnLRMI.Visible = DistrictParms.LRMITagging == "true";
        }

        /// <summary>
        /// Load standards
        /// </summary>
        /// <param name="lStandards"></param>
        protected void LoadStandardsArray(List<Base.Classes.Standards> lStandards)
        {
            List<StandardListForArray> std = new List<StandardListForArray>();

            foreach (var stds in lStandards)
            {
                StandardListForArray s = new StandardListForArray();
                s.EncID = Standpoint.Core.Classes.Encryption.EncryptString(stds.ID.ToString(CultureInfo.CurrentCulture));
                s.StandardName = stds.StandardName;
                std.Add(s);
            }
            if (std.Count == 1)
            {
                lblStandardNamePanelLink.Attributes.Add("onclick", "stopPropagation(event); OpenStandardText('" + std[0].EncID + "')");
                lblStandardNamePanelLink.Style.Add("display", "");
                imgRemoveStandard.Attributes.Add("onclick", "stopPropagation(event); document.getElementById('imgRemoveStandard').style.display = 'none'; RemoveStandardfromItem('" + std[0].EncID + "')");
                lblStandardNamePanelLink.InnerText = lStandards[0].StandardName;

                if (UseTq)
                {
                    imgRemoveStandard.Style.Add("display", "none");
                }
            }
            if (std.Count > 1)
            {
                rptrStandards.DataSource = std;
                rptrStandards.DataBind();
                lblStandardNamePanel.Text = "Standards listed below...";
            }
            if (std.Count != 1)
            {
                imgRemoveStandard.Style.Add("display", "none");
            }
            if (_serializer == null) _serializer = new JavaScriptSerializer();
            ScriptManager.RegisterStartupScript(this, typeof(string), "ItemStandardsArray", "var ItemStandardsArray=" + _serializer.Serialize(std) + "; ", true);
        }
        #endregion Protected

        #region Private Methods
        /// <summary>
        /// Update test question or bank question
        /// </summary>
        private void UpdateItem()
        {
            //#9006: The following code has been added to handle the item update in Mozilla Firefox browser, when 'cmbItemType' dropdown 
            //SelectedIndexChanged event is fired. 
            //This is needed becasue the Service methods are not getiting called from 'AssessmentItem_Change_Key_Value_Info'
            //and 'AssessmentItem_Change_ItemWeight' functions from 'Assessment_ChangeField_ItemType' function of 'ContentEditor_Item.js' file. 
            if (Request["__EVENTTARGET"] != null)
            {
                if (Request["__EVENTTARGET"].ToString(CultureInfo.CurrentCulture) == "cmbItemType")
                {
                    try
                    {
                        int itemID = Cryptography.GetDecryptedIDFromEncryptedValue(divEncItemID.InnerHtml, SessionObject.LoggedInUser.CipherKey);
                        int assessmentID = Cryptography.GetDecryptedIDFromEncryptedValue(divAssessmentID.InnerHtml, SessionObject.LoggedInUser.CipherKey);

                        if (assessmentID > 0)
                        {
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbItemType.Attributes["field"].ToString(CultureInfo.CurrentCulture), cmbItemType.SelectedValue);
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbScoreType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "W");
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbRubricType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "B");
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbRubricScoring.Attributes["field"].ToString(CultureInfo.CurrentCulture), "2");
                            TestQuestion.UpdateField(assessmentID, itemID, 0, "RubricID", "0");

                            TestQuestion.UpdateField(assessmentID, itemID, 0, "ItemWeight", "Normal");
                        }
                        else
                        {
                            SessionObject sessionObject = (SessionObject)Session["SessionObject"];

                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbItemType.Attributes["field"].ToString(CultureInfo.CurrentCulture), cmbItemType.SelectedValue);
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbScoreType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "W");
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbRubricType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "B");
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbRubricScoring.Attributes["field"].ToString(CultureInfo.CurrentCulture), "2");
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, "RubricID", "0");

                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, "ItemWeight", "Normal");
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Update Error", "MessageAlert('Error during update: " + ex.Message + "');", true);
                    }
                }
            }
        }

        /// <summary>
        /// Load Item from query string
        /// </summary>
        private void LoadItem()
        {
            _sItemID = "" + Request.QueryString["xID"];
            _sQuestionType = "" + Request.QueryString["qType"];         
            ItemID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey);

            _iAssessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "AssessmentID");
            lbl_TestCategory.Value = Request.QueryString["TestCategory"];

            lbl_TestType.Value = Request.QueryString["TestType"];
            if (_sItemID == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                if (_sQuestionType.Equals("TestQuestion"))
                {
                    // Remove the copy item button if we are coming from the item editor.
                    btnCopyItem.Visible = false;
                    if (ItemID == 0 && _iAssessmentID > 0 && !String.IsNullOrEmpty(_sItemID))
                    {
                        _itemTestQuestion = TestQuestion.GetTestQuestionByID(DataIntegrity.ConvertToInt(_sItemID), false);
                        ItemID = _itemTestQuestion.ID;
                        if (_itemTestQuestion.TestID != _iAssessmentID)
                        {
                            SessionObject.RedirectMessage = "Invalid entity ID provided in URL.";
                            Response.Redirect("~/PortalSelection.aspx", true);
                        }
                    }
                    else
                    {
                        _itemTestQuestion = TestQuestion.GetTestQuestionByID(ItemID, false);
                    }
                    UseTq = true;
                }
                else
                {
                    _itemBankQuestion = BankQuestion.GetQuestionByID(ItemID);
                    _itemBankQuestion.LoadAddendum();
                    UseTq = false;
                }

                LoadAddendums();
                LoadStandards();
                LoadItemText();

                _itemReservation = "No";
                if (UserHasPermission(Permission.Access_ItemReservation))
                    _itemReservation = "Yes";
                else
                    reservationDropDown.Enabled = false;

                string qType = UseTq ? "TestQuestion" : "";
                ScriptManager.RegisterStartupScript(this, typeof(string), "SetQType", "setQuestionType('"+qType+"');", true);
            }

            if (Request.QueryString["isCopy"] == "true")
            
                ScriptManager.RegisterStartupScript(this, typeof(string), "AlertForCopiedItem", "AlertForCopiedItem();", true);
        }

        /// <summary>
        /// Load item text based on test question or bank question
        /// </summary>
        private void LoadItemText()
        {
            //fetch the list of item responses ...
            List<AssessmentItemResponse> assessmentItemResponses = UseTq ? _itemTestQuestion.Responses : _itemBankQuestion.Responses;

            foreach (var assessmentItemResponse in assessmentItemResponses)
            {
                //perform an image url "repair" for each item
                assessmentItemResponse.DistractorText = AssessmentUtil.RepairImageUrl(assessmentItemResponse.DistractorText, _imagesUrl);
            }

            rptrItemQuestions.DataSource = assessmentItemResponses;
            rptrItemQuestions.DataBind();
        }

        /// <summary>
        /// Load addendum based on image url
        /// </summary>
        private void LoadAddendums()
        {
            Addendum itemAddendum = UseTq ? _itemTestQuestion.Addendum : _itemBankQuestion.Addendum;

            //Test Questions are not loading addendum names, so have to go get
            if (itemAddendum == null)
                return;
            if (UseTq)
                itemAddendum = Addendum.GetAddendumByAssessmentIDAndItemID(_itemTestQuestion.ID, _iAssessmentID);

            if (itemAddendum == null)
                return;

            itemAddendum.Addendum_Text = AssessmentUtil.RepairImageUrl(itemAddendum.Addendum_Text);

            lblNameAddendum.Text = itemAddendum.Addendum_Name;
            lblGenreAddendum.Text = itemAddendum.Addendum_Genre;
            lblTypeAddendum.Text = itemAddendum.Addendum_Type;
            if (itemAddendum.ID > 0)
            {
                trAddendum_No.Style.Add("display", "none");
                trAddendum_Yes.Style.Add("display", "");
            }
        }

        /// <summary>
        /// Load standards based on test question or bank questions
        /// </summary>
        private void LoadStandards()
        {
            List<Base.Classes.Standards> lStandards = new List<Base.Classes.Standards>();
            if (UseTq)
            {
                Base.Classes.Standards itemStandards = Base.Classes.Standards.GetStandardByID(_itemTestQuestion.StandardID);
                if (itemStandards != null)
                    lStandards.Add(itemStandards);
            }
            else
            {
                lStandards = _itemBankQuestion.Standards;
            }
            LoadStandardsArray(lStandards);
        }

        /// <summary>
        /// Set the content editor item control based on test question or bank question
        /// </summary>
        private void SetItemPageValues()
        {
            //RadEditorAddendumEdit.Content = useTestQuestion == true ? _itemTestQuestion.Question_Text : _itemBankQuestion.Question_Text;
            Question_Text.InnerHtml = UseTq ? _itemTestQuestion.Question_Text : _itemBankQuestion.Question_Text;
            Question_Text.InnerHtml = AssessmentUtil.RepairImageUrl(Question_Text.InnerHtml, _imagesUrl);

            gradeDropdown.Text = UseTq ? _itemTestQuestion.Grade : _itemBankQuestion.Grade;
            subjectDropdown.Text = UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject;

            lblGradeEdit.Text = UseTq ? _itemTestQuestion.Grade : _itemBankQuestion.Grade;
            lblSubjectEdit.Text = UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject;
            lblStatusEdit.Text = UseTq ? _itemTestQuestion.Status : _itemBankQuestion.Status;
            lblGradePreview.Text = UseTq ? _itemTestQuestion.Grade : _itemBankQuestion.Grade;
            lblSubjectPreview.Text = UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject;
            lblStatusPreview.Text = UseTq ? _itemTestQuestion.Status : _itemBankQuestion.Status;
            lblDistrict.Text = UseTq ? _itemTestQuestion.SourceName : _itemBankQuestion.SourceName;

            txtComments.Text = UseTq ? _itemTestQuestion.Comments : _itemBankQuestion.Comments;
            txtSource.Text = UseTq ? _itemTestQuestion.Source : _itemBankQuestion.Source;
            txtCredit.Text = UseTq ? _itemTestQuestion.Credit : _itemBankQuestion.Credit;
            lblExpirationDate.Text = UseTq ? _itemTestQuestion.CopyRightExpiryDate == null ? "No Expiration" : _itemTestQuestion.CopyRightExpiryDate + " (mm/dd/yyyy)" : _itemBankQuestion.CopyRightExpiryDate == null ? "No Expiration" : _itemBankQuestion.CopyRightExpiryDate + " (mm/dd/yyyy)";
            lblDateCreated.Text = UseTq ? _itemTestQuestion.DateCreated.ToString(CultureInfo.CurrentCulture) : _itemBankQuestion.DateCreated.ToString(CultureInfo.CurrentCulture);
            lblDateUpdated.Text = UseTq ? _itemTestQuestion.DateUpdated.ToString(CultureInfo.CurrentCulture) : _itemBankQuestion.DateUpdated.ToString(CultureInfo.CurrentCulture);
            lblAuthor.Text = UseTq ? _itemTestQuestion.CreatedByName : _itemBankQuestion.CreatedByName;
            txtKeywords.Text = UseTq ? _itemTestQuestion.Keywords : _itemBankQuestion.Keywords;

            divEncItemID.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(UseTq ? _itemTestQuestion.ID.ToString(CultureInfo.CurrentCulture) : _itemBankQuestion.ID.ToString(CultureInfo.CurrentCulture));
            divItemID.InnerHtml = UseTq ? _itemTestQuestion.ID.ToString(CultureInfo.CurrentCulture) : _itemBankQuestion.ID.ToString(CultureInfo.CurrentCulture);
            if (!_sQuestionType.Equals(""))
            {
                divAssessmentID.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(UseTq ? _iAssessmentID.ToString(CultureInfo.CurrentCulture) : "");
            }
            divTestQ.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(UseTq ? "true" : "false");
            string copyright = UseTq ? _itemTestQuestion.Copyright : _itemBankQuestion.Copyright;
            Boolean isAnchorItem = UseTq ? _itemTestQuestion.IsAnchorItem : _itemBankQuestion.IsAnchorItem;
            if (copyright == "Yes")
            {
                rbCopyRightYes.Checked = true;
            }
            else
            {
                rbCopyRightNo.Checked = true;
                trCredit.Style.Add("display", "none");
                trSource.Style.Add("display", "none");
                trExpiryDate.Style.Add("display", "none");

            }
            DistrictParms parms = DistrictParms.LoadDistrictParms();
            if (!parms.AllowCopyRightEditOnContentEditor)
            {
                rbCopyRightNo.Enabled = false;
                rbCopyRightYes.Enabled = false;
            }
            if (isAnchorItem)
                rbAnchorItemYes.Checked = true;
            else
                rbAnchorItemNo.Checked = true;

            SetDocumentUploadAndCommentControls();

            ItemContentTile_hdnThumbnailURL.Value = "?assessmentID=" + (UseTq ? "1" + _iAssessmentID.ToString(CultureInfo.CurrentCulture) : "0") + "&studentID=0&itemID=" + (UseTq ? _itemTestQuestion.ID : _itemBankQuestion.ID);


            if (UseTq)
            {
                tblItemBank.Style.Add("display", "none");
                ItemBankCheckBoxes.Style.Add("display", "none");
            }
        }

        /// <summary>
        /// Set Document Upoad and Allow Comment Control based on TestQuestion or BankQuestion Items
        /// </summary>
        private void SetDocumentUploadAndCommentControls()
        {
            if (UseTq)
            {
                //Set the DocumentUpload & Allow Comment control using TestQuestion Item
                SetDocumentUploadControl(_itemTestQuestion);
                SetAllowCommentControl(_itemTestQuestion);
                //if (!(lbl_TestType.Value.Contains("Performance") || lbl_TestType.Value.Contains("Observational")) && lbl_TestCategory.Value == "Classroom")
                //{
                //    rbDocUploadYes.Enabled = false;
                //    rbDocUploadNo.Enabled = false;
                //    rbAllowCommentNo.Enabled = false;
                //    rbAllowCommentYes.Enabled = false;
                //}
                //else
                //{
                //    rbDocUploadYes.Enabled = true;
                //    rbDocUploadNo.Enabled = true;
                //    rbAllowCommentNo.Enabled = true;
                //    rbAllowCommentYes.Enabled = true;
                //}
            }
            else
            {
                //Document Upload and Allow Comment configuration feature is only available for TestQuestion, not for BankQuestion
                trDocumentUpload.Visible = false;
                trAllowComments.Visible = false;
            }
        }

        /// <summary>
        /// Set Document Upload Control
        /// </summary>
        /// <param name="item"></param>
        private void SetDocumentUploadControl(QuestionBase item)
        {
            rbDocUploadYes.Checked = item.IsDocumentUpload;
            rbDocUploadNo.Checked = !item.IsDocumentUpload;
        }

        /// <summary>
        /// Set Allow Comment Control
        /// </summary>
        /// <param name="item"></param>
        private void SetAllowCommentControl(QuestionBase item)
        {
            rbAllowCommentYes.Checked = item.IsAllowComments;
            rbAllowCommentNo.Checked = !item.IsAllowComments;
        }

        /// <summary>
        /// Bind Item bank checkboxes
        /// Load grade, subject, item status, item type, itemreservation
        /// Show/Hide correct answer text based on question type
        /// </summary>
        private void BindPageControls()
        {
            BindItemBankCheckBoxes();
            LoadGradeDropdown();
            LoadSubjectDropDown();
            LoadItemReservation();
            LoadItemStatus();
            LoadItemType();
            ShowHideCorrectAnswerText();
        }

        /// <summary>
        /// Show/Hide answer text based on question type
        /// </summary>
        private void ShowHideCorrectAnswerText()
        {
            QuestionTypes questionTypes = (QuestionTypes)Enum.Parse(typeof(QuestionTypes), cmbItemType.SelectedValue.ToString(CultureInfo.CurrentCulture));

            if (questionTypes == QuestionTypes.S || questionTypes == QuestionTypes.E)
            { divCorrectAnswer.Style.Add("display", "none"); }
            else
            { divCorrectAnswer.Style.Add("display", "inline"); }
        }

        /// <summary>
        /// Bind the itembank check boxes
        /// </summary>
        private void BindItemBankCheckBoxes()
        {
            string renderItemsScript = "var itemBankList = [";

            string tbl = @"<table style=""width:100%;"">";
            int aCounter = 0;
            foreach (DataRow ibR in _itemBankTbl.Rows)
            {
                tbl += @"<tr>";
                tbl += @"<td class=""fieldLabel"" style=""width:20%; padding: 0px 10px 0px 10px; text-align:right;"">";
                tbl += @"<input type=""checkbox"" cbtype=""ItemBank"" ID=""ItemBank_" + ibR["TargetType"] + "_" + ibR["Target"] + @"""";
                tbl += @" class=""itemBankUpdate"" title=""Click to add this item to the " + ibR["Label"] + @" item bank""";

                tbl += @" TargetType=""" + ibR["TargetType"] + @"""";
                tbl += @" Target=""" + ibR["Target"] + @"""";
                tbl += @" multiBanks=""" + (ibR["MultiBanks"].ToString().ToLower() == "true" ? "true" : "false") + @"""";
                
                string tf;
                if (IsItemBankInMasterList(ibR["Label"].ToString()))
                {
                    tbl += @" inBank=""true""";
                    tbl += @" checked=""true""";
                    tf = "true";
                }
                else
                {
                    tbl += @" inBank=""false""";
                    tf = "false";
                }
                if (IsItemBankLocked(ibR))
                {
                    tbl += @" disabled=""true""";
                }
                tbl += @" onclick=""updateItemBanks(this);""/>";
                tbl += @"</td>";
                tbl += @"<td style=""width:80%"">" + ibR["Label"] + @"</td>";
                tbl += @"</tr>";

                aCounter++;
                renderItemsScript += "['" + ibR["TargetType"]+ "','" + ibR["Target"] + "','" + tf + "','" + (ibR["MultiBanks"].ToString().ToLower() == "true" ? "true" : "") + "']";
                if (aCounter != _itemBankTbl.Rows.Count)
                    renderItemsScript += ",";
            }
            renderItemsScript += "];";

            tbl += @"</table>";
            ItemBankCheckBoxes.InnerHtml = tbl;

            ScriptManager.RegisterStartupScript(this, typeof(string), "itemBankList", renderItemsScript, true);
        }

        /// <summary>
        /// Lock the item bank based on targettype
        /// </summary>
        /// <param name="drItemBank"></param>
        /// <returns></returns>
        private bool IsItemBankLocked(DataRow drItemBank)
        {
            if (drItemBank["TargetType"].ToString() == "2")
            {
                return true;
            }
            return DataIntegrity.ConvertToInt(drItemBank["TargetType"]) >= 6;
        }

        /// <summary>
        /// Set the itembank list label based on testquestion flag 
        /// </summary>
        /// <param name="itemBank"></param>
        /// <returns></returns>
        private bool IsItemBankInMasterList(string itemBank)
        {
            if (UseTq)
            {
                if (_itemTestQuestion.ItemBankList != null)
                {
                    for (int i = 0; i < DataIntegrity.ConvertToInt(_itemTestQuestion.ItemBankList.Count); i++)
                    {
                        if (_itemTestQuestion.ItemBankList[i].Label == itemBank)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (_itemBankQuestion.ItemBankList != null)
                {
                    for (int i = 0; i < DataIntegrity.ConvertToInt(_itemBankQuestion.ItemBankList.Count); i++)
                    {
                        if (_itemBankQuestion.ItemBankList[i].Label == itemBank)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Load grade dropdown
        /// </summary>
        private void LoadGradeDropdown()
        {
            gradeDropdown.Items.Clear();

            foreach (Grade grade in _gradeList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = grade.ToString();
                item.Value = grade.ToString();
                if (grade.ToString() == (UseTq ? _itemTestQuestion.Grade : _itemBankQuestion.Grade))
                    item.Selected = true;
                var gradeOrdinal = new Grade(grade.ToString());
                item.Attributes["gradeOrdinal"] = gradeOrdinal.GetFriendlyName();

                gradeDropdown.Items.Add(item);
            }
        }

        /// <summary>
        /// Load subject dropdown
        /// </summary>
        private void LoadSubjectDropDown()
        {
            subjectDropdown.Items.Clear();

            foreach (Subject subject in _subjectList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = subject.DisplayText;
                item.Value = subject.DisplayText;
                if (subject.DisplayText == (UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject))
                    item.Selected = true;
                var gradeOrdinal = new Subject(subject.DisplayText);
                item.Attributes["subjectOrdinal"] = gradeOrdinal.DisplayText;

                subjectDropdown.Items.Add(item);
            }

            RadComboBoxItem someitem = subjectDropdown.FindItemByValue((UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject));
            if (someitem == null)
            {
                RadComboBoxItem newitem = new RadComboBoxItem();
                newitem.Text = (UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject);
                newitem.Value = (UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject);
                newitem.Selected = true;

                var gOrdinal = new Base.Classes.Course((UseTq ? _itemTestQuestion.Subject : _itemBankQuestion.Subject));
                newitem.Attributes["subjectOrdinal"] = gOrdinal.CourseName;

                subjectDropdown.Items.Add(newitem);
            }
        }

        /// <summary>
        /// Load item reservation
        /// </summary>
        private void LoadItemReservation()
        {
            DataTable dtItemReservations = QuestionBase.ItemReservationsSelect(_itemReservation);

            bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            Dictionary<string, bool> dictionaryItem;
            string testCategory = string.Empty;
            if (Request.QueryString["TestCategory"] != null)
            {
                testCategory = Request.QueryString["TestCategory"];
            }
            if (Request.QueryString["TestType"] != null)
            {
                _testType = Request.QueryString["TestType"];
            }
            bool isSecuredFlag = false;
            if (!string.IsNullOrEmpty(testCategory))
            {
                dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory);
                isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            }
            if (hasPermission && isSecuredFlag)
            {
                if (!SecureType)
                {
                    for (var rowIndex = dtItemReservations.Rows.Count - 1; rowIndex >= 0; rowIndex--)
                    {
                        if (dtItemReservations.Rows[rowIndex].RowState != DataRowState.Deleted)
                        {
                            if (Convert.ToBoolean(dtItemReservations.Rows[rowIndex]["Secure"]))
                            { dtItemReservations.Rows[rowIndex].Delete(); }
                        }
                    }
                }
            }
            else
            {
                for (var rowIndex = dtItemReservations.Rows.Count - 1; rowIndex >= 0; rowIndex--)
                {
                    if (dtItemReservations.Rows[rowIndex].RowState != DataRowState.Deleted)
                    {
                        if (Convert.ToBoolean(dtItemReservations.Rows[rowIndex]["Secure"]))
                        { dtItemReservations.Rows[rowIndex].Delete(); }
                    }
                }
            }

            
            reservationDropDown.DataTextField = "Type";
            reservationDropDown.DataValueField = "Val";
            reservationDropDown.DataSource = dtItemReservations;
            reservationDropDown.DataBind();
            RadComboBoxItem someitem = reservationDropDown.FindItemByValue((UseTq ? _itemTestQuestion.Reservation : _itemBankQuestion.Reservation));
            if (someitem != null)
            {
                someitem.Selected = true;
            }
        }

        /// <summary>
        /// Load item status
        /// </summary>
        private void LoadItemStatus()
        {
            DataTable dtItemStatus = QuestionBase.ItemStatusSelect();
            itemstatusDropDown.DataTextField = "Status";
            itemstatusDropDown.DataValueField = "Val";
            itemstatusDropDown.DataSource = dtItemStatus;
            itemstatusDropDown.DataBind();
            RadComboBoxItem someitem =
                itemstatusDropDown.FindItemByValue(
                    (UseTq
                        ? _itemTestQuestion.ReviewStatus.ToString(CultureInfo.CurrentCulture)
                        : _itemBankQuestion.ReviewStatus.ToString(CultureInfo.CurrentCulture)));

            if (someitem != null)
            {
                someitem.Selected = true;
            }
        }

        /// <summary>
        /// Load item type
        /// </summary>
        private void LoadItemType()
        {
            if (!IsPostBack)
            {
                RadComboBoxItem newitem = new RadComboBoxItem();
                newitem.Text = "Multiple Choice (3 Distractors)";
                newitem.Value = QuestionTypes.MC3.ToString();
                if (newitem.Value == (UseTq ? _itemTestQuestion.QuestionType.ToString() : _itemBankQuestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "Multiple Choice (4 Distractors)";
                newitem.Value = QuestionTypes.MC4.ToString();
                if (newitem.Value == (UseTq ? _itemTestQuestion.QuestionType.ToString() : _itemBankQuestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "Multiple Choice (5 Distractors)";
                newitem.Value = QuestionTypes.MC5.ToString();
                if (newitem.Value == (UseTq ? _itemTestQuestion.QuestionType.ToString() : _itemBankQuestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                if (DistrictParms.LoadDistrictParms().DisplayTrueFalse)
                {
                    newitem = new RadComboBoxItem();
                    newitem.Text = "True/False";
                    newitem.Value = QuestionTypes.T.ToString();
                    if (newitem.Value == (UseTq ? _itemTestQuestion.QuestionType.ToString() : _itemBankQuestion.QuestionType.ToString()))
                        newitem.Selected = true;
                    cmbItemType.Items.Add(newitem);
                }

                newitem = new RadComboBoxItem();
                newitem.Text = "Short Answer";
                newitem.Value = QuestionTypes.S.ToString();
                if (newitem.Value == (UseTq ? _itemTestQuestion.QuestionType.ToString() : _itemBankQuestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "Essay";
                newitem.Value = QuestionTypes.E.ToString();
                if (newitem.Value == (UseTq ? _itemTestQuestion.QuestionType.ToString() : _itemBankQuestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);
            }

            //Since the following table's appearance is based solely on the QuestionType, 
            //We'll determine whether to display this table or not here.

            contentEditor_Item_tblAdvanced.Style["display"] = ((cmbItemType.SelectedValue).Contains("M") || (cmbItemType.SelectedValue).Contains("T")) ? "block" : "none";

            //Since the following div's appearance is based solely on the QuestionType, 
            //We'll determine whether to display this div or not here.
            divRubricTypeLabel.Style["display"] = ((cmbItemType.SelectedValue).Contains("S") || (cmbItemType.SelectedValue).Contains("E")) && (cmbScoreType.SelectedValue.Contains("R")) ? "inline" : "none";


            //Since the value of this dropdownlist effects cmbScoreType,
            //refresh that control as well.
            LoadScoreType();
        }

        /// <summary>
        /// Load score type
        /// </summary>
        private void LoadScoreType()
        {
            cmbScoreType.Items.Clear();

            RadComboBoxItem newitem = new RadComboBoxItem();
            newitem.Text = "Correct/Incorrect";
            newitem.Value = "W";
            newitem.Attributes["ItemTypes"] = "TMC3MC4MC5SE";
            cmbScoreType.Items.Add(newitem);

            newitem = new RadComboBoxItem();
            newitem.Text = "Partial Credit";
            newitem.Value = "P";
            newitem.Attributes["ItemTypes"] = "SE";

            cmbScoreType.Items.Add(newitem);

            newitem = new RadComboBoxItem();
            newitem.Text = "Rubric";
            newitem.Value = "R";
            newitem.Attributes["ItemTypes"] = "SE";
            cmbScoreType.Items.Add(newitem);

            //Decide which items should be visible based on
            //cmbItemType and which one is selected.
            foreach (RadComboBoxItem scoreTypeItem in cmbScoreType.Items)
            {
                scoreTypeItem.Visible = scoreTypeItem.Attributes["ItemTypes"].ToString(CultureInfo.CurrentCulture).Contains(cmbItemType.SelectedValue);

                //******* 2012-12-03 DHB Start code changes.
                // Prior to now, I thought the BankQuestion side of items held the descriptions in the scoreType property,
                // but it is actually the "value" that is held instead - so no need to differentiate between the two.
                if (!IsPostBack)
                    scoreTypeItem.Selected = (scoreTypeItem.Value == (UseTq ? _itemTestQuestion.ScoreType : _itemBankQuestion.ScoreType));
                else
                    scoreTypeItem.Selected = false;
                //******* 2012-12-03 DHB Stop code changes.
            }

            //Since the value of this dropdownlist effects cmbRubricType,
            //refresh that control as well.
            LoadRubricTypes();
        }

        /// <summary>
        /// Load rubric types
        /// </summary>
        private void LoadRubricTypes()
        {
            DataTable dtRubricTypes = Rubric.RubricTypesSelect();
            cmbRubricType.DataTextField = "Rubrictype";
            cmbRubricType.DataValueField = "Rubricval";
            cmbRubricType.DataSource = dtRubricTypes;
            cmbRubricType.DataBind();
            RadComboBoxItem someitem;

            if (cmbScoreType.SelectedValue == "R")
            {
                divRubricTypeLabel.Style["display"] = "inline";

                if (UseTq)
                    someitem = cmbRubricType.FindItemByValue((_itemTestQuestion.RubricType));
                else
                    someitem = cmbRubricType.FindItemByValue((_itemBankQuestion.RubricType));

                if (someitem != null)
                    someitem.Selected = true;
            }
            else
            {
                //rubric types control should be hidden now, but set it up
                //to default value so if the user's actions cause it to
                //appear, then it will show the default value
                divRubricTypeLabel.Style["display"] = "none";
                someitem = cmbRubricType.FindItemByValue("B");
                if (someitem != null) someitem.Selected = true;
            }

            //Since the value of this dropdownlist effects cmbRubricScoring,
            //refresh that control as well.
            LoadRubricScoreTypes();
        }

        /// <summary>
        /// Load rubric score types
        /// </summary>
        private void LoadRubricScoreTypes()
        {
            DataTable dtRubricTypes = Rubric.RubricScoreTypesSelect();
            cmbRubricScoring.DataTextField = "RubricScoreText";
            cmbRubricScoring.DataValueField = "RubricPoints";
            cmbRubricScoring.DataSource = dtRubricTypes;
            cmbRubricScoring.DataBind();

            //Add "Custom" option to the dropdownlist
            RadComboBoxItem newItem = new RadComboBoxItem();
            newItem = new RadComboBoxItem();
            newItem.Text = "Custom";
            newItem.Value = "0";
            cmbRubricScoring.Items.Add(newItem);


            //Decide whether cmbRubricScore should be visible
            if (divRubricTypeLabel.Style["display"] == "inline" && cmbRubricType.SelectedValue == "B")
            {
                divRubricScoringLabel.Style["display"] = "inline";

                //if the item's rubric is currently null, then display a default 2-pt rubric
                if (UseTq)
                    newItem = cmbRubricScoring.FindItemByValue(_itemTestQuestion.RubricPoints.ToString(CultureInfo.CurrentCulture));
                else
                    newItem = cmbRubricScoring.FindItemByValue(_itemBankQuestion.RubricPoints.ToString(CultureInfo.CurrentCulture));
                if (newItem != null)
                    newItem.Selected = true;
            }
            else
            {
                //rubric Scoring control should be hidden now, but set it up
                //to default value so if the user's actions cause it to
                //appear, then it will show the default value
                divRubricScoringLabel.Style["display"] = "none";
                newItem = cmbRubricScoring.FindItemByValue("2");
                if (newItem != null) newItem.Selected = true;
            }

            //Since the value of this dropdownlist effects pnlItem_Rubric,
            //refresh that control as well.
            LoadPnlItem_Rubric();
        }

        /// <summary>
        /// Load the rubrics
        /// </summary>
        private void LoadPnlItem_Rubric()
        {
            //Decide whether to display the rubric panel or not
            //should display if rubric present, 

            if (divRubricTypeLabel.Style["display"] == "none")
            {
                //Item's scoretype is not rubric, so hide the rubric panel
                pnlItem_Rubric.Style["display"] = "none";

                //custom rubric panel should be hidden now, but set it up
                //to default value so if the user's actions cause it to
                //appear, then it will show the default value
                trRubric_No.Style.Add("display", "inline");
                trRubric_Yes.Style.Add("display", "none");
            }
            else if (

                //The following two conditions represent custom rubrics.
                (divRubricScoringLabel.Style["display"] == "inline" && cmbRubricScoring.SelectedValue == "0")
                ||
                (divRubricTypeLabel.Style["display"] == "inline" && divRubricScoringLabel.Style["display"] == "none")
                )
            {
                //Item's score type is rubric and rubric is custom, so display rubric panel.
                pnlItem_Rubric.Style["display"] = "block";


                //Decide what within the Rubric Panel should show.    
                if ((UseTq ? _itemTestQuestion.RubricID : _itemBankQuestion.RubricID) == 0)
                {
                    //User has not imported a rubric yet, so display the rubric panel to allow user to import a rubric from the rubric search screen.
                    trRubric_No.Style.Add("display", "inline");
                    trRubric_Yes.Style.Add("display", "none");
                }
                else
                {
                    //User has already imported a rubric so display the rubric panel to allow user to delete the imported rubric so they can import another.
                    if (UseTq)
                    {
                        lblRubricName.Text = _itemTestQuestion.Rubric.Name;
                        lblRubricScoring.Text = _itemTestQuestion.Rubric.Scoring;
                        lblRubricType.Text = _itemTestQuestion.Rubric.TypeDesc;
                        if (_itemTestQuestion.RubricType == "A")
                        {
                            lblRubricType.Text += " " + _itemTestQuestion.Rubric.CriteriaCount.ToString(CultureInfo.CurrentCulture) + " x " + _itemTestQuestion.Rubric.MaxPoints.ToString(CultureInfo.CurrentCulture);
                            ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "none";
                        }
                        else
                        {
                            ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "inline";
                        }
                        ContentEditor_Item_hdnRubricContent.InnerHtml = _itemTestQuestion.Rubric.Content;
                    }
                    else
                    {
                        lblRubricName.Text = _itemBankQuestion.Rubric.Name;
                        lblRubricScoring.Text = _itemBankQuestion.Rubric.Scoring;
                        lblRubricType.Text = _itemBankQuestion.Rubric.TypeDesc;
                        if (_itemBankQuestion.RubricType == "A")
                        {
                            lblRubricType.Text += " " + _itemBankQuestion.Rubric.CriteriaCount.ToString(CultureInfo.CurrentCulture) + " x " + _itemBankQuestion.Rubric.MaxPoints.ToString(CultureInfo.CurrentCulture) + " Points";
                            ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "none";
                        }
                        else
                        {
                            ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "inline";
                        }
                        ContentEditor_Item_hdnRubricContent.InnerHtml = _itemBankQuestion.Rubric.Content;
                        ContentEditor_Item_hdnRubricID.Value = _itemBankQuestion.RubricID.ToString(CultureInfo.CurrentCulture);
                    }
                    ContentEditor_Item_imgRemoveRubric.Style["display"] = "inline";
                    trRubric_Yes.Style.Add("display", "inline");
                    trRubric_No.Style.Add("display", "none");
                }
            }

            else
            {
                pnlItem_Rubric.Style["display"] = "block";

                //Item's rubric is a default rubric.  Only show the rubric info and a hyperlink in the rubric 
                //panel - no upload, add new, or remove functionality because item does not specify a custom 
                //rubric.
                lblRubricName.Text = (UseTq ? _itemTestQuestion.Rubric.Name : _itemBankQuestion.Rubric.Name);
                lblRubricScoring.Text = (UseTq ? _itemTestQuestion.Rubric.Scoring : _itemBankQuestion.Rubric.Scoring);
                lblRubricType.Text = (UseTq ? _itemTestQuestion.Rubric.TypeDesc : _itemBankQuestion.Rubric.TypeDesc);

                ContentEditor_Item_imgRemoveRubric.Style["display"] = "none";
                ContentEditor_Item_hdnRubricID.Value = (UseTq ? -_itemTestQuestion.RubricPoints : -_itemBankQuestion.RubricPoints).ToString(CultureInfo.CurrentCulture);                

                trRubric_Yes.Style.Add("display", "block");
                trRubric_No.Style.Add("display", "none");
            }
        }
        #endregion Private Methods
    }
}
