using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.School
{
    public partial class SchoolIdentification : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var s = (Thinkgate.Base.Classes.School)Tile.TileParms.GetParm("school");
            if (s == null) return;

            if(!UserHasPermission(Base.Enums.Permission.Tile_Identification_School))
            {
                Tile.ParentContainer.Visible = false;
            }

            lblName.Text = s.Name ?? " ";
            lblID.Text = s.SchoolID ?? " ";
            lblAbbreviation.Text = s.Abbreviation ?? " ";
            lblPhone.Text = s.Phone ?? " ";
            lblType.Text = s.Type ?? " ";
            lblCluster.Text = s.Cluster ?? " ";
            lblStudentCt.Text = s.StudentCount.ToString() ?? " ";
            lblTeacherCt.Text = s.TeacherCount.ToString() ?? " ";
            lblClassCt.Text = s.ClassCount.ToString() ?? " ";
            lblGrades.Text = s.Grades ?? " ";
        }
    }
}