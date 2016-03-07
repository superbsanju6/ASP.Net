using System;
using Thinkgate.Classes;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Student
{
    public partial class StudentGrades : TileControlBase
    {
        private const String _yearFilterKey = "SgGradeFilterIdx";
        private const String _termFilterKey = "SgSubjectFilter";

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
            var oStudent = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");

            //TODO: Nead Real Data for Student Class Grades
            foreach (var c in oStudent.Classes)
            {
                var rnd = Standpoint.Core.Utilities.RandomsHelper.RandomIntBetween(1,4);
                switch(rnd)
                {
                    case 1:
                        c.StudentGrade = "A";
                        break;
                    case 2:
                        c.StudentGrade = "B";
                        break;
                    case 3:
                        c.StudentGrade = "C";
                        break;
                    case 4:
                        c.StudentGrade = "D";
                        break;
                }                
            }

            gridStudentGrades.DataSource = oStudent.Classes;
            gridStudentGrades.DataBind();
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