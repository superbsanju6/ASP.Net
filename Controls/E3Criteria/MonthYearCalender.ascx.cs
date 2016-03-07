using System;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class MonthYearCalender : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public string EmptyMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValueDisplayTemplateName = CriteriaName;

                rdPickerStart.ClientEvents.OnDateSelected = CriteriaName + "Controller.OnChange";
                rdPickerStart.DateInput.EmptyMessage = EmptyMessage;
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public RadMonthYearPicker RadMonthYear
        {
            get
            {
                return rdPickerStart;
            }
        }
      
        /// <summary>
        /// Simple name/value object to store the two values we have into the CriteriaSet
        /// </summary>
        [Serializable]
        public class ValueObject
        {
            public string Date { get; set; }
            public string Type { get; set; }
        }
    }
}