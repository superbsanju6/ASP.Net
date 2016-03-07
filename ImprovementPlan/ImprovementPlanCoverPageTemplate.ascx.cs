using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Controls.Banner;
using Thinkgate.Enums.ImprovementPlan;
using Thinkgate.Services.Contracts.ImprovementPlanService;

namespace Thinkgate.ImprovementPlan
{
    public partial class ImprovementPlanCoverPageTemplate : System.Web.UI.UserControl
    {
        #region Properties

        public bool IsPDF { get; set; }
        private int ImprovementID { get; set; }
        private ActionType ActionType { get; set; }
        private ImprovementPlanType ImpType { get; set; }
        private ImprovementPlanCoverPage CoverPageDetails { get; set; }
        private string ClientID { get; set; }
        private string ImprovementPlanDistrictDisplay { get; set; }
        private string Year { get; set; }
        private int? SchoolID { get; set; }
        private int currentStrategyIndex = default(int);
        private SessionObject sessionObject;

        #endregion Properties

        protected void Page_Load(object sender, EventArgs e)
        {
            sessionObject = (SessionObject) Session["SessionObject"];
            GetClientID();
            if (!IsPostBack)
            {
                if (Request.QueryString["year"] != null)
                    AddNewImprovementPlan();
                else
                {
                    GetImpPlanIDfromQueryString();
                }
                BindImprovementPlanDetails();
            }
            else
            {
                if (!string.IsNullOrEmpty(hiddenImprovementPlanID.Value))
                {
                    ImprovementID = Convert.ToInt32(hiddenImprovementPlanID.Value);
                    ActionType = !string.IsNullOrEmpty(hiddenImpPlanAction.Value)
                        ? (ActionType) Enum.Parse(typeof (ActionType), hiddenImpPlanAction.Value)
                        : ActionType.Edit;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "Alert",
                        "alert('Error during postback. Improvement Plan not found ')", true);
                }

            }
            if (IsPDF) imgLogo.Visible = false;
            SetUpReadOnlyMode();

        }

        #region Private Methods

        /// <summary>
        /// Get the client id from the district params
        /// </summary>
        private void GetClientID()
        {
            DistrictParms districtParams = DistrictParms.LoadDistrictParms();
            if (districtParams != default(DistrictParms))
            {
                this.ClientID = districtParams.ClientID;
                this.ImprovementPlanDistrictDisplay = districtParams.ImprovementPlanDistrictDisplayName;
            }
        }


        private void AddNewImprovementPlan()
        {
            GetQueryStringDataforNewIP();
            ImprovementID = SaveNewImprovementPlan();
            hiddenImprovementPlanID.Value = ImprovementID.ToString();
            //TriggerParentTileRefresh();

        }

        /// <summary>
        /// Get the improvement plan id and action type from query string.
        /// If those are not supplied the page will redirect with an error message.
        /// </summary>
        private void GetImpPlanIDfromQueryString()
        {
            if (Request.QueryString["impID"] != null)
                //ImprovementID = Cryptography.DecryptionToInt(Request.QueryString["impID"],sessionObject.LoggedInUser.CipherKey);
                ImprovementID = Convert.ToInt32(Request.QueryString["impID"]);

            if (Request.QueryString["actType"] != null)
                //ActionType = (ActionType)Enum.Parse(typeof(ActionType), Cryptography.DecryptionToString(Request.QueryString["actType"],sessionObject.LoggedInUser.CipherKey));
                ActionType = (ActionType) Enum.Parse(typeof (ActionType), Request.QueryString["actType"]);

            hiddenImprovementPlanID.Value = ImprovementID.ToString();
            hiddenImpPlanAction.Value = ActionType.ToString();
        }

