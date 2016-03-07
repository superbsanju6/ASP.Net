using System;
using System.Collections;
using System.Data;
using Telerik.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.ControlHost
{
    public partial class AssessmentResultsPortal : Thinkgate.Classes.RecordPage
    {
        private int _userPage = 110;
        
        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (!IsPostBack)
            {
                BindPageControls();                                
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReloadResults();
            }
        }

        public void ReloadResults()
        {
            SessionObject.TeacherPortal_assessmentRollup = null;
            SessionObject.TeacherPortal_resultsData = null;
            SessionObject.TeacherPortal_chartData = null;
            SessionObject.TeacherPortal_heirarchyData = null;
            SessionObject.TeacherPortal_assessmentData2 = null;

            radTreeResults.Columns.Clear();
            AddLevelNameColumnToTree();            

            radTreeResults.DataSource = null;
            radTreeResults.DataSource = GetDataTable();
            AddTestColumns();
            radTreeResults.DataBind();
            radTreeResults.ExpandToLevel(3);
            SetChartTitles();
        }

        private void AddLevelNameColumnToTree()
        {            
            var column = new TreeListBoundColumn();
            radTreeResults.Columns.Add(column);
            column.DataField = "LevelName";
            column.UniqueName = "LevelName";
            column.HeaderText = "Name";
        }

        private void SetChartTitles()
        {
			if (SessionObject.TeacherPortal_chartData == null) return;
			var chartData = SessionObject.TeacherPortal_chartData;
			if (chartData.Rows.Count == 0) return;
			var chartDataRow = chartData.Rows[0];

			lblChartTitle1.Text = chartDataRow["ChartTitle1"].ToString();
			lblChartTitle2.Text = chartDataRow["ChartTitle2"].ToString();
			lblChartTitle3.Text = chartDataRow["ChartTitle3"].ToString();
			lblChartTitle4.Text = chartDataRow["ChartTitle4"].ToString();

			lblChartTitle1.Visible = (chartDataRow["ChartTitle1"].ToString().Length > 0);
			lblChartTitle2.Visible = (chartDataRow["ChartTitle2"].ToString().Length > 0);
			lblChartTitle3.Visible = (chartDataRow["ChartTitle3"].ToString().Length > 0);
			lblChartTitle4.Visible = (chartDataRow["ChartTitle4"].ToString().Length > 0);
        }

        private void ExecuteResults()
        {
            string environmentName = System.Configuration.ConfigurationManager.AppSettings.Get("Environment");

            var dev = environmentName == "Dev" ? true : false;

            var Year = "";
            var School = "All";
            var Student = "slist";
            var Class = dev ? "11242" : "4290"; //51925
            var TestCategory = "District"; //ddlSelectedCategory.SelectedValue; //TODO: make this come from parms
            var userPage = dev ? _userPage : 60528;
            // Michael was not sure if this was being used anymore (DSS)  GetSelectedCheckBoxListItems(chkListYears);
            //var testTypes = GetSelectedCheckBoxListItems(chkListAssessmentTypes);
            //if (testTypes == "") testTypes = "All";
            var testTerms = "";// GetSelectedCheckBoxListItems(chkListTerms);
            if (testTerms == "") testTerms = "All";

            string CritOrides = dev ? "@@ADMINYEAR=10-11@@@@CID=11242@@@@CURR=25@@tsearchfromadminportal@@@@TYRS=10-11@@TTERMS=All@@TTYPES=All@@" : "@@ADMINYEAR=11-12@@@@CID=4290@@@@CURR=247@@tsearchfromadminportal@@@@TYRS=11-12@@TTERMS=All@@TTYPES=All@@";

            //command.Parameters.Add(new SqlParameter("Year", String.Empty));
            //command.Parameters.Add(new SqlParameter("School", String.Empty));
            //command.Parameters.Add(new SqlParameter("Student", "slist"));
            //command.Parameters.Add(new SqlParameter("Class", "18919"));
            //command.Parameters.Add(new SqlParameter("UserPage", 119));
            //command.Parameters.Add(new SqlParameter("TestCategory", "District"));
            //command.Parameters.Add(new SqlParameter("CritOrides", "@@ADMINYEAR=10-11@@@@CID=55484@@@@CURR=25@@tsearchfromadminportal@@@@TYRS=10-11@@TTERMS=All@@TTYPES=All@@"));

            var dataSet = Thinkgate.Base.Classes.AssessmentResultsPortal.LoadResults(Year, School, Student, Class, userPage, TestCategory, CritOrides);

            if (dataSet == null || dataSet.Tables.Count < 5) return;

            SessionObject.TeacherPortal_assessmentRollup = dataSet.Tables[0];
            SessionObject.TeacherPortal_resultsData = dataSet.Tables[1];
            SessionObject.TeacherPortal_chartData = dataSet.Tables[2];
			SessionObject.TeacherPortal_heirarchyData = dataSet.Tables[3];
			SessionObject.TeacherPortal_assessmentData2 = dataSet.Tables[4];
        }

        public string GetSelectedCheckBoxListItems(CheckBoxList list)
        {
            var selected = "";
            foreach (ListItem i in list.Items)
            {
                if (i.Selected)
                    selected += i.Value + ",";
            }

            return selected.TrimEnd(new char[] { ',' });
        }

        public DataTable GetDataTable()
        {
            if (SessionObject.TeacherPortal_heirarchyData == null)
                ExecuteResults();
                
            return SessionObject.TeacherPortal_heirarchyData;
        }

        private void BindPageControls()
        {

            ddlAssessmentCategory.DataTextField = "ListLabel";
            ddlAssessmentCategory.DataValueField = "ListVal";
            ddlAssessmentCategory.DataSource = Thinkgate.Base.EntityInflation.AssessmentResultsPortalDropdowns.GetAssessmentCategories("teacher", "teacher");
            ddlAssessmentCategory.DataBind();

            ddlRace.DataTextField = "Val";
            ddlRace.DataValueField = "Val";
            ddlRace.DataSource = Thinkgate.Base.EntityInflation.AssessmentResultsPortalDropdowns.GetRaces();
            ddlRace.DataBind();
        }

        private void AddTestColumns()
        {
            if (SessionObject.TeacherPortal_assessmentData2 == null) return;

            var assessmentData2 = SessionObject.TeacherPortal_assessmentData2;

            foreach (DataRow test in assessmentData2.Rows)
            {
                var dateTimeCalculated = GetTestDateTimeCalculated(test["ID"].ToString());

                var column = new TreeListNumericColumn();
                radTreeResults.Columns.Add(column);
                column.DataField = "test_" + test["ID"].ToString();                
                column.HeaderText = "<a title='" + test["Description"] + " as of " + dateTimeCalculated + "'>" 
                                  + test["TestName"].ToString() + "<br/>as of "
                                  + dateTimeCalculated + "</a>";
                column.NumericType = NumericType.Percent;                                
            }
        }

        private string GetTestDateTimeCalculated(string testID)
        {
            if (SessionObject.TeacherPortal_resultsData == null) return "N/A";
            var resultsData = SessionObject.TeacherPortal_resultsData;
            if (resultsData.Rows.Count == 0) return "N/A";

            foreach (DataRow r in resultsData.Rows)
                if (r["TestID"].Equals(testID) && r["DateTimeCalculated"].ToString().Length > 0)
                    return (Standpoint.Core.Utilities.DataIntegrity.ConvertToDate(r["DateTimeCalculated"])).ToShortDateString();

            return "N/A";
        }

        #region Control Page Events

        protected void radTreeResults_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {
            if (!(e.Item is TreeListDataItem)) return;
            if (SessionObject.TeacherPortal_assessmentData2 == null) return;
            if (SessionObject.TeacherPortal_resultsData == null) return;

            var resultsData = SessionObject.TeacherPortal_resultsData;
            var assessmentData2 = SessionObject.TeacherPortal_assessmentData2;

            var cellIndex = e.Item.Cells.Count - assessmentData2.Rows.Count;

            var dataItem = (DataRowView)((TreeListDataItem)e.Item).DataItem;

            foreach (DataRow test in assessmentData2.Rows)
            {
                foreach (DataRow r in resultsData.Rows)
                {
                    if (dataItem["Level"].Equals(r["Level"])
                            && dataItem["LevelID"].Equals(r["LevelID"])
                            && r["TestID"].Equals(test["ID"]))
                    {
                        e.Item.Cells[cellIndex].Text = r["Score"].ToString();
                        
                        if(chkPerformanceLevels.Checked)
                            e.Item.Cells[cellIndex].BackColor = System.Drawing.Color.FromName(r["PLevel"].ToString());

                        break;
                    }
                }

                cellIndex++;
            }
        }

        protected void radTreeResults_NeedDataSource(object source, TreeListNeedDataSourceEventArgs e)
        {
            radTreeResults.DataSource = GetDataTable();
            SetChartTitles();
        }

        protected void btnRefreshResults_Click(object sender, EventArgs e)
        {
			HideUncheckedItems(rgTerms);
			HideUncheckedItems(rgAssessments);
            ReloadResults();
        }

        protected void ddlAssessmentCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadResults();
			//TODO: Remove hardcoded values
			DataTable testType = Thinkgate.Base.Classes.Assessment.LoadTestTypes(119, ddlAssessmentCategory.SelectedValue);
			// The only existing column is 'Type'. We must add a column for 'BtnText'.

			testType.Columns.Add("checked", typeof(bool));
        	testType.Columns["TestType"].ColumnName = "categoryName";
        	bool hasAll = false;
        	foreach (DataRow row in testType.Rows)
        	{
				if (hasAll == false && row["categoryName"].ToString() == "All")
					hasAll = true;
        		row["checked"] = false;
        	}
			if (hasAll == false)
			{
				DataRow row = testType.NewRow();
				row["checked"] = false;
				row["categoryName"] = "All";
				testType.Rows.InsertAt(row, 0);
			}

        	rgAssessments.DataSource = testType;
			rgAssessments.DataBind();

        }

        protected void chkPerformanceLevels_CheckedChanged(object sender, EventArgs e)
        {
            ReloadResults();
        }

        protected void chkUnlockedColumns_CheckedChanged(object sender, EventArgs e)
        {
            ReloadResults();
        }

        #endregion

		#region Terms Methods
		protected void BuildTerms()
		{
			rgTerms_NeedDataSource(new object(), new GridNeedDataSourceEventArgs(GridRebindReason.PostBackEvent));
			rgTerms.DataBind();
		}

		protected void rgTerms_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem dataItem = (GridDataItem)e.Item;
				CheckBox checkbx = (CheckBox)dataItem.FindControl("checked");
				dataItem.Attributes.Add("onclick", "SelectCheckBox('" + rgTerms.ClientID + "','" + dataItem.ItemIndex + "','" + checkbx.ClientID + "');");

			}
		}

		protected void rgTerms_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			DataTable table = new DataTable();
			table.Columns.Add(new DataColumn("checked", typeof(bool)));
			table.Columns.Add(new DataColumn("categoryName", typeof(String)));
			table.Rows.Add(false, "All");
			table.Rows.Add(false, "1");
			table.Rows.Add(false, "2");
			table.Rows.Add(false, "3");
			table.Rows.Add(false, "4");

			rgTerms.DataSource = table;
		}

		protected void rbEditTerms_Click(object sender, EventArgs e)
		{
			EditCheckedItems(rgTerms);
		}

		protected void rbClearTerms_Click(object sender, EventArgs e)
		{
			ShowAllItems(rgTerms);
		}

		#endregion

		#region Assessment Methods
		protected void BuildAssessments()
		{
			rgAssessments_NeedDataSource(new object(), new GridNeedDataSourceEventArgs(GridRebindReason.PostBackEvent));
			rgAssessments.DataBind();
		}

		protected void rgAssessments_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem dataItem = (GridDataItem)e.Item;
				CheckBox checkbx = (CheckBox)dataItem.FindControl("checked");
				dataItem.Attributes.Add("onclick", "SelectCheckBox('" + rgAssessments.ClientID + "','" + dataItem.ItemIndex + "','" + checkbx.ClientID + "');");

			}
		}

		protected void rgAssessments_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			DataTable table = new DataTable();
			table.Columns.Add(new DataColumn("checked", typeof(bool)));
			table.Columns.Add(new DataColumn("categoryName", typeof(String)));
			table.Rows.Add(false, "All");
			table.Rows.Add(false, "Test");
			table.Rows.Add(false, "Quiz");
			table.Rows.Add(false, "Screen");

			rgAssessments.DataSource = table;
		}

		protected void rbEditAssessments_Click(object sender, EventArgs e)
		{
			EditCheckedItems(rgAssessments);
		}

		protected void rbClearAssessments_Click(object sender, EventArgs e)
		{
			ShowAllItems(rgAssessments);
		}
		#endregion

		protected void EditCheckedItems(RadGrid grid)
		{
			if (grid.MasterTableView.OwnerGrid.Items.Count < 1)
			{
				grid.Rebind();
				return;
			}

			ArrayList items = new ArrayList();
			foreach (GridDataItem item in grid.Items)
			{
				CheckBox checkbx = (CheckBox)item.FindControl("checked");
				items.Add(checkbx.Checked ? 1 : 0);
			}

			grid.Rebind();

			for (int i = 0; i < grid.Items.Count; i++)
			{
				CheckBox checkbx = (CheckBox)grid.Items[i].FindControl("checked");
				checkbx.Checked = ((int)items[i] == 1);
				grid.Items[i].Display = true;
			}
		}

		protected void ShowAllItems(RadGrid grid)
		{
			if (grid.DataSource == null)
			{
				grid.Rebind();
				return;
			}

			foreach (GridDataItem item in grid.Items)
			{
				CheckBox checkbx = (CheckBox)item.FindControl("checked");
				checkbx.Checked = false;
				item.Display = true;
				item.Enabled = true;
			}
		}

		protected void HideUncheckedItems(RadGrid grid)
		{
			bool checkedAll = false;
			bool anyItemIsChecked = false;
			CheckBox allCheckbx = new CheckBox();
			GridDataItem allItem = new GridDataItem(new GridTableView(grid), 0, 0 );

			foreach (GridDataItem item in grid.Items)
			{
				CheckBox checkbx = (CheckBox)item.FindControl("checked");
				if (item["category"].Text == "All" && checkbx.Checked)
				{
					checkedAll = true;
					allCheckbx = checkbx;
					allItem = item;
					anyItemIsChecked = true;
					break;
				}
			}

			foreach (GridDataItem item in grid.Items)
			{
				CheckBox checkbx = (CheckBox)item.FindControl("checked");
				checkbx.Checked = checkedAll ? false : checkbx.Checked;

				if (checkbx.Checked) anyItemIsChecked = true;
	
				item.Display = checkbx.Checked; //hide the row
				item.Enabled = true;
			}
			if (checkedAll)
			{
				allCheckbx.Checked = true;
				allItem.Display = true;
			}
			if(!anyItemIsChecked)
			{
				grid.DataSource = "";
				grid.DataBind();
			}
			
		}

    }
}