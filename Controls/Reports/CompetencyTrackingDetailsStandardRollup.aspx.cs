using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Standpoint.Core.Classes;
using System.Text.RegularExpressions;
using WebSupergoo.ABCpdf8;

namespace Thinkgate.Controls.Reports
{
    public partial class CompetencyTrackingDetailsStandardRollup : System.Web.UI.Page
    { 
        private int dataTableRowCount = 0;
        private int selectedObjectID;
        private int studentID;
        private int standardID;
        private int workSheetID;
        private int roblicSortOrder;
        private int viewBySelectedValue;
        private SessionObject session;
        public Guid userid;
        private string ID_Encrypted;
        public string StandardName;
        private int RubricItemSort, LevelID, LevelObjectID;
        DataSet ds = new DataSet();
        public string StripHtml(string arg)
        {
            return HtmlRemoval.StripTagsRegex(arg);
        }

        public static class HtmlRemoval
        {
            /// <summary>
            /// Remove HTML from string with Regex.
            /// </summary>
            public static string StripTagsRegex(string source)
            {
                source = Regex.Replace(source, "=\"", string.Empty);
                return Regex.Replace(source, "<.*?>", string.Empty);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            session = (SessionObject)Session["SessionObject"];
            userid = session.LoggedInUser.UserId;

            QueryStringData();

            if (!IsPostBack)
            {
                radGridCompetency.Visible = true;
                RadScriptManager scriptManager = (RadScriptManager)ScriptManager.GetCurrent(this.Page);
                Base.Classes.CompetencyTracking.CompetencyTrackingReport obj = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();

                ds = obj.GetStudentStandardList(workSheetID, standardID, roblicSortOrder, LevelID, LevelObjectID, studentID);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    lblStudentName.Text = ds.Tables[0].Rows[0]["StudentName"].ToString();
                    lblStandardDesc.Text = ds.Tables[0].Rows[0]["StandardDesc"].ToString();
                    StandardName = ds.Tables[0].Rows[0]["StandardName"].ToString();
                    lblrubricvalue.Text = ds.Tables[0].Rows[0]["Name"].ToString();
                    lnkStandard.Text = StandardName;
                    HyperLink link = new HyperLink();
                    ID_Encrypted = Encryption.EncryptInt(standardID);
                    link.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + ID_Encrypted;
                    lnkStandard.Attributes.Add("onclick", "window.open('" + link.ResolveClientUrl(link.NavigateUrl) + "');");
                    lnkStandard.Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");
                    lblcount.Text = ds.Tables[1].Rows.Count.ToString();
                    BindPerformanceGrid();

                }
            }
        }

        private void BindPerformanceGrid()
        {
            radGridCompetency.DataSource = ds.Tables[1];
            radGridCompetency.DataBind();
            radGridCompetency.Visible = true;


        }

        protected void radGridHistory_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                GridHeaderItem dataHeaderItem = e.Item as GridHeaderItem;

            }
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                Label lblchildStd = (Label)e.Item.FindControl("lblchildStd");
                int  StdId = Convert.ToInt32(((HiddenField)e.Item.FindControl("hd_std_id")).Value);
                GridDataItem dataBoundItem = e.Item as GridDataItem;
                HyperLink link = new HyperLink();
                ID_Encrypted = Encryption.EncryptInt((StdId));
                link.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + ID_Encrypted;
                lblchildStd.Attributes.Add("onclick", "window.open('" + link.ResolveClientUrl(link.NavigateUrl) + "');");
                lblchildStd.Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");
            }
        }

        protected void radGridCompetency_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindPerformanceGrid();
        }

        /// <summary>
        /// ItemCommand for handling Edit and Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radGridCompetency_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {

            if (e.CommandName.ToString() == "Remove")
            {
                int PerformanceId = Convert.ToInt32(e.CommandArgument.ToString());
                CompetencyWorkSheet.DeleteHistoryPerformance(PerformanceId);
            }
            BindPerformanceGrid();


        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            Doc theDoc = new Doc();
            //clear caching?
            theDoc.HtmlOptions.PageCacheEnabled = false;
            theDoc.HtmlOptions.UseNoCache = true;
            theDoc.HtmlOptions.PageCacheClear();
            theDoc.HtmlOptions.PageCachePurge();
            theDoc.HtmlOptions.UseResync = true;
            theDoc.Rect.String = "20 90 580 750";
            string hostURL = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host.ToString();
            string callUrl = ResolveUrl("~/Controls/Reports/CompetencyTrackingDetailsStandardPDF.aspx?StandardId=" + standardID + "&StudentId=" + studentID + "&WorksheetId=" + workSheetID + "&roblicSortOrder=" + roblicSortOrder);
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
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "CompetencyStandardList" + ".pdf");
            Response.AddHeader("content-length", pdf.Length.ToString());
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(pdf);
            Response.End();
        }


        private void QueryStringData()
        {


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
            if (!string.IsNullOrEmpty(Request.QueryString["studentID"]))
            {
                studentID = Convert.ToInt32(Request.QueryString["studentID"]);
            }
        }
    }
}