using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Classes.Search;
using Thinkgate.Domain.Classes;
using CultureInfo = System.Globalization.CultureInfo;

namespace Thinkgate.Controls.Documents
{
    public partial class GenericCMSDocumentTile : TileControlBase
    {
        #region Enumerations

        private enum CmsDocumentLocation
        {
            State = 0,
            District = 1,
            MyDocuments = 2
        }

        #endregion

        #region Constant Fields

        private const string ForwardSlash = "/";
        private const string ThinkgateResource = "thinkgate.resource";
        private const string ThinkgateUnitPlan = "thinkgate.UnitPlan";
        private const string ThinkgateCurriculumUnit = "thinkgate.curriculumUnit";
        private const string ThinkgateInstructionPlan = "thinkgate.InstructionalPlan";
        private const string ThinkgateLessonPlan = "thinkgate.LessonPlan";
        private const string ThinkgateCompetencyList = "Thinkgate.CompetencyList";
        private const string WebBased = "web-based";
        private const string Attachment = "attachment";
        private const string State = "state";

        private const String GradeFilterKey = "ReGradeFilterIdx";
        private const String SubjectFilterKey = "ReSubjectFilter";
        private const String CourseFilterKey = "ReCourseFilter";

        private const int DefaultNumberOfRecordsOnTile = 50;
       
        #endregion

        #region Readonly Fields

        private readonly string[] _gradeDefaultValues = { "All", "ALL", "all", "0", "grade", "Grade", "GRADE" };
        private readonly string[] _subjectDefaultValues = { "All", "ALL", "all", "0", "subject", "Subject", "SUBJECT" };
        private readonly string[] _courseDefaultValues = { "All", "ALL", "all", "0", "course", "Course", "COURSE" };

        private readonly string[] _defaultResourceTypeValues = { "All", "0" };
        private readonly string[] _defaultResourceSubTypeValues = { "All", "0" };

        #endregion

        #region Private Fields

        private List<int> _filterCurriculumIDs = new List<int>();

        private List<int> _filterStandardIDs = new List<int>();

        private TreeProvider _treeProvider;

        private List<UserNodeList> _userNodeList;

        private string _type;

        private IEnumerable<Subject> _subjectList;

        private SessionObject _session;

        private CourseList _currCourseList;

        #endregion

        #region Protected Fields

        protected EntityTypes Level;

        protected Int32 LevelId;

        protected bool ShowStateOnly;

        protected string TypeKeyPriv = string.Empty;

        protected Boolean IsSimulatedPostBack;

        protected bool IsCalledByUperCarousel;

        #endregion

        #region Public Fields

        public string ResourceToShow;

        public string[] StrResourcesToShowOnTile;

        public string ResourceForFolder;

        public string TypeKey;
        
        public int ClientBaseLookupEnum;        

        public static string PortalName { get; set; }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles Tile Initialization
        /// </summary>
        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (IsTileInvalid())
            {
                return;
            }

            GetParams();
            InitializeTileValues();
            CMSContext.Init();
        }

        /// <summary>
        /// Loads data and populates controls on Tile
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
           

            PortalName = SessionObject.CurrentPortal.ToString();
            SetupControl();
            SetupDialogs();
            RemoveAllUnneededWindows();

