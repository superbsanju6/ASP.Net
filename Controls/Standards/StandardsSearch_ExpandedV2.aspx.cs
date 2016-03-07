using System;
using System.Collections.Generic;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using Thinkgate.Controls.E3Criteria;
using System.Linq;
using System.Data;
using System.Web.Script.Serialization;
using Standpoint.Core.Utilities;
using System.Web.UI;
using ClosedXML.Excel;
using System.IO;
using Thinkgate.Base.DataAccess;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Standards
{
    public partial class StandardsSearch_ExpandedV2 : BasePage
    {
        private int _standardCourseIdParm;
        private string _standardSet;

        private const string PopupScript =
            "No Standard Set or Grade was selected for the search. You must specify at least a Standard Set and Grade for the search.";

        private const string LargeCountMessage =
           "The number of records returned on this search exceeds 5000.  The limit is 5000 and you are viewing a partial list of standards.  Please limit your search by including additional search criteria. <br/><br/> Multiple searches maybe required to view all records";

        private bool _isStandardEncrypted
        {
            get { return Convert.ToBoolean(ViewState["EncID"]); }
            set { ViewState["EncID"] = Convert.ToBoolean(value).ToString(); }
        }
       
        protected DataTable DtGrid;
		private string _standardLevel =  "";

        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += SearchHandler;
            base.Page_Init(sender, e);
            LoadSearchScripts();
        }

        protected bool CheckRequiredFields()
        {
            bool result = true;
            var criteriaController = Master.CurrentCriteria();      // just going to go ahead and pull this from master instead of from search handler so it will work on tree updates

            var selectedStandardSets = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("StandardSet").Select(x => x.Text).ToList();
            var selectedGrades = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Grade").Select(x => x.Text).ToList();


            result = ((selectedGrades.Count > 0) && (selectedStandardSets.Count > 0));
            if (!result)
            {
                radTreeResults.Visible = false;
                if (Master != null)
                {
                    var windowManager = Master.FindControl("RadWindowManager1");
                    if (windowManager != null)
                    {
                        RadWindowManager radWindowManager = (RadWindowManager) windowManager;
                        radWindowManager.RadAlert(PopupScript, 330, 180, "Message from webpage", string.Empty);
                    }
                }
            }
            return result;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Question"]))
            {
                hfQuestionID.Value = DataIntegrity.ConvertToInt(Request.QueryString["Question"]).ToString();
            }

            if (!IsPostBack)
            {
                ParseRequestQueryString();
                var serializer = new JavaScriptSerializer();
                
                CourseList courses = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);
				System.Collections.ArrayList standardLevels = CourseMasterList.GetStandardSets_StandardLevelsXref();

				GradeSubjectCourseStandardSet.GradeSubjectCourseStandardSetWithStandardLevels cscssDataSource =
					new GradeSubjectCourseStandardSet.GradeSubjectCourseStandardSetWithStandardLevels() 
						{ GradeSubjectCourseStandardSetData = courses.BuildJsonArray(), StandardLevelsData = standardLevels };
				
                txtSearch.DataSource = TextSearchDropdownValues();
                ctrlGradeSubjectCourseStandardSet.JsonDataSource = serializer.Serialize(cscssDataSource);

                if (_standardCourseIdParm > 0)
                {
                    var preSelectedCourse = CourseMasterList.StandCourseDict[_standardCourseIdParm];
                    ctrlGradeSubjectCourseStandardSet.DdlStandardSet.DefaultTexts =  new List<String> {_standardSet};
                    ctrlGradeSubjectCourseStandardSet.ChkGrade.DefaultTexts = new List<String>{preSelectedCourse.Grade.ToString()};
                    ctrlGradeSubjectCourseStandardSet.ChkSubject.DefaultTexts = new List<String> { preSelectedCourse.Subject.ToString() };
                    ctrlGradeSubjectCourseStandardSet.CmbCourse.DefaultTexts = new List<String> {preSelectedCourse.CourseName};
                }

                cmbStandardFilter.DataSource = SessionObject.LoggedInUser.StandardFilters.Distinct();
            }                
            SortBar.HideExcelButtonClientSide();
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
                    radScriptManager.Scripts.Add(new ScriptReference("~/Scripts/TeacherSearch.js"));
                }
            }
        }

        /// <summary>
        /// Handler for the Search Button
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            if (radTreeResults.Visible)
            {
                if (CheckRequiredFields())
                {
                    if (DtGrid == null) DoSearch();
                    DataView dv = new DataView(DtGrid, "", "", DataViewRowState.CurrentRows);
                    radTreeResults.DataSource = dv;
                    //radTreeResults.DataBind();
                    foreach (var item in radTreeResults.Items)
                    {
                        item.Expanded = false;
                    }
                }
            } 
            else
                radTreeResults.Visible = true;
        }
        
        protected void TreeListDataSourceNeeded(object sender, EventArgs e)
        {
            if (CheckRequiredFields())
            {
                if (DtGrid == null) DoSearch();
                DataView dv = new DataView(DtGrid, "", "", DataViewRowState.CurrentRows);
                radTreeResults.DataSource = dv;
                if (radTreeResults.Visible == false) radTreeResults.Visible = true;
            }
        }

        //protected void TreeListChild_DataSourceNeeded(object sender, TreeListChildItemsDataBindEventArgs e)
        //{
        //    int id = Convert.ToInt32(e.ParentDataKeyValues["StandardID"].ToString());
        //    if (DtGrid == null) DoSearch();
        //    DataView dv = new DataView(DtGrid, "ParentID = " + id, "", DataViewRowState.CurrentRows);
        //    e.ChildItemsDataSource = dv;
        //}


        protected void radTreeResults_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {
            if (e.Item is TreeListDataItem)
            {
                TreeListDataItem item = e.Item as TreeListDataItem;

                item.FindControl("lnkExpandAll").Visible = item.CanExpand;
                if (item.Expanded)
                {
                    item.FindControl("lnkCollapseAll").Visible = item.Expanded;
                    item.FindControl("lnkExpandAll").Visible = false;
                }
            }
        }     

        protected void radTreeResults_ItemCommand(object sender, TreeListCommandEventArgs e)
        {
            if (e.CommandName == RadTreeList.ExpandCollapseCommandName)
            {
                if ((e.Item as TreeListDataItem).HierarchyIndex.NestedLevel == 0 && e.CommandArgument.ToString() == "ExpandAll")
                {
                    e.Canceled = true;
                    if (!(e.Item as TreeListDataItem).Expanded)
                    {
                        radTreeResults.ExpandItemToLevel((e.Item as TreeListDataItem), 10);
                    }
                    else
                    {
                        radTreeResults.CollapseAllItems();
                    }
                }
            }
        }

        #region Private Methods
        
        private void DoSearch(bool excelExport=false)
        {
            var criteriaController = Master.CurrentCriteria();      // just going to go ahead and pull this from master instead of from search handler so it will work on tree updates
            
            /* Courses */
            var selectedGrades = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Grade").Select(x => x.Text).ToList();
            var selectedSubjects = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Subject").Select(x => x.Text).ToList();
            var selectedCourses = criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("Course").Select(x => x.Text).ToList();
            var selectedStandardSets = new drGeneric_String(criteriaController.ParseCriteria<E3Criteria.CheckBoxList.ValueObject>("StandardSet").Select(x => x.Text));     // take straight to drGeneric_String because it's going to SQL
            var filteredCourses = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser).FilterByGradesSubjectsStandardSetsAndCourse(selectedGrades, selectedSubjects, selectedStandardSets, selectedCourses);
			List<DropDownList.ValueObject> selectedLevels = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Level"); 
			_standardLevel = selectedLevels.Count > 0 ? selectedLevels[0].Text : "";

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

            /* Standard Filters */
            var standardFilters = new drGeneric_Int();
            var selectedStandardFilter = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StandardFilter");
            if (selectedStandardFilter != null && selectedStandardFilter.Count >= 1 && SessionObject.LoggedInUser.StandardFilters.ContainsKey(selectedStandardFilter[0].Text))
            {
                IEnumerable<int> ids = SessionObject.LoggedInUser.StandardFilters[selectedStandardFilter[0].Text].Split(',').Select(DataIntegrity.ConvertToInt);                
                standardFilters.AddRange(ids);
            }

            dtItemBank itemBanks = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");

			DtGrid = Thinkgate.Base.Classes.Standards.SearchStandards(	itemReservation: UserHasPermission(Permission.Access_ItemReservation),
																		textSearch:searchText,
																		textSearchVal:searchOption,
																		itemBanks:itemBanks,
																		standardCourses:filteredCourses,
																		filterList:standardFilters,
																		standardSetList:selectedStandardSets,
																		standardLevel: _standardLevel);

            
            
            // Add columns to the data table.
            DataColumn displayTextCol = DtGrid.Columns.Add("NameDisplayText", typeof(String));
            List<Int32> allStandardIds = (from s in DtGrid.AsEnumerable() select s.Field<Int32>("StandardID")).Distinct().ToList();
            String currRoot = "";
            foreach (DataRow row in DtGrid.Rows)
            {
                    currRoot = (String)((String)row["StandardName"]).Clone();
                    row[displayTextCol] = currRoot;
            }

            if (excelExport) 
                return;

            DtGrid = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(DtGrid, "StandardID", "LinkID");

            if (_isStandardEncrypted)
            {
                DtGrid = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(DtGrid, "StandardID", "EncryptedID");
            }
            else
            {
                DtGrid.Columns.Add("EncryptedID");
                foreach (DataRow dr in DtGrid.Rows)
                {
                    dr["EncryptedID"] = dr["StandardID"];
                }
            }

            foreach (DataRow row in DtGrid.Rows)
            {
               if (row["ParentID"].Equals(0) || !string.IsNullOrEmpty(searchText))
                {
                    row["ParentID"] = System.DBNull.Value;
                }
            }
            if (DtGrid != null)
            {
                if (DtGrid.Rows.Count > 5000)
                {
                    var windowManager = Master.FindControl("RadWindowManager1");
                    if (windowManager != null)
                    {
                        RadWindowManager radWindowManager = (RadWindowManager)windowManager;
                        radWindowManager.RadAlert(LargeCountMessage, 330, 180, "Message from webpage", string.Empty);
                    }
                    DataTable dt = DtGrid.Rows.Cast<System.Data.DataRow>().Take(5000).CopyToDataTable();
                    DtGrid.Clear();
                    DtGrid = dt;
                }
                DataView dv = new DataView(DtGrid, "", "", DataViewRowState.CurrentRows);
                radTreeResults.DataSource = dv;
            }
        }

        private void ParseRequestQueryString()
        {
            // We display the standards tree view only if not coming from an item editor.
            bool selectMode = (Request.QueryString.Keys.Count != 0 && string.IsNullOrEmpty(Request.QueryString["standardCourseID"]));
            bool singleSelect = DataIntegrity.ConvertToBool(Request.QueryString["MultiSelect"]);

            //Use standard course ID url parameter to search standards.
            _standardCourseIdParm = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "standardCourseID");
            _standardSet = Request.QueryString["standardSet"];
            _isStandardEncrypted = DataIntegrity.ConvertToBool(!string.IsNullOrEmpty(Request.QueryString["EncID"]) ? Request.QueryString["EncID"] : "True");

            if (_standardCourseIdParm > 0)
            {
                SearchAndClear.RunSearchOnPageLoad = true;
            }

            if (selectMode)
            {
                radTreeResults.ClientSettings.Selecting.AllowItemSelection = true;
                SortBar.ShowSelectAndReturnButton = true;
            }

            radTreeResults.Columns[0].Visible = selectMode;
            radTreeResults.AllowMultiItemSelection = !singleSelect;
        }

        #endregion

        public void ExportToExcel(DataTable dt)
        {
            // Create the workbook
            XLWorkbook workbook = Master.ConvertDataTableToSingleSheetWorkBook(dt, "Results");

            //Prepare the response

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
                Response.Clear();
                Response.Buffer = true;
                if (browser.Platform.IndexOf("WinNT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    workbook.SaveAs(memoryStream);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");
                    // Flush the workbook to the Response.OutputStream
                    Response.BinaryWrite(memoryStream.ToArray());
                }
                else
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.ContentType = "text/csv";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=ExportData.csv");
                    byte[] csv = ExportToCSV.ConvertXLToCSV(workbook);
                    Response.BinaryWrite(csv);
                }
                Response.End();
            }
        }

        protected void ExportGridImgBtn_Click(object sender, EventArgs e)
        {
            DoSearch(true);
            if (DtGrid != null && DtGrid.Rows.Count > 0)
            {
                DtGrid.Columns.Remove("StandardID");
                DtGrid.Columns.Remove("ParentID");
                ExportToExcel(DtGrid);
            }
        }

        private static List<NameValue> TextSearchDropdownValues()
        {
            return new List<NameValue>
                {
                    new NameValue("Any Words", "any"),
                    new NameValue("All Words","all"),
                    new NameValue("Exact Phrase","exact"),
                    new NameValue("Keywords","key"),
                    new NameValue("Standard State nbr","standardnbr"),
                    new NameValue("Standard Name","standardname"),
                    new NameValue("Standard Text","standardtext"),
                };
        }   
    }
}