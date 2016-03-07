using System;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Student
{
    public partial class StudentDiscipline : TileControlBase
    {
        private const String _yearFilterKey = "SdGradeFilterIdx";
        private const String _termFilterKey = "SdSubjectFilter";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Create the initial viewstate values.
            if (ViewState[_yearFilterKey] == null)
            {
                ViewState.Add(_yearFilterKey, "All");
                ViewState.Add(_termFilterKey, "All");
            }

            if (!IsPostBack)
            {
                BuildFilters();
                LoadData();
            }
        }

        private void BuildFilters()
        {
            cmbYear.Items.Clear();
            cmbYear.Items.Add(new RadComboBoxItem("Year", "All"));
            cmbYear.Items.Add(new RadComboBoxItem("11-12", "11-12"));
            cmbYear.Items.Add(new RadComboBoxItem("10-11", "10-11"));

            cmbTerm.Items.Clear();
            cmbTerm.Items.Add(new RadComboBoxItem("Term", "All"));
            cmbTerm.Items.Add(new RadComboBoxItem("Term 1", "1"));
            cmbTerm.Items.Add(new RadComboBoxItem("Term 2", "2"));
            cmbTerm.Items.Add(new RadComboBoxItem("Term 3", "3"));
            cmbTerm.Items.Add(new RadComboBoxItem("Term 4", "4"));
        }


        private void LoadData()
        {
            /* Get our student object from TileParms collection. */
            //var oStudent = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");
            
            LoadAndBindGradeLevel();
            LoadAndBindRefferalType();
            LoadAndBindConsequences();
        }

        private void LoadAndBindGradeLevel()
        {
            //TODO: Need to load real discipline data for grade level
            var dt = new DataTable();
            dt.Columns.Add("Grade");
            dt.Columns.Add("WTD");
            dt.Columns.Add("MTD");
            dt.Columns.Add("YTD");

            dt.Rows.Add(new object[] { "9th Grade", 3, 12, 20 });
            dt.Rows.Add(new object[] { "10th Grade", 0, 2, 14 });
            dt.Rows.Add(new object[] { "11th Grade", 3, 4, 15 });
            dt.Rows.Add(new object[] { "12th Grade", 2, 4, 15 });
            dt.Rows.Add(new object[] { "<b>Total</b>", 8, 22, 64 });

            gridGradeLevel.DataSource = dt;
            gridGradeLevel.DataBind();
        }

        private void LoadAndBindRefferalType()
        {
            //TODO: Need to load real discipline data for RefferalType
            var dt = new DataTable();
            dt.Columns.Add("ReferralType");
            dt.Columns.Add("WTD");
            dt.Columns.Add("MTD");
            dt.Columns.Add("YTD");

            dt.Rows.Add(new object[] { "Disruption", 3, 7, 19 });
            dt.Rows.Add(new object[] { "Disrespect", 0, 4, 7 });
            dt.Rows.Add(new object[] { "Fighting", 1, 1, 2 });
            dt.Rows.Add(new object[] { "Harassment", 0, 3, 3 });
            dt.Rows.Add(new object[] { "<b>Total</b>", 4, 15, 31 });

            gridReferralType.DataSource = dt;
            gridReferralType.DataBind();
        }

        private void LoadAndBindConsequences()
        {
            //TODO: Need to load real discipline data for Consequences
            var dt = new DataTable();
            dt.Columns.Add("Consequence");
            dt.Columns.Add("WTD");
            dt.Columns.Add("MTD");
            dt.Columns.Add("YTD");

            dt.Rows.Add(new object[] { "Bus Suspension", 3, 5, 12 });
            dt.Rows.Add(new object[] { "Detention", 1, 1, 1 });
            dt.Rows.Add(new object[] { "In School Suspension", 0, 0, 2 });
            dt.Rows.Add(new object[] { "Out of School Suspension", 0, 0, 1 });
            dt.Rows.Add(new object[] { "<b>Total</b>", 4, 6, 16 });

            gridConsequences.DataSource = dt;
            gridConsequences.DataBind();
        }

        protected void grid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var row = (DataRowView)((GridDataItem)e.Item).DataItem;
                
                //Color code based on thresholds
                ColorCodeCell(e.Item.Cells[3], DataIntegrity.ConvertToInt(row["WTD"]));
                ColorCodeCell(e.Item.Cells[4], DataIntegrity.ConvertToInt(row["MTD"]));
                ColorCodeCell(e.Item.Cells[5], DataIntegrity.ConvertToInt(row["YTD"]));
            } 
        }

        private void ColorCodeCell(TableCell cell, int value)
        {
            if (value > 5) cell.BackColor = System.Drawing.Color.Red;
            if (value == 0) cell.BackColor = System.Drawing.Color.Green;
        }

        protected void cmbYear_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_yearFilterKey] = e.Value;
            LoadData();
        }

        protected void cmbTerm_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_termFilterKey] = e.Value;
            LoadData();
        }
    }
}