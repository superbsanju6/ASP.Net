using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Teacher
{
    public partial class Teachers : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            var c = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");
            if (c == null) return;
            c.LoadTeachers();
            //Load teacher table
            DataTable teachersTbl = new DataTable();

            teachersTbl.Columns.Add("ID");
            teachersTbl.Columns.Add("FirstName");
            teachersTbl.Columns.Add("LastName");
            teachersTbl.Columns.Add("IsPrimary");

            Parallel.ForEach(c.Teachers, t =>
            {
                teachersTbl.Rows.Add(t.PersonID, t.FirstName, t.LastName, t.IsPrimary);
            });

            //load list view
            teachersGrid.DataSource = Cryptography.EncryptDataTableColumn(teachersTbl, "ID", "ID_Encrypted", SessionObject.LoggedInUser.CipherKey);
            teachersGrid.DataBind();

            //load graphic view
            teachersGrid_GraphicalView.DataSource = teachersTbl;
            teachersGrid_GraphicalView.DataBind();
            //----------------------------------------------
        }


        protected void SetTeacherIcons(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                Image primaryTeacherIcon = (Image)item.FindControl("PrimaryTeacherIcon");
                Image DegreeIcon = (Image)item["DegreeIcon"].Controls[0];
                Image CertificationIcon = (Image)item["CertificationIcon"].Controls[0];
                Image SummaryIcon = (Image)item.FindControl("SummaryIcon");
                DataRowView itemDataRow = (DataRowView)item.DataItem;

                SummaryIcon.Visible = UserHasPermission(Base.Enums.Permission.Icon_Summary_Teacher);

                bool showPrimaryTeacherIcon = DataIntegrity.ConvertToBool(itemDataRow.Row.ItemArray[3]);
                bool showDegreeIcon = DataIntegrity.ConvertToBool(itemDataRow.Row.ItemArray[3]);
                bool showCertificationIcon = DataIntegrity.ConvertToBool(itemDataRow.Row.ItemArray[3]);

                if (primaryTeacherIcon != null && showPrimaryTeacherIcon)
                {
                    primaryTeacherIcon.ImageUrl = "~/Images/star_icon.png";
                    primaryTeacherIcon.ToolTip = "Primary teacher";
                }
                if (DegreeIcon != null && showDegreeIcon)
                {
                    DegreeIcon.ImageUrl = "~/Images/diploma_icon.jpg";
                }
                if (CertificationIcon != null && showCertificationIcon)
                {
                    CertificationIcon.ImageUrl = "~/Images/certificate_icon_small.jpg";
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void SetTeacherIcons_GraphicView(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                Image primaryTeacherIcon = (Image)item.FindControl("GraphicViewPrimaryTeacherIcon");
                Image DegreeIcon = (Image)item.FindControl("GraphicViewDegreeIcon");
                Image CertificationIcon = (Image)item.FindControl("GraphicViewCertificationIcon");
                Image SummaryIcon = (Image)item.FindControl("GraphicViewSummaryIcon");
                DataRowView itemDataRow = (DataRowView)item.DataItem;

                SummaryIcon.Visible = UserHasPermission(Base.Enums.Permission.Icon_Summary_Teacher);

                bool showPrimaryTeacherIcon = DataIntegrity.ConvertToBool(itemDataRow.Row.ItemArray[3]);
                bool showDegreeIcon = DataIntegrity.ConvertToBool(itemDataRow.Row.ItemArray[3]);
                bool showCertificationIcon = DataIntegrity.ConvertToBool(itemDataRow.Row.ItemArray[3]);

                if (primaryTeacherIcon != null && showPrimaryTeacherIcon)
                {
                    primaryTeacherIcon.ImageUrl = "~/Images/star_icon.png";
                    primaryTeacherIcon.ToolTip = "Primary teacher";
                }
                if (DegreeIcon != null && showDegreeIcon)
                {
                    DegreeIcon.ImageUrl = "~/Images/diploma_icon.jpg";
                }
                if (CertificationIcon != null && showCertificationIcon)
                {
                    CertificationIcon.ImageUrl = "~/Images/certificate_icon_small.jpg";
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }
    }
}