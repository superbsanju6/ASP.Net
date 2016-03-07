using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using Standpoint.Core.Utilities;
using Telerik.Charting;
using Telerik.Charting.Styles;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes.Data;

namespace Thinkgate.Controls.Teacher
{
    public partial class TeacherSearch : TileControlBase
    {
        public static Base.Enums.EntityTypes _level;
        public static int _levelID;
        public static string _levelType;
        protected bool _perm_Tab_Search_Staff;
        public static DataTable _countData;

#region Asynchronous Tasks
        private void LoadTeacherCounts()
        {
            // asynchronous task.
            switch (_level)
            {
                case Base.Enums.EntityTypes.District:
                    _countData = Thinkgate.Base.Classes.Data.TeacherDB.GetTeacherCountsForDistrict(SessionObject.GlobalInputs);
                    break;
                case Base.Enums.EntityTypes.School:                    
                    if (_levelType == "High School")
                    {
                        _countData = Base.Classes.Teacher.GetTeacherCountsForSchoolBySubject(_levelID, SessionObject.GlobalInputs);
                    }
                    else
                    {
                        _countData = Thinkgate.Base.Classes.Data.TeacherDB.GetTeacherCountsForSchool(_levelID, SessionObject.GlobalInputs, 0);
                    }
                    break;
                default:
                    return;
            }
        }

        private void LoadDistrictParmsForPie()
        {
            var parms = Base.Classes.DistrictParms.LoadDistrictParms(SessionObject.GlobalInputs);
            if (parms.TilePieCharts_Clickable.ToLower() == "yes") teacherCountChart.Click += pieChart_Click;
        }
#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            /****************************************************************
             * if user does not have Tab_Search_Staff permissions, then hide
             * the search tab and the radPageView associated with it.
             * *************************************************************/
            if (!UserHasPermission(Permission.Tab_Search_Staff))
            {
                radPageViewTeacherSearch.Visible = false;
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
            TeacherSearch_DivAdvancedSearch.Visible = (UserHasPermission(Permission.Icon_ExpandedSearch_Staff));

            _level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));
            _levelType = Tile.TileParms.GetParm("levelType") != null ? Tile.TileParms.GetParm("levelType").ToString() : String.Empty;

