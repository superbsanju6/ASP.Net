
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Core.Extensions;
using CultureInfo = System.Globalization.CultureInfo;
using Thinkgate.Services.Contracts.LearningMedia;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{

	public partial class Tags : Classes.Search.Criterion
	{
   
		#region Variables

        private string rootConnectionString = ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;
        private string cmsConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        private string customTableClassLRMI = "thinkgate.LRMI";
        private string customTableClassLRMIDetail = "thinkgate.LRMIEducationalAlignment";
		public SessionObject SessionObject = null;

        public class ValueObject
        {
            public string ID { get; set; }
            public string Value { get; set; }
        }

		public int ObjectId { get; set; }
		public string EnumSelectionType { get; set; }
		private bool _assessedVisible = true;
		private bool _requiresVisible = true;
		private bool _teachesVisible = true;
        private bool _saveButtonVisible = true;
        private bool _assessedDisabled;
        private bool _requiresDisabled;
        private bool _teachesDisabled;
        private bool _educationalSubjectDisabled;
        private bool _gradeDisabled;
        private bool _learningResourceDisabled;
        private bool _educationalUseDisabled;
        private bool _endUserDisabled;
        private bool _mediaTypeDisabled;
        private bool _interactivityDisabled;
	    

        public bool AssessedVisible { get { return _assessedVisible; } set { _assessedVisible = value; } }
		public bool RequiresVisible { get { return _requiresVisible; } set { _requiresVisible = value; } }
		public bool TeachesVisible { get { return _teachesVisible; } set { _teachesVisible = value; } }
        public bool SaveButtonVisible { get { return _saveButtonVisible; } set { _saveButtonVisible = value; } }
        public bool AssessedDisabled { get { return _assessedDisabled; } set { _assessedDisabled = value; } }
        public bool RequiresDisabled { get { return _requiresDisabled; } set { _requiresDisabled = value; } }
        public bool TeachesDisabled { get { return _teachesDisabled; } set { _teachesDisabled = value; } }
        public bool EducationalSubjectDisabled { get { return _educationalSubjectDisabled; } set { _educationalSubjectDisabled = value; } }
        public bool GradeDisabled { get { return _gradeDisabled; } set { _gradeDisabled = value; } }
        public bool LearningResourceDisabled { get { return _learningResourceDisabled; } set { _learningResourceDisabled = value; } }
        public bool EducationalUseDisabled { get { return _educationalUseDisabled; } set { _educationalUseDisabled = value; } }
        public bool EndUserDisabled { get { return _endUserDisabled; } set { _endUserDisabled = value; } }
        public bool MediaTypeDisabled { get { return _mediaTypeDisabled; } set { _mediaTypeDisabled = value; } }
        public bool InteractivityDisabled { get { return _interactivityDisabled; } set { _interactivityDisabled = value; } }
       
		#endregion

		#region Event Methods
        public TagCriteriaSelectionParameters TagCriteriaSelectionParameters = new TagCriteriaSelectionParameters();

        private readonly string _rootConnectionString = ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;

	    public event EventHandler SaveCancelButtonClick;
		protected void Page_Load(object sender, EventArgs e)
		{
			if (SessionObject == null)
			{
				SessionObject = (SessionObject)Session["SessionObject"];
			}
			
			StandSetListDiv.SaveCancelButtonClick += new EventHandler(User_Clicked_Save_Button);

            ClientScriptManager clientScript = Page.ClientScript;
            StringBuilder javaScriptBuilder = new StringBuilder();
            javaScriptBuilder.Append(" $('#" + rtbDuration.ClientID + "').inputmask('integer', {allowMinus: false, allowPlus: false});");
            clientScript.RegisterStartupScript(GetType(), rtbDuration.ClientID,
                            javaScriptBuilder.ToString(), true);

			if (!IsPostBack)
			{
				BindLookups();              // Bind Lookup Values
				BindDropdowns();            // Bind Dropdowns

				btnAddAssessed.Attributes.Add("onclick", "openModalDialog('" + ClientID + "', '" + hdnAssessedIds.ClientID + "'); return false;");
				btnAddTeaches.Attributes.Add("onclick", "openModalDialog('" + ClientID + "', '" + hdnTeachesIds.ClientID + "'); return false;");
				btnAddRequires.Attributes.Add("onclick", "openModalDialog('" + ClientID + "', '" + hdnRequiresIds.ClientID + "'); return false;");

				for (int i = 0; i < 24; i++)
					DurationHours.Items.Add(i.ToString(CultureInfo.InvariantCulture));
				for (int i = 0; i < 60; i += 5)
					DurationMinutes.Items.Add(i.ToString(CultureInfo.InvariantCulture));

				lblAssessedText.Visible = _assessedVisible;
				btnAddAssessed.Visible = _assessedVisible && !_assessedDisabled; 
				lblRequiresText.Visible = _requiresVisible;
				btnAddRequires.Visible = _requiresVisible && !_requiresDisabled;
				lblTeachesText.Visible = _teachesVisible;
				btnAddTeaches.Visible = _teachesVisible && !_teachesDisabled;
			    RadButtonSave.Visible = _saveButtonVisible;

				DateCreatedtxt.Text = DateTime.Now.ToShortDateString();
                
			    if (TagCriteriaSelectionParameters.AssessedStandardIds.Count == 0)
			    {
			        AssessedIcon.Style.Add("display", "none");
                    if(_assessedVisible && _assessedDisabled) lblNoStandardsAssessed.Style.Clear();
                }
			    if (TagCriteriaSelectionParameters.TeachesStandardIds.Count == 0)
			    {
			        TeachesIcon.Style.Add("display", "none");
                    if (_teachesVisible && _teachesDisabled) lblNoStandardsTeaches.Style.Clear();
			    }
			    if (TagCriteriaSelectionParameters.RequiresStandardIds.Count == 0)
			    {
			        RequiresIcon.Style.Add("display", "none");
                    if (_requiresVisible && _requiresDisabled) lblNoStandardsRequires.Style.Clear();
			    }
                EducationalSubjectSelections.Text = "-- Select --";
                GradeSelections.Text = "-- Select --";

			    if (!TagCriteriaSelectionParameters.IsEmpty(TagCriteriaSelectionParameters))
			    {
			        ExtractCriteriaObject();
			    }
			}

            PopulateCheckBoxLists(false);
		    javaScriptBuilder.Clear();
            if (LearningResourceDisabled) javaScriptBuilder.Append("DisableCheckBox('" + chkListLearningResourceType.ID + "');");
            if (EducationalUseDisabled) javaScriptBuilder.Append("DisableCheckBox('" + chkEducationalUse.ID + "');");
            if (EndUserDisabled) javaScriptBuilder.Append("DisableCheckBox('" + chkListTargetAudience.ID + "');");
            if (MediaTypeDisabled) javaScriptBuilder.Append("DisableCheckBox('" + chkListInstructionTypes.ID + "');");
            if (InteractivityDisabled) javaScriptBuilder.Append("DisableCheckBox('" + chkListActivityType.ID + "');");

            if (javaScriptBuilder.Length > 0)
                Page.ClientScript.RegisterStartupScript(GetType(), "Thinkgate.Controls.E3Criteria.CriteriaControls.Tags2",
                           javaScriptBuilder.ToString(), true);
		}

		#endregion

		#region Private Methods

		#region Binding Methods

		private void BindLookups()
		{
			const string selectSql = "select LD.Enum, LD.Description, L.Name AS LookupType, LD.SelectionType AS SelectionType from LookupDetails LD inner join LookupType L on L.Enum=LD.LookupEnum";
			DataTable dataTable = GetDataTable(GetLocalDbConnectionString(_rootConnectionString), selectSql);

			if (dataTable.Rows.Count > 0)
			{
				// Bind Learning Resource Type : rcbLearningResourceType
				chkListLearningResourceType.DataSource = new DataView(dataTable, "LookupType='" + Enums.LrmiTags.LearningResourceType + "' AND SelectionType LIKE ('%" + EnumSelectionType + "%')", "Description", DataViewRowState.CurrentRows);
				chkListLearningResourceType.DataTextField = "Description";
				chkListLearningResourceType.DataValueField = "Enum";
				chkListLearningResourceType.DataBind();

				// Bind Educational Use: chkEducationalUse
				chkEducationalUse.DataSource = new DataView(dataTable, "LookupType='" + Enums.LrmiTags.EducationalUse + "' AND SelectionType LIKE ('%" + EnumSelectionType + "%')", "Description", DataViewRowState.CurrentRows);
				chkEducationalUse.DataTextField = "Description";
				chkEducationalUse.DataValueField = "Enum";
				chkEducationalUse.DataBind();

				// Bind Instruction: rcbInstructionType
				chkListInstructionTypes.DataSource = new DataView(dataTable, "LookupType='" + Enums.LrmiTags.MediaType + "' AND SelectionType LIKE ('%" + EnumSelectionType + "%')", "Description", DataViewRowState.CurrentRows);
				chkListInstructionTypes.DataTextField = "Description";
				chkListInstructionTypes.DataValueField = "Enum";
				chkListInstructionTypes.DataBind();

				// Bind Activity : rcbActivityType
				chkListActivityType.DataSource = new DataView(dataTable, "LookupType='" + Enums.LrmiTags.Activity + "' AND SelectionType LIKE ('%" + EnumSelectionType + "%')", "Description", DataViewRowState.CurrentRows);
				chkListActivityType.DataTextField = "Description";
				chkListActivityType.DataValueField = "Enum";
				chkListActivityType.DataBind();

				// Bind Target Audience : rcbTargetAudience
				chkListTargetAudience.DataSource = new DataView(dataTable, "LookupType='" + Enums.LrmiTags.EndUser + "' AND SelectionType LIKE ('%" + EnumSelectionType + "%')", "Description", DataViewRowState.CurrentRows);
				chkListTargetAudience.DataTextField = "Description";
				chkListTargetAudience.DataValueField = "Enum";
				chkListTargetAudience.DataBind();

                // Bind Target Age Appropriate 
                DataView ageAppropriateDataView = new DataView(dataTable, "LookupType='" + Enums.LrmiTags.AgeAppropriate + "'", "Enum", DataViewRowState.CurrentRows);
			    DataRow selectRow = ageAppropriateDataView.Table.NewRow();
			    selectRow["Description"] = "-- Select --";
			    selectRow["Enum"] = "0";
			    selectRow["LookupType"] = Enums.LrmiTags.AgeAppropriate;
                ageAppropriateDataView.Table.Rows.InsertAt(selectRow,0);

			    ddlAgeAppropriate.DataSource = ageAppropriateDataView;
                ddlAgeAppropriate.DataTextField = "Description";
                ddlAgeAppropriate.DataValueField = "Enum";
                ddlAgeAppropriate.DataBind();

               
			}
            const string creativeCommonSelect = "select * from CreativeCommon";
            DataTable creativeCommonDataTable = GetDataTable(GetLocalDbConnectionString(_rootConnectionString), creativeCommonSelect);
			if (creativeCommonDataTable.Rows.Count > 0)
			{
                // Bind Creative Common 
                DataRow selectRow = creativeCommonDataTable.NewRow();
                selectRow["SelectDescription"] = "-- Select --";
                selectRow["ID"] = "0";
                creativeCommonDataTable.Rows.InsertAt(selectRow, 0);

                ddlUsageRights.DataSource = creativeCommonDataTable;
                ddlUsageRights.DataTextField = "SelectDescription";
                ddlUsageRights.DataValueField = "ID";
                ddlUsageRights.DataBind();

                JavaScriptSerializer dataToSerializer = new JavaScriptSerializer();
                List<Dictionary<string, object>> rowsList = new List<Dictionary<string, object>>();

			    DataRow[] creativeRows = creativeCommonDataTable.Select("IconUrl NOT LIKE 'http://%'");
                creativeRows[0]["IconUrl"] = Request.ApplicationPath + creativeRows[0]["IconUrl"];

			    foreach (DataRow dataRow in creativeCommonDataTable.Rows)
			    {
			        Dictionary<string, object> row = new Dictionary<string, object>();
			        foreach (DataColumn dataColumn in creativeCommonDataTable.Columns)
			        {
			            row.Add(dataColumn.ColumnName.Trim(), dataRow[dataColumn]);   
			        }
                    rowsList.Add(row);
			    }

                StringBuilder javaScriptBuilder = new StringBuilder();
                javaScriptBuilder.Append("var jsonCreativeCommon = JSON.parse('" + dataToSerializer.Serialize(rowsList) + "');");
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "Thinkgate.Controls.E3Criteria.CriteriaControls.Tags",
                       javaScriptBuilder.ToString(), true);
			}
		}

		private void BindDropdowns()
		{
			LoadDropDownGrades();
			LoadDropDownSubjects(); 
            
            var languageType = Enum.GetValues(typeof(LanguageType))
                .Cast<LanguageType>()
                .Select(v => v.ToString())
                .ToList();
                ddlLanguage.Items.Add("-- Select --");
                foreach (string langt in languageType)
                {
                    ddlLanguage.Items.Add(langt);
                }
		}

		private void LoadDropDownGrades()
			{
			CourseList courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
			DataTable dtGrade = new DataTable();

			dtGrade.Columns.Add("GradeValue", typeof(String));
			dtGrade.Columns.Add("GradeText", typeof(String));

			if (courseList != null)
			{
				foreach (var grade in courseList.GetGradeList())
				{
					DataRow newGradeRow = dtGrade.NewRow();
					newGradeRow["GradeValue"] = grade.DisplayText;
					newGradeRow["GradeText"] = grade.DisplayText;
					dtGrade.Rows.Add(newGradeRow);
				}
			}

			// Data bind the combo box.
			GradeCheckboxList.DataTextField = "GradeText";
			GradeCheckboxList.DataValueField = "GradeValue";
			GradeCheckboxList.DataSource = dtGrade;
			GradeCheckboxList.DataBind();
			}

		private void LoadDropDownSubjects()
		{
			CourseList currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
			IEnumerable<Subject> subjectList = currCourseList.GetSubjectList().OrderBy(sub => sub.DisplayText);

			DataTable dtSubject = new DataTable();
			dtSubject.Columns.Add("SubjectValue");
			dtSubject.Columns.Add("SubjectText", typeof(String));

			foreach (var s in subjectList)
			{
				dtSubject.Rows.Add(s.DisplayText, s.DisplayText);
			}

			// Data bind the combo box.
			ddlEducationalSubject.DataTextField = "SubjectText";
			ddlEducationalSubject.DataValueField = "SubjectValue";
			ddlEducationalSubject.DataSource = dtSubject;
			ddlEducationalSubject.DataBind();
		}

	    /// <summary>
	    /// ExtractCriteriaObject - Extract data from Criteria Object and move values to screen
	    /// </summary>
	    private void ExtractCriteriaObject()
	    {
	        
            if (EducationalSubjectDisabled)
	        {
	            EducationalSubjectSelections.Attributes.Add("onclick", "");
                EducationalSubjectImage.Attributes.Add("onclick", "");
	        }
            if (GradeDisabled)
            {
                GradeSelections.Attributes.Add("onclick", "");
                GradeImage.Attributes.Add("onclick", "");
            }

	        DateCreatedtxt.Text = TagCriteriaSelectionParameters.DateCreated != null
	            ? Convert.ToDateTime(TagCriteriaSelectionParameters.DateCreated).ToShortDateString()
	            : DateTime.Now.ToShortDateString();

	        if (TagCriteriaSelectionParameters.TimeRequiredDays != null)
	            rtbDuration.Text = TagCriteriaSelectionParameters.TimeRequiredDays.ToString();
	        if (TagCriteriaSelectionParameters.TimeRequiredHours != null)
	            DurationHours.Text = TagCriteriaSelectionParameters.TimeRequiredHours.ToString();
	        if (TagCriteriaSelectionParameters.TimeRequiredMinutes != null)
	            DurationMinutes.Text = TagCriteriaSelectionParameters.TimeRequiredMinutes.ToString();
	        if (TagCriteriaSelectionParameters.AgeAppropriate != null)
	            ddlAgeAppropriate.Text = TagCriteriaSelectionParameters.AgeAppropriate.ToString();
	        if (!string.IsNullOrWhiteSpace(TagCriteriaSelectionParameters.Creator))
	            Creator.Text = TagCriteriaSelectionParameters.Creator;
	        if (!string.IsNullOrWhiteSpace(TagCriteriaSelectionParameters.Publisher))
	            Publisher.Text = TagCriteriaSelectionParameters.Publisher;
	        if (TagCriteriaSelectionParameters.Language != null)
	            ddlLanguage.SelectedValue =
	                 Enum.Parse(typeof (LanguageType), TagCriteriaSelectionParameters.Language.ToString()).ToString();
	        if (!string.IsNullOrWhiteSpace(TagCriteriaSelectionParameters.ReadingLevel))
	            ReadingLevel.Text = TagCriteriaSelectionParameters.ReadingLevel;
	        if (!string.IsNullOrWhiteSpace(TagCriteriaSelectionParameters.TextComplexity))
	            TextComplexity.Text = TagCriteriaSelectionParameters.TextComplexity;
	        if (!string.IsNullOrWhiteSpace(TagCriteriaSelectionParameters.OriginalThirdPartyUrl))
	            rtbOriginalThirdPartyURL.Text = TagCriteriaSelectionParameters.OriginalThirdPartyUrl;
            
	        if (TagCriteriaSelectionParameters.EducationalSubject.Count > 0)
	            if (TagCriteriaSelectionParameters.EducationalSubject.Count == 1)
	            {
	                EducationalSubjectSelections.Text =
	                    TagCriteriaSelectionParameters.EducationalSubject[0];
	            }
	            else
	            {
	                EducationalSubjectSelections.Text =
	                    TagCriteriaSelectionParameters.EducationalSubject.Count.ToString(CultureInfo.InvariantCulture) +
	                    " items selected.";
	            }
	        else EducationalSubjectSelections.Text = "-- Select --";
	        if (TagCriteriaSelectionParameters.GradeLevel.Count > 0)
	            if (TagCriteriaSelectionParameters.GradeLevel.Count == 1)
	            {
	                GradeSelections.Text =
	                    TagCriteriaSelectionParameters.GradeLevel[0];
	            }
	            else
	            {
	                GradeSelections.Text =
	                    TagCriteriaSelectionParameters.GradeLevel.Count.ToString(CultureInfo.InvariantCulture) +
	                    " items selected.";
	            }
	        else GradeSelections.Text = "--Select--";

            if (TagCriteriaSelectionParameters.AssessedStandardIds != null)
                foreach (var assessedStandardIds in TagCriteriaSelectionParameters.AssessedStandardIds)
                {
                   hdnAssessedIds.Value += assessedStandardIds + "|";
                }
            if (TagCriteriaSelectionParameters.TeachesStandardIds != null)
                foreach (var teachesStandardIds in TagCriteriaSelectionParameters.TeachesStandardIds)
                {
                    hdnTeachesIds.Value += teachesStandardIds + "|";
                }
            if (TagCriteriaSelectionParameters.RequiresStandardIds != null)
                foreach (var requiresStandardIds in TagCriteriaSelectionParameters.RequiresStandardIds)
                {
                    hdnRequiresIds.Value += requiresStandardIds + "|";
                }

	        if (TagCriteriaSelectionParameters.EducationalSubject != null)
                foreach (var educationalSubject in TagCriteriaSelectionParameters.EducationalSubject)
                {
                    ListItem listItem = ddlEducationalSubject.Items.FindByValue(educationalSubject);
                    if (listItem != null)
                        ddlEducationalSubject.Items.FindByValue(educationalSubject).Selected = true;
                    
                }
            if (TagCriteriaSelectionParameters.GradeLevel != null)
                foreach (var grade in TagCriteriaSelectionParameters.GradeLevel)
                {
                    ListItem listItem = GradeCheckboxList.Items.FindByValue(grade);
                    if (listItem != null)
                        GradeCheckboxList.Items.FindByValue(grade).Selected = true;
                  
                }

	        PopulateCheckBoxLists(true);

            if (TagCriteriaSelectionParameters.UseRightUrl != null)
            {              
                ddlUsageRights.Text = TagCriteriaSelectionParameters.UseRightUrl.ToString();
                string creativeCommonSelect = "select * from CreativeCommon where ID = " + TagCriteriaSelectionParameters.UseRightUrl;
                DataTable creativeCommonDataTable = GetDataTable(GetLocalDbConnectionString(_rootConnectionString), creativeCommonSelect);
                if (creativeCommonDataTable.Rows.Count > 0)
                {
                    DataRow[] creativeRows = creativeCommonDataTable.Select("IconUrl NOT LIKE 'http://%'");
                    if (creativeRows.Any()) creativeRows[0]["IconUrl"] = "~" + creativeRows[0]["IconUrl"];
                   
                        foreach (DataRow dataRow in creativeCommonDataTable.Rows)
                        {
                            if (!TagCriteriaSelectionParameters.UseRightUrlTxt.IsNotNullOrEmpty())
                            {
                                UsageRightUrl.NavigateUrl = dataRow["DescriptionUrl"].ToString();
                                UsageRightUrl.Text = dataRow["Description"].ToString();
                                UsageRightUrl.Target = "_blank";
                            }
                            else
                            {
                                UsageRightUrlTxt.Text = TagCriteriaSelectionParameters.UseRightUrlTxt;
                                UsageRightUrlTxt.Style.Add("display", "");
                            }
                            UsageRightsImage.ImageUrl = dataRow["IconUrl"].ToString();
                            UsageRightsImage.Visible = true;
                        }
                }
            }
           
            }
           
	    private void PopulateCheckBoxLists(bool repopulateAll)
	    {
            // So, why am I controlling the selected check boxes with a repopulate or disabled condition. The checkbox list when
            // disabled does not store it's state in the view_state. When the user selects a button that opens a new dialog
            // on top of the current user control the closing of the dialog will cause a post back. Since the view_state is 
            // not storing the disabled checkbox lists values the program must recheck the same boxes again.
            if (TagCriteriaSelectionParameters.LearningResourceType != null && (repopulateAll || LearningResourceDisabled))
                foreach (var learningResource in TagCriteriaSelectionParameters.LearningResourceType)
                {
                    if (chkListLearningResourceType.Items.FindByValue(learningResource.ToString(CultureInfo.InvariantCulture)) != null)
                    chkListLearningResourceType.Items.FindByValue(learningResource.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }
            if (TagCriteriaSelectionParameters.EducationalUse != null && (repopulateAll || EducationalUseDisabled))
                foreach (var educationalUse in TagCriteriaSelectionParameters.EducationalUse)
                {
                    chkEducationalUse.Items.FindByValue(educationalUse.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }
            if (TagCriteriaSelectionParameters.EndUser != null && (repopulateAll || EndUserDisabled))
                foreach (var endUser in TagCriteriaSelectionParameters.EndUser)
                {
                    chkListTargetAudience.Items.FindByValue(endUser.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }
            if (TagCriteriaSelectionParameters.MediaType != null && (repopulateAll || MediaTypeDisabled))
                foreach (var mediaType in TagCriteriaSelectionParameters.MediaType)
                {
                    chkListInstructionTypes.Items.FindByValue(mediaType.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }
            if (TagCriteriaSelectionParameters.InteractivityType != null && (repopulateAll || InteractivityDisabled))
                foreach (var interactivityType in TagCriteriaSelectionParameters.InteractivityType)
                {
                    chkListActivityType.Items.FindByValue(interactivityType.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }
	    }
		#endregion

		#region GetData Handler Methods

		private DataTable GetDataTable(string connectionString, string selectSql)
		{
			DataTable dataTable = new DataTable();

			if (selectSql != null)
			{
				string connectionStringToUse = connectionString ?? _rootConnectionString;

				SqlConnection sqlConnection = new SqlConnection(connectionStringToUse);
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter {SelectCommand = new SqlCommand(selectSql, sqlConnection)};

				try
				{
					sqlConnection.Open();
				}
				catch (SqlException ex)
				{
					Debug.WriteLine("SqlException: " + ex.Message);
					return dataTable;
				}

				try
				{
					sqlDataAdapter.Fill(dataTable);
				}
				finally
				{
					sqlConnection.Close();
				}
			}

			return dataTable;
		}

		private static string GetLocalDbConnectionString(string connString)
		{
			return connString;
		}

		#endregion

        #region Processing Methods
        /// <summary>
        /// RadButtonSave_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadButtonSave_Click(object sender, EventArgs e)
        {
            InitializeObject();

            TagCriteriaSelectionParameters.IsChanged = true;

            TagCriteriaSelectionParameters.TimeRequiredDays = string.IsNullOrWhiteSpace(rtbDuration.Text) ? 0 : Convert.ToInt32(rtbDuration.Text);
            TagCriteriaSelectionParameters.TimeRequiredHours = Convert.ToInt32(DurationHours.Text);
            TagCriteriaSelectionParameters.TimeRequiredMinutes = Convert.ToInt32(DurationMinutes.Text);
            TagCriteriaSelectionParameters.AgeAppropriate = Convert.ToInt32(ddlAgeAppropriate.Text);
            TagCriteriaSelectionParameters.Creator = Creator.Text;
            TagCriteriaSelectionParameters.DateCreated = Convert.ToDateTime(DateCreatedtxt.Text);
            TagCriteriaSelectionParameters.Publisher = Publisher.Text;
            TagCriteriaSelectionParameters.Language = ddlLanguage.SelectedValue != "-- Select --" ? (int) Enum.Parse(typeof(LanguageType), ddlLanguage.SelectedValue) : 0;
            TagCriteriaSelectionParameters.UseRightUrl = Convert.ToInt32(ddlUsageRights.Text);
            TagCriteriaSelectionParameters.UseRightUrlTxt = UsageRightUrlTxt.Text;

            TagCriteriaSelectionParameters.ReadingLevel = ReadingLevel.Text;
            TagCriteriaSelectionParameters.TextComplexity = TextComplexity.Text;
            TagCriteriaSelectionParameters.OriginalThirdPartyUrl = rtbOriginalThirdPartyURL.Text;

            foreach (ListItem educationalSubject in ddlEducationalSubject.Items)
            {
                if (educationalSubject.Selected)
                    TagCriteriaSelectionParameters.EducationalSubject.Add(educationalSubject.Value);
            }
            foreach (ListItem grade in GradeCheckboxList.Items)
            {
                if (grade.Selected)
                    TagCriteriaSelectionParameters.GradeLevel.Add(grade.Value);
            }
            foreach (ListItem learningResource in chkListLearningResourceType.Items)
            {
                if(learningResource.Selected)
                    TagCriteriaSelectionParameters.LearningResourceType.Add(Convert.ToInt32(learningResource.Value));
            }
            foreach (ListItem educationalUse in chkEducationalUse.Items)
            {
                if (educationalUse.Selected)
                    TagCriteriaSelectionParameters.EducationalUse.Add(Convert.ToInt32(educationalUse.Value));
            }
            foreach (ListItem endUser in chkListTargetAudience.Items)
            {
                if (endUser.Selected)
                    TagCriteriaSelectionParameters.EndUser.Add(Convert.ToInt32(endUser.Value));
            }
            foreach (ListItem mediaType in chkListInstructionTypes.Items)
            {
                if (mediaType.Selected)
                    TagCriteriaSelectionParameters.MediaType.Add(Convert.ToInt32(mediaType.Value));
            }
            foreach (ListItem interactivityType in chkListActivityType.Items)
            {
                if (interactivityType.Selected)
                    TagCriteriaSelectionParameters.InteractivityType.Add(Convert.ToInt32(interactivityType.Value));
            }

            UpdateStandardSetSelections();
            
            if (SaveCancelButtonClick != null) SaveCancelButtonClick(this, e);
        }
        /// <summary>
        /// RadButtonCancel_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadButtonCancel_Click(object sender, EventArgs e)
        {
           InitializeObject();

            if (SaveCancelButtonClick != null) SaveCancelButtonClick(this, e);
        }
        /// <summary>
        /// InitializeObject - Reset values in object.
        /// </summary>
	    private void InitializeObject()
	    {
            TagCriteriaSelectionParameters.IsChanged = false;

            TagCriteriaSelectionParameters.AssessedStandardIds = new List<int>();
            TagCriteriaSelectionParameters.RequiresStandardIds = new List<int>();
            TagCriteriaSelectionParameters.TeachesStandardIds = new List<int>();

            TagCriteriaSelectionParameters.GradeLevel = new List<string>();
            TagCriteriaSelectionParameters.EducationalSubject = new List<string>();

            if (!LearningResourceDisabled) TagCriteriaSelectionParameters.LearningResourceType = new List<int>();
            if (!EducationalSubjectDisabled) TagCriteriaSelectionParameters.EducationalUse = new List<int>();
            if (!EndUserDisabled) TagCriteriaSelectionParameters.EndUser = new List<int>();
            if (!MediaTypeDisabled) TagCriteriaSelectionParameters.MediaType = new List<int>();
            if (!InteractivityDisabled) TagCriteriaSelectionParameters.InteractivityType = new List<int>();

            TagCriteriaSelectionParameters.TimeRequiredDays = null;
            TagCriteriaSelectionParameters.TimeRequiredHours = null;
            TagCriteriaSelectionParameters.TimeRequiredMinutes = null;
            TagCriteriaSelectionParameters.AgeAppropriate = null;
            TagCriteriaSelectionParameters.Creator = string.Empty;
            TagCriteriaSelectionParameters.DateCreated = null;
            TagCriteriaSelectionParameters.Publisher = string.Empty;
            TagCriteriaSelectionParameters.Language = null;

            TagCriteriaSelectionParameters.ReadingLevel = string.Empty;
            TagCriteriaSelectionParameters.TextComplexity = string.Empty;
            TagCriteriaSelectionParameters.OriginalThirdPartyUrl = string.Empty;
	    }
        /// <summary>
        /// Update Standard Set Selections
        /// </summary>
	    private void UpdateStandardSetSelections()
        {
            TagCriteriaSelectionParameters.AssessedStandardIds.RemoveRange(0, TagCriteriaSelectionParameters.AssessedStandardIds.Count);
            TagCriteriaSelectionParameters.TeachesStandardIds.RemoveRange(0, TagCriteriaSelectionParameters.TeachesStandardIds.Count);
            TagCriteriaSelectionParameters.RequiresStandardIds.RemoveRange(0, TagCriteriaSelectionParameters.RequiresStandardIds.Count);

	        string[] idStrings = { };
            if (hdnAssessedIds.Value.Length > 0) idStrings = hdnAssessedIds.Value.Substring(0, hdnAssessedIds.Value.Length - 1).Split('|');
            foreach (string idString in idStrings)
            {
                TagCriteriaSelectionParameters.AssessedStandardIds.Add(Convert.ToInt32(idString));
            }
            Array.Resize(ref idStrings, 0);
            if (hdnTeachesIds.Value.Length > 0) idStrings = hdnTeachesIds.Value.Substring(0, hdnTeachesIds.Value.Length - 1).Split('|');
            foreach (string idString in idStrings)
            {
                TagCriteriaSelectionParameters.TeachesStandardIds.Add(Convert.ToInt32(idString));
            }
            Array.Resize(ref idStrings, 0);
            if (hdnRequiresIds.Value.Length > 0) idStrings = hdnRequiresIds.Value.Substring(0, hdnRequiresIds.Value.Length - 1).Split('|');
            foreach (string idString in idStrings)
            {
                TagCriteriaSelectionParameters.RequiresStandardIds.Add(Convert.ToInt32(idString));
            }

            TagCriteriaSelectionParameters.AssessedStandardIds = TagCriteriaSelectionParameters.AssessedStandardIds.Distinct().ToList();
            TagCriteriaSelectionParameters.TeachesStandardIds = TagCriteriaSelectionParameters.TeachesStandardIds.Distinct().ToList();
            TagCriteriaSelectionParameters.RequiresStandardIds = TagCriteriaSelectionParameters.RequiresStandardIds.Distinct().ToList();
	    }
        protected void BuildStandardSet_OnClick(object sender, EventArgs e)
        {
            ImageButton btnElement = (ImageButton)sender;
            UpdateStandardSetSelections();

            switch (btnElement.ID)
            {
                case "AssessedIcon":
                    StandSetListDiv.StandardIds = TagCriteriaSelectionParameters.AssessedStandardIds;
                    break;
                case "TeachesIcon":
                    StandSetListDiv.StandardIds = TagCriteriaSelectionParameters.TeachesStandardIds;
                    break;
                case "RequiresIcon":
                    StandSetListDiv.StandardIds = TagCriteriaSelectionParameters.RequiresStandardIds;
                    break;
            }
            StandSetListDiv.StandardAlignment = btnElement.ID;
            
            StandSetListDiv.BuildStandardSetList();
	    }
        /// <summary>
        /// Call back event for user selecting save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void User_Clicked_Save_Button(object sender, EventArgs e)
        {
            
            switch (StandSetListDiv.StandardAlignment)
            {
                case "AssessedIcon":
                    hdnAssessedIds.Value = "";
                    foreach (var assessedStandardIds in StandSetListDiv.StandardIds)
                    {
                        hdnAssessedIds.Value += assessedStandardIds + "|";
                    }
                    break;
                case "TeachesIcon":
                    hdnTeachesIds.Value = "";
                    foreach (var teachesStandardIds in StandSetListDiv.StandardIds)
                    {
                        hdnTeachesIds.Value += teachesStandardIds + "|";
                    }
                    break;
                case "RequiresIcon":
                    hdnRequiresIds.Value = "";
                    foreach (var requiresStandardIds in StandSetListDiv.StandardIds)
                    {
                        hdnRequiresIds.Value += requiresStandardIds + "|";
                    }
                    break;
            }
            if (hdnAssessedIds.Value != "") AssessedIcon.Style["display"] = "";
            else AssessedIcon.Style["display"] = "none";
            if (hdnTeachesIds.Value != "") TeachesIcon.Style["display"] = "";
            else TeachesIcon.Style["display"] = "none";
            if (hdnRequiresIds.Value != "") RequiresIcon.Style["display"] = "";
            else RequiresIcon.Style["display"] = "none";
        }
	    #endregion

        #endregion
    }

}