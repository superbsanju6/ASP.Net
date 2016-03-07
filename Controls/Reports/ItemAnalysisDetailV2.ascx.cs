using System;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Linq;
using System.Drawing;
using Telerik.Web.UI;
using Thinkgate.Base.Enums.AssessmentScheduling;

namespace Thinkgate.Controls.Reports
{
    public partial class ItemAnalysisDetailV2 : TileControlBase
    {
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";
        public AnalysisType? AnalysisType;
        public string Level;
        public string Guid;
        public string FormID;
        private bool _isContentLocked;

        /*********************************************************************
        Set up meaningful variables for grid columns
        *********************************************************************/
        protected int colIdxItemNbr = 0;
        protected int colIdxRBT = 1;
        protected int colIdxPctCorrect = 2;
        protected int colIdxA = 3;
        protected int colIdxB = 4;
        protected int colIdxC = 5;
        protected int colIdxD = 6;
        protected int colIdxE = 7;
        protected int colIdxTrue = 8;
        protected int colIdxFalse = 9;
        protected int colIdxRight = 10;
        protected int colIdxWrong = 11;
        protected int colIdxRubricType = 12;
        protected int colIdxPt0 = 13;
        protected int colIdxPt1 = 14;
        protected int colIdxPt2 = 15;
        protected int colIdxPt3 = 16;
        protected int colIdxPt4 = 17;
        protected int colIdxPt5 = 18;
        protected int colIdxPt6 = 19;
        protected int colIdxPt7 = 20;
        protected int colIdxPt8 = 21;
        protected int colIdxPt9 = 22;
        protected int colIdxPt10 = 23;
        protected int colIdxUnanswered = 24;

        /*********************************************************************
        Set up meaningful groupings of grid columns
        *********************************************************************/
        protected int[] colAryMC;
        protected int[] colAryTF;
        protected int[] colAryRW;
        protected int[] colAryRubric;
        protected int[] colAryUnanswered;

        private StringBuilder sb = new StringBuilder("");

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

