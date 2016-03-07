using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;    

namespace Thinkgate.Controls.E3Criteria.AutoCompleteCriteriaControls
{
    public partial class GetGradeSubjectCourseStandard : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public string DataValueField;
        public object StandardSetDataSource { get; set; }
        public string OnClientChange { get; set; }
        public string OnRemoveByKeyHandler { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValueDisplayTemplateName = CriteriaName;              
                ddlCriteriaStandardSet.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                ddlCriteriaGrades.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                ddlCriteriaSubjects.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                ddlCriteriaCourse.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                ddlCriteriaStandards.OnClientItemChecked = CriteriaName + "Controller.OnChange";
                ddlCriteriaStandards.Attributes.Add("CriteriaName", CriteriaName);
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // can only have 1 value

                /*This Block is used to bind the css that is used to reset dropdown*/
                ddlCriteriaStandardSet.CssClass = CriteriaName + "Finder";
                ddlCriteriaGrades.CssClass = CriteriaName + "Finder";
                ddlCriteriaSubjects.CssClass = CriteriaName + "Finder";
                ddlCriteriaCourse.CssClass = CriteriaName + "Finder";
                ddlCriteriaStandards.CssClass = CriteriaName + "Finder";

                ddlCriteriaStandardSet.DataSource = StandardSetDataSource;
                ddlCriteriaStandardSet.DataTextField = "Value";
                ddlCriteriaStandardSet.DataValueField = "Key";
                ddlCriteriaStandardSet.DataBind();

            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }


        /// <summary>
        /// Simple name/value object to store the two values we have into the CriteriaSet
        /// </summary>
        [Serializable]
        public class ValueObject
        {
            public string StandardSet { get; set; }
            public string Grades { get; set; }
            public string Subjects { get; set; }
            public string Courses { get; set; }
            public string Standard { get; set; }
            public string StandardId { get; set; }
            public string StandardName { get; set; }                    
        }
    }
}