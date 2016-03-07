using System;
using System.Data;
using Telerik.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Teacher
{
    public partial class TeacherPortalPreview : Thinkgate.Classes.TileControlBase
    {
        private int _userPage = 110;
        
        protected new void Page_Init(object sender, EventArgs e)
        {
            BindPageControls();            
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            ReloadResults();            
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
        }

        private void AddLevelNameColumnToTree()
        {
            var column = new TreeListBoundColumn();
            radTreeResults.Columns.Add(column);
            column.DataField = "LevelName";
            column.UniqueName = "LevelName";
            column.HeaderText = "Name";
        }

        private void ExecuteResults()
        {
            string environmentName = System.Configuration.ConfigurationManager.AppSettings.Get("Environment");

            var dev = environmentName == "Dev" ? true : false;

            var Year = "";
            var School = "All";
            var Student = "slist";
            var Class = dev ? "11242" : "4290"; //51925
            var TestCategory = "District"; //"District"; //TODO: make this come from parms
            var Product = String.Empty;
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
            if (dataSet == null || dataSet.Tables.Count < 3) return;

            SessionObject.TeacherPortal_assessmentRollup = dataSet.Tables[0];
            SessionObject.TeacherPortal_resultsData = dataSet.Tables[1];            
            SessionObject.TeacherPortal_heirarchyData = dataSet.Tables[2];
            
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
            LoadClassesDropDownItems();
            LoadCurriculumDropDownItems();
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

            if (cellIndex < 0) return;

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
        }

        protected void ddlCurriculum_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadResults();
        }

        protected void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadResults();
        } 

        #endregion

        private void LoadClassesDropDownItems()
        {
            ddlClass.Items.Clear();
            ddlClass.DataSource = Thinkgate.Base.Classes.Data.TeacherDB.GetClassesByUserPage(_userPage, "10-11", "Elements");
            ddlClass.DataTextField = "ListVal";
            ddlClass.DataValueField = "ListVal";
            var item = new RadComboBoxItem { Text = "Select", Value = "0" };
            ddlClass.DataBind();
            ddlClass.Items.Insert(0, item);
        }

        private void LoadCurriculumDropDownItems()
        {
            ddlCurriculum.Items.Clear();
            ddlCurriculum.DataSource = Thinkgate.Base.Classes.Data.TeacherDB.GetCurriculaByUserPage(_userPage);
            ddlCurriculum.DataTextField = "ListVal";
            ddlCurriculum.DataValueField = "ListVal";
            var item = new RadComboBoxItem { Text = "Select", Value = "0" };
            ddlCurriculum.DataBind();
            ddlCurriculum.Items.Insert(0, item);
        } 
    }
}