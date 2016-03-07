using System;
using System.Collections.Generic;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Controls.E3Criteria.Associations;
using Telerik.Web.UI;
using Thinkgate.Controls.E3Criteria;
using System.Linq;
using System.Collections;
using System.Data;
using System.Web.Script.Serialization;

namespace Thinkgate.Controls.Resources
{
    public partial class ResourceSearch : BasePage
    {
        public string ImageWebFolder { get; set; }
        protected bool _Perm_Resource_Link_Ok;

        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += new SearchMaster.SearchHandler(SearchHandler);
            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _Perm_Resource_Link_Ok = UserHasPermission(Permission.Hyperlink_Resource_Name);

            if (!IsPostBack)
            {
                ImageWebFolder = (Request.ApplicationPath.Equals("/") ? string.Empty : Request.ApplicationPath) + "/Images/";

                var serializer = new JavaScriptSerializer();
                /* convert resource category, type, subtype into an array, and then a string to use as a JSON data source */
                var resourceTypes = Thinkgate.Base.Classes.Resource.GetResourceCategoriesDataTable();
                var resourceTypeArry = new ArrayList();
                foreach (DataRow row in ((DataTable)resourceTypes).Rows)
                {
                    resourceTypeArry.Add(new string[] { row["Category"].ToString(), row["TYPE"].ToString(), row["SubType"].ToString() });
                }
                var resourceTypeStr = serializer.Serialize(resourceTypeArry);

                ctrlCategory.JsonDataSource = resourceTypeStr;
                ctrlDocuments.JsonDataSource = resourceTypeStr; 
                ctrlStandards.JsonDataSource = serializer.Serialize(CourseMasterList.StandCourseList.BuildJsonArray());
                ctrlCurriculum.JsonDataSource = serializer.Serialize(CourseMasterList.CurrCourseList.BuildJsonArray());
                ctrlClasses.JsonDataSource = serializer.Serialize(CourseMasterList.ClassCourseList.BuildJsonArray());
                txtSearch.DataSource = TextSearchDropdownValues();
                
                ctrlSchools.JsonDataSource = serializer.Serialize(SchoolMasterList.BuildJsonArrayForTypeAndSchool());
                ctrlStudents.DataSource = Grade.GetGradeListForDistrict(); //TODO: Not a fan of using this to load the grades. Should be in a global static list, but time is short

                ctrlTeachers.DataTextField = "RoleName";
                ctrlTeachers.DataSource = ThinkgateRole.GetRolesCollectionForApplication();
            }
            if (radGridResults.DataSource == null)
            {
                radGridResults.Visible = false;
            }

            if (IsPostBack)
            {
                radGridResults.Visible = true;
            }
                
        }


