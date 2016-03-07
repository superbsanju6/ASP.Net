using System.Text;
using ClosedXML.Excel;
using Standpoint.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using WebSupergoo.ABCpdf8;
using WebSupergoo.ABCpdf8.Objects;

namespace Thinkgate.Controls.Credentials
{
    public partial class EditCredentials : BasePage
    {
        string CredentialLogoURL = String.Empty;
        string CredentialHeaderText = String.Empty;
        string CredentialFooterText = String.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            bool alignmentDropdownPostback = IsAlignmentDropdownPostback();

            if (!IsPostBack || alignmentDropdownPostback == true)
            {
                if (SessionObject.IsAlignmentDataAvailable == null)
                {
                    SessionObject.IsAlignmentDataAvailable = Base.Classes.Credentials.IsAlignmentDataAvailable();
                }

                if (SessionObject.IsAlignmentDataAvailable == true)
                {
                    spnAlignment.Visible = true;
                }
                else
                {
                    spnAlignment.Visible = false;
                }

                Session.Remove("dsCredentials");
                BindPageControls();
            }
        }

        protected void ddlEarned_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindPageControls();
        }

        private bool IsAlignmentDropdownPostback()
        {
            return Request.Params["__EVENTTARGET"] != null && Request.Params["__EVENTARGUMENT"] != null && Request.Params["__EVENTTARGET"].EndsWith(this.ddlAlignments.ID) && Request.Params["__EVENTARGUMENT"].Equals("OnClientEntryAdded");
        }

        protected void gridCredentials_PreRender(object sender, EventArgs e)
        {
            if (SessionObject.IsAlignmentDataAvailable == true)
                gridCredentials.MasterTableView.GetColumn("Alignment").Visible = true;
            else
                gridCredentials.MasterTableView.GetColumn("Alignment").Visible = false;
            gridCredentials.Rebind();
        }

        private void BindPageControls()
        {
            List<string> selectedAlignments = GetSelectedNodeWithChildren();

            DataSet ds = new DataSet();
            bool flagFetchFreshData = true;
            if (Session["dsCredentials"] != null)
            {
                ds = (DataSet)Session["dsCredentials"];

                try
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        flagFetchFreshData = false;
                    }
                }
                catch (Exception ex)
                {
                    flagFetchFreshData = true;
                }
            }

            if (flagFetchFreshData)
            {
                drGeneric_String drStringObj = new drGeneric_String();
                foreach (string str in selectedAlignments)
                {
                    drStringObj.Add(str);
                }

                //include drStringObj in future
                ds = Base.Classes.Credentials.GetCredentials("ALL", "DETAIL");
                Session["dsCredentials"] = ds;
            }

            if (ds != null && ds.Tables.Count == 4)
            {
                ds.Relations.Clear();
                ds.Relations.Add(new DataRelation("CredentialUrls", ds.Tables[0].Columns["ID"], ds.Tables[3].Columns["CredentialId"], false));
                ds.Relations.Add(new DataRelation("CredentialAlignments", ds.Tables[0].Columns["ID"], ds.Tables[2].Columns["CredentialId"], false));
                ds.Relations.Add(new DataRelation("CredentialAlignmentsMaster", ds.Tables[2].Columns["AlignmentId"], ds.Tables[1].Columns["ID"], false));

                List<int> iAlign = new List<int>();

                if (!this.IsPostBack)
                {

                    ds.Tables[0].DefaultView.RowFilter = ddlEarned.SelectedIndex == 0 ? "IsActive = true" : "IsActive= false";
                    gridCredentials.DataSource = ds.Tables[0].DefaultView;
                    gridCredentials.DataBind();
                }

                else
                {
                    string filter = string.Empty;
                    if (selectedAlignments.Count > 0 && selectedAlignments[0] == "0")
                    {
                        filter = ddlEarned.SelectedIndex == 0 ? "IsActive = true" : "IsActive= false";
                    }
                    else
                    {
                        if (selectedAlignments.Count > 0)
                        {
                            ds.Tables[2].DefaultView.RowFilter = "AlignmentId in (" + string.Join(",", selectedAlignments.ToArray()) + ")";
                            foreach (DataRow dr in ds.Tables[2].DefaultView.ToTable().Rows)
                            {
                                iAlign.Add(Convert.ToInt32(dr["CredentialId"]));
                            }
                            if (iAlign.Count == 0)
                            {
                                iAlign.Add(0);
                            }
                        }
                        string status = ddlEarned.SelectedIndex == 0 ? "IsActive = true" : "IsActive= false";
                        if (iAlign.Count > 0)
                            filter = status + " AND ID in (" + string.Join(",", iAlign.ToArray()) + ")";
                        else
                            filter = status;
                    }
                    ds.Tables[0].DefaultView.RowFilter = filter;

                    gridCredentials.DataSource = ds.Tables[0].DefaultView;
                    gridCredentials.DataBind();
                }

            }
            BindAlignmentsTree();

            System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "SetControlCSS", "setControlCSS();", true);
        }

        protected void gridCredentials_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem gridItem = (GridDataItem)e.Item;
                DataRowView row = (DataRowView)(gridItem).DataItem;
                HtmlGenericControl spanCredential = (HtmlGenericControl)gridItem.FindControl("spanCredentialName");
                HtmlGenericControl spanAlignment = (HtmlGenericControl)gridItem.FindControl("spanAlignment");

                foreach (DataRow dr in row.Row.GetChildRows("CredentialUrls"))
                {
                    spanCredential.InnerHtml = spanCredential.InnerHtml + "<br/><a href='" + dr["URL"].ToString() + "' target='_blank' >" + dr["URL"].ToString() + "</a>";
                }

                foreach (DataRow dr in row.Row.GetChildRows("CredentialAlignments"))
                {
                    foreach (DataRow drAlign in dr.GetChildRows("CredentialAlignmentsMaster"))
                        spanAlignment.InnerHtml = spanAlignment.InnerHtml + drAlign["CredentialAlignment"].ToString() + "<br/>";
                }
            }
        }

        protected void ddlAlignments_DataBound(object sender, EventArgs e)
        {
            ((RadDropDownTree)sender).ExpandAllDropDownNodes();
        }

        protected void ddlAlignments_EntryAdded(object sender, DropDownTreeEntryEventArgs e)
        { }

        protected void ddlAlignments_NodeDataBound(object sender, DropDownTreeNodeDataBoundEventArguments e)
        {
            DataRowView row = (DataRowView)e.DropDownTreeNode.DataItem;

            if (e.DropDownTreeNode.Text.Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                e.DropDownTreeNode.CreateEntry();
                RadTreeNode node = ddlAlignments.EmbeddedTree.Nodes.FindNodeByValue(e.DropDownTreeNode.Value);
                if (node != null)
                {
                    node.Font.Bold = true;
                }
            }
        }

        private List<string> GetSelectedNodeWithChildren()
        {
            List<string> alignmentIds = new List<string>();

            if (ddlAlignments.EmbeddedTree.SelectedNode != null)
            {
                alignmentIds.Add(ddlAlignments.EmbeddedTree.SelectedNode.Value);
                foreach (RadTreeNode node in ddlAlignments.EmbeddedTree.SelectedNode.Nodes)
                {
                    alignmentIds.Add(node.Value);
                }
            }

            return alignmentIds;
        }

        private void BindAlignmentsTree()
        {
            if (!IsPostBack)
            {
                DataTable dtAlignments = GetAlignmentsData();
                if (dtAlignments != null && dtAlignments.Rows.Count > 1)
                {
                    ddlAlignments.DataSource = dtAlignments;
                    ddlAlignments.DataBind();

                    if (ddlAlignments.EmbeddedTree.Nodes.Count > 0)
                    {
                        FormatParentNodes(ddlAlignments.EmbeddedTree.Nodes[0]);
                    }
                }
            }
        }

        private DataTable GetAlignmentsData()
        {
            return Base.Classes.Credentials.GetAllAlignments();
        }

        protected void CommonStandardTree_OnNodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            DataRowView dataSourceRow = (DataRowView)e.Node.DataItem;
            e.Node.Attributes["ButtonText"] = dataSourceRow["CredentialAlignment"].ToString();
        }

        private void FormatParentNodes(RadTreeNode node)
        {
            if (node.Text.Equals("All", StringComparison.InvariantCultureIgnoreCase) && node.Index <= 0)
            {
                node = node.Next;
            }
            if (node.Nodes.Count > 0)
            {
                node.Font.Bold = true;
                foreach (RadTreeNode childNode in node.Nodes)
                {
                    FormatParentNodes(childNode);
                }
                if (node.Next != null)
                {
                    node = node.Next;
                    FormatParentNodes(node);
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
            theDoc.Rect.String = "20 90 580 750";
            string hostURL = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host.ToString();
            string callUrl = ResolveUrl("~/Controls/Credentials/PDF/CredentialListPdf.aspx");
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
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "CredentialList" + ".pdf");
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

        protected void btnHiddenBind_ServerClick(object sender, EventArgs e)
        {
            Session.Remove("dsCredentials");
            BindPageControls();
        }

        private XLWorkbook BuildWorkBook(DataSet reportData)
        {
            DataTable dt = reportData.Tables[0].Copy();
            dt.Columns.Remove("ID");
            dt.Columns.Remove("IsActive");
            dt.Columns["Name"].ColumnName = "Credential Name";
            dt.Columns["Status_Desc"].ColumnName = "Status";
            dt.Columns.Remove("DateCreated");
            dt.Columns.Remove("DateModified");
            dt.Columns["CreatedBy"].ColumnName = "Alignment";
            dt.Columns["ModifiedBy"].ColumnName = "URLs";
           
            XLWorkbook workbook = new XLWorkbook();

            if (dt.Rows.Count > 0)
            {
                var ws = workbook.Worksheets.Add("Sheet1");
                // set the header size and alignment
                IXLRows headerRow = ws.Rows(1, 1);
                if (headerRow != null)
                {
                    headerRow.Height = 20;
                    headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRow.Style.Font.SetBold();
                }

                int colCount;
                //Write second rows first so that we can calculate width of for the headers
                colCount = 1; //reset columns
                foreach (DataColumn column in dt.Columns)
                {
                    ws.Cell(1, colCount).Value = column.ColumnName;
                    ws.Cell(1, colCount).Style.Font.FontColor = XLColor.White;
                    ws.Cell(1, colCount).Style.Fill.BackgroundColor = XLColor.BlueBell;
                    ws.Cell(1, colCount).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column(colCount).AdjustToContents(70.00, 70.00);
                    if (colCount == 2)
                    {
                        ws.Column(colCount).AdjustToContents(37.00, 37.00);
                    }
                    if (colCount == 3)
                    {
                        ws.Column(colCount).AdjustToContents(11.00, 11.00);
                    }
                    colCount++;
                }
                var rowCount = 2;
                for(int i=0;i<dt.Rows.Count;i++) // Loop over the rows.
                {
                    DataRow row = reportData.Tables[0].Rows[i] ;
                    
                    colCount = 1;
                    string URLs = String.Empty;
                    string alignments = String.Empty;
                    char[] trimchars={'\r','\n'};
                       
                    DataRowView drv = reportData.Tables[0].DefaultView[reportData.Tables[0].Rows.IndexOf(row)];
                    foreach (DataRow dr in drv.Row.GetChildRows("CredentialUrls"))
                    {
                        
                        URLs = URLs + dr["URL"].ToString().Trim() + "\r\n";
                    }
                    //URLs = URLs.Remove(URLs.Length - 5, 4);
                    URLs = URLs.TrimEnd(trimchars);
                    foreach (DataRow dr in drv.Row.GetChildRows("CredentialAlignments"))
                    {
                        foreach (DataRow drAlign in dr.GetChildRows("CredentialAlignmentsMaster"))
                            alignments = alignments + drAlign["CredentialAlignment"].ToString() + "\r\n";
                    }
                    alignments = alignments.TrimEnd(trimchars);
                    foreach (var item in dt.Rows[i].ItemArray) // Loop over the items.
                    {
                        string theCellValue = item.ToString();

                        
                        ws.Cell(rowCount, colCount).Value = Server.HtmlDecode(theCellValue);
                        if (colCount == 1)
                        { 
                        ws.Cell(rowCount, colCount).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(rowCount, colCount).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell(rowCount, colCount).Style.Alignment.WrapText = true;
                      
                        }
                        if (colCount==2)
                            ws.Cell(rowCount, colCount).Value = alignments.Trim();
                        if (colCount == 4)
                        {
                            ws.Cell(rowCount, colCount).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Cell(rowCount, colCount).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(rowCount, colCount).Style.Alignment.WrapText = true;
                            ws.Cell(rowCount, colCount).Value = Server.HtmlDecode(URLs);
                            if (URLs!="")
                            {
                                ws.Cell(rowCount, colCount).Hyperlink = new XLHyperlink(Server.HtmlDecode(URLs));

                            }

                        }
                        colCount++;
                    }
                    rowCount++;
                }
                
               
                return workbook;
            }
            return null;
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = (DataSet)Session["dsCredentials"];
            if (ds != null && ds.Tables.Count == 4)
            {
                ds.Relations.Clear();
                ds.Relations.Add(new DataRelation("CredentialUrls", ds.Tables[0].Columns["ID"], ds.Tables[3].Columns["CredentialId"], false));
                ds.Relations.Add(new DataRelation("CredentialAlignments", ds.Tables[0].Columns["ID"], ds.Tables[2].Columns["CredentialId"], false));
                ds.Relations.Add(new DataRelation("CredentialAlignmentsMaster", ds.Tables[2].Columns["AlignmentId"], ds.Tables[1].Columns["ID"], false));
                ExportToExcel(ds);
            }

        }

        public void ExportToExcel(DataSet reportData)
        {

            // Create the workbook
            XLWorkbook workbook = BuildWorkBook(reportData);

            //Prepare the response
            System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
            Response.Clear();
            Response.Buffer = true;

            if (workbook == null)
            {
                ClientScriptManager clientScript = Page.ClientScript;
                StringBuilder javaScriptBuilder = new StringBuilder();

                javaScriptBuilder.Append("showNoResultPopUp();");

                clientScript.RegisterStartupScript(
                    GetType(),
                    "ValidationMessage",
                    javaScriptBuilder.ToString(),
                    true);
            }
            else
            {


                using (MemoryStream memoryStream = new MemoryStream())
                {
                    if (browser.Platform.IndexOf(
                        "WinNT",
                        StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        workbook.SaveAs(memoryStream);
                        Response.ContentType =
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader(
                            "content-disposition",
                            "attachment;filename= CredentialExport.xlsx");
                        Response.BinaryWrite(memoryStream.ToArray());
                    }

                    Response.End();
                }
            }
        }

    }
}