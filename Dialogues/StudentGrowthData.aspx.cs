using System;
using Standpoint.Core.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;

namespace Thinkgate.Dialogues
{
    public partial class StudentGrowthData : BasePage
    {
        private int _rubricResultID;
        private const string USERNAME_LABEL = "A Number";
        private string _aggregateConcordant;
        private DataSet _ds;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }

            _rubricResultID = GetDecryptedEntityId(X_ID);

            lblUserNameLabel.Text = USERNAME_LABEL;

            _ds = Thinkgate.Base.Classes.Rubric.GetRubricResultGrowthData(_rubricResultID);
            if (_ds == null || _ds.Tables.Count < 2) return;

            var headerData = _ds.Tables[0];

            if (headerData.Rows.Count == 0) return;

            lblTeacherName.Text = headerData.Rows[0]["EvaluateeName"].ToString();
            lblTeacherUserName.Text = headerData.Rows[0]["EvaluateeUserID"].ToString().ToUpper();
            _aggregateConcordant = headerData.Rows[0]["AggregateConcordant"].ToString();
            lblStudentGrowthConcordant.Text = _ds.Tables[1].Rows.Count > 1 || (_ds.Tables[1].Rows.Count > 0 && !_ds.Tables[1].Rows[0][0].ToString().Contains("N/A")) ? _aggregateConcordant : "N/A";

            switch (headerData.Rows[0]["EvalType"].ToString())
            {
                case "TeacherClassroom":
                    studentGrowthData_StaffTitle.Text = "Classroom Teacher Name: ";
                    break;

                case "TeacherNonClassroom":
                    studentGrowthData_StaffTitle.Text = "Non-Classroom Teacher Name: ";
                    break;

                case "Administrator":
                    studentGrowthData_StaffTitle.Text = "School-Based Administrator Name: ";
                    break;

                default:
                    studentGrowthData_StaffTitle.Text = "Name: ";
                    break;
            }


            BuildStudentGrowthTable();
            //repeaterGrowthData.DataSource = ds.Tables[1];
            //repeaterGrowthData.DataBind();
        }

        protected void repeaterGrowthData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                ((Label)e.Item.FindControl("lblAggregateConcordant")).Text = _aggregateConcordant;
            }
        }

        protected void BuildStudentGrowthTable()
        {
            var table = new HtmlTable();
            var headerRow = new HtmlTableRow();

            table.Width = "100%";
            table.Attributes["class"] = "sgTable";

            //Add header row
            foreach(DataColumn column in _ds.Tables[1].Columns)
            {
                var headerCell = new HtmlTableCell();
                headerCell.Attributes["class"] = "sgTableHeadCell";
                headerCell.InnerHtml = column.ColumnName;
                headerRow.Cells.Add(headerCell);
            }
            table.Rows.Add(headerRow);
            
            //Add body rows
            foreach(DataRow row in _ds.Tables[1].Rows)
            {
                var tableRow = new HtmlTableRow();
                foreach(DataColumn column in _ds.Tables[1].Columns)
                {
                    var tableCell = new HtmlTableCell();
                    tableCell.Attributes["class"] = table.Rows.Count % 2 == 0 ? "" : "altTD";
                    tableCell.InnerHtml = row[column].ToString();
                    tableRow.Cells.Add(tableCell);
                }
                table.Rows.Add(tableRow);
            }

            if (_ds.Tables[1].Rows.Count > 1 || (_ds.Tables[1].Rows.Count > 0 && !_ds.Tables[1].Rows[0][0].ToString().Contains("N/A")))
            {

                //Add spacer row
                var spacerRow = new HtmlTableRow();
                var spacerCell = new HtmlTableCell();
                spacerCell.ColSpan = _ds.Tables[1].Columns.Count;
                spacerCell.InnerHtml = "&nbsp;";
                spacerCell.BgColor = "bababa";
                spacerRow.Cells.Add(spacerCell);
                table.Rows.Add(spacerRow);

                //Add aggregate concordant row
                var concordRow = new HtmlTableRow();
                var concordCell1 = new HtmlTableCell();
                var concordCell2 = new HtmlTableCell();
                concordCell1.ColSpan = _ds.Tables[1].Columns.Count - 1;
                concordCell2.InnerHtml = "Aggregate Concordant = <span class=\"normalText\">" + _aggregateConcordant + "</span>";
                concordCell2.Attributes["class"] = "sgTableHeadCell";
                concordRow.Cells.Add(concordCell1);
                concordRow.Cells.Add(concordCell2);
                table.Rows.Add(concordRow);
            }

            //Add table to panel
            studentGrowDataTable.Controls.Add(table);
        }
    }
}