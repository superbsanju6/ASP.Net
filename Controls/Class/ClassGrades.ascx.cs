using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Class
{
    public partial class ClassGrades : TileControlBase
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            //TILE LIVES ON STUDENT AS WELL... CAN'T PASS 'CLASS' : USE LEVEL
            //Would notify somebody but there is no ownership in the E3TileList.

            //var c = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");
           // if (c == null) return;
            //lblClassTitle.Text = c.ClassName;
        }

        protected void ClassGradesClick(object sender, EventArgs e)
        {
           // if (Tile.DockClickMethod != null)
            //{
            //    Tile.DockClickMethod(Tile.TileParms);
           // }
        }
    }
}