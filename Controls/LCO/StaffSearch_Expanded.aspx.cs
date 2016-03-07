using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.Reports;

namespace Thinkgate.Controls.LCO
{
    using Standpoint.Core.Utilities;

    public partial class StaffSearch_Expanded : ExpandedSearchPage
    {
        public DataTable GridDataTable;

        public string PageGuid;
        public string Category;
        public string DataTableCount;

        public List<int> ListOfStaff
        {
            get { return (List<int>)Session["ListOfStaff_" + PageGuid]; }
            set { Session["ListOfStaff_" + PageGuid] = value; }
        }

        #region Properties

        public string HiddenGuid { get; set; }

        /// <summary>
        /// Get the current user id.
        /// </summary>

        /// <summary>
        /// Gets a data set consisting of all staff.
        /// It has three tables.
        /// Table[0] - StaffInfo
        ///	    UserID, UserPage, UserFullName, UserName (renamed to UserID)
        /// Table[1] - StaffRoles
        ///	    UserID, RoleName
        /// Table[2] - StaffSchools
        ///	    UserID, ID (renamed to SchoolID), SchoolName
        /// </summary>
        private DataSet AllStaff
        {
            get
            {
                const string key = "StaffSearch-AllStaff";
                DataSet allStaff = (DataSet)Thinkgate.Base.Classes.Cache.Get(key);

                if (allStaff == null)
                {
                    List<string> userTypes = new List<string>();
                    List<string> clusters = new List<string>();
                    List<string> schoolTypes = new List<string>();
                    List<int> schools = Thinkgate.Base.Classes.SchoolMasterList.GetSchoolTableIdsForUser(SessionObject.LoggedInUser);

                    allStaff = Thinkgate.Base.DataAccess.ThinkgateDataAccess.FetchDataSet(
                        "E3_Staff_Search",
                        new object[] { string.Empty, string.Empty, userTypes.ToSql(), clusters.ToSql(), schoolTypes.ToSql(), schools.ToSql() });

                    // Rename the tables and some columns.
                    allStaff = RenameResultSets(allStaff);

                    Thinkgate.Base.Classes.Cache.Insert(key, allStaff, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
                }

                return allStaff;
            }
        }

        private DataTable StaffInfo
        {
            get { return AllStaff.Tables["StaffInfo"]; }
        }

        private DataTable StaffRoles
        {
            get { return AllStaff.Tables["StaffRoles"]; }
        }

        private DataTable StaffSchools
        {
            get { return AllStaff.Tables["StaffSchools"]; }
        }

        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            LoadSearchScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(Page),
                    "anything",
                    BuildStartupScript(exportGridImgBtn.ClientID, "../..", PageGuid),
                    false);
            }

            ParseRequestQueryString();

            LoadSearchCriteriaControl();

            if (radGridResults.DataSource == null)
            {
                radGridResults.Visible = false;
                initialDisplayText.Controls.Add(new System.Web.UI.HtmlControls.HtmlGenericControl("div") { InnerText = "Press Search...." });
                initialDisplayText.Visible = true;
            }

