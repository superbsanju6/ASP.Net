using System;
using Thinkgate.Classes;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Student
{
    public partial class StudentAttendance : TileControlBase
    {
        private const String _yearFilterKey = "SaGradeFilterIdx";
        private const String _termFilterKey = "SaSubjectFilter";

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
            }

            LoadResults();
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

        private void LoadResults()
        {
            var s = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");
            if (s == null) return;

            var year = ViewState[_yearFilterKey].ToString();
            var term = ViewState[_termFilterKey].ToString();

            //Bind 3 Charts

        }

        protected void cmbYear_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_yearFilterKey] = e.Value;            
            LoadResults();
        }

        protected void cmbTerm_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_termFilterKey] = e.Value;
            LoadResults();
        }

    }
}