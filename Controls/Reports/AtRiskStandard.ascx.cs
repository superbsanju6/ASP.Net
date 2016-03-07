using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Reflection;

using Thinkgate.Classes;
using Thinkgate.ExceptionHandling;

using Telerik.Web.UI;


namespace Thinkgate.Controls.Reports
{
    public partial class AtRiskStandard : TileControlBase
    {
        public DataTable TreeData;
        public string Guid;
        public string multiTestIDs;
        string _permissions = "22222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222220000000022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222022222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222000000222222222222222222222222222222222222222222222222222222222222222222222222222222220002222222222222";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["selectedReport"] != null && !String.IsNullOrEmpty(Request.QueryString["selectedReport"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["selectedReport"].ToString() + "' report", Request.QueryString["selectedReport"].ToString(), sessionObject.LoggedInUser.UserName);
            }
            LoadCriteria();
            atRiskTree.DataSource = GetDataTable();
            atRiskTree.ExpandedIndexes.Add(new TreeListHierarchyIndex() {LevelIndex = 4,NestedLevel = 0});
            atRiskTree.DataBind();
            atRiskTree.ExpandToLevel(4);

            if (Parent.Page.Request.Params["__EVENTTARGET"] != null && !Parent.Page.Request.Params["__EVENTTARGET"].Contains("ExpandCollapseButton"))
            {
                
            }
            
        }
        protected void RemoteReportReload(object sender, EventArgs e)
        {
            atRiskTree.DataSource = null;

            atRiskTree.DataSource = GetDataTable();
            atRiskTree.DataBind();
            atRiskTree.ExpandToLevel(4);
        }
        private void LoadCriteria()
        {
            var ctlReportCriteria = (Thinkgate.Controls.Reports.ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
            ctlReportCriteria.ID = "ctlAtRiskStandardCriteria";

            if (Tile.TileParms.GetParm("CriteriaGUID") == null) return;
            Guid = (string)Tile.TileParms.GetParm("CriteriaGUID");
            multiTestIDs = (string)Tile.TileParms.GetParm("multiTestIDs");

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
                var parentKey = ((DataRowView)((TreeListDataItem)e.Item).DataItem)["ParentConcatKey"].ToString();
                var lblScore = ((Label)((TreeListDataItem)e.Item).FindControl("lblScore"));
                var lblKeyDisp = ((Label)((TreeListDataItem)e.Item).FindControl("lblKeyDisp"));

                if (level == "Standard")
                {
                    string standardID = key.Replace(parentKey, "");
                    lblKeyDisp.Attributes.Add("onclick", "window.open('StandardsPage.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(standardID) + "');");
                    lblKeyDisp.Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");

                    //Get count of students
                    int numStudents = (from DataRow sRow in TreeData.Rows
                                       select new
                                       {
                                           parentconcatkey = sRow["parentconcatkey"].ToString()
                                       }).Count(t => t.parentconcatkey == key);

                    
                    lblScore.Text += " : " + numStudents + " Students " + ((numStudents == 1) ? "is" : "are") + " of concern";
                    lblScore.ForeColor = System.Drawing.Color.FromName("Black");
                    lblScore.Font.Italic = true;
                }
                else if (level == "StudentName")
                {
                    string studentID = key.Replace(parentKey, "").Replace("Stud","");
                    lblKeyDisp.Attributes.Add("onclick", "window.open('Student.aspx?childPage=yes&xID=" + Standpoint.Core.Classes.Encryption.EncryptString(studentID) + "');");
                    lblKeyDisp.Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");
                }
                else if (level == "TestGrade" || level == "TestCourse")
                {
                    var lbl = ((Label)((TreeListDataItem)e.Item).FindControl("lblScore"));
                    lbl.Visible = false;
                }
            }
        }

        private DataTable GetDataTable()
        {
            var dev = System.Configuration.ConfigurationManager.AppSettings.Get("Environment") == "Dev" ? true : false;

            var year = Thinkgate.Base.Classes.AppSettings.Demo_Year;
            var userPage = dev ? 119 : 60528;
            var permissions = _permissions;
            var outFormat = "normal";

            string criteriaOverrides = "";
            
            var reportCriteria = (Criteria) Session["Criteria_" + Guid];
            var multiTestParm = "1test=yes@@@@PT=1@@";

            if (!String.IsNullOrEmpty(multiTestIDs))
                multiTestParm = String.Concat("@@RR=none@@ZZ=", multiTestIDs, "@@TLISTMASTER=", multiTestIDs, "@@@@PT=1@@");

            if (reportCriteria == null) return null;

            criteriaOverrides = ((Criteria)reportCriteria).GetCriteriaOverrideString();

            var ds = Thinkgate.Base.Classes.Reports.GetOutlineReport("6", year, "", "", "", "", "", "", 0, userPage,
                                                                         _permissions,
                                                                         outFormat, "", "", "", "", "", "",
                                                                         "@@Product=none@@@@RR=none" + criteriaOverrides +
                                                                         multiTestParm, "");


                if (ds.Tables.Count == 0) return null;

                TreeData = ds.Tables[0];

            return TreeData;
        }
       
    }
}