namespace Thinkgate.Controls.Banner
{
    using System;
    using System.Web.UI;

    using Telerik.Web.UI;
    using Thinkgate.Classes;
    using Thinkgate.Interfaces;

    public partial class ObjectScreenBanner : System.Web.UI.UserControl, IBannerControl
    {
        public void AddMenu(Banner.ContextMenu contextMenu, RadContextMenu menu)
        {
            switch (contextMenu)
            {
                case Banner.ContextMenu.Actions:
                    menu.ClientIDMode = ClientIDMode.Static;
                    menu.ID = "RadContextMenuActions";
                    menu.Skin = "Office2010Silver";
                    menu.EnableRoundedCorners = true;
                    menu.EnableShadows = true;

                    PlaceHolderActions.Controls.Add(menu);
                    break;
            }
        }

        public void HideContextMenu(Banner.ContextMenu contextMenu)
        {
            switch (contextMenu)
            {
                case Banner.ContextMenu.Actions:
                    LinkButtonActions.Visible = false;
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
