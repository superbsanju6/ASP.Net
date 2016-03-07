using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Collections.Generic;
using System.Collections;
using Thinkgate.Base.DataAccess;
using Standpoint.Core.Data;
using Thinkgate.Base.Enums;
using WebsitesScreenshot.SupportClasses;

namespace Thinkgate.Controls.Items
{
    public partial class AddNewItem : BasePage
	{
		SessionObject sessionObject;
        public string TestCategory { get; set; }

        private ItemTypes itemType
        {
            get { return (ItemTypes)Enum.Parse(typeof(ItemTypes), ViewState["itemType"].ToString()); }
            set { ViewState["itemType"] = value.ToString(); }
        }

		protected enum ItemTypes
		{
			Item,
			Image,
			Rubric,
			RubricHolistic,
			RubricAnalytical,
			Addendum
		}

		#region Page events
        
		protected void Page_Load(object sender, EventArgs e)
		{
			sessionObject = (SessionObject)Session["SessionObject"];

			if (Request.QueryString["xID"] == null)
			{
				SessionObject.RedirectMessage = "No item type provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else
			{
                itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), (String)Request.QueryString["xID"], true);
                switch (itemType)
				{
                    case ItemTypes.Addendum:
                        inpItemType.Value = itemType.ToString();
                        break;
                    case ItemTypes.Image:
                        inpItemType.Value = itemType.ToString();
                        break;
                    case ItemTypes.Item:
                        inpItemType.Value = itemType.ToString();
                        break;
                    case ItemTypes.Rubric:
                        inpItemType.Value = itemType.ToString();
                        break;
					case ItemTypes.RubricAnalytical:
                        inpItemType.Value = itemType.ToString();
						this.Title = "Analytical Rubric Identification";
						break;

					case ItemTypes.RubricHolistic:
                        inpItemType.Value = itemType.ToString();
						this.Title = "Custom Holistic Rubric Identification";
						break;

					default:
                        this.Title = itemType.ToString() + " Identification";
						break;
				}

				if (!IsPostBack)
				{
					SetControlVisibility();
					LoadGrades();
					LoadCourses();
					LoadItemBanks();
					LoadQuestionTypes();
					LoadAddendumTypes();
					LoadPassThruParms();
				}

				ScriptManager.RegisterStartupScript(this, typeof(string), "appClient", "var appClient = '" + AppClient() + "';", true);

			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// We have to set the visibility of some controls since this dialog is used for adding different kinds of things.
		/// All we will do is make table rows visible or not.
		/// </summary>
		protected void SetControlVisibility()
		{
			bool gradeVis = false, subjectVis = false, courseVis = false, itemBanksVis = false, scoreTypesVis = false;
            bool questionTypeVis = false, addendumTypeVis = false, addendumGenreVis = false, nameVis = false;
            switch (itemType)
			{
				case ItemTypes.Item:
					gradeVis = subjectVis = itemBanksVis = scoreTypesVis = questionTypeVis = true;
					break;
				case ItemTypes.Image:
					itemBanksVis = nameVis = true;
					break;
				case ItemTypes.RubricAnalytical:
				case ItemTypes.RubricHolistic:
				case ItemTypes.Rubric:
					gradeVis = subjectVis = courseVis = itemBanksVis = nameVis = true;
					break;
				case ItemTypes.Addendum:
					gradeVis = subjectVis = courseVis = itemBanksVis = addendumTypeVis = addendumGenreVis = nameVis = true;
					break;
			}			
			trGrade.Visible = gradeVis;
			trSubject.Visible = subjectVis;
			trCourse.Visible = courseVis;
			trItemBanks.Visible = itemBanksVis;
			trScoreType.Visible = scoreTypesVis;
			trQuestionType.Visible = questionTypeVis;
			trAddendumType.Visible = addendumTypeVis;
			trAddendumGenre.Visible = addendumGenreVis;
			trName.Visible = nameVis;

			SetContinueBtnState();
		}

		protected void SetContinueBtnState()
		{
			bool isContinueEnabled = false;
            switch (itemType)
            {
                case ItemTypes.Addendum:
                    isContinueEnabled = ((trGrade.Visible && cmbGrade.SelectedIndex >= 0) || !trGrade.Visible) &&
                                        ((trSubject.Visible && cmbSubject.SelectedIndex >= 0) || !trSubject.Visible) &&
                                        ((trCourse.Visible && cmbCourse.SelectedIndex >= 0) || !trCourse.Visible) &&
                                        ((trItemBanks.Visible && cmbItemBanks.CheckedItems.Count > 0) || !trItemBanks.Visible) &&
                                        ((trAddendumType.Visible && cmbAddendumType.SelectedIndex >= 0) || !trAddendumType.Visible) &&
                                        ((trAddendumGenre.Visible && cmbAddendumGenre.SelectedIndex >= 0) || !trAddendumGenre.Visible) &&
                                        ((trName.Visible && !String.IsNullOrEmpty(tbxName.Text)) || !trName.Visible);
                    break;
                case ItemTypes.Image:
                    isContinueEnabled = ((trItemBanks.Visible && cmbItemBanks.CheckedItems.Count > 0) || !trItemBanks.Visible) &&
                                        ((trName.Visible && !String.IsNullOrEmpty(tbxName.Text)) || !trName.Visible);
                    break;
                case ItemTypes.Item:
                    isContinueEnabled = ((trGrade.Visible && cmbGrade.SelectedIndex >= 0) || !trGrade.Visible) &&
                                        ((trSubject.Visible && cmbSubject.SelectedIndex >= 0) || !trSubject.Visible) &&
                                        ((trItemBanks.Visible && cmbItemBanks.CheckedItems.Count > 0) || !trItemBanks.Visible) &&
                                        ((trScoreType.Visible && cmbScoreType.SelectedIndex >= 0) || !trScoreType.Visible) &&
                                        ((trQuestionType.Visible && cmbQuestionType.SelectedIndex >= 0) || !trQuestionType.Visible);
                    break;
                case ItemTypes.Rubric:
                case ItemTypes.RubricAnalytical:
                case ItemTypes.RubricHolistic:
                    isContinueEnabled = ((trGrade.Visible && cmbGrade.SelectedIndex >= 0) || !trGrade.Visible) &&
                                        ((trSubject.Visible && cmbSubject.SelectedIndex >= 0) || !trSubject.Visible) &&
                                        ((trCourse.Visible && cmbCourse.SelectedIndex >= 0) || !trCourse.Visible) &&
                                        ((trItemBanks.Visible && cmbItemBanks.CheckedItems.Count > 0) || !trItemBanks.Visible) &&
                                        ((trName.Visible && !String.IsNullOrEmpty(tbxName.Text)) || !trName.Visible);
                    break;
                default:
                    isContinueEnabled = false;
                    break;
            }
			btnContinue.Enabled = isContinueEnabled && (cbxYes.Checked || cbxNo.Checked);
		}


		protected void LoadGrades()
		{
			if (cmbGrade.Visible)
			{
				CourseList courses = CoursesForUser;

				List<String> grades = (from g in courses.GetGradeList() select g.DisplayText).ToList();
				grades.Sort();
				cmbGrade.DataSource = grades;
				cmbGrade.DataBind();
				if(grades.Count == 1)
				{
					cmbGrade.SelectedIndex = 0;
					LoadSubjects(grades[0]);
				}
			}
		}

		protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			LoadSubjects(e.Text);
		}

