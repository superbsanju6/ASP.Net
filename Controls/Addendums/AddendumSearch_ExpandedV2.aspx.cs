using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Utilities;
using Thinkgate.Classes;
using Thinkgate.Classes.Serializers;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Addendums
{
	public partial class AddendumSearch_ExpandedV2 : BasePage
	{
		public string ImageWebFolder { get; set; }
		public string clientFolder { get; set; }
		protected string ExpandPerm_IconWidth { get; set; }
		protected string ExpandPerm_onclick { get; set; }
		private int _recordsPerPage = 100;
		protected bool _perm_Field_Lexile { get; set; }
        public static string TestCategory { get; set; }
        public static bool ShowExpiredItems { get; set; }
        protected Dictionary<string, bool> dictionaryItem;
        bool isSecuredFlag;
        private Int32 AssessmentID = 0;
        public Thinkgate.Base.Classes.Assessment selectedAssessment;

        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(selectedAssessment.TestType);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }


		protected new void Page_Init(object sender, EventArgs e)
		{
			base.Page_Init(sender, e);
			Master.Search += new SearchMaster.SearchHandler(SearchHandler);
			if (!IsPostBack)
			{
				LoadSearchScripts();

                if (Request.QueryString["AssessmentID"] != null)
                {
                   
                        AssessmentID = Convert.ToInt32(Request.QueryString["AssessmentID"]);
                 
                }

                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                Dictionary<string, bool> dictionaryItem;
                string testCategory = string.Empty;
                if (AssessmentID != 0)
                {
                    selectedAssessment = Thinkgate.Base.Classes.Assessment.GetAssessmentByID(AssessmentID);
                    dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(selectedAssessment.TestCategory);
                    bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString()) == true).Select(y => y.Key).ToList().Distinct().Count() > 0 ? true : false;
                    testCategory = selectedAssessment.TestCategory;
                    if (hasPermission && isSecuredFlag)
                    {
                        if (SecureType)
                        {
                            testCategory = selectedAssessment.TestType;
                        }
                    }
                }

                DistrictParms parms = DistrictParms.LoadDistrictParms();
                ShowExpiredItems = (Request.QueryString["ShowExpiredItems"] != "No" && parms.AllowSearchForCopyRightExpiredContent);
                if (!ShowExpiredItems)
			    {
			        ExpirationStatusDateRange.DDLExpirationStatus.Visible = false;
			     }

			    ImageWebFolder = (Request.ApplicationPath.Equals("/") ? string.Empty : Request.ApplicationPath) + "/Images/";

				if (!UserHasPermission(Permission.Icon_Expand_Item))
				{
					ExpandPerm_IconWidth = "0px";
					ExpandPerm_onclick = "";
				}
				else
				{
					ExpandPerm_IconWidth = "17px";
					ExpandPerm_onclick = "onclick=\"expandItem('{{:IDEncrypted}}');\"";
				}

				var serializer = new JavaScriptSerializer();

				var itemBankEditTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
				var itemBankCopyTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankCopy, "Search");
				serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
				string renderIBEditArray = "var IBEditArray = [" + serializer.Serialize(itemBankEditTbl) + "]; ";
				renderIBEditArray += "var IBCopyArray = [" + serializer.Serialize(itemBankCopyTbl) + "]; ";
				ScriptManager.RegisterStartupScript(this, typeof(string), "IBEditArray", renderIBEditArray, true);
				//If user does not have the following permission, then we are to hide the "Lexile:" information.
				_perm_Field_Lexile = UserHasPermission(Permission.Field_Lexile);

#if DEBUG
				clientFolder = "";
#else
				clientFolder = AppSettings.AppVirtualPath == "/" ? "" : AppSettings.AppVirtualPath;