            teacherSearch_HiddenLevel.Value = _level.ToString();
            teacherSearch_HiddenLevelID.Value = Standpoint.Core.Classes.Encryption.EncryptInt(_levelID);
            teacherPieChartXmlHttpPanel.Value = string.Empty;

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(LoadTeacherCounts));
            taskList.Add(new AsyncPageTask(LoadDistrictParmsForPie));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "TeacherSearch", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            LoadPieChart();

            if (IsPostBack)
            {
                // Reloads previous search results if session object search text has a value
                if (teachersSearchText_smallTile.Value == "Search by last name..." && !string.IsNullOrEmpty(SessionObject.TeacherSearchTxtPostBack_smallTile)
                        && SessionObject.TeacherSearchTxtPostBack_smallTile.Length > 0)
                {
                    SearchTeachersByLastName_Click(null, null);
                }

                return;
            }

            teacherSearchMoreLink.Visible = false;
            teachersSearchTileGrid.Visible = false;
            addNewStaff.Visible = UserHasPermission(Base.Enums.Permission.Create_Staff);
        }

        protected void LoadPieChart()
        {
            teacherCountChart.DataSource = _countData;
            teacherCountChart.DataBind();

            // Handle Legend Labels
            teacherCountChart.Series.Clear();
            var nSeries = new ChartSeries();

            teacherCountChart.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            teacherCountChart.PlotArea.Appearance.FillStyle.FillType = FillType.Solid;
            teacherCountChart.PlotArea.Appearance.Border.Color = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");

            teacherCountChart.PlotArea.XAxis.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);
            teacherCountChart.PlotArea.YAxis.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);

            nSeries.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.Black;
            nSeries.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);     

           

            nSeries.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            var count = 0;

            foreach (DataRow dr in _countData.Rows)
            {
                var value = DataIntegrity.ConvertToDouble(dr["TeacherCount"]);
                var myItem = new ChartSeriesItem(value) { Name = dr["Label"].ToString() };
                myItem.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml(StyleController.GetPieChartColor(dr["Label"].ToString(), count++));
                myItem.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;

                if (value <= 0) myItem.Label.Visible = false;
                nSeries.Type = ChartSeriesType.Pie;
                nSeries.AddItem(myItem);
            }

            teacherCountChart.Series.Add(nSeries);
        }

        protected void SearchTeachersByLastName_Click(object sender, EventArgs e)
        {
            // TODO: Analyze for performance on postback
            string searchVal = string.Empty;

            if (teachersSearchText_smallTile.Value.Length > 0 && teachersSearchText_smallTile.Value != "Search by last name...")
            {
                searchVal = teachersSearchText_smallTile.Value;

                // store text field value in the SessionObject
                SessionObject.TeacherSearchTxtPostBack_smallTile = teachersSearchText_smallTile.Value;
            }
            else if (SessionObject.TeacherSearchTxtPostBack_smallTile != null && SessionObject.TeacherSearchTxtPostBack_smallTile.Length > 0)
            {
                searchVal = SessionObject.TeacherSearchTxtPostBack_smallTile;
            }

            if (searchVal.Length > 0)
            {
                DataTable dt = Thinkgate.Base.Classes.Data.TeacherDB.SearchTeachersByLastName(_levelID, searchVal, _level);
                int teacherRecordCount = 0;

                if (dt.Rows.Count > 0)
                {
                    teacherNoRecordsMsg.Visible = false;
                    teachersSearchTileGrid.Visible = true;
                    teacherRecordCount = DataIntegrity.ConvertToInt(dt.Select()[0].ItemArray[2]);

                    dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "ID", "ID_Encrypted");

                    teachersSearchTileGrid.DataSource = dt;
                    teachersSearchTileGrid.DataBind();
                }
                else
                {
                    teachersSearchTileGrid.Visible = false;
                    teacherNoRecordsMsg.Visible = true;
                }

                if (teacherRecordCount > 100)
                {
                    teacherSearchMoreLink.Visible = true;
                }
            }
        }

        protected void pieChart_Click(object sender, ChartClickEventArgs args)
        {
            if (args.SeriesItem != null)
            {
                SessionObject.TeacherSearchParms.DeleteParm("SchoolType");
                SessionObject.TeacherSearchParms.AddParm("SchoolType", args.SeriesItem.Name.ToString());

                teacherPieChartXmlHttpPanel.Attributes["level"] = teacherSearch_HiddenLevel.Value;
                teacherPieChartXmlHttpPanel.Attributes["levelID"] = teacherSearch_HiddenLevelID.Value;
                teacherPieChartXmlHttpPanel.Attributes["searchTileName"] = "teachers";
                teacherPieChartXmlHttpPanel.Attributes["controlURL"] = "../Controls/Teacher/TeacherSearch_Expanded.aspx";
                teacherPieChartXmlHttpPanel.Value = "{\"EmptyString\":\"\"}";
                /*
                    if (Tile.ParentContainer != null)
                            Tile.ParentContainer.ExpandTile(Tile, "Expand");*/
            }
        }

        protected void expandTile_Click(object sender, EventArgs e)
        {
            LinkButton link = (LinkButton)sender;

            if (link.ClientID == "studentSearchMoreLink")
            {
                SessionObject.TeacherSearchParms.DeleteParm("Teacher Name");
                SessionObject.TeacherSearchParms.AddParm("Teacher Name", teachersSearchText_smallTile.Value);
            }

            if (Tile.ParentContainer != null)
            {
                Tile.ParentContainer.ExpandTile(Tile, "Expand");
            }
        }

        protected void teachersSearchTileGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                var dataItem = (DataRowView) item.DataItem;
                var link = (Label)item.FindControl("teacherNameLink");

                if(link != null)
                {
                    link.Text = dataItem["TeacherName"].ToString();

                    if (dataItem["UserType"].ToString().ToLower() == "teacher" && dataItem["HasClasses"].ToString() == "true")
                    {
                        if (UserHasPermission(Base.Enums.Permission.Hyperlink_Teacher))
                        {
                            link.Attributes["onclick"] = "window.open('Teacher.aspx?childPage=yes&xID=" + dataItem["ID_Encrypted"] + "'); return false;";
                            link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                            link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                            link.Attributes["style"] = "color:#00F; text-decoration:underline; cursor:pointer;";
                        }
                        else
                        {
                            link.Attributes["style"] = "color:#3B3B3B; text-decoration:none;";
                        }
                    }
                    else
                    {
                        link.Attributes["onclick"] = "window.open('Staff.aspx?xID=" + dataItem["ID_Encrypted"] + "'); return false;";
                        link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                        link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                        link.Attributes["style"] = "color:#00F; text-decoration:underline; cursor:pointer;";
                    }
                }
            }
        }
    }
}
