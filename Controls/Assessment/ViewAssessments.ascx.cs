using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Standpoint.Core.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Enums.AssessmentScheduling;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using System.Collections.Generic;
using System.Linq;

namespace Thinkgate.Controls.Assessment
{
    public partial class ViewAssessments : TileControlBase
    {
        // Level will be one of District, Teacher, School, Class;
        protected Thinkgate.Base.Enums.EntityTypes _level;
        // Category will be one of State, District, Classroom.
        protected String _category;
        protected Int32 _userID;
        protected Int32 _levelID;
        protected Int32 _schoolID;
        // True if this is a postback.
        protected Boolean _isPostBack;

        //Add to Calendar icon visible
        protected bool _calendarIconVisible = false;

        // Which filter buttons are visible.
        protected Boolean _gradeVisible, _subjectVisible, _termVisible, _testTypeVisible, _statusVisible;

        // View state keys.
        protected String _currentViewIdxKey = "CurrentViewIdx", _gradeFilterKey = "GradeFilter", _subjectFilterKey = "SubjectFilter", _termFilterKey = "TermFilter";
        protected String _testTypeFilterKey = "TestTypeFilter", _statusFilterKey = "StatusFilter";

        // We only want to show the first 100 assessments.
        private const Int32 _maxAssessments = 100;

        private DataTable dtGrade;
        private DataTable dtSubject;
        private DataTable dtTerm;
        private DataTable dtTestType;
        private DataTable dtAssessment;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null) return;

            _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            AttatchLevelToKeys();
            _category = (String)Tile.TileParms.GetParm("category");
            _levelID = (Int32)Tile.TileParms.GetParm("levelID");

            //PLH - 01/15/2013 - Fix for forcing Classroom assessments to drill down to selected school in Classes folder only. 
            if (Tile.TileParms.Parms.ContainsKey("schoolID"))
            {
                _schoolID = (Int32)Tile.TileParms.GetParm("schoolID");
            }
            
            if (Tile.TileParms.GetParm("showCalendarIcon") != null)
            {
                _calendarIconVisible = DataIntegrity.ConvertToBool(Tile.TileParms.GetParm("showCalendarIcon"));
            }
        }