        /// <summary>
        /// Handler for the Search Button
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            BindDataToGrid();
        }

        public void BindDataToGrid()
        {
            radGridResults.DataSource = SearchResourcesWithCriteria(SessionObject.LoggedInUser, Master.CurrentCriteria());
            radGridResults.DataBind();
        }

        public static List<Thinkgate.Base.Classes.Resource> SearchResourcesWithCriteria(ThinkgateUser user, CriteriaController criteriaController)
        {
            var selectedCategories = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Category").Select(x => x.Value);
            var selectedTypes = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Type").Select(x => x.Value);
            var selectedSubTypes = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Subtype").Select(x => x.Value);

            string resourceName = string.Empty;
            var resourceNameList = criteriaController.ParseCriteria<E3Criteria.Text.ValueObject>("ResourceName");
            if (resourceNameList.Count > 0) resourceName = resourceNameList[0].Text;

            string searchText = string.Empty;
            string searchOption = string.Empty;
            var txtSearchList = criteriaController.ParseCriteria<E3Criteria.TextWithDropdown.ValueObject>("TextSearch");
            if (txtSearchList.Count > 0)
            {
                var confirmedOption = TextSearchDropdownValues().Find(x => x.Name == txtSearchList[0].Option) ?? TextSearchDropdownValues().First();
                if (!String.IsNullOrEmpty(txtSearchList[0].Text))
                {
                    searchText = txtSearchList[0].Text;
                    searchOption = confirmedOption.Value;
                }
            }
            var selectedStandards = new ArrayList();
            var associationType = string.Empty;
            var associationField1 = string.Empty;
            var associationField2 = string.Empty;
            var associationField3 = string.Empty;
            var associatedCurriculum = criteriaController.ParseCriteria<Curriculum.ValueObject>("Curriculum");
            var associatedStandards = criteriaController.ParseCriteria<E3Criteria.Associations.Standards.ValueObject>("Standards");
            var associatedClasses = criteriaController.ParseCriteria<E3Criteria.Associations.Classes.ValueObject>("Classes");
            var associatedDocuments = criteriaController.ParseCriteria<E3Criteria.Associations.Documents.ValueObject>("Documents");
            var associatedSchools = criteriaController.ParseCriteria<E3Criteria.Associations.Schools.ValueObject>("Schools");
            var associatedStudents = criteriaController.ParseCriteria<E3Criteria.Associations.Students.ValueObject>("Students");
            var associatedTeachers = criteriaController.ParseCriteria<E3Criteria.Associations.Teachers.ValueObject>("Teachers"); 
            if (associatedCurriculum.Count > 0)
            {
                associationType = "Curriculum";
                associationField1 = associatedCurriculum[0].Grade;
                associationField2 = associatedCurriculum[0].Subject;
                associationField3 = associatedCurriculum[0].Curriculum;
            }
            else if (associatedStandards.Count > 0 && associatedStandards[0].Standards != null && associatedStandards[0].Standards.Count > 0)
            {
                associationType = "Standards"; 
                selectedStandards = associatedStandards[0].Standards;
            }
            else if (associatedClasses.Count > 0)
            {
                associationType = "Classes";
                associationField1 = associatedClasses[0].Grade;
                associationField2 = associatedClasses[0].Subject;
                associationField3 = associatedClasses[0].Course;
            }
            else if (associatedDocuments.Count > 0)
            {
                associationType = "Documents";
                associationField1 = associatedDocuments[0].TemplateType;
                associationField2 = associatedDocuments[0].TemplateName;
                associationField3 = associatedDocuments[0].DocumentName;
            }
            else if (associatedSchools.Count > 0)
            {
                associationType = "Schools";
                associationField1 = associatedSchools[0].SchoolType;
                associationField2 = associatedSchools[0].School;
                associationField3 = associatedSchools[0].SchoolId;
            }
            else if (associatedStudents.Count > 0)
            {
                associationType = "Students";
                associationField1 = associatedStudents[0].Name;
                associationField2 = associatedStudents[0].Id;
                associationField3 = associatedStudents[0].Grade;
            }
            else if (associatedTeachers.Count > 0)
            {
                associationType = "Teachers";
                associationField1 = associatedTeachers[0].Name;
                associationField2 = associatedTeachers[0].UserType;
                associationField3 = associatedTeachers[0].UserId;
            }
            
            
           

            return Thinkgate.Base.Classes.Resource.SearchAdvanced(selectedCategories, selectedTypes, selectedSubTypes, resourceName, searchText, searchOption,
                associationType, associationField1, associationField2, associationField3, selectedStandards);
        }


        protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            BindDataToGrid();
        }

        protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var item = (GridDataItem) e.Item;
                var hlName = (System.Web.UI.HtmlControls.HtmlAnchor)item.FindControl("hlName");
                var lblName = (System.Web.UI.HtmlControls.HtmlGenericControl)item.FindControl("lblName");
                var itemResource = (Thinkgate.Base.Classes.Resource) item.DataItem;
                if (itemResource.Type.ToLower() == "school improvement plan")
                    item.FindControl("ImgExcel").Visible = true;
                else item.FindControl("ImgExcel").Visible = false;
                if (_Perm_Resource_Link_Ok)
                {
                    lblName.Visible = false;
                    hlName.Attributes.Add("onclick", "openResource(" + itemResource.ID.ToString() + "); return false;");
                    hlName.HRef = "javascript:void(0);";
                    hlName.InnerText = itemResource.ResourceName;

                }
                else
                {
                    hlName.Visible = false;
                    lblName.InnerText = itemResource.ResourceName;
                }
            }
        }

        protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }

        protected void RadGrid_NeedDataSource(object sender, EventArgs e)
        {
        }

        private static List<NameValue> TextSearchDropdownValues()
        {
            return new List<NameValue>
                {
                    new NameValue("Any Words", "any"),
                    new NameValue("All Words","all"),
                    new NameValue("Exact Phrase","exact"),
                    new NameValue("Keywords","key")
                    //, new NameValue("Author","author"),
                    //new NameValue("Addendum Name","addendum name"),
                    //new NameValue("Standard State nbr","standardnbr"),
                    //new NameValue("Standard Name","standardname"),
                    //new NameValue("Standard Text","standardtext"),
                    //new NameValue("Author","author")
                };
        }
        
    }
}