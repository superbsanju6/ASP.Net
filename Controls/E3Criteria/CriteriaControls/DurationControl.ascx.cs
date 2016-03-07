using System;
using System.Globalization;
using System.Text;
using System.Web.UI;
using Standpoint.Core.ExtensionMethods;
using Thinkgate.Classes.Search;
using Thinkgate.Enums.Search;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class DurationControl : Criterion
    {
        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                for (int i = 0; i < 24; i++)
                    DurationHours.Items.Add(i.ToString(CultureInfo.InvariantCulture));
                for (int i = 0; i < 60; i += 5)
                    DurationMinutes.Items.Add(i.ToString(CultureInfo.InvariantCulture));

                rtbDuration.Text = string.Empty;
                DurationMinutes.Text = "0";
                DurationHours.Text = "0";
                CriteriaHeaderText.Text = Header;
                
                ClientScriptManager clientScript = Page.ClientScript;
                StringBuilder javaScriptBuilder = new StringBuilder();
                
                javaScriptBuilder.Append(" $('#" + rtbDuration.ClientID + "').inputmask('integer', {allowMinus: false, allowPlus: false});");
                                             
                clientScript.RegisterStartupScript(GetType(), rtbDuration.ClientID,
                            javaScriptBuilder.ToString(), true);
                
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