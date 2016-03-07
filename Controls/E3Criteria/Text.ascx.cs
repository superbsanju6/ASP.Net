using System;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class Text : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RadTextBox1.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // can only have 1 value
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        /// <summary>
        /// This methods clears out the selected Text Box for the control
        /// </summary>
        public void Clear()
        {
            RadTextBox1.Text = string.Empty;
        }
     
        /// <summary>
        /// Simple name/value object to store the two values we have into the CriteriaSet
        /// </summary>
        [Serializable]
        public class ValueObject
        {
            public string Text { get; set; }
        }
    }
}