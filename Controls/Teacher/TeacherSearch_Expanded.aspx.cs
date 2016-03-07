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

namespace Thinkgate.Controls.Teacher
{
    using Standpoint.Core.Utilities;
    using Thinkgate.Controls.E3Criteria;

    public partial class TeacherSearch_Expanded : ExpandedSearchPage
    {
        private bool _isTeacherAdd = false;
        private string _selectedSchoolType = string.Empty;
        private int dataTableRowCount = 0;
        public string HiddenGuid
        {
            get { return ViewState["StaffSearchGuid"] != null ? ViewState["StaffSearchGuid"].ToString() : string.Empty; }
            set { ViewState["StaffSearchGuid"] = value.ToString(); }
        }

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            LoadSearchScripts();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _selectedSchoolType = SessionObject.TeacherSearchParms.GetParm("SchoolType") != null
                                      ? SessionObject.TeacherSearchParms.GetParm("SchoolType").ToString().Replace(" ", "-s-")
                                      : null;

            ParseRequestQueryString();
            SetControlVisibility();
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
                RadScriptManager scriptManager = (RadScriptManager)ScriptManager.GetCurrent(this.Page);
                scriptManager.RegisterPostBackControl(this.exportGridImgBtn);
            }
        }

        private void SetControlVisibility()
        {
            btnSelectReturn.Visible = _isTeacherAdd;
            radGridResults.Height = _isTeacherAdd ? 500 : radGridResults.Height;
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

        protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindDataToGrid();
        }

        #region Private Methods
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

            bool userCrossSchools = SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Schools);

            if (!userCrossSchools && SessionObject.LoggedInUser.Schools != null && SessionObject.LoggedInUser.Schools.Count > 0)
            { schoolIdList = SessionObject.LoggedInUser.Schools.FindAll(s => schoolIdList.Contains(s.Id)).Select(s => s.Id).ToList(); }

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
                resultRow[nameCol] = infoRow["UserFullName"].ToString().Replace("''", "'");
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

        private Criteria LoadSearchCriteria()
        {
            var criteria = new Criteria();

            criteria.Add(new Criterion
            {
                Header = "Name",
                Key = "UserFullName",
                DataTextField = "UserFullName",
                DataValueField = "UserFullName",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                Removable = true,
                UIType = UIType.TextBox
            });

            criteria.Add(new Criterion
            {
                Header = "User ID",
                Key = "UserID",
                DataTextField = "UserID",
                DataValueField = "UserID",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                Removable = true,
                UIType = UIType.TextBox
            });

            criteria.Add(new Criterion
            {
                Header = "User Type",
                Key = "RoleName",
                DataTextField = "NAME",
                DataValueField = "NAME",
                Type = "String",
                DataSource = ThinkgateRole.GetAllRoleIdsAndNames(),
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
                Locked = dtCluster.Rows.Count <= 0 ? true : false,
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
                DefaultValue = _selectedSchoolType,
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
            bool criteriaIsValid = CheckValidSearchCriteria();
            if (!criteriaIsValid)
            {
                ((System.Web.UI.HtmlControls.HtmlGenericControl)initialDisplayText.Controls[0]).InnerText = "You must select at least 1 Search Criteria....";
                initialDisplayText.Visible = true;
                radGridResults.Visible = false;
            }
            else
            {
                string staffName = searchParms.CriterionList.Find(r => r.Key == "UserFullName").Object as string ?? string.Empty;
                staffName = staffName.Replace("'", "''");

                string staffID = searchParms.CriterionList.Find(r => r.Key == "UserID").Object as string ?? string.Empty;

                List<string> staffTypes = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("RoleName") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

                List<string> clusters = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Cluster") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

                List<string> schoolTypes = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("SchoolType") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

                var schoolCriterion = searchParms.CriterionList.Find(r => r.Key == "School");
                string schoolName = schoolCriterion.Object != null ? schoolCriterion.Object.ToString() : string.Empty;

                radGridResults.DataSource = GetResultsTable(staffName, staffID, staffTypes, clusters, schoolTypes, schoolName);
                dataTableRowCount = ((DataTable)radGridResults.DataSource).Rows.Count;
                radGridResults.DataBind();
                radGridResults.MasterTableView.GetColumn("Select").Visible = _isTeacherAdd;
                radGridResults.Visible = true;
                initialDisplayText.Visible = false;
            }
        }

        private bool CheckValidSearchCriteria()
        {
            bool _hasValidCriteria = false;
            List<Criterion> CritList = ((Criteria)Session["Criteria_" + HiddenGuid]).CriterionList;

            if (CritList.Find(r => r.Key == "UserFullName").Object != null ||
                CritList.Find(r => r.Key == "UserID").Object != null ||
                CritList.FindAll(r => !r.IsHeader && r.Key.Contains("RoleName") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList().Count > 0 ||
                CritList.FindAll(r => !r.IsHeader && r.Key.Contains("Cluster") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList().Count > 0 ||
                CritList.FindAll(r => !r.IsHeader && r.Key.Contains("SchoolType") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList().Count > 0 ||
                CritList.Find(r => r.Key == "School").Object != null)
            {
                _hasValidCriteria = true;
            }

            return _hasValidCriteria;
        }

        private void LoadSearchCriteriaControl()
        {
            var controlReportCriteria = (ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            controlReportCriteria.ID = "ctlSearchResultsCriteria";

            if (string.IsNullOrEmpty(HiddenGuid))
            {
                HiddenGuid = Guid.NewGuid().ToString();
                controlReportCriteria.Guid = HiddenGuid;
                controlReportCriteria.Criteria = LoadSearchCriteria();
                controlReportCriteria.FirstTimeLoaded = true;
            }
            else
            {
                controlReportCriteria.Guid = HiddenGuid;
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
            if (!string.IsNullOrEmpty(Request.QueryString["AddTeacher"]))
            {
                _isTeacherAdd = true;
            }
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
                        if ((lnk = (HyperLink)gridItem.FindControl("lnkListStaffName")) != null)
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
                System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
                Response.Clear();
                Response.Buffer = true;
                if (browser.Platform.IndexOf("WinNT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    workbook.SaveAs(memoryStream);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");
                    Response.BinaryWrite(memoryStream.ToArray());
                }
                else
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "text/csv";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=ExportData.csv");
                    byte[] csv = ExportToCSV.ConvertXLToCSV(workbook);
                    Response.BinaryWrite(csv);
                }

                Response.Flush();
                Response.End();

                Session["FileExport_Content" + HiddenGuid] = memoryStream.ToArray();
            }
        }

        protected void ExportGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            BindDataToGrid();
            if (dataTableRowCount > 0)
            {
                ExportToExcel(((DataTable)radGridResults.DataSource));
            }
        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }
    }
}
