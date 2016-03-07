using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Charting.Styles;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Classes.Data;
using Telerik.Charting;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Class
{
    public partial class ClassSearch : TileControlBase
    {
        public static DataTable countData;
        private EntityTypes level;
        private int levelID;

#region AsynchMethods
        private void GetClassCounts()
        {
            countData = Base.Classes.Class.GetClassCounts(level, levelID, SessionObject.GlobalInputs, 0);
        }

        private void LoadDistrictParms()
        {
            var parms = Base.Classes.DistrictParms.LoadDistrictParms(SessionObject.GlobalInputs);
            if (parms.TilePieCharts_Clickable.ToLower() == "yes") classCountChart.Click += pieChart_Click;
        }
#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(GetClassCounts));
            taskList.Add(new AsyncPageTask(LoadDistrictParms));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "ClassSearch", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            classCountChart.DataSource = countData;
            classCountChart.ChartTitle.Visible = false;
            classCountChart.DataBind();

            //Handle Legend Labels
            classCountChart.Series.Clear();
            var nSeries = new ChartSeries();

            classCountChart.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            classCountChart.PlotArea.Appearance.FillStyle.FillType = FillType.Solid;
            classCountChart.PlotArea.Appearance.Border.Color = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            nSeries.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);
            nSeries.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.Black;

            nSeries.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            var count = 0;

            for (var i = 0; i < countData.Rows.Count; i++)
            {

                var value = DataIntegrity.ConvertToDouble(countData.Rows[i]["ClassCount"]);
                var myItem = new ChartSeriesItem(value) { Name = countData.Rows[i]["SchoolType"].ToString() };
                myItem.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml(StyleController.GetPieChartColor(countData.Rows[i]["SchoolType"].ToString(), count)); ;
                myItem.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;

                if (value <= 0) myItem.Label.Visible = false;
                nSeries.Type = ChartSeriesType.Pie;
                nSeries.AddItem(myItem);

            }
            classCountChart.Series.Add(nSeries);

            addNewClass.Visible = UserHasPermission(Base.Enums.Permission.Create_Class);

            /****************************************************************
             * if user does not have Tab_Search_Classes permissions, then 
             * hide the search tab and the radPageView associated with it.
             * *************************************************************/
            if (!UserHasPermission(Permission.Tab_Search_Classes))
            {
                radPageViewClassSearch.Visible = false;
                foreach (Telerik.Web.UI.RadTab tab in RadTabStrip2.Tabs)
                {
                    if (tab.Text == "Search")
                    {
                        tab.Visible = false;
                        break;
                    }
                }
            }
        }

        protected void pieChart_Click(object sender, ChartClickEventArgs args)
        {
            if (args.SeriesItem != null)
            {
                SessionObject.SchoolSearchParms.DeleteParm("SchoolType");
                SessionObject.SchoolSearchParms.AddParm("SchoolType", args.SeriesItem.Name.ToString());

                classPieChartXmlHttpPanel.Attributes["searchTileName"] = "classes";
                classPieChartXmlHttpPanel.Attributes["controlURL"] = "../Controls/Class/ClassSearch_Expanded.aspx";
                classPieChartXmlHttpPanel.Value = "{\"EmptyString\":\"\"}";
                /*
                if (Tile.ParentContainer != null)
                    Tile.ParentContainer.ExpandTile(Tile, "Expand");*/
            }
        }
    }
}