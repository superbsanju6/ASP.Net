using System;
using System.Web.UI;
using System.IO;
using System.Web.Configuration;
using System.Reflection;
using Microsoft.Reporting.WebForms;

using Thinkgate.Classes;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.Reports
{
    public partial class BalancedScorecardPage : BasePage
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCriteria();
                SetReportConfiguration();
                GetReportData();
            }
        }

        protected void cmbGridDisplay_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            switch (cmbGoal.SelectedValue)
            {
                case "1":
                    reportPath.Value = GetReportPath("BalancedScorecardGoal1Report");
                    LoggedUserBalancedScoreCardReport("BalancedScorecardGoal1Report");
                    break;
                case "2":
                    reportPath.Value = GetReportPath("BalancedScorecardGoal2Report");
                    LoggedUserBalancedScoreCardReport("BalancedScorecardGoal2Report");
                    break;
                case "3":
                    reportPath.Value = GetReportPath("BalancedScorecardGoal3Report");
                    LoggedUserBalancedScoreCardReport("BalancedScorecardGoal3Report");
                    break;
                case "4":
                    reportPath.Value = GetReportPath("BalancedScorecardGoal4Report");
                    LoggedUserBalancedScoreCardReport("BalancedScorecardGoal4Report");
                    break;
                case "5":
                    reportPath.Value = GetReportPath("BalancedScorecardGoal5Report");
                    LoggedUserBalancedScoreCardReport("BalancedScorecardGoal5Report");
                    break;
            }

            if (reportPath.Value.Length != 0)
            { rptViewer.ServerReport.ReportPath = reportPath.Value; }
        }

        protected void btnExportExcel_Click(object sender, ImageClickEventArgs e)
        {
            ExportToExcel();
            rptViewer.ServerReport.ReportPath = reportPath.Value;
        }

        #endregion

        #region Private Methods

        private void LoadCriteria()
        {
            cmbGoal.DataSource = Thinkgate.Base.Classes.Reports.GetBSReportCriteriaData(); 
            cmbGoal.DataTextField = "Name";
            cmbGoal.DataValueField = "ID";
            cmbGoal.DataBind();
        }

        private string GetReportPath(string reportName)
        {
            var sSRSReportPaths = Thinkgate.Base.Classes.Reports.GetSSRSReportPaths(reportName);
            
            if (sSRSReportPaths.Rows.Count == 0)
            { return string.Empty; }

            return sSRSReportPaths.Rows[0]["Path"].ToString();
        }

        private void GetReportData()
        {
            if (reportServerUrl.Value.Length != 0 && reportPath.Value.Length != 0)
            {
                rptViewer.ProcessingMode = ProcessingMode.Remote;
                rptViewer.ServerReport.ReportServerUrl = new Uri(reportServerUrl.Value);
                rptViewer.ServerReport.ReportPath = reportPath.Value;
                rptViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials(reportUsername.Value, reportPassword.Value, reportDomain.Value);
                rptViewer.ShowParameterPrompts = false;
                rptViewer.ShowRefreshButton = false;
                rptViewer.ShowBackButton = false;
                rptViewer.ShowExportControls = false;

                string masterReportPath = GetReportPath("BalancedScorecardMasterReport");
                if (masterReportPath.Length != 0)
                {
                    rptViewerMasterReport.ProcessingMode = rptViewer.ProcessingMode;
                    rptViewerMasterReport.ServerReport.ReportServerUrl = rptViewer.ServerReport.ReportServerUrl;
                    rptViewerMasterReport.ServerReport.ReportPath = masterReportPath;
                    rptViewerMasterReport.ServerReport.ReportServerCredentials = rptViewer.ServerReport.ReportServerCredentials;
                }
            }
        }

        private void SetReportConfiguration()
        {
            var sSRSServerInfo = Thinkgate.Base.Classes.Reports.GetSSRSServerInfo(SessionObject.GlobalInputs);
            var sSRSReportPaths = Thinkgate.Base.Classes.Reports.GetSSRSReportPaths("BalancedScorecardGoal1Report");

            if (sSRSServerInfo.Rows.Count == 0 || sSRSReportPaths.Rows.Count == 0)
            { return; }

            reportServerUrl.Value = sSRSServerInfo.Rows[0]["URL"].ToString();
            reportPath.Value = sSRSReportPaths.Rows[0]["Path"].ToString();
            reportUsername.Value = sSRSServerInfo.Rows[0]["Username"].ToString();
            reportPassword.Value = sSRSServerInfo.Rows[0]["Password"].ToString();
            reportDomain.Value = sSRSServerInfo.Rows[0]["Domain"].ToString();

            LoggedUserBalancedScoreCardReport("BalancedScorecardGoal1Report");
        }

        private void ExportToExcel()
        {
            string reportPath = GetReportPath("BalancedScorecardMasterReport");

            if (reportPath.Length != 0)
            {
                rptViewer.ServerReport.ReportPath = reportPath;

                string mimeType;
                string encoding;
                string extension;
                string[] streams;
                Warning[] warnings;

                byte[] xlsBytes = rptViewer.ServerReport.Render("EXCEL", string.Empty, out mimeType, out encoding, out extension, out streams, out warnings);

                using (MemoryStream memoryStream = new MemoryStream(xlsBytes))
                {
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename=BalancedScorecardMasterReport.xls");
                    Response.BinaryWrite(memoryStream.ToArray());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private void LoggedUserBalancedScoreCardReport(string reportName)
        {
            SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + reportName + "' report", reportName, sessionObject.LoggedInUser.UserName);
        }

        #endregion
    }
}