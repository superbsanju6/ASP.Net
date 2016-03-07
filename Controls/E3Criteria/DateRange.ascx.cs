using System;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class DateRange : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;

        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValueDisplayTemplateName = CriteriaName;

                #region
                //RadTextBox1.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";
                //RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // can only have 1 value
                //rdPicker1.ClientEvents.OnDateSelected = "DateSelected";
                #endregion
                rdPickerStart.ClientEvents.OnDateSelected = CriteriaName + "Controller.OnChange";
                rdPickerStart.Calendar.SpecialDays.Add(new RadCalendarDay { Repeatable = Telerik.Web.UI.Calendar.RecurringEvents.Today });
                rdPickerStart.DateInput.EmptyMessage = "Start...";
                rdPickerStart.Calendar.SelectedDate = DateTime.Today;

                rdPickerEnd.ClientEvents.OnDateSelected = CriteriaName + "Controller.OnChange";
                rdPickerEnd.Calendar.SpecialDays.Add(new RadCalendarDay { Repeatable = Telerik.Web.UI.Calendar.RecurringEvents.Today });
                rdPickerEnd.DateInput.EmptyMessage = "End...";
                rdPickerStart.Calendar.SelectedDate = DateTime.Today;
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
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