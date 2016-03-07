using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.Reports;

namespace Thinkgate.Record
{
    public partial class TestCriteria : System.Web.UI.Page
    {
        public SearchParms SearchParms;
        public string Guid;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hiddenTxtBox.Text)) Guid = hiddenTxtBox.Text;

            if (Session["Criteria_" + Guid] == null) LoadSearchCriteria();
            AddCriteriaControl();
        }

        private void AddCriteriaControl()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlItemAnalysisCriteria";

            if (Session["Criteria_" + Guid] == null) return;

            ctlReportCriteria.Guid = Guid;
            ctlReportCriteria.ReloadReport += RemoteReportReload;
            phTestCriteria.Controls.Add(ctlReportCriteria);
        }

        protected void RemoteReportReload(object sender, EventArgs e)
        {
            

        }


        private void LoadSearchCriteria()
        {


            var searchCriteria = new Criteria();

            var demographicParms = new SearchParms();
            demographicParms.AddParm("", "");


            searchCriteria.Add(new Classes.Criterion()
            {
                Header = "Demographics",
                Key = "Demographics",
                Locked = true,
                Empty = true
                
            });

            searchCriteria.Add(new Classes.Criterion()
            {
                Header = "Demographics",
                Key = "Race",
                Removable = true

            });

            searchCriteria.Add(new Classes.Criterion()
            {
                Header = "Demographics",
                Key = "EOSL",
                Object = "Yes",
                Removable = true
            });

            searchCriteria.Add(new Classes.Criterion()
            {
                Header = "Demographics",
                Key = "Gifted",
                Object = "No",
                Removable = true
            });

            searchCriteria.Add(new Classes.Criterion()
            {
                Header = "Demographics",
                Key = "Special needs",
                Object = "Yes",
                Removable = true
            });

            SearchParms = new SearchParms();
            SearchParms.AddParm("reportCriteria", searchCriteria);

            var guid = System.Guid.NewGuid().ToString();
            hiddenTxtBox.Text = guid;
            Guid = guid;

            Session["Criteria_" + guid] = searchCriteria;
        }
    }
}