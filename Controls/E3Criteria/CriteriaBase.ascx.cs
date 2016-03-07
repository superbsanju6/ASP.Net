using Telerik.Web.UI;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.E3Criteria
{
    /// <summary>
    /// This is the parent class for all custom criteria objects. Common functions should go here.
    /// </summary>
    public partial class CriteriaBase : System.Web.UI.UserControl
    {
        public string Text;                                     // This is the actual text that will appear on the Criteria node
        public string CriteriaName;                             // This name is used in code to reference the target criteria object
        public string ValueDisplayTemplateName = "DefaultCriteriaValueDisplayTemplate";                 // this is for overriding the JS render template that will be used to render the selected criteria
        public string OnChange;
        public RestrictValueOptions RestrictValueCount;
        private CriteriaHeader _criteriaHeader;                 // a reference to the CriteriaHeader(visible node) for this specific criteria
        public Unit Width;
        public bool Required;
        public bool ReadOnly;

        /// <summary>
        /// Configures the criteria header with values it needs from the containing control
        /// </summary>
        /// <param name="criteriaHeader">The criteria header control.</param>
        /// <param name="radToolTip1">The tool tip being setup for this criteria control.</param>
        public void ConfigureCriteriaHeader(CriteriaHeader criteriaHeader, RadToolTip radToolTip1)
        {
            _criteriaHeader = criteriaHeader;
            criteriaHeader.Text = Text;
            criteriaHeader.CriteriaName = CriteriaName;
            criteriaHeader.ToolTip = radToolTip1;
            criteriaHeader.ValueDisplayTemplateName = ValueDisplayTemplateName;
            criteriaHeader.Required = Required;
            var config = new CriteriaConfig()
                             {
                                 //RestrictToSingleValue = RestrictToSingleValue,
                                 RestrictValueCount = RestrictValueCount,
                                 ValueDisplayTemplateName = ValueDisplayTemplateName,
                                 ReadOnly = ReadOnly
            
                             };
            ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "_Config", "var " + CriteriaName + "_Config = " + new JavaScriptSerializer().Serialize(config) + ";", true);
        }

        public class CriteriaConfig
        {
            public RestrictValueOptions RestrictValueCount;
            public string ValueDisplayTemplateName;
            public bool ReadOnly;
        }
       
        public enum RestrictValueOptions
        {
            Unlimited = 0,
            OnlyOneAppliedAtATime = 1,
            OnlyOne_Period = 2
        }
    }
}