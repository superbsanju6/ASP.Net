using System;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Student
{
    public partial class StudentAccommodation : TileControlBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            /* Get our student object from TileParms collection. */
            //var oStudent = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");

            LoadAndBindSubjectLevel();
//            LoadAndBindGlobal();
        }

        private void LoadAndBindSubjectLevel()
        {
            var s = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");
            if (s == null) return;

            gridSubjectLevel.DataSource = s.StudentAccommodations;
            gridSubjectLevel.DataBind();

        }

        //private void LoadAndBindGlobal()
        //{
        //    //TODO: Need to load real discipline data for RefferalType
        //    var dt = new DataTable();
        //    dt.Columns.Add("ReferralType");
        //    dt.Columns.Add("WTD");
        //    dt.Columns.Add("MTD");
        //    dt.Columns.Add("YTD");

        //    dt.Rows.Add(new object[] { "Disruption", 3, 7, 19 });
        //    dt.Rows.Add(new object[] { "Disrespect", 0, 4, 7 });
        //    dt.Rows.Add(new object[] { "Fighting", 1, 1, 2 });
        //    dt.Rows.Add(new object[] { "Harassment", 0, 3, 3 });
        //    dt.Rows.Add(new object[] { "<b>Total</b>", 4, 15, 31 });

        //    gridReferralType.DataSource = dt;
        //    gridReferralType.DataBind();
        //}

    }
}