            if (IsTilePopulatable())
            {
                if (System.Web.Configuration.WebConfigurationManager.AppSettings["INFOhioLink"] != null)
                {
                    hLinkOhio.NavigateUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["INFOhioLink"];
                }
                PopulateTile();
            }            
        }

        /// <summary>
        /// Event for OK click on third dialog encountered after Add button has been clicked on an
        /// Instructional Material tile.
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Standard event args</param>
        protected void BtnOkTypeOnClick(object sender, EventArgs e)
        {
            try
            {
                ProcessButtonClick();
            }

            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while adding resource.", ex);
            }
        }

        protected void cmbDocumentList_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                SearchDocumentTypeReferences();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while selecting document.", ex);
            }
        }

        protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                ViewState[GradeFilterKey] = e.Value;
                BuildCourses();
                SearchDocumentTypeReferences();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while selecting grade.", ex);
            }
        }

        protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                ViewState[SubjectFilterKey] = e.Value;
                BuildCourses();
                SearchDocumentTypeReferences();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while selecting subject.", ex);
            }
        }

        protected void cmbCourse_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                ViewState[CourseFilterKey] = e.Value;
                BuildCourses();
                SearchDocumentTypeReferences();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while selecting course.", ex);
            }
        }

        protected void cmbResourceType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                ProcessResourceSelectionChange(e);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while selecting resource.", ex);
            }
        }

        protected void cmbResourceSubType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(e.Value))
                {
                    SearchDocumentTypeReferences();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured while selecting sub type.", ex);
            }
        }

        #endregion

        #region Public Methods

        public bool RecordExistsInCache(string key)
        {
            return (Base.Classes.Cache.Get(key) != null);
        }

        #endregion

        #region Protected Methods

        protected void BuildGrades()
        {
            IEnumerable<Grade> gradeList;
            switch (Level)
            {
                case EntityTypes.Class:
                case EntityTypes.Teacher:
                case EntityTypes.District:
                    gradeList = _currCourseList.GetGradeList();
                    break;
                case EntityTypes.School:
                    gradeList = CourseMasterList.StandCourseList.GetGradeList();
                    break;
                default:
                    gradeList = _currCourseList.GetGradeList();
                    break;
            }

            DataTable dtGrade = new DataTable();
            dtGrade.Columns.Add("Grade", typeof(String));
            dtGrade.Columns.Add("CmbText", typeof(String));
            foreach (var g in gradeList)
                dtGrade.Rows.Add(g.DisplayText, g.DisplayText);

            DataRow newRow = dtGrade.NewRow();
            newRow["Grade"] = "All";
            newRow["CmbText"] = "Grade";
            dtGrade.Rows.InsertAt(newRow, 0);

            // Data bind the combo box.
           
            cmbGrade.DataTextField = "CmbText";
            cmbGrade.DataValueField = "Grade";
            cmbGrade.DataSource = dtGrade;
            cmbGrade.DataBind();
            cmbGrade.ClearSelection();
            cmbGrade.SelectedIndex = 0;

        }

        protected void BuildSubjects()
        {
            // load the filter button tables and databind.
            DataTable dtSubject = new DataTable();
            dtSubject.Columns.Add("Subject");
            dtSubject.Columns.Add("CmbText", typeof(String));

            foreach (var s in _subjectList)
            {
                dtSubject.Rows.Add(s.DisplayText, s.DisplayText);
            }

            DataRow newRow = dtSubject.NewRow();
            newRow["Subject"] = "All";
            newRow["CmbText"] = "Subject";
            dtSubject.Rows.InsertAt(newRow, 0);

            // Data bind the combo box.
            cmbSubject.Items.Clear();
            cmbSubject.DataTextField = "CmbText";
            cmbSubject.DataValueField = "Subject";
            cmbSubject.DataSource = dtSubject;
            cmbSubject.DataBind();
            cmbSubject.ClearSelection();
            cmbSubject.SelectedIndex = 0;

        }

        protected string GetClientDocumentType(string docName, string formType)
        {
            DataSet resourceToShow = KenticoHelper.GetTileMapLookupDataSet(docName);

            if (resourceToShow.Tables[0].Rows.Count > 0)
            {
                if (resourceToShow.Tables[0].Rows.Count == 1)
                    return (resourceToShow.Tables[0].Rows[0]["KenticoDocumentTypeToShow"].ToString());

                if (!string.IsNullOrEmpty(formType))
                {
                    DataRow selectRow = resourceToShow.Tables[0].Select("FriendlyName like'" + formType + "'").FirstOrDefault();
                    if (selectRow != null)
                        return selectRow["KenticoDocumentTypeToShow"].ToString();
                }
            }

            return docName;
        }

        #endregion

        #region Private Methods

        #region Initialization Helper Methods

        private bool IsTileInvalid()
        {
            return Tile == null || Tile.TileParms == null;
            }

        private void GetParams()
        {
            if (Tile.TileParms.GetParm("type") != null)
            {
                _type = Tile.TileParms.GetParm("type").ToString();
            }

            if (Tile.TileParms.GetParm("level") != null)
            {
                Level = (EntityTypes)Tile.TileParms.GetParm("level");
            }

            if (Tile.TileParms.GetParm("levelID") != null)
            {
                LevelId = (Int32)Tile.TileParms.GetParm("levelID");
            }

            if (Tile.TileParms.GetParm("showStateOnly") != null)
            {
                ShowStateOnly = (bool)Tile.TileParms.GetParm("showStateOnly");
            }

            if (Tile.TileParms.GetParm("typeKey") != null)
            {
                TypeKeyPriv = Tile.TileParms.GetParm("typeKey").ToString();
            }
            if (Tile.TileParms.GetParm("CallingByUperCarousel") != null)
            {
                IsCalledByUperCarousel = (bool)Tile.TileParms.GetParm("CallingByUperCarousel");
            }
        }

        private void InitializeTileValues()
        {
            btnAdd.Attributes["title"] = string.Format("Add New {0}", Tile.Title);

            _currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
        }

        private bool IsTilePopulatable()
        {
            return !IsPostBack || (ResourceForFolder != "" && cmbUserType.SelectedValue == "") || IsCalledByUperCarousel;
        }

        private void PopulateTile()
        {
            AddNew(ResourceToShow);
            PopulateResourceTypes(ResourceToShow);
            SetInitialViewstateValues();
            ExecuteAsyncTasks();
            PopulateDropDownLists();
            SearchDocumentTypeReferences();
            
            
            
        }

        private void PopulateDropDownLists()
        {
            SimulatePostback();

            if (!IsSimulatedPostBack)
            {
                if (IsTileInTeacherMode())
                {
                    HideGradeSubjectCourseDropDownLists();

                    if (IsShowingResources())
                    {
                        ShowResourceDropDownLists();
                        BindResourceDropdownData();
                    }
                }
                else
                {
                    BindGradeSubjectCourseData();
                }
            }
        }

        private void BindGradeSubjectCourseData()
        {
            BuildGrades();
            BuildSubjects();
            BuildCourses();
        }

        private void BindResourceDropdownData()
        {
            BindResourceTypes();
            BindDefaultResourceSubType();
        }

        private void ShowResourceDropDownLists()
        {
            cmbResourceType.Visible = true;
            cmbResourceSubType.Visible = true;
        }

        private bool IsShowingResources()
        {
            return ResourceToShow == ThinkgateResource;
        }

        private bool IsNotShowingCompetencyList()
        {
            return ResourceToShow != ThinkgateCompetencyList;
        }

        private void HideGradeSubjectCourseDropDownLists()
        {
            cmbGrade.Visible = false;
            cmbSubject.Visible = false;
            cmbCourse.Visible = false;
        }

        private bool IsTileInTeacherMode()
        {
            return IsUserATeacher() && IsNotShowingCompetencyList();
        }

        private void SimulatePostback()
        {
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);

            IsSimulatedPostBack = !String.IsNullOrEmpty(postBackControlID)
                          && !postBackControlID.StartsWith("folder")
                          && !postBackControlID.StartsWith("tileContainer");
        }

        private void SetInitialViewstateValues()
        {
            if (ViewState[GradeFilterKey] == null)
            {
                ViewState.Add(GradeFilterKey, "All");
                ViewState.Add(SubjectFilterKey, "All");
                ViewState.Add(CourseFilterKey, "All");
            }
        }

        private void ExecuteAsyncTasks()
        {
            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            if (!IsSimulatedPostBack)
            {
                taskList.Add(new AsyncPageTask(GetSubjects));
                taskList.Add(new AsyncPageTask(BuildCourses));
            }

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "Resources", true);
                Page.RegisterAsyncTask(newTask);
            }

            Page.ExecuteRegisteredAsyncTasks();
        }

        private void RemoveAllUnneededWindows()
        {
            for (int i = wndWindowManager.Windows.Count - 1; i > -1; i--)
            {
                if (!new List<string> { "wndAddDocument", "wndCmsNewDocumentShell" }.Contains(wndWindowManager.Windows[i].ID))
                {
                    wndWindowManager.Windows.Remove(wndWindowManager.Windows[i]);
                }
            }
        }

        private void SetupControl()
        {
            SetSessionObject();
            SetResourceTypesToShow();
            SetResourceFolder();
            SetTreeProvider();
        }

        private void SetupDialogs()
        {
            SetupCreateDocumentTypeDialog();
            SetupCreateNewDocumentDialog();
            SetupAddNewExistingDialog();
        }

        private void SetResourceFolder()
        {
            ResourceForFolder = Tile.TileParms.GetParm("resourceForFolder") == null
                ? ""
                : (string)Tile.TileParms.GetParm("resourceForFolder");
        }

        private void SetTreeProvider()
        {
            _treeProvider = KenticoHelper.GetUserTreeProvider(SessionObject.LoggedInUser.ToString());
        }

        private void SetResourceTypesToShow()
        {
            ResourceToShow = (string)Tile.TileParms.GetParm("resourceToShow");

            DataSet resourcesToShow = KenticoHelper.GetTileMapLookupDataSet(ResourceToShow);

            StrResourcesToShowOnTile = resourcesToShow.Tables[0]
                .AsEnumerable()
                .OrderBy(p => p["ItemOrder"].ToString())
                .Select(s => s.Field<string>("KenticoDocumentTypeToShow"))
                .ToArray();
        }

        private void SetSessionObject()
        {
            _session = (SessionObject)Session["SessionObject"];
        }

        private void SetupCreateDocumentTypeDialog()
        {
            int lastSIndex = Tile.Title.LastIndexOf('s');
            string titleName = Tile.Title;

            if (lastSIndex > 0 && titleName != "Lesson Plan")
            {
                titleName = Tile.Title.Remove(lastSIndex);                
            }

            btnOkType.Attributes["onclick"] = string.Format("javascript:validateTypeSubtye('{0}',$find('{1}'));", hdnCmsDocumentLocation.ClientID, wndAddDocument.ClientID);

            //btnCancelType.OnClientClicked = "closeCmsDialogUsingCommandArgument";
            btnCancelType.Attributes["onclick"] =
            string.Format(
                "javascript:setCmsDocumentLocationShared('{2}',$find('{3}'),'{4}','{5}','{1}','{7}','{6}','{0}');"
                , divAddWhere.ClientID
                , divTypeSubtype.ClientID
                , hdnCmsDocumentLocation.ClientID
                , wndAddDocument.ClientID
                , rdoMyDocuments.ClientID
                , titleName
                , rdoShared.ClientID
                , divAddNewOrExisting.ClientID);

            btnCancelType.CommandArgument = wndAddDocument.ClientID;

            BindTypes();
            BindSubTypes();
            BindFormTypes();
        }

        private void SetupCreateNewDocumentDialog()
        {
            string clientId = DistrictParms.LoadDistrictParms().ClientID;
            rdoDistrict.Text = "District (" + clientId + ")";
            var session = (SessionObject)Session["SessionObject"];
            btnCancelWhere.OnClientClicked = "closeCmsDialogUsingCommandArgument";

            btnCancelWhere.CommandArgument = wndAddDocument.ClientID;

            
            bool skipTypeSubTypeDialog = ResourceToShow == ThinkgateCompetencyList;

               btnOkWhere.Attributes["onclick"] =
                string.Format(
                         "javascript:setupCmsDocumentLocations('{2}',$find('{3}'),'{0}','{1}','{4}','{5}','{6}','{7}','{8}');"
                      , divAddWhere.ClientID
                       , divAddNewOrExisting.ClientID
                    , hdnCmsDocumentLocation.ClientID
                       , wndAddDocument.ClientID
                       , rdoShared.ClientID
                       , divTypeSubtype.ClientID
                       , rdoAddExisting.ClientID
                       , rdoAddNew.ClientID, skipTypeSubTypeDialog);
           
            //US15667
            if (!session.LoggedInUser.HasPermission(Permission.Create_State_Folder_IM))
            {
                rdoState.Visible = false;                
                litState.Text = ""; //using it to control line break between radiobuttons
            }            
            
            if (!session.LoggedInUser.HasPermission(Permission.Create_District_Folder_IM))
            {
                rdoDistrict.Visible = false;
                litDistrict.Text = ""; //using it to control line break between radiobuttons
            }
            
            if (!session.LoggedInUser.HasPermission(Permission.Create_Access_Shared_IM))
            {
                rdoShared.Visible = false;
            }
            
        }

        private void SetupAddNewExistingDialog()
        {
            int lastSIndex = Tile.Title.LastIndexOf('s');
            string titleName = Tile.Title;

            if (lastSIndex > 0 && titleName != "Lesson Plan")
            {
                titleName = Tile.Title.Remove(lastSIndex);                
            }

            wndAddDocument.Title = string.Format("Add New {0}", titleName);

            if (ResourceToShow == ThinkgateCompetencyList)
            {
                rdoAddExisting.Text = "Use an Existing List";
                rdoAddNew.Text = "Add New List";
            }

         //   btnCancelNew.OnClientClicked = "closeCmsDialogUsingCommandArgument";
            btnCancelNew.CommandArgument = wndAddDocument.ClientID;

                if (ResourceToShow == ThinkgateCompetencyList)
                    btnOkNew.Attributes["onclick"] = string.Format("javascript:triggerBtnOkTypeClick('{0}',$find('{1}'));", hdnCmsDocumentLocation.ClientID, wndAddDocument.ClientID);
                else
                    btnOkNew.Attributes["onclick"] =
                       string.Format(
                           "javascript:setCmsDocumentLocations('{2}',$find('{3}'),'{0}','{1}');"
                           , divAddNewOrExisting.ClientID
                           , divTypeSubtype.ClientID
                           , hdnCmsDocumentLocation.ClientID
                           , wndAddDocument.ClientID);
            
            btnCancelNew.Attributes["onclick"] =
                    string.Format(
                   "javascript:setCmsDocumentLocation('{2}',$find('{3}'),'{4}','{5}');document.getElementById('{0}').style.display = 'block';document.getElementById('{1}').style.display = 'none';"
                        , divAddWhere.ClientID
                        , divAddNewOrExisting.ClientID
                        , hdnCmsDocumentLocation.ClientID
                        , wndAddDocument.ClientID
                        , rdoMyDocuments.ClientID
                        , titleName
                        );

        }

        private void BindTypes()
        {
            if (!IsPostBack)
            {
                List<LookupDetails> types = KenticoHelper.GetLookupDetailsByClient("LookupEnum=" + ((int)LookupType.DocumentType), "Enum ASC");

                if (ResourceToShow == ThinkgateResource && types.Count > 0)
                    types = types.Where(x => !x.Description.ToLower().Contains(State)).ToList(); //Remove State descriptions from the Resource list

                if (types != null && types.Count > 0)
                {
                    types.ForEach(ld => ld.DropdownText = ld.Description);
                    LookupDetails all = new LookupDetails();
                    all.Enum = 0;
                    all.DropdownText = "-- Select --";
                    all.Description = "-- Select --";
                    all.Enum = 0;
                    types.Insert(0, all);


                    ddlType.DataValueField = "Enum";
                    ddlType.DataTextField = "Description";
                    ddlType.DataSource = types;
                    ddlType.DataBind();
                }
            }

            if (Tile.TileParms.Parms.ContainsKey("typeKey"))
            {
                TypeKey = Tile.TileParms.GetParm("typeKey").ToString();

                if (ddlType.Items.FindByValue(TypeKey) != null)
                {
                    if (TypeKey != ((int)LookupDetail.Resource).ToString(CultureInfo.CurrentCulture))
                    {
                        ddlType.Items.FindByValue(TypeKey).Selected = true;
                        ddlType.Enabled = false;
                    }
                }
            }
        }

        private void BindSubTypes()
        {
            if (!IsPostBack)
            {
                List<LookupDetails> resourceSubTypes = new List<LookupDetails>();

                if (TypeKey != null)
                {
                    if (TypeKey != ((int)LookupDetail.Resource).ToString(CultureInfo.CurrentCulture))
                    {
                        resourceSubTypes = KenticoHelper.GetLookupDetailsByClient("LookupEnum=" + TypeKey, "Enum ASC");
                    }
                }

                LookupDetails all = new LookupDetails();
                all.Enum = 0;
                all.DropdownText = "-- Select --";
                all.Description = "-- Select --";
                resourceSubTypes.Insert(0, all);

                ddlSubType.DataValueField = "Enum";
                ddlSubType.DataTextField = "Description";
                ddlSubType.DataSource = resourceSubTypes;
                ddlSubType.DataBind();
            }
        }

        private void BindFormTypes()
        {
            ResourceToShow = (string)Tile.TileParms.GetParm("resourceToShow");

            DataSet tileMapDataSet = KenticoHelper.GetTileMapLookupDataSet(ResourceToShow);
            var resultsubtype = (from s in tileMapDataSet.Tables[0].AsEnumerable()
                                 select new KeyValuePair { Key = s["FriendlyName"].ToString(), Value = s["FriendlyName"].ToString() }).ToList();

            resultsubtype = resultsubtype.OrderBy(v => v.Value).ToList();

            ddlFormType.DataValueField = "Key";
            ddlFormType.DataTextField = "Value";
            ddlFormType.DataSource = resultsubtype;
            ddlFormType.DataBind();
        }

        private void BindResourceTypes()
        {
            List<LookupDetails> resourceTypes = KenticoHelper.GetLookupDetailsByClient("LookupEnum=" + ((int)LookupType.DocumentType), "Description ASC");

            if (resourceTypes != null && resourceTypes.Count > 0)
            {
                resourceTypes.ForEach(ld => ld.DropdownText = ld.Description);
                LookupDetails all = new LookupDetails();
                all.Enum = 0;
                all.DropdownText = "All";
                all.Description = "Type";
                resourceTypes.Insert(0, all);

                cmbResourceType.DataValueField = "Enum";
                cmbResourceType.DataTextField = "Description";
                cmbResourceType.DataSource = resourceTypes;
                cmbResourceType.DataBind();
            }
        }

        private void BindDefaultResourceSubType()
        {
            List<LookupDetails> resourceSubTypes = new List<LookupDetails>();
            LookupDetails all = new LookupDetails();
            all.Enum = 0;
            all.DropdownText = "All";
            all.Description = "SubType";
            resourceSubTypes.Insert(0, all);

            cmbResourceSubType.DataValueField = "Enum";
            cmbResourceSubType.DataTextField = "Description";
            cmbResourceSubType.DataSource = resourceSubTypes;
            cmbResourceSubType.DataBind();
        }

        private void PopulateResourceTypes(string resourceToShow)
        {
            var lists = new List<TelerikList>();

            string clientId = DistrictParms.LoadDistrictParms().ClientID;

            if (IsStateModelCurriculumTile(resourceToShow))
            {
                cmbUserType.Visible = false;
                lists.Add(new TelerikList { Name = "State", Value = ResourceTypes.StateDocuments.ToString() });
            }
            else
            {
                lists.Add(new TelerikList { Name = "My Docs", Value = ResourceTypes.MyDocuments.ToString() });
                lists.Add(new TelerikList { Name = "District (" + clientId + ")", Value = ResourceTypes.DistrictDocuments.ToString() });
                lists.Add(new TelerikList { Name = "Shared", Value = ResourceTypes.SharedDocuments.ToString() }); //US15667
                lists.Add(new TelerikList { Name = "State", Value = ResourceTypes.StateDocuments.ToString() });
            }

            cmbUserType.Items.Clear();
            cmbUserType.DataSource = lists.ToList();
            cmbUserType.DataValueField = "Value";
            cmbUserType.DataTextField = "Name";
            cmbUserType.DataBind();
            cmbUserType.ClearSelection();
            cmbUserType.SelectedIndex = 0;
        }

        private bool IsStateModelCurriculumTile(string resourceToShow)
        {
            return (resourceToShow == ThinkgateUnitPlan || 
                    (resourceToShow == ThinkgateCurriculumUnit 
                     && !_session.LoggedInUser.HasPermission(Permission.Create_State_Folder_IM))) && ShowStateOnly;
        }

        private void AddNew(string resourceToShow)
        {
            btnAdd.Visible = true; // might need to disable via E3 roles later

            LookupDetail enumlkpDetail;
            if (TypeKeyPriv == string.Empty)
            {
                btnAdd.Visible = false;
            }
            else if (Enum.TryParse(TypeKeyPriv, true, out enumlkpDetail))
            {
                if (enumlkpDetail == LookupDetail.Resource && UserHasPermission(Permission.Icon_Add_Resource))
                {
                    btnAdd.Visible = true;
                }
            }

            // Mass should hit this path for the State Model Curriclum Units tile
            if (resourceToShow == ThinkgateUnitPlan && ShowStateOnly)
            {
                btnAdd.Visible = false;
            }

            // Ohio should hit this path for the State Model Curriclum tile
            if (resourceToShow == ThinkgateCurriculumUnit && ShowStateOnly && !UserHasPermission(Permission.Icon_Add_StateModelCurriculum))
            {
                btnAdd.Visible = false;
            }

            if (resourceToShow == ThinkgateCompetencyList && UserHasPermission(Permission.Icon_AddNew_Competencylist))
            {
                {
                    btnAdd.Visible = true;
                }
            }
            if (resourceToShow == ThinkgateResource && UserHasPermission(Permission.Show_INFOhio_Resources_Link_IM))
            {
                divOhioLink.Visible = true;
            }

            if (btnAdd.Visible)
            {
                // if a district admin or a state admin then display options to choose, if niether then only one option and skip this form and move ahead to next
                string titlekey = string.Empty;
                if (Tile.TileParms.Parms.ContainsKey("typeKey"))
                {
                    titlekey = Tile.TileParms.GetParm("typeKey").ToString();
                }

                hdnCmsDocumentLocation.Value = "2";
                btnAdd.Attributes["onclick"] = string.Format("javascript:openCmsDialogWindows($find('{0}'),'{1}','{2}','{3}');", wndAddDocument.ClientID, btnOkType.ClientID, titlekey,rdoMyDocuments.ClientID);
            }
        }

        private void GetSubjects()
        {
            _subjectList = _currCourseList.GetSubjectList().OrderBy(sub => sub.DisplayText);
        }

        private void BuildCourses()
        {
            // Now load the filter button tables and databind.
            DataTable dtCourse = new DataTable();
            dtCourse.Columns.Add("Course");
            dtCourse.Columns.Add("CmbText", typeof(String));
            List<string> gradeList = null;
            List<string> subjectList = null;

            if (ViewState[GradeFilterKey].ToString() != "All" && ViewState[GradeFilterKey].ToString() != "")
            {
                gradeList = new List<string>();
                gradeList.Add(ViewState[GradeFilterKey].ToString());
            }

            if (ViewState[SubjectFilterKey].ToString() != "All" && ViewState[SubjectFilterKey].ToString() != "")
            {
                subjectList = new List<string>();
                subjectList.Add(ViewState[SubjectFilterKey].ToString());
            }

            var courseList = Level == EntityTypes.Teacher ? CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(gradeList, subjectList).GetCourseNames().ToList() : CourseMasterList.CurrCourseList.FilterByGradesAndSubjects(gradeList, subjectList).GetCourseNames().ToList();
            courseList.Sort();

            foreach (var c in courseList)
            {
                dtCourse.Rows.Add(c, c);
            }

            DataRow newRow = dtCourse.NewRow();
            newRow["Course"] = "All";
            newRow["CmbText"] = "Course";
            dtCourse.Rows.InsertAt(newRow, 0);

            // Data bind the combo box.
            cmbCourse.DataTextField = "CmbText";
            cmbCourse.DataValueField = "Course";
            cmbCourse.DataSource = dtCourse;
            cmbCourse.DataBind();
            cmbCourse.ClearSelection();
            cmbCourse.SelectedIndex = 0;

            // Initialize the current selection. Sometimes the filter item no longer exists when changing
            // tabs from School, District, Classroom.
            RadComboBoxItem item = cmbCourse.Items.FindItemByValue((String)ViewState[CourseFilterKey], true) ?? cmbCourse.Items[0];
            ViewState[CourseFilterKey] = item.Value;
            Int32 selIdx = cmbCourse.Items.IndexOf(item);
            cmbCourse.SelectedIndex = selIdx;
        }

        /// <summary>
        /// Returns a dataset of curriculums based on grade, subject and course from standards table
        /// </summary>
        private List<int> GetStandardIDsformStandard(string standardSet, string grade, string subject, string course, string standardName, string level)
        {
            List<int> standardIds = new List<int>();
            DataTable dt = MCU.GetStandardIdsforStandards(standardSet, grade, subject, course, standardName, level);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                            standardIds.Add((int)row["ID"]);
                    }
                }
            return standardIds;
                        }

        /// <summary>
        /// Filters the list results based on dropdown filters
        /// </summary>
        private IEnumerable<int> FilterByGradeSubjectAndCourse()
        {
            List<string> grade = new List<string>();
                if (cmbGrade.SelectedItem.Text.ToLower() != "grade")   
                grade.Add(cmbGrade.SelectedItem.Text);
            List<string> subject = new List<string>();
                if (cmbSubject.SelectedItem.Text.ToLower() != "subject")
                subject.Add(cmbSubject.SelectedItem.Text);
            List<string> course = new List<string>();
                if (cmbCourse.SelectedItem.Text.ToLower() != "course")
                course.Add(cmbCourse.SelectedItem.Text);

            return
                CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser)
                                .FilterByGradesSubjectsAndCourse(grade, subject, course).GetCourseIds().ToList();

        }

        /// <summary>
        /// Filters the list results based on dropdown filters
        /// </summary>
        private IEnumerable<int> FilterListByStandard(Base.Classes.Standards standards)
        {
            List<int> standardIds = new List<int>();
            if (standards != null)
            {
                string grade = standards.Grade;
                string subject = standards.Subject;
                string course = standards.Course;
                string standardSet = standards.Standard_Set;
                string standardName = standards.StandardName;
                string level = standards.Level;


               standardIds = GetStandardIDsformStandard(standardSet,grade, subject, course, standardName,level);
            }
            return standardIds;
        }

        private IEnumerable<int> FilterListByTeacher()
                {
            return CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser).GetCourseIds().ToList();

        }

        #endregion

        #region Addding Resource Helper Methods

        private void ProcessButtonClick()
        {
            if (rdoAddExisting.Checked)
            {
                AddExistingResource();
            }
            else
            {
                AddNewResource();
            }
        }

        private void AddNewResource()
        {
            string resourceToShow = (string)Tile.TileParms.GetParm("resourceToShow");
            string resourceTitle = (string)Tile.TileParms.GetParm("title");
            string userName = string.Empty;
            string clientId = DistrictParms.LoadDistrictParms().ClientID;
            string userAliasPath = string.Empty;
            CmsDocumentLocation selectedCmsDocumentLocation;
            string fromtypevalue = hdnFormTypes.Value;

            var nameCalculator = new KenticoNameCalculator();
            string kenticoUserName = nameCalculator.RemoveIllegalCharsFromGroupName(SessionObject.LoggedInUser.UserName);

            if (Enum.TryParse(hdnCmsDocumentLocation.Value, true, out selectedCmsDocumentLocation))
            {
                switch (selectedCmsDocumentLocation)
                {
                    case CmsDocumentLocation.State:
                        userAliasPath = String.Concat(ForwardSlash, KenticoHelper.GetKenticoMainFolderName(clientId), ForwardSlash,
                            "Documents");
                        break;
                    case CmsDocumentLocation.District:
                        userName = clientId;
                        userAliasPath = String.Concat(ForwardSlash, KenticoHelper.GetKenticoMainFolderName(clientId), ForwardSlash,
                            "Districts");
                        break;
                    case CmsDocumentLocation.MyDocuments:
                        userName = String.Concat(clientId, "-", kenticoUserName);
                        userAliasPath = String.Concat(ForwardSlash, KenticoHelper.GetKenticoMainFolderName(clientId), ForwardSlash, "Users");
                        break;
                }
            }
            else
            {
                throw new Exception(
                    string.Format("CMS Document Location could not be determined. Valid values are {0}, {1}, and {2}"
                        , Enum.GetName(typeof(CmsDocumentLocation), CmsDocumentLocation.State)
                        , Enum.GetName(typeof(CmsDocumentLocation), CmsDocumentLocation.District)
                        , Enum.GetName(typeof(CmsDocumentLocation), CmsDocumentLocation.MyDocuments)));
            }

            int theUsersBaseTreeNodeId =
                KenticoHelper.GetUsersBaseTreeNodeID(string.Empty, userAliasPath, userName, _treeProvider);

            if (theUsersBaseTreeNodeId <= 0)
            {
                throw new Exception(String.Format("The Folder for {0} could not be found.",
                    Enum.GetName(typeof(CmsDocumentLocation), selectedCmsDocumentLocation)));
            }

            if (IsNotShowingCompetencyList())
            {
                resourceToShow = DeriveResourceToShow(resourceToShow);
            }

            resourceToShow = GetClientDocumentType(resourceToShow, fromtypevalue);

            // the new form (class) must be setup under a parent class that allows it - this table will show what children classes can be under a parent [Kentico7].[dbo].[CMS_AllowedChildClasses]
            string kenticoUrl = KenticoHelper.GetFormURL(resourceToShow,
                theUsersBaseTreeNodeId.ToString(CultureInfo.InvariantCulture));

            //To pre-populate Type and SubType in kentico form
            if (IsNotShowingCompetencyList())
            {
                kenticoUrl += "&Type=" + ddlType.SelectedValue + "&SubType=" + hdnSubTypes.Value;
            }

            var newWindow = new RadWindow
            {
                ID =
                    Tile.TileParms.Parms.ContainsKey("typeKey")
                        ? "CmsNewDocument" + Tile.TileParms.Parms.ContainsKey("typeKey")
                        : "CmsNewDocument",
                NavigateUrl = kenticoUrl,
                Modal = true,
                VisibleOnPageLoad = true,
                ShowContentDuringLoad = false,
                VisibleStatusbar = false,
                Width = new Unit(1200, UnitType.Pixel),
                Height = new Unit(660, UnitType.Pixel),
                Title = string.Concat("Add New ", resourceTitle),
                Skin = "Web20",
                Behaviors = WindowBehaviors.Close | WindowBehaviors.Move | WindowBehaviors.Resize,
                OnClientBeforeClose = "OnClientBeforeClose",
                OnClientClose = "closeCmsDialogDoPostBack"
            };

            wndWindowManager.Windows.Add(newWindow);
        }

        private void AddExistingResource()
        {
            string resourceToShow = (string)Tile.TileParms.GetParm("resourceToShow");
            string resourceTitle = (string)Tile.TileParms.GetParm("title");
            string clientId = DistrictParms.LoadDistrictParms().ClientID;
            string userAliasPath;
            int theUsersBaseTreeNodeId;
            string userName = string.Empty;
            string subtypevalue = ddlSubType.SelectedValue;
            //US15667
            string isShared = "false";
            
            CmsDocumentLocation selectedCmsDocumentLocation;
            Enum.TryParse(hdnCmsDocumentLocation.Value, true, out selectedCmsDocumentLocation);

            var nameCalculator = new KenticoNameCalculator();
            string kenticoUserName = nameCalculator.RemoveIllegalCharsFromGroupName(SessionObject.LoggedInUser.UserName);

            if (rdoDistrict.Checked)
            {
                userName = clientId;
                userAliasPath = BuildAliasPath(clientId, "Districts");
                theUsersBaseTreeNodeId =
                    KenticoHelper.GetUsersBaseTreeNodeID(clientId, userAliasPath, userName, _treeProvider);
            }
            else if (rdoState.Checked)
            {
                userAliasPath = BuildAliasPath(clientId, "Documents");
                theUsersBaseTreeNodeId =
                    KenticoHelper.GetUsersBaseTreeNodeID(clientId, userAliasPath, userName, _treeProvider);
            }
            else if (rdoShared.Checked) //US15667
            {                
                isShared = "true";
                string environmentRoot = "/" + KenticoHelper.GetKenticoMainFolderName(clientId) + "/" + "Shared" ;
                userAliasPath = String.Concat(ForwardSlash, KenticoHelper.GetKenticoMainFolderName(clientId), ForwardSlash,
                    "Shared", ForwardSlash, clientId);
                theUsersBaseTreeNodeId = KenticoHelper.GetEnvironmentBaseTreeNodeID(clientId, userAliasPath, KenticoHelper.GetKenticoMainFolderName(clientId), _treeProvider,  environmentRoot);
                
                if (theUsersBaseTreeNodeId <= 0)
                {
                    throw new Exception(String.Format("The Folder for {0} could not be found.",
                        Enum.GetName(typeof(CmsDocumentLocation), selectedCmsDocumentLocation)));
                }
            }
            else
            {
                userName = String.Concat(clientId, "-", kenticoUserName);
                userAliasPath = BuildAliasPath(clientId, "Users");
                theUsersBaseTreeNodeId =
                    KenticoHelper.GetUsersBaseTreeNodeID(clientId, userAliasPath, userName, _treeProvider);
            }

            if (IsNotShowingCompetencyList())
            {
                if (subtypevalue == "0")
                {
                    subtypevalue = hdnSubTypes.Value;
                }

                resourceToShow = DeriveResourceToShow(resourceToShow);

                PopAddExistingSearchWindow(resourceTitle, resourceToShow, theUsersBaseTreeNodeId, subtypevalue,isShared);
            }
            else
            {
                PopAddExistingSearchWindowCl(resourceTitle, resourceToShow, theUsersBaseTreeNodeId);
            }
        }

        private string BuildAliasPath(string clientId, string postFix)
        {
            return String.Concat(ForwardSlash, KenticoHelper.GetKenticoMainFolderName(clientId), ForwardSlash, postFix);
        }

        private bool IfResourceIsNotFormBased(string subtype)
        {
            return subtype.ToLower().Contains(Attachment) || subtype.ToLower().Contains(WebBased);
        }

        private string DeriveResourceToShow(string resourceToShow)
        {
            string subtype = ddlSubType.SelectedItem.Text;

            if (IfResourceIsNotFormBased(subtype))
            {
                resourceToShow = ThinkgateResource;
            }

            LookupDetail enumlkpDetail;

            if (Enum.TryParse(hdnSubTypes.Value, true, out enumlkpDetail))
            {
                switch (enumlkpDetail)
                {
                    case LookupDetail.CurriculumMapForm:
                        resourceToShow = ThinkgateInstructionPlan;
                        break;
                    case LookupDetail.UnitPlanForm:
                        resourceToShow = ThinkgateUnitPlan;
                        break;
                    case LookupDetail.LessonPlanForm:
                        resourceToShow = ThinkgateLessonPlan;
                        break;
                    case LookupDetail.StateModelCurriculumForm:
                        resourceToShow = ThinkgateCurriculumUnit;
                        break;
                    case LookupDetail.StateModelCurriculumUnitForm:
                        resourceToShow = ThinkgateUnitPlan;
                        break;
                }
            }

            return resourceToShow;
        }

        private void PopAddExistingSearchWindow(string title, string docType, int parentNodeID, string subtype,string isShared)
        {
            var win = new RadWindow
            {
                ID = "CmsNewDocument",
                NavigateUrl = string.Format("~/Controls/UnitPlans/AddExistingSearch.aspx?doctype={0}&parentnodeid={1}&subtype={2}&isshared={3}", docType, parentNodeID, subtype, isShared),
                VisibleOnPageLoad = true,
                ShowContentDuringLoad = false,
                VisibleStatusbar = false,
                Width = new Unit(1200, UnitType.Pixel),
                Height = new Unit(660, UnitType.Pixel),
                Title = string.Concat("Add New ", title),
                Skin = "Web20",
                Behaviors = WindowBehaviors.Close | WindowBehaviors.Move | WindowBehaviors.Resize,
                OnClientBeforeClose = "OnClientBeforeClose",
                OnClientClose = "closeCmsDialogDoPostBack",
                Top=new Unit(37, UnitType.Pixel),
                Left = new Unit(153, UnitType.Pixel)

            };

            wndWindowManager.Windows.Add(win);
        }

        private void PopAddExistingSearchWindowCl(string title, string docType, int parentNodeID)
        {
            var win = new RadWindow
            {
                ID = "CmsNewDocument",
                NavigateUrl = string.Format("~/Controls/UnitPlans/AddExistingCompetencyList.aspx?doctype={0} &parentnodeid={1} &reqtype=1", docType, parentNodeID),
                VisibleOnPageLoad = true,
                ShowContentDuringLoad = false,
                VisibleStatusbar = false,
                Width = new Unit(1100, UnitType.Pixel),
                Height = new Unit(660, UnitType.Pixel),
                Title = string.Concat("Add New ", title),
                Skin = "Web20",
                Behaviors = WindowBehaviors.Close | WindowBehaviors.Move | WindowBehaviors.Resize,
                OnClientBeforeClose = "OnClientBeforeClose",
                OnClientClose = "closeCmsDialogDoPostBack"
            };

            wndWindowManager.Windows.Add(win);
        }

        #endregion

        #region Filtering Helper Methods

        /// <summary>
        /// Return Document Type References from Kentico DB for the tileParm _resourceToShow.
        /// </summary>
        private void SearchDocumentTypeReferences()
        {
            if (IsSeachNotNeeded())
            {
                return;
            }

            EmptyUserNodeList();

            LoadResources();

            DisplayResults();
        }

       
        private void LoadResources()
        {
            string resourceScope = GetResourceScope();
            int maxResults = Convert.ToInt32(DistrictParms.LoadDistrictParms().NumberOfKenticoRecordsOnTile);
            List<int> curriculaIds = new List<int>();
            List<int> standardIds = new List<int>();
            bool includeExpiredDocuments = IsFilteringByUsageRightsExpiration();
            string type = null;
            string subtype = null;
            if (IsFilterByTeacherNecessary())
            {
                curriculaIds.AddRange(FilterListByTeacher());
            }

            if (IsFilterByGradeSubjectCourseNecessary())
            {
                if (curriculaIds.Count == 0)
                curriculaIds.AddRange(FilterByGradeSubjectAndCourse());
                else
                {
                    curriculaIds.Intersect(FilterByGradeSubjectAndCourse());
                }
            }

            if (IsShowingResources())
            {

                if (FilterByStandardsIfNecessary())
                {
                    Base.Classes.Standards standards = (Base.Classes.Standards)Tile.TileParms.GetParm("standards");
                    standardIds.AddRange(FilterListByStandard(standards));
            }

                type = FilterByResourceTypeIfNecessary();
                subtype = FilterByResourceSubTypeIfNecessary();
        }


            foreach (string tileResource in StrResourcesToShowOnTile)
            {

                _userNodeList.AddRange(KenticoHelper.GetKenticoDocuments(SessionObject.LoggedInUser.UserName, tileResource, resourceScope, includeExpiredDocuments, curriculaIds, standardIds, type, subtype, maxResults));
            }
        }

        private void EmptyUserNodeList()
        {
            _userNodeList = new List<UserNodeList>();
        }

        private bool IsSeachNotNeeded()
        {
            return IsFilterByTeacherNecessary() && IsTeacherNotInCache();
        }

        private bool IsFilterByTeacherNecessary()
        {
            bool isUserATeacher = IsUserATeacher();
            bool isTeacherViewingHigherLevelResources = isUserATeacher
                                                     && IsTeacherViewingHigherLevelResources();

            return isUserATeacher && isTeacherViewingHigherLevelResources;
        }

        private bool IsTeacherNotInCache()
        {
            var key = GetTeacherKey();

            return !RecordExistsInCache(key);
        }

        private string GetTeacherKey()
        {
            return "Teacher_" + _session.TeacherTileParms.GetParm("userID");
        }

        private bool IsTeacherViewingHigherLevelResources()
        {
            string resourceScope = GetResourceScope();

            var resType = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), resourceScope);

            return (resType == ResourceTypes.DistrictDocuments || resType == ResourceTypes.StateDocuments) && _session.TeacherTileParms.GetParm("userID") != null;
        }

        private string GetResourceScope()
        {
            string resourceScope = IsRadComboBoxSelected(cmbUserType)
                ? cmbUserType.SelectedValue
                : ResourceTypes.MyDocuments.ToString();

            return resourceScope;
        }

        private bool IsFilteringByUsageRightsExpiration()
        {
            return UserHasPermission(Permission.AllowViewForIMUsageRightExpiredContent) || UserHasPermission(Permission.AllowEditForIMUsageRightExpiredContent);
        }

        private bool IsFilterByGradeSubjectCourseNecessary()
        {
            return IsShowingGradeSubjectAndCourseFilters() && IsAtLeastOneGradeSubjectOrCourseFiltered();
        }

        private bool IsAtLeastOneGradeSubjectOrCourseFiltered()
        {
            //string grade = cmbGrade.SelectedItem.Text.ToLower() == "grade" ? null : cmbGrade.SelectedItem.Text;
            //string subject = cmbSubject.SelectedItem.Text.ToLower() == "subject"
            //    ? null
            //    : cmbSubject.SelectedItem.Text;
            //string course = cmbCourse.SelectedItem.Text.ToLower() == "course"
            //    ? null
            //    : cmbCourse.SelectedItem.Text;

            return IsRadComboBoxSelected(cmbGrade, _gradeDefaultValues)
                   || IsRadComboBoxSelected(cmbSubject, _subjectDefaultValues)
                   || IsRadComboBoxSelected(cmbCourse, _courseDefaultValues);
        }

        private bool IsShowingGradeSubjectAndCourseFilters()
        {
            return cmbGrade.Visible && cmbSubject.Visible && cmbCourse.Visible;
        }

        private Boolean FilterByStandardsIfNecessary()
            {
            return Tile.TileParms.Parms.ContainsKey("standards");

        }

       

        private string FilterByResourceSubTypeIfNecessary()
        {
           return IsRadComboBoxSelected(cmbResourceSubType, _defaultResourceSubTypeValues)? GetResourceFilterDropdownValue(cmbResourceSubType, _defaultResourceSubTypeValues) : null;

        }

        private string FilterByResourceTypeIfNecessary()
            {
            return IsRadComboBoxSelected(cmbResourceType, _defaultResourceTypeValues) ? GetResourceFilterDropdownValue(cmbResourceType, _defaultResourceTypeValues) : null;
        }

        private bool IsRadComboBoxSelected(RadComboBox resourceFilterComboBox, params string[] defaultValues)
        {
            Debug.Assert(resourceFilterComboBox != null);

            if (resourceFilterComboBox.SelectedItem == null)
            {
                return false;
            }

            string selectedValue = resourceFilterComboBox.SelectedItem.Value;

            if (string.IsNullOrWhiteSpace(selectedValue))
            {
                return false;
            }

            return defaultValues == null || !defaultValues.Contains(selectedValue);
        }

        private string GetResourceFilterDropdownValue(RadComboBox resourceFilterComboBox, params string[] defaultValues)
        {
            string result = null;

            if (IsRadComboBoxSelected(resourceFilterComboBox, defaultValues))
            {
                result = resourceFilterComboBox.SelectedItem.Value;
            }

            return result;
        }
        
        //Instruction tiles will display a max number of records equal to the parm setting
        protected static int GetNumberOfRecordsOnTile(string districtParmValue)
        {
            if (!string.IsNullOrWhiteSpace(districtParmValue))
            {
                int numberOfKenticoRecordsOnTile;
                return int.TryParse(districtParmValue, out numberOfKenticoRecordsOnTile) ? numberOfKenticoRecordsOnTile : DefaultNumberOfRecordsOnTile;
            }
                return DefaultNumberOfRecordsOnTile;
            }

        private void DisplayResults()
        {
            if (_userNodeList != null)
            {
                int numberOfRecordsOnTile = GetNumberOfRecordsOnTile(DistrictParms.LoadDistrictParms().NumberOfKenticoRecordsOnTile);

                _userNodeList.Sort((x, y) => -x.LastModified.CompareTo(y.LastModified));

                pnlNoResults.Visible = false;
                lbxdocumentTypeList.DataSource = _userNodeList.Count > 25 ? _userNodeList.ToList().Take(numberOfRecordsOnTile) : _userNodeList.ToList();
                lbxdocumentTypeList.DataBind();
            }
            else
            {
                pnlNoResults.Visible = true;
                lbxdocumentTypeList.DataSource = null;
                lbxdocumentTypeList.DataBind();
            }
        }

        private bool IsUserATeacher()
        {
            return _session.LoggedInUser.Roles.Any(r => r.RoleName.ToLower() == ElementRole.teacher.ToString());
        }

        private void ProcessResourceSelectionChange(RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBoxItem selectedItem = cmbResourceType.Items.FindItemByValue(e.Value);

            if (!string.IsNullOrWhiteSpace(selectedItem.Value))
            {
                List<LookupDetails> resourceSubTypes = KenticoHelper.GetLookupDetailsByClient(
                    "LookupEnum=" + selectedItem.Value, "Description ASC");

                if (resourceSubTypes != null)
                {
                    resourceSubTypes.ForEach(ld => ld.DropdownText = ld.Description);

                    // Kumar: 3/19/2014. The Subtype list in the Resource tile should not show Forms. Only Attachment and Web-resources
                    resourceSubTypes = resourceSubTypes.Where(p => !p.Description.Contains("Form")).ToList();

                    LookupDetails all = new LookupDetails();
                    all.Enum = 0;
                    all.DropdownText = "All";
                    all.Description = "SubType";
                    resourceSubTypes.Insert(0, all);

                    cmbResourceSubType.DataValueField = "Enum";
                    cmbResourceSubType.DataTextField = "Description";
                    cmbResourceSubType.DataSource = resourceSubTypes;
                    cmbResourceSubType.DataBind();
                }

                SearchDocumentTypeReferences();
            }
        }
        public class TelerikList
        {
            public string Value { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #endregion
    }
}
