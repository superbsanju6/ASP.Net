using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.Reports;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.School
{
    using System.Linq;
    using Thinkgate.Controls.E3Criteria;

    public partial class SchoolSearch_Expanded : ExpandedSearchPage
    {
        public DataTable GridDataTable;

        public string PageGuid;
        public string Category;
        public string DataTableCount;
        private string _selectedSchoolType;

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
            //Start Block- Uncomment this block to unit test the implementation of Exception Application Block
            
            /*ExceptionHandler.exceptionManager.Process(() =>
            {
                CreateFileNotFoundException();
            }, ExceptionPolicies.CommunicationToExternalSourcesException.ToString());*/
            
            //End Block

            if (!string.IsNullOrEmpty(hiddenGuidBox.Text))
            {
                PageGuid = hiddenGuidBox.Text;
            }
            else
            {
                PageGuid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                hiddenGuidBox.Text = PageGuid;
            }

            _selectedSchoolType = SessionObject.SchoolSearchParms.GetParm("SchoolType") != null ? SessionObject.SchoolSearchParms.GetParm("SchoolType").ToString().Replace(" ", "-s-") : null;

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

                RadScriptManager scriptManager = (RadScriptManager)ScriptManager.GetCurrent(this.Page);
                scriptManager.RegisterPostBackControl(this.exportGridImgBtn);
            }
        }

        //Start Block- Uncomment this block to unit test the implementation of Exception Application Block

        /*private void CreateFileNotFoundException()
        {
            string text = System.IO.File.ReadAllText(@"C:\fileDoesNotExist.txt");
        }

        private void CreateDivideByZeroException()
        {
            int num1 = 10;
            int num2 = 0;
            int val = num1 / num2;
        }*/

        //End Block

        private void LoadSearchScripts()
        {
            if (Master != null)
            {
                var scriptManager = Master.FindControl("RadScriptManager2");
                if (scriptManager != null)
                {
                    var radScriptManager = (RadScriptManager)scriptManager;
                    radScriptManager.Scripts.Add(new ScriptReference("~/Scripts/SchoolSearch.js"));
                }
            }
        }


        private void BindDataToGrid()
        {
            Criteria searchParms = (Criteria)Session["Criteria_" + HiddenGuid];

            List<string> clusters = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Cluster") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

            List<string> schoolTypes = searchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Type") && r.Object != null).Select(criterion => criterion.Object.ToString()).ToList();

            var schoolCriterion = searchParms.CriterionList.Find(r => r.Key == "School");
            string schoolName = schoolCriterion.Object != null ? schoolCriterion.Object.ToString() : string.Empty;

            string schoolId = searchParms.CriterionList.Find(r => r.Key == "SchoolID").ReportStringVal ?? string.Empty;

            List<Thinkgate.Base.Classes.School> schools = Thinkgate.Base.Classes.SchoolMasterList.GetSchoolsForCriteria(clusters, schoolTypes, schoolName, schoolId);
            GridDataTable = Thinkgate.Base.Classes.SchoolMasterList.ConstructDataTableForSearch(schools);

            DataTableCount = GridDataTable.Rows.Count.ToString();
            radGridResults.DataSource = GridDataTable;
            radGridResults.DataBind();
            initialDisplayText.Visible = false;
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

        private Criteria LoadSearchCriteria()
        {
            var criteria = new Criteria();

            #region Criterion: Cluster
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
            #endregion

            #region Criterion: School type
            var schoolTypeDataTable = new DataTable();
            var schoolTypesForLoggedInUser = SchoolTypeMasterList.GetSchoolTypeListForUser(SessionObject.LoggedInUser);
            schoolTypeDataTable.Columns.Add("SchoolType");

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
                DefaultValue = _selectedSchoolType,
                ServiceUrl = "../../Services/School.svc/GetAllSchoolsFromSchoolTypes",
                ServiceOnSuccess = "getAllSchoolsFromSchoolTypes",
                Dependencies = new[]
                {
                    Criterion.CreateDependency("SchoolType", "SchoolTypes"),
                    Criterion.CreateDependency("School", "Schools")
                }
            });
            #endregion

            #region Criterion: School
            var schoolDataTable = new DataTable();
            var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);

            var result = schoolsForLooggedInUser.Select(e => new ComparerSchools { Name = e.Name, ID = e.ID }).ToList();

            result.Sort(new AlphanumComparator());

            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID");

            foreach (var s in result)
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
                UIType = UIType.DropDownList,
                DataSource = schoolDataTable,
                DataTextField = "Name",
                DataValueField = "ID",
                Removable = true
            });
            #endregion

            #region Criterion: School ID
            DataTable dtSchoolId = Thinkgate.Base.Classes.SchoolMasterList.GetSchoolIdsForUser(SessionObject.LoggedInUser).ToDataTable("SchoolID");
            criteria.Add(new Criterion
            {
                Header = "School ID",
                Key = "SchoolID",
                Type = "String",
                Description = string.Empty,
                Locked = false,
                Removable = true,
                Object = Category,
                UIType = UIType.DropDownList,
                DataSource = dtSchoolId,
                DataTextField = "SchoolID",
                DataValueField = "SchoolID"
            });
            #endregion

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
                HyperLink lnk;
                if ((lnk = (HyperLink)gridItem.FindControl("lnkListSchoolName")) != null)
                    lnk.NavigateUrl = "~/Record/School.aspx?childPage=yes&xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["ID"].ToString());
            }
        }

        public void ExportToExcel(DataTable dt)
        {
            // Create the workbook
            XLWorkbook workbook = ConvertDataTableToSingleSheetWorkBook(dt, "Results");

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
                    ((RadWindowManager)radWinManager).RadAlert(text, null, 100, "Message", "");
                }
            }
        }

        public string AddPlurality(int count)
        {
            return count == 0 || count > 1 ? "s have" : " has";
        }
    }
}
