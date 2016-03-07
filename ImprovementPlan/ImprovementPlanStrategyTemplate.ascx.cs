using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Enums.ImprovementPlan;
using Thinkgate.Services.Contracts.ImprovementPlanService;
using Thinkgate.Utilities;
//using ExtensionHelper = Thinkgate.DTO.ImprovementPlanService.ExtensionHelper;

namespace Thinkgate.ImprovementPlan
{
    public partial class ImprovementPlanStrategyTemplate : UserControl
    {
        #region Properties
        private int ImprovementID { get; set; }
        private ActionType ActionType { get; set; }
        private SessionObject SessionObject { get; set; }
        private ImprovementPlanStrategyInfo ImprovementPlanStrategyInfo { get; set; }
        private ImprovementPlanOutput ImprovementPlanOutput { get; set; }
        private string DistrictName { get; set; }
        private string ClientID { get; set; }
        private string ClientName { get; set; }
        private int? currentStrategyValue = default(int?);
        private EventTargets eventTargetEnum;
        private int districtRowCount = 1;

        #endregion Properties

        #region Private Methods

        private void LoadDateByActionType()
        {
            switch (ActionType)
            {
                case Enums.ImprovementPlan.ActionType.Edit:
                    GetImprovementPlanData();
                    BindStrategyDropDown();
                    break;
            }
        }

        /// <summary>
        /// Get the client id from the district params
        /// </summary>
        private void GetClientID()
        {
            DistrictParms districtParams = DistrictParms.LoadDistrictParms();
            if (districtParams != default(DistrictParms))
            {
                this.ClientID = districtParams.ClientID;
                this.ClientName = districtParams.ImprovementPlanDistrictDisplayName;
            }
        }

