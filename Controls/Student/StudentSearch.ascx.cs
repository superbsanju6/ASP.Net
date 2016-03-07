using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using Standpoint.Core.Utilities;
using Telerik.Charting;
using Telerik.Charting.Styles;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes.Data;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace Thinkgate.Controls.Student
{
    public partial class StudentSearch : TileControlBase
    {
        public static Base.Enums.EntityTypes _level;
        public static int _levelID;
        public static Interfaces.IRotatorControl ctlRotator;
        public static DataTable countData;

        private void GetStudentCounts()
        {
            // asynchronous task.
            countData = Base.Classes.Data.StudentDB.GetStudentCountsDT(_levelID, _level);
        }

        private void LoadDistrictParms()
        {
            var parms = Base.Classes.DistrictParms.LoadDistrictParms();
            if (parms.TilePieCharts_Clickable.ToLower() == "yes") studentCountChart.Click += pieChart_Click;
        }

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null) return;

            _level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));
            ctlRotator = (Interfaces.IRotatorControl)Tile.TileParms.GetParm("ctlDoublePanel");

            studentsearch_hiddenLevel.Value = _level.ToString();
            studentsearch_hiddenLevelID.Value = Standpoint.Core.Classes.Encryption.EncryptInt(_levelID);
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (_levelID <= 0) return;


            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(GetStudentCounts));
            taskList.Add(new AsyncPageTask(LoadDistrictParms));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "StudentSearch", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            studentCountChart.DataSource = countData;
            studentCountChart.ChartTitle.Visible = false;
            studentCountChart.DataBind();

            studentPieChartXmlHttpPanel.Value = "";

            //Handle Legend Labels
            studentCountChart.Series.Clear();
            var nSeries = new ChartSeries();

            studentCountChart.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            studentCountChart.PlotArea.Appearance.FillStyle.FillType = FillType.Solid;
            studentCountChart.PlotArea.Appearance.Border.Color = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
           
            studentCountChart.PlotArea.XAxis.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);
            studentCountChart.PlotArea.YAxis.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);
            studentCountChart.PlotArea.XAxis.Appearance.LabelAppearance.Position.AlignedPosition = AlignedPositions.TopRight;
            nSeries.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.Black;
            nSeries.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 8);
            nSeries.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            
            var count = 0;

            //change to foreach for easier reading
            var gradeList = new List<string>();
            foreach (DataRow dr in countData.Rows)
            {
                var value = DataIntegrity.ConvertToDouble(dr["StudentCount"]);
                var xValue = 0;
                switch (_level)
                {
                    case EntityTypes.Teacher:
                        xValue = dr.Table.Columns.Contains("ClassID") ? DataIntegrity.ConvertToInt(dr["ClassID"]) : 0;
                        break;
                    case EntityTypes.School:
                        gradeList.Add(dr.Table.Columns.Contains("GradeNumber") ? dr["GradeNumber"].ToString() : string.Empty);
                        xValue = gradeList.Count - 1;
                        break;
                }

                var myItem = new ChartSeriesItem(value) { Name = dr["Grade"].ToString(), XValue = xValue };
                myItem.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml(StyleController.GetPieChartColor(dr["Grade"].ToString(), count++)); ;
                myItem.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;
                
                if (value <= 0) myItem.Label.Visible = false;

                nSeries.Type = ChartSeriesType.Pie;
                nSeries.AddItem(myItem);
                /*
                var classIDValue = DataIntegrity.ConvertToDouble(dr["ClassID"]);
                var classItem = new ChartSeriesItem(classIDValue) { XValue = DataIntegrity.ConvertToDouble(dr["ClassID"]) };
                classItem.Label.Visible = false;*/


                /****************************************************************
                 * if user does not have Tab_Search_Staff permissions, then hide
                 * the search tab and the radPageView associated with it.
                 * *************************************************************/
                if (!UserHasPermission(Permission.Tab_Search_Students))
                {
                    RadPageView2.Visible = false;
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
                 * If user does not have Icon_ExpandedSearch_Students permissions, 
                 * then hide the "Advanced Search" link via its surrounding div tag.
                 * ****************************************************************/

                StudentSearch_DivAdvancedSearch.Visible = (UserHasPermission(Permission.Icon_ExpandedSearch_Students));

            }

            SessionObject.StudentSearchParms.AddParm("GradeListFilter", gradeList);

            studentCountChart.Series.Add(nSeries);

            if (IsPostBack)
            {
                //Reloads previous search results if session object search text has a value
                if (studentsSearchText_smallTile.Value == "Search by last name..." && SessionObject.StudentSearchTxtPostBack_smallTile != null
                    && SessionObject.StudentSearchTxtPostBack_smallTile.Length > 0)
                    SearchStudentsByLastName_Click(null, null);

                return;
            }

            studentSearchMoreLink.Visible = false;
            studentsSearchTileGrid.Visible = false;
            enrollStudent.Visible = UserHasPermission(Base.Enums.Permission.Enroll_Student);
            addNewStudent.Visible = UserHasPermission(Base.Enums.Permission.Create_Student);
        }

        protected void SearchStudentsByLastName_Click(object sender, EventArgs e)
        {
            //TODO: Analyze for performance on postback
            String searchVal = "";

            if (studentsSearchText_smallTile.Value.Length > 0 && studentsSearchText_smallTile.Value != "Search by last name...")
            {
                searchVal = studentsSearchText_smallTile.Value;

                //store text field value in the SessionObject
                SessionObject.StudentSearchTxtPostBack_smallTile = studentsSearchText_smallTile.Value;
            }
            else if (SessionObject.StudentSearchTxtPostBack_smallTile != null && SessionObject.StudentSearchTxtPostBack_smallTile.Length > 0)
            {
                searchVal = SessionObject.StudentSearchTxtPostBack_smallTile;
            }

            if (searchVal.Length > 0)
            {
                DataTable dt = Base.Classes.Data.StudentDB.SearchStudentsByLastName(_levelID, searchVal, _level);
                int studentRecordCount = 0;

                if (dt.Rows.Count > 0)
                {
                    studentNoRecordsMsg.Visible = false;
                    studentsSearchTileGrid.Visible = true;
                    studentRecordCount = DataIntegrity.ConvertToInt(dt.Select()[0].ItemArray[7]);

                    dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "ID", "ID_Encrypted");

                    studentsSearchTileGrid.DataSource = dt;
                    studentsSearchTileGrid.DataBind();
                }
                else
                {
                    studentsSearchTileGrid.Visible = false;
                    studentNoRecordsMsg.Visible = true;
                }
                
                if (studentRecordCount > 100)
                {
                    studentSearchMoreLink.Visible = true;
                }
            }
        }

        protected void pieChart_Click(object sender, ChartClickEventArgs args)
        {
            if (args.SeriesItem != null)
            {
                var pieChartValue = string.Empty;
                switch(_level)
                {
                    case EntityTypes.Teacher:
                    case EntityTypes.School:
                        pieChartValue = args.SeriesItem.XValue.ToString();
                        break;
                    case EntityTypes.District:
                        pieChartValue = args.SeriesItem.Name;
                        break;
                }
                SessionObject.StudentSearchParms.DeleteParm("PieChartValue");
                SessionObject.StudentSearchParms.AddParm("PieChartValue", pieChartValue);

                studentPieChartXmlHttpPanel.Attributes["level"] = studentsearch_hiddenLevel.Value;
                studentPieChartXmlHttpPanel.Attributes["levelID"] = studentsearch_hiddenLevelID.Value;
                studentPieChartXmlHttpPanel.Attributes["searchTileName"] = "students";
                studentPieChartXmlHttpPanel.Attributes["controlURL"] = "../Controls/Student/StudentSearch_Expanded.aspx";
                studentPieChartXmlHttpPanel.Value = "{\"EmptyString\":\"\"}";
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
                SessionObject.StudentSearchParms.DeleteParm("Student Name");
                SessionObject.StudentSearchParms.AddParm("Student Name", studentsSearchText_smallTile.Value.ToString());
            }

            if (Tile.ParentContainer != null)
                Tile.ParentContainer.ExpandTile(Tile, "Expand");
        }

        protected void studentsSearchTileGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                var dataItem = (DataRowView)item.DataItem;
                var link = (Label)item.FindControl("studentNameLink");

                if (link != null)
                {
                    link.Text = dataItem["Student_Name"].ToString();
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                    {
                        link.Attributes["onclick"] = "window.open('Student.aspx?childPage=yes&xID=" + dataItem["ID_Encrypted"] + "'); return false;";
                        link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                        link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                        link.Attributes["style"] = "color:#00F; text-decoration:underline; cursor:pointer;";
                    }
                    else
                    {
                        link.Attributes["style"] = "color:#3B3B3B; text-decoration:none;";
                    }
                }
            }
        }
    }
}