#region Asynchronous Tasks
        private void LoadGrades()
        {
            dtGrade = Thinkgate.Base.Classes.Assessment.LoadMockGrades(_userID, SessionObject.GlobalInputs);
        }

        private void LoadSubjects()
        {
            dtSubject = Thinkgate.Base.Classes.Assessment.LoadMockSubjects(_userID, SessionObject.GlobalInputs);
        }

        private void LoadTerms()
        {
            dtTerm = Thinkgate.Base.Classes.Assessment.LoadTerms(_userID, SessionObject.GlobalInputs);
        }

        private void LoadTestTypes()
        {
             dtTestType = Thinkgate.Base.Classes.Assessment.LoadTestTypes(_userID, _category, SessionObject.GlobalInputs);
        }

        private void LoadAssessments()
        {
            String gradeFilter = (String)ViewState[_gradeFilterKey];
            String subjectFilter = (String)ViewState[_subjectFilterKey];
            Thinkgate.Base.Classes.CourseList courses = Thinkgate.Base.Classes.CourseMasterList.CurrCourseList;
            courses = courses.FilterByGradeAndSubject((gradeFilter == "All") ? null : gradeFilter, (subjectFilter == "All") ? null : subjectFilter);
            List<Int32> courseIds = new List<int>();
            foreach (Thinkgate.Base.Classes.Course c in courses)
                courseIds.Add(c.ID);
            List<ThinkgateSchool> schools = SessionObject.LoggedInUser.Schools;
            List<Int32> schoolIds = new List<Int32>(); 
            //PLH - 01/15/2013 - Drill down to school for Classroom Assessments from Classes Folder ONLY
            if (_schoolID != 0)
            {
                schoolIds.Add(_schoolID);
            }
            else
            {
                schoolIds = (from s in schools select s.Id).ToList();
            }
            List<ThinkgateRole> roles = SessionObject.LoggedInUser.Roles;
            List<String> roleNames = (from r in roles select r.RoleName).ToList();

            dtAssessment = Thinkgate.Base.Classes.Assessment.LoadAssessments(_category, _level.ToString(), _levelID,
                                                                                                                 courseIds.ToSql(), (String)ViewState[_termFilterKey],
                                                                                                                 (String)ViewState[_testTypeFilterKey], (String)ViewState[_statusFilterKey],
                                                                                                                 schoolIds.ToSql(), roleNames.ToSql(), UserHasPermission(Permission.User_Cross_Schools), SessionObject.GlobalInputs, false);

            while (dtAssessment.Rows.Count > _maxAssessments)
                dtAssessment.Rows.RemoveAt(dtAssessment.Rows.Count - 1);

            dtAssessment = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtAssessment, "TestID", "TestID_Encrypted");

            // We must sometimes truncate the test description so that it can fit in the area provided.
            // When truncated we add ellipsis (...).
            DataColumn listDescCol = dtAssessment.Columns.Add("ListDescription");
            DataColumn graphicDescCol = dtAssessment.Columns.Add("GraphicDescription");
            DataColumn dateEdited = dtAssessment.Columns.Add("DateEdited");
            DataColumn percentage = dtAssessment.Columns.Add("Percentage", typeof(Int32));

            const Int32 listChars = 47, graphicChars = 36;
            for (Int32 i = 0; i < dtAssessment.Rows.Count; i++)
            {
                DataRow row = dtAssessment.Rows[i];
                String name = row["TestName"] != DBNull.Value ? (String)row["TestName"] : "";
                String listDesc = (row["Description"] is String && !String.IsNullOrEmpty((String)row["Description"])) ? " - " + (String)row["Description"] : "";
                String graphicDesc = listDesc;

                if (name.Length + listDesc.Length > listChars)
                    listDesc = listDesc.Substring(0, Math.Max(0, listChars - name.Length - 3)) + "...";
                row[listDescCol] = listDesc;

                if (name.Length + graphicDesc.Length > graphicChars)
                    graphicDesc = graphicDesc.Substring(0, Math.Max(0, graphicChars - name.Length - 3)) + "...";
                row[graphicDescCol] = graphicDesc;

                // Stored proc sometimes returns the number of scored students as null.
                if (!(row["Scored"] is Int32))
                    row["Scored"] = 0;
                if (!(row["Seated"] is Int32))
                    row["Seated"] = 0;

                row[dateEdited] = ((DateTime)row["DateUpdated"]).ToShortDateString();

                // Add the percentage scored entry.
                Int32 scored = (Int32)row["Scored"];
                Int32 seated = (Int32)row["Seated"];
                row[percentage] = (seated <= 0) ? 0 : (Int32)Math.Round(100.0 * scored / seated);
            }

            // Add an empty row at the end if we have the maximum number of assessments.
            // This is used as a placeholder for the 'More Results...' line.
            if (dtAssessment.Rows.Count >= _maxAssessments)
            {
                DataRow newRow = dtAssessment.NewRow();
                dtAssessment.Rows.Add(newRow);
            }

            Boolean isEmpty = dtAssessment.Rows.Count == 0;

            lbxList.DataSource = dtAssessment;
            lbxList.DataBind();
            lbxList.Visible = !isEmpty;
            pnlListNoResults.Visible = isEmpty;

            lbxGraphic.DataSource = dtAssessment;
            lbxGraphic.DataBind();
            lbxGraphic.Visible = !isEmpty;
            pnlGraphicNoResults.Visible = isEmpty;
        }
