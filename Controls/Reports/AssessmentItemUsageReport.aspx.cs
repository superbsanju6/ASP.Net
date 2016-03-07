using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.UsageStatisticsReport;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Classes.UsageStatisticsReport;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Domain.Classes;
using Thinkgate.Enums;
using Thinkgate.Services.Contracts.ServiceModel;
using Thinkgate.Services.Contracts.UsageStatistics;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Services.Contracts.AssessmentUsageStatistics;
using System.Net;
using System.Net.Security;
using System.Web.UI.HtmlControls;
using WebSupergoo.ABCpdf9;
using CMS.GlobalHelper;

namespace Thinkgate.Controls.Reports
{
    public partial class AssessmentItemUsageReport : BasePage
    {
        private List<string> dtItemBank = new List<string>();
        private List<string> selectedSubjects = new List<string>();
        private DataTable lstItemBank = new DataTable();
        private string selectedCategory;
        private string selectedSchool;
        private string selectedGrades;
        private drGeneric_String_String selectedDateRange = new drGeneric_String_String();
        private string _clientID = string.Empty;
        private AssessmentItemUsageStatisticData _reportData;
        public string StartDate = string.Empty;
        public string EndDate = string.Empty;
        private AssessmentItemStatisticMappings mappings = new AssessmentItemStatisticMappings();

        private static int LoggedInUserSchool { get; set; }
        private static List<ThinkgateSchool> LoggedUsersSchoolCollections { get; set; }

        protected string testCategory;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            Master.Search += (SearchHandler);

            LoggedUsersSchoolCollections = new List<ThinkgateSchool>();

            testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? SessionObject.AssessmentBuildParms["TestCategory"] : "Classroom";

            DistrictParms parms = DistrictParms.LoadDistrictParms();
            _clientID = parms.ClientID;
            var yearRange = parms.Year.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            StartDate = "01/01/20" + yearRange[0];
            EndDate = DateTime.Parse("01-01-20" + (int.Parse(yearRange[1]) + 1)).AddDays(-1).ToShortDateString();

        }

