
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Domain.Classes;
using CMS.CMSHelper;

namespace Thinkgate
{
	public partial class MessageCenter : ExpandedSearchPage
	{
		#region " Fields "

		public string PageGuid;        
		TreeProvider treeProvider = null;
		DataTable table = null;
		List<LookupDetails> lookupData = null;
        public DistrictParms districtParm;
		#endregion

		#region " Properties "

		public string CMSSiteUrl
		{
			get
			{
				if (ConfigurationManager.AppSettings["CMSSiteUrl"] == null)
				{
                    return string.Format("/{0}/", KenticoHelper.KenticoVirtualFolder);
				}
				return ConfigurationManager.AppSettings["CMSSiteUrl"];
			}
		}

		#endregion

		#region " Event Handlers "

		protected void Page_Load(object sender, EventArgs e)
		{
            districtParm = DistrictParms.LoadDistrictParms();
            treeProvider = KenticoHelper.GetUserTreeProvider(SessionObject.LoggedInUser.ToString());

			Master.Search += SearchHandler;
			base.Page_Init(sender, e);

			if (!IsPostBack)
			{            
				InitializeCriteriaControls();
			}

			btnAdd.Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
            string cmsTreePathToMessages = ConfigurationManager.AppSettings["CMSTreePathToMessages"];
            if (districtParm.isStateSystem)
                stateInitial.Value = districtParm.State.ToString();
            if (!string.IsNullOrWhiteSpace(cmsTreePathToMessages))
            {
                TreeNode parentNode = treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, cmsTreePathToMessages.Substring(0, cmsTreePathToMessages.Length - 2), CMSContext.PreferredCultureCode);
                if (parentNode != null)
                {
                    int messageCenterClassId = CMS.SettingsProvider.DataClassInfoProvider.GetDataClass("TG.MessageCenter").ClassID;
                    classId.Value = messageCenterClassId.ToString();
                    parentNodeId.Value = parentNode.NodeID.ToString();
                    clientName.Value = districtParm.ClientID.ToString();
                }
            }
		}

		protected void SearchHandler(object sender, CriteriaController criteriaController)
		{
			MessageSearchCriteria messageSearchCriteria = GetSelectedCriteria(criteriaController);
			PopulateGridWithCMSData(messageSearchCriteria, "PostOn DESC");
		}

		protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Header)
			{
				GridHeaderItem dataHeaderItem = e.Item as GridHeaderItem;

                dataHeaderItem["EditButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
                dataHeaderItem["DeleteButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
                dataHeaderItem["PostOn"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
                dataHeaderItem["RemoveOn"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message); 
                

			}
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				GridDataItem dataBoundItem = e.Item as GridDataItem;
				Thinkgate.Base.Classes.MessageCenter message = dataBoundItem.DataItem as Thinkgate.Base.Classes.MessageCenter;

				dataBoundItem["UserGroupEnum"].Text = message.UserGroupEnum == 0 ? "" : lookupData.Where(l => l.Enum == message.UserGroupEnum).FirstOrDefault().Description;
				dataBoundItem["MessageCenterEnum"].Text = message.MessageCenterEnum == 0 ? "" : lookupData.Where(l => l.Enum == (int)message.MessageCenterEnum).FirstOrDefault().Description;

                dataBoundItem["DateAdded"].Text = message.DateAdded.HasValue == false ? string.Empty : message.DateAdded.Value.ToShortDateString();
                dataBoundItem["PostOn"].Text = message.PostOn.HasValue == false ? string.Empty : message.PostOn.Value.ToShortDateString();
                dataBoundItem["RemoveOn"].Text = message.RemoveOn.HasValue == false ? string.Empty : message.RemoveOn.Value.ToShortDateString();

                dataBoundItem["EditButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
                dataBoundItem["DeleteButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
                dataBoundItem["PostOn"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
                dataBoundItem["RemoveOn"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);

                System.Web.UI.HtmlControls.HtmlAnchor EditDocument = (System.Web.UI.HtmlControls.HtmlAnchor)dataBoundItem["EditButton"].FindControl("EditDocumentImg");
                EditDocument.Attributes.Add("onclick","editMessage('" + message.DocumentNodeID  + "')");
                if (Session["KenticoUserInfo"] != null)
                {
                    UserInfo userinfo = (UserInfo)Session["KenticoUserInfo"];
                    if (!userinfo.IsInRole(KenticoRole.UserAgreement.ToString(), CMSContext.CurrentSiteName))
                    {
                        if (message.MessageCenterEnum == LookupDetail.UserAgreement)
                        {
                            
                            EditDocument.Style.Add("display", "none");

                            System.Web.UI.WebControls.ImageButton DeleteDcoument = (System.Web.UI.WebControls.ImageButton)dataBoundItem["DeleteButton"].FindControl("DeleteDocument");
                            DeleteDcoument.Style.Add("display", "none");
                        }
                    }
                }
			}
		}

		protected void radGridResults_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			string sortDirection = "asc";
			if (e.NewSortOrder == GridSortOrder.Descending)
			{
				sortDirection = "desc";
			}
			MessageSearchCriteria messageSearchCriteria = this.GetSelectedCriteria(this.Master.CurrentCriteria());
			PopulateGridWithCMSData(messageSearchCriteria, e.SortExpression + " " + sortDirection);
		}

		protected void radGridResults_ItemCommand(object sender, GridCommandEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				if (e.CommandName.CompareTo("MessageDelete") == 0)
				{
					TreeNode document = treeProvider.SelectSingleDocument(DataIntegrity.ConvertToInt(e.CommandArgument));
					if (document != null)
					{
						document.Delete(false);
						MessageSearchCriteria messageSearchCriteria = this.GetSelectedCriteria(this.Master.CurrentCriteria());
						PopulateGridWithCMSData(messageSearchCriteria, "PostOn DESC");
					}
				}
			}
		}

		public void ExportToExcel(DataTable dt)
		{
			XLWorkbook workbook = Master.ConvertDataTableToSingleSheetWorkBook(dt, "Results");
			using (MemoryStream memoryStream = new MemoryStream())
			{
				workbook.SaveAs(memoryStream);
				Response.Clear();
				Response.Buffer = true;
				Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");
				Response.BinaryWrite(memoryStream.ToArray());
				Response.End();
			}
		}

		protected void ExportGridImgBtn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			
                Master.RegisterScript();
                if (gridResultsPanel.Visible == true) // code added for TFS 9942 if Grid is visible then process further.
                {
                    MessageSearchCriteria messageSearchCriteria;
                    var criteria = this.Master.CurrentCriteria();
                    if (criteria == null)
                    {
                        messageSearchCriteria = new MessageSearchCriteria();
                        messageSearchCriteria.SearchCriteria = "";
                    }
                    else
                    { messageSearchCriteria = this.GetSelectedCriteria(criteria); }


                    SessionObject obj = (SessionObject)Session["SessionObject"];
                UserInfo userinfo = (UserInfo)Session["KenticoUserInfo"];
                    DataSet messagesDataSet = CMS.DocumentEngine.DocumentHelper.GetDocuments(ConfigurationManager.AppSettings["CMSSiteName"], ConfigurationManager.AppSettings["CMsTreePathToMessages"], "en-us", true, "TG.MessageCenter", messageSearchCriteria.SearchCriteria, "PostOn DESC", 5, false, treeProvider);

                    if (messagesDataSet != null && messagesDataSet.Tables.Count > 0 && messagesDataSet.Tables[0].Rows.Count > 0)
                    {
                        lookupData = KenticoHelper.GetLookupDetailsByClient(null, null);
                        using (DataTable dt = new DataTable())
                        {
                            table = messagesDataSet.Tables[0].Copy();
                            bool addMessagePermission = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
                            foreach (DataColumn col in messagesDataSet.Tables[0].Columns)
                            {
                                switch (col.ColumnName)
                                {
                                    case "Title":
                                    case "UserGroupEnum":
                                    case "MessageCenterEnum":
                                    case "DateAdded":
                                    case "PostOn":
                                    case "RemoveOn":
                                    case "ClientTargets":
                                case "DocumentCreatedByUserID":
                                        break;
                                    default:
                                        table.Columns.Remove(col.ColumnName);
                                        break;
                                }
                            }

                            dt.Columns.Add("Title");
                            dt.Columns.Add("UserGroup");
                            dt.Columns.Add("Type");
                            dt.Columns.Add("DateAdded");
                            if (addMessagePermission)
                            {
                                dt.Columns.Add("PostOn");
                                dt.Columns.Add("RemoveOn");
                            }
                            foreach (DataRow row in table.Rows)
                            {
                                DataRow dr = dt.NewRow();


                                dr["Title"] = row["Title"].ToString();

                                if (string.IsNullOrWhiteSpace(row["UserGroupEnum"].ToString()))
                                { dr["UserGroup"] = string.Empty; }
                                else if (row["UserGroupEnum"].ToString() == "0")
                                { dr["UserGroup"] = string.Empty; }
                                else
                                { dr["UserGroup"] = lookupData.Where(l => l.Enum == DataIntegrity.ConvertToInt(row["UserGroupEnum"].ToString())).FirstOrDefault().Description; }

                                dr["Type"] = string.IsNullOrWhiteSpace(row["MessageCenterEnum"].ToString()) ? string.Empty : lookupData.Where(l => l.Enum == DataIntegrity.ConvertToInt(row["MessageCenterEnum"].ToString())).FirstOrDefault().Description;

                                if (row["DateAdded"] != null && row["DateAdded"] != DBNull.Value && DateTime.Parse(row["DateAdded"].ToString()) != DateTime.MinValue)
                                    dr["DateAdded"] = DataIntegrity.ConvertToDate(row["DateAdded"].ToString()).ToShortDateString();
                                if (addMessagePermission)
                                {
                                    if (row["PostOn"] != null && row["PostOn"] != DBNull.Value && DateTime.Parse(row["PostOn"].ToString()) != DateTime.MinValue)
                                        dr["PostOn"] = DataIntegrity.ConvertToDate(row["PostOn"].ToString()).ToShortDateString();
                                    else
                                        dr["PostOn"] = string.Empty;
                                    if (row["RemoveOn"] != null && row["RemoveOn"] != DBNull.Value && DateTime.Parse(row["RemoveOn"].ToString()) != DateTime.MinValue)
                                        dr["RemoveOn"] = DataIntegrity.ConvertToDate(row["RemoveOn"].ToString()).ToShortDateString();
                                    else
                                        dr["RemoveOn"] = string.Empty;
                                }

                            if (IsMessageSearchable(row, obj, userinfo))
                            { dt.Rows.Add(dr); }
                                }
                        if (dt.Rows.Count > 0)
                        { ExportToExcel(dt); }
                                                }
                                            }
                                        }
                                            }

			
		
		protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
		{
			MessageSearchCriteria messageSearchCriteria = this.GetSelectedCriteria(this.Master.CurrentCriteria());
			PopulateGridWithCMSData(messageSearchCriteria, "PostOn DESC");
		}

		#endregion

		#region " Criteria Controls Initialization  "

		/// <summary>
		/// Populate criteria controls
		/// </summary>
		void InitializeCriteriaControls()
		{
            lookupData = KenticoHelper.GetLookupDetailsByClient(null, null);

			/* UserGroup criteria control */
			cblUserGroup.DataValueField = "Enum";
			cblUserGroup.DataTextField = "Description";
			cblUserGroup.EmptyMessage = "Select a User Group";

            if (SessionObject.LoggedInUser.Roles != null && SessionObject.LoggedInUser.Roles.Count > 0)
            {
                RolePortal _roleportalID = (RolePortal)(SessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection));
                if (_roleportalID == RolePortal.District || _roleportalID == RolePortal.School)
                {
                    cblUserGroup.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.UserGroup);

                }
                else if (_roleportalID == RolePortal.Teacher)
                {
                    cblUserGroup.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.UserGroup).ToList().Where(p => p.Enum == (int)LookupDetail.All || p.Enum == (int)LookupDetail.Teacher);
                }
            }
            

			/* Type criteria control */
            cblType.DataValueField = "Enum";
			cblType.DataTextField = "Description";
			cblType.EmptyMessage = "Select a Type";
            cblType.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.MessageCenter);
          

			cblTextSearch.DataTextField = "Name";
			cblTextSearch.DataSource = TextSearchDropdownValues();

			bool addMessagePermission = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Message);
			if (!addMessagePermission)
			{
				cblPostOn.Visible = false;
				cblRemoveOn.Visible = false;
			}
		}

		List<NameValue> TextSearchDropdownValues()
		{
			return new List<NameValue>
                {
                    new NameValue("Any Words", "any"),
                    new NameValue("All Words", "all"),
                    new NameValue("Exact Phrase", "exact"),
                    new NameValue("Keywords", "key"),
                };
		}

		/// <summary>
		/// Gets criteria lookup data from Kentico.
		/// </summary>
		/// <returns></returns>
        //List<LookupDetails> GetLookupDetailsFromCMS()
        //{
        //    List<LookupDetails> lookupData = new List<LookupDetails>();
        //    DataClassInfo customTable = DataClassInfoProvider.GetDataClass("TG.LookupDetails");
        //    if (customTable != null)
        //    {
        //        string kenticoUserName = KenticoHelper.getKenticoUser(SessionObject.LoggedInUser.ToString());
        //        UserInfo userInfo = UserInfoProvider.GetUserInfo(kenticoUserName);

        //        DataSet lookupDataSet = (new CMS.SiteProvider.CustomTableItemProvider(userInfo)).GetItems("TG.LookupDetails", string.Empty, string.Empty);
        //        if (lookupDataSet != null && lookupDataSet.Tables.Count > 0 && lookupDataSet.Tables[0].Rows.Count > 0)
        //        {
        //            foreach (DataRow row in lookupDataSet.Tables[0].Rows)
        //            {
        //                LookupDetails lookupTableEntity = new LookupDetails();

        //                lookupTableEntity.ID = DataIntegrity.ConvertToInt(row["ItemID"]);
        //                lookupTableEntity.Description = row["Description"].ToString();
        //                lookupTableEntity.Enum = DataIntegrity.ConvertToInt(row["Enum"]);
        //                lookupTableEntity.LookupEnum = (LookupType)Enum.Parse(typeof(LookupType), row["LookupEnum"].ToString());

        //                lookupData.Add(lookupTableEntity);
        //            }
        //            return lookupData;
        //        }
        //    }
        //    return null;
        //}

		#endregion

		#region " Extract criteria controls criteria as WHERE condition "

		/// <summary>
		/// Should be moved to BL
		/// </summary>
		/// <param name="criteriaController"></param>
		/// <returns></returns>
		MessageSearchCriteria GetSelectedCriteria(CriteriaController criteriaController)
		{
			/* Get selected user group criteria value */
			MessageSearchCriteria messageSearchCriteria = new MessageSearchCriteria();
			string documentSearchCriteria = string.Empty;
			List<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject> selectedCriteria = null;

			selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("UserGroup").ToList();
			if (selectedCriteria.Count > 0)
			{
				documentSearchCriteria += " UserGroupEnum = '" + selectedCriteria[0].Value + "' ";
			}

           

			selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Type").ToList();
            if (selectedCriteria.Count > 0)
            {
                documentSearchCriteria = AppendSQLClause(documentSearchCriteria);
                documentSearchCriteria += " MessageCenterEnum = '" + selectedCriteria[0].Value + "' ";
            }
            

			string searchText = string.Empty;
			string searchOption = string.Empty;
			List<TextWithDropdown.ValueObject> textSearchList = criteriaController.ParseCriteria<TextWithDropdown.ValueObject>("TextSearch");
			if (textSearchList.Count > 0)
			{
				NameValue confirmedOption = TextSearchDropdownValues().Find(x => x.Name == textSearchList[0].Option) ?? TextSearchDropdownValues().First();
				if (!String.IsNullOrEmpty(textSearchList[0].Text))
				{
					searchText = textSearchList[0].Text;
					searchOption = confirmedOption.Value;
					string[] searchWords = System.Text.RegularExpressions.Regex.Split(searchText, " ");
					string[] searchColumns = new string[] { "Title", "Description", "Keyword" };

					documentSearchCriteria = AppendSQLClause(documentSearchCriteria);

					if (searchOption == "key")
					{
						documentSearchCriteria += " (Keyword LIKE '%" + searchText + "%') ";
					}
					else if (searchOption == "any")
					{
						string anyWordsCondition = TextSearchCondition(searchWords, searchColumns, true);
						documentSearchCriteria += anyWordsCondition;
					}
					else if (searchOption == "all")
					{
						string allWordsCondition = TextSearchCondition(searchWords, searchColumns, false);
						documentSearchCriteria += allWordsCondition;
					}
					else if (searchOption == "exact")
					{
						documentSearchCriteria += " ( Title LIKE '%" + searchText + "%' ";
						documentSearchCriteria += " OR Keyword LIKE '%" + searchText + "%' ";
						documentSearchCriteria += " OR Description LIKE '%" + searchText + "%') ";
					}
				}
			}
			var dateSelected = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DateRange.ValueObject>("DateAdded").ToList();
			if (dateSelected.Count > 0)
			{
                documentSearchCriteria = AppendSQLClause(documentSearchCriteria);
                Master.ChangeCriteriaOrder(ref dateSelected);
                if (dateSelected[0].Type == "Start")
                {                    
                    documentSearchCriteria += " (convert(varchar,DateAdded,111) >= convert(datetime,'" + dateSelected[0].Date + "',111) ";
                    if (dateSelected.Count > 1 && !string.IsNullOrEmpty(dateSelected[1].Date))
                        documentSearchCriteria += " And convert(varchar,DateAdded,111) <= convert(datetime,'" + dateSelected[1].Date + "',111)) ";
                    else
                        documentSearchCriteria += ")";
                }
                else if (!string.IsNullOrEmpty(dateSelected[0].Date))
                { documentSearchCriteria += " (convert(varchar,DateAdded,111) <= convert(datetime,'" + dateSelected[0].Date + "',111))"; }
			}
			var dateSelected1 = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DateRange.ValueObject>("PostOn").ToList();
			if (dateSelected1.Count > 0)
			{
                documentSearchCriteria = AppendSQLClause(documentSearchCriteria);
                Master.ChangeCriteriaOrder(ref dateSelected1);
                if (dateSelected1[0].Type == "Start")
                {
                    documentSearchCriteria += " (convert(varchar,PostOn,111) >= convert(datetime,'" + dateSelected1[0].Date + "',111) ";
                    if (dateSelected1.Count > 1 && !string.IsNullOrEmpty(dateSelected1[1].Date))
                        documentSearchCriteria += " And PostOn <= convert(datetime,'" + dateSelected1[1].Date + "',111)) ";
                    else
                        documentSearchCriteria += ")";
                }
                else if(!string.IsNullOrEmpty(dateSelected1[0].Date))
                { documentSearchCriteria += " (convert(varchar,PostOn,111) <=  convert(datetime,'" + dateSelected1[0].Date + "',111))"; }
			}
			var dateSelected2 = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DateRange.ValueObject>("RemoveOn").ToList();
			if (dateSelected2.Count > 0)
			{
                documentSearchCriteria = AppendSQLClause(documentSearchCriteria);
                Master.ChangeCriteriaOrder(ref dateSelected2);
                if (dateSelected2[0].Type == "Start")
                {
                    documentSearchCriteria += " (convert(varchar,RemoveOn,111) >= convert(datetime,'" + dateSelected2[0].Date + "',111) ";
                    if (dateSelected2.Count > 1 && !string.IsNullOrEmpty(dateSelected2[1].Date))
                        documentSearchCriteria += " And convert(varchar,RemoveOn,111) <= convert(datetime,'" + dateSelected2[1].Date + "',111)) ";                    
                    else    
                        documentSearchCriteria += ")";
                }
                else if (!string.IsNullOrEmpty(dateSelected2[0].Date))
                { documentSearchCriteria += " (convert(varchar,RemoveOn,111) <= convert(datetime,'" + dateSelected2[0].Date + "',111))";}
			}
            if (!string.IsNullOrEmpty(documentSearchCriteria))
            {
                string lastString = documentSearchCriteria.Trim().Substring(documentSearchCriteria.Trim().Length - 3);
                documentSearchCriteria = lastString.ToUpper() == "AND" ? documentSearchCriteria.Trim().Substring(0, documentSearchCriteria.Trim().Length - 3) : documentSearchCriteria;
            }
			messageSearchCriteria.SearchCriteria = documentSearchCriteria;
			return messageSearchCriteria;
		}

		string AppendSQLClause(string documentSearchCriteria)
		{
			documentSearchCriteria += string.IsNullOrWhiteSpace(documentSearchCriteria) ? string.Empty : " AND ";
			return documentSearchCriteria;
		}

		private static string TextSearchCondition(string[] searchWords, string[] searchColumns, bool anyWords)
		{
			StringBuilder perColumnCondition = new StringBuilder(" (");
			for (int j = 0; j < searchColumns.Length; j++)
			{
				StringBuilder perWordCondition = new StringBuilder(" (");
				for (int i = 0; i < searchWords.Length; i++)
				{
					if (i > 0)
					{
						perWordCondition.Append(anyWords == true ? " OR " : " AND ");
					}
					perWordCondition
						.Append(" ")
						.Append(searchColumns[j])
						.Append(" LIKE '%")
						.Append(searchWords[i])
						.Append("%' ");
				}
				perWordCondition.Append(") ");
				if (j > 0)
					perColumnCondition.Append(" OR ").Append(perWordCondition);
				else
					perColumnCondition.Append(perWordCondition);
			}
			perColumnCondition.Append(") ");
			return perColumnCondition.ToString();
		}

		#endregion

		#region " Populate Grid With CMS Data "

		private void PopulateGridWithCMSData(MessageSearchCriteria where, string orderBy)
		{
            lookupData = KenticoHelper.GetLookupDetailsByClient(null, null);
			List<Thinkgate.Base.Classes.MessageCenter> messages = SearchMessages(where, orderBy);
                      
			radGridResults.DataSource = messages;
			radGridResults.DataBind();

			gridResultsPanel.Visible = true;
			initMessage.Visible = false;
		}

		/// <summary>
		/// To be moved to DAL
		/// </summary>
		/// <param name="referenceSearchCriteria"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		List<Thinkgate.Base.Classes.MessageCenter> SearchMessages(MessageSearchCriteria messageSearchCriteria, string orderBy = "PostOn DESC")
		{
			
			SessionObject obj = (SessionObject)Session["SessionObject"];
            UserInfo userinfo = (UserInfo)Session["KenticoUserInfo"];
			/* Kentico related parameters like site name, username etc would go configurable */
			List<Thinkgate.Base.Classes.MessageCenter> messages = new List<Thinkgate.Base.Classes.MessageCenter>();
			TreeNodeDataSet messagesDataSet = CMS.DocumentEngine.DocumentHelper.GetDocuments(ConfigurationManager.AppSettings["CMSSiteName"], ConfigurationManager.AppSettings["CMSTreePathToMessages"], "en-us", true, "TG.MessageCenter", messageSearchCriteria.SearchCriteria, orderBy, 5, false, treeProvider);
			if (messagesDataSet != null && messagesDataSet.Tables.Count > 0 && messagesDataSet.Tables[0].Rows.Count > 0)
			{
				table = messagesDataSet.Tables[0];
				bool hasPermission = UserHasPermission(Permission.Add_Message);
                bool includeForAllClients = false;
				for (int i = 0; i < messagesDataSet.Tables[0].Rows.Count; i++)
				{
					DataRow row = messagesDataSet.Tables[0].Rows[i];
					if (!string.IsNullOrEmpty(row["DocumentID"].ToString()))
					{
                        if (IsMessageSearchable(row, obj, userinfo))
                        { messages.Add(AddMessage(row)); }
											}
										}
				return messages;
			}
			else
			{
				return (new List<Thinkgate.Base.Classes.MessageCenter>());
			}
		}

		private Thinkgate.Base.Classes.MessageCenter AddMessage(DataRow row)
		{
			Thinkgate.Base.Classes.MessageCenter message = new Thinkgate.Base.Classes.MessageCenter();

			message.DocumentID = DataIntegrity.ConvertToInt(row["DocumentID"].ToString());
			message.DocumentNodeID = DataIntegrity.ConvertToInt(row["DocumentNodeID"].ToString());

			message.UserGroupEnum = DataIntegrity.ConvertToInt(row["UserGroupEnum"].ToString());
			message.MessageCenterEnum = (Thinkgate.Base.Enums.LookupDetail)Enum.Parse(typeof(Thinkgate.Base.Enums.LookupDetail), row["MessageCenterEnum"].ToString());
            if (row["DateAdded"] != DBNull.Value)
				message.DateAdded = DataIntegrity.ConvertToDate(row["DateAdded"].ToString());
            if (row["PostOn"] != DBNull.Value)
				message.PostOn = DataIntegrity.ConvertToDate(row["PostOn"].ToString());
            if (row["RemoveOn"] != DBNull.Value)
				message.RemoveOn = DataIntegrity.ConvertToDate(row["RemoveOn"].ToString());
			message.Title = row["Title"].ToString();
			message.Keywords = row["Keyword"].ToString();
			message.Description = row["Description"].ToString();
			message.NodeAlias = row["NodeAlias"].ToString();

			return message;
		}

        private bool IsMessageSearchable(DataRow dataRow, SessionObject sessionObject, UserInfo userinfo)
        {
            Boolean isSearchable = false;

            //For Super Admin
            if (sessionObject.LoggedInUser.IsSuperAdmin)
            { isSearchable = true; }
            else if (userinfo.UserID == DataIntegrity.ConvertToInt(dataRow["DocumentCreatedByUserID"].ToString()))
            { isSearchable = true; }
            else
            {
                if (ifLEAListContainsLEA(dataRow["ClientTargets"].ToString(), districtParm.ClientID.ToString()) ||
                    (districtParm.isStateSystem && ifLEAListContainsLEAsInState(dataRow["ClientTargets"].ToString(), districtParm.State.ToString())))
                {
                    //For Teacher Role
                    if (sessionObject.LoggedInUser.Roles.Where(r => r.RoleName.ToLower() == "teacher").Count() > 0)
                    {
                        if (DataIntegrity.ConvertToInt(dataRow["UserGroupEnum"].ToString()) == (int)LookupDetail.All || DataIntegrity.ConvertToInt(dataRow["UserGroupEnum"].ToString()) == (int)LookupDetail.Teacher)
                        { isSearchable = IsMessageSearchableForUserRole(dataRow); }
                    }
                    else// For Administrator and Other Roles.
                    { isSearchable = IsMessageSearchableForUserRole(dataRow); }
                }
            }
            return isSearchable;
        }

        private bool IsMessageSearchableForUserRole(DataRow dataRow)
        {
            Boolean isSearchable = false;

            if (!string.IsNullOrEmpty(dataRow["PostOn"].ToString()) && !string.IsNullOrEmpty(dataRow["RemoveOn"].ToString()))
            {
                if (DateTime.Now.Date >= DataIntegrity.ConvertToDate(dataRow["PostOn"].ToString()).Date && DateTime.Now.Date <= DataIntegrity.ConvertToDate(dataRow["RemoveOn"].ToString()).Date)
                { isSearchable = true; }
            }
            else if (!string.IsNullOrEmpty(dataRow["PostOn"].ToString()))
            {
                if (DateTime.Now.Date >= DataIntegrity.ConvertToDate(dataRow["PostOn"].ToString()).Date)
                { isSearchable = true; }
            }
            else if (!string.IsNullOrEmpty(dataRow["RemoveOn"].ToString()))
            {
                if (DateTime.Now.Date <= DataIntegrity.ConvertToDate(dataRow["RemoveOn"].ToString()).Date)
                { isSearchable = true; }
            }
            else
            { isSearchable = true; }

            return isSearchable;
        }

		#endregion

		#region " Kentico DTOs (to be moved to corresponding project...) "

		public class MessageSearchCriteria
		{
			public MessageSearchCriteria()
			{ }

			public MessageSearchCriteria(string criteria, string format)
				: this()
			{
				this.SearchCriteria = criteria;
			}

			public string SearchCriteria { get; set; }
			public string SearchFileFormat { get; set; }
		}

		#endregion

	}
}

