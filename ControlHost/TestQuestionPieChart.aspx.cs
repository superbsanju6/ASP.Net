using System;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Telerik.Charting;

namespace Thinkgate.ControlHost
{
    public partial class TestQuestionPieChart : BasePage
    {
        private int _sort;
        private string _level;
        private string _levelID;
        private string _critOrides;
        private string _testID;
        private string _formID;
        private string _parent;
        private string _parentID;
        private string _schoolID;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Request.QueryString["sort"] == null || Request.QueryString["level"] == null)
            {
                RedirectToPortalSelectionScreenWithCustomMessage("Invalid Request.");
            }

            _sort = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(Request.QueryString["sort"]);
            
            if (_sort == 0)
            {
                RedirectToPortalSelectionScreenWithCustomMessage("Invalid sort provided.");
                
            }
            _level = Request.QueryString["level"];
            _levelID = GetDecryptedEntityId("levelID").ToString();
            _critOrides = String.IsNullOrEmpty(Request.QueryString["critOrides"]) ? GetCriteriaOverrides(_level) : Request.QueryString["critOrides"];
            _testID = (String.IsNullOrEmpty(Request.QueryString["testID"]) ? String.Empty : Request.QueryString["testID"]);
            _formID = (String.IsNullOrEmpty(Request.QueryString["formID"]) ? String.Empty : Request.QueryString["formID"]);
            _parent = (String.IsNullOrEmpty(Request.QueryString["parent"]) ? String.Empty : Request.QueryString["parent"]);
            _parentID = (String.IsNullOrEmpty(Request.QueryString["parentID"]) ? String.Empty : Request.QueryString["parentID"]);
            _schoolID = String.Empty;

            if (_parent == "School")
            {
                _schoolID = _parentID;
            }
            else if (_level == "School")
            {
                _schoolID = _levelID;
            }

            var rationaleData = Thinkgate.Base.Classes.TestQuestion.GetTestQuestionRationale(0, _sort, SessionObject.LoggedInUser.Page, _level, _levelID, "", _critOrides, _testID, _formID, _level, _schoolID);
            if (rationaleData == null || rationaleData.Rows.Count == 0) return;
            var rationaleRow = rationaleData.Rows[0];

            testQuestionPieChart.Series.Clear();
            var nSeries = new ChartSeries();
            nSeries.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;

            nSeries.Type = ChartSeriesType.Pie;

