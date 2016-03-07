using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Class
{
    public partial class ClassAttendance : TileControlBase
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            //THIS IS A LEVEL TILE, GET LEVEL AND LEVELID

            //var c = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");
            //if (c == null) return;
           
            lblClassTitle.Text = "Elements Class";
        }

        protected void ClassAttendanceClick(object sender, EventArgs e)
        {
            //if (Tile.DockClickMethod != null)
            //{
                //Tile.DockClickMethod(Tile.TileParms);
            //}
        }
    }
}