using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Services;
using System.Web.WebPages;
using System.Xml;
using Standpoint.Core.ExtensionMethods;
using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Classes.Search;
using Thinkgate.Classes.Serializers;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Domain.Classes.Review;
using Thinkgate.Enums;
using Thinkgate.Services;
using TextWithDropdown = Thinkgate.Controls.E3Criteria.TextWithDropdown;
using Thinkgate.Domain.Facades;
using Thinkgate.Services.Contracts.LearningMedia;
using Subject = Thinkgate.Base.Classes.Subject;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Reflection;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.Items
{
    public partial class ItemSearch : BasePage, ICallbackEventHandler
    {
        #region Private Variables
        private ReviewFacade _reviewFacade;
        private LeaQuestionReviewAverages leaQuestionReviewAverages;
        public string ImageWebFolder { get; set; }
        public string HideIfNotMultiSelect { get; set; }
        public string HideIfNotSingleSelect { get; set; }
        static public ItemSearchModes SearchMode;
        static public ItemFilterModes FilterMode;
        protected string ExpandPermIconWidth { get; set; }
        protected string ExpandPermOnclick { get; set; }
        public static string TestCategory { get; set; }
        static public string TestYear { get; set; }
        static public int TestCurrCourseID { get; set; }
        public static List<NameValue> DokNameValueList;
        private readonly string _rootConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
        private CourseList _courses;
        public static bool ShowExpiredItems { get; set; }
        public int AssessmentID { get; set; }

        #endregion Private Variables

        #region Events
        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            Master.Search += (SearchHandler);

            if (!IsPostBack)
            {
                LoadSearchScripts();

               

                ImageWebFolder = GetImageWebFolder();

                Enum.TryParse(Request.QueryString["ItemSearchMode"], true, out SearchMode);
                Enum.TryParse(Request.QueryString["ItemFilterMode"], true, out FilterMode);
                HideIfNotMultiSelect = (SearchMode != ItemSearchModes.MultiSelect ? "display: none" : "");
                HideIfNotSingleSelect = (SearchMode != ItemSearchModes.SingleSelect ? "display: none" : "");
                if (!UserHasPermission(Permission.Icon_Expand_Item))
                {
                    ExpandPermIconWidth = "0px";
                    ExpandPermOnclick = "";
                }
                else
                {
                    ExpandPermIconWidth = "17px";
                    ExpandPermOnclick = "onclick=\"expandItem('{{:IDEncrypted}}');\"";
                }
                SortBar.ShowSelectAndReturnButton = SearchMode == ItemSearchModes.MultiSelect;
                TestCategory = Request.QueryString["TestCategory"];

                DistrictParms parms = DistrictParms.LoadDistrictParms();
                ShowExpiredItems = (Request.QueryString["ShowExpiredItems"] != "No" && parms.AllowSearchForCopyRightExpiredContent);
                if (!ShowExpiredItems)
                    ExpirationStatusDateRange.DDLExpirationStatus.Visible = false;

                ctrlAddendumSearch.TestCategory = Request.QueryString["TestCategory"];
                if (Request.QueryString["AssessID"] != null)
                {
                    ctrlAddendumSearch.AssessmentID = Convert.ToInt32(Request.QueryString["AssessID"]);
                }
                
                TestYear = Request.QueryString["TestYear"];
                TestCurrCourseID = DataIntegrity.ConvertToInt(Request.QueryString["TestCurrCourseID"]);
                ScriptManager.RegisterStartupScript(this, typeof(string), "JSVars", "var TestCurrCourseID=" + TestCurrCourseID + "; var TestYear='" + TestYear + "';", true);

                var serializer = new JavaScriptSerializer();

                var itemBankEditTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
                var itemBankCopyTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankCopy, "Search");
                serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
                string renderIbEditArray = "var IBEditArray = [" + serializer.Serialize(itemBankEditTbl) + "]; ";
                renderIbEditArray += "var IBCopyArray = [" + serializer.Serialize(itemBankCopyTbl) + "]; ";
                ScriptManager.RegisterStartupScript(this, typeof(string), "IBEditArray", renderIbEditArray, true);

                _courses = (FilterMode == ItemFilterModes.Unfiltered) ? CourseMasterList.StandCourseList : CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);

                ctrlGradeSubjectCourseStandardSet.JsonDataSource = serializer.Serialize(_courses.BuildJsonArray(true, true));

                ctrlGradeSubjectCourseStandardSet.ChkGrade.DefaultTexts = PossibleDefaultTexts(Request.QueryString["grade"]);
                ctrlGradeSubjectCourseStandardSet.ChkSubject.DefaultTexts = PossibleDefaultTexts(Request.QueryString["subject"]);
                ctrlGradeSubjectCourseStandardSet.CmbCourse.DefaultTexts = PossibleDefaultTexts(Request.QueryString["coursename"]);

                //the standard set was not available from assesment page currently
                //so standardid is passed and a lookup is done
                int standardID = DataIntegrity.ConvertToInt(Request.QueryString["standardid"]);
                string standardDesc = "";
                if (standardID > 0) standardDesc = Base.Classes.Standards.GetStandardByID(standardID).Standard_Set;

                ctrlGradeSubjectCourseStandardSet.ChkStandardSet.DefaultTexts = PossibleDefaultTexts(standardDesc);

                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                Dictionary<string, bool> dictionaryItem;
                dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(TestCategory);
                bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                bool isSecureAssessment = !string.IsNullOrEmpty(Request.QueryString["isSecure"]) && Convert.ToBoolean(Request.QueryString["isSecure"]);
               
                string testCategory = TestCategory;
                if (hasPermission && isSecuredFlag &&isSecureAssessment && Request.QueryString["AssessmentType"] != null)
                     testCategory = Request.QueryString["AssessmentType"];
                        
               dtItemBank dtItemBank = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.Read, testCategory).DistinctByLabel();

                if (TestCategory != null && TestCategory == AssessmentCategories.District.ToString())
                {
                    for (var rowIndex = dtItemBank.Rows.Count - 1; rowIndex >= 0; rowIndex--)
                    {
                        if (dtItemBank.Rows[rowIndex]["Label"].ToString() == "Personal")
                        { dtItemBank.Rows[rowIndex].Delete(); }
                    }
                }

                cblItemBank.DataSource = dtItemBank;
                var list = new List<NameValue>();
                
                list = LoadItemReservations();
                
                
                cblItemReservation.DataSource = list;
                cblItemType.DataSource = ItemTypes();
                cblScoreType.DataSource = ScoreTypes();
                cblItemReviewStatus.DataSource = ItemReviewStatus();
                cblDifficultyIndex.DataSource = DifficultyIndex();

                var dparms = DistrictParms.LoadDistrictParms();
                lbl_OTCUrl.Value = AppSettings.OTCUrl + dparms.ClientID;
                DokNameValueList = Rigor.CreateRigorListByDOK(DistrictParms.DOK);
                cblRigor.DataSource = DokNameValueList;
                cblRigor.Text = DistrictParms.DOK;

                ctrlAddendumType.ParentDataSource = Addendum.AddendumTypes;
                ctrlAddendumType.ChildDataSource = Addendum.AddendumGenres;
                ddlAnchorItem.DataSource = AnchorItemList();
                ddlAnchorItem.DefaultTexts = PossibleDefaultTexts("Include Anchor Items");
                cmbStandardFilter.DataSource = SessionObject.LoggedInUser.StandardFilters.Distinct();
                txtSearch.DataSource = TextSearchDropdownValues();
                SortBar.DataSource = Questions.SortFields;

            }
            _reviewFacade = new ReviewFacade(Base.Classes.AppSettings.ConnectionString);

        }

        private string GetImageWebFolder()
        {
            return (Request.ApplicationPath != null && Request.ApplicationPath.Equals("/") ? string.Empty : Request.ApplicationPath) + "/Images/";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DistrictParms.LoadDistrictParms().LRMITagging != "true")
            {
                rtsSearch.Tabs[2].Visible = false;
            }
            if (!IsPostBack)
            {
                LoadLrmiCriteria();
            }

            RegisterScripts();
        }

        /// <summary>
        /// Handler for the Search Button
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            var criteriaObject = BuildSearchCriteriaObject(SessionObject.LoggedInUser, criteriaController, SortBar.SelectedSortText());
            int testID = 0;
            if (Request.QueryString["testID"] != null)
                testID = DataIntegrity.ConvertToInt(Request.QueryString["testID"]);

            var resultsPage1 = Questions.Search(criteriaObject, 1, 100, SearchMode == ItemSearchModes.MultiSelect || SearchMode == ItemSearchModes.SingleSelect, TestYear, TestCurrCourseID, testID);

            var questionsPage1 = resultsPage1.BankQuestionsList;

            if (Master != null)
            {
                SearchPager.Visible = true;
                if (TagCriteriaSelectionParameters.IsEmpty(criteriaObject.LrmiTagCriteria))
                    SearchPager.PageSize = 100;
                else
                    SearchPager.PageSize = 10;
                SearchPager.ResultCount = resultsPage1.FullResultCount;
                SearchPager.DataBind();

                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer(), new StandardsSerializer() });
                string renderItemsScript = "ItemArray = []; ";
                renderItemsScript += "maxPage = " + SearchPager.GetNumberOfPages() + ";";
                switch (SortBar.SelectedSortText())
                {
                    case "ItemBank":
                        renderItemsScript += "ItemArray[0] = " + serializer.Serialize(from question in questionsPage1
                                                                                      orderby (question.ItemBankList.Count > 0 ? question.ItemBankList[0].Label : "")
                                                                                      select question) + ";";
                        break;
                    case "Standard":
                        renderItemsScript += "ItemArray[0] = " + serializer.Serialize(from question in questionsPage1
                                                                                      orderby (question.Standards.Count > 0 ? question.Standards[0].StandardName : "")
                                                                                      select question) + ";";
                        break;
                    default:
                        renderItemsScript += "ItemArray[0] = " + serializer.Serialize(questionsPage1) + ";";
                        break;
                }
                renderItemsScript += "SortField = '" + SortBar.SelectedSortText() + "';";
                renderItemsScript += "RenderFirstPage();";

                ScriptManager.RegisterStartupScript(this, typeof(string), "AssessmentItems", renderItemsScript, true);
            }
        }
        #endregion Events

        #region Public Methods
        /// <summary>
        /// Builds the QuestionSearchCriteria. Used both from Search Button and from Service
        /// </summary>
        public static QuestionSearchCriteria BuildSearchCriteriaObject(ThinkgateUser user, CriteriaController criteriaController, string requestedSortField = null)
        {
            try
            {
                /* Sort */
                string sortField = null;
                NameValue nvSortField = Questions.SortFields.Find(x => x.Name == requestedSortField);   // ensure that the user given value for sort is in the defined list of options
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
                IEnumerable<string> banks = selectedItemBanks as IList<string> ?? selectedItemBanks.ToList();
                if (banks.Any())
                {
                    itemBanks.DeleteByLabel(banks);
                }

                /* Item Reservation */
                var selectedItemReservations = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("ItemReservation").Select(x => x.Value);
                var itemReservationList = new drGeneric_String(ItemReservations().Select(x => x.Value).Intersect(selectedItemReservations));

                /* Standard Filters */
                var standardFilters = new drGeneric_Int();
                var selectedStandardFilter = criteriaController.ParseCriteria<DropDownList.ValueObject>("StandardFilter");
               if (selectedStandardFilter != null && selectedStandardFilter.Count >= 1 && user.StandardFilters.ContainsKey(selectedStandardFilter[0].Text))
                {
                    standardFilters.AddRange(user.StandardFilters[selectedStandardFilter[0].Text].Split(',').Select(DataIntegrity.ConvertToInt));
                }
               else
                {
                    /* Standard is not passed as criteria, so try retrieve it from query string and
                     * add it to standard filters. "from" parameter is used to identify the Manual
                     * Replace link in assessment.
                     */
                    if (GetQueryStringValue("standardid") != null && GetQueryStringValue("from") != null
                        && GetQueryStringValue("from").Equals("ManualReplace", StringComparison.InvariantCultureIgnoreCase)
                    )
                    {
                        standardFilters.Add(DataIntegrity.ConvertToInt(GetQueryStringValue("standardid")));
                    }
                }

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
                CourseList filteredCourses;
                switch (FilterMode)
                {
                    case ItemFilterModes.Unfiltered:
                        filteredCourses = CourseMasterList.StandCourseList.FilterByGradesSubjectsStandardSetsAndCourse(selectedGrades, selectedSubjects, selectedStandardSets, selectedCourses);
                        break;
                    default:
                        filteredCourses = CourseMasterList.GetStandardCoursesForUser(user).FilterByGradesSubjectsStandardSetsAndCourse(selectedGrades, selectedSubjects, selectedStandardSets, selectedCourses);
                        break;
                }

                /* ADVANCED SEARCH CRITERIA */
                /* Item Type */
                var selectedItemTypes = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("ItemType").Select(x => x.Value);
                var itemTypeList = new drGeneric_String(ItemTypes().Select(x => x.Value).Intersect(selectedItemTypes));

                /* Score Type */
                var selectedScoreTypes = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("ScoreType").Select(x => x.Value);
                var scoreTypeList = new drGeneric_String(ScoreTypes().Select(x => x.Value).Intersect(selectedScoreTypes));

                /* Item Review Status */
                var itemStatusList = new drGeneric_Int();
                var selectedItemStatus = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("ItemReviewStatus").Select(x => x.Value);
                foreach (var strID in selectedItemStatus)
                {
                    Int32 id;
                    if (Int32.TryParse(strID.Trim(), out id)) itemStatusList.Add(id);
                }


                /* Difficulty */
                var selectedDifficultyIndex = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("DifficultyIndex").Select(x => x.Value);
                var difficultyIndexList = new drGeneric_String(DifficultyIndex().Select(x => x.Value).Intersect(selectedDifficultyIndex));

                /* Rigor - District Parm "DOK" = RBT, Webb, Marzano */
                var selectedRigor = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Rigor").Select(x => x.Value);
                if (DokNameValueList == null)
                {
                    DistrictParms parms = DistrictParms.LoadDistrictParms();
                    DokNameValueList = Rigor.CreateRigorListByDOK(parms.DOK);
                }
                var rigorList = new drGeneric_String(DokNameValueList.Select(x => x.Value).Intersect(selectedRigor));

                /* Addendum Type/Genre */
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

                /* Created Date Range */
                drGeneric_String_String createdDateRange = new drGeneric_String_String();
                foreach (var val in criteriaController.ParseCriteria<DateRange.ValueObject>("CreatedDate"))
                {
                    createdDateRange.Add(val.Type == "Start" ? "CreatedDateStart" : "CreatedDateEnd", val.Date);
                }

                /* Difficulty Range */

                string scoreAverage1 = null;
                string scoreAverage2 = null;

                foreach (var val in criteriaController.ParseCriteria<DifficultyRange.ValueObject>("Difficulty"))
                {
                    if (val.Type == "Start")
                    {
                        scoreAverage1 = val.Difficulty;
                    }
                    if (val.Type == "End")
                    {
                        scoreAverage2 = val.Difficulty;
                    }
                }

                /* Addendum ID List */
                drGeneric_Int addendumIDList = new drGeneric_Int();
                var addendumIDVo = criteriaController.ParseCriteria<AddendumSearch.ValueObject>("Addendum");
                if (addendumIDVo != null && addendumIDVo.Count > 0)
                {
                    foreach (AddendumSearch.ValueObject vo in addendumIDVo)
                    {
                        if (vo != null && vo.ID != null)
                        {
                            Int32 addendumID;
                            string strID = vo.ID;
                            if (Int32.TryParse(strID.Trim(), out addendumID)) addendumIDList.Add(addendumID);
                        }
                    }
                }

                /* Anchor Item Selection */

                var selectedAnchorItemFilter = criteriaController.ParseCriteria<DropDownList.ValueObject>("AnchorItem").FirstOrDefault() != null ?
                    criteriaController.ParseCriteria<DropDownList.ValueObject>("AnchorItem").FirstOrDefault().Value.ToString(CultureInfo.CurrentCulture) : "I";
                var selectedExpirationStatus = criteriaController.ParseCriteria<DropDownList.ValueObject>("ExpirationStatus").FirstOrDefault() != null ?
                    criteriaController.ParseCriteria<DropDownList.ValueObject>("ExpirationStatus").FirstOrDefault().Value.ToString(CultureInfo.CurrentCulture) : "I";
                if (!ShowExpiredItems)
                {
                    selectedExpirationStatus = "E";
                }
                drGeneric_String_String selectedExpirationDateRange = new drGeneric_String_String();
                foreach (var val in criteriaController.ParseCriteria<DateRange.ValueObject>("ExpirationDateRange"))
                {
                    selectedExpirationDateRange.Add(val.Type == "Start" ? "CreatedDateStart" : "CreatedDateEnd", val.Date);
                }


                TagCriteriaSelectionParameters lrmiTagCriteria = new TagCriteriaSelectionParameters();
                var lrmiValueObject = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("GradeLevel");
                foreach (CheckBoxList.ValueObject vo in lrmiValueObject)
                {
                    lrmiTagCriteria.GradeLevel.Add(vo.Value);
                }
                lrmiValueObject = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("EducationalSubject");
                foreach (CheckBoxList.ValueObject vo in lrmiValueObject)
                {
                    lrmiTagCriteria.EducationalSubject.Add(vo.Value);
                }
                lrmiValueObject = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("LearningResource");
                foreach (CheckBoxList.ValueObject vo in lrmiValueObject)
                {
                    lrmiTagCriteria.LearningResourceType.Add(Convert.ToInt32(vo.Value));
                }
                lrmiValueObject = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("EducationalUse");
                foreach (CheckBoxList.ValueObject vo in lrmiValueObject)
                {
                    lrmiTagCriteria.EducationalUse.Add(Convert.ToInt32(vo.Value));
                }
                lrmiValueObject = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("EndUser");
                foreach (CheckBoxList.ValueObject vo in lrmiValueObject)
                {
                    lrmiTagCriteria.EndUser.Add(Convert.ToInt32(vo.Value));
                }
                lrmiValueObject = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("MediaType");
                foreach (CheckBoxList.ValueObject vo in lrmiValueObject)
                {
                    lrmiTagCriteria.MediaType.Add(Convert.ToInt32(vo.Value));
                }
                lrmiValueObject = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Interactivity");
                foreach (CheckBoxList.ValueObject vo in lrmiValueObject)
                {
                    lrmiTagCriteria.InteractivityType.Add(Convert.ToInt32(vo.Value));
                }

                var lrmiDropDownObject = criteriaController.ParseCriteria<DropDownList.ValueObject>("UsageRight");
                if (lrmiDropDownObject.Count > 0) lrmiTagCriteria.UseRightUrl = Convert.ToInt32(lrmiDropDownObject[0].Value);
                lrmiDropDownObject = criteriaController.ParseCriteria<DropDownList.ValueObject>("Language");
                if (lrmiDropDownObject.Count > 0) lrmiTagCriteria.Language = Convert.ToInt32(lrmiDropDownObject[0].Value);
                lrmiDropDownObject = criteriaController.ParseCriteria<DropDownList.ValueObject>("AgeAppropriate");
                if (lrmiDropDownObject.Count > 0) lrmiTagCriteria.AgeAppropriate = Convert.ToInt32(lrmiDropDownObject[0].Value);


                var lrmiTextObject = criteriaController.ParseCriteria<Text.ValueObject>("Creator");
                if (lrmiTextObject.Count > 0) lrmiTagCriteria.Creator = lrmiTextObject[0].Text;
                lrmiTextObject = criteriaController.ParseCriteria<Text.ValueObject>("Publisher");
                if (lrmiTextObject.Count > 0) lrmiTagCriteria.Publisher = lrmiTextObject[0].Text;
                lrmiTextObject = criteriaController.ParseCriteria<Text.ValueObject>("TextComplexity");
                if (lrmiTextObject.Count > 0) lrmiTagCriteria.TextComplexity = lrmiTextObject[0].Text;
                lrmiTextObject = criteriaController.ParseCriteria<Text.ValueObject>("ReadingLevel");
                if (lrmiTextObject.Count > 0) lrmiTagCriteria.ReadingLevel = lrmiTextObject[0].Text;
                lrmiTextObject = criteriaController.ParseCriteria<Text.ValueObject>("Duration");
                if (lrmiTextObject.Count > 0)
                {
                    int[] arrayDuration = lrmiTextObject[0].Text.Split(':').Select(Int32.Parse).ToArray();
                    lrmiTagCriteria.TimeRequiredDays = arrayDuration[0];
                    lrmiTagCriteria.TimeRequiredHours = arrayDuration[1];
                    lrmiTagCriteria.TimeRequiredMinutes = arrayDuration[2];
                }
                var lrmiStandards = criteriaController.ParseCriteria<StandardGradeSubjectId.ValueObject>("StandardSetId");
                if (lrmiStandards.Count > 0)
                {
                    StandardSelections selectedAssessed = new StandardSelections
                    {
                        StandardSet = lrmiStandards[0].StandardSet != "0" ? lrmiStandards[0].StandardSet : "",
                        Grade = lrmiStandards[0].Grades != "0" ? lrmiStandards[0].Grades : "",
                        Subject = lrmiStandards[0].Subjects != "0" ? lrmiStandards[0].Subjects : "",
                        Course = lrmiStandards[0].Courses != "0" ? lrmiStandards[0].Courses : "",
                        Key = lrmiStandards[0].StandardId != "0" ? lrmiStandards[0].StandardId : ""
                    };
                    if (GetListOfStandards(selectedAssessed).IsNotNullOrEmpty())
                        lrmiTagCriteria.AssessedStandardIds =
                            new List<int>(GetListOfStandards(selectedAssessed).Split(',').Select(int.Parse));
                }

                //Check for rating filter dropdown values.
                List<CheckBoxList.ValueObject> selectedRatings = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("RatingFilter");
                drGeneric_String ratingValues = default(drGeneric_String);
                if (selectedRatings != null && selectedRatings.Any())
                {
                    ratingValues = new drGeneric_String();
                    selectedRatings.ForEach(loop => ratingValues.Add(loop.Value));
                }

                /* Build Criteria Object */
                var qsc = new QuestionSearchCriteria
                          {
                              CourseList = filteredCourses,
                              ItemReservation = itemReservationList,
                              ItemBanks = itemBanks,
                              Text = searchText,
                              TextOpt = searchOption,
                              Filter = standardFilters,
                              SortKeyword = sortField == "ItemBank" || sortField == "Standard" ? null : sortField,
                              StandardSets = selectedStandardSets,
                              QuestionType = itemTypeList,
                              ScoreType = scoreTypeList,
                              DifficultyIndex = difficultyIndexList,
                              ItemReviewStatus = itemStatusList,
                              Rigor = rigorList,
                              AddendumType = selectedAddendumTypes,
                              AddendumGenre = selectedAddendumGenres,
                              AddendumIDs = addendumIDList,
                              CreatedDateRange = createdDateRange,
                              ScoreAverage1 = scoreAverage1,
                              ScoreAverage2 = scoreAverage2,
                              AnchorItem = selectedAnchorItemFilter,
                              ExpirationStatus = selectedExpirationStatus,
                              ExpirationDateRange = selectedExpirationDateRange,
                              LrmiTagCriteria = lrmiTagCriteria,
                              Ratings = ratingValues
                          };

                return qsc;   
            }
            catch (Exception ex)
            {
                ThinkgateEventSource.Log.ApplicationCritical("ItemSearch.aspx.cs --> BuildSearchCriteriaObject", ex.Message, ex.StackTrace);
                throw new Exception(ex.Message);
                return new QuestionSearchCriteria();
            }
            
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Populate the rating filter control
        /// </summary>
        private void PopulateRatingFilter()
        {
            cmbRatingFilter.DataSource = _reviewFacade.GetRatingFilterForItemSearch();
            cmbRatingFilter.DataBind();
        }

        private List<String> PossibleDefaultTexts(object input)
        {
            if (input == null) return null;
            var list = new List<String>();
            list.AddRange(input.ToString().Split(','));
            return list;
        }

        private void LoadLrmiCriteria()
        {
            cbGradeLevel.DataSource = GradeLevels();
            cbEducationalSubject.DataSource = EducationSubject();
            cbLearningResource.DataSource = GetTagElements(LrmiTags.LearningResourceType);
            cbEducationalUse.DataSource = GetTagElements(LrmiTags.EducationalUse);
            cbEndUser.DataSource = GetTagElements(LrmiTags.EndUser);
            cbInteractivity.DataSource = GetTagElements(LrmiTags.Activity);
            cbMediaType.DataSource = GetTagElements(LrmiTags.MediaType);
            cmbAgeAppropriate.DataSource = GetTagElements(LrmiTags.AgeAppropriate).OrderBy(v => v.Value);

            var usageRightTypes = GetCommonCreative();
            cmbUsageRight.DataSource = usageRightTypes.OrderBy(o => o.Value).ToList();
            cmbLanguage.DataSource = LanguageType();

            List<KeyValuePair> standardSetList = CriteriaHelper.GetStandardSetList().OrderBy(x => x.Value).ToList();
            standardSetList.Insert(0, new KeyValuePair("0", "Select Set"));
            cmbStandardSet.StandardSetDataSource = standardSetList;

            PopulateRatingFilter();
        }

        private static List<NameValue> TextSearchDropdownValues()
        {
            return new List<NameValue>
                {
                    new NameValue("Any Words", "any"),
                    new NameValue("All Words","all"),
                    new NameValue("Exact Phrase","exact"),
                    new NameValue("Keywords","key"),
                    new NameValue("Author","author"),
                    //new NameValue("Addendum Name","addendum name"),
                    new NameValue("Standard State nbr","standardnbr"),
                    new NameValue("Standard Name","standardname"),
                    new NameValue("Standard Text","standardtext")
                };
        }

        private static List<NameValue> ItemTypes()
        {

            List<NameValue> itemTypes = new List<NameValue>();

            itemTypes.Add(new NameValue("Multiple Choice (3 Distractors)", "MC3"));
            itemTypes.Add(new NameValue("Multiple Choice (4 Distractors)", "MC4"));
            itemTypes.Add(new NameValue("Multiple Choice (5 Distractors)", "MC5"));
            if (DistrictParms.LoadDistrictParms().DisplayTrueFalse)
            {
                itemTypes.Add(new NameValue("True/False", "T"));
            }
            itemTypes.Add(new NameValue("Short Answer", "S"));
            itemTypes.Add(new NameValue("Essay", "E"));

            return itemTypes;
        }

        private static List<NameValue> ScoreTypes()
        {
            return new List<NameValue>
                {
                    new NameValue("Correct/Incorrect", "W"),
                    new NameValue("Partial Credit","P"),
                    new NameValue("Rubric","R")
                };
        }

        private static List<NameValue> ItemReviewStatus()
        {
            return new List<NameValue>
                {
                    new NameValue("Pending", "1"),
                    new NameValue("Approved","2"),
                    new NameValue("Rejected","3")
                };
        }

        private static List<NameValue> DifficultyIndex()
        {
            return new List<NameValue>
                {
                    new NameValue("Easy", "E"),
                    new NameValue("Medium","M"),
                    new NameValue("Hard","H")
                };
        }

        private static List<NameValue> AnchorItemList()
        {
            return new List<NameValue>
                {
                    new NameValue("Include Anchor Items", "I"),
                    new NameValue("Exclude Anchor Items","E"),
                    new NameValue("Show only Anchor Items","O")
                };
        }

        
        private List<NameValue> LoadItemReservations()
        {
            var dparms = DistrictParms.LoadDistrictParms();

            // determine top level category (State or District).            
            string topLevelCategory = dparms.isStateSystem ? "State" : "District";

            Dictionary<string, bool> typeDictionary = TestTypes.TypeWithSecureFlag((string.IsNullOrEmpty(topLevelCategory) ? "District" : topLevelCategory));

            var list = new List<NameValue>();
            list.Add(new NameValue(topLevelCategory, topLevelCategory));

            List<string> typeListTable = new List<string>();
            bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            bool isSecuredFlag = typeDictionary != null && typeDictionary.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            bool isSecureAssessment = !string.IsNullOrEmpty(Request.QueryString["isSecure"]) && Convert.ToBoolean(Request.QueryString["isSecure"]);

            if (typeDictionary != null)
            {
                if (hasPermission && isSecuredFlag && isSecureAssessment)
                    typeListTable = typeDictionary.Select(x => x.Key).ToList();
                else
                    typeListTable = typeDictionary.Where(x => !x.Value).Select(x => x.Key).ToList();
            }
            list.AddRange(typeListTable.Select(val => new NameValue(val, val)));

            list.Add(new NameValue("Not Reserved", "nores"));
            return list;
        }

        private static List<NameValue> ItemReservations()
        {
            var dparms = DistrictParms.LoadDistrictParms();

            // determine top level category (State or District).            
            string topLevelCategory = dparms.isStateSystem ? "State" : "District";

            var list = new List<NameValue>();
            list.Add(new NameValue(topLevelCategory, topLevelCategory));

            foreach (var val in TestTypes.TypeForCategory(topLevelCategory))
            {
                list.Add(new NameValue(val, val));
            }
            list.Add(new NameValue("Not Reserved", "nores"));
            return list;
        }

        /// <summary>
        /// Gets the value for query string parameter.
        /// </summary>
        /// <param name="parameterName">Name of query string parameter.</param>
        /// <returns>Returns a string containing the value of query string parameter.</returns>
        private static string GetQueryStringValue(string parameterName)
        {
            return string.IsNullOrWhiteSpace(parameterName)
                ? null
                : HttpContext.Current.Request.QueryString.Get(parameterName);
        }

        private void LoadSearchScripts()
        {
            if (Master != null)
            {
                var scriptManager = Master.FindControl("RadScriptManager2");
                if (scriptManager != null)
                {
                    RadScriptManager radScriptManager = (RadScriptManager)scriptManager;
                    radScriptManager.Services.Add(new ServiceReference("Services/Service2.svc"));
                }
            }
        }

        private static List<NameValue> GradeLevels()
        {
            List<KeyValuePair> gradeList = CriteriaHelper.GetGradesList().OrderBy(x => x.Value).ToList();

            return gradeList.Select(kvp => new NameValue(value: kvp.Value, name: kvp.Key)).ToList();
        }

        private List<NameValue> EducationSubject()
        {
            List<NameValue> edSubjectList = new List<NameValue>();
            CourseList currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            IEnumerable<Subject> subjectList = currCourseList.GetSubjectList().OrderBy(sub => sub.DisplayText);
            foreach (Subject sList in subjectList)
            {
                edSubjectList.Add(new NameValue(sList.DisplayText, sList.DisplayText));
            }
            return edSubjectList;
        }

        private List<NameValue> LanguageType()
        {
            List<NameValue> languageTypeList = new List<NameValue>();
            var languageType = Enum.GetValues(typeof(LanguageType))
                .Cast<LanguageType>()
                .Select(v => v.ToString())
                .ToList();
            foreach (string langt in languageType)
            {
                languageTypeList.Add(new NameValue(langt, Convert.ToString((int)Enum.Parse(typeof(LanguageType), langt))));
            }
            return languageTypeList;
        }

        /// <summary>
        /// Get Tag check box items
        /// </summary>
        /// <returns>List</returns>
        private List<NameValue> GetTagElements(LrmiTags lookupType)
        {
            string sql =
                @"select LD.Enum, LD.Description, L.Name AS LookupType from LookupDetails LD inner join LookupType L on L.Enum=LD.LookupEnum WHERE L.Name = '" +
                lookupType + "'" +
                " AND LD.SelectionType LIKE '%" + SelectionType.Items + "%'" +
                " ORDER BY LD.Description;";
            DataTable tagDataTable = GetDataTable(_rootConnectionString, sql);

            List<NameValue> tagTable = (from s in tagDataTable.AsEnumerable()
                                        select new NameValue(value: s["Enum"].ToString(), name: s["Description"].ToString())).ToList<NameValue>();

            return tagTable;
        }

        /// <summary>
        /// Get the Common Creative Usage Rights
        /// </summary>
        /// <returns></returns>
        private IEnumerable<NameValue> GetCommonCreative()
        {
            const string sql = @"select * from CreativeCommon";
            DataTable tagDataTable = GetDataTable(_rootConnectionString, sql);

            List<NameValue> tagTable = (from s in tagDataTable.AsEnumerable()
                                        select new NameValue(value: s["ID"].ToString(), name: s["SelectDescription"].ToString())).ToList<NameValue>();

            return tagTable;
        }

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
                finally
                {
                    sqlConnection.Close();
                }
            }
            return dataTable;
        }

        /// <summary>
        /// GetListOfStandards - Get string back of standard Id's to be used as a list.
        /// </summary>
        /// <param name="selectedStandards"></param>
        /// <returns>string</returns>
        private static string GetListOfStandards(StandardSelections selectedStandards)
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

        #endregion Private Methods

        public string GetCallbackResult()
        {
            string callBackResult = default(string);

            if (leaQuestionReviewAverages != default(LeaQuestionReviewAverages))
            {
                callBackResult = GenerateCallBackXml();
            }

            return callBackResult;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            if (!string.IsNullOrEmpty(eventArgument))
            {
                leaQuestionReviewAverages =
                    _reviewFacade.GetQuestionReviewAveragesAndCount(Convert.ToInt32(eventArgument));
            }
        }

        private string GenerateCallBackXml()
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement root = xmlDoc.CreateElement("CallBackResult");
            XmlElement averageRating = xmlDoc.CreateElement("AverageRating");
            XmlElement reviewCount = xmlDoc.CreateElement("ReviewCount");

            averageRating.InnerText =
                leaQuestionReviewAverages.AverageRating.ToString(CultureInfo.InvariantCulture);

            reviewCount.InnerText =
                leaQuestionReviewAverages.ReviewCount.ToString(CultureInfo.InvariantCulture);

            root.AppendChild(averageRating);
            root.AppendChild(reviewCount);

            xmlDoc.AppendChild(root);

            return xmlDoc.InnerXml;
        }

        /// <summary>
        /// Register the callback scripts
        /// </summary>
        private void RegisterScripts()
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                                                        "SetCallBack",
                                                        "function GetReviewAverageAndCount(arg, context) { " + Page.ClientScript.GetCallbackEventReference(this, "arg", "GetCallbackResult", "") + ";  }",
                                                        true);


        }

    }
}