using System;
using System.IO;
using Telerik.Web.UI;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Class
{
    public partial class ClassStandards : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            //var c = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");
            if (!IsPostBack)
            {
                PopulateSubjectDdl();
            
            }

            
        }


        private void PopulateSubjectDdl()
        {


        }
    }
}