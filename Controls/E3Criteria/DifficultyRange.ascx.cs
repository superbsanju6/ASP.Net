using System;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class DifficultyRange : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValueDisplayTemplateName = "DifficultyCriteriaValueDisplayTemplate";
                
                rntbStart.ClientEvents.OnValueChanged = CriteriaName + "Controller.OnChange";
                rntbStart.MinValue = 0.20;
                rntbStart.MaxValue = 0.90;
                rntbStart.MaxLength = 3;
                
                rntbEnd.ClientEvents.OnValueChanged = CriteriaName + "Controller.OnChange";
                rntbEnd.MinValue = 0.20;
                rntbEnd.MaxValue = 0.90;
                rntbEnd.MaxLength = 3;
                
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

      
        /// <summary>
        /// Simple name/value object to store the two values we have into the CriteriaSet
        /// </summary>
        [Serializable]
        public class ValueObject
        {
            public string Difficulty { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}