using System;
using System.Data;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Drawing;
using Telerik.Web.UI;
using System.Text;
using Thinkgate.Base.Enums.AssessmentScheduling;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Reports
{
    public partial class StandardAnalysisDetail : TileControlBase
    {
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";
        public AnalysisType AnalysisType;
        public string Level;
        public string Guid;
        public string FormID;
        private bool _isContentLocked;


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

            if (Tile == null) return;
            if (Tile.TileParms.GetParm("AnalysisType") == null) return;

            // TFS 1123 : Check the content is locked or unlocked for specific Test ID
            if (Request.QueryString["testID"] != null)
            {
                int testID = Convert.ToInt32(Request.QueryString["testID"]);
                DataRow row = Thinkgate.Base.Classes.Assessment.GetAssessmentSchedule(testID);

                if (row != null)
                {
                    string contentLock;
                    string content = row["CONTENT"].ToString();
                    if (content == "Disabled")
                    { _isContentLocked = true; }
                    else if (content == "Enabled")
                    { _isContentLocked = false; }
					else if (row["CONTENT"].ToString().Split(' ')[0] == "Enabled")
                    {
                        contentLock = row["CONTENT"].ToString();
                        DateTime dateFrom = DateTime.MinValue;
                        DateTime dateTo = DateTime.MaxValue;

                        if (contentLock.IndexOf(" - ") > -1) //Enabled 10/15/2013 - 10/31/2013
                        {
                            dateFrom = Convert.ToDateTime(contentLock.ToString().Split(' ')[1].Trim());
                            dateTo = Convert.ToDateTime(contentLock.ToString().Split(' ')[3].Trim());
                        }

                        else if (contentLock.IndexOf("Starting") > -1) //Enabled Starting 10/15/2013
                        {
                            dateFrom = Convert.ToDateTime(contentLock.ToString().Split(' ')[2]);
                        }

                        else if (contentLock.IndexOf("Until") > -1) //Enabled Until 10/15/2013
                        {
                            dateTo = Convert.ToDateTime(contentLock.ToString().Split(' ')[2]);
                        }

                        if (dateFrom <= DateTime.Today && DateTime.Today <= dateTo)
                        { _isContentLocked = false; }
                        else
                        { _isContentLocked = true; }
                    }
                }
            }

            AnalysisType = (AnalysisType)Tile.TileParms.GetParm("AnalysisType");
            Level = Tile.TileParms.GetParm("Level").ToString();
            FormID = Tile.TileParms.GetParm("FormID").ToString();

            LoadReport();
        }

        private DataSet GetDataTable()
        {
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

            string selectedLevel = Request.QueryString["level"];
            var selectedLevelId = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "levelID");

            var criteriaOverrides = (ReportCriteria).GetCriteriaOverrideString();
            var itemAnalysisDataSet = new DataSet();
              DataSet rawDataSet  = Thinkgate.Base.Classes.Reports.GetStandardAnaylsisData(year, selectedTest, selectedLevel, selectedLevelId, userPage,
                  _permissions, "", "", "", 0, selectedClass.ToString() != "0" ? selectedClass.ToString() : "0",
                                                                            "@@Product=none@@@@RR=none" +
                                                                                                     criteriaOverrides +
                                                                                                     "1test=yes@@@@PT=1@@",
                                                                             FormID, "SS");



              itemAnalysisDataSet.Tables.Add(Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(rawDataSet.Tables[0].Copy(), "StandardID", "xID"));
              itemAnalysisDataSet.Tables.Add(rawDataSet.Tables[1].Copy());

            return itemAnalysisDataSet;
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
            var questionList = dataItem["QuestionList"].ToString().Split(',');
            var itemLinks = (Label)e.Item.FindControl("itemLinks");
            var links = new StringBuilder();

            foreach(var questionIDAndSort in questionList)
            {
                //questionArr[0] = Question ID
                //questionArr[1] = Question Number
                var questionArr = questionIDAndSort.Split('|');
                if (links.Length > 0) links.Append(",");

                // TFS 1123: Disable item preview link for Teacher and School Admin login if assessment content is locked
                ReportHelper reportHelper = new ReportHelper();
                reportHelper.UserRoles = SessionObject.LoggedInUser.Roles;
                if (_isContentLocked && reportHelper.DisableItemLinks())
                    links.Append(questionArr[1]);
                else
                    links.Append("<a href=\"javascript:void(0);\" onclick=\"openItemPreview('../ControlHost/PreviewTestQuestion.aspx?xID=" + 
                    Standpoint.Core.Classes.Encryption.EncryptInt(DataIntegrity.ConvertToInt(questionArr[0])) 
                    + "'); return false;\">" + questionArr[1] + "</a>");
            }

            itemLinks.Text = links.ToString();
        }
    }
}