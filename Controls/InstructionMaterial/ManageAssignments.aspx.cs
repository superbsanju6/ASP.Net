using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.Groups;
using Thinkgate.Services.Contracts.SearchCriteria;
using Thinkgate.Controls.E3Criteria;
using CheckBoxList = Thinkgate.Controls.E3Criteria.CheckBoxList;
using DropDownList = Thinkgate.Controls.E3Criteria.DropDownList;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using System.Data;
using Thinkgate.Base.Classes.InstructionMaterial;
using Thinkgate.Base.DataAccess;


namespace Thinkgate.Controls.InstructionMaterial
{
    /// <summary>
    /// This Page allows for a user to add students to, or remove students from a Group.
    /// </summary>
    public partial class ManageAssignments : Page
    {
        #region Constants

        private const String EditGroupColumnName = "EditGroup";
        private const string RtiFormerYear = "Former Year";
        private const string RtiTier1 = "Current Tier 1";
        private const string RtiTier2 = "Current Tier 2";
        private const string RtiTier3 = "Current Tier 3";
        private const string NameColumn = "Name";
        private const string DescriptionColumn = "Description";
        //private int ClassId = 0;
        // private int GroupId = 5492;

        private List<GroupDataContract> _groupsWithStudents;

        #endregion

        #region Variables

        readonly GroupsProxy _groupProxy = new GroupsProxy();
        private GroupDataContract _studentGroup;

        private int DocumentNodeID = 0;

        private List<UserNodeList> _imNodeList;
        private TreeProvider _treeProvider;
        public string[] StrResourcesToShowOnTile;
        public List<KeyValuePair<string, string>> StrResourcesFriendlyName;

        public int GroupId;
        public int ClassId;

        #endregion

        #region Properties

        //public int GroupId
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(Request.QueryString["GroupID"]))
        //        {
        //            throw new Exception("GroupID Missing; Expected in QueryString");
        //        }
        //        return Int32.Parse(Request.QueryString["GroupID"]);
        //    }
        //    set
        //    {
        //        GroupId = value;
        //    }
        //}

