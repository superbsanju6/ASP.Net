using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using System.Data.SqlClient;
using Thinkgate.Base.DataAccess;
using Thinkgate.Core.Extensions;
using Thinkgate.Classes;
using Newtonsoft.Json;

namespace Thinkgate.Account
{
    public partial class AdministrationV2 : System.Web.UI.Page
    {
        Thinkgate.Base.Classes.Administration adm = new Thinkgate.Base.Classes.Administration();
        List<ThinkgatePermission> AllPermissions = new List<ThinkgatePermission>();
        List<ThinkgateRole> AllRoles = new List<ThinkgateRole>();
        List<ThinkgateSchool> AllSchools = new List<ThinkgateSchool>();
        List<ThinkgatePricingModule> AllPricingModules = new List<ThinkgatePricingModule>();
        List<SchoolType> AllSchoolTypes = new List<SchoolType>();
        List<Grade> AllGrades = new List<Grade>();
        List<Subject> AllSubjects = new List<Subject>();
        List<Period> AllPeriods = new List<Period>();

        const string tabType = "tabType";

        private TabTypes currMode
        {
            get { return ViewState[tabType] != null ? (TabTypes)Enum.Parse(typeof(TabTypes), ViewState[tabType].ToString()) : TabTypes.None; }
            set { ViewState[tabType] = value.ToString(); }
        }

        private enum ItemType
        {
            None,
            AllPermissions,
            AllRoles,
            AllUsers,
            AllSchools,
            AllPricingModules,
            AllSchoolTypes,
            AllPortals,
            AllGrades,
            AllSubjects,
            AllPeriods
        }

        private enum TabTypes
        {
            None,
            PermissionList,
            PermissionCreate,
            PermissionEdit,
            PermissionRoles,
            PermissionUsers,
            RoleList,
            RoleCreate,
            RoleEdit,
            RolePermissions,
            RoleHierarchy,
            UserList,
            UserCreate,
            UserEdit,
            UserResetPassword,
            UserMultiResetPassword,
            UserMultiRole,
            UserMultiSchool,
            UserRoles,
            UserPermissions,
            UserSchools,
            SchoolList,
            SchoolCreate,
            SchoolEdit,
            PricingModuleList,
            PricingModuleStatus,
            GradeList,
            GradeCreate,
            GradeEdit,
            SubjectList,
            SubjectCreate,
            SubjectEdit,
            PeriodList,
            PeriodCreate,
            PeriodEdit
        }

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SearchEdit.SearchSelect += new EventHandler(Search_SelectSearch);
            SearchEdit.SearchSearch += new EventHandler(Search_SearchSearch);
            SearchEnable.SearchSelect += new EventHandler(Search_SelectSearch);
            SearchEnable.SearchSearch += new EventHandler(Search_SearchSearch);
            SearchSpecial.SearchSelect += new EventHandler(Search_SelectSearch);
            SearchSpecial.SearchSearch += new EventHandler(Search_SearchSearch);
            ComboEdit.ComboSelect += new EventHandler(Combo_ComboSelect);
            ComboEnable.ComboSelect += new EventHandler(Combo_ComboSelect);
            SearchSpecial.SearchSelect += new EventHandler(Search_SelectSearch);
            SearchSpecial.SearchSearch += new EventHandler(Search_SearchSearch);
            ComboSpecial.ComboSelect += new EventHandler(Combo_ComboSelect);
        }

        protected void rtsMain_TabClick(object sender, RadTabStripEventArgs e)
        {
            if (e.Tab == tabPermissionList)
            {
                currMode = TabTypes.PermissionList;
                gdlList.DataBind(GridList.ListMode.Permissions);
            }
            else if (e.Tab == tabPermissionCreate)
            {
                currMode = TabTypes.PermissionCreate;
            }
            else if (e.Tab == tabPermissionEdit)
            {
                currMode = TabTypes.PermissionEdit;
            }
            else if (e.Tab == tabPermissionRoles)
            {
                currMode = TabTypes.PermissionRoles;
                ComboEnable.ReloadGUI(SearchCombo.ComboModes.Permissions);
                LoadEnableView();
            }
            else if (e.Tab == tabPermissionUsers)
            {
                currMode = TabTypes.PermissionUsers;
                ComboEnable.ReloadGUI(SearchCombo.ComboModes.Permissions);
                LoadEnableView();
            }
            else if (e.Tab == tabRoleList)
            {
                currMode = TabTypes.RoleList;
                gdlList.DataBind(GridList.ListMode.Roles);
            }
            else if (e.Tab == tabRoleCreate)
            {
                currMode = TabTypes.RoleCreate;
            }
            else if (e.Tab == tabRoleEdit)
            {
                currMode = TabTypes.RoleEdit;
                ComboEdit.ReloadGUI(SearchCombo.ComboModes.Roles);
                LoadEditView();
            }
            else if (e.Tab == tabRolePermissions)
            {
                currMode = TabTypes.RolePermissions;
                ComboEnable.ReloadGUI(SearchCombo.ComboModes.Roles);
                LoadEnableView();
            }
            else if (e.Tab == tabRoleHiearchy)
            {
                currMode = TabTypes.RoleHierarchy;
                LoadSortList();
            }
            else if (e.Tab == tabUserList)
            {
                currMode = TabTypes.UserList;
            }
            else if (e.Tab == tabUserCreate)
            {
                currMode = TabTypes.UserCreate;
                LoadCreateView();
            }
            else if (e.Tab == tabUserEdit)
            {
                currMode = TabTypes.UserEdit;
                SearchEdit.ReloadGUI(SearchGrid.SearchModes.User);
                LoadEditView();
            }
            else if (e.Tab == tabUserResetPassword)
            {
                currMode = TabTypes.UserResetPassword;
                SearchSpecial.ReloadGUI(SearchGrid.SearchModes.User);
                LoadSpecialView();
            }
            else if (e.Tab == tabUserMultiPasswords)
            {
                currMode = TabTypes.UserMultiResetPassword;
            }
            else if (e.Tab == tabUserMultiRoles)
            {
                currMode = TabTypes.UserMultiRole;
            }
            else if (e.Tab == tabUserMultiSchools)
            {
                currMode = TabTypes.UserMultiSchool;
            }
            else if (e.Tab == tabUserRoles)
            {
                currMode = TabTypes.UserRoles;
                SearchEnable.ReloadGUI(SearchGrid.SearchModes.User);
                LoadEnableView();
            }
            else if (e.Tab == tabUserPermissions)
            {
                currMode = TabTypes.UserPermissions;
                SearchEnable.ReloadGUI(SearchGrid.SearchModes.User);
                LoadEnableView();
            }
            else if (e.Tab == tabUserSchools)
            {
                currMode = TabTypes.UserSchools;
                SearchEnable.ReloadGUI(SearchGrid.SearchModes.User);
                LoadEnableView();
            }
            else if (e.Tab == tabSchoolList)
            {
                currMode = TabTypes.SchoolList;
                gdlList.DataBind(GridList.ListMode.Schools);
            }
            else if (e.Tab == tabSchoolCreate)
            {
                currMode = TabTypes.SchoolCreate;
                LoadCreateView();
            }
            else if (e.Tab == tabSchoolEdit)
            {
                currMode = TabTypes.SchoolEdit;
                ComboEdit.ReloadGUI(SearchCombo.ComboModes.Schools);
                LoadEditView();
            }
            else if (e.Tab == tabPricingModuleList)
            {
                currMode = TabTypes.PricingModuleList;
                gdlList.DataBind(GridList.ListMode.PricingModules);
            }
            else if (e.Tab == tabPricingModuleStatus)
            {
                currMode = TabTypes.PricingModuleStatus;
                LoadEnableView();
            }
            else if (e.Tab == tabGradeList)
            {
                currMode = TabTypes.GradeList;
                gdlList.DataBind(GridList.ListMode.Grades);
            }
            else if (e.Tab == tabGradeCreate)
            {
                currMode = TabTypes.GradeCreate;
            }
            else if (e.Tab == tabGradeEdit)
            {
                currMode = TabTypes.GradeEdit;
            }
            else if (e.Tab == tabSubjectList)
            {
                currMode = TabTypes.SubjectList;
                gdlList.DataBind(GridList.ListMode.Subjects);
            }
            else if (e.Tab == tabSubjectCreate)
            {
                currMode = TabTypes.SubjectCreate;
                LoadCreateView();
            }
            else if (e.Tab == tabSubjectEdit)
            {
                currMode = TabTypes.SubjectEdit;
                ComboEdit.ReloadGUI(SearchCombo.ComboModes.Subjects);
                LoadEditView();
            }
            else if (e.Tab == tabPeriodList)
            {
                currMode = TabTypes.PeriodList;
                gdlList.DataBind(GridList.ListMode.Periods);
            }
        }