		protected void LoadSubjects(String grade)
		{
			if (cmbSubject.Visible)
			{
				CourseList allCourses = CoursesForUser;

				CourseList subjCourses = allCourses.FilterByGrade(grade);
				List<String> subjects = (from s in subjCourses.GetSubjectList() select s.DisplayText).Distinct().ToList();
				subjects.Sort();
				cmbSubject.DataSource = subjects;
				cmbSubject.DataBind();
				SetContinueBtnState();
				if (subjects.Count == 1)
				{
					cmbSubject.SelectedIndex = 0;
					LoadCourses();
				}
			}
		}

		protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			LoadCourses();
			SetContinueBtnState();
		}

		protected void LoadCourses()
		{
			if (cmbCourse.Visible && cmbGrade.SelectedIndex >= 0 && cmbSubject.SelectedIndex >= 0)
			{
				CourseList courses = CoursesForUser.FilterByGradeAndSubject(cmbGrade.SelectedItem.Text, cmbSubject.SelectedItem.Text);
				List<String> courseNames = (from c in courses.AsEnumerable<Thinkgate.Base.Classes.Course>() select c.CourseName).Distinct().ToList();
				courseNames.Sort();
				cmbCourse.DataSource = courseNames;
				cmbCourse.DataBind();
				cmbCourse.SelectedIndex = (courseNames.Count == 1) ? 0 : -1;
			}
		}

