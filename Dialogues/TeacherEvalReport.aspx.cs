using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using Telerik.Charting.Styles;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Thinkgate.Classes;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Charting;
using Thinkgate.Base.Enums;
using System.Linq;

namespace Thinkgate.Dialogues
{
    public partial class TeacherEvalReport : BasePage
    {
        private int _rubricResultID;
        private Int32 _userID;
        private DataRow _evalDataRow;
        private EvaluationTypes _evaluationType;
        private DataTable _concordantGroups;
        private bool _isPrideGroup;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userID = SessionObject.LoggedInUser.Page;

            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            _rubricResultID = GetDecryptedEntityId(X_ID);

            LoadAndBindData();
        }

        #region Load Data Methods

        private void LoadAndBindData()
        {
            var dt = Thinkgate.Base.Classes.Rubric.GetEvaluationReportData(_rubricResultID);
            if (dt == null || dt.Rows.Count == 0)
            {
                RedirectToPortalSelectionScreenWithCustomMessage("Evaluation Not Found.");
            }
            
            _evalDataRow = dt.Rows[0];
            _isPrideGroup = _evalDataRow["Group"].ToString().ToUpper() == "PRIDE";

            DetermineEvaluationTypeAndSetPageTitle();

            lblTeacherName.Text = _evalDataRow["EvaluateeName"].ToString() + (string.IsNullOrEmpty(_evalDataRow["Group#"].ToString()) ? "" : " (" + _evalDataRow["Group#"].ToString() + ")");
            lblTeacherUserName.Text = _evalDataRow["EvaluateeUserName"].ToString().ToUpper();
            lblTeacherPosition.Text = _evalDataRow["Position"].ToString();

            var totalPoints = Math.Round(DataIntegrity.ConvertToDouble(_evalDataRow["TotalPoints"]), 2);
            lblTotalPoints.Text = totalPoints.ToString("0.00");

            lblPrideLabel.Text = (_evaluationType == EvaluationTypes.Administrator) ? "School-Based\nAdministrator\nAppraisal" : "PRIDE Component";

            var prideScore = Math.Round(DataIntegrity.ConvertToDouble(_evalDataRow["TotalScore"]), 2);
            BuildScoreDiv(divPrideScore, prideScore, "#99FF33", "Black");
            
            var growthScore = Math.Round(DataIntegrity.ConvertToDouble(_evalDataRow["StudentGrowthScore"]), 2);
            BuildScoreDiv(divStudentGrowthScore, growthScore, "Blue", "White");

            if (_isPrideGroup) lblPrideWeight.Text = "100";
            else if (!string.IsNullOrEmpty(_evalDataRow["StudentGrowthScore"].ToString())) lblPrideWeight.Text = (100 - (100 * DataIntegrity.ConvertToDouble(_evalDataRow["StudentGrowthWeight"]))).ToString() + "%";
            var growthPoints = Math.Round(DataIntegrity.ConvertToDouble(_evalDataRow["StudentGrowthPoints"]), 2);
            //BuildStudentGrowthDiv(growthPoints);

            lblPrideYears.Text = _evalDataRow["PrideYears"].ToString();
            lblStudentGrowthYears.Text = _evalDataRow["StudentGrowthYears"].ToString();
            if (_isPrideGroup) lblStudentGrowthWeight.Text = "0";
            else if (!string.IsNullOrEmpty(_evalDataRow["StudentGrowthScore"].ToString())) lblStudentGrowthWeight.Text = (100 * DataIntegrity.ConvertToDouble(_evalDataRow["StudentGrowthWeight"])).ToString() + "%";

            //var finalScore = (prideScore * (1 - DataIntegrity.ConvertToInt(_evalDataRow["StudentGrowthWeight"]))) + (growthScore * DataIntegrity.ConvertToInt(_evalDataRow["StudentGrowthWeight"]));
            var finalScore = DataIntegrity.ConvertToDouble(_evalDataRow["FinalEvaluationScore"]);
            //if (growthScore > 0) lblFinalRating.Text = GetFinalRatingText(finalScore);
            if (!string.IsNullOrEmpty(_evalDataRow["StudentGrowthScore"].ToString()) || _isPrideGroup) lblFinalRating.Text = string.Concat(finalScore.ToString(), " ", _evalDataRow["FinalRatingText"].ToString());
            if (!string.IsNullOrEmpty(_evalDataRow["StudentGrowthScore"].ToString()) || _isPrideGroup) BuildScoreDiv(divFinalScore, finalScore, "#E46C0A", "White");

            if (_isPrideGroup) studentGrowthDetailsLnk.Visible = false;

            SetupChart(prideScore, _evalDataRow["StudentGrowthScore"].ToString(), finalScore);
            GetAndBindConcordantGroups();
            GetAndBindConcordantTables();
            SetupSignatureRows();
        }

