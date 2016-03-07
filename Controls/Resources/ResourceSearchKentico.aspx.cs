using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using DocumentFormat.OpenXml.Bibliography;
using Standpoint.Core.ExtensionMethods;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Classes.Search;
using Thinkgate.Domain.Classes;
using Thinkgate.Domain.Classes.Review;
using Thinkgate.Services.Contracts.EsQueryService;
using Thinkgate.Services.Contracts.LearningMedia;
using Criteria = Thinkgate.Classes.Search.Criteria;
using Criterion = Thinkgate.Classes.Search.Criterion;
using CultureInfo = System.Globalization.CultureInfo;
using Resource = Thinkgate.Base.Classes.Resource;
using System.Web.Script.Serialization;
using Thinkgate.Services.Contracts.Shared;
using Thinkgate.Domain.Facades;
using System.Web.Security;

namespace Thinkgate.Controls.Resources
{
    public partial class ResourceSearchKentico : BasePage
	{
		public struct CmsCustomTable
		{
			public int ItemId { get; set; }
			public int DocId { get; set; }
			public int Id { get; set; }
			public int ReferenceId { get; set; }
			public string DocumentName { get; set; }
		}

        private ReviewFacade _reviewFacade;
       

		private const string IpClassName = "thinkgate.InstructionalPlan";
		private const string UpClassName = "thinkgate.UnitPlan";
		private const string LpClassName = "thinkgate.LessonPlan";
		private const string CuClassName = "thinkgate.curriculumUnit";
		private const string ResourceClassName = "thinkgate.resource";
		private const string FORWARD_SLASH = "/";

		private const string NationalLearningRegistry = "National Learning Registry";
		private const string PBSLearningMedia = "PBS Learning Media";
		private const int NationalLearningRegistryMaxResults = 100;

		private const int TagInstructionsType = 1;
		private const int TagActivityType = 2;
		private const int TagLearningResourceType = 3;
		private const int TagTargetAudience = 4;

		private string _typeKey;
		protected bool _Perm_Resource_Link_Ok;
		private List<LookupDetails> resourceTypes = null;

		protected bool PermResourceLinkOk;

		private readonly string _rootConnectionString =
			ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;

		private readonly string _cmsConnectionString =
			ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

		private List<LookupDetails> _resourceTypes;

		private const string District = "District";
		private const string State = "State";
		private const string MyDocuments = "MyDocs";
		private const string InternalSource = "Internal Source";
        
        private const string TeacherMyDocs = "TeachersMyDocsfolder";
        //US15667
        private const string SharedDocument = "Shared";

		private System.Text.StringBuilder _districts;
		private System.Text.StringBuilder _stateDocuments;
        //US15667
        private System.Text.StringBuilder _sharedDocuments;

		private SessionObject _sessionObject;
        private bool filterUsageRightExpiredContent;
        private bool isFilteredResourceEmpty;
        private string _selectedStandardId;
        private string _from;
        private Base.Classes.Standards _standard = null;
        public bool DefaultRun { get; set; }

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            _reviewFacade = new ReviewFacade(AppSettings.ConnectionString);
        }

        public bool isInstructionMaterial = false;

        private const string ROLE_DISTRICTADMIN = "District Administrator";
        private const string ROLE_SCHOOLADMIN = "School Administrator";
        private const string ROLE_TEACHER = "Teacher";
        private const string TeacherMyDocsFolder = "Teacher (My Docs folder)";


        public int ClassID = 0;
        public int GroupID = 0;

        protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
			CMSContext.Init();
            }
            isFilteredResourceEmpty = false;
			_typeKey = Request.QueryString["type"] ?? "Resources";


            if (Request.QueryString["isInstructionMaterial"] != null && Request.QueryString["isInstructionMaterial"] == "1")
            {
                isInstructionMaterial = true;
                Page.Title = "Instruction Search";
            }
            else
            {
                isInstructionMaterial = false;
                Page.Title = "Resource Search";
            }

            _from = Request.QueryString["from"] ?? string.Empty;
            _selectedStandardId = Request.QueryString["standardID"] ?? string.Empty;
            filterUsageRightExpiredContent = !UserHasPermission(Permission.AllowViewForIMUsageRightExpiredContent) && !UserHasPermission(Permission.AllowEditForIMUsageRightExpiredContent);

			if (_sessionObject == null)
			{
				_sessionObject = (SessionObject)Session["SessionObject"];
                ClassID = _sessionObject.clickedClass != null ? _sessionObject.clickedClass.ID : 0;
                GroupID = _sessionObject.clickedGroup != null ? _sessionObject.clickedGroup.ID : 0;
			}

			if (DistrictParms.LoadDistrictParms().LRMITagging != "true")
			{
				tabLRMI.Visible = false;
			}

			if (!IsPostBack)
			{
				LoadCriteria();
			}

            if (_from.Equals("suggestedresources", StringComparison.InvariantCultureIgnoreCase))
            {
                DefaultRun = true;
            }
            
		}

		protected void btnGetValue_Click(object sender, EventArgs e)
		{
			ResetGrid();
			List<Resource> lstResource = SearchResourcesWithCriteria(true);
			if (radGridResults.CurrentPageIndex > 0) { radGridResults.CurrentPageIndex = 0; }
			radGridResults.DataSource = lstResource;
			radGridResults.DataBind();
            radGridResults.Rebind();
			UpdatePanelSearchResults.Update();
		}

		/// <summary>
		/// Get Standard data from thinkgate.docToStandards table.
		/// </summary>
		/// <returns></returns>
		protected List<CmsCustomTable> GetDocToStandardsFromCms(UserInfo userInfo, string standardId)
		{
			DataClassInfo customTable = DataClassInfoProvider.GetDataClass("thinkgate.docToStandards");
			if (customTable != null)
			{
				DataSet standardDataSet = (new CustomTableItemProvider(userInfo)).GetItems("thinkgate.docToStandards",
					"standardID IN (" + standardId + ")", string.Empty);
                if (standardDataSet != null && standardDataSet.Tables.Count > 0 &&
                    standardDataSet.Tables[0].Rows.Count > 0)
				{

					List<CmsCustomTable> lstResult = (from s in standardDataSet.Tables[0].AsEnumerable()
													  select
														  new CmsCustomTable
														  {
															  ItemId = Convert.ToInt32(s["ItemID"]),
															  DocId = Convert.ToInt32(s["docID"]),
															  Id = Convert.ToInt32(s["standardID"])
														  }).ToList<CmsCustomTable>();

					return lstResult;

				}
			}
			return null;
		}

		/// <summary>
		/// Get Standard data from thinkgate.docToCurriculums table.
		/// </summary>
		/// <returns></returns>
		protected List<CmsCustomTable> GetDocToCurucullumsFromCms(UserInfo userInfo)
		{
			DataClassInfo customTable = DataClassInfoProvider.GetDataClass("thinkgate.docToCurriculums");
			if (customTable != null)
			{
				DataSet standardDataSet = (new CustomTableItemProvider(userInfo)).GetItems("thinkgate.docToCurriculums",
					string.Empty, string.Empty);
                if (standardDataSet != null && standardDataSet.Tables.Count > 0 &&
                    standardDataSet.Tables[0].Rows.Count > 0)
				{

					List<CmsCustomTable> lstResult = (from s in standardDataSet.Tables[0].AsEnumerable()
													  select
														  new CmsCustomTable
														  {
															  ItemId = Convert.ToInt32(s["ItemID"]),
															  DocId = Convert.ToInt32(s["docID"]),
															  Id = Convert.ToInt32(s["curriculumID"])
														  }).ToList<CmsCustomTable>();

					return lstResult;

				}
			}
			return null;
		}

		/// <summary>
		/// GetDocuments
		/// </summary>
		/// <param name="userInfo"></param>
		/// <param name="className"></param>
		/// <param name="treeProvider"></param>
		/// <param name="where"></param>
		/// <param name="orderby"></param>
		/// <param name="columnNames"></param>
		/// <param name="whereTextClause"></param>
		/// <param name="lookupDataSet"></param>
		/// <param name="pResourceType"></param>
		/// <returns>List</returns>
        protected List<Resource> GetDocuments(UserInfo userInfo, string className, TreeProvider treeProvider,
            string where,
			string orderby, string[] columnNames, string whereTextClause, DataSet lookupDataSet,
			ResourceTypes pResourceType = ResourceTypes.All)
		{
			if (whereTextClause != "")
			{
				where += where == "" ? columnNames[1] + whereTextClause : " AND " + columnNames[1] + whereTextClause;
			}
            DataSet resourceDs = KenticoHelper.ExpandedSearchDocumentType(userInfo, className, treeProvider,
                pResourceType, where,
				orderby, filterUsageRightExpiredContent);

			List<Resource> lstResource = new List<Resource>();

			if (resourceDs != null)
			{

				lstResource = (from res in resourceDs.Tables[0].AsEnumerable()
                    join ctdtl in lookupDataSet.Tables[0].AsEnumerable() on Convert.ToInt32(res["type"].ToString())
                        equals
								   Convert.ToInt32(ctdtl["enum"].ToString())
                    join stdtl in lookupDataSet.Tables[0].AsEnumerable() on Convert.ToInt32(res["subtype"].ToString())
                        equals
								   Convert.ToInt32(stdtl["enum"].ToString())
							   select
								   new Resource
								   {
									   NodeAliasPath = res["NodeAliasPath"].ToString(),
									   ResourceName = res[columnNames[1]].ToString(),
									   Description = res[columnNames[0]].ToString(),
									   Type = ctdtl["Description"].ToString(),
									   Subtype = stdtl["Description"].ToString(),
									   ViewLink =
										   columnNames[2] != ""
                                    ? string.IsNullOrEmpty(res[columnNames[2]].ToString())
                                        ? ""
                                        : res[columnNames[2]].ToString()
											   : string.Empty,
									   ID = Convert.ToInt32(res["DocumentID"]),
									   DocumentForeignKeyValue = Convert.ToInt32(res["DocumentForeignKeyValue"].ToString()),
									   DocumentType = res["ClassName"].ToString(),
									   DocumentNodeID = Convert.ToInt32(res["DocumentNodeID"]),
                                       ExpirationDate = res["ExpirationDate"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(res["ExpirationDate"]),
									   AverageRating = res["AverageRating"] == DBNull.Value ? default(decimal) : Convert.ToDecimal(res["AverageRating"])		  
								   }).ToList<Resource>();

				return lstResource;

			}
			return lstResource;
		}

		/// <summary>
		/// GetDocTypeValueFromTileMap
		/// </summary>
		/// <param name="resourceToShow"></param>
		/// <returns>int</returns>
        private int GetDocTypeValueFromTileMap(string resourceToShow, UserInfo ui)
		{
            CustomTableItemProvider tp = new CustomTableItemProvider(ui);
         
            DataSet lookupDataSet = tp.GetItems("thinkgate.TileMap", "BaseTileType = '" + resourceToShow + "' ",
                string.Empty);
			int lookupval = 0;
			if (lookupDataSet.Tables[0].Rows.Count > 0)
			{
				lookupval = (int)lookupDataSet.Tables[0].Rows[0]["Value"];
			}

			return lookupval;

		}

		/// <summary>
		/// GetDocumentResource
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="className"></param>
		/// <param name="treeProvider"></param>
		/// <param name="whereClause"></param>
		/// <param name="orderby"></param>
		/// <param name="whereTextClause"></param>
		/// <param name="lookupDataSet"></param>
		/// <param name="pResourceType"></param>
		/// <returns>List</returns>
		protected List<Resource> GetDocumentResource(UserInfo ui, string className, TreeProvider treeProvider,
			string whereClause, string orderby, string whereTextClause, DataSet lookupDataSet,
			ResourceTypes pResourceType = ResourceTypes.All)
		{

            int lookupval = GetDocTypeValueFromTileMap(className, ui);
			CustomTableItemProvider tp = new CustomTableItemProvider((UserInfo)Session["KenticoUserInfo"]);
			string filtercriteria = "LookupValue = " + lookupval + " and (ISNULL(StateLEA,'')='' or StateLEA = '" +
									DistrictParms.LoadDistrictParms().ClientID + "' or StateLEA = '" +
									DistrictParms.LoadDistrictParms().State + "')";
			DataSet resourcesToShow = tp.GetItems("thinkgate.TileMap_Lookup", filtercriteria, string.Empty);

			List<Resource> lstResource = new List<Resource>();
			foreach (DataRow dr in resourcesToShow.Tables[0].Select().OrderBy(p => p["ItemOrder"].ToString()))
			{
                if (isInstructionMaterial == true && dr["KenticoDocumentTypeToShow"].ToString().ToLower() == "thinkgate.competencylist")
                {
                    continue;
                }
				string[] columnNames =
				{
					dr["DescriptionColumnName"].ToString(), dr["NameColumnName"].ToString(),
					dr["AttachmentColumnName"].ToString(), dr["FriendlyName"].ToString()
				};
				List<Resource> resource =
					(from s in
                        GetDocuments(ui, dr["KenticoDocumentTypeToShow"].ToString(), treeProvider, whereClause,
                            string.Empty, columnNames,
							 whereTextClause, lookupDataSet, pResourceType)
					 select new Resource
					 {
						 Source =
							 s.NodeAliasPath.IndexOf(_districts.ToString(), StringComparison.Ordinal) >= 0
								 ? District
                                    : s.NodeAliasPath.IndexOf(_stateDocuments.ToString(), StringComparison.Ordinal) >= 0
                                        ? State
                                            : s.NodeAliasPath.IndexOf(_sharedDocuments.ToString(), StringComparison.Ordinal) >= 0
                                            ? SharedDocument //US15667
                                                : MyDocuments,
						 ID = s.ID,
						 DocumentType = s.DocumentType,
						 ID_Encrypted = "",
						 NodeAliasPath = s.NodeAliasPath,
						 ResourceName = s.ResourceName,
						 Description = s.Description,
						 Type = s.Type,
						 Subtype = s.Subtype,
						 ViewLink = s.ViewLink,
						 DocumentForeignKeyValue = s.DocumentForeignKeyValue,
						 DocumentNodeID = Convert.ToInt32(s.DocumentNodeID),
                         ExpirationDate = s.ExpirationDate,
                         AverageRating = s.AverageRating
												  
					 }).ToList<Resource>();
				if (resource.Count == 0)
				{
					resource = new List<Resource>();
				}
				lstResource.AddRange(resource);
			}
			return lstResource;
		}

		/// <summary>
		///  GetKenticoCurricumumUnitTypeName
		/// </summary>
		/// <param name="districtParms"></param>
		/// <returns>string</returns>
		protected string GetKenticoCurricumumUnitTypeName(DistrictParms districtParms)
		{
			if (districtParms.ClientState != null && !string.IsNullOrWhiteSpace(districtParms.ClientState))
			{
				var clientName = districtParms.ClientState;

				switch (clientName.ToUpper())
				{
					//  case "OH":
					//    return  CUClassNameOH ;
					default:
						return CuClassName;
				}
			}
			return CuClassName;
		}

		/// <summary>
		///  SearchResourcesWithCriteria
		/// </summary>
		/// <returns>List</returns>
		public List<Resource> SearchResourcesWithCriteria(bool search = false)
		{
			Criteria criteria = SearchControl.GetCriteria();

			ResourceTypes resUnitType;
			TagCriteriaSelectionParameters tagCriteriaSelectionParameters = new TagCriteriaSelectionParameters();
			string sourceSelection = "";
			string selectedTypes = "";
			string selectedSubTypes = "";
			string textSearch = "";
            List<int> averageRating = new List<int>();
		    DateTime? startExpirationDate = default(DateTime?);
            DateTime? endExpirationDate = default(DateTime?);
			StandardSelections selectedStandards = new StandardSelections();
			StandardSelections selectedAssessed = new StandardSelections();
			StandardSelections selectedTeaches = new StandardSelections();
			StandardSelections selectedRequires = new StandardSelections();
			CurriculumSelections selectedCurriculums = new CurriculumSelections();

            SchoolGradeNameSelections selectedSchoolGradeName = new SchoolGradeNameSelections();

			string selectedMediaType = "";
			string selectedLanguageType = "";

			foreach (Criterion cr in criteria.Criterias)
			{
				switch (cr.Key)
				{
					case "Source":
						sourceSelection = cr.Value.Key;
						break;
					case "Type":
						selectedTypes = cr.Value.Key;
						break;
					case "SubType":
						selectedSubTypes = cr.Value.Key;
						break;
					case "Text":
						textSearch = cr.Value.Value;
						break;
					case "Keywords":
						tagCriteriaSelectionParameters.Keywords = cr.Value.Value;
						break;
					case "Standards":
						selectedStandards.StandardSet = cr.StandardSelection.StandardSet;
						selectedStandards.Grade = cr.StandardSelection.Grade;
						selectedStandards.Subject = cr.StandardSelection.Subject;
						selectedStandards.Course = cr.StandardSelection.Course;
						selectedStandards.Key = cr.Value.Key;
						break;
					case "Curriculum":
						selectedCurriculums.Grade = cr.CurriculumSelection.Grade;
						selectedCurriculums.Subject = cr.CurriculumSelection.Subject;
						selectedCurriculums.Course = cr.CurriculumSelection.Course;
						selectedCurriculums.Key = cr.Value.Key;
						break;
                    case "ExpirationDate":
                        if (cr.Value.Value.IsNotNullOrWhiteSpace())
                        {
                            string[] dateRangeString = cr.Value.Value.Split(':');
                            startExpirationDate = dateRangeString[0].IsNullOrWhiteSpace() ? default(DateTime?) : Convert.ToDateTime(dateRangeString[0]);
                            endExpirationDate = dateRangeString[1].IsNullOrWhiteSpace() ? default(DateTime?) : Convert.ToDateTime(dateRangeString[1]);

                        }
                        break;
                    case "AverageRating":
                        if (cr.Value.Value.IsNotNullOrWhiteSpace())
                        {
                            averageRating = new List<int>(cr.Value.Value.Replace(", ", ",").Split(',').Select(int.Parse));
                        }
                        break;
					case "MediaType":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
						{
							tagCriteriaSelectionParameters.MediaType =
								new List<int>(cr.Value.Value.Replace(" ", "").Split(',').Select(int.Parse));
                            selectedMediaType =
                                tagCriteriaSelectionParameters.MediaType[0].ToString(CultureInfo.InvariantCulture);
						}
						break;
					case "Language":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
						{
							selectedLanguageType = cr.Value.Value;
                            tagCriteriaSelectionParameters.Language =
                                (int)Enum.Parse(typeof(LanguageType), cr.Value.Value);
						}
						break;
					case "Grades":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
                            tagCriteriaSelectionParameters.GradeLevel =
                                new List<string>(cr.Value.Value.Replace(", ", ",").Split(','));
						break;
					case "Assessed":
						selectedAssessed.StandardSet = cr.StandardSelection.StandardSet;
						selectedAssessed.Grade = cr.StandardSelection.Grade;
						selectedAssessed.Subject = cr.StandardSelection.Subject;
						selectedAssessed.Course = cr.StandardSelection.Course;
						selectedAssessed.Key = cr.Value.Key;
						if (IEnumerableExtensions.IsNotNullOrEmpty(GetListOfStandards(selectedAssessed)))
							tagCriteriaSelectionParameters.AssessedStandardIds =
								new List<int>(GetListOfStandards(selectedAssessed).Split(',').Select(int.Parse));
						break;
					case "Teaches":
						selectedTeaches.StandardSet = cr.StandardSelection.StandardSet;
						selectedTeaches.Grade = cr.StandardSelection.Grade;
						selectedTeaches.Subject = cr.StandardSelection.Subject;
						selectedTeaches.Course = cr.StandardSelection.Course;
						selectedTeaches.Key = cr.Value.Key;
						if (IEnumerableExtensions.IsNotNullOrEmpty(GetListOfStandards(selectedTeaches)))
							tagCriteriaSelectionParameters.TeachesStandardIds =
								new List<int>(GetListOfStandards(selectedTeaches).Split(',').Select(int.Parse));
						;
						break;
					case "Requires":
						selectedRequires.StandardSet = cr.StandardSelection.StandardSet;
						selectedRequires.Grade = cr.StandardSelection.Grade;
						selectedRequires.Subject = cr.StandardSelection.Subject;
						selectedRequires.Course = cr.StandardSelection.Course;
						selectedRequires.Key = cr.Value.Key;
						if (IEnumerableExtensions.IsNotNullOrEmpty(GetListOfStandards(selectedRequires)))
							tagCriteriaSelectionParameters.RequiresStandardIds =
								new List<int>(GetListOfStandards(selectedRequires).Split(',').Select(int.Parse));
						;
						break;
					case "TextComplexity":
						tagCriteriaSelectionParameters.TextComplexity = cr.Value.Value;
						break;
					case "ReadingLevel":
						tagCriteriaSelectionParameters.ReadingLevel = cr.Value.Value;
						break;
					case "EducationalSubject":
						if (cr.Value.Key.IsNotNullOrWhiteSpace())
                            tagCriteriaSelectionParameters.EducationalSubject =
                                new List<string>(cr.Value.Value.Replace(", ", ",").Split(','));
						break;

					case "InteractivityType":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
							tagCriteriaSelectionParameters.InteractivityType =
								new List<int>(cr.Value.Value.Replace(" ", "").Split(',').Select(int.Parse));

						if (sourceSelection == NationalLearningRegistry)
						{
							tagCriteriaSelectionParameters.InteractivityTypes = new List<string>();
                            DataTable dtActivity =
                                Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.Activity,
                                    Enums.SelectionType.Assessment.ToString());
						
							List<KeyValuePair<string, string>> theList = new List<KeyValuePair<string, string>>();
							foreach (DataRow row in dtActivity.Rows)
							{
                                theList.Add(new KeyValuePair<string, string>(row["ID"].ToString(),
                                    row["Description"].ToString()));
							}

							foreach (int item in tagCriteriaSelectionParameters.InteractivityType)
							{
                                tagCriteriaSelectionParameters.InteractivityTypes.Add(
                                    theList.First(kvp => kvp.Key == item.ToString()).Value);
							}
						}
						break;

					case "LearningResource":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
							tagCriteriaSelectionParameters.LearningResourceType =
								new List<int>(cr.Value.Value.Replace(" ", "").Split(',').Select(int.Parse));
						break;
					case "EducationalUse":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
							tagCriteriaSelectionParameters.EducationalUse =
								new List<int>(cr.Value.Value.Replace(" ", "").Split(',').Select(int.Parse));

						if (sourceSelection == NationalLearningRegistry)
						{
							tagCriteriaSelectionParameters.EducationalUses = new List<string>();
                            DataTable dtEducationalSubjects =
                                Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.EducationalUse,
                                    Enums.SelectionType.Resource.ToString());

							List<KeyValuePair<string, string>> theList = new List<KeyValuePair<string, string>>();
							foreach (DataRow row in dtEducationalSubjects.Rows)
							{
                                theList.Add(new KeyValuePair<string, string>(row["ID"].ToString(),
                                    row["Description"].ToString()));
							}

							foreach (int item in tagCriteriaSelectionParameters.EducationalUse)
							{
                                tagCriteriaSelectionParameters.EducationalUses.Add(
                                    theList.First(kvp => kvp.Key == item.ToString()).Value);
							}
						}
						break;
					case "EndUser":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
							tagCriteriaSelectionParameters.EndUser =
								new List<int>(cr.Value.Value.Replace(" ", "").Split(',').Select(int.Parse));

						if (sourceSelection == NationalLearningRegistry)
						{
							tagCriteriaSelectionParameters.EndUsers = new List<string>();
                            DataTable dtEndUsers =
                                Base.Classes.Assessment.LoadLrmiTagsByLookupEnum((int)Enums.LrmiTags.EndUser,
                                    Enums.SelectionType.Resource.ToString());

							List<KeyValuePair<string, string>> theList = new List<KeyValuePair<string, string>>();
							foreach (DataRow row in dtEndUsers.Rows)
							{
                                theList.Add(new KeyValuePair<string, string>(row["ID"].ToString(),
                                    row["Description"].ToString()));
							}

							foreach (int item in tagCriteriaSelectionParameters.EndUser)
							{
                                tagCriteriaSelectionParameters.EndUsers.Add(
                                    theList.First(kvp => kvp.Key == item.ToString()).Value);
							}
						}

						break;
					case "Duration":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
						{
							string[] durationStrings = cr.Value.Value.Split(':');
							tagCriteriaSelectionParameters.TimeRequiredDays = Convert.ToInt32(durationStrings[0]);
							tagCriteriaSelectionParameters.TimeRequiredHours = Convert.ToInt32(durationStrings[1]);
							tagCriteriaSelectionParameters.TimeRequiredMinutes = Convert.ToInt32(durationStrings[2]);
						}
						break;
					case "AgeAppropriate":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
						{
							if (sourceSelection == NationalLearningRegistry)
							{
								string validAges = mapAgeAppropriateToAges(cr.Value.Value);
								if (!string.IsNullOrEmpty(validAges))
								{
									tagCriteriaSelectionParameters.Ages = validAges;
								}
								//cant convert a string like "16-18" to an int
								// this needs to be looked at...
								//tagCriteriaSelectionParameters.AgeAppropriate = Convert.ToInt32(cr.Value.Value);
							}
						}
						break;
					case "Creator":
						tagCriteriaSelectionParameters.Creator = cr.Value.Value;
						break;
					case "Publisher":
						tagCriteriaSelectionParameters.Publisher = cr.Value.Value;
						break;
					case "UsageRights":
						if (cr.Value.Value.IsNotNullOrWhiteSpace())
						{
							tagCriteriaSelectionParameters.UseRightUrl = Convert.ToInt32(cr.Value.Key);

							if (sourceSelection == NationalLearningRegistry)
							{
                                string theURLResult =
                                    GetCreativeCommonsUrls().First(kvp => kvp.Key == cr.Value.Key).Value;

								if (cr.Value.Value.Equals("Custom", StringComparison.OrdinalIgnoreCase))
								{
									theURLResult = cr.Value.Value;
								}

								if (!string.IsNullOrEmpty(theURLResult))
								{
									//TODO: match theURLResult to LRMI tag dropdown items
									tagCriteriaSelectionParameters.UseRightUrlTxt = theURLResult;
									//If no match then set to "CUSTOM" I guess
								}
							}
						}
						break;
                    case "SchoolGradeName":
                        selectedSchoolGradeName.School = cr.SchoolGradeNameSelection.School;
                        selectedSchoolGradeName.Grade = cr.SchoolGradeNameSelection.Grade;
                        selectedSchoolGradeName.Name = cr.SchoolGradeNameSelection.Name; // userid
                        selectedSchoolGradeName.Key = cr.SchoolGradeNameSelection.Key;
                        break;


				}
			}

			string whereclause = "";
			string whereClauseIp = "";
			string whereClauseUp = "";
			string whereClauseLp = "";
			string whereClauseMu = "";
			string whereTextSearch = "";

			List<Resource> resultResource = new List<Resource>();

			if (sourceSelection == "" ||
				sourceSelection == District ||
				sourceSelection == State ||
				sourceSelection == MyDocuments ||
                sourceSelection == InternalSource ||
                sourceSelection == TeacherMyDocs ||
                sourceSelection == SharedDocument)//US15667
			{

				resUnitType = ResourceTypes.All;

				switch (sourceSelection)
				{
					case District:
						resUnitType = ResourceTypes.DistrictDocuments;
						break;
					case State:
						resUnitType = ResourceTypes.StateDocuments;
						break;
					case MyDocuments:
						resUnitType = ResourceTypes.MyDocuments;
						break;
                    case TeacherMyDocs: 
                        resUnitType = ResourceTypes.MyDocuments;
                        break;
                    case SharedDocument:
                        resUnitType = ResourceTypes.SharedDocuments;
                        break;
				}

				if (selectedTypes != null && (selectedTypes.Any() && selectedTypes != ""))
				{
					whereClauseIp = " Type = " + selectedTypes;
					whereClauseLp = " Type = " + selectedTypes;
					whereClauseMu = " Type = " + selectedTypes;
					if (selectedTypes == ((int)LookupDetail.CurriculumUnitMA).ToString(CultureInfo.InvariantCulture))
					{
						whereClauseUp = " Type = " + ((int)LookupDetail.UnitPlan) + " OR Type = " +
										((int)LookupDetail.CurriculumUnitMA);
						resUnitType = ResourceTypes.StateDocuments;
					}
					else
					{
						whereClauseUp = " Type = " + selectedTypes;
					}
					whereclause += whereclause == ""
						? " TYPE in ( " + string.Join(",", selectedTypes.Split(',')) +
						  ") "
						: "AND" + " TYPE in ( " +
						  string.Join(",", selectedTypes.Split(',')) + ") ";
				}
				if (selectedSubTypes.Any() && selectedSubTypes != "")
				{
					whereClauseIp += whereClauseIp == ""
						? " SubType = " + selectedSubTypes
						: " AND" + " SubType = " + selectedSubTypes;
					whereClauseLp += whereClauseLp == ""
						? " SubType = " + selectedSubTypes
						: " AND" + " SubType = " + selectedSubTypes;
					whereClauseMu += whereClauseMu == ""
						? " SubType = " + selectedSubTypes
						: " AND" + " SubType = " + selectedSubTypes;

					if (selectedSubTypes ==
						((int)LookupDetail.StateModelCurriculumUnitForm).ToString(CultureInfo.InvariantCulture))
					{
						whereClauseUp += whereClauseUp == ""
							? " SubType = " + ((int)LookupDetail.UnitPlanForm) + " OR SubType= " +
							  ((int)LookupDetail.StateModelCurriculumUnitForm)
							: " AND" + " SubType = " + ((int)LookupDetail.UnitPlanForm) + " OR SubType= " +
							  ((int)LookupDetail.StateModelCurriculumUnitForm);
						resUnitType = ResourceTypes.StateDocuments;
					}
					else
					{
						whereClauseUp += whereClauseUp == ""
							? " SubType = " + selectedSubTypes
							: " AND" + " SubType = " + selectedSubTypes;
					}
					whereclause += whereclause == ""
						? " SUBTYPE in ( " +
						  string.Join(",", selectedSubTypes.Split(',')) + ")"
						: "AND" + " SUBTYPE in ( " +
						  string.Join(",", selectedSubTypes.Split(',')) + ")";
				}
				if (textSearch.Any() && textSearch.Trim() != "")
				{
					whereclause += whereclause == ""
						? " Name like ( '%" + textSearch.Trim() + "%')"
						: " AND" + " Name like ( '%" + textSearch.Trim() + "%')";
					whereTextSearch = " like ( '%" + textSearch.Trim() + "%')";
				}

				DistrictParms districtParms = DistrictParms.LoadDistrictParms();
				string envState = KenticoHelper.GetKenticoMainFolderName(districtParms.ClientID);
				_districts = new System.Text.StringBuilder();
				_stateDocuments = new System.Text.StringBuilder();
                //US15667
                _sharedDocuments = new System.Text.StringBuilder();
				_districts.Append("/" + envState + "/Districts/");
				_stateDocuments.Append("/" + envState + "/Documents/");
                //US15667
                _sharedDocuments.Append("/" + envState + "/Shared/");

                UserInfo ui = new UserInfo();
                if (!string.IsNullOrEmpty(selectedSchoolGradeName.Name))
                {
                    if ((bool)Session["KenticoEnabled"])
                    {
                        KenticoHelper.KenticoUserName = KenticoHelper.GetKenticoUser(selectedSchoolGradeName.Name);
                        ui = CMS.SiteProvider.UserInfoProvider.GetFullUserInfo(KenticoHelper.KenticoUserName);
                }
                    else
                    { ui = (UserInfo)Session["KenticoUserInfo"]; }
                }
                else
                { ui = (UserInfo)Session["KenticoUserInfo"]; }


				string where = whereclause;
				const string orderBy = "Name";

				TreeProvider treeProvider =
					KenticoHelper.GetUserTreeProvider(_sessionObject.LoggedInUser.ToString());
				DataSet resourceDs = KenticoHelper.ExpandedSearchDocumentType(ui, ResourceClassName,
					treeProvider, resUnitType, where, orderBy, filterUsageRightExpiredContent);

				List<Resource> lstResource = new List<Resource>();
                List<Resource> listResourceTemp = new List<Resource>();

				DataSet lookupDataSet = new DataSet();
				DataClassInfo customTable = DataClassInfoProvider.GetDataClass("TG.LookupDetails");
				if (customTable != null)
				{
					lookupDataSet = (new CustomTableItemProvider(ui)).GetItems("TG.LookupDetails",
						string.Empty, string.Empty);
				}

				//For IP, UP, LP, and MU
				List<Resource> lstIpup = GetPlans(ui, treeProvider, whereClauseIp, whereClauseUp, whereClauseLp,
					whereClauseMu, whereTextSearch, lookupDataSet, districtParms, resUnitType);



				if (!string.IsNullOrWhiteSpace(selectedStandards.StandardSet) ||
					!string.IsNullOrWhiteSpace(selectedCurriculums.Grade))
				{
					if (!string.IsNullOrWhiteSpace(selectedStandards.StandardSet))
					{

						string stringListOfStandards = GetListOfStandards(selectedStandards);
						List<CmsCustomTable> lstStandards = GetDocToStandardsFromCms(ui,
							stringListOfStandards);

						if (lstStandards != null && lstStandards.Count > 0)
						{
							if (resourceDs != null && resourceDs.Tables[0].Rows.Count > 0)
							{
					            listResourceTemp = (from res in resourceDs.Tables[0].AsEnumerable()
											   join ctdtl in lookupDataSet.Tables[0].AsEnumerable() on
												   Convert.ToInt32(res["type"].ToString()) equals
												   Convert.ToInt32(ctdtl["enum"].ToString())
											   join stdtl in lookupDataSet.Tables[0].AsEnumerable() on
												   Convert.ToInt32(res["subtype"].ToString()) equals
												   Convert.ToInt32(stdtl["enum"].ToString())
					                                join s in lstStandards on
					                                    Convert.ToInt32(res["DocumentNodeID"]) equals s.DocId
											   select
												   new Resource
												   {
													   Source =
                                                res["NodeAliasPath"].ToString()
					                                                                .IndexOf(
					                                                                    _districts.ToString(),
					                                                                    StringComparison.Ordinal)
					                                            >= 0
															   ? District
                                                    : res["NodeAliasPath"].ToString()
					                                                                      .IndexOf(
					                                                                          _stateDocuments
					                                            .ToString(),
					                                                                          StringComparison
					                                            .Ordinal) >=
                                                      0
																   ? State
                                                    : res["NodeAliasPath"].ToString()
                                                        .IndexOf(_sharedDocuments.ToString(), StringComparison.Ordinal) >=
                                                      0
                                                                   ? SharedDocument //US15667
																   : MyDocuments,
													   ID = Convert.ToInt32(res["DocumentID"]),
													   DocumentNodeID = Convert.ToInt32(res["DocumentNodeID"]),
													   DocumentForeignKeyValue =
					                                            Convert.ToInt32(
					                                                res["DocumentForeignKeyValue"].ToString()),
													   DocumentType = res["ClassName"].ToString(),
													   ID_Encrypted = "",
													   NodeAliasPath = res["NodeAliasPath"].ToString(),
													   ResourceName = res["Name"].ToString(),
													   Description = res["Description"].ToString(),
													   Type = ctdtl["Description"].ToString(),
													   Subtype = stdtl["Description"].ToString(),
													   ViewLink =
					                                            string.IsNullOrEmpty(
					                                                res["AttachmentName"].ToString())
															   ? ""
															   : res["AttachmentName"].ToString(),
					                                        ExpirationDate =
					                                            res["ExpirationDate"] == DBNull.Value
					                                                ? default(DateTime)
					                                                : Convert.ToDateTime(res["ExpirationDate"]),
					                                        AverageRating =
					                                            res["AverageRating"] == DBNull.Value
					                                                ? default(decimal)
                                                                    : Convert.ToDecimal(res["AverageRating"]),
                                                                    DocumentCreatedWhen= res["DocumentModifiedWhen"] == DBNull.Value
                                                            ? default(DateTime)
                                                            : Convert.ToDateTime(res["DocumentModifiedWhen"])
												   }).Distinct().ToList<Resource>();
							}

						lstIpup = (from res in lstIpup
										   join s in lstStandards on res.DocumentNodeID equals s.DocId
										   select res).ToList<Resource>();
							
					        if (lstResource.Count > 0 || isFilteredResourceEmpty)
					            lstResource = (from res in lstResource
					                           join t in listResourceTemp on res.ID equals t.ID
					                           select res).ToList<Resource>();
					        else
					            lstResource.AddRange(listResourceTemp);

					        if (listResourceTemp.Count == 0)
					            isFilteredResourceEmpty = true;
						}
                        else
					    {
                            lstIpup = new List<Resource>();
					        lstResource = new List<Resource>();
					    }

					}

					if (!string.IsNullOrWhiteSpace(selectedCurriculums.Grade))
					{

						List<CmsCustomTable> lstCurriculum = GetDocToCurucullumsFromCms(ui);
						List<object> listCurriculums = GetCurriculumsIDsFromSelectedCriteria(selectedCurriculums);
						lstCurriculum = lstCurriculum.FindAll(f => listCurriculums.Contains(f.Id));
                        if (lstCurriculum.Count > 0)
                        {
						if (resourceDs != null && resourceDs.Tables[0].Rows.Count > 0)
						{
					            listResourceTemp = (from res in resourceDs.Tables[0].AsEnumerable()
										   join ctdtl in lookupDataSet.Tables[0].AsEnumerable() on
											   Convert.ToInt32(res["type"].ToString()) equals
											   Convert.ToInt32(ctdtl["enum"].ToString())
										   join stdtl in lookupDataSet.Tables[0].AsEnumerable() on
											   Convert.ToInt32(res["subtype"].ToString()) equals
											   Convert.ToInt32(stdtl["enum"].ToString())
					                                join s in lstCurriculum on
					                                    Convert.ToInt32(res["DocumentNodeID"]) equals s.DocId
										   select
											   new Resource
											   {
												   Source =
                                            res["NodeAliasPath"].ToString()
					                                                                .IndexOf(
					                                                                    _districts.ToString(),
					                                                                    StringComparison.Ordinal)
					                                            >= 0
														   ? District
                                                : res["NodeAliasPath"].ToString()
					                                                                      .IndexOf(
					                                                                          _stateDocuments
					                                            .ToString(),
					                                                                          StringComparison
					                                            .Ordinal) >= 0
															   ? State
                                                : res["NodeAliasPath"].ToString()
                                                    .IndexOf(_sharedDocuments.ToString(), StringComparison.Ordinal) >= 0
                                                               ? SharedDocument//US15667
															   : MyDocuments,
												   ID = Convert.ToInt32(res["DocumentID"]),
												   DocumentNodeID = Convert.ToInt32(res["DocumentNodeID"]),
												   DocumentForeignKeyValue =
					                                            Convert.ToInt32(
					                                                res["DocumentForeignKeyValue"].ToString()),
												   DocumentType = res["ClassName"].ToString(),
												   ID_Encrypted = "",
												   NodeAliasPath = res["NodeAliasPath"].ToString(),
												   ResourceName = res["Name"].ToString(),
												   Description = res["Description"].ToString(),
												   Type = ctdtl["Description"].ToString(),
												   Subtype = stdtl["Description"].ToString(),
												   ViewLink =
					                                            string.IsNullOrEmpty(
					                                                res["AttachmentName"].ToString())
														   ? ""
														   : res["AttachmentName"].ToString(),
					                                        ExpirationDate =
					                                            res["ExpirationDate"] == DBNull.Value
					                                                ? default(DateTime)
					                                                : Convert.ToDateTime(res["ExpirationDate"]),
					                                        AverageRating =
					                                            res["AverageRating"] == DBNull.Value
					                                                ? default(decimal)
                                                                    : Convert.ToDecimal(res["AverageRating"]),
                                                            DocumentCreatedWhen= res["DocumentModifiedWhen"] == DBNull.Value
                                                                   ? default(DateTime)
                                                                   : Convert.ToDateTime(res["DocumentModifiedWhen"])
												  
											   }).ToList<Resource>();
						}

						
							lstIpup = (from res in lstIpup
									   join s in lstCurriculum on res.DocumentNodeID equals s.DocId
									   select res).ToList<Resource>();

					        if (lstResource.Count > 0  || isFilteredResourceEmpty)
					            lstResource = (from res in lstResource
					                           join t in listResourceTemp on res.ID equals t.ID
					                           select res).ToList<Resource>();
					        else
					            lstResource.AddRange(listResourceTemp);

					        if (listResourceTemp.Count == 0)
					            isFilteredResourceEmpty = true;
						}
                        else
					    {
                            lstIpup = new List<Resource>();
					        lstResource = new List<Resource>();
					    }
					}

				}
				else
				{

					if (resourceDs != null && resourceDs.Tables[0].Rows.Count > 0)
					{

						listResourceTemp = (from res in resourceDs.Tables[0].AsEnumerable()
									   join ctdtl in lookupDataSet.Tables[0].AsEnumerable() on
										   Convert.ToInt32(res["type"].ToString()) equals Convert.ToInt32(ctdtl["enum"].ToString())
									   join stdtl in lookupDataSet.Tables[0].AsEnumerable() on
										   Convert.ToInt32(res["subtype"].ToString()) equals
										   Convert.ToInt32(stdtl["enum"].ToString())
									   select
										   new Resource
										   {
											   Source =
                                        res["NodeAliasPath"].ToString()
                                            .IndexOf(_districts.ToString(), StringComparison.Ordinal) >= 0
													   ? District
                                            : res["NodeAliasPath"].ToString()
                                                .IndexOf(_stateDocuments.ToString(), StringComparison.Ordinal) >= 0
														   ? State
                                            : res["NodeAliasPath"].ToString()
                                                .IndexOf(_sharedDocuments.ToString(), StringComparison.Ordinal) >= 0
                                                           ? SharedDocument //US15667
														   : MyDocuments,
											   ID = Convert.ToInt32(res["DocumentID"]),
											   DocumentForeignKeyValue = Convert.ToInt32(res["DocumentForeignKeyValue"].ToString()),
											   DocumentType = res["ClassName"].ToString(),
											   ID_Encrypted = "",
											   NodeAliasPath = res["NodeAliasPath"].ToString(),
											   ResourceName = res["Name"].ToString(),
											   Description = res["Description"].ToString(),
											   Type = ctdtl["Description"].ToString(),
											   Subtype = stdtl["Description"].ToString(),
											   ViewLink =
												   string.IsNullOrEmpty(res["AttachmentName"].ToString())
													   ? ""
													   : res["AttachmentName"].ToString(),
											   DocumentNodeID = Convert.ToInt32(res["DocumentNodeID"]),
                                               ExpirationDate = res["ExpirationDate"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(res["ExpirationDate"]),
                                                    AverageRating = res["AverageRating"] == DBNull.Value ? default(decimal) : Convert.ToDecimal(res["AverageRating"]),
                                                    DocumentCreatedWhen= res["DocumentModifiedWhen"] == DBNull.Value? default(DateTime): Convert.ToDateTime(res["DocumentModifiedWhen"])												  
										   }).ToList<Resource>();
					}

                    if (lstResource.Count > 0  || isFilteredResourceEmpty )
                        lstResource = (from res in lstResource
                                       join t in listResourceTemp on res.ID equals t.ID
                                       select res).ToList<Resource>();
                    else
                        lstResource.AddRange(listResourceTemp);

                    

				}

				if (lstIpup.Count > 0)
				{
					lstResource.AddRange(lstIpup);
				}

				string hostURL = Request.Url.ToString();
				try
				{
					int hostPosition = hostURL.LastIndexOf(districtParms.ClientID) - 1;
					hostURL = hostURL.Substring(0, hostPosition);
				}
				catch
				{
					hostURL = "";
				}

			   if (startExpirationDate != default(DateTime?))
                {
                    lstResource = lstResource.FindAll(unl => unl.ExpirationDate >= startExpirationDate && unl.ExpirationDate != default(DateTime));
                }

                if (endExpirationDate != default(DateTime?))
                {
                    lstResource = lstResource.FindAll(unl => unl.ExpirationDate <= endExpirationDate && unl.ExpirationDate != default(DateTime));
                }

			    if (averageRating.Count > 0)
			    {
			        List<RatingRange> ratingRange = _reviewFacade.GetRatingFilterForItemSearch().ToList();
                    List<Resource> newLstResource = new List<Resource>();
			        decimal maxvalue;
			        decimal minvalue;
			        foreach (var id in averageRating )
			        {
			            maxvalue = ratingRange.Find(x => x.Id == id).EndValue;
                        minvalue = ratingRange.Find(x => x.Id == id).StartValue;
			            newLstResource.AddRange(
			                lstResource.FindAll(
			                    unl => unl.AverageRating >= minvalue && unl.AverageRating <= maxvalue).ToList());

			        }
			        lstResource = newLstResource;
			    }

			    resultResource =
					lstResource.GroupBy(
						x =>
							new
							{
								x.Source,
								x.ID,
								x.DocumentNodeID,
								x.ResourceName,
                                x.ResourceNameReadOnly,
								x.Description,
								x.Type,
								x.Subtype,
								x.ViewLink,
								x.NodeAliasPath,
								x.DocumentForeignKeyValue,
								x.DocumentType,
                                x.ExpirationDate,
                                x.AverageRating,
                                x.DocumentCreatedWhen
							})
						.Select(r => new Resource
						{
							Source = r.Key.Source,
							ID = r.Key.ID,
							DocumentNodeID = r.Key.DocumentNodeID,
							ResourceName = r.Key.ResourceName,
                            ResourceNameReadOnly = r.Key.ResourceName,
							Description = r.Key.Description,
							Type = r.Key.Type,
							Subtype = r.Key.Subtype,
							//ViewLink = string.IsNullOrWhiteSpace(r.Key.ViewLink) ? "" : hostURL + KenticoHelper.KenticoVirtualFolderRelative + "/cmspages/getfile.aspx?guid=" + r.Key.ViewLink + "&disposition=inline",
							ViewLink =
								string.IsNullOrWhiteSpace(r.Key.ViewLink)
									? ""
                                    : KenticoHelper.KenticoVirtualFolderRelative + "/cmspages/getfile.aspx?guid=" +
                                      r.Key.ViewLink +
									  "&disposition=inline",
							NodeAliasPath = r.Key.NodeAliasPath,
							DocumentForeignKeyValue = r.Key.DocumentForeignKeyValue,
							DocumentType = r.Key.DocumentType,
                            ExpirationDate = r.Key.ExpirationDate,
                            AverageRating = r.Key.AverageRating,
                            DocumentCreatedWhen=r.Key.DocumentCreatedWhen
						}).ToList();

                resultResource = resultResource.OrderByDescending(rslRsc => rslRsc.DocumentCreatedWhen).ToList();

				if (tagCriteriaSelectionParameters != null)
				{
					if (!TagCriteriaSelectionParameters.IsEmpty(tagCriteriaSelectionParameters))
					{
						List<Resource> filteredByTags = FilterResourcesByTags(resultResource,
							tagCriteriaSelectionParameters);
						resultResource = filteredByTags;
					}
				}
			}
			else
			{
				string resourceTextSearch = textSearch.Trim() == ""
					? LearningMediaFacets.SearchTermAll
					: textSearch.Trim();

				if (sourceSelection == NationalLearningRegistry)
				{
                    NLRSearchParameters nlrSearchParameters = getNLRSearchParameters(resourceTextSearch,
                        tagCriteriaSelectionParameters);
					resultResource = GetLearningRegistryResources(search, nlrSearchParameters);
				}

				if (sourceSelection == PBSLearningMedia)
				{
					if (tagCriteriaSelectionParameters.GradeLevel == null)
						tagCriteriaSelectionParameters.GradeLevel = new List<string>();
                    resultResource = GetWgbhResources(search, tagCriteriaSelectionParameters.GradeLevel,
                        resourceTextSearch,
						selectedMediaType, selectedLanguageType);
				}
			}
			return resultResource;
		}

		private string mapAgeAppropriateToAges(string ageRange)
		{
			string theAgeString = string.Empty;
			if (!string.IsNullOrEmpty(ageRange))
			{
				if (ageRange.Contains("-"))
				{
					string[] rangeValues = ageRange.Split('-');

					List<int> listOfValues = new List<int>();

					for (int i = int.Parse(rangeValues[0]); i <= int.Parse(rangeValues[1]); i++)
					{
						listOfValues.Add(i);
					}

					//					IEnumerable<int> range = Enumerable.Range(int.Parse(rangeValues[0]), 2);
					theAgeString = String.Join(" ", listOfValues);
				}
				else
				{
					switch (ageRange)
					{
						case "Very Young":
							theAgeString = "0 1 2 3 4";
							break;
						case "Kindergarten":
							theAgeString = "5 6";
							break;
						case "Postsecondary":
							theAgeString = String.Join(" ", 20, 20); //20-40
							break;
						case "Technical":
							theAgeString = String.Join(" ", 18, 12); //18-30
							break;
						case "Adult Education":
							theAgeString = String.Join(" ", 35, 45); //40-80
							break;
					}
				}
			}
			return theAgeString;
		}

		private string JoinThevalues(IEnumerable<int> range)
		{
			string theJoinedString = string.Empty;

			theJoinedString = String.Join(" ", range);

			return theJoinedString;
		}

		private NLRSearchParameters getNLRSearchParameters(string criteriaSelectionParameters,
			TagCriteriaSelectionParameters tagCriteriaSelectionParameters)
		{
			NLRSearchParameters theNLRSearchParameters = new NLRSearchParameters();

			theNLRSearchParameters.creator = tagCriteriaSelectionParameters.Creator;
			theNLRSearchParameters.publisher = tagCriteriaSelectionParameters.Publisher;
			//theNLRSearchParameters.usageRightsURL = tagCriteriaSelectionParameters.UseRightUrl;
			//theNLRSearchParameters.author = tagCriteriaSelectionParameters.Author;
			theNLRSearchParameters.created = tagCriteriaSelectionParameters.DateCreated.ToString();
			//theNLRSearchParameters.accessibilityControl
			//theNLRSearchParameters.accessibilityFeature
			//theNLRSearchParameters.accessibilityHazard
			//theNLRSearchParameters.bookFormat
			//theNLRSearchParameters.copyrightHolder
			theNLRSearchParameters.title =
				theNLRSearchParameters.description =
					theNLRSearchParameters.titleAndDescriptionSearchText = criteriaSelectionParameters;

			if (tagCriteriaSelectionParameters.Language > -1)
			{
				theNLRSearchParameters.inLanguage =
					Enum.Parse(typeof(LanguageType), tagCriteriaSelectionParameters.Language.ToString()).ToString();
			}
			theNLRSearchParameters.typicalAgeRange = tagCriteriaSelectionParameters.Ages;

			theNLRSearchParameters.interactivityType = tagCriteriaSelectionParameters.InteractivityTypes;

			theNLRSearchParameters.educationalUse = tagCriteriaSelectionParameters.EducationalUses;

			theNLRSearchParameters.educationalRole = tagCriteriaSelectionParameters.EndUsers;

			theNLRSearchParameters.usageRightsURL = tagCriteriaSelectionParameters.UseRightUrlTxt;
			
			theNLRSearchParameters.keywords = tagCriteriaSelectionParameters.Keywords;

			theNLRSearchParameters.educationalRole = tagCriteriaSelectionParameters.EndUsers;

			theNLRSearchParameters.usageRightsURL = tagCriteriaSelectionParameters.UseRightUrlTxt;

			return theNLRSearchParameters;
		}

		/// <summary>
		/// GetListOfStandards - Get string back of standard Id's to be used as a list.
		/// </summary>
		/// <param name="selectedStandards"></param>
		/// <returns>string</returns>
		private string GetListOfStandards(StandardSelections selectedStandards)
		{
			string stringListOfStandards = selectedStandards.Key;

			if (!stringListOfStandards.IsInt() && !string.IsNullOrEmpty(selectedStandards.StandardSet))
			{
				var standardSetList = new drGeneric_String();
				if (!String.IsNullOrEmpty(selectedStandards.StandardSet))
				{
					standardSetList.Add(selectedStandards.StandardSet.Trim());
				}


				var gradeList = new drGeneric_String();
				if (!String.IsNullOrEmpty(selectedStandards.Grade))
				{
					gradeList.Add(selectedStandards.Grade.Trim());
				}

				var subjectList = new drGeneric_String();
				if (!String.IsNullOrEmpty(selectedStandards.Subject))
				{
					subjectList.Add(selectedStandards.Subject.Trim());
				}

				var courseList = new drGeneric_String();
				if (!String.IsNullOrEmpty(selectedStandards.Course))
				{
					courseList.Add(selectedStandards.Course.Trim());
				}
				DataTable filteredStandards = Base.Classes.Standards.SearchStandards(false,
					"", //TextSearch
					"", //TextSearchVal
					null, //itemBanks
					null, //StandardCourses
					null,
					standardSetList.Count > 0 ? standardSetList : null,
					gradeList.Count > 0 ? gradeList : null,
					subjectList.Count > 0 ? subjectList : null,
					courseList.Count > 0 ? courseList : null);
				List<object> listOfStandards = (from lststd in filteredStandards.DataSet.Tables[0].AsEnumerable()
												select lststd["StandardID"]).ToList();
				stringListOfStandards = string.Join(",", listOfStandards);
				if (stringListOfStandards == "") stringListOfStandards = "0";
			}
			return stringListOfStandards;
		}

		/// <summary>
		/// Get all plans from kentico based on search criteria
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="treeProvider"></param>
		/// <param name="whereClauseIp"></param>
		/// <param name="whereClauseUp"></param>
		/// <param name="whereClauseLp"></param>
		/// <param name="whereClauseMu"></param>
		/// <param name="whereTextSearch"></param>
		/// <param name="lookupDataSet"></param>
		/// <param name="districtParms"></param>
		/// <param name="resUnitType"></param>
		/// <returns>List</returns>
        private List<Resource> GetPlans(UserInfo ui, TreeProvider treeProvider, string whereClauseIp,
            string whereClauseUp,
			string whereClauseLp, string whereClauseMu, string whereTextSearch, DataSet lookupDataSet,
			DistrictParms districtParms, ResourceTypes resUnitType)
		{

			List<Resource> buildList = new List<Resource>();
			buildList.AddRange(GetDocumentResource(ui, IpClassName, treeProvider, whereClauseIp, string.Empty,
				whereTextSearch, lookupDataSet, resUnitType));
			buildList.AddRange(GetDocumentResource(ui, UpClassName, treeProvider, whereClauseUp, string.Empty,
				whereTextSearch, lookupDataSet, resUnitType));
			buildList.AddRange(GetDocumentResource(ui, LpClassName, treeProvider, whereClauseLp, string.Empty,
				whereTextSearch, lookupDataSet, resUnitType));
			if (districtParms.State.ToUpper() == "OH")
				buildList.AddRange(GetDocumentResource(ui, GetKenticoCurricumumUnitTypeName(districtParms),
					treeProvider, whereClauseMu, string.Empty, whereTextSearch, lookupDataSet, resUnitType));
			return buildList;
		}

		/// <summary>
		/// GetWgbhResources - Returns a resource list of WGBH search items.
		/// </summary>
		/// <param name="grade"></param>
		/// <param name="textSearch"></param>
		/// <param name="media"></param>
		/// <param name="language"></param>
		/// <returns>List</returns>
		private List<Resource> GetWgbhResources(bool search, List<string> grade = null, string textSearch = null,
			string media = null, string language = null)
		{
			if (grade == null) throw new ArgumentNullException("grade");
			if (textSearch == null) throw new ArgumentNullException("textSearch");
			if (media == null) throw new ArgumentNullException("media");
			if (language == null) throw new ArgumentNullException("language");

			var list = new List<Resource>();
			var learningMediaRequestFilter = new LearningMediaRequestFilter
			{
				SearchTerm = LearningMediaFacets.SearchTermAll,
				LanguageTypes = new List<LanguageType>(),
				AccessibilityTypes = new List<AccessibilityType>(),
				PermittedUseTypes = new List<PermittedUseType>(),
				GradeTypes = new List<GradeType>(),
				RequestedPage = 0
			};

			MediaType mediaType = new MediaType();
			if (media.IsNotNullOrWhiteSpace() && media != "-1")
				mediaType = (MediaType)Enum.Parse(typeof(MediaType), media);


			string[] validWGBHLanguages = { "English", "Spanish", "Navajo" };

			LanguageType languageType = new LanguageType();
			if (language.IsNotNullOrWhiteSpace() && language != "-1" && validWGBHLanguages.Contains(language))
				languageType = (LanguageType)Enum.Parse(typeof(LanguageType), language);

			if (grade.Count > 0)
			{
				foreach (string gradeList in grade)
				{
					if (gradeList.IsNotNullOrWhiteSpace() && gradeList != "-1")
					{
						GradeType gradeType = new GradeType();
						if (gradeList != "9_12" && gradeList != "PK")
							gradeType = (GradeType)Enum.Parse(typeof(GradeType), gradeList);

						if (gradeList == "9_12")
						{
							learningMediaRequestFilter.GradeTypes.Add(GradeType.Nine);
							learningMediaRequestFilter.GradeTypes.Add(GradeType.Ten);
							learningMediaRequestFilter.GradeTypes.Add(GradeType.Eleven);
							learningMediaRequestFilter.GradeTypes.Add(GradeType.Twelve);
						}
						else if (gradeList == "PK")
						{
							learningMediaRequestFilter.GradeTypes.Add(GradeType.PreK);
						}
						else
							learningMediaRequestFilter.GradeTypes.Add(gradeType);
					}
				}
			}

			learningMediaRequestFilter.SearchTerm = textSearch;
			learningMediaRequestFilter.MediaType = mediaType;
			learningMediaRequestFilter.LanguageTypes.Add(languageType);

			var wgbhUserName = ConfigurationManager.AppSettings["WGBHUserName"];
            NameValueCollection appSecureSettings =
                (NameValueCollection)ConfigurationManager.GetSection("appSecureSettings");
			var wgbhUserPassword = appSecureSettings["WGBHUserPassword"];
			var wgbhBaseRequestUri = ConfigurationManager.AppSettings["WGBHBaseRequestUri"];
			var wgbhRequestUri = ConfigurationManager.AppSettings["WGBHRequestUri"];

			var learningMediaProxy = new LearningMediaProxy();
			var resourceSearch = learningMediaProxy.SearchResources(wgbhUserName, wgbhUserPassword,
				new Uri(wgbhBaseRequestUri), learningMediaRequestFilter);

			if (!resourceSearch.IsNotNull()) return list;

			if (resourceSearch.LearningMediaResponseHeader.TotalPages > 10 && search)
			{
                TelerikWindowManager.RadAlert("Search Request exceeds 200 items. The first 200 have been returned.", 400,
                    100,
					PBSLearningMedia + " Response", "");
			}

			list = BuildResourceMediaList(resourceSearch.LearningMediaSearchResults);
			if (resourceSearch.LearningMediaResponseHeader.TotalPages > 1)
				for (int i = 1; i < resourceSearch.LearningMediaResponseHeader.TotalPages; i++)
				{
					if (i > 9) break;
					resourceSearch = learningMediaProxy.SearchNextPage(wgbhUserName, wgbhUserPassword,
						new Uri(wgbhRequestUri + resourceSearch.LearningMediaResponseHeader.NextPage));
					list.AddRange(BuildResourceMediaList(resourceSearch.LearningMediaSearchResults));
				}

			return list;
		}

		private List<Resource> BuildResourceMediaList(IEnumerable<LearningMediaSearchResult> learningMediaSearchResults)
		{
			var list = new List<Resource>();
			foreach (LearningMediaSearchResult row in learningMediaSearchResults)
			{
				var resource = new Resource
				{
					Source = PBSLearningMedia,
					ID = DataIntegrity.ConvertToInt(row.UniqueIdentifier),
					ResourceName = row.Title,
					Type = row.MediaType,
					URL = row.Url,
					ViewLink = String.Empty,
					Description = row.ShortDescription
				};
				list.Add(resource);
			}

			return list;
		}

		private List<Resource> GetLearningRegistryResources(bool search, NLRSearchParameters nlrSearchParameters)
		{
			//call ElasticSearch web service
			var elasticSearchProxy = new EsQueryProxy();

			string theQuery = BuildQuery(nlrSearchParameters);

			EsQueryResult result = elasticSearchProxy.EsQuery(theQuery);
			StatusResponse statusResponse = result.Status;
			bool isSuccess = statusResponse.IsSuccess();

            if (isSuccess && result.QueryResults != null &&
                result.QueryResults.Count >= NationalLearningRegistryMaxResults &&
				search)
			{
				TelerikWindowManager.RadAlert(
					"Search Request exceeds " + NationalLearningRegistryMaxResults.ToString() + " items. The first " +
					NationalLearningRegistryMaxResults.ToString() + " have been returned.", 400, 100,
					NationalLearningRegistry + " Response", "");
			}

			return isSuccess ? BuildLearningRegistryResultList(result) : new List<Resource>();
		}

		private string BuildQuery(NLRSearchParameters nlrSearchParameters)
		{
			if (nlrSearchParameters == null) return null;

			if (string.IsNullOrEmpty(nlrSearchParameters.titleAndDescriptionSearchText) ||
				nlrSearchParameters.titleAndDescriptionSearchText == "*")
			{
				nlrSearchParameters.titleAndDescriptionSearchText = string.Empty;
			}

			StringBuilder sb = new StringBuilder();


			sb.Append("{\"query\": {");
			sb.Append("\"bool\" : {");
			sb.Append("\"must\" : [ ");

			if (!string.IsNullOrEmpty(nlrSearchParameters.titleAndDescriptionSearchText))
			{
				sb.Append("{");
				sb.Append("\"multi_match\": {");
				sb.Append("\"query\": \"").Append(nlrSearchParameters.titleAndDescriptionSearchText).Append("\",");
				sb.Append("\"fields\":");
				sb.Append("[");
				//Multi-match query on "title", "description" and "keywords" fields
				//sb.Append("\"title\", \"description\", \"keywords\"");
				sb.Append("\"title\", \"description\"");
				sb.Append("],");
				sb.Append("\"operator\": \"or\"");
				sb.Append("}");
				sb.Append("},");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.creator))
			{
				//Multi-match query on both "creator" and "author" fields
                sb.Append("{\"multi_match\": {\"query\": \"")
                    .Append(nlrSearchParameters.creator)
                    .Append("\",\"fields\":[ \"creator\", \"author\"],\"operator\": \"or\"}},");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.publisher))
			{
				sb.Append("{ \"match\": { \"publisher\" : \"").Append(nlrSearchParameters.publisher).Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.inLanguage))
			{
				string theLanguage = MapLanguage(nlrSearchParameters.inLanguage);

				sb.Append("{ \"match\": { \"language\" : \"").Append(theLanguage).Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.keywords))
			{
				//This is an "_all" query so it will try and match the search term in "all" fields
				sb.Append("{ \"match\": { \"_all\" : \"").Append(nlrSearchParameters.keywords).Append("\" } },");
			}

			if (nlrSearchParameters.educationalUse.Count > 0)
			{
				string educationalUses = string.Join(" ", nlrSearchParameters.educationalUse.ToArray());

				sb.Append("{ \"match\": { \"educationalUses\" : \"")
					.Append(educationalUses.ToLower())
					.Append("\" } },");
			}

			//if (!string.IsNullOrEmpty(nlrSearchParameters.author))
			//{
			//	sb.Append("{ \"match\": { \"author\" : \"").Append(nlrSearchParameters.author).Append("\" } },");
			//}

			if (nlrSearchParameters.interactivityType.Count > 0)
			{
				string interactivityTypes = string.Join(" ", nlrSearchParameters.interactivityType.ToArray());
				
				sb.Append("{ \"match\": { \"interactivityType\" : \"")
					.Append(interactivityTypes.ToLower())
					.Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.accessibilityFeature))
			{
				sb.Append("{ \"match\": { \"accessibilityFeature\" : \"")
					.Append(nlrSearchParameters.accessibilityFeature)
					.Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.accessibilityHazard))
			{
				sb.Append("{ \"match\": { \"accessibilityHazard\" : \"")
					.Append(nlrSearchParameters.accessibilityHazard)
					.Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.bookFormat))
			{
				sb.Append("{ \"match\": { \"bookFormat\" : \"").Append(nlrSearchParameters.bookFormat).Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.copyrightHolder))
			{
                sb.Append("{ \"match\": { \"copyrightHolder\" : \"")
                    .Append(nlrSearchParameters.copyrightHolder)
                    .Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.learningResourceType))
			{
				sb.Append("{ \"match\": { \"learningResourceType\" : \"")
					.Append(nlrSearchParameters.learningResourceType)
					.Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.accessibilityControl))
			{
				sb.Append("{ \"match\": { \"accessibilityControl\" : \"")
					.Append(nlrSearchParameters.accessibilityControl)
					.Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.typicalAgeRange))
			{
                sb.Append("{ \"match\": { \"typicalAgeRange\" : \"")
                    .Append(nlrSearchParameters.typicalAgeRange)
                    .Append("\" } },");
			}

			if (nlrSearchParameters.educationalRole.Count > 0)
			{
				string educationalRoles = string.Join(" ", nlrSearchParameters.educationalRole.ToArray());

				sb.Append("{ \"match\": { \"audience\" : \"")
					.Append(educationalRoles.ToLower())
					.Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.usageRightsURL))
			{

				if (nlrSearchParameters.usageRightsURL.Equals("Custom", StringComparison.OrdinalIgnoreCase))
				{
					sb.Append("{\"wildcard\": { \"usageRightsURL\": { \"value\": \"*\" }}},");
			}
				else
				{
					sb.Append("{ \"match\": { \"usageRightsURL\" : \"")
						.Append(nlrSearchParameters.usageRightsURL)
						.Append("\" } },");
				}
			}

			if (nlrSearchParameters.educationalRole.Count > 0)
			{
				string educationalRoles = string.Join(" ", nlrSearchParameters.educationalRole.ToArray());

				sb.Append("{ \"match\": { \"audience\" : \"")
					.Append(educationalRoles.ToLower())
					.Append("\" } },");
			}

			if (!string.IsNullOrEmpty(nlrSearchParameters.usageRightsURL))
			{

				if (nlrSearchParameters.usageRightsURL.Equals("Custom", StringComparison.OrdinalIgnoreCase))
				{
					sb.Append("{\"wildcard\": { \"usageRightsURL\": { \"value\": \"*\" }}},");
				}
				else
				{
					sb.Append("{ \"match\": { \"usageRightsURL\" : \"")
						.Append(nlrSearchParameters.usageRightsURL)
						.Append("\" } },");
				}
			}

			//remove trailing comma...
			var index = sb.ToString().LastIndexOf(',');
			if (index >= 0)
				sb.Remove(index, 1);
			//////////////////////////
			sb.Append("]");
			sb.Append("}},\"size\": ").Append(NationalLearningRegistryMaxResults.ToString());
			sb.Append("}");

			return sb.ToString();
		}

		private static string MapLanguage(string language)
		{
			switch (language.ToUpper())
			{
				case "ENGLISH":
					return "en eng english";
				case "SPANISH":
					return "sp spa spanish";
				case "HINDI":
					return "hi hin hindi";
				case "ARABIC":
					return "ar ara arabic";
				default:
					return language;
			}
		}

		private List<Resource> BuildLearningRegistryResultList(EsQueryResult esQuerySearchResults)
		{

			List<EsQueryResultRow> searchResults = esQuerySearchResults.QueryResults;

			var list = new List<Resource>();
			foreach (EsQueryResultRow row in searchResults)
			{
				var resource = new Resource
				{
					Source = NationalLearningRegistry,
					//ID = DataIntegrity.ConvertToInt(row.IntID),
					ResourceName = row.Title,
					Type = row.Type,
					ID_Encrypted = row.URL,
					ViewLink = String.Empty,
					Description = row.Description,

					Publisher = row.Publisher,
					URL = row.URL,
					Creator = row.Author + "<br>" + row.Creator,
					Created = row.Created,
					UsageRightsURL = row.UsageRightsURL,
					ViewsCount = row.ViewsCount
				};
				list.Add(resource);
			}

			return list;
		}


		/// <summary>
		///  AddResource
		/// </summary>
		/// <param name="list"></param>
		/// <param name="resource"></param>
		private void AddResource(List<Resource> list, Resource resource)
		{
			if (list.SingleOrDefault(r => r.DocumentNodeID == resource.DocumentNodeID) == null)
			{
				list.Add(resource);
			}
		}

		/// <summary>
		/// Filter Resource List By Tag Selections
		/// </summary>
		/// <param name="resultResource"></param>
		/// <param name="tagCriteriaSelectionParameters"></param>
		/// <returns>List</returns>
		private List<Resource> FilterResourcesByTags(List<Resource> resultResource,
			TagCriteriaSelectionParameters tagCriteriaSelectionParameters)
		{
			if (resultResource == null || resultResource.Count == 0)
				return null;

			List<Resource> newResourcesList = new List<Resource>();
            List<CmsResourceTagAssociationDetails> associatedLrmiDetails =
                GetResourcesWithTagAssociations(resultResource);

			if (associatedLrmiDetails == null || associatedLrmiDetails.Count == 0)
				return new List<Resource>();

			const string defaultStandardId = "0";

			resultResource.ForEach(res =>
			{
				// Retrieve all educational alignments for a resource.
				List<CmsResourceTagAssociationDetails> tagResources =
					associatedLrmiDetails.FindAll(
						r =>
							r.ResourceId == res.DocumentForeignKeyValue &&
							r.ClassName.Equals(res.DocumentType, StringComparison.InvariantCultureIgnoreCase));
				if (tagResources.Count > 0)
				{
					bool includeAssessed = true;
					bool includeTeaches = true;
					bool includeRequires = true;
					bool includeEs = true;
					bool includeEl = true;
					bool includeLearningResource = true;
					bool includeTc = true;
					bool includeRl = true;
					bool includeAgeAppropriate = true;
					bool includeUrUrl = true;
					bool includeEducationalUse = true;
					bool includeEndUser = true;
					bool includeInteractivity = true;
					bool includeMediaType = true;
					bool includeLanguage = true;
					bool includeDuration = true;
					bool includeCreator = true;
					bool includePublisher = true;

					if (tagCriteriaSelectionParameters.AssessedStandardIds.Count > 0)
					{
						string[] assessedStdIds =
                            tagCriteriaSelectionParameters.AssessedStandardIds.Select(
                                i => i.ToString(CultureInfo.InvariantCulture))
								.ToArray();
                        includeAssessed = LRMIListInclude(assessedStdIds, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.Assessed));
					}
					if (tagCriteriaSelectionParameters.TeachesStandardIds.Count > 0)
					{
						string[] teachesStdIds =
                            tagCriteriaSelectionParameters.TeachesStandardIds.Select(
                                i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
                        includeTeaches = LRMIListInclude(teachesStdIds, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.Teaches));
					}
					if (tagCriteriaSelectionParameters.RequiresStandardIds.Count > 0)
					{
						string[] requiresStdIds =
                            tagCriteriaSelectionParameters.RequiresStandardIds.Select(
                                i => i.ToString(CultureInfo.InvariantCulture))
								.ToArray();
                        includeRequires = LRMIListInclude(requiresStdIds, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.Requires));
					}
					if (tagCriteriaSelectionParameters.EducationalSubject.Count > 0)
					{
						string[] educationalSubject =
                            tagCriteriaSelectionParameters.EducationalSubject.Select(
                                i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
                        includeEs = LRMIListInclude(educationalSubject, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.EducationalSubject));
					}
					if (tagCriteriaSelectionParameters.GradeLevel.Count > 0)
					{
						string[] gradeLevels =
                            tagCriteriaSelectionParameters.GradeLevel.Select(
                                i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
                        includeEl = LRMIListInclude(gradeLevels, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.EducationalLevel));
					}
					if (tagCriteriaSelectionParameters.LearningResourceType.Count > 0)
					{
						string[] learningResources =
                            tagCriteriaSelectionParameters.LearningResourceType.Select(
                                i => i.ToString(CultureInfo.InvariantCulture))
								.ToArray();
						includeLearningResource = LRMIListInclude(learningResources, tagResources,
							Convert.ToInt32(Enums.LrmiTags.LearningResourceType));
					}
					if (tagCriteriaSelectionParameters.EducationalUse.Count > 0)
					{
						string[] educationalUse =
                            tagCriteriaSelectionParameters.EducationalUse.Select(
                                i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
						includeEducationalUse = LRMIListInclude(educationalUse, tagResources,
							Convert.ToInt32(Enums.LrmiTags.EducationalUse));
					}
					if (tagCriteriaSelectionParameters.EndUser.Count > 0)
					{
						string[] endUser =
                            tagCriteriaSelectionParameters.EndUser.Select(i => i.ToString(CultureInfo.InvariantCulture))
                                .ToArray();
						includeEndUser = LRMIListInclude(endUser, tagResources, Convert.ToInt32(Enums.LrmiTags.EndUser));
					}
					if (tagCriteriaSelectionParameters.InteractivityType.Count > 0)
					{
						string[] interactivityType =
                            tagCriteriaSelectionParameters.InteractivityType.Select(
                                i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
                        includeInteractivity = LRMIListInclude(interactivityType, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.Activity));
					}
					if (tagCriteriaSelectionParameters.MediaType.Count > 0)
					{
						string[] mediaType =
                            tagCriteriaSelectionParameters.MediaType.Select(
                                i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
                        includeMediaType = LRMIListInclude(mediaType, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.MediaType));
					}

					if (!string.IsNullOrWhiteSpace(tagCriteriaSelectionParameters.TextComplexity))
					{
						string[] textComplexity = { tagCriteriaSelectionParameters.TextComplexity };
                        includeTc = LRMIListInclude(textComplexity, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.TextComplexity));
					}
					if (!string.IsNullOrWhiteSpace(tagCriteriaSelectionParameters.ReadingLevel))
					{
						string[] readingLevel = { tagCriteriaSelectionParameters.ReadingLevel };
                        includeRl = LRMIListInclude(readingLevel, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.ReadingLevel));
					}
                    if (tagCriteriaSelectionParameters.AgeAppropriate.HasValue &&
                        tagCriteriaSelectionParameters.AgeAppropriate > 0)
					{
						string[] ageAppropriate = { tagCriteriaSelectionParameters.AgeAppropriate.ToString() };
						includeAgeAppropriate = LRMIListInclude(ageAppropriate, tagResources,
							Convert.ToInt32(Enums.LrmiTags.AgeAppropriate));
					}
                    if (tagCriteriaSelectionParameters.UseRightUrl.HasValue &&
                        tagCriteriaSelectionParameters.UseRightUrl > 0)
					{
						string[] usageRights = { tagCriteriaSelectionParameters.UseRightUrl.ToString() };
                        includeUrUrl = LRMIListInclude(usageRights, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.UsageRights));
					}
					if (tagCriteriaSelectionParameters.Language.HasValue && tagCriteriaSelectionParameters.Language >= 0)
					{
						string[] language = { tagCriteriaSelectionParameters.Language.ToString() };
                        includeLanguage = LRMIListInclude(language, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.Language));
					}
					if (!string.IsNullOrWhiteSpace(tagCriteriaSelectionParameters.Creator))
					{
						string[] creator = { tagCriteriaSelectionParameters.Creator };
						includeCreator = LRMIListInclude(creator, tagResources, Convert.ToInt32(Enums.LrmiTags.Creator));
					}
					if (!string.IsNullOrWhiteSpace(tagCriteriaSelectionParameters.Publisher))
					{
						string[] publisher = { tagCriteriaSelectionParameters.Publisher };
                        includePublisher = LRMIListInclude(publisher, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.Publisher));
					}
                    if (tagCriteriaSelectionParameters.TimeRequiredDays.HasValue &&
                        tagCriteriaSelectionParameters.TimeRequiredDays > 0 ||
						tagCriteriaSelectionParameters.TimeRequiredHours.HasValue &&
						tagCriteriaSelectionParameters.TimeRequiredHours > 0 ||
						tagCriteriaSelectionParameters.TimeRequiredMinutes.HasValue &&
						tagCriteriaSelectionParameters.TimeRequiredMinutes > 0)
					{
						string[] timeRequired =
						{
							tagCriteriaSelectionParameters.TimeRequiredDays + ":" +
							tagCriteriaSelectionParameters.TimeRequiredHours + ":" +
							tagCriteriaSelectionParameters.TimeRequiredMinutes
						};
                        includeDuration = LRMIListInclude(timeRequired, tagResources,
                            Convert.ToInt32(Enums.LrmiTags.TimeRequired));
					}


					bool includeInSearch = includeTc && includeRl && includeEs && includeEl;
					includeInSearch = includeInSearch && includeAssessed && includeTeaches && includeRequires;
					includeInSearch = includeInSearch && includeDuration && includeAgeAppropriate && includeUrUrl;
					includeInSearch = includeInSearch && includeInteractivity && includeMediaType && includeLanguage;
                    includeInSearch = includeInSearch && includeLearningResource && includeEducationalUse &&
                                      includeEndUser;
					includeInSearch = includeInSearch && includeCreator && includePublisher;
					if (includeInSearch)
					{
						AddResource(newResourcesList, res);
					}
				}

			});

			return newResourcesList;
		}

		private bool LRMIListInclude(string[] standardSetList, List<CmsResourceTagAssociationDetails> tagResources,
			int educationalAlignmentEnum)
		{
			if (standardSetList.Length > 0)
			{
				List<CmsResourceTagAssociationDetails> resourceRequires = (from tagResource in tagResources
																		   let tagResourceRequires =
																			   tagResources.Where(
																				   x =>
																					   x.EducationalAlignmentEnum == educationalAlignmentEnum &&
																					   standardSetList.Contains(x.EducationAlignmentValues))
																		   from tagResourceRequires1 in tagResourceRequires
																		   where
																			   tagResource.EducationalAlignmentEnum == educationalAlignmentEnum &&
																			   tagResource.EducationAlignmentValues == tagResourceRequires1.EducationAlignmentValues
																		   select tagResource).ToList();

				if (resourceRequires.Count == 0)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// GetCurriculumsIDsFromSelectedCriteria
		/// </summary>
		/// <param name="selectedCurriculum"></param>
		/// <returns>int[]</returns>
		private List<object> GetCurriculumsIDsFromSelectedCriteria(CurriculumSelections selectedCurriculum)
		{
			List<object> idFields = new List<object>();


			string sql = "SELECT DISTINCT ID From CurrCourses WHERE 1 = 1";
			string grade = selectedCurriculum.Grade;
			string subject = selectedCurriculum.Subject;
			string course = selectedCurriculum.Course;

			if (!string.IsNullOrWhiteSpace(grade))
			{
				sql += " And (Grade = '" + grade.Trim() + "')";
			}
			if (!string.IsNullOrWhiteSpace(subject))
			{
				sql += " And (Subject = '" + subject.Trim() + "')";
			}
			if (!string.IsNullOrWhiteSpace(course))
			{
				sql += " And (Course = '" + course.Trim() + "')";
			}

			DataTable currIDsTable = CriteriaHelper.GetDataTable(_rootConnectionString, sql);
			if (currIDsTable != null)
			{
				idFields = (from lstcur in currIDsTable.AsEnumerable()
							select lstcur["ID"]).ToList();
			}

			return idFields;
		}

		/// <summary>
		/// RadGridResults_PageIndexChanged
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RadGridResults_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			radGridResults.DataSource = SearchResourcesWithCriteria(false);
			radGridResults.DataBind();
		}

		protected void RadGridResults_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			radGridResults.DataSource = SearchResourcesWithCriteria(false);
			radGridResults.DataBind();
		}

		private void ShowMDSButtons(GridDataItem item, bool showTheButtons)
		{
            // ------------------------------------------
            // check to make sure the user have permissions to add otherwise do not show button
            if (Session["KenticoUserInfo"] == null) return;

            var kenticoUserInfo = (UserInfo)Session["KenticoUserInfo"];

            var userInfo = UserInfoProvider.GetUserInfo(kenticoUserInfo.UserName);
            var currentUser = new CurrentUserInfo(userInfo, true);

            if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "EditForm"))
            {
                var button = (ImageButton)item.FindControl("AddAsNewMyDocsResource");
                if (button != null)
                {
                    button.Enabled = false;
                    button.CssClass = "disabledButton";
                }
                //item.FindControl("AddAsNewMyDocsResource").Visible = false;
                return;
            }
            // ------------------------------------------

			item.FindControl("AddAsNewMyDocsResource").Visible = showTheButtons;
			//item.FindControl("AddAsNewDistrictResource").Visible = showTheButtons;
			//item.FindControl("AddAsNewStateResource").Visible = showTheButtons;
		}

		protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
		{
			var gridDataItem = e.Item as GridDataItem;
            if (gridDataItem == null)
            {
                radGridResults.MasterTableView.GetColumnSafe("ResourceNameReadOnly").Visible = false;
                return;
            }
			if (gridDataItem != null)
			{
				var item = gridDataItem;
                var image1 = (Image)item.FindControl("Image1");
                var viewLinkAnchor = (HyperLink)item.FindControl("ViewLinkAnchor");
                var lblExpirationDate = (Label)item.FindControl("lblExpirationDate");
                var lblResourceName = (Label)item.FindControl("lblResourceName");
               
                
                var lblAverageRating = (Label)item.FindControl("lblAverageRating");
				ShowMDSButtons(item, false);
				//var addAsNewResourceButton = (Button) item.FindControl("AddAsNewResourceButton");
				//addAsNewResourceButton.Visible = false;

				var itemResource = (Resource)item.DataItem;

				if (itemResource.ViewLink == "")
				{
					image1.Visible = false;
				}

			    lblExpirationDate.Text = itemResource.ExpirationDate == default(DateTime) ? "" : itemResource.ExpirationDate.ToString("MM/dd/yyyy");
                lblAverageRating.Text = itemResource.AverageRating == default(Decimal) ? "No Rating" : itemResource.AverageRating.ToString("0.00");

				if (itemResource.Source == PBSLearningMedia)
				{
					viewLinkAnchor.Visible = true;
					viewLinkAnchor.CssClass = "tooltip-block";
					viewLinkAnchor.Attributes.Add("onclick",
						"Resource_onClick('" + itemResource.Source + "','" + itemResource.URL + "','" +
						itemResource.NodeAliasPath + "');");
                    ShowMDSButtons(item, true);
				}

				if (itemResource.Source == NationalLearningRegistry)
				{
					viewLinkAnchor.Visible = true;
					viewLinkAnchor.CssClass = "tooltip-block";
					viewLinkAnchor.Attributes.Add("onclick",
						"Resource_onClick('" + itemResource.Source + "','" + itemResource.URL + "');");

					ShowMDSButtons(item, true);

					//TODO: Remove/hide columns not valid for NLR
					radGridResults.MasterTableView.GetColumnSafe("Created").Visible = true;
					radGridResults.MasterTableView.GetColumnSafe("Creator").Visible = true;
					radGridResults.MasterTableView.GetColumnSafe("Publisher").Visible = true;

					radGridResults.MasterTableView.GetColumnSafe("Type").Visible = false;
					radGridResults.MasterTableView.GetColumnSafe("Subtype").Visible = false;
                    radGridResults.MasterTableView.GetColumnSafe("ExpirationDate").Visible = false;
                    radGridResults.MasterTableView.GetColumnSafe("AverageRating").Visible = false;
				}
				else
				{
					radGridResults.MasterTableView.GetColumnSafe("Created").Visible = false;
					radGridResults.MasterTableView.GetColumnSafe("Creator").Visible = false;
					radGridResults.MasterTableView.GetColumnSafe("Publisher").Visible = false;

					radGridResults.MasterTableView.GetColumnSafe("Type").Visible = true;
					radGridResults.MasterTableView.GetColumnSafe("Subtype").Visible = true;
                    radGridResults.MasterTableView.GetColumnSafe("ExpirationDate").Visible = true;
                    radGridResults.MasterTableView.GetColumnSafe("AverageRating").Visible = true;
				}


                if (isInstructionMaterial == true)
                {
                    radGridResults.MasterTableView.GetColumnSafe("ResourceNameReadOnly").Visible = true;                  
                    radGridResults.MasterTableView.GetColumnSafe("StudentAssignment").Visible = true;                                       
                    radGridResults.MasterTableView.GetColumnSafe("Name").Visible = false;                  
                }
                else
                {
                    radGridResults.MasterTableView.GetColumnSafe("ResourceNameReadOnly").Visible = false;                    
                    radGridResults.MasterTableView.GetColumnSafe("StudentAssignment").Visible = false;                   
                    radGridResults.MasterTableView.GetColumnSafe("Name").Visible = true;
                           
                }

				foreach (GridColumn col in radGridResults.MasterTableView.RenderColumns)
				{
					if (col.UniqueName == "Description")
					{
						GridDataItem dataItem = gridDataItem;
                        dataItem[col.UniqueName].Attributes.Add("onclick", "ResizeColumn('" + col.UniqueName + "');");
						dataItem[col.UniqueName].CssClass = "tooltip-block";
						dataItem[col.UniqueName].ToolTip = "Widen/Compress Column";
					}
				}
			}
		}

		protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
		{
			if (SourceCriteriaExists())
			{
			radGridResults.DataSource = SearchResourcesWithCriteria(false);
			radGridResults.DataBind();
		}
		}

		protected void RadGrid_NeedDataSource(object sender, EventArgs e)
		{
		}


		protected void gridResources_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			radGridResults.DataSource = SearchResourcesWithCriteria(false);
			radGridResults.DataBind();
		}

		protected void btnReset_Click(object sender, EventArgs e)
		{
			ResetCriteria();
			ResetGrid();
		}

		#region Private Methods

		private bool SourceCriteriaExists()
		{
			Criteria criteria = SearchControl.GetCriteria();

			foreach (Criterion cr in criteria.Criterias)
			{
				if ((!string.IsNullOrEmpty(cr.Key) && cr.Key == "Source") && (!string.IsNullOrEmpty(cr.Value.Value)))
				{
					return true;
				}

			}
			return false;
		}

		private void ResetCriteria()
		{
			SearchControl.Criteria.Criterias.Clear();
		}

		private void ResetGrid()
		{
			//List<ExampleDataDTO> gridData = new List<ExampleDataDTO>();
			radGridResults.MasterTableView.SortExpressions.Clear();
			radGridResults.DataSource = new string[] { };
			radGridResults.DataBind();
			UpdatePanelSearchResults.Update();
		}

		private void LoadCriteria()
		{

			_resourceTypes = KenticoHelper.GetLookupDetailsByClient(null, null);

			List<KeyValuePair> lstsel = new List<KeyValuePair>();
			lstsel.Add(new KeyValuePair { Key = "0", Value = "-- Select --" });

			var resulttype = (from s in _resourceTypes
							  where s.LookupEnum == LookupType.DocumentType
                              select new KeyValuePair { Key = s.Enum.ToString(CultureInfo.InvariantCulture), Value = s.Description })
                .ToList();

			var educationalUses = GetTagElements(Enums.LrmiTags.EducationalUse);
			var activityTypes = GetTagElements(Enums.LrmiTags.Activity);
			var learningResourceTypes = GetTagElements(Enums.LrmiTags.LearningResourceType);
			var targetTypes = GetTagElements(Enums.LrmiTags.EndUser);
			var mediaTypes = GetTagElements(Enums.LrmiTags.MediaType);
			var ageAppropriateTypes = GetTagElements(Enums.LrmiTags.AgeAppropriate);
			var usageRightTypes = GetCommonCreative();

            // create JSON objects for Media Type for LRMI and PBS Learning so that the client can switch between source types
            List<KeyValuePair> mediaTypeList = new List<KeyValuePair>();
            var mediaTypeEnum = Enum.GetValues(typeof(MediaType))
                .Cast<MediaType>()
                .Select(v => v.ToString())
                .ToList();
            foreach (string mt in mediaTypeEnum)
            {
                mediaTypeList.Add(new KeyValuePair { Value = mt, Key = Convert.ToString((int)Enum.Parse(typeof(MediaType), mt)) });
            }
		    var lrmiMediaJsonObject = Json.Encode(mediaTypes);
		    var pbsMediaJsonOjbect = Json.Encode(mediaTypeList);
            ClientScriptManager clientScript = Page.ClientScript;
            clientScript.RegisterStartupScript(GetType(), "LRMIMediaJSONObject",
                            "var lrmiMedicaJsonObject = " + lrmiMediaJsonObject.ToString(CultureInfo.InvariantCulture) + ";", true);
            clientScript.RegisterStartupScript(GetType(), "PBSMediaJSONObject",
                            "var pbsMedicaJsonObject = " + pbsMediaJsonOjbect.ToString(CultureInfo.InvariantCulture) + ";", true);


			// TFS-2887
			List<KeyValuePair> srcsel = new List<KeyValuePair>();
			List<KeyValuePair> sourcetype = new List<KeyValuePair>();
			List<KeyValuePair> gradeList = GetGradesList();

			DataSet repositoryDataSet = Repository.GetRepository();

           if ((_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_DISTRICTADMIN, StringComparison.InvariantCultureIgnoreCase)) == null) && (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_SCHOOLADMIN, StringComparison.InvariantCultureIgnoreCase)) == null))
            {
                foreach (var repDataRow in repositoryDataSet.Tables[0].Select())
                {
                    if (repDataRow["repositoryName"].ToString() == TeacherMyDocsFolder.ToString())
                    {
                        repositoryDataSet.Tables[0].Rows.Remove(repDataRow);
                    }
                }
            }


			foreach (DataRow row in repositoryDataSet.Tables[0].Rows)
			{
                sourcetype.Add(new KeyValuePair
                {
                    Value = row["repositoryName"].ToString(),
                    Key = (row["repositoryName"].ToString() == TeacherMyDocsFolder.ToString() ? "TeachersMyDocsfolder" : row["repositoryName"].ToString())
                });
			}

			// Begin source changes TFS-2887

			//Source Criteria
			srcsel.Add(new KeyValuePair { Key = "0", Value = "-- Select --" });            
			srcsel.AddRange(sourcetype);

			Criterion sourceCriteria = new Criterion
			{
				UIType = UIType.DropDownList,
				Key = "Source",
				Header = "Source:",
				DataTextField = "Value",
				DataValueField = "Key",
                DataSource = srcsel,
                IsRequired = true
			};

			SearchControl.Criteria.Criterias.Add(sourceCriteria);

			// End source changes TFS-2887


            //Begin Teacher Control
            #region School Name as loggedin user

            DataTable dtSchoolList = new DataTable();
            dtSchoolList.Columns.Add("Key", typeof(string));
            dtSchoolList.Columns.Add("Value", typeof(int));

            var schoolList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);

            if (SessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_SCHOOLADMIN, StringComparison.InvariantCultureIgnoreCase)) != null && SessionObject.LoggedInUser.Schools.Count > 1)
            {
                DataRow dr = dtSchoolList.NewRow();

                var key = "School_" + SessionObject.LoggedInUser.School;

                if (!RecordExistsInCache(key)) return;
                var schoolText = ((Base.Classes.School)Base.Classes.Cache.Get(key)).Name;

                dr["Key"] = schoolText;
                dr["Value"] = SessionObject.LoggedInUser.School;
                dtSchoolList.Rows.Add(dr);
            }
            else if (schoolList != null)
            {
                foreach (Base.Classes.School school in schoolList)
                {
                    DataRow dr = dtSchoolList.NewRow();
                    dr["Key"] = school.Name;
                    dr["Value"] = school.ID;
                    dtSchoolList.Rows.Add(dr);
                }
            }

            List<KeyValuePair<String, int>> schoolSetList = new List<KeyValuePair<string, int>>();
            foreach (DataRow dr in dtSchoolList.Rows)
            {
                schoolSetList.Add(new KeyValuePair<string, int>(dr["Key"].ToString(), Convert.ToInt16(dr["Value"])));
            }
            if (schoolSetList.Count > 1)
            {
                schoolSetList.Insert(0, new KeyValuePair<string, int>("Select School", 0));
            }
           

            Criterion teacherCriteria = new Criterion
            {
                UIType = UIType.SchoolGradeName,
                Key = "SchoolGradeName",
                Header = "Teacher:",
                SchoolDataSource = schoolSetList,
                GradesDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Grade") },
                TeacherNameDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Name") },
                IsRequired = true                
            };

            SearchControl.Criteria.Criterias.Add(teacherCriteria);
            #endregion
            //End Teacher Control


			var typeValue = (from s in resulttype where s.Key == _typeKey select s.Value);
			//Type Criteria

			lstsel.AddRange(resulttype);

			Criterion typeCriteria = new Criterion
			{
				UIType = UIType.DropDownList,
				Key = "Type",
				Header = "Type:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = lstsel
			};
			var typeValueenumerable = typeValue as IList<string> ?? typeValue.ToList();
			if (typeValueenumerable.Any())
			{
				typeCriteria.DefaultValue = new KeyValuePair(_typeKey, typeValueenumerable.ElementAt(0));
			}

			//SubType Criteria

			var resultsubtype = (from s in _resourceTypes
								 where Convert.ToInt32(s.LookupEnum) >= 43 && Convert.ToInt32(s.LookupEnum) <= 51
                                 select new KeyValuePair { Key = s.Enum.ToString(CultureInfo.InvariantCulture), Value = s.Description })
                .ToList();
			resultsubtype.Insert(0, new KeyValuePair("0", "-- Select --"));

			Criterion subtypeCriteria = new Criterion
			{
				UIType = UIType.DropDownList,
				Key = "SubType",
				Header = "SubType:",
				DataTextField = "Value",
				DataValueField = "Key",
				HandlerName = "populatesubtype"
			};
			subtypeCriteria.Dependencies.Add(Criterion.CreateDependency("Type"));
			subtypeCriteria.DataSource = resultsubtype;


			//Resource Name

			Criterion textCriteria = new Criterion
			{
				UIType = UIType.TextBoxEdit,
				Key = "Text",
				Header = "Text Search:",
				DataTextField = "Value",
				DataValueField = "Key",
				EditMask = EditMaskType.NoMask
				//DataSource = GetTextSearchOptions()
			};

			Criterion keywordsCriteria = new Criterion
			{
				UIType = UIType.TextBoxEdit,
				Key = "Keywords",
				Header = "Keywords:",
				DataTextField = "Value",
				DataValueField = "Key",
				EditMask = EditMaskType.NoMask
			};

			SearchControl.Criteria.Criterias.Add(typeCriteria);
			SearchControl.Criteria.Criterias.Add(subtypeCriteria);
			SearchControl.Criteria.Criterias.Add(textCriteria);
			SearchControl.Criteria.Criterias.Add(keywordsCriteria);

			//Criterion uiTableCriteria = new Criterion {UIType = Base.Enums.UIType.UITable};
			//SearchControl.Criteria.Criterias.Add(uiTableCriteria);

			List<KeyValuePair> standardSetList = GetStandardSetList();
			standardSetList.Insert(0, new KeyValuePair("0", "Select Set"));

			SessionObject sessionObject = (SessionObject)Session["SessionObject"];
			var courses = CourseMasterList.GetStandardCoursesForUser(sessionObject.LoggedInUser);
			courses.AddRange(CourseMasterList.GetClassCoursesForUser(sessionObject.LoggedInUser));
			courses.AddRange(CourseMasterList.GetCurrCoursesForUser(sessionObject.LoggedInUser));
			var allCourses = courses.AsEnumerable<Base.Classes.Course>().ToList();

			var allSubjects = allCourses.GroupBy(g => new { g.Subject.DisplayText });
			var subjects = (from s in allSubjects
							select new KeyValuePair
							{
								Key = s.Key.DisplayText,
								Value = s.Key.DisplayText,
							}).OrderBy(s => s.Value).ToList();
			subjects.Insert(0, new KeyValuePair("0", "Select Subject"));

			var allCurriculums = allCourses.GroupBy(g => new { g.CourseName });
			var listCourses = (from s in allCurriculums
							   select new KeyValuePair
							   {
								   Key = s.Key.CourseName,
								   Value = s.Key.CourseName,
							   }).OrderBy(s => s.Value).ToList();
			listCourses.Insert(0, new KeyValuePair("0", "Select Curriculum"));

			SearchControl.ToolTipDelayHide = 20; //Seconds
			// Curricula Criteria Control
			Criterion curriculaCriteria = new Criterion
			{
				UIType = UIType.GradeSubjectCurricula,
				Key = "Curriculum",
				Header = "Curriculum:",
				GradesDataSource = gradeList,
				SubjectsDataSource = subjects,
				CurriculaDataSource = listCourses
			};

			SearchControl.Criteria.Criterias.Add(curriculaCriteria);

			//Standard Criteria Controls
			Criterion standardCriteria = new Criterion
			{
				UIType = UIType.GradeSubjectStandards,
				Key = "Standards",
				Header = "Standards:",
				StandardSetDataSource = standardSetList,
				GradesDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Grade") },
				SubjectsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Subject") },
				CourseDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Course") },
				StandardsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Standards") }
			};

			SearchControl.Criteria.Criterias.Add(standardCriteria);

            /* Criteria Pre-Selection For Suggested Resources */
            if (_from.Equals("suggestedresources", StringComparison.InvariantCultureIgnoreCase))
            {
                Criterion sourceCriteriaSR = this.SearchControl.Criteria.Criterias.Find(c => c.Key.Equals("Source"));
                Criterion standardsCriteriaSR = this.SearchControl.Criteria.Criterias.Find(c => c.Key.Equals("Standards"));

                if (sourceCriteriaSR != null)
                {
                    sourceCriteriaSR.DefaultValue = new KeyValuePair("MyDocs", "MyDocs");
                }
                if (standardsCriteriaSR != null)
                {
                    _standard = Base.Classes.Standards.GetStandardByID(int.Parse(_selectedStandardId));
                    string props = (new JavaScriptSerializer()).Serialize(new { StandardId = _standard.ID, StandardSet = _standard.Standard_Set, StandardName = _standard.StandardName, Grade = _standard.Grade, Subject = _standard.Subject, CourseName = _standard.Course });
                    standardsCriteriaSR.DefaultValue = new KeyValuePair(_standard.ID.ToString(), props);
                }
            }

            #region Criterion: Expiration Date
            Criterion expirationDate = new Criterion
            {
                Header = "Expiration Date",
                Key = "ExpirationDate",
                Locked = false,
                UIType = UIType.DateRange
            };

           
            #endregion
            SearchControl.Criteria.Criterias.Add(expirationDate);

            List<KeyValuePair> ratingList = new List<KeyValuePair>();
		    List<RatingRange> ratingRange = _reviewFacade.GetRatingFilterForItemSearch().ToList();
            foreach (RatingRange r in ratingRange)
            {
                ratingList.Add(new KeyValuePair { Key = r.Id.ToString(), Value = r.DisplayValue });
            }
           
            // Rating
            Criterion ratingCriteria = new Criterion
            {
                UIType = UIType.CheckBoxList,
                Key = "AverageRating",
                Header = "Average Rating:",
                DataTextField = "Value",
                DataValueField = "Key",
                DataSource = ratingList
            };
            SearchControl.Criteria.Criterias.Add(ratingCriteria);

			// Assessed
			Criterion assessedCriteria = new Criterion
			{
				UIType = UIType.GradeSubjectStandards,
				Key = "Assessed",
				Header = "Assessed:",
				StandardSetDataSource = standardSetList,
				GradesDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Grade") },
				SubjectsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Subject") },
				CourseDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Course") },
				StandardsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Standards") }
			};
			// Teaches
			Criterion teachesCriteria = new Criterion
			{
				UIType = UIType.GradeSubjectStandards,
				Key = "Teaches",
				Header = "Teaches:",
				StandardSetDataSource = standardSetList,
				GradesDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Grade") },
				SubjectsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Subject") },
				CourseDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Course") },
				StandardsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Standards") }
			};
			// Requires
			Criterion requiresCriteria = new Criterion
			{
				UIType = UIType.GradeSubjectStandards,
				Key = "Requires",
				Header = "Requires:",
				StandardSetDataSource = standardSetList,
				GradesDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Grade") },
				SubjectsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Subject") },
				CourseDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Course") },
				StandardsDataSource = new List<KeyValuePair> { new KeyValuePair("0", "Select Standards") }
			};
			// Text Complexity
			Criterion textComplexityCriteria = new Criterion
			{
				UIType = UIType.TextBoxEdit,
				Key = "TextComplexity",
				Header = "Text&nbsp;Complexity:",
				DataTextField = "Value",
				DataValueField = "Key",
				EditMask = EditMaskType.NoMask
			};
			// Reading Level
			Criterion readingLevelCriteria = new Criterion
			{
				UIType = UIType.TextBoxEdit,
				Key = "ReadingLevel",
				Header = "Reading&nbsp;Level:",
				DataTextField = "Value",
				DataValueField = "Key",
				EditMask = EditMaskType.NoMask
			};
			// Educational Subject

			List<KeyValuePair> edSubjectList = new List<KeyValuePair>();

			CourseList currCourseList = CourseMasterList.GetCurrCoursesForUser(_sessionObject.LoggedInUser);
			IEnumerable<Subject> subjectList = currCourseList.GetSubjectList().OrderBy(sub => sub.DisplayText);
			foreach (Subject sList in subjectList)
			{
				edSubjectList.Add(new KeyValuePair { Key = sList.DisplayText, Value = sList.DisplayText });
			}

			Criterion educationalSubjectCriteria = new Criterion
			{
				UIType = UIType.CheckBoxList,
				Key = "EducationalSubject",
				Header = "Educational&nbsp;Subject:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = edSubjectList
			};
			// Grades
			Criterion gradeCriteria = new Criterion
			{
				UIType = UIType.CheckBoxList,
				Key = "Grades",
				Header = "Grades:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = gradeList
			};
			// Instruction Type
			Criterion EducationalUse = new Criterion
			{
				UIType = UIType.CheckBoxList,
				Key = "EducationalUse",
				Header = "Eductional&nbsp;Use:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = educationalUses
			};
			// Activity Type
			Criterion interactivityTypeCriteria = new Criterion
			{
				UIType = UIType.CheckBoxList,
				Key = "InteractivityType",
				Header = "Interactivity&nbsp;Type:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = activityTypes
			};
			// Learning Resource Type
			Criterion learningResourceCriteria = new Criterion
			{
				UIType = UIType.CheckBoxList,
				Key = "LearningResource",
				Header = "Learning&nbsp;Resource:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = learningResourceTypes
			};
			// Target Audience
			Criterion endUser = new Criterion
			{
				UIType = UIType.CheckBoxList,
				Key = "EndUser",
				Header = "End&nbsp;User:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = targetTypes
			};
			// Duration
			Criterion durationCriteria = new Criterion
			{
				UIType = UIType.Duration,
				Key = "Duration",
				Header = "Time&nbsp;Required:",
				DataTextField = "Value",
				DataValueField = "Key",
				EditMask = EditMaskType.NumericMask
			};
			// Age Appropriate
			ageAppropriateTypes.Add(new KeyValuePair { Key = "0", Value = "-- Select --" });
			ageAppropriateTypes = ageAppropriateTypes.OrderBy(o => o.Key).ToList();
			Criterion ageAppropriateCriteria = new Criterion
			{
				UIType = UIType.DropDownList,
				Key = "AgeAppropriate",
				Header = "Age&nbsp;Appropriate:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = ageAppropriateTypes
			};
			// Media Type
			//List<KeyValuePair> mediaTypeList = new List<KeyValuePair>();
			//mediaTypeList.Add(new KeyValuePair { Key = "0", Value = "-- Select --" });
			//var mediaType = Enum.GetValues(typeof(MediaType))
			//    .Cast<MediaType>()
			//    .Select(v => v.ToString())
			//    .ToList();
			//foreach (string mt in mediaType)
			//{
			//    mediaTypeList.Add(new KeyValuePair { Key = mt, Value = mt });
			//}

			Criterion mediaCriteria = new Criterion
			{
				UIType = UIType.CheckBoxList,
				Key = "MediaType",
				Header = "Media&nbsp;Type:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = mediaTypes
			};

			// Language Type
			List<KeyValuePair> languageTypeList = new List<KeyValuePair>();
			languageTypeList.Add(new KeyValuePair { Key = "0", Value = "-- Select --" });
			var languageType = Enum.GetValues(typeof(LanguageType))
				.Cast<LanguageType>()
				.Select(v => v.ToString())
				.ToList();
			foreach (string langt in languageType)
			{
				languageTypeList.Add(new KeyValuePair { Key = langt, Value = langt });
			}

			Criterion languageCriteria = new Criterion
			{
				UIType = UIType.DropDownList,
				Key = "Language",
				Header = "Language:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = languageTypeList
			};

			// Creator
			Criterion creatorCriterion = new Criterion
			{
				UIType = UIType.TextBoxEdit,
				Key = "Creator",
				Header = "Creator:",
				DataTextField = "Value",
				DataValueField = "Key",
				EditMask = EditMaskType.NoMask
			};

			// Publisher
			Criterion publisherCriteria = new Criterion
			{
				UIType = UIType.TextBoxEdit,
				Key = "Publisher",
				Header = "Publisher:",
				DataTextField = "Value",
				DataValueField = "Key",
				EditMask = EditMaskType.NoMask
			};

			// Usage Rights

			usageRightTypes.Add(new KeyValuePair { Key = "0", Value = "-- Select --" });
			usageRightTypes = usageRightTypes.OrderBy(o => o.Key).ToList();
			Criterion usageRightsCriteria = new Criterion
			{
				UIType = UIType.DropDownList,
				Key = "UsageRights",
				Header = "Usage&nbsp;Rights:",
				DataTextField = "Value",
				DataValueField = "Key",
				DataSource = usageRightTypes
			};

			SearchLMRIControl.Criteria.Criterias.Add(gradeCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(educationalSubjectCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(learningResourceCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(EducationalUse);
			SearchLMRIControl.Criteria.Criterias.Add(endUser);
			SearchLMRIControl.Criteria.Criterias.Add(creatorCriterion);
			SearchLMRIControl.Criteria.Criterias.Add(publisherCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(usageRightsCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(mediaCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(languageCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(ageAppropriateCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(durationCriteria);

			SearchLMRIControl.Criteria.Criterias.Add(assessedCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(teachesCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(requiresCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(textComplexityCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(readingLevelCriteria);
			SearchLMRIControl.Criteria.Criterias.Add(interactivityTypeCriteria);
		}

		private List<KeyValuePair> GetStandardSetList()
		{
			return CriteriaHelper.GetStandardSetList().OrderBy(x => x.Value).ToList();
		}

		private List<KeyValuePair> GetGradesList()
		{
			return CriteriaHelper.GetGradesList().OrderBy(x => x.Value).ToList();
		}

		//private List<KeyValuePair> GetTextSearchOptions()
		//{
		//    return new List<KeyValuePair>
		//        {
		//            new KeyValuePair("0", "-- Select --"),
		//            new KeyValuePair("any", "Any Words"),
		//            new KeyValuePair("all", "All Words"),
		//            new KeyValuePair("exact", "Exact Phrase"),
		//            new KeyValuePair("key", "Keywords"),
		//        };
		//}
		/// <summary>
		/// GetAssociatedResources
		/// </summary>
		/// <returns>List</returns>
		private List<KeyValuePair> GetAssociatedResources()
		{
			const string sql =
				@"select ResourceID, Name from thinkgate_resource where resourceid in (select distinct resourceid from thinkgate_docToResources) order by Name";
			DataTable resourcesDataTable = GetDataTable(_cmsConnectionString, sql);

			List<KeyValuePair> resourcesTable = (from s in resourcesDataTable.AsEnumerable()
                                                 select new KeyValuePair { Key = s["ResourceID"].ToString(), Value = s["Name"].ToString() })
                .ToList<KeyValuePair>();

			return resourcesTable;
		}

		/// <summary>
		/// Get the Common Creative Usage Rights
		/// </summary>
		/// <returns></returns>
		private List<KeyValuePair> GetCommonCreative()
		{
			const string sql = @"select * from CreativeCommon";
			DataTable tagDataTable = GetDataTable(_rootConnectionString, sql);

			List<KeyValuePair> tagTable = (from s in tagDataTable.AsEnumerable()
                                           select new KeyValuePair { Key = s["ID"].ToString(), Value = s["SelectDescription"].ToString() })
                .ToList<KeyValuePair>();

			return tagTable;
		}

		/// <summary>
		/// Get the Common Creative Usage Rights
		/// </summary>
		/// <returns></returns>
		private List<KeyValuePair> GetCreativeCommonsUrls()
		{
			const string sql = @"select * from CreativeCommon";
			DataTable tagDataTable = GetDataTable(_rootConnectionString, sql);

			List<KeyValuePair> tagTable = (from s in tagDataTable.AsEnumerable()
                                           select new KeyValuePair { Key = s["ID"].ToString(), Value = s["DescriptionUrl"].ToString() })
                .ToList<KeyValuePair>();

			return tagTable;
		}

		
		/// <summary>
		/// Get Tag check box items
		/// </summary>
		/// <returns>List</returns>
		private List<KeyValuePair> GetTagElements(Enums.LrmiTags lookupType)
		{
			string sql =
				@"select LD.Enum, LD.Description, L.Name AS LookupType from LookupDetails LD inner join LookupType L on L.Enum=LD.LookupEnum WHERE L.Name = '" +
				lookupType + "' ORDER BY LD.Description;";
			DataTable tagDataTable = GetDataTable(_rootConnectionString, sql);

			List<KeyValuePair> tagTable = (from s in tagDataTable.AsEnumerable()
                                           select new KeyValuePair { Key = s["Enum"].ToString(), Value = s["Description"].ToString() })
                .ToList<KeyValuePair>();

			return tagTable;
		}

		#endregion

		/// <summary>
		/// GetDataTable
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="selectSql"></param>
		/// <returns>DataTable</returns>
		private DataTable GetDataTable(string connectionString, string selectSql)
		{
			DataTable dataTable = new DataTable();

			if (selectSql != null)
			{
				string connectionStringToUse = connectionString ?? _rootConnectionString;

				SqlConnection sqlConnection = new SqlConnection(connectionStringToUse);
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter
				{
					SelectCommand = new SqlCommand(selectSql, sqlConnection)
				};

				try
				{
					sqlConnection.Open();
				}
				catch (SqlException)
				{
					return dataTable;
				}
				try
				{
					sqlDataAdapter.Fill(dataTable);
				}
				catch (SqlException)
				{
					return dataTable;
				}
				finally
				{
					sqlConnection.Close();
				}
			}
			return dataTable;
		}

		/// <summary>
		/// GetResourcesWithTagAssociations
		/// </summary>
		/// <param name="resultResource"></param>
		/// <returns>List</returns>
		private List<CmsResourceTagAssociationDetails> GetResourcesWithTagAssociations(List<Resource> resultResource)
		{
			if (resultResource == null || resultResource.Count == 0)
				return new List<CmsResourceTagAssociationDetails>();

			string resourceIds = string.Join(",",
                resultResource.Select(r => "'" + r.DocumentForeignKeyValue.ToString(CultureInfo.InvariantCulture) + "'")
                    .ToArray());

			List<CmsResourceTagAssociationDetails> list = new List<CmsResourceTagAssociationDetails>();
			/* Include IP, UP, LP, CU and Resources */
			string sql = @" select DISTINCT doc.DocumentForeignKeyValue ResourceId, doc.DocumentNodeID DocumentNodeID, IsNull(det.EducationalAlignmentEnum, 0) EducationalAlignmentEnum, IsNull(det.EducationalAlignmentValues, '') EducationalAlignmentValues, class.ClassName
                            from CMS_Document doc
                            inner join CMS_Tree tree on doc.DocumentNodeID = tree.NodeID
                            inner join CMS_Class class on tree.NodeClassID = class.ClassID
                            left join thinkgate_docToLRMIDetails det on det.LRMIItemID = doc.DocumentNodeID
                            where class.ClassName in ('thinkgate.InstructionalPlan', 'thinkgate.UnitPlan', 'thinkgate.LessonPlan', 'thinkgate.LessonPlan_MA','thinkgate.curriculumUnit', 'thinkgate.resource')
                            and doc.DocumentNodeID > 0 and doc.DocumentForeignKeyValue > 0 
                            and doc.DocumentForeignKeyValue in (" + resourceIds + ") order by 1,2,3";
			DataTable dataTable = GetDataTable(_cmsConnectionString, sql);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					string values = row["EducationalAlignmentValues"].ToString();
					string[] associatedLrmi = values.Split('|');
					if (associatedLrmi.Length > 0)
					{
						for (int i = 0; i < associatedLrmi.Length; i++)
						{
							list.Add(new CmsResourceTagAssociationDetails
							{
								ResourceId = Convert.ToInt32(row["ResourceID"].ToString()),
								DocumentId = Convert.ToInt32(row["DocumentNodeID"].ToString()),
								DocumentNodeId = Convert.ToInt32(row["DocumentNodeID"].ToString()),
								ClassName = string.Format("{0}", row["ClassName"]),
								EducationalAlignmentEnum = Convert.ToInt32(row["EducationalAlignmentEnum"].ToString()),
								EducationAlignmentValues = string.Format("{0}", associatedLrmi[i]),
								DurationMinutes = "",
								AgeAppropriateCriteria = "",
								UseRightUrl = "",
								OriginalThirdPartyUrl = "",
								InstructionEnum = 0,
								ActivityEnum = 0,
								LearningResourceTypeEnum = 0,
								TargetAudienceEnum = 0,
							});
						}
					}
				}
			}
			return list;
		}

		protected void AddResource_Click(object sender, ImageClickEventArgs e)
		{
            try
            {
                if (Session["KenticoUserInfo"] == null) return;

                var kenticoUserInfo = (UserInfo)Session["KenticoUserInfo"];

                var userInfo = UserInfoProvider.GetUserInfo(kenticoUserInfo.UserName);
                var currentUser = new CurrentUserInfo(userInfo, true);

			if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "EditForm"))
			{
				return;
			}

			var theImageButton = sender as ImageButton;
			if (theImageButton == null)
			{
				return;
			}

			string[] row = null;
			if (!string.IsNullOrEmpty(theImageButton.CommandArgument))
			{
				row = theImageButton.CommandArgument.Split(new[] { "|@|" }, StringSplitOptions.None);
			}
                var theResource = new Resource();

			if (row == null || row.Length <= 0) return;

		    theResource.Source = row[0];
            theResource.URL = theResource.Source == PBSLearningMedia ? row[5] : row[1];
                theResource.ResourceName = row[3];
                theResource.Description = row[4];

			var clientId = DistrictParms.LoadDistrictParms().ClientID;
                var userAliasPath = String.Concat(FORWARD_SLASH, KenticoHelper.GetKenticoMainFolderName(clientId), FORWARD_SLASH,
                    "Users");
                var provider = new TreeProvider(currentUser);

                var parent = provider.SelectSingleNode(CMSContext.CurrentSiteName,
                    userAliasPath + FORWARD_SLASH + kenticoUserInfo.UserName, "en-us");
            var webLink = theResource.Source == PBSLearningMedia
                    ? Request.Url.GetLeftPart(UriPartial.Authority) +
                  ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx?resource=PBSLearningMedia&url=" +
                             theResource.URL)
                : theResource.URL;

                if (parent == null) return;

                var ci = DataClassInfoProvider.GetDataClass(ResourceClassName);

				var node = CMS.DocumentEngine.TreeNode.New(ResourceClassName, provider);
				node.SetValue("Type", 84); // static Resource ID
				node.SetValue("SubType", 97); // static Resource Web-Based ID
				node.SetValue("Name", theResource.ResourceName);
				node.SetValue("DocumentName", theResource.ResourceName);
				node.SetValue("Description", theResource.Description);
				node.SetValue("DocumentCulture", "en-us");
				node.SetValue("CreateDate", DateTime.Now);
                node.SetValue("CreatedBy", kenticoUserInfo.UserName);
				node.SetValue("Category", 0);
				node.SetValue("AttachmentName", null);
				node.SetValue("Grade", null);
				node.SetValue("Subject", null);
				node.SetValue("Courses", null);
				node.SetValue("DocRecordID", null);
                node.SetValue("WebLink", webLink);
				node.SetValue("DocumentPageTemplateID", ci.ClassDefaultPageTemplateID);
				node.Insert(parent.NodeID);

				theImageButton.ImageUrl = "~/Images/Check.png";
				theImageButton.ToolTip = "Resource Added to My Docs";
				theImageButton.Enabled = false;
			}
            catch (Exception ex)
            {
                throw ex;
            }

		}
	}


	public class CmsResourceTagAssociationDetails
	{
		public int ResourceId { get; set; }
		public int DocumentId { get; set; }
		public int DocumentNodeId { get; set; }
		public string ClassName { get; set; }
		public int EducationalAlignmentEnum { get; set; }
		public string EducationAlignmentValues { get; set; }
		public int InstructionEnum { get; set; }
		public int ActivityEnum { get; set; }
		public int LearningResourceTypeEnum { get; set; }
		public int TargetAudienceEnum { get; set; }
		public string DurationMinutes { get; set; }
		public string AgeAppropriateCriteria { get; set; }
		public string UseRightUrl { get; set; }
		public string OriginalThirdPartyUrl { get; set; }
	}


}