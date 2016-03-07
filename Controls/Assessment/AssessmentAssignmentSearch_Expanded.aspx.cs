using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.Reports;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentAssignmentSearch_Expanded : ExpandedSearchPage
    {
        private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";
        private const int UserPage = 110;
        public DataTable GridDataTable;
        public DataTable tempDataTable;
        public List<Int64> ListOfTestEventIDs;
        public List<TestIDClassIDMapping> ListOfTestClassIDs = new List<TestIDClassIDMapping>();
        public string PageGuid;
        public int TestID;
        public string DataTableCount;
        private string _contentTypeURLParam;
        private string _contentType;
        private string _scoreType;
        private string _testCategory;
        private string _category;
        private bool _isProofed;
        protected EntityTypes _level;
        private static Base.Classes.Assessment _selectedAssessment;

        protected bool permAssessNameHyperLinksActive;
        protected bool IsDistrictOrSchool;
        protected bool IsDistrictPortal;
        private bool _isKentico = false;
        protected Dictionary<string, bool> dictionaryItem;
        bool isSecuredFlag;

        #region Properties

        public string HiddenGuid { get; set; }

        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            LoadSearchScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["testcategory"]))
                _category = Request.QueryString["testcategory"].ToString();
            if (!string.IsNullOrEmpty(Request.QueryString["level"]))
            {
                _level = (EntityTypes)Enum.Parse(typeof(EntityTypes), Request.QueryString["level"].ToString());
                IsDistrictOrSchool = (_level == Base.Enums.EntityTypes.District || _level == Base.Enums.EntityTypes.School);
            }

            //Get this parm if the page calling from kentico docs on Assessment link.
            if (Request.QueryString["IsKentico"] != null)
            { _isKentico = true; }
           
            // PBI: 2417 Add Test ID search criteria
            // Test ID criteria
            if (Request.QueryString["navigateFrom"] == null)
            {

                if (Request.QueryString["assessmentID"] == null || Request.QueryString["encrypted"] == null) return;

                bool idIsEncrypted = Standpoint.Core.Utilities.DataIntegrity.ConvertToBool(Request.QueryString["encrypted"]);
                TestID = idIsEncrypted ?
                        Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "assessmentID") :
                        DataIntegrity.ConvertToInt(Request.QueryString["assessmentID"]);

                if (TestID == 0) return;

                if (!IsPostBack) _selectedAssessment = Base.Classes.Assessment.GetAssessmentByID(TestID);

                _contentTypeURLParam = Request.QueryString["contentType"] == null ? string.Empty : Request.QueryString["contentType"].ToString();
                _contentType = _selectedAssessment.ContentType;
                _scoreType = _selectedAssessment.ScoreType;
                _testCategory = _selectedAssessment.TestCategory;
                _isProofed = _selectedAssessment.IsProofed;

                if (_contentType == "No Items/Content") headerButtonsWrapper.Visible = false;

                if (!string.IsNullOrEmpty(hiddenGuidBox.Text))
                {
                    PageGuid = hiddenGuidBox.Text;
                }
                else
                {
                    PageGuid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    hiddenGuidBox.Text = PageGuid;
                }

                if (!IsPostBack)
                {
                    if (!Standpoint.Core.Utilities.DataIntegrity.ConvertToBool(Request.QueryString["multipleAssessment"]))
                        this.Title = "Assessment Assignments: " + _selectedAssessment.TestName + " - " + _selectedAssessment.Description;
                    else
                        this.Title = "Assessment Assignments: Test Events for multiple Assessments";
                    this.hiddenAssessmentCategory.Value = _selectedAssessment.Category.ToString();
                }
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "anything", BuildStartupScript(exportGridImgBtn.ClientID, "../..", PageGuid), false);

                ParseRequestQueryString();

                LoadSearchCriteriaControl();

                if (!IsPostBack)
                    BindDataToGrid(); 
            }
            // PBI: 2417 Add Test ID search criteria
            // Test ID criteria
            else
            {
                _testCategory = _category;
                if (!string.IsNullOrEmpty(hiddenGuidBox.Text))
                {
                    PageGuid = hiddenGuidBox.Text;
                }
                else
                {
                    PageGuid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    hiddenGuidBox.Text = PageGuid;
                }

                this.hiddenAssessmentCategory.Value =_category;

                if (!IsPostBack)
                    this.Title = "Assessment Assignments";

                ScriptManager.RegisterStartupScript(Page, typeof(Page), "anything", BuildStartupScript(exportGridImgBtn.ClientID, "../..", PageGuid), false);

                LoadSearchCriteriaControl();

                if (!IsPostBack)
                    BindDataToGrid();
            }
            dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(_category);
            isSecuredFlag = dictionaryItem != null &&  dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            bool hasPermissionBubbleSheet = SessionObject.LoggedInUser.HasPermission(Permission.Icon_Access_SecureTesting_PrintBubbleSheets);
            bool hasPermissionEnableOtc = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting_EnableOTC);
            bool hasPermissionViewStudentScores = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting_ViewStudentScores);

            imgPrintBubble.Enabled = radBtnEnable.Enabled = radBtnDisable.Enabled = false;
                     

            if (!string.IsNullOrEmpty(Request.QueryString["isSecure"]))
            {
                 if (Request.QueryString["isSecure"].ToLower() == "true" && UserHasPermission(Permission.Access_SecureTesting) && isSecuredFlag )
                 { 
                    printBubble.Visible = hasPermissionBubbleSheet;
                    btnEnable.Visible = hasPermissionEnableOtc;
                    btnDisable.Visible = hasPermissionEnableOtc;
                    if(!hasPermissionViewStudentScores)
                    {
                        foreach (GridColumn column in radGridResults.Columns)
                        {
                            if (column.UniqueName == "AvgScore")
                            {
                                column.Visible = false;
                            }

                        }
                    }
                                     
                 }
             }

        }

        private void LoadSearchScripts()
        {
            if (Master != null)
            {
                var scriptManager = Master.FindControl("RadScriptManager2");
                if (scriptManager != null)
                {
                    RadScriptManager radScriptManager = (RadScriptManager)scriptManager;

                    radScriptManager.Scripts.Add(new ScriptReference("~/Scripts/AssessmentAssignmentSearch.js"));
                }
            }
        }

        private void BindDataToGrid()
        {
            var searchParms = (Criteria)Session["Criteria_" + HiddenGuid];

            // PBI: 2417 Add Test ID search criteria
            // Test ID criteria

            #region Get TestID criterion
            // The value displayed in column is a combination of class id and test id
            // need to filter test id from this value
            var testID = searchParms.CriterionList.Find(r => r.Key == "TestID").ReportStringVal ?? string.Empty;
            #endregion

            #region GetMultipleTestIDs

            List<int> ListOfMultipleTestIDs = new List<int>();
            if (Standpoint.Core.Utilities.DataIntegrity.ConvertToBool(Request.QueryString["multipleAssessment"]) && Session["ListOfSelectedTests"] != null)
            {
                ListOfMultipleTestIDs = (List<int>)Session["ListOfSelectedTests"];
            }

            #endregion

            bool hasSecurePermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);

            if (Request.QueryString["navigateFrom"] == null)
            {
                #region Get: CourseList
                var classCourseIDs = new CourseList();

                var selectedClassCourse = searchParms.CriterionList.Find(r => r.Key == "ClassCourses" && r.Empty == false && !string.IsNullOrEmpty(r.ReportStringVal));

                if (selectedClassCourse != null)
                {
                    classCourseIDs = new CourseList() { Thinkgate.Base.Classes.CourseMasterList.GetClassCourseById(Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(selectedClassCourse.ReportStringVal)) };
                }
                else
                {
                    classCourseIDs = Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
                }
                #endregion

                #region Get SchoolType criterion

                List<string> schoolTypes = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("SchoolType") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

                #endregion

                #region Get School criterion

                var masterUserSchools = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser); //Thinkgate.Base.Classes.School.GetSchoolListForDropDown(SessionObject.LoggedInUser.Page);

                var masterUserSchoolList = (from school in masterUserSchools select DataIntegrity.ConvertToInt(school.ID)).ToList();

                List<int> schools =
                        searchParms.CriterionList.Exists(r => r.Key == "School" && !string.IsNullOrEmpty(r.ReportStringVal))
                                ? searchParms.CriterionList.FindAll(r => r.Key == "School" && !string.IsNullOrEmpty(r.ReportStringVal))
                                            .Select(s => DataIntegrity.ConvertToInt(s.ReportStringVal)).ToList()
                                : masterUserSchoolList;

                #endregion

                #region Get Teacher criterion
                var teacher = searchParms.CriterionList.Find(r => r.Key == "AssessmentAssignmentTeacher").ReportStringVal ?? string.Empty;
                #endregion

                #region Get Section criterion
                var section = searchParms.CriterionList.Find(r => r.Key == "Section").ReportStringVal ?? string.Empty;
                #endregion

                #region Get Block criterion
                var block = searchParms.CriterionList.Find(r => r.Key == "Block").ReportStringVal ?? string.Empty;
                #endregion

                #region Get Period criterion

                List<string> periodList = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Period") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

                #endregion

                #region Get SemesterList criterion

                List<string> semesterList = searchParms.CriterionList.Exists(r => r.Key == "Semester" && !string.IsNullOrEmpty(r.ReportStringVal))
                                ? searchParms.CriterionList.FindAll(r => r.Key == "Semester" && !string.IsNullOrEmpty(r.ReportStringVal))
                                            .Select(s => s.ReportStringVal).ToList()
                                : new List<string>();

                #endregion

                if (testID != string.Empty)
                {
                    int lenTestId = Convert.ToInt32(testID.ToString().Substring(0, 1)) - 3;
                    TestID = Convert.ToInt32(testID.ToString().Substring(testID.ToString().Length - lenTestId));
                }

                tempDataTable = Thinkgate.Base.Classes.Assessment.SearchAssessmentAssignments(
                    TestID,
                    classCourseIDs,
                    SessionObject.LoggedInUser.Roles.Select(r => r.RoleName.ToString()).ToList(),
                    _category,
                    ListOfMultipleTestIDs,
                    schools,
                    schoolTypes,
                    teacher,
                    section,
                    block,
                    periodList,
                    semesterList,
                    SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Schools) ? "Yes" : "No",
                    SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Courses) ? "Yes" : "No",
                    _contentType == "No Items/Content");


            }
            else
            {
                
                var classCourseIDs = Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);

                var masterUserSchools = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser); //Thinkgate.Base.Classes.School.GetSchoolListForDropDown(SessionObject.LoggedInUser.Page);

                var masterUserSchoolList = (from school in masterUserSchools select DataIntegrity.ConvertToInt(school.ID)).ToList();

                List<int> schools = masterUserSchoolList;

                tempDataTable = Thinkgate.Base.Classes.Assessment.SearchAssessmentAssignmentsByEventID(
                       testID,
                       classCourseIDs,
                       SessionObject.LoggedInUser.Roles.Select(r => r.RoleName.ToString()).ToList(),
                       _category,
                       ListOfMultipleTestIDs,
                       schools,
                       new List<string>(),
                       string.Empty,
                       string.Empty,
                       string.Empty,
                       new List<string>(),
                       new List<string>(),
                       SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Schools) ? "Yes" : "No",
                       SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Courses) ? "Yes" : "No",
                       _contentType == "No Items/Content",hasSecurePermission);

            }
            GridDataTable = new DataTable();
            if (testID != string.Empty)
            {
                string expression;
                expression = "TestEventID='" + testID + "'";
                DataRow[] rowArray;

                // Use the Select method to find all rows matching the filter.
                rowArray = tempDataTable.Select(expression);
                if(rowArray.Length > 0 )
                    GridDataTable = rowArray.CopyToDataTable();
            }
            else
                GridDataTable = tempDataTable;

            if (GridDataTable.Rows.Count > 0)
            {
                GridDataTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(GridDataTable, "SchoolID", "EncryptedID");
                GridDataTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(GridDataTable, "TestID", "EncryptedTestID");
                ListOfTestEventIDs = new List<Int64>();
                ListOfTestClassIDs = new List<TestIDClassIDMapping>();
                foreach (DataRow row in GridDataTable.Rows)
                {
                    Int64 tempID;
                    Int64.TryParse(row["TestEventID"].ToString(), out tempID);
                    ListOfTestEventIDs.Add(tempID);

                    Int32 tempTestID;
                    Int32.TryParse(row["TestID"].ToString(), out tempTestID);

                    Int32 tempClassID;
                    Int32.TryParse(row["ClassID"].ToString(), out tempClassID);

                    ListOfTestClassIDs.Add(new TestIDClassIDMapping { TestID = tempTestID, ClassID = tempClassID });
                }

                hiddenListOfTestEventIDs.Text = string.Join(",", ListOfTestEventIDs);
                imgPrintBubble.Attributes["TEIDS"] = string.Join(",", ListOfTestEventIDs);
                // Start: TFS# 280 , 19 Nov 2012: Sanjeev Kumar, To store data in ViewState. 
                ViewState["ListOfTestEventIDs"] = ListOfTestEventIDs;
                // End: TFS# 280 , 19 Nov 2012: Sanjeev Kumar, To store data in ViewState. 
                Session["ListOfTestClassIDs"] = ListOfTestClassIDs;
            }

            DataTableCount = GridDataTable.Rows.Count.ToString();
            radGridResults.DataSource = GridDataTable;
            radGridResults.DataBind();
            initialDisplayText.Visible = false;
            //if (_contentType == "No Items/Content") radGridResults.Columns[0].Visible = false;

            //Start: BugID#:150 , 12/12/2012: Jeetendra Kumar, To Display No of Results Found after searching.
            if (DataIntegrity.ConvertToInt(DataTableCount) > 0)
            {
                lblSearchTotal.Text = string.Format("Results Found: {0}", DataTableCount);
            }
            else
            {
                lblSearchTotal.Text = "Results Found: 0";
            }
            //End: BugID#:150 , 12/12/2012: Jeetendra Kumar, To Display No of Results Found after searching.
        }

        protected void ImageIcon_Click(object sender, EventArgs e)
        {
        }

        protected void ImageGrid_Click(object sender, EventArgs e)
        {
        }

        protected void GotoAssessmentButtonClick(object sender, EventArgs eventArgs)
        {
        }

        #region Private Methods

        private Criteria LoadSearchCriteria()
        {
            var criteria = new Criteria();

            // PBI: 2417 Add Test ID search criteria
            // Test ID criteria
            #region TestID Criterion
            criteria.Add(new Criterion
            {
                Header = "TestID",
                Key = "TestID",
                Locked = false,
                Removable = true,
                Description = string.Empty,
                Type = "String",
                UIType = UIType.TextBox

            });

            #endregion

            if (Request.QueryString["navigateFrom"] == null)
            {
                #region Course Criterion
                var classCourses =
                    Thinkgate.Base.Classes.CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
                var classcourseDt = new DataTable();
                classcourseDt.Columns.Add("CourseName");
                classcourseDt.Columns.Add("CourseID");
                classCourses.Sort((x, y) => String.Compare(x.Grade.DisplayText, y.Grade.DisplayText));
                foreach (var c in classCourses)
                {
                    classcourseDt.Rows.Add(c.Grade + "-" + c.CourseName, c.ID);
                }

                criteria.Add(new Criterion
                {
                    Header = "Course",
                    Key = "ClassCourses",
                    DataSource = classcourseDt,
                    DataTextField = "CourseName",
                    DataValueField = "CourseID",
                    Locked = false,
                    Removable = true,
                    Description = string.Empty,
                    Type = "String",
                    UIType = UIType.DropDownList
                });
                #endregion

                #region SchoolType Criterion
                var schoolTypeDataTable = new DataTable();
                var schoolTypesForLoggedInUser = SchoolTypeMasterList.GetSchoolTypeListForUser(SessionObject.LoggedInUser);
                schoolTypeDataTable.Columns.Add("SchoolType");

                //BJC 08/01/2012: Get school data before School Type criterion so that we can determine whether to include the dependencies service or not.
                var schoolDataTable = new DataTable();
                var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
                schoolDataTable.Columns.Add("Name");
                schoolDataTable.Columns.Add("ID");
                foreach (var s in schoolsForLooggedInUser)
                {
                    schoolDataTable.Rows.Add(s.Name, s.ID);
                }
                bool showOneSchool = !SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Schools) && schoolDataTable.Rows.Count == 1;
                //-------------------------------------------------------------------------------------------------------------------------------

                foreach (var s in schoolTypesForLoggedInUser)
                {
                    schoolTypeDataTable.Rows.Add(s);
                }

                criteria.Add(new Criterion
                {
                    Header = "School Type",
                    Key = "SchoolType",
                    Type = "String",
                    Description = string.Empty,
                    Locked = false,
                    Removable = true,
                    DataSource = schoolTypeDataTable,
                    UIType = UIType.CheckBoxList,
                    DataTextField = "SchoolType",
                    DataValueField = "SchoolType",
                    ServiceUrl = showOneSchool ? null : "../../Services/School.svc/GetAllSchoolsFromSchoolTypes",
                    ServiceOnSuccess = showOneSchool ? null : "getAllSchoolsFromSchoolTypes",
                    Dependencies = showOneSchool ? null : new[]
								{
										Criterion.CreateDependency("SchoolType", "SchoolTypes"),
										Criterion.CreateDependency("School", "Schools")
								}
                });

                #endregion

                #region School Criterion
                criteria.Add(new Criterion
                {
                    Header = "School",
                    Key = "School",
                    Type = "String",
                    Description = string.Empty,
                    Locked = showOneSchool,
                    Removable = !showOneSchool,
                    DataSource = schoolDataTable,
                    UIType = UIType.DropDownList,
                    Object = showOneSchool ? schoolDataTable.Rows[0][0].ToString() : null,
                    DefaultValue = showOneSchool ? schoolDataTable.Rows[0][1].ToString() : null,
                    DataTextField = "Name",
                    DataValueField = "ID"
                });
                #endregion

                #region Teacher Criterion

                criteria.Add(new Criterion
                {
                    Header = "Teacher",
                    Key = "AssessmentAssignmentTeacher",
                    //DataTextField = "CourseName",
                    //DataValueField = "CourseID",
                    Locked = false,
                    Removable = true,
                    Description = string.Empty,
                    Type = "String",
                    UIType = UIType.TextBox

                });
                #endregion

                #region Section Criterion
                criteria.Add(new Criterion
                {
                    Header = "Section",
                    Key = "Section",
                    Type = "String",
                    Description = string.Empty,
                    Locked = false,
                    Removable = false,
                    UIType = UIType.TextBox
                });
                #endregion

                #region Period Criterion

                criteria.Add(new Criterion
                {
                    Header = "Period",
                    Key = "Period",
                    Type = "String",
                    Description = string.Empty,
                    Locked = false,
                    DataTextField = "Period",
                    DataValueField = "Period",
                    Removable = true,
                    DataSource = Thinkgate.Base.Classes.PeriodMasterList.GetPeriodDataTableForDropDown("Period"),
                    UIType = UIType.CheckBoxList

                });


                #endregion

                #region Block Criterion
                criteria.Add(new Criterion
                {
                    Header = "Block",
                    Key = "Block",
                    Type = "String",
                    Description = string.Empty,
                    Locked = false,
                    Removable = true,
                    UIType = UIType.TextBox
                    //Object = Category
                });
                #endregion

                #region Semester Criterion
                criteria.Add(new Criterion
                {
                    Header = "Semester",
                    Key = "Semester",
                    Type = "String",
                    Description = string.Empty,
                    Locked = false,
                    Removable = true,
                    DataTextField = "Semester",
                    DataValueField = "Semester",
                    DataSource = Thinkgate.Base.Classes.Semester.GetSemesterListForDropDown(),
                    UIType = UIType.DropDownList
                });
                #endregion
            }
            return criteria;
        }

        private void LoadSearchCriteriaControl()
        {
            var controlReportCriteria = (ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            controlReportCriteria.ID = "ctlSearchResultsCriteria";

            if (string.IsNullOrEmpty(hiddenTextBox.Text))
            {
                HiddenGuid = Guid.NewGuid().ToString();
                hiddenTextBox.Text = HiddenGuid;
                controlReportCriteria.Guid = HiddenGuid;
                controlReportCriteria.Criteria = LoadSearchCriteria();
                controlReportCriteria.FirstTimeLoaded = true;

                initialDisplayText.Controls.Add(new System.Web.UI.HtmlControls.HtmlGenericControl("div") { InnerText = "Press Update Results...." });
                initialDisplayText.Visible = true;
            }
            else
            {
                HiddenGuid = hiddenTextBox.Text;
                controlReportCriteria.Guid = hiddenTextBox.Text;
                controlReportCriteria.FirstTimeLoaded = false;
            }

            controlReportCriteria.ReloadReport += ReportCriteria_ReloadReport;

            criteraDisplayPlaceHolder.Controls.Add(controlReportCriteria);
        }

        private void ReportCriteria_ReloadReport(object sender, EventArgs e)
        {
            ClearSelection();
            BindDataToGrid();
        }

        private void ParseRequestQueryString()
        {
            //if (string.IsNullOrEmpty(Request.QueryString["level"]))
            //{
            //    SessionObject.RedirectMessage = "No level provided in URL.";
            //    Response.Redirect("~/PortalSelection.aspx", true);
            //}

            //_level = (EntityTypes)EnumUtils.enumValueOf(Request.QueryString["level"], typeof(EntityTypes));
        }

        #endregion

        protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
        {

            if (e.Item.ItemType == GridItemType.Header)
            {
                System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Item.FindControl("chkAll");
                chk.Attributes["onclick"] = "selectAll(this,'" + radGridResults.ClientID + "'," + DataTableCount + ");";

                if (hiddenChkAll.Text == "1" && string.IsNullOrEmpty(hiddenDeSelected.Text))
                    chk.Checked = true;
            }

            if (e.Item is GridDataItem)
            {
                GridItem gridItem = e.Item;
                DataRowView row = (DataRowView)(gridItem).DataItem;

                bool isSecureAssessment = Convert.ToBoolean(row["Secure"].ToString());
                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);

                if (hasPermission && isSecureAssessment)
                {
                    var img = e.Item.FindControl("imgIconSecure");
                    img.Visible = true;
                }

                String testLinkUrl = "~/Record/AssessmentObjects.aspx?xID=" + (String)row["EncryptedTestID"];
                HyperLink assessmentNameLabel;

                if (!string.IsNullOrEmpty(_category))
                {
                    switch (_category.ToLower().Trim())
                    {
                        case "classroom":
                            permAssessNameHyperLinksActive = UserHasPermission(Permission.Hyperlink_AssessmentNameClassroom);
                            IsDistrictPortal = false;
                            break;

                        case "district":
                            permAssessNameHyperLinksActive = UserHasPermission(Permission.Hyperlink_AssessmentNameDistrict);
                            IsDistrictPortal = true;
                            break;

                        case "state":
                            permAssessNameHyperLinksActive = UserHasPermission(Permission.Hyperlink_AssessmentNameState);
                            IsDistrictPortal = false;
                            break;
                    }
                }

                /***********************************************************
                 * if user has permission then display link to assessment.  
                 * Otherwise only display label of assessment name.
                 * ********************************************************/
                bool IsTestNameLinkActive = (IsDistrictOrSchool || IsDistrictPortal) && permAssessNameHyperLinksActive;
                bool IsTargeted = Convert.ToBoolean(row["Targeted"]);
                int CreatedByPage = string.IsNullOrEmpty(row["Author"].ToString()) ? 0 : Convert.ToInt32(row["Author"].ToString());
                bool isSloTeacherAndNonSloAssessment = false;
                var sloTeacher = SessionObject.LoggedInUser.Roles.Find(x => x.RoleName == "SLOTeach");
                string isGroup = row["IsGroup"].ToString() == "1" ? "True" : "False";
                if (sloTeacher != null && (row["ItemClassId"].ToString() != "2") && (_category.ToLower().Trim() == "district"))
                {
                    isSloTeacherAndNonSloAssessment = true;
                }

                if ((IsTargeted && !IsDistrictOrSchool && CreatedByPage != SessionObject.LoggedInUser.Page) || isSloTeacherAndNonSloAssessment )
                {
                    IsTestNameLinkActive = false;
                    permAssessNameHyperLinksActive = false;
                }

                assessmentNameLabel = (HyperLink)gridItem.FindControl("lnkListTestName");
                Label assessmentNameLabelNormalText = (Label)gridItem.FindControl("lblListTestName");

                assessmentNameLabel.Enabled = (IsTestNameLinkActive || permAssessNameHyperLinksActive);
                assessmentNameLabel.Visible = (IsTestNameLinkActive || permAssessNameHyperLinksActive);
                assessmentNameLabel.NavigateUrl = testLinkUrl;

                assessmentNameLabelNormalText.Visible = !assessmentNameLabel.Visible;

                String classLinkUrl = "~/Record/Class.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["ClassID"].ToString());
                HyperLink lnk;
                

                
                    if ((lnk = (HyperLink) gridItem.FindControl("lnkListClassName")) != null)
                    {
                            lnk.Visible = true;
                            lnk.NavigateUrl = classLinkUrl;
                            lnk.Enabled = !Convert.ToBoolean(isGroup);
                    }
                    

                System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Item.FindControl("chkRowInput");
                chk.Attributes["onclick"] = "selectThisRow(this,'" + radGridResults.ClientID + "'," + e.Item.ItemIndex + "," + DataTableCount + "," + row["TestID"] + "," + row["ClassID"] + "," + row["TestEventID"] + ");";
                chk.Attributes.Add("rowIndex", e.Item.ItemIndex.ToString());
                chk.Attributes.Add("assessmentSecurity", row["AssessmentSecurity"].ToString());
                chk.Attributes.Add("currentPortal", SessionObject.CurrentPortal.ToString());

                string[] arrUnChecked = hiddenDeSelected.Text.Split(',').ToArray();
                var foundUnChecked = arrUnChecked.FirstOrDefault(x => x.ToString() == row["TestID"].ToString() + "-" + row["ClassID"].ToString());
                if (hiddenChkAll.Text == "1" && string.IsNullOrEmpty(foundUnChecked))
                {
                    gridItem.Selected = true;
                    chk.Checked = true;
                }

                string[] arrChecked = hiddenSelected.Text.Split(',').ToArray();
                var foundChecked = arrChecked.FirstOrDefault(x => x.ToString() == row["TestID"].ToString() + "-" + row["ClassID"].ToString());
                if (!string.IsNullOrEmpty(foundChecked))
                {
                    gridItem.Selected = true;
                    chk.Checked = true;
                }

                if (Convert.ToBoolean(isGroup))
                {
                    e.Item.BackColor = System.Drawing.Color.LightGray;
                }
                //string isSecure = "false";
                //if (!string.IsNullOrEmpty(Request.QueryString["isSecure"]))
                //{
                //    if (Request.QueryString["isSecure"].ToLower() == "true")
                //    {
                //        isSecure = "true";
                //    }
                //}
                string category = string.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["testcategory"]))
                {
                    category = Request.QueryString["testcategory"];
                }
                String imgonclick = @"searchAsssessmentAssignment_adminClick(" + row["TestID"].ToString() + "," + row["ClassID"].ToString() + ",'" + _contentTypeURLParam + "','" + _scoreType + "','" + isGroup + "'," + _isKentico.ToString().ToLowerInvariant() + ",'" + isSecureAssessment + "','" + category + "')";
                System.Web.UI.HtmlControls.HtmlImage imgBtn;
                if ((imgBtn = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("imgGraphicAdmin")) != null)
                {
                    // #7795: Show/Hide Administration icon based on security settings
                    Base.Enums.Permission administPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Icon_Administration" + _testCategory, true);
                    bool isTestProofed = !string.IsNullOrEmpty(row["TestProofed"].ToString()) && row["TestProofed"].ToString().ToLower().Trim() == "yes";
                    Boolean showAdministBtn = UserHasPermission(administPerm) && isTestProofed;

                    imgBtn.Visible = showAdministBtn;
                    if (showAdministBtn && Convert.ToBoolean(row["DisplayDashboard"]) && !UserHasPermission(Permission.Icon_AdministrationIcon_SecurityOverride))
                    {
                        imgBtn.Attributes["onclick"] = "return false;";
                        imgBtn.Attributes["style"] = "cursor:default; opacity:.3; filter:alpha(opacity=30);";
                    }
                    else
                    {
                        imgBtn.Attributes["onclick"] = imgonclick;
                    }
                }

                //System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Item.FindControl("chkRowInput");
                //chk.Attributes["onchange"] = "selectThisRow(this,'" + radGridResults.ClientID + "'," + e.Item.ItemIndex + "); return false;";
                //chk.Attributes.Add("rowIndex", e.Item.ItemIndex.ToString());
                ////ShowAndHideIcons
                //String status = (String)row["STATUS"];

                //if (status.Trim() != "Proofed")
                //{
                //    var item = e.Item.FindControl("imgGraphicAdmin");
                //}

                //Base.Enums.Permission assessmentPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_Assessment" + Category, true);
                //Base.Enums.Permission answerKeyPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_AnswerKey" + Category, true);
                //String printonclick = @"searchAsssessment_printClick(" + "'" + row["TestID"].ToString() + "'"
                //                                                                                         + "," + "'" + row["TestName"].ToString() + "'" + ")";
                //Boolean showPrint = UserHasPermission(assessmentPerm) || UserHasPermission(answerKeyPerm);
                //System.Web.UI.HtmlControls.HtmlImage imgBtn;
                //if ((imgBtn = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("imgGraphicPrint")) != null)
                //{
                //    imgBtn.Attributes["onclick"] = printonclick;
                //    imgBtn.Visible = showPrint;
                //}

                //Boolean? editItemMode = null;
                //if (!string.IsNullOrEmpty(status))
                //    editItemMode = status == "Proofed" ? false : true;

                //HyperLink imgOnclickLink = new HyperLink();
                //string imgOnclickLinkString;
                //imgOnclickLink.NavigateUrl = "~/Record/AssessmentPage.aspx?xID=" + (string)row["EncryptedID"];
                //imgOnclickLinkString = imgOnclickLink.ResolveClientUrl(imgOnclickLink.NavigateUrl);
                //string assessmentTitle = "Term " + row["Term"] + " " + row["TestType"] + " - " + row["Grade"] + " Grade " + row["Subject"] + (row["Course"].ToString() == row["Subject"].ToString() ? string.Empty : " " + row["Course"]);
                //string onClientClick = "setTimeout(function() { customDialog({ url: '" + imgOnclickLinkString + "', title: '" + assessmentTitle + "', maximize: true}); }, 0); return false;";
                //string onClientClick2 = @"searchAsssessment_editClick(" + "'" + row["EncryptedID"].ToString() + "'"
                //                                                                                         + "," + "'" + assessmentTitle + "'" + ")";

                //System.Web.UI.HtmlControls.HtmlImage editBtn;
                //if (editItemMode.HasValue)
                //{

                //    if ((editBtn = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("btnGraphicEdit1")) != null)
                //    {
                //        editBtn.Visible = editItemMode.Value;
                //        editBtn.Attributes["onclick"] = onClientClick2;
                //    }
                //}
                //else
                //{
                //    if ((editBtn = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("btnGraphicEdit1")) != null)
                //    {
                //        editBtn.Visible = false;
                //        editBtn.Attributes["onclick"] = "alert('Proofed status could not be verified.'); return false;";
                //    }

                //}

            }
        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }

        protected void RadGridPageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindDataToGrid();

            if (!string.IsNullOrEmpty(hiddenSelectedCount.Text))
            {
                lblSelectedCount.InnerText = "Records selected: " + hiddenSelectedCount.Text;
                btnEnable.Visible = true;
                btnDisable.Visible = true;
                printBubble.Visible = true;
                imgPrintBubble.Enabled = radBtnEnable.Enabled = radBtnDisable.Enabled = true;
                
               
            }
            else
            {
                lblSelectedCount.InnerText = "";
                btnEnable.Visible = false;
                btnDisable.Visible = false;
                printBubble.Visible = false;
                imgPrintBubble.Enabled = radBtnEnable.Enabled = radBtnDisable.Enabled = false;
               
               

            }
        }

        public void ExportToExcel(DataTable dt)
        {
            // Create the workbook
            XLWorkbook workbook = ConvertDataTableToSingleSheetWorkBook(dt, "Results");

            //Prepare the response

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);

                Session["FileExport_Content" + PageGuid] = memoryStream.ToArray();

            }

        }

        protected void ExportGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            BindDataToGrid();
            ExportToExcel(GridDataTable);
        }

        protected void EnableAssessments_Click(object sender, EventArgs e)
        {
            int updatedCount = 0;
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }
            }

            if (selectAll)
            {
                ListOfTestClassIDs = Session["ListOfTestClassIDs"] != null ? (List<TestIDClassIDMapping>)Session["ListOfTestClassIDs"] : null;

                if (ListOfTestClassIDs != null && ListOfTestClassIDs.Count > 0)
                {
                    foreach (TestIDClassIDMapping record in ListOfTestClassIDs)
                    {
                        Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(true, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(record.TestID),
                                    Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(record.ClassID), SessionObject.LoggedInUser.Page);
                    }

                    updatedCount = ListOfTestClassIDs.Count;
                }
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                ListOfTestClassIDs = Session["ListOfTestClassIDs"] != null ? (List<TestIDClassIDMapping>)Session["ListOfTestClassIDs"] : null;

                if (ListOfTestClassIDs != null && ListOfTestClassIDs.Count > 0)
                {
                    string[] arrUnChecked = hiddenDeSelected.Text.Split(',').ToArray();

                    foreach (TestIDClassIDMapping record in ListOfTestClassIDs)
                    {
                        Int32 tempTestID;
                        Int32.TryParse(record.TestID.ToString(), out tempTestID);

                        Int32 tempClassID;
                        Int32.TryParse(record.ClassID.ToString(), out tempClassID);

                        var foundUnChecked = arrUnChecked.FirstOrDefault(x => x.ToString() == tempTestID.ToString() + "-" + tempClassID.ToString());
                        if (string.IsNullOrEmpty(foundUnChecked) && tempTestID != 0 && tempClassID != 0)
                        {
                            Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(true, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempTestID),
                                    Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempClassID), SessionObject.LoggedInUser.Page);

                            updatedCount += 1;
                    }
                }
            }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                ListOfTestClassIDs = Session["ListOfTestClassIDs"] != null ? (List<TestIDClassIDMapping>)Session["ListOfTestClassIDs"] : null;

                if (ListOfTestClassIDs != null && ListOfTestClassIDs.Count > 0)
                {
                    string[] arrChecked = hiddenSelected.Text.Split(',').ToArray();

                    foreach (TestIDClassIDMapping record in ListOfTestClassIDs)
                    {
                        Int32 tempTestID;
                        Int32.TryParse(record.TestID.ToString(), out tempTestID);

                        Int32 tempClassID;
                        Int32.TryParse(record.ClassID.ToString(), out tempClassID);

                        var foundChecked = arrChecked.FirstOrDefault(x => x.ToString() == record.TestID.ToString() + "-" + record.ClassID.ToString());
                        if (!string.IsNullOrEmpty(foundChecked) && tempTestID != 0 && tempClassID != 0)
                        {
                            Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(true, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempTestID),
                                    Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempClassID), SessionObject.LoggedInUser.Page);

                            updatedCount += 1;
                    }
                }
            }
            }
            else
            {
                foreach (GridDataItem item in radGridResults.SelectedItems)
                {
                    if (item.FindControl("hiddenClassID") != null && item.FindControl("hiddenTestID") != null)
                    {
                        var testID = ((RadTextBox)item.FindControl("hiddenTestID")).Text;
                        var classID = ((RadTextBox)item.FindControl("hiddenClassID")).Text;

                        Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(true, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(testID),
                                Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(classID), SessionObject.LoggedInUser.Page);
                    }
                }

                updatedCount = radGridResults.SelectedItems.Count;
            }

                DisplayMessageInAlert("Online testing has been enabled for " + updatedCount + " assessment" + AddPlurality(updatedCount) + ".");
            ClearSelection();
            BindDataToGrid();
        }

        protected void DisableAssessments_Click(object sender, EventArgs e)
        {
            int updatedCount = 0;
            bool selectAll = new bool();
            var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
            if (header != null)
            {
                if (header.FindControl("chkAll") != null)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)header.FindControl("chkAll");

                    if (chk.Attributes["checked"] == "checked")
                    {
                        selectAll = true;
                    }
                }
            }

            if (selectAll)
            {
                ListOfTestClassIDs = Session["ListOfTestClassIDs"] != null ? (List<TestIDClassIDMapping>)Session["ListOfTestClassIDs"] : null;

                if (ListOfTestClassIDs != null && ListOfTestClassIDs.Count > 0)
                {
                    foreach (TestIDClassIDMapping record in ListOfTestClassIDs)
                    {
                        Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(false, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(record.TestID),
                                    Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(record.ClassID), SessionObject.LoggedInUser.Page);
                    }

                    updatedCount = ListOfTestClassIDs.Count;
                }
            }
            else if (hiddenChkAll.Text == "1" && !string.IsNullOrEmpty(hiddenDeSelected.Text))
            {
                ListOfTestClassIDs = Session["ListOfTestClassIDs"] != null ? (List<TestIDClassIDMapping>)Session["ListOfTestClassIDs"] : null;

                if (ListOfTestClassIDs != null && ListOfTestClassIDs.Count > 0)
                {
                    string[] arrUnChecked = hiddenDeSelected.Text.Split(',').ToArray();

                    foreach (TestIDClassIDMapping record in ListOfTestClassIDs)
                    {
                        Int32 tempTestID;
                        Int32.TryParse(record.TestID.ToString(), out tempTestID);

                        Int32 tempClassID;
                        Int32.TryParse(record.ClassID.ToString(), out tempClassID);

                        var foundUnChecked = arrUnChecked.FirstOrDefault(x => x.ToString() == tempTestID.ToString() + "-" + tempClassID.ToString());
                        if (string.IsNullOrEmpty(foundUnChecked) && tempTestID != 0 && tempClassID != 0)
                        {
                            Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(false, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempTestID),
                                    Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempClassID), SessionObject.LoggedInUser.Page);

                            updatedCount += 1;
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(hiddenSelected.Text))
            {
                ListOfTestClassIDs = Session["ListOfTestClassIDs"] != null ? (List<TestIDClassIDMapping>)Session["ListOfTestClassIDs"] : null;

                if (ListOfTestClassIDs != null && ListOfTestClassIDs.Count > 0)
                {
                    string[] arrChecked = hiddenSelected.Text.Split(',').ToArray();

                    foreach (TestIDClassIDMapping record in ListOfTestClassIDs)
                    {
                        Int32 tempTestID;
                        Int32.TryParse(record.TestID.ToString(), out tempTestID);

                        Int32 tempClassID;
                        Int32.TryParse(record.ClassID.ToString(), out tempClassID);

                        var foundChecked = arrChecked.FirstOrDefault(x => x.ToString() == record.TestID.ToString() + "-" + record.ClassID.ToString());
                        if (!string.IsNullOrEmpty(foundChecked) && tempTestID != 0 && tempClassID != 0)
                        {
                            Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(false, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempTestID),
                                    Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(tempClassID), SessionObject.LoggedInUser.Page);

                            updatedCount += 1;
                        }
                    }
                }
            }
            else
            {
                foreach (GridDataItem item in radGridResults.SelectedItems)
                {
                    if (item.FindControl("hiddenClassID") != null && item.FindControl("hiddenTestID") != null)
                    {
                        var testID = ((RadTextBox)item.FindControl("hiddenTestID")).Text;
                        var classID = ((RadTextBox)item.FindControl("hiddenClassID")).Text;

                        Thinkgate.Base.Classes.Assessment.UpdateOnlineTestStatus(false, Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(testID),
                                Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(classID), SessionObject.LoggedInUser.Page);
                    }
                }

                updatedCount = radGridResults.SelectedItems.Count;
            }

            DisplayMessageInAlert("Online testing has been disabled for " + updatedCount + " assessment" + AddPlurality(updatedCount) + ".");
            ClearSelection();
            BindDataToGrid();
        }

        public void DisplayMessageInAlert(string text)
        {
            if (Master != null)
            {
                var radWinManager = Master.FindControl("RadWindowManager");

                if (radWinManager != null)
                {
                    ((RadWindowManager)radWinManager).RadAlert(text, null, 100, "Message", "");

                }
            }

        }

        public string AddPlurality(int count)
        {
            return count == 0 || count > 1 ? "s" : "";
        }

        public void ClearSelection()
        {
            // Clear checked-unchecked status of the checkboxes
            lblSelectedCount.InnerText = string.Empty;
            hiddenChkAll.Text = string.Empty;
            hiddenSelected.Text = string.Empty;
            hiddenDeSelected.Text = string.Empty;
            hiddenSelectedCount.Text = string.Empty;
            hiddenTotalCount.Text = string.Empty;

            // Disable action buttons
            imgPrintBubble.Enabled = false;
            radBtnEnable.Enabled = false;
            radBtnDisable.Enabled = false;
          
        }
    }

    [Serializable]
    public class TestIDClassIDMapping
    {
        public int TestID { get; set; }

        public int ClassID { get; set; }
    }

}
