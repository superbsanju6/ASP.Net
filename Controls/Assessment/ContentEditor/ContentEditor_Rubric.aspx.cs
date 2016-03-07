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
using System.Web.Script.Serialization;
using Thinkgate.Classes.Serializers;


namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_Rubric : BasePage
    {
        private Rubric _rubric;
        private CourseList _standardCourseList;
        private IEnumerable<Grade> _gradeList;
        private dtItemBank _itemBankTbl;
        private IEnumerable<Subject> _subjectList;
        private IEnumerable<Base.Classes.Course> _courseList;

        protected void Page_Load(object sender, EventArgs e)
        {
            var gradeList = new List<string>();
            var subjectList = new List<string>();

            SessionObject = (SessionObject)Session["SessionObject"];
            _itemBankTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");

            if (LoadRubric() == false)
                return;

            _standardCourseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);
            _gradeList = _standardCourseList.GetGradeList();
            _subjectList = _standardCourseList.FilterByGrade(_rubric.Grade).GetSubjectList();

            gradeList.Add(_rubric.Grade);
            subjectList.Add(_rubric.Subject);
            _courseList = _standardCourseList.FilterByGradesAndSubjects(gradeList, subjectList);

            BindPageControls();

            /* Preview consists of Directions section + Content */
            ContentEditor_Rubric_Preview_Directions.InnerHtml = _rubric.Directions;
            ContentEditor_Rubric_Preview_Contents.InnerHtml = _rubric.Content;
        }

		//public string AppClient()
		//{
		//    if (Request.Url.Authority == "localhost")
		//        return Thinkgate.Base.Classes.AppSettings.FullyQualifiedLocalhostAppPath;
		//    else
		//        return Thinkgate.Base.Classes.AppSettings.FullyQualifiedAppPath;
		//}

        protected bool LoadRubric()
        {
            string sRubricID = Request.QueryString["xID"];
            if (sRubricID == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
                return false;
            }
            else
            {
                int _iRubricID = GetDecryptedEntityId(X_ID);
                _rubric = Base.Classes.Rubric.GetRubricByID(_iRubricID);

                if (_rubric == null)
                    return false;

                //RubricTitle.Text = _rubric.Name ?? string.Empty;

                txtName.Text = _rubric.Name;
                txtKeywords.Text = _rubric.Keywords;
                gradeDropdown.Text = _rubric.Grade;
                subjectDropdown.Text = _rubric.Subject;
                courseDropdown.Text = _rubric.Course;
                lblAuthor.Text = _rubric.CreatedByName;
                lblDistrict.Text = _rubric.SourceName;
                txtSource.Text = _rubric.Source;
                txtCredit.Text = _rubric.Credit;
                lblExpirationDate.Text = _rubric.CopyRightExpiryDate == null ? "No Expiration" : _rubric.CopyRightExpiryDate + " (mm/dd/yyyy)";
                lblDateCreated.Text = _rubric.DateCreated;
                lblDateUpdated.Text = _rubric.DateUpdated;
                if (_rubric.Copyright == "Yes")
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

                RubricItem.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(_iRubricID.ToString());
                lblNamePreview.Text = _rubric.Name;
                lblTypePreview.Text = _rubric.TypeDesc;
                lblScoringPreview.Text = _rubric.Scoring;

                lblNameEdit.Text = _rubric.Name;

                lblTypeEdit.Text = _rubric.TypeDesc;
                lblScoringEdit.Text = _rubric.Scoring;

                if (_rubric.TypeDesc == "Analytical")
                {
                    IdentificationPanel_Scoring.Style["display"] = "none";
                    lblTypeEdit.Text += " " + _rubric.CriteriaCount.ToString() + " x " + _rubric.MaxPoints.ToString();

					PreviewPanel_Scoring.Style["display"] = "none";
					lblTypePreview.Text = lblTypeEdit.Text;
                }
                else
                {
                    IdentificationPanel_Scoring.Style["display"] = "inline";
					PreviewPanel_Scoring.Style["display"] = "inline";
                }

                txtPoints.Text = _rubric.MaxPoints.ToString();
                txtCriteria.Text = _rubric.CriteriaCount.ToString();

                lblCkEditorRubricDirections.Text = _rubric.Directions;
            	ContentEditor_Rubric_hdnRubricID.Value = _iRubricID.ToString();


                if (_rubric.Type == "A")
                {
                    trRubricScoring.Style.Add("display", "none");
                    trAnalyticalCriteriaPointsSelection.Style.Add("display", "");
                }

                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
                string renderItemsScript = "var RubricItemArray = []; RubricItemArray[0] = " + serializer.Serialize(_rubric.RubricItemsList) + ";";

                renderItemsScript += "renderData(RubricItemArray[0],'" + _rubric.Type + "');";

                ScriptManager.RegisterStartupScript(this, typeof(string), "RubricItemsScript", renderItemsScript, true);
                ScriptManager.RegisterStartupScript(this, typeof(string), "appClient", "var appClient = '" + AppClient() + "';", true);

                if (Request.QueryString["NewAndReturnType"] != null)
                {
                    string newAndReturnType = Request.QueryString["NewAndReturnType"].ToString();
                    if (newAndReturnType.Length > 0)
                    {
                        FinishAndReturn.Style.Add("display", "");
                        string sOnclick = "SendResultsToWindow();";
                        FinishAndReturn.Attributes.Add("onclick", sOnclick);
                    }
                }
                return true;
            }
        }

        private void BindPageControls()
        {

            BindItemBankCheckBoxes();
            LoadGradeDropdown();
            LoadSubjectDropDown();
            LoadCourseDropDown();
            LoadRubricType();
            LoadRubricScoring();
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
                renderItemsScript += "['" + ibR["TargetType"].ToString() + "','" + ibR["Target"].ToString() + "','" + tf + "','" + (ibR["MultiBanks"].ToString().ToLower() == "true" ? "true" : "") + "']";
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
            if (Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(drItemBank["TargetType"]) >= 6)
                return true;
            else
                return false;
        }

        private bool IsItemBankInMasterList(string itemBank)
        {

            if (_rubric.ItemBankList != null)
            {
                for (int i = 0; i < DataIntegrity.ConvertToInt(_rubric.ItemBankList.Count); i++)
                {
                    if (_rubric.ItemBankList[i].Label == itemBank)
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
                if (grade.DisplayText == _rubric.Grade)
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
                if (subject.DisplayText == _rubric.Subject)
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
                if (course.CourseName == _rubric.Course)
                    item.Selected = true;
                var gradeOrdinal = new Base.Classes.Course(course.CourseName);
                item.Attributes["courseOrdinal"] = gradeOrdinal.CourseName;

                courseDropdown.Items.Add(item);

            }
            RadComboBoxItem someitem = courseDropdown.FindItemByValue(_rubric.Course);
            if (someitem == null)
            {
                RadComboBoxItem newitem = new RadComboBoxItem();
                newitem.Text = _rubric.Course;
                newitem.Value = _rubric.Course;
                newitem.Selected = true;

                var gOrdinal = new Base.Classes.Course(_rubric.Course);
                newitem.Attributes["courseOrdinal"] = gOrdinal.CourseName;

                courseDropdown.Items.Add(newitem);

            }

        }

        private void LoadRubricType()
        {
            DataTable dtRubricTypes = Base.Classes.Rubric.RubricTypesSelect();
            cmbType.DataTextField = "Rubrictype";
            cmbType.DataValueField = "Rubricval";
            cmbType.DataSource = dtRubricTypes;
            cmbType.DataBind();
            RadComboBoxItem someitem = cmbType.FindItemByValue(_rubric.Type);
            if (someitem != null)
                someitem.Selected = true;

			if (Request.QueryString["Type"] != null && (Request.QueryString["Type"].ToString() == "Holistic" || Request.QueryString["Type"].ToString() == "Analytical")) cmbType.Enabled = false;
        }


        private void LoadRubricScoring()
        {
            DataTable dtRubricTypes = Base.Classes.Rubric.RubricScoreTypesSelect();
            cmbScoring.DataTextField = "RubricScoreText";
            cmbScoring.DataValueField = "RubricPoints";
            cmbScoring.DataSource = dtRubricTypes;
            cmbScoring.DataBind();
            RadComboBoxItem someitem = cmbScoring.FindItemByValue(_rubric.MaxPoints.ToString());
            if (someitem != null)
                someitem.Selected = true;
        }

		//private string appClient()
		//{
		//    if (Request.Url.Authority.Contains("localhost"))
		//        return Thinkgate.Base.Classes.AppSettings.FullyQualifiedLocalhostAppPath;
		//    else
		//        return Thinkgate.Base.Classes.AppSettings.FullyQualifiedAppPath;
		//}

    }
}