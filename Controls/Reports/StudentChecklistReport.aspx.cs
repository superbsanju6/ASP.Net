using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Extensions;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;

namespace Thinkgate.Controls.Reports
{

    public partial class StudentChecklistReport : BasePage
    {
        protected enum SearchTab { Basic, Advanced }

        //using for ordering for the months which session has started. In description, oreder of the month is specified.
        private enum MonthsOrder
        {
            [Description("7")]
            January = 1,
            [Description("8")]
            February,
            [Description("9")]
            March,
            [Description("10")]
            April,
            [Description("11")]
            May,
            [Description("12")]
            June,
            [Description("1")]
            July,
            [Description("2")]
            August,
            [Description("3")]
            September,
            [Description("4")]
            October,
            [Description("5")]
            November,
            [Description("6")]
            December

        }

        private enum ItemStatus
        {
            [Description("Completed")]
            Completed,
            [Description("Incomplete")]
            Incomplete
        }

        private StudentChecklistReportVM ViewModel
        {
            get;
            set;
        }

        private IEnumerable<StudentChecklistReportVM> TempData
        {
            get { return ViewState["TempData"] as IEnumerable<StudentChecklistReportVM>; }
            set { ViewState["TempData"] = value; }
        }


        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.ViewModel = new StudentChecklistReportVM();

        }
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.cblMonth.CheckBoxes = this.rtsTabs.SelectedIndex == (int)SearchTab.Basic;
            this.ddlSchool.Required = this.rtsTabs.SelectedIndex == (int)SearchTab.Advanced;
            this.Master.Search += SearchHandler;
            this.ddlStudentGrade.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(ddlStudentGrade_ItemsRequested);
            this.cblMonth.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cblMonth_ItemsRequested);
            this.ddlSchool.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(ddlSchool_ItemsRequested);
            this.ddlStundentName.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(ddlStundentName_ItemsRequested);
            this.ddlCounselor.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(ddlCounselor_ItemsRequested);
            this.ddlItemNumber.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(ddlItemNumber_ItemRequested);
            RegisterScript();

        }

        public void RegisterScript()
        {
            bool firstOne = true;
            string enumStr = "CriteriaController.RestrictValueOptions = {";
            foreach (int option in Enum.GetValues(typeof(CriteriaBase.RestrictValueOptions)))
            {
                if (!firstOne) enumStr += ",";
                enumStr += "\"" + Enum.GetName(typeof(CriteriaBase.RestrictValueOptions), option) + "\" : " + option;
                firstOne = false;
            }
            enumStr += "};";
            ScriptManager.RegisterStartupScript(this, typeof(string), "SearchEnums", enumStr, true);
        }

        protected void rtsTabs_Init(object sender, EventArgs e)
        {
            this.rtsTabs.DataSource = Enum.GetValues(typeof(SearchTab));
            this.rtsTabs.DataBind();
        }


        protected void ddlStudentGrade_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlStudentGrade.DataSource = this.ViewModel.GetAllGrades().Select(c => new { Text = string.Format("{0}th Grade", c.Text), Value = c.Text });
            this.ddlStudentGrade.DataBind();
        }

        protected void cblMonth_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.cblMonth.DataSource = this.ViewModel.GetAllMonthsByGrade(e.Context["Grade"].ToString());
            this.cblMonth.DataBind();
        }

        protected void ddlSchool_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            if (this.SessionObject != null && this.SessionObject.LoggedInUser != null)
            {
                var loggedInUser = this.SessionObject.LoggedInUser;
                if (loggedInUser.Roles != null && loggedInUser.Roles.Any())
                {
                    this.ddlSchool.DataSource = this.ViewModel.GetAllSchoolByGrade(e.Context["Grade"].ToString(), loggedInUser);

                    this.ddlSchool.DataBind();
                }
            }
        }

        protected void ddlStundentName_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            e.NumberOfItems = (e.NumberOfItems == 0 ? 1 : e.NumberOfItems) * 10;
            this.ddlStundentName.DataSource = this.ViewModel.GetAllStundentBySchool(e.Context["School"].ToString(), e.Text, e.NumberOfItems / 10);
            this.ddlStundentName.DataBind();
        }

        protected void ddlCounselor_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlCounselor.DataSource = this.ViewModel.GetAllCounselorBySchool(e.Context["School"].ToString()).Select(c => new
            {
                Name = string.Format("{0} {1}", c.CounselorFirstName, c.CounselorLastName),
                CounselorId = c.CounselorId
            });
            this.ddlCounselor.DataBind();
        }

        protected void ddlItemNumber_ItemRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlItemNumber.DataSource = this.ViewModel.GetItemNumberByGradeMonth(e.Context["Grade"].ToString(), e.Context["Months"].ToString()).Select(c => new
            {
                Text = string.Format("{0}.{1}.{2}", e.Context["Grade"].ToString(), e.Context["Months"].ToString(), c.SequenceNumber),
                Value = c.SequenceNumber
            });
            this.ddlItemNumber.DataBind();
        }

        protected void imgExport_Click(object sender, EventArgs e)
        {
            this.radTreeResults.Columns.Clear();
            RadTreeList_ColumnBind(int.Parse(string.IsNullOrWhiteSpace(this.hdnGrade.Value) ? "0" : this.hdnGrade.Value), this.hdnMonth.Value, this.hdnItemNumber.Value, false);
            this.radTreeResults.DataSource = this.TempData;
            this.radTreeResults.ExpandToLevel(3);

            foreach (var item in this.radTreeResults.Items)
                foreach (TableCell cells in item.Cells)
                    cells.Text = cells.Text.StripHtml();

            this.radTreeResults.ExportToExcel();
        }

        protected void SearchHandler(object sender, CriteriaController model)
        {
            this.dvEmpty.Visible = false;

            int _grade = 0;
            string _month = string.Empty;
            string _schoolId = string.Empty;
            string _student_id = string.Empty;
            int _counselorId = 0;
            string _itemNumber = string.Empty;


            int.TryParse(string.Join(",", model.ParseCriteria<Thinkgate.Controls.E3Criteria.RadDropDownList.ValueObject>(this.ddlStudentGrade.CriteriaName).Select(x => x.Value)), out _grade);
            this.hdnMonth.Value = _month = string.Join(",", model.ParseCriteria<Thinkgate.Controls.E3Criteria.RadDropDownList.ValueObject>(this.cblMonth.CriteriaName).Select(x => x.Value));
            _schoolId = string.Join(",", model.ParseCriteria<Thinkgate.Controls.E3Criteria.RadDropDownList.ValueObject>(this.ddlSchool.CriteriaName).Select(x => x.Value));
            _student_id = string.Join(",", model.ParseCriteria<Thinkgate.Controls.E3Criteria.RadDropDownList.ValueObject>(this.ddlStundentName.CriteriaName).Select(x => x.Value));
            int.TryParse(string.Join(",", model.ParseCriteria<Thinkgate.Controls.E3Criteria.RadDropDownList.ValueObject>(this.ddlCounselor.CriteriaName).Select(x => x.Value)), out _counselorId);
            this.hdnItemNumber.Value = _itemNumber = string.Join(",", model.ParseCriteria<Thinkgate.Controls.E3Criteria.RadDropDownList.ValueObject>(this.ddlItemNumber.CriteriaName).Select(x => x.Value));
            this.hdnGrade.Value = _grade.ToString();

            if (this.SessionObject != null && this.SessionObject.LoggedInUser != null)
            {
                var loggedInUser = this.SessionObject.LoggedInUser;
                if (loggedInUser.Roles != null && loggedInUser.Roles.Any())
                {
                    var data = this.ViewModel.GetStudentChecklistReport(_grade, _month, _schoolId, _student_id, _counselorId, _itemNumber, loggedInUser);

                    this.radTreeResults.Columns.Clear();

                    this.radTreeResults.DataSource = data;
                    this.TempData = data.ToList();

                    RadTreeList_ColumnBind(_grade, _month, _itemNumber);

                    this.radTreeResults.DataBind();

                    this.radTreeResults.ExpandToLevel(1);

                    this.imgExport.Visible = data.Any();
                    this.radTreeResults.Visible = true;
                }
                else
                {
                    this.radTreeResults.NoRecordsText = "You don't have permission to view this report.";
                }
            }


        }

        protected void radTreeResults_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            this.radTreeResults.Columns.Clear();
            RadTreeList_ColumnBind(int.Parse(string.IsNullOrWhiteSpace(this.hdnGrade.Value) ? "0" : this.hdnGrade.Value), this.hdnMonth.Value, this.hdnItemNumber.Value);
            this.radTreeResults.DataSource = this.TempData;
        }

        protected void radTreeResults_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {

            if (e.Item.ItemType == TreeListItemType.Item || e.Item.ItemType == TreeListItemType.AlternatingItem)
            {
                var column = (TreeListDataItem)e.Item;

                var dataItem = (StudentChecklistReportVM)((TreeListDataItem)e.Item).DataItem;

                switch (column.HierarchyIndex.NestedLevel)
                {

                    // commented as per Chengdian sugested on dated : September 17, 2014
                    //case 0:
                    //    e.Item.Cells[column.HierarchyIndex.NestedLevel + 1].Text = string.Format(@"<img src='{0}' alt='{1}' /> ", ResolveUrl("~/Images/TreeIcons/district.png"), dataItem.Name) + dataItem.Name;
                    //    break;
                    //case 1:
                    //    e.Item.Cells[column.HierarchyIndex.NestedLevel + 1].Text = string.Format(@"<img src='{0}' alt='{1}' /> ", ResolveUrl("~/Images/TreeIcons/school.png"), dataItem.Name) + dataItem.Name;
                    //    break;
                    //case 2:
                    //    e.Item.Cells[column.HierarchyIndex.NestedLevel + 1].Text = string.Format(@"<img src='{0}' alt='{1}' /> ", ResolveUrl("~/Images/TreeIcons/teacher.png"), dataItem.Name) + dataItem.Name;


                    case 2:
                        var _columnIndex = 8;
                        var itemDataNumber = (new List<object>()).Select(c => new
                        {
                            SequenceNumber = -1,
                            CheckboxStatus = false
                        });
                        if (!string.IsNullOrWhiteSpace(dataItem.ItemNumberData))
                            itemDataNumber = ((DataTable)JsonConvert.DeserializeObject(dataItem.ItemNumberData, (typeof(DataTable)))).AsEnumerable().Select(c => new
                           {
                               SequenceNumber = Convert.ToInt32(c["SequenceNumber"]),
                               CheckboxStatus = Convert.ToBoolean(c["CheckboxStatus"])
                           });
                        foreach (var item in this.hdnItemNumber.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(c => int.Parse(c)))
                        {
                            var data = itemDataNumber.FirstOrDefault(c => c.SequenceNumber == int.Parse(item));
                            e.Item.Cells[_columnIndex].Text = data != null && data.CheckboxStatus ? ItemStatus.Completed.Description() : ItemStatus.Incomplete.Description();
                            _columnIndex++;
                        }

                        break;
                }


                if (dataItem.StudentId.HasValue)
                    e.Item.Cells[e.Item.Cells.Count - 1].Text = string.Format("<img src='{0}' alt='{1}' width='16px' style='cursor:pointer' onclick='customDialog({{ url: \"{2}\", maximize: true, resizable: true, title: \"Advisement CheckList\", maxwidth: 930, maxheight: 675 }})'/>", ResolveUrl(@"~/Images/ChecklistIcon.png"), dataItem.Name, ResolveUrl(string.Format(@"~/Controls/Student/StudentChecklist.aspx?grade={0}&studentId={1}", dataItem.StudentGrade, dataItem.StudentId)));
            }
        }

        private void RadTreeList_ColumnBind(int grade, string month, string itemNumber, bool isChecklistVisible = true)
        {

            var cl = new TreeListBoundColumn();
            cl.UniqueName = cl.HeaderText = cl.DataField = "Name";
            this.radTreeResults.Columns.Add(cl);

            cl = new TreeListBoundColumn();
            cl.UniqueName = cl.DataField = "StudentCount";
            cl.HeaderText = "Students";
            cl.HeaderStyle.HorizontalAlign = cl.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.radTreeResults.Columns.Add(cl);

            cl = new TreeListBoundColumn();
            cl.UniqueName = cl.DataField = "StudentGrade";
            cl.HeaderText = "Student Grade";
            cl.HeaderStyle.HorizontalAlign = cl.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.radTreeResults.Columns.Add(cl);

            var months = this.ViewModel.GetAllMonthsByGrade(string.Format("{0:00}", grade));
            foreach (var item in month.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(c => ((MonthsOrder)Enum.Parse(typeof(MonthsOrder), c))).OrderBy
                                    (c => int.Parse(c.Description())))
            {
                var dbMonth = months.FirstOrDefault(c => c.SequenceMonth.HasValue && c.SequenceMonth.Value == (int)item);
                if (dbMonth != null)
                {
                    cl = new TreeListBoundColumn();
                    cl.UniqueName = cl.DataField = dbMonth.Month;
                    cl.HeaderText = string.Format("{0} % Complete", cl.UniqueName);
                    cl.DataFormatString = "{0:0}%";
                    cl.HeaderStyle.HorizontalAlign = cl.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    this.radTreeResults.Columns.Add(cl);
                }
            }

            cl = new TreeListBoundColumn();
            cl.UniqueName = cl.DataField = "CounselorName";
            cl.HeaderText = "Counselor";
            cl.HeaderStyle.HorizontalAlign = cl.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.radTreeResults.Columns.Add(cl);

            if (!string.IsNullOrWhiteSpace(itemNumber))
                foreach (var item in itemNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(c => int.Parse(c)))
                {
                    cl = new TreeListBoundColumn();
                    cl.UniqueName = cl.DataField = "ItemNumber";
                    cl.HeaderText = string.Format("Item {0} Status", item);
                    cl.HeaderStyle.HorizontalAlign = cl.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    this.radTreeResults.Columns.Add(cl);
                }

            if (isChecklistVisible)
            {
                cl = new TreeListBoundColumn();
                cl.UniqueName = cl.DataField = "StudentId";
                cl.HeaderText = "Checklist";
                cl.HeaderStyle.HorizontalAlign = cl.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                this.radTreeResults.Columns.Add(cl);
            }
        }
    }

}