using System;
using System.Collections.Generic;
using System.Data;
using Standpoint.Core.Utilities;
using Thinkgate.Classes;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;
using Thinkgate.Base.Enums;

namespace Thinkgate.Dialogues
{
    public partial class TeacherEvalForm : BasePage
    {
        private int _rubricResultID;
        private DataTable _evalData;
        private int _compentencyCounter;
        private double _overallDistrictAverage;
        private EvaluationTypes _evaluationType;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }

            _rubricResultID = GetDecryptedEntityId(X_ID);

            LoadAndBindData();
        }

        private void LoadAndBindData()
        {
            var ds = Base.Classes.Rubric.GetEvaluationFormData(_rubricResultID);
            if (ds == null || ds.Tables.Count < 2) return;
            var overallData = ds.Tables[0];
            _evalData = ds.Tables[1];
            if (overallData.Rows.Count == 0) return;

            DetermineEvaluationTypeAndConfigurePage(overallData.Rows[0]["Type"].ToString(), overallData.Rows[0]["Year"].ToString());

            lblTeacherName.Text = overallData.Rows[0]["EvaluateeName"].ToString();
            lblSchool.Text = overallData.Rows[0]["SchoolName"].ToString();
            lblPosition.Text = overallData.Rows[0]["Position"].ToString();

            //lblEvaluator.Text = _overallData.Rows[0]["EvaluatorName"].ToString();

            lblTotalPoints.Text = Math.Round(DataIntegrity.ConvertToDouble(overallData.Rows[0]["TotalPoints"]), 2).ToString("0.00");
            lblOverallTotalPoints.Text = Math.Round(DataIntegrity.ConvertToDouble(overallData.Rows[0]["TotalPoints"]), 2).ToString("0.00");
            lblConcordantScore.Text = Math.Round(DataIntegrity.ConvertToDouble(overallData.Rows[0]["TotalScore"]), 2).ToString("0.00");

            List<Int32> comps = (from s in _evalData.AsEnumerable() select s.Field<Int32>("CompentencyID")).Distinct().ToList();

            compentencyRepeater.DataSource = comps;
            compentencyRepeater.DataBind();
        }

        private void DetermineEvaluationTypeAndConfigurePage(string type, string year)
        {
            switch (type)
            {
                case "TeacherClassroom":
                    _evaluationType = EvaluationTypes.TeacherClassroom;
                    Page.Title = string.Format("Classroom Teacher PRIDE Final Evaluation {0}", year);
                    trPosition.Visible = false;
                    break;
                case "TeacherNonClassroom":
                    _evaluationType = EvaluationTypes.TeacherNonClassroom;
                    Page.Title = string.Format("Non-Classroom Teacher PRIDE Final Evaluation {0}", year);
                    trPosition.Visible = false;
                    break;
                case "Administrator":
                    _evaluationType = EvaluationTypes.Administrator;
                    Page.Title = string.Format("School-Based Administrator Appraisal for {0}", year);
                    trPosition.Visible = true;

                    //Hide All Distrct Avg and Prof Dev
                    thDistrictAvg.Visible = false;
                    tdDistrictAvg.Visible = false;
                    thProfDev.Visible = false;
                    tdProfDev.Visible = false;
                    tableOverallScore.Visible = false;
                    break;
            }
        }

        protected void compentencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var compID = (Int32)e.Item.DataItem;
            _compentencyCounter++;

            var skillsRepeater = (Repeater)e.Item.FindControl("skillsRepeater");
            var compRows = _evalData.Select("CompentencyID = '" + compID + "'");

            if (compRows.Length > 0)
                ((Label)e.Item.FindControl("lblCompText")).Text = compRows[0]["CompentencyText"].ToString();
            else
                ((Label)e.Item.FindControl("lblCompText")).Text = string.Format("Compentency {0}", _compentencyCounter);

            skillsRepeater.DataSource = compRows;
            skillsRepeater.DataBind();

            SetTotalPoints(compRows, ((Label)e.Item.FindControl("lblCompTotalPoints")), ((Label)e.Item.FindControl("lblCompDistrictAvg")));

            if (_evaluationType != EvaluationTypes.Administrator) return;

            e.Item.FindControl("tdDistrictAvg").Visible = false;
            e.Item.FindControl("tdProfDev").Visible = false;
        }

        protected void skillsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    {
                        var data = (DataRow)e.Item.DataItem;
                        var points = DataIntegrity.ConvertToInt(data["Points"]);

                        ((Label)e.Item.FindControl("lblSkillText")).Text = data["SkillText"].ToString();
                        ((HiddenField)e.Item.FindControl("hiddenRubricText")).Value = data["RubricText"].ToString();

                        e.Item.FindControl("lblScore0").Visible = (points == 0);
                        e.Item.FindControl("lblScore1").Visible = (points == 1);
                        e.Item.FindControl("lblScore2").Visible = (points == 2);
                        e.Item.FindControl("lblScore3").Visible = (points == 3);

                        ((Label)e.Item.FindControl("lblWeight")).Text = Math.Round(DataIntegrity.ConvertToDouble(data["Weight"]), 2).ToString("0.00");
                        ((Label)e.Item.FindControl("lblTotalPoints")).Text = Math.Round(DataIntegrity.ConvertToDouble(data["TotalPoints"]), 2).ToString("0.00");
                        ((Label)e.Item.FindControl("lblDistrictAverage")).Text = Math.Round(DataIntegrity.ConvertToDouble(data["DistrictAverage"]), 2).ToString("0.00");

                        if (_evaluationType == EvaluationTypes.Administrator)
                        {
                            e.Item.FindControl("tdDistrictAvg").Visible = false;
                            e.Item.FindControl("tdProfDev").Visible = false;
                        }

                        //TODO: Would be nice to only enable each link if resources exist.
                        ((HtmlTableCell)e.Item.FindControl("tdProfDev")).Attributes.Add("rubricItemID", data["RubricItemID"].ToString());
                    }
                    break;
                case ListItemType.Header:
                    if (_evaluationType == EvaluationTypes.Administrator)
                    {
                        e.Item.FindControl("thDistrictAvg").Visible = false;
                        e.Item.FindControl("thProfDev").Visible = false;
                    }
                    break;
            }
        }

        private void SetTotalPoints(IEnumerable<DataRow> compRows, Label lblTotal, Label lblAvg)
        {
            var totalPoints = 0.00;
            var districtAvg = 0.00;
            foreach (var row in compRows)
            {
                totalPoints += DataIntegrity.ConvertToDouble(row["TotalPoints"]);
                districtAvg += DataIntegrity.ConvertToDouble(row["DistrictAverage"]);
            }

            lblTotal.Text = Math.Round(DataIntegrity.ConvertToDouble(totalPoints), 2).ToString("0.00");
            lblAvg.Text = Math.Round(DataIntegrity.ConvertToDouble(districtAvg), 2).ToString("0.00");

            //_overallTotalPoints += totalPoints;
            _overallDistrictAverage += districtAvg;

            lblOverallDistrictAverage.Text = Math.Round(DataIntegrity.ConvertToDouble(_overallDistrictAverage), 2).ToString("0.00");
        }
    }
}
