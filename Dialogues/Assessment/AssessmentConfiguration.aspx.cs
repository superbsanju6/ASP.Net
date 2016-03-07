using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System.Web.UI;
using Thinkgate.Base.DataAccess;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;
using Thinkgate.Utilities;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentConfiguration : System.Web.UI.Page
    {
        protected SessionObject SessionObject;
        protected int classID;
        protected string subject;
        protected int courseID;
        protected string courseName;
        protected string grade;
        protected string type;
        protected int term;
        protected string content;
        protected string description;
        protected int currUserID;
        protected int standardID_URLParm;
        protected string standardName_URLParm;
        protected int itemCountInput_URLParm;
        protected string testCategory;
        protected StandardRigorLevels rigorLevels;
        protected int _assessmentID;
        protected String cacheKey;
        protected Base.Classes.Assessment _assessment;
        private DataSet _ds;
        protected Thinkgate.Base.Classes.AssessmentInfo _assessmentInfo;
        DistrictParms DParmsNew;
        protected const string ContenType = "No Items/Content";

        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(this.type);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }
        

        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];

            if (String.IsNullOrEmpty(Request.QueryString["xID"]))
            {
                classID = DataIntegrity.ConvertToInt(SessionObject.TeacherTileParms.GetParm("levelID"));
                subject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : "";
                courseID = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]) : 0;
                Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(courseID);
                courseName = assessmentCourse != null ? assessmentCourse.CourseName : "";
                grade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : "";
                type = SessionObject.AssessmentBuildParms.ContainsKey("Type") ? SessionObject.AssessmentBuildParms["Type"] : "";
                term = SessionObject.AssessmentBuildParms.ContainsKey("Term") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Term"]) : 0;
                content = SessionObject.AssessmentBuildParms.ContainsKey("Content") ? SessionObject.AssessmentBuildParms["Content"] : "";
                description = SessionObject.AssessmentBuildParms.ContainsKey("Description") ? SessionObject.AssessmentBuildParms["Description"] : "";
                currUserID = SessionObject.LoggedInUser != null ? (SessionObject.LoggedInUser.Page > 0 ? SessionObject.LoggedInUser.Page : AppSettings.Demo_TeacherID) : AppSettings.Demo_TeacherID;
                testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? SessionObject.AssessmentBuildParms["TestCategory"] : "Classroom";
                Page.Title = "Assessment Configuration";
                if (SecureType)
                {
                    Page.Title = "[SECURE] Assessment Configuration";
                }
            }
            else
            {
                if (_assessment == null)
                {
                    LoadAssessment();
                    LoadAssessmentInfo();
                }

                testCategory = _assessmentInfo.Category;
                subject = _assessment.Subject;
                courseName = _assessment.Course;
                grade = _assessment.Grade;
                type = _assessment.TestType;
                term = DataIntegrity.ConvertToInt(_assessment.Term);
                content = _assessment.ContentType;
                description = _assessment.Description;
                rcbAllowedOTCNavigation.SelectedValue = _assessmentInfo.IsOTCNavigationRestricted;
                currUserID = SessionObject.LoggedInUser != null ? (SessionObject.LoggedInUser.Page > 0 ? SessionObject.LoggedInUser.Page : AppSettings.Demo_TeacherID) : AppSettings.Demo_TeacherID;
                //Page.Title = "Assessment Configuration";
                //if (SecureType)
                //{
                //    Page.Title = "[SECURE] Assessment Configuration";
                //}
                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                Dictionary<string, bool> dictionaryItem;
                dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory);
                bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                hiddenAccessSecureTesting.Value = hasPermission.ToString();
                hiddenIsSecuredFlag.Value = isSecuredFlag.ToString();
                hiddenSecureType.Value = SecureType.ToString();
                ScriptManager.RegisterStartupScript(this, typeof(string), "Title", "setConfigTitle();", true);

                _ds = ThinkgateDataAccess.FetchDataSet("E3_Assessment_GetByID", new object[] { _assessmentID, currUserID });
                _ds.Tables[0].TableName = "Summary";
                _ds.Tables[1].TableName = "StandardQuestionCounts";
                _ds.Tables[2].TableName = "RigorCounts";
            }

            if (ContenType == content)
            {
                fieldsetAllowedOTCNavigation.Visible = false;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DParmsNew = DistrictParms.LoadDistrictParms();
            if (!IsPostBack)
            {                
                SetAssessmentType();
                LoadTimedAssessmentData();
                switch (Request.QueryString["headerImg"])
                {
                    case "lightningbolt":
                        headerImg.Src = "../../Images/lightningbolt.png";
                        headerImg.Attributes["headerImgName"] = "lightningbolt";
                        break;
                    case "magicwand":
                        headerImg.Src = "../../Images/magicwand.png";
                        headerImg.Attributes["headerImgName"] = "magicwand";
                        break;
                    default:
                        headerImg.Visible = false;
                        break;
                }

                assessmentTitle.InnerHtml = "Term " + term.ToString() + " " + type + " - " + grade + " Grade " + subject +
                                            " " + (courseName == string.Empty ? string.Empty : courseName) + " - " + description;

                if (_assessment == null)
                {
                    //Code for Wizard goes here
                }
                else
                {
                    generateButton.Text = "  Update  ";
                    backButton.Visible = false;
                    cancelButton.OnClientClick = "cancelButtonClick(); return false;";

                    //Content Type radio check
                    switch (_assessmentInfo.ContentType)
                    {
                        case "External":
                            externalRadio.Checked = true;
                            includeFieldTestCell.Attributes["style"] = "display:none;";
                            onlineContentFormatCell.Attributes["style"] = string.Empty;
                            itemBankCell.Attributes["style"] = "display:none;";
                            break;
                        default:
                            itemBankRadio.Checked = true;
                            includeFieldTestCell.Attributes["style"] = string.Empty;
                            onlineContentFormatCell.Attributes["style"] = "display:none";
                            itemBankCell.Attributes["style"] = string.Empty;
                            break;
                    }

                    //Include Field Test radio check
                    if (_assessmentInfo.AllowFieldTest && _assessmentInfo.Category != "Classroom")
                    {
                        switch (_assessmentInfo.IncludeFieldTest)
                        {
                            case true:
                                includeFieldTestYes.Checked = true;
                                includeFieldTestNo.Disabled = true;
                                break;
                            default:
                                includeFieldTestNo.Checked = true;
                                break;
                        }
                    }
                    else
                    {
                        includeFieldTestNo.Checked = true;
                        includeFieldTestNo.Disabled = true;
                        includeFieldTestYes.Disabled = true;
                    }

                    //Online Content Format radio check
                    switch (_assessmentInfo.OnlineContentFormat)
                    {
                        case "Test and Response Card":
                            onlineContentFormatBoth.Checked = true;
                            break;
                        default:
                            onlineContentFormatResponseOnly.Checked = true;
                            break;
                    }

                    LoadNumberOfFormsDropdown();
                    LoadDistractorLabels();
                    if (!String.IsNullOrEmpty(_assessmentInfo.Keywords)) keywords.Text = _assessmentInfo.Keywords;
                    LoadPerformanceLevels();
                    LoadItemBanks();
                    sourceInput.Value = _assessmentInfo.Source;
                    creditInput.Value = _assessmentInfo.Credit;
                    schoolDistrictName.InnerHtml = _assessmentInfo.Client;
                    authorName.InnerHtml = _assessmentInfo.Author;
                    assessmentItemCount.Value = _assessment.Items.Count.ToString();
                    hiddenUserID.Value = currUserID.ToString();
                    assessmentID.Value = _assessmentID.ToString();
                    encryptedAssessmentID.Value = Request.QueryString["xID"];
                    encryptedTeacherID.Value = Request.QueryString["yID"];

                    //Short Answer Section on Bubble Sheet radio check
                    switch (_assessmentInfo.DisplayShortAnswer)
                    {
                        case "Yes":
                            shortAnswerBubbleSheetYes.Checked = true;
                            break;
                        default:
                            shortAnswerBubbleSheetNo.Checked = true;
                            break;
                    }

                    //Number of Columns radio check
                    switch (_assessmentInfo.PrintColumns)
                    {
                        case 2:
                            numberOfColumns2.Checked = true;
                            break;
                        default:
                            numberOfColumns1.Checked = true;
                            break;
                    }

                    //Include Cover Page radio check
                    switch (_assessmentInfo.IncludeCoverPage)
                    {
                        case "Yes":
                            includeCoverPageYes.Checked = true;
                            break;
                        default:
                            includeCoverPageNo.Checked = true;
                            break;
                    }

                    if (_assessment.IsProofed)
                    {
                        itemBankRadio.Disabled = true;
                        externalRadio.Disabled = true;
                        includeFieldTestYes.Disabled = true;
                        includeFieldTestNo.Disabled = true;
                        onlineContentFormatBoth.Disabled = true;
                        onlineContentFormatResponseOnly.Disabled = true;
                        numberOfForms.Enabled = false;
                        distractorLabels.Enabled = false;
                        keywords.Enabled = false;
                        scoreType.Enabled = false;
                        performanceLevelSet.Enabled = false;
                        assessmentIdentificationButton.Disabled = true;
                        assessmentIdentificationButton.Style.Add("Color", "lightgray");
                        assessmentIdentificationButton.Attributes["onclick"] = "return false;";
                        sourceInput.Disabled = true;
                        creditInput.Disabled = true;
                        shortAnswerBubbleSheetYes.Disabled = true;
                        shortAnswerBubbleSheetNo.Disabled = true;
                        includeCoverPageYes.Disabled = true;
                        includeCoverPageNo.Disabled = true;
                        rcbAllowedOTCNavigation.Enabled = false;
                    }
                }

                if (!SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Access_CoverPage_AssessmentConfiguration))
                {
                    coverPageGroup.Attributes["style"] = "display:none;";
                }


            }

            if (!SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Access_AssessmentToolConfiguration))
            {
                RadTab onlineToolsTab = RadTabStrip1.FindTabByText("Online Tools");
                onlineToolsTab.Visible = false;
            }

            switch (Request.Form["__EVENTTARGET"])
            {
                case "RadButtonOk":
                    UpdateTestToolClick(this, new EventArgs());
                    break;

            }
        }

        private void SetAssessmentType()
        {
            if (_assessmentInfo != null)
            {
                hdnAssessmentType.Value = _assessmentInfo.Category.ToUpper();
                hdnIsAssessmentProofed.Value = _assessment.IsProofed.ToString();
            }


        }


        protected void UpdateTestToolClick(object sender, EventArgs e)
        {
            Save_TestTools();
        }


        protected void Save_TestTools()
        {
            try
            {
                DataTable table = Base.Classes.Assessment.GetTestTool(_assessmentID);

                foreach (GridDataItem item in gridTestTool.Items)
                {
                    int id = Convert.ToInt32(item["ID"].Text);

                    CheckBox chk = item.FindControl("CheckBox2") as CheckBox;
                    bool status = chk.Checked;
                    bool isAdded = false;
                    bool needRemove = false;

                    if (chk.Checked)
                    {
                        foreach (DataRow row in table.Rows)
                        {

                            // If test tool is already added to the assessment
                            if (Convert.ToInt32(row["OnlineToolID"].ToString()) == id)
                                isAdded = true;
                        }
                        if (!isAdded)
                            Base.Classes.Assessment.AddTestTool(id, _assessmentID);
                    }
                    else
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            // If test tool is already added to the assessment
                            if (Convert.ToInt32(row["OnlineToolID"].ToString()) == id)
                                needRemove = true;
                        }

                        if (needRemove)
                            Base.Classes.Assessment.RemoveTestTool(id, _assessmentID);
                    }

                }

                if (_assessment.IsProofed == false)
                {
                    SaveUpdateTimedConfiguration();
                }
                UpdateConfigurationAllowedOTCNavigation();
                string js = "parent.window.location.reload();";
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "AssessmentConfigurationRefresh", js, true);
            }
            catch (ApplicationException e)
            {

            }
        }

        private void SaveUpdateTimedConfiguration()
        {
            var isTimedAssessment = rcbTimedAssessment.SelectedValue != "0";
            var timedAssessment = new TimedAssessmentInfo();
            timedAssessment.TestId = _assessmentID;
            timedAssessment.IsTimedAssessment = isTimedAssessment;
            if (!string.IsNullOrEmpty(rtbTimeAllotted.Text))
                timedAssessment.TimeAllotted = int.Parse(rtbTimeAllotted.Text);
            else
                timedAssessment.TimeAllotted = 0;
            timedAssessment.IsTimeExtension = rcbTimeExtension.SelectedValue != "0";
            timedAssessment.IsAutoSubmit = rcbAutoSubmit.SelectedValue != "0";
            timedAssessment.EnforceTimedAssessment = chkTimedAssessment.Checked;
            timedAssessment.EnforceTimeAllotted = true;
            timedAssessment.EnforceTimeExtension = chkTimeExtension.Checked;
            timedAssessment.EnforceTimeWarningInterval = chkTimeWarningInterval.Checked;
            timedAssessment.EnforceAutoSubmit = chkHasAutoSubmit.Checked;
            Base.Classes.Assessment.SaveUpdateTimedAssessment(timedAssessment);
        }

        private void LoadAssessment()
        {
            if (Request.QueryString["xID"] == null ||
                    (_assessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey)) <= 0)
            {
                SessionObject.RedirectMessage = "No assessment ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                cacheKey = "Assessment_" + Request.QueryString["xID"];
                /*if (Base.Classes.Cache.Get(cacheKey) == null)
                {*/
                _assessment = Base.Classes.Assessment.GetAssessmentAndQuestionsByID(_assessmentID);
                if (_assessment == null)
                //Base.Classes.Cache.Insert(cacheKey, _assessment);
                //else
                {
                    SessionObject.RedirectMessage = "Could not find the assessment.";
                    Response.Redirect("~/PortalSelection.aspx", true);
                }
                /*}
                else
                    _assessment = (Base.Classes.Assessment)Cache[cacheKey];
                     */
            }
        }

        private void LoadAssessmentInfo()
        {
            if (Request.QueryString["xID"] == null ||
                    (_assessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey)) <= 0)
            {
                SessionObject.RedirectMessage = "No assessment ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                cacheKey = "AssessmentInfo_" + _assessmentID;

                /*if (Base.Classes.Cache.Get(cacheKey) == null)
                {*/
                _assessmentInfo = Thinkgate.Base.Classes.Assessment.GetConfigurationInformation(_assessmentID, currUserID);
                if (_assessmentInfo == null)
                //      Thinkgate.Base.Classes.Cache.Insert(cacheKey, _assessmentInfo);
                //else
                {
                    SessionObject.RedirectMessage = "Could not find the assessment.";
                    Response.Redirect("~/PortalSelection.aspx", true);
                }
                /*}
                else
                    _assessmentInfo = (Base.Classes.AssessmentInfo)Cache[cacheKey];*/
            }
        }

        private void LoadNumberOfFormsDropdown()
        {
            for (var i = 1; i < 100; i++)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Value = i.ToString();
                item.Text = i.ToString();

                if (i == _assessmentInfo.NumForms) item.Selected = true;

                numberOfForms.Items.Add(item);
            }

            if (!_assessmentInfo.AllowMultiForms || _assessmentInfo.ContentType == "External")
            {
                numberOfForms.SelectedIndex = 0;
                numberOfForms.Enabled = false;
            }
        }

        private void LoadDistractorLabels()
        {
            DataTable distractorLabelsTable = Base.Classes.Assessment.GetDistractorLabels(_assessmentInfo.NumDistractors > 0 ? _assessmentInfo.NumDistractors : 5, currUserID);

            foreach (DataRow row in distractorLabelsTable.Rows)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = row["DistractorLabel"].ToString();
                item.Value = row["Value"].ToString();
                if (row["Value"].ToString() == _assessmentInfo.DistractorLabels)
                {
                    item.Selected = true;
                }

                distractorLabels.Items.Add(item);
            }
        }

        private void LoadPerformanceLevels()
        {
            DataTable performanceLevelsTable = Base.Classes.Assessment.GetPerformanceLevels(_assessmentID, currUserID);

            foreach (DataRow row in performanceLevelsTable.Rows)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = row["PerformanceLevel"].ToString();
                item.Value = row["PerformanceLevel"].ToString();
                if (row["PerformanceLevel"].ToString() == _assessmentInfo.PerformanceLevels)
                {
                    item.Selected = true;
                }

                performanceLevelSet.Items.Add(item);
            }
        }

        private void LoadItemBanks()
        {
            int count = 0;
            bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            Dictionary<string, bool> dictionaryItem;
            dictionaryItem = TestTypes.TypeWithSecureFlag(testCategory);
            bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            if (hasPermission && isSecuredFlag && SecureType)
            testCategory = type;
          
            DataTable itemBanks = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory).DistinctByLabel();
            TableRow tableRow = new TableRow();

            foreach (DataRow row in itemBanks.Rows)
            {
                if (count == 0 || count % 4 == 0)
                    tableRow = new TableRow();

                TableCell tableCell = new TableCell();
                tableCell.Attributes["style"] = "white-space:nowrap; border:solid 0px #000;";

                CheckBox checkBox = new CheckBox();
                checkBox.ID = "itemBank" + count.ToString();
                checkBox.ClientIDMode = ClientIDMode.Static;
                checkBox.Text = row["Label"].ToString();
                checkBox.InputAttributes["value"] = row["Label"].ToString();
                checkBox.InputAttributes["class"] = "inputCheckboxes";

                if (SessionObject.ItemBankLabels.ContainsKey(_assessmentID))
                {
                    if (SessionObject.ItemBankLabels[_assessmentID].IndexOf(row["Label"].ToString()) > -1)
                        checkBox.Checked = true;
                }
                else
                {
                    checkBox.Checked = true;
                }
                if (hasPermission && isSecuredFlag)
                {
                    if (SecureType)
                    {
                        checkBox.Enabled = false;
                    }
                }

                if (_assessment.IsProofed)
                    checkBox.Enabled = false;

                tableCell.Controls.Add(checkBox);
                tableRow.Cells.Add(tableCell);
                itemBankTable.Rows.Add(tableRow);

                count++;
            }
        }

        private void UpdateConfigurationAllowedOTCNavigation()
        {
            _assessmentInfo.ContentType = itemBankRadio.Checked ? "Item Bank" : "External";
            _assessmentInfo.IncludeFieldTest = includeFieldTestYes.Checked;
            _assessmentInfo.OnlineContentFormat = itemBankRadio.Checked ? "Single Question" : onlineContentFormatBoth.Checked ? "Test and Response Card" : "Response Card Only";
            _assessmentInfo.NumForms = DataIntegrity.ConvertToInt(numberOfForms.SelectedValue);
            _assessmentInfo.DistractorLabels = distractorLabels.SelectedValue;
            _assessmentInfo.Keywords = keywords.Text;
            _assessmentInfo.ScoreType = scoreType.SelectedValue;
            _assessmentInfo.PerformanceLevels = performanceLevelSet.SelectedValue;
            _assessmentInfo.Source = sourceInput.Value;
            _assessmentInfo.Credit = creditInput.Value;
            _assessmentInfo.PrintColumns = numberOfColumns1.Checked ? 1 : 2;
            _assessmentInfo.PrintShortAnswer = shortAnswerBubbleSheetYes.Checked;
            _assessmentInfo.IncludeCoverPage = includeCoverPageYes.Checked ? "Yes" : "No";
            _assessmentInfo.IsOTCNavigationRestricted = rcbAllowedOTCNavigation.SelectedValue == "1" ? "1" : "0";
            Base.Classes.Assessment.SaveConfigurationInformation(_assessmentInfo, _assessmentID, currUserID);
        }

        protected void UpdateConfigurationContentTypeChange_Click(object sender, EventArgs e)
        {

            _assessmentInfo.ContentType = itemBankRadio.Checked ? "Item Bank" : "External";
            _assessmentInfo.IncludeFieldTest = includeFieldTestYes.Checked;
            _assessmentInfo.OnlineContentFormat = itemBankRadio.Checked ? "Single Question" : "Test and Response Card";
            _assessmentInfo.NumForms = DataIntegrity.ConvertToInt(numberOfForms.SelectedValue);
            _assessmentInfo.DistractorLabels = distractorLabels.SelectedValue;
            _assessmentInfo.Keywords = keywords.Text;
            _assessmentInfo.ScoreType = scoreType.SelectedValue;
            _assessmentInfo.PerformanceLevels = performanceLevelSet.SelectedValue;
            _assessmentInfo.Source = sourceInput.Value;
            _assessmentInfo.Credit = creditInput.Value;
            _assessmentInfo.PrintColumns = numberOfColumns1.Checked ? 1 : 2;
            _assessmentInfo.PrintShortAnswer = shortAnswerBubbleSheetYes.Checked;
            _assessmentInfo.IncludeCoverPage = includeCoverPageYes.Checked ? "Yes" : "No";

            _assessment.Items.Clear();
            assessmentItemCount.Value = "0";

            //Content Type radio check
            switch (_assessmentInfo.ContentType)
            {
                case "External":
                    externalRadio.Checked = true;
                    onlineContentFormatBoth.Checked = true;
                    onlineContentFormatResponseOnly.Checked = false;
                    includeFieldTestCell.Attributes["style"] = "display:none;";
                    onlineContentFormatCell.Attributes["style"] = string.Empty;
                    itemBankCell.Attributes["style"] = "display:none;";
                    break;
                default:
                    itemBankRadio.Checked = true;
                    includeFieldTestCell.Attributes["style"] = string.Empty;
                    onlineContentFormatCell.Attributes["style"] = "display:none";
                    itemBankCell.Attributes["style"] = string.Empty;
                    break;
            }

            Base.Classes.Assessment.SaveConfigurationInformation(_assessmentInfo, _assessmentID, currUserID, "All");
            LoadItemBanks();
        }

        protected void UpdateConfigurationFieldTestChange_Click(object sender, EventArgs e)
        {
            _assessmentInfo.ContentType = itemBankRadio.Checked ? "Item Bank" : "External";
            _assessmentInfo.IncludeFieldTest = includeFieldTestYes.Checked;
            _assessmentInfo.OnlineContentFormat = itemBankRadio.Checked ? "Single Question" : onlineContentFormatBoth.Checked ? "Test and Response Card" : "Response Card Only";
            _assessmentInfo.NumForms = DataIntegrity.ConvertToInt(numberOfForms.SelectedValue);
            _assessmentInfo.DistractorLabels = distractorLabels.SelectedValue;
            _assessmentInfo.Keywords = keywords.Text;
            _assessmentInfo.ScoreType = scoreType.SelectedValue;
            _assessmentInfo.PerformanceLevels = performanceLevelSet.SelectedValue;
            _assessmentInfo.Source = sourceInput.Value;
            _assessmentInfo.Credit = creditInput.Value;
            _assessmentInfo.PrintColumns = numberOfColumns1.Checked ? 1 : 2;
            _assessmentInfo.PrintShortAnswer = shortAnswerBubbleSheetYes.Checked;
            _assessmentInfo.IncludeCoverPage = includeCoverPageYes.Checked ? "Yes" : "No";

            //Include Field Test radio check
            if (_assessmentInfo.AllowFieldTest && _assessmentInfo.Category != "Classroom")
            {
                switch (_assessmentInfo.IncludeFieldTest)
                {
                    case true:
                        includeFieldTestYes.Checked = true;
                        break;
                    default:
                        includeFieldTestNo.Checked = true;
                        break;
                }
            }
            else
            {
                includeFieldTestNo.Checked = true;
                includeFieldTestNo.Disabled = true;
                includeFieldTestYes.Disabled = true;
            }

            Base.Classes.Assessment.SaveConfigurationInformation(_assessmentInfo, _assessmentID, currUserID, "FieldTest");

            Base.Classes.Cache.Remove(cacheKey);
            LoadAssessment();
            LoadItemBanks();
            assessmentItemCount.Value = _assessment.Items.Count.ToString();
        }

        private void LoadTimedAssessmentData()
        {
            var timedAssessmentInfo = Base.Classes.Assessment.GetTimedAssessmentById(_assessmentID);
            if (timedAssessmentInfo != null)
            {
                rcbTimedAssessment.SelectedValue = timedAssessmentInfo.IsTimedAssessment ? "1" : "0";
                if (timedAssessmentInfo.TimeAllotted.HasValue)
                    rtbTimeAllotted.Text = timedAssessmentInfo.TimeAllotted.Value.ToString();
                rcbTimeExtension.SelectedValue = timedAssessmentInfo.IsTimeExtension ? "1" : "0";
                hdnTimeIntervals.Value = timedAssessmentInfo.TimeWarningInterval;
                rcbAutoSubmit.SelectedValue = timedAssessmentInfo.IsAutoSubmit ? "1" : "0";
                chkTimedAssessment.Checked = timedAssessmentInfo.EnforceTimedAssessment;
                chkTimeAllotted.Checked = timedAssessmentInfo.EnforceTimeAllotted;
                chkTimeExtension.Checked = timedAssessmentInfo.EnforceTimeExtension;
                chkTimeWarningInterval.Checked = timedAssessmentInfo.EnforceTimeWarningInterval;
                chkHasAutoSubmit.Checked = timedAssessmentInfo.EnforceAutoSubmit;
            }
        }

        [System.Web.Services.WebMethod]
        public static string AddTimeWarningInterval(int testId, string timeIntervalVal)
        {
            Base.Classes.Assessment.AddTimeWarningInterval(testId, timeIntervalVal);
            return "";
        }

        [System.Web.Services.WebMethod]
        public static string DeleteTimeWarningInterval(int testId, string timeIntervalVal)
        {
            Base.Classes.Assessment.DeleteTimeWarningInterval(testId, timeIntervalVal);
            return "";
        }

        protected void UpdateConfiguration_Click(object sender, EventArgs e)
        {
            _assessmentInfo.ContentType = itemBankRadio.Checked ? "Item Bank" : "External";
            _assessmentInfo.IncludeFieldTest = includeFieldTestYes.Checked;
            _assessmentInfo.OnlineContentFormat = itemBankRadio.Checked ? "Single Question" : onlineContentFormatBoth.Checked ? "Test and Response Card" : "Response Card Only";
            _assessmentInfo.NumForms = DataIntegrity.ConvertToInt(numberOfForms.SelectedValue);
            _assessmentInfo.DistractorLabels = distractorLabels.SelectedValue;
            _assessmentInfo.Keywords = keywords.Text;
            _assessmentInfo.ScoreType = scoreType.SelectedValue;
            _assessmentInfo.PerformanceLevels = performanceLevelSet.SelectedValue;
            _assessmentInfo.Source = sourceInput.Value;
            _assessmentInfo.Credit = creditInput.Value;
            _assessmentInfo.PrintColumns = numberOfColumns1.Checked ? 1 : 2;
            _assessmentInfo.PrintShortAnswer = shortAnswerBubbleSheetYes.Checked;
            _assessmentInfo.IncludeCoverPage = includeCoverPageYes.Checked ? "Yes" : "No";

            Base.Classes.Assessment.SaveConfigurationInformation(_assessmentInfo, _assessmentID, currUserID);

            string js = "parent.window.location.reload();";
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "AssessmentConfigurationRefresh", js, true);
        }

        protected void gridTestTool_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Access_AssessmentToolConfiguration))
            {
                DataTable table = Base.Classes.Assessment.GetDefautTestTools();
                gridTestTool.DataSource = table;
            }

        }

        protected void gridTestTool_ItemDataBound(object sender, GridItemEventArgs e)
        {
            DataTable table = Base.Classes.Assessment.GetTestTool(_assessmentID);

            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = (GridDataItem)e.Item;

                int id = Convert.ToInt32(dataItem["ID"].Text);
                string toolName = dataItem["Name"].Text;

                CheckBox checkbx = (CheckBox)dataItem.FindControl("CheckBox2");

                foreach (DataRow row in table.Rows)
                {
                    if (Convert.ToInt32(row["OnlineToolID"].ToString()) == id)
                        checkbx.Checked = true;
                }

                // highlighter option is disabled for external assessments
                if (_assessmentInfo.ContentType == "External" && toolName.ToLower() == "highlighter")
                {
                    checkbx.Enabled = false;
                    checkbx.Checked = false;
                }
            }
        }

        protected void btnHelp_Click(object sender, ImageClickEventArgs e)
        {
            String FileName = DParmsNew.HelpScreenFileName.ToString();
            String FilePath = Page.ResolveUrl("~/faq/" + FileName);
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/pdf";
            response.AddHeader("Content-Disposition", "attachment; filename=\"" + FileName + "\"");
            response.WriteFile(FilePath);
            response.Flush();
            response.End();
        }
    }
}