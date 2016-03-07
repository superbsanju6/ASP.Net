using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;
using System.Drawing;
using System.Web.UI;
using System.Reflection;

using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.ExceptionHandling;

using Telerik.Web.UI;
namespace Thinkgate.Controls.Reports
{
    public partial class PerformanceSummary : TileControlBase
    {      
        public string Guid;
        public int FormID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            if (Request.QueryString["folder"] != null && !String.IsNullOrEmpty(Request.QueryString["folder"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["folder"].ToString() + "' report", Request.QueryString["folder"].ToString(), sessionObject.LoggedInUser.UserName);
            }
            LoadCriteria();
            LoadReport();
        }

        protected void RemoteReportReload(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlPerformanceSummaryCriteria";

            if (Tile.TileParms.GetParm("CriteriaGUID") == null) return;
            Guid = (string)Tile.TileParms.GetParm("CriteriaGUID");

            if (Session["Criteria_" + Guid] == null) return;

            ctlReportCriteria.Guid = Guid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            criteriaPlaceholder.Controls.Add(ctlReportCriteria);
        }

        private DataSet GetDataTable()
        {
            var dev = System.Configuration.ConfigurationManager.AppSettings.Get("Environment") == "Dev" ? true : false;

            //TODO: Get userPage from actual user.
            var userPage = dev ? 119 : 60528;

            var year = Thinkgate.Base.Classes.AppSettings.Demo_Year;

            if (string.IsNullOrEmpty(Guid))
            {
                SessionObject.RedirectMessage =
                    "Report criteria could not be found in session. No GUID.";
                Response.Redirect("~/PortalSelection.aspx");
                return null;

            }

            var reportCriteria = (Criteria)Session["Criteria_" + Guid];
            var selectedTest = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(reportCriteria.GetAssessmentCriterion().ReportStringVal);
            var level = Request.QueryString["level"] ?? "Class";
            var selectedLevelCriterion = reportCriteria.GetLevelCriterion(level);
            var selectedLevelID = DataIntegrity.ConvertToInt(selectedLevelCriterion != null ? selectedLevelCriterion.ReportStringVal : Request.QueryString["levelID"]);
            var selectedClass = (level == "Class") ? selectedLevelID.ToString() : "";
            var criteriaOverrides = (reportCriteria).GetCriteriaOverrideString();
            var itemAnalysisData = new DataSet();
            return itemAnalysisData;
        }

        private void LoadReport()
        {
           
          
        }       
    }
}