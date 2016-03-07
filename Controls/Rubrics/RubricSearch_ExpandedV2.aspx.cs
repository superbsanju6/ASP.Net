using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Classes.Serializers;
using Thinkgate.Controls.E3Criteria;
using System.Web.Script.Serialization;
using System.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Base.DataAccess;

namespace Thinkgate.Controls.Rubrics
{
    public partial class RubricSearch_ExpandedV2 : BasePage
    {
        public string ImageWebFolder { get; set; }
        public string clientFolder { get; set; }
        protected string ExpandPerm_IconWidth { get; set; }
        protected string ExpandPerm_onclick { get; set; }
        public static int RecordsPerPage = 100;
        public static string TestCategory { get; set; }
        public static bool ShowExpiredItems { get; set; }

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            Master.Search += new SearchMaster.SearchHandler(SearchHandler);
            if (!IsPostBack)
            {
                LoadSearchScripts();


                DistrictParms parms = DistrictParms.LoadDistrictParms();
                ShowExpiredItems = (Request.QueryString["ShowExpiredItems"] != "No" && parms.AllowSearchForCopyRightExpiredContent);
                if (!ShowExpiredItems)
                    ExpirationStatusDateRange.DDLExpirationStatus.Visible = false;

                ImageWebFolder = (Request.ApplicationPath.Equals("/") ? string.Empty : Request.ApplicationPath) + "/Images/";

                if (!UserHasPermission(Permission.Icon_Expand_Item))
                {
                    ExpandPerm_IconWidth = "0px";
                    ExpandPerm_onclick = "";
                }
                else
                {
                    ExpandPerm_IconWidth = "17px";
                    ExpandPerm_onclick = "onclick=\"window.open('" + clientFolder + "/Record/RubricPage.aspx?xID={{:ID_Encrypted}}');\"";
                }

                var serializer = new JavaScriptSerializer();

                var itemBankEditTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
                var itemBankCopyTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankCopy, "Search");
                serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
                string renderIBEditArray = "var IBEditArray = [" + serializer.Serialize(itemBankEditTbl) + "]; ";
                renderIBEditArray += "var IBCopyArray = [" + serializer.Serialize(itemBankCopyTbl) + "]; ";
                ScriptManager.RegisterStartupScript(this, typeof(string), "IBEditArray", renderIBEditArray, true);


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
                //ctrlRubricType.DataSource = new List<String> {"Holistic", "Advanced"};
                drGeneric_String_String lstRubrics = new drGeneric_String_String();
                lstRubrics.Add("Holistic", "B");
                lstRubrics.Add("Analytical", "A");
                System.Data.DataTable dtRubricTypes = lstRubrics.ToDataTable();
                ctrlRubricType.DataTextField = "Name";
                ctrlRubricType.DataValueField = "Value";
                ctrlRubricType.DataSource = dtRubricTypes;

                dtItemBank dtItemBank = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser).DistinctByLabel();

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
                string rubricType = Request.QueryString["rubrictype"];
                if (!String.IsNullOrEmpty(rubricType))
                {
                    ctrlRubricType.DefaultTexts = new List<string> { rubricType };
                    ctrlRubricType.ReadOnly = true;
                }
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

            var resultsPage1 = Rubric.GetRubricsByCriteriaPaging(criteriaObject, 1, RecordsPerPage);

            var rubricPage1 = resultsPage1.RubricList;

            var siteMaster = this.Master;
            if (siteMaster != null && this.Master is SearchMaster)
            {
                SearchPager.Visible = true;
                SearchPager.PageSize = RecordsPerPage;
                //SearchPager.ResultCount = resultsPage1.FullResultCount;
                SearchPager.ResultCount = resultsPage1.RubricList.Count;
                SearchPager.DataBind();

                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer(), new StandardsSerializer() });
                string renderItemsScript = "ItemArray = []; ";
                renderItemsScript += "maxPage = " + SearchPager.GetNumberOfPages() + ";";
                switch (SortBar.SelectedSortText())
                {
                    case "ItemBank":
                        renderItemsScript += "ItemArray[0] = " + serializer.Serialize(from rubric in rubricPage1
                                                                                      orderby rubric.ItemBanks
                                                                                      select rubric) + ";";
                        break;
                    /* case "Standard":
                         renderItemsScript += "ItemArray[0] = " + serializer.Serialize(from question in questionsPage1
                                                                                       orderby (question.Standards.Count > 0 ? question.Standards[0].StandardName : "")
                                                                                       select question) + ";";
                         break;*/
                    default:
                        renderItemsScript += "ItemArray[0] = " + serializer.Serialize(rubricPage1) + ";";
                        break;
                }
                renderItemsScript += "SortField = '" + SortBar.SelectedSortText() + "';";
                renderItemsScript += "RenderFirstPage();";

                ScriptManager.RegisterStartupScript(this, typeof(string), "data", renderItemsScript, true);
            }
        }


        /// <summary>
        /// Builds the QuestionSearchCriteria. Used both from Search Button and from Service
        /// </summary>
        public static RubricSearchCriteriaV2 BuildSearchCriteriaObject(ThinkgateUser user, CriteriaController criteriaController, string requestedSortField = null)
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


            /* Rubric Type */
            var selectedRubricType = new drGeneric_String(criteriaController.ParseCriteria<CheckBoxList.ValueObject>("RubricType").Select(x => x.Value));

            var createdDateRange = new drGeneric_String_String();
            foreach (var val in criteriaController.ParseCriteria<DateRange.ValueObject>("CreatedDate"))
            {
                createdDateRange.Add(val.Type == "Start" ? "CreatedDateStart" : "CreatedDateEnd", val.Date);
            }
            
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
            var asc = new RubricSearchCriteriaV2()
            {
                ItemBanks = itemBanks,
                CourseList = filteredCourses,
                RubricTypes = selectedRubricType,
                UserAccessItemReservations = "",
                CreatedDateRange = createdDateRange,
                Text = searchText,
                TextOpt = searchOption,
                SortKeyword = sortField == "ItemBank" || sortField == "Standard"
                    ? null
                    : sortField,
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
                           new NameValue("Grade", "Grade"),
                           new NameValue("Subject", "Subject"),
                           new NameValue("Standard", "Standard"),
                           new NameValue("Name", "Name"),
                           new NameValue("Author", "Author"),
                           new NameValue("Keywords", "Keywords"),
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

        protected void copyRubricXmlHttpPanel_ServiceRequest(object sender, RadXmlHttpPanelEventArgs e)
        {
            int newRubricID = Base.Classes.Rubric.CopyRubric(Convert.ToInt32(e.Value));
            if (newRubricID == 0)
            {
                throw new Exception("An error occured while copying the Rubric");
            }
        }
    }
}