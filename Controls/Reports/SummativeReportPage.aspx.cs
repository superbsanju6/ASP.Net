using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System.Text.RegularExpressions;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Web.Script.Serialization;
using Thinkgate.Controls.Reports;
using ClosedXML.Excel;
using System.IO;
using System.Web.UI;

namespace Thinkgate.Controls.Reports
{
    public partial class SummativeReportPage : ExpandedSearchPage
    {
        public string PageGuid;
        public int FormID;
        private List<string> _ignoredColumns = new List<string>();
        private int _totalColumns = 0;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            _ignoredColumns.Add("Level");
            _ignoredColumns.Add("LevelID");            
            _ignoredColumns.Add("ParentID");
            _ignoredColumns.Add("ConcatKey");
            _ignoredColumns.Add("ParentConcatKey");
            _ignoredColumns.Add("SortKey");

        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hiddenGuidBox.Text))
            {
                PageGuid = hiddenGuidBox.Text;
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["xID"]))
            {
                PageGuid = Request.QueryString["xID"].ToString();
            }
            else
            {
                PageGuid = Guid.NewGuid().ToString().Replace("-", "");
                hiddenGuidBox.Text = PageGuid;
            }

            LoadCriteria();
            var _dp = DistrictParms.LoadDistrictParms();
            btnFLDOE.Visible = _dp.FCAT_FLDOE;
                
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "anything", BuildStartupScript(btnExportExcel.ClientID, "../..", PageGuid), false);
            }
        }

        protected void RemoteReportReload(object sender, EventArgs e)
        {
            SessionObject.SummativeReport_DataTable = null;
            LoadRootNodes();
        }

        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlSummativeReportCriteria";

            if (Session["Criteria_" + PageGuid] == null) return;

            ctlReportCriteria.Guid = PageGuid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            criteraDisplayPlaceHolder.Controls.Add(ctlReportCriteria);

            ctlReportCriteria.HideCriterionSection("Teacher");
            ctlReportCriteria.HideCriterionSection("Class");
        }

        public DataTable GetDataTable()
        {
            if (SessionObject.SummativeReport_DataTable == null)
                ExecuteResults();

            return SessionObject.SummativeReport_DataTable;
        }

        public DataTable GetDataTableForClass(int classID)
        {
            return ExecuteResults(classID);
        }

        private DataTable ExecuteResults(int classID = 0)
        {
            radTreeResults.Visible = true;
            lblInitialText.Visible = false;

            var criteria = (Criteria)Session["Criteria_" + PageGuid];

            //Assessment            
            string testType = criteria.FindCriterionHeaderByText("Assessment").ReportStringVal;

            //Year
            var yearCriterion = criteria.FindCriterionHeaderByText("Year");
            string year = (yearCriterion == null || string.IsNullOrEmpty(yearCriterion.ReportStringVal)) ? DistrictParms.LoadDistrictParms().Year : string.Join(",", yearCriterion.ReportStringVal);                    
            
            //Grade
            var gradeControl = criteria.FindCriterionHeaderByText("Grade");            
            string grades = (gradeControl == null || string.IsNullOrEmpty(gradeControl.ReportStringVal)) ? "" : string.Join(",", gradeControl.ReportStringVal);

            //Subject
            var subjectControl = criteria.FindCriterionHeaderByText("Subject");
            string subjects = (subjectControl == null || subjectControl.Object == null || string.IsNullOrEmpty(subjectControl.Object.ToString())) ? "" : string.Join(",", subjectControl.Object.ToString());

            //School
            int schoolID = DataIntegrity.ConvertToInt(criteria.FindCriterionHeaderByText("School").ReportStringVal);

            //Demographics
            var demographicControl = criteria.FindCriterionHeaderByText("Demographics");
            string demoString = string.Empty;
            if (demographicControl != null && !string.IsNullOrEmpty(demographicControl.ReportStringVal))
            {
                var serializer = new JavaScriptSerializer();
                var demographicObject = serializer.Deserialize<ReportCriteria.DemographicJsonObject>(demographicControl.ReportStringVal);
                if (demographicObject != null)
                {
                    demoString = demographicObject.items.Aggregate(string.Empty, (current, demographic) => current + ("@@D" + demographic.demoField + "=" + demographic.value + "@@"));
                }
            }
            


            if (classID > 0)
            {
                var dt = Thinkgate.Base.Classes.Reports.GetSummativeReportForClass(classID, testType, year, grades, subjects, schoolID, demoString);
                dt.Columns.Add("ConcatKey");
                dt.Columns.Add("ParentConcatKey");
                dt.Columns.Add("SortKey");
                return dt;
            }
            else
            {
                SessionObject.SummativeReport_DataTable = ConvertDataSetToFormattedDataTable(Thinkgate.Base.Classes.Reports.GetSummativeReport(testType, year, grades, subjects, schoolID, demoString));
                return SessionObject.SummativeReport_DataTable;
            }
        }

        private DataTable ConvertDataSetToFormattedDataTable(DataSet ds)
        {
            //Add ConcatKey and ParentConcatKey columns to 1st record set so we only have to deal with it going forward
            //Resulting DataTable is what goes into session
            var reportData = ds.Tables[0];
            reportData.Columns.Add("ConcatKey");
            reportData.Columns.Add("ParentConcatKey");
            reportData.Columns.Add("SortKey");

            foreach (DataRow row in reportData.Rows)
            {
                var rows = ds.Tables[1].Select("Level='" + row["Level"] + "' and LevelID=" + row["LevelID"] + " and ParentLevelID='" + row["ParentID"]+"'");
                if (rows == null || rows.Length == 0) continue;
                var correspondingRow = rows[0];
                row["ConcatKey"] = correspondingRow["ConcatKey"];
                row["ParentConcatKey"] = correspondingRow["ParentConcatKey"];
                row["SortKey"] = correspondingRow["SortKey"];
            }

            return reportData;
        }

        private void AddColumnsToTreeList()
        {
            if (radTreeResults.Columns.Count > 0) return; //Don't duplicate effort

            var dt = GetDataTable();
            if (dt == null) return;

            foreach (DataColumn c in dt.Columns)
            {
                if (IgnoreColumn(c.ColumnName)) continue;

                _totalColumns++;

                var column = new TreeListNumericColumn();
                radTreeResults.Columns.Add(column);
                column.DataField = c.ColumnName;

                switch (c.ColumnName)
                {
                    case "LevelName":
                        column.HeaderText = "Name";
                        column.HeaderStyle.Width = 200;
                        column.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        break;

                    default:
                        column.HeaderText = (c.ColumnName == "StudentCount") ? "Students" : c.ColumnName;
                        column.HeaderStyle.Width = 70;
                        column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                        column.MinWidth = Unit.Parse("50");
                        column.MaxWidth = Unit.Parse("200");
                        if (c.ColumnName.Contains("%"))
                        {
                            column.DataFormatString = "{0:P}";
                        }
                        break;
                }
            }
        }

        #region RadTreeView Data Binding Events

        protected void radTreeResults_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {
            if (!(e.Item is TreeListDataItem)) return;            
            var dataItem = (DataRowView)((TreeListDataItem)e.Item).DataItem;

            switch (dataItem["Level"].ToString())
            {
                case "District":
                    e.Item.Cells[1].Text = "<img src='../../Images/TreeIcons/district.png' alt='district'/>&nbsp;" + e.Item.Cells[1].Text;
                    break;

                case "School":
                    e.Item.Cells[2].Text = "<img src='../../Images/TreeIcons/school.png' alt='school'/>&nbsp;" + e.Item.Cells[2].Text;
                    break;                

                case "Teacher":
                    e.Item.Cells[3].Text = "<img src='../../Images/TreeIcons/teacher.png' alt='teacher'/>&nbsp;" + e.Item.Cells[3].Text;
                    break;

                case "Class":
                    e.Item.Cells[4].Text = "<img src='../../Images/TreeIcons/class.png' alt='class'/>&nbsp;" + e.Item.Cells[4].Text;
                    break;

                case "Student":
                    //((TreeListDataItem)e.Item).Expanded = true; //Always a leaf
                    e.Item.Cells[4].Controls[0].Visible = false;
                    e.Item.Cells[5].Text = "<img src='../../Images/TreeIcons/student.png' alt='student'/>&nbsp;" + e.Item.Cells[5].Text;
                    break;
            }
        }

        protected void TreeListChild_DataSourceNeeded(object sender, TreeListChildItemsDataBindEventArgs e)
        {
            string concatKey = e.ParentDataKeyValues["ConcatKey"].ToString();
            AddColumnsToTreeList();

            if (concatKey.Contains("CL")) //Expanding Class Level
            {
                var classID = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(concatKey.Substring(concatKey.IndexOf("CL")+2));                
                e.ChildItemsDataSource = GetDataTableForClass(classID);
            }
            else
            {
                var dt = GetDataTable();
                if (dt == null) return;
                DataView dv = new DataView(dt,
                               "ParentConcatKey = '" + concatKey + "'", "SortKey", DataViewRowState.CurrentRows);
                e.ChildItemsDataSource = dv;
            }

        }

        private void LoadRootNodes()
        {
            AddColumnsToTreeList();

            var dt = GetDataTable();
            if (dt == null) return;
            DataView dv = new DataView(dt,
                               "ParentConcatKey is null", "SortKey", DataViewRowState.CurrentRows);
            radTreeResults.DataSource = dv;
        }

        protected void TreeListDataSourceNeeded(object sender, EventArgs e)
        {
            LoadRootNodes();
        }

        #endregion

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

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (SessionObject.SummativeReport_DataTable != null && SessionObject.SummativeReport_DataTable.Rows.Count > 0)
            {
                ExportToExcel(SessionObject.SummativeReport_DataTable);
            }
        }

        private bool IgnoreColumn(string columnName)
        {
            return _ignoredColumns.Contains(columnName);
        }
    }
}