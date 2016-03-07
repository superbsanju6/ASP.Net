using System;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.IO;

namespace Thinkgate.Controls.Class
{
    public partial class ClassRoster : TileControlBase
    {                
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            var c = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");
            if (c == null) return;

            DataTable rosterTbl = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(Thinkgate.Base.Classes.Class.StudentsDataTable(c.ID, 0), "ID", "ID_Encrypted");

            //load list view
            rosterGrid.DataSource = rosterTbl;
            rosterGrid.DataBind();

            //load graphic view
            rosterGrid_GraphicView.DataSource = rosterTbl;
            rosterGrid_GraphicView.DataBind();
        }

        protected void ClassRosterClick(object sender, EventArgs e)
        {
            if (Tile.DockClickMethod != null)
            {
                Tile.DockClickMethod(Tile.TileParms);
            }
        }

        protected void SetRTIImage(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                Image rtiImage = (Image)item["RTIImage"].Controls[0];
                Image SummaryIcon = (Image)item.FindControl("SummaryIcon");
                SummaryIcon.Visible = UserHasPermission(Base.Enums.Permission.Icon_Summary_Student);
                DataRowView itemDataRow = (DataRowView)item.DataItem;
                string rtiImageURL = (String.IsNullOrEmpty(itemDataRow.Row["rtiimg"].ToString()) ? "" : itemDataRow.Row["rtiimg"].ToString());

                if (rtiImage != null && rtiImageURL.Length > 0 && rtiImageURL.IndexOf("blank") == -1)
                {
                    rtiImage.ImageUrl = "~/Images/" + rtiImageURL;
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void SetRTIImage_GraphicView(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                Image rtiImage = (Image)item.FindControl("GraphicViewRTIImage");
                Image studentImage = (Image)item.FindControl("StudentPhoto");
                Image SummaryIcon = (Image)item.FindControl("GraphicViewSummaryIcon");
                SummaryIcon.Visible = UserHasPermission(Base.Enums.Permission.Icon_Summary_Student);
                DataRowView itemDataRow = (DataRowView)item.DataItem;

                //RTI Image Path
                string rtiImageURL = (String.IsNullOrEmpty(itemDataRow.Row["rtiimg"].ToString()) ? "" : itemDataRow.Row["rtiimg"].ToString());

                //Student Image Path
                string imgName = itemDataRow.Row["picture"].ToString();

                //Default if it's null in the DB, or missing from the web server path
                if (String.IsNullOrEmpty(imgName) || !File.Exists(Server.MapPath(AppSettings.ProfileImageStudentWebPath + "/" + imgName)))
                {
                    studentImage.ImageUrl = ResolveUrl("~/Images/new/male_student.png");
                }
                else studentImage.ImageUrl = AppSettings.ProfileImageStudentWebPath + '/' + imgName;

                if (rtiImage != null && rtiImageURL.Length > 0 && rtiImageURL.IndexOf("blank") == -1)
                {
                    rtiImage.ImageUrl = "~/Images/" + rtiImageURL;
                }

            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }
    }
}