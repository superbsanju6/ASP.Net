using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Activation;
using System.Web;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System;
using System.Reflection;

using Thinkgate.ExceptionHandling;

namespace Thinkgate.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReportsWCF : Interfaces.IReportsWCF
    {
        public List<object> GetReportListByLevel(ReportsWCFVariables reportVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var reports = Base.Classes.Reports.GetReportList(reportVars.testCategory);
            var reportList = new List<object>();

            var selectFilter = "LEVEL = '" + reportVars.Level + "' and LinkType = '" + reportVars.ReportType + "'";
            var sessionObject = (Thinkgate.Classes.SessionObject) System.Web.HttpContext.Current.Session["SessionObject"];

            /* all flavors of Proficiency report have SourceID = 139. It also needs to be regulated *
             * by permission Report_ProficiencyReport.  The following does just that.         */
            var perm_Report_ProficiencyReport = sessionObject.LoggedInUser.HasPermission(Permission.Report_ProficiencyReport);
            if (!perm_Report_ProficiencyReport) selectFilter += " and not SourceID = 139 ";

            if(reports.Tables.Count == 2 && reports.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in reports.Tables[1].Select(selectFilter))
                {
                    var reportRow = reports.Tables[0].Select("ID = " + row["SourceID"]);
                    if(reportRow.Length > 0)
                    {
                        reportList.Add(new object[] { row["Title"].ToString(), reportRow[0]["ReportType"].ToString(), 
                            reportRow[0]["Criteria"].ToString(), reportRow[0]["ID"].ToString(), row["Title"].ToString() });
                    }
                }
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return reportList;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request ReportsWCF", "ReportsWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end ReportsWCF", "ReportsWCF"); }
        }
    }
}
