using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Web.Script.Serialization;
using ClosedXML.Excel;
using System.IO;
using Thinkgate.Controls.E3Criteria;

namespace Thinkgate.Controls.Reports
{
    public partial class SummativeReportPageV2 : BasePage
    {
        public string PageGuid;
        public int FormID;
        private List<string> _ignoredColumns = new List<string>();
        
        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += SearchHandler;
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
            if (!IsPostBack)
            {
                var serializer = new JavaScriptSerializer();
                var data = Thinkgate.Base.Classes.Reports.GetCriteriasForStateReporting();
                var schoolsForLoggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
                // create distinct grade list so we can filter the combo
                ctrlTestYearGradeSubject.JsonDataSource = serializer.Serialize(TestYearGradeSubject.BuildJsonArray(data));

                //schoolsForLoggedInUser = schoolsForLoggedInUser.FindAll(s => s.Grades.IndexOf("03") > -1);
                cmbSchool.JsonDataSource =  serializer.Serialize(BuildSchoolGradeJsonArray(schoolsForLoggedInUser));
                cmbSchool.OnClientLoad = "InitialLoadOfSchoolList";         // I've decided here to not put the school in with TestYeraGradeSubject control. Wanted to investigate linking an external control. These two lines are how I've gone about setting that dependency up
                ctrlTestYearGradeSubject.OnChange = "ChangeSchoolBasedOnGrade();";

                var _dp = DistrictParms.LoadDistrictParms();
                btnFLDOE.Visible = _dp.FCAT_FLDOE;
            }            
        }

        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            SessionObject.SummativeReport_DataTable = null;
            LoadRootNodes();
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

            //var criteria = (Criteria)Session["Criteria_" + PageGuid];
            var criteriaController = Master.CurrentCriteria();      // just going to go ahead and pull this from master instead of from search handler so it will work on tree updates
            
            //Assessment            
            //string testType = criteria.FindCriterionHeaderByText("Assessment").ReportStringVal;
            string selectedTestType = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("AssessmentType").Select(x => x.Text).FirstOrDefault();
            
            //Year
            string selectedYear = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Year").Select(x => x.Text).FirstOrDefault();
            if (String.IsNullOrEmpty(selectedYear)) selectedYear = DistrictParms.LoadDistrictParms().Year;
            //var yearCriterion = criteria.FindCriterionHeaderByText("Year");
            //string year = (yearCriterion == null || string.IsNullOrEmpty(yearCriterion.ReportStringVal)) ? DistrictParms.LoadDistrictParms().Year : string.Join(",", yearCriterion.ReportStringVal);                    
            
            //Grade
            string selectedGrade = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Grade").Select(x => x.Text).FirstOrDefault();
            
            //Subject
            string selectedSubject = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Subject").Select(x => x.Text).FirstOrDefault();
            selectedSubject = selectedSubject == "Reading" ? "Language Arts" : selectedSubject;
            //var subjectControl = criteria.FindCriterionHeaderByText("Subject");
            //string subjects = (subjectControl == null || subjectControl.Object == null || string.IsNullOrEmpty(subjectControl.Object.ToString())) ? "" : string.Join(",", subjectControl.Object.ToString());

            //School
            int selectedSchoolId = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("School").Select(x => x.Value).FirstOrDefault());
            //int schoolID = DataIntegrity.ConvertToInt(criteria.FindCriterionHeaderByText("School").ReportStringVal);

            //Demographics
            var sb = new StringBuilder();
            var selectedDemographics = criteriaController.ParseCriteria<E3Criteria.Demographics.ValueObject>("Demographics");
            foreach (var demo in selectedDemographics)
            {
                sb.Append("@@D" + demo.DemoField + "=" + demo.DemoValue + "@@");
            }
            string demoString = sb.ToString();
            

            if (classID > 0)
            {
                var dt = Thinkgate.Base.Classes.Reports.GetSummativeReportForClass(classID, selectedTestType, selectedYear, selectedGrade, selectedSubject, selectedSchoolId, demoString);
                dt.Columns.Add("ConcatKey");
                dt.Columns.Add("ParentConcatKey");
                dt.Columns.Add("SortKey");
                return dt;
            }
            else
            {
                SessionObject.SummativeReport_DataTable = ConvertDataSetToFormattedDataTable(Thinkgate.Base.Classes.Reports.GetSummativeReport(selectedTestType, selectedYear, selectedGrade, selectedSubject, selectedSchoolId, demoString));
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
            XLWorkbook workbook = Master.ConvertDataTableToSingleSheetWorkBook(dt, "Results");

            //Prepare the response

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");

                // Flush the workbook to the Response.OutputStream

                Response.BinaryWrite(memoryStream.ToArray());

                Response.End();
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

        public ArrayList BuildSchoolGradeJsonArray(List<Base.Classes.School> schools)
        {
            var arry = new ArrayList();
            foreach (var s in schools)
            {
                arry.Add(new object[]
                                 {
                                     s.Name, s.ID, s.Grades.Replace(" ", "").Split(',')
                                 });
            }
            return arry;
        }

    }
}
