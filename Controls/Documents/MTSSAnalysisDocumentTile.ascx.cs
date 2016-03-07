using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using CultureInfo = System.Globalization.CultureInfo;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace Thinkgate.Controls.Documents
{
    public partial class MTSSDiagnosticsDocumentTile : TileControlBase
    {
        //Variables
        private string _type;
        public int _clientBaseLookupEnum;
        private string legacyUrl;
        private string visibleTab = null;
        private bool _isFormColumnEnabled;

        //Objects
        private List<UserNodeList> userNodeList;
        private EntityTypes _level;
        private Int32 _levelID;
        private List<ThinkgateSchool> allSchools;
        private IEnumerable<Grade> gradeList;
        private IEnumerable<Subject> subjectList;
        private MTSSSessionObject _MTSSSession;
        private CourseList _CurrCourseList;
        private UserInfo ui;
        private TreeProvider tree;

        //Constants
        private const String FORWARD_SLASH = "/";
        private const String KenticoUserInfo = "KenticoUserInfo";
        private const String E3UserInfo = "SessionObject";
        private const String PageSessionStateName = "MTSSAnalysis";
        private const String KenticoItemType = "CMS.MenuItem";
        private const String Tier1 = "Analysis";
        private const String Tier2_3 = "Intervention";
        private const String InterventionBehavioral = "Behavioral";
        private const String InterventionAcademic = "Academic";
        private const String InterventionAll = "Behavioral, Academic";

        #region Events
        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null || Tile.TileParms == null) return;

            /**********************************************************************************************
            *  Only update for this control firing (Telerik Ajax postback for all user controls)          *
            *  Looks for postback control id's containing "MTSS Analysis" (quick solution)                *
            **********************************************************************************************/
            if (IsPostBack)
            {
                String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
                if (!String.IsNullOrEmpty(postBackControlID)
                    && !postBackControlID.StartsWith("folder")
                    && !postBackControlID.StartsWith("tileContainer")
                    && !postBackControlID.Contains("MTSSAnalysis"))
                {
                    return; //Return to avoid pulling data again when other user controls refresh via ajax
                }
            }

            //Tile Parms
            if (Tile.TileParms.GetParm("type") != null)
                _type = Tile.TileParms.GetParm("type").ToString();

            if (Tile.TileParms.GetParm("level") != null)
                _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");

            if (Tile.TileParms.GetParm("levelID") != null)
                _levelID = (Int32)Tile.TileParms.GetParm("levelID");

            _isFormColumnEnabled = (bool)Tile.TileParms.GetParm("isFormColumnEnabled");

            btnAdd.Attributes["title"] = string.Format("Add New {0}", Tile.Title);

            _CurrCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            initializeSession();

            setTabSecurity();

            //Remove any existing windows
            for (int i = wndWindowManager.Windows.Count - 1; i > -1; i--)
            {
                if (!new List<string> { "wndAddDocument", "wndCmsNewDocumentShell" }.Contains(wndWindowManager.Windows[i].ID))
                {
                    wndWindowManager.Windows.Remove(wndWindowManager.Windows[i]);
                }
            }

            if (!IsPostBack)
            {

                List<AsyncPageTask> taskList = new List<AsyncPageTask>();
                taskList.Add(new AsyncPageTask(GetGrades));
                taskList.Add(new AsyncPageTask(GetSubjects));

                foreach (AsyncPageTask page in taskList)
                {
                    PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "Resources", true);
                    Page.RegisterAsyncTask(newTask);
                }

                taskList = null;
                Page.ExecuteRegisteredAsyncTasks();

                BuildSchools();
                BuildGrades();
                BuildSubjects();
                BuildStudents();
                RefreshMTSSData();
            }

        }

        protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            _MTSSSession.Grade = e.Value;
            RefreshMTSSData();
        }

        protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            _MTSSSession.Subject = e.Value;
            RefreshMTSSData();

        }

        protected void cmbSchool_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            _MTSSSession.SchoolID = string.IsNullOrEmpty(e.Value) ? 0 : Int32.Parse(e.Value);
            BuildStudents();
            RefreshMTSSData();
        }

        protected void cmbStudents_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            _MTSSSession.StudentID = Int32.Parse(e.Value);
            MTSSAnalysiscmbStudents.Text = e.Text;
            RefreshMTSSData();

        }

        protected void refreshTile(object sender, EventArgs e)
        {
            BuildStudents();
            RefreshMTSSData();
        }

        #endregion

        #region Functions

        private void initializeSession()
        {
            //Kentico Objects
            ui = (UserInfo)Session[KenticoUserInfo];
            tree = KenticoHelper.GetUserTreeProvider(SessionObject.LoggedInUser.ToString());
            legacyUrl = ResolveUrl("~/SessionBridge.aspx");

            //MTSS Session
            if (Session[PageSessionStateName] == null)
            {
                _MTSSSession = new MTSSSessionObject();
                _MTSSSession.SchoolID = 0;
                _MTSSSession.Grade = String.Empty;
                _MTSSSession.Subject = String.Empty;
                _MTSSSession.StudentID = 0;
                _MTSSSession.UserPage = SessionObject.LoggedInUser.Page;
                _MTSSSession.UserCrossCourses = SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Courses);
                Session[PageSessionStateName] = _MTSSSession;

            }
            else
            {
                _MTSSSession = (MTSSSessionObject)Session[PageSessionStateName];

            }

            btnAdd.Attributes["onclick"] = "javascript:window.open('" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "pageactions.asp?action=route%26key=0%26formatoption=refresh%26retrievemode=page%26buttonid=7274|Add%20Class%26passthroughparms=yes%26??GRADE=null%26??SUBJECT=null%26??COURSE=null%26??SUBTYPE=null%26??SEM=null%26??PER=null%26??BLK=null%26??SCH=null%26??SEC=null%26??YEAR=%26??UPage=@UserPage%26??CC=','_blank')";
        }

        private void setTabSecurity()
        {
            if (_level == EntityTypes.District)
            {
                if (UserHasPermission(Permission.Tab_Inactive_RTIAnalysisTile_DistrictPortal))
                {
                    MTSSAnalysisInactiveView.Visible = true;
                    visibleTab = "Inactive";
                }

                if (UserHasPermission(Permission.Tab_Behavioral_RTIAnalysisTile_DistrictPortal))
                {
                    MTSSAnalysisBehavioralView.Visible = true;
                    visibleTab = "Behavioral";
                }

                if (UserHasPermission(Permission.Tab_Academic_RTIAnalysisTile_DistrictPortal))
                {
                    MTSSAnalysisAcademicView.Visible = true;
                    visibleTab = "Academic";
                }

            }
            else if (_level == EntityTypes.School)
            {
                if (UserHasPermission(Permission.Tab_Inactive_RTIAnalysisTile_DistrictPortal))
                {
                    MTSSAnalysisInactiveView.Visible = true;
                    visibleTab = "Inactive";
                }

                if (UserHasPermission(Permission.Tab_Behavioral_RTIAnalysisTile_SchoolPortal))
                {
                    MTSSAnalysisBehavioralView.Visible = true;
                    visibleTab = "Behavioral";
                }

                if (UserHasPermission(Permission.Tab_Academic_RTIAnalysisTile_SchoolPortal))
                {
                    MTSSAnalysisAcademicView.Visible = true;
                    visibleTab = "Academic";
                }
            }
            else if (_level == EntityTypes.Teacher)
            {
                if (UserHasPermission(Permission.Tab_Inactive_RTIAnalysisTile_TeacherPortal))
                {
                    MTSSAnalysisInactiveView.Visible = true;
                    visibleTab = "Inactive";
                }

                if (UserHasPermission(Permission.Tab_Behavioral_RTIAnalysisTile_TeacherPortal))
                {
                    MTSSAnalysisBehavioralView.Visible = true;
                    visibleTab = "Behavioral";
                }

                if (UserHasPermission(Permission.Tab_Academic_RTIAnalysisTile_TeacherPortal))
                {
                    MTSSAnalysisAcademicView.Visible = true;
                    visibleTab = "Academic";
                }

            }

            //Configure Active Tab/Add Button
            if (visibleTab != null)
            {
                MTSSAnalysisRadTabStrip.Tabs.FindTabByText(visibleTab).Selected = true;
            }
            else
            {
                //Disable Add button if no tabs enabled
                btnAdd.Style["Display"] = "None";
            }
        }

        private void RefreshMTSSData()
        {
            //School must be selected and at least one tab must be visible to refresh data
            if (_MTSSSession.SchoolID > 0 && visibleTab != null)
            {
                //Initialize Object
                userNodeList = new List<UserNodeList>();

                //Find Kentico ASPX pages that are hosting BizForms
                userNodeList = KenticoHelper.SearchDocumentTypeReferencesModified(ui, KenticoItemType, tree, "Forms");

                //Get Kentico Form Records
                List<KenticoMTSSFormData> kentico = KenticoHelper.GetKenticoFormData(userNodeList);

                //Call RTI Stored Procedure in E3 
                E3InterventionDataObject E3obj = MTSSHelper.getE3InterventionData(_MTSSSession.UserPage, _MTSSSession.UserCrossCourses, _MTSSSession.SchoolID, _MTSSSession.Grade, _MTSSSession.Subject, _MTSSSession.StudentID, Tier1);

                //Create HTML Table for tiles
                Table table = MTSSHelper.GenerateHTMLTableByIntervention(legacyUrl, kentico, E3obj, wndAddDocument, InterventionAcademic, true, _isFormColumnEnabled);
                if (table.Rows.Count <= 1)
                {
                    Panel p1 = new Panel();
                    p1.Controls.Add(new LiteralControl("No results found for selected criteria"));
                    p1.Style.Add("position", "relative");
                    p1.Style.Add("text-align", "top");
                    AcademicPlaceholder.Controls.Add(p1);

                }
                else
                {
                    AcademicPlaceholder.Controls.Add(table);
                }
                
                table = MTSSHelper.GenerateHTMLTableByIntervention(legacyUrl, kentico, E3obj, wndAddDocument, InterventionAll, false, _isFormColumnEnabled);
                if (table.Rows.Count <= 1)
                {
                    Panel p1 = new Panel();
                    p1.Controls.Add(new LiteralControl("No results found for selected criteria"));
                    p1.Style.Add("position", "relative");
                    p1.Style.Add("text-align", "top");
                    InactivePlaceholder.Controls.Add(p1);

                }
                else
                {
                    InactivePlaceholder.Controls.Add(table);
                }


                table = MTSSHelper.GenerateHTMLTableByIntervention(legacyUrl, kentico, E3obj, wndAddDocument, InterventionBehavioral, true, _isFormColumnEnabled);
                if (table.Rows.Count <= 1)
                {
                    Panel p1 = new Panel();
                    p1.Controls.Add(new LiteralControl("No results found for selected criteria"));
                    p1.Style.Add("position", "relative");
                    p1.Style.Add("text-align", "top");
                    BehavioralPlaceholder.Controls.Add(p1);

                }
                else
                {
                    BehavioralPlaceholder.Controls.Add(table);
                }



            }
            //Prompt user to select a school
            else
            {
                Panel p1 = new Panel();
                p1.Controls.Add(new LiteralControl("Please select a school"));
                p1.Style.Add("position", "relative");
                p1.Style.Add("top", "50%");
                p1.Style.Add("text-align", "center");

                Panel p2 = new Panel();
                p2.Controls.Add(new LiteralControl("Please select a school"));
                p2.Style.Add("position", "relative");
                p2.Style.Add("top", "50%");
                p2.Style.Add("text-align", "center");

                Panel p3 = new Panel();
                p3.Controls.Add(new LiteralControl("Please select a school"));
                p3.Style.Add("position", "relative");
                p3.Style.Add("top", "50%");
                p3.Style.Add("text-align", "center");

                AcademicPlaceholder.Controls.Add(p1);
                BehavioralPlaceholder.Controls.Add(p2);
                InactivePlaceholder.Controls.Add(p3);
            }


        }

        private void BuildStudents()
        {
            DataTable dtStudents = new DataTable();
            dtStudents.Columns.Add("StudentID", typeof(Int32));
            dtStudents.Columns.Add("StudentName");

            var allRow = dtStudents.NewRow();
            allRow["StudentID"] = 0;
            allRow["StudentName"] = "Student";
            dtStudents.Rows.InsertAt(allRow, 0);

            if (_MTSSSession.SchoolID > 0)
            {
                dtStudents.Merge(MTSSHelper.getE3InterventionStudents(_MTSSSession.UserPage, _MTSSSession.UserCrossCourses, _MTSSSession.SchoolID, _MTSSSession.Grade, _MTSSSession.Subject, Tier1));

            }


            // Data bind the combo box.
            MTSSAnalysiscmbStudents.DataTextField = "StudentName";
            MTSSAnalysiscmbStudents.DataValueField = "StudentID";
            MTSSAnalysiscmbStudents.DataSource = dtStudents;
            MTSSAnalysiscmbStudents.DataBind();

            // Initialize the current selection. 
            RadComboBoxItem item = this.MTSSAnalysiscmbStudents.Items.FindItemByValue(((int)_MTSSSession.StudentID).ToString(), true) ?? this.MTSSAnalysiscmbStudents.Items[0];
            _MTSSSession.StudentID = Int32.Parse(item.Value.CompareTo(String.Empty) == 0 ? "0" : item.Value);
            Int32 selIdx = MTSSAnalysiscmbStudents.Items.IndexOf(item);
            MTSSAnalysiscmbStudents.SelectedIndex = selIdx;

        }

        private void addNew(TreeProvider tp, TileParms tParms)
        {
            string theClientID = string.Empty;
            theClientID = DistrictParms.LoadDistrictParms().ClientID;

            if (theClientID != null && theClientID != string.Empty)
            {
                if (btnAdd.Visible)
                {
                    // if a district admin or a state admin then display options to choose, if niether then only one option and skip this form and move ahead to next
                    btnAdd.Attributes["onclick"] = legacyUrl + "pageactions.asp?action=route%26key=0%26formatoption=refresh%26retrievemode=page%26buttonid=7274|Add%20Class%26passthroughparms=yes%26??GRADE=null%26??SUBJECT=null%26??COURSE=null%26??SUBTYPE=null%26??SEM=null%26??PER=null%26??BLK=null%26??SCH=null%26??SEC=null%26??YEAR=%26??UPage=@UserPage%26??CC=";
                }
            }
        }

        private void GetGrades()
        {
            gradeList = _CurrCourseList.GetGradeList();
        }

        private void GetSubjects()
        {
            subjectList = _CurrCourseList.GetSubjectList().OrderBy(sub => sub.DisplayText);
        }

        protected void BuildGrades()
        {
            IEnumerable<Grade> gradeList;
            switch (_level)
            {
                case EntityTypes.Class:
                case EntityTypes.Teacher:
                case EntityTypes.District:
                    gradeList = _CurrCourseList.GetGradeList();
                    break;
                case EntityTypes.School:
                    gradeList = CourseMasterList.StandCourseList.GetGradeList();
                    break;
                default:
                    gradeList = _CurrCourseList.GetGradeList();
                    break;
            }

            DataTable dtGrade = new DataTable();
            dtGrade.Columns.Add("Grade", typeof(String));
            dtGrade.Columns.Add("CmbText", typeof(String));
            foreach (var g in gradeList)
                dtGrade.Rows.Add(g.DisplayText, g.DisplayText);


            DataRow newRow = dtGrade.NewRow();
            newRow["Grade"] = String.Empty;
            newRow["CmbText"] = "Grade";
            dtGrade.Rows.InsertAt(newRow, 0);

            // Data bind the combo box.
            MTSSAnalysiscmbGrade.DataTextField = "CmbText";
            MTSSAnalysiscmbGrade.DataValueField = "Grade";
            MTSSAnalysiscmbGrade.DataSource = dtGrade;
            MTSSAnalysiscmbGrade.DataBind();

            // Initialize the current selection. Sometimes the filter item no longer exists when changing
            // tabs from School, District, Classroom.
            RadComboBoxItem item = this.MTSSAnalysiscmbGrade.Items.FindItemByValue((String)_MTSSSession.Grade, true) ?? this.MTSSAnalysiscmbGrade.Items[0];
            _MTSSSession.Grade = item.Value;
            Int32 selIdx = MTSSAnalysiscmbGrade.Items.IndexOf(item);
            MTSSAnalysiscmbGrade.SelectedIndex = selIdx;

        }

        protected void BuildSubjects()
        {
            // load the filter button tables and databind.
            DataTable dtSubject = new DataTable();
            dtSubject.Columns.Add("Subject");
            dtSubject.Columns.Add("CmbText", typeof(String));

            foreach (var s in subjectList)
            {
                dtSubject.Rows.Add(s.DisplayText, s.DisplayText);
            }

            DataRow newRow = dtSubject.NewRow();
            newRow["Subject"] = String.Empty;
            newRow["CmbText"] = "Subject";
            dtSubject.Rows.InsertAt(newRow, 0);

            // Data bind the combo box.
            MTSSAnalysiscmbSubject.DataTextField = "CmbText";
            MTSSAnalysiscmbSubject.DataValueField = "Subject";
            MTSSAnalysiscmbSubject.DataSource = dtSubject;
            MTSSAnalysiscmbSubject.DataBind();

            // Initialize the current selection. Sometimes the filter item no longer exists when changing
            // tabs from School, District, Classroom.
            RadComboBoxItem item = this.MTSSAnalysiscmbSubject.Items.FindItemByValue((String)_MTSSSession.Subject, true) ?? this.MTSSAnalysiscmbSubject.Items[0];
            _MTSSSession.Subject = item.Value;
            Int32 selIdx = MTSSAnalysiscmbSubject.Items.IndexOf(item);
            MTSSAnalysiscmbSubject.SelectedIndex = selIdx;
        }

        protected void BuildSchools()
        {
            if (_level != EntityTypes.District)
            {
                allSchools = ThinkgateSchool.GetSchoolCollectionForUser(SessionObject.LoggedInUser);
            }
            else
            {
                allSchools = ThinkgateSchool.GetSchoolCollectionForApplication();
            }

            MTSSAnalysiscmbSchool.DataTextField = "Abbreviation";
            MTSSAnalysiscmbSchool.DataValueField = "ID";
            MTSSAnalysiscmbSchool.DataSource = allSchools;
            MTSSAnalysiscmbSchool.DataBind();

            // Initialize the current selection.
            if ((int)_MTSSSession.SchoolID > 0)
            {
                RadComboBoxItem item = MTSSAnalysiscmbSchool.Items.FindItemByValue(((int)_MTSSSession.SchoolID).ToString(), true);
                Int32 selIdx = MTSSAnalysiscmbSchool.Items.IndexOf(item);
                MTSSAnalysiscmbSchool.SelectedIndex = selIdx;
            }
        }

        private RadButton GetSelectedRadButtonByGroupName(string groupName)
        {
            return Controls.OfType<RadButton>().FirstOrDefault(radButton => radButton.Checked && String.Compare(radButton.GroupName, groupName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private List<int> GetCurriculumIDs(string grade, string subject, string course)
        {
            List<int> CurriculumIDs = new List<int>();

            DataSet ds = MCU.GetCurriculumCousesforStandards(grade, subject, course);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        CurriculumIDs.Add((int)row["ID"]);
                    }
                }
            }

            return CurriculumIDs;
        }


        #endregion
    }
}
