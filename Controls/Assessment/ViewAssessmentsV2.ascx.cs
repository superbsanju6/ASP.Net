using System;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Enums.AssessmentScheduling;
using Thinkgate.Base.Classes.Data;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Standpoint.Core.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment
{
    public partial class ViewAssessmentsV2 : TileControlBase
    {
        protected string _category;
        protected EntityTypes _level;
        protected int _levelID;
        protected int _schoolID;
        protected bool _calendarIconVisible = false;
        public static string _loggedOnUserRoleName = string.Empty;
        public static int _schoolId = 0;
        public DataTable dtPrintDateAllTest_district;
        public DataTable dtPrintDateAllTest_school;
        public List<TestType> lstTestType;
        protected Dictionary<string, bool> dictionaryItem;
        protected int count = 0;
        bool isSecuredFlag;
        public List<int> ListOfTests
        {
            get { return (List<int>)Session["ListOfTests_"]; }
            set { Session["ListOfTests_"] = value; }
        }

        bool _isPostBack = false;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            _category = (String)Tile.TileParms.GetParm("category");
            _level = (EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = (Int32)Tile.TileParms.GetParm("levelID");

            //PLH - 01/15/2013 - Fix for forcing Classroom assessments to drill down to selected school in Classes folder only. 
            if (Tile.TileParms.Parms.ContainsKey("schoolID"))
            {
                _schoolID = (int)Tile.TileParms.GetParm("schoolID");
            }

            if (Tile.TileParms.GetParm("showCalendarIcon") != null)
            {
                _calendarIconVisible = DataIntegrity.ConvertToBool(Tile.TileParms.GetParm("showCalendarIcon"));
            }
            dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(_category);
            isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            SetFilterVisibility();
            SetSecureTabVisibility();

            BtnAdd.Attributes["testCategory"] = _category;
            BtnTestEvents.Attributes["testCategory"] = _category;
            BtnTestEvents.Attributes["level"] = _level.ToString();
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
                case EntityTypes.State:
                    BtnAdd.Attributes["level"] = "State";
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
                case EntityTypes.State:
                    btnScheduler.Attributes["xID"] = Encryption.EncryptString(AssessmentScheduleLevels.State.ToString());
                    break;
            }


            btnScheduler.Visible = (_category == AssessmentCategories.District.ToString() && UserHasPermission(Permission.Icon_AssessmentSchedules_District))
                                    || (_category == AssessmentCategories.Classroom.ToString() && DistrictParms.LoadDistrictParms().AssessmentSchedulerClassroomAssessments && UserHasPermission(Permission.Icon_AssessmentSchedules_Classroom))
                                    || (_category == AssessmentCategories.State.ToString() && UserHasPermission(Permission.Icon_AssessmentSchedules_State));


            //Icon_TestEvents_State permission should only work for state tile 
            if (_category == AssessmentCategories.State.ToString())
            {
                BtnTestEvents.Visible = UserHasPermission(Permission.Icon_TestEvents_State);
            }


            if (!_isPostBack)
            {
                LoadDropDownGrades();
                LoadDropDownSubjects();
                LoadDropDownTerms();
                LoadDropDownStatuses();

                // get role name from the session object
                Thinkgate.Classes.SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                _loggedOnUserRoleName = sessionObject.LoggedInUser.Roles[0].RoleName;
                _schoolId = sessionObject.LoggedInUser.School;
                // LoadAssessments(false);
            }

            bool isTabSecure = SecureFormativetabStrip.SelectedIndex == 1;
            LoadDropDownTestTypes(isTabSecure);
            LoadAssessments(isTabSecure);

        }

        protected void SetFilterVisibility()
        {
            cmbGrade.Visible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School;
            cmbSubject.Visible = _level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School || _level == EntityTypes.Teacher;
            cmbTerm.Visible = (!_calendarIconVisible); //Hide if calendar icon is visible, otherwise show.
            cmbTestType.Visible = !(_level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School);
            cmbStatus.Visible = (_category != "District" || UserHasPermission(Permission.Edit_AssessmentDistrict_Unproofed));
        }


        protected void performanceLevelSetIcon_Click(object sender, EventArgs e)
        {



        }


        private void LoadDropDownGrades()
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
        }

        private void LoadDropDownSubjects()
        {
            DataTable dtSubject = Thinkgate.Base.Classes.Assessment.LoadMockSubjects(SessionObject.LoggedInUser.Page, SessionObject.GlobalInputs);

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
        }

        private void LoadDropDownTerms()
        {
            DataTable dtTerm = Thinkgate.Base.Classes.Assessment.LoadTerms(SessionObject.LoggedInUser.Page, SessionObject.GlobalInputs);
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
        }

        private void LoadDropDownTestTypes(bool isSecure)
        {

            lstTestType = TestTypes.GetTestTypesForDropDownsOnTile(SessionObject.LoggedInUser.Page, _category);

            // Data bind the combo box.
            if (!_isPostBack)
            {
                cmbTestType.DataTextField = "Abbreviation";
                cmbTestType.DataValueField = "Type";
                cmbTestType.DataSource = lstTestType.Where(x => x.Secure == isSecure || x.Type == "All");
                cmbTestType.DataBind();
            }

        }

        private void LoadDropDownStatuses()
        {
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

            // Note: This change is specific to sites like NCCTE that use a "State" level:
            //  If the "isStateSystem" parm is false, and the current "Category" is State (an LEA), apply the following condition.
            if (_category.ToLower() == "state" && !(DistrictParms.LoadDistrictParms().isStateSystem))
            {
                // force available selections to "proofed" only.
                // (remove "All" and "Unproofed" from available selections, leaving only "proofed".)
                DataRow[] rows = dtStatus.Select("Status = 'All' OR Status = 'Unproofed'");
                foreach (DataRow r in rows)
                    r.Delete();
            }

            // Data bind the combo box.
            cmbStatus.DataTextField = "CmbText";
            cmbStatus.DataValueField = "Status";
            cmbStatus.DataSource = dtStatus;
            cmbStatus.DataBind();
        }

        private void LoadAssessments(bool IsSecure)
        {
            string selectedGrade = cmbGrade.SelectedItem.Value;
            string selectedSubject = cmbSubject.SelectedItem.Value;
            string selectedTerm = cmbTerm.SelectedItem.Value;
            string selectedTestType = cmbTestType.SelectedItem.Value;
            string selectedStatus = (_category == "District" && !UserHasPermission(Permission.Edit_AssessmentDistrict_Unproofed)) ? "Proofed" : cmbStatus.SelectedItem.Value;

            Thinkgate.Base.Classes.CourseList courses = Thinkgate.Base.Classes.CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);

            //PLH - quick hack to make Class Assessment Tiles populate correctly. TODO: Change to better method
            if (_level == EntityTypes.Class)
            {
                Base.Classes.Class selectedClass = Base.Classes.Class.GetClassByID(_levelID);
                courses = CourseMasterList.GetCurrCourseForClass(selectedClass);
            }
            else
            {
                string gradeParm = selectedGrade == "All" ? null : selectedGrade;
                string subjectParm = selectedSubject == "All" ? null : selectedSubject;
                courses = courses.FilterByGradeAndSubject(gradeParm, subjectParm);
            }

            List<int> courseIDs = new List<int>();
            foreach (Base.Classes.Course c in courses)
            {
                courseIDs.Add(c.ID);
            }

            List<int> schoolIDs = new List<int>();
            if (_schoolID != 0)
            {
                schoolIDs.Add(_schoolID);
            }
            else
            {
                schoolIDs = (from s in SessionObject.LoggedInUser.Schools select s.Id).ToList();
            }

            List<string> roleNames = (from r in SessionObject.LoggedInUser.Roles select r.RoleName).ToList();

            DataTable dtAssessment = Base.Classes.Assessment.LoadAssessments(_category,
                                                                            _level.ToString(),
                                                                            _levelID,
                                                                            courseIDs.ToSql(),
                                                                            selectedTerm,
                                                                            selectedTestType,
                                                                            selectedStatus,
                                                                            schoolIDs.ToSql(),
                                                                            roleNames.ToSql(),
                                                                            UserHasPermission(Permission.User_Cross_Schools),
                                                                            SessionObject.GlobalInputs,
                                                                            false, IsSecure);
            if (dtAssessment.Rows.Count > 0)
            {
                dtAssessment = Cryptography.EncryptDataTableColumn(dtAssessment, "TestID", "TestID_Encrypted",
                    SessionObject.LoggedInUser.CipherKey);
                ListOfTests = new List<int>();
                foreach (DataRow row in dtAssessment.Rows)
                {
                    ListOfTests.Add(DataIntegrity.ConvertToInt(row["TestID"]));
                }
            }
            dtPrintDateAllTest_district = Thinkgate.Base.Classes.Assessment.GetPrintSecurity_Secutity_Status_AllTest("Assessment", "District", ListOfTests);
            dtPrintDateAllTest_school = Thinkgate.Base.Classes.Assessment.GetPrintSecurity_Secutity_Status_AllTest("school", _schoolId.ToString(), ListOfTests);
            foreach (DataRow dr in dtAssessment.Rows)
            {
                if (dr["Description"].ToString().Length > 36)
                {
                    dr["Description"] = dr["Description"].ToString().Remove(35, dr["Description"].ToString().Length - 36);
                }
            }


            rlbSecure.Visible = dtAssessment.Rows.Count > 0;

            if (!IsSecure)
            {
                lbx.Visible = dtAssessment.Rows.Count > 0;
                pnlNoResults.Visible = dtAssessment.Rows.Count == 0;
                if (dtAssessment.Rows.Count > 0)
                {
                    lbx.DataSource = dtAssessment;
                    lbx.DataBind();

                }

            }
            else
            {

                rlbSecure.Visible = dtAssessment.Rows.Count > 0;
                pnlNoResultsSecure.Visible = dtAssessment.Rows.Count == 0;
                if (dtAssessment.Rows.Count > 0)
                {
                    rlbSecure.DataSource = dtAssessment;
                    rlbSecure.DataBind();

                }

            }
        }

        protected void SetSecureTabVisibility()
        {
            if (isSecuredFlag && UserHasPermission(Permission.Access_SecureTesting))
            {
                SecureFormativetabStrip.Visible = true;
                Session["IsSecure"] = true;
            }
            else
            {
                SecureFormativetabStrip.Visible = false;
                Session["IsSecure"] = false;
            }

        }

        protected void lbxList_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            DataRowView row = (DataRowView)(listBoxItem).DataItem;

            // 6158:  Hide assessment in Teacher and School Portal if assessment security is inactive and does not have a date range
            // if TestEvents_DisplayInactiveDisctrictAssessments parm is turned on then no need to worry about
            // hiding assessments of inactive security status
            // if parm is turned off then check the security status and date range of the assessment
            IEnumerable<DataRow> tmpRow_distict = (from DataRow row1 in dtPrintDateAllTest_district.Rows
                                                   where row1["TestID"].ToString() == row["TestID"].ToString()
                                                   select row1);

            IEnumerable<DataRow> tmpRow_school = (from DataRow row1 in dtPrintDateAllTest_school.Rows
                                                  where row1["TestID"].ToString() == row["TestID"].ToString()
                                                  select row1);
            if (DistrictParms.LoadDistrictParms().TestEvents_DisplayInactiveDistrictAssessments.Trim().ToLower() == "no")
            {
                DataTable dtPrintEndDate = new DataTable();
                // For school and teacher portal
                if (_level == EntityTypes.Teacher || _level == EntityTypes.School || (_level == EntityTypes.Class && (SessionObject.CurrentPortal == EntityTypes.Teacher || SessionObject.CurrentPortal == EntityTypes.School)))
                {
                    // scheduler level is school for teacher and school scheduler
                    if (tmpRow_school.Any())
                    {
                        dtPrintEndDate = tmpRow_school.CopyToDataTable<DataRow>();
                    }
                    // At this point, user has not set the security at the school level, now check the security status at assessment level
                    if (tmpRow_distict.Any())
                    {
                        if (dtPrintEndDate.Rows.Count == 0)
                        {
                            dtPrintEndDate = tmpRow_distict.CopyToDataTable<DataRow>();
                        }
                    }
                    //}
                    //else
                    //{
                    //    // scheduler level is Assessment for district role
                    //    dtPrintEndDate = Thinkgate.Base.Classes.Assessment.GetSecurityStatus("Assessment", "District", Convert.ToInt32(row["TestID"].ToString()));
                    //}

                    if (dtPrintEndDate.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtPrintEndDate.Rows)
                        {
                            // If admin security is inactive and no date range
                            if (dr["admin_lock"].ToString() == "True" && (dr["admin_begin"].ToString() == string.Empty) && (dr["admin_end"].ToString() == string.Empty))
                                lbx.Items.Remove(listBoxItem);
                        }
                    }

                }
            }
            // For users with the 'SLOTeach' role, only show Unproofed Districts that were created by the user.
            var sloTeacher = SessionObject.LoggedInUser.Roles.Find(x => x.RoleName == "SLOTeach");
            if ((_category.ToUpper() == "DISTRICT") && (sloTeacher != null) && row["Proofed"].ToString() == "No")
            {
                if (Convert.ToInt32(row["CreatedBy"].ToString()) != SessionObject.LoggedInUser.Page)
                {
                    lbx.Items.Remove(listBoxItem);
                }
            }

            // 6362: the print icon for District Assessments( and State Assessments - NCCTE Only) 
            // should be controlled by the print window security settings.
            bool bPrintFlag = true;

            DataTable dtPrintDate = new DataTable();
            // print window is derived for school and teacher portal from district level 
            if (tmpRow_distict.Any())
            {
                dtPrintDate = tmpRow_distict.CopyToDataTable<DataRow>();
            }
            DateTime todayDate = DateTime.Today.Date;

            if (dtPrintDate.Rows.Count > 0 && !UserHasPermission(Permission.Icon_PrintIcon_SecurityOverride))
            {
                foreach (DataRow dr in dtPrintDate.Rows)
                {
                    // If print security status is inactive then print icon should be disbaled
                    if (dr["print_lock"].ToString() == "True")
                        bPrintFlag = false;
                    else
                    {
                        if (dr["print_begin"].ToString() != string.Empty)
                        {
                            /// date range 3/21/14   thru  3/25/14
                            /// today = 3/24/14
                            /// today >= 3/21/14 and today <= 3/25/14
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

            HyperLink hlkTestname = (HyperLink)listBoxItem.FindControl("lnkTestName");
            hlkTestname.Text = row["TestName"].ToString();

            Label lblDesc = (Label)listBoxItem.FindControl("lblDesc");
            lblDesc.Text = row["Description"].ToString();

            HtmlGenericControl spnNumItems = (HtmlGenericControl)listBoxItem.FindControl("spnNumItems");
            spnNumItems.InnerText = row["NumItems"].ToString() + " Items";

            Panel pnlLastEdit = (Panel)listBoxItem.FindControl("graphicLine3Edit");
            Image imgProofed = (Image)listBoxItem.FindControl("testImg");

            HyperLink hlkEdit = (HyperLink)listBoxItem.FindControl("hlkEdit");
            // TFS: 6703
            //HyperLink hlkAdmin = (HyperLink)listBoxItem.FindControl("hlkAdmin");

            System.Web.UI.HtmlControls.HtmlImage hlkAdmin;

            hlkAdmin = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("hlkAdmin");

            HyperLink hlkPrint = (HyperLink)listBoxItem.FindControl("hlkPrint");
            //TFS: 6362
            if (bPrintFlag)
            {
                hlkPrint.Visible = IsPrintVisible(DataIntegrity.ConvertToBool(row["HasRubrics"]), row["ContentType"].ToString() != "No Items/Content");
                hlkPrint.NavigateUrl = "javascript:" + @"viewAsssessment_printClick(" + "'" + Encryption.EncryptString(row["TestID"].ToString()) + "','" + row["TestName"].ToString() + "')";
            }
            else
            {
                hlkPrint.ImageUrl = "~/Images/Printer_Disabled.png";
                hlkPrint.Attributes["style"] = "cursor:default;";
                hlkPrint.Attributes["onclick"] = "return false;";
            }
            HtmlGenericControl spnLastEdit = (HtmlGenericControl)listBoxItem.FindControl("spnLastEdit");
            spnLastEdit.InnerText = "Last Edited: " + ((DateTime)row["DateUpdated"]).ToShortDateString();

            Boolean? editItemMode = null;
            if (row["Proofed"] is String) editItemMode = !DataIntegrity.ConvertToBool(row["Proofed"]);

            if (editItemMode.HasValue)
            {
                // District test links are not active on teacher page.
                bool IsDistrictOrSchool = (_level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School);
                bool IsDistrictPortal = _category.ToLower().Trim() == "district";
                bool IsTargetted = Convert.ToBoolean(row["Targetted"]);
                int CreatedByPage = string.IsNullOrEmpty(row["createdBy"].ToString()) ? 0 : Convert.ToInt32(row["createdBy"].ToString());

                bool HasHyperlinkPermission = false;
                bool HasAdminIconPermission = false;

                switch (_category.ToLower())
                {
                    case "classroom":
                        HasHyperlinkPermission = UserHasPermission(Permission.Hyperlink_AssessmentNameClassroom);
                        HasAdminIconPermission = true;
                        break;

                    case "district":
                        HasHyperlinkPermission = UserHasPermission(Permission.Hyperlink_AssessmentNameDistrict);
                        HasAdminIconPermission = true;
                        break;

                    case "state":
                        HasHyperlinkPermission = UserHasPermission(Permission.Hyperlink_AssessmentNameState);
                        HasAdminIconPermission = UserHasPermission(Permission.Icon_AdministrationState);
                        break;
                }


                bool IsTestNameLinkActive = (IsDistrictOrSchool || IsDistrictPortal) && HasHyperlinkPermission;
                bool isSloTeacherAndNonSloAssessment = sloTeacher != null && (row["ItemClassId"].ToString() != "2") && (_category.ToUpper() == "DISTRICT");

                //determine if this is an SLOTeach user if the assessment is for an SLO CurrCourse

                if (IsTargetted && !IsDistrictOrSchool && CreatedByPage != SessionObject.LoggedInUser.Page)
                {
                    IsTestNameLinkActive = false;
                    HasHyperlinkPermission = false;
                }

                imgProofed.ImageUrl = (editItemMode.Value) ? "~/Images/editable.png" : "~/Images/proofed.png";
                hlkTestname.Enabled = ((IsTestNameLinkActive || HasHyperlinkPermission) && !isSloTeacherAndNonSloAssessment);
                hlkTestname.NavigateUrl = "~/Record/AssessmentObjects.aspx?xID=" + (String)row["TestID_Encrypted"];


                spnNumItems.Visible = editItemMode.Value;

                //WR 2932: Edit Button only visible for District Assessments when user/role has "Icon_Edit_District_Assessment" permission and item is not proofed
                hlkEdit.Visible = editItemMode.Value && (!IsDistrictPortal || (UserHasPermission(Permission.Icon_Edit_District_Assessment)));


                //MF 4022: Admin icon visible only when user/role has "Icon_AdministrationState" permission and item is proofed.
                hlkAdmin.Visible = editItemMode.Value == true ? false : HasAdminIconPermission;
                pnlLastEdit.Visible = editItemMode.Value;

                HyperLink imgOnclickLink = new HyperLink();
                imgOnclickLink.NavigateUrl = "~/Record/AssessmentPage.aspx?xID=" + (string)row["TestID_Encrypted"];

                string imgOnclickLinkString = imgOnclickLink.ResolveClientUrl(imgOnclickLink.NavigateUrl);
                string SecureText = "Term ";
                string isSecure = "false";
                if (SecureFormativetabStrip.SelectedIndex == 1)
                {
                    //SecureText = "[SECURE] Term ";
                    isSecure = "true";
                }
                string assessmentTitle = SecureText + row["Term"] + " " + row["TestType"] + " - " + row["Grade"] + " Grade " + row["Subject"] + (row["Course"].ToString() == row["Subject"].ToString() ? string.Empty : " " + row["Course"]);


                string onClientClick = "var _this=this; this.disabled=true; setTimeout(function(){ _this.disabled=false; }, 500); viewAssessments_editLink_onClick('" + imgOnclickLinkString + "', '" + assessmentTitle + "');";

                if (hlkEdit != null) hlkEdit.NavigateUrl = "javascript:" + onClientClick;

                // String for onclick for showing Assessment Administration popup. We pass parameters that we have.
                String classid = (_level == Base.Enums.EntityTypes.Class) ? _levelID.ToString() : String.Empty;
                String imgonclick = @"viewAsssessment_adminClick(" + row["TestID"].ToString() + "," + (String.IsNullOrEmpty(classid) ? "null" : classid) + ",'" + assessmentTitle + " - " + row["Description"].ToString().Replace("'", "") + "','" + _category.ToString() + "','" + _level.ToString() + "','" + false + "','" + isSecure + "')";

                if (hlkAdmin != null)
                {
                    if (row["DisplayDashboard"].ToString() == "No" && !UserHasPermission(Permission.Icon_AdministrationIcon_SecurityOverride))
                    {
                        hlkAdmin.Attributes["style"] = "opacity:.3; filter:alpha(opacity=30); cursor:default;";
                        hlkAdmin.Attributes["onclick"] = "return false;";
                    }
                    else
                    {
                        // TFS: 6703
                        //hlkAdmin.NavigateUrl = "javascript:" + imgonclick;
                        hlkAdmin.Attributes["onclick"] = "javascript:" + imgonclick;
                    }
                }
            }
        }

        protected void rlbSecureList_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            DataRowView row = (DataRowView)(listBoxItem).DataItem;

            // 6158:  Hide assessment in Teacher and School Portal if assessment security is inactive and does not have a date range
            // if TestEvents_DisplayInactiveDisctrictAssessments parm is turned on then no need to worry about
            // hiding assessments of inactive security status
            // if parm is turned off then check the security status and date range of the assessment
            IEnumerable<DataRow> tmpRow_distict = (from DataRow row1 in dtPrintDateAllTest_district.Rows
                                                   where row1["TestID"].ToString() == row["TestID"].ToString()
                                                   select row1);

            IEnumerable<DataRow> tmpRow_school = (from DataRow row1 in dtPrintDateAllTest_school.Rows
                                                  where row1["TestID"].ToString() == row["TestID"].ToString()
                                                  select row1);
            if (DistrictParms.LoadDistrictParms().TestEvents_DisplayInactiveDistrictAssessments.Trim().ToLower() == "no")
            {
                DataTable dtPrintEndDate = new DataTable();
                // For school and teacher portal
                if (_level == EntityTypes.Teacher || _level == EntityTypes.School || (_level == EntityTypes.Class && (SessionObject.CurrentPortal == EntityTypes.Teacher || SessionObject.CurrentPortal == EntityTypes.School)))
                {
                    // scheduler level is school for teacher and school scheduler
                    if (tmpRow_school.Any())
                    {
                        dtPrintEndDate = tmpRow_school.CopyToDataTable<DataRow>();
                    }
                    // At this point, user has not set the security at the school level, now check the security status at assessment level
                    if (tmpRow_distict.Any())
                    {
                        if (dtPrintEndDate.Rows.Count == 0)
                        {
                            dtPrintEndDate = tmpRow_distict.CopyToDataTable<DataRow>();
                        }
                    }
                    //}
                    //else
                    //{
                    //    // scheduler level is Assessment for district role
                    //    dtPrintEndDate = Thinkgate.Base.Classes.Assessment.GetSecurityStatus("Assessment", "District", Convert.ToInt32(row["TestID"].ToString()));
                    //}

                    if (dtPrintEndDate.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtPrintEndDate.Rows)
                        {
                            // If admin security is inactive and no date range
                            if (dr["admin_lock"].ToString() == "True" && (dr["admin_begin"].ToString() == string.Empty) && (dr["admin_end"].ToString() == string.Empty))
                                rlbSecure.Items.Remove(listBoxItem);
                        }
                    }

                }
            }
            // For users with the 'SLOTeach' role, only show Unproofed Districts that were created by the user.
            var sloTeacher = SessionObject.LoggedInUser.Roles.Find(x => x.RoleName == "SLOTeach");
            if ((_category.ToUpper() == "DISTRICT") && (sloTeacher != null) && row["Proofed"].ToString() == "No")
            {
                if (Convert.ToInt32(row["CreatedBy"].ToString()) != SessionObject.LoggedInUser.Page)
                {
                    rlbSecure.Items.Remove(listBoxItem);
                }
            }

            // 6362: the print icon for District Assessments( and State Assessments - NCCTE Only) 
            // should be controlled by the print window security settings.
            bool bPrintFlag = true;

            DataTable dtPrintDate = new DataTable();
            // print window is derived for school and teacher portal from district level 
            if (tmpRow_distict.Any())
            {
                dtPrintDate = tmpRow_distict.CopyToDataTable<DataRow>();
            }
            DateTime todayDate = DateTime.Today.Date;

            if (dtPrintDate.Rows.Count > 0 && !UserHasPermission(Permission.Icon_PrintIcon_SecurityOverride))
            {
                foreach (DataRow dr in dtPrintDate.Rows)
                {
                    // If print security status is inactive then print icon should be disbaled
                    if (dr["print_lock"].ToString() == "True")
                        bPrintFlag = false;
                    else
                    {
                        if (dr["print_begin"].ToString() != string.Empty)
                        {
                            /// date range 3/21/14   thru  3/25/14
                            /// today = 3/24/14
                            /// today >= 3/21/14 and today <= 3/25/14
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

            HyperLink hlkTestname = (HyperLink)listBoxItem.FindControl("lnkTestName");
            hlkTestname.Text = row["TestName"].ToString();

            Label lblDesc = (Label)listBoxItem.FindControl("lblDesc");
            lblDesc.Text = row["Description"].ToString();

            HtmlGenericControl spnNumItems = (HtmlGenericControl)listBoxItem.FindControl("spnNumItemsSecure");
            spnNumItems.InnerText = row["NumItems"].ToString() + " Items";

            Panel pnlLastEdit = (Panel)listBoxItem.FindControl("graphicLine3EditSecure");
            Image imgProofed = (Image)listBoxItem.FindControl("testImg");

            HyperLink hlkEdit = (HyperLink)listBoxItem.FindControl("hlkEditSecure");
            // TFS: 6703
            //HyperLink hlkAdmin = (HyperLink)listBoxItem.FindControl("hlkAdmin");

            System.Web.UI.HtmlControls.HtmlImage hlkAdmin;

            hlkAdmin = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("hlkAdminSecure");

            HyperLink hlkPrint = (HyperLink)listBoxItem.FindControl("hlkPrintSecure");
            //TFS: 6362
            if (bPrintFlag)
            {
                hlkPrint.Visible = IsPrintVisible(DataIntegrity.ConvertToBool(row["HasRubrics"]), row["ContentType"].ToString() != "No Items/Content");
                hlkPrint.NavigateUrl = "javascript:" + @"viewAsssessment_printClick(" + "'" + Encryption.EncryptString(row["TestID"].ToString()) + "','" + row["TestName"].ToString() + "')";
            }
            else
            {
                hlkPrint.ImageUrl = "~/Images/Printer_Disabled.png";
                hlkPrint.Attributes["style"] = "cursor:default;";
                hlkPrint.Attributes["onclick"] = "return false;";
            }
            HtmlGenericControl spnLastEdit = (HtmlGenericControl)listBoxItem.FindControl("spnLastEditSecure");
            spnLastEdit.InnerText = "Last Edited: " + ((DateTime)row["DateUpdated"]).ToShortDateString();

            Boolean? editItemMode = null;
            if (row["Proofed"] is String) editItemMode = !DataIntegrity.ConvertToBool(row["Proofed"]);

            if (editItemMode.HasValue)
            {
                // District test links are not active on teacher page.
                bool IsDistrictOrSchool = (_level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School);
                bool IsDistrictPortal = _category.ToLower().Trim() == "district";
                bool IsTargetted = Convert.ToBoolean(row["Targetted"]);
                int CreatedByPage = string.IsNullOrEmpty(row["createdBy"].ToString()) ? 0 : Convert.ToInt32(row["createdBy"].ToString());

                bool HasHyperlinkPermission = false;
                bool HasAdminIconPermission = false;

                switch (_category.ToLower())
                {
                    case "classroom":
                        HasHyperlinkPermission = UserHasPermission(Permission.Hyperlink_AssessmentNameClassroom);
                        HasAdminIconPermission = true;
                        break;

                    case "district":
                        HasHyperlinkPermission = UserHasPermission(Permission.Hyperlink_AssessmentNameDistrict);
                        HasAdminIconPermission = true;
                        break;

                    case "state":
                        HasHyperlinkPermission = UserHasPermission(Permission.Hyperlink_AssessmentNameState);
                        HasAdminIconPermission = UserHasPermission(Permission.Icon_AdministrationState);
                        break;
                }


                bool IsTestNameLinkActive = (IsDistrictOrSchool || IsDistrictPortal) && HasHyperlinkPermission;
                bool isSloTeacherAndNonSloAssessment = sloTeacher != null && (row["ItemClassId"].ToString() != "2") && (_category.ToUpper() == "DISTRICT");

                //determine if this is an SLOTeach user if the assessment is for an SLO CurrCourse

                if (IsTargetted && !IsDistrictOrSchool && CreatedByPage != SessionObject.LoggedInUser.Page)
                {
                    IsTestNameLinkActive = false;
                    HasHyperlinkPermission = false;
                }

                imgProofed.ImageUrl = (editItemMode.Value) ? "~/Images/editable.png" : "~/Images/proofed.png";
                hlkTestname.Enabled = ((IsTestNameLinkActive || HasHyperlinkPermission) && !isSloTeacherAndNonSloAssessment);
                hlkTestname.NavigateUrl = "~/Record/AssessmentObjects.aspx?xID=" + (String)row["TestID_Encrypted"];


                spnNumItems.Visible = editItemMode.Value;

                //WR 2932: Edit Button only visible for District Assessments when user/role has "Icon_Edit_District_Assessment" permission and item is not proofed
                hlkEdit.Visible = editItemMode.Value && (!IsDistrictPortal || (UserHasPermission(Permission.Icon_Edit_District_Assessment)));


                //MF 4022: Admin icon visible only when user/role has "Icon_AdministrationState" permission and item is proofed.
                hlkAdmin.Visible = !editItemMode.Value && HasAdminIconPermission;
                pnlLastEdit.Visible = editItemMode.Value;

                HyperLink imgOnclickLink = new HyperLink();
                imgOnclickLink.NavigateUrl = "~/Record/AssessmentPage.aspx?xID=" + (string)row["TestID_Encrypted"];

                string imgOnclickLinkString = imgOnclickLink.ResolveClientUrl(imgOnclickLink.NavigateUrl);
                string SecureText = "Term ";
                string isSecure = "false";
                if (SecureFormativetabStrip.SelectedIndex == 1)
                {
                    //SecureText = "[SECURE] Term ";
                    isSecure = "true";
                }
                string assessmentTitle = SecureText + row["Term"] + " " + row["TestType"] + " - " + row["Grade"] + " Grade " + row["Subject"] + (row["Course"].ToString() == row["Subject"].ToString() ? string.Empty : " " + row["Course"]);

                string onClientClick = "var _this=this; this.disabled=true; setTimeout(function(){ _this.disabled=false; }, 500); viewAssessments_editLink_onClick('" + imgOnclickLinkString + "', '" + assessmentTitle + "');";

                if (hlkEdit != null) hlkEdit.NavigateUrl = "javascript:" + onClientClick;

                // String for onclick for showing Assessment Administration popup. We pass parameters that we have.
                String classid = (_level == Base.Enums.EntityTypes.Class) ? _levelID.ToString() : String.Empty;
                String imgonclick = @"viewAsssessment_adminClick(" + row["TestID"].ToString() + "," + (String.IsNullOrEmpty(classid) ? "null" : classid) + ",'" + assessmentTitle + " - " + row["Description"].ToString().Replace("'", "") + "','" + _category.ToString() + "','" + _level.ToString() + "','" + false + "','" + isSecure + "')";

                if (hlkAdmin != null)
                {
                    if (row["DisplayDashboard"].ToString() == "No" && !UserHasPermission(Permission.Icon_AdministrationIcon_SecurityOverride))
                    {
                        hlkAdmin.Attributes["style"] = "opacity:.3; filter:alpha(opacity=30); cursor:default;";
                        hlkAdmin.Attributes["onclick"] = "return false;";
                    }
                    else
                    {
                        // TFS: 6703
                        //hlkAdmin.NavigateUrl = "javascript:" + imgonclick;
                        hlkAdmin.Attributes["onclick"] = "javascript:" + imgonclick;
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

        protected void RadTabStrip1_TabClick(object sender, RadTabStripEventArgs e)
        {
            LoadAssessments(e.Tab.TabStrip.SelectedIndex == 1);
        }
    }
}