        private void GetQueryStringDataforNewIP()
        {

            if (Request.QueryString["schoolID"] != null)
            {
                //string schoolDetail = Cryptography.DecryptionToString(Request.QueryString["schoolID"], sessionObject.LoggedInUser.CipherKey);
                string schoolDetail = Request.QueryString["schoolID"];

                if (!string.IsNullOrEmpty(schoolDetail))
                    SchoolID = Convert.ToInt32(schoolDetail);

            }

            if (Request.QueryString["year"] != null)
                //Year = Cryptography.DecryptionToString(Request.QueryString["year"], sessionObject.LoggedInUser.CipherKey);
                Year = Request.QueryString["year"];

            if (Request.QueryString["plantype"] != null)
                //ImpType = (ImprovementPlanType)Enum.Parse(typeof(ImprovementPlanType), Cryptography.DecryptionToString(Request.QueryString["planType"], sessionObject.LoggedInUser.CipherKey));
                ImpType =
                    (ImprovementPlanType) Enum.Parse(typeof (ImprovementPlanType), Request.QueryString["planType"]);

            if (Request.QueryString["actType"] != null)
                //ActionType = (ActionType)Enum.Parse(typeof(ActionType), Cryptography.DecryptionToString(Request.QueryString["actType"], sessionObject.LoggedInUser.CipherKey));
                ActionType = (ActionType) Enum.Parse(typeof (ActionType), Request.QueryString["actType"]);
        }

        /// <summary>
        /// Get the improvement data from database
        /// </summary>
        private int SaveNewImprovementPlan()
        {
            Thinkgate.Services.Contracts.ImprovementPlanService.ImprovementPlan newImprovementPlan =
                new Thinkgate.Services.Contracts.ImprovementPlanService.ImprovementPlan();

            List<ImprovementPlanActions> actions = new List<ImprovementPlanActions>();
            actions.Add(ImprovementPlanActions.ImprovementPlan);
            newImprovementPlan.Year = Year;
            newImprovementPlan.ImprovementPlanType = ImpType;
            newImprovementPlan.SchoolID = SchoolID;
            return new ImprovementPlanProxy().SaveImprovementPlan(newImprovementPlan, actions, ClientID);

        }


        private void SetUpReadOnlyMode()
        {
            if (ActionType == ActionType.View)
            {
                txtSchoolYear.ReadOnly = txtSchoolName.ReadOnly = txtDistrictName.ReadOnly =
                    txtSuperintendent.ReadOnly = txtPrincipal.ReadOnly = true;


                txtSchoolYear.CssClass = txtSchoolName.CssClass = txtDistrictName.CssClass
                    = txtSuperintendent.CssClass = txtPrincipal.CssClass = "viewModeCss inputtextbox";

                chkFinalized.Enabled = false;

                btnAddStrategicGoal.Visible = false;
                btnCoverPageSave.Visible = false;
                btnCoverPageSaveContinue.Visible = false;


                rdpCreateDateCoverPage.DateInput.ReadOnly = true;
                rdpCreateDateCoverPage.CssClass = "viewModeCss";

                rdpCreateDateCoverPage.DatePopupButton.Visible = false;
                rdpCreateDateCoverPage.DateInput.EmptyMessage = string.Empty;
            }

        }

        private void BindImprovementPlanDetails()
        {
            CoverPageDetails = new ImprovementPlanProxy().GetImprovementPlanCoverPage(ImprovementID, ClientID);
            if (CoverPageDetails == null) return;
            ImpType = (ImprovementPlanType) Convert.ToInt32(CoverPageDetails.ImprovementPlanType);
            lblPageTitle.Text = "ANNUAL " + ImpType.ToString().ToUpper() + " ACADEMIC IMPROVEMENT PLAN";
            txtDistrictName.Text = ImprovementPlanDistrictDisplay;
            if (ImpType == ImprovementPlanType.District)
            {
                lblSchoolNameLabel.Visible = false;
                txtSchoolName.Visible = false;
                txtPrincipal.Visible = false;
                lblPrincipalNameLabel.Visible = false;
                lblDisplayForSIP.Visible = false;
            }
            else
            {
                txtSchoolName.Text = CoverPageDetails.SchoolName;
                txtPrincipal.Text = CoverPageDetails.Principal;
            }


            rdpCreateDateCoverPage.SelectedDate = CoverPageDetails.SignedDate;
            txtSchoolYear.Text = CoverPageDetails.ImprovementPlanYear;
            txtSuperintendent.Text = CoverPageDetails.Superintendent;
            chkFinalized.Checked = CoverPageDetails.IsFinalized;

            rptStrategicGoal.DataSource = CoverPageDetails.ImprovementPlanStrategicGoals ??
                                          new List<ImprovementPlanStrategicGoal> {new ImprovementPlanStrategicGoal {}};
            rptStrategicGoal.DataBind();
        }

        #endregion Private Methods

