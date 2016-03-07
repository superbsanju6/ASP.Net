using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using Standpoint.Core.Utilities;
using Telerik.Charting;
using Telerik.Charting.Styles;
using Thinkgate.Classes;
using Thinkgate.Base.Classes.Data;

namespace Thinkgate.Controls.School
{
    public partial class SchoolDropoutPrevention : TileControlBase
    {
        public static Base.Enums.EntityTypes _level;
        public static int _levelID;
        protected bool _perm_Tab_Search_Staff;
        private DataTable countData;

        #region Asynchronous Methods
        private void GetDropOutPrevention()
        {
            countData = Base.Classes.School.GetDropoutPreventionRatesforSchool(_levelID, SessionObject.GlobalInputs);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            /*******************************************************************
             * If user does not have Icon_Expanded_Staff permissions, then hide
             * the "Advanced Search" link via its surrounding div tag.
             * ****************************************************************/
            _level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(GetDropOutPrevention));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "SchoolDropOut", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();
                   
            //if (parms.TilePieCharts_Clickable.ToLower() == "yes") teacherCountChart.Click += pieChart_Click;

            // Handle Legend Labels
            teacherCountChart.Series.Clear();
            var nSeries = new ChartSeries();
            var nSeries2 = new ChartSeries();

            nSeries.Name = "School %";
            nSeries2.Name = "District %";
            teacherCountChart.Legend.Appearance.ItemTextAppearance.AutoTextWrap = AutoTextWrap.False;

            teacherCountChart.PlotArea.YAxis.AutoScale = false;


            //double minval = 100;
            //double maxval = 0;
            ChartAxisItemsCollection caic = new ChartAxisItemsCollection();
            var year_list = new List<String>();

            foreach (DataRow dr in countData.Rows)
            {
                var myItem = new ChartSeriesItem(DataIntegrity.ConvertToDouble(dr["DropoutRate"]), dr["LevelLabel"].ToString()) { Name = dr["SchoolYear"].ToString() };
                if (String.IsNullOrEmpty(dr["DropoutRate"].ToString())) myItem.Appearance.Visible = false;   // hide bar and text if the value is null (no data)
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

                //if (minval > DataIntegrity.ConvertToDouble(dr["DropoutRate"])) minval = DataIntegrity.ConvertToDouble(dr["DropoutRate"]);
                //if (maxval < DataIntegrity.ConvertToDouble(dr["DropoutRate"])) maxval = DataIntegrity.ConvertToDouble(dr["DropoutRate"]);
            }
            foreach (var yl in year_list.Distinct())
            {
                ChartAxisItem ai = new ChartAxisItem();
                ai.TextBlock.Text = yl;
                caic.Add(ai);
            }
            teacherCountChart.PlotArea.XAxis.AddItem(caic);

            teacherCountChart.PlotArea.YAxis.MinValue = 0;
            teacherCountChart.PlotArea.YAxis.Step = 5;
            teacherCountChart.PlotArea.YAxis.AutoScale = true;
            //teacherCountChart.PlotArea.YAxis.MaxValue = 100;

            teacherCountChart.Series.Add(nSeries);
            teacherCountChart.Series.Add(nSeries2);

            teacherCountChart.ChartTitle.Visible = false;

            teacherCountChart.SetSkin("SchoolDropoutPrevention");
        }

    }
}