		protected void cmbCourse_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			SetContinueBtnState();
		}

		protected void LoadItemBanks()
		{
			if (cmbItemBanks.Visible)
			{
                try
                {
                dtItemBank itemBankTbl = ItemBanksForUser;
                var itemBanks = from i in itemBankTbl.AsEnumerable()
                                select new RadComboBoxItem
                                {
                                    Text = i.Field<String>("Label"),
                                    Value = i.Field<int>("TargetType").ToString() + ":" + i.Field<String>("Multibanks")
                                };
                cmbItemBanks.DataTextField = "Text";
                cmbItemBanks.DataValueField = "Value";
                cmbItemBanks.DataSource = itemBanks;
				cmbItemBanks.DataBind();
                
				
                    if (TestCategory != null && TestCategory == AssessmentCategories.District.ToString())
                    {
                        switch (itemType)
                        {
                            case ItemTypes.Addendum:
                                foreach (RadComboBoxItem item in itemBanks)
                                {
                                    if (item.Text == "District")
                                    {
                                        item.Checked = true;
                                        break;
                                    }
                                }
                                break;
                            case ItemTypes.RubricAnalytical:
                                 break;
                            case ItemTypes.RubricHolistic:
                                break;
                            default:
                                foreach (RadComboBoxItem item in itemBanks)
                                {
                                    if (item.Text == "Personal")
                                    {
                                        item.Checked = true;
                                        break;
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        foreach (RadComboBoxItem item in itemBanks)
                        {
                            if (item.Text == "Personal")
                            {
                                item.Checked = true;
                                break;
                            }
                        }
                    }
				}
				catch (Exception) { }
			}
		}

		protected void LoadQuestionTypes()
		{
			if (cmbQuestionType.Visible)
			{
				List<String> questionTypeNames = Thinkgate.Base.Classes.BankQuestion.QuestionTypeNames;
				cmbQuestionType.DataSource = questionTypeNames;
				cmbQuestionType.DataBind();
				try
				{
                    /// Kumar: MultipleChoice(4) is the correct default value
                    Int32 idx = questionTypeNames.FindIndex(t => String.Compare(t, "MultipleChoice(4)", true) == 0);
					cmbQuestionType.SelectedIndex = idx;
				}
				catch (Exception) { }
				LoadScoreTypes(cmbQuestionType.SelectedItem.Text);
			}
		}

		protected void cmbQuestionType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			LoadScoreTypes(e.Text);
			SetContinueBtnState();
		}

		protected void LoadScoreTypes(String questionTypeName)
		{
			if (cmbScoreType.Visible)
			{
				List<String> scoreTypeNames = Thinkgate.Base.Classes.BankQuestion.ScoreTypeNamesForQuestionTypeName(questionTypeName);
				cmbScoreType.DataSource = scoreTypeNames;
				cmbScoreType.DataBind();
				try
				{
					Int32 idx = scoreTypeNames.FindIndex(t => t.ToUpper().StartsWith("CORRECT"));
					cmbScoreType.SelectedIndex = idx;
				}
				catch (Exception) { }
			}
		}

		protected void cmbScoreType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			SetContinueBtnState();
		}

		protected void LoadAddendumTypes()
		{
			if(cmbAddendumType.Visible)
			{
				List<String> addendumTypes = Thinkgate.Base.Classes.Addendum.AddendumTypes;
				cmbAddendumType.DataSource = addendumTypes;
				cmbAddendumType.DataBind();
				try
				{
					Int32 idx = addendumTypes.FindIndex(t => String.Compare(t, "passage", true) == 0);
					cmbAddendumType.SelectedIndex = idx;
				}
				catch(Exception) {}
				LoadAddendumGenres(cmbAddendumType.SelectedItem.Text);
			}
		}

		protected void cmbAddendumType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			LoadAddendumGenres(e.Text);
			SetContinueBtnState();
		}

		protected void LoadAddendumGenres(String addendumType)
		{
			List<String> addendumGenres = Thinkgate.Base.Classes.Addendum.AddendumGenresForType(addendumType);
			if(addendumGenres.Count > 0)
			{
				trAddendumGenre.Visible = true;
				cmbAddendumGenre.DataSource = addendumGenres;
				cmbAddendumGenre.DataBind();
				try
				{
					Int32 idx = addendumGenres.FindIndex(t => String.Compare(t, "Not Specified", true) == 0);
					cmbAddendumGenre.SelectedIndex = idx;
				}
				catch (Exception) { }
			}
			else
				trAddendumGenre.Visible = false;
			SetContinueBtnState();
		}

		protected void cmbAddendumGenre_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			SetContinueBtnState();
		}

		protected void tbxName_TextChanged(object sender, EventArgs e)
		{
			SetContinueBtnState();
		}

		/// <summary>
		/// Create a new item and set its id in a hidden field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnContinue_Click(object sender, EventArgs e)
		{
			// Create the table valued item banks to send to the stored proc if needed.
			SQLTableValueInput outputItemBanksSQL = null;
			if(cmbItemBanks.Visible)
			{
				dtItemBank inputItemBanks = ItemBanksForUser;
				dtItemBank outputItemBanks = new dtItemBank();
				foreach(var v in cmbItemBanks.CheckedItems)
				{
					String bankName = v.Text;
					foreach(DataRow row in inputItemBanks.Rows)
					{
						if(row["Label"].ToString() == bankName)
							outputItemBanks.Add(row);
					}
				}
				outputItemBanksSQL = outputItemBanks.ToSql();
			}

			// We must also get item type and score type abbreviations.
			String questionTypeAbbrev = "", scoreTypeAbbrev = "";
			if(cmbQuestionType.Visible)
				questionTypeAbbrev = Thinkgate.Base.Classes.BankQuestion.AbbrevForQuestionTypeName(cmbQuestionType.Text);
			if(cmbScoreType.Visible)
				scoreTypeAbbrev = Thinkgate.Base.Classes.BankQuestion.AbbrevForScoreTypeName(cmbScoreType.Text);
			// Get a string for the copyright.
			String copyright = (cbxYes.Checked) ? "Yes" : "No";
		   	// The id of the new item created.
			DataRow drRow;
			Int32 id = 0;

            switch (itemType)
			{
				case ItemTypes.Image:
					drRow = ThinkgateDataAccess.FetchDataRow("E3_Image_Create", new object[] { tbxName.Text, outputItemBanksSQL, copyright });
					id = (Int32)drRow[0];
					break;

				case ItemTypes.Item:
                    drRow = ThinkgateDataAccess.FetchDataRow("E3_Question_Create", new object[] { cmbGrade.Text, cmbSubject.Text, outputItemBanksSQL, questionTypeAbbrev, scoreTypeAbbrev, copyright});
					id = (Int32)drRow[0];
					break;

				case ItemTypes.Addendum:
                    drRow = ThinkgateDataAccess.FetchDataRow("E3_Addendum_Create", new object[] { tbxName.Text, cmbGrade.Text, cmbSubject.Text, cmbCourse.Text, outputItemBanksSQL, copyright, cmbAddendumType.Text, cmbAddendumGenre.Text });
					id = (Int32)drRow[0];
					break;

				case ItemTypes.RubricHolistic:
                    drRow = ThinkgateDataAccess.FetchDataRow("E3_Rubric_Create", new object[] { tbxName.Text, cmbGrade.Text, cmbSubject.Text, cmbCourse.Text, outputItemBanksSQL, copyright, "B" });
					id = (Int32)drRow[0];
					break;

				case ItemTypes.RubricAnalytical:
                    drRow = ThinkgateDataAccess.FetchDataRow("E3_Rubric_Create", new object[] { tbxName.Text, cmbGrade.Text, cmbSubject.Text, cmbCourse.Text, outputItemBanksSQL, copyright, "A" });
					id = (Int32)drRow[0];
					break;
				
				case ItemTypes.Rubric:
                    drRow = ThinkgateDataAccess.FetchDataRow("E3_Rubric_Create", new object[] { tbxName.Text, cmbGrade.Text, cmbSubject.Text, cmbCourse.Text, outputItemBanksSQL, copyright, "B" });
					id = (Int32)drRow[0];
					break;
			}

		inpxID.Value = id.ToString();
		inpxIDEnc.Value = Standpoint.Core.Classes.Encryption.EncryptInt(id);
    }

	protected void LoadPassThruParms()
	{
			inpxNewAndReturnID.Value = Request.QueryString["NewAndReturnID"];
			inpxNewAndReturnType.Value = Request.QueryString["NewAndReturnType"];
	}
		#endregion

		#region Properties
		/// <summary>
		/// Get the current user id.
		/// </summary>
		protected Int32 UserID
		{
			get { return sessionObject.LoggedInUser.Page; }
		}


		/// <summary>
		/// Get all possible courses for the current user.
		/// </summary>
		protected Thinkgate.Base.Classes.CourseList CoursesForUser
		{
			get
			{
				String key = "AddNewItem-Courses-" + UserID.ToString();
				CourseList allCourses = (CourseList)Thinkgate.Base.Classes.Cache.Get(key);
				if (allCourses == null)
				{
                    allCourses = Thinkgate.Base.Classes.CourseMasterList.GetStandardCoursesForUser(sessionObject.LoggedInUser);

					Thinkgate.Base.Classes.Cache.Insert(key, allCourses, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
				}
				return allCourses;
			}
		}

		/// <summary>
		/// Return the possible item banks for this user.
		/// </summary>
		protected dtItemBank ItemBanksForUser
		{
			get
			{
                String key = "AddNewItem-ItemBanks-" + UserID.ToString();
                dtItemBank itemBanks = (dtItemBank)Thinkgate.Base.Classes.Cache.Get(key);

                TestCategory = Request.QueryString["TestCategory"];
                if (TestCategory != null && TestCategory == AssessmentCategories.District.ToString())
                {
                    switch (itemType)
                    {
                        case ItemTypes.Addendum:
                        case ItemTypes.RubricAnalytical:
                        case ItemTypes.RubricHolistic:
                            String keyPersonalExcluded = "AddNewItem-ItemBanksPersonalExcluded-" + UserID.ToString();
                            dtItemBank itemBanksPersonalExcluded = (dtItemBank)Thinkgate.Base.Classes.Cache.Get(keyPersonalExcluded);

                            if (itemBanksPersonalExcluded == null)
                            {
                                itemBanksPersonalExcluded = ItemBankMasterList.GetItemBanksForUser(sessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
                                for (var rowIndex = itemBanksPersonalExcluded.Rows.Count - 1; rowIndex >= 0; rowIndex--)
                                    {
                                        if (itemBanksPersonalExcluded.Rows[rowIndex]["Label"].ToString() == "Personal")
                                        { itemBanksPersonalExcluded.Rows[rowIndex].Delete(); }
                                    }
                                Thinkgate.Base.Classes.Cache.Insert(keyPersonalExcluded, itemBanksPersonalExcluded, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
                            }
                            return itemBanksPersonalExcluded;
                        default:
                            if (itemBanks == null)
                            {
                                itemBanks = ItemBankMasterList.GetItemBanksForUser(sessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
                                Thinkgate.Base.Classes.Cache.Insert(key, itemBanks, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
                            }
                            return itemBanks;
                    }
                }

                if (itemBanks == null)
                {
                    // This is where the item banks are pulled and added to cache                         
                    itemBanks = ItemBankMasterList.GetItemBanksForUser(sessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankAdd, "Search"); 
                    Thinkgate.Base.Classes.Cache.Insert(key, itemBanks, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
                }
                return itemBanks;
			}
		}

        protected void cmbItemBanks_ItemChecked(object sender, RadComboBoxItemEventArgs e)
        {
            //Get Selected items
            var checkedItems = cmbItemBanks.CheckedItems;

            //Lists to Categorize items
            RadComboBoxItem personalbnk = new RadComboBoxItem();
            RadComboBoxItem teacherBnk = new RadComboBoxItem();
            List<RadComboBoxItem> schoolBnk = new List<RadComboBoxItem>();
            List<RadComboBoxItem> districtbnk_multi = new List<RadComboBoxItem>();
            List<RadComboBoxItem> districtbnk_nonmulti = new List<RadComboBoxItem>();
            List<RadComboBoxItem> thirdpartybnk = new List<RadComboBoxItem>();

            //Variables to parse items
            String[] valType = null;
            ItemBankType enumVal = ItemBankType.Personal;
            bool multiBankEnabled = false;
            


            //Classify each Checked Item
            foreach (var v in checkedItems)
            {
                //Split Item bank type and Multibanks value
                valType = v.Value.ToString().Split(':');

                //Store Type and Multibank flag
                enumVal = (ItemBankType)Enum.Parse(typeof(ItemBankType), valType[0].ToString());
                multiBankEnabled = Boolean.Parse(valType[1].ToString());

                //Personal 
                if (enumVal == ItemBankType.Personal)
                    personalbnk = v;

                //Teacher
                else if (enumVal == ItemBankType.Teacher)
                    teacherBnk = v;

                //School
                else if (enumVal == ItemBankType.School)
                    schoolBnk.Add(v);

                //District with multi-bank enabled
                else if (enumVal == ItemBankType.District && multiBankEnabled)
                    districtbnk_multi.Add(v);

                 //District with multi-bank disabled
                else if (enumVal == ItemBankType.District && !multiBankEnabled)
                    districtbnk_nonmulti.Add(v);

                //else Third Party bank
                else thirdpartybnk.Add(v);
            }

            //Uncheck Personal, Teacher and School Banks if any District/Third Party bank enabled
            if (districtbnk_multi.Count > 0 || districtbnk_nonmulti.Count > 0 || thirdpartybnk.Count > 0)
            { 
                if (personalbnk.Checked)
                    personalbnk.Checked = false;


                if (teacherBnk.Checked)
                    teacherBnk.Checked = false;

                if (schoolBnk.Count > 0)
                    foreach (var ckbx in schoolBnk)
                    {
                        ckbx.Checked = false;
                    }
            }

            //Check Personal Bank if Teacher Bank or school bank is checked is selected
            personalbnk = cmbItemBanks.Items.FindItemByText("Personal");
            if ((teacherBnk.Checked || schoolBnk.Count > 0) && personalbnk != null)
            {
                personalbnk.Checked = true;
                personalbnk.Enabled = false;
            }

            if (!teacherBnk.Checked && schoolBnk.Count == 0 && personalbnk != null)
                personalbnk.Enabled = true;


            //If multi and non-multi items exist, keep multi
            if (districtbnk_multi.Count > 0 && (districtbnk_nonmulti.Count > 0))
            {
                foreach (RadComboBoxItem ckbx in districtbnk_nonmulti)
                    ckbx.Checked = false;
                foreach (RadComboBoxItem ckbx in thirdpartybnk)
                    ckbx.Checked = false;
            }
            //If only non-multi items exist, leave last one enabled
            else if (districtbnk_nonmulti.Count > 1 && districtbnk_multi.Count == 0)
            {
                foreach (RadComboBoxItem ckbx in districtbnk_nonmulti)
                    ckbx.Checked = false;
                foreach (RadComboBoxItem ckbx in thirdpartybnk)
                    ckbx.Checked = false;
                districtbnk_nonmulti.Last().Checked = true;
            }
            //Disable Third Party if multi items exist
            else if (districtbnk_multi.Count > 0)
                foreach (RadComboBoxItem ckbx in thirdpartybnk)
                    ckbx.Checked = false;

            //Disable multi and non-multi and leave ThirdParty
            else if (thirdpartybnk.Count > 1)
            {
                foreach (RadComboBoxItem ckbx in districtbnk_multi)
                    ckbx.Checked = false;
                foreach (RadComboBoxItem ckbx in districtbnk_nonmulti)
                    ckbx.Checked = false;
                foreach (RadComboBoxItem ckbx in thirdpartybnk)
                    ckbx.Checked = false;
                thirdpartybnk.Last().Checked = true;
            }

            SetContinueBtnState();
        }

		/// <summary>
		/// Item type to construct. May be one of "Item", "Image", "Rubric", "Addendum".
		/// </summary>
		#endregion


	}
}