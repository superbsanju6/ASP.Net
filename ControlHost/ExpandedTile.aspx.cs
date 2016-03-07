using System;
using Thinkgate.Classes;

namespace Thinkgate.ControlHost
{
    public partial class ExpandedTile : BasePage
    {
        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
           
            //TODO: Need to look into when to load and not load the tile again. 
            //Right now this line is commented out so it will load all the time because there is a bug preventing the alternate view from displaying.
            //if (!IsPostBack) LoadControlToPlaceHolder(); 

            LoadControlToPlaceHolder();
        }

        protected void Page_Load(object sender, EventArgs e)
        {                        
        }

        private void LoadControlToPlaceHolder()
        {
            if (SessionObject.ExpandingTile_ControlTile == null) return;

            var widget = SessionObject.ExpandingTileMode == "Expand"
                       ? LoadControl(SessionObject.ExpandingTile_ControlTile.ExpandedControlPath)
                       : LoadControl(SessionObject.ExpandingTile_ControlTile.EditControlPath);

            ((TileControlBase)widget).Tile = SessionObject.ExpandingTile_ControlTile;
            
            TilePlaceHolder.Controls.Clear();
            TilePlaceHolder.Controls.Add(widget);
        }

    }
}