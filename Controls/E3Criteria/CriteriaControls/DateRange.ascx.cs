using System;
using Telerik.Web.UI.Calendar;
using Thinkgate.Classes.Search;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class DateRange : Criterion
    {
        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                CriteriaHeaderText.Text = Header+":";
                
                if (IsRequired)
                {
                    RequiredCriteriaIndicator.Text = "*";
                    RequiredCriteriaIndicator.Style.Add("color", "red");
                    RequiredCriteriaIndicator.Style.Add("font-weight", "bold");
                }
            }
        }

        #endregion

    }
}