using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class StandardGradeSubjectId : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public object StandardSetDataSource { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValueDisplayTemplateName = CriteriaName;
                ddlCriteriaStandardSet.Attributes.Add("onchange", CriteriaName + "Controller.OnChange()");
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // can only have 1 value

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
        }
    }
}