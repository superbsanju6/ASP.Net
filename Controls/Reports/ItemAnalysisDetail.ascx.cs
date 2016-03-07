using System;
using System.Data;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Drawing;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Reports
{
    public partial class ItemAnalysisDetail : TileControlBase
    {
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";
        public AnalysisType? AnalysisType;
        public string Level;
        public string Guid;
        public string FormID;

        private DataSet ItemAnalysisData
        {
            get { return (DataSet)Session["ItemAnalysisData_" + AnalysisType.ToString() + "_" + Guid]; }
            set { Session["ItemAnalysisData_" + AnalysisType.ToString() + "_" + Guid] = value; }
        }

        private Criteria ReportCriteria
        {
            get { return (Criteria)Session["Criteria_" + Guid]; }
            set { Session["Criteria_" + Guid] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Guid)) return;

            if (ReportCriteria == null) return;

            if (Tile == null) return;
            if (Tile.TileParms.GetParm("AnalysisType") == null) return;
            AnalysisType = (AnalysisType)Tile.TileParms.GetParm("AnalysisType");
            Level = Tile.TileParms.GetParm("Level").ToString();
            FormID = Tile.TileParms.GetParm("FormID").ToString();

            LoadReport();
        }

        private DataSet GetDataTable()
        {
            if (ItemAnalysisData == null)
            {
                //Do switch here on report type
                if (!AnalysisType.HasValue) AnalysisType = Reports.AnalysisType.ItemAnalysis;

                var dev = System.Configuration.ConfigurationManager.AppSettings.Get("Environment") == "Dev" ? true : false;
                var userPage = dev ? 119 : 60528;

                var year = Thinkgate.Base.Classes.AppSettings.Demo_Year;


                if (string.IsNullOrEmpty(Request.QueryString["cid"]) || string.IsNullOrEmpty(Request.QueryString["testID"]))
                {
                    SessionObject.RedirectMessage =
                        "Either the course or test ID could not be found.";
                    Response.Redirect("~/PortalSelection.aspx");

                }

                var selectedClass = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(Request.QueryString["cid"]);
                var selectedTest = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(Request.QueryString["testID"]);


                var criteriaOverrides = ReportCriteria.GetCriteriaOverrideString();

                var rawDataSet = new DataSet();

                switch (AnalysisType)
                {
                    case Reports.AnalysisType.ItemAnalysis:
                        rawDataSet = Thinkgate.Base.Classes.Reports.GetItemAnaylsisData(year, selectedTest,
                                                                                                    "Class",
                                                                                                    selectedClass,
                                                                                                    userPage,
                                                                                                    _permissions, "", "",
                                                                                                    "", 0, "",
                                                                                                    "@@Product=none@@@@RR=none" +
                                                                                                    criteriaOverrides +
                                                                                                    "1test=yes@@@@PT=1@@",
                                                                                                    FormID);
                        break;

                    case Reports.AnalysisType.StandardAnalysis:
                        rawDataSet = Thinkgate.Base.Classes.Reports.GetStandardAnaylsisData(year, selectedTest, "Class", selectedClass, userPage,
                                                                            _permissions, "", "", "", 0, selectedClass.ToString(),
                                                                           "@@Product=none@@@@RR=none" +
                                                                                                    criteriaOverrides +
                                                                                                    "1test=yes@@@@PT=1@@",
                                                                            FormID, "SS");

                        
                        break;
                }

                ItemAnalysisData.Tables.Add(Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(rawDataSet.Tables[0].Copy(), "Question", "xID"));
                ItemAnalysisData.Tables.Add(rawDataSet.Tables[1].Copy());
                
            }

            return ItemAnalysisData;
        }

        private void LoadReport()
        {
            var dt = GetDataTable();
            if (dt == null || dt.Tables[0].Rows.Count == 0) return;
                        
            reportGrid.DataSource = dt.Tables[0].Select("ChartItem = '" + Level + "'"); 
            reportGrid.DataBind();
        }

        protected void reportGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (!(e.Item is GridDataItem)) return;

            var dataItem = (DataRowView)(e.Item).DataItem;
            var correctAnswer = dataItem["CorrectAnswer"].ToString();
            var isFieldTest = dataItem["FieldTest"].ToString() == "1";
            var startCellIndex = 5; //The index of the A column
            var itemNumberCellIndex = 1; //The index of the A column
            var green = Color.GreenYellow;
            var yellow = Color.Yellow;


            if(isFieldTest)
            {
                e.Item.Cells[itemNumberCellIndex].BackColor = yellow;
            }

            switch (correctAnswer)
            {
                case "A":
                    e.Item.Cells[startCellIndex].BackColor = green;
                    break;

                case "B":
                    e.Item.Cells[startCellIndex+1].BackColor = green;
                    break;

                case "C":
                    e.Item.Cells[startCellIndex + 2].BackColor = green;
                    break;

                case "D":
                    e.Item.Cells[startCellIndex + 3].BackColor = green;
                    break;

            }
        }
    }
}