using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using WebSupergoo.ABCpdf8;
using WebSupergoo.ABCpdf8.Objects;

namespace Thinkgate.Controls.Reports
{
    public partial class CredentialTrackingStudentCount : BasePage
    {
        string CredentialLogoURL = String.Empty;
        string CredentialHeaderText = String.Empty;
        string CredentialFooterText = String.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                
                BindStudentCountGrid();
                
            }
        }

        protected void BindStudentCountGrid()
        {
            if (Session["dtStudentCount"] != null)
            { 
                DataTable dtStudentCount = ((DataTable)Session["dtStudentCount"]);
                //Session.Remove("dtStudentCount");
                if (dtStudentCount != null && dtStudentCount.Rows.Count > 0)
                {
                    radGridStudentCount.DataSource = dtStudentCount;
                    lblStudentCount.Text = dtStudentCount.Rows.Count.ToString();
                    
                }
                else
                {
                    radGridStudentCount.DataSource = new string[0];
                    lblStudentCount.Text = "0";
                }

                radGridStudentCount.DataBind();
            }        
        }

        protected void radGridStudentCount_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {

        }

        protected void btnPrintBtn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            int credentialID = Convert.ToInt32(Request.QueryString["credentialID"]);
            Doc theDoc = new Doc();
            //clear caching?
            theDoc.HtmlOptions.PageCacheEnabled = false;
            theDoc.HtmlOptions.UseNoCache = true;
            theDoc.HtmlOptions.PageCacheClear();
            theDoc.HtmlOptions.PageCachePurge();
            theDoc.HtmlOptions.UseResync = true;
            theDoc.Rect.String = "10 90 600 750";




            string selectedCriteria = Session["selectedCriteria"].ToString();

            string hostURL = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host.ToString();
            string callUrl = ResolveUrl("~/Controls/Credentials/PDF/StudentCountListPdf.aspx?xCriteria=" + selectedCriteria);
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
            //string filename = lblStudentName.Text.Replace(',', '-') + "EarnedCredentialList";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "CredentialList" + ".pdf");
            Response.AddHeader("content-length", pdf.Length.ToString());
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(pdf);
            Response.End();
            theDoc.Clear();

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
                if (i == 1)
                {
                    theDoc.Rect.String = "500 730 600 775";
                    if (File.Exists(imgPath))
                    {
                        theDoc.AddImage(imgPath);
                    }
                    else
                    {
                        imgPath = Server.MapPath("~/Images" + "/CredentialLogo.jpg");
                        if (File.Exists(imgPath))
                            theDoc.AddImage(imgPath);
                    }
                    //SetImageWidthAndHeight(theDoc);//Do not remove 
                }
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
        private void SetImageWidthAndHeight(Doc theDoc)
        {
            
            foreach (IndirectObject io in theDoc.ObjectSoup)
            {
                if (io is PixMap)
                {
                    PixMap pm = (PixMap)io;
                    pm.Realize(); // eliminate indexed color images
                    pm.Resize(200, 200);
                }
            }
        }

        
        
    }
}