        /// <summary>
        /// Get the improvement plan id and action type from query string.
        /// If those are not supplied the page will redirect with an error message.
        /// </summary>
        private void GetQueryStringData()
        {
            if (Request.QueryString["impID"] != null)
                this.ImprovementID = DataIntegrity.ConvertToInt(Request.QueryString["impID"]);

            if (Request.QueryString["actType"] != null)
                this.ActionType = (ActionType)Enum.Parse(typeof(ActionType), Request.QueryString["actType"]);

            //this.ImprovementID = 1;
            //this.ActionType = Enums.ImprovementPlan.ActionType.Edit;

            if (ImprovementID == default(int) || ActionType == ActionType.None)
            {
                SessionObject.RedirectMessage = "No improvement plan id provided";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
        }

        /// <summary>
        /// Get the improvement data from database
        /// </summary>
        private void GetImprovementPlanData()
        {
            this.ImprovementPlanStrategyInfo = new ImprovementPlanProxy().GetImprovementPlanStrategyInfo(this.ImprovementID, this.ClientID);

        }

        /// <summary>
        /// 
        /// </summary>
        private void BindHeader()
        {
            this.lblDistrict.Text = this.ClientName;

            if (this.ImprovementPlanStrategyInfo != null && this.ImprovementPlanStrategyInfo.ImprovementPlanInfo != null)
            {
                SetPlanTypeMode(this.ImprovementPlanStrategyInfo.ImprovementPlanInfo.PlanType);

                if (this.ImprovementPlanStrategyInfo.ImprovementPlanInfo.School != null)
                    lblSchoolVal.Text = this.ImprovementPlanStrategyInfo.ImprovementPlanInfo.School.SchoolName;

                lblSchoolYear.Text = this.ImprovementPlanStrategyInfo.ImprovementPlanInfo.Year;

                lblPageTitle.Text = string.Format("ANNUAL {0} ACADEMIC IMPROVEMENT PLAN", this.ImprovementPlanStrategyInfo.ImprovementPlanInfo.PlanType.ToString().ToUpper());
            }
        }

        /// <summary>
        /// Bind the strategy dropdown details
        /// </summary>
        private void BindStrategyDropDown()
        {
            if (this.ImprovementPlanStrategyInfo != null)
            {
                this.ddlStrategy.Items.Clear();

                if (this.ImprovementPlanStrategyInfo.ImprovementPlanStrategies != null)
                {
                    foreach (ImprovementPlanStrategy planStrategy in this.ImprovementPlanStrategyInfo.ImprovementPlanStrategies)
                    {
                        ListItem listItem = new ListItem();
                        listItem.Value = planStrategy.ID.ToString();
                        listItem.Text = planStrategy.StrategyName;
                        listItem.Selected = (planStrategy.ID == currentStrategyValue);
                        this.ddlStrategy.Items.Add(listItem);
                    }
                }

                if (currentStrategyValue == default(int?))
                    ddlStrategy.SelectedIndex = 0;

                ddlStrategy_SelectedIndexChanged(this.ddlStrategy, new EventArgs());
            }
        }


        /// <summary>
        /// Create the action step entities from the controls
        /// </summary>
        /// <param name="actionCtrl"></param>
        /// <param name="statusCtrl"></param>
        /// <param name="strategyCtrl"></param>
        /// <param name="actionStepCtrl"></param>
        /// <param name="personResponsibleCtrl"></param>
        /// <param name="startDateCtrl"></param>
        /// <param name="finishDateCtrl"></param>
        /// <param name="resourceCostsCtrl"></param>
        /// <param name="expectedResultsCtrl"></param>
        /// <returns></returns>
        private ImprovementPlanActionStep CreateActionStepFromControl(HiddenField actionCtrl,
                                                                   DropDownList statusCtrl,
                                                                   DropDownList strategyCtrl,
                                                                   TextBox actionStepCtrl,
                                                                   TextBox personResponsibleCtrl,
                                                                   RadDatePicker startDateCtrl,
                                                                   RadDatePicker finishDateCtrl,
                                                                   TextBox resourceCostsCtrl,
                                                                   TextBox expectedResultsCtrl)
        {
            return new ImprovementPlanActionStep
            {
                ID = DataIntegrity.ConvertToInt(actionCtrl.Value),
                StrategicGoalID = DataIntegrity.ConvertToNullableInt(strategyCtrl.SelectedValue),
                StrategyID = DataIntegrity.ConvertToInt(this.ddlStrategy.SelectedValue),
                ActionStep = actionStepCtrl.Text,
                PersonResponsible = personResponsibleCtrl.Text,
                ImprovementPlanID = this.ImprovementID,
                StartDate = startDateCtrl.SelectedDate,
                FinishDate = finishDateCtrl.SelectedDate,
                StatusID = DataIntegrity.ConvertToNullableInt(statusCtrl.SelectedValue),
                ResourceCosts = resourceCostsCtrl.Text,
                ExpectedResults = expectedResultsCtrl.Text
            };
        }


        /// <summary>
        /// Raise the event based on the autopost back
        /// </summary>
        /// <param name="eventTargets"></param>
        private void RaiseEventByTarget(string eventTargets)
        {


            Enum.TryParse(eventTargets, true, out eventTargetEnum);

            switch (eventTargetEnum)
            {
                case EventTargets.btnSave:
                    btnSave_Click(this.btnSave, new EventArgs());
                    ddlStrategy_SelectedIndexChanged(this.ddlStrategy, new EventArgs());
                    break;
                case EventTargets.btnCover:
                    Response.Redirect("ImprovementCoverPage.aspx?impID=" + ImprovementID + "&actType=" + ActionType.Edit, true);
                    break;
                case EventTargets.btnDelete:
                    btnDelete_Click(this.btnDelete, new EventArgs());
                    break;
                case EventTargets.btnSaveAndAdd:
                    btnSave_Click(this.btnSave, new EventArgs());
                    ddlStrategy_SelectedIndexChanged(this.ddlStrategy, new EventArgs());
                    break;
                case EventTargets.ddlStrategy:
                    ddlStrategy_SelectedIndexChanged(this.ddlStrategy, new EventArgs());
                    break;
            }
        }

        /// <summary>
        /// Remove the strategy dropdown values when user perform delete operation
        /// </summary>
        /// <param name="listItem"></param>
        private void RemoveStrategyDropDown(ListItem listItem)
        {
            if (this.ddlStrategy.Items.Count > 0)
            {
                this.ddlStrategy.Items.Remove(listItem);
            }
        }

        /// <summary>
        /// Update the strategy dropdown when user changes the existing strategy value
        /// </summary>
        /// <param name="listItem"></param>
       // private void ReplaceStrategyDropDown(ListItem listItem, bool isNew = default(bool))
        private void ReplaceStrategyDropDown(ListItem listItem, bool isNew = default(bool))
        {
                if (!isNew)
            {
                if (ddlStrategy.Items.Count > 0)
                {
                var currentIndex = this.ddlStrategy.SelectedIndex;
                    ddlStrategy.Items.RemoveAt(currentIndex);
                    ddlStrategy.Items.Insert(currentIndex, listItem);

                }
                else
                {
                    ddlStrategy.SelectedIndex = -1;
                    ddlStrategy.Items.Add(listItem);
                    ddlStrategy.SelectedIndex = 0;
                }
            }
            else
            {
                ddlStrategy.SelectedIndex = -1;
                ddlStrategy.Items.Insert(0, listItem);
                ddlStrategy.SelectedIndex = 0;
        }
        }
        /// <summary>
        /// Set the page view based on action type
        /// </summary>
        private void SetMode()
        {
            switch (this.ActionType)
            {
                case Enums.ImprovementPlan.ActionType.View:
                case Enums.ImprovementPlan.ActionType.None:

                    this.dvActions.Visible = this.btnAddSmartGoal.Visible = this.btnAddAction.Visible = false;
                    this.txtPersonResponsibles.ReadOnly = this.txtStrategy.ReadOnly = true;

                    this.ddlStrategy.Visible = false;

                    this.txtPersonResponsibles.CssClass = this.txtStrategy.CssClass = "viewModeCss text-area-height width-half white-forecolor";
                    break;
            }
        }

        /// <summary>
        /// Set the control based on plan type
        /// </summary>
        private void SetPlanTypeMode(ImprovementPlanType planType)
        {
            switch (planType)
            {
                case ImprovementPlanType.District:
                    this.lblSchool.Visible = this.lblSchoolVal.Visible = false;
                    break;
            }
            
        }

     
        #endregion Private Methods

        #region Events

        /// <summary>
        /// Fires on page load
        /// Gets the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            SessionObject = (SessionObject)Session["SessionObject"];
            GetClientID();
            GetQueryStringData();
            SetMode();
            if (!Page.IsPostBack)
            {
                LoadDateByActionType();
            }
            if (Request.Form["__EVENTTARGET"] != null && !string.IsNullOrEmpty(Request.Form["__EVENTTARGET"].ToString()))
            {
                RaiseEventByTarget(Request.Form["__EVENTTARGET"].ToString());
            }

            BindHeader();
        }

