using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using Standpoint.Core.ExtensionMethods;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria.CriteriaControls;
using Thinkgate.Enums;
using Thinkgate.Utilities;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class ItemTags : Page
    {
        private readonly string _rootConnectionString = ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;
        public TagCriteriaSelectionParameters LrmiTags;
        private Int32 _itemId;
        SessionObject _sessionObject;
        private DataTable _dt;
        private string _sItemId;
        private int _iItemId;
        private BankQuestion _itemBq;
        private TestQuestion _itemTq;

        protected void Page_Load(object sender, EventArgs e)
        {
            _sessionObject = (SessionObject)Session["SessionObject"];
            ctlTags.EnumSelectionType = SelectionType.Items.ToString();
            ctlTags.SaveCancelButtonClick += ctlTags_SaveCancelButtonClick;
            ctlTags.EducationalSubjectDisabled = true;
            ctlTags.GradeDisabled = true;
            ctlTags.RequiresVisible = false;
            ctlTags.TeachesVisible = false;
            ctlTags.LearningResourceDisabled = true;
            ctlTags.EndUserDisabled = true;
            ctlTags.EducationalUseDisabled = true;
            ctlTags.MediaTypeDisabled = true;
            ctlTags.AssessedDisabled = true;
            if (Request.QueryString["xID"] == null)
            {
                _sessionObject.RedirectMessage = "No Item ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _itemId = Cryptography.GetDecryptedID(_sessionObject.LoggedInUser.CipherKey);
                var sQuestionType = "";
                if (_itemId == 0)
                {
                    _itemId = Convert.ToInt32(Request.QueryString["xID"]);
                    sQuestionType = "TestQuestion";
                }
                else
                    sQuestionType = "" + Request.QueryString["qType"];

                if (!IsPostBack)
                {
                    _dt = Base.Classes.Assessment.LoadLrmiTagsForItem(_itemId);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        LoadTagsForItem(_dt);
                    }
                    else
                    {
                         if (sQuestionType.Equals("TestQuestion"))
                        {
                            _itemTq = TestQuestion.GetTestQuestionByID(_itemId);
                            if (_itemTq.StandardID>0)
                            ctlTags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(_itemTq.StandardID);
                            ctlTags.TagCriteriaSelectionParameters.GradeLevel.Add(_itemTq.Grade);
                            ctlTags.TagCriteriaSelectionParameters.EducationalSubject.Add(_itemTq.Subject);
                            ctlTags.TagCriteriaSelectionParameters.Creator = _itemTq.CreatedByName;
                        }
                        else
                        {
                            _itemBq = BankQuestion.GetQuestionByID(_itemId);
                            if (_itemBq.StandardID > 0)
                            ctlTags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(_itemBq.StandardID);
                        ctlTags.TagCriteriaSelectionParameters.GradeLevel.Add(_itemBq.Grade);
                        ctlTags.TagCriteriaSelectionParameters.EducationalSubject.Add(_itemBq.Subject);
                            ctlTags.TagCriteriaSelectionParameters.Creator = _itemBq.CreatedByName;
                        }
                        var lrmiTag = GetLrmiTag(Enums.LrmiTags.LearningResourceType, "Assessment Item");
                        if (lrmiTag > 0) ctlTags.TagCriteriaSelectionParameters.LearningResourceType.Add(lrmiTag);
                        lrmiTag = GetLrmiTag(Enums.LrmiTags.EducationalUse, "Assessment");
                        if (lrmiTag > 0) ctlTags.TagCriteriaSelectionParameters.EducationalUse.Add(lrmiTag);
                        lrmiTag = GetLrmiTag(Enums.LrmiTags.EndUser, "Student");
                        if (lrmiTag > 0) ctlTags.TagCriteriaSelectionParameters.EndUser.Add(lrmiTag);
                        lrmiTag = GetLrmiTag(Enums.LrmiTags.MediaType, "Application/Software");
                        if (lrmiTag > 0) ctlTags.TagCriteriaSelectionParameters.MediaType.Add(lrmiTag);
                        if (ctlTags.TagCriteriaSelectionParameters.Creator.IsNullOrWhiteSpace()) 
                        ctlTags.TagCriteriaSelectionParameters.Creator = (_sessionObject.LoggedInUser).UserName;
                    }
                }
            }
        }
        /// <summary>
        /// GetLrmiTag variables
        /// </summary>
        /// <param name="lrmiTag"></param>
        /// <param name="description"></param>
        /// <returns>int</returns>
        private int GetLrmiTag(Enum lrmiTag, string description)
        {
            string selectSql = "select LD.Enum FROM LookupDetails LD INNER JOIN LookupType L on L.Enum=LD.LookupEnum " +
                                            "WHERE Name='" + lrmiTag + "' AND Description = '" + description + "'";
            DataTable lookupDetailDataTable = GetDataTable(GetLocalDbConnectionString(_rootConnectionString), selectSql);
            if (lookupDetailDataTable.Rows.Count > 0)
            {
                DataRow lookupDetailDataRow = lookupDetailDataTable.Rows[0];
                return (int)lookupDetailDataRow["Enum"];
            }
            return 0;
        }
        /// <summary>
        /// Save Cancel Button Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ctlTags_SaveCancelButtonClick(object sender, EventArgs e)
        {
            if (ctlTags.TagCriteriaSelectionParameters.IsChanged)
            {
                SaveTags(ctlTags.TagCriteriaSelectionParameters);
            }
            //const string scriptText = "var win = this;setTimeout(function () {win.close();}, 0);";
            const string scriptText = "var win = parent.$find('RadWindow1Url');setTimeout(function () {win.close();}, 0);";
            System.Web.UI.ScriptManager.RegisterStartupScript(Page, typeof(Page), "Close Window", scriptText, true);
        }
        /// <summary>
        /// Save the LRMI tag data
        /// </summary>
        /// <param name="tags"></param>
        private void SaveTags(TagCriteriaSelectionParameters tags)
        {
            // Set default tags
            var lrmiTag = GetLrmiTag(Enums.LrmiTags.LearningResourceType, "Assessment");
            if (lrmiTag > 0) tags.LearningResourceType.Add(lrmiTag);
            lrmiTag = GetLrmiTag(Enums.LrmiTags.EducationalUse, "Assessment");
            if (lrmiTag > 0) tags.EducationalUse.Add(lrmiTag);
            lrmiTag = GetLrmiTag(Enums.LrmiTags.EndUser, "Student");
            if (lrmiTag > 0) tags.EndUser.Add(lrmiTag);
            lrmiTag = GetLrmiTag(Enums.LrmiTags.MediaType, "Application/Software");
            if (lrmiTag > 0) tags.MediaType.Add(lrmiTag);

            // Process user selected tags
            Base.Classes.Assessment.ClearLrmiTagsForItem(_itemId);
            if (tags.TextComplexity != string.Empty)
            {
                SaveTag((int)Enums.LrmiTags.TextComplexity, tags.TextComplexity,
                    true);
            }
            if (tags.ReadingLevel != string.Empty)
            {
                SaveTag((int)Enums.LrmiTags.ReadingLevel, tags.ReadingLevel,
                    true);
            }
            if (tags.AgeAppropriate != null && tags.AgeAppropriate != 0)
            {
                SaveTag((int)Enums.LrmiTags.AgeAppropriate, tags.AgeAppropriate.ToString(), true);
            }
            if (tags.Creator != string.Empty)
            {
                SaveTag((int)Enums.LrmiTags.Creator, tags.Creator, true);
            }
            if (tags.Publisher != string.Empty)
            {
                SaveTag((int)Enums.LrmiTags.Publisher, tags.Publisher, true);
            }
            if (tags.UseRightUrlTxt != string.Empty)
            {
                SaveTag((int)Enums.LrmiTags.UsageRightsUrl, tags.UseRightUrlTxt, true);
            }
            if (tags.DateCreated != null)
            {
                SaveTag((int)Enums.LrmiTags.DateCreated, tags.DateCreated.ToString(), true);
            }
            if (tags.Language != null)
            {
                SaveTag((int)Enums.LrmiTags.Language, tags.Language.ToString(), true);
            }
            if (tags.UseRightUrl != null && tags.UseRightUrl != 0)
            {
                SaveTag((int)Enums.LrmiTags.UsageRights, tags.UseRightUrl.ToString(), true);
            }
            if (tags.OriginalThirdPartyUrl != string.Empty)
            {
                SaveTag((int)Enums.LrmiTags.Original3RdParty, tags.OriginalThirdPartyUrl, true);
            }
            if (tags.TimeRequiredDays != null && tags.TimeRequiredDays != 0 ||
                tags.TimeRequiredHours != null && tags.TimeRequiredHours != 0 ||
                tags.TimeRequiredMinutes != null && tags.TimeRequiredMinutes != 0)
            {
                SaveTag((int)Enums.LrmiTags.TimeRequired, tags.TimeRequiredDays + ":" +
                tags.TimeRequiredHours + ":" +
                tags.TimeRequiredMinutes, true);
                
            }
            if (tags.AssessedStandardIds != null && tags.AssessedStandardIds.Count > 0)
            {
                foreach (var assessedStandardId in tags.AssessedStandardIds)
                {
                    SaveTag((int)Enums.LrmiTags.Assessed, assessedStandardId.ToString(CultureInfo.InvariantCulture), false);
                }
            }
            if (tags.GradeLevel != null && tags.GradeLevel.Count > 0)
            {
                SaveTag((int)Enums.LrmiTags.EducationalLevel, tags.GradeLevel[0], false);
            }
            if (tags.EducationalSubject != null && tags.EducationalSubject.Count > 0)
            {
                SaveTag((int)Enums.LrmiTags.EducationalSubject, tags.EducationalSubject[0], false);
            }
            if (tags.LearningResourceType != null && tags.LearningResourceType.Count > 0)
            {
                foreach (var resourceType in tags.LearningResourceType)
                {
                    SaveTag((int)Enums.LrmiTags.LearningResourceType, resourceType.ToString(CultureInfo.InvariantCulture), true);
                }
            }
            if (tags.EducationalUse != null && tags.EducationalUse.Count > 0)
            {
                foreach (var educationUse in tags.EducationalUse)
                {
                    SaveTag((int)Enums.LrmiTags.EducationalUse, educationUse.ToString(CultureInfo.InvariantCulture), true);
                }
            }
            if (tags.EndUser != null && tags.EndUser.Count > 0)
            {
                foreach (var endUser in tags.EndUser)
                {
                    SaveTag((int)Enums.LrmiTags.EndUser, endUser.ToString(CultureInfo.InvariantCulture), true);
                }
            }
            if (tags.MediaType != null && tags.MediaType.Count > 0)
            {
                foreach (var mediaType in tags.MediaType)
                {
                    SaveTag((int)Enums.LrmiTags.MediaType, mediaType.ToString(CultureInfo.InvariantCulture), true);
                }
            }
            if (tags.InteractivityType != null && tags.InteractivityType.Count > 0)
            {
                foreach (var activityType in tags.InteractivityType)
                {
                    SaveTag((int)Enums.LrmiTags.Activity, activityType.ToString(CultureInfo.InvariantCulture), true);
                }
            }
        }
        /// <summary>
        /// Save Tage middleware call method
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="value"></param>
        /// <param name="isEditable"></param>
        private void SaveTag(int tagId, string value, bool isEditable)
        {
            Base.Classes.Assessment.SaveLrmiTagForItem(_itemId, tagId, value, isEditable);
        }
        /// <summary>
        /// Load Standards for an Item
        /// </summary>
        private void LoadStandardsForItem()
        {
            DataTable dt = Base.Classes.Assessment.GetStandardsForItemId(_itemId);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                ctlTags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(int.Parse(dr["ID"].ToString()));
                if (dr["Subject"].ToString() != string.Empty && ctlTags.TagCriteriaSelectionParameters.EducationalSubject.Count == 0)
                {
                    ctlTags.TagCriteriaSelectionParameters.EducationalSubject.Add(dr["Subject"].ToString());
                }
                if (dr["Grade"].ToString() != string.Empty && ctlTags.TagCriteriaSelectionParameters.GradeLevel.Count == 0)
                {
                    ctlTags.TagCriteriaSelectionParameters.GradeLevel.Add(dr["Grade"].ToString());
                }
            }
        }
        /// <summary>
        /// Load tags for an individual item
        /// </summary>
        /// <param name="dt"></param>
        private void LoadTagsForItem(DataTable dt)
        {
            var sQuestionType = "";
            if (_itemId == 0)
            {
                _itemId = Convert.ToInt32(Request.QueryString["xID"]);
                sQuestionType = "TestQuestion";
            }
            else
                sQuestionType = "" + Request.QueryString["qType"];
            if (sQuestionType.Equals("TestQuestion"))
            {
                _itemTq = TestQuestion.GetTestQuestionByID(_itemId);
                if (_itemTq.StandardID > 0)
                ctlTags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(_itemTq.StandardID);
            }
            else
            {
                _itemBq = BankQuestion.GetQuestionByID(_itemId);
                if (_itemBq.StandardID > 0)
                ctlTags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(_itemTq.StandardID);
            }
            // load control with currently set tags.
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                int drTagId = (int)dr["TagId"];
                string drTagValue = dr["TagValue"].ToString();

                switch (drTagId)
                {
                   case (int)Enums.LrmiTags.MediaType:
                        {
                            ctlTags.TagCriteriaSelectionParameters.MediaType.Add(Convert.ToInt32(drTagValue));
                            break;
                        }
                    case (int)Enums.LrmiTags.Activity:
                        {
                            ctlTags.TagCriteriaSelectionParameters.InteractivityType.Add(Convert.ToInt32(drTagValue));
                            break;
                        }
                    case (int)Enums.LrmiTags.EndUser:
                        {
                            ctlTags.TagCriteriaSelectionParameters.EndUser.Add(Convert.ToInt32(drTagValue));
                            break;
                        }
                    case (int)Enums.LrmiTags.LearningResourceType:
                        {
                            ctlTags.TagCriteriaSelectionParameters.LearningResourceType.Add(Convert.ToInt32(drTagValue));
                            break;
                        }
                    case (int)Enums.LrmiTags.EducationalUse:
                        {
                            ctlTags.TagCriteriaSelectionParameters.EducationalUse.Add(Convert.ToInt32(drTagValue));
                            break;
                        }
                    case (int)Enums.LrmiTags.AgeAppropriate:
                        {
                            ctlTags.TagCriteriaSelectionParameters.AgeAppropriate = Convert.ToInt32(drTagValue);
                            break;
                        }
                    case (int)Enums.LrmiTags.TimeRequired:
                        {
                            int[] timeRequired = drTagValue.Split(':').Select(n => Convert.ToInt32(n)).ToArray();
                            ctlTags.TagCriteriaSelectionParameters.TimeRequiredDays = timeRequired[0];
                            ctlTags.TagCriteriaSelectionParameters.TimeRequiredHours = timeRequired[1];
                            ctlTags.TagCriteriaSelectionParameters.TimeRequiredMinutes = timeRequired[2];
                            break;
                        }
                    case (int)Enums.LrmiTags.Assessed:
                        ctlTags.TagCriteriaSelectionParameters.AssessedStandardIds.Add(int.Parse(drTagValue));
                        break;
                    case (int)Enums.LrmiTags.Teaches:
                        ctlTags.TagCriteriaSelectionParameters.TeachesStandardIds.Add(int.Parse(drTagValue));
                        break;
                    case (int)Enums.LrmiTags.Requires:
                        ctlTags.TagCriteriaSelectionParameters.RequiresStandardIds.Add(int.Parse(drTagValue));
                        break;
                    case (int)Enums.LrmiTags.TextComplexity:
                        ctlTags.TagCriteriaSelectionParameters.TextComplexity = drTagValue;
                        break;
                    case (int)Enums.LrmiTags.ReadingLevel:  
                        ctlTags.TagCriteriaSelectionParameters.ReadingLevel = drTagValue;
                        break;
                    case (int)Enums.LrmiTags.EducationalSubject:
                        ctlTags.TagCriteriaSelectionParameters.EducationalSubject.Add(drTagValue);
                        break;
                    case (int)Enums.LrmiTags.EducationalLevel:
                        ctlTags.TagCriteriaSelectionParameters.GradeLevel.Add(drTagValue);
                        break;
                    case (int)Enums.LrmiTags.UsageRights:
                        ctlTags.TagCriteriaSelectionParameters.UseRightUrl = int.Parse(drTagValue);
                        break;
                    case (int)Enums.LrmiTags.UsageRightsUrl:
                        ctlTags.TagCriteriaSelectionParameters.UseRightUrlTxt = drTagValue;
                        break;
                    case (int)Enums.LrmiTags.Original3RdParty:
                        ctlTags.TagCriteriaSelectionParameters.OriginalThirdPartyUrl = drTagValue;
                        break;
                    case (int)Enums.LrmiTags.Creator:
                        ctlTags.TagCriteriaSelectionParameters.Creator = drTagValue;
                        break;
                    case (int)Enums.LrmiTags.DateCreated:
                        ctlTags.TagCriteriaSelectionParameters.DateCreated = Convert.ToDateTime(drTagValue);
                        break;
                    case (int)Enums.LrmiTags.Publisher:
                        ctlTags.TagCriteriaSelectionParameters.Publisher = drTagValue;
                        break;
                    case (int)Enums.LrmiTags.Language:
                        ctlTags.TagCriteriaSelectionParameters.Language = int.Parse(drTagValue);
                        break;
                    default:
                    {
                        break;
                    }
                }
            }
            if (!ctlTags.TagCriteriaSelectionParameters.Creator.IsNotNullOrWhiteSpace())
            {
                ctlTags.TagCriteriaSelectionParameters.Creator = (_sessionObject.LoggedInUser).UserName;
            }
        }
        #region GetData Handler Methods
        /// <summary>
        /// Get table data from database using inline SQL
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        private DataTable GetDataTable(string connectionString, string selectSql)
        {
            DataTable dataTable = new DataTable();

            if (selectSql != null)
            {
                string connectionStringToUse = connectionString ?? _rootConnectionString;

                SqlConnection sqlConnection = new SqlConnection(connectionStringToUse);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter { SelectCommand = new SqlCommand(selectSql, sqlConnection) };

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
        /// <summary>
        /// Get Local Database Connection string from object
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        private static string GetLocalDbConnectionString(string connString)
        {
            return connString;
        }
        #endregion
    }
}