            if (rationaleRow["QuestionType"].ToString().Contains("MC"))
            {
                if (rationaleRow["QuestionType"].ToString() == "MC3")
                {
                    nSeries.AddItem(GetChartSeriesItem("A", DataIntegrity.ConvertToDouble(rationaleRow["answerA"])));
                    nSeries.AddItem(GetChartSeriesItem("B", DataIntegrity.ConvertToDouble(rationaleRow["answerB"])));
                    nSeries.AddItem(GetChartSeriesItem("C", DataIntegrity.ConvertToDouble(rationaleRow["answerC"])));

                    lblAPercent.Text = String.Concat(rationaleRow["answerA"].ToString(), "%");
                    lblBPercent.Text = String.Concat(rationaleRow["answerB"].ToString(), "%");
                    lblCPercent.Text = String.Concat(rationaleRow["answerC"].ToString(), "%");

                    lblADescription.Text = rationaleRow["rationaleA"].ToString();
                    lblBDescription.Text = rationaleRow["rationaleB"].ToString();
                    lblCDescription.Text = rationaleRow["rationaleC"].ToString();

                    RationaleD.Visible = false;
                    RationaleE.Visible = false;
                }

                if (rationaleRow["QuestionType"].ToString() == "MC4")
                {
                    nSeries.AddItem(GetChartSeriesItem("A", DataIntegrity.ConvertToDouble(rationaleRow["answerA"])));
                    nSeries.AddItem(GetChartSeriesItem("B", DataIntegrity.ConvertToDouble(rationaleRow["answerB"])));
                    nSeries.AddItem(GetChartSeriesItem("C", DataIntegrity.ConvertToDouble(rationaleRow["answerC"])));
                    nSeries.AddItem(GetChartSeriesItem("D", DataIntegrity.ConvertToDouble(rationaleRow["answerD"])));

                    lblAPercent.Text = String.Concat(rationaleRow["answerA"].ToString(), "%");
                    lblBPercent.Text = String.Concat(rationaleRow["answerB"].ToString(), "%");
                    lblCPercent.Text = String.Concat(rationaleRow["answerC"].ToString(), "%");
                    lblDPercent.Text = String.Concat(rationaleRow["answerD"].ToString(), "%");

                    lblADescription.Text = rationaleRow["rationaleA"].ToString();
                    lblBDescription.Text = rationaleRow["rationaleB"].ToString();
                    lblCDescription.Text = rationaleRow["rationaleC"].ToString();
                    lblDDescription.Text = rationaleRow["rationaleD"].ToString();

                    RationaleE.Visible = false;
                }

                if (rationaleRow["QuestionType"].ToString() == "MC5")
                {
                    nSeries.AddItem(GetChartSeriesItem("A", DataIntegrity.ConvertToDouble(rationaleRow["answerA"])));
                    nSeries.AddItem(GetChartSeriesItem("B", DataIntegrity.ConvertToDouble(rationaleRow["answerB"])));
                    nSeries.AddItem(GetChartSeriesItem("C", DataIntegrity.ConvertToDouble(rationaleRow["answerC"])));
                    nSeries.AddItem(GetChartSeriesItem("D", DataIntegrity.ConvertToDouble(rationaleRow["answerD"])));
                    nSeries.AddItem(GetChartSeriesItem("E", DataIntegrity.ConvertToDouble(rationaleRow["answerE"])));

                    lblAPercent.Text = String.Concat(rationaleRow["answerA"].ToString(), "%");
                    lblBPercent.Text = String.Concat(rationaleRow["answerB"].ToString(), "%");
                    lblCPercent.Text = String.Concat(rationaleRow["answerC"].ToString(), "%");
                    lblDPercent.Text = String.Concat(rationaleRow["answerD"].ToString(), "%");
                    lblEPercent.Text = String.Concat(rationaleRow["answerE"].ToString(), "%");

                    lblADescription.Text = rationaleRow["rationaleA"].ToString();
                    lblBDescription.Text = rationaleRow["rationaleB"].ToString();
                    lblCDescription.Text = rationaleRow["rationaleC"].ToString();
                    lblDDescription.Text = rationaleRow["rationaleD"].ToString();
                    lblEDescription.Text = rationaleRow["rationaleE"].ToString();
                }
            }

            if (rationaleRow["QuestionType"].ToString().Trim() == "T")
            {
                RationaleA.Cells[0].InnerHtml = "T";
                RationaleB.Cells[0].InnerHtml = "F";

                nSeries.AddItem(GetChartSeriesItem("T", DataIntegrity.ConvertToDouble(rationaleRow["answerA"])));
                nSeries.AddItem(GetChartSeriesItem("F", DataIntegrity.ConvertToDouble(rationaleRow["answerB"])));

                lblAPercent.Text = String.Concat(rationaleRow["answerA"].ToString(), "%");
                lblBPercent.Text = String.Concat(rationaleRow["answerB"].ToString(), "%");

                lblADescription.Text = rationaleRow["rationaleA"].ToString();
                lblBDescription.Text = rationaleRow["rationaleB"].ToString();

                RationaleC.Visible = false;
                RationaleD.Visible = false;
                RationaleE.Visible = false;
            }



            testQuestionPieChart.Series.Add(nSeries);
            testQuestionPieChart.ChartTitle.TextBlock.Text = String.Concat("Item ", _sort);

        }

        private ChartSeriesItem GetChartSeriesItem(string label, double value)
        {
            var chartItem = new ChartSeriesItem(value);
            chartItem.Name = label;
            chartItem.Label.TextBlock.Text = label;

            return chartItem;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private string GetCriteriaOverrides(string level)
        {
            switch (level)
            {
                case "Class":
                    return "@@Product=none@@@@RR=none@@TID=" + _testID + "@@CID=" + _levelID + "@@TYRS=10-11@@TTERMS=All@@TTYPES=All@@@@1test=yes@@";

                case "Teacher":
                    return "@@Product=none@@@@RR=none@@TID=" + _testID + "@@TEAID=" + _levelID + "@@@@TYRS=10-11@@TTERMS=All@@TTYPES=All@@@@SCH=" + _schoolID + "@@@@1test=yes@@";

                case "School":
                case "District":
                    return "@@Product=none@@@@RR=none@@TID=" + _testID + "@@SCH=" + _schoolID + "@@@@TYRS=10-11@@TTERMS=All@@TTYPES=All@@@@1test=yes@@";
            }

            return "";
        }
    }
}