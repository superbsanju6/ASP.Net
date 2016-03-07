using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Thinkgate.Classes;
using Telerik.Web.UI;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using WebSupergoo.ABCpdf9;
using System.Collections.Generic;
using System.IO;
using Thinkgate.Base.Classes; 
using ClosedXML.Excel;
using GemBox.Spreadsheet;
using Thinkgate.Domain.Classes;
using Thinkgate.Base.Enums.AssessmentScheduling;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.Reports
{
    public partial class DistractorAnalysis : TileControlBase
    {
        private DataTable _columnData;
        private DataSet _distractorAnalysisData;
        private DataTable _contentData;
       
        public string Guid;
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";
        string _demoLabel;
        string _demoVal;
        string _selectedFormID;
        string _selectedSortColumn;
        string _selectedSortMethod;
        private string _level;
        private string _levelID;
        private string _testID;
        private string _parent;
        private string _parentID;
        private Criteria _reportCriteria;
        private string _criteriaOverrides;
        private bool isPDFView;
        private bool _isExcel;
        private bool _highlightCorrectPDFParm;
        private bool _highlightIncorrectPDFParm;
        private bool _hideStudentName;
        private bool _hideStudentID;
        private bool _showRigorPDFParm;
        private bool _isContentLocked;
        private string _guidforPDFView;
                

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
           
            // TFS 1123 : Check the content is locked or unlocked for specific Test ID
            if (Request.QueryString["testID"] != null)
            {
                string contentLock;
                int testID = Convert.ToInt32(Request.QueryString["testID"]);
                DataRow row = Thinkgate.Base.Classes.Assessment.GetAssessmentSchedule(testID);

                if (row != null)
                {
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


            if (Request.QueryString["selectedReport"] != null && !String.IsNullOrEmpty(Request.QueryString["selectedReport"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["selectedReport"].ToString() + "' report", Request.QueryString["selectedReport"].ToString(), sessionObject.LoggedInUser.UserName);
            }

            _selectedFormID = rcbDistractorAnalysisForms.SelectedValue;
            isPDFView = Request.QueryString["printPDFView"] == "yes";
            _highlightCorrectPDFParm = Request.QueryString["highlightCorrect"] == "yes";
            _highlightIncorrectPDFParm = Request.QueryString["highlightIncorrect"] == "yes";
            _hideStudentName = Request.QueryString["hdStdName"] == "yes";
            _hideStudentID = Request.QueryString["hdStdID"] == "yes";
            _showRigorPDFParm = Request.QueryString["showRigor"] == "yes";
            _guidforPDFView = Request.QueryString["guid"];

            LoadCriteria();

            if (!isPDFView)
            {
                
                LoadReport();
                LoadFormButton();
            }
            highlightCorrect_checkbox.Checked = true;
            highlightIncorrect_checkbox.Checked = true;
            showStandards_checkbox.Checked = true;
            showRigor_checkbox.Checked = true;

            if (isPDFView)
            {
                var settings = new PdfRenderSettings();
                settings.PageHeight = 612;
                settings.PageWidth = 792;
                settings.HeaderHeight = 0;
                settings.FooterHeight = 10;
                settings.LeftMargin = 10;
                settings.RightMargin = 10;
                Doc doc = DistractorAnalysisToPdfDoc(settings);
                byte[] theData = doc.GetData();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
                Response.AddHeader("content-length", theData.Length.ToString());
                Response.BinaryWrite(theData);
            }
        }

        protected void RemoteReportReload(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Guid)) KillReport("No GUID for report criteria. <br/> Contact support.");
            if (Session["Criteria_" + Guid] == null) KillReport("Criteria session object lost. <br/> Contact support.");
            _reportCriteria = (Criteria)Session["Criteria_" + Guid];
            _criteriaOverrides = ((Criteria)_reportCriteria).GetCriteriaOverrideString();
            LoadReport();
        }

        private void KillReport(string message)
        {
            SessionObject.RedirectMessage = message;
            Response.Redirect("~/PortalSelection.aspx", true);
        }

        private void PreventChangeToReport(string message)
        {
            SessionObject.RecoverableRedirectMessage = message;
            Response.Redirect(Request.RawUrl);

            //if (Page.Master == null) return;
            //var radNotification = Page.Master.FindControl("genericRadNotification");

            //if (radNotification == null) return;

            //var js = "var radNotification1 = $find('" + radNotification.ClientID + "');";
            //js += "if (radNotification1) radNotification1.set_text = '" + message + "';";

            //Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "radnotification", js);

        }



        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlAtRiskStandardCriteria";

            if (Tile.TileParms.GetParm("CriteriaGUID") == null) return;

            Guid = (!string.IsNullOrEmpty(_guidforPDFView)) ? _guidforPDFView : (string)Tile.TileParms.GetParm("CriteriaGUID");

            if (Session["Criteria_" + Guid] == null) return;

            ctlReportCriteria.Guid = Guid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            criteriaPlaceholder.Controls.Add(ctlReportCriteria);

            _level = Request.QueryString["level"];
            _levelID = Request.QueryString["levelID"];
            _testID = Request.QueryString["testID"];
            _parent = Request.QueryString["parent"];
            _parentID = Request.QueryString["parentID"];
            _reportCriteria = (Criteria)Session["Criteria_" + Guid];
            _criteriaOverrides = ((Criteria)_reportCriteria).GetCriteriaOverrideString();
        }

        private DataSet GetDataSet()
        {
            //var dev = System.Configuration.ConfigurationManager.AppSettings.Get("Environment") == "Dev" ? true : false;
            //var userPage = dev ? 119 : 60528;
            var userPage = SessionObject.LoggedInUser.Page;
            var year = Thinkgate.Base.Classes.AppSettings.Demo_Year;

            if (_reportCriteria == null) return null;

            string demo_label = String.IsNullOrEmpty(_demoLabel) ? "" : _demoLabel;
            string demo_val = String.IsNullOrEmpty(_demoVal) ? "" : _demoVal;
            string formID = string.IsNullOrEmpty(_selectedFormID) ? "0" : _selectedFormID;

            _distractorAnalysisData = Thinkgate.Base.Classes.Reports.GetDistractorAnalysis(0, year, userPage, _permissions, "",
                                                                                    "", demo_label, demo_val,
                                                                                    "@@Product=none@@@@RR=none" +
                                                                                    _criteriaOverrides +
                                                                                    "1test=yes@@@@PT=1@@", 0, formID);


            return _distractorAnalysisData;
        }

        protected void chk_CheckedChanged(Object sender, EventArgs args)
        {
            if (sender is CheckBox)
            {
                if (sender == chkHideStudentName)
                {
                    reportGrid.MasterTableView.GetColumn("Name").Visible = !chkHideStudentName.Checked ? true : false;
                }
                else if (sender == chkHideStudentID)
                {
                    reportGrid.MasterTableView.GetColumn("StudentID").Visible = !chkHideStudentID.Checked ? true : false;
                }
            }
        }

        private DataTable GetContentDataTable()
        {
            if (_contentData == null || (!String.IsNullOrEmpty(_demoLabel) && !String.IsNullOrEmpty(_demoVal)) || !String.IsNullOrEmpty(_selectedFormID) || _isExcel)
            {
                _contentData = new DataTable();

                if (_distractorAnalysisData.Tables[0].Rows.Count > 0)
                {
                    int questionCount = _distractorAnalysisData.Tables[1].Rows.Count;
                    DataTable contentColumns = _distractorAnalysisData.Tables[1];

                    DataColumn name = new DataColumn("Name");
                    _contentData.Columns.Add(name);

                    DataColumn studentID = new DataColumn("StudentID");
                    _contentData.Columns.Add(studentID);

                    DataColumn score = new DataColumn("Score");
                    _contentData.Columns.Add(score);
                                            
                   // if(contentColumns.Columns.Contains("TR_Sort") && contentColumns.Columns.Contains("Sort"))
                    //{
                        foreach (DataRow row in contentColumns.Rows)
                        {
                            var newColName = row["TR_Sort"].ToString() != "0"
                                                 ? row["Sort"].ToString() + " r(" + row["TR_Sort"].ToString() + ")"
                                                 : row["Sort"].ToString();
                            DataColumn newCol = new DataColumn(newColName);
                            _contentData.Columns.Add(newCol);
                        }
                    //}

                    if (isPDFView || _isExcel)
                    {
                        DataColumn TestResponses = new DataColumn("TestResponses");
                        _contentData.Columns.Add(TestResponses);
                        DataColumn AltResponses = new DataColumn("AltResponses");
                        _contentData.Columns.Add(AltResponses);
                        DataColumn AnswerString = new DataColumn("AnswerString");
                        _contentData.Columns.Add(AnswerString);
                        DataColumn AdditionalData = new DataColumn("AdditionalData");
                        _contentData.Columns.Add(AdditionalData);
                    }

                    //Build Standards and Rigor Rows
                    DataRow standardsRow = _contentData.NewRow();
                    standardsRow[name] = "Standards";
                    standardsRow[score] = string.Empty;


                    DataRow rigorRow = _contentData.NewRow();
                    rigorRow[name] = _distractorAnalysisData.Tables[0].Rows[0]["RigorType"].ToString();
                    rigorRow[score] = string.Empty;


                    showRigor_checkbox.Value = _distractorAnalysisData.Tables[0].Rows[0]["RigorType"].ToString();
                    showRigor_span.InnerHtml = "Show " + _distractorAnalysisData.Tables[0].Rows[0]["RigorType"].ToString();

                    SessionObject.PreviewStandardsDialogParms.Clear();

                    foreach (DataRow contentRow in contentColumns.Rows)
                    {
                        var colName = contentRow["TR_Sort"].ToString() != "0"
                                             ? contentRow["Sort"].ToString() + " r(" + contentRow["TR_Sort"].ToString() + ")"
                                             : contentRow["Sort"].ToString();
                        ImageButton img = new ImageButton();
                        img.ImageUrl = "~/Images/preview1.gif";
                        string standardID_Encrypted = Encryption.EncryptInt(DataIntegrity.ConvertToInt(contentRow["StandardID"]));

                        Thinkgate.Base.Classes.Standards standard = new Thinkgate.Base.Classes.Standards(DataIntegrity.ConvertToInt(contentRow["StandardID"]),
                            contentRow["StandardName"].ToString(), contentRow["SText"].ToString(), 0, string.Empty);
                        
                        if (!SessionObject.PreviewStandardsDialogParms.ContainsKey(DataIntegrity.ConvertToInt(contentRow["StandardID"])))
                        {
                            SessionObject.PreviewStandardsDialogParms.Add(DataIntegrity.ConvertToInt(contentRow["StandardID"]), standard);
                        }

                        standardsRow[colName] = "<img src=\"" + img.ResolveClientUrl(img.ImageUrl)
                            + "\" onclick=\"customDialog({ maximize_height:true, maxheight: 400,url: '../ControlHost/PreviewStandard.aspx?xID=" + standardID_Encrypted + "' }); return false;\" style=\"cursor:pointer;height:100%;\" title=\""
                            + contentRow["StandardName"].ToString() + ": " + contentRow["SText_Hover"].ToString() + "\"/>";

                        rigorRow[colName] = contentRow["qrigor"].ToString();
                    }
                    _contentData.Rows.Add(standardsRow);
                    _contentData.Rows.Add(rigorRow);

                    //Build Answer Rows
                    foreach (DataRow row in _distractorAnalysisData.Tables[0].Rows)
                    {
                        DataRow newRow = _contentData.NewRow();
                        newRow[name] = row["Student_Name"].ToString().Length > 0 ? row["Student_Name"].ToString() : row["Demo_Name"].ToString();
                        newRow[studentID] = row["StudentID"].ToString();
                        newRow[score] = row["Student_Name"].ToString().Length > 0 && !isPDFView && !_isExcel ? row["PctCorrectDisp_SORT"].ToString() : row["PctCorrectDisp"].ToString();

                        string testResponses = row["TestResponses"].ToString();
                        string altResponses = row["AltResponses"].ToString();
                        string answerString = row["AnswerString"].ToString();
                        int counter = 0;

                        if (isPDFView || _isExcel)
                        {
                            newRow["TestResponses"] = row["TestResponses"].ToString();
                            newRow["AltResponses"] = row["AltResponses"].ToString();
                            newRow["AnswerString"] = row["AnswerString"].ToString();
                            newRow["AdditionalData"] = row["AdditionalData"].ToString();
                        }

                        if (answerString.Length > 0) //For overall average and demographic rows
                        {
                            foreach (DataRow contentRow in contentColumns.Rows)
                            {
                                int sort = counter + 1;
                                var rubricID = contentRow["RubricID"].ToString();
                                var trSort = DataIntegrity.ConvertToInt(contentRow["TR_Sort"]);
                                var sortColName = trSort > 0
                                                 ? contentRow["Sort"].ToString() + " r(" + trSort.ToString() + ")"
                                                 : contentRow["Sort"].ToString();
                                var maxPoints = DataIntegrity.ConvertToInt(contentRow["MaxPoints"]);
                                var masterSort = DataIntegrity.ConvertToInt(contentRow["MasterSort"]) - 1;
                                string[] answers = new string[contentColumns.Rows.Count];

                                answers = answerString.Split(';');

                                if (!string.IsNullOrEmpty(contentRow["DistractorRationale"].ToString()) && !_isExcel && !isPDFView)
                                {
                                        var js =
                                            "customDialog({ url: '../ControlHost/TestQuestionPieChart.aspx?level=" +
                                            _level + "&levelID=" + _levelID + "&testID=" + _testID +
                                        "&sort=" + contentRow["Sort"].ToString() + "&critOrides=" + _criteriaOverrides +
                                            "&formID=" + rcbDistractorAnalysisForms.SelectedValue +
                                            "&parent=" + _parent + "&parentID=" + _parentID +
                                            "', title: 'Distractor Rationale', maximize: true, maxwidth: 550, maxheight: 500 }); return false;";

                                    newRow[sortColName] = "<a style=\"color: blue;\" href=\"javascript:void();\" onclick=\"" + js +
                                                                  "\">" + answers[counter] + "</a>";
                                    }
                                    else
                                    {
                                        newRow[sortColName] = answers[counter];

                                    }

                                    counter++;
                            }
                        }
                        else if (testResponses.Length > 0)
                        {
                            if (altResponses.Length == 0) continue;
                            var additionalData = row["AdditionalData"].ToString();
                            foreach (DataRow contentRow in contentColumns.Rows)
                            {
                                int sort = counter + 1;
                                var scoreOnTest = contentRow["ScoreOnTest"].ToString();
                                var rubricID = contentRow["RubricID"].ToString();
                                var trSort = DataIntegrity.ConvertToInt(contentRow["TR_Sort"]);
                                var sortColName = trSort > 0
                                                 ? contentRow["Sort"].ToString() + " r(" + trSort.ToString() + ")"
                                                 : contentRow["Sort"].ToString();
                                var maxPoints = DataIntegrity.ConvertToInt(contentRow["MaxPoints"]);
                                var masterSort = DataIntegrity.ConvertToInt(contentRow["MasterSort"]);
                                var answer = string.Empty;
                                //var testResponse = testResponses.Substring((masterSort-1), 1);
                                //var altResponse = altResponses.Substring(masterSort == 1 ? 0 : (masterSort*3) - 3, 3);
                                var testResponse = testResponses.Length >= masterSort ? testResponses.Substring((masterSort - 1), 1) : testResponses.Substring(0, 1);
                                var altResponse = altResponses.Length > ((masterSort * 3) - 3) ? altResponses.Substring((masterSort * 3) - 3, 3) : altResponses.Substring(0, 3);
                                var altResponseDisplay = string.Empty;
                                var questionType = contentRow["QuestionType"].ToString();
                                var scoreType = contentRow["ScoreType"].ToString();
                                var questionID = DataIntegrity.ConvertToInt(contentRow["QuestionID"]);
                                var rubricPointListString = string.Empty;
                                var temp = string.Empty;

                                var rubricPointList = new string[] { };
                                var rubricPointCount = 0;

                                if (additionalData.Length > 0 && additionalData.IndexOf("score_" + questionID + "=,") > -1)
                                {
                                    rubricPointListString = additionalData.Substring(additionalData.IndexOf("score_" + questionID + "=,") + ("score_" + questionID + "=,").Length);
                                    if (rubricPointListString.IndexOf(",|?|") > -1) rubricPointListString = rubricPointListString.Substring(0, rubricPointListString.IndexOf(",|?|"));
                                    temp = rubricPointListString;

                                    while (rubricPointListString.IndexOf("_") > -1)
                                    {
                                        rubricPointCount++;
                                        rubricPointListString = rubricPointListString.Substring(rubricPointListString.IndexOf("_") + 1);
                                    }
                                }

                                switch (altResponse)
                                {
                                    case "000":
                                        altResponseDisplay = "0%";
                                        break;
                                    case "100":
                                        altResponseDisplay = "100%";
                                        break;
                                    default:
                                        if (altResponse.Substring(0, 1) == "0")
                                        {
                                            altResponseDisplay = altResponse.Substring(1, 2) + "%";
                                        }
                                        else
                                        {
                                            altResponseDisplay = "%";
                                        }
                                        break;
                                }

                                if (altResponse == "***")
                                {
                                    answer = "ns";
                                }
                                else if (testResponse == "*" && (questionType == "MC3" || questionType == "MC4" || questionType == "MC5" || questionType == "T"))
                                {
                                    answer = "x";
                                }
                                else if (testResponse == "*")
                                {
                                    if (rubricID != "0")
                                    {
                                        string scored = "0";

                                        List<int> list = new List<int>();
                                        while (temp.IndexOf("_") > -1)
                                        {
                                            temp = temp.Substring(temp.IndexOf("_") + 1);
                                            scored = (Convert.ToInt32(scored) + Convert.ToInt32(temp.Substring(0, 1))).ToString();
                                            list.Add(Convert.ToInt32(temp.Substring(0, 1)));
                                        }

                                        if (trSort > 0)
                                        {
                                            // overall percentage is altReponseDisplay on all rubric criteria, now get the rubric point set up for each criteria
                                            //int totalRubricCount = Convert.ToInt32(scored) * 100 / Convert.ToInt32( altResponseDisplay.Substring(0,altResponseDisplay.IndexOf("%")));
                                            int totalRubricCount = maxPoints;
                                            // Since altResponseDisplay returns round figure of percentage   
                                            if ((Convert.ToInt32(scored) * 100 / totalRubricCount) != Convert.ToInt32(altResponseDisplay.Substring(0, altResponseDisplay.IndexOf("%"))))
                                                totalRubricCount = (Convert.ToInt32(scored) * 100) / (Convert.ToInt32(altResponseDisplay.Substring(0, altResponseDisplay.IndexOf("%"))) - 1);

                                            int rubricCount = totalRubricCount / list.Count;

                                            if (list.Count > 1)
                                            {
                                                double percentage = (double)((double)list[trSort - 1] * 100 / (double)rubricCount);
                                                answer = list[trSort - 1].ToString() + "(" + Math.Round(percentage).ToString() + "%" + ")";
                                            }
                                            else
                                                answer = list[trSort - 1].ToString() + "(" + altResponseDisplay + ")";

                                        }
                                        else
                                        {
                                            // sum of scored point
                                            answer = scored + "(" + altResponseDisplay + ")";
                                        }
                                    }
                                    else
                                    {
                                        answer = altResponseDisplay;
                                    }
                                }
                                else
                                {
                                    answer = testResponse;
                                }

                                if (_isExcel || isPDFView)
                                {
                                    newRow[sortColName] = answer + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "");
                                }
                                else if (!string.IsNullOrEmpty(contentColumns.Rows[counter]["DistractorRationale"].ToString()))
                                {
                                    newRow[sortColName] = testResponses.Length > (counter) ? testResponses.Substring(counter, 1) : testResponses.Substring(0, 1); // testResponses.Substring(counter, 1);
                                    var js = "customDialog({ url: '../ControlHost/TestQuestionPieChart.aspx?level=" + _level + "&levelID=" + _levelID + "&testID=" + _testID +
                                        "&sort=" + contentRow["Sort"].ToString() + "&critOrides=" + _criteriaOverrides + "&formID=" + rcbDistractorAnalysisForms.SelectedValue +
                                        "&parent=" + _parent + "&parentID=" + _parentID + "', title: 'Distractor Rationale', maximize: true, maxwidth: 550, maxheight: 500 }); return false;";

                                    var color = string.Empty;
                                    if ((scoreType == "P" || scoreType == "R")
                                        && altResponse != "000" && altResponse != "100")
                                        color = "black";
                                    else
                                        color = "white";
                                        
                                    //newRow[sort.ToString()] = "<a href=\"javascript:void();\" studentRow=\"true\" onclick=\"" + js + "\">" + answer + (row["ScoreOnTest"].ToString == "No" ? "*" : "") + "</a>";
                                    newRow[sortColName] = "<a style=\"color: " + color + ";\" href=\"javascript:void();\" studentRow=\"true\" onclick=\"" + js + "\">" + answer + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "") + "</a>";
                                }
                                else
                                {
                                    newRow[sortColName] = "<span studentRow=\"true\">" + answer + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "") + "</span>";
                                }

                                counter++;
                            }
                        }
                        else
                        {
                            if (testResponses.Length == 0) continue;
                            if (altResponses.Length == 0) continue;

                            foreach (DataRow contentRow in contentColumns.Rows)
                            {
                                var trSort = DataIntegrity.ConvertToInt(contentRow["TR_Sort"]);
                                var sortColName = trSort > 0
                                                 ? contentRow["Sort"].ToString() + " r(" + trSort.ToString() + ")"
                                                 : contentRow["Sort"].ToString();
                                newRow[sortColName] = testResponses.Substring(counter, 1) + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "");

                                counter++;
                            }
                        }

                        _contentData.Rows.Add(newRow);
                    }
                }
            }

            return _contentData;
        }
        private DataTable GetContentDataTable_ExportToExcel()
        {
            if (_contentData == null || (!String.IsNullOrEmpty(_demoLabel) && !String.IsNullOrEmpty(_demoVal)) || !String.IsNullOrEmpty(_selectedFormID) || _isExcel)
            {
                _contentData = new DataTable();

                if (_distractorAnalysisData.Tables[0].Rows.Count > 0)
                {
                    int questionCount = _distractorAnalysisData.Tables[1].Rows.Count;
                    DataTable contentColumns = _distractorAnalysisData.Tables[1];

                    DataColumn name = new DataColumn("Name");
                    _contentData.Columns.Add(name);

                    DataColumn studentID = new DataColumn("StudentID");
                    _contentData.Columns.Add(studentID);

                    DataColumn score = new DataColumn("Score");
                    _contentData.Columns.Add(score);

                    // if(contentColumns.Columns.Contains("TR_Sort") && contentColumns.Columns.Contains("Sort"))
                    //{
                    foreach (DataRow row in contentColumns.Rows)
                    {
                        var newColName = row["TR_Sort"].ToString() != "0"
                                             ? row["Sort"].ToString() + " r(" + row["TR_Sort"].ToString() + ")"
                                             : row["Sort"].ToString();
                        DataColumn newCol = new DataColumn(newColName);
                        _contentData.Columns.Add(newCol);
                    }
                    //}

                    if (isPDFView || _isExcel)
                    {
                        DataColumn TestResponses = new DataColumn("TestResponses");
                        _contentData.Columns.Add(TestResponses);
                        DataColumn AltResponses = new DataColumn("AltResponses");
                        _contentData.Columns.Add(AltResponses);
                        DataColumn AnswerString = new DataColumn("AnswerString");
                        _contentData.Columns.Add(AnswerString);
                        DataColumn AdditionalData = new DataColumn("AdditionalData");
                        _contentData.Columns.Add(AdditionalData);
                    }

                    //Build Standards and Rigor Rows
                    DataRow standardsRow = _contentData.NewRow();
                    standardsRow[name] = "Standards";

                    foreach (DataRow row in _distractorAnalysisData.Tables[0].Rows)
                    {
                        standardsRow[score] = row["PctCorrectDisp"].ToString();


                    }



                    DataRow rigorRow = _contentData.NewRow();
                    rigorRow[name] = _distractorAnalysisData.Tables[0].Rows[0]["RigorType"].ToString();

                    foreach (DataRow row in _distractorAnalysisData.Tables[0].Rows)
                    {
                        rigorRow[score] = row["PctCorrectDisp"].ToString();


                    }

                    showRigor_checkbox.Value = _distractorAnalysisData.Tables[0].Rows[0]["RigorType"].ToString();
                    showRigor_span.InnerHtml = "Show " + _distractorAnalysisData.Tables[0].Rows[0]["RigorType"].ToString();

                    SessionObject.PreviewStandardsDialogParms.Clear();

                    foreach (DataRow contentRow in contentColumns.Rows)
                    {
                        var colName = contentRow["TR_Sort"].ToString() != "0"
                                             ? contentRow["Sort"].ToString() + " r(" + contentRow["TR_Sort"].ToString() + ")"
                                             : contentRow["Sort"].ToString();
                        ImageButton img = new ImageButton();
                        img.ImageUrl = "~/Images/preview1.gif";
                        string standardID_Encrypted = Encryption.EncryptInt(DataIntegrity.ConvertToInt(contentRow["StandardID"]));

                        Thinkgate.Base.Classes.Standards standard = new Thinkgate.Base.Classes.Standards(DataIntegrity.ConvertToInt(contentRow["StandardID"]),
                            contentRow["StandardName"].ToString(), contentRow["SText"].ToString(), 0, string.Empty);

                        if (!SessionObject.PreviewStandardsDialogParms.ContainsKey(DataIntegrity.ConvertToInt(contentRow["StandardID"])))
                        {
                            SessionObject.PreviewStandardsDialogParms.Add(DataIntegrity.ConvertToInt(contentRow["StandardID"]), standard);
                        }

                        standardsRow[colName] = "<img src=\"" + img.ResolveClientUrl(img.ImageUrl)
                            + "\" onclick=\"customDialog({ maximize_height:true, maxheight: 400,url: '../ControlHost/PreviewStandard.aspx?xID=" + standardID_Encrypted + "' }); return false;\" style=\"cursor:pointer;height:100%;\" title=\""
                            + contentRow["StandardName"].ToString() + ": " + contentRow["SText_Hover"].ToString() + "\"/>";

                        rigorRow[colName] = contentRow["qrigor"].ToString();
                    }
                    _contentData.Rows.Add(standardsRow);
                    _contentData.Rows.Add(rigorRow);

                    //Build Answer Rows
                    foreach (DataRow row in _distractorAnalysisData.Tables[0].Rows)
                    {
                        DataRow newRow = _contentData.NewRow();
                        newRow[name] = row["Student_Name"].ToString().Length > 0 ? row["Student_Name"].ToString() : row["Demo_Name"].ToString();
                        newRow[studentID] = row["StudentID"].ToString();
                        newRow[score] = row["Student_Name"].ToString().Length > 0 && !isPDFView && !_isExcel ? row["PctCorrectDisp_SORT"].ToString() : row["PctCorrectDisp"].ToString();

                        string testResponses = row["TestResponses"].ToString();
                        string altResponses = row["AltResponses"].ToString();
                        string answerString = row["AnswerString"].ToString();
                        int counter = 0;

                        if (isPDFView || _isExcel)
                        {
                            newRow["TestResponses"] = row["TestResponses"].ToString();
                            newRow["AltResponses"] = row["AltResponses"].ToString();
                            newRow["AnswerString"] = row["AnswerString"].ToString();
                            newRow["AdditionalData"] = row["AdditionalData"].ToString();
                        }

                        if (answerString.Length > 0) //For overall average and demographic rows
                        {
                            foreach (DataRow contentRow in contentColumns.Rows)
                            {
                                int sort = counter + 1;
                                var rubricID = contentRow["RubricID"].ToString();
                                var trSort = DataIntegrity.ConvertToInt(contentRow["TR_Sort"]);
                                var sortColName = trSort > 0
                                                 ? contentRow["Sort"].ToString() + " r(" + trSort.ToString() + ")"
                                                 : contentRow["Sort"].ToString();
                                var maxPoints = DataIntegrity.ConvertToInt(contentRow["MaxPoints"]);
                                var masterSort = DataIntegrity.ConvertToInt(contentRow["MasterSort"]) - 1;
                                string[] answers = new string[contentColumns.Rows.Count];

                                answers = answerString.Split(';');

                                if (!string.IsNullOrEmpty(contentRow["DistractorRationale"].ToString()) && !_isExcel && !isPDFView)
                                {
                                    var js =
                                        "customDialog({ url: '../ControlHost/TestQuestionPieChart.aspx?level=" +
                                        _level + "&levelID=" + _levelID + "&testID=" + _testID +
                                        "&sort=" + contentRow["Sort"].ToString() + "&critOrides=" + _criteriaOverrides +
                                        "&formID=" + rcbDistractorAnalysisForms.SelectedValue +
                                        "&parent=" + _parent + "&parentID=" + _parentID +
                                        "', title: 'Distractor Rationale', maximize: true, maxwidth: 550, maxheight: 500 }); return false;";

                                    newRow[sortColName] = "<a style=\"color: blue;\" href=\"javascript:void();\" onclick=\"" + js +
                                                              "\">" + answers[counter] + "</a>";
                                }
                                else
                                {
                                    newRow[sortColName] = answers[counter];

                                }

                                counter++;
                            }
                        }
                        else if (testResponses.Length > 0)
                        {
                            if (altResponses.Length == 0) continue;
                            var additionalData = row["AdditionalData"].ToString();
                            foreach (DataRow contentRow in contentColumns.Rows)
                            {
                                int sort = counter + 1;
                                var scoreOnTest = contentRow["ScoreOnTest"].ToString();
                                var rubricID = contentRow["RubricID"].ToString();
                                var trSort = DataIntegrity.ConvertToInt(contentRow["TR_Sort"]);
                                var sortColName = trSort > 0
                                                 ? contentRow["Sort"].ToString() + " r(" + trSort.ToString() + ")"
                                                 : contentRow["Sort"].ToString();
                                var maxPoints = DataIntegrity.ConvertToInt(contentRow["MaxPoints"]);
                                var masterSort = DataIntegrity.ConvertToInt(contentRow["MasterSort"]);
                                var answer = string.Empty;
                                //var testResponse = testResponses.Substring((masterSort-1), 1);
                                //var altResponse = altResponses.Substring(masterSort == 1 ? 0 : (masterSort*3) - 3, 3);
                                var testResponse = testResponses.Length >= masterSort ? testResponses.Substring((masterSort - 1), 1) : testResponses.Substring(0, 1);
                                var altResponse = altResponses.Length > ((masterSort * 3) - 3) ? altResponses.Substring((masterSort * 3) - 3, 3) : altResponses.Substring(0, 3);
                                var altResponseDisplay = string.Empty;
                                var questionType = contentRow["QuestionType"].ToString();
                                var scoreType = contentRow["ScoreType"].ToString();
                                var questionID = DataIntegrity.ConvertToInt(contentRow["QuestionID"]);
                                var rubricPointListString = string.Empty;
                                var temp = string.Empty;

                                var rubricPointList = new string[] { };
                                var rubricPointCount = 0;

								if (additionalData.Length > 0 && additionalData.IndexOf("score_" + questionID + "=,") > -1)
                                {
                                    rubricPointListString = additionalData.Substring(additionalData.IndexOf("score_" + questionID + "=,") + ("score_" + questionID + "=,").Length);
									if (rubricPointListString.IndexOf(",|?|") > -1) rubricPointListString = rubricPointListString.Substring(0, rubricPointListString.IndexOf(",|?|"));
                                    temp = rubricPointListString;

                                    while (rubricPointListString.IndexOf("_") > -1) 
									{
										rubricPointCount++;
										rubricPointListString = rubricPointListString.Substring(rubricPointListString.IndexOf("_") + 1);
									}
                                }

                                switch (altResponse)
                                {
                                    case "000":
                                        altResponseDisplay = "0%";
                                        break;
                                    case "100":
                                        altResponseDisplay = "100%";
                                        break;
                                    default:
                                        if (altResponse.Substring(0, 1) == "0")
                                        {
                                            altResponseDisplay = altResponse.Substring(1, 2) + "%";
                                        }
                                        else
                                        {
                                            altResponseDisplay = "%";
                                        }
                                        break;
                                }

                                if (altResponse == "***")
                                {
                                    answer = "ns";
                                }
                                else if (testResponse == "*" && (questionType == "MC3" || questionType == "MC4" || questionType == "MC5" || questionType == "T"))
                                {
                                    answer = "x";
                                }
                                else if (testResponse == "*")
                                {
                                    if (rubricID != "0")
                                    {
                                        string scored = "0";

                                        List<int> list = new List<int>();
                                        while (temp.IndexOf("_") > -1)
                                        {
                                            temp = temp.Substring(temp.IndexOf("_") + 1);
                                            scored = (Convert.ToInt32(scored) + Convert.ToInt32(temp.Substring(0, 1))).ToString();
                                            list.Add(Convert.ToInt32(temp.Substring(0, 1)));
                                        }

                                        if (trSort > 0)
                                        {
                                            // overall percentage is altReponseDisplay on all rubric criteria, now get the rubric point set up for each criteria
                                            //int totalRubricCount = Convert.ToInt32(scored) * 100 / Convert.ToInt32( altResponseDisplay.Substring(0,altResponseDisplay.IndexOf("%")));
                                            int totalRubricCount = maxPoints;
                                            // Since altResponseDisplay returns round figure of percentage   
                                            if ((Convert.ToInt32(scored) * 100 / totalRubricCount) != Convert.ToInt32(altResponseDisplay.Substring(0, altResponseDisplay.IndexOf("%"))))
                                                totalRubricCount = (Convert.ToInt32(scored) * 100) / (Convert.ToInt32(altResponseDisplay.Substring(0, altResponseDisplay.IndexOf("%"))) - 1);

                                            int rubricCount = totalRubricCount / list.Count;

                                            if (list.Count > 1)
                                            {
                                                double percentage = (double)((double)list[trSort - 1] * 100 / (double)rubricCount);
                                                answer = list[trSort - 1].ToString() + "(" + Math.Round(percentage).ToString() + "%" + ")";
                                            }
                                            else
                                                answer = list[trSort - 1].ToString() + "(" + altResponseDisplay + ")";
                                            
                                        }
                                        else
                                        {
                                           // sum of scored point
                                            answer = scored + "(" + altResponseDisplay + ")";
										}
                                    }
                                    else
                                    {
                                        answer = altResponseDisplay;
                                    }
                                }
                                else
                                {
                                    answer = testResponse;
                                }

                                if (_isExcel || isPDFView)
                                {
                                    newRow[sortColName] = answer + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "");
                                }
                                else if (!string.IsNullOrEmpty(contentColumns.Rows[counter]["DistractorRationale"].ToString()))
                                {
                                    newRow[sortColName] = testResponses.Length > (counter) ? testResponses.Substring(counter, 1) : testResponses.Substring(0, 1); // testResponses.Substring(counter, 1);
                                    var js = "customDialog({ url: '../ControlHost/TestQuestionPieChart.aspx?level=" + _level + "&levelID=" + _levelID + "&testID=" + _testID +
                                        "&sort=" + contentRow["Sort"].ToString() + "&critOrides=" + _criteriaOverrides + "&formID=" + rcbDistractorAnalysisForms.SelectedValue +
                                        "&parent=" + _parent + "&parentID=" + _parentID + "', title: 'Distractor Rationale', maximize: true, maxwidth: 550, maxheight: 500 }); return false;";

                                    var color = string.Empty;
                                    if ((scoreType == "P" || scoreType == "R")
                                        && altResponse != "000" && altResponse != "100")
                                        color = "black";
                                    else
                                        color = "white";

                                    //newRow[sort.ToString()] = "<a href=\"javascript:void();\" studentRow=\"true\" onclick=\"" + js + "\">" + answer + (row["ScoreOnTest"].ToString == "No" ? "*" : "") + "</a>";
                                    newRow[sortColName] = "<a style=\"color: " + color + ";\" href=\"javascript:void();\" studentRow=\"true\" onclick=\"" + js + "\">" + answer + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "") + "</a>";
                                }
                                else
                                {
                                    newRow[sortColName] = "<span studentRow=\"true\">" + answer + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "") + "</span>";
                                }

                                counter++;
                            }
                        }
                        else
                        {
                            if (testResponses.Length == 0) continue;
                            if (altResponses.Length == 0) continue;

                            foreach (DataRow contentRow in contentColumns.Rows)
                            {
                                var trSort = DataIntegrity.ConvertToInt(contentRow["TR_Sort"]);
                                var sortColName = trSort > 0
                                                 ? contentRow["Sort"].ToString() + " r(" + trSort.ToString() + ")"
                                                 : contentRow["Sort"].ToString();
                                newRow[sortColName] = testResponses.Substring(counter, 1) + (contentRow["ScoreOnTest"].ToString() == "No" ? "*" : "");

                                counter++;
                            }
                        }

                        _contentData.Rows.Add(newRow);
                    }
                }
            }

            return _contentData;
        }
        private void SetNoRecords(bool recordsExist)
        {
            if (!recordsExist)
            {
                reportGrid.DataSource = null;
                reportGrid.DataBind();
            }

            reportGrid.Visible = recordsExist;
            lblNoRecords.Visible = !recordsExist;
        }

        private void LoadReport()
        {

            chkHideStudentName_checkbox.Checked = false;
            chkHideStudentID_checkbox.Checked = false;
            highlightCorrect_checkbox.Checked = true;
            highlightIncorrect_checkbox.Checked = true;
            showStandards_checkbox.Checked = true;
            showRigor_checkbox.Checked = true;

            DataSet ds = GetDataSet();
            DataTable contentTable = GetContentDataTable();

            if (ds == null || ds.Tables.Count < 2 || contentTable.Rows.Count == 0)
            {
                SetNoRecords(false);
                return;
            }
            else
            {
                SetNoRecords(true);
            }

            if (contentTable.Rows.Count > 2) SetLabels(ds.Tables[0].Rows[1]);

            _columnData = ds.Tables[1];

            reportGrid.DataSource = contentTable;
            reportGrid.DataBind();
        }

        private HtmlTable LoadPDFReport()
        {
            DataSet ds = GetDataSet();
            DataTable contentTable = GetContentDataTable();

            if (ds == null || ds.Tables.Count < 2 || contentTable.Rows.Count == 0 || ds.Tables[1].Rows.Count < 1)
                return new HtmlTable();

            if (contentTable.Rows.Count > 2) SetLabels(ds.Tables[0].Rows[1]);

            _columnData = ds.Tables[1];

            //Create PDF friendly grid
            var table = new HtmlTable();
            var headRow = new HtmlTableRow();
            table.Border = 1;
            table.Attributes["style"] = "border:solid 1px #000; font-family:Arial, sans-serif; border-collapse:collapse;";
            var colIndex = 0;
            foreach (DataColumn column in contentTable.Columns)
            {
                if (column.ColumnName == "TestResponses" || column.ColumnName == "AltResponses" || column.ColumnName == "AnswerString" || column.ColumnName == "AdditionalData" || (column.ColumnName.ToLower() == "studentid" && _hideStudentID))
                    continue;

                var headCell = new HtmlTableCell();
                var headCellBGColorStyle = DataIntegrity.ConvertToBool(_columnData.Rows[colIndex]["FieldTest"]) ? " background-color:#FFFF00;" : " background-color:#D0D0D0;";
                headCell.InnerHtml = column.ColumnName;
                headCell.Attributes["style"] = "padding:5px; font-weight:bold;" + headCellBGColorStyle +
                                               (column.ColumnName == "Name" ? " width:150px;" : column.ColumnName == "StudentID" ? " width:100px;" : string.Empty);
                headRow.Cells.Add(headCell);

                if (column.ColumnName != "Name" && column.ColumnName != "StudentID" && column.ColumnName != "Score")
                    colIndex++;
            }

            table.Rows.Add(headRow);

            foreach (DataRow row in contentTable.Rows)
            {
                if (row["Name"].ToString() == "Standards") continue;
                if (row["Name"].ToString() == "RBT" && !_showRigorPDFParm) continue;
                var newRow = new HtmlTableRow();
                var cellIndex = 0;
                foreach (DataColumn column in contentTable.Columns)
                {
                    if (column.ColumnName == "TestResponses" || column.ColumnName == "AltResponses" || column.ColumnName == "AnswerString" || column.ColumnName == "AdditionalData" || (column.ColumnName.ToLower() == "studentid" && _hideStudentID))
                        continue;

                    var testResponses = row["TestResponses"].ToString();
                    var altResponses = row["AltResponses"].ToString();
                    var answerString = row["AnswerString"].ToString();
                    var additionalData = row["AdditionalData"].ToString();
                    var cell = new HtmlTableCell();
                    var cellData = string.IsNullOrEmpty(row[column.ColumnName].ToString()) ? "&nbsp;" : (column.ColumnName.ToLower() == "name" && _hideStudentName && row["TestResponses"].ToString() != "") ? "&nbsp;" : row[column.ColumnName].ToString();
                    cell.InnerHtml = cellData;
                    cell.Attributes["style"] = "padding:5px; white-space:nowrap;" + (answerString.Length > 0 ? " font-size:xx-small;" : "");

                    if (testResponses.Length > 0 && altResponses.Length > 0 && column.ColumnName != "Name" && column.ColumnName != "StudentID" && column.ColumnName != "Score")
                    {
                        var fieldTest = _columnData.Rows[cellIndex]["FieldTest"].ToString() == "1";
                        var displayFieldTestItemData = _columnData.Rows[cellIndex]["DisplayFieldTestItemData"].ToString() == "yes";
                        var cellBGColor = System.Drawing.Color.Transparent;
                        var trSort = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["TR_Sort"]);
                        var maxPoints = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["MaxPoints"]);
                        var masterSort = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["MasterSort"]);
                        var answer = string.Empty;
                        var testResponse = testResponses.Substring(masterSort - 1, 1);
                        var altResponse = altResponses.Substring(masterSort == 1 ? 0 : (masterSort * 3) - 3, 3);
                        var questionType = _columnData.Rows[cellIndex]["QuestionType"].ToString();
                        var scoreType = _columnData.Rows[cellIndex]["ScoreType"].ToString();
                        var questionId = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["QuestionID"]);
                        var rubricPointListString = string.Empty;
                        var rubricPointList = new string[] { };
                        var rubricPointCount = 0;

                        if (additionalData.Length > 0 && additionalData.IndexOf("score_" + questionId + "=,") > -1)
                        {
                            rubricPointListString = additionalData.Substring(additionalData.IndexOf("score_" + questionId + "=,") + ("score_" + questionId + "=,").Length);
                            rubricPointListString = rubricPointListString.Substring(0, rubricPointListString.IndexOf(",|?|"));
                            rubricPointList = rubricPointListString.Split(',');
                            for (var i = 0; i < rubricPointList.Length; i++)
                            {
                                var points = DataIntegrity.ConvertToInt(rubricPointList[i].Substring(rubricPointList[i].IndexOf("_") + 1));
                                rubricPointCount = rubricPointCount + points;
                            }
                        }

                        if (altResponse != "***" && testResponse == "*" &&
                            (questionType == "MC3" || questionType == "MC4" || questionType == "MC5" || questionType == "T"))
                        {
                            answer = "x";
                        }

                        //Logic from legacy system.
                        if (!displayFieldTestItemData && fieldTest)
                            cellBGColor = System.Drawing.Color.FromName("Gray");
                        else if (trSort > 0 && rubricPointCount == maxPoints && _highlightCorrectPDFParm)
                            cellBGColor = System.Drawing.Color.FromName("Green");
                        else if (trSort > 0 && rubricPointCount == maxPoints && !_highlightCorrectPDFParm)
                            cellBGColor = System.Drawing.Color.Transparent;
                        else if (trSort > 0 && rubricPointCount == 0 && _highlightIncorrectPDFParm)
                            cellBGColor = System.Drawing.Color.FromName("Red");
                        else if (trSort > 0 && rubricPointCount == 0 && !_highlightIncorrectPDFParm)
                            cellBGColor = System.Drawing.Color.Transparent;
                        else if (altResponse == "100" && _highlightCorrectPDFParm)
                            cellBGColor = System.Drawing.Color.FromName("Green");
                        else if (altResponse == "100" && !_highlightCorrectPDFParm)
                            cellBGColor = System.Drawing.Color.Transparent;
                        else if (altResponse == "***")
                            cellBGColor = System.Drawing.Color.FromName("LightGray");
                        else if (answer == "x")
                            cellBGColor = System.Drawing.Color.FromName("Gray");
                        else if (altResponse == "000" && _highlightIncorrectPDFParm)
                            cellBGColor = System.Drawing.Color.FromName("Red");
                        else if (altResponse == "000" && !_highlightIncorrectPDFParm)
                            cellBGColor = System.Drawing.Color.Transparent;
                        else if (string.IsNullOrEmpty(altResponse))
                            cellBGColor = System.Drawing.Color.FromName("Black");
                        /*else if (!_highlightCorrectPDFParm && !_highlightIncorrectPDFParm)
                        {
                            if ((trSort > 0 && rubricPointCount == maxPoints) || (trSort > 0 && rubricPointCount == 0)
                                || altResponse == "100" || altResponse == "000")
                                cellBGColor = System.Drawing.Color.Transparent;
                            else
                            {
                                cellBGColor = System.Drawing.Color.FromName("Yellow");
                            }
                        }*/
                        else if (_highlightIncorrectPDFParm)
                        {
                            cellBGColor = System.Drawing.Color.FromName("Yellow");
                        }

                        if (cellBGColor != System.Drawing.Color.FromName("Black"))
                            cell.BgColor = cellBGColor.Name;

                        cellIndex++;
                    }

                    newRow.Cells.Add(cell);
                }
                table.Rows.Add(newRow);
            }

            return table;
        }

        private HtmlTable LoadPDFHeaderInfo()
        {
            var table = new HtmlTable();
            var row1 = new HtmlTableRow();
            var cell1 = new HtmlTableCell();
            var cell2 = new HtmlTableCell();
            var cell3 = new HtmlTableCell();
            var cell4 = new HtmlTableCell();
            table.Border = 1;
            table.Attributes["style"] = "border:solid 1px #000; border-collapse:collapse; font-family:Arial, sans-serif; width:100%;";
            table.CellPadding = 2;
            DataSet ds = GetDataSet();
            DataTable contentTable = GetContentDataTable();
            LoadFormButton();

            cell1.InnerHtml = "Report";
            cell2.InnerHtml = "Distractor Analysis (" + rcbDistractorAnalysisForms.SelectedItem.Text + ")";
            cell3.InnerHtml = "District";
            cell4.InnerHtml = AppSettings.ApplicationName;
            cell4.ColSpan = 5;

            cell1.Attributes["style"] = "font-weight:bold; background-color:#D0D0D0;";
            cell2.Attributes["style"] = "font-weight:bold; width:40%; background-color:#D0D0D0;";
            cell3.Attributes["style"] = "font-weight:bold; background-color:#D0D0D0;";
            cell4.Attributes["style"] = "font-weight:bold; width:50%; background-color:#D0D0D0;";

            row1.Cells.Add(cell1);
            row1.Cells.Add(cell2);
            row1.Cells.Add(cell3);
            row1.Cells.Add(cell4);
            table.Rows.Add(row1);

            if (_distractorAnalysisData.Tables[0].Rows.Count > 0)
            {
                var row2 = new HtmlTableRow();
                var row2cell1 = new HtmlTableCell();
                var row2cell2 = new HtmlTableCell();
                var row2cell3 = new HtmlTableCell();
                var row2cell4 = new HtmlTableCell();
                var row2cell5 = new HtmlTableCell();
                var row2cell6 = new HtmlTableCell();
                var row2cell7 = new HtmlTableCell();
                var row2cell8 = new HtmlTableCell();
                row2cell1.InnerHtml = "<b>Test</b> ";
                row2cell2.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["TestName"].ToString();
                row2cell3.InnerHtml = "<b>School</b> ";
                row2cell4.InnerHtml = _level == "School"
                                          ? ((Criteria)_reportCriteria).CriterionList.FindAll(
                                              r => r.Key.Contains(_level) && r.Object != null).Select(
                                                  criterion => criterion.Object.ToString()).ToList().ToString()
                                          : _distractorAnalysisData.Tables[0].Rows[0]["SchoolName"].ToString();
                row2cell5.InnerHtml = "<b># Schools</b> ";
                row2cell6.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["SchoolCount"].ToString();
                row2cell7.InnerHtml = "<b>High</b> ";
                row2cell8.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["Highest"].ToString();
                row2.Cells.Add(row2cell1);
                row2.Cells.Add(row2cell2);
                row2.Cells.Add(row2cell3);
                row2.Cells.Add(row2cell4);
                row2.Cells.Add(row2cell5);
                row2.Cells.Add(row2cell6);
                row2.Cells.Add(row2cell7);
                row2.Cells.Add(row2cell8);

                var row3 = new HtmlTableRow();
                var row3cell1 = new HtmlTableCell();
                var row3cell2 = new HtmlTableCell();
                var row3cell3 = new HtmlTableCell();
                var row3cell4 = new HtmlTableCell();
                var row3cell5 = new HtmlTableCell();
                var row3cell6 = new HtmlTableCell();
                var row3cell7 = new HtmlTableCell();
                var row3cell8 = new HtmlTableCell();
                row3cell1.InnerHtml = "<b>Criteria</b>";
                row3cell2.InnerHtml = "Group: " + _reportCriteria.GetLevelCriterion("group").Object.ToString();

                row3cell3.InnerHtml = "<b>Teacher</b> ";

                if (_level == "Teacher" && _reportCriteria != null)
                {
                    List<string> teachers = ((Criteria)_reportCriteria).CriterionList.FindAll(
                                                  r => r.Key.Contains(_level) && r.Object != null).Select(
                                                  criterion => criterion.Object.ToString()).ToList();

                    if (teachers.Count < 1)
                    {
                        row3cell4.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["TeacherName"].ToString();
                    }
                    else
                    {
                        foreach (string teacher in teachers)
                        {
                            row3cell4.InnerHtml = row3cell4.InnerHtml + teacher + "<br />";
                        }
                    }
                }
                else row3cell4.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["TeacherName"].ToString();
                
                row3cell5.InnerHtml = "<b># Teachers</b>";
                row3cell6.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["TeacherCount"].ToString();
                row3cell7.InnerHtml = "<b>Low</b> ";
                row3cell8.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["Lowest"].ToString();

                row3cell1.Attributes["style"] = "border-bottom:solid 0px #000;";
                row3cell2.Attributes["style"] = "border-bottom:solid 0px #000;";

                row3.Cells.Add(row3cell1);
                row3.Cells.Add(row3cell2);
                row3.Cells.Add(row3cell3);
                row3.Cells.Add(row3cell4);
                row3.Cells.Add(row3cell5);
                row3.Cells.Add(row3cell6);
                row3.Cells.Add(row3cell7);
                row3.Cells.Add(row3cell8);

                var row4 = new HtmlTableRow();
                var row4cell1 = new HtmlTableCell();
                var row4cell2 = new HtmlTableCell();
                var row4cell3 = new HtmlTableCell();
                var row4cell4 = new HtmlTableCell();
                var row4cell5 = new HtmlTableCell();
                var row4cell6 = new HtmlTableCell();
                var row4cell7 = new HtmlTableCell();
                var row4cell8 = new HtmlTableCell();
                row4cell3.InnerHtml = "<b>Class</b> ";
                row4cell4.InnerHtml = _level == "Class"
                                          ? string.Join(",",
                                                        ((Criteria)_reportCriteria).CriterionList.FindAll(
                                                            r => r.Key.Contains(_level) && r.Object != null).Select(
                                                                criterion => criterion.Object.ToString()).ToList())
                                          : _distractorAnalysisData.Tables[0].Rows[0]["ClassName"].ToString();
                row4cell5.InnerHtml = "<b># Classes</b>";
                row4cell6.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["ClassCount"].ToString();
                row4cell7.InnerHtml = "<b>Median</b> ";
                row4cell8.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["Median"].ToString();

                row4cell1.Attributes["style"] = "border-bottom:solid 0px #000;";
                row4cell2.Attributes["style"] = "border-bottom:solid 0px #000;";

                row4.Cells.Add(row4cell1);
                row4.Cells.Add(row4cell2);
                row4.Cells.Add(row4cell3);
                row4.Cells.Add(row4cell4);
                row4.Cells.Add(row4cell5);
                row4.Cells.Add(row4cell6);
                row4.Cells.Add(row4cell7);
                row4.Cells.Add(row4cell8);

                var row5 = new HtmlTableRow();
                var row5cell1 = new HtmlTableCell();
                var row5cell2 = new HtmlTableCell();
                var row5cell3 = new HtmlTableCell();
                var row5cell4 = new HtmlTableCell();
                var row5cell5 = new HtmlTableCell();
                var row5cell6 = new HtmlTableCell();
                var row5cell7 = new HtmlTableCell();
                var row5cell8 = new HtmlTableCell();
                row5cell3.InnerHtml = "<b>Student</b> ";
                row5cell4.InnerHtml = _level == "Student"
                                          ? string.Join(",",
                                                        ((Criteria)_reportCriteria).CriterionList.FindAll(
                                                            r => r.Key.Contains(_level) && r.Object != null).Select(
                                                                criterion => criterion.Object.ToString()).ToList())
                                          : "";
                row5cell5.InnerHtml = "<b># Students</b>";
                row5cell6.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["StudentCount"].ToString();
                row5cell7.InnerHtml = "<b>Mean</b> ";
                row5cell8.InnerHtml = _distractorAnalysisData.Tables[0].Rows[0]["Mean"].ToString();

                row5.Cells.Add(row5cell1);
                row5.Cells.Add(row5cell2);
                row5.Cells.Add(row5cell3);
                row5.Cells.Add(row5cell4);
                row5.Cells.Add(row5cell5);
                row5.Cells.Add(row5cell6);
                row5.Cells.Add(row5cell7);
                row5.Cells.Add(row5cell8);

                table.Rows.Add(row2);
                table.Rows.Add(row3);
                table.Rows.Add(row4);
                table.Rows.Add(row5);
            }


            return table;
        }

        private void LoadFormButton()
        {
            rcbDistractorAnalysisForms.Items.Clear();
            rcbDistractorAnalysisForms.Items.Add(new RadComboBoxItem() { Text = "All Students", Value = "0" });
            DataTable dtForms = Thinkgate.Base.Classes.Assessment.GetFormsByAssessmentID(Convert.ToInt32(_testID));

            foreach (DataRow dr in dtForms.Rows)
            {
                RadComboBoxItem cmbFormitem = new RadComboBoxItem(dr["Label"].ToString());
                cmbFormitem.Value = dr["ID"].ToString();

                rcbDistractorAnalysisForms.Items.Add(cmbFormitem);
            }

           
            rcbDistractorAnalysisForms.SelectedValue = _selectedFormID;

            rcbDistractorAnalysisForms.Enabled = (rcbDistractorAnalysisForms.Items.Count > 1);
        }

        private void SetLabels(DataRow row)
        {
            lblHigh.Text = row["Highest"].ToString();
            lblLow.Text = row["Lowest"].ToString();
            lblMedian.Text = row["Median"].ToString();
            lblMean.Text = row["Mean"].ToString();

            lblSchoolCount.Text = row["SchoolCount"].ToString();
            lblTeacherCount.Text = row["TeacherCount"].ToString();
            lblClassCount.Text = row["ClassCount"].ToString();
            lblStudentCount.Text = row["StudentCount"].ToString();
        }

        protected void reportGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                if (!(e.Item is GridDataItem))
                {
                    if (e.Item.ItemIndex == -1)
                    {
                        GridItem[] header = reportGrid.MasterTableView.GetItems(GridItemType.Header);
                        GridHeaderItem headerItem = (GridHeaderItem)header[0];

                        LinkButton headerNameLink = (LinkButton)headerItem["Name"].Controls[0];
                        headerItem["Name"].Attributes["style"] = "cursor:pointer;"
                                                                 +
                                                                 (_selectedSortColumn == "Name"
                                                                      ? "background: url(../Images/" +
                                                                        (_selectedSortMethod == "DESC"
                                                                             ? "uparrow.gif"
                                                                             : "downarrow.gif") +
                                                                        ") no-repeat right center;"
                                                                      : "");
                        if (_selectedSortColumn == "Name") headerItem["Name"].Attributes["selectedSortColumn"] = "true";
                        else headerItem["Name"].Attributes["selectedSortColumn"] = "false";
                        headerItem["Name"].Attributes["onclick"] = "sortRadGridWithCustomHeaderLinks_OnClick(this, event);";
                        headerItem["Name"].Attributes["onmouseover"] = "showHideSortArrow(this);";
                        headerItem["Name"].Attributes["onmouseout"] = "showHideSortArrow(this);";

                        LinkButton headerStudentIDLink = (LinkButton)headerItem["StudentID"].Controls[0];
                        headerItem["StudentID"].Attributes["style"] = "cursor:pointer;"
                                                                  +
                                                                  (_selectedSortColumn == "StudentID"
                                                                       ? "background: url(../Images/" +
                                                                         (_selectedSortMethod == "DESC"
                                                                              ? "uparrow.gif"
                                                                              : "downarrow.gif") +
                                                                         ") no-repeat right center;"
                                                                       : "");
                        if (_selectedSortColumn == "StudentID") headerItem["StudentID"].Attributes["selectedSortColumn"] = "true";
                        else headerItem["StudentID"].Attributes["selectedSortColumn"] = "false";
                        headerItem["StudentID"].Attributes["onclick"] = "sortRadGridWithCustomHeaderLinks_OnClick(this, event);";
                        headerItem["StudentID"].Attributes["onmouseover"] = "showHideSortArrow(this);";
                        headerItem["StudentID"].Attributes["onmouseout"] = "showHideSortArrow(this);";
                        headerItem["StudentID"].Attributes.Add("studentid", "studentid");

                        LinkButton headerScoreLink = (LinkButton)headerItem["Score"].Controls[0];
                        headerItem["Score"].Attributes["style"] = "cursor:pointer;"
                                                                  +
                                                                  (_selectedSortColumn == "Score"
                                                                       ? "background: url(../Images/" +
                                                                         (_selectedSortMethod == "DESC"
                                                                              ? "uparrow.gif"
                                                                              : "downarrow.gif") +
                                                                         ") no-repeat right center;"
                                                                       : "");
                        if (_selectedSortColumn == "Score") headerItem["Score"].Attributes["selectedSortColumn"] = "true";
                        else headerItem["Score"].Attributes["selectedSortColumn"] = "false";
                        headerItem["Score"].Attributes["onclick"] = "sortRadGridWithCustomHeaderLinks_OnClick(this, event);";
                        headerItem["Score"].Attributes["onmouseover"] = "showHideSortArrow(this);";
                        headerItem["Score"].Attributes["onmouseout"] = "showHideSortArrow(this);";


                        foreach (DataRow row in _columnData.Rows)
                        {
                            var sortColName = row["TR_Sort"].ToString() != "0"
                                                 ? row["Sort"].ToString() + " r(" + row["TR_Sort"].ToString() + ")"
                                                 : row["Sort"].ToString();
                            var bgColor = DataIntegrity.ConvertToBool(row["FieldTest"].ToString()) ? "#FFFF00" : "";
                            LinkButton headerLink = (LinkButton)headerItem[sortColName].Controls[0];

                            // TFS 1123: Disable item preview link for Teacher and School Admin login if assessment content is locked
                            ReportHelper reportHelper = new ReportHelper();
                            reportHelper.UserRoles = SessionObject.LoggedInUser.Roles;
                            if (_isContentLocked && reportHelper.DisableItemLinks())
                            {
                                headerLink.Enabled = false;
                                headerLink.Attributes["style"] = "font-weight: bold; disabled: true;";
                            }
                            else
                            {
                                headerLink.Attributes["style"] = "color:#00F;font-weight:bold;text-decoration:underline;";
                                headerLink.OnClientClick =
                                    "customDialog({ url: '../ControlHost/PreviewTestQuestion.aspx?xID=" +
                                    Encryption.EncryptInt(DataIntegrity.ConvertToInt(row["QuestionID"]))
                                    + "', title: 'Item Preview', maximize: true, maxwidth: 600, maxheight: 450 });return false;";
                                headerItem[sortColName].Attributes["style"] = "cursor:pointer;"
                                                                                         +
                                                                                         (_selectedSortColumn ==
                                                                                          sortColName
                                                                                              ? "background: url(../Images/" +
                                                                                                (_selectedSortMethod == "DESC"
                                                                                                     ? "uparrow.gif"
                                                                                                     : "downarrow.gif") +
                                                                                                ") no-repeat right center;"
                                                                                                + "background-color: " + bgColor
                                                                                              : "background-color:" + bgColor + ";");
                                if (_selectedSortColumn == sortColName)
                                    headerItem[sortColName].Attributes["selectedSortColumn"] = "true";
                                else headerItem[sortColName].Attributes["selectedSortColumn"] = "false";
                                headerItem[sortColName].Attributes["onclick"] =
                                    "sortRadGridWithCustomHeaderLinks_OnClick(this, event);";
                                headerItem[sortColName].Attributes["onmouseover"] = "showHideSortArrow(this, '" + bgColor + "');";
                                headerItem[sortColName].Attributes["onmouseout"] = "showHideSortArrow(this, '" + bgColor + "');";
                            }
                        }
                    }

                    return;
                }

                var cellIndex = e.Item.Cells.Count - _columnData.Rows.Count;
                var counter = 0;
                var dataItem = (DataRowView)(e.Item).DataItem;
                var testResponses = string.Empty;
                var altResponses = string.Empty;
                var answerString = string.Empty;
                var additionalData = string.Empty;

                e.Item.Cells[cellIndex - 3].Wrap = false;

                if (dataItem["Name"].ToString() == "Standards")
                {
                    e.Item.Cells[cellIndex - 3].ID = "standardsRow";
                    e.Item.Cells[cellIndex - 3].ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    e.Item.Cells[3].Attributes.Add("studentid", "studentid");
                }
                if (dataItem["Name"].ToString() == showRigor_checkbox.Value)
                {
                    e.Item.Cells[cellIndex - 3].ID = "rigorRow";
                    e.Item.Cells[cellIndex - 3].ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    e.Item.Cells[3].Attributes.Add("studentid", "studentid");
                }

                foreach (DataRow row in _distractorAnalysisData.Tables[0].Rows)
                {
                    testResponses = row["TestResponses"].ToString();
                    altResponses = row["AltResponses"].ToString();
                    answerString = row["AnswerString"].ToString();
                    additionalData = row["AdditionalData"].ToString();
                    e.Item.Cells[cellIndex - 1].Text = row["PctCorrectDisp"].ToString();

                    if (dataItem["Name"].ToString() == row["Student_Name"].ToString())
                    {
                        //Make student name a link            
                        if (!String.IsNullOrEmpty(row["StudentRecID"].ToString()) && DataIntegrity.ConvertToInt(row["StudentRecID"]) > 0)
                        {
                            e.Item.Cells[2].Attributes.Add("onclick", "window.open('Student.aspx?childPage=yes&xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["StudentRecID"].ToString()) + "');");
                            e.Item.Cells[2].Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3; white-space: nowrap;");
                            e.Item.Cells[2].Attributes.Add("onclickHide", "window.open('Student.aspx?childPage=yes&xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["StudentRecID"].ToString()) + "');");
                            e.Item.Cells[2].Attributes.Add("styleHide", "cursor:pointer; text-decoration: underline; color: #034AF3; white-space: nowrap;");
                            e.Item.Cells[2].Attributes.Add("studentname", "studentname");
                            e.Item.Cells[2].Attributes.Add("sname", row["Student_Name"].ToString());
                            e.Item.Cells[3].Attributes.Add("studentid", "studentid");
                        }
                        break;
                    }

                    else if (dataItem["Name"].ToString() == row["Demo_Name"].ToString())
                    {
                        LinkButton newLink = new LinkButton();
                        newLink.Click += new EventHandler(newLink_Click);
                        newLink.Attributes["demoLabel"] = row["DemoLabel"].ToString();
                        newLink.Attributes["demoVal"] = row["DemoVal"].ToString();
                        newLink.Text = row["Demo_Name"].ToString();
                        e.Item.Cells[cellIndex - 3].Controls.Add(newLink);
                        e.Item.Cells[3].Attributes.Add("studentid", "studentid");
                        break;
                    }
                }

                if (answerString.Length > 0) //For overall average and demographic cell background colors
                {
                    var answers = answerString.Split((";").ToCharArray());
                    //if (answers.GetUpperBound(0) < _columnData.Rows.Count) return;

                    foreach (var answer in answers)
                    {
                        if (cellIndex >= e.Item.Cells.Count) return;

                        e.Item.Cells[cellIndex].Font.Size = FontUnit.XXSmall;
                        e.Item.Cells[cellIndex].Wrap = false;
                        e.Item.Cells[3].Attributes.Add("studentid", "studentid");

                        cellIndex++;
                        counter++;
                    }
                }
                else
                {
                    if (testResponses.Length == 0) return;
                    if (altResponses.Length == 0) return;

                    foreach (DataRow row in _columnData.Rows)
                    {
                        var fieldTest = row["FieldTest"].ToString() == "1";
                        var displayFieldTestItemData = row["DisplayFieldTestItemData"].ToString() == "yes";
                        var scoreOnTest = row["ScoreOnTest"].ToString();
                        var cellBGColor = System.Drawing.Color.FromName("Green");
                        var trSort = DataIntegrity.ConvertToInt(row["TR_Sort"]);
                        var maxPoints = DataIntegrity.ConvertToInt(row["MaxPoints"]);
                        var masterSort = DataIntegrity.ConvertToInt(row["MasterSort"]);
                        var answer = string.Empty;
                        //var testResponse = testResponses.Substring(masterSort-1, 1);
                        //var altResponse = altResponses.Substring(masterSort == 1 ? 0 : (masterSort * 3) - 3, 3);

                        var testResponse = testResponses.Length >= masterSort ? testResponses.Substring((masterSort - 1), 1) : testResponses.Substring(0, 1);
                        var altResponse = altResponses.Length > ((masterSort * 3) - 3) ? altResponses.Substring((masterSort * 3) - 3, 3) : altResponses.Substring(0, 3);

                        var questionType = row["QuestionType"].ToString();
                        var questionID = DataIntegrity.ConvertToInt(row["QuestionID"]);
                        var rubricPointListString = string.Empty;
                        var rubricPointList = new string[] { };
                        var rubricPointCount = 0;

                        if (additionalData.Length > 0 && additionalData.IndexOf("score_" + questionID + "=,") > -1)
                        {
                            rubricPointListString = additionalData.Substring(additionalData.IndexOf("score_" + questionID + "=,") + ("score_" + questionID + "=,").Length);
                            rubricPointListString = rubricPointListString.Substring(0, rubricPointListString.IndexOf(",|?|"));
                            rubricPointList = rubricPointListString.Split(',');
                            if (trSort > 0)
                            {
                                rubricPointCount = DataIntegrity.ConvertToInt(rubricPointList[trSort - 1].Substring(rubricPointList[trSort - 1].IndexOf("_") + 1));
                            }
                        }

                        if (altResponse != "***" && testResponse == "*" && (questionType == "MC3" || questionType == "MC4" || questionType == "MC5" || questionType == "T"))
                        {
                            answer = "x";
                        }

                        //Logic from legacy system.
                        if (scoreOnTest == "No" && altResponse == "100")
                        {
                            cellBGColor = System.Drawing.Color.FromName("Green");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "green";
                            e.Item.Cells[cellIndex].Style.Add("opacity", "0.4");
                            e.Item.Cells[cellIndex].Style.Add("filter", "alpha(opacity=50)");
                        }
                        else if (scoreOnTest == "No" && altResponse == "000")
                        {
                            cellBGColor = System.Drawing.Color.FromName("Red");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "red";
                            e.Item.Cells[cellIndex].Style.Add("opacity", "0.4");
                            e.Item.Cells[cellIndex].Style.Add("filter", "alpha(opacity=50)");
                        }
                        else if (!displayFieldTestItemData && fieldTest)
                        {
                            cellBGColor = System.Drawing.Color.FromName("Gray");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "gray";
                        }
                        else if (trSort > 0 && rubricPointCount == maxPoints)
                        {
                            cellBGColor = System.Drawing.Color.FromName("Green");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "green";
                            e.Item.Cells[cellIndex].Style.Add("color", "White");
                        }
                        else if (trSort > 0 && rubricPointCount == 0)
                        {
                            cellBGColor = System.Drawing.Color.FromName("Red");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "red";
                            e.Item.Cells[cellIndex].Style.Add("color", "White");
                        }
                        else if (altResponse == "100")
                        {
                            cellBGColor = System.Drawing.Color.FromName("Green");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "green";
                        }
                        else if (altResponse == "***")
                        {
                            cellBGColor = System.Drawing.Color.FromName("LightGray");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "lightgray";
                        }
                        else if (answer == "x")
                        {
                            cellBGColor = System.Drawing.Color.FromName("Gray");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "gray";
                        }
                        else if (altResponse == "000")
                        {
                            cellBGColor = System.Drawing.Color.FromName("Red");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "red";
                        }
                        else if (string.IsNullOrEmpty(altResponse))
                        {
                            cellBGColor = System.Drawing.Color.FromName("Black");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "";
                        }
                        else
                        {
                            cellBGColor = System.Drawing.Color.FromName("Yellow");
                            e.Item.Cells[cellIndex].Attributes["answerType"] = "yellow";
                        }

                        if (cellBGColor != System.Drawing.Color.FromName("Black"))
                        {
                            e.Item.Cells[cellIndex].BackColor = cellBGColor;
                        }

                        e.Item.Cells[cellIndex].Wrap = false;
                        if ((!row["ScoreType"].Equals("P") && !row["ScoreType"].Equals("R")) || altResponse == "000" || altResponse == "100")
                            e.Item.Cells[cellIndex].Style.Add("color", "White");

                        counter += 3;
                        cellIndex++;
                    }
                }
            }

            catch (Exception exc)
            {
                PreventChangeToReport(exc.Message);

            }
        }

        protected void ReportGrid_SortCommand(object source, GridSortCommandEventArgs e)
        {
            GridSortExpression sortExpr = new GridSortExpression();
            DataTable sortTable = new DataTable();
            e.Canceled = true;

            foreach (DataColumn col in _contentData.Columns)
            {
                DataColumn newCol = new DataColumn(col.ColumnName);
                sortTable.Columns.Add(newCol);
            }

            DataRow standardsRow = sortTable.NewRow();
            DataRow rigorRow = sortTable.NewRow();
            foreach (DataColumn col in _contentData.Columns)
            {
                standardsRow[col.ColumnName] = _contentData.Rows[0][col];
                rigorRow[col.ColumnName] = _contentData.Rows[1][col];
            }

            sortTable.Rows.Add(standardsRow);
            sortTable.Rows.Add(rigorRow);

            switch (SessionObject.Reports_GridSortOrder)
            {
                case "DESC":
                    _contentData.Select("[1] Like '%studentRow=%' and Len(Score) > 0", e.SortExpression + " DESC").CopyToDataTable<System.Data.DataRow>(sortTable, System.Data.LoadOption.PreserveChanges);
                    _contentData.Select("[1] Not Like '%studentRow=%' and Len(Score) > 0").CopyToDataTable<System.Data.DataRow>(sortTable, System.Data.LoadOption.PreserveChanges);
                    _selectedSortColumn = e.SortExpression;
                    _selectedSortMethod = "DESC";
                    reportGrid.DataSource = sortTable;
                    reportGrid.DataBind();

                    SessionObject.Reports_GridSortOrder = "ASC";

                    break;
                case "ASC":
                    _contentData.Select("[1] Like '%studentRow=%' and Len(Score) > 0", e.SortExpression + " ASC").CopyToDataTable<System.Data.DataRow>(sortTable, System.Data.LoadOption.PreserveChanges);
                    _contentData.Select("[1] Not Like '%studentRow=%' and Len(Score) > 0").CopyToDataTable<System.Data.DataRow>(sortTable, System.Data.LoadOption.PreserveChanges);
                    _selectedSortColumn = e.SortExpression;
                    _selectedSortMethod = "ASC";
                    reportGrid.DataSource = sortTable;
                    reportGrid.DataBind();

                    SessionObject.Reports_GridSortOrder = "DESC";
                    break;
                default:
                    _contentData.Select("[1] Like '%studentRow=%' and Len(Score) > 0", e.SortExpression + " DESC").CopyToDataTable<System.Data.DataRow>(sortTable, System.Data.LoadOption.PreserveChanges);
                    _contentData.Select("[1] Not Like '%studentRow=%' and Len(Score) > 0").CopyToDataTable<System.Data.DataRow>(sortTable, System.Data.LoadOption.PreserveChanges);
                    _selectedSortColumn = e.SortExpression;
                    _selectedSortMethod = "DESC";
                    reportGrid.DataSource = sortTable;
                    reportGrid.DataBind();

                    SessionObject.Reports_GridSortOrder = "ASC";
                    break;
            }
        }

        public void newLink_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            _demoLabel = button.Attributes["demoLabel"];
            _demoVal = button.Attributes["demoVal"];
            LoadReport();
        }

        protected void ctxForm_ItemClick(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //RadMenuItem item = (RadMenuItem)e.Item;
            //_selectedFormID = item.Value;
            //btnFormSelection.Text = item.Text;
            //LoadReport();


            _selectedFormID = e.Value;
            RemoteReportReload(this, null);
            rcbDistractorAnalysisForms.Text = e.Text;
        }

        //Method to remove blank pages from the pdf
        protected void RemoveBlankPages(Doc pdf)
        {
            for (int i = pdf.PageCount; i > 0; i--)
            {
                pdf.PageNumber = i;

                //get the pdf content
                string textContent = pdf.GetText("Text");

                //delete the page if it is blank
                if (string.IsNullOrEmpty(textContent))
                { pdf.Delete(pdf.Page); }
            }
        }

        protected Doc DistractorAnalysisToPdfDoc(PdfRenderSettings settings = null)
        {
           var dp = DistrictParms.LoadDistrictParms();
            if (settings == null) settings = new PdfRenderSettings();

            StringWriter sw = new StringWriter();
            HtmlTextWriter w = new HtmlTextWriter(sw);
            //distractorContent.Attributes["style"] = "font-family: Sans-Serif, Arial;font-weight: bold;position: relative;font-size: .8em;";
            distractorContent.Controls.Add(LoadPDFHeaderInfo());
            var br = new HtmlGenericControl();
            br.InnerHtml = "<br/><br/>";
            distractorContent.Controls.Add(br);
            distractorContent.Controls.Add(LoadPDFReport());
            distractorContent.RenderControl(w);
            string result_html = sw.GetStringBuilder().ToString();

            int topOffset = settings.HeaderHeight > 0 ? settings.HeaderHeight : 0;
            int bottomOffset = settings.FooterHeight > 0 ? settings.FooterHeight : 0;

            Doc doc = new Doc();
            doc.HtmlOptions.HideBackground = true;
            doc.HtmlOptions.PageCacheEnabled = false;
            doc.HtmlOptions.UseScript = true;
            doc.HtmlOptions.Timeout = 36000;
            doc.HtmlOptions.BreakZoneSize = 50;    // I experiemented with this being 99% instead of 100%, but you end up with passages getting cut off in unflattering ways. This may lead to more blank space... but I think it's the lessor of evils
            doc.HtmlOptions.ImageQuality = 70;

            doc.MediaBox.String = "0 0 " + settings.PageWidth + " " + settings.PageHeight;
            doc.Rect.String = settings.LeftMargin + " " + (0 + bottomOffset).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + (settings.PageHeight - topOffset).ToString();
            doc.HtmlOptions.AddTags = true;
            doc.SetInfo(0, "ApplyOnLoadScriptOnceOnly", "1");

            List<int> forms = new List<int>();

            int theID = doc.AddImageHtml(result_html);

            Thinkgate.Base.Classes.Assessment.ChainPDFItems(doc, theID, forms, settings, dp.PdfPrintPageLimit);

            if (settings.HeaderHeight > 0 && !String.IsNullOrEmpty(settings.HeaderText))
            {
                /*HttpServerUtility Server = HttpContext.Current.Server;
                headerText = Server.HtmlDecode(headerText);*/
                Doc headerDoc = new Doc();

                headerDoc.MediaBox.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                headerDoc.Rect.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                headerDoc.VPos = 0.5;
                int headerID = headerDoc.AddImageHtml(settings.HeaderText);

                if (!String.IsNullOrEmpty(settings.HeaderText))
                {
                    int form_ref = 0;
                    for (int i = 1; i <= doc.PageCount; i++)
                    {
                        if (form_ref < forms.Count && forms[form_ref] == i)
                        {
                            form_ref++;
                        }
                        else
                        {
                            if (i > 1 || settings.ShowHeaderOnFirstPage)
                            {
                                doc.PageNumber = i;
                                doc.Rect.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                                doc.VPos = 0.5;

                                theID = doc.AddImageDoc(headerDoc, 1, null);
                                theID = doc.AddImageToChain(theID);
                            }
                        }
                    }
                }
            }

            RemoveBlankPages(doc);

            for (int i = 1; i <= doc.PageCount; i++)
            {
                doc.PageNumber = i;
                doc.Flatten();
            }
            return doc;
        }

        public override void ExportToExcel()
        {
            _isExcel = true;
            // Create the workbook
            ExcelFile ef = new ExcelFile();
            ef.DefaultFontName = "Calibri";
            ExcelWorksheet ws = ef.Worksheets.Add("DataSheet");
            FormatDistractorAnalysis(ws);
            ef.Save(Response, "ExportData.xlsx");
        }


        public void FormatDistractorAnalysis(ExcelWorksheet ws)
        {
            DataSet ds = GetDataSet();
            DataTable contentTable = GetContentDataTable_ExportToExcel();


            if (ds == null || ds.Tables.Count < 2 || contentTable.Rows.Count == 0 || ds.Tables[1].Rows.Count < 1) return;

            _columnData = ds.Tables[1];

            ws.PrintOptions.PrintGridlines = true;

            int colCount;
            //Write second rows first so that we can calculate width of for the headers
            var rowCount = 1;
            foreach (DataRow row in contentTable.Rows) // Loop over the rows.
            {
                // if (row["Name"].ToString() == "Standards") continue;

                if (row["Name"].ToString() == "RBT" && !showRigor_checkbox.Checked) continue;
                colCount = 0;
                var cellIndex = 0;

                foreach (DataColumn column in contentTable.Columns) // Loop over the items.
                {
                    if (column.ColumnName == "TestResponses" || column.ColumnName == "AltResponses" || column.ColumnName == "AnswerString" || column.ColumnName == "AdditionalData" || (column.ColumnName.ToLower() == "studentid" && chkHideStudentID_checkbox.Checked)) continue;
                    var testResponses = row["TestResponses"].ToString();
                    var altResponses = row["AltResponses"].ToString();
                    var answerString = row["AnswerString"].ToString();
                    var additionalData = row["AdditionalData"].ToString();

                    if (testResponses.Length > 0 && altResponses.Length > 0 && column.ColumnName != "Name" && column.ColumnName != "StudentID" && column.ColumnName != "Score")
                    {
                        var fieldTest = _columnData.Rows[cellIndex]["FieldTest"].ToString() == "1";
                        var displayFieldTestItemData = _columnData.Rows[cellIndex]["DisplayFieldTestItemData"].ToString() == "yes";
                        Color cellBGColor = Color.Yellow;
                        var trSort = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["TR_Sort"]);
                        var maxPoints = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["MaxPoints"]);
                        var masterSort = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["MasterSort"]);
                        var answer = string.Empty;
                        //var testResponse = testResponses.Substring(masterSort - 1, 1);
                        //var altResponse = altResponses.Substring(masterSort == 1 ? 0 : (masterSort * 3) - 3, 3);

                        var testResponse = testResponses.Length > masterSort ? testResponses.Substring((masterSort - 1), 1) : testResponses.Substring(0, 1);
                        var altResponse = altResponses.Length > ((masterSort * 3) - 3) ? altResponses.Substring((masterSort * 3) - 3, 3) : altResponses.Substring(0, 3);

                        var questionType = _columnData.Rows[cellIndex]["QuestionType"].ToString();
                        var questionID = DataIntegrity.ConvertToInt(_columnData.Rows[cellIndex]["QuestionID"]);
                        var rubricPointListString = string.Empty;
                        var rubricPointList = new string[] { };
                        var rubricPointCount = 0;

                        if (additionalData.Length > 0 && additionalData.IndexOf("score_" + questionID + "=,") > -1)
                        {
                            rubricPointListString = additionalData.Substring(additionalData.IndexOf("score_" + questionID + "=,") + ("score_" + questionID + "=,").Length);
                            rubricPointListString = rubricPointListString.Substring(0, rubricPointListString.IndexOf(",|?|"));
                            rubricPointList = rubricPointListString.Split(',');
                            if (trSort > 0)
                            {
                                rubricPointCount = DataIntegrity.ConvertToInt(rubricPointList[trSort - 1].Substring(rubricPointList[trSort - 1].IndexOf("_") + 1));
                            }
                        }

                        if (altResponse != "***" && testResponse == "*" &&
                            (questionType == "MC3" || questionType == "MC4" || questionType == "MC5" || questionType == "T"))
                        {
                            answer = "x";
                        }

                        //Logic from legacy system.
                        if (!displayFieldTestItemData && fieldTest)
                        {
                            cellBGColor = Color.Gray;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.White;
                        }
                        else if (trSort > 0 && rubricPointCount == maxPoints && highlightCorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.Green;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.White;
                        }
                        else if (trSort > 0 && rubricPointCount == maxPoints && !highlightCorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.White;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.Black;
                        }
                        else if (trSort > 0 && rubricPointCount == 0 && highlightIncorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.Red;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.White;
                        }
                        else if (trSort > 0 && rubricPointCount == 0 && !highlightIncorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.White;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.Black;
                        }
                        else if (altResponse == "100" && highlightCorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.Green;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.White;
                        }
                        else if (altResponse == "100" && !highlightCorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.White;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.Black;
                        }
                        else if (altResponse == "***")
                        {
                            cellBGColor = Color.LightGray;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.White;
                        }
                        else if (answer == "x")
                        {
                            cellBGColor = Color.Gray;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.White;
                        }
                        else if (altResponse == "000" && highlightIncorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.Red;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.White;
                        }
                        else if (altResponse == "000" && !highlightIncorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.White;
                            ws.Cells[rowCount, colCount].Style.Font.Color = Color.Black;
                        }
                        else if (string.IsNullOrEmpty(altResponse))
                            cellBGColor = Color.White;
                        /*else if (!highlightCorrect_checkbox.Checked && !highlightIncorrect_checkbox.Checked)
                        {
                            if ((trSort > 0 && rubricPointCount == maxPoints) || (trSort > 0 && rubricPointCount == 0)
                                || altResponse == "100" || altResponse == "000")
                                cellBGColor = Color.White;
                            else
                            {
                                cellBGColor = Color.Yellow;
                                ws.Cells[rowCount, colCount].Style.Font.Color = Color.Black;
                            }
                        }*/

                        else if (highlightIncorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.Yellow;
                            }
                        else if (!highlightIncorrect_checkbox.Checked)
                        {
                            cellBGColor = Color.White;
                        }

                        ws.Cells[rowCount, colCount].Style.FillPattern.SetSolid(cellBGColor);
                        cellIndex++;
                    }

                    if (column.ColumnName == "Name" && chkHideStudentName_checkbox.Checked)
                    {
                        ws.Cells[rowCount, colCount].Value = "";
                    }

                    ws.Cells[rowCount, colCount].Style.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
                    ws.Cells[rowCount, colCount].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    if (!row[column.ColumnName].ToString().Contains("preview1.gif"))
                    {
                        ws.Cells[rowCount, colCount].Value = (column.ColumnName == "Name" && testResponses.Length > 0 && chkHideStudentName_checkbox.Checked) ? "" : row[column.ColumnName].ToString();
                    }
                    else
                    {
                        var imgPath = Server.MapPath("./Images/").Replace("Record\\", "");
                        ws.Pictures.Add(Path.Combine(imgPath, "preview1.gif"), PositioningMode.Move, new AnchorCell(ws.Columns[colCount], ws.Rows[rowCount], true));
                    }
                    colCount++;
                }

                rowCount++;
            }

            colCount = 0; //reset columns
            var colDataIndex = 0;
            foreach (DataColumn column in contentTable.Columns)
            {
                if (column.ColumnName == "TestResponses" || column.ColumnName == "AltResponses" || column.ColumnName == "AnswerString" || column.ColumnName == "AdditionalData" || (column.ColumnName.ToLower() == "studentid" && chkHideStudentID_checkbox.Checked)) continue;
                Color headCellBGColorStyle = DataIntegrity.ConvertToBool(_columnData.Rows[colDataIndex]["FieldTest"]) ? Color.Yellow : Color.LightGray;
                ws.Cells[0, colCount].Value = column.ColumnName;
                ws.Cells[0, colCount].Style.Font.Weight = ExcelFont.BoldWeight;
                ws.Columns[colCount].AutoFit();
                ws.Cells[0, colCount].Style.FillPattern.SetSolid(headCellBGColorStyle);
                ws.Cells[0, colCount].Style.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
                ws.Cells[0, colCount].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                colCount++;
                if (column.ColumnName != "Name" && column.ColumnName != "StudentID" && column.ColumnName != "Score") colDataIndex++;
            }
        }
    }
}