#endif

				if (UserHasPermission(Permission.Icon_Expand_Addendum))
				{
					ExpandPerm_IconWidth = "17px";
					ExpandPerm_onclick = "onclick=\"window.open('" + clientFolder + "/Record/AddendumPage.aspx?xID={{:ID_Encrypted}}');\"";
				}
				else
				{
					ExpandPerm_IconWidth = "0px";
					ExpandPerm_onclick = "";
				}

				var courses = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);

                ctrlGradeSubjectCourseStandardSet.JsonDataSource = serializer.Serialize(courses.BuildJsonArray());
				ctrlGradeSubjectCourseStandardSet.ChkGrade.DefaultTexts = PossibleDefaultTexts(Request.QueryString["grade"]);
				ctrlGradeSubjectCourseStandardSet.ChkSubject.DefaultTexts = PossibleDefaultTexts(Request.QueryString["subject"]);
				ctrlGradeSubjectCourseStandardSet.CmbCourse.DefaultTexts = PossibleDefaultTexts(Request.QueryString["coursename"]);
				ctrlAddendumType.ParentDataSource = Addendum.AddendumTypes;
				ctrlAddendumType.ChildDataSource = Addendum.AddendumGenres;

				//cblItemBank.DataSource = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser).DistinctByLabel();
                dtItemBank dtItemBank = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.Read, testCategory).DistinctByLabel();
                //dtItemBank dtItemBank = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser).DistinctByLabel();
                
                TestCategory = Request.QueryString["TestCategory"];

                if (TestCategory != null && TestCategory == AssessmentCategories.District.ToString())
                {
                    for (var rowIndex = dtItemBank.Rows.Count - 1; rowIndex >= 0; rowIndex--)
                    {
                        if (dtItemBank.Rows[rowIndex]["Label"].ToString() == "Personal")
                        { dtItemBank.Rows[rowIndex].Delete(); }
                    }
                }
                cblItemBank.DataSource = dtItemBank;

				txtSearch.DataSource = TextSearchDropdownValues();
				SortBar.DataSource = SortFields();
			}
		}

	    private List<String> PossibleDefaultTexts(object input)
		{
			if (input == null) return null;
			var list = new List<String>();
			list.AddRange(input.ToString().Split(','));
			return list;
		}

		/// <summary>
		/// Handler for the Search Button
		/// </summary>
		protected void SearchHandler(object sender, CriteriaController criteriaController)
		{
			var criteriaObject = BuildSearchCriteriaObject(SessionObject.LoggedInUser, criteriaController, SortBar.SelectedSortText());

			var resultsPage1 = Addendum.GetAddendumsByCriteriaPaging(criteriaObject, 1, _recordsPerPage);

			var addendumPage1 = resultsPage1.AddendumList;

			var siteMaster = this.Master;
			if (siteMaster != null && this.Master is SearchMaster)
			{
				SearchPager.Visible = true;
				SearchPager.PageSize = _recordsPerPage;
				SearchPager.ResultCount = resultsPage1.FullResultCount;
				SearchPager.DataBind();

				var serializer = new JavaScriptSerializer();
				serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer(), new StandardsSerializer() });
				string renderItemsScript = "ItemArray = []; ";
				renderItemsScript += "maxPage = " + SearchPager.GetNumberOfPages() + ";";
				switch (SortBar.SelectedSortText())
				{
					case "ItemBank":
						renderItemsScript += "ItemArray[0] = " + serializer.Serialize(from addendum in addendumPage1
																					  orderby addendum.Addendum_ItemBanks
																					  select addendum) + ";";
						break;
					/* case "Standard":
						 renderItemsScript += "ItemArray[0] = " + serializer.Serialize(from question in questionsPage1
																					   orderby (question.Standards.Count > 0 ? question.Standards[0].StandardName : "")
																					   select question) + ";";
						 break;*/
					default:
						renderItemsScript += "ItemArray[0] = " + serializer.Serialize(addendumPage1) + ";";
						break;
				}
				renderItemsScript += "SortField = '" + SortBar.SelectedSortText() + "';";

				renderItemsScript = AssessmentUtil.RepairImageUrl(renderItemsScript, ConfigHelper.GetImagesUrl());

				renderItemsScript += "RenderFirstPage();";

				ScriptManager.RegisterStartupScript(this, typeof(string), "data", renderItemsScript, true);
			}
		}


		/// <summary>
		/// Builds the QuestionSearchCriteria. Used both from Search Button and from Service
		/// </summary>
		public static AddendumSearchCriteriaV2 BuildSearchCriteriaObject(ThinkgateUser user, CriteriaController criteriaController, string requestedSortField = null)
		{

			/* Sort */
			string sortField = null;
			NameValue nvSortField = SortFields().Find(x => x.Name == requestedSortField);   // ensure that the user given value for sort is in the defined list of options
			if (nvSortField != null) sortField = nvSortField.Value;

			/* Item Banks - Remove from master list based on filter if given */
			var itemBanks = ItemBankMasterList.GetItemBanksForUser(user);

            if (TestCategory != null && TestCategory == AssessmentCategories.District.ToString())
            {
                for (var rowIndex = itemBanks.Rows.Count - 1; rowIndex >= 0; rowIndex--)
                {
                    if (itemBanks.Rows[rowIndex]["Label"].ToString() == "Personal")
                    { itemBanks.Rows[rowIndex].Delete(); }
                }
            }

            var selectedItemBanks = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("ItemBank").Select(x => x.Text);
			if (selectedItemBanks.Any()) itemBanks.DeleteByLabel(selectedItemBanks);

			/* Text Search */
			string searchText = string.Empty;
			string searchOption = string.Empty;
			var txtSearchList = criteriaController.ParseCriteria<TextWithDropdown.ValueObject>("TextSearch");
			if (txtSearchList.Count > 0)
			{
				// we ensure that the value the user gave us for text search type is a valid option
				var confirmedOption = TextSearchDropdownValues().Find(x => x.Name == txtSearchList[0].Option) ?? TextSearchDropdownValues().First();
				if (!String.IsNullOrEmpty(txtSearchList[0].Text))
				{
					searchText = txtSearchList[0].Text;
					searchOption = confirmedOption.Value;
				}
			}

			/* Courses */
			var selectedGrades = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Grade").Select(x => x.Text).ToList();
			var selectedSubjects = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Subject").Select(x => x.Text).ToList();
			var selectedCourses = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Course").Select(x => x.Text).ToList();
			var selectedStandardSets = new drGeneric_String(criteriaController.ParseCriteria<CheckBoxList.ValueObject>("StandardSet").Select(x => x.Text));     // take straight to drGeneric_String because it's going to SQL
			var filteredCourses = CourseMasterList.GetStandardCoursesForUser(user).FilterByGradesSubjectsStandardSetsAndCourse(selectedGrades, selectedSubjects, selectedStandardSets, selectedCourses);


			/* Type/Genre */
			var selectedAddendumGenres = new drGeneric_String();
			var selectedAddendumTypes = new drGeneric_String();
			var selectedTypesGenres = criteriaController.ParseCriteria<AddendumType.ValueObject>("AddendumType");
			foreach (var selectedTypesGenre in selectedTypesGenres)
			{
				if (selectedTypesGenre.Genre != null)
				{
					selectedAddendumGenres.Add(selectedTypesGenre.Genre);
				}
				else
				{
					selectedAddendumTypes.Add(selectedTypesGenre.Text);
				}
			}
			if (selectedAddendumGenres.Count > 0)
				selectedAddendumTypes.Add("Passage");


		    var selectedExpirationStatus = criteriaController.ParseCriteria<DropDownList.ValueObject>("ExpirationStatus").FirstOrDefault() != null ? 
                criteriaController.ParseCriteria<DropDownList.ValueObject>("ExpirationStatus").FirstOrDefault().Value : "I";
            if (!ShowExpiredItems)
            {
                selectedExpirationStatus = "E";

            }
            drGeneric_String_String selectedExpirationDateRange = new drGeneric_String_String();
            foreach (var val in criteriaController.ParseCriteria<DateRange.ValueObject>("ExpirationDateRange"))
            {
                selectedExpirationDateRange.Add(val.Type == "Start" ? "CreatedDateStart" : "CreatedDateEnd", val.Date);
            }


			/* Build Criteria Object */
            var asc = new AddendumSearchCriteriaV2()
                                             {
                                                 ItemBanks = itemBanks,
                                                 CourseList = filteredCourses,
                                                 AddendumType = selectedAddendumTypes,
                                                 AddendumGenre = selectedAddendumGenres,
                                                 UserAccessItemReservations = "",
                                                 Text = searchText,
                                                 TextOpt = searchOption,                                                 
                                                 SortKeyword = sortField,
                                                 courseSelected = selectedCourses.Count > 0 ? true : false,
                                                 ExpirationStatus = selectedExpirationStatus,
                                                 ExpirationDateRange = selectedExpirationDateRange
                                             };

			return asc;
		}

		private static List<NameValue> TextSearchDropdownValues()
		{
			return new List<NameValue>
                {
                    new NameValue("Any Words", "Any Words"),
                    new NameValue("All Words","All Word"),
                    new NameValue("Exact Phrase","Exact Phrase"),
                    new NameValue("Keywords","Keywords"),
                };
		}

		public static List<NameValue> SortFields()
		{
			return new List<NameValue>
                       {
                           new NameValue("Item Bank", "ItemBank"),
                           new NameValue("# of Items", "ItemCount"),
                           new NameValue("Name", "Name"),
                           new NameValue("Grade", "Grade"),
                           new NameValue("Subject", "Subject"),
                           new NameValue("Standard", "Standard"),
                           new NameValue("Type", "Type"),
                           new NameValue("Author", "Author"),
                           new NameValue("Date Created", "DateCreated"),
                           new NameValue("Last Updated", "LastUpdated")
                       };
		}


		private void LoadSearchScripts()
		{
			if (Master != null)
			{
				RadScriptManager radScriptManager;
				var scriptManager = Master.FindControl("RadScriptManager2");
				if (scriptManager != null)
				{
					radScriptManager = (RadScriptManager)scriptManager;
					radScriptManager.Services.Add(new ServiceReference("Services/Service2.svc"));
				}
			}
			else
			{
				return;
			}
		}

        protected void copyAddendumXmlHttpPanel_ServiceRequest(object sender, RadXmlHttpPanelEventArgs e)
        {
            int newAddendumID = Base.Classes.Addendum.CopyAddendum(Convert.ToInt32(e.Value));
            if (newAddendumID == 0)
            {
                throw new Exception("Unable to copy Addendum");
            }
        }
	}
}
