using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.District
{
    public partial class DistrictInformation : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            Base.Classes.District district = (Base.Classes.District)Tile.TileParms.GetParm("district");
            Thinkgate.Base.Enums.EntityTypes level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            int levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            lblDistrict.Text = district.DistrictName;
            lblClientID.Text = district.ClientID;
            lblSchools.Text = district.SchoolCount.ToString();
            lblTeachers.Text = district.TeacherCount.ToString();
            lblStudents.Text = district.StudentCount.ToString();
        }
    }
}