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

namespace Thinkgate.Controls.Staff
{
    public partial class StaffDemographics : TileControlBase
    {
        public static Base.Enums.EntityTypes _level;
        public static int _levelID;
        protected bool _perm_Tab_Search_Staff;
        private DataSet data;

#region Asynchronous Methods
        private void GetStaffDemoBySchool()
        {
            data = Base.Classes.Staff.GetStaffDemographicsBySchool(_levelID, SessionObject.GlobalInputs);
        }
#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserHasPermission(Base.Enums.Permission.Tile_StaffDemographics_School))
            {
                Tile.ParentContainer.Visible = false;
            }

            _level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(GetStaffDemoBySchool));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "StaffDemographics", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            BuildChart(chartExperience, data.Tables[0]);
            BuildChart(chartCertifications, data.Tables[1]);
            BuildChart(chartEndorsements, data.Tables[2]);

            PopulateCounts(data.Tables[3]);

            radPageViewExperience.Visible = tabExperience.Visible = UserHasPermission(Base.Enums.Permission.Tab_Experience_Staff_School);
            radPageViewCertifications.Visible = tabCertification.Visible = UserHasPermission(Base.Enums.Permission.Tab_Certifications_Staff_School);
            radPageViewEndorsements.Visible = tabEndorsements.Visible = UserHasPermission(Base.Enums.Permission.Tab_Endorsements_Staff_School);
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

        private void PopulateCounts(DataTable data)
        {
            foreach (DataRow row in data.Rows)
            {
                if (row["Level"].ToString().Equals("District"))
                {
                    lblDistrictCount.Text = String.Format("{0,12:N0}", row["StaffCount"]);
                } else
                {
                    lblSchoolCount.Text = String.Format("{0,12:N0}", row["StaffCount"]);
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
            
            //chart.Legend.TextBlock.Appearance.Position.AlignedPosition = AlignedPositions.Bottom;
            //chart.Legend.Appearance.Dimensions.Margins = "1px 50px 1px 1px";
            //chart.Legend.Appearance.Dimensions.Height = 150;
            //chart.Legend.TextBlock.Appearance.Position.Y = 50;
            //chart.Legend.TextBlock.Appearance.Position.X = 50;
            //chart.Legend.TextBlock.Visible = true;

            //double minval = 100;
            //double maxval = 0;

            ChartAxisItemsCollection caic = new ChartAxisItemsCollection();
            var group_list = new List<String>();
            var startDataCol = 2;
            // Begin Experience tab
            foreach (DataRow dr in seriesData.Rows)
            {
                for (var j=startDataCol; j < seriesData.Columns.Count; j++) {
                    var myItem = new ChartSeriesItem(DataIntegrity.ConvertToDouble(dr[j]), (DataIntegrity.ConvertToDouble(dr[j])).ToString("0.0%").Replace("%", "")) { Name = seriesData.Columns[j].ColumnName.ToString() };
                    if (String.IsNullOrEmpty(dr[j].ToString())) myItem.Appearance.Visible = false;   // hide bar and text if the value is null (no data)
                    
                    if (dr["Level"].Equals("District"))
                    {
                        nSeries2.AddItem(myItem);
                    }
                    else
                    {
                        nSeries.AddItem(myItem);
                    }
                    group_list.Add(seriesData.Columns[j].ColumnName.ToString());
                    //if (minval > DataIntegrity.ConvertToDouble(dr[j])) minval = DataIntegrity.ConvertToDouble(dr[j]);
                    //if (maxval < DataIntegrity.ConvertToDouble(dr[j])) maxval = DataIntegrity.ConvertToDouble(dr[j]);
                }
                                
            }
            foreach (var yl in group_list.Distinct())
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
            //chart.PlotArea.YAxis.AutoScale = false;
            //chart.PlotArea.YAxis.MaxValue = maxval == 100 ? maxval : maxval + 5;
            //chart.PlotArea.YAxis.a = maxval == 100 ? maxval : maxval + 5;
            /*chart.Legend.Appearance.Position.AlignedPosition = AlignedPositions.Top; 
            chart.Legend.Appearance.Overflow = Overflow.Row;
            chart.Legend.Appearance.ItemTextAppearance.TextProperties.Font = new Font("Verdana", 7);
            chart.Legend.Appearance.Dimensions.Paddings = "0, 0, 0, 0";
            */
            //chart.Legend.Appearance.Position.AlignedPosition = AlignedPositions.Top; 

            chart.Series.Add(nSeries);
            chart.Series.Add(nSeries2);

            chart.ChartTitle.Visible = false;

            chart.SetSkin("StaffDemographics");
            /*
            var skin = StyleController.GetChartConfigFile("StaffDemographics");
            if (skin != null)
            {
                var myCustomSkin1 = new ChartSkin("CustomSkin") {XmlSource = skin};
                myCustomSkin1.ApplyTo(chart.Chart);    
            }
            */
            /*ChartSkin myCustomSkin1 = new ChartSkin("CustomSkin");
            myCustomSkin1.XmlSource.Load(Server.MapPath("~/Skins/Chart1.xml"));
            myCustomSkin1.ApplyTo(chart.Chart);*/

            /*ChartSkin myCustomSkin = new ChartSkin();
            myCustomSkin.CreateFromChart(chart.Chart, "CustomSkin");
            myCustomSkin.XmlSource.Save(Server.MapPath("~/Skins/Chart1.xml"));*/
            //chart.Skin = "Web20";

        }


    }
}