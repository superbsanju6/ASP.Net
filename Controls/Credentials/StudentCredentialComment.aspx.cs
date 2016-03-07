using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Classes;
using System.Collections;
using System.Web.UI.HtmlControls;
using WebSupergoo.ABCpdf8;

namespace Thinkgate.Controls.Credentials
{
    public partial class StudentCredentialComment : System.Web.UI.Page
    {
        public int studentCredentialID, studentID, credentialID;
        string CredentialLogoURL = String.Empty;
        string CredentialHeaderText = String.Empty;
        string CredentialFooterText = String.Empty;
        public string hashKey;        
        public Int32 userid;
        private SessionObject session;
        protected void Page_Load(object sender, EventArgs e)
        {
            session = (SessionObject)Session["SessionObject"];
            userid = session.LoggedInUser.Page;
            
            studentCredentialID = Request.QueryString["studCrdId"] != null ? Convert.ToInt32(Request.QueryString["studCrdId"]) : 0;
            studentID = Request.QueryString["studId"] != null ? Convert.ToInt32(Request.QueryString["studId"]) : 0;
            credentialID = Request.QueryString["crdId"] != null ? Convert.ToInt32(Request.QueryString["crdId"]) : 0;          

            hashKey = studentID.ToString() + "_" + credentialID.ToString();

            if (!IsPostBack)
            {
                LoadAndBindStudentCredentialsComment();
            }
        }