#endregion

        private void AttatchLevelToKeys()
        {
            _currentViewIdxKey += Tile.Title.Replace(" ", string.Empty);
            _gradeFilterKey += Tile.Title.Replace(" ", string.Empty);
            _subjectFilterKey += Tile.Title.Replace(" ", string.Empty);
            _termFilterKey += Tile.Title.Replace(" ", string.Empty);
            _testTypeFilterKey += Tile.Title.Replace(" ", string.Empty);
            _statusFilterKey += Tile.Title.Replace(" ", string.Empty);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _userID = SessionObject.LoggedInUser.Page;

            // Simulate IsPostBack.
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            // Create the initial viewstate values.
            if (ViewState[_currentViewIdxKey] == null)
            {
                ViewState.Add(_currentViewIdxKey, 0);
                ViewState.Add(_gradeFilterKey, "All");
                ViewState.Add(_subjectFilterKey, "All");
                ViewState.Add(_termFilterKey, "All");
                ViewState.Add(_testTypeFilterKey, "All");                
                SetViewstateForStatusFilter();
            }


            BtnAdd.Attributes["testCategory"] = _category;
            switch (_level)
            {
                case EntityTypes.Class:
                    BtnAdd.Attributes["level"] = "Class";
                    break;
                case EntityTypes.Teacher:
                    BtnAdd.Attributes["level"] = "Teacher";
                    break;
                case EntityTypes.School:
                    BtnAdd.Attributes["level"] = "School";
                    break;
                case EntityTypes.District:
                    BtnAdd.Attributes["level"] = "District";
                    break;
            }


            BtnAdd.Visible = (_category == AssessmentCategories.District.ToString() && UserHasPermission(Permission.Create_AssessmentDistrict)) ||
                                             (_category == AssessmentCategories.Classroom.ToString() && UserHasPermission(Permission.Create_AssessmentClassroom)) ||
											 (_category == AssessmentCategories.State.ToString() && UserHasPermission(Permission.Create_AssessmentState));



			btnScheduler.Attributes["yID"] = Encryption.EncryptString(_category);
			switch (_level)
			{
				case EntityTypes.Class:
					btnScheduler.Attributes["xID"] = Encryption.EncryptString(AssessmentScheduleLevels.Class.ToString());
					break;
				case EntityTypes.Teacher:
					btnScheduler.Attributes["xID"] = Encryption.EncryptString(AssessmentScheduleLevels.Class.ToString());
					break;
				case EntityTypes.School:
					btnScheduler.Attributes["xID"] = Encryption.EncryptString(AssessmentScheduleLevels.School.ToString());
					break;
				case EntityTypes.District:
					btnScheduler.Attributes["xID"] = Encryption.EncryptString(AssessmentScheduleLevels.District.ToString());
					break;
			}


			btnScheduler.Visible = (_category == AssessmentCategories.District.ToString() && UserHasPermission(Permission.Icon_AssessmentSchedules_District))
									|| (_category == AssessmentCategories.Classroom.ToString() && DistrictParms.LoadDistrictParms().AssessmentSchedulerClassroomAssessments && UserHasPermission(Permission.Icon_AssessmentSchedules_Classroom))
									|| (_category == AssessmentCategories.State.ToString() && UserHasPermission(Permission.Icon_AssessmentSchedules_State));

			
            // Set the current filter visibility.
            SetFilterVisibility();

            // Set the current view.
            SetView();

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(LoadGrades));
            taskList.Add(new AsyncPageTask(LoadSubjects));
            taskList.Add(new AsyncPageTask(LoadTerms));
            taskList.Add(new AsyncPageTask(LoadTestTypes));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "ViewAssessments", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            if (!_isPostBack)
            {
                BuildGrades();
                BuildSubjects();
                BuildTerms();
                BuildTestTypes();
                BuildStatuses();
            }

            LoadAssessments();
        }
        
        protected string GetSummaryElement(object dataItem)
        {
            if (_category == "District" && !UserHasPermission(Base.Enums.Permission.Icon_Summary_AssessmentDistrict)) return "";
            if (_category == "State" && !UserHasPermission(Base.Enums.Permission.Icon_Summary_AssessmentState)) return "";
            if (_category == "Classroom" && !UserHasPermission(Base.Enums.Permission.Icon_Summary_AssessmentClassroom)) return "";

            const String enabled = @"<img id='imgSummary' alt='Summary' src='../Images/summary.png' title='Summary' style='cursor: pointer; margin-left: 4px;' onclick=""alert('Functionality is under development.');""/>";
            const String disabled = @"<img id='imgSummary' alt='Summary' src='../Images/summary_disabled.png' style='margin-left: 4px;'/>";
            Object obj = DataBinder.Eval(dataItem, "Scored");
            return (obj is Int32 && ((Int32)obj) > 0) ? enabled : disabled;
        }

        protected void lbxList_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            DataRowView row = (DataRowView)(listBoxItem).DataItem;

            Boolean showPrint = IsPrintVisible(DataIntegrity.ConvertToBool(row["HasRubrics"]), row["ContentType"].ToString() != "No Items/Content");

            // Handle print icons.
            SetControlVisibility(listBoxItem.FindControl("imgListPrint1"), showPrint);
            SetControlVisibility(listBoxItem.FindControl("imgListPrint2"), showPrint);
            SetControlVisibility(listBoxItem.FindControl("imgGraphicPrint1"), showPrint);
            SetControlVisibility(listBoxItem.FindControl("imgGraphicPrint2"), showPrint);

            String printonclick = @"viewAsssessment_printClick(" + "'" + Encryption.EncryptString(row["TestID"].ToString()) + "','" + row["TestName"].ToString() + "')";

            var link1 = (HyperLink)listBoxItem.FindControl("imgListPrint1");
            var link2 = (HyperLink)listBoxItem.FindControl("imgListPrint2");
            var graphicLink1 = (HyperLink)listBoxItem.FindControl("imgGraphicPrint1");
            var graphicLink2 = (HyperLink)listBoxItem.FindControl("imgGraphicPrint2");

            if (link1 != null) link1.NavigateUrl = "javascript:" + printonclick;
            if (link2 != null) link2.NavigateUrl = "javascript:" + printonclick;
            if (graphicLink1 != null) graphicLink1.NavigateUrl = "javascript:" + printonclick;
            if (graphicLink2 != null) graphicLink2.NavigateUrl = "javascript:" + printonclick;

            // Edit mode controls how the row is displayed. If null, it shows 'More Results...'.
            Boolean? editItemMode = null;
            if (row["Proofed"] is String) editItemMode = !DataIntegrity.ConvertToBool(row["Proofed"]);

            SetControlVisibility(listBoxItem.FindControl("graphicMore"), !editItemMode.HasValue);
            SetControlVisibility(listBoxItem.FindControl("listMore"), !editItemMode.HasValue);
            SetControlVisibility(listBoxItem.FindControl("listLine2Summary"), editItemMode.HasValue && !editItemMode.Value);
            SetControlVisibility(listBoxItem.FindControl("graphicLine2Summary"), editItemMode.HasValue && !editItemMode.Value);
            SetControlVisibility(listBoxItem.FindControl("graphicLine3Summary"), editItemMode.HasValue && !editItemMode.Value);
            SetControlVisibility(listBoxItem.FindControl("imgAddToCalendar"), _calendarIconVisible);
            SetControlVisibility(listBoxItem.FindControl("testImg"), editItemMode.HasValue);
            SetControlVisibility(listBoxItem.FindControl("imgSummary"), IsSummaryVisible());

            String moreUrl = @"'../Controls/Assessment/AssessmentSearchExpanded.aspx?category=" + _category + "'";

            if (!editItemMode.HasValue) SetOnClick((System.Web.UI.HtmlControls.HtmlAnchor)listBoxItem.FindControl("lnkListMore"), @"doOpenExpandEditRadWindow(" + moreUrl + "); return false;");
            if (!editItemMode.HasValue) SetOnClick((System.Web.UI.HtmlControls.HtmlAnchor)listBoxItem.FindControl("lnkGraphicMore"), @"doOpenExpandEditRadWindow(" + moreUrl + "); return false;");

            if (editItemMode.HasValue)
            {
                // District test links are not active on teacher page.
                Boolean isDistrictOrSchool = (_level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School);
                bool IsDistrictPortal = _category.ToLower().Trim() == "district"; 
                Boolean testNameLinkActive = isDistrictOrSchool || String.Compare(_category, "District", true) != 0;
                Object scoredObj = row["Scored"];
                Boolean testScoredLinkActive = (scoredObj is Int32 && ((Int32)scoredObj) > 0 && !isDistrictOrSchool);                
                Boolean hasPermissionActiveTestNameLnk = false;

                switch (_category.ToLower())
                {
                    case "classroom":
                        hasPermissionActiveTestNameLnk = UserHasPermission(Permission.Hyperlink_AssessmentNameClassroom);
                        break;

                    case "district":
                        hasPermissionActiveTestNameLnk = UserHasPermission(Permission.Hyperlink_AssessmentNameDistrict);
                        break;

                    case "state":
                        hasPermissionActiveTestNameLnk = UserHasPermission(Permission.Hyperlink_AssessmentNameState);
                        break;
                } 

                if (!hasPermissionActiveTestNameLnk)
                { testNameLinkActive = false; }

                if (Convert.ToBoolean(row["Targetted"]) && !isDistrictOrSchool && Convert.ToInt32(row["createdBy"].ToString()) != SessionObject.LoggedInUser.Page)
                {
                    testNameLinkActive = false;
                    hasPermissionActiveTestNameLnk = false;
                }

                // For now we have been asked to make the scored hyperlinks disabled.
                testScoredLinkActive = false;

                SetControlVisibility(listBoxItem.FindControl("lblListTestName"), !(testNameLinkActive || hasPermissionActiveTestNameLnk));
                SetControlVisibility(listBoxItem.FindControl("lnkListTestName"), testNameLinkActive || hasPermissionActiveTestNameLnk);
                SetControlVisibility(listBoxItem.FindControl("lblListTestScored"), !testScoredLinkActive && !isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("lblListTestScoredPct"), !testScoredLinkActive && isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("lnkListTestScored"), testScoredLinkActive && !isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("lnkListTestScoredPct"), testScoredLinkActive && isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("lblGraphicTestName"), !(testNameLinkActive || hasPermissionActiveTestNameLnk));
                SetControlVisibility(listBoxItem.FindControl("lnkGraphicTestName"), testNameLinkActive || hasPermissionActiveTestNameLnk);
                SetControlVisibility(listBoxItem.FindControl("lblGraphicTestScored"), !testScoredLinkActive && !isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("lblGraphicTestScoredPct"), !testScoredLinkActive && isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("lnkGraphicTestScored"), testScoredLinkActive && !isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("lnkGraphicTestScoredPct"), testScoredLinkActive && isDistrictOrSchool);
                SetControlVisibility(listBoxItem.FindControl("btnGraphicUpdate"), editItemMode.Value);
                SetControlVisibility(listBoxItem.FindControl("btnListUpdate"), editItemMode.Value);
                SetControlVisibility(listBoxItem.FindControl("listLine2Edit"), editItemMode.Value);
                SetControlVisibility(listBoxItem.FindControl("graphicLine2Edit"), editItemMode.Value);
                SetControlVisibility(listBoxItem.FindControl("graphicLine3Edit"), editItemMode.Value);

                String testLinkUrl = "~/Record/AssessmentObjects.aspx?xID=" + (String)row["TestID_Encrypted"];
                SetNavigateURL((HyperLink)listBoxItem.FindControl("lnkListTestName"), testLinkUrl);
                SetNavigateURL((HyperLink)listBoxItem.FindControl("lnkGraphicTestName"), testLinkUrl);

                HyperLink imgOnclickLink = new HyperLink();
                imgOnclickLink.NavigateUrl = "~/Record/AssessmentPage.aspx?xID=" + (string)row["TestID_Encrypted"];

                string imgOnclickLinkString = imgOnclickLink.ResolveClientUrl(imgOnclickLink.NavigateUrl);
                string assessmentTitle = "Term " + row["Term"] + " " + row["TestType"] + " - " + row["Grade"] + " Grade " + row["Subject"] + (row["Course"].ToString() == row["Subject"].ToString() ? string.Empty : " " + row["Course"]);
                string onClientClick = "var _this=this; this.disabled=true; setTimeout(function(){ _this.disabled=false; }, 500); viewAssessments_editLink_onClick('" + imgOnclickLinkString + "', '" + assessmentTitle + "');";

                var btnListEdit = (HyperLink)listBoxItem.FindControl("btnListEdit");
                
                var btnGraphicEdit = (HyperLink)listBoxItem.FindControl("btnGraphicEdit");
                var btnGraphicUpdate = (HyperLink)listBoxItem.FindControl("btnGraphicUpdate");
                var btnListUpdate = (HyperLink)listBoxItem.FindControl("btnListUpdate");

                if (btnListEdit != null)
                {
                    btnListEdit.NavigateUrl = "javascript:" + onClientClick;
                    //WR 2932: Edit Button only visible for District Assessments when user/role has "Icon_Edit_District_Assessment" permission and item is not proofed
                    btnListEdit.Visible = editItemMode.Value && (!IsDistrictPortal || (UserHasPermission(Permission.Icon_Edit_District_Assessment)));
                }

                if (btnGraphicEdit != null)
                {
                    btnGraphicEdit.NavigateUrl = "javascript:" + onClientClick;
                    //WR 2932: Edit Button only visible for District Assessments when user/role has "Icon_Edit_District_Assessment" permission and item is not proofed
                    btnGraphicEdit.Visible = editItemMode.Value && (!IsDistrictPortal || (UserHasPermission(Permission.Icon_Edit_District_Assessment)));
                }
                if (btnGraphicUpdate != null) btnGraphicUpdate.NavigateUrl = "javascript:" + onClientClick;
                if (btnListUpdate != null) btnListUpdate.NavigateUrl = "javascript:" + onClientClick;

                System.Web.UI.WebControls.Image proofedImg = (System.Web.UI.WebControls.Image)listBoxItem.FindControl("testImg");
                if (proofedImg != null) proofedImg.ImageUrl = (editItemMode.Value) ? "~/Images/editable.png" : "~/Images/proofed.png";

                // String for onclick for showing Assessment Administration popup. We pass parameters that we have.
                String classid = (_level == Base.Enums.EntityTypes.Class) ? _levelID.ToString() : String.Empty;
                string isSecureText = "false";
                String imgonclick = @"viewAsssessment_adminClick(" + row["TestID"].ToString() + "," + (String.IsNullOrEmpty(classid) ? "null" : classid) + ",'" + assessmentTitle + " - " + row["Description"].ToString().Replace("'", "") + "','" + isSecureText + "')";

                var imgListAdmin = (HyperLink)listBoxItem.FindControl("imgListAdmin");
                var imgGraphicAdmin = (HyperLink)listBoxItem.FindControl("imgGraphicAdmin");

                if (imgListAdmin != null)
                {
                    if (row["DisplayDashboard"].ToString() == "No" && !UserHasPermission(Permission.Icon_AdministrationIcon_SecurityOverride)) 
                    {
                        imgListAdmin.Attributes["style"] = "opacity:.3; filter:alpha(opacity=30); cursor:default;";
                        imgListAdmin.Attributes["onclick"] = "return false;";
                    }
                    else
                    {
                        imgListAdmin.NavigateUrl = "javascript:" + imgonclick;
                    }
                }
                if (imgGraphicAdmin != null)
                {
                    if (row["DisplayDashboard"].ToString() == "No" && !UserHasPermission(Permission.Icon_AdministrationIcon_SecurityOverride))
                    {
                        imgGraphicAdmin.Attributes["style"] = "opacity:.3; filter:alpha(opacity=30); cursor:default;";
                        imgGraphicAdmin.Attributes["onclick"] = "return false;";
                    }
                    else
                    {
                        imgGraphicAdmin.NavigateUrl = "javascript:" + imgonclick;
                    }
                }
            }
        }

        private bool IsPrintVisible(bool hasRubrics, bool isOnlineTest = true)
        {
            // First just set the print icon visibility depending on permissions.            
            Base.Enums.Permission assessmentPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_Assessment" + _category, true);
            Base.Enums.Permission answerKeyPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_AnswerKey" + _category, true);
            Base.Enums.Permission rubricsPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_Rubrics" + _category, true);

            return (UserHasPermission(assessmentPerm) || UserHasPermission(answerKeyPerm) || (UserHasPermission(rubricsPerm) && hasRubrics)) && isOnlineTest;
        }

        private bool IsSummaryVisible()
        {
            Base.Enums.Permission perm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Icon_Summary_Assessment" + _category, true);
            return UserHasPermission(perm);
        }

        private void SetControlVisibility(Control ctrl, bool isVisible)
        {
            if (ctrl == null) return;
            ctrl.Visible = isVisible;
        }

        private void SetNavigateURL(HyperLink lnk, string value)
        {
            if (lnk == null) return;
            lnk.NavigateUrl = value;
        }

        private void SetOnClientClick(ImageButton btn, string value)
        {
            if (btn == null) return;
            btn.OnClientClick = value;
        }

        private void SetOnClick(IAttributeAccessor ctrl, string value)
        {
            if (ctrl == null) return;
            ctrl.SetAttribute("onclick", value);
        }

        protected void BuildGrades()
        {
            if (_gradeVisible)
            {
                CourseList courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
                DataTable dtGrade = new DataTable();

                dtGrade.Columns.Add("Grade", typeof(String));
                dtGrade.Columns.Add("CmbText", typeof(String));

                if (courseList != null)
                {
                    foreach (var grade in courseList.GetGradeList())
                    {
                        DataRow newGradeRow = dtGrade.NewRow();
                        newGradeRow["Grade"] = grade.DisplayText;
                        newGradeRow["CmbText"] = grade.DisplayText;
                        dtGrade.Rows.Add(newGradeRow);
                    }
                }

                DataRow newRow = dtGrade.NewRow();
                newRow["Grade"] = "All";
                newRow["CmbText"] = "Grade";
                dtGrade.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbGrade.DataTextField = "CmbText";
                cmbGrade.DataValueField = "Grade";
                cmbGrade.DataSource = dtGrade;
                cmbGrade.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbGrade.Items.FindItemByValue((String)ViewState[_gradeFilterKey], true);
                Int32 selIdx = cmbGrade.Items.IndexOf(item);
                cmbGrade.SelectedIndex = selIdx;
            }
        }

        protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_gradeFilterKey] = e.Value;

            SetFilterVisibility();
            LoadAssessments();

            cmbGrade.Text = e.Text;
        }


        protected void BuildSubjects()
        {
            if (_subjectVisible)
            {
                // Now load the filter button tables and databind.
                // The existing columns are 'Subject' and 'Abbreviation'.
                // Add a column for 'DropdownText'.
                dtSubject.Columns.Add("DropdownText");
                foreach (DataRow row in dtSubject.Rows)
                {
                    row["DropdownText"] = (String)row["Subject"];
                    if ((String)row["Subject"] != (String)row["Abbreviation"])
                        row["DropdownText"] += " (" + (String)row["Abbreviation"] + ")";
                }

                // We will rename Abbreviation to CmbText.
                dtSubject.Columns["Abbreviation"].ColumnName = "CmbText";

                DataRow newRow = dtSubject.NewRow();
                newRow["Subject"] = "All";
                newRow["DropdownText"] = "All";
                newRow["CmbText"] = "Subject";
                dtSubject.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbSubject.DataTextField = "CmbText";
                cmbSubject.DataValueField = "Subject";
                cmbSubject.DataSource = dtSubject;
                cmbSubject.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbSubject.Items.FindItemByValue((String)ViewState[_subjectFilterKey], true);
                Int32 selIdx = cmbSubject.Items.IndexOf(item);
                cmbSubject.SelectedIndex = selIdx;
            }
        }

        protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_subjectFilterKey] = e.Value;

            SetFilterVisibility();
            LoadAssessments();
            cmbSubject.Text = e.Text;
        }

        protected void BuildTerms()
        {
            if (_termVisible)
            {
                // The only existing column is 'Term'. We must add a column for 'CmbText'.
                dtTerm.Columns.Add("CmbText", typeof(String));
                foreach (DataRow row in dtTerm.Rows)
                    row["CmbText"] = "Term " + row["Term"];
                DataRow newRow = dtTerm.NewRow();
                newRow["Term"] = "All";
                newRow["CmbText"] = "Term";
                dtTerm.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbTerm.DataTextField = "CmbText";
                cmbTerm.DataValueField = "Term";
                cmbTerm.DataSource = dtTerm;
                cmbTerm.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbTerm.Items.FindItemByValue((String)ViewState[_termFilterKey], true);
                Int32 selIdx = cmbTerm.Items.IndexOf(item);
                cmbTerm.SelectedIndex = selIdx;
            }
        }

        protected void cmbTerm_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_termFilterKey] = e.Value;

            cmbTerm.Text = e.Text;
            RadComboBoxItem item = cmbTerm.Items.FindItemByText(e.Text);
            Int32 selIdx = cmbTerm.Items.IndexOf(item);
            cmbTerm.SelectedIndex = selIdx;

            SetFilterVisibility();
            LoadAssessments();
        }

        protected void BuildTestTypes()
        {
            if (_testTypeVisible)
            {
                // The existing columns are 'Type' and 'Abbreviation'.
                // Add a column for 'DropdownText'.
                dtTestType.Columns.Add("DropdownText");
                foreach (DataRow row in dtTestType.Rows)
                {
                    row["DropdownText"] = (String)row["TestType"];
                    if ((String)row["TestType"] != (String)row["Abbreviation"])
                        row["DropdownText"] += " (" + (String)row["Abbreviation"] + ")";
                }
                // We will rename Abbreviation to CmbText.
                dtTestType.Columns["Abbreviation"].ColumnName = "CmbText";
                DataRow newRow = dtTestType.NewRow();
                newRow["TestType"] = "All";
                newRow["DropdownText"] = "All";
                newRow["CmbText"] = "Type";
                dtTestType.Rows.InsertAt(newRow, 0);

                // Data bind the combo box.
                cmbTestType.DataTextField = "CmbText";
                cmbTestType.DataValueField = "TestType";
                cmbTestType.DataSource = dtTestType;
                cmbTestType.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbTestType.Items.FindItemByValue((String)ViewState[_testTypeFilterKey], true);
                Int32 selIdx = cmbTestType.Items.IndexOf(item);
                cmbTestType.SelectedIndex = selIdx;
            }
        }

        protected void cmbTestType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_testTypeFilterKey] = e.Value;

            SetFilterVisibility();
            LoadAssessments();
            cmbTestType.Text = e.Text;
            RadComboBoxItem item = cmbTestType.Items.FindItemByText(e.Text);
            Int32 selIdx = cmbTestType.Items.IndexOf(item);
            cmbTestType.SelectedIndex = selIdx;
        }

        protected void BuildStatuses()
        {
            if (_statusVisible)
            {
                bool isStateSystem = DistrictParms.LoadDistrictParms().isStateSystem;

                DataTable dtStatus = new DataTable();
                dtStatus.Columns.Add("Status", typeof(String));
                dtStatus.Columns.Add("CmbText", typeof(String));
                dtStatus.Columns.Add("DropdownText", typeof(String));
                DataRow newRow = dtStatus.NewRow();
                
                newRow["Status"] = "All";
                newRow["CmbText"] = "Status";
                newRow["DropdownText"] = "All";
                dtStatus.Rows.Add(newRow);
                
                newRow = dtStatus.NewRow();
                newRow["Status"] = "Proofed";
                newRow["CmbText"] = "Proofed";
                newRow["DropdownText"] = "Proofed";
                dtStatus.Rows.Add(newRow);
                
                newRow = dtStatus.NewRow();
                newRow["Status"] = "Unproofed";
                newRow["CmbText"] = "Unproofed";
                newRow["DropdownText"] = "Unproofed";
                dtStatus.Rows.Add(newRow);

                // don't add "All" or "Unproofed" selections in this case (basically force to proofed)
                if (_category.ToLower() == "state" && !isStateSystem)
                {                    
                    DataRow[] rows = dtStatus.Select("Status = 'All' OR Status = 'Unproofed'");
                    foreach (DataRow r in rows)
                    {
                        r.Delete();
                    }
                }

                // Data bind the combo box.
                cmbStatus.DataTextField = "CmbText";
                cmbStatus.DataValueField = "Status";
                cmbStatus.DataSource = dtStatus;
                cmbStatus.DataBind();

                // Initialize the current selection.
                RadComboBoxItem item = cmbStatus.Items.FindItemByValue((String)ViewState[_statusFilterKey], true);
                Int32 selIdx = cmbStatus.Items.IndexOf(item);
                cmbStatus.SelectedIndex = selIdx;
            }
        }

        protected void cmbStatus_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_statusFilterKey] = e.Value;

            SetFilterVisibility();
            LoadAssessments();

            cmbStatus.Text = e.Text;
            RadComboBoxItem item = cmbStatus.Items.FindItemByText(e.Text);
            Int32 selIdx = cmbStatus.Items.IndexOf(item);
            cmbStatus.SelectedIndex = selIdx;
        }

        /// <summary>
        /// Set the current filter visibility.
        /// </summary>
        protected void SetFilterVisibility()
        {
            // Subject filter is visible for these conditions:
            //	Teacher view with no class selected (1st row on teacher screen) and district category.
            //	Teacher view with no class selected (1st row on teacher screen) and classrooom category.
            //	District view.
            //	School view.
            _subjectVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School || _level == EntityTypes.Teacher;
            _termVisible = (!_calendarIconVisible); //Hide if calendar icon is visible, otherwise show.
            // Test type is not visible on district or school.
            _testTypeVisible = !(_level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School);
            // Proofed status is visible on classroom assessments and on District and School pages.
            _statusVisible = (_category != "District" || UserHasPermission(Permission.Edit_AssessmentDistrict_Unproofed));
            // Grade is visible only on district and school.
            _gradeVisible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;


            cmbGrade.Visible = _gradeVisible;
            cmbSubject.Visible = _subjectVisible;
            cmbTerm.Visible = _termVisible;
            cmbTestType.Visible = _testTypeVisible;
            cmbStatus.Visible = _statusVisible;
        }

        /// <summary>
        /// Set the current view to list or graphical.
        /// This must be done so that postbacks do not change the current view that was set in javascript.
        /// </summary>
        protected void SetView()
        {
            String view = currView.Value;
            divListView.Style["display"] = (view == "listView") ? "" : "none";
            divGraphicalView.Style["display"] = (view == "listView") ? "none" : "";
        }

        /// <summary>
        /// Handles conditional logic for setting the filter for the status dropdown list.
        /// </summary>
        private void SetViewstateForStatusFilter()
        {
            string category = _category.ToLower();
            if (category == "district" && !UserHasPermission(Permission.Edit_AssessmentDistrict_Unproofed))
            {
                ViewState.Add(_statusFilterKey, "Proofed");
            }
            else if (category == "state" && !(DistrictParms.LoadDistrictParms().isStateSystem))
            {
                ViewState.Add(_statusFilterKey, "Proofed");
            }
            else
            {
                ViewState.Add(_statusFilterKey, "All");
            }
        }
    }
}
