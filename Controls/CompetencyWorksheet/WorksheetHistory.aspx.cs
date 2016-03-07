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

namespace Thinkgate.Controls.CompetencyWorksheet
{
   public partial class WorksheetHistory : System.Web.UI.Page
    {
        private int dataTableRowCount = 0;
        public Int32 StandardId;
        public int StudentId;
        public Int32 WorksheetId;
        private SessionObject session;
        public Guid userid;
        private string ID_Encrypted;
        public string StandardName;

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
                source= Regex.Replace(source, "=\"", string.Empty);
                return Regex.Replace(source, "<.*?>", string.Empty);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            session = (SessionObject)Session["SessionObject"];
            userid = session.LoggedInUser.UserId;
  
               
                if (Request.QueryString["StandardID"] != null && Request.QueryString["StudentID"] != null && Request.QueryString["WorksheetID"] != null)
                {
                    StandardId = Convert.ToInt32(Request.QueryString["StandardID"]);
                    StudentId = Convert.ToInt32(Request.QueryString["StudentID"]);
                    WorksheetId = Convert.ToInt32(Request.QueryString["WorksheetID"]);
                }
                if (!IsPostBack)
                {
                    radGridHistory.Visible = true;
                    radGridComments.Visible = true;
                    RadScriptManager scriptManager = (RadScriptManager)ScriptManager.GetCurrent(this.Page);

                    DataSet ds = CompetencyWorkSheet.GetCompetencyWorksheetSingleStudentStandard(StandardId, StudentId, WorksheetId);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        lblStudentName.Text = ds.Tables[0].Rows[0]["StudentName"].ToString();
                        lblStandardDesc.Text = ds.Tables[0].Rows[0]["StandardDesc"].ToString();
                        StandardName = ds.Tables[0].Rows[0]["StandardName"].ToString();
                        lnkStandard.Text = StandardName;
                        HyperLink link = new HyperLink();
                        ID_Encrypted = Encryption.EncryptInt(StandardId);
                        link.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + ID_Encrypted;
                        lnkStandard.Attributes.Add("onclick", "window.open('" + link.ResolveClientUrl(link.NavigateUrl) + "');");
                        lnkStandard.Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");
                        BindPerformanceGrid();
                        BindCommentGrid();
                    }
                }
        }

        private void BindPerformanceGrid()
        {
            DataSet HistoryDs = CompetencyWorkSheet.GetCompetencyWorksheetHistory(StandardId, StudentId);
            if (HistoryDs != null && HistoryDs.Tables[0].Rows.Count > 0)
            {
                radGridHistory.DataSource = HistoryDs;
                radGridHistory.DataBind();
                radGridHistory.Visible = true;
            }

            else
            {
                //lblPerformanceMsg.Visible = true;
                //radGridHistory.Visible = false;
                radGridHistory.DataSource = HistoryDs;
                radGridHistory.DataBind();
            }

        }

        protected void radGridHistory_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                GridHeaderItem dataHeaderItem = e.Item as GridHeaderItem;

            }
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                ImageButton lnkDeleteButton = (ImageButton)e.Item.FindControl("lnkDelete");
                 GridDataItem dataBoundItem = e.Item as GridDataItem;
                 if (userid.ToString() == dataBoundItem["Teacher"].Text)
                 {
                     lnkDeleteButton.Visible = true;
                 }
                 else
                 {
                     lnkDeleteButton.Visible = false;
                 }
            }
        }

        protected void radGridHistory_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindPerformanceGrid();
        }

        /// <summary>
        /// ItemCommand for handling Edit and Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radGridHistory_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
           
                if (e.CommandName.ToString() == "Remove")
                {
                    int PerformanceId = Convert.ToInt32(e.CommandArgument.ToString());
                    CompetencyWorkSheet.DeleteHistoryPerformance(PerformanceId);
                }
                BindPerformanceGrid();    
            
            
        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindPerformanceGrid();
        }

      
      
        private void BindCommentGrid()
        {
            DataSet ds = CompetencyWorkSheet.GetCompetencyComments(StandardId, StudentId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                radGridComments.DataSource = ds;
                radGridComments.DataBind();
                radGridComments.Visible = true;
            }
            else
            {
                //lblCommentMsg.Visible = true;
                //radGridComments.Visible = false;
                radGridComments.DataSource = ds;
                radGridComments.DataBind();
            }

        }

        protected void radGridComments_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                GridHeaderItem dataHeaderItem = e.Item as GridHeaderItem;

            }
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                ImageButton lnkDeleteButton = (ImageButton)e.Item.FindControl("lnkDelete");
                ImageButton lnkEditButton = (ImageButton)e.Item.FindControl("lnkEdit");
                GridDataItem dataBoundItem = e.Item as GridDataItem;
                if (userid.ToString() == dataBoundItem["Userid"].Text)
                {
                    lnkDeleteButton.Visible = true;
                    lnkEditButton.Visible = true;
                }
                else
                {
                    lnkDeleteButton.Visible = false;
                    lnkEditButton.Visible = false;

                }
            }
        }

        protected void RadGridComments_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindCommentGrid();
        }

        protected void OnCommentSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindCommentGrid();
        }

        /// <summary>
        /// ItemCommand for handling Edit and Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radGridComments_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
           if (e.CommandName.ToString() == "Remove")
               {
                   int CommentId = Convert.ToInt32(e.CommandArgument.ToString());
                   CompetencyWorkSheet.DeleteCompetencyComments(CommentId);
                  
               }
               BindCommentGrid();
          
        }
    }
}