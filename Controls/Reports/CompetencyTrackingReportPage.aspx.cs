using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using CMS.GlobalHelper;
using Standpoint.Core.ExtensionMethods;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.CompetencyTracking;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Domain.Classes;
using Thinkgate.Classes.Search;
using System.Globalization;
using Thinkgate.Controls.Documents;
using System.Text.RegularExpressions;

namespace Thinkgate.Controls.Reports
{
    public partial class CompetencyTrackingReportPage : BasePage
    {
        public string PageGuid;
        public int FormID;
        private readonly List<string> _excelIgnoredColumns = new List<string>();
        private static string _loggedOnUserRoleName = string.Empty;
        private static ThinkgateUser _loggedOnUser = null;
        private EnvironmentParametersViewModel _enviromentParameter;
        private string _viewBySelectedValue;
        private string _currentViewByValue = string.Empty;
        //private int _currentViewByValueCount = 0;
        private static readonly string DefaultStandardSets = DistrictParms.LoadDistrictParms().AddRemove_Competency_DefaultStandardSetList;

        public DataTable GridDataTable;
        public int DataTableCount;
        private int _distinctRowsCount;
        private int _distinctObjectivesCount;

        public List<int> _filterStandardIDs = new List<int>();
        private List<int> _filterCurriculumIDs = new List<int>();

        private const string CompetencyTrackingReport = "Competency Tracking Report";
        private const string CompetencyTrackingReport_NoSpaces = "CompetencyTrackingReport";
        private SessionObject _sessionObject;
        private int demographicID = 0;
        private int groupID = 0;
        public int selectedlist = 0;
        private string totalStudents = "";


        private string SelectedObject;

        protected new void Page_Init(object sender, EventArgs e)
        {
            cmbStandardSet.OnClientChange = "GetSelectedLevelText()";

            Master.Search += SearchHandler;
            base.Page_Init(sender, e);
            _currentViewByValue = string.Empty;
            _enviromentParameter = new EnvironmentParametersFactory(AppSettings.ConnectionStringName).GetEnvironmentParameters();
            _loggedOnUserRoleName = SessionObject.LoggedInUser.Roles[0].RoleName;
            _loggedOnUser = SessionObject.LoggedInUser;

            _excelIgnoredColumns.Add("EncryptedID");
            _excelIgnoredColumns.Add("WorksheetId");
            _excelIgnoredColumns.Add("StandardId");
            _excelIgnoredColumns.Add("Included");
            _excelIgnoredColumns.Add("Total");
            _excelIgnoredColumns.Add("Excluded");
            _excelIgnoredColumns.Add("StandardLevel");
            _excelIgnoredColumns.Add("ScoreColumnA");
            _excelIgnoredColumns.Add("ScoreColumnB");
            _excelIgnoredColumns.Add("ScoreColumnC");
            _excelIgnoredColumns.Add("ScoreColumnD");
            _excelIgnoredColumns.Add("ScoreColumnE");
            //_excelIgnoredColumns.Add("CountA");
            //_excelIgnoredColumns.Add("CountB");
            //_excelIgnoredColumns.Add("CountC");
            //_excelIgnoredColumns.Add("CountD");
            //_excelIgnoredColumns.Add("CountE");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateViewByDropdown();
            PopulateDemographicDropdown();

            PopulateStandardListDropdown();

            PopulateSchoolDropdown();

            PopulateGroupsDropdown();
            PopulateSchool();


            //if (SessionObject.LoggedInUser.UserFullName != null)
            //{
            //    PopulateListSelectionDropdown(SessionObject.LoggedInUser.UserFullName);
            //}
            //else
            //{
            //    PopulateListSelectionDropdown(null);
            //}

            if (!IsPostBack)
            {
                LoadCriteria();
            }

            this.cmbStandardLevel.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cmbStandardLevel_ItemsRequested);

