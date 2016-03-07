using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Charting.Styles;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Telerik.Charting;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;

namespace Thinkgate.Controls.School
{
    public partial class SchoolResults : TileControlBase
    {
        public static Base.Enums.EntityTypes _level;
        public static int _levelID;

        public static DataTable countData;
        private AsyncTaskDelegate asyncTaskDelegate;

        // Create delegate. 
        protected delegate void AsyncTaskDelegate();

        private void ExecuteAsyncTask()
        {
            // asynchronous task.
            countData = Base.Classes.School.GetSchoolCounts(_level, _levelID, SessionObject.GlobalInputs, 0);
        }

        // Define the method that will get called to 
        // start the asynchronous task. 
        private IAsyncResult OnBegin(object sender, EventArgs e,
            AsyncCallback cb, object extraData)
        {
            asyncTaskDelegate = new AsyncTaskDelegate(ExecuteAsyncTask);
            IAsyncResult result = asyncTaskDelegate.BeginInvoke(cb, extraData);

            return result;
        }

        // Define the method that will get called when 
        // the asynchronous task is ended. 
        private void OnEnd(IAsyncResult ar)
        {
            asyncTaskDelegate.EndInvoke(ar);

            schoolCountChart.DataSource = countData;
            schoolCountChart.ChartTitle.Visible = false;
            schoolCountChart.DataBind();

            var parms = Base.Classes.DistrictParms.LoadDistrictParms();
            if (parms.TilePieCharts_Clickable.ToLower() == "yes") schoolCountChart.Click += pieChart_Click;

            //Handle Legend Labels
            schoolCountChart.Series.Clear();
            var nSeries = new ChartSeries();

            schoolCountChart.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            schoolCountChart.PlotArea.Appearance.FillStyle.FillType = FillType.Solid;
            schoolCountChart.PlotArea.Appearance.Border.Color = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            schoolCountChart.PlotArea.XAxis.Appearance.TextAppearance.TextProperties.Font =  new System.Drawing.Font("Arial", 8);

            schoolCountChart.PlotArea.YAxis.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial",8); 

            nSeries.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.Black;
            nSeries.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);        

            nSeries.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            var count = 0;

            foreach (DataRow dr in countData.Rows)
            {
                var value = DataIntegrity.ConvertToDouble(dr["TypeCount"]);
                var myItem = new ChartSeriesItem(value) { Name = dr["SchoolType"].ToString() };
                myItem.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml(StyleController.GetPieChartColor(dr["SchoolType"].ToString(), count++));
                myItem.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;

                if (value <= 0) myItem.Label.Visible = false;

                nSeries.Type = ChartSeriesType.Pie;
                nSeries.AddItem(myItem);
            }
            schoolCountChart.Series.Add(nSeries);

            if (IsPostBack)
            {
                //Reloads previous search results if session object search text has a value
                if (schoolsSearchText_smallTile.Value == "Search by school name..." && SessionObject.SchoolSearchTxtPostBack_smallTile != null
                        && SessionObject.SchoolSearchTxtPostBack_smallTile.Length > 0)
                    SearchSchoolsByName_Click(null, null);

                return;
            }
            // Not post back.
            else
            {
                if (!UserHasPermission(Permission.Tab_Search_Schools))
                {
                    RadTab tab = tabs.FindTabByText("Search", true);
                    if (tab != null)
                        tab.Visible = false;
                }
            }

            schoolSearchMoreLink.Visible = false;
            schoolsSearchTileGrid.Visible = false;
            addNewSchool.Visible = UserHasPermission(Base.Enums.Permission.Create_School);
            schoolAdvancedSearchLink_smallTile.Visible = UserHasPermission(Base.Enums.Permission.Icon_ExpandedSearch_Schools);
        }