        /// <summary>
        /// Fires when user changes any strategy 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList strategyCtrl = (DropDownList)sender;

            if (strategyCtrl != null && strategyCtrl.SelectedItem != null)
            {



                    //Get Strategy Goal , District Strategic Goal, Status And Action Info
                    this.ImprovementPlanOutput = new ImprovementPlanProxy().GetImprovementPlanByActions(
                                                                      DataIntegrity.ConvertToInt(strategyCtrl.SelectedItem.Value),
                                                                      this.ImprovementID,
                                                                      new List<ImprovementPlanActions> 
                                                                  { 
                                                                      ImprovementPlanActions.SmartGoal, 
                                                                      ImprovementPlanActions.Status, 
                                                                      ImprovementPlanActions.StrategicGoal, 
                                                                      ImprovementPlanActions.ActionStep,
                                                                      ImprovementPlanActions.StrategyPlan
                                                                  },
                                                                      this.ClientID);

                this.txtStrategy.Text = strategyCtrl.SelectedItem.Text;

                this.currentStrategyValue = DataIntegrity.ConvertToNullableInt(strategyCtrl.SelectedValue);

                this.txtStrategy.Attributes.Add("InitialValue", this.txtStrategy.Text);


                this.txtPersonResponsibles.Text = this.ImprovementPlanOutput.PersonResponsible;
                this.txtPersonResponsibles.Attributes.Add("InitialValue", txtPersonResponsibles.Text);

                this.rptSmartGoal.DataSource = this.ImprovementPlanOutput.ImprovementPlanSmartGoal ?? new List<ImprovementPlanSmartGoal> { new ImprovementPlanSmartGoal { } };
                this.rptSmartGoal.DataBind();


                this.dgActions.DataSource = this.ImprovementPlanOutput.ImprovementPlanActionStep ?? new List<ImprovementPlanActionStep> { new ImprovementPlanActionStep { } };
                this.dgActions.DataBind();


                //this.rptDistrictStrategyGoal.DataSource = this.ImprovementPlanOutput.ImprovementPlanActionStep ?? new List<ImprovementPlanActionStep> { new ImprovementPlanActionStep { } };


                //this.rptDistrictStrategyGoal.DataBind();
            }
            else
            {
                this.txtPersonResponsibles.Text = this.txtStrategy.Text = string.Empty;

                this.ImprovementPlanOutput = new ImprovementPlanProxy().GetImprovementPlanByActions(
                                                                  0,
                                                                  this.ImprovementID,
                                                                  new List<ImprovementPlanActions> 
                                                                  {                                                                        
                                                                      ImprovementPlanActions.Status, 
                                                                      ImprovementPlanActions.StrategicGoal
                                                                  },
                                                                  this.ClientID);

                rptSmartGoal.DataSource = new List<ImprovementPlanSmartGoal> { new ImprovementPlanSmartGoal { } };
                rptSmartGoal.DataBind();

                this.dgActions.DataSource = new List<ImprovementPlanActionStep> { new ImprovementPlanActionStep { } };
                this.dgActions.DataBind();

                //this.rptDistrictStrategyGoal.DataSource = new List<ImprovementPlanActionStep> { new ImprovementPlanActionStep { } };
                //this.rptDistrictStrategyGoal.DataBind();
            }
        }