        private void GetAndBindConcordantTables()
        {
            prideConcordantRangesTable.DataSource = Thinkgate.Base.Classes.Rubric.GetConcordantTable(EvaluationTypes.TeacherClassroom, _evalDataRow["year"].ToString());
            prideConcordantRangesTable.DataBind();

            if (_evaluationType == EvaluationTypes.Administrator) //Only need this chart if current type is Administrator
            {
                adminConcordantRangesTable.DataSource = Thinkgate.Base.Classes.Rubric.GetConcordantTable(EvaluationTypes.Administrator, _evalDataRow["year"].ToString());
                adminConcordantRangesTable.DataBind();
            }
        }

        private void GetAndBindConcordantGroups()
        {
            _concordantGroups = Thinkgate.Base.Classes.Rubric.GetCorcordantGroups();
            List<Int32> groups = (from s in _concordantGroups.AsEnumerable() select s.Field<Int32>("groupNumber")).Distinct().ToList();
            concordantGroupsRepeater.DataSource = groups;
            concordantGroupsRepeater.DataBind();
        }

        #endregion

        #region On Item Data Bound Events

        protected void concordantGroupsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var groupNumber = (Int32)e.Item.DataItem;

                var concordantRows = _concordantGroups.Select("groupNumber = '" + groupNumber + "'");
                var concordantYears = _concordantGroups.Select("groupNumber = '" + groupNumber + "' and Year <> ''");
                var concordantYearRows = new DataTable();
                var groupCol = new DataColumn("groupNumber");
                var yearCol = new DataColumn("Year");
                concordantYearRows.Columns.Add(groupCol);
                concordantYearRows.Columns.Add(yearCol);
                var columns = new DataColumn[1];
                columns[0] = yearCol;
                concordantYearRows.PrimaryKey = columns;

                foreach(var row in concordantYears)
                {
                    if (concordantYearRows.Rows.Contains(row["Year"].ToString())) continue;
                    var newRow = concordantYearRows.NewRow();
                    newRow["groupNumber"] = row["groupNumber"];
                    newRow["Year"] = row["Year"];
                    concordantYearRows.Rows.Add(newRow);
                }

                ((Label)e.Item.FindControl("lblGroupNumber")).Text = "Group " + groupNumber;

                if (groupNumber % 2 == 0)
                    ((HtmlTableCell)e.Item.FindControl("groupNumberCell")).Style.Add("background-color", "#dadada");

