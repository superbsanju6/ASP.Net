using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using CultureInfo = System.Globalization.CultureInfo;
using Standpoint.Core.Classes;

namespace Thinkgate.Controls.CompetencyWorksheet
{
    using System.Data;
    using Telerik.Web.UI;
    using Thinkgate.Base.Classes;

    public partial class Comments : System.Web.UI.Page
    {
        public Int32 StandardId;
        public int StudentId;
        public Int32 WorksheetId;
        private SessionObject session;
        public int CommentsCount;
        public Guid userid;
        public string ID_Encrypted;
        public string StandardName;
        public bool isGridLoaded = false;

     
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
                radGridComments.Visible = true;
                RadScriptManager scriptManager = (RadScriptManager)ScriptManager.GetCurrent(this.Page);
                DataSet ds = CompetencyWorkSheet.GetCompetencyWorksheetSingleStudentStandard(StandardId, StudentId, WorksheetId);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    lblStudentName.Text = ds.Tables[0].Rows[0]["StudentName"].ToString();
                    ID_Encrypted = Encryption.EncryptInt(StandardId);
                    StandardName = ds.Tables[0].Rows[0]["StandardName"].ToString();
                    lblStandardDesc.Text = ds.Tables[0].Rows[0]["StandardDesc"].ToString();
                    lnkStandard.Text = StandardName;
                    HyperLink link = new HyperLink();
                    link.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + ID_Encrypted;
                    lnkStandard.Attributes.Add("onclick", "window.open('" + link.ResolveClientUrl(link.NavigateUrl) + "');");
                    lnkStandard.Attributes.Add("style", "cursor:pointer; text-decoration: underline; color: #034AF3");
                    BindDataToGrid();
                }
                
            }
         }

       private void BindDataToGrid()
       {
           DataSet ds = CompetencyWorkSheet.GetCompetencyComments(StandardId, StudentId);//, WorksheetId);
           if (ds != null && ds.Tables[0].Rows.Count > 0)
           {
               CommentsCount = ds.Tables[0].Rows.Count;
               radGridComments.DataSource = ds;
               radGridComments.DataBind();
               radGridComments.Visible = true;
               isGridLoaded = true;
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
           BindDataToGrid();
       }

       protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
       {
           BindDataToGrid();
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
                   //resultPanel.Visible = true;
                   //addPanel.Visible = false;
                   //lblResultMessage.Text = @"Comment Deleted Successfully!";
               }
               BindDataToGrid();
           }
           
       

    }
}