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
using CMS.CMSHelper;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Domain.Classes;

namespace Thinkgate
{
	public partial class ReferenceCenter : ExpandedSearchPage
	{
		
		TreeProvider treeProvider = null;
		DataTable table = null;
		List<LookupDetails> lookupData = null;
        public DistrictParms districtParm;
		

		#region Events Methods
		protected void Page_Load(object sender, EventArgs e)
		{
			districtParm =  DistrictParms.LoadDistrictParms();
            treeProvider = KenticoHelper.GetUserTreeProvider(SessionObject.LoggedInUser.ToString());
			Master.Search += SearchHandler;

			base.Page_Init(sender, e);

			if (!IsPostBack)
			{ InitializeCriteriaControls(); }

			btnAdd.Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Reference);
            string cmsTreePathToReferences = ConfigurationManager.AppSettings["CMSTreePathToReferences"];
            if (districtParm.isStateSystem)
                stateInitial.Value = districtParm.State.ToString();
            if (!string.IsNullOrWhiteSpace(cmsTreePathToReferences))
            {
                TreeNode tNode = treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, cmsTreePathToReferences.Substring(0, cmsTreePathToReferences.Length - 2), CMSContext.PreferredCultureCode);

                if (tNode != null)
                {
                    int messageCenterClassId = CMS.SettingsProvider.DataClassInfoProvider.GetDataClass("Thinkgate.ReferenceCenter").ClassID;
                    classId.Value = messageCenterClassId.ToString();
                    parentNodeId.Value = tNode.NodeID.ToString();
                    clientName.Value = districtParm.ClientID.ToString();
                }
            }
		}

		protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
		{
			ReferenceSearchCriteria referenceSearchCriteria = this.GetSelectedCriteria(this.Master.CurrentCriteria());
			PopulateGridWithCMSData(referenceSearchCriteria, "Title ASC");
		}

		protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item.ItemType == GridItemType.Header)
			{
				GridHeaderItem dataHeaderItem = e.Item as GridHeaderItem;

                if (UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Reference))                
                {
                    dataHeaderItem["EditButton"].Style.Add("Width", "25px");
                    dataHeaderItem["DeleteButton"].Style.Add("Width", "30px");
                }
                dataHeaderItem["Title"].Style.Add("Width", "200px");
				dataHeaderItem["EditButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Reference);
				dataHeaderItem["DeleteButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Reference);             
                

			}

			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				GridDataItem dataBoundItem = e.Item as GridDataItem;
				Reference reference = dataBoundItem.DataItem as Reference;

               		

				dataBoundItem["UserGroup"].Text = reference.UserGroup == 0 ? "" : lookupData.Where(l => l.Enum == reference.UserGroup).FirstOrDefault().Description;
				dataBoundItem["Component"].Text = reference.Component == 0 ? "" : lookupData.Where(l => l.Enum == reference.Component).FirstOrDefault().Description;
				dataBoundItem["CategoryList"].Text = reference.CategoryList == 0 ? "" : lookupData.Where(l => l.Enum == reference.CategoryList).FirstOrDefault().Description;
				dataBoundItem["FileTypes"].Text = reference.FileTypes == 0 ? "" : lookupData.Where(l => l.Enum == reference.FileTypes).FirstOrDefault().Description;

				dataBoundItem["Format"].Text = reference.Format;
				

				dataBoundItem["DateAdded"].Text = reference.DateAdded.ToShortDateString();
                if (UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Reference))
                {
                    dataBoundItem["EditButton"].Style.Add("Width", "25px");
                    dataBoundItem["DeleteButton"].Style.Add("Width", "30px");                    
                }
                dataBoundItem["Title"].Style.Add("Width", "200px");
				dataBoundItem["EditButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Reference);
				dataBoundItem["DeleteButton"].Visible = UserHasPermission(Thinkgate.Base.Enums.Permission.Add_Reference);

                
			}
		}

		protected void radGridResults_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			string sortDirection = "asc";
			if (e.NewSortOrder == GridSortOrder.Descending)
			{
				sortDirection = "desc";
			}
			ReferenceSearchCriteria referenceSearchCriteria = this.GetSelectedCriteria(this.Master.CurrentCriteria());
			PopulateGridWithCMSData(referenceSearchCriteria, e.SortExpression + " " + sortDirection);
		}

		protected void radGridResults_ItemCommand(object sender, GridCommandEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				if (e.CommandName.CompareTo("ReferenceDelete") == 0)
				{
					TreeNode document = treeProvider.SelectSingleDocument(DataIntegrity.ConvertToInt(e.CommandArgument));
					if (document != null)
					{
						document.Delete(false);
						ReferenceSearchCriteria referenceSearchCriteria = this.GetSelectedCriteria(this.Master.CurrentCriteria());
						PopulateGridWithCMSData(referenceSearchCriteria, "Title ASC");
					}
				}
			}
		}

		protected void exportGridImgBtn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{

			
                Master.RegisterScript();

                if (gridResultsPanel.Visible == true) // code added for TFS 9942 if Grid is visible then process further.
                {
                    var criteria = this.Master.CurrentCriteria();
                    if (criteria == null)
                    { return; }

                    ReferenceSearchCriteria referenceSearchCriteria = this.GetSelectedCriteria(criteria);
                    PopulateGridWithCMSData(referenceSearchCriteria, "Title ASC");

                    DataSet referencesDataSet = CMS.DocumentEngine.DocumentHelper.GetDocuments(ConfigurationManager.AppSettings["CMSSiteName"], ConfigurationManager.AppSettings["CMSTreePathToReferences"], "en-us", true, "Thinkgate.ReferenceCenter", referenceSearchCriteria.SearchCriteria, "Title ASC", 5, false, treeProvider);
                    if (referencesDataSet != null && referencesDataSet.Tables.Count > 0 && referencesDataSet.Tables[0].Rows.Count > 0)
                    {

                        DataTable dt = new DataTable();
                        table = referencesDataSet.Tables[0].Copy();

                        foreach (DataColumn col in referencesDataSet.Tables[0].Columns)
                        {
                            switch (col.ColumnName)
                            {
                                case "DocumentID":
                                    break;
                                case "DocumentNodeID":
                                    break;
                                case "AddNewReferenceCenterId":
                                    break;
                                case "UserGroup":
                                    break;
                                case "CategoryList":
                                    break;
                                case "Component":
                                    break;
                                case "FileTypes":
                                    break;
                                case "PostOn":
                                    break;
                                case "RemoveOn":
                                    break;
                                case "Title":
                                    break;
                                case "Keyword":
                                    break;
                                case "Description":
                                    break;
                                case "File":
                                    break;
                                case "DateAdded":
                                    break;
                                case "ClientTargets":
                                    break;
                                case "IsGlobal":
                                    break;
                            case "DocumentCreatedByUserID":
                                break;
                                default:
                                    table.Columns.Remove(col.ColumnName);
                                    break;


                            }
                        }


                        dt.Columns.Add("Title");
                        dt.Columns.Add("UserGroup");
                        dt.Columns.Add("Component");
                        dt.Columns.Add("Category");
                        dt.Columns.Add("Type");
                        dt.Columns.Add("Format");
                        dt.Columns.Add("DateAdded");


                        SessionObject obj = (SessionObject)Session["SessionObject"];
                    UserInfo userinfo = (UserInfo)Session["KenticoUserInfo"];
                        foreach (DataRow row in table.Rows)
                        {
                            bool isGlobal = false;
                            isGlobal = Convert.ToBoolean(row["IsGlobal"].ToString());

                        if (IsReferenceSearchable(row, obj, userinfo, isGlobal))
                            {
                            DataRow dataRow = dt.NewRow();
                            dataRow = (AddExportReference(row, dataRow));

                            if (dataRow != null)
                            { dt.Rows.Add(dataRow); }
                                        }
                                    }

                    if (dt.Rows.Count > 0)
                    { ExportToExcel(dt); }
                                    }

                            }

                        }

		DataRow AddExportReference(DataRow row, DataRow dr)
		{



                dr["Title"] = row["Title"].ToString();
                dr["UserGroup"] = Convert.ToInt32(row["UserGroup"]) == 0 ? "" : lookupData.Where(l => l.Enum == Convert.ToInt32(row["UserGroup"])).FirstOrDefault().Description;
                dr["Category"] = Convert.ToInt32(row["CategoryList"]) == 0 ? "" : lookupData.Where(l => l.Enum == Convert.ToInt32(row["CategoryList"])).FirstOrDefault().Description;
                dr["Component"] = Convert.ToInt32(row["Component"]) == 0 ? "" : lookupData.Where(l => l.Enum == Convert.ToInt32(row["Component"])).FirstOrDefault().Description;
                string Format = string.IsNullOrEmpty(row["File"].ToString()) ? "" : new CMS.DocumentEngine.AttachmentManager().GetAttachmentInfo(row["File"].ToString(), ConfigurationManager.AppSettings["CMSSiteName"]).AttachmentExtension; //DataIntegrity.ConvertToInt(row["Format"].ToString());                      
                dr["Format"] = GetFormat(Format.Replace(".", ""));
                dr["Type"] = Convert.ToInt32(row["FileTypes"]) == 0 ? "" : lookupData.Where(l => l.Enum == Convert.ToInt32(row["FileTypes"])).FirstOrDefault().Description;
                dr["DateAdded"] = row["DateAdded"].ToString();

                //Format Search Functionality
                CriteriaController criteriaController = this.Master.CurrentCriteria();

                List<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject> selectedCriteria = null;
                selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Format").ToList();
                if (selectedCriteria.Count > 0)
                {
                    if (GetFormat(Format.Replace(".", "")) == selectedCriteria[0].Text)
                    {
                        return dr;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {

                    return dr;
                }
            }
		

		#endregion

		#region Private Methods

		protected void ExportToExcel(DataTable dt)
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

		protected void InitializeCriteriaControls()
		{
            lookupData = KenticoHelper.GetLookupDetailsByClient(null, null);
			
			/* UserGroup criteria control */
			cblUserGroup.DataValueField = "Enum";
			cblUserGroup.DataTextField = "Description";
			cblUserGroup.EmptyMessage = "Select a User Group";

			SessionObject obj = (SessionObject)Session["SessionObject"];


			foreach (ThinkgateRole rol in obj.LoggedInUser.Roles)
			{
				if (rol.RoleName.ToLower() == "teacher")
				{
					cblUserGroup.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.UserGroup).ToList().Where(p => p.Enum == Convert.ToInt32(LookupDetail.Teacher) || p.Enum == Convert.ToInt32(LookupDetail.All) );
					break;
				}
				else
				{
					cblUserGroup.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.UserGroup);
					break;
				}
			}
			/* Category criteria control */
			cblCategory.DataValueField = "Enum";
			cblCategory.DataTextField = "Description";
			cblCategory.EmptyMessage = "Select a Category";
			cblCategory.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.Category);

			/* Component criteria control */
			cblComponent.DataValueField = "Enum";
			cblComponent.DataTextField = "Description";
			cblComponent.EmptyMessage = "Select a Component";
			cblComponent.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.Component);

			/* Type criteria control */
			cblType.DataValueField = "Enum";
			cblType.DataTextField = "Description";
			cblType.EmptyMessage = "Select a Type";
			cblType.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.Event);

			/* Format criteria control */
			cblFormat.DataValueField = "Enum";
			cblFormat.DataTextField = "Description";
			cblFormat.EmptyMessage = "Select a Format";
			cblFormat.DataSource = lookupData.FindAll(l => l.LookupEnum == LookupType.FileFormat);

			cblTextSearch.DataTextField = "Name";
			cblTextSearch.DataSource = TextSearchDropdownValues();
		}

		protected void PopulateGridWithCMSData(ReferenceSearchCriteria where, string orderBy)
		{
            lookupData = KenticoHelper.GetLookupDetailsByClient(null, null);
			List<Reference> references = SearchReferences(where, orderBy);
           
			//Format Search Functionality
			CriteriaController criteriaController = this.Master.CurrentCriteria();

			List<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject> selectedCriteria = null;
			selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Format").ToList();
			if (selectedCriteria.Count > 0)
			{

				references = references.Where(r => r.Format == selectedCriteria[0].Text).ToList();

			}

			radGridResults.DataSource = references;
			radGridResults.DataBind();

			gridResultsPanel.Visible = true;
			initMessage.Visible = false;
		}

		protected ReferenceSearchCriteria GetSelectedCriteria(CriteriaController criteriaController)
		{
			/* Get selected user group criteria value */
			ReferenceSearchCriteria referenceSearchCriteria = new ReferenceSearchCriteria();
			string documentSearchCriteria = string.Empty;
			List<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject> selectedCriteria = null;

			selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("UserGroup").ToList();
			if (selectedCriteria.Count > 0)
			{
                /* TFS 9069 In case of 'All' UserGroup option selected, the default search is be run. Skip condition in this case. */
                if (selectedCriteria[0].Value != Convert.ToInt32(LookupDetail.All).ToString() && selectedCriteria[0].Text != LookupDetail.All.ToString())
                {
                    documentSearchCriteria += " UserGroup = '" + selectedCriteria[0].Value + "' ";
                }
			}
			selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Category").ToList();
			if (selectedCriteria.Count > 0)
			{
				documentSearchCriteria += string.IsNullOrWhiteSpace(documentSearchCriteria) ? string.Empty : " AND ";
				documentSearchCriteria += " CategoryList = '" + selectedCriteria[0].Value + "' ";
			}
			selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Component").ToList();
			if (selectedCriteria.Count > 0)
			{
				documentSearchCriteria += string.IsNullOrWhiteSpace(documentSearchCriteria) ? string.Empty : " AND ";
				documentSearchCriteria += " Component = '" + selectedCriteria[0].Value + "' ";
			}
			selectedCriteria = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Type").ToList();
			if (selectedCriteria.Count > 0)
			{
				documentSearchCriteria += string.IsNullOrWhiteSpace(documentSearchCriteria) ? string.Empty : " AND ";
				documentSearchCriteria += " FileTypes = '" + selectedCriteria[0].Value + "' ";
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

					documentSearchCriteria += string.IsNullOrWhiteSpace(documentSearchCriteria) ? string.Empty : " AND ";

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
                documentSearchCriteria += string.IsNullOrWhiteSpace(documentSearchCriteria) ? string.Empty : " AND ";
               
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
			referenceSearchCriteria.SearchCriteria = documentSearchCriteria;
			return referenceSearchCriteria;
		}

		protected void SearchHandler(object sender, CriteriaController criteriaController)
		{

			ReferenceSearchCriteria referenceSearchCriteria = this.GetSelectedCriteria(criteriaController);

			PopulateGridWithCMSData(referenceSearchCriteria, "Title ASC");

		}

		protected List<Reference> SearchReferences(ReferenceSearchCriteria referenceSearchCriteria, string orderBy = "Title ASC")
		{
			
			/* Kentico related parameters like site name, username etc would go configurable */
			List<Reference> references = new List<Reference>();

			SessionObject obj = (SessionObject)Session["SessionObject"];
            UserInfo userinfo = (UserInfo)Session["KenticoUserInfo"];
			TreeNodeDataSet referencesDataSet = CMS.DocumentEngine.DocumentHelper.GetDocuments(ConfigurationManager.AppSettings["CMSSiteName"], ConfigurationManager.AppSettings["CMSTreePathToReferences"], "en-us", true, "Thinkgate.ReferenceCenter", referenceSearchCriteria.SearchCriteria, orderBy.ToString().ToUpper().Contains("FORMAT") ? "Title ASC" : orderBy, 5, false, treeProvider);
            
			if (referencesDataSet != null && referencesDataSet.Tables.Count > 0 && referencesDataSet.Tables[0].Rows.Count > 0)
			{
                List<TreeNode> treeNodes = referencesDataSet.ToList();
				table = referencesDataSet.Tables[0];
				foreach (DataRow row in referencesDataSet.Tables[0].Rows)
				{
                    TreeNode documentNode = treeNodes.Find(tn => tn.DocumentID == Convert.ToInt32(row["DocumentID"].ToString()));
					if (!string.IsNullOrEmpty(row["DocumentID"].ToString()))
					{
                      //  bool includeForAllClients = false;
                        bool isGlobal = false;
                      //  includeForAllClients =row["ClientTargets"].ToString().ToUpperInvariant() == "ALL";
                        isGlobal = Convert.ToBoolean(row["IsGlobal"].ToString());

                        if (IsReferenceSearchable(row, obj, userinfo, isGlobal))
                        { references.Add(AddReference(row, documentNode)); }
                        }
										}
                return references;
									}
            else
										{
                return (new List<Reference>());
										}
									}

        private bool IsReferenceSearchable(DataRow dataRow, SessionObject sessionObject, UserInfo userinfo, bool isGlobal)
										{
            Boolean isSearchable = false;

            //For Super Admin
            if (sessionObject.LoggedInUser.IsSuperAdmin)
            { isSearchable = true; }
            else if (userinfo.UserID == DataIntegrity.ConvertToInt(dataRow["DocumentCreatedByUserID"].ToString()))
            { isSearchable = true; }
									else
									{
                if (isGlobal || ifLEAListContainsLEA(dataRow["ClientTargets"].ToString(), districtParm.ClientID.ToString()) ||
                    (districtParm.isStateSystem && ifLEAListContainsLEAsInState(dataRow["ClientTargets"].ToString(), districtParm.State.ToString())))
								{
                    //For Teacher Role
                    if (sessionObject.LoggedInUser.Roles.Where(r => r.RoleName.ToLower() == "teacher").Count() > 0)
									{
                        if (DataIntegrity.ConvertToInt(dataRow["UserGroup"].ToString()) == (int)LookupDetail.All || DataIntegrity.ConvertToInt(dataRow["UserGroup"].ToString()) == (int)LookupDetail.Teacher)
                        { isSearchable = IsReferenceSearchableForUserRole(dataRow); }
									}
                    else// For Administrator and Other Roles.
                    { isSearchable = IsReferenceSearchableForUserRole(dataRow); }
								}
									}
            return isSearchable;
								}

        private bool IsReferenceSearchableForUserRole(DataRow dataRow)
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

		/// <summary>
		/// To Add record in reference list
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		Reference AddReference(DataRow row, TreeNode treeNode)
		{
			Reference reference = new Reference();

			reference.DocumentID = DataIntegrity.ConvertToInt(row["DocumentID"].ToString());
			reference.DocumentNodeID = DataIntegrity.ConvertToInt(row["DocumentNodeID"].ToString());

			reference.ReferenceID = DataIntegrity.ConvertToInt(row["AddNewReferenceCenterId"].ToString());
			reference.UserGroup = DataIntegrity.ConvertToInt(row["UserGroup"].ToString());
			reference.CategoryList = DataIntegrity.ConvertToInt(row["CategoryList"].ToString());
			reference.Component = DataIntegrity.ConvertToInt(row["Component"].ToString());
			reference.FileTypes = DataIntegrity.ConvertToInt(row["FileTypes"].ToString());
			reference.PostOn = DataIntegrity.ConvertToDate(row["PostOn"].ToString());
			reference.RemoveOn = DataIntegrity.ConvertToDate(row["RemoveOn"].ToString());
			reference.Title = row["Title"].ToString();
			reference.Keywords = row["Keyword"].ToString();
			reference.Description = row["Description"].ToString();

            if (!string.IsNullOrWhiteSpace(row["File"].ToString()))
            {
                if (treeNode.AllAttachments != null && treeNode.AllAttachments.Count > 0)
                {
                    AttachmentInfo attachmentInfo = (AttachmentInfo)treeNode.AllAttachments[0];
                    reference.FileAttachmentName = attachmentInfo.AttachmentName;
                    reference.Format = attachmentInfo.AttachmentExtension;
			reference.Format = GetFormat(reference.Format.Replace(".", ""));
                }
            }
			reference.DateAdded = DataIntegrity.ConvertToDate(row["DateAdded"].ToString());
			reference.NodeAlias = row["NodeAlias"].ToString();
			return reference;
		}

		string GetFormat(string AttachmentExtention)
		{
			if (AttachmentExtention == "")
				return string.Empty;
			string format = string.Empty;
			switch (AttachmentExtention.ToUpper())
			{
				case "DOC":
					format = "Word";
					break;
				case "DOCX":
					format = "Word";
					break;
				case "XLS":
					format = "Excel";
					break;
				case "XLSX":
					format = "Excel";
					break;
				case "PPT":
					format = "PowerPoint";
					break;
				case "PPTX":
					format = "PowerPoint";
					break;
				case "PDF":
					format = "PDF";
					break;
				default:
					format = GetImageVideoFormat(AttachmentExtention.ToUpper());
					break;
			}
			return format;
		}


		string GetImageVideoFormat(string AttachmentExtention)
		{
			string format = string.Empty;

			if (ConfigurationManager.AppSettings["CMSCriteriaImageFormats"].ToString().ToUpper().Contains(AttachmentExtention.ToUpper()))
			{
				format = "Images";
			}
			else if (ConfigurationManager.AppSettings["CMSCriteriaVideoFormats"].ToString().ToUpper().Contains(AttachmentExtention.ToUpper()))
			{
				format = "Videos";
			}
			else
			{
				format = "Others";
			}
			return format;
		}


		protected List<NameValue> TextSearchDropdownValues()
		{
			return new List<NameValue>
                {
                    new NameValue("Any Words", "any"),
                    new NameValue("All Words", "all"),
                    new NameValue("Exact Phrase", "exact"),
                    new NameValue("Keywords", "key"),
                };
		}

		protected static string TextSearchCondition(string[] searchWords, string[] searchColumns, bool anyWords)
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
	}

	public class Reference
	{
		public int DocumentID { get; set; }
		public int DocumentNodeID { get; set; }
		public int ReferenceID { get; set; }
		public int UserGroup { get; set; }
		public int Component { get; set; }
		public int CategoryList { get; set; }
		public int FileTypes { get; set; }
		public string Title { get; set; }
		public string Keywords { get; set; }
		public string Description { get; set; }
		public DateTime PostOn { get; set; }
		public DateTime RemoveOn { get; set; }
		public string FileAttachmentName { get; set; }
		public string Format { get; set; }
		public DateTime DateAdded { get; set; }
		public string NodeAlias { get; set; }
	}

	public class ReferenceSearchCriteria
	{
		public ReferenceSearchCriteria()
		{ }

		public ReferenceSearchCriteria(string criteria, string format)
			: this()
		{
			this.SearchCriteria = criteria;
		}

		public string SearchCriteria { get; set; }
		public string SearchFileFormat { get; set; }
	}
}