                if (concordantYearRows.Rows.Count > 0)
                {
                    var concordantYearsRepeater = (Repeater)e.Item.FindControl("concordantYearsRepeater");
                    concordantYearsRepeater.DataSource = concordantYearRows;
                    concordantYearsRepeater.DataBind();
                }
                else
                {
                    var concordantRepeater = (Repeater)e.Item.FindControl("concordantRepeater");
                    concordantRepeater.DataSource = concordantRows;
                    concordantRepeater.DataBind();
                }
            }
        }

        protected void concordantRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var data = (DataRow)e.Item.DataItem;
                ((Label)e.Item.FindControl("lblConcordantName")).Text = data["concordantName"].ToString();
                ((HtmlAnchor)e.Item.FindControl("aTagOpenConcordant")).Attributes.Add("onclick", "openConcordantTable('" + data["ID"] + "')");
            }
        }

        protected void concordantYearsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var data = (DataRowView)e.Item.DataItem;
                var concordantRows = _concordantGroups.Select("groupNumber = '" + data["groupNumber"] + "' and Year = '" + data["Year"] + "'");
                ((Label)e.Item.FindControl("lblGroupYear")).Text = "School Year " + data["Year"].ToString();

                var concordantForYearRepeater = (Repeater)e.Item.FindControl("concordantForYearRepeater");
                concordantForYearRepeater.DataSource = concordantRows;
                concordantForYearRepeater.DataBind();
            }
        }

        #endregion

        #region Evaluation Components Chart Setup

        private void SetupChart(double prideScore, string growthScoreText, double finalScore)
        {
            evaluationChart.Series.Clear();

            var series = new ChartSeries("EvaluationData", ChartSeriesType.Bar);
            double growthScore = Math.Round(DataIntegrity.ConvertToDouble(growthScoreText), 2);

            series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.Nothing;
            series.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.White;
            series.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font(evaluationChart.ChartTitle.TextBlock.Appearance.TextProperties.Font.Name,
                            12, evaluationChart.ChartTitle.TextBlock.Appearance.TextProperties.Font.Unit);
            series.Appearance.LabelAppearance.LabelLocation = StyleSeriesItemLabel.ItemLabelLocation.Inside;

            evaluationChart.AddChartSeries(series);

            series.AddItem(GetChartSeriesItem(prideScore, System.Drawing.Color.FromArgb(153, 255, 51), System.Drawing.Color.FromArgb(0, 0, 0)));
            series.AddItem(GetChartSeriesItem(growthScore, System.Drawing.Color.FromArgb(0, 0, 255), System.Drawing.Color.FromArgb(255, 255, 255)));
            if (!string.IsNullOrEmpty(growthScoreText) || _isPrideGroup) series.AddItem(GetChartSeriesItem(finalScore, System.Drawing.Color.FromArgb(228, 108, 10), System.Drawing.Color.FromArgb(255, 255, 255)));

            //Chart Styles
            evaluationChart.PlotArea.XAxis.Visible = Telerik.Charting.Styles.ChartAxisVisibility.False;
            evaluationChart.PlotArea.YAxis.MaxValue = 4.0;
            evaluationChart.PlotArea.YAxis.AutoScale = false;

            BuildChartLegend((!string.IsNullOrEmpty(growthScoreText) || _isPrideGroup));
        }

        private ChartSeriesItem GetChartSeriesItem(double value, System.Drawing.Color color, System.Drawing.Color textColor)
        {
            var item = new ChartSeriesItem(value, value.ToString("0.00"));
            item.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;
            item.Appearance.FillStyle.MainColor = color;
            item.Label.TextBlock.Appearance.TextProperties.Color = textColor;
            return item;
        }

        private void BuildChartLegend(bool showFinalConcordant = true)
        {
            evaluationChart.Legend.RemoveAllLabels();

            var label = (_evaluationType == EvaluationTypes.Administrator)
                      ? "School-Based\nAdministrator Appraisal"
                      : "PRIDE Component";

            var fillStyle = new Telerik.Charting.Styles.FillStyle(System.Drawing.Color.FromArgb(153, 255, 51), Telerik.Charting.Styles.FillType.Solid);

            evaluationChart.Legend.AddCustomItemToLegend(label, fillStyle, "X");

            label = "Student Growth\nComponent";
            fillStyle = new Telerik.Charting.Styles.FillStyle(System.Drawing.Color.FromArgb(0, 0, 255), Telerik.Charting.Styles.FillType.Solid);

            evaluationChart.Legend.AddCustomItemToLegend(label, fillStyle, "X");

            if (showFinalConcordant)
            {
                label = "Final Concordant";
                fillStyle = new Telerik.Charting.Styles.FillStyle(System.Drawing.Color.FromArgb(228, 108, 10), Telerik.Charting.Styles.FillType.Solid);

                evaluationChart.Legend.AddCustomItemToLegend(label, fillStyle, "X");
            }
        }

        #endregion

        #region Helper Methods

        private void DetermineEvaluationTypeAndSetPageTitle()
        {
            switch (_evalDataRow["Type"].ToString())
            {
                case "TeacherClassroom":
                    _evaluationType = EvaluationTypes.TeacherClassroom;
                    Page.Title = "Classroom Teacher Final Evaluation Report for " + _evalDataRow["Year"].ToString();
                    break;
                case "TeacherNonClassroom":
                    _evaluationType = EvaluationTypes.TeacherNonClassroom;
                    Page.Title = "Non-Classroom Teacher Final Evaluation Report for " + _evalDataRow["Year"].ToString();
                    break;
                case "Administrator":
                    _evaluationType = EvaluationTypes.Administrator;
                    Page.Title = "School-Based Administrator Appraisal Report for " + _evalDataRow["Year"].ToString();
                    break;
            }

            hiddenEvalType.Value = _evaluationType.ToString();
        }


        private void SetupSignatureRows()
        {
            var dateEvaulateeSigned = DataIntegrity.ConvertToNullableDate(_evalDataRow["EvaluateeSignedDate"]);
            //var dateEvaulatorSigned =  DataIntegrity.ConvertToNullableDate(_evalDataRow["EvaluatorSignedDate"]);

            var hasEvaluateeSigned = dateEvaulateeSigned.HasValue;
            //var hasEvaluatorSigned = dateEvaulatorSigned.HasValue;

            chkEvaluateeSigned.Checked = hasEvaluateeSigned;
            divEvaluateeSignLine.Visible = hasEvaluateeSigned;
            //chkEvaluatorSigned.Checked = hasEvaluatorSigned;
            //divEvaluatorSignLine.Visible = hasEvaluatorSigned;

            chkEvaluateeSigned.Enabled = false;
            //chkEvaluatorSigned.Enabled = false;

            if (hasEvaluateeSigned)
            {
                lblEvaluateeSignature.Text = _evalDataRow["EvaluateeName"].ToString();
                lblEvaluateeDate.Text = String.Format("{0:MMMM dd, yyyy h:mm tt}", dateEvaulateeSigned.Value);
            }
            else if (_userID == DataIntegrity.ConvertToInt(_evalDataRow["EvaluateeUserID"])) //can only sign if evaluatee signed in
            {
                chkEvaluateeSigned.Enabled = true;
            }

            /*
            if (hasEvaluatorSigned)
            {
                lblEvaluatorSignature.Text = _evalDataRow["EvaluatorName"].ToString();
                lblEvaluatorDate.Text = dateEvaulatorSigned.Value.ToShortDateString();
            }
            else if (_userID == DataIntegrity.ConvertToInt(_evalDataRow["EvaluatorUserID"])) //can only sign if evaluatee signed in
            {
                chkEvaluatorSigned.Enabled = true;
            }
            */
        }

        private void BuildScoreDiv(HtmlGenericControl div, double score, string color, string fontColor)
        {
            var width = Math.Round(100 * score / 4.00, 2);
            bool scoreLess1 = score < 1;
            fontColor = fontColor == "White" && scoreLess1 ? "Black" : fontColor;

            div.Attributes.Add("style", "color: " + fontColor + "; background-color: " + color + "; width: " + width + "%;" + (scoreLess1 ? "float:left;" : ""));
            if (scoreLess1)
            {
                var divContainer = (HtmlGenericControl)FindControl(div.ID + "Container");
                if (divContainer != null)
                {
                    divContainer.Attributes.Add("style", "color: " + fontColor + "; font-weight: bold; text-align: left;");
                    divContainer.InnerHtml = score.ToString("0.00");
                    divContainer.Controls.AddAt(0, div);
                }
            }
            else
            {
                div.InnerText = score.ToString("0.00");
            }
        }

        private string GetFinalRatingText(double score)
        {
            var text = score.ToString("0.00");
            if (score > 3.0) return text + " Highly Effective";
            else if (score > 2.0) return text + " Effective";
            else if (score > 1.0) return text + " Needs Improvement/Developing";
            else return text + " Unsatisfactory";
        }

        #endregion

        protected void chkSigned_Checked(object sender, EventArgs e)
        {
            var success = Thinkgate.Base.Classes.Rubric.SignEvaluationReport(_rubricResultID, _userID);
            LoadAndBindData();
        }
    }
}