        #region Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Thinkgate.Services.Contracts.ImprovementPlanService.ImprovementPlan improvementPlan =
                new Thinkgate.Services.Contracts.ImprovementPlanService.ImprovementPlan();
            List<ImprovementPlanActions> actions = new List<ImprovementPlanActions>();

            List<ImprovementPlanStrategicGoal> strategicGoals = new List<ImprovementPlanStrategicGoal>();

            #region Get Strategic Goal Changes

            foreach (RepeaterItem item in this.rptStrategicGoal.Items.AsParallel())
            {
                TextBox strategicGoalCtrl = (TextBox) item.FindControl("txtStrategicGoal");

                strategicGoals.Add(new ImprovementPlanStrategicGoal
                {
                    StrategicGoal = strategicGoalCtrl.Text,
                    ID = DataIntegrity.ConvertToInt(strategicGoalCtrl.Attributes["StrategicGoalID"]),
                    ImprovementPlanID = ImprovementID
                });
            }

            #endregion Get Strategic Goal Changes


            if (strategicGoals.Count > 0)
            {
                actions.Add(ImprovementPlanActions.StrategicGoal);
                improvementPlan.ImprovementPlanStrategicGoals = strategicGoals;
            }

            actions.Add(ImprovementPlanActions.ImprovementPlan);
            improvementPlan.ID = ImprovementID;
            improvementPlan.Superintendent = txtSuperintendent.Text;
            improvementPlan.Principal = txtPrincipal.Text;
            improvementPlan.SignedDate = rdpCreateDateCoverPage.SelectedDate;
            improvementPlan.IsFinalized = chkFinalized.Checked;



            ImprovementID = new ImprovementPlanProxy().SaveImprovementPlan(improvementPlan, actions, ClientID);
            Button buttonClicked = (Button) sender;
            if (buttonClicked.Text.ToLower().Contains("continue"))
            {
                //Response.Redirect("../ImprovementPlan/ImprovementPlanStrategyPage.aspx?impID=" +
                //                  Cryptography.EncryptInt(ImprovementID, sessionObject.LoggedInUser.CipherKey) +
                //                  "&actType=" +
                //                  Cryptography.EncryptString(ActionType.Edit.ToString(),
                //                      sessionObject.LoggedInUser.CipherKey));

                Response.Redirect("../ImprovementPlan/ImprovementPlanStrategyPage.aspx?impID=" + ImprovementID +
                                  "&actType=" + ActionType.Edit);

            }
            else
            {
                BindImprovementPlanDetails();

            }

        }

        protected void btnAddStrategicGoal_Click(object sender, EventArgs e)
        {
            List<ImprovementPlanStrategicGoal> strategicGoals = new List<ImprovementPlanStrategicGoal>();

            foreach (RepeaterItem item in rptStrategicGoal.Items.AsParallel())
            {
                TextBox strategicGoalCtrl = (TextBox) item.FindControl("txtStrategicGoal");

                strategicGoals.Add(new ImprovementPlanStrategicGoal
                {
                    StrategicGoal = strategicGoalCtrl.Text,
                    ID = DataIntegrity.ConvertToInt(strategicGoalCtrl.Attributes["StrategicGoalID"]),
                    ImprovementPlanID = DataIntegrity.ConvertToInt(strategicGoalCtrl.Attributes["ImprovementPlanID"])
                });


            }

            strategicGoals.Add(new ImprovementPlanStrategicGoal
            {
                ImprovementPlanID = DataIntegrity.ConvertToInt(this.ImprovementID)
            });

            rptStrategicGoal.DataSource = strategicGoals;
            rptStrategicGoal.DataBind();
        }


        #endregion Events

        protected void rptStrategicGoal_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            TextBox itemStrageticGoal = ((TextBox) e.Item.FindControl("txtStrategicGoal"));
            Label lblStrageticGoalReadOnly = ((Label)e.Item.FindControl("lblStrategicGoalReadOnly"));
            
            itemStrageticGoal.Attributes.Add("ItemChanged", "true");
            if (ActionType == ActionType.View)
            {
                //itemStrageticGoal.ReadOnly = true;
                //itemStrageticGoal.CssClass = "viewModeCss width-90per text-area-heigth";
                itemStrageticGoal.Visible = false;
                lblStrageticGoalReadOnly.Visible = true;
                lblStrageticGoalReadOnly.Text = itemStrageticGoal.Text;

            }

        }

    }

}