        private void LoadSortList()
        {
            ResetSortGUI();

            switch (currMode)
            {
                case TabTypes.RoleHierarchy:
                    lbxSort.DataSource = AllRoles.OrderBy(x => x.RoleHierarchyLevel).Select(y => y.RoleName);
                    lbxSort.DataBind();
                    break;
                default:
                    lbxSort.DataSource = null;
                    break;
            }
        }

        private void LoadCreateView()
        {
            ResetCreateGUI();

            switch (currMode)
            {
                case TabTypes.UserCreate:
                    pnlUserCreate.Visible = true;
                    BindComboBox(cmbCreateUserRoles, false, ItemType.AllRoles);
                    BindComboBox(cmbCreateUserSchools, false, ItemType.AllSchools);
                    break;
                case TabTypes.SchoolCreate:
                    pnlSchoolCreate.Visible = true;
                    BindComboBox(cmbSchoolType, false, ItemType.AllSchoolTypes);
                    break;
                case TabTypes.SubjectCreate:
                    pnlSubjectCreate.Visible = true;
                    break;
            }

            btnCreate.Visible = true;
        }

        private void LoadEditView()
        {
            ResetEditGUI();

            switch (currMode)
            {
                case TabTypes.RoleEdit:
                case TabTypes.SchoolEdit:
                case TabTypes.SubjectEdit:
                    ComboEdit.Visible = true;
                    SearchEdit.Visible = false;
                    break;
                case TabTypes.UserEdit:
                    ComboEdit.Visible = false;
                    SearchEdit.Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void LoadEnableView()
        {
            ResetEnableGUI();

            switch (currMode)
            {
                case TabTypes.PermissionRoles:
                case TabTypes.PermissionUsers:
                case TabTypes.RolePermissions:
                    ComboEnable.Visible = true;
                    SearchEnable.Visible = false;
                    break;
                case TabTypes.UserRoles:
                case TabTypes.UserPermissions:
                case TabTypes.UserSchools:
                    ComboEnable.Visible = false;
                    SearchEnable.Visible = true;
                    break;
                case TabTypes.PricingModuleStatus:
                    ComboEnable.Visible = false;
                    SearchEnable.Visible = false;
                    grdEnable.Visible = true;
                    grdEnable.DataSource = AllPricingModules;
                    grdEnable.DataBind();
                    btnEnable.Visible = true;
                    btnPrint.Visible = true;
                    break;
            }
        }

        protected void Search_SearchSearch(object sender, EventArgs e)
        {
            switch (currMode)
            {
                case TabTypes.UserEdit:
                    ComboEdit.Visible = false;
                    SearchEdit.Visible = true;
                    pnlEditUser.Visible = false;
                    btnEdit.Visible = false;
                    lblEdit.Text = string.Empty;
                    break;
                case TabTypes.UserRoles:
                case TabTypes.UserPermissions:
                case TabTypes.UserSchools:
                    ComboEnable.Visible = false;
                    SearchEnable.Visible = true;
                    grdEnable.Visible = false;
                    btnEnable.Visible = false;
                    btnPrint.Visible = false;
                    lblEnable.Text = string.Empty;
                    break;
                default:
                    break;
            }
        }

        protected void Search_SelectSearch(object sender, EventArgs e)
        {
            switch (currMode)
            {
                case TabTypes.UserEdit:
                    BindEdit(((GridDataItem)((GridCommandEventArgs)e).Item)["UserId"].Text);
                    break;
                case TabTypes.UserRoles:
                case TabTypes.UserPermissions:
                case TabTypes.UserSchools:
                    BindEnable(((GridDataItem)((GridCommandEventArgs)e).Item)["UserId"].Text);
                    break;
                case TabTypes.UserResetPassword:
                    BindSpecial(((GridDataItem)((GridCommandEventArgs)e).Item)["username"].Text);
                    break;
            }
        }

        protected void Combo_ComboSelect(object sender, EventArgs e)
        {
            switch (currMode)
            {
                case TabTypes.PermissionRoles:
                case TabTypes.PermissionUsers:
                case TabTypes.RolePermissions:
                    BindEnable(((SearchCombo)sender).Attributes[ComboEnable.ID.ToString()]);
                    break;
                case TabTypes.RoleEdit:
                case TabTypes.SchoolEdit:
                case TabTypes.SubjectEdit:
                    BindEdit(((SearchCombo)sender).Attributes[ComboEdit.ID.ToString()]);
                    break;
            }
            btnEdit.Visible = true;
        }

        private void BindEnable(string binding)
        {
            switch (currMode)
            {
                case TabTypes.PermissionRoles:
                    grdEnable.DataSource = adm.GetRolesinPermission(binding);
                    break;
                case TabTypes.PermissionUsers:
                    grdEnable.DataSource = adm.GetUsersinPermission(binding);
                    break;
                case TabTypes.RolePermissions:
                    grdEnable.DataSource = adm.GetPermissionsInRole(new Guid(binding));
                    break;
                case TabTypes.UserRoles:
                    grdEnable.DataSource = adm.GetRolesForUser(new Guid(binding));
                    break;
                case TabTypes.UserPermissions:
                    grdEnable.DataSource = adm.GetPermissionsForUser(new Guid(binding));
                    break;
                case TabTypes.UserSchools:
                    grdEnable.DataSource = adm.GetSchoolsForUser(new Guid(binding));
                    break;
            }

            grdEnable.DataBind();
            grdEnable.Visible = true;

            btnEnable.Visible = true;
            btnPrint.Visible = true;
            lblEnable.Text = string.Empty;
        }

        private void BindEdit(string binding)
        {
            btnEdit.Visible = true;
            lblEdit.Text = string.Empty;

            switch (currMode)
            {
                case TabTypes.RoleEdit:
                    ComboEdit.Visible = true;
                    SearchEdit.Visible = false;
                    pnlEditRole.Visible = true;
                    BindComboBox(cmbEditRolePortal, false, ItemType.AllPortals);
                    ThinkgateRole selectedRole = AllRoles.Find(x => x.RoleId.ToString().ToLower() == binding.ToLower());
                    lblRoleName.Text = selectedRole.RoleName;
                    txtRoleDescription.Text = selectedRole.Description;
                    RadComboBoxItem selectedRoleItem = cmbEditRolePortal.FindItemByValue(selectedRole.RolePortalSelection.ToString(), true);
                    selectedRoleItem.Selected = true;
                    chkEditRoleActive.Checked = selectedRole.Active;
                    break;
                case TabTypes.UserEdit:
                    ComboEdit.Visible = false;
                    SearchEdit.Visible = true;
                    pnlEditUser.Visible = true;
                    ThinkgateUser selectedUser = ThinkgateUser.GetThinkgateUserByID(new Guid(binding));
                    txtUserEditFirstName.Text = selectedUser.FirstName;
                    txtUserEditMiddleName.Text = selectedUser.MiddleName;
                    txtUserEditLastName.Text = selectedUser.LastName;
                    txtUserEditLoginID.Text = selectedUser.UserName;
                    txtUserEditEmail.Text = selectedUser.Email;
                    txtUserEditTeacherID.Text = selectedUser.TeacherID;
                    chkUserEditIsApproved.Checked = selectedUser.IsApproved;
                    chkUserEditIsLocked.Checked = selectedUser.IsLockedOut;
                    lblUserEditCreatedDate.Text = selectedUser.CreationDate.ToShortDateString();
                    lblUserEditLastLogin.Text = selectedUser.LastLoginDate.ToShortDateString();
                    lblUserEditLastPasswordChange.Text = selectedUser.LastPasswordChangedDate.ToShortDateString();
                    break;
                case TabTypes.SchoolEdit:
                    ComboEdit.Visible = true;
                    SearchEdit.Visible = false;
                    pnlEditSchool.Visible = true;
                    BindComboBox(cmbSchoolEditType, false, ItemType.AllSchoolTypes);
                    ThinkgateSchool selectedSchool = AllSchools.Find(x => x.Id.ToString().ToLower() == binding.ToLower());
                    txtSchoolEditName.Text = selectedSchool.Name;
                    txtSchoolEditAbbr.Text = selectedSchool.Abbreviation;
                    txtSchoolEditCluster.Text = selectedSchool.Cluster;
                    txtSchoolEditID.Text = selectedSchool.SchoolId;
                    txtSchoolEditPhone.Text = selectedSchool.Phone;
                    RadComboBoxItem selectedSchoolItem = cmbSchoolEditType.FindItemByValue(selectedSchool.SchoolType.ToString(), true);
                    selectedSchoolItem.Selected = true;
                    break;
                case TabTypes.SubjectEdit:
                    ComboEdit.Visible = true;
                    SearchEdit.Visible = false;
                    pnlEditSubject.Visible = true;
                    Subject sub = AllSubjects.Find(x => x.DisplayText.ToLower() == binding.ToLower());
                    txtEditSubjectName.Text = sub.DisplayText;
                    txtEditSubjectAbbreviation.Text = sub.Abbreviation;
                    break;
            }
        }

        private void BindSpecial(string binding)
        {
            switch (currMode)
            {
                case TabTypes.UserResetPassword:
                    pnlResetPassword.Visible = true;
                    lblResetUser.Text = binding;
                    btnSpecial.Text = "Reset";
                    btnSpecial.Visible = true;
                    // set parms values in hidden field.
                    var DParmsNew = DistrictParms.LoadDistrictParms();
                    hdnPasswordFormatReg.Value = DParmsNew.PasswordFormatReg;
                    hdnPasswordConfReq.Value = DParmsNew.PasswordConfigurationRequired;
                    hdnPasswordValidationMsg.Value = DParmsNew.PasswordValidationMsg;
                    if (hdnPasswordConfReq.Value == "No")
                    {
                        h1MinimumPassword.Visible = false;
                    }
                    else
                        displayMsgChild.InnerHtml = MinimumPasswordRequirementHelper.GetMinumumPasswordRequirementMsg();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPermissions();
                LoadRoles();
                LoadSchools();
                LoadPricingModules();
                LoadSchoolTypes();
                LoadGrades();
                LoadSubjects();
                LoadPeriods();
            }
            else
            {
                AllPermissions = (List<ThinkgatePermission>)Cache.Get(ItemType.AllPermissions.ToString());
                AllRoles = (List<ThinkgateRole>)Cache.Get(ItemType.AllRoles.ToString());
                AllSchools = (List<ThinkgateSchool>)Cache.Get(ItemType.AllSchools.ToString());
                AllPricingModules = (List<ThinkgatePricingModule>)Cache.Get(ItemType.AllPricingModules.ToString());
                AllSchoolTypes = (List<SchoolType>)Cache.Get(ItemType.AllSchoolTypes.ToString());
                AllGrades = (List<Grade>)Cache.Get(ItemType.AllGrades.ToString());
                AllSubjects = (List<Subject>)Cache.Get(ItemType.AllSubjects.ToString());
                AllPeriods = (List<Period>)Cache.Get(ItemType.AllPeriods.ToString());
            }
        }

        #region Initial Loading Of Classes
        private void LoadPermissions()
        {
            AllPermissions = adm.GetPermissionsAll();
            InsertCache(ItemType.AllPermissions);
        }

        private void LoadRoles()
        {
            AllRoles = adm.GetRolesAll();
            InsertCache(ItemType.AllRoles);
        }

        private void LoadSchools()
        {
            AllSchools = adm.GetSchoolsAll();
            InsertCache(ItemType.AllSchools);
        }

        private void LoadPricingModules()
        {
            AllPricingModules = adm.GetPricingModules();
            InsertCache(ItemType.AllPricingModules);
        }

        private void LoadSchoolTypes()
        {
            AllSchoolTypes = adm.GetSchoolTypes();
            InsertCache(ItemType.AllSchoolTypes);
        }

        private void LoadGrades()
        {
            AllGrades = adm.GetGradesAll();
            InsertCache(ItemType.AllGrades);
        }

        private void LoadSubjects()
        {
            AllSubjects = adm.GetSubjectsAll();
            InsertCache(ItemType.AllSubjects);
        }

        private void LoadPeriods()
        {
            AllPeriods = adm.GetPeriodsAll();
            InsertCache(ItemType.AllPeriods);
        }
        #endregion

        #region ListRegion
        protected void grdList_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            if (sender is RadGrid && currMode != TabTypes.None)
            {
                switch (currMode)
                {
                    case TabTypes.PermissionList:
                        e.Column.Visible = e.Column.UniqueName.ToLower() != "permissionname" ? false : true;
                        break;
                    case TabTypes.RoleList:
                        e.Column.Visible = e.Column.UniqueName.ToLower() != "rolename" ? false : true;
                        break;
                    case TabTypes.SchoolList:
                    case TabTypes.PricingModuleList:
                        e.Column.Visible = e.Column.UniqueName.ToLower() != "name" ? false : true;
                        break;
                    case TabTypes.UserList:
                        if ((e.Column.UniqueName.ToLower() != "username") && (e.Column.UniqueName.ToLower() != "firstname") && e.Column.UniqueName.ToLower() != "lastname")
                        {
                            e.Column.Visible = false;
                        }
                        break;
                    case TabTypes.SubjectList:
                        if ((e.Column.UniqueName.ToLower() != "displaytext") && (e.Column.UniqueName.ToLower() != "abbreviation"))
                        {
                            e.Column.Visible = false;
                        }
                        break;
                    case TabTypes.PeriodList:
                        e.Column.Visible = e.Column.UniqueName.ToLower() != "period" ? false : true;
                        break;
                }
            }
        }
        #endregion

        #region CreateRegion
        private void CreateUser()
        {
            List<string> roles = new List<string>();
            List<int> schoolIDs = new List<int>();

            foreach (RadComboBoxItem cbi in cmbCreateUserRoles.Items)
            {
                if (cbi.Checked)
                {
                    roles.Add(cbi.Text.ToString());
                }
            }

            foreach (RadComboBoxItem cbi in cmbCreateUserSchools.Items)
            {
                if (cbi.Checked)
                {
                    schoolIDs.Add(Convert.ToInt32(cbi.Value.ToString()));
                }
            }

            if (txtCreateUserUserName.Text == string.Empty || txtCreateUserLastName.Text == string.Empty || txtCreateUserFirstName.Text == string.Empty || roles.Count < 1 || schoolIDs.Count < 1 || cmbCreateUserPrimaryRole.SelectedItem == null || cmbCreateUserPrimarySchool.SelectedItem == null)
            {
                lblCreate.Text = "Validation failed. Please make sure the user has: First Name, Last Name, LoginID, At least one Role and Primary Role, and at least one School and Primary School.";
            }
            else if (Membership.FindUsersByName(txtCreateUserUserName.Text).Count > 0)
            {
                lblCreate.Text = "User " + txtCreateUserUserName.Text + " already exists!";
                txtCreateUserEmail.Text = string.Empty;
                txtCreateUserFirstName.Text = string.Empty;
                txtCreateUserLastName.Text = string.Empty;
                txtCreateUserMiddleName.Text = string.Empty;
                txtCreateUserTeacherID.Text = string.Empty;
                txtCreateUserUserName.Text = string.Empty;
                cmbCreateUserPrimaryRole.ClearSelection();
                cmbCreateUserPrimarySchool.ClearSelection();
                cmbCreateUserRoles.ClearCheckedItems();
                cmbCreateUserSchools.ClearCheckedItems();
            }
            else
            {
                adm.CreateUser(txtCreateUserUserName.Text, txtCreateUserFirstName.Text, txtCreateUserMiddleName.Text, txtCreateUserLastName.Text, txtCreateUserEmail.Text, roles, schoolIDs, cmbCreateUserPrimaryRole.SelectedItem.Text, cmbCreateUserPrimarySchool.SelectedItem.Value, txtCreateUserTeacherID.Text);
                KenticoBusiness.AddUserAndRoles(txtCreateUserUserName.Text);

                //Dan - UserSync - Queue a UserSync Message here!
                //TODO: Michael Rue - complete user sync functionality
                //UserSyncHelperFactory.GetMsmqHelper().AddOrUpdateUser(txtCreateUserUserName.Text, null, null, txtCreateUserEmail.Text, JsonConvert.SerializeObject(roles));

                lblCreate.Text = "Success! User: " + txtCreateUserUserName.Text + " Created.";
                txtCreateUserEmail.Text = string.Empty;
                txtCreateUserFirstName.Text = string.Empty;
                txtCreateUserLastName.Text = string.Empty;
                txtCreateUserMiddleName.Text = string.Empty;
                txtCreateUserTeacherID.Text = string.Empty;
                txtCreateUserUserName.Text = string.Empty;
                cmbCreateUserPrimaryRole.ClearSelection();
                cmbCreateUserPrimarySchool.ClearSelection();
                cmbCreateUserRoles.ClearCheckedItems();
                cmbCreateUserSchools.ClearCheckedItems();
            }
        }

        private void CreateSchool()
        {
            if (txtSchoolAbbreviation.Text == string.Empty || txtSchoolID.Text == string.Empty || txtSchoolName.Text == string.Empty || cmbSchoolType.Text == "Select...")
            {
                lblCreate.Text = "You must enter a Name, Type, Abbreviation, District, and School ID. Please try again.";
                return;
            }
            else if (AllSchools.Find(x => x.SchoolId.ToString().ToLower() == txtSchoolID.Text.ToLower()) != null)
            {
                lblCreate.Text = "There is already a school with this School ID assigned. Please choose a different School ID.";
                return;
            }

            adm.CreateSchool(txtSchoolName.Text, cmbSchoolType.Text, txtSchoolCluster.Text, txtSchoolAbbreviation.Text, txtSchoolPhone.Text, txtSchoolID.Text);
            lblCreate.Text = "Success. School Created.";
            txtSchoolName.Text = string.Empty;
            txtSchoolAbbreviation.Text = string.Empty;
            txtSchoolPhone.Text = string.Empty;
            txtSchoolCluster.Text = string.Empty;
            cmbSchoolType.ClearSelection();
            cmbSchoolType.Text = cmbSchoolType.EmptyMessage;
            txtSchoolID.Text = string.Empty;
            LoadSchools();
        }

        private void CreateSubject()
        {
            if (txtSubjectName.Text == string.Empty || txtSubjectAbbreviation.Text == string.Empty)
            {
                lblCreate.Text = "You must enter a Name, Type, Abbreviation, District, and School ID. Please try again.";
                return;
            }
            else if (AllSubjects.Find(x => x.DisplayText.ToString().ToLower() == txtSubjectName.Text.ToLower()) != null)
            {
                lblCreate.Text = "There is already a subject with this name assigned. Please choose a different name.";
                return;
            }

            adm.CreateSubject(txtSubjectName.Text, txtSubjectAbbreviation.Text);
            lblCreate.Text = "Success. Subject Created.";
            txtSubjectAbbreviation.Text = string.Empty;
            txtSubjectName.Text = string.Empty;
            LoadSubjects();
        }

        protected void btnCreate_OnClick(object o, EventArgs e)
        {
            switch (currMode)
            {
                case TabTypes.UserCreate:
                    CreateUser();
                    break;
                case TabTypes.SchoolCreate:
                    CreateSchool();
                    break;
                case TabTypes.SubjectCreate:
                    CreateSubject();
                    break;
            }
        }
        #endregion

        #region EditRegion


        protected void btnSpecial_OnClick(object o, EventArgs e)
        {
            if (o is RadButton && currMode != TabTypes.None)
            {
                switch (currMode)
                {
                    case TabTypes.UserResetPassword:
                        ResetPasswords();
                        break;
                }
            }

        }

        protected void btnEdit_OnClick(object o, EventArgs e)
        {
            if (o is RadButton && currMode != TabTypes.None)
            {
                switch (currMode)
                {
                    case TabTypes.RoleEdit:
                        SaveRole();
                        break;
                    case TabTypes.UserEdit:
                        SaveUser();
                        break;
                    case TabTypes.SchoolEdit:
                        SaveSchool();
                        break;
                    case TabTypes.SubjectEdit:
                        SaveSubject();
                        break;
                }
            }
        }

        private void SaveSchool()
        {
            if (ComboEdit.Attributes[ComboEdit.ID.ToString()] == null || txtSchoolEditName.Text == string.Empty || txtSchoolEditID.Text == string.Empty)
            {
                lblEdit.Text = "The School Name and/or SchoolID are not valid. Please try again.";
                return;
            }

            adm.UpdateSchool(Convert.ToInt32(ComboEdit.Attributes[ComboEdit.ID.ToString()]),
                 txtSchoolEditName.Text,
                 txtSchoolEditAbbr.Text,
                 txtSchoolEditPhone.Text,
                 txtSchoolEditCluster.Text,
                 cmbSchoolEditType.Text,
                 txtSchoolEditID.Text);
            ThinkgateSchool school = AllSchools.Find(x => x.Id.ToString().ToLower() == ComboEdit.Attributes[ComboEdit.ID.ToString()].ToLower());
            school.Name = txtSchoolEditName.Text;
            school.Abbreviation = txtSchoolEditAbbr.Text;
            school.Phone = txtSchoolEditPhone.Text;
            school.Cluster = txtSchoolCluster.Text;
            school.SchoolType = cmbSchoolEditType.Text;
            school.SchoolId = txtSchoolEditID.Text;
            lblEdit.Text = "School Saved Successfully!";
            InsertCache(ItemType.AllSchools);
        }

        private void SaveSubject()
        {
            if (ComboEdit.Attributes[ComboEdit.ID.ToString()] == null || txtEditSubjectName.Text == string.Empty || txtEditSubjectAbbreviation.Text == string.Empty)
            {
                lblEdit.Text = "The Subject Name and/or Abbreviation are not valid. Please try again.";
                return;
            }

            adm.UpdateSubject(ComboEdit.Attributes[ComboEdit.ID.ToString()], txtEditSubjectName.Text, txtEditSubjectAbbreviation.Text);
            Subject subject = AllSubjects.Find(x => x.DisplayText.ToLower() == ComboEdit.Attributes[ComboEdit.ID.ToString()].ToLower());
            subject.DisplayText = txtEditSubjectName.Text;
            subject.Abbreviation = txtEditSubjectAbbreviation.Text;
            lblEdit.Text = "Subject Saved Successfully!";
            InsertCache(ItemType.AllSubjects);
        }

        private void SaveRole()
        {
            if (ComboEdit.Attributes[ComboEdit.ID.ToString()] == null || lblRoleName.Text == string.Empty || cmbEditRolePortal.SelectedValue == null)
            {
                lblEdit.Text = "RoleName and Portal Selection are required";
                return;
            }

            adm.UpdateRole(new Guid(ComboEdit.Attributes[ComboEdit.ID.ToString()]), lblRoleName.Text, txtRoleDescription.Text, Convert.ToInt32(cmbEditRolePortal.SelectedValue), chkEditRoleActive.Checked);
            ThinkgateRole role = AllRoles.Find(x => x.RoleId.ToString().ToLower() == ComboEdit.Attributes[ComboEdit.ID.ToString()].ToLower());
            role.RolePortalSelection = Convert.ToInt32(cmbEditRolePortal.SelectedValue);
            role.Description = txtRoleDescription.Text;
            role.Active = chkEditRoleActive.Checked;
            lblEdit.Text = ComboEdit.GetComboTextByValue(ComboEdit.Attributes[ComboEdit.ID.ToString()]) + " saved successfully!";
            InsertCache(ItemType.AllRoles);
        }

        private void SaveUser()
        {
            if (txtUserEditLoginID.Text == string.Empty || txtUserEditFirstName.Text == string.Empty || txtUserEditLastName.Text == string.Empty)
            {
                lblEdit.Text = "LoginID, First Name, and Last Name must have values!";
                return;
            }

            adm.UpdateUser(new Guid(SearchEdit.Attributes[SearchEdit.ID.ToString()]),
            txtUserEditFirstName.Text,
            txtUserEditMiddleName.Text,
            txtUserEditLastName.Text,
            txtUserEditLoginID.Text.ToLower(),
            txtUserEditTeacherID.Text,
            txtUserEditEmail.Text,
            chkUserEditIsApproved.Checked,
            chkUserEditIsLocked.Checked);
            lblEdit.Text = "User: " + txtUserEditLoginID.Text + " Saved Successfully!";
        }

        private void ResetEditGUI()
        {
            foreach (Control p in rpvEdit.Controls)
            {
                if (p is Panel)
                {
                    p.Visible = false;
                }
            }

            btnEdit.Visible = false;
            lblEdit.Text = string.Empty;
        }
        #endregion



        private void LoadSpecialView()
        {
            ResetSpecialGUI();

            switch (currMode)
            {
                case TabTypes.UserResetPassword:
                    ComboSpecial.Visible = false;
                    SearchSpecial.Visible = true;
                    break;
            }
        }

        private void ResetListGUI()
        {
            //grdList.DataSource = null;
            //grdList.DataBind();
        }

        private void ResetSpecialGUI()
        {
            foreach (Control p in rpvSpecial.Controls)
            {
                if (p is Panel)
                {
                    p.Visible = false;
                }
            }

            btnSpecial.Visible = false;
            lblSpecial.Text = string.Empty;
        }

        private void ResetCreateGUI()
        {
            foreach (Control p in rpvCreate.Controls)
            {
                if (p is Panel)
                {
                    p.Visible = false;
                }
            }

            btnCreate.Visible = false;
            lblCreate.Text = string.Empty;
        }

        private void ResetSortGUI()
        {
            lbxSort.DataSource = null;
            lbxSort.DataBind();

            lblSort.Text = string.Empty;
        }

        private void ResetEnableGUI()
        {
            grdEnable.DataSource = null;
            grdEnable.DataBind();
            grdEnable.Visible = false;

            btnEnable.Visible = false;
            btnPrint.Visible = false;
            lblEnable.Text = string.Empty;
        }
        #region Simple Button Events

        protected void btnPrint_OnClick(object o, EventArgs e)
        {
            if (o is RadButton && currMode != TabTypes.None)
            {
                switch (currMode)
                {
                    case TabTypes.PermissionRoles:
                    case TabTypes.PermissionUsers:
                    case TabTypes.RolePermissions:
                    case TabTypes.UserRoles:
                    case TabTypes.UserPermissions:
                    case TabTypes.UserSchools:
                    case TabTypes.PricingModuleStatus:
                        PrintGridToPDF(grdEnable);
                        break;
                }
            }
        }

        private void PrintGridToPDF(RadGrid grid)
        {
            grid.ExportSettings.OpenInNewWindow = true;
            foreach (GridDataItem item in grid.MasterTableView.Items)
            {
                CheckBox chkEnabled;
                GridColumn column;
                switch (currMode)
                {
                    case TabTypes.PricingModuleStatus:
                        chkEnabled = (CheckBox)item["Active"].Controls[0];
                        column = grid.MasterTableView.GetColumn("Active");
                        break;
                    default:
                        chkEnabled = (CheckBox)item["Enabled"].Controls[0];
                        column = grid.MasterTableView.GetColumn("Enabled");
                        break;
                }
                item.Visible = chkEnabled.Checked == true ? true : false;
                column.Visible = false;
            }
            grid.MasterTableView.ExportToPdf();
        }

        protected void grid_PdfExporting(object sender, GridPdfExportingArgs e)
        {
            switch (currMode)
            {
                case TabTypes.PermissionRoles:
                case TabTypes.PermissionUsers:
                case TabTypes.RolePermissions:
                    e.RawHTML = "<div style='font-weight: bold; font-size: larger; border-bottom: 5px; text-align: center;'>" + rtsMain.SelectedTab.SelectedTab.Text + "</div><div style='font-weight: bold; font-size: larger; text-align: center;'>" + ComboEnable.GetComboTextByValue(ComboEnable.Attributes[ComboEnable.ID.ToString()]) + "</div>" + e.RawHTML;
                    break;
                case TabTypes.UserRoles:
                case TabTypes.UserPermissions:
                case TabTypes.UserSchools:
                    e.RawHTML = "<div style='font-weight: bold; font-size: larger; border-bottom: 5px; text-align: center;'>" + rtsMain.SelectedTab.SelectedTab.Text + "</div><div style='font-weight: bold; font-size: larger; text-align: center;'>" + SearchEnable.Attributes[SearchEnable.userName] + "</div>" + e.RawHTML;
                    break;
                case TabTypes.PricingModuleStatus:
                    e.RawHTML = "<div style='font-weight: bold; font-size: larger; border-bottom: 5px; text-align: center;'>" + rtsMain.SelectedTab.SelectedTab.Text + "</div><div style='font-weight: bold; font-size: larger; text-align: center;'></div>" + e.RawHTML;
                    break;
                case TabTypes.None:
                    break;
            }
        }

        protected void btnEnable_OnClick(object o, EventArgs e)
        {
            if (o is RadButton && currMode != TabTypes.None)
            {
                switch (currMode)
                {
                    case TabTypes.PermissionRoles:
                        adm.UpdatePermWithRoles(GetCheckedItemsFromGrid(grdEnable).Cast<string>().ToList(), new Guid(ComboEnable.Attributes[ComboEnable.ID.ToString()]));
                        lblEnable.Text = "Roles Saved for " + ComboEnable.GetComboTextByValue(ComboEnable.Attributes[ComboEnable.ID.ToString()]) + ".";
                        break;
                    case TabTypes.PermissionUsers:
                        adm.UpdatePermWithUsers(GetCheckedItemsFromGrid(grdEnable).Cast<string>().ToList(), new Guid(ComboEnable.Attributes[ComboEnable.ID.ToString()]));
                        lblEnable.Text = "Users Saved for " + ComboEnable.GetComboTextByValue(ComboEnable.Attributes[ComboEnable.ID.ToString()]) + ".";
                        break;
                    case TabTypes.RolePermissions:
                        adm.UpdateRoleWithPermissions(GetCheckedItemsFromGrid(grdEnable).Cast<string>().ToList(), new Guid(ComboEnable.Attributes[ComboEnable.ID.ToString()]));
                        lblEnable.Text = "Permissions Saved for " + ComboEnable.GetComboTextByValue(ComboEnable.Attributes[ComboEnable.ID.ToString()]) + ".";
                        break;
                    case TabTypes.UserPermissions:
                        adm.UpdateUserWithPermissions(GetCheckedItemsFromGrid(grdEnable).Cast<string>().ToList(), new Guid(SearchEnable.Attributes[SearchEnable.ID.ToString()]));
                        lblEnable.Text = "Permissions Saved!";
                        break;
                    case TabTypes.UserRoles:
                        adm.UpdateUserWithRoles(GetCheckedItemsFromGrid(grdEnable).Cast<string>().ToList(), new Guid(SearchEnable.Attributes[SearchEnable.ID.ToString()]));
                        lblEnable.Text = "Roles Saved!";
                        break;
                    case TabTypes.UserSchools:
                        adm.UpdateUserWithSchools(GetCheckedItemsFromGrid(grdEnable).ConvertAll(i => Convert.ToInt32(i.ToString())).ToList(), new Guid(SearchEnable.Attributes[SearchEnable.ID.ToString()]));
                        lblEnable.Text = "Schools Saved!";
                        break;
                    case TabTypes.PricingModuleStatus:
                        adm.UpdatePricingModules(GetCheckedItemsFromGrid(grdEnable).ConvertAll(i => Convert.ToInt32(i.ToString())).ToList());
                        AllPricingModules = adm.GetPricingModules();
                        InsertCache(ItemType.AllPricingModules);
                        lblEnable.Text = "Pricing Modules Saved!";
                        break;
                }
            }
        }
        #endregion

        #region Shared Procedures

        private List<object> GetCheckedItemsFromGrid(RadGrid grid)
        {
            List<object> lstItems = new List<object>();

            switch (currMode)
            {
                case TabTypes.PricingModuleStatus:
                    foreach (GridDataItem gdi in grid.Items)
                    {
                        if (((CheckBox)gdi["Active"].Controls[0]).Checked)
                        {
                            lstItems.Add(gdi["Id"].Text);
                        }
                    }
                    break;
                default:
                    foreach (GridDataItem gdi in grid.Items)
                    {
                        if (((CheckBox)gdi["Enabled"].Controls[0]).Checked)
                        {
                            lstItems.Add(gdi["IDLabel"].Text);
                        }
                    }
                    break;
            }

            return lstItems;
        }

        private List<object> GetListBoxOrder(RadListBox listBox)
        {
            List<object> lstItems = new List<object>();

            switch (currMode)
            {
                case TabTypes.RoleHierarchy:
                    for (int i = 0; i < listBox.Items.Count; i++)
                    {
                        lstItems.Add(AllRoles.First(x => x.RoleName == listBox.Items[i].Text).RoleId);
                    }
                    break;
            }

            return lstItems;
        }

        private void BindComboBox(RadComboBox comboBox, bool hasAll, ItemType type)
        {
            string text = string.Empty;
            string value = string.Empty;
            comboBox.Items.Clear();

            switch (type)
            {
                case ItemType.AllRoles:
                    text = "rolename";
                    value = "roleid";
                    comboBox.DataSource = AllRoles;
                    break;
                case ItemType.AllSchools:
                    text = "name";
                    value = "id";
                    comboBox.DataSource = AllSchools;
                    break;
                case ItemType.AllSchoolTypes:
                    text = "name";
                    value = "name";
                    comboBox.DataSource = AllSchoolTypes;
                    break;
                case ItemType.AllPortals:
                    text = "portalname";
                    value = "id";
                    comboBox.DataSource = adm.GetPortalSelections();
                    break;
            }

            comboBox.DataTextField = text;
            comboBox.DataValueField = value;
            comboBox.DataBind();

            if (hasAll)
            {
                comboBox.Items.Add(new RadComboBoxItem("All", "All"));
            }
        }
        #endregion

        private void InsertCache(ItemType type)
        {
            switch (type)
            {
                case ItemType.AllPermissions:
                    Cache.Insert(ItemType.AllPermissions.ToString(), AllPermissions);
                    break;
                case ItemType.AllRoles:
                    Cache.Insert(ItemType.AllRoles.ToString(), AllRoles);
                    break;
                case ItemType.AllUsers:
                    //Cache.Insert(ItemType.AllUsers.ToString(), AllUsers);
                    break;
                case ItemType.AllSchools:
                    Cache.Insert(ItemType.AllSchools.ToString(), AllSchools);
                    break;
                case ItemType.AllSchoolTypes:
                    Cache.Insert(ItemType.AllSchoolTypes.ToString(), AllSchoolTypes);
                    break;
                case ItemType.AllPricingModules:
                    Cache.Insert(ItemType.AllPricingModules.ToString(), AllPricingModules);
                    break;
                case ItemType.AllGrades:
                    Cache.Insert(ItemType.AllGrades.ToString(), AllGrades);
                    break;
                case ItemType.AllSubjects:
                    Cache.Insert(ItemType.AllSubjects.ToString(), AllSubjects);
                    break;
                case ItemType.AllPeriods:
                    Cache.Insert(ItemType.AllPeriods.ToString(), AllPeriods);
                    break;
            }
        }

        private bool HasPasswordRequirements(RadTextBox txtBox, bool hasConfirm, RadTextBox txtBoxConfirm = null)
        {
            if (hdnPasswordConfReq.Value == "Yes")
                return true;
            int countAlphaNum = 0;

            for (int i = 0; i < txtBox.Text.Length; i++)
            {
                if (!char.IsLetterOrDigit(txtBox.Text, i)) countAlphaNum++;
            }

            if ((txtBox.Text.Length < Membership.MinRequiredPasswordLength) || (countAlphaNum < Membership.MinRequiredNonAlphanumericCharacters))
            {
                return false;
            }
            else
            {
                if (hasConfirm)
                {
                    if (txtBoxConfirm.Text != txtBox.Text)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private void ResetPasswords()
        {
            bool isPasswordVerified = false;

            switch (currMode)
            {
                case TabTypes.UserResetPassword:
                    isPasswordVerified = HasPasswordRequirements(txtResetNew, true, txtResetConfirm);
                    break;
            }

            if (!isPasswordVerified)
            {
                switch (currMode)
                {
                    case TabTypes.UserResetPassword:
                        lblSpecial.Text = "Password does not meet minimum requirements or passwords do not match. Please try again.";
                        return;
                }
            }

            List<ThinkgateUser> lstUsers = new List<ThinkgateUser>();

            switch (currMode)
            {
                case TabTypes.UserResetPassword:
                    lstUsers.Add(ThinkgateUser.GetThinkgateUserByID(new Guid(SearchSpecial.Attributes[SearchSpecial.ID.ToString()])));
                    break;
            }

            try
            {
                switch (currMode)
                {
                    case TabTypes.UserResetPassword:
                        adm.UpdatePasswordsForUsers(lstUsers, txtResetNew.Text);
                        txtResetNew.Text = string.Empty;
                        txtResetConfirm.Text = string.Empty;
                        lblSpecial.Text = lblResetUser.Text + " password reset successfully!";
                        break;
                }

                lstUsers = null;
            }
            catch (Exception ex)
            {
                lstUsers = null;
                switch (currMode)
                {
                    case TabTypes.UserResetPassword:
                        lblSpecial.Text = "Error Updating Password. Try again.";
                        break;
                }
                return;
            }
        }

        #region Grid Manipulation
        protected void gridEnable_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = (GridDataItem)e.Item;

                switch (currMode)
                {
                    case TabTypes.PricingModuleStatus:
                        ((CheckBox)dataItem["Active"].Controls[0]).Enabled = true;
                        break;
                    default:
                        ((CheckBox)dataItem["Enabled"].Controls[0]).Enabled = true;
                        break;
                }
            }
        }

        protected void gridText_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                ((RadTextBox)e.Item.FindControl("txtSubject")).Text = ((System.Data.DataRowView)(e.Item.DataItem)).Row["Subject"].ToString();
                ((RadTextBox)e.Item.FindControl("txtAbbreviation")).Text = ((System.Data.DataRowView)(e.Item.DataItem)).Row["Abbreviation"].ToString();
            }
        }

