using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using Standpoint.Core.Utilities;
using Telerik.Charting;
using Telerik.Charting.Styles;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Base.Classes.Data;

namespace Thinkgate.Controls.School
{
    public partial class AbsenceRates : TileControlBase
    {
        public static Base.Enums.EntityTypes _level;
        public static int _levelID;
        protected bool _perm_Tab_Search_Staff;
        private DataSet data;

        #region Asynchronous Methods
        private void GetAbsenceTardyRates()
        {
            data = Base.Classes.School.GetAbsenceAndTardyRatesforSchool(_levelID, SessionObject.GlobalInputs);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserHasPermission(Base.Enums.Permission.Tile_Absences_School))
            {
                Tile.ParentContainer.Visible = false;
            }

            _level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(GetAbsenceTardyRates));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "AbsenceRates", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            BuildChart(chartAbsences, data.Tables[0]);
            BuildChart(chartTardies, data.Tables[1]);

            radPageViewAbsences.Visible = tabAbsences.Visible = UserHasPermission(Base.Enums.Permission.Tab_Absences_Absences_School);
            radPageViewTardies.Visible = tabTardies.Visible = UserHasPermission(Base.Enums.Permission.Tab_Tardies_Absences_School);
            foreach (RadTab radTab in RadTabStrip2.Tabs)
            {
                if (radTab.Visible)
                {
                    // select first visible tab and page view
                    radTab.Selected = true;
                    radTab.PageView.Selected = true;
                    break;
                }
            }
        }

        private void BuildChart(RadChart chart, DataTable seriesData)
        {
            
            // Handle Legend Labels
            chart.Series.Clear();
            var nSeries = new ChartSeries();
            var nSeries2 = new ChartSeries();
            
            nSeries.Name = "School %";
            nSeries2.Name = "District %";
            chart.Legend.Appearance.ItemTextAppearance.AutoTextWrap = AutoTextWrap.False;

            //double minval = 100;
            //double maxval = 0;

            ChartAxisItemsCollection caic = new ChartAxisItemsCollection();
            var year_list = new List<String>();
            
            // Begin Experience tab
            foreach (DataRow dr in seriesData.Rows)
            {
                var myItem = new ChartSeriesItem(DataIntegrity.ConvertToDouble(dr["Rate"]), dr["LevelLabel"].ToString()) { Name = dr["SchoolYear"].ToString() };
                if (String.IsNullOrEmpty(dr["Rate"].ToString())) myItem.Appearance.Visible = false;   // hide bar and text if the value is null (no data)
                //myItem.Label.Appearance.LabelLocation = StyleSeriesItemLabel.ItemLabelLocation.Auto;
                if (dr["Level"].Equals("District"))
                {
                    nSeries2.AddItem(myItem);
                }
                else
                {
                    nSeries.AddItem(myItem);
                }
                year_list.Add(dr["SchoolYear"].ToString());
                
                //if (minval > DataIntegrity.ConvertToDouble(dr["Rate"])) minval = DataIntegrity.ConvertToDouble(dr["Rate"]);
                //if (maxval < DataIntegrity.ConvertToDouble(dr["Rate"])) maxval = DataIntegrity.ConvertToDouble(dr["Rate"]);
            }
            foreach (var yl in year_list.Distinct())
            {
                ChartAxisItem ai = new ChartAxisItem();
                ai.TextBlock.Text = yl;
                caic.Add(ai);
            }
            chart.PlotArea.XAxis.AddItem(caic);

            chart.PlotArea.YAxis.MaxValue = 1;
            chart.PlotArea.YAxis.MinValue = 0;
            //chart.PlotArea.YAxis.Step = (maxval - (minval - 5)) / 2;
            chart.PlotArea.YAxis.Step = 5;
            chart.PlotArea.YAxis.AutoScale = false;
            //chart.PlotArea.YAxis.MaxValue = maxval == 100 ? maxval : maxval + 5;
            //chart.PlotArea.YAxis.a = maxval == 100 ? maxval : maxval + 5;
            
            chart.Series.Add(nSeries);
            chart.Series.Add(nSeries2);

            chart.ChartTitle.Visible = false;
            chart.SetSkin("AbsenceRates");
        }

    }
}