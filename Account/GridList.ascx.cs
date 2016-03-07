using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;

namespace Thinkgate.Account
{
    public partial class GridList : System.Web.UI.UserControl
    {
        Thinkgate.Base.Classes.Administration Admin = new Thinkgate.Base.Classes.Administration();

        public int GridScrollHeight { get; set; }
        public int GridWidth { get; set; }
        public int GridMarginTop { get; set; }
        public ListMode Mode { get; set; }
            
        public enum ListMode
        {
            None,
            Permissions,
            Roles,
            Users,
            Schools,
            PricingModules,
            Grades,
            Subjects,
            Periods
        }

        public GridList(ListMode mode = ListMode.None)
        {
            GridScrollHeight = 600;
            GridWidth = 500;
            GridMarginTop = 10;
            Mode = mode;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFormatting();
                LoadList(Mode);
            }
        }

        public void DataBind(ListMode mode = ListMode.None)
        {
            Mode = mode;
            LoadFormatting();
            LoadList(Mode);
        }

        private void LoadFormatting()
        {
            grdList.ClientSettings.Scrolling.ScrollHeight = GridScrollHeight;
            grdList.Style.Add(HtmlTextWriterStyle.Width, GridWidth.ToString() + "px");
            grdList.Style.Add(HtmlTextWriterStyle.MarginTop, GridMarginTop.ToString() + "px");
        }

        private void LoadList(ListMode mode)
        {
            switch (mode)
            {
                case ListMode.Permissions:
                    grdList.DataSource = Admin.GetPermissionsAll().Select(x => x.PermissionName);
                    break;
                case ListMode.Roles:
                    grdList.DataSource = Admin.GetRolesAll().Select(x => x.RoleName);
                    break;
                case ListMode.Users:
                    break;
                case ListMode.Schools:
                    grdList.DataSource = Admin.GetSchoolsAll().Select(x => x.Name);
                    break;
                case ListMode.PricingModules:
                    grdList.DataSource = Admin.GetPricingModules().Select(x => x.Name);
                    break;
                case ListMode.Grades:
                    grdList.DataSource = Admin.GetGradesAll().Select(x => x.DisplayText);
                    break;
                case ListMode.Subjects:
                    grdList.DataSource = Admin.GetSubjectsAll().Select(x => x.DisplayText);
                    break;
                case ListMode.Periods:
                    grdList.DataSource = Admin.GetPeriodsAll().Select(x => x.PeriodName);
                    break;
                default:
                    grdList.DataSource = null;
                    break;
            }
            grdList.DataBind();
        }

        protected void grdList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;
                switch (Mode)
                {
                    case ListMode.Permissions:
                        header["Item"].Text = "Permission Name";
                        break;
                    case ListMode.Roles:
                        header["Item"].Text = "Role Name";
                        break;
                    case ListMode.Users:
                        break;
                    case ListMode.Schools:
                        header["Item"].Text = "School Name";
                        break;
                    case ListMode.PricingModules:
                        header["Item"].Text = "Pricing Module Name";
                        break;
                    case ListMode.Grades:
                        header["Item"].Text = "Grades";
                        break;
                    case ListMode.Subjects:
                        header["Item"].Text = "Subjects";
                        break;
                    case ListMode.Periods:
                        header["Item"].Text = "Periods";
                        break;
                    default:
                        header["Item"].Text = "Item";
                        break;
                }
            }
        }
    }
}