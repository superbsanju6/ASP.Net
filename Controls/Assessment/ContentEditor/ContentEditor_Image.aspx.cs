using System;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using Standpoint.Core.Utilities;
using System.Data;
using CultureInfo = System.Globalization.CultureInfo;

namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_Image : RecordPage
    {
        private int _imageID;
        public ItemImage SelectedImage;
        private CourseList _standardCourseList;
        private IEnumerable<Grade> _gradeList;
        private IEnumerable<Subject> _subjectList;
        private IEnumerable<Base.Classes.Course> _courseList;
        private dtItemBank _itemBankTbl;

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadItemImage();

            if (hdnFlagDeleteConfirmation.Value != "")
            {
                RadToolBarItem itemToDisable = mainToolBar.FindItemByValue("DeleteItemImage");
                itemToDisable.Enabled = false;
            }

            _standardCourseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);
            _gradeList = _standardCourseList.GetGradeList();
            _subjectList = _standardCourseList.FilterByGrade(SelectedImage.Grade).GetSubjectList();

            var gradeList = new List<string>();
            var subjectList = new List<string>();
            gradeList.Add(SelectedImage.Grade);
            subjectList.Add(SelectedImage.Subject);

            _courseList = _standardCourseList.FilterByGradesAndSubjects(gradeList, subjectList);
            BindPageControls();

            ScriptManager.RegisterStartupScript(this, typeof(string), "appClient", "var gr_preset = '" + SelectedImage.Grade + "';var sb_preset = '" + SelectedImage.Subject + "';var cr_preset = '" + SelectedImage.Course + "';", true);
        }


        private void LoadItemImage()
        {
            string imageID = Request.QueryString["xID"];
            if (imageID == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _imageID = GetDecryptedEntityId(X_ID);
                SelectedImage = ItemImage.GetImageByID(_imageID);

                if (SelectedImage == null)
                {
                    SessionObject.RedirectMessage = "Could not find the image.";
                    Response.Redirect("~/PortalSelection.aspx", true);
                }               
                else
                {
                    if (SelectedImage.Copyright == "Yes")
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

                    if (!DistrictParms.AllowCopyRightEditOnContentEditor)
                    {
                        rbCopyRightNo.Enabled = false;
                        rbCopyRightYes.Enabled = false;
                    }

                    if (SelectedImage.District == 0)
                    {
                        hdnFlagDeleteConfirmation.Value = "DELETED";                        
                        return;
                    }

                    txtName.Text = SelectedImage.Name;
                    lblFileName.Text = SelectedImage.FileNameOrig;
                    txtDescription.Text = SelectedImage.Description;
                    txtKeywords.Text = SelectedImage.Keywords;

                    lblAuthor.Text = SelectedImage.CreatedByName;
                    lblDistrict.Text = SelectedImage.SourceName;
                    txtSource.Text = SelectedImage.Source;
                    txtCredit.Text = SelectedImage.Credit;
                    lblExpirationDate.Text = SelectedImage.CopyRightExpiryDate == null
                        ? "No Expiration"
                        : SelectedImage.CopyRightExpiryDate + "(mm/dd/yyyy)";
                    lblDateCreated.Text = SelectedImage.DateCreated;
                    lblDateUpdated.Text = SelectedImage.DateUpdated;                  

                    ImageItem.InnerHtml =
                        Standpoint.Core.Classes.Encryption.EncryptString(
                            _imageID.ToString(CultureInfo.CurrentCulture));

                    string sItemImageSrc = AppSettings.UploadFolderWebPath.Replace("/", "") + @"/"
                                           + SelectedImage.FileName;
                    previewImg.InnerHtml = @"<img src=""" + sItemImageSrc + @""" />";
                    ImageSrc.InnerHtml = sItemImageSrc;

                    uploadFrame.Attributes["src"] = "ContentEditor_ImageUpload.aspx?xID="
                                                    + Standpoint.Core.Classes.Encryption.EncryptInt(
                                                        _imageID);
                    lblName.Text = SelectedImage.Name;
                    lblDescription.Text = SelectedImage.Description;
                    lblSize.Text = "";
                }

                if (Request.QueryString["NewAndReturnType"] != null)
                {
                    string newAndReturnType = Request.QueryString["NewAndReturnType"].ToString(CultureInfo.CurrentCulture);
                    string newAndReturnID = Request.QueryString["NewAndReturnID"].ToString(CultureInfo.CurrentCulture);
                    if (newAndReturnType.Length > 0 && newAndReturnID.Length > 0)
                    {
                        newAndReturnID = Standpoint.Core.Classes.Encryption.DecryptString(newAndReturnID);
                        FinishAndReturn.Style.Add("display", "");
                    }
                }
            }
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
            _itemBankTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
            string renderItemsScript = "var itemBankList = [";

            string tbl = @"<table style=""width:100%;"">";
            int aCounter = 0;
            foreach (DataRow ibR in _itemBankTbl.Rows)
            {

                tbl += @"<tr>";
                tbl += @"<td class=""fieldLabel"" style=""width:20%; padding: 0px 10px 0px 10px; text-align:right;"">";
                tbl += @"<input type=""checkbox"" cbtype=""ItemBank"" ID=""ItemBank_" + ibR["TargetType"] + "_" + ibR["Target"] + @"""";
                tbl += @" class=""itemBankUpdate"" title=""Click to add this item to the " + ibR["Label"] + @" item bank""";

                tbl += @" TargetType=""" + ibR["TargetType"] + @"""";
                tbl += @" Target=""" + ibR["Target"] + @"""";
                tbl += @" multiBanks=""" + (ibR["MultiBanks"].ToString().ToLower() == "true" ? "true" : "") + @"""";

                string tf;
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
                tbl += @"<td style=""width:80%"">" + ibR["Label"] + @"</td>";
                tbl += @"</tr>";

                aCounter++;
                renderItemsScript += "['" + ibR["TargetType"] + "','" + ibR["Target"] + "','" + tf + "','" + (ibR["MultiBanks"].ToString().ToLower() == "true" ? "true" : "") + "']";
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
            {
                return true;
            }

            return (DataIntegrity.ConvertToInt(drItemBank["TargetType"]) >= 6);
        }

        private bool IsItemBankInMasterList(string itemBank)
        {

            if (SelectedImage.ItemBankList != null)
            {
                for (int i = 0; i < DataIntegrity.ConvertToInt(SelectedImage.ItemBankList.Count); i++)
                {
                    if (SelectedImage.ItemBankList[i].Label == itemBank)
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
                if (grade.ToString() == SelectedImage.Grade)
                    item.Selected = true;
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
                if (subject.DisplayText == SelectedImage.Subject)
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
                if (course.CourseName == SelectedImage.Course)
                    item.Selected = true;
                var gradeOrdinal = new Base.Classes.Course(course.CourseName);
                item.Attributes["courseOrdinal"] = gradeOrdinal.CourseName;

                courseDropdown.Items.Add(item);

            }
            RadComboBoxItem someitem = courseDropdown.FindItemByValue(SelectedImage.Course);
            if (someitem == null)
            {
                RadComboBoxItem newitem = new RadComboBoxItem();
                newitem.Text = SelectedImage.Course;
                newitem.Value = SelectedImage.Course;
                newitem.Selected = true;

                var gOrdinal = new Base.Classes.Course(SelectedImage.Course);
                newitem.Attributes["courseOrdinal"] = gOrdinal.CourseName;

                courseDropdown.Items.Add(newitem);

            }

        }


    }
}
