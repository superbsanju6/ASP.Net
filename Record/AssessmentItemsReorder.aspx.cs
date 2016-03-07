using System;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Web.Script.Serialization;
using System.Web.UI;
using Thinkgate.Base.Enums;
using System.Linq;

namespace Thinkgate.Record
{
    public partial class AssessmentItemsReorder : RecordPage
    {
        private int _assessmentID;
        private Assessment _selectedAssessment;
        public string ImageWebFolder;
        private JavaScriptSerializer _serializer;
        
        #region Check Secure
        protected Dictionary<string, bool> dictionaryItem;
        bool isSecuredFlag;
        private Int32 AssessmentID = 0;
        public Thinkgate.Base.Classes.Assessment selectedAssessment;
        bool SecureTitle = false;
        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(selectedAssessment.TestType);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _assessmentID = GetDecryptedEntityId(X_ID);
            }

            ScriptManager.RegisterStartupScript(this, typeof(string), "encryptedUserID", "var FieldTestFormId=" + AssessmentForm.FieldTestFormId + ";" +
                                                                                            " var UpdatedDataAttached=" + (int)ActionResult.GenericStatus.UpdatedDataAttached + ";"
                                                                                            , true);

            ImageWebFolder = (Request.ApplicationPath.Equals("/") ? "" : Request.ApplicationPath) + "/Images/";
            _selectedAssessment = Assessment.GetAssessmentAndQuestionsByID(_assessmentID);
            _selectedAssessment.LoadForms(true);
            if (_selectedAssessment == null)
            {
                return;
                //_selectedAssessment = Base.Classes.Assessment.GetAssessmentAndQuestionsByID(_assessmentID);
                //Cache.Insert(key, _selectedAssessment, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(2));
            }

            if (!IsPostBack)
            {
                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);

                if (_assessmentID != 0)
                {
                    selectedAssessment = Thinkgate.Base.Classes.Assessment.GetAssessmentByID(_assessmentID);
                    dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(selectedAssessment.TestCategory);
                    isSecuredFlag = dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                    SecureTitle = hasPermission && isSecuredFlag && SecureType;
                        
                }
             
                BindAssessmentItems(_selectedAssessment.Items);
                BindFormData();
                BindRadtab_Forms();
            }

        }

        public void BindAssessmentItems(List<TestQuestion> items)
        {
            TestQuestion.LoadStandardsForTestQuestions(items);
            rep_assessmentItems.DataSource = items;
            rep_assessmentItems.DataBind();
        }

        private void BindFormData()
        {
            if (_serializer == null) _serializer = new JavaScriptSerializer();
            ScriptManager.RegisterStartupScript(this, typeof(string), "AssessmentForms", "var AssessmentForms=" + _serializer.Serialize(_selectedAssessment.PrepareFormsForJson()) + ";", true);

        }
        protected void BindRadtab_Forms()
        {
            string activeForm = Request.QueryString["activeForm"].ToString();
            if (_selectedAssessment.IncludeFieldTest)
            {
                _selectedAssessment.Forms.Add(new AssessmentForm(AssessmentForm.FieldTestFormId));
            }
            if (_selectedAssessment.Forms.Count > 5)
            {
                radtab_Forms.Width = 277;
                radtab_Forms.ScrollChildren = true;
            }
            radtab_Forms.DataSource = _selectedAssessment.Forms;
            radtab_Forms.DataBind();
            if (!String.IsNullOrEmpty(activeForm))
            {
                Telerik.Web.UI.RadTab tab = radtab_Forms.FindTabByValue(activeForm);
                tab.Selected = true;
                ScriptManager.RegisterStartupScript(this, typeof(string), "activeForm", "renderForm(" + activeForm + ");", true);
            }
            

        }

        protected string ConditionalAddendum(Object dataItem)
        {
            var item = (Thinkgate.Base.Classes.TestQuestion) dataItem;
            if (item.Addendum != null)
            {
                return item.Addendum.ID.ToString();
            }
            else
            {
                return "";
            }
        }

    }



}