using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Reflection;

using Thinkgate.Classes;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.Reports
{
    public partial class ReportCardStandard : TileControlBase
    {
        private DataTable _columnData;
        private DataTable _cellData;
        private DataTable PLevels;

        public string Guid;
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";
        //private string _type;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            if (Request.QueryString["selectedReport"] != null && !String.IsNullOrEmpty(Request.QueryString["selectedReport"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["selectedReport"].ToString() + "' report", Request.QueryString["selectedReport"].ToString(), sessionObject.LoggedInUser.UserName);
            }
            LoadCriteria();
             LoadReport();

        }

        protected void RemoteReportReload(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlAtRiskStandardCriteria";

            if (Tile.TileParms.GetParm("CriteriaGUID") == null) return;
            Guid = (string)Tile.TileParms.GetParm("CriteriaGUID");

            if (Session["Criteria_" + Guid] == null) return;

            ctlReportCriteria.Guid = Guid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            criteriaPlaceholder.Controls.Add(ctlReportCriteria);
        }

        private void LoadReport()
        {
            DataSet ds = new DataSet();

                var dev = System.Configuration.ConfigurationManager.AppSettings.Get("Environment") == "Dev" ? true : false;
                reportGrid.DataSource = null;

                var userPage = SessionObject.LoggedInUser.Page;
                var year = Thinkgate.Base.Classes.AppSettings.Demo_Year;

                var reportCriteria = (Criteria)Session["Criteria_" + Guid];

                if (reportCriteria == null) return;

                var selectedTestType = Tile.TileParms.GetParm("type").ToString();

                if (string.IsNullOrEmpty(selectedTestType))
                {
                    SessionObject.RedirectMessage =
                        "Could not interpret a value for test type";
                    Response.Redirect("~/PortalSelection.aspx");

                }

                var output = "normal";
                var scoreType = "pct";
                var student = "stud";

                var criteriaOverrides = ((Criteria)reportCriteria).GetCriteriaOverrideString();

                 ds = Thinkgate.Base.Classes.Reports.GetReportCardByStandard(0, year, userPage, _permissions,
                                                                                      selectedTestType, scoreType,
                                                                                      output, "", "", student,
                                                                                      "@@Product=none@@@@RR=none" +
                                                                                      criteriaOverrides +
                                                                                      "1test=yes@@@@PT=1@@", "SS");

            
                Session[string.Format("ReportCardStandard_{0}", Standpoint.Core.Classes.Encryption.DecryptString(Request.QueryString["xID"]))] = ds;


            if (ds == null || ds.Tables.Count < 3)
                return;
               
            _columnData = ds.Tables[0];
            _cellData = ds.Tables[1];
            PLevels = ds.Tables[2];
       
                AddColumns();
            AddPlevels();
            

            var rowData = (from DataRow dRow in _cellData.Rows
                                    select new
                                    {
                                        RecType = DataIntegrity.ConvertToInt(dRow["RecType"]),
                                        LevelID = dRow["LevelID"].ToString(),
                                        LevelName = dRow["LevelName"].ToString(),
                                        StudentID = dRow["Student_ID"].ToString()
                                    }).Where(t => t.RecType == 0);
            reportGrid.DataSource = rowData;
            reportGrid.DataBind();

            if (ds.Tables.Count > 2) SetLabels(ds.Tables[2].Rows[0]);
        }
        private void AddPlevels()
        {
            var pLevelDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            pLevelDiv.InnerHtml = PLevels.Rows[0]["ChartTitle2"].ToString();
            pLevelDiv.ClientIDMode = ClientIDMode.Static;
            pLevelDiv.ID = "PlevelKey";
            //unchecked onload

            pPlevels.Controls.Add(pLevelDiv);
        }

        private void AddColumns()
        {
            reportGrid.Columns.Clear();
            var nameColumn = new GridBoundColumn();
            nameColumn.DataField = "LevelName";
            nameColumn.HeaderText = "Name";
            nameColumn.HeaderStyle.Wrap = false;
            nameColumn.ItemStyle.Wrap = false;

            reportGrid.Columns.Add(nameColumn);

            var columns = (from DataRow dRow in _columnData.Rows
                           select new
                           {
                               TestYear = dRow["TestYear"].ToString(),
                               TypeAbbr = dRow["TypeAbbr"].ToString(),
                               Score = dRow["Score"].ToString(),
                               StandardName = dRow["StandardName"].ToString(),
                               StandardNameLeft = dRow["StanNameLeft"].ToString(),
                               StandardNameRight = dRow["StanNameRight"].ToString(),
                               StandardID = dRow["StandardID"].ToString(),
                               StandardCCSSName = dRow["CCSSStandardName"].ToString(),
                               StandardCCSSDesc = dRow["CCSSDesc"].ToString(),
                               StandardSSDesc = dRow["SSDesc"].ToString()
                           }).Where(t => t.StandardName.Length > 0 );
            
            foreach (var column in columns)
            {
                var col = new Telerik.Web.UI.GridBoundColumn();
                var tooltip = (column.StandardSSDesc) ?? "";
                tooltip += (column.StandardCCSSDesc) ?? "";
                col.HeaderText = column.TestYear + "<br/>" + column.TypeAbbr
                                + "<br/><a href='javascript: window.open(\"StandardsPage.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(column.StandardID) + "\");' style='color: blue; text-decoration:underline;' title=" + tooltip + " alt='" + tooltip + "'>" + column.StandardNameLeft 
                                + "<br/>" + column.StandardNameRight + "</a><br/>" + column.Score;
                col.UniqueName = column.StandardName;
                col.HeaderStyle.Font.Size = FontUnit.XSmall;
                reportGrid.Columns.Add(col);
            }
        }

        protected void reportGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            
            if (!(e.Item is GridDataItem)) return;

            var cellIndex = e.Item.Cells.Count - reportGrid.Columns.Count;
            var dataItem = (e.Item).DataItem;
            Type ty = dataItem.GetType();
            PropertyInfo pi = ty.GetProperty("LevelID");
            string levelID = pi.GetValue(dataItem, null) as string;

            //Make student name a link            
            e.Item.Cells[2].Attributes.Add("onclick", "window.open('Student.aspx?childPage=yes&xID=" + Standpoint.Core.Classes.Encryption.EncryptString(levelID) + "');");
            e.Item.Cells[2].Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");
            
            foreach (GridBoundColumn column in reportGrid.Columns)
            {
                if (String.IsNullOrEmpty(column.UniqueName)) continue;

                var standardName = column.UniqueName;

                var row = (from DataRow dRow in _cellData.Rows
                           select new
                           {
                               LevelID = dRow["LevelID"].ToString(),
                               StandardName = dRow["StandardName"].ToString(),
                               Score = dRow["Score"].ToString(),
                               PLevel = dRow["PLevel"].ToString()
                           }).FirstOrDefault(t => t.LevelID == levelID && t.StandardName == standardName);

                if (row != null)
                {
                    e.Item.Cells[cellIndex].Text = row.Score;
                    e.Item.Cells[cellIndex].Attributes.Add("plevel",row.PLevel);
                    e.Item.Cells[cellIndex].CssClass = "reportCardStandardItem";
                }

                cellIndex++;
            }

        }

        private void SetLabels(DataRow row)
        {
            lblSchoolCount.Text = row["SchoolCount"].ToString();
            lblTeacherCount.Text = row["TeacherCount"].ToString();
            lblClassCount.Text = row["ClassCount"].ToString();
            lblStudentCount.Text = row["StudentCount"].ToString();
        }

        protected void ReloadPerformanceLevelColoring___(object sender, EventArgs e)
        {
            LoadReport();
        }
    }
}