            // TFS 1123 : Check the content is locked or unlocked for specific Test ID
            if (Request.QueryString["testID"] != null)
            {
                int testID = Convert.ToInt32(Request.QueryString["testID"]);
                DataRow row = Thinkgate.Base.Classes.Assessment.GetAssessmentSchedule(testID);

                if (row != null)
                {
                    string contentLock;
                    string content = row["CONTENT"].ToString();
                    if (content == AssessmentScheduleStatus.Disabled.ToString())
                    { _isContentLocked = true; }
					else if (content == AssessmentScheduleStatus.Enabled.ToString())
                    { _isContentLocked = false; }
					else if (row["CONTENT"].ToString().Split(' ')[0] == AssessmentScheduleStatus.Enabled.ToString())
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

            // populate our array properties
            colAryMC = new  int[] {colIdxA, colIdxB, colIdxC, colIdxD,colIdxE};
            colAryTF = new  int[] {colIdxTrue, colIdxFalse};
            colAryRW = new  int[] {colIdxRight, colIdxWrong};
            colAryRubric = new  int[] {colIdxRubricType, colIdxPt0, colIdxPt1, colIdxPt2, colIdxPt3, colIdxPt4, colIdxPt5, colIdxPt6, colIdxPt7, colIdxPt8, colIdxPt9, colIdxPt10};
            colAryUnanswered = new  int[] {colIdxUnanswered};

            AnalysisType = (AnalysisType)Tile.TileParms.GetParm("AnalysisType");
            Level = Tile.TileParms.GetParm("Level").ToString();
            FormID = Tile.TileParms.GetParm("FormID").ToString();

            LoadReport();

            loadColIdxArrays();
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
                        rawDataSet = Thinkgate.Base.Classes.Reports.GetItemAnaylsisData(year, 
                                                                                        selectedTest,
                                                                                        Level,
                                                                                        selectedClass,
                                                                                        userPage,
                                                                                        _permissions,
                                                                                        "", 
                                                                                        "",
                                                                                        "", 
                                                                                        0, 
                                                                                        "",
                                                                                        "@@Product=none@@@@RR=none" + criteriaOverrides + "1test=yes@@@@PT=1@@@@FormID=" + FormID.ToString() + "@@",
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

            var rptRows = dt.Tables[0].Select("ChartItem = '" + Level + "'");

            hideUnnecessaryColumns(ref rptRows);

            ItemAnalysisDetail_reportGrid.DataSource = rptRows;

            ItemAnalysisDetail_reportGrid.DataBind();
        }

        /// <summary>
        /// Search through items and columns in the data table. Hide columns that,
        /// according to the items or column values are not needed.  EX: if none of
        /// the items have rubrics, then hide the rubric columns and the rubric
        /// check box.
        /// </summary>
        private void hideUnnecessaryColumns(ref DataRow[] rptRows)
        {
            /******************************************************************************************************
             * In order to answer the questions about which columns we can hide, we need to query two columns in
             * our datatable:
             *      1st -   get a distinct set of values in the "QuestionType" column.  Use this to decide which 
             *              Questions types are not in use so we can hide those columns.
             *              
             *      2nd -   Determine the Maximum value in the "MaxPoints" column.  If the answer is 0, then we
             *              know that there were no items with rubrics.  If the answer is anything other than 0
             *              then we can hide all the rubric point columns greater than our answer.
            ******************************************************************************************************/

            //var rptRows = rptSet.Tables[0].Rows;
            var questionTypes = rptRows.OfType<DataRow>().Select(r => r["QuestionType"].ToString()).Distinct();
            int maxPoints = rptRows.OfType<DataRow>().Max(r => ((int)r["MaxPoints"]));
 
            // Test for presence of Mult Choice items.
            if (!questionTypes.Any(q => (q == "MC5" || q == "MC4" || q == "MC3")))
            {
                foreach (int i in colAryMC) ItemAnalysisDetail_reportGrid.Columns[i].Display = false;
                ItemAnalysisDetail_MCToggle.Visible = false;
                Array.Resize(ref colAryMC, 0);
            }
            if (!questionTypes.Any(q => (q == "MC5")))
            {
                ItemAnalysisDetail_reportGrid.Columns[colIdxE].Display = false;
                ItemAnalysisDetail_MCToggle.Visible = false;
                //Array.Resize(ref colAryMC, 0);
                colAryMC = colAryMC.Where(val => val != colIdxE).ToArray();
            }

            if (!questionTypes.Any(q => (q == "MC5" || q == "MC4")))
            {
                ItemAnalysisDetail_reportGrid.Columns[colIdxD].Display = false;
                ItemAnalysisDetail_MCToggle.Visible = false;
                //Array.Resize(ref colAryMC, 0);
                colAryMC = colAryMC.Where(val => val != colIdxD).ToArray();

                ItemAnalysisDetail_reportGrid.Columns[colIdxE].Display = false;
                ItemAnalysisDetail_MCToggle.Visible = false;
                //Array.Resize(ref colAryMC, 0);
                colAryMC = colAryMC.Where(val => val != colIdxE).ToArray();
            }

            // Test for presence of T/F items.
            if (!questionTypes.Any(q => (q == "T")))
            {
                foreach (int i in colAryTF) ItemAnalysisDetail_reportGrid.Columns[i].Display = false;
                ItemAnalysisDetail_TFToggle.Visible = false;
                Array.Resize(ref colAryTF, 0);
            }

            // Test for the presence of Right/Wrong items.
            if (!questionTypes.Any(q => (q == "S")) //Short Answer
                &&
                !questionTypes.Any(q => (q == "E"))) //Essay
            {
                foreach (int i in colAryRW) ItemAnalysisDetail_reportGrid.Columns[i].Display = false;
                ItemAnalysisDetail_RWToggle.Visible = false;
                Array.Resize(ref colAryRW, 0);
            }

            // Test for the presence of Rubric Item.
            if (maxPoints == 0)
            {
                foreach (int i in colAryRubric) ItemAnalysisDetail_reportGrid.Columns[i].Display = false;
                ItemAnalysisDetail_RubricToggle.Visible = false;
                Array.Resize(ref colAryRubric, 0);
            }
            else
            {
                // If Rubric Items do exist, then test for which rubric point columns to display.
                for (int i = colIdxPt0 + maxPoints + 1; i <= colIdxPt10; i++) ItemAnalysisDetail_reportGrid.Columns[i].Display = false;
                Array.Resize(ref colAryRubric, maxPoints + 2);
            }

        }

        protected void reportGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (!(e.Item is GridDataItem)) return;

            // Assign local variables for common Assessment Data values.
            var dataItem = (DataRowView) (e.Item).DataItem;
            var QuestionType = dataItem["QuestionType"].ToString();
            var correctAnswer = dataItem["CorrectAnswer"].ToString();
            var isFieldTest = dataItem["FieldTest"].ToString() == "1";
			var scoreType = dataItem["ScoreType"].ToString();
            var ChartGroup = dataItem["ChartGroup"].ToString();
            var itemID = dataItem["xID"].ToString();
            var rubricSort = dataItem["TR_Sort"].ToString();

            // Assign local variables for commonly referred to columns
            const int indexMultChoiceA = 5; //The index of the A column
            const int itemNumberCellIndex = 1; //
            var green = Color.GreenYellow;
            var yellow = Color.Yellow;
            var itemLinks = (Label)e.Item.FindControl("itemLinks");

            // TFS 1123: Disable item preview link for Teacher and School Admin login if assessment content is locked
            ReportHelper reportHelper = new ReportHelper();
            reportHelper.UserRoles = SessionObject.LoggedInUser.Roles;
            if (_isContentLocked && reportHelper.DisableItemLinks())
                itemLinks.Text = ChartGroup;
            else
                itemLinks.Text = "<a href=\"javascript:void(0);\" onclick=\"openItemPreview('../ControlHost/PreviewTestQuestion.aspx?xID=" +
                    itemID + "'); return false;\">" + ChartGroup + "</a>";

            /**************************************************************************
            * Display all field test questions with a yellow background.
            **************************************************************************/
            if (isFieldTest)
            {
                e.Item.Cells[itemNumberCellIndex].BackColor = yellow;
            }


            /***************************************************************************
            * For all other distractors that are not part of the test question, blank 
            * these out.  We wouldn't want them to "distract" the user from clearly
            * regarding the test question's answers.
            **************************************************************************/
            if (QuestionType != "MC4" && QuestionType != "MC3" && QuestionType != "MC5") foreach (var i in colAryMC) e.Item.Cells[i + 2].Text = "-";

            if (QuestionType != "T") foreach (var i in colAryTF) e.Item.Cells[i + 2].Text = "-";

			if (scoreType != "W") foreach (var i in colAryRW) e.Item.Cells[i + 2].Text = "-";

            if (scoreType == "R" && rubricSort == "0")
            {
                foreach (var i in colAryRubric)
                {
                    if (i != colIdxRubricType)
                        e.Item.Cells[i + 2].Text = "-";
                }

                foreach (var i in colAryUnanswered)
                    e.Item.Cells[i + 2].Text = "-";
            }

			if (scoreType == "W") foreach (var i in colAryRubric) e.Item.Cells[i + 2].Text = "-";

            /**************************************************************************
             * Highlight cell with correct answer
             *************************************************************************/
            switch (QuestionType)
            {
                case "MC5": // Multiple Choice
                    switch (correctAnswer)
                    {
                        case "A":
                            e.Item.Cells[indexMultChoiceA].BackColor = green;
                            break;

                        case "B":
                            e.Item.Cells[indexMultChoiceA + 1].BackColor = green;
                            break;

                        case "C":
                            e.Item.Cells[indexMultChoiceA + 2].BackColor = green;
                            break;

                        case "D":
                            e.Item.Cells[indexMultChoiceA + 3].BackColor = green;
                            break;

                        case "E":
                            e.Item.Cells[indexMultChoiceA + 4].BackColor = green;
                            break;

                    }
                    break;

                case "MC4": // Multiple Choice
                    switch (correctAnswer)
                    {
                        case "A":
                            e.Item.Cells[indexMultChoiceA].BackColor = green;
                            break;

                        case "B":
                            e.Item.Cells[indexMultChoiceA + 1].BackColor = green;
                            break;

                        case "C":
                            e.Item.Cells[indexMultChoiceA + 2].BackColor = green;
                            break;

                        case "D":
                            e.Item.Cells[indexMultChoiceA + 3].BackColor = green;
                            break;

                    }
                    break;

                case "MC3": // Multiple Choice
                    switch (correctAnswer)
                    {
                        case "A":
                            e.Item.Cells[indexMultChoiceA].BackColor = green;
                            break;

                        case "B":
                            e.Item.Cells[indexMultChoiceA + 1].BackColor = green;
                            break;

                        case "C":
                            e.Item.Cells[indexMultChoiceA + 2].BackColor = green;
                            break;
                    }
                    break;

                case "T":  //True or False
                    switch (correctAnswer)
                    {
                        case "A":
                            e.Item.Cells[colIdxTrue + 2].BackColor = green;
                            break;

                        case "B":
                            e.Item.Cells[colIdxTrue + 3].BackColor = green;
                            break;
                    }
                    break;

                /* At this point, there is no "Correct Answer" for Short Answer or Essay type questions - probably because partial credit is regarded with these. */
                //case "S":  //Short Answer
                //    break;

                //case "E":  //Essay
                //    break;
            }

            /***************************************************************************
             * If test question has a distractor rationale, then answer columns for the
             * test question should be hyperlinks that will display a pie chart break
             * down of the answers chosen.
             * 
             * Until we get this working ht, let's just test it on Answer A
             **************************************************************************/
            string pieChartLink;
            if (dataItem["DistractorRationale"].ToString() != "" && QuestionType != "E" && QuestionType != "S")
            {
                pieChartLink = displayCountAsPieChartAnchor(dataItem);

                //e.Item.Cells[colIdxA + 2].Text = displayCountAsPieChartAnchor(dataItem);
                switch (QuestionType)
                {
                    case "MC5":

                        //When used to locate the right cells within the Item Databound event, we 
                        //must add two to our column names.
                        e.Item.Cells[colIdxA + 2].Text = pieChartLink.Replace("{*value*}", dataItem["AFreq#"].ToString());
                        e.Item.Cells[colIdxB + 2].Text = pieChartLink.Replace("{*value*}", dataItem["BFreq#"].ToString());
                        e.Item.Cells[colIdxC + 2].Text = pieChartLink.Replace("{*value*}", dataItem["CFreq#"].ToString());
                        e.Item.Cells[colIdxD + 2].Text = pieChartLink.Replace("{*value*}", dataItem["DFreq#"].ToString());
                        e.Item.Cells[colIdxE + 2].Text = pieChartLink.Replace("{*value*}", dataItem["EFreq#"].ToString());
                        e.Item.Cells[colIdxRight + 2].Text = pieChartLink.Replace("{*value*}", dataItem["RFreq#"].ToString());
                        e.Item.Cells[colIdxWrong + 2].Text = pieChartLink.Replace("{*value*}", dataItem["WFreq#"].ToString());
                        break;

                    case "MC4":

                        //When used to locate the right cells within the Item Databound event, we 
                        //must add two to our column names.
                        e.Item.Cells[colIdxA + 2].Text = pieChartLink.Replace("{*value*}", dataItem["AFreq#"].ToString());
                        e.Item.Cells[colIdxB + 2].Text = pieChartLink.Replace("{*value*}", dataItem["BFreq#"].ToString());
                        e.Item.Cells[colIdxC + 2].Text = pieChartLink.Replace("{*value*}", dataItem["CFreq#"].ToString());
                        e.Item.Cells[colIdxD + 2].Text = pieChartLink.Replace("{*value*}", dataItem["DFreq#"].ToString());
                        e.Item.Cells[colIdxRight + 2].Text = pieChartLink.Replace("{*value*}", dataItem["RFreq#"].ToString());
                        e.Item.Cells[colIdxWrong + 2].Text = pieChartLink.Replace("{*value*}", dataItem["WFreq#"].ToString());
                        break;


                    case "MC3":

                        //When used to locate the right cells within the Item Databound event, we 
                        //must add two to our column names.
                        e.Item.Cells[colIdxA + 2].Text = pieChartLink.Replace("{*value*}", dataItem["AFreq#"].ToString());
                        e.Item.Cells[colIdxB + 2].Text = pieChartLink.Replace("{*value*}", dataItem["BFreq#"].ToString());
                        e.Item.Cells[colIdxC + 2].Text = pieChartLink.Replace("{*value*}", dataItem["CFreq#"].ToString());
                        e.Item.Cells[colIdxRight + 2].Text = pieChartLink.Replace("{*value*}", dataItem["RFreq#"].ToString());
                        e.Item.Cells[colIdxWrong + 2].Text = pieChartLink.Replace("{*value*}", dataItem["WFreq#"].ToString());
                        break;

                    case "T":
                        e.Item.Cells[colIdxTrue + 2].Text = pieChartLink.Replace("{*value*}", dataItem["TFreq#"].ToString());
                        e.Item.Cells[colIdxFalse + 2].Text = pieChartLink.Replace("{*value*}", dataItem["FFreq#"].ToString());
                        e.Item.Cells[colIdxRight + 2].Text = pieChartLink.Replace("{*value*}", dataItem["RFreq#"].ToString());
                        e.Item.Cells[colIdxWrong + 2].Text = pieChartLink.Replace("{*value*}", dataItem["WFreq#"].ToString());
                        e.Item.Cells[colIdxRight + 2].Text = pieChartLink.Replace("{*value*}", dataItem["RFreq#"].ToString());
                        e.Item.Cells[colIdxWrong + 2].Text = pieChartLink.Replace("{*value*}", dataItem["WFreq#"].ToString());
                        break;

                    case "S":
                    case "E":
                        e.Item.Cells[colIdxRight + 2].Text = pieChartLink.Replace("{*value*}", dataItem["RFreq#"].ToString());
                        e.Item.Cells[colIdxWrong + 2].Text = pieChartLink.Replace("{*value*}", dataItem["WFreq#"].ToString());
                        break;
                }
            }
        }

        /// <summary>
        /// Constructs an anchor tag for the supplied datarow and field.  This anchor tag will display a custom dialog with a
        /// pie chart graphing the test question's selected answer demographics and distractor rational.  Returns the anchor
        /// tag as string so it can be placed in a grid cell to be displayed.
        /// </summary>
        /// <param name="di"></param>
        /// <param name="rowField"></param>
        /// <returns></returns>
        private string displayCountAsPieChartAnchor(DataRowView di)
        {
            sb.Clear();
            sb = new StringBuilder("<a href=\"javascript:void();\" onclick=\"customDialog({url:'../ControlHost/TestQuestionPieChart.aspx");
            sb.Append("?level=" + Request.QueryString["level"]);
            sb.Append("&levelID=" + Request.QueryString["levelID"]);
            sb.Append("&testID=" + Request.QueryString["testID"]);
            var rowItemContent = di["QNbr#_Sort"].ToString();
            sb.Append((rowItemContent.IndexOf(".") > 0) ? "&sort=" + rowItemContent.Substring(0, rowItemContent.IndexOf(".")) : "&sort=" + rowItemContent);
            sb.Append("&critOrides=" + ReportCriteria.GetCriteriaOverrideString());
            sb.Append("&formID=" + FormID);
            sb.Append("&parent=" + Request.QueryString["parent"]);
            sb.Append("&parentID=" + Request.QueryString["parentID"]);
            sb.Append("");
            sb.Append("', title: 'Distractor Rationale', maximize: true, maxwidth: 550, maxheight: 500 }); return false;\">{*value*}</a>");
            
            return sb.ToString();
        }

        private void loadColIdxArrays()
        {
            sb.Clear();
            sb.Append("var colAryMC = [");
            foreach (int i in colAryMC) sb.Append(i.ToString() + ", ");
            sb.Append("]; ");

            sb.Append("var colAryTF = [");
            foreach (int i in colAryTF) sb.Append(i.ToString() + ", ");
            sb.Append("]; ");

            sb.Append("var colAryRW = [");
            foreach (int i in colAryRW) sb.Append(i.ToString() + ", ");
            sb.Append("]; ");

            sb.Append("var colAryRubric = [");
            foreach (int i in colAryRubric) sb.Append(i.ToString() + ", ");
            sb.Append("]; ");

            sb.Replace(", ]", "]");

            RadScriptManager.RegisterStartupScript(Page, typeof(string), "Instantiate column index arrays", sb.ToString(), true);
        }
    }
}