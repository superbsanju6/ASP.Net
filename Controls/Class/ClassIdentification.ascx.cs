using System;
using Thinkgate.Classes;


namespace Thinkgate.Controls.Class
{
    public partial class ClassIdentification : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var c = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");
            if (c == null) return;

            lblSchool.Text = c.School != null ? c.School.Name : " ";
            lblGrade.Text = c.Grade != null ? c.Grade.DisplayText: " ";
            lblSubject.Text = c.Subject != null ? c.Subject.DisplayText : " ";
            lblCourse.Text = c.Course.CourseName;
            lblSection.Text = c.Section;
            lblYear.Text = c.Year;
            lblSemester.Text = c.Semester;
            lblPeriod.Text = c.Period.ToString();
            lblBlock.Text = c.Block;
            lblRetainOnResync.Text = c.RetainOnResync ? "Yes" : "No";

            var clone = Tile.TileParms.GetParm("clonedClassID");
            if (clone != null && (int)clone == c.ID) tdCloneNotification.Style["display"] = "block";
        }
    }
}