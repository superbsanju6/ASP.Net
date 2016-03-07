using System;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class TextWithDropdown : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValueDisplayTemplateName = "TextWithDropdownCriteriaValueDisplayTemplate";
                if (!String.IsNullOrEmpty(DataTextField)) RadComboBox1.DataTextField = DataTextField;
                
                RadComboBox1.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                RadTextBox1.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";
                RadTextBox1.ClientEvents.OnKeyPress = CriteriaName + "Controller.OnKeyPress";
                
                RadComboBox1.DataSource = DataSource;
                RadComboBox1.DataBind();
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // dropdown list can only have 1 value
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

      
        public class ValueObject
        {
            public string Text { get; set; }
            public string Option { get; set; }
        }
    }
}