        protected void gridEnable_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            if (sender is RadGrid && currMode != TabTypes.None)
            {
                switch (currMode)
                {
                    case TabTypes.PermissionRoles:
                    case TabTypes.PermissionUsers:
                    case TabTypes.RolePermissions:
                    case TabTypes.UserRoles:
                    case TabTypes.UserPermissions:
                    case TabTypes.UserSchools:
                        if (e.Column.UniqueName.ToLower() == "idlabel")
                        {
                            e.Column.Display = false;
                        }
                        break;
                    case TabTypes.PricingModuleStatus:
                        if (e.Column.UniqueName.ToLower() != "active" && e.Column.UniqueName.ToLower() != "name")
                        {
                            e.Column.Display = false;
                        }
                        break;
                }
            }
        }

        protected void gridSearch_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            if (sender is RadGrid && currMode != TabTypes.None)
            {
                switch (currMode)
                {
                    case TabTypes.UserEdit:
                    case TabTypes.UserRoles:
                    case TabTypes.UserPermissions:
                    case TabTypes.UserSchools:
                    case TabTypes.UserResetPassword:
                        if ((e.Column.UniqueName.ToLower() != "username") && (e.Column.UniqueName.ToLower() != "firstname") && e.Column.UniqueName.ToLower() != "lastname")
                        {
                            e.Column.Visible = false;
                        }
                        break;
                }
            }
        }
        #endregion

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            switch (currMode)
            {
                case TabTypes.RoleHierarchy:
                    adm.UpdateRoleHeirarchy(GetListBoxOrder(lbxSort).Cast<Guid>().ToList());
                    lblSort.Text = "Role Hierarchy Updated!";
                    LoadRoles();
                    break;
                default:
                    break;
            }
        }
    }
}