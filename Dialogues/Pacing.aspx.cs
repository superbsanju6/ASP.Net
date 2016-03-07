using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Thinkgate.Classes;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Thinkgate.Dialogues
{
    public partial class Pacing : BasePage
    {
        public int _planID;
        public Base.Classes.InstructionalPlan _selectedPlan;
        public Base.Classes.Standards _selectedStandard;

        private const double SCORE_RED_THRESHOLD = 60.00;
        private const double SCORE_YELLOW_THRESHOLD = 85.00;

        protected void Page_Load(object sender, EventArgs e)
        {            
            SessionObject = (SessionObject)Session["SessionObject"];

            LoadInstructionalPlan();
            if (_selectedPlan == null) return;

            SetPageTitle();
            
            if (_selectedPlan.Standards.Count == 0) return;

            SetSelectedStandard();
            if (_selectedStandard == null) return; //Error determining selected standard
            BindStandardsRepeater();

            SetEssentialTextBlocks();
            BindAssessmentRepeaters();
            BindLessonPlans();
            BindResources();            
        }

        private void SetSelectedStandard()
        {
            if (hiddenSelectedStandard.Value.Length > 0)
            {
                _selectedStandard = _selectedPlan.Standards.Find(t => t.ID == DataIntegrity.ConvertToInt(hiddenSelectedStandard.Value));                
            }
            else //select first standard
            {
                _selectedStandard = _selectedPlan.Standards[0];
            }
        }

        private void LoadInstructionalPlan()
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _planID = DataIntegrity.ConvertToInt(Request.QueryString["xID"]);
                var key = "InstructionalPlan_" + _planID;
                if (!RecordExistsInCache(key)) return;
                _selectedPlan = ((Base.Classes.InstructionalPlan)Base.Classes.Cache.Get(key));
            }
        }

        private void SetPageTitle()
        {
            var title = "Period " + _selectedPlan.Period + ":"
                      + _selectedPlan.ShortTitle
                      + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                      + "Term 3" //TODO: Show Term
                      + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                      + "Begins: " + _selectedPlan.StartDate.ToShortDateString()
                      + "&nbsp;&nbsp;Ends: " + _selectedPlan.EndDate.ToShortDateString();

            Page.Title = title;
        }
        
        private void SetEssentialTextBlocks()
        {
            essentialLearningDiv.InnerHtml = _selectedPlan.EssentialLearningHtml;
            essentialTerminologyDiv.InnerHtml = _selectedPlan.EssentialTermsHtml;
        }

        private void BindStandardsRepeater()
        {
            repeaterStandards.DataSource = _selectedPlan.Standards;
            repeaterStandards.DataBind();
        }

        private void BindAssessmentRepeaters()
        {
            repeaterDistrictAssessments.DataSource = _selectedPlan.Assessments.FindAll(t => t.Category == Base.Enums.AssessmentCategories.District);
            repeaterDistrictAssessments.DataBind();

            repeaterClassroomAssessments.DataSource = _selectedPlan.Assessments.FindAll(t => t.Category == Base.Enums.AssessmentCategories.Classroom);
            repeaterClassroomAssessments.DataBind();                
        }

        private void BindLessonPlans()
        {
            repeaterLessonPlans.DataSource = _selectedPlan.LessonPlans;
            repeaterLessonPlans.DataBind();
        }

        private void BindResources()
        {
            repeaterResources.DataSource = _selectedPlan.Resources;
            repeaterResources.DataBind();
        }

        protected void repeaterStandards_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {            
            if (e.Item.ItemType == ListItemType.Header)
            {
                var lblInstructionalDays = (Label)e.Item.FindControl("lblInstructionalDays");
                var lblUnplannedDays = (Label)e.Item.FindControl("lblUnplannedDays");

                lblInstructionalDays.Text = "12"; //TODO
                lblUnplannedDays.Text = "5";//TODO
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (Thinkgate.Base.Classes.Standards)e.Item.DataItem;
                if (dataItem == null) return;

                var radioSelectedItem = (RadioButton)e.Item.FindControl("radioSelectedItem");
                var chkIsCompleted = (CheckBox)e.Item.FindControl("chkIsCompleted");
                var tdDistrictScore = (HtmlTableCell)e.Item.FindControl("tdDistrictScore");
                var tdClassroomScore = (HtmlTableCell)e.Item.FindControl("tdClassroomScore");
                var trStandard = (HtmlTableRow)e.Item.FindControl("trStandard");

                if (dataItem.Equals(_selectedStandard)) //We're on the Selected Standard
                {
                    hiddenSelectedStandard.Value = dataItem.ID.ToString();
                    radioSelectedItem.Checked = true;
                    trStandard.Style.Add("background-color", "#99BB33");
                }
                else
                {
                    radioSelectedItem.Checked = false;
                }

                chkIsCompleted.Checked = dataItem.Completed;

                SetScoreColor(tdDistrictScore, dataItem.DistrictScore);
                SetScoreColor(tdClassroomScore, dataItem.ClassroomScore);
            }
        }

        private void SetScoreColor(HtmlTableCell cell, double score)
        {
            if (score <= SCORE_RED_THRESHOLD)
                cell.Style.Add("background-color", "red");
            else if (score <= SCORE_YELLOW_THRESHOLD)
                cell.Style.Add("background-color", "yellow");
            else
                cell.Style.Add("background-color", "green");
        }
    }
}