        private void LoadAndBindStudentCredentialsComment()
        {
            DataSet ds = GetStudentComments();
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblstudentname.Text = ds.Tables[0].Rows[0]["StudentName"].ToString();
                        lblCredential.Text = ds.Tables[0].Rows[0]["CredentialName"].ToString();
                    }
                }
            }
            RadGridStudentCredentialsComment.DataSource = ds.Tables[1];
            RadGridStudentCredentialsComment.DataBind();

            if (Request.QueryString["lnkId"] != null)
            {
                string strRefreshCall = "refreshCommentLink('" + Request.QueryString["lnkId"].ToString() + "','" + ds.Tables[1].Rows.Count + "');";                
                System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "RefreshCommentLink", strRefreshCall, true);
            }            

        }


        private DataSet GetStudentComments()
        {
            Hashtable hst = new Hashtable();
            DataSet ds = new DataSet();
            ds = Base.Classes.Credentials.GetStudentCredentialsComment(studentID, credentialID);
            return ds;
        }

        protected void RadGridStudentCredentialsComment_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            string ID = (e.Item as GridDataItem).GetDataKeyValue("ID").ToString();
            Int32 commentId = Convert.ToInt32(ID);
            string commandName = e.CommandName.ToString();

            if (commandName == "Delete")
            {
                DataSet ds = GetStudentComments();
                if (ds != null && ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[1];
                    DataRow[] foundRows;
                    foundRows = dt.Select("ID =" + commentId);
                    if (foundRows.Length > 0)
                        dt.Rows.Remove(foundRows[0]);
                }
                Base.Classes.Credentials.DeleteComment(studentCredentialID, commentId, "NO");
                LoadAndBindStudentCredentialsComment();
            }
        }

        protected void RadGridStudentCredentialsComment_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                GridHeaderItem dataHeaderItem = e.Item as GridHeaderItem;

            }
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem dataItem = e.Item as GridDataItem;
                    ImageButton lnkeditbutton = (ImageButton)e.Item.FindControl("lnkedit");
                    if (userid == Convert.ToInt32(dataItem["userid"].Text))
                    {
                        (dataItem["RemoveBtn"].Controls[0] as ImageButton).Visible = true;
                        lnkeditbutton.Visible = true;
                    }
                    else
                    {
                        (dataItem["RemoveBtn"].Controls[0] as ImageButton).Visible = false;
                        lnkeditbutton.Visible = false;
                    }

                    DataRowView row = (DataRowView)(dataItem).DataItem;
                    HtmlGenericControl spanDateCommented = (HtmlGenericControl)e.Item.FindControl("spnCommentDate");
                    spanDateCommented.InnerHtml = String.Format("{0:MM/dd/yy}", Convert.ToDateTime(row["DateCommented"]));

                }


            }
        }
        protected void btnPrintBtn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            Doc theDoc = new Doc();
            //clear caching?
            theDoc.HtmlOptions.PageCacheEnabled = false;
            theDoc.HtmlOptions.UseNoCache = true;
            theDoc.HtmlOptions.PageCacheClear();
            theDoc.HtmlOptions.PageCachePurge();
            theDoc.HtmlOptions.UseResync = true;
            theDoc.Rect.String = "10 90 600 750";
            string hostURL = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host.ToString();
            string callUrl = ResolveUrl("~/Controls/Credentials/PDF/StudentCredentialCommentsPdf.aspx?studentCredentialID=" + studentCredentialID + "&studentID=" + studentID + "&credentialID=" + credentialID);
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
            theDoc = AddHeaderFooter(theDoc);
            byte[] pdf = theDoc.GetData();

            Response.Clear();
            Response.ClearHeaders();    // Add this line
            Response.ClearContent();    // Add this line

            Response.AddHeader("Content-Disposition", "attachment; filename=" + "StudentCredentialComments" + ".pdf");
            Response.AddHeader("content-length", pdf.Length.ToString());
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(pdf);
            Response.End();
        }

        private Doc AddHeaderFooter(Doc theDoc)
        {
            int theCount = theDoc.PageCount;
            int i = 0;
            //This block add Header
            BindPDFInfoData();
            string theFont = "Helvetica-Bold";
            theDoc.Font = theDoc.AddFont(theFont);
            theDoc.FontSize = 10;
            string imgPath = Server.MapPath(CredentialLogoURL);
            for (i = 1; i <= theCount; i++)
            {
                theDoc.PageNumber = i;
                theDoc.Rect.String = "10 750 490 777";
                theDoc.HPos = 0.5;
                theDoc.VPos = 0.4;
                theDoc.AddText(CredentialHeaderText);
                //if (i == 1)
                //{
                //    theDoc.Rect.String = "500 730 600 775";
                //    if (File.Exists(imgPath))
                //    {
                //        theDoc.AddImage(imgPath);
                //    }
                //    else
                //    {
                //        imgPath = Server.MapPath("~/Images" + "/CredentialLogo.jpg");
                //        if (File.Exists(imgPath))
                //            theDoc.AddImage(imgPath);
                //    }
                //    //SetImageWidthAndHeight(theDoc);//Do not remove 
                //}
                theDoc.Rect.String = "5 10 605 783";
                theDoc.FrameRect();
            }
            //This block add Footer
            for (i = 1; i <= theCount; i++)
            {
                theDoc.PageNumber = i;
                theDoc.Rect.String = "10 10 500 60";
                theDoc.HPos = 0.5;
                theDoc.VPos = 0.5;
                theDoc.FontSize = 10;
                theDoc.AddText(CredentialFooterText);
                theDoc.Rect.String = "500 10 590 60";
                theDoc.HPos = 0.5;
                theDoc.VPos = 0.5;
                theDoc.FontSize = 8;
                theDoc.AddText("Page " + i + " of " + theCount); //Setting page number  
            }
            return theDoc;
        }

        private void BindPDFInfoData()
        {
            DataRow dtRow = Base.Classes.Credentials.GetPDFReportInfo();
            if (dtRow != null)
            {
                if (dtRow.ItemArray.Length > 0)
                {
                    CredentialLogoURL = "~/Images" + dtRow["CredentialLogoURL"].ToString();
                    CredentialHeaderText = dtRow["CredentialHeaderText"].ToString().Equals("") ? "Credentials" : dtRow["CredentialHeaderText"].ToString();
                    CredentialFooterText = dtRow["CredentialFooterText"].ToString();
                }
            }
        }

        protected void RadGridStudentCredentialsComment_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["isReport"]) && Request.QueryString["isReport"].ToString().Equals("yes", StringComparison.CurrentCultureIgnoreCase))
            {
                RadButton1.Visible = false;
                RadGridStudentCredentialsComment.Columns[0].Visible = false;
                RadGridStudentCredentialsComment.Columns[1].Visible = false;
            }
        }
    }
}