using System;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Reflection;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using Standpoint.Core.Utilities;
using System.Data;

namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_Addendum : BasePage
    {     
        public int _AddendumID;
        public Base.Classes.Addendum _selectedAddendum;
        private string _AddendumTitle;
        private CourseList _standardCourseList;
        private IEnumerable<Grade> _gradeList;
        private dtItemBank _itemBankTbl;

        private IEnumerable<Subject> _subjectList;
        private IEnumerable<Base.Classes.Course> _courseList;

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            _itemBankTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
            LoadItemImage();
            _standardCourseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);
            _gradeList = _standardCourseList.GetGradeList();
            _subjectList = _standardCourseList.FilterByGrade(_selectedAddendum.Addendum_Grade).GetSubjectList();

            var gradeList = new List<string>();
            var subjectList = new List<string>();
            gradeList.Add(_selectedAddendum.Addendum_Grade);
            subjectList.Add(_selectedAddendum.Addendum_Subject);

            _courseList = _standardCourseList.FilterByGradesAndSubjects(gradeList, subjectList);
            BindPageControls();
        }

        private void LoadItemImage()
        {

            string addendumID = Request.QueryString["xID"];
            if (addendumID == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {

                _AddendumID = GetDecryptedEntityId(X_ID);

                var key = "Image_" + _AddendumID;

                string itemReservation = "No";
                if (UserHasPermission(Base.Enums.Permission.Access_ItemReservation))
                {
                    itemReservation = "Yes";
                }

                _selectedAddendum = Base.Classes.Addendum.GetAddendumByIDandItemBank(_AddendumID, itemReservation, _itemBankTbl);
                if (_selectedAddendum == null)
                {
                    SessionObject.RedirectMessage = "Could not find the image.";
                    Response.Redirect("~/PortalSelection.aspx", true);
                }

                // Set the page title text.
                _AddendumTitle = _selectedAddendum.Addendum_Name ?? string.Empty;

                txtName.Text = _selectedAddendum.Addendum_Name;
                //txtDescription.Text = _selectedAddendum.Description;
                txtKeywords.Text = _selectedAddendum.Addendum_Keywords;
                gradeDropdown.Text = _selectedAddendum.Addendum_Grade;
                subjectDropdown.Text = _selectedAddendum.Addendum_Subject;
                courseDropdown.Text = _selectedAddendum.Addendum_Course;
                lblAuthor.Text = _selectedAddendum.Addendum_CreatedByName;
                lblDistrict.Text = _selectedAddendum.Addendum_SourceName;
                txtSource.Text = _selectedAddendum.Addendum_Source;
                txtCredit.Text = _selectedAddendum.Addendum_Credit;
                lblExpirationDate.Text = _selectedAddendum.Addendum_CopyRightExpiryDate == null ? "No Expiration" : _selectedAddendum.Addendum_CopyRightExpiryDate + "(mm/dd//yyyy)";
                lblDateCreated.Text = _selectedAddendum.Addendum_DateCreated;
                lblDateUpdated.Text = _selectedAddendum.Addendum_DateUpdated;
                if (_selectedAddendum.Addendum_Copyright == "Yes")
                {
                    rbCopyRightYes.Checked = true;
                }
                else
                {
                    rbCopyRightNo.Checked = true;
                    trCredit.Style.Add("display", "none");
                    trSource.Style.Add("display", "none");
                    trExpiryDate.Style.Add("display", "none");
                }

                DistrictParms parms = DistrictParms.LoadDistrictParms();
                if (!parms.AllowCopyRightEditOnContentEditor)
                {
                    rbCopyRightNo.Enabled = false;
                    rbCopyRightYes.Enabled = false;
                }
                AddendumItem.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(_AddendumID.ToString());
                lblNamePreview.Text = _selectedAddendum.Addendum_Name;
                lblTypePreview.Text = _selectedAddendum.Addendum_Type;
                lblGenrePreview.Text = _selectedAddendum.Addendum_Genre;

                lblNameEdit.Text = _selectedAddendum.Addendum_Name;
                lblTypeEdit.Text = _selectedAddendum.Addendum_Type;
                lblGenreEdit.Text = _selectedAddendum.Addendum_Genre;

                lblItemCount.Text = _selectedAddendum.Addendum_ItemCount.ToString();
                previewAddendum.InnerHtml = _selectedAddendum.Addendum_Text;
                CkEditorAddendumEdit.InnerText = _selectedAddendum.Addendum_Text;

                //When we have a new Addendum, show editor only, collapse other panes
                string isNew = Request.QueryString["isNew"] == "" ? "" : Request.QueryString["isNew"];
                if (isNew != "Yes")
                {
                    Addendum_Identification.Expanded = false;
                    Addendum_Image.Expanded = false;
                    Addendum_Edit.Expanded = true;
                }
                if (Request.QueryString["NewAndReturnType"] != null)
                {
                    string newAndReturnType = Request.QueryString["NewAndReturnType"].ToString();
                    string newAndReturnID = Request.QueryString["NewAndReturnID"].ToString();
                    if (newAndReturnType.Length > 0 && newAndReturnID.Length > 0)
                    {
                        newAndReturnID = Standpoint.Core.Classes.Encryption.DecryptString(newAndReturnID);
                        FinishAndReturn.Style.Add("display", "");
                        string sOnclick = "Addendum_Edit_save(); InsertAddendum(" + _selectedAddendum.ID + ",'" + _selectedAddendum.Addendum_Name.Replace("'", "\'") + "','" + _selectedAddendum.Addendum_Type + "','" + _selectedAddendum.Addendum_Genre + "');";
                        FinishAndReturn.Attributes.Add("onclick", sOnclick);
                    }
                }
                LoadAddendumTypes();
                Addendum_Edit.Expanded = true;
            }
        }

        public string appClient()
        {
            if (Request.Url.Authority == "localhost")
                return Thinkgate.Base.Classes.AppSettings.FullyQualifiedLocalhostAppPath;
            else
                return Thinkgate.Base.Classes.AppSettings.FullyQualifiedAppPath;
        }

        protected void LoadAddendumTypes()
        {
            List<String> addendumTypes = Thinkgate.Base.Classes.Addendum.AddendumTypes;
            cmbType.DataSource = addendumTypes;
            cmbType.DataBind();
            try
            {
                cmbType.SelectedValue = _selectedAddendum.Addendum_Type;
            }
            catch (Exception) { }
            LoadAddendumGenres(cmbType.SelectedItem.Text);

        }

        protected void LoadAddendumGenres(String addendumType)
        {
            List<String> addendumGenres = Thinkgate.Base.Classes.Addendum.AddendumGenresForType("passage");
            cmbGenre.Visible = true;
            cmbGenre.DataSource = addendumGenres;
            cmbGenre.DataBind();
            try
            {
                cmbGenre.SelectedValue = _selectedAddendum.Addendum_Genre;
            }
            catch (Exception) { }
            if (addendumType != "passage")
                divcmbGenre.Style.Add("display", "none");
        }

        private void BindPageControls()
        {

            BindItemBankCheckBoxes();
            LoadGradeDropdown();
            LoadSubjectDropDown();
            LoadCourseDropDown();
        }

        private void BindItemBankCheckBoxes()
        {
            //List<String> itemBanks = (from i in itemBankTbl.AsEnumerable() select i.Field<String>("Label")).Distinct().ToList();
            string renderItemsScript = "var itemBankList = [";

            string tbl = @"<table style=""width:100%;"">";
            int aCounter = 0;
            string tf = "";
            foreach (DataRow ibR in _itemBankTbl.Rows)
            {

                tbl += @"<tr>";
                tbl += @"<td class=""fieldLabel"" style=""width:20%; padding: 0px 10px 0px 10px; text-align:right;"">";
                tbl += @"<input type=""checkbox"" cbtype=""ItemBank"" ID=""ItemBank_" + ibR["TargetType"].ToString() + "_" + ibR["Target"].ToString() + @"""";
                tbl += @" class=""itemBankUpdate"" title=""Click to add this item to the " + ibR["Label"].ToString() + @" item bank""";

                tbl += @" TargetType=""" + ibR["TargetType"].ToString() + @"""";
                tbl += @" Target=""" + ibR["Target"].ToString() + @"""";
                tbl += @" multiBanks=""" + (ibR["MultiBanks"].ToString().ToLower() == "true" ? "true" : "") + @"""";
                if (IsItemBankInMasterList(ibR["Label"].ToString()))
                {
                    tbl += @" inBank=""true""";
                    tbl += @" checked=""true""";
                    tf = "true";
                }
                else
                {
                    tbl += @" inBank=""false""";
                    tf = "false";
                }
                if (IsItemBankLocked(ibR))
                {
                    tbl += @" disabled=""true""";
                }
                tbl += @" onclick=""updateItemBanks(this);""/>";
                tbl += @"</td>";
                tbl += @"<td style=""width:80%"">" + ibR["Label"].ToString() + @"</td>";
                tbl += @"</tr>";

                aCounter++;
                renderItemsScript += "['" + ibR["TargetType"].ToString() + "','" + ibR["Target"].ToString() + "','" + tf + "','" + (ibR["MultiBanks"].ToString().ToLower()=="true" ? "true" : "" ) + "']";
                if (aCounter != _itemBankTbl.Rows.Count)
                    renderItemsScript += ",";
            }
            renderItemsScript += "];";

            tbl += @"</table>";
            ItemBankCheckBoxes.InnerHtml = tbl;

            ScriptManager.RegisterStartupScript(this, typeof(string), "itemBankList", renderItemsScript, true);
        }

        private bool IsItemBankLocked(DataRow drItemBank)
        {
            if (drItemBank["TargetType"].ToString() == "2")
                return true;
            if ( Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(drItemBank["TargetType"]) >= 6)
                return true;
            else
                return false;
        }

        private bool IsItemBankInMasterList(string ItemBank)
        {

            if (_selectedAddendum.Addendum_ItemBankList != null)
            {
                for (int i = 0; i < DataIntegrity.ConvertToInt(_selectedAddendum.Addendum_ItemBankList.Count); i++)
                {
                    if (_selectedAddendum.Addendum_ItemBankList[i].Label == ItemBank)
                    {
                        return true;
                    }
                }
            }

            return false;

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

        private void LoadSubjectDropDown()
        {

            subjectDropdown.Items.Clear();

            foreach (Subject subject in _subjectList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = subject.DisplayText;
                item.Value = subject.DisplayText;
                if (subject.DisplayText == _selectedAddendum.Addendum_Subject)
                    item.Selected = true;
                var gradeOrdinal = new Subject(subject.DisplayText);
                item.Attributes["subjectOrdinal"] = gradeOrdinal.DisplayText;

                subjectDropdown.Items.Add(item);

            }

        }

        private void LoadCourseDropDown()
        {

            courseDropdown.Items.Clear();

            foreach (Base.Classes.Course course in _courseList)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = course.CourseName;
                item.Value = course.CourseName;
                if (course.CourseName == _selectedAddendum.Addendum_Course)
                    item.Selected = true;
                var gradeOrdinal = new Base.Classes.Course(course.CourseName);
                item.Attributes["courseOrdinal"] = gradeOrdinal.CourseName;

                courseDropdown.Items.Add(item);

            }
            RadComboBoxItem someitem = courseDropdown.FindItemByValue(_selectedAddendum.Addendum_Course);
            if (someitem == null)
            {
                RadComboBoxItem newitem = new RadComboBoxItem();
                newitem.Text = _selectedAddendum.Addendum_Course;
                newitem.Value = _selectedAddendum.Addendum_Course;
                newitem.Selected = true;

                var gOrdinal = new Base.Classes.Course(_selectedAddendum.Addendum_Course);
                newitem.Attributes["courseOrdinal"] = gOrdinal.CourseName;

                courseDropdown.Items.Add(newitem);

            }

        }

        protected void RadTab_OnTabClick(object sender, RadTabStripEventArgs e)
        {
            switch (e.Tab.Value)
            {
                case "edit":
                    executeMathjax.Visible = false;
                    break;
                case "preview":
                    executeMathjax.Visible = true;
                    break;
            }
        }

    }
}