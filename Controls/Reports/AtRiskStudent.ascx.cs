using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Thinkgate.Classes;
using Thinkgate.ExceptionHandling;

using Telerik.Web.UI;


namespace Thinkgate.Controls.Reports
{
    public partial class AtRiskStudent : TileControlBase
    {
        public string Guid;
        public string multiTestIDs;
        public DataTable TreeData;

        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            if (Request.QueryString["selectedReport"] != null && !String.IsNullOrEmpty(Request.QueryString["selectedReport"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["selectedReport"].ToString() + "' report", Request.QueryString["selectedReport"].ToString(), sessionObject.LoggedInUser.UserName);
            }
            LoadCriteria();
            atRiskTree.DataSource = GetDataTable();
            atRiskTree.DataBind();
            //atRiskTree.ExpandToLevel(2);
        }
        protected void RemoteReportReload(object sender, EventArgs e)
        {
            atRiskTree.DataSource = null;

            atRiskTree.DataSource = GetDataTable();
            atRiskTree.DataBind();
            //atRiskTree.ExpandToLevel(4);
        }
        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlAtRiskStandardCriteria";

            if (Tile.TileParms.GetParm("CriteriaGUID") == null) return;
            Guid = (string)Tile.TileParms.GetParm("CriteriaGUID");
            multiTestIDs = (string) Tile.TileParms.GetParm("multiTestIDs");

            if (Session["Criteria_" + Guid] == null) return;

            ctlReportCriteria.Guid = Guid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            criteriaPlaceholder.Controls.Add(ctlReportCriteria);
        }
        
        protected void atRiskTree_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {
            if (e.Item is TreeListDataItem)
            {
                var level = ((DataRowView)((TreeListDataItem)e.Item).DataItem)["level"].ToString();
                var key = ((DataRowView)((TreeListDataItem)e.Item).DataItem)["Key"].ToString();
                
                if (level == "StudentName")
                {
                    //Get count of standards
                    int numStandards = (from DataRow sRow in TreeData.Rows
                                     select new
                                     {
                                         parentconcatkey = sRow["parentconcatkey"].ToString()
                                     }).Count(t => string.Compare(t.parentconcatkey, key) == 0);

                    var lbl = ((Label)((TreeListDataItem)e.Item).FindControl("lblScore"));
                    lbl.Text = numStandards + " Standard" + ((numStandards == 1) ? " is" : "s are") + " of concern";
                    lbl.ForeColor = System.Drawing.Color.FromName("Black");
                    lbl.Font.Italic = true;
                }
            }
        }
          
        private DataTable GetDataTable()
        {
            var dev = System.Configuration.ConfigurationManager.AppSettings.Get("Environment") == "Dev" ? true : false;
            var userPage = dev ? 119 : 60528;
            var reportID = dev ? 92 : 35;
            var outFormat = "normal";
            var year = Thinkgate.Base.Classes.AppSettings.Demo_Year;


            var reportCriteria = (Criteria)Session["Criteria_" + Guid];
            var multiTestParm = "1test=yes@@@@PT=1@@";

            if (!String.IsNullOrEmpty(multiTestIDs))
                multiTestParm = String.Concat("@@RR=none@@ZZ=", multiTestIDs, "@@TLISTMASTER=", multiTestIDs, "@@@@PT=1@@");

                if (reportCriteria == null) return null;

                var criteriaOverrides = ((Criteria)reportCriteria).GetCriteriaOverrideString();

                var ds =  Thinkgate.Base.Classes.Reports.GetOutlineReport(reportID.ToString(), year, "", "", "", "",
                                                                               "", "", 0,
                                                                               userPage, _permissions,
                                                                               outFormat, "", "", "", "", "", "",
                                                                               "@@Product=none@@@@RR=none" +
                                                                               criteriaOverrides +
                                                                               multiTestParm,
                                                                               "")
                    ;
                if (ds.Tables.Count == 0) return null;

                TreeData = ds.Tables[0];
            
            return TreeData;
        }
    }
}