using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Standpoint.Core.Utilities;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Controls.LCO
{
    public partial class StaffSearch : TileControlBase
    {
        public static Base.Enums.EntityTypes _level;
        public static int _levelID;
        protected bool _perm_Tab_Search_Staff;

        protected void Page_Load(object sender, EventArgs e)
        {
            /****************************************************************
             * if user does not have Tab_Search_Staff permissions, then hide
             * the search tab and the radPageView associated with it.
             * *************************************************************/
            if (!UserHasPermission(Permission.Tab_Search_Staff))
            {
                radPageViewStaffSearch.Visible = false;
                foreach (Telerik.Web.UI.RadTab tab in RadTabStrip2.Tabs)
                {
                    if (tab.Text == "Search")
                    {
                        tab.Visible = false;
                        break;
                    }
                }
            }

            /*******************************************************************
             * If user does not have Icon_Expanded_Staff permissions, then hide
             * the "Advanced Search" link via its surrounding div tag.
             * ****************************************************************/
            StaffSearch_DivAdvancedSearch.Visible = (UserHasPermission(Permission.Icon_ExpandedSearch_Staff));

            _level = SessionObject.LCOrole;
            _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            staffSearch_HiddenLevel.Value = _level.ToString();
            staffSearch_HiddenLevelID.Value = Standpoint.Core.Classes.Encryption.EncryptInt(_levelID);
            staffPieChartXmlHttpPanel.Value = string.Empty;

            DataTable countData;
            string chartSeriesItemName = string.Empty;
            switch (_level)
            {
                case Base.Enums.EntityTypes.LCOAdministrator:
                    countData = Base.Classes.LCO.GetStaffCounts();
                    chartSeriesItemName = "PortalName";
                    break;
                case Base.Enums.EntityTypes.District:
                    countData = Base.Classes.Staff.GetStateStaffCounts();
                    chartSeriesItemName = "loweredrolename";
                    break;
                default:
                    return;
            }

            staffCountChart.DataSource = countData;
            staffCountChart.ChartTitle.Visible = false;
            staffCountChart.DataBind();

            // Handle Legend Labels
            staffCountChart.Series.Clear();
            var nSeries = new ChartSeries();
            nSeries.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;

            foreach (DataRow dr in countData.Rows)
            {
                var value = DataIntegrity.ConvertToDouble(dr["StaffCount"]);
                var myItem = new ChartSeriesItem(value) { Name = dr[chartSeriesItemName].ToString() };
                if (value <= 0) myItem.Label.Visible = false;
                nSeries.Type = ChartSeriesType.Pie;
                nSeries.AddItem(myItem);
            }

            staffCountChart.Series.Add(nSeries);

            if (IsPostBack)
            {
                // Reloads previous search results if session object search text has a value
                if (staffSearchText_smallTile.Value == "Search by last name..." && SessionObject.TeacherSearchTxtPostBack_smallTile != null
                        && SessionObject.TeacherSearchTxtPostBack_smallTile.Length > 0)
                {
                    SearchStaffByLastName_Click(null, null);
                }

                return;
            }

            staffSearchMoreLink.Visible = false;
            staffSearchTileGrid.Visible = false;
            addNewStaff.Visible = UserHasPermission(Base.Enums.Permission.Create_Staff);
        }

        protected void SearchStaffByLastName_Click(object sender, EventArgs e)
        {
            // TODO: Analyze for performance on postback
            string searchVal = string.Empty;

            if (staffSearchText_smallTile.Value.Length < 1)
            {
                return;
            }



            if (staffSearchText_smallTile.Value.Length > 0 && staffSearchText_smallTile.Value != "Search by last name...")
            {
                searchVal = staffSearchText_smallTile.Value;

                // store text field value in the SessionObject
                SessionObject.TeacherSearchTxtPostBack_smallTile = staffSearchText_smallTile.Value;
            }
            else if (SessionObject.TeacherSearchTxtPostBack_smallTile != null && SessionObject.TeacherSearchTxtPostBack_smallTile.Length > 0)
            {
                searchVal = SessionObject.TeacherSearchTxtPostBack_smallTile;
            }

            if (searchVal.Length > 0)
            {
                DataTable dt;
                switch (_level)
                {
                    case Base.Enums.EntityTypes.LCOAdministrator:
                        dt = Base.Classes.LCO.SearchStaffByLastName(searchVal);
                        break;
                    case Base.Enums.EntityTypes.District:
                        dt = Base.Classes.Staff.SearchStateStaffByLastName(searchVal);
                        break;
                    default:
                        return;
                }

                if (dt.Rows.Count > 0)
                {
                    staffNoRecordsMsg.Visible = false;
                    staffSearchTileGrid.Visible = true;

                    dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "ID", "ID_Encrypted");

                    staffSearchTileGrid.DataSource = dt;
                    staffSearchTileGrid.DataBind();
                }
                else
                {
                    staffSearchTileGrid.Visible = false;
                    staffNoRecordsMsg.Visible = true;
                }
            }
        }

        protected void pieChart_Click(object sender, ChartClickEventArgs args)
        {
            //if (args.SeriesItem != null)
            //{
            //    SessionObject.TeacherSearchParms.DeleteParm("SchoolType");
            //    SessionObject.TeacherSearchParms.AddParm("SchoolType", args.SeriesItem.XValue.ToString());

            //    staffPieChartXmlHttpPanel.Attributes["level"] = staffSearch_HiddenLevel.Value;
            //    staffPieChartXmlHttpPanel.Attributes["levelID"] = staffSearch_HiddenLevelID.Value;
            //    staffPieChartXmlHttpPanel.Attributes["searchTileName"] = "staff";
            //    staffPieChartXmlHttpPanel.Attributes["controlURL"] = "../Controls/LCO/StaffSearch_Expanded.aspx";
            //    staffPieChartXmlHttpPanel.Value = "{\"EmptyString\":\"\"}";
            //    /*
            //        if (Tile.ParentContainer != null)
            //                Tile.ParentContainer.ExpandTile(Tile, "Expand");*/
            //}
        }

        protected void expandTile_Click(object sender, EventArgs e)
        {
            LinkButton link = (LinkButton)sender;

            if (link.ClientID == "studentSearchMoreLink")
            {
                SessionObject.TeacherSearchParms.DeleteParm("Teacher Name");
                SessionObject.TeacherSearchParms.AddParm("Teacher Name", staffSearchText_smallTile.Value);
            }

            if (Tile.ParentContainer != null)
            {
                Tile.ParentContainer.ExpandTile(Tile, "Expand");
            }
        }

        protected void staffSearchTileGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                var dataItem = (DataRowView) item.DataItem;
                var link = (Label)item.FindControl("staffNameLink");

                if (link != null)
                {
                    link.Text = dataItem["StaffName"].ToString();

                    link.Attributes["onclick"] = "window.open('Staff.aspx?xID=" + dataItem["ID_Encrypted"] + "'); return false;";
                    link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                    link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                    link.Attributes["style"] = "color:#00F; text-decoration:underline; cursor:pointer;";
                }
            }
        }
    }
}