            if (IsPostBack)
            {
                radGridResults.Visible = true;
            }
        }

        private void LoadSearchScripts()
        {
            if (Master != null)
            {
                var scriptManager = Master.FindControl("RadScriptManager2");
                if (scriptManager != null)
                {
                    var radScriptManager = (RadScriptManager)scriptManager;
                    radScriptManager.Scripts.Add(new ScriptReference("~/Scripts/TeacherSearch.js"));
                }
            }
        }

        protected void ImageIcon_Click(object sender, EventArgs e)
        {
        }

        protected void ImageGrid_Click(object sender, EventArgs e)
        {
        }

        protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindDataToGrid();
        }

        #region Private Methods

        /// <summary>
        /// Rename result set tables.
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private DataSet RenameResultSets(DataSet ds)
        {
            ds.Tables[0].TableName = "StaffInfo";
            ds.Tables[1].TableName = "StaffRoles";
            ds.Tables[2].TableName = "StaffSchools";
            return ds;
        }

        /// <summary>
        /// Return a data table with a single column containing user names.
        /// </summary>
        /// <returns></returns>
        private DataTable GetUserNames()
        {
            List<string> userName = (from s in StaffInfo.AsEnumerable() select s.Field<string>("UserFullName")).Distinct().ToList();
            userName.Sort();
            return userName.ToDataTable("UserFullName");
        }

        /// <summary>
        /// Return a data table with a single column containing user ids.
        /// </summary>
        /// <returns></returns>
        private DataTable GetUserIds()
        {
            List<string> userId = (from s in StaffInfo.AsEnumerable() select s.Field<string>("UserName")).Distinct().ToList();
            userId.Sort();
            return userId.ToDataTable("UserId");
        }

        /// <summary>
        /// Return a data table with a single column containing school names.
        /// </summary>
        /// <returns></returns>
        private DataTable GetSchoolNames()
        {
            List<string> names = (from s in StaffSchools.AsEnumerable() select s.Field<string>("SchoolName")).Distinct().ToList();
            names.Sort();
            return names.ToDataTable("SchoolName");
        }

        /// <summary>
        /// Get a data table comprising the search results.
        /// </summary>
        /// <param name="staffName"></param>
        /// <param name="staffID"></param>
        /// <param name="staffTypes"></param>
        /// <param name="clusters"></param>
        /// <param name="schoolTypes"></param>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        private DataTable GetResultsTable(string staffName, string staffID, List<string> staffTypes, List<string> clusters, List<string> schoolTypes, string schoolName)
        {
            // The stored proc doesn't like passing null. Pass empty strings for null values.
            if (staffName == null)
            {
                staffName = string.Empty;
            }

            if (staffID == null)
            {
                staffID = string.Empty;
            }

            // We have to generate a list of school ids for these criteria.
            List<Thinkgate.Base.Classes.School> schoolList = new List<Thinkgate.Base.Classes.School>();
            if (clusters != null && clusters.Count > 0)
            {
                schoolList.AddRange(SchoolMasterList.GetSchoolsForCriteria(clusters, null, null, null));
            }

            if (!string.IsNullOrEmpty(schoolName))
            {
                schoolList.AddRange(SchoolMasterList.GetSchoolsForCriteria(null, null, schoolName, null));
            }
            else if (schoolTypes != null && schoolTypes.Count > 0)
            {
                schoolList.AddRange(SchoolMasterList.GetSchoolsForCriteria(null, schoolTypes, null, null));
            }

            schoolList = schoolList.Distinct().ToList();
            List<int> schoolIdList = (from sc in schoolList select sc.ID).Distinct().ToList();

            // If we are not searching on school, we must list all possible schools for this user.
            if (schoolIdList.Count == 0)
            {
                schoolIdList = SchoolMasterList.GetSchoolTableIdsForUser(SessionObject.LoggedInUser);
            }

            DataSet filteredStaffDataSet = Thinkgate.Base.DataAccess.ThinkgateDataAccess.FetchDataSet(
                "E3_Staff_Search",
                new object[] { staffName, staffID, staffTypes.ToSql(), clusters.ToSql(), schoolTypes.ToSql(), schoolIdList.ToSql() });

            filteredStaffDataSet = RenameResultSets(filteredStaffDataSet);

            // Now we have to build a single data table to show in the grid.
            DataTable dt = new DataTable();
            DataColumn nameCol = dt.Columns.Add("Name");
            DataColumn userIDCol = dt.Columns.Add("UserID");
            DataColumn userPageCol = dt.Columns.Add("UserPage", typeof(int));
            DataColumn userTypeCol = dt.Columns.Add("UserType");
            DataColumn schoolCol = dt.Columns.Add("School");

            foreach (DataRow infoRow in filteredStaffDataSet.Tables["StaffInfo"].Rows)
            {
                DataRow resultRow = dt.NewRow();
                resultRow[nameCol] = infoRow["UserFullName"];
                resultRow[userIDCol] = infoRow["UserName"];
                resultRow[userPageCol] = infoRow["UserPage"];

                List<string> userTypes = (from t in filteredStaffDataSet.Tables["StaffRoles"].AsEnumerable()
                                          where t.Field<Guid>("UserID") == (Guid)infoRow["UserID"]
                                          select t.Field<string>("RoleName")).Distinct().ToList();

                resultRow[userTypeCol] = string.Join(", ", userTypes);

                List<string> schools = (from t in filteredStaffDataSet.Tables["StaffSchools"].AsEnumerable()
                                        where t.Field<Guid>("UserID") == (Guid)infoRow["UserID"]
                                        select t.Field<string>("SchoolName")).Distinct().ToList();

                resultRow[schoolCol] = string.Join(", ", schools);

                dt.Rows.Add(resultRow);
            }

            return dt;
        }

        /// <summary>
        /// Return a data table with a single column containing user roles.
        /// </summary>
        /// <returns></returns>
        private DataTable GetRoleNames()
        {
            List<string> userRole = (from s in StaffRoles.AsEnumerable() select s.Field<string>("RoleName")).Distinct().ToList();
            userRole.Sort();
            return userRole.ToDataTable("RoleName");
        }

        private Criteria LoadSearchCriteria()
        {
            var criteria = new Criteria();

            DataTable dtUserName = GetUserNames();
            criteria.Add(new Criterion
            {
                Header = "Name",
                Key = "UserFullName",
                DataTextField = "UserFullName",
                DataValueField = "UserFullName",
                Type = "String",
                DataSource = dtUserName,
                Description = string.Empty,
                Locked = false,
                Removable = true,
                UIType = UIType.DropDownList
            });

            DataTable dtUserId = GetUserIds();
            criteria.Add(new Criterion
            {
                Header = "User ID",
                Key = "UserID",
                DataTextField = "UserID",
                DataValueField = "UserID",
                Type = "String",
                DataSource = dtUserId,
                Description = string.Empty,
                Locked = false,
                Removable = true,
                UIType = UIType.DropDownList
            });

            DataTable dtRoleName = GetRoleNames();
            criteria.Add(new Criterion
            {
                Header = "User Type",
                Key = "RoleName",
                DataTextField = "RoleName",
                DataValueField = "RoleName",
                Type = "String",
                DataSource = dtRoleName,
                Description = string.Empty,
                Locked = false,
                Removable = true,
                UIType = UIType.CheckBoxList
            });

            DataTable dtCluster = Thinkgate.Base.Classes.SchoolMasterList.GetClustersForUser(SessionObject.LoggedInUser).ToDataTable("Cluster");
            criteria.Add(new Criterion
            {
                Header = "Cluster",
                Key = "Cluster",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Cluster",
                DataValueField = "Cluster",
                Removable = true,
                DataSource = dtCluster,
                UIType = UIType.CheckBoxList
            });

            DataTable dtSchoolType = Thinkgate.Base.Classes.SchoolMasterList.GetSchoolTypesForUser(SessionObject.LoggedInUser).ToDataTable("Type");
            criteria.Add(new Criterion
            {
                Header = "School Type",
                Key = "SchoolType",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Type",
                DataValueField = "Type",
                Removable = true,
                DataSource = dtSchoolType,
                UIType = UIType.CheckBoxList,
                ServiceUrl = "../../Services/School.svc/GetAllSchoolsFromSchoolTypes",
                ServiceOnSuccess = "getAllSchoolsFromSchoolTypes",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("SchoolType", "SchoolTypes"),
                    Criterion.CreateDependency("School", "Schools")
                }
            });

            // School
            var schoolDataTable = new DataTable();
            var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID");

            foreach (var s in schoolsForLooggedInUser)
            {
                schoolDataTable.Rows.Add(s.Name, s.ID);
            }

            criteria.Add(new Criterion
            {
                Header = "School",
                Key = "School",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                DataTextField = "Name",
                DataValueField = "ID",
                Removable = true,
                DataSource = schoolDataTable,
                UIType = UIType.DropDownList
            });

            return criteria;
        }

        private void BindDataToGrid()
        {
            Criteria searchParms = (Criteria)Session["Criteria_" + HiddenGuid];

            string staffName = searchParms.CriterionList.Find(r => r.Key == "UserFullName").ReportStringVal ?? string.Empty;

            string staffID = searchParms.CriterionList.Find(r => r.Key == "UserID").ReportStringVal ?? string.Empty;

            List<string> staffTypes = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("RoleName") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

            List<string> clusters = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Cluster") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

            List<string> schoolTypes = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("SchoolType") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

            var schoolCriterion = searchParms.CriterionList.Find(r => r.Key == "School");
            string schoolName = schoolCriterion.Object != null ? schoolCriterion.Object.ToString() : string.Empty;

            GridDataTable = GetResultsTable(staffName, staffID, staffTypes, clusters, schoolTypes, schoolName);

            DataTableCount = GridDataTable.Rows.Count.ToString();
            radGridResults.DataSource = GridDataTable;
            radGridResults.DataBind();
            initialDisplayText.Visible = false;
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
            }
            else
            {
                HiddenGuid = hiddenTextBox.Text;
                controlReportCriteria.Guid = hiddenTextBox.Text;
                controlReportCriteria.FirstTimeLoaded = false;
            }

            controlReportCriteria.InitialButtonText = "Search";
            controlReportCriteria.ReloadReport += ReportCriteria_ReloadReport;

            criteraDisplayPlaceHolder.Controls.Add(controlReportCriteria);
        }

        private void ReportCriteria_ReloadReport(object sender, EventArgs e)
        {
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
            if (e.Item is GridDataItem)
            {
                GridItem gridItem = e.Item;
                DataRowView row = (DataRowView)(gridItem).DataItem;

                if (String.Compare(row["UserType"].ToString(), "teacher", true) == 0)
                {
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_Teacher))
                    {
                        HyperLink lnk;
                        if ((lnk = (HyperLink) gridItem.FindControl("lnkListStaffName")) != null)
                            lnk.NavigateUrl = "~/Record/Teacher.aspx?childPage=yes&xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["UserPage"].ToString());
                    }
                }
                else
                {
                    HyperLink lnk;
                    if ((lnk = (HyperLink)gridItem.FindControl("lnkListStaffName")) != null)
                        lnk.NavigateUrl = "~/Record/Staff.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["UserPage"].ToString());
                }
            }
        }

        public void ExportToExcel(DataTable dt)
        {
            // Create the workbook
            XLWorkbook workbook = ConvertDataTableToSingleSheetWorkBook(dt, "Results");

            // Prepare the response

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
            if (GridDataTable != null && GridDataTable.Rows.Count > 0)
                ExportToExcel(GridDataTable);
        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }

        public void DisplayMessageInAlert(string text)
        {
            if (Master != null)
            {
                var radWinManager = Master.FindControl("RadWindowManager");

                if (radWinManager != null)
                {
                    ((RadWindowManager)radWinManager).RadAlert(text, null, 100, "Message", string.Empty);
                }
            }
        }

        public string AddPlurality(int count)
        {
            return count == 0 || count > 1 ? "s have" : " has";
        }
    }
}