        // Define the method that will get called if the task 
        // is not completed within the asynchronous timeout interval. 
        private void OnTimeout(IAsyncResult ar)
        {
            // code that will get called if the task 
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            schoolsearch_hiddenLevel.Value = _level.ToString();
            schoolsearch_hiddenLevelID.Value = Standpoint.Core.Classes.Encryption.EncryptInt(_levelID);
            schoolPieChartXmlHttpPanel.Value = "";

            if (_levelID <= 0) return;

            PageAsyncTask pageAsyncTask = new PageAsyncTask(OnBegin, OnEnd, OnTimeout, "SchoolResults", true);

            // Register the asynchronous task.
            Page.RegisterAsyncTask(pageAsyncTask);

            // Execute the register asynchronous task.
            Page.ExecuteRegisteredAsyncTasks();
        }

        protected void SearchSchoolsByName_Click(object sender, EventArgs e)
        {

            //TODO: Analyze for performance on postback
            String searchVal = "";

            if (schoolsSearchText_smallTile.Value.Length > 0 && schoolsSearchText_smallTile.Value != "Search by school name...")
            {
                searchVal = schoolsSearchText_smallTile.Value;

                //store text field value in the SessionObject
                SessionObject.SchoolSearchTxtPostBack_smallTile = schoolsSearchText_smallTile.Value;
            }
            else if (SessionObject.SchoolSearchTxtPostBack_smallTile != null && SessionObject.SchoolSearchTxtPostBack_smallTile.Length > 0)
            {
                searchVal = SessionObject.SchoolSearchTxtPostBack_smallTile;
            }

            if (searchVal.Length > 0)
            {
                DataTable dt = Base.Classes.School.SearchSchoolsByName(_levelID, searchVal, _level);
                int schoolRecordCount = 0;

                if (dt.Rows.Count > 0)
                {
                    schoolNoRecordsMsg.Visible = false;
                    schoolsSearchTileGrid.Visible = true;
                    schoolRecordCount = DataIntegrity.ConvertToInt(dt.Select()[0].ItemArray[7]);

                    dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "ID", "ID_Encrypted");

                    schoolsSearchTileGrid.DataSource = dt;
                    schoolsSearchTileGrid.DataBind();
                }
                else
                {
                    schoolsSearchTileGrid.Visible = false;
                    schoolNoRecordsMsg.Visible = true;
                }

                if (schoolRecordCount > 100)
                {
                    schoolSearchMoreLink.Visible = true;
                }
            }
        }

        protected void pieChart_Click(object sender, ChartClickEventArgs args)
        {
            if (args.SeriesItem != null)
            {
                SessionObject.SchoolSearchParms.DeleteParm("SchoolType");
                SessionObject.SchoolSearchParms.AddParm("SchoolType", args.SeriesItem.Name.ToString());

                schoolPieChartXmlHttpPanel.Attributes["level"] = schoolsearch_hiddenLevel.Value;
                schoolPieChartXmlHttpPanel.Attributes["levelID"] = schoolsearch_hiddenLevelID.Value;
                schoolPieChartXmlHttpPanel.Attributes["searchTileName"] = "schools";
                schoolPieChartXmlHttpPanel.Attributes["controlURL"] = "../Controls/School/SchoolSearch_Expanded.aspx";
                schoolPieChartXmlHttpPanel.Value = "{\"EmptyString\":\"\"}";
                /*
                if (Tile.ParentContainer != null)
                    Tile.ParentContainer.ExpandTile(Tile, "Expand");*/
            }
        }

        protected void expandTile_Click(object sender, EventArgs e)
        {
            LinkButton link = (LinkButton)sender;

            if (link.ClientID == "schoolSearchMoreLink")
            {
                SessionObject.SchoolSearchParms.DeleteParm("School Name");
                SessionObject.SchoolSearchParms.AddParm("School Name", schoolsSearchText_smallTile.Value.ToString());
            }

            if (Tile.ParentContainer != null)
                Tile.ParentContainer.ExpandTile(Tile, "Expand");
        }

        protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridPagerItem)
            {
                GridPagerItem pager = (GridPagerItem)e.Item;

                Label lbl = (Label)pager.FindControl("ChangePageSizeLabel");

                lbl.Visible = false;

                RadComboBox combo = (RadComboBox)pager.FindControl("PageSizeComboBox");

                combo.Visible = false;
            }
        } 
    }
}