            this.cmbCompentencyList.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cmbListSelection_ItemsRequested);
            this.cmbCompentencyWorksheet.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cmbListSelection_ItemsRequested);



            //PopulateStandardLevelDropdown();

        }

        protected void cmbStandardLevel_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            int WorksheetId = 0;
            string StandardSet = e.Context["StandardSet"].ToString();
            if (e.Context["StandardList"] != null)
            {
                selectedlist = Convert.ToInt32(e.Context["StandardList"].ToString());
            }
            else
            {
                selectedlist = 0;
            }

            Base.DataAccess.dtGeneric_Int _standardids = new Base.DataAccess.dtGeneric_Int();
            if (selectedlist == 9)//for competency list option only
            {
                var CompetencyId = 0;
                Base.DataAccess.dtGeneric_Int _standardid = new Base.DataAccess.dtGeneric_Int();
                if (e.Context["ListSelection"] != null)
                {
                    CompetencyId = Convert.ToInt32(e.Context["ListSelection"].ToString());
                }
                else
                {
                    CompetencyId = 0;
                }
                _standardid.Add(Convert.ToInt32(CompetencyId));
                DataTable dsstandard = CompetencyWorkSheet.GetCurrStabdardsById_Kentico(_standardid, true);

                foreach (DataRow dr in dsstandard.Rows)
                {
                    _standardids.Add(Convert.ToInt32(dr[0]));
                }

            }
            else
            {
                if (e.Context["ListSelection"] != null && e.Context["ListSelection"] != string.Empty)
                {
                    WorksheetId = Convert.ToInt32(e.Context["ListSelection"].ToString());
                }
                else
                {
                    WorksheetId = 0;
                }
            }

            DataTable tmpdatatable = CriteriaHelper.GetStandardLevelbyStandardList(_standardids, StandardSet, WorksheetId);
            cmbStandardLevel.DataSource = tmpdatatable;
            cmbStandardLevel.DataBind();
        }

        protected void cmbListSelection_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var objStandardList = Convert.ToInt32(e.Context["StandardList"]);
            int objTeacherID = Convert.ToInt32(e.Context["TeacherID"]);
            if (objStandardList == 9)
            {
                cmbCompentencyList.DataSource = GetCompetencyListTable();
                cmbCompentencyList.DataTextField = "FriendlyName";
                cmbCompentencyList.DataValueField = "DocumentId";
                cmbCompentencyList.DataBind();
            }
            else if (objStandardList == 8)
            {
                cmbCompentencyWorksheet.DataSource = PopulateListSelectionDropdownByTeacher(objTeacherID);


                //  cmbListSelection.DataSource = GetCompetencyListTable();
                cmbCompentencyWorksheet.DataTextField = "Name";
                cmbCompentencyWorksheet.DataValueField = "ID";
                cmbCompentencyWorksheet.DataBind();
            }




        }

        private void PopulateSchoolDropdown()
        {

            var schoolDataTable = new DataTable();
            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID");

            var rolePortal = (RolePortal)_sessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);

            if (rolePortal.ToString().ToLower() == Convert.ToString(RolePortal.Teacher).ToLower())
            {
                var userID = SessionObject.LoggedInUser.Page;
                var schoolList = Thinkgate.Base.Classes.Class.GetClassesForTeacher(userID, 0);
                schoolList = schoolList.GroupBy(x => x.SchoolID).Select(y => y.First()).ToList();
                foreach (var s in schoolList)
                {

                    schoolDataTable.Rows.Add(s.School.Name, s.SchoolID);
                }
            }
            else
            {
                var schoolList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser).ToList();
                foreach (var s in schoolList)
                {

                    schoolDataTable.Rows.Add(s.Name, s.ID);
                }
            }

            //var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            //schoolDataTable.Columns.Add("Name");
            //schoolDataTable.Columns.Add("ID");

            //foreach (var s in schoolsForLooggedInUser)
            //{
            //    schoolDataTable.Rows.Add(s.Name, s.ID);
            //}







            if (schoolDataTable.Rows.Count > 0)
            {
                cmbSchool.DataTextField = "Name";
                cmbSchool.DataValueField = "Id";
                cmbSchool.DataSource = schoolDataTable;
            }
        }



        //private void PopulateListSelectionDropdown(string teacherName)
        //{
        //    cmbListSelection.DataTextField = "Name";
        //    cmbListSelection.DataValueField = "ID";

        //    cmbListSelection.DataSource = (getListOfWorksheets(teacherName));
        //}

        private void LoadCriteria()
        {
            List<KeyValuePair> standardSetList = CriteriaHelper.GetStandardSetList().OrderBy(x => x.Value).ToList();
            // standardSetList.Insert(0, new KeyValuePair("0", "Select Set"));         

            cmbStandardSet.StandardSetDataSource = standardSetList;

        }

        private DataTable getListOfWorksheets(string teacherName)
        {
            DataTable cvteReportByStudent = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_GetWorksheetList",
                    Connection = conn
                };
                cmd.Parameters.Add(teacherName != null
                    ? new SqlParameter { ParameterName = "TeacherName", Value = teacherName }
                    : new SqlParameter { ParameterName = "TeacherName", Value = DBNull.Value });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(cvteReportByStudent);
            }
            return cvteReportByStudent;
        }

        private void PopulateStandardLevelDropdown()
        {
            var theList = new List<KeyValuePair<string, string>>();
            theList.Add(new KeyValuePair<string, string>("Strand", "Strand"));
            theList.Add(new KeyValuePair<string, string>("Topic", "Topic"));
            theList.Add(new KeyValuePair<string, string>("Standard", "Standard"));
            theList.Add(new KeyValuePair<string, string>("Objective", "Objective"));

            cmbStandardLevel.DataTextField = "Key";
            cmbStandardLevel.DataValueField = "Value";
            cmbStandardLevel.DataSource = theList;
        }

        private void PopulateGroupsDropdown()
        {
            var _roleportalID = (RolePortal)(SessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection));
            IEnumerable<GroupViewModel> grpNames;
            if (_roleportalID == RolePortal.District || _roleportalID == RolePortal.Teacher)
            {
                //grpNames = (new Grouping(_enviromentParameter).GetGroupsForPortal(_roleportalID)).OrderBy(r => r.DisplayName);
                Thinkgate.Services.Contracts.Groups.GroupsProxy _groupProxy = new Thinkgate.Services.Contracts.Groups.GroupsProxy();
                grpNames = _groupProxy.GetGroupsForUser(SessionObject.LoggedInUser.Page,
                    DistrictParms.LoadDistrictParms().ClientID).Select(x => new GroupViewModel() { TargetVisibilityId = x.ID, DisplayName = x.Name });
            }
            else if (_roleportalID == RolePortal.School)
            {
                grpNames = (new Grouping(_enviromentParameter).GetGroupsForSchoolPortal(_roleportalID, SessionObject.LoggedInUser.School)).OrderBy(r => r.DisplayName);
            }
            else grpNames = null;

            if (grpNames != null && grpNames.ToList().Count > 0)
            {
                cmbGroup.DataSource = grpNames;
                cmbGroup.DataTextField = "DisplayName";
                cmbGroup.DataValueField = "TargetVisibilityId";
            }
            else
            {
                cmbGroup.EmptyMessage = "No groups available";
            }
        }



        private void PopulateViewByDropdown()
        {
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            DataTable competencyTypeList = oCTR.GetCompetencyTypeList();


            if (_sessionObject == null)
            {
                _sessionObject = (SessionObject)Session["SessionObject"];
            }

            var rolePortal = (RolePortal)_sessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);
            /* Check rolePortal */


            switch (rolePortal)
            {

                case RolePortal.District:
                    {
                        for (int i = competencyTypeList.Rows.Count - 1; i >= 0; i--)
                            if (competencyTypeList.Rows[i]["TypeValue"].ToString() == "Group")
                            {
                                competencyTypeList.Rows.RemoveAt(i);
                            }
                        cmbGroup.Visible = false;
                        break;
                    }

                case RolePortal.School:
                    {
                        for (int i = competencyTypeList.Rows.Count - 1; i >= 0; i--)
                            if (competencyTypeList.Rows[i]["TypeValue"].ToString() == "District" || competencyTypeList.Rows[i]["TypeValue"].ToString() == "Group")
                            {
                                competencyTypeList.Rows.RemoveAt(i);
                            }
                        cmbGroup.Visible = false;
                        break;
                    }
                case RolePortal.Teacher:
                    {

                        for (int i = competencyTypeList.Rows.Count - 1; i >= 0; i--)
                            if (competencyTypeList.Rows[i]["TypeValue"].ToString() == "District" || competencyTypeList.Rows[i]["TypeValue"].ToString() == "School")
                            {
                                competencyTypeList.Rows.RemoveAt(i);
                            }

                        break;
                    }
            }

            cmbViewBy.DataSource = competencyTypeList;
            cmbViewBy.DataTextField = "TypeValue";
            cmbViewBy.DataValueField = "EnumValue";

        }


        private void PopulateDemographicDropdown()
        {
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            cmbDemos.DataSource = oCTR.GetDemographicList();
            cmbDemos.DataTextField = "Label";
            cmbDemos.DataValueField = "ID";
        }

        private void PopulateSchool()
        {

            var schoolDataTable = new DataTable();
            var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID");

            foreach (var s in schoolsForLooggedInUser)
            {
                schoolDataTable.Rows.Add(s.Name, s.ID);
            }

            if (schoolDataTable.Rows.Count > 0)
            {
                cmbTeacher.DataTextField = "Name";
                cmbTeacher.DataValueField = "Id";
                cmbTeacher.DataSource = schoolDataTable;
            }
        }


        private static bool IsSchoolUser()
        {
            bool isSchoolUser =
                _loggedOnUserRoleName.Equals("School Administrator", StringComparison.InvariantCultureIgnoreCase)
                || _loggedOnUserRoleName.Equals("School Support", StringComparison.InvariantCultureIgnoreCase);
            return isSchoolUser;
        }

        private static bool IsDistrictUser()
        {
            bool isDistrictUser =
                _loggedOnUserRoleName.Equals("District Administrator", StringComparison.InvariantCultureIgnoreCase)
                || _loggedOnUserRoleName.Equals("District Support", StringComparison.InvariantCultureIgnoreCase);
            return isDistrictUser;
        }

        private void PopulateStandardListDropdown()
        {
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            cmbStandardList.DataSource = oCTR.GetStandardList();
            cmbStandardList.DataTextField = "TypeValue";
            cmbStandardList.DataValueField = "EnumValue";
        }

        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            //radGridResults.MasterTableView.Columns.Clear();
            BindDataToGrid();
        }

        private DataTable GetClassesForTeacher(int schoolID, string teacherName)
        {
            var classCourseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
            List<Int32> schoolIDs = new List<int>();
            schoolIDs.Add(schoolID);

            return Base.Classes.Class.SearchClasses(classCourseList, null, schoolIDs, "", teacherName);
        }

        //public DataTable GetClassesForTeacherOnly()
        //{
        //    DataTable classDataTable = new DataTable();

        //    if (_sessionObject == null)
        //    {
        //        _sessionObject = (SessionObject)Session["SessionObject"];
        //    }
        // var classList=   SessionObject.LoggedInUser.Classes;
        // classDataTable = ThinkgatePermissionsCollection.ToDataTable(classList);
        // return classDataTable;
        //}

        //protected void RadGridResults_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        //{
        //    BindDataToGrid();
        //}

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }

        protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var criteriaController = Master.CurrentCriteria();
            _viewBySelectedValue = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ViewBy").Select(x => x.Text).FirstOrDefault();
            string viewBySelectedValue = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ViewBy").Select(x => x.Value).FirstOrDefault();
            string demograID = criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.ACDropDownList.ValueObject>("Demographic").Select(x => x.Value).FirstOrDefault();
            string grp = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Group").Select(x => x.Value).FirstOrDefault();
            //demographicID = Convert.ToInt32(viewBySelectedValue);

            if (_viewBySelectedValue == "Demographics")
            {
                demographicID = Convert.ToInt32(SelectedObject);
            }
            else
            {
                if (demograID != null && demograID != "")
                    demographicID = Convert.ToInt32(demograID);
            }
            if (_viewBySelectedValue == "Group")
            {
                groupID = Convert.ToInt32(SelectedObject);
            }
            else if (_viewBySelectedValue == "Demographics")
            {
                if (grp != null && grp != "")
                    groupID = Convert.ToInt32(grp);
            }

            string standardLevel = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StandardLevel").Select(x => x.Text).FirstOrDefault();
            TGDetail selectedType;
            Enum.TryParse(_viewBySelectedValue, true, out  selectedType);

            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem item = e.Item as GridHeaderItem;
                if (_viewBySelectedValue != null)
                {
                    if (DataTableCount != 0)
                    {
                        if (DataTableCount > 0)
                        {
                            item[_viewBySelectedValue + "Name"].Text = _viewBySelectedValue + " (" + _distinctRowsCount + ")";

                            item["StandardName"].Text = IEnumerableExtensions.IsNotNullOrEmpty(standardLevel)
                                ? standardLevel + " (" + _distinctObjectivesCount + ")"
                                : "Competencies (" + _distinctObjectivesCount + ")";
                        }
                        else
                        {
                            item[_viewBySelectedValue + "Name"].Text = _viewBySelectedValue;

                            item["StandardName"].Text = IEnumerableExtensions.IsNotNullOrEmpty(standardLevel)
                                ? standardLevel
                                : "Competencies";

                        }


                    }
                }
            }

            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;

                // if the viewby cell value is the same as the previous then put in an empty string
                if (_currentViewByValue.Equals(item[_viewBySelectedValue + "Name"].Text + (selectedType != TGDetail.Student ? "<br/><b>" + "(" + totalStudents + " Students<b>)" : "")))
                {
                    item[_viewBySelectedValue + "Name"].Text = "";
                }
                else
                {
                    item[_viewBySelectedValue + "Name"].Text = item[_viewBySelectedValue + "Name"].Text + (selectedType != TGDetail.Student ? "<br/><b>" + "(" + totalStudents + " Students<b>)" : "");
                    _currentViewByValue = item[_viewBySelectedValue + "Name"].Text;

                }

                item[_viewBySelectedValue + "Name"].VerticalAlign = VerticalAlign.Middle;
                item[_viewBySelectedValue + "Name"].BorderWidth = 0;
                item[_viewBySelectedValue + "Name"].BackColor = System.Drawing.Color.Transparent;
                if (e.Item.ItemIndex == DataTableCount - 1)
                {
                    item[_viewBySelectedValue + "Name"].BorderWidth = 1;
                }


                if (selectedType != null)
                {

                    // var Total = int.Parse(item["CountA"].Text) + int.Parse(item["CountB"].Text) + int.Parse(item["CountC"].Text) + int.Parse(item["CountD"].Text) + int.Parse(item["CountE"].Text);

                    if (int.Parse(item["CountA"].Text) >= 1)
                    {
                        var thePercentage = CalcPercentage(totalStudents, item["CountA"].Text);
                        //var thePercentage = CalcPercentage(item["Total"].Text, item["Included"].Text);
                        if (!String.IsNullOrEmpty(standardLevel))
                        {

                            thePercentage = CalcPercentage(item["TotalCompetencies"].Text, item["CountA"].Text);

                            if (selectedType == TGDetail.Student)
                            {
                                item["DateA"].Text = ("<a href='#' onclick=StanderdCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 1 + "','" + viewBySelectedValue + "','" + item["StudentID"].Text + "')> <b>" + thePercentage + "%</b></a> (" + item["CountA"].Text + ")");


                            }
                            else
                            {
                                item["DateA"].Text = "<b>" + item["CountA"].Text + "%</b>";
                            }
                        }
                        else
                        {
                            if (selectedType == TGDetail.Student)
                                item["DateA"].Text += " (" + item["CountA"].Text + ")";
                            else
                                item["DateA"].Text = (" <a href='#'  onclick=StudentsCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 1 + "','" + viewBySelectedValue + "','" + demographicID + "','" + groupID + "')> <b>" + thePercentage + "%</b> </a> (" + item["CountA"].Text + ")");
                        }
                    }

                    if (int.Parse(item["CountB"].Text) >= 1)
                    {
                        var thePercentage = CalcPercentage(totalStudents, item["CountB"].Text);
                        //var thePercentage = CalcPercentage(item["Total"].Text, item["Included"].Text);
                        if (!String.IsNullOrEmpty(standardLevel))
                        {

                            thePercentage = CalcPercentage(item["TotalCompetencies"].Text, item["CountB"].Text);
                            if (selectedType == TGDetail.Student)
                            {
                                item["DateB"].Text = ("<a href='#' onclick=StanderdCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 2 + "','" + viewBySelectedValue + "','" + item["StudentID"].Text + "')> <b>" + thePercentage + "%</b></a> (" + item["CountB"].Text + ")");


                            }
                            else
                            {
                                item["DateB"].Text = "<b>" + item["CountB"].Text + "%</b>";
                            }
                        }
                        else
                        {
                            if (selectedType == TGDetail.Student)
                                item["DateB"].Text += " (" + item["CountB"].Text + ")";
                            else
                                item["DateB"].Text = ("<a href='#'  onclick=StudentsCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 2 + "','" + viewBySelectedValue + "','" + demographicID + "','" + groupID + "')> <b>" + thePercentage + "%</b></a> (" + item["CountB"].Text + ")");
                        }
                    }

                    if (int.Parse(item["CountC"].Text) >= 1)
                    {
                        var thePercentage = CalcPercentage(totalStudents, item["CountC"].Text);
                        //var thePercentage = CalcPercentage(item["Total"].Text, item["Included"].Text);
                        if (!String.IsNullOrEmpty(standardLevel))
                        {
                            thePercentage = CalcPercentage(item["TotalCompetencies"].Text, item["CountC"].Text);
                            if (selectedType == TGDetail.Student)
                            {
                                item["DateC"].Text = ("<a href='#' onclick=StanderdCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 3 + "','" + viewBySelectedValue + "','" + item["StudentID"].Text + "')> <b>" + thePercentage + "%</b></a> (" + item["CountC"].Text + ")");


                            }
                            else
                            {
                                item["DateC"].Text = "<b>" + item["CountC"].Text + "%</b>";
                            }
                        }
                        else
                        {
                            if (selectedType == TGDetail.Student)
                                item["DateC"].Text += " (" + item["CountC"].Text + ")";
                            else
                                item["DateC"].Text = ("<a href='#' onclick=StudentsCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 3 + "','" + viewBySelectedValue + "','" + demographicID + "','" + groupID + "')> <b>" + thePercentage + "%</b></a> (" + item["CountC"].Text + ")");
                        }
                    }

                    if (int.Parse(item["CountD"].Text) >= 1)
                    {
                        var thePercentage = CalcPercentage(totalStudents, item["CountD"].Text);
                        //var thePercentage = CalcPercentage(item["Total"].Text, item["Included"].Text);
                        if (!String.IsNullOrEmpty(standardLevel))
                        {
                            thePercentage = CalcPercentage(item["TotalCompetencies"].Text, item["CountD"].Text);
                            if (selectedType == TGDetail.Student)
                            {
                                item["DateD"].Text = ("<a href='#' onclick=StanderdCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 4 + "','" + viewBySelectedValue + "','" + item["StudentID"].Text + "')> <b>" + thePercentage + "%</b></a> (" + item["CountD"].Text + ")");
                            }
                            else
                            {
                                item["DateD"].Text = "<b>" + item["CountD"].Text + "%</b>";
                            }
                        }
                        else
                        {
                            if (selectedType == TGDetail.Student)
                                item["DateD"].Text += " (" + item["CountD"].Text + ")";
                            else
                                item["DateD"].Text = ("<a href='#' onclick=StudentsCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 4 + "','" + viewBySelectedValue + "','" + demographicID + "','" + groupID + "')> <b>" + thePercentage + "%</b></a> (" + item["CountD"].Text + ")");
                        }
                    }

                    if (int.Parse(item["CountE"].Text) >= 1)
                    {
                        var thePercentage = CalcPercentage(totalStudents, item["CountE"].Text);
                        //var thePercentage = CalcPercentage(item["Total"].Text, item["Included"].Text);
                        if (!String.IsNullOrEmpty(standardLevel))
                        {
                            thePercentage = CalcPercentage(item["TotalCompetencies"].Text, item["CountE"].Text);
                            if (selectedType == TGDetail.Student)
                            {
                                item["DateE"].Text = ("<a href='#' onclick=StanderdCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 5 + "','" + viewBySelectedValue + "','" + item["StudentID"].Text + "')> <b>" + thePercentage + "%</b></a> (" + item["CountE"].Text + ")");
                            }
                            else
                            {
                                item["DateE"].Text = "<b>" + item["CountE"].Text + "%</b>";
                            }
                        }
                        else
                        {
                            if (selectedType == TGDetail.Student)
                                item["DateE"].Text += " (" + item["CountE"].Text + ")";
                            else
                                item["DateE"].Text = ("<a href='#' onclick=StudentsCTLView('" + SelectedObject + "','" + item["StandardId"].Text + "','" + item["WorksheetId"].Text + "','" + 5 + "','" + viewBySelectedValue + "','" + demographicID + "','" + groupID + "')> <b>" + thePercentage + "%</b></a> (" + item["CountE"].Text + ")");
                        }
                    }
                }
                else
                {
                    item["DateA"].Text += " (" + item["CountA"].Text + ")";
                    item["DateB"].Text += " (" + item["CountB"].Text + ")";
                    item["DateC"].Text += " (" + item["CountC"].Text + ")";
                    item["DateD"].Text += " (" + item["CountD"].Text + ")";
                    item["DateE"].Text += " (" + item["CountE"].Text + ")";
                }

                //Set cell Background colors
                if (item["DateA"].Text != "&nbsp;" && selectedType == TGDetail.Student)
                {
                    item["DateA"].BackColor = System.Drawing.Color.LightGray;
                }
                if (item["DateB"].Text != "&nbsp;" && selectedType == TGDetail.Student)
                {
                    item["DateB"].BackColor = System.Drawing.Color.LightGray;
                }
                if (item["DateC"].Text != "&nbsp;" && selectedType == TGDetail.Student)
                {
                    item["DateC"].BackColor = System.Drawing.Color.LightGray;
                }
                if (item["DateD"].Text != "&nbsp;" && selectedType == TGDetail.Student)
                {
                    item["DateD"].BackColor = System.Drawing.Color.LightGray;
                }
                if (item["DateE"].Text != "&nbsp;" && selectedType == TGDetail.Student)
                {
                    item["DateE"].BackColor = System.Drawing.Color.LightGray;
                }
            }
        }

        public static String GetPercentage(Int32 value, Int32 total, Int32 places)
        {
            Decimal percent = 0;
            String retval = string.Empty;
            String strplaces = new String('0', places);

            if (value == 0 || total == 0)
            {
                percent = 0;
            }

            else
            {
                percent = Decimal.Divide(value, total) * 100;

                if (places > 0)
                {
                    strplaces = "." + strplaces;
                }
            }

            retval = percent.ToString("#" + strplaces);

            return retval;
        }

        private static string CalcPercentage(string total, string included)
        {
            string percentage = GetPercentage(int.Parse(included), int.Parse(total), 0);
            return percentage;
        }

        private SelectedCriteria GetCriteriaControlValues()
        {
            var criteriaController = Master.CurrentCriteria();

            SelectedCriteria selectedCriteria = new SelectedCriteria();

            //ViewBy
            selectedCriteria.SelectedViewBy = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ViewBy").Select(x => x.Text).FirstOrDefault();
            //School
            selectedCriteria.SelectedSchoolId = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("School").Select(x => x.Value).FirstOrDefault());
            selectedCriteria.SelectedSchoolName = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("School").Select(x => x.Value).FirstOrDefault();
            //Teacher
            selectedCriteria.SelectedTeacher = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("TeacherName").Select(x => x.Value).FirstOrDefault();
            //Class
            selectedCriteria.SelectedClass = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Class").Select(x => x.Value).FirstOrDefault();
            //Student
            selectedCriteria.SelectedStudent = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Student").Select(x => x.Value).FirstOrDefault();
            //StandardLevel
            selectedCriteria.StandardLevel = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StandardLevel").Select(x => x.Value).FirstOrDefault();
            //Demographics
            selectedCriteria.SelectedDemographics = criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.ACDropDownList.ValueObject>("Demographic").Select(x => x.Value).FirstOrDefault();
            //Group;
            selectedCriteria.SelectedGroup = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Group").Select(x => x.Value).FirstOrDefault();
            //Worksheet List
            string selectedListId = "0";
            if (criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ListSelection").Select(x => x.Value).FirstOrDefault() != null)
            {
                selectedListId = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ListSelection").Select(x => x.Value).FirstOrDefault();
            }
            else
            {
                selectedListId = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("WorksheetSelection").Select(x => x.Value).FirstOrDefault();
            }
            selectedCriteria.SelectedWorksheet = selectedListId;
            //StandardList
            selectedCriteria.StandardList = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StandardList").Select(x => x.Value).FirstOrDefault();

            selectedCriteria.StandardSet = criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard.ValueObject>("StandardSet").Select(x => x.StandardSet).FirstOrDefault();

            selectedCriteria.StandardId = criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard.ValueObject>("StandardSet").Select(x => x.StandardId).FirstOrDefault();
            //if (criteriaController.CriteriaNodes[2].Values[0].Value != null && criteriaController.CriteriaNodes[2].Values[0].Value.ContainsKey("Grades"))
            //{
            selectedCriteria.Grade = criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard.ValueObject>("StandardSet").Select(x => x.Grades).FirstOrDefault();//criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard.ValueObject>("Grades").Select(x => x.Grades).FirstOrDefault();
            //}

            //if (criteriaController.CriteriaNodes[2].Values[0].Value != null && criteriaController.CriteriaNodes[2].Values[0].Value.ContainsKey("Subjects"))
            //{
            selectedCriteria.Subject = criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard.ValueObject>("StandardSet").Select(x => x.Subjects).FirstOrDefault(); //criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard.ValueObject>("Subjects").Select(x => x.Subjects).FirstOrDefault();
            //}

            //if (criteriaController.CriteriaNodes[2].Values[0].Value != null && criteriaController.CriteriaNodes[2].Values[0].Value.ContainsKey("Courses"))
            //{
            selectedCriteria.Course = criteriaController.ParseCriteria<E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard.ValueObject>("StandardSet").Select(x => x.Courses).FirstOrDefault();
            //}


            //TODO UNCOMMENT AND VERIFY BELOW CODE ONCE STANDARD LIST IS WORKING

            //selectedCriteria.StandardList = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ListSelection").Select(x => x.Value).FirstOrDefault();
            selectedlist = Convert.ToInt32(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StandardList").Select(x => x.Value).FirstOrDefault());
            if (selectedlist == 9)//for competency list option only
            {
                Base.DataAccess.dtGeneric_Int _standardids = new Base.DataAccess.dtGeneric_Int();
                _standardids.Add(Convert.ToInt32(selectedCriteria.SelectedWorksheet));
                DataTable dsstandard = CompetencyWorkSheet.GetCurrStabdardsById_Kentico(_standardids, true);
                var stdListFromCompetencyList = "";
                foreach (DataRow dr in dsstandard.Rows)
                {
                    stdListFromCompetencyList += "_" + dr[0].ToString();
                }

                if (!string.IsNullOrEmpty(stdListFromCompetencyList))
                {
                    selectedCriteria.StandardId = stdListFromCompetencyList.Substring(1);
                }
                selectedCriteria.SelectedWorksheet = null;
            }

            /* Created Date Range */
            foreach (var val in criteriaController.ParseCriteria<DateRange.ValueObject>("DateRange"))
            {
                if (val.Type == "Start")
                {
                    selectedCriteria.StartDate = val.Date;
                }
                else
                {
                    selectedCriteria.EndDate = val.Date;
                }
            }

            return selectedCriteria;
        }

        public void BindDataToGrid()
        {
            radGridResults.MasterTableView.Columns.Clear();
            radGridResults.MasterTableView.DataSource = new string[0];

            radGridResults.Visible = true;
            lblInitialText.Visible = false;

            SelectedCriteria selectedCriteria = GetCriteriaControlValues();

            //ViewBy
            string selectedViewBy = selectedCriteria.SelectedViewBy;
            //School
            int selectedSchoolId = selectedCriteria.SelectedSchoolId;

            //Teacher
            //The code below forces passing the current teacher to the backend procs, this way a teacher can ONLY see their data.
            string selectedTeacher = string.Empty;
            if (_loggedOnUserRoleName != null
                && _loggedOnUserRoleName.Equals("Teacher", StringComparison.InvariantCultureIgnoreCase)
                && (_loggedOnUser != null && !string.IsNullOrEmpty(_loggedOnUser.Page.ToString())))
            {
                selectedTeacher = _loggedOnUser.Page.ToString(); // force current user
            }
            else
            {
                selectedTeacher = selectedCriteria.SelectedTeacher;
            }

            //Class
            string selectedClass = selectedCriteria.SelectedClass;
            //Student
            string selectedStudent = selectedCriteria.SelectedStudent;
            //StandardLevel
            string standardLevel = selectedCriteria.StandardLevel;
            //Demographics
            string selectedDemographics = selectedCriteria.SelectedDemographics;
            //Group;
            string selectedGroup = selectedCriteria.SelectedGroup;
            //Worksheet List
            string selectedWorksheet = selectedCriteria.SelectedWorksheet;

            /* Created Date Range */
            string startDate = selectedCriteria.StartDate;
            string endDate = selectedCriteria.EndDate;

            //GET THE DATA HERE
            GridDataTable = new DataTable();

            string standardSet = string.Empty; //GetStandardSetValueFromParmsTable();


            TGDetail selectedType;
            Enum.TryParse(selectedViewBy, true, out  selectedType);

            if (selectedType != null)
            {
                Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
                //selectedWorksheet, standardSet, standardLevel, selectedSchoolId.ToString(), selectedTeacher, selectedClass, selectedDemographics, selectedGroup, selectedStudent, startDate, endDate
                oCTR.worksheetId = selectedWorksheet;
                oCTR.standardSet = selectedCriteria.StandardSet;
                oCTR.standardLevel = standardLevel;
                oCTR.schoolID = selectedSchoolId.ToString();
                oCTR.teacherId = selectedTeacher;
                oCTR.className = selectedClass;
                oCTR.demographicID = selectedDemographics;
                oCTR.groupID = selectedGroup;
                oCTR.studentId = selectedStudent;
                oCTR.startDate = startDate;
                oCTR.endDate = endDate;

                oCTR.standardId = String.IsNullOrEmpty(selectedCriteria.StandardId) ? null : selectedCriteria.StandardId.Split('_').Select(str => int.Parse(str)).ToArray();
                oCTR.grade = selectedCriteria.Grade;
                oCTR.course = selectedCriteria.Course;
                oCTR.subject = selectedCriteria.Subject;

                switch (selectedType)
                {
                    case TGDetail.Student:
                        GridDataTable = oCTR.GetCvteReportByStudent();
                        break;
                    case TGDetail.District:
                        GridDataTable = oCTR.GetCvteReportByDistrict();
                        if (GridDataTable.Rows.Count > 0)
                            SelectedObject = "0";//GridDataTable.Rows[0]["DistrictID"].ToString();
                        break;
                    case TGDetail.School:
                        GridDataTable = oCTR.GetCvteReportBySchool();
                        if (GridDataTable.Rows.Count > 0)
                            SelectedObject = selectedSchoolId.ToString();//GridDataTable.Rows[0]["SchoolID"].ToString();
                        break;
                    case TGDetail.Teacher:
                        GridDataTable = oCTR.GetCvteReportByTeacher();
                        if (GridDataTable.Rows.Count > 0)
                            SelectedObject = selectedTeacher;//GridDataTable.Rows[0]["TeacherID"].ToString();
                        break;
                    case TGDetail.Class:
                        GridDataTable = oCTR.GetCvteReportByClass();
                        if (GridDataTable.Rows.Count > 0)
                            SelectedObject = selectedClass; //GridDataTable.Rows[0]["ClassID"].ToString();
                        break;
                    case TGDetail.Demographics:
                        GridDataTable = oCTR.GetCvteReportByDemographics();
                        if (GridDataTable.Rows.Count > 0)
                        {
                            SelectedObject = selectedDemographics;//GridDataTable.Rows[0]["DemographicsID"].ToString();
                        }
                        break;
                    case TGDetail.Group:
                        GridDataTable = oCTR.GetCvteReportByGroup();
                        if (GridDataTable.Rows.Count > 0)
                            SelectedObject = selectedGroup;//GridDataTable.Rows[0]["DistrictID"].ToString();
                        break;

                }
                if (GridDataTable.Rows.Count > 0)
                {
                    GridDataTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(GridDataTable, "StandardID", "EncryptedID");
                    DataTableCount = GridDataTable.Rows.Count;
                    DataView view = new DataView(GridDataTable);
                    _distinctRowsCount = view.ToTable(true, selectedViewBy + "Name").Rows.Count;
                    _distinctObjectivesCount = view.ToTable(true, "StandardName").Rows.Count;
                    if (oCTR.worksheetId != null)
                        totalStudents = Convert.ToString(GridDataTable.Rows[0]["TotalStudent"]);
                    else
                    {
                        totalStudents = GridDataTable.DefaultView.ToTable(true, "TotalStudent").Compute("Sum(TotalStudent)", "").ToString();
                    }
                    radGridResults.DataSource = GridDataTable;

                    BuildTheColumns(selectedType, GridDataTable.Rows[0], standardLevel);
                }
            }

            //if (selectedViewBy != null)
            //{
            //    switch (selectedViewBy.ToLower())
            //    {
            //        case "student":
            //            GridDataTable = GetCvteReportByStudent(selectedWorksheet, standardSet, standardLevel, selectedSchoolId.ToString(), selectedTeacher, selectedClass, selectedDemographics, selectedGroup, selectedStudent, startDate, endDate);
            //            break;
            //        case "teacher":
            //            GridDataTable = GetCvteReportByTeacher(selectedWorksheet, standardSet, standardLevel, selectedSchoolId.ToString(), selectedTeacher, selectedDemographics, selectedGroup, startDate, endDate);
            //            break;
            //        case "class":
            //            GridDataTable = GetCvteReportByClass(selectedWorksheet, standardSet, standardLevel, selectedSchoolId.ToString(), selectedClass, selectedTeacher, selectedStudent, selectedDemographics, selectedGroup, startDate, endDate);
            //            break;
            //        case "demographic":
            //            GridDataTable = GetCvteReportByDemographic(selectedWorksheet, standardSet, standardLevel, selectedSchoolId.ToString(), selectedClass, selectedTeacher, selectedDemographics, selectedGroup, startDate, endDate);
            //            break;
            //    }

            //    if (GridDataTable.Rows.Count > 0)
            //    {
            //        GridDataTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(GridDataTable, "StandardID", "EncryptedID");
            //        DataTableCount = GridDataTable.Rows.Count;
            //        DataView view = new DataView(GridDataTable);
            //        _distinctRowsCount = view.ToTable(true, selectedViewBy + "Name").Rows.Count;
            //        _distinctObjectivesCount = view.ToTable(true, "StandardName").Rows.Count;
            //        radGridResults.DataSource = GridDataTable;

            //        BuildTheColumns(selectedViewBy, GridDataTable.Rows[0]);
            //    }
            //}

            //FormatColumnsForExcel(selectedViewBy, standardLevel, SelectedCriteria);
            FormatColumnsForExcel(selectedCriteria);

            radGridResults.DataBind();
            initialDisplayText.Visible = false;
        }


        private static string GetStandardSetValueFromParmsTable()
        {
            List<string> standardSetValuesList = DefaultStandardSets.Split(',').Select(values => values.Trim()).ToList().Where(x => !string.IsNullOrEmpty(x)).ToList();

            return standardSetValuesList.Count == 0 ? null : standardSetValuesList[0];
        }

        private void FormatColumnsForExcel(SelectedCriteria selectedCriteria)
        {
            string selectedViewBy = selectedCriteria.SelectedViewBy;
            string standardLevel = selectedCriteria.StandardLevel;

            DataTable localDataTable = GridDataTable.Copy();

            localDataTable.Columns[selectedViewBy + "Name"].ColumnName = selectedViewBy + " (" + _distinctRowsCount + ")";
            if (!string.IsNullOrEmpty(standardLevel))
            {
                localDataTable.Columns["StandardName"].ColumnName = standardLevel + " (" + _distinctObjectivesCount + ")";
            }
            else
            {
                localDataTable.Columns["StandardName"].ColumnName = "Competencies (" + _distinctObjectivesCount + ")";
            }

            //localDataTable.Columns["DateA"].ColumnName = "No Attempt";
            //localDataTable.Columns["DateB"].ColumnName = "Beginning\r\n/Novice";
            //localDataTable.Columns["DateC"].ColumnName = "Developing\r\n/Emerging";
            //localDataTable.Columns["DateD"].ColumnName = "Accomplished\r\n/Proficient";
            //localDataTable.Columns["DateE"].ColumnName = "Exemplary\r\n/Master";

            ReportData theReportData = new ReportData
            {
                ReportDataTable = localDataTable,
                ReportSelectedCriteria = selectedCriteria
            };

            SessionObject.CompetencyTracking_ReportData = theReportData;


        }


        private void BuildTheColumns(TGDetail selectedViewBy, DataRow dataRow, string StandardLevel = "")
        {
            GridBoundColumn gridBoundColumn = new GridBoundColumn
            {
                DataField = selectedViewBy + "Name",
                HeaderText = "",
                ShowSortIcon = true,
                AllowSorting = true
            };
            radGridResults.MasterTableView.Columns.Add(gridBoundColumn);

            GridHyperLinkColumn hyperLinkColumn = new GridHyperLinkColumn
            {
                HeaderText = "StandardLevel",
                DataTextField = "StandardName",
                DataNavigateUrlFields = new[] { "EncryptedID" },
                DataNavigateUrlFormatString = "~/Record/StandardsPage.aspx?xID={0}",
                Target = "_new",
                ShowSortIcon = true,
                AllowSorting = true
            };
            radGridResults.MasterTableView.Columns.Add(hyperLinkColumn);


            if (StandardLevel != null && StandardLevel != string.Empty)
            {
                GridBoundColumn TotalCompletencies = new GridBoundColumn();
                radGridResults.MasterTableView.Columns.Add(TotalCompletencies);
                TotalCompletencies.HeaderText = "Competencies";
                TotalCompletencies.DataField = "TotalCompetencies";
                TotalCompletencies.Display = true;
            }

            //GridBoundColumn StudentID = new GridBoundColumn();
            //radGridResults.MasterTableView.Columns.Add(StudentID);
            //StudentID.DataField = "StudentID";
            //StudentID.Display = false;

            GridBoundColumn StudentID = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(StudentID);
            StudentID.DataField = "StudentID";
            StudentID.Display = false;

            GridBoundColumn TeacherID = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(TeacherID);
            TeacherID.DataField = "TeacherID";
            TeacherID.Display = false;

            GridBoundColumn StanderdID = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(StanderdID);
            StanderdID.DataField = "StandardId";
            StanderdID.Display = false;

            GridBoundColumn WorksheetID = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(WorksheetID);
            WorksheetID.DataField = "WorksheetID";
            WorksheetID.Display = false;

            GridBoundColumn included = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(included);
            included.DataField = "Included";
            included.Display = false;

            GridBoundColumn total = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(total);
            total.DataField = "Total";
            total.Display = false;

            GridBoundColumn excluded = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(excluded);
            excluded.DataField = "Excluded";
            excluded.Display = false;



            GridBoundColumn scoreColumnA = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(scoreColumnA);
            scoreColumnA.DataField = "DateA";
            scoreColumnA.HeaderText = dataRow["ScoreColumnA"].ToString();
            scoreColumnA.DataFormatString = "{0:MM/dd/yy}";

            GridBoundColumn countColumnA = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(countColumnA);
            countColumnA.DataField = "CountA";
            countColumnA.Display = false;

            GridBoundColumn scoreColumnB = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(scoreColumnB);
            scoreColumnB.DataField = "DateB";
            scoreColumnB.HeaderText = dataRow["ScoreColumnB"].ToString();
            scoreColumnB.DataFormatString = "{0:MM/dd/yy}";
            GridBoundColumn countColumnB = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(countColumnB);
            countColumnB.DataField = "CountB";
            countColumnB.Display = false;

            GridBoundColumn scoreColumnC = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(scoreColumnC);
            scoreColumnC.DataField = "DateC";
            scoreColumnC.HeaderText = dataRow["ScoreColumnC"].ToString();
            scoreColumnC.DataFormatString = "{0:MM/dd/yy}";
            GridBoundColumn countColumnC = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(countColumnC);
            countColumnC.DataField = "CountC";
            countColumnC.Display = false;

            GridBoundColumn scoreColumnD = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(scoreColumnD);
            scoreColumnD.DataField = "DateD";
            scoreColumnD.HeaderText = dataRow["ScoreColumnD"].ToString();
            scoreColumnD.DataFormatString = "{0:MM/dd/yy}";
            GridBoundColumn countColumnD = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(countColumnD);
            countColumnD.DataField = "CountD";
            countColumnD.Display = false;

            GridBoundColumn scoreColumnE = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(scoreColumnE);
            scoreColumnE.DataField = "DateE";
            scoreColumnE.HeaderText = dataRow["ScoreColumnE"].ToString();
            scoreColumnE.DataFormatString = "{0:MM/dd/yy}";
            GridBoundColumn countColumnE = new GridBoundColumn();
            radGridResults.MasterTableView.Columns.Add(countColumnE);
            countColumnE.DataField = "CountE";
            countColumnE.Display = false;

            //GridBoundColumn perA = new GridBoundColumn();
            //radGridResults.MasterTableView.Columns.Add(perA);
            //perA.DataField = "PerA";
            //perA.Display = false;

            //GridBoundColumn perB = new GridBoundColumn();
            //radGridResults.MasterTableView.Columns.Add(perB);
            //perB.DataField = "PerB";
            //perB.Display = false;

            //GridBoundColumn perC = new GridBoundColumn();
            //radGridResults.MasterTableView.Columns.Add(perC);
            //perC.DataField = "PerC";
            //perC.Display = false;

            //GridBoundColumn perD = new GridBoundColumn();
            //radGridResults.MasterTableView.Columns.Add(perD);
            //perD.DataField = "PerD";
            //perD.Display = false;

            //GridBoundColumn perE = new GridBoundColumn();
            //radGridResults.MasterTableView.Columns.Add(perE);
            //perE.DataField = "PerE";
            //perE.Display = false;





        }

        public static DataTable GetCvteReportByClass(string worksheetId, string standardSet, string standardLevel, string schoolId = null,
            string className = null, string teacherId = null, string studentId = null, List<Demographics.ValueObject> selectedDemographics = null,
            string groupID = null, string startDate = null, string endDate = null)
        {
            DataTable cvteReportByClass = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_CVTEReportByClass",
                    Connection = conn
                };

                cmd.Parameters.Add(new SqlParameter { ParameterName = "WSID", Value = worksheetId });

                cmd.Parameters.Add(standardSet == null
                    ? new SqlParameter { ParameterName = "StandardSet", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardSet", Value = standardSet });

                cmd.Parameters.Add(standardLevel == null
                    ? new SqlParameter { ParameterName = "StandardLevel", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardLevel", Value = standardLevel });

                cmd.Parameters.Add((schoolId == null || schoolId == "0")
                    ? new SqlParameter { ParameterName = "SchoolID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "SchoolID", Value = schoolId });

                cmd.Parameters.Add(className == null
                    ? new SqlParameter { ParameterName = "ClassName", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "ClassName", Value = className });

                cmd.Parameters.Add(teacherId == null
                    ? new SqlParameter { ParameterName = "TeacherId", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "TeacherId", Value = teacherId });

                cmd.Parameters.Add(studentId == null
                    ? new SqlParameter { ParameterName = "StudentId", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StudentId", Value = studentId });

                if (selectedDemographics != null)
                {
                    foreach (var selectedDemographic in selectedDemographics)
                    {
                        switch (selectedDemographic.DemoLabel)
                        {
                            case "Gender":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gender", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                                break;
                            case "Race":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Race", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                                break;
                            case "English Language Learner":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EnglishLearner", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                                break;
                            case "Economically Disadvantaged":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EconomicDisAdv", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                                break;
                            case "Gifted":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gifted", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                                break;
                            case "Students With Disabilities":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Disability", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                                break;
                            case "Early Intervention Program":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EarlyInt", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                                break;
                        }
                    }
                }

                if (!cmd.Parameters.Contains("Gender"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Race"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EnglishLearner"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EconomicDisAdv"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Gifted"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Disability"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EarlyInt"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                }

                cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(groupID)
                    ? new SqlParameter { ParameterName = "GroupID", Value = groupID }
                    : new SqlParameter { ParameterName = "GroupID", Value = DBNull.Value });
                cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(startDate)
                    ? new SqlParameter { ParameterName = "StartDate", Value = startDate }
                    : new SqlParameter { ParameterName = "StartDate", Value = DBNull.Value });
                cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(endDate)
                    ? new SqlParameter { ParameterName = "EndDate", Value = endDate }
                    : new SqlParameter { ParameterName = "EndDate", Value = DBNull.Value });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(cvteReportByClass);
            }
            return cvteReportByClass;
        }

        private DataTable GetCvteReportByDemographic(string worksheetId, string standardSet, string standardLevel, string schoolID = null,
            string className = null, string teacherId = null, List<Demographics.ValueObject> selectedDemographics = null,
            string groupID = null, string startDate = null, string endDate = null)
        {
            DataTable cvteReportByDemographic = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_CVTEReportByDemographic",
                    Connection = conn
                };

                cmd.Parameters.Add(new SqlParameter { ParameterName = "WSID", Value = worksheetId });

                cmd.Parameters.Add(standardSet == null
                    ? new SqlParameter { ParameterName = "StandardSet", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardSet", Value = standardSet });

                cmd.Parameters.Add(standardLevel == null
                    ? new SqlParameter { ParameterName = "StandardLevel", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardLevel", Value = standardLevel });

                cmd.Parameters.Add((schoolID == null || schoolID == "0")
                    ? new SqlParameter { ParameterName = "SchoolID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "SchoolID", Value = schoolID });

                cmd.Parameters.Add(className == null
                    ? new SqlParameter { ParameterName = "ClassName", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "ClassName", Value = className });

                cmd.Parameters.Add(teacherId == null
                    ? new SqlParameter { ParameterName = "TeacherName", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "TeacherName", Value = teacherId });

                if (selectedDemographics != null)
                {
                    foreach (var selectedDemographic in selectedDemographics)
                    {
                        switch (selectedDemographic.DemoLabel)
                        {
                            case "Gender":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gender", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                                break;
                            case "Race":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Race", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                                break;
                            case "English Language Learner":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EnglishLearner", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                                break;
                            case "Economically Disadvantaged":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EconomicDisAdv", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                                break;
                            case "Gifted":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gifted", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                                break;
                            case "Students With Disabilities":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Disability", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                                break;
                            case "Early Intervention Program":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EarlyInt", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                                break;
                        }
                    }
                }

                if (!cmd.Parameters.Contains("Gender"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Race"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EnglishLearner"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EconomicDisAdv"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Gifted"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Disability"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EarlyInt"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                }

                cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(groupID)
                    ? new SqlParameter { ParameterName = "GroupID", Value = groupID }
                    : new SqlParameter { ParameterName = "GroupID", Value = DBNull.Value });
                cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(startDate)
                    ? new SqlParameter { ParameterName = "StartDate", Value = startDate }
                    : new SqlParameter { ParameterName = "StartDate", Value = DBNull.Value });
                cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(endDate)
                    ? new SqlParameter { ParameterName = "EndDate", Value = endDate }
                    : new SqlParameter { ParameterName = "EndDate", Value = DBNull.Value });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(cvteReportByDemographic);
            }
            return cvteReportByDemographic;
        }

        public static DataTable GetCvteReportByTeacher(string worksheetId, string standardSet, string standardLevel, string schoolID = null,
            string teacherId = null, List<Demographics.ValueObject> selectedDemographics = null, string groupID = null, string startDate = null, string endDate = null)
        {
            DataTable cvteReportByTeacher = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_CVTEReportByTeacher",
                    Connection = conn
                };

                cmd.Parameters.Add(worksheetId == null
                    ? new SqlParameter { ParameterName = "WSID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "WSID", Value = worksheetId });

                cmd.Parameters.Add(standardSet == null
                    ? new SqlParameter { ParameterName = "StandardSet", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardSet", Value = standardSet });

                cmd.Parameters.Add(standardLevel == null
                    ? new SqlParameter { ParameterName = "StandardLevel", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardLevel", Value = standardLevel });

                cmd.Parameters.Add((schoolID == null || schoolID == "0")
                    ? new SqlParameter { ParameterName = "SchoolID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "SchoolID", Value = schoolID });


                cmd.Parameters.Add(teacherId == null
                    ? new SqlParameter { ParameterName = "TeacherId", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "TeacherId", Value = teacherId });

                if (selectedDemographics != null)
                {
                    foreach (var selectedDemographic in selectedDemographics)
                    {
                        switch (selectedDemographic.DemoLabel)
                        {
                            case "Gender":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gender", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                                break;
                            case "Race":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Race", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                                break;
                            case "English Language Learner":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EnglishLearner", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                                break;
                            case "Economically Disadvantaged":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EconomicDisAdv", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                                break;
                            case "Gifted":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gifted", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                                break;
                            case "Students With Disabilities":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Disability", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                                break;
                            case "Early Intervention Program":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EarlyInt", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                                break;
                        }
                    }
                }

                if (!cmd.Parameters.Contains("Gender"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Race"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EnglishLearner"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EconomicDisAdv"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Gifted"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("Disability"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                }
                if (!cmd.Parameters.Contains("EarlyInt"))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                }

                cmd.Parameters.Add(new SqlParameter { ParameterName = "GroupID", Value = DBNull.Value });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "StartDate", Value = DBNull.Value });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "EndDate", Value = DBNull.Value });

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(cvteReportByTeacher);
            }
            return cvteReportByTeacher;
        }

        public static DataTable GetCvteReportByStudent(string worksheetId, string standardSet, string standardLevel,
            string schoolID = null, string teacherId = null, string className = null,
            List<Demographics.ValueObject> selectedDemographics = null, string groupID = null, string studentName = null, string startDate = null, string endDate = null)
        {
            DataTable cvteReportByStudent = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_CVTEReportByStudent",
                    Connection = conn
                };

                cmd.Parameters.Add(worksheetId == null
                    ? new SqlParameter { ParameterName = "WSID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "WSID", Value = worksheetId });

                cmd.Parameters.Add(standardSet == null
                    ? new SqlParameter { ParameterName = "StandardSet", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardSet", Value = standardSet });

                cmd.Parameters.Add(standardLevel == null
                    ? new SqlParameter { ParameterName = "StandardLevel", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "StandardLevel", Value = standardLevel });
                cmd.Parameters.Add((schoolID == null || schoolID == "0")
                    ? new SqlParameter { ParameterName = "SchoolID", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "SchoolID", Value = schoolID });
                cmd.Parameters.Add(className == null
                    ? new SqlParameter { ParameterName = "ClassName", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "ClassName", Value = className });
                cmd.Parameters.Add(teacherId == null
                    ? new SqlParameter { ParameterName = "TeacherId", Value = DBNull.Value }
                    : new SqlParameter { ParameterName = "TeacherId", Value = teacherId });

                if (selectedDemographics != null)
                {
                    foreach (var selectedDemographic in selectedDemographics)
                    {
                        switch (selectedDemographic.DemoLabel)
                        {
                            case "Gender":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gender", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                                break;
                            case "Race":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Race", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                                break;
                            case "English Language Learner":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EnglishLearner", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                                break;
                            case "Economically Disadvantaged":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EconomicDisAdv", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                                break;
                            case "Gifted":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Gifted", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                                break;
                            case "Students With Disabilities":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "Disability", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                                break;
                            case "Early Intervention Program":
                                cmd.Parameters.Add(selectedDemographic.DemoValue != null
                                    ? new SqlParameter { ParameterName = "EarlyInt", Value = selectedDemographic.DemoValue }
                                    : new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                                break;
                        }
                    }

                    if (!cmd.Parameters.Contains("Gender"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Gender", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("Race"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Race", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("EnglishLearner"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "EnglishLearner", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("EconomicDisAdv"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "EconomicDisAdv", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("Gifted"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Gifted", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("Disability"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "Disability", Value = DBNull.Value });
                    }
                    if (!cmd.Parameters.Contains("EarlyInt"))
                    {
                        cmd.Parameters.Add(new SqlParameter { ParameterName = "EarlyInt", Value = DBNull.Value });
                    }

                    cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(studentName)
                        ? new SqlParameter { ParameterName = "StudentId", Value = studentName }
                        : new SqlParameter { ParameterName = "StudentId", Value = DBNull.Value });
                    cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(groupID)
                        ? new SqlParameter { ParameterName = "GroupID", Value = groupID }
                        : new SqlParameter { ParameterName = "GroupID", Value = DBNull.Value });
                    cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(startDate)
                        ? new SqlParameter { ParameterName = "StartDate", Value = startDate }
                        : new SqlParameter { ParameterName = "StartDate", Value = DBNull.Value });
                    cmd.Parameters.Add(IEnumerableExtensions.IsNotNullOrEmpty(endDate)
                        ? new SqlParameter { ParameterName = "EndDate", Value = endDate }
                        : new SqlParameter { ParameterName = "EndDate", Value = DBNull.Value });

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(cvteReportByStudent);
                }
                return cvteReportByStudent;
            }
        }

        public void ExportToExcel()
        {

            // Remove columns from datatable
            //reportData.ReportDataTable = FormatDataTableForExcelExport(reportData.ReportDataTable);

            // Create the workbook
            XLWorkbook workbook = BuildWorkBook(CompetencyTrackingReport);

            //Prepare the response
            System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
            Response.Clear();
            Response.Buffer = true;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (browser.Platform.IndexOf("WinNT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    workbook.SaveAs(memoryStream);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + CompetencyTrackingReport_NoSpaces + ".xlsx");
                    Response.BinaryWrite(memoryStream.ToArray());
                }
                else
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "text/csv";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + CompetencyTrackingReport_NoSpaces + ".csv");
                    byte[] csv = ExportToCSV.ConvertXLToCSV(workbook);
                    Response.BinaryWrite(csv);
                }

                Response.End();
            }
        }

        private DataTable FormatDataTableForExcelExport(DataTable exportReadyDataTable)
        {
            foreach (string ignoredColumn in _excelIgnoredColumns)
            {
                if (exportReadyDataTable.Columns.Contains(ignoredColumn))
                {
                    exportReadyDataTable.Columns.Remove(ignoredColumn);
                }
            }
            return exportReadyDataTable;
        }

        private void BuildCriteriaSheet(string worksheetName, XLWorkbook workbook, SelectedCriteria selectedCriteria)
        {
            //You can find info on manipulating the Excel workbook here - https://closedxml.codeplex.com/documentation
            var ws = workbook.Worksheets.Add(worksheetName);

            ws.Range("B2:B4").Style.Font.Bold = true;
            ws.Cell(2, 2).Value = CompetencyTrackingReport;
            ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;  //Center the cell
            ws.Range("B2:C2").Row(1).Merge(); //Merge across cells
            ws.Range("B2:C2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            ws.Cell(3, 2).Value = "Standard List";
            ws.Cell(4, 2).Value = "Criteria";
            ws.Cell("B4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;  //Center the cell
            ws.Range("B4:B7").Column(1).Merge(); ; //Merge across columns


            if (string.IsNullOrEmpty(selectedCriteria.SelectedWorksheet))
            {
                ws.Cell(3, 3).Value = selectedCriteria.StandardList;
            }
            else
            {
                ws.Cell(3, 3).Value = selectedCriteria.StandardList + ": \"" + selectedCriteria.SelectedWorksheet + "\"";
            }

            //View By, Demographics, Standard Level, and Data Range
            ws.Cell(4, 3).Value = "View By = " + selectedCriteria.SelectedViewBy;
            ws.Cell(5, 3).Value = "Demographics = " + selectedCriteria.SelectedDemographics.ToArray();
            ws.Cell(6, 3).Value = "Standard Level = " + selectedCriteria.StandardLevel;
            ws.Cell(7, 3).Value = "Date Range = (start:" + selectedCriteria.StartDate + " - end: " + selectedCriteria.EndDate + ")";

            ws.Range("D2:D7").Style.Font.Bold = true;
            var theDistrict = Base.Classes.District.GetDistrictByID(SessionObject.LoggedInUser.District);
            ws.Cell(2, 4).Value = theDistrict.DistrictName + " (" + theDistrict.ClientID + ")";

            ws.Cell("D2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;  //Center the cell
            ws.Range("D2:G2").Row(1).Merge(); //Merge across cells
            ws.Range("D2:G2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            ws.Cell(3, 4).Value = "School";
            ws.Cell(4, 4).Value = "Teacher";
            ws.Cell(5, 4).Value = "Class";
            ws.Cell(6, 4).Value = "Group";
            ws.Cell(7, 4).Value = "Student";

            ws.Cell(3, 5).Value = selectedCriteria.SelectedSchoolName;
            ws.Cell(4, 5).Value = selectedCriteria.SelectedTeacher;
            ws.Cell(5, 5).Value = selectedCriteria.SelectedClass;
            ws.Cell(6, 5).Value = selectedCriteria.SelectedGroup;
            ws.Cell(7, 5).Value = selectedCriteria.SelectedStudent;

            ws.Range("F3:F6").Style.Font.Bold = true;
            ws.Cell(3, 6).Value = "# Schools";
            ws.Cell(4, 6).Value = "# Teachers";
            ws.Cell(5, 6).Value = "# Classes";
            ws.Cell(6, 6).Value = "# Groups";
            ws.Cell(7, 6).Value = "# Students";

            ws.Cell(3, 7).Value = "?";
            ws.Cell(4, 7).Value = "?";
            ws.Cell(5, 7).Value = "?";
            ws.Cell(6, 7).Value = "?";
            ws.Cell(7, 7).Value = "?";

            ws.Columns().AdjustToContents();
        }



        private XLWorkbook BuildWorkBook(string sheet = "")
        {
            DataTable dt = new DataTable();
            foreach (GridColumn col in radGridResults.Columns)
            {
                DataColumn colString = new DataColumn(col.UniqueName);
                dt.Columns.Add(colString);

            }
            foreach (GridDataItem row in radGridResults.Items) // loops through each rows in RadGrid
            {
                DataRow dr = dt.NewRow();
                foreach (GridColumn col in radGridResults.Columns) //loops through each column in RadGrid
                {
                    if (col.UniqueName.ToString() == "StandardName")
                        dr[col.UniqueName] = Regex.Replace((((System.Web.UI.WebControls.HyperLink)(row[col.UniqueName].Controls[0]))).Text, @"<[^>]+>|&nbsp;", "").Trim();
                    else
                        dr[col.UniqueName] = Regex.Replace(row[col.UniqueName].Text, @"<[^>]+>|&nbsp;", "").Trim();
                    // dr[col.UniqueName] = row[col.UniqueName].Text;

                }
                dt.Rows.Add(dr);
            }


            SelectedCriteria selectedCriteria = SessionObject.CompetencyTracking_ReportData.ReportSelectedCriteria;
            var columnDelte = 0;

            if (dt.Columns[2].ColumnName == "TotalCompetencies")
            {
                dt.Columns[0].ColumnName = Convert.ToString(SessionObject.CompetencyTracking_ReportData.ReportDataTable.Columns[0]);
                dt.Columns[1].ColumnName = Convert.ToString(SessionObject.CompetencyTracking_ReportData.ReportDataTable.Columns[1]);
                dt.Columns[2].ColumnName = "Competencies";
                columnDelte = 1;
            }
            else if (dt.Columns[0].ColumnName == "DemographicsName")
            {
                dt.Columns[0].ColumnName = Convert.ToString(SessionObject.CompetencyTracking_ReportData.ReportDataTable.Columns[2]);
                dt.Columns[1].ColumnName = Convert.ToString(SessionObject.CompetencyTracking_ReportData.ReportDataTable.Columns[3]);
            }
            else
            {
                dt.Columns[0].ColumnName = Convert.ToString(SessionObject.CompetencyTracking_ReportData.ReportDataTable.Columns[0]);
                dt.Columns[1].ColumnName = Convert.ToString(SessionObject.CompetencyTracking_ReportData.ReportDataTable.Columns[1]);

            }



            dt.Columns["DateA"].ColumnName = "No Attempt";
            dt.Columns["DateB"].ColumnName = "Beginning\r\n/Novice";
            dt.Columns["DateC"].ColumnName = "Developing\r\n/Emerging";
            dt.Columns["DateD"].ColumnName = "Accomplished\r\n/Proficient";
            dt.Columns["DateE"].ColumnName = "Exemplary\r\n/Master";


            //DataTable dt = reportData.ReportDataTable;
            //SelectedCriteria selectedCriteria = reportData.ReportSelectedCriteria;
            XLWorkbook workbook = new XLWorkbook();

            if (dt.Rows.Count > 0)
            {
                var ws = workbook.Worksheets.Add(sheet ?? "Sheet1");

                //Comment out this for now - not working 100% correctly yet
                //BuildCriteriaSheet("Query Criteria", workbook, selectedCriteria);

                // set the header size and alignment
                ws.Range("A1:G1").Merge();
                ws.Range("A1:G1").Value = GetMessage();
                ws.Range("A1:G1").Style.Alignment.WrapText = true;
                // ws.Range["A1:I1"] = GetMessage();
                IXLRows headerRow = ws.Rows(1, 1);
                if (headerRow != null)
                {
                    headerRow.Height = 40;
                    headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRow.Style.Font.SetBold();
                }

                int colCount;
                //Write second rows first so that we can calculate width of for the headers
                var rowCount = 3;
                foreach (DataRow row in dt.Rows) // Loop over the rows.
                {
                    colCount = 1;
                    foreach (var item in row.ItemArray) // Loop over the items.
                    {
                        string theCellValue = item.ToString();
                        if (colCount > 2)
                        {
                            if (colCount < row.ItemArray.Length && !string.IsNullOrEmpty(row.ItemArray[colCount].ToString()))
                            {
                                int outNum;
                                int nextCellvalue = int.TryParse(row.ItemArray[colCount].ToString(), out outNum) ? DataIntegrity.ConvertToInt(row.ItemArray[colCount].ToString()) : 0;

                                DateTime outDateTime;
                                string theShortData = DateTime.TryParse(item.ToString(), out outDateTime) ? DataIntegrity.ConvertToDate((item.ToString())).ToShortDateString() : "";


                            }
                        }

                        ws.Cell(rowCount, colCount).Value = theCellValue;

                        if (!string.IsNullOrEmpty(ws.Cell(rowCount, colCount).Value.ToString()))
                        {
                            if (colCount > 2)
                            {
                                ws.Cell(rowCount, colCount).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                ws.Cell(rowCount, colCount).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                var criteriaController = Master.CurrentCriteria();
                                _viewBySelectedValue = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ViewBy").Select(x => x.Text).FirstOrDefault();
                                if (_viewBySelectedValue.ToLower() == "student")
                                {
                                    ws.Cell(rowCount, colCount).Style.Fill.BackgroundColor = XLColor.LightGray;
                                }

                            }
                        }
                        colCount++;
                    }
                    rowCount++;






                }

                colCount = 1; //reset columns
                foreach (DataColumn column in dt.Columns)
                {
                    ws.Cell(2, colCount).Value = column.ColumnName;
                    ws.Cell(2, colCount).Style.Font.FontColor = XLColor.White;
                    ws.Cell(2, colCount).Style.Fill.BackgroundColor = (XLColor)XLColor.FromHtml("#5C82BE");
                    ws.Cell(2, colCount).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Cell(2, colCount).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(colCount).AdjustToContents();
                    colCount++;
                }

                //Should be able to delete these by name - CountA - CountE
                //ws.Column(1).Delete();

                if (columnDelte == 1)
                {
                    ws.Column(4).Delete();
                    ws.Column(4).Delete();
                    ws.Column(4).Delete();
                    ws.Column(4).Delete();
                    ws.Column(4).Delete();
                    ws.Column(4).Delete();
                    ws.Column(4).Delete();
                    ws.Column(5).Delete();
                    ws.Column(6).Delete();
                    ws.Column(7).Delete();
                    ws.Column(8).Delete();
                    ws.Column(9).Delete();
                }
                else
                {
                    ws.Column(3).Delete();
                    ws.Column(3).Delete();
                    ws.Column(3).Delete();
                    ws.Column(3).Delete();
                    ws.Column(3).Delete();
                    ws.Column(3).Delete();
                    ws.Column(3).Delete();
                    ws.Column(4).Delete();
                    ws.Column(5).Delete();
                    ws.Column(6).Delete();
                    ws.Column(7).Delete();
                    ws.Column(8).Delete();
                }




                return workbook;
            }
            return null;
        }



        private string GetMessage()
        {
            string message = string.Empty;
            var criteriaController = Master.CurrentCriteria();
            _viewBySelectedValue = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ViewBy").Select(x => x.Text).FirstOrDefault();
            string standardLevel = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StandardLevel").Select(x => x.Text).FirstOrDefault();
            string standardLevelValue = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StandardLevel").Select(x => x.Value).FirstOrDefault();
            if (_viewBySelectedValue.ToLower() == "student" && standardLevel == null)
            {
                message = "*Results represent the most recent date a student was scored for each competency and a count of the number of times marked per rubric column.";
            }
            else if (_viewBySelectedValue.ToLower() == "student" && standardLevel != null)
            {
                message = "*Percentages represent the % of competencies in column 3 marked for each rubric column.";
            }
            else if (_viewBySelectedValue.ToLower() != "student" && standardLevelValue == null)
            {
                message = "*Percentages represent the % of students from column 1 most recently  marked for each rubric column.";
            }
            else if (_viewBySelectedValue.ToLower() != "student" && standardLevelValue != null)
            {
                message = "*Percentages represent the % of total results for students in column 1 and competencies in column 3 most recently marked for each rubric column.";
            }
            return message;
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (SessionObject.CompetencyTracking_ReportData != null)
            {
                DataTable localDataTable = SessionObject.CompetencyTracking_ReportData.ReportDataTable;
                if (localDataTable.Rows.Count > 0)
                {
                    ExportToExcel();
                }
            }
        }

        public List<UserNodeList> FilterByStandard(List<UserNodeList> userNodeList)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            Base.Classes.Standards standards = sessionObject.Standards_SelectedStandard;
            if (standards != null)
            {
                userNodeList = FilterListByStandard(userNodeList, standards);
            }
            return userNodeList;
        }

        private void GetCurriculumIDsforStandards(string standardSet, string grade, string subject, string course, string standardName, string level)
        {
            DataSet ds = MCU.GetCurriculumCousesforStandards(standardSet, grade, subject, course, standardName, level);

            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        _filterCurriculumIDs.Add((int)row["ID"]);
                    }
                }

                if (ds.Tables.Count > 1)
                {
                    dt = ds.Tables[1];

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            _filterStandardIDs.Add((int)row["ID"]);
                        }
                    }
                }
            }
        }

        public List<UserNodeList> FilterListByStandard(List<UserNodeList> userNodeList, Base.Classes.Standards standards)
        {
            List<UserNodeList> nodeList = null;
            if (standards != null)
            {
                string grade = standards.Grade;
                string subject = standards.Subject;
                string course = standards.Course;
                string standardSet = standards.Standard_Set;
                string standardName = standards.StandardName;
                string level = standards.Level;


                GetCurriculumIDsforStandards(standardSet, grade, subject, course, standardName, level);

                List<UserNodeList> nodesToAdd = (from nl in userNodeList
                                                 let nodeListStandardIDs =
                             nl.AssociatedStandardIds.ToList()
                                                 where nodeListStandardIDs.Any() && _filterStandardIDs.Any()
                                                 where (from s in nodeListStandardIDs
                                                        join f in _filterStandardIDs
                                                        on Convert.ToInt32(s.ToString(CultureInfo.CurrentCulture))
                                                        equals Convert.ToInt32(f.ToString(CultureInfo.CurrentCulture))
                                                        select s).ToList().Count > 0
                                                 select nl).ToList();

                nodeList = userNodeList.Intersect(nodesToAdd.Distinct()).ToList();
            }

            return nodeList;
        }

        [System.Web.Services.WebMethod]
        public static string GetCompetencyList()
        {
            string[] StrResourcesToShowOnTile;
            string ResourceToShow = string.Empty;
            ResourceToShow = "Thinkgate.CompetencyList";
            DataSet resourcesToShow = KenticoHelper.GetTileMapLookupDataSet(ResourceToShow);

            StrResourcesToShowOnTile = resourcesToShow.Tables[0]
                .AsEnumerable()
                .OrderBy(p => p["ItemOrder"].ToString())
                .Select(s => s.Field<string>("KenticoDocumentTypeToShow"))
                .ToArray();

            KenticoHelper kHelper = new KenticoHelper();
            List<UserNodeList> nodeList = kHelper.GetCompetencyList(StrResourcesToShowOnTile);

            CompetencyTrackingReportPage trackingPage = new CompetencyTrackingReportPage();
            nodeList = trackingPage.FilterByStandard(nodeList);

            var result = (from t in nodeList
                          select new { t.NodeId, t.FriendlyName }).ToArray();

            DataTable dtNodeList = new DataTable();
            dtNodeList.Columns.Add(
                new DataColumn()
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = "DocumentId"
                }
            );
            dtNodeList.Columns.Add(
               new DataColumn()
               {
                   DataType = System.Type.GetType("System.String"),
                   ColumnName = "FriendlyName"
               }
           );

            foreach (var element in result)
            {
                var row = dtNodeList.NewRow();
                row["DocumentId"] = element.NodeId.ToString();
                row["FriendlyName"] = element.FriendlyName.ToString();
                dtNodeList.Rows.Add(row);
            }
            return dtNodeList.ToJSON(false);
        }

        public DataTable GetCompetencyListTable()
        {
            string[] StrResourcesToShowOnTile;
            string ResourceToShow = string.Empty;
            ResourceToShow = "Thinkgate.CompetencyList";

            DataSet resourcesToShow = KenticoHelper.GetTileMapLookupDataSet(ResourceToShow);

            StrResourcesToShowOnTile = resourcesToShow.Tables[0]
                .AsEnumerable()
                .OrderBy(p => p["ItemOrder"].ToString())
                .Select(s => s.Field<string>("KenticoDocumentTypeToShow"))
                .ToArray();

            KenticoHelper kHelper = new KenticoHelper();
            List<UserNodeList> nodeList = kHelper.GetCompetencyList(StrResourcesToShowOnTile);

            CompetencyTrackingReportPage trackingPage = new CompetencyTrackingReportPage();
            nodeList = trackingPage.FilterByStandard(nodeList);

            var result = (from t in nodeList
                          select new { t.NodeId, t.FriendlyName }).ToArray();

            DataTable dtNodeList = new DataTable();
            dtNodeList.Columns.Add(
                new DataColumn()
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = "DocumentId"
                }
            );
            dtNodeList.Columns.Add(
               new DataColumn()
               {
                   DataType = System.Type.GetType("System.String"),
                   ColumnName = "FriendlyName"
               }
           );

            foreach (var element in result)
            {
                var row = dtNodeList.NewRow();
                row["DocumentId"] = element.NodeId.ToString();
                row["FriendlyName"] = element.FriendlyName.ToString();
                dtNodeList.Rows.Add(row);
            }
            return dtNodeList;
        }




        [System.Web.Services.WebMethod]
        public static string GetTeacherListForSchool(int schoolID)
        {
            if (schoolID < 0) return null;
            DataTable teacherListForSchool = new DataTable();

            //set teacher dropdown to the current user if they are a teacher
            if (_loggedOnUserRoleName != null && _loggedOnUserRoleName.Equals("Teacher", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_loggedOnUser.UserFullName) || !string.IsNullOrEmpty(_loggedOnUser.Page.ToString()))
                {
                    return "[{\"TeacherName\":\"" + _loggedOnUser.UserFullName + "\",\"TeacherPage\":\"" + _loggedOnUser.Page + "\"}]";
                }

                return teacherListForSchool.ToJSON(false); //user was a teacher but SessionObject.LoggedInUser object had null values (return new 'empty' datatable')
            }
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            teacherListForSchool = oCTR.GetTeacherListBySchool(schoolID);

            return teacherListForSchool.ToJSON(false);
        }



        [System.Web.Services.WebMethod]
        public static string GetStudentListBySchoolAndTeacher(int schoolID, int teacherPage)
        {
            if (schoolID < 0) return null;
            DataTable studentListBySchoolAndTeacher = new DataTable();
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            studentListBySchoolAndTeacher = oCTR.GetStudentListBySchoolAndTeacher(schoolID, teacherPage);
            return studentListBySchoolAndTeacher.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string GetClassListBySchoolAndTeacher(int schoolId, int teacherId)
        {
            // DataTable classListBySchoolAndTeacher = new DataTable();
            // CompetencyTrackingReportPage obj=new CompetencyTrackingReportPage();
            // if (_loggedOnUserRoleName.ToLower() == Convert.ToString(RolePortal.Teacher).ToLower())
            // {
            //     classListBySchoolAndTeacher = obj.GetClassesForTeacherOnly();                          
            // }
            // else
            // { 
            // if (schoolId < 0) return null;          
            // Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            // classListBySchoolAndTeacher = oCTR.GetClassListBySchoolAndTeacher(schoolId, teacherId);

            //}
            // return classListBySchoolAndTeacher.Rows.Count <= 0 ? null : classListBySchoolAndTeacher.ToJSON(false);
            if (schoolId < 0) return null;
            DataTable classListBySchoolAndTeacher = new DataTable();

            //using (SqlConnection conn = new SqlConnection(AppSettings.ConnectionString))
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand
            //    {
            //        CommandType = CommandType.StoredProcedure,
            //        CommandText = "E3_GetCVTEClassesForSchoolTeacherForCTR",
            //        Connection = conn
            //    };

            //    cmd.Parameters.Add(new SqlParameter { ParameterName = "SchoolId", Value = schoolId });
            //    cmd.Parameters.Add(new SqlParameter { ParameterName = "TeacherId", Value = teacherId });

            //    SqlDataAdapter da = new SqlDataAdapter(cmd);

            //    da.Fill(classListBySchoolAndTeacher);
            //}
            var clsssList = Thinkgate.Base.Classes.Class.GetClassesForTeacher(teacherId, 0);
            clsssList = clsssList.Where(x => x.SchoolID == schoolId).ToList();
            classListBySchoolAndTeacher.Columns.Add("FriendlyName");
            classListBySchoolAndTeacher.Columns.Add("ID");

            foreach (var s in clsssList)
            {
                classListBySchoolAndTeacher.Rows.Add(s.FriendlyName, s.ID);
            }




            return classListBySchoolAndTeacher.Rows.Count <= 0 ? null : classListBySchoolAndTeacher.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string GetStudentListBySchoolTeacherAndClass(int schoolID, int teacherPage, int classId)
        {
            if (schoolID < 0) return null;
            DataTable studentListBySchoolTeacherAndClass = new DataTable();
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            studentListBySchoolTeacherAndClass = oCTR.GetStudentListBySchoolTeacherAndClass(schoolID, teacherPage, classId);

            return studentListBySchoolTeacherAndClass.ToJSON(false);
        }

        public DataTable PopulateListSelectionDropdownByTeacher(int teacherID)
        {
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
            return oCTR.GetListSelectionByTeacher(teacherID);
        }

    }
    [Serializable]
    public class ReportData
    {
        public DataTable ReportDataTable { get; set; }
        public SelectedCriteria ReportSelectedCriteria { get; set; }
    }

    [Serializable]
    public class SelectedCriteria
    {
        // Properties. 
        public string SelectedViewBy { get; set; }
        public int SelectedSchoolId { get; set; }
        public string SelectedSchoolName { get; set; }
        public string SelectedTeacher { get; set; }
        public string SelectedClass { get; set; }
        public string SelectedStudent { get; set; }
        public string StandardLevel { get; set; }
        public string SelectedDemographics { get; set; }
        public string SelectedGroup { get; set; }
        public string StandardList { get; set; }
        public string SelectedWorksheet { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StandardSet { get; set; }
        public string StandardId { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Course { get; set; }
    }

}