        //public int ClassId
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(Request.QueryString["ClassID"]))
        //        {
        //            throw new Exception("ClassID Missing; Expected in QueryString");
        //        }
        //        return Int32.Parse(Request.QueryString["ClassID"]);
        //    }
        //    set  {
        //        ClassId = value;
        //    }
        //}

        private SessionObject SessionObject
        {
            get
            {
                return (SessionObject)Session["SessionObject"];
            }
        }

        
        #endregion

        #region Page Load and Initializations

        /// <summary>
        /// This event fires when the Page is first Initialized
        /// It assigns the SearchHandler for Criteria provided by the Master Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += SearchHandler;
        }

        /// <summary>
        /// This event handler fires the event from this Page when it is loading.  
        /// It performs the following initialization steps:
        /// 1. Initialize the Group Detail Control
        /// 2. Initializes the Available Students Search Control
        /// 3. Sets the page title
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterScript();
            InitializeSearchControl();

            if (Request.QueryString["DocumentNodeID"] != null && Request.QueryString["DocumentNodeID"].ToString().Trim() != string.Empty)
                DocumentNodeID = Convert.ToInt32(Request.QueryString["DocumentNodeID"]);

            if (!string.IsNullOrWhiteSpace(Request.QueryString["GroupID"]))
                GroupId = int.Parse(Request.QueryString["GroupID"]);

            if (!string.IsNullOrWhiteSpace(Request.QueryString["ClassID"]))
                ClassId = int.Parse(Request.QueryString["ClassID"]);

            if (Page.IsPostBack)
            {
                grdDetail.Rebind();
            }

            if (!IsPostBack)
            {
                DisplaySelectedStudentsForIM(null, null);
                DisplayDetailForInstructionMaterial(null, null);

            }

            if (GroupId > 0)
            {
                ctrlClass.Visible = false;
                ctrlGroup.Visible = true;


            }
            else
            {
                ctrlClass.Visible = true;
                ctrlGroup.Visible = false;
            }

        }

        /// <summary>
        /// This method populates the data sources for the various fields of the search control
        /// </summary>
        private void InitializeSearchControl()
        {
            ctrlStudentName.Clear();
            ctrlStudentName.DataSource = null;
            ctrlStudentName.DataBind();
            ctrlStudentID.Clear();
            ctrlStudentID.DataSource = null;
            ctrlStudentID.DataBind();
            ctrlRTI.DataSource = RTI.GetAllTiersToDataTable();
            ctrlRTI.DataBind();
            ctrlSchoolType.DataSource = SchoolTypeMasterList.GetSchoolTypeListDataTableForUser(SessionObject.LoggedInUser);
            ctrlSchoolType.DataBind();
            LoadSchoolsCriteria();
            ctrlGrade.DataSource = Grade.GetGradesForStudents();
            ctrlGrade.DataBind();
            // ctrlDemographics.Clear();
            ctrlClass.DataSource = SessionObject.LoggedInUser.Classes;
            ctrlClass.DataBind();
            ctrlGroup.DataSource = _groupProxy.GetGroupsForUser(SessionObject.LoggedInUser.Page, DistrictParms.LoadDistrictParms().ClientID);
            ctrlGroup.DataBind();

        }

        /// <summary>
        /// This method loads the Schools Criteria and locks down the control if only one school is available for the user
        /// </summary>
        private void LoadSchoolsCriteria()
        {
            List<Thinkgate.Base.Classes.School> lstSchools = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            if (lstSchools.Count == 1)
            {
                ctrlSchool.DefaultTexts = new List<string>() { lstSchools[0].Name };
                ctrlSchool.ReadOnly = true;

                ctrlCluster.DefaultTexts = new List<string>() { lstSchools[0].Cluster };
                ctrlCluster.ReadOnly = true;
            }
            ctrlSchool.DataSource = lstSchools;
            ctrlSchool.DataBind();

            ctrlCluster.DataSource = SchoolMasterList.GetClustersForUser(SessionObject.LoggedInUser).ToDataTable("Cluster");
            ctrlCluster.DataBind();
        }

        /// <summary>
        /// Registers Properties for Criteria.js file to use for Value Options. Required.
        /// </summary>
        public void RegisterScript()
        {
            bool firstOne = true;
            string enumStr = "CriteriaController.RestrictValueOptions = {";
            foreach (int option in Enum.GetValues(typeof(CriteriaBase.RestrictValueOptions)))
            {
                if (!firstOne) enumStr += ",";
                enumStr += "\"" + Enum.GetName(typeof(CriteriaBase.RestrictValueOptions), option) + "\" : " + option;
                firstOne = false;
            }
            enumStr += "};";
            ScriptManager.RegisterStartupScript(this, typeof(string), "SearchEnums", enumStr, true);
        }

        /// <summary>
        /// This method sets the page title for this page
        /// </summary>
        private void SetPageTitle()
        {
            Page.Title = "Manage Assignments ";
        }

        /// <summary>
        /// This method contains the logic that determines of a Groups detail information is editable
        /// for the current user
        /// </summary>
        /// <returns>true if the current user created the group; false otherwise</returns>
        private bool GetEditColumnVisibility()
        {
            return _studentGroup.CreatorPage == SessionObject.LoggedInUser.Page;
        }

        #endregion

        #region Grid Item Events

        /// <summary>
        /// This is the item bind event for the Group Details grid. This will be used for adjusting display properties of the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdDetails_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataBoundItem = e.Item as GridDataItem;
                if (dataBoundItem[NameColumn].Text.Length > 25)
                {
                    dataBoundItem[NameColumn].ToolTip = dataBoundItem[NameColumn].Text;
                    dataBoundItem[NameColumn].Text = dataBoundItem[NameColumn].Text.Substring(0, 25) + "...";
                }

                if (dataBoundItem[DescriptionColumn].Text.Length > 25)
                {
                    dataBoundItem[DescriptionColumn].ToolTip = dataBoundItem[DescriptionColumn].Text;
                    dataBoundItem[DescriptionColumn].Text = dataBoundItem[DescriptionColumn].Text.Substring(0, 25) + "...";
                }
            }
        }

        /// <summary>
        /// Method that retrieves the studentID from the GridDataItem related to the Link Button of a grid
        /// </summary>
        /// <param name="btnLink">The actual button initiating the remove/add</param>
        /// <returns></returns>
        private int GetStudentIDFromLinkButton(LinkButton btnLink)
        {
            GridDataItem item = (GridDataItem)btnLink.NamingContainer;
            return GetStudentIDFromGridItem(item);
        }

        /// <summary>
        /// Method that retrieves the DataKeyValue related to a RadGrid based on the GridDataItem(Row) selected
        /// </summary>
        /// <param name="item">The row selected in the RadGrid for Add/Remove</param>
        /// <returns></returns>
        private int GetStudentIDFromGridItem(GridDataItem item)
        {
            return Int32.Parse(item.GetDataKeyValue("ID").ToString());
        }

        /// <summary>
        /// Eventhandler that handles the Click event of the Delete Button on the 
        /// Group detail grid
        /// </summary>
        /// <param name="sender">Delete BUtton on grdDetail</param>
        /// <param name="e">EventArgs passed to eventhandler</param>
        protected void DeleteGroup_Click(object sender, EventArgs e)
        {
            DeleteGroup();
            ClosePage();
        }

        #endregion



        #region Get Selected and Available Students

        /// <summary>
        /// Handler for the Search Button
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            bool isSaved = ViewState["isSaved"] == null ? true : (bool)ViewState["isSaved"];
            if (!isSaved && isCompleted.Value != "1")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "assignmentSaved", "var modalWin = getCurrentCustomDialog(); modalWin.remove_beforeClose(callmessage);modalWin.remove_pageLoad(callmessage); customDialog({ title: \"Confirm changes\", maximize: true, maxwidth: 500, maxheight: 100, animation: \"None\", dialog_style: \"confirm\", content: \"Do you want to save the changes to the current assignments?\"   }, [{ title: 'No',callback:confirmCallbackClose },{ title: 'Yes', callback: confirmCallbackFunction }]); ", true);
                return;
            }
            DisplayStudentsAvailable(null, null);
            grdAvailable.Rebind();
            if (!IsPostBack)
            {
                ViewState["selectedStudents"] = null;
                ViewState["availableStudents"] = null;
            }
            ViewState["isSaved"] = null;
            isCompleted.Value = "";
            Page.ClientScript.RegisterHiddenField("savedState", "");
            DisplaySelectedStudentsForIM(null, null);
            grdSelected.Rebind();
            DisplayDetailForInstructionMaterial(null, null);
            grdDetail.Rebind();
        }


        /// <summary>
        /// This method loads all students the the current user has visibility too AND that are not already 
        /// enrolled in the current group.
        /// </summary>
        /// <returns>A list of students who are eligable to be added to this group</returns>
        public IEnumerable<IMAvailableStudent> LoadStudentsAvailableForIM()
        {
            var availableStudents = new List<IMAvailableStudent>();
            var studentSearchCriteria = new IMStudentSearchCriteria();
            var IM = new IMAssignment();
            var criteriaController = GetCriteriaControllerFromMasterPage();

            if (criteriaController != null)
            {
                studentSearchCriteria = GetStudentCriteriaFromController(criteriaController);
            }


            switch (SessionObject.CurrentPortal)
            {
                case EntityTypes.District:
                    availableStudents = IM.GetAvaiableStudentsForIM(GroupId,
                     SessionObject.LoggedInUser.Page, studentSearchCriteria,
                      DistrictParms.LoadDistrictParms().ClientID, ClassId, DocumentNodeID);
                    break;
                case EntityTypes.School:
                    if (studentSearchCriteria.School == 0)
                    {
                        studentSearchCriteria.School = SessionObject.LoggedInUser.School;
                    }

                    availableStudents = IM.GetAvaiableStudentsForIM(GroupId,
                     SessionObject.LoggedInUser.Page, studentSearchCriteria,
                      DistrictParms.LoadDistrictParms().ClientID, ClassId, DocumentNodeID);
                    break;
                case EntityTypes.Teacher:

                    availableStudents = IM.GetAvaiableStudentsForIM(GroupId,
                      SessionObject.LoggedInUser.Page, studentSearchCriteria,
                      DistrictParms.LoadDistrictParms().ClientID, ClassId, DocumentNodeID);
                    break;
            }



            return availableStudents;
        }


        /// <summary>
        /// Retrieves the CriteriaController from the master page if initialized
        /// </summary>
        /// <returns></returns>
        private CriteriaController GetCriteriaControllerFromMasterPage()
        {
            CriteriaController criteriaController;
            try
            {
                criteriaController = Master.CurrentCriteria();
            }
            catch (CriteriaNotLoaded)
            {
                criteriaController = null;
            }
            return criteriaController;
        }

        /// <summary>
        /// This method populates a StudentSearchCriteria object with values from the Criteria Controller
        /// </summary>
        /// <param name="criteriaController"></param>
        /// <returns></returns>
        private IMStudentSearchCriteria GetStudentCriteriaFromController(CriteriaController criteriaController)
        {
            var studentSearchCriteria = new IMStudentSearchCriteria();
            var studentName = criteriaController.ParseCriteria<Text.ValueObject>("StudentName").FirstOrDefault();
            studentSearchCriteria.StudentName = studentName == null ? string.Empty : studentName.Text;
            var studentId = criteriaController.ParseCriteria<Text.ValueObject>("StudentID").FirstOrDefault();
            studentSearchCriteria.StudentID = studentId == null ? string.Empty : studentId.Text;
            var inactiveRti =
                criteriaController.ParseCriteria<CheckBoxList.ValueObject>("RTI").Find(x => x.Text == RtiFormerYear);
            studentSearchCriteria.InactiveRTI = inactiveRti == null ? string.Empty : inactiveRti.Text;
            var tier1Rti = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("RTI")
                .Find(x => x.Text == RtiTier1);
            studentSearchCriteria.Tier1RTI = tier1Rti == null ? string.Empty : tier1Rti.Text;
            var tier2Rti = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("RTI")
                .Find(x => x.Text == RtiTier2);
            studentSearchCriteria.Tier2RTI = tier2Rti == null ? string.Empty : tier2Rti.Text;
            var tier3Rti = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("RTI")
                .Find(x => x.Text == RtiTier3);
            studentSearchCriteria.Tier3RTI = tier3Rti == null ? string.Empty : tier3Rti.Text;
            studentSearchCriteria.Cluster = string.Join(",",
                criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Cluster").Select(x => x.Text).ToList());
            studentSearchCriteria.SchoolTypes = string.Join(",",
                criteriaController.ParseCriteria<CheckBoxList.ValueObject>("SchoolType").Select(x => x.Text).ToList());
            studentSearchCriteria.School =
                Convert.ToInt32(criteriaController.ParseCriteria<DropDownList.ValueObject>("School")
                    .Select(x => x.Value)
                    .ToList()
                    .FirstOrDefault());
            studentSearchCriteria.Grades = string.Join(",",
                criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Grade").Select(x => x.Text).ToList());
            studentSearchCriteria.ClassID = Convert.ToInt32(criteriaController.ParseCriteria<DropDownList.ValueObject>("Class").Select(x => x.Value).ToList().FirstOrDefault());
            ClassId = studentSearchCriteria.ClassID > 0 ? studentSearchCriteria.ClassID : ClassId;
            studentSearchCriteria.GroupID = Convert.ToInt32(criteriaController.ParseCriteria<DropDownList.ValueObject>("Group").Select(x => x.Value).ToList().FirstOrDefault());
            GroupId = studentSearchCriteria.GroupID > 0 ? studentSearchCriteria.GroupID : GroupId;
            return studentSearchCriteria;
        }

        /// <summary>
        /// This method makes a call to load all students available for this group, then displays them in the 
        /// Available students grid
        /// </summary>
        /// <param name="sender">object that fired this event; grdAvailable</param>
        /// <param name="e">GridNeedDataSourceEventArgs passed to the eventhandler</param>
        protected void DisplayStudentsAvailable(object sender, GridNeedDataSourceEventArgs e)
        {
            var studentsForGroup = LoadStudentsAvailableForIM().ToList(); //LoadStudentsAvailableForGroup().ToList();
            grdAvailable.DataSource = studentsForGroup.OrderBy(x=>x.Name);
            lblAvailable.Text = string.Format("Available Students: " + studentsForGroup.Count);
            ViewState["availableStudents"] = studentsForGroup;

        }

        /// <summary>
        /// This method makes a call to load all students available for this class, then displays them in the 
        /// Available students grid
        /// </summary>
        /// <param name="sender">object that fired this event; grdAvailable</param>
        /// <param name="e">GridNeedDataSourceEventArgs passed to the eventhandler</param>
        protected void DisplayStudentsAvailableForClass(object sender, GridNeedDataSourceEventArgs e)
        {
            //var studentsForClass = LoadStudentsAvailableForGroup().ToList();
            //grdAvailable.DataSource = studentsForClass;
            //lblAvailable.Text = string.Format("Available Students: " + studentsForClass.Count);
        }

        /// <summary>
        /// Loads the appropriate Group object and sets it as the data source for the group detail grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DisplayDetailForInstructionMaterial(object sender, GridNeedDataSourceEventArgs e)
        {
            GetInstructionMaterial();
            if (ViewState["selectedStudents"] != null)
            {
                if (_imNodeList.Count>0)
                _imNodeList[0].StudentCount = ((List<IMAvailableStudent>)ViewState["selectedStudents"]).Count;
            }

            grdDetail.DataSource = _imNodeList; //new List<GroupDataContract> {_studentGroup};            
            if (_imNodeList.Count > 0)
            {
                calMasterAssignmentDate.MaxDate = _imNodeList[0].ExpirationDate == null ? Convert.ToDateTime("12/31/9999") : _imNodeList[0].ExpirationDate;
                calMasterDueDate.MaxDate = _imNodeList[0].ExpirationDate == null ? Convert.ToDateTime("12/31/9999") : _imNodeList[0].ExpirationDate;
            }
            // grdDetail.MasterTableView.GetColumn(EditGroupColumnName).Visible = GetEditColumnVisibility();
            SetPageTitle();
        }



        /// <summary>
        /// Loads the appropriate Class object and sets it as the data source for the group detail grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DisplayDetailForClass(object sender, GridNeedDataSourceEventArgs e)
        {
            _studentGroup = _groupProxy.GetGroup(ClassId, DistrictParms.LoadDistrictParms().ClientID);
            grdDetail.DataSource = new List<GroupDataContract> { _studentGroup };
            grdDetail.MasterTableView.GetColumn(EditGroupColumnName).Visible = GetEditColumnVisibility();
            SetPageTitle();
        }

        /// <summary>
        /// This method returns all students currently members of the group currently displayed
        /// </summary>
        /// <returns>A list of all students currently members of the group currently displayed</returns>
        private IEnumerable<IMAvailableStudent> LoadSelectedStudentsForIM()
        {


            var selectedStudents = new IMAssignment().GetSelectedStudentsForIM(SessionObject.LoggedInUser.Page, GroupId, ClassId, DocumentNodeID);

            //var selectedStudents =
            //    _groupProxy.GetStudentsInGroup(GroupId, DistrictParms.LoadDistrictParms().ClientID);

            return selectedStudents;
        }

        /// <summary>
        /// This method makes a call to load all students in the current group then displays them in the
        /// selected students grid
        /// </summary>
        /// <param name="sender">Object that fired this event; grdSelected</param>
        /// <param name="e">GridNeedDataSourceEventArgs passed to the eventhandler</param>
        protected void DisplaySelectedStudentsForIM(object sender, GridNeedDataSourceEventArgs e)
        {
            var studentsForGroup = LoadSelectedStudentsForIM().ToList();
            grdSelected.DataSource = studentsForGroup.OrderBy(x=>x.Name);
            lblSelected.Text = string.Format("Selected Students: " + studentsForGroup.Count);
            ViewState["selectedStudents"] = studentsForGroup;
            if (studentsForGroup.Count > 0)
            {
                calMasterAssignmentDate.Enabled = true;
                calMasterDueDate.Enabled = true;
            }
            else
            {
                calMasterAssignmentDate.Enabled = false;
                calMasterDueDate.Enabled = false;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method refreshes the Databinding for the Available and Selected Grouping grids.
        /// This causes the UI to update
        /// </summary>
        private void RefreshDataBindingForGroupingGrids()
        {
            if (ViewState["selectedStudents"] == null)
            {
                DisplayStudentsAvailable(null, null);
                DisplaySelectedStudentsForIM(null, null);
                DisplayDetailForInstructionMaterial(null, null);
                grdAvailable.Rebind();
                grdSelected.Rebind();
                grdDetail.Rebind();
            }
            else
            {
                var avalist = (List<IMAvailableStudent>)ViewState["availableStudents"];
                var sellist = (List<IMAvailableStudent>)ViewState["selectedStudents"];
                grdAvailable.DataSource = avalist.OrderBy(x=>x.Name);
                grdSelected.DataSource = sellist.OrderBy(x => x.Name);
                lblAvailable.Text = string.Format("Available Students: " + avalist.Count);
                lblSelected.Text = string.Format("Selected Students: " + sellist.Count);
                grdAvailable.Rebind();
                grdSelected.Rebind();
                DisplayDetailForInstructionMaterial(null, null);
                grdDetail.Rebind();
                if (sellist.Count > 0)
                {
                    calMasterAssignmentDate.Enabled = true;
                    calMasterDueDate.Enabled = true;
                }
                else
                {
                    calMasterAssignmentDate.Enabled = false;
                    calMasterDueDate.Enabled = false;
                }

            }
        }

        /// <summary>
        /// This method is called from the RadGrid ItemCommand methods and contains the logic to extract the 
        /// studentIDs of the selected students in order to either add or remove them from a group
        /// </summary>
        /// <param name="gridCommandEventArgs">GridCommandEventArgs containing the selected 
        /// GridItem which is bound to a Student object</param>
        /// <returns>A list of student IDs corresponding to the selected students</returns>
        private static List<int> GetSelectedStudentIDs(GridCommandEventArgs gridCommandEventArgs)
        {
            var item = (GridDataItem)gridCommandEventArgs.Item;
            var studentId = Int32.Parse(item.GetDataKeyValue("ID").ToString());
            var studentIDs = new List<int> { studentId };
            return studentIDs;
        }

        /// <summary>
        /// Calling this method will programatically close the page
        /// </summary>
        private void ClosePage()
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeScript", "CloseDialog('Saved');", true);
        }

        /// <summary>
        /// This method will cause the currently displayed group to be deleted
        /// </summary>
        private void DeleteGroup()
        {
            _groupProxy.DeleteGroup(GroupId, DistrictParms.LoadDistrictParms().ClientID);
        }

        /// <summary>
        /// This method initializes adding/removing all students from a group based on Add All or Remove All
        /// </summary>
        /// <param name="isRemove">Determines whether the mode is Removing or Adding</param>
        private void AddRemoveAllStudents(bool isRemove)
        {
            List<int> lstStudentIDs = new List<int>();
            RadGrid grdCurrent = isRemove ? grdSelected : grdAvailable;
            foreach (GridDataItem gdi in grdCurrent.Items)
            {
                int studentID = GetStudentIDFromGridItem(gdi);
                lstStudentIDs.Add(studentID);
            }
            AddRemoveStudentsFromIM(lstStudentIDs, isRemove);
        }

        /// <summary>
        /// This method initializes adding/removing a single student from a group based on Add or Remove
        /// </summary>
        /// <param name="studentID">ID of the student being added/removed</param>
        /// <param name="isRemove">Determines whether the mode is Removing or Adding</param>
        private void AddRemoveStudent(int studentID, bool isRemove)
        {
            List<int> studentIDS = new List<int> { studentID };
            AddRemoveStudentsFromIM(studentIDS, isRemove);
        }

        /// <summary>
        /// This method performs the call to the group service to either delete or add a selection of students based on StudentID
        /// </summary>
        /// <param name="studentIDs">List of integer values representing IDs of students</param>
        /// <param name="isDelete">Determines if the method is a delete or add</param>
        private void AddRemoveStudentsFromIM(List<int> studentIDs, bool isDelete)
        {
            ViewState["isSaved"] = false;
            if (isDelete)
            {
                //_groupProxy.DeleteStudentsForGroup(GroupId, studentIDs, DistrictParms.LoadDistrictParms().ClientID);
                List<IMAvailableStudent> avilablelist = (List<IMAvailableStudent>)ViewState["availableStudents"];
                List<IMAvailableStudent> selectedlist = (List<IMAvailableStudent>)ViewState["selectedStudents"];
                List<IMAvailableStudent> selected = selectedlist.Where(x => studentIDs.Contains(x.ID)).ToList();
                ViewState["availableStudents"] = avilablelist.Union(selected).ToList();
                ViewState["selectedStudents"] = selectedlist.Where(x => !studentIDs.Contains(x.ID)).ToList();

            }
            else
            {
                List<IMAvailableStudent> avilablelist = (List<IMAvailableStudent>)ViewState["availableStudents"];
                List<IMAvailableStudent> selectedlist = (List<IMAvailableStudent>)ViewState["selectedStudents"];
                List<IMAvailableStudent> ava = avilablelist.Where(x => studentIDs.Contains(x.ID)).Select(y => new IMAvailableStudent { ID = y.ID, IsClassMember = y.IsClassMember, IsGroupMember = y.IsGroupMember, Name = y.Name, SchoolName = y.SchoolName, AssignmentDate = y.AssignmentDate, DueDate = y.DueDate, TeacherName = "",IsNew=true }).ToList();
                ViewState["selectedStudents"] = selectedlist.Union(ava).ToList();
                ViewState["availableStudents"] = avilablelist.Where(x => !studentIDs.Contains(x.ID)).ToList();

                // new IMAssignment().SaveStudentForIM(SessionObject.LoggedInUser.Page,)
            }
            RefreshDataBindingForGroupingGrids();
        }

        #endregion

        #region Button Methods

        /// <summary>
        /// Click event for the Select All button of the Available Grid
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Args</param>
        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            AddRemoveAllStudents(false);
        }

        /// <summary>
        /// Click event for the Remove All button of the Selected Grid
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Args</param>
        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            AddRemoveAllStudents(true);
        }

        /// <summary>
        /// Click event for the Add buttons in the Available Grid
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Args</param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            //ViewState["isSaved"] = false;
            isCompleted.Value = "";
            var studentId = GetStudentIDFromLinkButton((LinkButton)sender);
            AddRemoveStudent(studentId, false);
        }

        /// <summary>
        /// Click event for the Remove buttons in the Selected Grid
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Args</param>
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            //ViewState["isSaved"] = false;
            isCompleted.Value = "";
            var studentId = GetStudentIDFromLinkButton((LinkButton)sender);
            AddRemoveStudent(studentId, true);
        }

        #endregion


        private void GetInstructionMaterial()
        {
            var ui = (UserInfo)Session["KenticoUserInfo"];
            SetTreeProvider();
            SetResourceTypesToShow();


            _imNodeList = new List<UserNodeList>();
            foreach (string tileResource in StrResourcesToShowOnTile)
            {
                _imNodeList.AddRange(KenticoHelper.GetKenticoDocsForInstuctionAssignmentTiles(ui, tileResource, _treeProvider, "NodeID=" + DocumentNodeID.ToString()));
                if (StrResourcesFriendlyName.Count > 0)
                    _imNodeList.Where(z => z.ClassName.ToLower() == tileResource.ToLower()).ToList().ForEach(x => x.FriendlyName = (StrResourcesFriendlyName.Where(y => y.Value.ToLower() == tileResource.ToLower()).FirstOrDefault().Key));
            }

            // DataSet resourcesToShow = KenticoHelper.GetTileMapLookupDataSet();


        }

        private void SetTreeProvider()
        {
            _treeProvider = KenticoHelper.GetUserTreeProvider(SessionObject.LoggedInUser.ToString());
        }

        private void SetResourceTypesToShow()
        {
            DataSet resourcesToShow = KenticoHelper.GetTileMapLookupDataSet();

            StrResourcesToShowOnTile = resourcesToShow.Tables[0]
                .AsEnumerable()
                .OrderBy(p => p["ItemOrder"].ToString())
                .Select(s => s.Field<string>("KenticoDocumentTypeToShow"))
                .ToArray();

            StrResourcesFriendlyName = resourcesToShow.Tables[0]
              .AsEnumerable()
              .OrderBy(p => p["ItemOrder"].ToString())
              .Select(s => new KeyValuePair<string, string>(s.Field<string>("FriendlyName"), s.Field<string>("KenticoDocumentTypeToShow")))
              .ToList();


        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "assignmentSaved", "Saved(); ", true); 
            List<IMAvailableStudent> availablelist = (List<IMAvailableStudent>)ViewState["availableStudents"];
            List<IMAvailableStudent> selectedlist = (List<IMAvailableStudent>)ViewState["selectedStudents"];
            IMAssignment obj = new IMAssignment();
            obj.SaveStudentForIM(SessionObject.LoggedInUser.Page, _imNodeList[0].NodeId, GetStudentAssignmentandDueDate(selectedlist));
            ViewState["isSaved"] = true;
            isCompleted.Value = "1";
            ViewState["selectedStudents"] = null;
            ViewState["availableStudents"] = null;
            RefreshDataBindingForGroupingGrids();
            if (isCompleted.Value == "1")            
                ScriptManager.RegisterStartupScript(this, typeof(string), "assignmentSaved", "btnSearchClick(); ", true);
            isCompleted.Value = "";
           


        }

        //[System.Web.Services.WebMethod]
        //public static string SaveData()
        //{
           
        //   SessionObject objSession=new SessionObject();
        //    objSession=(SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

        //        ManageAssignments objViewState=new ManageAssignments();
                
                
        //         List<IMAvailableStudent> availablelist = (List<IMAvailableStudent>)objViewState.ViewState["availableStudents"];
        //    List<IMAvailableStudent> selectedlist = (List<IMAvailableStudent>)objViewState.ViewState["selectedStudents"];
        //    IMAssignment obj = new IMAssignment();
        //    obj.SaveStudentForIM(objSession.LoggedInUser.Page, _imNodeList[0].NodeId, GetStudentAssignmentandDueDate(selectedlist));
        //    objViewState.ViewState["isSaved"] = true;
        //    //isCompleted.Value = "";           
        //    objViewState.ViewState["selectedStudents"] = null;
        //    objViewState.ViewState["availableStudents"] = null


        //        //page.ViewState["MyVal"] = "Hello";
        //        //return outputToReturn;
           
        
        //}

        protected void calAssignmentDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            ViewState["isSaved"] = false;
            isCompleted.Value = "";
            RadDatePicker datePicker1 = (RadDatePicker)sender;
            GridDataItem item = (GridDataItem)datePicker1.NamingContainer;
            List<IMAvailableStudent> selectedlist = (List<IMAvailableStudent>)ViewState["selectedStudents"];
            selectedlist.Find(x => x.ID == Convert.ToInt32(item.OwnerTableView.DataKeyValues[item.ItemIndex]["ID"].ToString())).AssignmentDate = datePicker1.SelectedDate != null ? datePicker1.SelectedDate.Value : default(DateTime);
            // RadDatePicker datePicker2 = (RadDatePicker)item.FindControl("calDueDate");
            ViewState["selectedStudents"] = selectedlist;
        }

        protected void calDueDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            ViewState["isSaved"] = false;
            isCompleted.Value = "";
            RadDatePicker datePicker1 = (RadDatePicker)sender;
            GridDataItem item = (GridDataItem)datePicker1.NamingContainer;
            List<IMAvailableStudent> selectedlist = (List<IMAvailableStudent>)ViewState["selectedStudents"];
            selectedlist.Find(x => x.ID == Convert.ToInt32(item.OwnerTableView.DataKeyValues[item.ItemIndex]["ID"].ToString())).DueDate = datePicker1.SelectedDate != null ? datePicker1.SelectedDate.Value : default(DateTime);
            // RadDatePicker datePicker2 = (RadDatePicker)item.FindControl("calAssignmentDate");
            ViewState["selectedStudents"] = selectedlist;
        }

        public static drGeneric_3Strings GetStudentAssignmentandDueDate(List<IMAvailableStudent> studentList)
        {
            var newSortXref = new drGeneric_3Strings();
            foreach (var std in studentList)
            {
                newSortXref.Add(std.ID.ToString(), std.AssignmentDate.ToString(), std.DueDate.ToString());
            }
            return newSortXref;
        }

        protected void grdSelected_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem editform = (GridDataItem)e.Item;
                bool isNewRecord = ((Thinkgate.Base.Classes.InstructionMaterial.IMAvailableStudent)(e.Item.DataItem)).IsNew;//ViewState["selectedStudents"] == null ? false : true;
                DateTime assignmentDate = ((Thinkgate.Base.Classes.InstructionMaterial.IMAvailableStudent)(e.Item.DataItem)).AssignmentDate;
                DateTime dueDate = ((Thinkgate.Base.Classes.InstructionMaterial.IMAvailableStudent)(e.Item.DataItem)).DueDate;
                RadDatePicker datepcker1 = (RadDatePicker)editform.FindControl("calAssignmentDate");
                RadDatePicker datepcker2 = (RadDatePicker)editform.FindControl("calDueDate");
                DateTime docExpiredDate = _imNodeList[0].ExpirationDate;

                if (datepcker1.SelectedDate <= docExpiredDate)
                    datepcker1.MaxDate = docExpiredDate;

                if (datepcker2.SelectedDate<=docExpiredDate)
                datepcker2.MaxDate = docExpiredDate;

                if (assignmentDate == default(DateTime))
                {
                    datepcker1.Clear();
                    if (isNewRecord)
                    {
                        datepcker1.MinDate = DateTime.Now;
                        datepcker1.ToolTip = "Entered date must be current date or greater than current date.";
                    }
                }
                else
                    datepcker2.MinDate = assignmentDate;

                if (dueDate == default(DateTime))
                {
                    datepcker2.Clear();
                    if (isNewRecord)
                    {
                        datepcker2.MinDate = DateTime.Now;
                        datepcker2.ToolTip = "Entered date must be current date or greater than current date.";
                    }
                }
                else
                    datepcker1.MaxDate = dueDate;

            }
        }

        protected void btnSaveClient_Click(object sender, EventArgs e)
        {
            List<IMAvailableStudent> availablelist = (List<IMAvailableStudent>)ViewState["availableStudents"];
            List<IMAvailableStudent> selectedlist = (List<IMAvailableStudent>)ViewState["selectedStudents"];
            IMAssignment obj = new IMAssignment();
            obj.SaveStudentForIM(SessionObject.LoggedInUser.Page, _imNodeList[0].NodeId, GetStudentAssignmentandDueDate(selectedlist));
            ViewState["isSaved"] = true;            
            ViewState["selectedStudents"] = null;
            ViewState["availableStudents"] = null;
            RefreshDataBindingForGroupingGrids();
            isCompleted.Value = "";
            ScriptManager.RegisterStartupScript(this, typeof(string), "assignmentSaved", "confirmCallbackCloseClient(); ", true);              
            isCompleted.Value = "";
        }

    }
}
