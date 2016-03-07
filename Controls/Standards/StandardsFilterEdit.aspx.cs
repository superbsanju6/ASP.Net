using System;
using Thinkgate.Classes;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Thinkgate.Controls.Standards
{
    public partial class StandardsFilterEdit : BasePage
    {
        private DataTable _standardsFilterDataTable;
        private DataTable _standardsSearchDataTable;
        protected string _filterName;
        private CourseList _standardCourseList;
        private DataTable _standardSets;
        private IEnumerable<Grade> _gradeList;
        private IEnumerable<Subject> _subjectList;
        protected Int32 _userID;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (!IsPostBack)
            {
                _userID = SessionObject.LoggedInUser.Page;
                _standardCourseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);
                _standardSets = Thinkgate.Base.Classes.Standards.GetStandardSets(SessionObject.GlobalInputs, _userID);
                _gradeList = _standardCourseList.GetGradeList();
                _subjectList = _standardCourseList.GetSubjectList();
            }

            _filterName = Request.QueryString["filterName"];

            _standardsFilterDataTable = new DataTable();
            foreach (DataColumn col in Thinkgate.Base.Classes.Standards.TableStructure.Columns)
            {
                DataColumn newCol = new DataColumn(col.ColumnName);
                _standardsFilterDataTable.Columns.Add(newCol);
            }

            _standardsSearchDataTable = new DataTable();
            foreach (DataColumn col in Thinkgate.Base.Classes.Standards.TableStructure.Columns)
            {
                DataColumn newCol = new DataColumn(col.ColumnName);
                _standardsSearchDataTable.Columns.Add(newCol);
            }

            LoadStandardsFilterDataTable();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(standardFilterName.Value))
            {
                standardFilterName.Value = _filterName;
            }
            else
            {
                _filterName = standardFilterName.Value;
            }

            standardFilterName.Attributes["style"] = "width: " + (String.IsNullOrEmpty(_filterName) ? 50 : _filterName.Length * 11).ToString() + "px;";

            LoadStandardsFilterTable();

            if (!IsPostBack)
            {
                LoadStandardsSetDropdown();
                LoadGradeDropdown();
                LoadSubjectDropdown();
                LoadStandardCourses();
                filterIDs.Value = SessionObject.LoggedInUser.StandardFilters.ContainsKey(_filterName) && SessionObject.LoggedInUser.StandardFilters[_filterName].Length > 0 
                    ? SessionObject.LoggedInUser.StandardFilters[_filterName] + "," : "";

                if(String.IsNullOrEmpty(_filterName))
                {
                    updateButton.Enabled = false;
                }
            }
        }

        private void LoadStandardsFilterDataTable()
        {
            if (Request.QueryString["filterName"] == null)
            {
                SessionObject.RedirectMessage = "No filter ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }



            List<int> filterList = new List<int>();
            _standardsFilterDataTable.Clear();
            if (SessionObject.LoggedInUser.StandardFilters.ContainsKey(_filterName) &&
                SessionObject.LoggedInUser.StandardFilters[_filterName].Length > 0)
            {
                if (IsPostBack)
                {
                    filterIDs.Value = SessionObject.LoggedInUser.StandardFilters[_filterName] + ",";
                    foreach (var strID in Regex.Replace(filterIDs.Value, ",$", "").Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Int32 id;
                        if (Int32.TryParse(strID.Trim(), out id)) filterList.Add(id);
                    }
                }
                else
                {
                    foreach (var strID in SessionObject.LoggedInUser.StandardFilters[_filterName].Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Int32 id;
                        if (Int32.TryParse(strID.Trim(), out id)) filterList.Add(id);
                    }
                }
            }

            if (filterList.Count > 0)
            {
                //filterList = filterList. .Distinct().ToList();
                _standardsFilterDataTable = Thinkgate.Base.Classes.Standards.GetStandardsByIDs(filterList);
            }

        }

        private void LoadStandardsSearchDataTable()
        {
            string standardSetList = null;
            if (!String.IsNullOrEmpty(standardsSetDropdown.SelectedValue) || standardsSetDropdown.SelectedValue.ToLower() == "all")
            {
                standardSetList = standardsSetDropdown.SelectedValue;
            }
            
            string gradeList = null;
            if (!String.IsNullOrEmpty(gradeDropdown.SelectedValue))
            {
                gradeList = gradeDropdown.SelectedValue;
            }

            string subjectList = null;
            if (!String.IsNullOrEmpty(subjectDropdown.SelectedValue))
            {
                subjectList = subjectDropdown.SelectedValue;
            }

            string courseList = null;
            if (!String.IsNullOrEmpty(courseDropdown.SelectedValue))
            {
                courseList = courseDropdown.SelectedValue;
            }

            string searchText = null;
            if (!string.IsNullOrEmpty(textInput.Text))
            {
                searchText = textInput.Text;
            }

            _standardsSearchDataTable.Clear();
            _standardsSearchDataTable = Base.Classes.Standards.GetStandardsListWithTextSearch(
                                                             gradeList,
                                                             standardSetList,
                                                             subjectList,
                                                             courseList,
                                                             searchText,
                                                             "exact"
                                                             );
        }

        private void LoadStandardsFilterTable()
        {
            standardsFilterTable.Rows.Clear();

            if (_standardsFilterDataTable != null && _standardsFilterDataTable.Rows.Count > 0)
            {
                tableContainerDiv.Attributes["style"] = "display:block;";
                updateButton.Enabled = true;
                defaultMessageDiv.Attributes["style"] = "display:none;";
            }
            else
            {
                tableContainerDiv.Attributes["style"] = "display:none;";
                defaultMessageDiv.Attributes["style"] = "display:block;";
                if(IsPostBack && filterIDs.Value.Length > 0) updateButton.Enabled = false;
            }

            foreach(DataRow row in _standardsFilterDataTable.Rows)
            {
                TableRow newRow = new TableRow();
                TableCell cell1 = new TableCell();
                TableCell cell2 = new TableCell();
                TableCell cell3 = new TableCell();
                TableCell cell4 = new TableCell();
                TableCell cell5 = new TableCell();
                TableCell cell6 = new TableCell();
                TableCell cell7 = new TableCell();

                HtmlInputCheckBox cell1Data = new HtmlInputCheckBox();
                cell1Data.ID = "standardsFilterCheckbox_" + row["StandardID"].ToString();
                cell1Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell1Data.Attributes["onclick"] = "removeFromFilter(this, this.checked);";
                cell1Data.Attributes["standardID"] = row["StandardID"].ToString();
                cell1.CssClass = "alignCellCenter";
                cell1.Width = 50;
                cell1.Controls.Add(cell1Data);

                HyperLink cell2Data = new HyperLink();
                cell2Data.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["StandardID"].ToString());
                cell2Data.Attributes["onclick"] = "window.open('" + cell2Data.ResolveClientUrl(cell2Data.NavigateUrl) + "'); return false;";
                cell2Data.Text = row["StandardName"].ToString();
                cell2.ToolTip = row["StandardName"].ToString();
                cell2.Style.Add("overflow", "hidden");
                cell2.Width = 100;
                cell2.Controls.Add(cell2Data);

                HtmlGenericControl cell3Data = new HtmlGenericControl("DIV");
                ImageButton closeButton = new ImageButton();
                closeButton.ImageUrl = "~/Images/X.png";
                string closeButtonImgURL = closeButton.ResolveClientUrl(closeButton.ImageUrl);

                cell3Data.InnerHtml = "<div class=\"divOverflowHidden\"><a href=\"javascript:void(0);\" onclick=\"displayFullDescription(this); return false;\" class=\"standardTextLink\" >"
                    + row["Desc"].ToString() + "</a></div><div class='fullText'><img src='" + closeButtonImgURL
                    + "' onclick='this.parentNode.style.display=&quot;none&quot;;' style='position:relative; float:right; cursor:pointer;' />" + row["Desc"].ToString() + "</div>";
                cell3.Controls.Add(cell3Data);
                cell3.Width = 250;

                cell4.Text = row["Grade"].ToString();
                cell4.Width = 50;

                cell5.Text = row["Subject"].ToString();
                cell5.Width = 100;

                cell6.Text = row["Course"].ToString();
                cell6.Width = 100;

                cell7.Text = row["Level"].ToString();
                if (_standardsFilterDataTable.Rows.Count > 13) cell7.Width = 91;
                else cell7.Width = 100;

                newRow.Cells.Add(cell1);
                newRow.Cells.Add(cell2);
                newRow.Cells.Add(cell3);
                newRow.Cells.Add(cell4);
                newRow.Cells.Add(cell5);
                newRow.Cells.Add(cell6);
                newRow.Cells.Add(cell7);

                standardsFilterTable.Rows.Add(newRow);
            }

            standardsFilterTable.DataBind();
        }

        private void LoadStandardsSetDropdown()
        {
            standardsSetDropdown.Items.Clear();
            RadComboBoxItem standardsSetAllItem = new RadComboBoxItem();
            standardsSetAllItem.Text = "All";
            standardsSetAllItem.Value = "";
            standardsSetAllItem.Selected = true;
            standardsSetDropdown.Items.Add(standardsSetAllItem);

            foreach (DataRow standardSet in _standardSets.Rows)
            {
                RadComboBoxItem standardSetItem = new RadComboBoxItem();
                standardSetItem.Text = standardSet["Standard_Set"].ToString();
                standardSetItem.Value = standardSet["Standard_Set"].ToString();

                standardsSetDropdown.Items.Add(standardSetItem);
            }
        }

        private void LoadGradeDropdown()
        {
            gradeDropdown.Items.Clear();

            foreach (Grade grade in _gradeList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = grade.ToString();
                item.Value = grade.ToString();

                var gradeOrdinal = new Grade(grade.ToString());
                item.Attributes["gradeOrdinal"] = gradeOrdinal.GetFriendlyName();

                gradeDropdown.Items.Add(item);
            }
        }

        private void LoadSubjectDropdown()
        {
            subjectDropdown.Items.Clear();

            foreach(Subject subject in _subjectList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = subject.DisplayText;
                item.Value = subject.DisplayText;

                subjectDropdown.Items.Add(item);
            }
        }

        private void LoadStandardCourses()
        {
            courseDropdown.Items.Clear();

            foreach(string course in _standardCourseList.GetCourseNames())
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = course;
                item.Value = course;

                courseDropdown.Items.Add(item);
            }
        }

        public void LoadStandardsSearchTable_Click(object sender, EventArgs e)
        {
            LoadStandardsSearchDataTable();
            standardsSearchTable.Rows.Clear();

            if(_standardsSearchDataTable.Rows.Count > 0)
            {
                standardsSearchTable.Visible = true;
                standardsSearchHeader.Visible = true;
            }
            else
            {
                standardsSearchTable.Visible = false;
                standardsSearchHeader.Visible = false;
            }

            List<int> filterList = new List<int>();
            if (SessionObject.LoggedInUser.StandardFilters.ContainsKey(_filterName) &&
                SessionObject.LoggedInUser.StandardFilters[_filterName].Length > 0)
            {
                foreach (var strID in SessionObject.LoggedInUser.StandardFilters[_filterName].Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Int32 id;
                    if (Int32.TryParse(strID.Trim(), out id)) filterList.Add(id);
                }
            }

            foreach (DataRow row in _standardsSearchDataTable.Rows)
            {
                if (!filterList.Contains(Convert.ToInt32(row["ID"])))
                {
                    TableRow newRow = new TableRow();
                    TableCell cell1 = new TableCell();
                    TableCell cell2 = new TableCell();
                    TableCell cell3 = new TableCell();
                    TableCell cell4 = new TableCell();
                    TableCell cell5 = new TableCell();
                    TableCell cell6 = new TableCell();
                    TableCell cell7 = new TableCell();

                    newRow.Attributes["standardID"] = row["ID"].ToString();

                    HtmlInputCheckBox cell1Data = new HtmlInputCheckBox();
                    cell1Data.ID = "standardsSearchCheckbox_" + row["ID"].ToString();
                    cell1Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    cell1Data.Attributes["onclick"] = "addToFilter(this); return false;";
                    cell1Data.Attributes["standardID"] = row["ID"].ToString();
                    cell1.CssClass = "alignCellCenter";
                    cell1.Width = 50;
                    cell1.Controls.Add(cell1Data);

                    HyperLink cell2Data = new HyperLink();
                    cell2Data.NavigateUrl = "~/Record/StandardsPage.aspx?xID="
                                            + Standpoint.Core.Classes.Encryption.EncryptString(
                                                row["ID"].ToString());
                    cell2Data.Attributes["onclick"] = "window.open('"
                                                      + cell2Data.ResolveClientUrl(
                                                          cell2Data.NavigateUrl)
                                                      + "'); return false;";
                    cell2Data.Text = row["StandardName"].ToString();
                    cell2.ToolTip = row["StandardName"].ToString();
                    cell2.Style.Add("overflow", "hidden");
                    cell2.Width = 100;
                    cell2.Controls.Add(cell2Data);

                    HtmlGenericControl cell3Data = new HtmlGenericControl("DIV");
                    ImageButton closeButton = new ImageButton();
                    closeButton.ImageUrl = "~/Images/X.png";
                    string closeButtonImgURL = closeButton.ResolveClientUrl(closeButton.ImageUrl);

                    cell3Data.InnerHtml =
                        "<div class=\"divOverflowHidden\"><a href=\"javascript:void(0);\" onclick=\"displayFullDescription(this); return false;\" class=\"standardTextLink\" >"
                        + row["Description"].ToString()
                        + "</a></div><div class='fullText'><img src='" + closeButtonImgURL
                        + "' onclick='this.parentNode.style.display=&quot;none&quot;;' style='position:relative; float:right; cursor:pointer;' />"
                        + row["Description"].ToString() + "</div>";
                    cell3.Controls.Add(cell3Data);
                    cell3.Width = 250;

                    cell4.Text = row["Grade"].ToString();
                    cell4.Width = 50;

                    cell5.Text = row["Subject"].ToString();
                    cell5.Width = 100;

                    cell6.Text = row["Course"].ToString();
                    cell6.Width = 100;

                    cell7.Text = row["Level"].ToString();
                    if (_standardsSearchDataTable.Rows.Count > 13)
                        cell7.Width = 91;
                    else
                        cell7.Width = 100;

                    newRow.Cells.Add(cell1);
                    newRow.Cells.Add(cell2);
                    newRow.Cells.Add(cell3);
                    newRow.Cells.Add(cell4);
                    newRow.Cells.Add(cell5);
                    newRow.Cells.Add(cell6);
                    newRow.Cells.Add(cell7);

                    standardsSearchTable.Rows.Add(newRow);
                }
            }
            standardsSearchTable.DataBind();
        }

        public void UpdateFilter_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(standardFilterName.Value)) return;

            string filterList = filterIDs.Value.LastIndexOf(",") == filterIDs.Value.Length-1 ? Regex.Replace(filterIDs.Value, ",$", "") : filterIDs.Value;
            Base.Classes.Standards.UpdateStandardFilter(SessionObject.LoggedInUser.Page, standardFilterName.Value, "@@filter=" + (String.IsNullOrEmpty(filterList) ? "" : ",") + filterList + "@@");

            SessionObject.LoggedInUser.StandardFilters.Remove(_filterName);
            SessionObject.LoggedInUser.StandardFilters.Add(standardFilterName.Value, filterList);

            filterIDs.Value = SessionObject.LoggedInUser.StandardFilters[_filterName].Length > 0 ? SessionObject.LoggedInUser.StandardFilters[_filterName] + "," : "";

            LoadStandardsFilterDataTable();
            LoadStandardsFilterTable();
            standardsSearchTable.Visible = false;
            standardsSearchHeader.Visible = false;

            string js = "if(parent.$find('standardFilterRefreshTrigger')) parent.$find('standardFilterRefreshTrigger').click();";
            System.Web.UI.ScriptManager.RegisterStartupScript(Page, typeof(System.Web.UI.Page), "standardFilterRefresh", js, true);
        }
    }
}