        /// <summary>
        /// Bind the action plans data into the repeater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptDistrictStrategyGoal_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Header)
            {
                ImprovementPlanActionStep actionStep = e.Item.DataItem as ImprovementPlanActionStep;

                if (districtRowCount > 1)
                {
                    HtmlTable strategyTable = e.Item.FindControl("tbStrategyGoalItem") as HtmlTable;
                    if (strategyTable.Rows.Count > 0)
                    {
                        strategyTable.Rows[0].Attributes["style"] = "visibility:collapse";
                        strategyTable.Rows[0].Attributes["class"] = "";
                    }
                }

                //Set Status
                #region Status Dropdown
                DropDownList statusCtrl = e.Item.FindControl("ddlStatus") as DropDownList;
                if (this.ActionType == ActionType.Edit)
                {
                    if (statusCtrl != null && this.ImprovementPlanOutput != null && this.ImprovementPlanOutput.ImprovementPlanStatusKey != null)
                    {
                    statusCtrl.Items.Clear();
                    ExtensionHelper.ForEach(this.ImprovementPlanOutput.ImprovementPlanStatusKey.AsParallel(), loop =>
                    {
                            statusCtrl.Items.Add(new ListItem { Text = loop.StatusKey, Value = loop.ID.ToString(), Selected = (loop.ID == actionStep.StatusID) });
                    });
                }

                    statusCtrl.Items.Insert(0, new ListItem { Text = " ", Value = "", Selected = (actionStep.StatusID == default(int?)) });
                }
                else
                {
                    statusCtrl.Visible = false;
                    Label statusLabelCtrl = e.Item.FindControl("lblStatus") as Label;
                    statusLabelCtrl.Visible = true;
                    statusLabelCtrl.Text = actionStep.StatusDescription;
                }
                #endregion Status Dropdown

                //Set Strategic Goal
                #region Strategy Control
                DropDownList strategyCtrl = e.Item.FindControl("ddlStrategyGoal") as DropDownList;
                if (this.ActionType == Enums.ImprovementPlan.ActionType.Edit)
                {
                    if (strategyCtrl != null && this.ImprovementPlanOutput != null && this.ImprovementPlanOutput.ImprovementPlanStrategicGoal != null)
                    {
                    strategyCtrl.Items.Clear();
                        int strategyGoalCount = default(int);
                    ExtensionHelper.ForEach(this.ImprovementPlanOutput.ImprovementPlanStrategicGoal.AsParallel(), loop =>
                    {
                            strategyGoalCount++;
                            ListItem strategy = new ListItem();
                            strategy.Text = string.Format("Strategic Goal {0}", strategyGoalCount);
                            strategy.Value = loop.ID.ToString();
                            strategy.Selected = (loop.ID == actionStep.StrategicGoalID);
                            strategy.Attributes["title"] = loop.StrategicGoal;
                            strategyCtrl.Items.Add(strategy);
                    });
                    }

                    strategyCtrl.Items.Insert(0, new ListItem { Text = "Select Strategic Goal", Value = "", Selected = (actionStep.StrategicGoalID == default(int?)) });
                }
                else
                {
                    strategyCtrl.Visible = false;
                    Label strategyLabelCtrl = e.Item.FindControl("lblStrategyGoal") as Label;
                    strategyLabelCtrl.Visible = true;
                    strategyLabelCtrl.Text = actionStep.StrategyGoalName;
                }
                #endregion Strategy Control

                //Set Action Step Details
                #region Action Control
                if (actionStep != null)
                {
                    TextBox actionCtrl = ((TextBox)e.Item.FindControl("txtActionSteps"));
                    TextBox personCtrl = ((TextBox)e.Item.FindControl("txtPersonResponsible"));
                    TextBox resourceCtrl = ((TextBox)e.Item.FindControl("txtResrouceCosts"));
                    TextBox expectedCtrl = ((TextBox)e.Item.FindControl("txtExpectedResults"));
                    RadDatePicker startDateCtrl = ((RadDatePicker)e.Item.FindControl("rdpStartDate"));
                    RadDatePicker finishDateCtrl = ((RadDatePicker)e.Item.FindControl("rdpFinishDate"));

                    if (ActionType == Enums.ImprovementPlan.ActionType.Edit)
                    {

                        actionCtrl.Text = actionStep.ActionStep ?? default(string);
                        personCtrl.Text = actionStep.PersonResponsible ?? default(string);
                        resourceCtrl.Text = actionStep.ResourceCosts ?? default(string);
                        expectedCtrl.Text = actionStep.ExpectedResults ?? default(string);
                        if (ActionType == Enums.ImprovementPlan.ActionType.Edit)
                        {
                            startDateCtrl.SelectedDate = actionStep.StartDate ?? default(DateTime?);
                            finishDateCtrl.SelectedDate = actionStep.FinishDate ?? default(DateTime?);
                        }
                        else
                        {
                            personCtrl.Visible = resourceCtrl.Visible = expectedCtrl.Visible =
                            actionCtrl.Visible = expectedCtrl.Visible = resourceCtrl.Visible =
                            startDateCtrl.Visible = finishDateCtrl.Visible = false;

                            Label startDatelbl = ((Label)e.Item.FindControl("lblStartDate"));
                            Label finishDatelbl = ((Label)e.Item.FindControl("lblFinishDate"));
                            Label actionStepLbl = ((Label)e.Item.FindControl("lblActionSteps"));
                            Label personRespLbl = ((Label)e.Item.FindControl("lblPersonResponsible"));
                            Label resourceCostLbl = ((Label)e.Item.FindControl("lblResourceCost"));
                            Label expectedResultLbl = ((Label)e.Item.FindControl("lblExpectedResults"));



                            startDatelbl.Visible = finishDatelbl.Visible = actionStepLbl.Visible = personRespLbl.Visible = resourceCostLbl.Visible =
                            expectedResultLbl.Visible = true;

                            actionStepLbl.Text = actionStep.ActionStep ?? default(string);
                            personRespLbl.Text = actionStep.PersonResponsible ?? default(string);
                            resourceCostLbl.Text = actionStep.ResourceCosts ?? default(string);
                            expectedResultLbl.Text = actionStep.ExpectedResults ?? default(string);


                            startDatelbl.Text = actionStep.StartDate != default(DateTime?) ? DataIntegrity.ConvertToDate(actionStep.StartDate).ToShortDateString() : default(string);
                            finishDatelbl.Text = actionStep.FinishDate != default(DateTime?) ? DataIntegrity.ConvertToDate(actionStep.FinishDate).ToShortDateString() : default(string);
                        }
                    }
                }
                #endregion Action Control


                districtRowCount++;
            }
        }

        /// <summary>
        /// Bind the smart goal to the repeater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptSmartGoal_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            TextBox smartGoalCtrl = (TextBox)e.Item.FindControl("txtSmartGoal");
            Label smartGoalLblCtrl = (Label)e.Item.FindControl("lblSmartGoalDetail");

            if (ActionType != Enums.ImprovementPlan.ActionType.Edit)
            {
                smartGoalCtrl.Visible = false;
                smartGoalLblCtrl.Visible = true;                
            }
        }

        /// <summary>
        /// Save the improvement plan details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

               var improvementPlan = new Thinkgate.Services.Contracts.ImprovementPlanService.ImprovementPlan();
                improvementPlan.ID = this.ImprovementID;

                List<ImprovementPlanActions> actions = new List<ImprovementPlanActions>();
                ImprovementPlanStrategy planStrategy = default(ImprovementPlanStrategy);
                List<ImprovementPlanSmartGoal> smartGoals = new List<ImprovementPlanSmartGoal>();
                List<ImprovementPlanActionStep> actionSteps = new List<ImprovementPlanActionStep>();

                #region Get Strategy

                //if (eventTargetEnum == EventTargets.btnSaveAndAdd || txtStrategy.Attributes["InitialValue"] != txtStrategy.Text.Trim() ||
                //    this.txtPersonResponsibles.Attributes["InitialValue"].ToString() != this.txtPersonResponsibles.Text.Trim()
                //    )


                if (txtStrategy.Attributes["InitialValue"] != txtStrategy.Text.Trim() ||
                    txtPersonResponsibles.Attributes["InitialValue"] != txtPersonResponsibles.Text.Trim()
                    )
                {                    
                    planStrategy = new ImprovementPlanStrategy
                {
                        ID = ddlStrategy.SelectedValue == String.Empty || ddlStrategy.SelectedIndex == -1 ? default(int) : DataIntegrity.ConvertToInt(ddlStrategy.SelectedValue),
                        ImprovementPlanID = this.ImprovementID,
                        StrategyName = this.txtStrategy.Text.Trim(),
                        PersonResponsible = this.txtPersonResponsibles.Text.Trim()
                    };
                }


                #endregion Get Strategy

                #region Get Smart Goal Changes
                foreach (RepeaterItem item in this.rptSmartGoal.Items.AsParallel())
                {
                    TextBox smartGoalCtrl = (TextBox)item.FindControl("txtSmartGoal");

                    int smartGoalID = DataIntegrity.ConvertToInt(smartGoalCtrl.Attributes["SmartGoalID"].ToString());

                    if (smartGoalCtrl.Text.Trim() != smartGoalCtrl.Attributes["InitialValue"].ToString().Trim())
                        smartGoals.Add(new ImprovementPlanSmartGoal
                        {
                            SmartGoal = smartGoalCtrl.Text,
                            ID = smartGoalID,
                            StrategyID = DataIntegrity.ConvertToInt(this.ddlStrategy.SelectedValue)
                        });
                }
                #endregion Get Smart Goal Changes

                #region Get Action Step Changes
                foreach (DataGridItem item in this.dgActions.Items.AsParallel())
                {
                    HiddenField actionIDCtrl = ((HiddenField)item.FindControl("hidActionID"));
                    DropDownList statusCtrl = ((DropDownList)item.FindControl("ddlStatus"));
                    DropDownList strategyCtrl = ((DropDownList)item.FindControl("ddlStrategyGoal"));
                    TextBox actionStepCtrl = (TextBox)item.FindControl("txtActionSteps");
                    TextBox personResponsibleCtrl = (TextBox)item.FindControl("txtPersonResponsible");
                    RadDatePicker startDateCtrl = ((RadDatePicker)item.FindControl("rdpStartDate"));
                    RadDatePicker finishDateCtrl = ((RadDatePicker)item.FindControl("rdpFinishDate"));
                    TextBox resourceCostsCtrl = (TextBox)item.FindControl("txtResrouceCosts");
                    TextBox expectedResultsCtrl = (TextBox)item.FindControl("txtExpectedResults");


                    if ((statusCtrl.SelectedValue != statusCtrl.Attributes["InitialValue"].ToString()) ||
                        (strategyCtrl.SelectedValue != strategyCtrl.Attributes["InitialValue"].ToString()) ||
                        (actionStepCtrl.Text.Trim() != actionStepCtrl.Attributes["InitialValue"].ToString().Trim()) ||
                        (personResponsibleCtrl.Text.Trim() != personResponsibleCtrl.Attributes["InitialValue"].ToString().Trim()) ||
                        (startDateCtrl.SelectedDate != DataIntegrity.ConvertToNullableDate(startDateCtrl.Attributes["InitialValue"].ToString())) ||
                        (finishDateCtrl.SelectedDate != DataIntegrity.ConvertToNullableDate(finishDateCtrl.Attributes["InitialValue"].ToString())) ||
                        (resourceCostsCtrl.Text.Trim() != resourceCostsCtrl.Attributes["InitialValue"].ToString().Trim()) ||
                        (expectedResultsCtrl.Text.Trim() != expectedResultsCtrl.Attributes["InitialValue"].ToString().Trim()))
                        actionSteps.Add(CreateActionStepFromControl(actionIDCtrl, statusCtrl, strategyCtrl,
                                                                          actionStepCtrl, personResponsibleCtrl, startDateCtrl,
                                                                          finishDateCtrl, resourceCostsCtrl, expectedResultsCtrl));

                }
                #endregion Get Action Step Changes

                if (planStrategy != default(ImprovementPlanStrategy))
                {
                    actions.Add(ImprovementPlanActions.StrategyPlan);
                    improvementPlan.ImprovementPlanStrategies = planStrategy;
                }

                if (smartGoals.Count > 0)
                {
                    actions.Add(ImprovementPlanActions.SmartGoal);
                    improvementPlan.ImprovementPlanSmartGoals = smartGoals;
                }

                if (actionSteps.Count > 0)
                {
                    actions.Add(ImprovementPlanActions.ActionStep);
                    improvementPlan.ImprovementPlanActionSteps = actionSteps;
                }

                int strategyID = new ImprovementPlanProxy().SaveImprovementPlan(improvementPlan, actions, this.ClientID);

                #region Reload the details after save to maintain consistency

                currentStrategyValue = strategyID;

                if (actions.Contains(ImprovementPlanActions.StrategyPlan))
                    ReplaceStrategyDropDown(
                        new ListItem
                        {
                            Value = strategyID.ToString(),
                            Text = txtStrategy.Text.Trim(),
                            Selected = true
                        }, false);

                #endregion Reload the details after save to maintain consistency

                if (eventTargetEnum == EventTargets.btnSaveAndAdd)
                {
                   ReplaceStrategyDropDown(
                            new ListItem
                            {
                                Value = "",
                                Text = string.Empty,
                                Selected = true
                            }, true);

                }

            }
            catch (Exception exception)
            {

            }
        }

        /// <summary>
        /// Add new smart goal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddSmartGoal_Click(object sender, EventArgs e)
        {
            List<ImprovementPlanSmartGoal> smartGoals = new List<ImprovementPlanSmartGoal>();

            foreach (RepeaterItem item in this.rptSmartGoal.Items.AsParallel())
            {
                TextBox smartGoalCtrl = (TextBox)item.FindControl("txtSmartGoal");

                int smartGoalID = DataIntegrity.ConvertToInt(smartGoalCtrl.Attributes["SmartGoalID"].ToString());

                smartGoals.Add(new ImprovementPlanSmartGoal
                {
                    SmartGoal = smartGoalCtrl.Text,
                    ID = smartGoalID,
                    StrategyID = DataIntegrity.ConvertToInt(smartGoalCtrl.Attributes["StrategyID"].ToString()),
                    InitialSmartGoal = smartGoalCtrl.Attributes["InitialValue"].ToString()
                });
            }

            smartGoals.Add(new ImprovementPlanSmartGoal { StrategyID = DataIntegrity.ConvertToInt(this.ddlStrategy.SelectedValue) });

            this.rptSmartGoal.DataSource = smartGoals;
            this.rptSmartGoal.DataBind();
        }

        /// <summary>
        /// Add new action details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddAction_Click(object sender, EventArgs e)
        {
            List<ImprovementPlanActionStep> actionSteps = new List<ImprovementPlanActionStep>();

            foreach (DataGridItem item in this.dgActions.Items.AsParallel())
            {
                DropDownList statusCtrl = ((DropDownList)item.FindControl("ddlStatus"));
                DropDownList strategyCtrl = ((DropDownList)item.FindControl("ddlStrategyGoal"));

                actionSteps.Add(new ImprovementPlanActionStep
                {
                    ID = DataIntegrity.ConvertToInt(((HiddenField)item.FindControl("hidActionID")).Value),
                    StrategicGoalID = DataIntegrity.ConvertToInt(strategyCtrl.SelectedValue),
                    ActionStep = ((TextBox)item.FindControl("txtActionSteps")).Text,
                    PersonResponsible = ((TextBox)item.FindControl("txtPersonResponsible")).Text,
                    StartDate = ((RadDatePicker)item.FindControl("rdpStartDate")).SelectedDate,
                    FinishDate = ((RadDatePicker)item.FindControl("rdpFinishDate")).SelectedDate,
                    StatusID = DataIntegrity.ConvertToInt(statusCtrl.SelectedValue),
                    ResourceCosts = ((TextBox)item.FindControl("txtResrouceCosts")).Text,
                    ExpectedResults = ((TextBox)item.FindControl("txtExpectedResults")).Text
                });

                #region Set Strategy & Status Details
                if (this.ImprovementPlanOutput == null)
                {
                    this.ImprovementPlanOutput = new ImprovementPlanOutput();
                    List<ImprovementPlanStatusKey> statusKey = new List<ImprovementPlanStatusKey>();




                    this.ImprovementPlanOutput = new ImprovementPlanProxy().GetImprovementPlanByActions(
                                                                      DataIntegrity.ConvertToInt(strategyCtrl.SelectedItem.Value),
                                                                      this.ImprovementID,
                                                                      new List<ImprovementPlanActions> 
                                                                  {                                                                       
                                                                      ImprovementPlanActions.StrategicGoal                                                                      
                                                                  },
                                                                      this.ClientID);


                    if (this.ImprovementPlanOutput == null)
                        this.ImprovementPlanOutput = new ImprovementPlanOutput();

                    foreach (ListItem listItem in statusCtrl.Items.AsParallel())
                    {
                        if (!string.IsNullOrEmpty(listItem.Value))
                        {

                            statusKey.Add(new ImprovementPlanStatusKey
                    {
                                ID = DataIntegrity.ConvertToInt(listItem.Value),
                                StatusKey = listItem.Text
                            });
                        }
                    }


                    this.ImprovementPlanOutput.ImprovementPlanStatusKey = statusKey;
                }
                #endregion Set Strategy & Status Details
            }

            actionSteps.Add(new ImprovementPlanActionStep { StartDate = default(DateTime?), FinishDate = default(DateTime?) });



            this.dgActions.DataSource = actionSteps;
            this.dgActions.DataBind();
        }

        /// <summary>
        /// Remove the startegy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            new ImprovementPlanProxy().DeleteImprovementPlanStrategy(DataIntegrity.ConvertToInt(this.ddlStrategy.SelectedValue), this.ClientID);
            RemoveStrategyDropDown(this.ddlStrategy.SelectedItem);
            currentStrategyValue = 0;
            if (this.ddlStrategy.Items.Count > 0)
            this.ddlStrategy.SelectedIndex = 0;
            ddlStrategy_SelectedIndexChanged(this.ddlStrategy, new EventArgs());
        }

        #endregion Events

        protected void dgActions_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ImprovementPlanActionStep actionStep = e.Item.DataItem as ImprovementPlanActionStep;


                //Set Status
                #region Status Dropdown
                DropDownList statusCtrl = e.Item.FindControl("ddlStatus") as DropDownList;
                if (this.ActionType == ActionType.Edit)
                {
                    if (statusCtrl != null && this.ImprovementPlanOutput != null && this.ImprovementPlanOutput.ImprovementPlanStatusKey != null)
                    {
                        statusCtrl.Items.Clear();
                        ExtensionHelper.ForEach(this.ImprovementPlanOutput.ImprovementPlanStatusKey.AsParallel(), loop =>
                        {
                            statusCtrl.Items.Add(new ListItem { Text = loop.StatusKey, Value = loop.ID.ToString(), Selected = (loop.ID == actionStep.StatusID) });
                        });
                    }

                    statusCtrl.Items.Insert(0, new ListItem { Text = " ", Value = "", Selected = (actionStep.StatusID == default(int?)) });
                }
                else
                {
                    statusCtrl.Visible = false;
                    Label statusLabelCtrl = e.Item.FindControl("lblStatus") as Label;
                    statusLabelCtrl.Visible = true;
                    statusLabelCtrl.Text = actionStep.StatusDescription;
                }
                #endregion Status Dropdown

                //Set Strategic Goal
                #region Strategy Control
                DropDownList strategyCtrl = e.Item.FindControl("ddlStrategyGoal") as DropDownList;
                if (this.ActionType == Enums.ImprovementPlan.ActionType.Edit)
                {
                    if (strategyCtrl != null && this.ImprovementPlanOutput != null && this.ImprovementPlanOutput.ImprovementPlanStrategicGoal != null)
                    {
                        strategyCtrl.Items.Clear();
                        int strategyGoalCount = default(int);
                        ExtensionHelper.ForEach(this.ImprovementPlanOutput.ImprovementPlanStrategicGoal.AsParallel(), loop =>
                        {
                            strategyGoalCount++;
                            ListItem strategy = new ListItem();
                            strategy.Text = string.Format("Strategic Goal {0}", strategyGoalCount);
                            strategy.Value = loop.ID.ToString();
                            strategy.Selected = (loop.ID == actionStep.StrategicGoalID);
                            strategy.Attributes["title"] = loop.StrategicGoal;
                            strategyCtrl.Items.Add(strategy);
                        });
                    }

                    strategyCtrl.Items.Insert(0, new ListItem { Text = "Select Strategic Goal", Value = "", Selected = (actionStep.StrategicGoalID == default(int?)) });
                }
                else
                {
                    strategyCtrl.Visible = false;
                    Label strategyLabelCtrl = e.Item.FindControl("lblStrategyGoal") as Label;
                    strategyLabelCtrl.Visible = true;
                    strategyLabelCtrl.Text = actionStep.StrategyGoalName;
                }
                #endregion Strategy Control

                //Set Action Step Details
                #region Action Control
                if (actionStep != null)
                {
                    TextBox actionCtrl = ((TextBox)e.Item.FindControl("txtActionSteps"));
                    TextBox personCtrl = ((TextBox)e.Item.FindControl("txtPersonResponsible"));
                    TextBox resourceCtrl = ((TextBox)e.Item.FindControl("txtResrouceCosts"));
                    TextBox expectedCtrl = ((TextBox)e.Item.FindControl("txtExpectedResults"));
                    RadDatePicker startDateCtrl = ((RadDatePicker)e.Item.FindControl("rdpStartDate"));
                    RadDatePicker finishDateCtrl = ((RadDatePicker)e.Item.FindControl("rdpFinishDate"));

                    if (ActionType == Enums.ImprovementPlan.ActionType.Edit)
                    {

                        actionCtrl.Text = actionStep.ActionStep ?? default(string);
                        personCtrl.Text = actionStep.PersonResponsible ?? default(string);
                        resourceCtrl.Text = actionStep.ResourceCosts ?? default(string);
                        expectedCtrl.Text = actionStep.ExpectedResults ?? default(string);
                        startDateCtrl.SelectedDate = actionStep.StartDate ?? default(DateTime?);
                        finishDateCtrl.SelectedDate = actionStep.FinishDate ?? default(DateTime?);
                    }
                    else
                    {
                        personCtrl.Visible = resourceCtrl.Visible = expectedCtrl.Visible =
                        actionCtrl.Visible = expectedCtrl.Visible = resourceCtrl.Visible =
                        startDateCtrl.Visible = finishDateCtrl.Visible = false;

                        Label startDatelbl = ((Label)e.Item.FindControl("lblStartDate"));
                        Label finishDatelbl = ((Label)e.Item.FindControl("lblFinishDate"));
                        Label actionStepLbl = ((Label)e.Item.FindControl("lblActionSteps"));
                        Label personRespLbl = ((Label)e.Item.FindControl("lblPersonResponsible"));
                        Label resourceCostLbl = ((Label)e.Item.FindControl("lblResourceCost"));
                        Label expectedResultLbl = ((Label)e.Item.FindControl("lblExpectedResults"));



                        startDatelbl.Visible = finishDatelbl.Visible = actionStepLbl.Visible = personRespLbl.Visible = resourceCostLbl.Visible =
                        expectedResultLbl.Visible = true;

                        actionStepLbl.Text = actionStep.ActionStep ?? default(string);
                        personRespLbl.Text = actionStep.PersonResponsible ?? default(string);
                        resourceCostLbl.Text = actionStep.ResourceCosts ?? default(string);
                        expectedResultLbl.Text = actionStep.ExpectedResults ?? default(string);


                        startDatelbl.Text = actionStep.StartDate != default(DateTime?) ? DataIntegrity.ConvertToDate(actionStep.StartDate).ToShortDateString() : default(string);
                        finishDatelbl.Text = actionStep.FinishDate != default(DateTime?) ? DataIntegrity.ConvertToDate(actionStep.FinishDate).ToShortDateString() : default(string);
                    }
                }
                #endregion Action Control



    }
}
    }
}