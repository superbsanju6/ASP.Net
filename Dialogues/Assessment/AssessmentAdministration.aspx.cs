using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Standpoint.Core.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Linq;
using Thinkgate.Services.Contracts.Groups;
using Thinkgate.Core.Enumerations;
using Thinkgate.Core.Extensions;
using System.Text.RegularExpressions;


namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentAdministration : BasePage
    {
        #region Constants
        private const string ISGROUP = "IsGroup";       
        #endregion

        #region Variables

        DataSet _dsAssessmentAdmin;
        DataTable _dtAssessmentInfo;
        DataTable _dtAssessmentResults;
        int _userID;
        public static string LoggedOnUserRoleName = string.Empty;
        public static int SchoolId = 0;
        DistrictParms DParmsNew;
        public static string administrationDirections = string.Empty;
        bool isSecuredFlag;
        protected Dictionary<string, bool> dictionaryItem;
          
        #endregion


        /// <summary>
        /// Event fires when the page loads.  It extracts the AssessmentID and ClassID from the 
        /// QueryString, then calls methods that Load the necessary DataTables, and Initialize the UI.
        /// </summary>
        /// <param name="sender">Object that fired this event</param>
        /// <param name="e">EventArgs passed to the eventhandler</param>
        protected void Page_Load(object sender, EventArgs e)
        {
           
            _userID = SessionObject.LoggedInUser.Page;
            // converting value in mili secsecond.
             DParmsNew = DistrictParms.LoadDistrictParms();
           
            if (AreRequiredIDsContainedInQueryString())
            {
                AssessmentID = ExtractAssessmentID();
                ClassID = ExtractClassID();
                IsGroup = ExtractGroupStatus();
            }
            else
            {
                RedirectToPortalSelectionScreenWithCustomMessage("The class ID and/or assessment ID was not provided in URL.");
            }
            if (!IsPostBack)
            {
                // get role name from the session object
                LoggedOnUserRoleName = SessionObject.LoggedInUser.Roles[0].RoleName;
                SchoolId = SessionObject.LoggedInUser.School;
                BuildAssessment();
                ValidateCurriculumRestriction();
                InitializeUI(_dtAssessmentInfo.Rows[0]);
                // save configuration data for testeventid if data exist in configuration table.
                if (IsGroup)
                {
                    AddAssessmentAdministrationTimedInfo(AssessmentID, null, ClassID, CteID.ToString());
                }
                else
                {
                    AddAssessmentAdministrationTimedInfo(AssessmentID, ClassID, null, CteID.ToString());
                }

            }
            SetHiddenFields(GetAssessmentAdministrationTimedInfo(AssessmentID, CteID.ToString()));
            if (!IsPostBack)
            {
                
                hideUnhideColumn();
                grdAssessmentAdmin.DataSource = _dtAssessmentResults;
                grdAssessmentAdmin.DataBind();
                GenerateAdministrationInstructionsPoup();                
            }

            hdnAssessmentId.Value = AssessmentID.ToString();
            hdnClassId.Value = ClassID.ToString();
            hdnTestId.Value = CteID.ToString();
            //Fixed TFS bug#27033
            //Timer1.Interval = Convert.ToInt32(DParmsNew.AdministratorRefreshScreenInterval) * (60000);

            if (hiddenTimeAssessment.Value.ToLower() == "true")
            {
                Timer1.Interval = Convert.ToInt32(DParmsNew.AdministratorRefreshScreenInterval) * (60000);

            }
            else
            {
                Timer1.Enabled = false;
            }
            
        }


        private void SetPermissionsForSecureTest()
        {

            if (Request.QueryString["isSecure"] != null && Request.QueryString["isSecure"].ToLower() == "true" && UserHasPermission(Permission.Access_SecureTesting))
            {
                string category = string.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["Category"]))
                {
                    category = Request.QueryString["Category"];
                }
                dictionaryItem = TestTypes.TypeWithSecureFlag(category);
                isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                if (isSecuredFlag)
                {

                bool hasPermissionPrintAssessment = SessionObject.LoggedInUser.HasPermission(Permission.Icon_Access_SecureTesting_PrintAssessments);
                bool hasPermissionPrintBubbleSheets =SessionObject.LoggedInUser.HasPermission(Permission.Icon_Access_SecureTesting_PrintBubbleSheets);
                bool hasPermissionResetScores =SessionObject.LoggedInUser.HasPermission(Permission.Icon_Access_SecureTesting_ResetScores);
                bool hasPermissionManaualInput =SessionObject.LoggedInUser.HasPermission(Permission.Icon_Access_SecureTesting_ManualInput);
                bool hasPermissionScans =SessionObject.LoggedInUser.HasPermission(Permission.Icon_Access_SecureTesting_Scans);
                bool hasPermissionEnableOtc =SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting_EnableOTC);
                bool hasPermissionViewStudentScores =SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting_ViewStudentScores);
                hiddenAccessSecurePermission.Value =UserHasPermission(Permission.Access_SecureTesting).ToString();
                hiddenisSecuredFlag.Value = isSecuredFlag.ToString();
                hiddenisSecureAssessment.Value = "true";
                
                cntPrintAssessment.Visible = hasPermissionPrintAssessment;
                cntdisPrintAssessment.Visible = false;
                bubbleSheetContainer.Visible = hasPermissionPrintBubbleSheets;
                divButtonReset.Visible = hasPermissionResetScores;
                divButtonManual.Visible = hasPermissionManaualInput;
                btnScanContainer.Visible = hasPermissionScans;
                hiddenShowSecureOTCRefreshButton.Value = hasPermissionEnableOtc.ToString();
                if (!hasPermissionViewStudentScores)
                {
                    //hideUnhideColumn("true");
                    foreach (GridColumn column in grdAssessmentAdmin.Columns)
                    {
                        if (column.UniqueName == "Score")
                        {
                            column.Visible = false;
                        }

                    }
                }




                }
            }
        }

        private void SetHiddenFields(AssessmentAdministrationTimedInfo timedInfo)
        {            
            if (timedInfo != null)
            {
                hiddenTimeAssessment.Value = timedInfo.IsConfigurationTimedAssessment.ToString();
                hiddenTimeAllottedVal.Value = timedInfo.TimeAllotted.ToString();
                hiddenIsTimeExtension.Value = timedInfo.IsTimeExtension.ToString();
                hiddenTimeWarningInterval.Value = timedInfo.TimeWarningInterval.ToString();
                hiddenIsAutoSubmit.Value = timedInfo.IsConfigurationAutoSubmit.ToString();
                hiddenEnForceChkTimeAssessment.Value = timedInfo.EnforceTimedAssessment.ToString();
                hiddenEnForceChkTimeAllotted.Value = timedInfo.EnforceTimeAllotted.ToString();
                hiddenEnForceChkTimeExtension.Value = timedInfo.EnforceTimeExtension.ToString();
                hiddenEnForceChkTimeWarning.Value = timedInfo.EnforceTimeWarningInterval.ToString();
                hiddenEnForceChkAutoSubmit.Value = timedInfo.EnforceAutoSubmit.ToString();
                rtbTimeAllotted.Text = timedInfo.TimeAllotted.ToString();
                ChkboxAutoSubmit.Checked = Convert.ToBoolean(timedInfo.IsConfigurationAutoSubmit.ToString());
                hdnIsAdministrationTimeAssessment.Value = timedInfo.IsAdministrationTimedAssessment.ToString();
                hdnIsAdministrationAutoSuubmit.Value = timedInfo.IsAdministrationAutoSubmit.ToString();           

            }
            else
                hiddenTimeAssessment.Value = false.ToString();
        }

        /// <summary>
        /// AddAssessmentAdministrationTimedInfo
        /// </summary>
        /// <param name="testId"></param>
        /// <param name="classId"></param>
        /// <param name="testEventId"></param>
        private void AddAssessmentAdministrationTimedInfo(int testId, int? classId, int? groupId, string testEventId)
        {
            Base.Classes.Assessment.AddAssessmentAdministrationTimedInfo(testId, classId, groupId, testEventId);
        }

        private AssessmentAdministrationTimedInfo GetAssessmentAdministrationTimedInfo(int testId, string testEventId)
        {
            return Base.Classes.Assessment.GetAssessmentAdministrationTimedInfo(testId, testEventId);
        }

        //Save Time Interval
        [System.Web.Services.WebMethod]
        public static string AddTimeWarningInterval(string testEventId, int timeIntervalVal)
        {
            Base.Classes.Assessment.AddAdminTimeWarningInterval(testEventId, timeIntervalVal);
            return "";

        }

        [System.Web.Services.WebMethod]
        public static string DeleteTimeWarningInterval(string testEventId, int timeIntervalVal)
        {
            Base.Classes.Assessment.DeleteAdminTimeWarningInterval(testEventId, timeIntervalVal);
            return "";
        }


        //Save Timed assessment
        [System.Web.Services.WebMethod]
        public static string SaveTimedAssessment(int testId, string isTimeAessessment, string isAutoSubmit, string timeExtn, string timeAllotedVal, string studentIds, string TestEventId)
        {
            var studentIdsList = studentIds.Split(',').Select(int.Parse).ToList();
            DataTable dtStudentIds = new DataTable();
            dtStudentIds.Columns.Add("Value", typeof(int));
            foreach (int item in studentIdsList)
            {
                DataRow dr = dtStudentIds.NewRow();
                dr["Value"] = item;
                dtStudentIds.Rows.Add(dr);
            }

            Base.Classes.Assessment.SaveTimedAssessment(testId, Convert.ToBoolean(isTimeAessessment), Convert.ToBoolean(isAutoSubmit), timeExtn == "" ? 0 : Convert.ToInt32(timeExtn), Convert.ToInt32(timeAllotedVal), dtStudentIds, TestEventId);
            return "true";

        }

        [System.Web.Services.WebMethod]
        public static string timedOnCheckChanged(string testEventId, bool isTimeChecked, int studentId, int testId)
        {
            Base.Classes.Assessment.TimedAssessmentCheckUncheckForStudent(testEventId, isTimeChecked, studentId, testId);
            return "true";

        }

        /// <summary>
        /// This method displays the loaded data on our UI, sets the Statuses and Visibility of the various
        /// buttons, and binds our assessment data to the Assessment Datagrid
        /// </summary>
        /// <param name="row">The DataRow that contains the necessary information for our UI</param>
        private void InitializeUI(DataRow row)
        {
            InitializeHeader(row);
            OnlineEnabled = string.Compare(row["OnlineTestStatus"].ToString(), "Disabled", true, CultureInfo.CurrentCulture) != 0;
            SetupButtons(row);
        }

        /// <summary>
        /// Sets the data to display on the controls of our Page Header...Teacher Name, Test Name, etc
        /// </summary>
        /// <param name="row">The DataRow that contains the necessary information for our UI</param>
        private void InitializeHeader(DataRow row)
        {
            // Save the class test event id.
            CteID = Int64.Parse(row["ClassTestEventID"].ToString());
            lblTestName.Text = row["TestName"].ToString();
            lblDescription.Text = row["Description"].ToString();
            lblTeacherName.Text = row["TeacherName"].ToString();
            lblClassTestEventID.Text = CteID.ToString(CultureInfo.CurrentCulture);
            lblSecurityStatus.Text = row["SecurityStatus"].ToString();
            lblContentWindow.Text = row["ContentWindow"].ToString();
            lblPrintWindow.Text = row["PrintWindow"].ToString();
            lblNameIdentifier.Text = IsGroup ? "Group Name:" : "Class Name:";
            lblClassName.Text = row["ClassName"].ToString();
            administrationDirections = RemoveHRefs((row["AdministrationDirections"]).ToString());           
        }

        /// <summary>
        /// Validates students belong to the teacher's curriculum for that particular class 
        /// </summary>
        /// <param name="studentCount">count of students belonging to the group</param>
        private void ValidateCurriculumRestriction()
        {
            if (IsGroup)
            {
                int studentCount = _dsAssessmentAdmin.Tables[1].Rows.Count;
                GroupsProxy prxGroups = new GroupsProxy();
                List<GroupStudentDataContract> lstGroupStudents = prxGroups.GetStudentsInGroup(
                    ClassID,
                    Base.Classes.DistrictParms.LoadDistrictParms().ClientID);

                if (studentCount < lstGroupStudents.Count)
                {
                    string currMessage = (lstGroupStudents.Count - studentCount) + " of "
                                         + lstGroupStudents.Count
                                         + " students excluded due to curriculum restriction on the assessment.";

                    ScriptManager.RegisterStartupScript(this.Page, typeof(string), "CurriculumRestriction", "parent.customDialog({ title: \"Curriculum Restriction\", height: 110, width: 500, animation: \"None\", dialog_style: \"alert\", content: \"" + currMessage + "\"   }, [{ title: \"Ok\"}]);; ", true);
                }
            }
        }

        /// <summary>
        /// This method calls various sub methods to setup the visibility and state of the various buttons on the UI
        /// </summary>
        /// <param name="row">The DataRow that contains the necessary information to determine the state of our buttons</param>
        private void SetupButtons(DataRow row)
        {
            SetOnlineButton();
            SetManualButtonEnabledState(row);
            SetResetButtonState(row);
            SetResetButtonDivVisibility(row);
            SetManualInputVisibility(row);
        }

        /// <summary>
        /// Sets the visibility for the "Manual" button
        /// </summary>
        /// <param name="row">The DataRow containing the information we need to determine this buttons state</param>
        private void SetManualInputVisibility(DataRow row)
        {
            if (row["TestCategory"].ToString() == EntityTypes.District.ToString() &&
                !UserHasPermission(Permission.Icon_ManualInput))
            {
                divButtonManual.Visible = false;
            }
        }

        //TODO: Figure out what this is exactly
        private void SetResetButtonDivVisibility(DataRow row)
        {
            if (row["TestCategory"].ToString() == EntityTypes.District.ToString() &&
                !UserHasPermission(Permission.Icon_ResetDistrictAssessment))
            {
                divButtonReset.Visible = false;
            }
        }

        /// <summary>
        /// Sets the visibility for the "Reset" button
        /// </summary>
        /// <param name="row">The DataRow containing the information we need to determine this buttons state</param>
        private void SetResetButtonState(DataRow row)
        {
            // enable/disable "reset" button.
            if (row["TestCategory"].ToString() == EntityTypes.State.ToString() &&
                !UserHasPermission(Permission.Icon_ResetStateAssessment))
            {
                btnReset.Attributes["buttonSecurity"] = "Inactive";
            }
        }

        private void SetManualButtonEnabledState(DataRow row)
        {
            // enable/disable "manual" button.
            if (!UserHasPermission(Permission.Icon_ManualInput_SecurityOverride)
                && row["AssessmentSecurity"].ToString() == "Inactive"
                && DistrictParms.ManualInput_AdministrationWindow.ToLower() == "yes"
                || (row["TestCategory"].ToString() == "State"
                    && !UserHasPermission(Permission.Icon_ManualInputState)))
            {
                btnManualInput.Attributes["buttonSecurity"] = "Inactive";
            }
            else
            {
                btnManualInput.Attributes["buttonSecurity"] = "Active";
                btnManualInput.Attributes["contentType"] = row["ContentType"].ToString();
            }
        }

        protected void SetOnlineButton()
        {
            if (_dtAssessmentInfo == null)
            {
                _dsAssessmentAdmin = Base.Classes.Assessment.LoadAssessmentAdmin(_userID, AssessmentID, ClassID, IsGroup);
                _dtAssessmentInfo = _dsAssessmentAdmin.Tables[0];
            }
            var assessmentSecurity = _dtAssessmentInfo.Rows[0]["AssessmentSecurity"].ToString();
            var extraText = assessmentSecurity == "Inactive" ? "_inactive" : string.Empty;
            btnEnableDisable.ImageUrl = (OnlineEnabled) ? "~/Images/enable" + extraText + ".png" : "~/Images/disable" + extraText + ".png";
            txtEnableDisableLive.InnerText = (OnlineEnabled) ? "Enabled" : "Disabled";
            btnEnableDisable.Attributes["onclick"] = assessmentSecurity == "Inactive" ? "return false;" : string.Empty;
            btnEnableDisable.Attributes["style"] = assessmentSecurity == "Inactive" ? "cursor:default;" : string.Empty;
        }

        /// <summary>
        /// Loads the DataSet that contains all the information we need to display on this page
        /// and pulls our the first two tables into 
        /// </summary>
        private void LoadDataTables()
        {
            _dsAssessmentAdmin = Base.Classes.Assessment.LoadAssessmentAdmin(_userID, AssessmentID, ClassID, IsGroup);
            _dtAssessmentInfo = _dsAssessmentAdmin.Tables[0];
            _dtAssessmentResults = _dsAssessmentAdmin.Tables[1];
            _dtAssessmentResults = Encryption.EncryptDataTableColumn(_dtAssessmentResults, "StudentID", "ID_Encrypted");
        }

        //TODO This method needs to be refactored badly, it is almost 200 lines long
        /// <summary>
        /// Refactor this method when someone has time.  Its ridiculously long....
        /// </summary>
        protected void BuildAssessment()
        {
            LoadDataTables();
            if (_dtAssessmentInfo != null && _dtAssessmentInfo.Rows.Count > 0)
            {
                hiddenAssessmentCategory.Value = _dtAssessmentInfo.Rows[0]["TestCategory"].ToString();
            }
           List<string> AllStudentIdsList = new List<string>();
            // Compute total time column.
            foreach (DataRow row in _dtAssessmentResults.Rows)
            {
                DateTime start, end; 
                if (DParmsNew.ClientTimeZone == ClientTimeZone.EST.ToString())
                {                    
                    if (row["TestStartDate"] is String && DateTime.TryParse((String)row["TestStartDate"], out start))
                    {
                        row["TestStartDate"] = TimeZoneInfo.ConvertTimeFromUtc(start, TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.EST.Description()));
                    }
                    if (row["TestCompleteDate"] is String && DateTime.TryParse((String)row["TestCompleteDate"], out end))
                    {
                        row["TestCompleteDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(Convert.ToString(row["TestCompleteDate"])), TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.EST.Description()));
                    }
                }
                else if (DParmsNew.ClientTimeZone == ClientTimeZone.CST.ToString())
                {
                    if (row["TestStartDate"] is String && DateTime.TryParse((String)row["TestStartDate"], out start))
                    {
                        row["TestStartDate"] = TimeZoneInfo.ConvertTimeFromUtc(start, TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.CST.Description()));
                    }
                    if (row["TestCompleteDate"] is String && DateTime.TryParse((String)row["TestCompleteDate"], out end))
                    {
                        row["TestCompleteDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(Convert.ToString(row["TestCompleteDate"])), TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.CST.Description()));
                    }
                }
                else if (DParmsNew.ClientTimeZone == ClientTimeZone.PST.ToString())
                {
                    if (row["TestStartDate"] is String && DateTime.TryParse((String)row["TestStartDate"], out start))
                    {
                        row["TestStartDate"] = TimeZoneInfo.ConvertTimeFromUtc(start, TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.PST.Description()));
                    }
                    if (row["TestCompleteDate"] is String && DateTime.TryParse((String)row["TestCompleteDate"], out end))
                    {
                        row["TestCompleteDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(Convert.ToString(row["TestCompleteDate"])), TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.PST.Description()));
                    }
                }
                else if (DParmsNew.ClientTimeZone == ClientTimeZone.MST.ToString())
                {
                    if (row["TestStartDate"] is String && DateTime.TryParse((String)row["TestStartDate"], out start))
                    {
                        row["TestStartDate"] = TimeZoneInfo.ConvertTimeFromUtc(start, TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.MST.Description()));
                    }
                    if (row["TestCompleteDate"] is String && DateTime.TryParse((String)row["TestCompleteDate"], out end))
                    {
                        row["TestCompleteDate"] = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(Convert.ToString(row["TestCompleteDate"])), TimeZoneInfo.FindSystemTimeZoneById(ClientTimeZone.MST.Description()));
                    }
                }
                if (row["TestStartDate"] is String && DateTime.TryParse((String)row["TestStartDate"], out start) &&
                         row["TestCompleteDate"] is String && DateTime.TryParse((String)row["TestCompleteDate"], out end))
                {
                    TimeSpan span = end - start;                                   
                    row["TotalTime"] = span.TotalMinutes.ToString();
                }
                AllStudentIdsList.Add(row["StudentID"].ToString());
            }
            hdnAllStudentIdsList.Value = string.Join(",", AllStudentIdsList.ToArray()); 
            // We must save the dataset in view state to allow sorting later.
            ViewState["_dtAssessmentResults"] = _dtAssessmentResults;

            DataTable dtScans = _dsAssessmentAdmin.Tables[2];

            // Wd have bubble sheet scans if the number of rows is > 0.
            Boolean hasScans = dtScans.Rows.Count > 0;

            // Now we need to check for errors.
            Boolean hasErrors = false;

            foreach (DataRow row in dtScans.Rows)
            {
                if ((Int32)row["#Error"] > 0)
                    hasErrors = true;
            }

            btnScans.Src = (!hasScans) ? "~/Images/BtnScansDisabled.png" : (hasErrors) ? "~/Images/BtnScansNotification.png" : "~/Images/BtnScans.png";
            btnScans.Disabled = !hasScans;
            String scansOnclick = @"showAssessmentScans(" + AssessmentID.ToString(CultureInfo.CurrentCulture) + "," + ClassID.ToString(CultureInfo.CurrentCulture) + ",'" + IsGroup.ToString(CultureInfo.CurrentCulture) + "')";
            if (btnScans.Disabled == false)
            {
                btnScans.Attributes["onclick"] = scansOnclick;
            }
            btnScans.Style["cursor"] = (hasScans) ? "pointer" : "default";

            if (_dtAssessmentInfo != null &&
                _dtAssessmentInfo.Rows[0]["ContentType"].ToString() == "No Items/Content")
            {
                printIconTable.Attributes["style"] = "display:none;";
                btnScans.Attributes["style"] = "display:none;";
                Panel2.Attributes["style"] = "display:none;";
                scansTD.Attributes["style"] = "display:none;";
            }
            btnManualInput.Attributes["AssessmentID"] = AssessmentID.ToString(CultureInfo.CurrentCulture);
            btnManualInput.Attributes["ClassID"] = ClassID.ToString(CultureInfo.CurrentCulture);

            if (_dtAssessmentInfo != null)
            {
                btnManualInput.Attributes["ScoreType"] = _dtAssessmentInfo.Rows[0]["ScoreType"].ToString();
            }

            // Set the print button visibility.
            // Get the assessment type and rubric.
            String assessmentType;
            Boolean hasRubric, hasUpload,hasInstructions; // hasUpload is unused.
            Base.Classes.Assessment.GetPrintOptions(_userID, AssessmentID, out assessmentType, out hasRubric, out hasUpload, out hasInstructions);
            Permission assessmentPerm = (Permission)Enum.Parse(typeof(Permission), "Print_Assessment" + assessmentType, true);
            Permission answerKeyPerm = (Permission)Enum.Parse(typeof(Permission), "Print_AnswerKey" + assessmentType, true);
            Permission rubricsPerm = (Permission)Enum.Parse(typeof(Permission), "Print_Rubrics" + assessmentType, true);
            cntPrintAssessment.Visible = UserHasPermission(assessmentPerm) || UserHasPermission(answerKeyPerm) ||
                                                                     (hasRubric && UserHasPermission(rubricsPerm));

            // 6362: the print icon for District Assessments( and State Assessments - NCCTE Only) 
            // should be controlled by the print window security settings. 
            bool bPrintFlag = true;

            DataTable dtPrintEndDate = Base.Classes.Assessment.GetPrintSecurityStatus("Assessment", "District", AssessmentID);

            DateTime todayDate = DateTime.Today.Date;

            if (dtPrintEndDate.Rows.Count > 0 && !UserHasPermission(Permission.Icon_PrintIcon_SecurityOverride))
            {
                foreach (DataRow dr in dtPrintEndDate.Rows)
                {
                    // If print security status is inactive then print icon should be disbaled
                    if (dr["print_lock"].ToString() == "True")
                        bPrintFlag = false;
                    else
                    {
                        if (dr["print_begin"].ToString() != string.Empty)
                        {
                            DateTime printBeginDate = Convert.ToDateTime(dr["print_begin"].ToString()).Date;
                            // If the today's is before the begin date then gray out print icon
                            if (todayDate < printBeginDate) bPrintFlag = false;
                        }
                        if (dr["print_end"].ToString() != string.Empty)
                        {
                            DateTime printEndDate = Convert.ToDateTime(dr["print_end"].ToString()).Date;
                            // If the print end date is in past today's date then gray out print icon
                            if (todayDate > printEndDate) bPrintFlag = false;
                        }
                    }
                }
            }


            if (bPrintFlag)
            {
                cntPrintAssessment.Visible = UserHasPermission(assessmentPerm) || UserHasPermission(answerKeyPerm) ||
                                                                    (hasRubric && UserHasPermission(rubricsPerm));

                cntdisPrintAssessment.Visible = false;
            }
            else
            {
                cntdisPrintAssessment.Visible = true;
                cntPrintAssessment.Visible = false;
            }
            SetPermissionsForSecureTest();
        }

        /// <summary>
        /// The online enabled property.
        /// </summary>
        protected Boolean OnlineEnabled
        {
            get { return DataIntegrity.ConvertToBool(inpOnlineEnabled.Value); }
            set { inpOnlineEnabled.Value = value.ToString(); }
        }

        /// <summary>
        /// Gotta have it for sort to work ??? wierd.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void grdAssessmentAdmin_SortCommand(object source, GridSortCommandEventArgs e)
        {
        }

        /// <summary>
        /// grdAssessmentAdmin fires the NeedDataSource event each time it needs to be bound to a data source
        /// </summary>
        /// <param name="source">The object that fired this event - grdAssessmentAdmin</param>
        /// <param name="e">GridNeedDataSourceEventArgs passed to the handler</param>
        protected void grdAssessmentAdmin_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            hideUnhideColumn();
            String gridSortString = grdAssessmentAdmin.MasterTableView.SortExpressions.GetSortString();
            _dtAssessmentResults = (DataTable)ViewState["_dtAssessmentResults"];

            if (_dtAssessmentResults != null)
            {
                DataView view = _dtAssessmentResults.AsDataView();
                view.Sort = gridSortString;
                grdAssessmentAdmin.DataSource = _dtAssessmentResults;
            }
        }

        //hind the grid column

        private void hideUnhideColumn(string isSecure = "false")
        {
            //if (hiddenTimeAssessment.Value.ToLower() == "false" || Convert.ToBoolean(hiddenEnForceChkTimeAssessment.Value)==false)
            if (hiddenTimeAssessment.Value.ToLower() == "false")
            {
                foreach (GridColumn column in grdAssessmentAdmin.Columns)
                {
                    if (column.UniqueName == "TTimed" || column.UniqueName == "TimeRemaining")
                    {
                        column.Visible = false;
                    }

                }
            }
            else

                foreach (GridColumn column in grdAssessmentAdmin.Columns)
                {
                    if (column.UniqueName == "TTimed" || column.UniqueName == "TimeRemaining")
                    {
                        column.Visible = true;
                    }

                }

            //if (isSecure == "false")
            //{
            //    foreach (GridColumn column in grdAssessmentAdmin.Columns)
            //    {
            //        if (column.UniqueName == "Score")
            //        {
            //            column.Visible = false;
            //        }

            //    }
            //}
        }


        /// <summary>
        /// The assessment id property.
        /// </summary>
        protected Int32 AssessmentID
        {
            //Rob: 6/5/2014  
            //Don't think we need to encrypt or decrypt these values...
            //If Im wrong, this was the original code
            //get { return Int32.Parse(Encryption.DecryptString(inpAssessmentID.Value)); }
            //set { inpAssessmentID.Value = Encryption.EncryptString(value.ToString(CultureInfo.CurrentCulture)); }

            get { return Int32.Parse(inpAssessmentID.Value); }
            set { inpAssessmentID.Value = value.ToString(CultureInfo.CurrentCulture); }
        }

        /// <summary>
        /// The class id property.
        /// </summary>
        protected Int32 ClassID
        {
            get { return Int32.Parse(inpClassID.Value); }
            set { inpClassID.Value = value.ToString(CultureInfo.CurrentCulture); }
        }

        /// <summary>
        /// Determins if a class is actually a Group
        /// </summary>
        protected bool IsGroup
        {
            get { return Boolean.Parse(inpIsGroup.Value); }
            set { inpIsGroup.Value = value.ToString(); }
        }

        /// <summary>
        /// The class test event id property.
        /// </summary>
        protected Int64 CteID
        {
            get { return Int64.Parse(inpCteID.Value); }
            set { inpCteID.Value = value.ToString(CultureInfo.CurrentCulture); }
        }

        /// <summary>
        /// Refresh the info in the data grid.
        /// </summary>
        protected void RefreshGrid()
        {
            BuildAssessment();
            grdAssessmentAdmin.Rebind();
        }

        /// <summary>
        /// Gets a list of all the StudentIDs corresponding to the students who have been selected
        /// in the data grid
        /// </summary>
        /// <returns>List of student IDs for the students selected in the datagrid</returns>
        protected List<Int32> GetSelectedStudentIds()
        {
            _dtAssessmentResults = (DataTable)ViewState["_dtAssessmentResults"];

            List<Int32> studentIds = new List<int>();
            GridItemCollection coll = grdAssessmentAdmin.SelectedItems;
            foreach (GridItem item in coll)
                studentIds.Add((Int32)_dtAssessmentResults.Rows[item.DataSetIndex]["StudentID"]);
            return studentIds;
        }

        #region Click Events

        /// <summary>
        /// Reset any selected students to 'no score'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDoReset_Click(object sender, EventArgs e)
        {
            Base.Classes.Assessment.ResetScore(GetSelectedStudentIds(), AssessmentID, ClassID, _userID);
            this.Page_Load(null, null);
            RefreshGrid();
        }

        protected void btnEnableDisable_Click(object sender, ImageClickEventArgs e)
        {
            OnlineEnabled = !OnlineEnabled;
            Base.Classes.Assessment.UpdateOnlineTestStatus(OnlineEnabled, AssessmentID, ClassID, _userID);
            SetOnlineButton();
        }

        protected void btnContinue_Click(object sender, ImageClickEventArgs e)
        {
            Base.Classes.Assessment.ContinueAssessment(GetSelectedStudentIds(), AssessmentID, ClassID, _userID);
            RefreshGrid();
        }
        protected void Btnrefresh1_Click(object sender, ImageClickEventArgs e)
        {
            RefreshGrid();
        }

        protected void btnSuspend_Click(object sender, ImageClickEventArgs e)
        {
            Base.Classes.Assessment.SuspendAssessment(GetSelectedStudentIds(), AssessmentID, _userID);
            RefreshGrid();
        }

        protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
        {
            RefreshGrid();
        }


        /// <summary>
        /// 
        /// Scans button has been clicked.
        /// We must launch a new modal dialog showing the scan data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScans_Click(object sender, ImageClickEventArgs e)
        {
        }

        #endregion

        /// <summary>
        /// Checks to see if the AssessmentID and the ClassID are stored in the QueryString
        /// </summary>
        /// <returns>true if QueryString contains AssessmentID and ClassID; false otherwise</returns>
        private bool AreRequiredIDsContainedInQueryString()
        {
            return !IsQueryStringMissingParameter(X_ID) && !IsQueryStringMissingParameter(Y_ID) && !IsQueryStringMissingParameter(ISGROUP);
        }

        /// <summary>
        /// Extracts the AssessmentID from the QueryString
        /// </summary>
        /// <returns>The AssessmentID contained in the Query String</returns>
        private int ExtractAssessmentID()
        {
            int returnValue;
            Int32.TryParse(Request.QueryString[X_ID], out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Extracts the ClassID from the QueryString
        /// </summary>
        /// <returns>The ClassID contained in the Query String</returns>
        private int ExtractClassID()
        {
            int returnValue;
            Int32.TryParse(Request.QueryString[Y_ID], out returnValue);
            return returnValue;
        }

        private bool ExtractGroupStatus()
        {
            bool isGroup;
            Boolean.TryParse(Request.QueryString[ISGROUP], out isGroup);
            return isGroup;
        }

        protected void grdAssessmentAdmin_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                DataRowView row1 = (DataRowView)item.DataItem;
              
                //Your Condition to show the image

                if (!string.IsNullOrEmpty(row1["MinuteDiff"].ToString()))
                {
                    if (Convert.ToInt64(row1["MinuteDiff"].ToString()) > Convert.ToInt64(DParmsNew.OfflineIndicatorInterval))
                    {
                        if (row1["Status"].ToString() == "In Process")
                        {
                            var img = item.FindControl("imgeTringle");
                            img.Visible = true;
                        }
                    }
                }

                //1 min functionality
                //if (clontrolName != null && clontrolName=="Timer2")
                //{
                //    if (row1["Status"].ToString() == "In Process")
                //    {
                //        row1["TimeRemaining"] = Convert.ToInt32(row1["TimeRemaining"].ToString()) - 1;
                //        //item["TimeRemaining"] = Convert.ToInt32(item["TimeRemaining"].ToString()) - 1;
                //    }
                //}

                //Timed and TimeRemaining enable disable
                DataRowView row = (DataRowView)item.DataItem;
                CheckBox chk = (CheckBox)item["TTimed"].Controls[1];


                //if (chk.Checked == false)
                //{
                //    item["TimeRemaining"].ForeColor = Color.Gray;
                //}
                if (hdnIsAdministrationTimeAssessment.Value.ToLower() == "false")
                {
                    chk.Enabled = false;
                    item["TimeRemaining"].ForeColor = Color.Gray;
                }
                //if (hdnIsAdministrationTimeAssessment.Value.ToLower() == "true" && chk.Checked == false)
                //{
                //    chk.Enabled = false;
                //    item["TimeRemaining"].ForeColor = Color.Gray;
                //}
            }
        }

        protected void grdAssessmentAdmin_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                string studentId = e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["StudentID"].ToString();
                CheckBox cbChecked = (CheckBox)e.Item.FindControl("cbChecked");
                cbChecked.Attributes["onclick"] = String.Format("AlertMessage('{0}','{1}');", studentId, cbChecked.ClientID);

            }
        }

        public void Timer1_Tick(object sender, EventArgs e)
        {
            RefreshGrid();
           
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
       
        /// <summary>
        /// This is made server side to control the scroll position at top when click on view Link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkViewInst_Click(object sender, EventArgs e)
        {           
            string script = "function f(){$find(\"" + RadWindow1.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        private void GenerateAdministrationInstructionsPoup()
        {           
            if (!string.IsNullOrEmpty(administrationDirections))
            {
                string script = "function f(){$find(\"" + RadWindow1.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
        }
        private string RemoveHRefs(string instructions)
        {
            if (!string.IsNullOrEmpty(instructions))
            {
                instructions = Regex.Replace(instructions, "<a href=\"(.+?)\">(.+?)</a>", "$2", RegexOptions.IgnoreCase);
            }           
            return instructions;
        }
        
    }
}
