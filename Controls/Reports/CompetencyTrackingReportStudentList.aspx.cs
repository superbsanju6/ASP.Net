using Standpoint.Core.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using WebSupergoo.ABCpdf8;

namespace Thinkgate.Controls.Reports
{
    public partial class CompetencyTrackingReportStudentList : System.Web.UI.Page
    {

        private int selectedObjectID;
        private int standardID;
        private int workSheetID;
        private int roblicSortOrder;
        private int viewBySelectedValue;
        private int demographicID = 0;
        private int groupID = 0;
        private string ID_Encrypted;

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
                DataSet dsStudentList = oCTR.GetCVTEReportStudentList(workSheetID, standardID, roblicSortOrder, viewBySelectedValue, selectedObjectID, demographicID, groupID);
                if (dsStudentList != null)
                {
                    DataTable dtViewByDetail = dsStudentList.Tables[0];
                    if (dtViewByDetail.Rows.Count > 0)
                    {
                        lblViewBy.Text = dtViewByDetail.Rows[0]["ViewBy"].ToString() + ": ";// +dtViewByDetail.Rows[0]["ViewByValue"].ToString();
                        tdViewByText.InnerText = dtViewByDetail.Rows[0]["ViewByValue"].ToString();
                        lblRubric.Text = "Rubric Value: ";
                        spnRubricValue.InnerText = dtViewByDetail.Rows[0]["RubricValue"].ToString();
                        lblCompetency.Text = "Competency:  ";
                        aCompetencyValue.Text = dtViewByDetail.Rows[0]["Competency"].ToString();

                        HyperLink link = new HyperLink();
                        ID_Encrypted = Encryption.EncryptInt(standardID);
                        link.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + ID_Encrypted;
                        aCompetencyValue.Attributes.Add("onclick", "window.open('" + link.ResolveClientUrl(link.NavigateUrl) + "');");
                        aCompetencyValue.Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");

                        StringWriter compentencyDetail = new StringWriter();
                        HttpUtility.HtmlDecode(dtViewByDetail.Rows[0]["CompetencyDesc"].ToString(), compentencyDetail);
                        lblCompetencyDetail.Text = compentencyDetail.ToString();
                        lblStudentCount.Text = "Students:" + dtViewByDetail.Rows[0]["StudentCount"].ToString();
                       
                    }

                    DataTable dtStudentList = dsStudentList.Tables[1];
                    if (dtStudentList.Rows.Count > 0)
                    {
                        radGridStudentCount.Visible = true;
                        lblResultMessage.Visible = false;
                        radGridStudentCount.DataSource = dtStudentList;
                        radGridStudentCount.DataBind();
                        
                    }
                    else
                    {
                        radGridStudentCount.Visible = false;
                        lblResultMessage.Visible = true;
                        lblResultMessage.Text = "No records found";
                    }
                }
                else
                {
                    radGridStudentCount.Visible = false;
                    lblResultMessage.Visible = true;
                    lblResultMessage.Text = "No records found";
                }
            }
            catch (Exception ex)
            {
                radGridStudentCount.Visible = false;
                lblResultMessage.Visible = true;
                lblResultMessage.Text = ex.Message.ToString();
            }
        }

        private void QueryStringData()
        {

            if (!string.IsNullOrEmpty(Request.QueryString["demographicID"]))
            {
                demographicID = Convert.ToInt32(Request.QueryString["demographicID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["groupID"]))
            {
                groupID = Convert.ToInt32(Request.QueryString["groupID"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["selectedObjectID"]))
            {
                selectedObjectID = Convert.ToInt32(Request.QueryString["selectedObjectID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["standartID"]))
            {
                standardID = Convert.ToInt32(Request.QueryString["standartID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["workSheetID"]))
            {
                workSheetID = Convert.ToInt32(Request.QueryString["workSheetID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["robricsortNumber"]))
            {
                roblicSortOrder = Convert.ToInt32(Request.QueryString["robricsortNumber"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["viewbySelectedValue"]))
            {
                viewBySelectedValue = Convert.ToInt32(Request.QueryString["viewbySelectedValue"]);
            }
        }
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            Doc theDoc = new Doc();
            QueryStringData();
            //clear caching?
            theDoc.HtmlOptions.PageCacheEnabled = false;
            theDoc.HtmlOptions.UseNoCache = true;
            theDoc.HtmlOptions.PageCacheClear();
            theDoc.HtmlOptions.PageCachePurge();
            theDoc.HtmlOptions.UseResync = true;
            theDoc.Rect.String = "20 90 580 750";
            string hostURL = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host.ToString();
            /*workSheetID, standardID, roblicSortOrder, viewBySelectedValue, techerID, 0, 0
             */
            string callUrl = ResolveUrl("~/Controls/Reports/CompetencyTrackingReportStudentListPDF.aspx?workSheetID=" + workSheetID + "&standardID=" + standardID + "&roblicSortOrder=" + roblicSortOrder + "&viewBySelectedValue=" + viewBySelectedValue + "&techerID=" + selectedObjectID + "&demogrID=" + demographicID + "&groupID=" + groupID);
            int theID;
            theID = theDoc.AddImageUrl(hostURL + callUrl);
            while (true)
            {

                if (!theDoc.Chainable(theID))
                    break;
                theDoc.Page = theDoc.AddPage();
                theID = theDoc.AddImageToChain(theID);
            }
            for (int i = 1; i <= theDoc.PageCount; i++)
            {
                theDoc.PageNumber = i;

                theDoc.Flatten();
            }

            byte[] pdf = theDoc.GetData();

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "CompetencyStudentList" + ".pdf");
            Response.AddHeader("content-length", pdf.Length.ToString());
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(pdf);
            Response.End();
        }

        protected void radGridStudentCount_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                var dataItem = (DataRowView)item.DataItem;
                var link = (Label)item.FindControl("studentNameLink");
                string studentID = dataItem["StudentID"]==null ?"":dataItem["StudentID"].ToString();
                string Student_ID_Encrypted = Encryption.EncryptString(studentID);
                if (link != null)
                {
                    link.Text = dataItem["Name"].ToString();
                    link.Attributes["onclick"] = "window.open('../../Record/Student.aspx?childPage=yes&xID=" + Student_ID_Encrypted + "'); return false;";
                    link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                    link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                    link.Attributes["style"] = "color:#00F; text-decoration:underline; cursor:pointer;";

                }
            }
        }

    }
}