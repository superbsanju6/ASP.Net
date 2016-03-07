namespace Thinkgate.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Thinkgate.Classes;

    public partial class Folders : System.Web.UI.UserControl
    {
        public int NumberOfFolders { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void BindFolderList(List<Folder> folders)
        {
            NumberOfFolders = folders.Count;

            foldersRepeater.DataSource = folders;
            foldersRepeater.DataBind();
        }

        public void HighlightFolder(string folderName)
        {
            foreach (RepeaterItem item in foldersRepeater.Items)
            {
                var img = item.FindControl("folderIcon");
                if (img == null)
                {
                    return;
                }

                var imageButton = (System.Web.UI.HtmlControls.HtmlAnchor)img;
                if (imageButton.Attributes["CommandArgument"] == folderName)
                {
                    imageButton.Attributes["selected"] = "selected";
                }
                else
                {
                    imageButton.Attributes["selected"] = string.Empty;
                }
            }
        }

        protected void foldersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dividerDiv = e.Item.FindControl("folderDividerDiv");
                if (dividerDiv == null)
                {
                    return;
                }

                if (!(e.Item.ItemIndex < NumberOfFolders - 1))
                {
                    dividerDiv.Visible = false;
                }

                var img = e.Item.FindControl("folderIcon");
                if (img == null)
                {
                    return;
                }

                var imageButton = (System.Web.UI.HtmlControls.HtmlAnchor)img;

                imageButton.Attributes["onclick"] = "folderClick('" + DataBinder.Eval(e.Item.DataItem, "Text") + "')";
            }
        }
    }
}
