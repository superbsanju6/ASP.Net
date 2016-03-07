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
    public partial class StudentCredentials_Expanded : BasePage
    {
        protected Base.Classes.Student _selectedStudent;
        public string CurrentStudentID = string.Empty;

        string CredentialLogoURL = String.Empty;
        string CredentialHeaderText = String.Empty;
        string CredentialFooterText = String.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadStudentObject();
            bool alignmentDropdownPostback = IsAlignmentDropdownPostback();

            if (IsPostBack == false || alignmentDropdownPostback == true)
            {
                if (SessionObject.IsAlignmentDataAvailable == null)
                {
                    SessionObject.IsAlignmentDataAvailable = Base.Classes.Credentials.IsAlignmentDataAvailable();
                }

                if (SessionObject.IsAlignmentDataAvailable != true)
                {
                    spnAlignment.Visible = false;
                }

                Session.Remove("hstCredentialComments");
                Session.Remove("dsStudentCredentials");
                BindPageControls();


            }
        }

        private void PopulateYearDropdown(DropDownList objcmbYear)
        {
            DataTable DtYearList;
            if (ViewState["DtYearList"] == null)
            {
                DtYearList = Base.Classes.Credentials.GetYearList();
                DataRow drBlank = DtYearList.NewRow();
                drBlank[0]= "";
                DtYearList.Rows.InsertAt(drBlank, 0);
                ViewState["DtYearList"] = DtYearList;
            }
            else
                DtYearList = (DataTable)ViewState["DtYearList"];

            objcmbYear.DataSource = DtYearList;
            objcmbYear.DataTextField = "YEAR_LIST";
            objcmbYear.DataValueField = "YEAR_LIST";
            objcmbYear.DataBind();

        }

        private bool IsAlignmentDropdownPostback()
        {
            return Request.Params["__EVENTTARGET"] != null && Request.Params["__EVENTARGUMENT"] != null && Request.Params["__EVENTTARGET"].EndsWith(this.ddlAlignments.ID) && Request.Params["__EVENTARGUMENT"].Equals("OnClientEntryAdded");
        }

        private void BindPageControls()
        {
            lblStudentName.Text = _selectedStudent.StudentName;
            lblStudentName.Attributes.Add("initialValue", lblStudentName.Text);
            List<string> selectedAlignments = GetSelectedNodeWithChildren();

            DataSet ds = new DataSet();
            bool flagFetchFreshData = true;
            if (Session["dsStudentCredentials"] != null)
            {
                ds = (DataSet)Session["dsStudentCredentials"];

                try
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["StudentId"]) == GetDecryptedEntityId(X_ID))
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

                ds = Base.Classes.Credentials.GetCredentialsForStudent(_selectedStudent.ID, StudentCredentialsView.Detail, drStringObj);

                Session["dsStudentCredentials"] = ds;
            }

            if (ds != null)
            {
                if (ds.Tables.Count > 2)
                {
                    ds.Relations.Clear();
                    ds.Relations.Add(new DataRelation("CredentialUrls", ds.Tables[0].Columns["CredentialID"], ds.Tables[2].Columns["CredentialID"], false));
                }

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        DataSet dsComment = GetStudentComments(_selectedStudent.ID.ToString() + "_" + dr["CredentialID"].ToString(), Convert.ToInt32(Convert.IsDBNull(dr["ID"]) ? 0 : dr["ID"]), _selectedStudent.ID, Convert.ToInt32(dr["CredentialID"]));
                        if (dsComment != null && dsComment.Tables.Count > 1)
                        {
                            dr["CommentsCount"] = dsComment.Tables[1].Rows.Count;

                        }
                    }
                    ds.Tables[0].DefaultView.RowFilter = ddlEarned.SelectedIndex == 1 ? "Earned = 1" : "1=1";
                    gridStudentCredentials.DataSource = ds.Tables[0];
                    gridStudentCredentials.DataBind();
                }
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0 /*There should always be 1 row containing credentials count*/)
                {
                    lblCredentialsEarned.Text = ds.Tables[1].Rows[0]["CredentialsCount"].ToString();
                }
            }

            if (SessionObject.IsAlignmentDataAvailable == null)
            {
                SessionObject.IsAlignmentDataAvailable = Base.Classes.Credentials.IsAlignmentDataAvailable();
            }

            if (SessionObject.IsAlignmentDataAvailable == true)
            {
                ddlAlignments.Visible = true;
                BindAlignmentsTree();
            }
            else
            {
                ddlAlignments.Visible = false;
            }

            

            System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "RefreshCommentLink", "commentLinkEnable();", true);

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

        /// <summary>
        /// decrypt the xID query parameter and use to create a Student Object.  
        /// If no xID parameter, then return with message.
        /// </summary>
        private void LoadStudentObject()
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                var studentID = GetDecryptedEntityId(X_ID);
                _selectedStudent = Base.Classes.Data.StudentDB.GetStudentByID(studentID);
                CurrentStudentID = _selectedStudent.ID.ToString();  //TODO: need to verify the value of this ID, whether it is correct value for this student ID or not.
            }
        }

        protected void gridStudentCredentials_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem gridItem = (GridDataItem)e.Item;
                DataRowView row = (DataRowView)(gridItem).DataItem;
                HtmlGenericControl span = (HtmlGenericControl)gridItem.FindControl("spanCredentialName");

                DropDownList cmbYear = (gridItem.FindControl("cmbYear") as DropDownList);
                PopulateYearDropdown(cmbYear);
                if (!Convert.IsDBNull(row.Row["SchoolYearEarned"])) cmbYear.Items.FindByText(row.Row["SchoolYearEarned"].ToString()).Selected = true;
                else cmbYear.SelectedIndex = 0;

                if (!Convert.IsDBNull(row["Earned"]) && Convert.ToBoolean(row["Earned"]) == true)
                {
                    if (!Convert.IsDBNull(row["EarnedDate"]) && Convert.ToDateTime(row["EarnedDate"]) <= System.DateTime.Now)
                        ((Telerik.Web.UI.RadDatePicker)(gridItem.FindControl("rdpEarnedDate"))).DbSelectedDate = Convert.ToDateTime(row["EarnedDate"]);
                    if (!Convert.IsDBNull(row["ExpirationDate"]))
                        ((Telerik.Web.UI.RadDatePicker)(gridItem.FindControl("rdpExpirationDate"))).DbSelectedDate = Convert.ToDateTime(row["ExpirationDate"]);
                }

                foreach (DataRow dr in row.Row.GetChildRows("CredentialUrls"))
                {
                    span.InnerHtml = span.InnerHtml + "<br/><a href='" + dr["URL"].ToString() + "' target='_blank' >" + dr["URL"].ToString() + "</a>";
                }
            }
        }

        protected void RadButtonUpdate_Click(object sender, EventArgs e)
        {
            var alignmentId = int.Parse(ddlAlignments.SelectedValue);
            var newSortXref = new drGeneric_3Strings();
            var schoolYrEarned = new drGeneric_3Strings();
            var credentialComments = new drGeneric_3Strings();
            newSortXref.Clear();
            schoolYrEarned.Clear();
            foreach (GridDataItem gridDataItem in gridStudentCredentials.Items)
            {
                if (gridDataItem is GridDataItem)
                {
                    CheckBox cbEarned = (CheckBox)gridDataItem.FindControl("chkEarned");
                    if (cbEarned.Checked)
                    {
                        newSortXref.Add(gridStudentCredentials.MasterTableView.DataKeyValues[gridDataItem.ItemIndex]["CredentialID"].ToString(),
                            (((Telerik.Web.UI.RadDatePicker)(gridDataItem.FindControl("rdpEarnedDate"))).DbSelectedDate == null ? null : ((Telerik.Web.UI.RadDatePicker)(gridDataItem.FindControl("rdpEarnedDate"))).DbSelectedDate.ToString()),
                            (((Telerik.Web.UI.RadDatePicker)(gridDataItem.FindControl("rdpExpirationDate"))).DbSelectedDate == null ? null : ((Telerik.Web.UI.RadDatePicker)(gridDataItem.FindControl("rdpExpirationDate"))).DbSelectedDate.ToString()));

                        schoolYrEarned.Add(gridStudentCredentials.MasterTableView.DataKeyValues[gridDataItem.ItemIndex]["CredentialID"].ToString()
                            , (gridDataItem.FindControl("cmbYear") as DropDownList).SelectedValue, ""
                            );

                        /*Add comment parameter here*/
                        string hashKey = _selectedStudent.ID.ToString() + "_" + (gridStudentCredentials.MasterTableView.DataKeyValues[gridDataItem.ItemIndex]["CredentialID"]).ToString();
                        DataSet ds = GetStudentComments(hashKey, Convert.ToInt32(gridStudentCredentials.MasterTableView.DataKeyValues[gridDataItem.ItemIndex]["ID"]), _selectedStudent.ID, System.Convert.ToInt32(gridStudentCredentials.MasterTableView.DataKeyValues[gridDataItem.ItemIndex]["CredentialID"]));
                        if (ds != null && ds.Tables.Count > 1)
                            foreach (DataRow dr in ds.Tables[1].Rows)
                            {
                                credentialComments.Add(dr["ID"].ToString(), dr["CredentialID"].ToString(), dr["CommentText"].ToString());
                            }
                    }
                }
            }

            foreach (string[] crdparam in newSortXref)
            {
                crdparam[2] = (crdparam[2] == null || crdparam[2] == "") ? "" : crdparam[2];
            }

            bool isSuccess = Thinkgate.Base.Classes.Credentials.SaveCredentialForStudent(_selectedStudent.ID, SessionObject.LoggedInUser.Page, newSortXref, credentialComments, schoolYrEarned, alignmentId);
            if (isSuccess)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateStudentsCredentials", "closeSaveWindow();", true);
            }
        }

        private DataSet GetStudentComments(string hashKey, int studentCredentialID, int studentID, int credentialID)
        {
            Hashtable hst = new Hashtable();
            DataSet ds = new DataSet();

            if (Session["hstCredentialComments"] != null)
            {
                hst = (Hashtable)Session["hstCredentialComments"];
            }

            if (hashKey != null && hst.Contains(hashKey))
            {
                ds = (DataSet)hst[hashKey];
            }
            else
            {
                ds = Base.Classes.Credentials.GetStudentCredentialsComment(studentID, credentialID);
                hst.Add(hashKey, ds);
                Session["hstCredentialComments"] = hst;
            }
            return ds;
        }

        protected void ddlEarned_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindPageControls();
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
            var studentID = GetDecryptedEntityId(X_ID);
            Doc theDoc = new Doc();
            //clear caching?
            theDoc.HtmlOptions.PageCacheEnabled = false;
            theDoc.HtmlOptions.UseNoCache = true;
            theDoc.HtmlOptions.PageCacheClear();
            theDoc.HtmlOptions.PageCachePurge();
            theDoc.HtmlOptions.UseResync = true;
            theDoc.Rect.String = "10 90 600 750";
            string hostURL = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host.ToString();
            string callUrl = ResolveUrl("~/Controls/Credentials/PDF/StudentCredentialPdf.aspx?xSID=" + studentID);
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

        protected void btnRefreshCredential_ServerClick(object sender, EventArgs e)
        {
            BindPageControls();
        }
    }
}