using System;
using Thinkgate.Classes;
using System.Data;
using Telerik.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Class
{
    public partial class ClassResults : TileControlBase
    {
        private Thinkgate.Base.Classes.Class _class;
        //Parms Dictionary includes: class

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            BindControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _class = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");
            if (_class == null) return;
            ReloadResults();
        }

        private void BindControls()
        {
            //cmbTerm.DataTextField = "Term";
            //cmbTerm.DataValueField = "Term";
            //cmbTerm.DataSource = Thinkgate.Base.EntityInflation.AssessmentResultsPortalDropdowns.GetTerms();
            //cmbTerm.DataBind();

            //cmbAssessmentType.DataTextField = "ListVal";
            //cmbAssessmentType.DataValueField = "ListVal";
            //cmbAssessmentType.DataSource = Thinkgate.Base.EntityInflation.AssessmentResultsPortalDropdowns.GetAssessmentTypes(null, "teacher", "teacher");
            //cmbAssessmentType.DataBind();
        }

        private void ReloadResults()
        {
            SessionObject.TeacherPortal_assessmentRollup = null;
            SessionObject.TeacherPortal_resultsData = null;
            SessionObject.TeacherPortal_chartData = null;
            SessionObject.TeacherPortal_heirarchyData = null;
            SessionObject.TeacherPortal_assessmentData2 = null;

            //radTreeResults.Columns.Clear();
            //AddLevelNameColumnToTree();      

            //radTreeResults.DataSource = null;
            //radTreeResults.DataSource = GetDataTable();
            //AddTestColumns();
            //radTreeResults.DataBind();
            //radTreeResults.ExpandToLevel(2);
        }

        private void AddLevelNameColumnToTree()
        {
            //var column = new TreeListBoundColumn();
            //radTreeResults.Columns.Add(column);
            //column.DataField = "LevelName";
            //column.UniqueName = "LevelName";
            //column.HeaderText = "Name";
        }

        private void ExecuteResults()
        {
            //var testType = cmbAssessmentType.SelectedValue;
            //var testTerm = cmbTerm.SelectedValue;

            //var dataSet = Thinkgate.Base.Classes.AssessmentResultsPortal.LoadResults(_class.ID, _userPage, testType, testTerm);

            //if (dataSet == null || dataSet.Tables.Count < 3) return;

            //SessionObject.TeacherPortal_assessmentData2 = dataSet.Tables[0];
            //SessionObject.TeacherPortal_resultsData = dataSet.Tables[1];
            //SessionObject.TeacherPortal_heirarchyData = dataSet.Tables[2];
        }

        private string GetSelectedCheckBoxListItems(CheckBoxList list)
        {
            var selected = "";
            foreach (ListItem i in list.Items)
            {
                if (i.Selected)
                    selected += i.Value + ",";
            }

            return selected.TrimEnd(new char[] { ',' });
        }

        private DataTable GetDataTable()
        {
            if (SessionObject.TeacherPortal_heirarchyData == null)
                ExecuteResults();

            return SessionObject.TeacherPortal_heirarchyData;
        }

        private void AddTestColumns()
        {
            if (SessionObject.TeacherPortal_assessmentData2 == null) return;

            var assessmentData2 = SessionObject.TeacherPortal_assessmentData2;

            foreach (DataRow test in assessmentData2.Rows)
            {
                //var column = new TreeListNumericColumn();
                //radTreeResults.Columns.Add(column);
                //column.DataField = "test_" + test["ID"].ToString();
                //column.HeaderText = test["TestName"].ToString();                                  
                //column.NumericType = NumericType.Percent;
            }
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

        protected void imgGo_Click(object sender, EventArgs e)
        {
            ReloadResults();
        }

        protected void radTreeResults_NeedDataSource(object source, TreeListNeedDataSourceEventArgs e)
        {
            //radTreeResults.DataSource = GetDataTable();
        }

        #endregion
    }
}