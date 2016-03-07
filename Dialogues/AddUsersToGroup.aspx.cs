using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Services.Contracts.Groups;
using Thinkgate.Services.Contracts.SearchCriteria;
using CheckBoxList = Thinkgate.Controls.E3Criteria.CheckBoxList;
using DropDownList = Thinkgate.Controls.E3Criteria.DropDownList;

namespace Thinkgate.Dialogues
{
    /// <summary>
    /// This Page allows for a user to add students to, or remove students from a Group.
    /// </summary>
    public partial class AddUsersToGroup : Page
    {
        #region Constants

        private const String EditGroupColumnName = "EditGroup";
        private const string RtiFormerYear = "Former Year";
        private const string RtiTier1 = "Current Tier 1";
        private const string RtiTier2 = "Current Tier 2";
        private const string RtiTier3 = "Current Tier 3";
        private const string NameColumn = "Name";
        private const string DescriptionColumn = "Description";

        #endregion

        #region Variables

        readonly GroupsProxy _groupProxy = new GroupsProxy();
        private GroupDataContract _studentGroup;

        #endregion

        #region Properties

        private int GroupId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Request.QueryString["groupId"]))
                {
                    throw new Exception("GroupID Missing; Expected in QueryString");
                }
                return Int32.Parse(Request.QueryString["groupId"]);
            }
        }

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

            if (Page.IsPostBack)
            {
                grdDetail.Rebind();
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
            ctrlDemographics.Clear();
            ctrlClass.DataSource = SessionObject.LoggedInUser.Classes;
            ctrlClass.DataBind();
        }

        /// <summary>
        /// This method loads the Schools Criteria and locks down the control if only one school is available for the user
        /// </summary>
        private void LoadSchoolsCriteria()
        {
            List<School> lstSchools = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            if (lstSchools.Count == 1)
            {
                ctrlSchool.DefaultTexts = new List<string>(){lstSchools[0].Name};
                ctrlSchool.ReadOnly = true;

                ctrlCluster.DefaultTexts = new List<string>(){lstSchools[0].Cluster};
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
            Page.Title = "Group: " + _studentGroup.Name;
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
            DisplayStudentsAvailableForGroup(null, null);
            grdAvailable.Rebind();
        }

        /// <summary>
        /// This method loads all students the the current user has visibility too AND that are not already 
        /// enrolled in the current group.
        /// </summary>
        /// <returns>A list of students who are eligable to be added to this group</returns>
        public IEnumerable<GroupStudentDataContract> LoadStudentsAvailableForGroup()
        {
            var availableStudents = new List<GroupStudentDataContract>();
            var studentSearchCriteria = new StudentSearchCriteria();
            var criteriaController = GetCriteriaControllerFromMasterPage();

            if (criteriaController != null)
            {
                studentSearchCriteria = GetStudentCriteriaFromController(criteriaController);
            }

            switch (SessionObject.CurrentPortal)
            {
                case EntityTypes.District:
                    availableStudents = _groupProxy.GetStudentsByDistrictAvailableForGroup(GroupId,
                        SessionObject.LoggedInUser.Page, studentSearchCriteria,
                        DistrictParms.LoadDistrictParms().ClientID);
                    break;
                case EntityTypes.School:
                    if (studentSearchCriteria.School == 0)
                    {
                        studentSearchCriteria.School = SessionObject.LoggedInUser.School;
                    }

                    availableStudents = _groupProxy.GetStudentsBySchoolAvailableForGroup(GroupId,
                        SessionObject.LoggedInUser.Page, studentSearchCriteria, DistrictParms.LoadDistrictParms().ClientID);
                    break;
                case EntityTypes.Teacher:
                    availableStudents = _groupProxy.GetStudentsByTeacherAvailableForGroup(GroupId,
                        SessionObject.LoggedInUser.Page, studentSearchCriteria,
                        DistrictParms.LoadDistrictParms().ClientID);
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
        private StudentSearchCriteria GetStudentCriteriaFromController(CriteriaController criteriaController)
        {
            var studentSearchCriteria = new StudentSearchCriteria();
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
            var demo1 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "1");
            studentSearchCriteria.Demo1 = demo1 == null ? string.Empty : demo1.DemoValue;
            var demo2 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "2");
            studentSearchCriteria.Demo2 = demo2 == null ? string.Empty : demo2.DemoValue;
            var demo3 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "3");
            studentSearchCriteria.Demo3 = demo3 == null ? string.Empty : demo3.DemoValue;
            var demo4 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "4");
            studentSearchCriteria.Demo4 = demo4 == null ? string.Empty : demo4.DemoValue;
            var demo5 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "5");
            studentSearchCriteria.Demo5 = demo5 == null ? string.Empty : demo5.DemoValue;
            var demo6 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "6");
            studentSearchCriteria.Demo6 = demo6 == null ? string.Empty : demo6.DemoValue;
            var demo7 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "7");
            studentSearchCriteria.Demo7 = demo7 == null ? string.Empty : demo7.DemoValue;
            var demo8 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "8");
            studentSearchCriteria.Demo8 = demo8 == null ? string.Empty : demo8.DemoValue;
            var demo9 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "9");
            studentSearchCriteria.Demo9 = demo9 == null ? string.Empty : demo9.DemoValue;
            var demo10 = criteriaController.ParseCriteria<Demographics.ValueObject>("Demographics").Find(x => x.DemoField == "10");
            studentSearchCriteria.Demo10 = demo10 == null ? string.Empty : demo10.DemoValue;
            studentSearchCriteria.ClassID = Convert.ToInt32(criteriaController.ParseCriteria<DropDownList.ValueObject>("Class").Select(x => x.Value).ToList().FirstOrDefault());
            return studentSearchCriteria;
        }

        /// <summary>
        /// This method makes a call to load all students available for this group, then displays them in the 
        /// Available students grid
        /// </summary>
        /// <param name="sender">object that fired this event; grdAvailable</param>
        /// <param name="e">GridNeedDataSourceEventArgs passed to the eventhandler</param>
        protected void DisplayStudentsAvailableForGroup(object sender, GridNeedDataSourceEventArgs e)
        {
            var studentsForGroup = LoadStudentsAvailableForGroup().ToList();
            grdAvailable.DataSource = studentsForGroup;
            lblAvailable.Text = string.Format("Available Students: " + studentsForGroup.Count);
        }

        /// <summary>
        /// Loads the appropriate Group object and sets it as the data source for the group detail grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DisplayDetailForGroup(object sender, GridNeedDataSourceEventArgs e)
        {
            _studentGroup = _groupProxy.GetGroup(GroupId, DistrictParms.LoadDistrictParms().ClientID);
            grdDetail.DataSource = new List<GroupDataContract> {_studentGroup};
            grdDetail.MasterTableView.GetColumn(EditGroupColumnName).Visible = GetEditColumnVisibility();
            SetPageTitle();
        }

        /// <summary>
        /// This method returns all students currently members of the group currently displayed
        /// </summary>
        /// <returns>A list of all students currently members of the group currently displayed</returns>
        private IEnumerable<GroupStudentDataContract> LoadStudentsInGroup()
        {
            var selectedStudents =
                _groupProxy.GetStudentsInGroup(GroupId, DistrictParms.LoadDistrictParms().ClientID);

            return selectedStudents;
        }

        /// <summary>
        /// This method makes a call to load all students in the current group then displays them in the
        /// selected students grid
        /// </summary>
        /// <param name="sender">Object that fired this event; grdSelected</param>
        /// <param name="e">GridNeedDataSourceEventArgs passed to the eventhandler</param>
        protected void DisplayStudentsInGroup(object sender, GridNeedDataSourceEventArgs e)
        {
            var studentsForGroup = LoadStudentsInGroup().ToList();
            grdSelected.DataSource = studentsForGroup;
            lblSelected.Text = string.Format("Selected Students: " + studentsForGroup.Count);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method refreshes the Databinding for the Available and Selected Grouping grids.
        /// This causes the UI to update
        /// </summary>
        private void RefreshDataBindingForGroupingGrids()
        {
            DisplayStudentsAvailableForGroup(null, null);
            DisplayStudentsInGroup(null, null);
            DisplayDetailForGroup(null, null);
            grdAvailable.Rebind();
            grdSelected.Rebind();
            grdDetail.Rebind();
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
            ScriptManager.RegisterStartupScript(Page, typeof (Page), "closeScript", "CloseDialog('Saved');", true);
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
            AddRemoveStudentsFromGroup(lstStudentIDs, isRemove);
        }

        /// <summary>
        /// This method initializes adding/removing a single student from a group based on Add or Remove
        /// </summary>
        /// <param name="studentID">ID of the student being added/removed</param>
        /// <param name="isRemove">Determines whether the mode is Removing or Adding</param>
        private void AddRemoveStudent(int studentID, bool isRemove)
        {
            List<int> studentIDS = new List<int> { studentID };
            AddRemoveStudentsFromGroup(studentIDS, isRemove);
        }

        /// <summary>
        /// This method performs the call to the group service to either delete or add a selection of students based on StudentID
        /// </summary>
        /// <param name="studentIDs">List of integer values representing IDs of students</param>
        /// <param name="isDelete">Determines if the method is a delete or add</param>
        private void AddRemoveStudentsFromGroup(List<int> studentIDs, bool isDelete)
        {
            if (isDelete)
            {
                _groupProxy.DeleteStudentsForGroup(GroupId, studentIDs, DistrictParms.LoadDistrictParms().ClientID);
            }
            else
            {
                _groupProxy.SaveStudentsForGroup(GroupId, studentIDs, DistrictParms.LoadDistrictParms().ClientID);
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
            var studentId = GetStudentIDFromLinkButton((LinkButton)sender);
            AddRemoveStudent(studentId, true);
        }

        #endregion
    }
}