        private void LoadItemBankInfo()
        {
            dtItemBank selectedItemBanksTable =new dtItemBank();   
            dtItemBank itemBankListTable = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory);
            dtItemBank itemBankListTableByLabel = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory).DistinctByLabel();

            foreach (DataRow itemBankRow in from DataRow row in itemBankListTableByLabel.Rows
                                            where itemBankListTable.Select("Label = '" + row["Label"] + "'").Length != 0
                                            where itemBankListTable.Select("Label = '" + row["Label"] + "'")[0]["isFieldTestBank"]
                                                .ToString() == "False"
                                            select itemBankListTable.Select("Label = '" + row["Label"] + "'")[0]) {
                                                selectedItemBanksTable.Add(
                                                    DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                                    itemBankRow["Target"].ToString(),
                                                    DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                                    itemBankRow["Label"].ToString(),
                                                    itemBankRow["isFieldTestBank"].ToString());
                                            }

            if (chkItemBank != null)
            {
                chkItemBank.DataSource = selectedItemBanksTable;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoggedInUserSchool = SessionObject.LoggedInUser.School;
                LoggedUsersSchoolCollections = SessionObject.LoggedInUser.Schools;

                //ItemBank
                LoadItemBankInfo();
                
                //Category
                ddlCategory.DataSource = new List<string> { "Classroom", "District" };
                ddlCategory.DefaultTexts = new List<string> { "District" };

                //School
                // ddlSchool.DataSource = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
                PopulateSchoolDropdown();

                PopulatesGrade();

                //Subject
                var subjectsDT = new DataTable();
                var subjectsByCurrCourses = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).GetSubjectList();
                subjectsDT.Columns.Add("Subject");

                foreach (var s in subjectsByCurrCourses)
                {
                    subjectsDT.Rows.Add(s.DisplayText);
                }
                chkSubject.DataSource = subjectsDT;
                chkSubject.DataBind();

                //Last run date
                var lastRun = mappings.GetReportLastRunDate();
                lblDate.Text = lastRun != default(DateTime) ? lastRun.ToShortDateString() : string.Empty;
            }
        }



        protected void radGridItemBank_ItemDataBound(object sender, GridItemEventArgs e)
        {

        }

        protected void radGridItemBank_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {

        }

        protected void radGridResults_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            string action = "Search";
            DoSearch(action);
        }

        protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                HyperLink itemIdLink = (HyperLink)item.FindControl("itemIDLink");
                var rowItem = (Thinkgate.Services.Contracts.AssessmentUsageStatistics.AssessmentItemFrequency)item.DataItem;

                string xID = Encryption.EncryptInt(rowItem.ItemID);
                itemIdLink.NavigateUrl = "~/Record/BankQuestionPage.aspx?xID=" + xID;
                itemIdLink.Attributes["target"] = "_blank";
                itemIdLink.Attributes["style"] = "color:#00F;";
                itemIdLink.Text = rowItem.ItemID.ToString();

                HtmlGenericControl spanBankTotal = (HtmlGenericControl)item.FindControl("spanbankTotal");

                spanBankTotal.InnerHtml = rowItem.Frequency + (rowItem.BankTotal > 1 ? " (*)" : string.Empty);
                spanBankTotal.Attributes.Add("Title", rowItem.BankTotal > 1 ? "This amount includes all assessment item iterations of the original bank item." : string.Empty);
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void radGridResults_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {

        }

        protected void radGridResults_SortCommand(object sender, GridSortCommandEventArgs e)
        {

        }

        protected void radGridItemBank_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            string action = "Search";
            DoSearch(action);
        }

        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            string action = "Search";
            DoSearch(action);

        }

        private void DoSearch(string action)
        {
            CriteriaController criteriaController = Master.CurrentCriteria();
            criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.CheckBoxList.ValueObject>("ItemBank").ToList().ForEach(x => dtItemBank.Add(x.Text));
            selectedCategory = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Category").Select(x => x.Text).FirstOrDefault();
            selectedSchool = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("School").Select(x => x.Text).FirstOrDefault();
            selectedGrades = criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.DropDownList.ValueObject>("Grade").Select(x => x.Text).FirstOrDefault();
            criteriaController.ParseCriteria<Thinkgate.Controls.E3Criteria.CheckBoxList.ValueObject>("Subject").Select(x => x.Text).ToList().ForEach(x => selectedSubjects.Add(x));

            var dateList = criteriaController.ParseCriteria<DateRange.ValueObject>("DateRange");
            selectedDateRange.Add("DateStart", dateList.FirstOrDefault(x => x.Type == "Start") != null ? dateList.FirstOrDefault(x => x.Type == "Start").Date : null);
            selectedDateRange.Add("DateEnd", dateList.FirstOrDefault(x => x.Type != "Start") != null ? dateList.FirstOrDefault(x => x.Type != "Start").Date : null);

            hdnItemBank.Value = string.Empty;
            foreach (var val in dtItemBank)
            {
                hdnItemBank.Value = hdnItemBank.Value + "," + val;
            }

            if (hdnItemBank.Value != "")
            {
                if (hdnItemBank.Value[0] == ',')
                    hdnItemBank.Value = hdnItemBank.Value.Substring(1);
            }

            hdnCategory.Value = selectedCategory;
            hdnSchool.Value = selectedSchool;
            hdnGrade.Value = selectedGrades;
            hdnSubject.Value = string.Empty;
            foreach (var val in selectedSubjects)
            {
                hdnSubject.Value = hdnSubject.Value + "," + val;
            }

            if (hdnSubject.Value != "")
            {
                if (hdnSubject.Value[0] == ',')
                    hdnSubject.Value = hdnSubject.Value.Substring(1);
            }

            hdnStartDate.Value = (selectedDateRange[0].Value != null ? DateTime.Parse(selectedDateRange[0].Value).ToString() : string.Empty);
            hdnEndDate.Value = (selectedDateRange[1].Value != null ? DateTime.Parse(selectedDateRange[1].Value).ToString() : string.Empty);
            DistrictParms parms = DistrictParms.LoadDistrictParms();
            var paramanters = new AssessmentItemUsageStatisticInputParameters()
            {
                ClientID = parms.ClientID,
                ItemBanks = dtItemBank,
                Category = selectedCategory,
                School = selectedSchool,
                Grade = selectedGrades,
                Subjects = selectedSubjects,
                StartDate = selectedDateRange[0].Value != null ? DateTime.Parse(selectedDateRange[0].Value) : default(DateTime?),//DateTime.Parse("1/1/1753"),
                EndDate = selectedDateRange[1].Value != null ? DateTime.Parse(selectedDateRange[1].Value) : default(DateTime?)//DateTime.MaxValue,
            };

            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(
                    delegate
                    { return true; }
                );

            var proxy =
                new UsageStatisticsProxy(new SamlSecurityTokenSettings
                {
                    SamlSecurityTokenizerAction = SamlSecurityTokenizerAction.UseThreadPrincipalIdentity,
                    ServiceCertificateStoreName = ConfigurationManager.AppSettings["ServiceCertificateStoreName"],
                    ServiceCertificateThumbprint = ConfigurationManager.AppSettings["ServiceCertificateThumbprint"]
                });

          
            _reportData = proxy.GetAssessmentItemUsageStatistics(paramanters);
            

            radGridResults.DataSource = _reportData.ItemFrequencies;
            radGridResults.DataBind();
            radGridItemBank.DataSource = _reportData.ItemBankFrequencies;
            radGridItemBank.DataBind();

            if (_reportData.ItemFrequencies.Count == 0 && _reportData.ItemBankFrequencies.Count == 0 && action == "Search")
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(Page), Guid.NewGuid().ToString(), "alert('Administered assessment results are not available for the selected search criteria.');", true);
            }
            else 
            { 
                radGridResults.Visible = true;
                radGridItemBank.Visible = true;
            }
            initialDisplayText.Visible = false;
        }

        private void ExportToExcel(XLWorkbook workbook)
        {
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

        private void AddWorkSheet<T>(XLWorkbook workbook, List<T> data, string sheetName = null) where T : class
        {
            var sheet = sheetName ?? "SheetName" + Math.Ceiling((double)new Random().Next(1000, 10000)).ToString();
            if (data.Count > 0)
            {
                workbook.Worksheets.Add(sheetName);

                var currentSheet = workbook.Worksheet(sheetName);
                currentSheet.Column(1).Width = 12;
                currentSheet.Column(2).Width = 28;
                currentSheet.Range("A1", "C1").Merge();
                currentSheet.Cell(1, 1).Value = "Assessment Item Usage Report";
                currentSheet.Cell(1, 1).Style.Font.SetBold();

                string _itemBank = "";
                string _subject = "";
                string _date = "Entire School Year";
                foreach (var val in dtItemBank)
                {
                    _itemBank = _itemBank + "," + val;
                }
                foreach (var val in selectedSubjects)
                {
                    _subject = _subject + "," + val;
                }
                if (selectedDateRange[0].Value != null && selectedDateRange[1].Value != null)
                    _date = selectedDateRange[0].Value + " - " + selectedDateRange[1].Value;
                currentSheet.Range("A2", "A7").Merge();
                currentSheet.Cell(2, 1).Value = "Assessment Item Usage Report as of: " + lblDate.Text;
                currentSheet.Cell(2, 1).Style.Alignment.WrapText = true;
                currentSheet.Cell(2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                currentSheet.Cell(2, 2).Value = "Item Bank: " + (string.IsNullOrEmpty(_itemBank) ? "All" : _itemBank.Substring(1));
                currentSheet.Cell(2, 2).Style.Alignment.WrapText = true;
                currentSheet.Cell(3, 2).Value = "Category: " + selectedCategory;
                currentSheet.Cell(3, 2).Style.Alignment.WrapText = true;
                currentSheet.Cell(4, 2).Value = "School: " + (string.IsNullOrEmpty(selectedSchool) ? "All" : selectedSchool);
                currentSheet.Cell(4, 2).Style.Alignment.WrapText = true;
                currentSheet.Cell(5, 2).Value = "Grade: " + (string.IsNullOrEmpty(selectedGrades) ? "All" : selectedGrades);
                currentSheet.Cell(5, 2).Style.Alignment.WrapText = true;
                currentSheet.Cell(6, 2).Value = "Subject: " + (string.IsNullOrEmpty(_subject) ? "All" : _subject.Substring(1));
                currentSheet.Cell(6, 2).Style.Alignment.WrapText = true;
                currentSheet.Cell(7, 2).Value = "Date Range: " + (string.IsNullOrEmpty(_date) ? "Entire School Year" : _date);
                currentSheet.Cell(7, 2).Style.Alignment.WrapText = true;

                int colCount;
                var rowCount = 9;
                foreach (var item in data)
                {
                    colCount = 1;
                    foreach (var prop in typeof(T).GetProperties()) // Loop over the items.
                    {
                        if (prop.Name != "BankTotal")
                        {
                            currentSheet.Cell(rowCount, colCount).Value = prop.GetValue(item);
                            colCount++;
                        }
                    }

                    rowCount++;
                }

                colCount = 1; //reset columns
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.Name != "BankTotal")
                    {
                        currentSheet.Cell(8, colCount).Value = prop.Name;
                        currentSheet.Cell(8, colCount).Style.Font.SetBold();
                        colCount++;
                    }
                }
            }
        }

        protected void exportGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            string action = "export";
            var workBook = new XLWorkbook();
            if (radGridResults.Visible == true && radGridItemBank.Visible == true)
            {
                DoSearch(action);
                AddWorkSheet<AssessmentItemFrequency>(workBook, _reportData.ItemFrequencies.ToList(), "Item ID Sheet");
                AddWorkSheet<ItemBankFrequency>(workBook, _reportData.ItemBankFrequencies.ToList(), "Item Bank Sheet");
                ExportToExcel(workBook);
            }
            ddlCategory.DefaultTexts = new List<string> { "District" };
        }

        protected void printGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (radGridResults.Visible == true && radGridItemBank.Visible == true)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SubmitPrint", "submit_print();", true);
            }
            ddlCategory.DefaultTexts = new List<string> { "District" };
        }

        [System.Web.Services.WebMethod]
        public static string GetGradeListOnSchoolSelection(int schoolID)
        {
            if (schoolID <= 0)
            {
                return null;
            }

            DataTable dtGrade = new DataTable();
            string[] GradeListCollection = null;
            List<Grade> lstGrade = null;

            string gradesList = Base.Classes.School.GetSchoolByID(schoolID).Grades;

            if (!String.IsNullOrEmpty(gradesList))
            {
                if (gradesList.Contains(','))
                {
                    GradeListCollection = gradesList.Split(',');
                }
            }

            if (GradeListCollection != null)
            {
                lstGrade = new List<Grade>();
                dtGrade.Columns.Add(new DataColumn("Grade", typeof(string)));

                foreach (var itemGrade in GradeListCollection)
                {
                    dtGrade.Rows.Add(itemGrade);
                }
            }
            return dtGrade.Rows.Count <= 0 ? null : dtGrade.ToJSON(false);
        }



        [System.Web.Services.WebMethod]
        public static string PopulateDefaultGrade()
        {
            DataTable dtGrade = GetUsersSchoolGrade();

            return dtGrade.Rows.Count <= 0 ? null : dtGrade.ToJSON(false);
        }

        private void PopulateSchoolDropdown()
        {
            var schoolDataTable = new DataTable();
            var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolDataTable.Columns.Add("Name");
            schoolDataTable.Columns.Add("ID");

            foreach (var s in schoolsForLooggedInUser)
            {
                schoolDataTable.Rows.Add(s.Name, s.ID);
            }

            if (schoolDataTable.Rows.Count > 0)
            {
                ddlSchool.DataTextField = "Name";
                ddlSchool.DataValueField = "Id";
                ddlSchool.DataSource = schoolDataTable;
            }
        }

        private void PopulatesGrade()
        {
            ddlGrade.DataSource = GetUsersSchoolGrade();
            ddlGrade.DataBind();
        }

        public static DataTable GetUsersSchoolGrade()
        {

            DataTable dtGrade = new DataTable();
            dtGrade.Columns.Add(new DataColumn("Grade", typeof(string)));

            List<Grade> lstAllGrade = new List<Grade>();

            if (LoggedUsersSchoolCollections != null)
            {
                string[] GradeList = new string[] { }; 
                foreach (var item in LoggedUsersSchoolCollections)
                {

                    string[] GradeListCollection = null;
                   
                    string gradesList = Base.Classes.School.GetSchoolByID(item.Id).Grades;

                    if (!String.IsNullOrEmpty(gradesList))
                    {
                        if (gradesList.Contains(','))
                        {
                            GradeListCollection =gradesList.Split(',');
                        }
                    }
                    if (GradeListCollection != null)
                    {
                        foreach (var itemGrade in GradeListCollection)
                        {
                            lstAllGrade.Add(new Grade { DisplayText = itemGrade.ToString() });
                        }
                    }
                }

                lstAllGrade = lstAllGrade.Distinct().ToList();

                foreach (var itemGrade in lstAllGrade)
                {
                    dtGrade.Rows.Add(itemGrade.DisplayText);
                }
            }
            return dtGrade;
        }

    }
}