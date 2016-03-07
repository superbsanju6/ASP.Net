using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Reports
{
    public partial class CompetencyTrackingReportStudentListPDF : System.Web.UI.Page
    {
        private int techerID;
        private int standardID;
        private int workSheetID;
        private int roblicSortOrder;
        private int viewBySelectedValue;
        private int demographicID;
        private int groupID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStudentDetails();
            }
        }
        protected void BindStudentDetails()
        {
            Base.Classes.CompetencyTracking.CompetencyTrackingReport oCTR = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();

            QueryStringData();

            //TODO  PASS QUERYSTRING VALUES HERE and remove hard coded values
            /*When something in not selected pass 0 it will be converted to null in procedure and will not be included in where clause*/
            try
            {
                DataSet dsStudentList = oCTR.GetCVTEReportStudentList(workSheetID, standardID, roblicSortOrder, viewBySelectedValue, techerID, demographicID, groupID);
                if (dsStudentList != null)
                {
                    DataTable dtViewByDetail = dsStudentList.Tables[0];
                    if (dtViewByDetail.Rows.Count > 0)
                    {
                        lblViewBy.Text = dtViewByDetail.Rows[0]["ViewBy"].ToString() + "   : ";// +dtViewByDetail.Rows[0]["ViewByValue"].ToString();
                        tdViewByText.InnerText = dtViewByDetail.Rows[0]["ViewByValue"].ToString();
                        lblRubric.Text = "Rubric Value: ";
                        spnRubricValue.InnerText = dtViewByDetail.Rows[0]["RubricValue"].ToString();
                        lblCompetency.Text = "Competency:  ";
                        aCompetencyValue.InnerText = dtViewByDetail.Rows[0]["Competency"].ToString();
                        spnCompetencyDetail.InnerText = dtViewByDetail.Rows[0]["CompetencyDesc"].ToString();
                        lblStudentCount.Text = "Students:" + dtViewByDetail.Rows[0]["StudentCount"].ToString();
                    }

                    DataTable dtStudentList = dsStudentList.Tables[1];
                    radGridStudentCount.DataSource = dtStudentList;
                    radGridStudentCount.DataBind();

                }

            }
            catch (Exception ex)
            {


            }
        }

        private void QueryStringData()
        {

            if (!string.IsNullOrEmpty(Request.QueryString["techerID"]))
            {
                techerID = Convert.ToInt32(Request.QueryString["techerID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["standardID"]))
            {
                standardID = Convert.ToInt32(Request.QueryString["standardID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["workSheetID"]))
            {
                workSheetID = Convert.ToInt32(Request.QueryString["workSheetID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["roblicSortOrder"]))
            {
                roblicSortOrder = Convert.ToInt32(Request.QueryString["roblicSortOrder"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["viewBySelectedValue"]))
            {
                viewBySelectedValue = Convert.ToInt32(Request.QueryString["viewBySelectedValue"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["demogrID"]))
            {
                demographicID = Convert.ToInt32(Request.QueryString["demogrID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["groupID"]))
            {
                groupID = Convert.ToInt32(Request.QueryString["groupID"]);
            }
        }
    }
}