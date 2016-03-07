using System;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
namespace Thinkgate.Controls.Student
{
    public partial class StudentPortfolio : TileControlBase
    {
        private const String _yearFilterKey = "SpGradeFilterIdx";
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create the initial viewstate values.
            if (ViewState[_yearFilterKey] == null)
            {
                ViewState.Add(_yearFilterKey, "All");
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
        }


        private void LoadData()
        {
            /* Get our student object from TileParms collection. */
            var oStudent = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");

            gridPortfolio.DataSource = Base.Classes.Data.StudentDB.GetPortfolioDT();
            gridPortfolio.DataBind();
        }

        protected void gridPortfolio_ItemDataBound(object sender, GridItemEventArgs e)
        {
        }

        protected void cmbYear_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_yearFilterKey] = e.Value;
            LoadData();
        }
    }
}